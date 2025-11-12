# Part 7: 프로덕션 준비 - 실시간 통신과 클라이언트 통합

## 이론에서 실전으로: 프로덕션 환경의 도전과 해결책

Part 6에서 훌륭한 API를 만드는 방법을 배웠습니다. RESTful 원칙, 강력한 보안, GraphQL의 유연성, SignalR의 실시간성—이 모든 것은 개발 환경에서 완벽하게 작동합니다. 하지만 실제 프로덕션 환경은 개발 환경과 다릅니다. 수천 명의 동시 사용자, 예측 불가능한 네트워크 장애, 긴 실행 시간의 작업, 타사 서비스와의 통합, 그리고 클라이언트와 서버 간의 타입 불일치 문제... Part 7은 이러한 현실적인 도전들을 해결하는 방법을 다룹니다.

프론트엔드 개발자로서 여러분은 이미 이런 문제들을 클라이언트 측에서 경험했을 것입니다. API 호출이 실패했을 때의 재시도 로직, 네트워크가 불안정할 때의 재연결, 긴 작업의 진행 상태 표시, TypeScript 타입과 실제 API 응답의 불일치... 이제 서버 측에서 이러한 문제를 근본적으로 해결하는 방법을 배웁니다. 클라이언트의 고통을 아는 개발자만이 진정으로 견고한 시스템을 만들 수 있습니다.

### 실시간 통신의 복잡성: 단순한 요청-응답을 넘어서

전통적인 HTTP는 요청-응답 모델입니다. 클라이언트가 요청하면 서버가 응답하고, 연결이 끊어집니다. 이는 대부분의 경우 완벽히 작동하지만, 실시간 애플리케이션에서는 한계가 있습니다. 새로운 메시지가 도착했는지 어떻게 알 수 있을까요? 서버에서 클라이언트로 능동적으로 데이터를 보낼 수 있을까요?

폴링(Polling)은 가장 단순한 해결책입니다. 클라이언트가 주기적으로(예: 3초마다) 서버에 "새 데이터 있어?"라고 물어봅니다. 구현은 쉽지만, 비효율적입니다. 대부분의 요청은 "없어"라는 응답만 받으며, 서버와 네트워크에 불필요한 부하를 줍니다. 게다가 실시간성도 떨어집니다. 최악의 경우 3초의 지연이 발생할 수 있습니다.

롱 폴링(Long Polling)은 이를 개선합니다. 클라이언트가 요청을 보내면, 서버는 새 데이터가 있을 때까지 응답을 보류합니다. 새 데이터가 도착하면 즉시 응답하고, 클라이언트는 다시 요청을 보냅니다. 실시간성은 향상되지만, 여전히 각 업데이트마다 HTTP 요청-응답 사이클을 거쳐야 합니다.

Server-Sent Events (SSE)는 더 나은 접근입니다. HTTP 연결을 계속 열어두고, 서버가 일방적으로 데이터를 스트리밍합니다. 실시간 알림, 주식 가격 업데이트, 뉴스 피드에 완벽합니다. 단점은 단방향이라는 것입니다. 서버에서 클라이언트로만 데이터를 보낼 수 있으며, 클라이언트가 서버로 메시지를 보내려면 별도의 HTTP 요청이 필요합니다.

WebSocket은 완전한 양방향 통신을 제공합니다. 핸드셰이크 후 연결이 유지되며, 양쪽 모두 언제든지 메시지를 보낼 수 있습니다. 채팅, 실시간 게임, 협업 편집기에 이상적입니다. 하지만 직접 구현하기에는 복잡합니다. 재연결 로직, 하트비트, 메시지 큐, 에러 처리... 모든 것을 직접 만들어야 합니다.

ASP.NET Core는 이 모든 것을 우아하게 지원합니다. SignalR은 WebSocket의 복잡성을 숨기며, 불가능할 때 자동으로 폴백합니다. 내장된 Server-Sent Events 지원으로 단방향 스트리밍도 간단합니다. 그리고 WebSocket을 직접 사용해야 한다면, `System.Net.WebSockets` 네임스페이스가 저수준 제어를 제공합니다.

### 백그라운드 작업: 사용자를 기다리게 하지 마라

웹 애플리케이션에서 일부 작업은 즉시 완료되지 않습니다. 이미지 리사이징, 비디오 인코딩, 복잡한 리포트 생성, 대량의 이메일 발송, 외부 API 호출... 이런 작업을 HTTP 요청 내에서 처리하면 어떻게 될까요? 사용자는 몇 초, 심지어 몇 분을 기다려야 하며, 브라우저는 타임아웃될 수 있습니다. 게다가 서버 리소스가 묶여, 다른 요청을 처리할 수 없게 됩니다.

Node.js 개발자라면 이 문제를 Worker Threads나 Bull(Redis 기반 작업 큐)로 해결했을 것입니다. ASP.NET Core도 유사한 패턴을 제공하지만, 더 강력한 기능을 가지고 있습니다.

**IHostedService: 백그라운드 서비스의 기초**

`IHostedService`는 애플리케이션 수명 주기와 함께 실행되는 백그라운드 작업을 정의합니다. 애플리케이션이 시작될 때 `StartAsync`가 호출되고, 종료될 때 `StopAsync`가 호출됩니다. 이를 통해 지속적으로 실행되는 작업(예: 5분마다 데이터 동기화)을 구현할 수 있습니다.

```csharp
public class DataSyncService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // 데이터 동기화 로직
            await SyncDataAsync();

            // 5분 대기
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

이는 Node.js의 setInterval과 유사하지만, 더 견고합니다. 정상 종료(graceful shutdown)를 지원하며, 의존성 주입을 통해 서비스를 사용할 수 있고, 예외 처리가 내장되어 있습니다.

**IBackgroundTaskQueue: 작업 큐 패턴**

사용자가 트리거하는 긴 작업은 어떻게 처리할까요? "이미지 업로드 → 리사이징 → 썸네일 생성"같은 시나리오를 생각해보세요. 사용자는 업로드만 완료되면 바로 응답을 받아야 하며, 나머지 작업은 백그라운드에서 처리되어야 합니다.

작업 큐 패턴이 이를 해결합니다. HTTP 요청 핸들러는 작업을 큐에 추가하고 즉시 응답합니다. 백그라운드 서비스는 큐에서 작업을 꺼내 처리합니다.

```csharp
// 작업을 큐에 추가
public class ImageController : ControllerBase
{
    private readonly IBackgroundTaskQueue _taskQueue;

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        // 파일 저장
        var path = await SaveFileAsync(file);

        // 백그라운드 작업 큐에 추가
        _taskQueue.QueueBackgroundWorkItem(async token =>
        {
            await ResizeImageAsync(path);
            await GenerateThumbnailAsync(path);
        });

        // 즉시 응답
        return Ok(new { message = "업로드 완료, 처리 중..." });
    }
}
```

이는 Bull이나 BullMQ의 개념과 동일하지만, 외부 의존성(Redis) 없이 작동합니다. 메모리 기반 큐는 간단한 시나리오에 충분하며, 분산 환경에서는 Azure Queue Storage, AWS SQS, RabbitMQ 같은 영속적 큐로 대체할 수 있습니다.

**Hangfire: 스케줄링과 재시도의 강력함**

더 복잡한 시나리오도 있습니다. "매일 자정에 리포트 생성", "실패한 작업을 자동으로 재시도", "작업 진행 상태를 UI에 표시"... 이런 요구사항을 직접 구현하는 것은 어렵습니다.

Hangfire는 이 모든 것을 제공하는 라이브러리입니다. 작업을 데이터베이스에 영속화하며, 재시도, 스케줄링, 작업 체인, 진행 상태 추적을 내장하고 있습니다. 게다가 대시보드 UI도 제공하여, 작업 상태를 실시간으로 모니터링할 수 있습니다.

```csharp
// 즉시 실행
BackgroundJob.Enqueue(() => Console.WriteLine("즉시 실행!"));

// 지연 실행
BackgroundJob.Schedule(() => SendEmailAsync(userId), TimeSpan.FromMinutes(30));

// 반복 실행 (cron 표현식)
RecurringJob.AddOrUpdate("daily-report", () => GenerateReportAsync(), Cron.Daily);

// 작업 체인
var jobId = BackgroundJob.Enqueue(() => ProcessImageAsync(path));
BackgroundJob.ContinueJobWith(jobId, () => NotifyUserAsync(userId));
```

이는 Node.js의 node-cron이나 Agenda보다 훨씬 강력합니다. 작업이 실패하면 자동으로 재시도하며(지수 백오프 포함), 서버가 재시작되어도 작업이 유실되지 않습니다. 분산 환경에서도 작동하며, 여러 서버가 같은 큐를 공유할 수 있습니다.

### 타입 안전한 API 클라이언트: 컴파일 타임 검증의 힘

프론트엔드 개발자로서 이런 경험이 있을 것입니다. 백엔드 API가 변경되었는데, 프론트엔드는 그대로 둔 채 배포했습니다. 프로덕션에서 `undefined`를 읽다가 에러가 발생합니다. 사용자들이 불평하고, 긴급 롤백을 합니다. 타입 안전성이 있었다면 방지할 수 있었던 문제입니다.

TypeScript는 이 문제를 부분적으로 해결합니다. API 응답의 타입을 정의할 수 있습니다:

```typescript
interface User {
  id: number;
  name: string;
  email: string;
}

const response = await fetch('/api/users/1');
const user: User = await response.json();
```

하지만 이는 **런타임에 검증되지 않습니다**. 서버가 실제로 `{ id, name, email }`을 반환한다는 보장이 없습니다. `email` 필드가 제거되었다면? TypeScript는 컴파일 타임에 문제를 발견하지 못하고, 런타임에 `user.email`이 `undefined`가 됩니다.

진정한 타입 안전성은 **서버의 스키마에서 클라이언트 타입을 자동 생성**하는 것입니다. 서버의 API 정의가 변경되면, 클라이언트 코드가 컴파일 에러를 발생시켜야 합니다. 이것이 타입 안전한 API 클라이언트의 본질입니다.

**OpenAPI에서 TypeScript 클라이언트 생성: NSwag의 마법**

ASP.NET Core API는 OpenAPI(Swagger) 명세를 자동으로 생성합니다. 이 명세는 모든 엔드포인트, 요청/응답 타입, HTTP 메서드, 상태 코드를 정확히 기술합니다. NSwag는 이 명세를 읽어 TypeScript 클라이언트 코드를 생성합니다.

```bash
# OpenAPI 명세에서 TypeScript 클라이언트 생성
nswag openapi2tsclient /input:http://localhost:5000/swagger/v1/swagger.json /output:src/api/client.ts
```

생성된 클라이언트는 완전히 타입 안전합니다:

```typescript
// 자동 생성된 클라이언트
const client = new UserClient('http://localhost:5000');
const user = await client.getUser(1); // user는 User 타입
console.log(user.name); // 자동 완성!

// API가 변경되면 컴파일 에러
console.log(user.phoneNumber); // 에러: Property 'phoneNumber' does not exist
```

서버의 `User` 모델에서 `email` 필드가 제거되면, 생성된 TypeScript 타입에도 반영됩니다. 프론트엔드 코드에서 `user.email`을 사용하는 곳은 모두 컴파일 에러가 발생하며, 배포 전에 발견할 수 있습니다.

**Kiota: Microsoft의 차세대 API 클라이언트 생성기**

NSwag는 훌륭하지만, OpenAPI에만 의존합니다. Kiota는 Microsoft의 새로운 도구로, OpenAPI뿐만 아니라 다른 API 명세도 지원할 예정입니다. 생성된 클라이언트는 더 현대적이며, fluent API를 사용하여 읽기 쉽습니다.

```typescript
// Kiota로 생성된 클라이언트
const user = await client.users.byUserId(1).get();
const posts = await client.users.byUserId(1).posts.get();
```

체이닝 방식은 REST API의 계층 구조를 자연스럽게 표현하며, IDE의 자동 완성이 API 탐색을 돕습니다.

### 캐싱: 성능의 저비용 고효율 전략

가장 빠른 API 호출은 하지 않는 것입니다. 데이터가 자주 변경되지 않는다면, 매번 데이터베이스를 조회할 필요가 없습니다. 캐싱은 성능을 극적으로 향상시키며, 서버 부하를 줄이고, 사용자 경험을 개선합니다.

캐싱 전략은 여러 계층에서 적용할 수 있습니다:

**1. 브라우저 캐싱: HTTP 헤더의 힘**

브라우저는 HTTP 캐시 헤더를 존중합니다. `Cache-Control`, `ETag`, `Last-Modified`를 적절히 설정하면, 브라우저는 서버에 요청조차 보내지 않고 캐시된 응답을 사용합니다.

```csharp
[HttpGet("{id}")]
[ResponseCache(Duration = 3600)] // 1시간 캐싱
public IActionResult GetUser(int id)
{
    var user = _context.Users.Find(id);
    return Ok(user);
}
```

정적 자산(이미지, CSS, JS)은 더 긴 시간 캐싱할 수 있습니다. .NET 9의 `MapStaticAssets`는 파일 내용 기반 해시를 URL에 추가하여, 파일이 변경되면 URL도 변경되게 합니다. 따라서 무한정 캐싱해도 안전합니다.

**2. 서버 사이드 메모리 캐싱: IMemoryCache**

서버 메모리에 데이터를 캐싱하면, 데이터베이스 쿼리를 건너뛸 수 있습니다. `IMemoryCache`는 ASP.NET Core의 내장 메모리 캐시입니다.

```csharp
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly DbContext _context;

    public async Task<Product> GetProductAsync(int id)
    {
        var cacheKey = $"product_{id}";

        if (_cache.TryGetValue(cacheKey, out Product product))
        {
            return product; // 캐시 히트
        }

        product = await _context.Products.FindAsync(id);

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        _cache.Set(cacheKey, product, cacheOptions);

        return product;
    }
}
```

메모리 캐시는 빠르지만, 서버 인스턴스에 국한됩니다. 로드 밸런서 뒤에 여러 서버가 있다면, 각 서버가 자신의 캐시를 가지므로 일관성 문제가 발생할 수 있습니다.

**3. 분산 캐싱: IDistributedCache**

분산 캐시는 모든 서버가 공유하는 캐시입니다. Redis, SQL Server, Azure Cosmos DB가 백엔드로 사용될 수 있습니다.

```csharp
public class ProductService
{
    private readonly IDistributedCache _cache;

    public async Task<Product> GetProductAsync(int id)
    {
        var cacheKey = $"product_{id}";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (cached != null)
        {
            return JsonSerializer.Deserialize<Product>(cached);
        }

        var product = await _context.Products.FindAsync(id);

        await _cache.SetStringAsync(cacheKey,
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return product;
    }
}
```

분산 캐시는 일관성을 보장하지만, 네트워크 왕복이 필요하므로 메모리 캐시보다 느립니다.

**4. HybridCache (.NET 9): 두 세계의 장점**

.NET 9의 `HybridCache`는 두 접근을 결합합니다. L1(메모리 캐시) + L2(분산 캐시) 구조로, 먼저 메모리에서 찾고, 없으면 분산 캐시에서 찾습니다. 게다가 "stampede" 문제(여러 요청이 동시에 캐시 미스를 경험하여 데이터베이스에 동일한 쿼리를 여러 번 보내는 현상)를 자동으로 방지합니다.

```csharp
public class ProductService
{
    private readonly HybridCache _cache;

    public async Task<Product> GetProductAsync(int id)
    {
        return await _cache.GetOrCreateAsync(
            $"product_{id}",
            async cancel => await _context.Products.FindAsync(id),
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(2)
            });
    }
}
```

이는 개발자 경험을 크게 향상시킵니다. 복잡한 캐싱 로직을 직접 작성할 필요 없이, 선언적으로 캐싱을 적용할 수 있습니다.

### 메시지 큐: 느슨한 결합과 확장성

마이크로서비스 아키텍처에서 서비스 간 통신은 어떻게 해야 할까요? 직접 HTTP 호출은 간단하지만, 강한 결합을 만들어냅니다. 서비스 A가 서비스 B를 호출하려면, B가 실행 중이어야 하고, 네트워크가 연결되어 있어야 하며, B의 URL을 알아야 합니다. B가 다운되면 A도 영향을 받습니다.

메시지 큐는 이를 해결합니다. 서비스 A는 메시지를 큐에 발행(publish)하고, 서비스 B는 큐에서 메시지를 구독(subscribe)합니다. 둘은 서로를 알 필요가 없으며, 시간적으로도 분리됩니다. B가 잠시 다운되어도, 메시지는 큐에 남아 있다가 B가 복구되면 처리됩니다.

**RabbitMQ: 엔터프라이즈급 메시지 브로커**

RabbitMQ는 가장 인기 있는 메시지 큐 중 하나입니다. AMQP(Advanced Message Queuing Protocol)를 구현하며, 복잡한 라우팅, 메시지 지속성, 확인(acknowledgment) 메커니즘을 제공합니다.

```csharp
// 메시지 발행
public class OrderService
{
    public async Task CreateOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        // RabbitMQ에 "주문 생성됨" 이벤트 발행
        _messageBus.Publish("order.created", new OrderCreatedEvent
        {
            OrderId = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount
        });
    }
}

// 다른 서비스에서 메시지 구독
public class EmailService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageBus.Subscribe<OrderCreatedEvent>("order.created", async evt =>
        {
            await SendOrderConfirmationEmailAsync(evt.OrderId);
        });

        return Task.CompletedTask;
    }
}
```

이 패턴은 Node.js의 EventEmitter와 유사하지만, 분산 환경에서 작동합니다. 서로 다른 서버, 심지어 다른 언어로 작성된 서비스들이 메시지를 교환할 수 있습니다.

**Azure Service Bus / AWS SQS: 클라우드 네이티브 큐**

자체 RabbitMQ 서버를 운영하기 부담스럽다면, 관리형 서비스를 사용할 수 있습니다. Azure Service Bus, AWS SQS, Google Cloud Pub/Sub는 모두 메시지 큐를 서비스로 제공합니다.

ASP.NET Core는 이들을 쉽게 통합할 수 있는 라이브러리를 제공하며, MassTransit 같은 추상화 라이브러리를 사용하면 메시지 브로커를 쉽게 교체할 수 있습니다.

### Part 7에서 배울 내용

Part 7은 프로덕션 환경의 현실적인 문제들을 다룹니다. 실시간 통신의 다양한 전략, 백그라운드 작업 처리, 타입 안전한 API 클라이언트, 캐싱 전략, 메시지 큐... 이 모든 것은 독립적으로도 가치 있지만, 함께 사용될 때 견고하고 확장 가능한 시스템을 만듭니다.

**Chapter 17: 실시간 통신과 백그라운드 처리**

실시간 웹의 모든 패턴을 배웁니다. Server-Sent Events로 실시간 알림을 구현하고, WebSocket으로 양방향 통신을 만들며, SignalR로 복잡성을 숨기고 견고성을 얻습니다.

백그라운드 작업의 전체 스펙트럼을 다룹니다. `IHostedService`로 지속 실행 서비스를 만들고, 작업 큐 패턴으로 사용자 요청을 빠르게 처리하며, Hangfire로 복잡한 스케줄링과 재시도 로직을 구현합니다. 메시지 큐(RabbitMQ, Azure Service Bus)로 마이크로서비스 간 통신을 느슨하게 결합합니다.

실습에서는 실시간 대시보드를 만들며, 백그라운드에서 데이터를 처리하고 SignalR로 클라이언트에 실시간 업데이트를 보냅니다. 긴 작업(비디오 인코딩 시뮬레이션)은 Hangfire로 처리하며, 진행 상태를 UI에 표시합니다.

**Chapter 18: API 클라이언트 패턴**

타입 안전성의 끝판왕을 경험합니다. NSwag로 OpenAPI 명세에서 TypeScript 클라이언트를 자동 생성하며, 서버 타입 변경이 즉시 프론트엔드 컴파일 에러로 이어지는 것을 확인합니다.

React Query, Redux Toolkit의 RTK Query와 생성된 클라이언트를 통합하여, 선언적 데이터 페칭과 타입 안전성을 동시에 얻습니다. Optimistic UI 업데이트, 에러 처리, 재시도 로직, 캐싱 정책을 구현합니다.

HybridCache (.NET 9)를 사용하여 서버 측 캐싱을 적용하고, 클라이언트 측 캐싱(React Query)과 조합하여 최적의 성능을 얻습니다. HTTP 캐시 헤더를 적절히 설정하여 브라우저 캐싱도 활용합니다.

실습에서는 전체 스택(ASP.NET Core API + 생성된 TypeScript 클라이언트 + React Query)을 통합하며, 서버 타입을 변경했을 때 프론트엔드가 자동으로 업데이트되거나 컴파일 에러를 발생시키는 것을 경험합니다.

## 학습 목표

Part 7을 마치면 다음을 할 수 있습니다:

- 실시간 통신의 여러 패턴(Polling, Long Polling, SSE, WebSocket)을 이해하고 적절히 선택할 수 있습니다
- Server-Sent Events로 서버에서 클라이언트로 실시간 데이터를 스트리밍합니다
- WebSocket을 직접 사용하여 양방향 통신을 구현할 수 있습니다
- SignalR로 복잡한 실시간 기능을 간단하게 만듭니다
- `IHostedService`와 `BackgroundService`로 백그라운드 작업을 구현합니다
- 작업 큐 패턴으로 긴 작업을 비동기로 처리합니다
- Hangfire로 스케줄링, 재시도, 작업 체인을 구현합니다
- RabbitMQ나 Azure Service Bus로 메시지 기반 아키텍처를 만듭니다
- NSwag나 Kiota로 타입 안전한 API 클라이언트를 생성합니다
- React Query나 RTK Query와 생성된 클라이언트를 통합합니다
- 여러 계층의 캐싱 전략(브라우저, 메모리, 분산, 하이브리드)을 적용합니다
- Optimistic UI 업데이트와 에러 처리를 구현합니다

## 챕터 구성

### Chapter 17: 실시간 통신과 백그라운드 처리

프로덕션 환경의 복잡한 요구사항을 해결하는 고급 패턴을 배웁니다.

**실시간 통신:**
- 실시간 통신의 진화: Polling → Long Polling → SSE → WebSocket
- Server-Sent Events (SSE): 단방향 스트리밍
  - ASP.NET Core에서 SSE 구현
  - 실시간 알림, 주식 가격, 로그 스트리밍
- WebSocket: 양방향 실시간 통신
  - `System.Net.WebSockets` 네임스페이스
  - 연결 관리, 메시지 송수신, 에러 처리
  - 재연결 로직과 하트비트
- SignalR 고급 패턴
  - Strongly-typed Hub
  - 그룹 관리와 동적 그룹
  - Redis backplane으로 확장성 확보
  - Azure SignalR Service 통합
- 실시간 통신 선택 가이드: 언제 무엇을 사용할까?

**백그라운드 처리:**
- `IHostedService`와 `BackgroundService`
  - 지속 실행 서비스 구현
  - 정상 종료(graceful shutdown)
  - 의존성 주입과 스코프 관리
- 작업 큐 패턴: `IBackgroundTaskQueue`
  - 메모리 기반 큐 구현
  - 사용자 요청을 빠르게 응답하고 백그라운드로 처리
- Hangfire: 강력한 백그라운드 작업 라이브러리
  - Fire-and-forget, Delayed, Recurring, Continuation
  - 자동 재시도와 지수 백오프
  - 대시보드 UI로 작업 모니터링
  - 분산 환경에서의 작업 관리
- 스케줄링 패턴
  - Cron 표현식: Node.js의 node-cron과 비교
  - 시간대(Timezone) 처리
  - 중복 실행 방지
- 메시지 큐와 이벤트 기반 아키텍처
  - RabbitMQ: AMQP 프로토콜
  - Azure Service Bus: 클라우드 네이티브 큐
  - 메시지 발행/구독 패턴
  - 느슨한 결합과 확장성

**핵심 개념**: SSE, WebSocket, SignalR, IHostedService, Hangfire, 메시지 큐, 이벤트 기반 아키텍처

**실습**:
- 실시간 대시보드: SignalR로 서버 메트릭 실시간 업데이트
- 백그라운드 작업 시스템: 이미지 처리를 백그라운드로 처리하고 진행 상태 표시
- 이벤트 기반 주문 시스템: 주문 생성 → 이메일 발송, 재고 업데이트 (메시지 큐 사용)

### Chapter 18: API 클라이언트 패턴

타입 안전성과 선언적 데이터 페칭을 결합하여, 프론트엔드-백엔드 통합의 최선의 경험을 만듭니다.

- 타입 안전한 API 클라이언트의 필요성
  - 런타임 vs 컴파일 타임 타입 검증
  - 서버 타입 변경 시 프론트엔드 자동 감지
- NSwag: OpenAPI에서 TypeScript 클라이언트 생성
  - CLI 도구 사용법
  - MSBuild 통합으로 자동 생성
  - 생성된 클라이언트 사용법
  - 커스터마이징: 템플릿, 네이밍 규칙
- Kiota: Microsoft의 차세대 생성기
  - Fluent API 스타일
  - 다양한 언어 지원
  - 플러그인 시스템
- 프론트엔드 상태 관리와 API 통합
  - React Query + 생성된 클라이언트
  - RTK Query + OpenAPI 타입
  - SWR + TypeScript
- Optimistic UI 업데이트
  - 낙관적 업데이트 패턴
  - 실패 시 롤백
  - 서버 조정(Reconciliation)
- 에러 처리 패턴
  - 전역 에러 핸들러
  - 재시도 로직: 지수 백오프
  - 에러 경계(Error Boundary)
  - 사용자 친화적 에러 메시지
- 캐싱 전략의 모든 것
  - 브라우저 캐싱: Cache-Control, ETag, Last-Modified
  - 클라이언트 사이드 캐싱: React Query, SWR
  - 서버 사이드 캐싱: IMemoryCache, IDistributedCache
  - HybridCache (.NET 9): L1 + L2 캐싱
  - 캐시 무효화 전략
  - Stale-While-Revalidate 패턴

**핵심 개념**: 타입 안전성, API 클라이언트 생성, React Query, Optimistic UI, 다계층 캐싱

**실습**:
- NSwag로 타입 안전한 클라이언트 생성 및 React 앱 통합
- React Query + 생성된 클라이언트로 선언적 데이터 페칭
- Optimistic UI로 즉각적인 사용자 경험 구현
- HybridCache + React Query로 다계층 캐싱 적용

## 프로덕션 체크리스트

Part 7을 학습하며 다음 원칙들을 내재화하세요:

**실시간 통신:**
- [ ] 적절한 실시간 전략 선택 (SSE vs WebSocket vs SignalR)
- [ ] 재연결 로직 구현 (클라이언트)
- [ ] 하트비트로 연결 유지 확인
- [ ] 확장성 고려 (Redis backplane, Azure SignalR)
- [ ] 에러 처리 및 로깅
- [ ] 인증된 사용자만 연결 허용

**백그라운드 작업:**
- [ ] 긴 작업은 HTTP 요청에서 분리
- [ ] 정상 종료(graceful shutdown) 지원
- [ ] 재시도 로직과 지수 백오프
- [ ] 작업 실패 시 알림
- [ ] 진행 상태 추적 및 UI 표시
- [ ] 분산 환경에서 중복 실행 방지
- [ ] 작업 로그 및 모니터링

**API 클라이언트:**
- [ ] 타입 안전한 클라이언트 자동 생성
- [ ] CI/CD에서 클라이언트 재생성 자동화
- [ ] 에러 처리 및 재시도 로직
- [ ] 타임아웃 설정
- [ ] 취소(Cancellation) 지원
- [ ] 로딩 상태 표시

**캐싱:**
- [ ] 적절한 캐싱 계층 선택
- [ ] 캐시 키 전략 수립
- [ ] 만료 시간 설정
- [ ] 캐시 무효화 전략
- [ ] Stampede 문제 방지
- [ ] 메모리 사용량 모니터링

## 다음 단계

Part 7을 마치면, 여러분은 프로덕션 환경에서 견고하고 확장 가능한 시스템을 만들 수 있습니다. 실시간 통신, 백그라운드 작업, 타입 안전한 통합, 효율적인 캐싱—이 모든 것이 실전에서 작동합니다.

**Part 8: 상태 관리와 패턴**에서는 더 고급 아키텍처 패턴을 배웁니다. Clean Architecture, CQRS, Domain-Driven Design, Microservices... 대규모 시스템을 설계하고 유지보수하는 방법을 마스터하게 될 것입니다.

지금 바로 Chapter 17로 이동하여, 첫 실시간 기능을 구현해보세요!

---

## 참고 자료

- [Server-Sent Events 가이드](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events)
- [WebSocket Protocol RFC](https://datatracker.ietf.org/doc/html/rfc6455)
- [SignalR 공식 문서](https://docs.microsoft.com/aspnet/core/signalr/)
- [Hosted Services in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/host/hosted-services)
- [Hangfire 문서](https://docs.hangfire.io/)
- [RabbitMQ Tutorials](https://www.rabbitmq.com/getstarted.html)
- [NSwag 문서](https://github.com/RicoSuter/NSwag/wiki)
- [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/)
- [React Query 문서](https://tanstack.com/query/latest)
- [.NET 9 HybridCache](https://learn.microsoft.com/aspnet/core/performance/caching/hybrid)

**예상 학습 시간**: 2-3주 (각 챕터당 7-10일, 실습 포함)
