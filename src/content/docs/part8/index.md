---
title: "Part 8 - 상태 관리와 패턴"
---

# Part 8: 상태 관리와 패턴

## 복잡성을 제어하는 아키텍처: 확장 가능한 시스템의 설계

Part 7까지 여러분은 API를 만들고, 실시간 통신을 구현하며, 백그라운드 작업을 처리하고, 클라이언트와 서버를 타입 안전하게 연결하는 방법을 배웠습니다. 이제 여러분의 애플리케이션은 작동합니다. 사용자는 데이터를 조회하고, 생성하며, 실시간으로 업데이트를 받습니다. 하지만 시간이 지나면서 새로운 도전이 나타납니다. 코드베이스가 커지고, 팀이 성장하며, 요구사항이 복잡해집니다. 처음에는 명확했던 구조가 점점 뒤엉키고, 새 기능을 추가하는 것이 점점 어려워집니다. "이 로직은 어디에 있어야 하지?", "이 클래스의 책임이 너무 많은 것 아닌가?", "왜 이 테스트를 작성하기가 이렇게 어렵지?"—이런 질문들이 더 자주 떠오릅니다.

Part 8은 이런 복잡성을 제어하는 방법을 다룹니다. 상태를 어떻게 관리하고, 아키텍처를 어떻게 설계하며, 패턴을 어떻게 적용하여 시스템이 시간이 지나도 유지보수 가능하고 확장 가능하도록 만드는지 배웁니다. 프론트엔드 개발자로서 Redux, MobX, Vuex 같은 상태 관리 라이브러리를 경험했을 것입니다. 컴포넌트 간 상태 공유, 전역 상태, 로컬 상태의 균형... 이 모든 개념은 서버 사이드에도 적용됩니다. 하지만 서버의 상태는 다릅니다. 수천 명의 동시 사용자, 여러 서버 인스턴스, 분산 캐시, 데이터베이스—이 모든 것이 상태의 일부이며, 조화롭게 관리되어야 합니다.

### 상태의 본질: 프론트엔드와 서버의 차이

프론트엔드에서 상태는 명확합니다. React의 `useState`, Redux의 store, Vue의 reactive data—모두 단일 브라우저 인스턴스에 존재하며, 사용자 한 명에게만 영향을 미칩니다. 사용자가 페이지를 새로고침하면 상태는 사라지고, 서버에서 다시 가져옵니다.

서버의 상태는 훨씬 복잡합니다. 여러 계층과 범위에 걸쳐 있으며, 각각 다른 수명 주기를 가집니다:

**1. 요청 범위 상태 (Request-scoped State)**

HTTP 요청이 들어오면, ASP.NET Core는 해당 요청을 처리하는 동안만 존재하는 서비스 인스턴스를 생성합니다. 이것이 Scoped 서비스입니다. Entity Framework Core의 `DbContext`가 대표적인 예입니다. 각 요청은 자신만의 `DbContext`를 가지며, 요청이 끝나면 폐기됩니다.

```csharp
services.AddScoped<ApplicationDbContext>();
```

이는 React의 컴포넌트 로컬 상태(`useState`)와 유사합니다. 컴포넌트(요청)가 마운트될 때 생성되고, 언마운트될 때 사라집니다. 이 범위는 동시성 문제로부터 안전합니다. 각 요청이 독립적이므로, 한 사용자의 요청이 다른 사용자의 요청에 영향을 주지 않습니다.

**2. 애플리케이션 상태 (Application-scoped State)**

애플리케이션이 시작될 때 한 번 생성되고, 종료될 때까지 유지되는 상태입니다. Singleton 서비스가 이 범주에 속합니다. 설정 값, 캐시, 연결 풀, 로거—이런 것들은 모든 요청에서 공유되므로 효율적입니다.

```csharp
services.AddSingleton<IMemoryCache, MemoryCache>();
```

이는 프론트엔드의 전역 상태(Redux store, Vuex store)와 유사합니다. 모든 컴포넌트가 접근할 수 있지만, 동시성 문제를 주의해야 합니다. 여러 요청이 동시에 같은 Singleton을 수정하려 하면 경쟁 조건(race condition)이 발생할 수 있습니다.

**3. Transient 서비스 (Transient-scoped State)**

매번 요청될 때마다 새로운 인스턴스가 생성됩니다. 가벼운 상태 비저장 서비스에 적합하며, 각 사용처마다 독립적인 인스턴스를 갖습니다.

```csharp
services.AddTransient<IEmailSender, EmailSender>();
```

`IEmailSender`가 주입될 때마다 새 `EmailSender` 인스턴스가 생성됩니다. 메모리 오버헤드가 있지만, 스레드 안전성을 보장하기 쉽습니다. 상태를 가지지 않는 유틸리티 서비스나 짧은 수명의 작업에 적합합니다.

**언제 어떤 범위를 사용할까?**
- **Singleton**: 상태가 없거나, 스레드 안전하게 공유 가능한 서비스 (로거, 캐시, 설정)
- **Scoped**: 요청 단위로 상태를 유지하는 서비스 (DbContext, 현재 사용자 정보)
- **Transient**: 상태가 없고 가벼운 서비스 (이메일 발송, 데이터 변환)

**4. 사용자 세션 상태 (Session State)**

특정 사용자와 연관되어, 여러 요청에 걸쳐 유지되는 상태입니다. 로그인 정보, 장바구니, 사용자 설정—이런 데이터는 세션에 저장됩니다. ASP.NET Core는 세션을 쿠키나 분산 캐시(Redis 등)에 저장할 수 있습니다.

```csharp
HttpContext.Session.SetString("CartId", cartId);
```

프론트엔드의 localStorage나 sessionStorage와 개념적으로 유사하지만, 서버에서 관리된다는 차이가 있습니다. 이는 보안성은 높지만, 확장성 문제가 있습니다. 로드 밸런서 뒤에 여러 서버가 있다면, 세션 정보를 어떻게 공유할까요? 이것이 분산 세션(Redis)이 필요한 이유입니다.

**4. 데이터베이스 상태 (Persistent State)**

가장 영속적인 상태로, 애플리케이션이 재시작되어도 유지됩니다. 사용자 데이터, 주문 내역, 제품 정보—이 모든 것은 데이터베이스에 저장됩니다. 이는 프론트엔드의 영역 밖이지만, 서버 개발의 핵심입니다.

각 상태 범위를 적절히 선택하는 것이 중요합니다. 모든 것을 Singleton으로 만들면 메모리를 절약할 수 있지만 동시성 문제에 취약해지고, 모든 것을 Scoped로 만들면 안전하지만 성능이 저하됩니다.

### 세션 vs JWT: 상태 저장의 두 철학

사용자 인증 정보를 어떻게 관리할까요? 전통적인 세션 방식과 현대적인 JWT 방식은 근본적으로 다른 철학을 가지고 있습니다.

**세션 방식: 서버가 진실의 원천**

사용자가 로그인하면, 서버는 세션 ID를 생성하고 세션 저장소(메모리, Redis, 데이터베이스)에 사용자 정보를 저장합니다. 클라이언트는 쿠키로 세션 ID만 받습니다. 이후 요청마다 세션 ID를 보내면, 서버는 저장소를 조회하여 사용자를 식별합니다.

장점:
- **즉시 무효화 가능**: 사용자를 로그아웃시키려면 세션을 삭제하면 됩니다. 다음 요청은 즉시 실패합니다.
- **서버가 완전히 제어**: 세션 데이터를 언제든 수정할 수 있습니다.
- **작은 쿠키 크기**: 세션 ID만 전송하므로, 네트워크 오버헤드가 적습니다.

단점:
- **상태 저장**: 서버가 세션 정보를 유지해야 하므로, 메모리나 저장소가 필요합니다.
- **확장성 문제**: 여러 서버가 있다면, 세션을 어떻게 공유할까요? Sticky session(같은 사용자는 항상 같은 서버로)이나 분산 세션(Redis)이 필요합니다.

**JWT 방식: 클라이언트가 상태를 소유**

사용자가 로그인하면, 서버는 사용자 정보를 JWT로 인코딩하고 서명하여 클라이언트에 전달합니다. 클라이언트는 이후 요청마다 이 토큰을 헤더에 포함합니다. 서버는 서명을 검증하여 토큰이 위조되지 않았음을 확인하고, 토큰 내부의 정보를 신뢰합니다.

장점:
- **상태 비저장**: 서버가 아무것도 저장하지 않으므로, 메모리가 절약되고 수평 확장이 쉽습니다.
- **마이크로서비스 친화적**: 토큰만 있으면 어떤 서비스든 사용자를 인증할 수 있습니다.
- **크로스 도메인**: 쿠키와 달리 도메인 제한이 없습니다.

단점:
- **즉시 무효화 불가**: 토큰은 만료 시간까지 유효합니다. 사용자를 로그아웃시켜도 토큰이 살아있으면 계속 사용할 수 있습니다.
- **토큰 크기**: 모든 사용자 정보가 포함되므로, 쿠키보다 큽니다.
- **갱신 복잡성**: 단기 Access Token + 장기 Refresh Token 패턴이 필요합니다.

선택 기준:
- **세션을 선택하세요**: 엄격한 보안이 필요하거나(예: 뱅킹), 사용자를 즉시 로그아웃시켜야 하거나, 단일 모놀리스 애플리케이션인 경우.
- **JWT를 선택하세요**: 마이크로서비스 아키텍처이거나, 모바일 앱 지원이 필요하거나, 상태 비저장 확장이 중요한 경우.

많은 시스템은 하이브리드를 사용합니다. Access Token은 JWT로 발급하되, Refresh Token은 서버 데이터베이스에 저장하여 즉시 무효화할 수 있게 합니다.

### 캐싱: 성능의 배가수

캐싱은 성능 최적화에서 가장 효과적인 기법입니다. 데이터베이스 쿼리는 밀리초 단위지만, 메모리 접근은 나노초 단위입니다. 캐싱을 적절히 적용하면, 응답 시간을 수십 배 단축하고, 데이터베이스 부하를 극적으로 줄일 수 있습니다.

하지만 캐싱은 복잡성을 증가시킵니다. Phil Karlton의 유명한 말처럼, "컴퓨터 과학에서 어려운 것은 두 가지뿐이다: 캐시 무효화와 이름 짓기." 캐시된 데이터가 언제 stale해지는지, 어떻게 무효화할지, 얼마나 오래 보관할지—이 모든 것을 신중히 결정해야 합니다.

**계층적 캐싱 전략**

ASP.NET Core는 여러 캐싱 계층을 제공합니다:

**1. Response Caching (HTTP 캐싱)**

가장 외부 계층으로, HTTP 헤더를 통해 브라우저나 CDN이 응답을 캐싱하도록 합니다.

```csharp
[ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "id" })]
public IActionResult GetProduct(int id)
{
    var product = _productService.GetProduct(id);
    return Ok(product);
}
```

이 응답은 1시간 동안 브라우저에 캐싱됩니다. 같은 제품을 다시 요청하면 서버에 도달조차 하지 않습니다. 단, 동적 컨텐츠나 사용자별 데이터에는 적합하지 않습니다.

**2. In-Memory Caching (IMemoryCache)**

서버의 메모리에 데이터를 캐싱합니다. 빠르고 간단하지만, 서버 인스턴스마다 독립적입니다.

```csharp
public async Task<Product> GetProductAsync(int id)
{
    var cacheKey = $"product_{id}";

    if (!_cache.TryGetValue(cacheKey, out Product product))
    {
        product = await _db.Products.FindAsync(id);

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1))
            .SetSize(1);

        _cache.Set(cacheKey, product, cacheOptions);
    }

    return product;
}
```

`SetSlidingExpiration`은 접근할 때마다 만료 시간이 연장되고, `SetAbsoluteExpiration`은 절대 만료 시간을 설정합니다. `SetSize`는 메모리 관리를 위한 상대적 크기입니다.

**3. Distributed Caching (IDistributedCache)**

여러 서버가 공유하는 캐시로, 주로 Redis를 사용합니다.

```csharp
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "MyApp_";
});
```

분산 캐시는 일관성을 보장하지만, 네트워크 왕복이 필요하므로 메모리 캐시보다 느립니다. 하지만 단일 서버의 메모리 제약을 넘어서는 대용량 데이터를 캐싱하거나, 여러 서버 간 일관성이 중요할 때 필수적입니다.

**4. HybridCache (.NET 9의 게임 체인저)**

.NET 9의 `HybridCache`는 L1(메모리) + L2(분산 캐시)의 2단계 캐싱을 제공하며, 캐시 stampede 문제를 자동으로 해결합니다.

```csharp
public async Task<Product> GetProductAsync(int id)
{
    return await _hybridCache.GetOrCreateAsync(
        $"product_{id}",
        async cancel => await _db.Products.FindAsync(id, cancel),
        new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(2)
        }
    );
}
```

이 코드는 먼저 메모리 캐시를 확인하고, 없으면 Redis를 확인하며, 그것도 없으면 데이터베이스를 조회합니다. 여러 요청이 동시에 같은 키를 요청해도, 데이터베이스 쿼리는 한 번만 실행되고 결과는 모든 요청에 공유됩니다.

**캐시 무효화 전략**

캐시의 가장 어려운 부분은 언제 무효화할지 결정하는 것입니다. 여러 전략이 있습니다:

- **Time-based Expiration (TTL)**: 가장 단순합니다. N분 후 자동으로 만료됩니다. 정확성보다 성능이 중요하고, 약간 오래된 데이터가 허용되는 경우(뉴스 피드, 제품 목록)에 적합합니다.

- **Event-based Invalidation**: 데이터가 변경되면 명시적으로 캐시를 무효화합니다. 정확성이 중요한 경우(가격 정보, 재고)에 필수적입니다.

```csharp
public async Task UpdateProductAsync(Product product)
{
    await _db.SaveChangesAsync();
    _cache.Remove($"product_{product.Id}");
}
```

- **Cache Tagging**: 관련된 캐시 항목을 태그로 그룹화하여 한 번에 무효화합니다. "products", "orders" 같은 태그로 묶어, 전체 카테고리를 무효화할 수 있습니다.

### Clean Architecture: 의존성의 방향을 제어하라

시스템이 커지면서 가장 큰 문제는 의존성의 얽힘입니다. UI가 데이터베이스를 직접 호출하고, 비즈니스 로직이 프레임워크에 강하게 결합되며, 테스트는 점점 어려워집니다. Clean Architecture는 이 문제를 해결하기 위한 패턴입니다.

Clean Architecture의 핵심 원칙은 **의존성 규칙(Dependency Rule)**입니다: 외부 계층이 내부 계층을 의존할 수 있지만, 내부 계층은 외부 계층을 알아서는 안 됩니다.

**계층 구조와 의존성 방향:**

```
┌─────────────────────────────────────────┐
│  Presentation & Infrastructure Layer   │  ← 외부 계층 (Framework, DB, UI)
│           ↓ 의존 방향 ↓                 │
├─────────────────────────────────────────┤
│       Application Layer                 │  ← 유즈 케이스
│           ↓ 의존 방향 ↓                 │
├─────────────────────────────────────────┤
│         Domain Layer                    │  ← 핵심 비즈니스 로직
│      (독립적, 의존성 없음)               │
└─────────────────────────────────────────┘
```

**의존성 규칙**:
- Presentation과 Infrastructure는 모두 Application에 의존합니다
- Application은 Domain에 의존합니다
- Domain은 어떤 계층에도 의존하지 않습니다 (순수한 비즈니스 로직)
- 화살표는 항상 안쪽(Domain)을 향합니다

**Domain Layer (핵심)**

비즈니스 규칙과 엔티티가 존재합니다. 프레임워크, 데이터베이스, UI에 대해 아무것도 모릅니다. 순수한 C# 클래스로만 구성됩니다.

```csharp
// Domain/Entities/Order.cs
public class Order
{
    public int Id { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    public decimal TotalAmount => Items.Sum(i => i.Price * i.Quantity);

    public void AddItem(Product product, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive");

        Items.Add(new OrderItem(product, quantity));
    }
}
```

**Application Layer**

유즈 케이스를 구현합니다. MediatR의 Command/Query 패턴을 자주 사용합니다.

```csharp
// Application/Orders/Commands/CreateOrderCommand.cs
public record CreateOrderCommand(int UserId, List<OrderItemDto> Items) : IRequest<int>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderRepository _repository;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandHandler(
        IOrderRepository repository,
        IProductRepository productRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var order = new Order(request.UserId);
        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            order.AddItem(product, item.Quantity);
        }

        await _repository.AddAsync(order);
        return order.Id;
    }
}
```

**Infrastructure Layer**

데이터베이스, 외부 API, 파일 시스템 같은 외부 의존성을 구현합니다.

```csharp
// Infrastructure/Persistence/OrderRepository.cs
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
}
```

**Presentation Layer**

API 컨트롤러나 Razor Pages가 있습니다. 얇은 계층으로, 요청을 Application Layer로 전달하고 응답을 형식화합니다.

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        var command = new CreateOrderCommand(User.GetUserId(), dto.Items);
        var orderId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = orderId }, null);
    }
}
```

이 구조의 장점은 명확합니다:
- **테스트 가능성**: Domain과 Application 계층은 프레임워크 없이 테스트할 수 있습니다.
- **유연성**: 데이터베이스를 SQL Server에서 PostgreSQL로, UI를 MVC에서 Blazor로 바꿔도 Domain/Application은 변경되지 않습니다.
- **명확한 책임**: 각 계층의 역할이 분명합니다.

### CQRS: 읽기와 쓰기를 분리하라

전통적인 CRUD 접근에서는 하나의 모델이 읽기와 쓰기를 모두 처리합니다. 하지만 읽기와 쓰기의 요구사항은 종종 다릅니다. 주문을 생성할 때는 비즈니스 규칙 검증이 중요하지만, 주문 목록을 조회할 때는 성능이 중요합니다. 하나의 모델로 두 가지를 모두 최적화하기는 어렵습니다.

CQRS (Command Query Responsibility Segregation)는 읽기(Query)와 쓰기(Command)를 완전히 분리합니다.

**Command (쓰기)**

상태를 변경하지만 값을 반환하지 않습니다(또는 최소한의 확인 정보만 반환).

```csharp
public record CreateOrderCommand(int UserId, List<OrderItemDto> Items) : IRequest<int>;
```

Command는 비즈니스 로직을 거치며, 도메인 이벤트를 발생시킬 수 있고, 트랜잭션을 사용합니다.

**Query (읽기)**

상태를 변경하지 않고, 데이터만 반환합니다.

```csharp
public record GetOrdersQuery(int UserId, int Page, int PageSize) : IRequest<PagedResult<OrderDto>>;
```

Query는 비즈니스 로직을 우회하고, 데이터베이스를 직접 조회하여 DTO로 프로젝션합니다. 심지어 읽기 전용 데이터베이스 복제본을 사용할 수도 있습니다.

**MediatR을 통한 구현**

MediatR 라이브러리는 CQRS를 쉽게 구현하게 해줍니다.

```csharp
// Program.cs
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Controller
[HttpPost]
public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
{
    var command = new CreateOrderCommand(User.GetUserId(), dto.Items);
    var orderId = await _mediator.Send(command);
    return Ok(new { orderId });
}

[HttpGet]
public async Task<IActionResult> GetOrders(int page = 1, int pageSize = 20)
{
    var query = new GetOrdersQuery(User.GetUserId(), page, pageSize);
    var result = await _mediator.Send(query);
    return Ok(result);
}
```

CQRS의 이점:
- **성능 최적화**: 읽기는 비정규화된 뷰를 사용하고, 쓰기는 정규화된 모델을 사용할 수 있습니다.
- **확장성**: 읽기 데이터베이스와 쓰기 데이터베이스를 물리적으로 분리할 수 있습니다.
- **명확한 의도**: 코드를 보면 상태를 변경하는지(Command) 조회만 하는지(Query)가 분명합니다.

### Part 8에서 배울 내용

이제 여러분은 상태 관리의 복잡성을 이해하고, 아키텍처 패턴으로 복잡성을 제어하는 방법을 배우게 될 것입니다.

**Chapter 19: 서버 사이드 상태 관리**

상태의 여러 계층(요청, 애플리케이션, 세션, 영속적)을 이해하고 각각을 적절히 사용합니다. 세션과 JWT의 차이를 깊이 비교하며, 각각의 적합한 사용 사례를 배웁니다.

캐싱의 모든 계층(Response, Memory, Distributed, Hybrid)을 마스터하며, 캐시 무효화 전략을 학습합니다. .NET 9의 HybridCache가 어떻게 L1+L2 캐싱과 stampede 방지를 제공하는지 경험합니다.

TempData, ViewData, ViewBag의 차이와 사용 시나리오를 배우며, 실습에서는 분산 세션을 사용한 장바구니와 HybridCache를 적용한 제품 카탈로그를 구현합니다.

**Chapter 20: 고급 아키텍처 패턴**

Clean Architecture를 처음부터 구축하며, Domain, Application, Infrastructure, Presentation 계층을 명확히 분리합니다. SOLID 원칙을 각 계층에 적용하고, 의존성 규칙이 어떻게 테스트 가능성을 향상시키는지 경험합니다.

CQRS 패턴을 MediatR로 구현하며, Command와 Query를 분리합니다. Repository와 Unit of Work 패턴의 장단점을 배우며, EF Core가 이미 이 패턴을 구현하고 있다는 것을 이해합니다.

Domain-Driven Design (DDD)의 기초를 배우며, Aggregate, Entity, Value Object, Domain Event를 실전에 적용합니다. 실습에서는 Modular Monolith 구조로 프로젝트를 설계하며, 점진적으로 마이크로서비스로 전환할 수 있는 기반을 만듭니다.

## 학습 목표

Part 8을 마치면 다음을 할 수 있습니다:

- 서버 사이드 상태의 여러 범위(Scoped, Singleton, Session)를 이해하고 적절히 선택할 수 있습니다
- 세션 기반 인증과 JWT 기반 인증의 차이를 이해하고 트레이드오프를 고려하여 선택합니다
- 여러 계층의 캐싱 전략을 적용하고 캐시 무효화 패턴을 구현할 수 있습니다
- HybridCache를 사용하여 L1+L2 캐싱과 stampede 방지를 구현합니다
- Clean Architecture 원칙에 따라 프로젝트를 계층화할 수 있습니다
- CQRS 패턴을 MediatR로 구현하여 읽기와 쓰기를 분리합니다
- Repository와 Unit of Work 패턴을 이해하고 적절히 사용합니다
- DDD의 기본 개념(Aggregate, Entity, Value Object)을 적용할 수 있습니다
- Modular Monolith 구조로 확장 가능한 시스템을 설계합니다

## 챕터 구성

### [Chapter 19: 서버 사이드 상태 관리](./chapter19/)

서버에서 상태를 관리하는 모든 방법을 배우고, 각각의 적합한 사용 사례를 이해합니다.

**상태의 범위:**
- Scoped 서비스: 요청당 하나의 인스턴스
- Singleton 서비스: 애플리케이션당 하나의 인스턴스
- Transient 서비스: 요청할 때마다 새 인스턴스
- 각 범위의 장단점과 동시성 고려사항

**세션 관리:**
- In-Memory 세션: 단일 서버 환경
- Distributed 세션: Redis를 통한 서버 간 공유
- 쿠키 기반 세션: 클라이언트 측 저장
- 세션 vs JWT: 심층 비교
  - 상태 저장 vs 상태 비저장
  - 즉시 무효화 vs 만료 시간
  - 확장성 vs 유연성
- Refresh Token 패턴 재방문

**캐싱 전략의 모든 것:**
- Response Caching: HTTP 헤더 기반 브라우저/CDN 캐싱
  - Cache-Control, ETag, Last-Modified
  - Vary 헤더로 조건부 캐싱
- IMemoryCache: 서버 메모리 캐싱
  - Sliding vs Absolute Expiration
  - 메모리 제한과 우선순위
  - 캐시 항목 크기 관리
- IDistributedCache: Redis 기반 분산 캐싱
  - 여러 서버 간 일관성
  - 직렬화와 역직렬화 오버헤드
  - 네트워크 지연 고려
- HybridCache (.NET 9): 통합된 L1+L2 캐싱
  - 메모리 캐시 + 분산 캐시 자동 관리
  - Stampede 방지: 동시 요청 시 단일 쿼리만 실행
  - 타입 안전한 API

**캐시 무효화 패턴:**
- Time-based: TTL (Time To Live)
- Event-based: 데이터 변경 시 명시적 무효화
- Cache Tagging: 관련 항목 그룹화하여 무효화
- Cache-Aside vs Write-Through vs Write-Behind
- Stale-While-Revalidate: 만료된 캐시를 반환하며 백그라운드에서 갱신

**TempData, ViewData, ViewBag:**
- TempData: 리디렉션 간 데이터 전달
- ViewData: 뷰로 데이터 전달 (딕셔너리)
- ViewBag: ViewData의 dynamic 래퍼
- 각각의 수명과 사용 사례

**핵심 개념**: 상태 범위, 세션 관리, 다계층 캐싱, HybridCache, 캐시 무효화

**실습**:
- 분산 세션을 사용한 장바구니 시스템
- HybridCache로 제품 카탈로그 최적화
- 캐시 무효화 전략 비교 (Time-based vs Event-based)

### [Chapter 20: 고급 아키텍처 패턴](./chapter20/)

복잡성을 제어하고 유지보수 가능한 시스템을 만드는 아키텍처 패턴을 배웁니다.

**Clean Architecture:**
- 계층 분리의 원칙
  - Domain Layer: 비즈니스 로직과 엔티티
  - Application Layer: 유즈 케이스
  - Infrastructure Layer: 데이터 액세스, 외부 서비스
  - Presentation Layer: API, UI
- 의존성 규칙: 내부 계층은 외부를 모른다
- 의존성 역전 원칙 (DIP) 적용
- 프로젝트 구조 예제
- SOLID 원칙과의 관계

**CQRS (Command Query Responsibility Segregation):**
- Command와 Query의 분리
- MediatR 라이브러리를 통한 구현
  - IRequest, IRequestHandler 인터페이스
  - Pipeline Behaviors: 로깅, 유효성 검사, 트랜잭션
- 읽기/쓰기 모델 분리의 이점
- 성능 최적화: 읽기 전용 데이터베이스 복제본
- Event Sourcing과의 결합 (소개)

**Repository와 Unit of Work 패턴:**
- Generic Repository 구현
- Unit of Work로 트랜잭션 관리
- 언제 사용하고 언제 피해야 할까?
  - EF Core는 이미 Repository + Unit of Work
  - 추상화의 과도함 vs 테스트 용이성
- Specification 패턴으로 쿼리 재사용

**Domain-Driven Design (DDD) 기초:**
- Aggregate: 일관성 경계
- Entity: 식별자를 가진 객체
- Value Object: 불변 객체
- Domain Events: 도메인 내 통신
- Bounded Context: 모델의 경계
- Ubiquitous Language: 공통 언어
- DDD와 Clean Architecture의 결합

**Microservices 아키텍처 소개:**
- 모놀리스에서 마이크로서비스로의 전환
- 서비스 간 통신: REST, gRPC, 메시지 큐
- API Gateway: YARP (Yet Another Reverse Proxy)
  - 라우팅, 로드 밸런싱, 인증 집중화
- 서비스 디스커버리: Consul, Eureka
- 분산 트랜잭션의 어려움: Saga 패턴

**Modular Monolith:**
- 마이크로서비스의 이점을 모놀리스에서
- 모듈별 프로젝트 분리
- 모듈 간 통신: .NET Channels, MediatR
- 점진적 마이크로서비스 전환 전략

**핵심 개념**: Clean Architecture, CQRS, MediatR, DDD, Modular Monolith

**실습**:
- Clean Architecture로 주문 시스템 구축
- MediatR로 CQRS 패턴 적용
- Domain Events로 모듈 간 통신
- Modular Monolith 구조 설계

## 아키텍처 설계 체크리스트

Part 8을 학습하며 다음 원칙들을 내재화하세요:

**상태 관리:**
- [ ] 적절한 서비스 수명 선택 (Scoped, Singleton, Transient)
- [ ] 세션 vs JWT 트레이드오프 이해
- [ ] 분산 환경에서 세션 공유 전략 수립
- [ ] 캐싱 계층 선택 (Response, Memory, Distributed, Hybrid)
- [ ] 캐시 무효화 전략 명확히 정의
- [ ] Stampede 문제 인식 및 방지

**Clean Architecture:**
- [ ] 의존성이 항상 내부를 향하도록 설계
- [ ] Domain Layer는 프레임워크 독립적
- [ ] Infrastructure는 인터페이스를 통해서만 접근
- [ ] Application Layer는 유즈 케이스 중심으로 구성
- [ ] Presentation은 얇은 계층으로 유지

**CQRS:**
- [ ] 읽기와 쓰기 모델을 분리
- [ ] Command는 상태 변경, Query는 조회만
- [ ] MediatR Pipeline으로 횡단 관심사 처리
- [ ] 읽기 성능 최적화 (비정규화, 프로젝션)

**DDD:**
- [ ] Aggregate 경계를 명확히 정의
- [ ] Entity와 Value Object 구분
- [ ] Domain Event로 느슨한 결합
- [ ] Ubiquitous Language 사용
- [ ] 비즈니스 로직은 Domain Layer에

## 실습 프로젝트

### Chapter 19 실습: 고성능 장바구니 시스템

분산 세션과 HybridCache를 결합하여 확장 가능한 장바구니를 구현합니다:
- Redis를 사용한 분산 세션
- 사용자별 장바구니 상태 관리
- HybridCache로 제품 정보 캐싱
- 캐시 무효화: 제품 정보 변경 시 즉시 반영
- Stampede 방지 검증 (동시 요청 시나리오)
- 성능 벤치마크: 캐싱 전/후 비교

### Chapter 20 실습: Clean Architecture 주문 시스템

완전한 Clean Architecture 구조로 전자상거래 주문 시스템을 구축합니다:
- Domain Layer: Order, OrderItem, Product Aggregate
- Application Layer: CreateOrderCommand, GetOrdersQuery
- Infrastructure Layer: EF Core Repository, Redis Cache
- Presentation Layer: API Controllers
- MediatR로 CQRS 구현
- Domain Events로 주문 생성 시 재고 차감
- Validation Pipeline: FluentValidation 통합
- Transaction Pipeline: 자동 트랜잭션 관리
- Logging Pipeline: 모든 Command/Query 로깅

## 다음 단계

Part 8을 마치면, 여러분은 복잡한 시스템을 설계하고 상태를 효율적으로 관리할 수 있게 됩니다. Clean Architecture, CQRS, DDD—이 패턴들은 대규모 시스템에서 검증된 접근 방식입니다.

**Part 9: 테스팅 전략**에서는 이렇게 설계된 시스템을 어떻게 테스트하는지 배웁니다. 단위 테스트, 통합 테스트, E2E 테스트... 각 계층을 독립적으로 테스트하며, TDD(테스트 주도 개발)로 견고한 코드를 작성하는 방법을 마스터하게 될 것입니다.

지금 바로 Chapter 19로 이동하여, 상태 관리를 마스터하세요!

---

## 참고 자료

- [Dependency Injection in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/dependency-injection)
- [Response Caching in ASP.NET Core](https://docs.microsoft.com/aspnet/core/performance/caching/response)
- [Distributed Caching in ASP.NET Core](https://docs.microsoft.com/aspnet/core/performance/caching/distributed)
- [HybridCache in .NET 9](https://learn.microsoft.com/aspnet/core/performance/caching/hybrid)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR/wiki)
- [Domain-Driven Design](https://martinfowler.com/tags/domain%20driven%20design.html)
- [Modular Monolith](https://www.kamilgrzybek.com/blog/posts/modular-monolith-primer)

**예상 학습 시간**: 2-3주 (각 챕터당 7-10일, 실습 포함)
