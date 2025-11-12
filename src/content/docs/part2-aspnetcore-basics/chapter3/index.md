---
title: "Chapter 3 - ASP.NET Core의 핵심 아키텍처"
---

# Chapter 3: ASP.NET Core의 핵심 아키텍처

## 3.1 요청-응답 파이프라인: Express 미들웨어와의 비교

### 미들웨어 파이프라인의 실행 순서

웹 애플리케이션의 핵심은 HTTP 요청을 받아 처리하고 응답을 반환하는 것입니다. 이 과정에서 인증 확인, 로깅, 에러 처리 등 다양한 작업이 필요한데, 이를 체계적으로 관리하는 것이 미들웨어 파이프라인입니다.

Express.js를 사용해본 경험이 있다면, 미들웨어 개념이 익숙할 것입니다. Express에서 `app.use()`로 등록한 미들웨어들이 순서대로 실행되는 것처럼, ASP.NET Core도 동일한 패턴을 따릅니다. 하지만 ASP.NET Core는 더 명확한 구조와 타입 안정성을 제공합니다.

**Express.js 미들웨어 예제**:
```javascript
const express = require('express');
const app = express();

// 1. 로깅 미들웨어
app.use((req, res, next) => {
  console.log(`${req.method} ${req.url}`);
  next();  // 다음 미들웨어로 전달
});

// 2. JSON 파싱 미들웨어
app.use(express.json());

// 3. 인증 미들웨어
app.use((req, res, next) => {
  if (!req.headers.authorization) {
    return res.status(401).json({ error: 'Unauthorized' });
  }
  next();
});

// 4. 라우트 핸들러
app.get('/api/data', (req, res) => {
  res.json({ message: 'Success' });
});

// 5. 에러 핸들링 미들웨어
app.use((err, req, res, next) => {
  console.error(err);
  res.status(500).json({ error: 'Internal Server Error' });
});
```

**ASP.NET Core 미들웨어 예제**:
```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 1. 로깅 미들웨어 (내장)
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"{context.Request.Method} {context.Request.Path}");
    await next(context);  // 다음 미들웨어로 전달
});

// 2. 인증 미들웨어
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
        return;  // 파이프라인 중단
    }
    await next(context);
});

// 3. 라우팅 미들웨어
app.UseRouting();

// 4. 엔드포인트 실행
app.MapGet("/api/data", () => new { message = "Success" });

// 5. 에러 핸들링 (맨 앞에 배치해야 모든 예외를 잡음)
app.UseExceptionHandler("/error");

app.Run();
```

미들웨어 파이프라인은 양방향으로 흐릅니다. 요청이 들어오면 첫 번째 미들웨어부터 마지막까지 순차적으로 실행되고, 응답은 역순으로 돌아갑니다.

```
Request →  MW1 → MW2 → MW3 → Endpoint
Response ← MW1 ← MW2 ← MW3 ← Endpoint
```

실제 흐름을 코드로 확인해보겠습니다:

```csharp
app.Use(async (context, next) =>
{
    Console.WriteLine("MW1: Before");
    await next(context);
    Console.WriteLine("MW1: After");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("MW2: Before");
    await next(context);
    Console.WriteLine("MW2: After");
});

app.MapGet("/", () =>
{
    Console.WriteLine("Endpoint");
    return "Hello";
});

// 출력:
// MW1: Before
// MW2: Before
// Endpoint
// MW2: After
// MW1: After
```

이 양방향 흐름을 통해 응답 헤더 추가, 실행 시간 측정, 로깅 등을 우아하게 처리할 수 있습니다.

### `Use`, `Run`, `Map` 메서드의 차이

ASP.NET Core는 미들웨어를 등록하는 세 가지 주요 메서드를 제공합니다. 각각의 용도와 동작 방식이 다르므로 올바르게 이해하는 것이 중요합니다.

**`Use`: 체인을 계속 연결**

`Use`는 가장 일반적인 미들웨어 등록 방법으로, `next` 델리게이트를 호출하여 다음 미들웨어로 요청을 전달할 수 있습니다.

```csharp
app.Use(async (context, next) =>
{
    // 요청 전처리
    context.Response.Headers["X-Custom-Header"] = "MyValue";

    // 다음 미들웨어 실행
    await next(context);

    // 응답 후처리
    Console.WriteLine($"Response status: {context.Response.StatusCode}");
});
```

Express.js의 `app.use()`와 정확히 동일한 개념입니다. `next()`를 호출하지 않으면 파이프라인이 중단됩니다.

**`Run`: 터미널 미들웨어**

`Run`은 파이프라인을 종료하는 터미널 미들웨어입니다. `next` 델리게이트가 없으며, 이후의 미들웨어는 실행되지 않습니다.

```csharp
app.Run(async context =>
{
    await context.Response.WriteAsync("This is the end!");
    // 더 이상 진행되지 않음
});

// 이 미들웨어는 절대 실행되지 않음
app.Use(async (context, next) =>
{
    Console.WriteLine("Never called");
    await next(context);
});
```

`Run`은 보통 파이프라인의 마지막에 fallback 응답을 제공할 때 사용합니다:

```csharp
app.UseRouting();
app.MapControllers();

// 어떤 라우트와도 매치되지 않으면 404 반환
app.Run(async context =>
{
    context.Response.StatusCode = 404;
    await context.Response.WriteAsJsonAsync(new { error = "Not Found" });
});
```

**`Map`: 경로 기반 분기**

`Map`은 특정 경로에만 미들웨어를 적용하고 싶을 때 사용합니다. URL 경로를 기준으로 파이프라인을 분기합니다.

```csharp
// /api로 시작하는 요청만 처리
app.Map("/api", apiApp =>
{
    apiApp.Use(async (context, next) =>
    {
        Console.WriteLine("API request");
        await next(context);
    });

    apiApp.MapGet("/users", () => new { users = new[] { "Alice", "Bob" } });
});

// /admin으로 시작하는 요청만 처리
app.Map("/admin", adminApp =>
{
    adminApp.Use(async (context, next) =>
    {
        // 관리자 인증 체크
        if (!IsAdmin(context))
        {
            context.Response.StatusCode = 403;
            return;
        }
        await next(context);
    });

    adminApp.MapGet("/dashboard", () => "Admin Dashboard");
});
```

`MapWhen`을 사용하면 더 복잡한 조건으로 분기할 수 있습니다:

```csharp
app.MapWhen(
    context => context.Request.Headers["API-Version"] == "2.0",
    apiV2App =>
    {
        apiV2App.MapGet("/data", () => "API Version 2.0 Data");
    }
);
```

### 미들웨어 체인과 next() 개념

Express.js에서 `next()` 함수의 역할을 정확히 이해하는 것이 중요하듯, ASP.NET Core에서도 `next` 델리게이트의 역할을 이해해야 합니다.

**next()를 호출하는 경우**:
```csharp
app.Use(async (context, next) =>
{
    Console.WriteLine("Before");
    await next(context);  // 다음 미들웨어 실행
    Console.WriteLine("After");
});
```

**next()를 호출하지 않는 경우** (단락 회로, Short-circuit):
```csharp
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/health")
    {
        // 헬스 체크는 즉시 응답하고 종료
        await context.Response.WriteAsync("OK");
        return;  // next() 호출 안 함
    }

    await next(context);  // 다른 경로는 계속 진행
});
```

단락 회로는 성능 최적화에 유용합니다. 예를 들어, 정적 파일 요청이나 헬스 체크는 전체 파이프라인을 거칠 필요가 없으므로 조기에 응답하고 종료할 수 있습니다.

**미들웨어 클래스로 추출**:

복잡한 미들웨어는 별도 클래스로 분리하는 것이 좋습니다. Express.js에서 함수를 모듈로 분리하는 것과 유사합니다.

```csharp
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // 다음 미들웨어 실행
        await _next(context);

        stopwatch.Stop();
        _logger.LogInformation(
            "Request {Method} {Path} completed in {ElapsedMilliseconds}ms",
            context.Request.Method,
            context.Request.Path,
            stopwatch.ElapsedMilliseconds
        );
    }
}

// 확장 메서드로 등록 편의성 제공
public static class RequestTimingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestTimingMiddleware>();
    }
}

// Program.cs에서 사용
app.UseRequestTiming();
```

이제 TypeScript의 Express 미들웨어와 비교해보겠습니다:

```typescript
// Express.js
function requestTiming(req: Request, res: Response, next: NextFunction) {
  const start = Date.now();

  res.on('finish', () => {
    const duration = Date.now() - start;
    console.log(`${req.method} ${req.path} - ${duration}ms`);
  });

  next();
}

app.use(requestTiming);
```

ASP.NET Core의 클래스 기반 접근은 의존성 주입, 타입 안정성, 테스트 용이성 면에서 장점이 있습니다.

### 실행 순서의 중요성: 인증 → 라우팅 → 엔드포인트

미들웨어의 순서는 매우 중요합니다. 잘못된 순서는 보안 취약점이나 예상치 못한 동작을 초래할 수 있습니다.

**올바른 순서**:
```csharp
var app = builder.Build();

// 1. 예외 처리 (맨 먼저 등록하여 모든 예외를 잡음)
app.UseExceptionHandler("/error");

// 2. HTTPS 리디렉션 (HTTP → HTTPS)
app.UseHttpsRedirection();

// 3. 정적 파일 (인증 불필요, 조기 반환으로 성능 향상)
app.UseStaticFiles();

// 4. 라우팅 (경로 매칭)
app.UseRouting();

// 5. CORS (라우팅 후, 인증 전)
app.UseCors("MyPolicy");

// 6. 인증 (사용자 확인)
app.UseAuthentication();

// 7. 권한 부여 (인증 후 권한 체크)
app.UseAuthorization();

// 8. 커스텀 미들웨어
app.UseRequestTiming();

// 9. 엔드포인트 실행
app.MapControllers();

app.Run();
```

**잘못된 순서의 예**:
```csharp
// ❌ 잘못됨: 인증 전에 권한 부여
app.UseAuthorization();  // 아직 사용자 정보가 없음!
app.UseAuthentication();

// ❌ 잘못됨: 라우팅 전에 CORS
app.UseCors();
app.UseRouting();  // CORS가 제대로 작동하지 않을 수 있음

// ❌ 잘못됨: 예외 처리가 너무 늦음
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler("/error");  // 인증 중 예외를 잡지 못함
```

실제 시나리오로 이해해보겠습니다:

```csharp
// 시나리오: /api/protected 엔드포인트에 인증된 사용자만 접근
app.UseRouting();           // 1. 경로가 /api/protected임을 확인
app.UseAuthentication();    // 2. JWT 토큰 검증, User 정보 설정
app.UseAuthorization();     // 3. [Authorize] 특성 확인, 권한 체크
app.MapControllers();       // 4. 컨트롤러 액션 실행

// 만약 인증 전에 권한 부여가 실행되면?
app.UseRouting();
app.UseAuthorization();     // User가 null이므로 항상 거부됨
app.UseAuthentication();    // 너무 늦음
app.MapControllers();
```

## 3.2 의존성 주입(DI): Angular에서 본 것과 비슷하지만 더 강력한

### IoC 컨테이너의 개념

의존성 주입(Dependency Injection, DI)은 현대 웹 프레임워크의 핵심 패턴입니다. Angular, NestJS를 사용해봤다면 이미 익숙한 개념일 것입니다. 하지만 ASP.NET Core의 DI는 프레임워크 수준에서 깊이 통합되어 있고, 더 강력한 기능을 제공합니다.

먼저 DI가 없는 코드와 있는 코드를 비교해보겠습니다.

**DI 없이 (나쁜 예)**:
```csharp
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsers()
    {
        // 직접 인스턴스 생성 - 강한 결합
        var dbConnection = new SqlConnection("connection-string");
        var userService = new UserService(dbConnection);
        var users = userService.GetAll();

        return Ok(users);
    }
}
```

이 코드의 문제점:
- 테스트하기 어려움 (실제 DB 연결 필요)
- 유연하지 않음 (다른 DB로 교체 불가)
- 리소스 관리 어려움 (연결 해제 책임)
- 강한 결합 (UserService 변경 시 컨트롤러도 수정)

**DI 사용 (좋은 예)**:
```csharp
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    // 생성자 주입
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
}
```

IoC(Inversion of Control) 컨테이너가 자동으로 `IUserService`의 구현체를 찾아 주입합니다. 이를 위해 `Program.cs`에서 등록합니다:

```csharp
builder.Services.AddScoped<IUserService, UserService>();
```

Angular과 비교해보겠습니다:

**Angular**:
```typescript
@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private http: HttpClient) {}

  getUsers() {
    return this.http.get('/api/users');
  }
}

@Component({...})
export class UsersComponent {
  constructor(private userService: UserService) {
    // Angular가 자동 주입
  }
}
```

**ASP.NET Core**:
```csharp
// 서비스 등록
builder.Services.AddScoped<IUserService, UserService>();

// 자동 주입
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
}
```

개념은 유사하지만, ASP.NET Core는 인터페이스 기반 주입을 권장하여 더 나은 추상화를 제공합니다.

### 서비스 생명주기: Singleton, Scoped, Transient

DI 컨테이너에 서비스를 등록할 때, 인스턴스의 생명주기를 지정해야 합니다. 이는 언제 인스턴스가 생성되고 파괴되는지를 결정합니다.

**Transient (일시적)**: 매번 새 인스턴스 생성
```csharp
builder.Services.AddTransient<IEmailService, EmailService>();
```

- 요청할 때마다 새로운 인스턴스를 생성합니다
- 가볍고 상태가 없는 서비스에 적합합니다
- 예: 이메일 발송, 암호화 서비스

```csharp
public class EmailService : IEmailService
{
    private readonly Guid _instanceId = Guid.NewGuid();

    public void Send(string to, string message)
    {
        Console.WriteLine($"Instance {_instanceId}: Sending email to {to}");
    }
}

// 사용
public class OrderController
{
    private readonly IEmailService _email1;
    private readonly IEmailService _email2;

    public OrderController(IEmailService email1, IEmailService email2)
    {
        _email1 = email1;
        _email2 = email2;
        // email1과 email2는 서로 다른 인스턴스!
    }
}
```

**Scoped (범위)**: HTTP 요청당 하나의 인스턴스
```csharp
builder.Services.AddScoped<IUserService, UserService>();
```

- HTTP 요청 하나당 하나의 인스턴스를 생성하고 공유합니다
- 요청이 끝나면 자동으로 Dispose됩니다
- 데이터베이스 컨텍스트, 리포지토리에 가장 적합합니다
- **가장 많이 사용되는 생명주기**입니다

```csharp
public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext context, IScopedService service)
    {
        // 이 요청 동안 모든 곳에서 동일한 service 인스턴스 사용
        Console.WriteLine($"Request {context.TraceIdentifier}: {service.InstanceId}");
        await _next(context);
    }
}
```

**Singleton (싱글톤)**: 애플리케이션 전체에서 하나의 인스턴스
```csharp
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
```

- 앱이 시작될 때 한 번 생성되고, 종료될 때까지 유지됩니다
- 모든 요청이 동일한 인스턴스를 공유합니다
- 캐시, 구성, 상태 없는 서비스에 적합합니다
- 스레드 안전성을 고려해야 합니다

```csharp
public class CacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, object> _cache = new();

    public void Set(string key, object value)
    {
        _cache[key] = value;  // 모든 요청이 공유
    }

    public object Get(string key)
    {
        _cache.TryGetValue(key, out var value);
        return value;
    }
}
```

**생명주기 비교 표**:

| 생명주기 | 생성 시점 | 파괴 시점 | 사용 예 |
|---------|----------|----------|--------|
| Transient | 매 요청마다 | 범위 종료 시 | 상태 없는 가벼운 서비스 |
| Scoped | HTTP 요청당 1회 | 요청 종료 시 | DbContext, 리포지토리 |
| Singleton | 앱 시작 시 1회 | 앱 종료 시 | 캐시, 설정, 로거 |

**실제 예제**:
```csharp
// Singleton: 구성 (변하지 않음)
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Scoped: 데이터베이스 컨텍스트 (요청당)
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Transient: 유틸리티 서비스
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
```

### 생성자 주입 패턴

ASP.NET Core에서 DI를 사용하는 가장 일반적인 방법은 생성자 주입입니다. 클래스의 생성자에서 필요한 의존성을 매개변수로 받습니다.

**기본 패턴**:
```csharp
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;
    private readonly IMapper _mapper;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger,
        IMapper mapper)
    {
        _productService = productService;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching all products");
        var products = await _productService.GetAllAsync();
        var dtos = _mapper.Map<List<ProductDto>>(products);
        return Ok(dtos);
    }
}
```

TypeScript/NestJS와 비교:
```typescript
@Controller('products')
export class ProductsController {
  constructor(
    private readonly productService: ProductService,
    private readonly logger: Logger
  ) {}

  @Get()
  async getAll() {
    this.logger.log('Fetching all products');
    return await this.productService.getAll();
  }
}
```

**여러 레이어에서의 DI**:

```csharp
// Controller
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }
}

// Service Layer
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IPaymentGateway _paymentGateway;

    public OrderService(
        IOrderRepository repository,
        IEmailService emailService,
        IPaymentGateway paymentGateway)
    {
        _repository = repository;
        _emailService = emailService;
        _paymentGateway = paymentGateway;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        // 1. 결제 처리
        var payment = await _paymentGateway.ChargeAsync(dto.Amount);

        // 2. 주문 저장
        var order = new Order { ... };
        await _repository.AddAsync(order);

        // 3. 이메일 발송
        await _emailService.SendOrderConfirmationAsync(order);

        return order;
    }
}

// Repository Layer
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
}
```

DI 컨테이너가 자동으로 의존성 그래프를 해결합니다:
```
OrdersController
    └─ IOrderService → OrderService
        ├─ IOrderRepository → OrderRepository
        │   └─ AppDbContext
        ├─ IEmailService → EmailService
        └─ IPaymentGateway → StripePaymentGateway
```

**선택적 의존성**:
때로는 의존성이 선택적일 수 있습니다. 등록되지 않았을 때 null을 받고 싶다면:

```csharp
public class MyService
{
    private readonly IOptionalService? _optionalService;

    public MyService(IOptionalService? optionalService = null)
    {
        _optionalService = optionalService;
    }

    public void DoWork()
    {
        if (_optionalService != null)
        {
            _optionalService.Execute();
        }
        else
        {
            // 대체 로직
        }
    }
}
```

### Keyed Services (.NET 9 신기능)

.NET 8까지는 동일한 인터페이스의 여러 구현체를 등록하기 어려웠습니다. .NET 9의 Keyed Services는 이 문제를 우아하게 해결합니다.

**문제 상황**:
```csharp
// 두 개의 다른 캐시 구현이 필요
public interface ICacheService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
}

public class RedisCache : ICacheService { ... }
public class MemoryCache : ICacheService { ... }

// ❌ 이전 방식: 마지막 등록만 사용됨
builder.Services.AddScoped<ICacheService, RedisCache>();
builder.Services.AddScoped<ICacheService, MemoryCache>();  // 이것만 사용됨
```

**.NET 9 Keyed Services**:
```csharp
// 키로 구분하여 등록
builder.Services.AddKeyedScoped<ICacheService, RedisCache>("redis");
builder.Services.AddKeyedScoped<ICacheService, MemoryCache>("memory");

// 사용
public class ProductService
{
    private readonly ICacheService _redisCache;
    private readonly ICacheService _memoryCache;

    public ProductService(
        [FromKeyedServices("redis")] ICacheService redisCache,
        [FromKeyedServices("memory")] ICacheService memoryCache)
    {
        _redisCache = redisCache;
        _memoryCache = memoryCache;
    }

    public async Task<Product> GetProductAsync(int id)
    {
        // 먼저 메모리 캐시 확인
        var cached = await _memoryCache.GetAsync($"product:{id}");
        if (cached != null)
        {
            return JsonSerializer.Deserialize<Product>(cached);
        }

        // 그 다음 Redis 확인
        cached = await _redisCache.GetAsync($"product:{id}");
        if (cached != null)
        {
            // 메모리 캐시에도 저장
            await _memoryCache.SetAsync($"product:{id}", cached);
            return JsonSerializer.Deserialize<Product>(cached);
        }

        // DB에서 조회...
    }
}
```

**실용적인 예제: 여러 결제 게이트웨이**:
```csharp
public interface IPaymentGateway
{
    Task<PaymentResult> ChargeAsync(decimal amount, string token);
}

public class StripeGateway : IPaymentGateway { ... }
public class PayPalGateway : IPaymentGateway { ... }
public class TossGateway : IPaymentGateway { ... }

// 등록
builder.Services.AddKeyedScoped<IPaymentGateway, StripeGateway>("stripe");
builder.Services.AddKeyedScoped<IPaymentGateway, PayPalGateway>("paypal");
builder.Services.AddKeyedScoped<IPaymentGateway, TossGateway>("toss");

// 동적으로 선택
public class PaymentService
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(
        string provider,
        decimal amount,
        string token)
    {
        // 런타임에 결정
        var gateway = _serviceProvider.GetRequiredKeyedService<IPaymentGateway>(provider);
        return await gateway.ChargeAsync(amount, token);
    }
}

// 사용
var result = await _paymentService.ProcessPaymentAsync("stripe", 100.00m, token);
```

### React Context API vs ASP.NET Core DI

React 개발자라면 Context API로 상태나 서비스를 공유하는 패턴에 익숙할 것입니다.

**React Context API**:
```typescript
const UserContext = createContext<UserService | null>(null);

function App() {
  const userService = new UserService();

  return (
    <UserContext.Provider value={userService}>
      <Dashboard />
    </UserContext.Provider>
  );
}

function Dashboard() {
  const userService = useContext(UserContext);
  // userService 사용
}
```

**ASP.NET Core DI**는 비슷하지만 더 강력합니다:
- 타입 안전성: 컴파일 타임에 의존성 확인
- 생명주기 관리: Singleton, Scoped, Transient 자동 관리
- 중첩 의존성: 자동으로 의존성 그래프 해결
- 테스트 용이성: Mock 객체 주입 간단

```csharp
// 테스트에서 Mock 주입
var mockService = new Mock<IUserService>();
mockService.Setup(s => s.GetUserAsync(1))
    .ReturnsAsync(new User { Id = 1, Name = "Test" });

var controller = new UsersController(mockService.Object);
```

## 3.3 라우팅 시스템

### 컨벤션 기반 라우팅

ASP.NET Core MVC는 전통적으로 컨벤션 기반 라우팅을 사용했습니다. 이는 하나의 중앙 위치에서 모든 라우트를 정의하는 방식입니다.

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

이 패턴은 다음과 같이 해석됩니다:
- `/Products/Details/5` → ProductsController의 Details 액션, id=5
- `/Home/Index` → HomeController의 Index 액션
- `/` → HomeController의 Index 액션 (기본값)

Express.js와 비교:
```javascript
// Express.js
app.get('/:controller/:action/:id?', (req, res) => {
  const controller = req.params.controller || 'home';
  const action = req.params.action || 'index';
  // 동적으로 라우팅...
});
```

하지만 ASP.NET Core API 개발에서는 **특성 기반 라우팅**이 훨씬 더 일반적이고 명확합니다.

### 특성 기반 라우팅(Attribute Routing)

특성 라우팅은 각 컨트롤러와 액션에 직접 라우트를 지정하는 방식입니다. REST API 개발에 최적화되어 있습니다.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // GET: api/products
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new[] { "Product1", "Product2" });
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { id, name = "Product" });
    }

    // GET: api/products/5/reviews
    [HttpGet("{id}/reviews")]
    public IActionResult GetReviews(int id)
    {
        return Ok(new[] { "Review1", "Review2" });
    }

    // POST: api/products
    [HttpPost]
    public IActionResult Create(Product product)
    {
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT: api/products/5
    [HttpPut("{id}")]
    public IActionResult Update(int id, Product product)
    {
        return NoContent();
    }

    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return NoContent();
    }
}
```

이는 Express.js의 명시적 라우팅과 유사합니다:
```javascript
router.get('/products', getAllProducts);
router.get('/products/:id', getProductById);
router.post('/products', createProduct);
router.put('/products/:id', updateProduct);
router.delete('/products/:id', deleteProduct);
```

**라우트 토큰**:
- `[controller]`: 컨트롤러 이름 (Products)
- `[action]`: 액션 메서드 이름
- `{id}`: 경로 매개변수
- `{id?}`: 선택적 매개변수
- `{id:int}`: 타입 제약

```csharp
[Route("api/v{version:int}/products")]  // api/v1/products
[Route("api/products/{id:int:min(1)}")]  // id는 1 이상의 정수
[Route("api/search/{term:alpha}")]  // term은 알파벳만
```

### React Router와의 개념적 유사성

프론트엔드 개발자라면 React Router에 익숙할 것입니다:

```typescript
// React Router
<Routes>
  <Route path="/products" element={<ProductList />} />
  <Route path="/products/:id" element={<ProductDetails />} />
  <Route path="/products/:id/reviews" element={<ProductReviews />} />
</Routes>
```

ASP.NET Core의 라우팅은 서버 사이드 버전입니다:

```csharp
// ASP.NET Core
[Route("products")]
public IActionResult List() { ... }

[Route("products/{id}")]
public IActionResult Details(int id) { ... }

[Route("products/{id}/reviews")]
public IActionResult Reviews(int id) { ... }
```

주요 차이점:
- **서버 vs 클라이언트**: ASP.NET Core는 HTTP 요청을, React Router는 브라우저 URL을 라우팅
- **타입 안정성**: C#은 컴파일 타임에 경로 매개변수 타입을 확인
- **자동 바인딩**: ASP.NET Core는 URL, 쿼리, 본문을 자동으로 모델에 바인딩

### 라우트 제약 조건과 매개변수

라우트 제약 조건은 매개변수가 특정 조건을 만족할 때만 매치되도록 합니다.

**기본 제약 조건**:
```csharp
[HttpGet("{id:int}")]  // 정수만
public IActionResult Get(int id) { }

[HttpGet("{slug:alpha}")]  // 알파벳만
public IActionResult GetBySlug(string slug) { }

[HttpGet("{name:minlength(3):maxlength(50)}")]  // 길이 제한
public IActionResult Search(string name) { }

[HttpGet("{date:datetime}")]  // 날짜 형식
public IActionResult GetByDate(DateTime date) { }

[HttpGet("{guid:guid}")]  // GUID 형식
public IActionResult GetByGuid(Guid guid) { }
```

**복합 제약 조건**:
```csharp
[HttpGet("{id:int:min(1):max(1000)}")]  // 1-1000 범위의 정수
public IActionResult Get(int id) { }

[HttpGet("{year:int:length(4)}/{month:int:range(1,12)}")]
public IActionResult GetArchive(int year, int month) { }
```

**커스텀 제약 조건**:
```csharp
public class SlugConstraint : IRouteConstraint
{
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        if (values.TryGetValue(routeKey, out var value))
        {
            var slug = value?.ToString();
            // slug는 소문자, 숫자, 하이픈만 허용
            return slug != null && Regex.IsMatch(slug, @"^[a-z0-9-]+$");
        }
        return false;
    }
}

// 등록
builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("slug", typeof(SlugConstraint));
});

// 사용
[HttpGet("{slug:slug}")]
public IActionResult GetBySlug(string slug) { }
```

**쿼리 문자열 바인딩**:
```csharp
// GET: api/products?page=1&pageSize=20&sort=name
[HttpGet]
public IActionResult GetAll([FromQuery] int page = 1,
                            [FromQuery] int pageSize = 10,
                            [FromQuery] string? sort = null)
{
    // page, pageSize, sort 자동 바인딩
}

// 또는 DTO 사용
public record ProductQuery(int Page = 1, int PageSize = 10, string? Sort = null);

[HttpGet]
public IActionResult GetAll([FromQuery] ProductQuery query)
{
    // 쿼리 문자열이 자동으로 ProductQuery 객체로 변환
}
```

## 3.4 구성 관리(Configuration)

### appsettings.json: package.json과는 다른 역할

Node.js에서 `package.json`은 의존성과 메타데이터를 정의합니다. ASP.NET Core의 `appsettings.json`은 **애플리케이션 설정**을 저장하는 구성 파일입니다.

**appsettings.json 예제**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;Trusted_Connection=True;"
  },
  "AppSettings": {
    "ApiKey": "your-api-key",
    "MaxUploadSizeMB": 10,
    "EnableFeatureX": true
  },
  "AllowedHosts": "*"
}
```

Node.js의 환경 변수나 config 파일과 유사:
```javascript
// config.js
module.exports = {
  database: {
    host: process.env.DB_HOST || 'localhost',
    port: process.env.DB_PORT || 5432
  },
  apiKey: process.env.API_KEY,
  maxUploadSize: 10 * 1024 * 1024
};
```

### 환경별 구성: Development, Staging, Production

ASP.NET Core는 환경별로 다른 설정 파일을 자동으로 로드합니다.

파일 구조:
```
appsettings.json  (모든 환경의 기본값)
appsettings.Development.json  (개발 환경)
appsettings.Staging.json  (스테이징 환경)
appsettings.Production.json  (프로덕션 환경)
```

**appsettings.json** (기본):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AppSettings": {
    "EnableDetailedErrors": false
  }
}
```

**appsettings.Development.json** (개발용 재정의):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AppSettings": {
    "EnableDetailedErrors": true,
    "UseMockServices": true
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp_Dev;"
  }
}
```

**appsettings.Production.json** (프로덕션):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=MyApp_Prod;User=sa;Password=***;"
  }
}
```

환경 설정:
```bash
# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Development
dotnet run

# Windows PowerShell
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run

# launchSettings.json에서도 설정 가능
```

### 환경 변수와 사용자 시크릿

민감한 정보(API 키, 비밀번호)는 appsettings.json에 저장하면 안 됩니다. 대신 환경 변수나 사용자 시크릿을 사용합니다.

**환경 변수 (프로덕션)**:
```bash
# 환경 변수는 appsettings.json을 재정의
export ConnectionStrings__DefaultConnection="Server=prod;Database=MyApp"
export AppSettings__ApiKey="secret-key"
```

콜론(`:`) 대신 이중 언더스코어(`__`)를 사용하여 중첩된 설정을 표현합니다.

**사용자 시크릿 (개발)**:
개발 환경에서 로컬 머신에만 저장되는 비밀 정보:

```bash
# 사용자 시크릿 초기화
dotnet user-secrets init

# 비밀 추가
dotnet user-secrets set "AppSettings:ApiKey" "my-secret-key"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;..."

# 비밀 목록 확인
dotnet user-secrets list

# 비밀 제거
dotnet user-secrets remove "AppSettings:ApiKey"
```

사용자 시크릿은 `~/.microsoft/usersecrets/<user-secrets-id>/secrets.json`에 저장되며, 프로젝트 디렉토리 밖에 있어 Git에 커밋되지 않습니다.

Node.js의 `.env` 파일과 유사:
```bash
# .env (Node.js)
API_KEY=my-secret-key
DB_HOST=localhost

# dotenv 사용
require('dotenv').config();
console.log(process.env.API_KEY);
```

### Options 패턴: 강타입 구성

구성 값을 문자열로 접근하는 대신, 강타입 클래스로 매핑할 수 있습니다.

**구성 클래스 정의**:
```csharp
public class AppSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public int MaxUploadSizeMB { get; set; }
    public bool EnableFeatureX { get; set; }
    public EmailSettings Email { get; set; } = new();
}

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string From { get; set; } = string.Empty;
}
```

**appsettings.json**:
```json
{
  "AppSettings": {
    "ApiKey": "your-api-key",
    "MaxUploadSizeMB": 10,
    "EnableFeatureX": true,
    "Email": {
      "SmtpServer": "smtp.gmail.com",
      "Port": 587,
      "From": "noreply@myapp.com"
    }
  }
}
```

**등록 및 사용**:
```csharp
// Program.cs에서 등록
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// 컨트롤러에서 사용
public class EmailController : ControllerBase
{
    private readonly AppSettings _settings;
    private readonly ILogger<EmailController> _logger;

    public EmailController(IOptions<AppSettings> options, ILogger<EmailController> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    [HttpPost("send")]
    public IActionResult SendEmail(string to, string message)
    {
        _logger.LogInformation(
            "Sending email via {SmtpServer}:{Port}",
            _settings.Email.SmtpServer,
            _settings.Email.Port
        );

        // 이메일 발송 로직...
        return Ok();
    }
}
```

**IOptions vs IOptionsSnapshot vs IOptionsMonitor**:

- **IOptions<T>**: Singleton, 앱 시작 시 한 번 읽음
- **IOptionsSnapshot<T>**: Scoped, 요청마다 다시 읽음 (파일 변경 감지)
- **IOptionsMonitor<T>**: Singleton, 실시간 변경 감지, 이벤트 알림

```csharp
// 실시간 변경 감지
public class MyService
{
    private readonly IOptionsMonitor<AppSettings> _optionsMonitor;

    public MyService(IOptionsMonitor<AppSettings> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;

        // 변경 감지 이벤트 등록
        _optionsMonitor.OnChange(settings =>
        {
            Console.WriteLine($"Settings changed! New API Key: {settings.ApiKey}");
        });
    }

    public void DoWork()
    {
        // 항상 최신 설정 사용
        var currentSettings = _optionsMonitor.CurrentValue;
    }
}
```

## 3.5 로깅과 모니터링

### 구조화된 로깅(Structured Logging)

전통적인 로깅은 단순 문자열을 출력합니다:
```csharp
// 안 좋은 예
Console.WriteLine($"User {userId} logged in at {DateTime.Now}");
```

구조화된 로깅은 데이터를 구조화하여 나중에 쿼리하고 분석할 수 있게 합니다:
```csharp
// 좋은 예
_logger.LogInformation("User {UserId} logged in at {LoginTime}", userId, DateTime.Now);
```

이 로그는 다음과 같이 JSON으로 출력될 수 있습니다:
```json
{
  "timestamp": "2025-11-12T10:30:00Z",
  "level": "Information",
  "message": "User 123 logged in at 2025-11-12T10:30:00Z",
  "properties": {
    "UserId": 123,
    "LoginTime": "2025-11-12T10:30:00Z"
  }
}
```

**ILogger 사용**:
```csharp
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;

    public UsersController(
        ILogger<UsersController> logger,
        IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        _logger.LogInformation("Fetching user {UserId}", id);

        try
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", id);
                return NotFound();
            }

            _logger.LogDebug("User {UserId} retrieved: {@User}", id, user);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user {UserId}", id);
            return StatusCode(500);
        }
    }
}
```

JavaScript/Node.js와 비교:
```javascript
// Winston or Pino (Node.js)
logger.info('Fetching user', { userId: id });

try {
  const user = await userService.getById(id);
  if (!user) {
    logger.warn('User not found', { userId: id });
    return res.status(404).send();
  }
  logger.debug('User retrieved', { userId: id, user });
  return res.json(user);
} catch (error) {
  logger.error('Error fetching user', { userId: id, error });
  return res.status(500).send();
}
```

### 로그 레벨과 필터링

로그 레벨은 중요도를 나타냅니다:

1. **Trace**: 가장 상세한 메시지, 디버깅용
2. **Debug**: 개발 중 유용한 정보
3. **Information**: 일반적인 흐름 정보
4. **Warning**: 비정상적이지만 예상 가능한 상황
5. **Error**: 오류 발생
6. **Critical**: 심각한 오류, 즉시 조치 필요

**appsettings.json에서 설정**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

**환경별 로그 레벨**:

Development:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "MyApp": "Trace"
    }
  }
}
```

Production:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "MyApp": "Information"
    }
  }
}
```

### Application Insights 통합

Azure Application Insights는 Microsoft의 APM(Application Performance Monitoring) 서비스입니다.

**설치**:
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

**Program.cs**:
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

**appsettings.json**:
```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-key;..."
  }
}
```

이제 모든 로그, 요청, 의존성 호출, 예외가 자동으로 Application Insights로 전송됩니다. Azure Portal에서 실시간으로 모니터링할 수 있습니다.

### Serilog와 서드파티 로깅 프레임워크

Serilog는 가장 인기 있는 .NET 로깅 라이브러리로, 풍부한 싱크(sink)와 강력한 구조화된 로깅을 제공합니다.

**설치**:
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Seq
```

**Program.cs**:
```csharp
using Serilog;

// Serilog 구성
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(new JsonFormatter())
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")  // Seq 서버
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog 사용
    builder.Host.UseSerilog();

    var app = builder.Build();

    // HTTP 요청 로깅
    app.UseSerilogRequestLogging();

    app.MapGet("/", () => "Hello World!");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

Serilog는 Node.js의 Winston이나 Pino와 유사합니다:

```javascript
// Winston (Node.js)
const winston = require('winston');

const logger = winston.createLogger({
  level: 'info',
  format: winston.format.json(),
  transports: [
    new winston.transports.File({ filename: 'error.log', level: 'error' }),
    new winston.transports.File({ filename: 'combined.log' }),
  ],
});
```

## 3.6 실습: 미들웨어 파이프라인 구축

### 커스텀 미들웨어 작성

실습으로 요청 ID를 생성하고 모든 응답에 추가하는 미들웨어를 만들어봅시다.

```csharp
public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestIdMiddleware> _logger;

    public RequestIdMiddleware(RequestDelegate next, ILogger<RequestIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 요청 ID 생성 또는 헤더에서 가져오기
        var requestId = context.Request.Headers["X-Request-ID"].FirstOrDefault()
                        ?? Guid.NewGuid().ToString();

        // HttpContext에 저장
        context.Items["RequestId"] = requestId;

        // 응답 헤더에 추가
        context.Response.Headers["X-Request-ID"] = requestId;

        _logger.LogInformation("Request {RequestId} started", requestId);

        try
        {
            // 다음 미들웨어 실행
            await _next(context);

            _logger.LogInformation(
                "Request {RequestId} completed with status {StatusCode}",
                requestId,
                context.Response.StatusCode
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request {RequestId} failed", requestId);
            throw;
        }
    }
}

// 확장 메서드
public static class RequestIdMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestIdMiddleware>();
    }
}

// 사용
app.UseRequestId();
```

### 로깅 미들웨어 구현

요청과 응답을 상세히 로깅하는 미들웨어:

```csharp
public class DetailedLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DetailedLoggingMiddleware> _logger;

    public DetailedLoggingMiddleware(RequestDelegate next, ILogger<DetailedLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 요청 로깅
        _logger.LogInformation(
            "HTTP {Method} {Path} started",
            context.Request.Method,
            context.Request.Path
        );

        // 요청 본문 읽기 (Body는 한 번만 읽을 수 있으므로 버퍼링 활성화)
        context.Request.EnableBuffering();
        var requestBody = await ReadBodyAsync(context.Request.Body);
        context.Request.Body.Position = 0;  // 다시 처음으로

        _logger.LogDebug("Request body: {RequestBody}", requestBody);

        // 원래 응답 스트림 저장
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            // 다음 미들웨어 실행
            await _next(context);

            // 응답 로깅
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation(
                "HTTP {Method} {Path} completed with {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode
            );

            _logger.LogDebug("Response body: {ResponseBody}", responseText);

            // 원래 스트림에 복사
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static async Task<string> ReadBodyAsync(Stream body)
    {
        body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(body).ReadToEndAsync();
        body.Seek(0, SeekOrigin.Begin);
        return text;
    }
}
```

### 에러 처리 미들웨어

전역 예외 처리 미들웨어:

```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new
        {
            error = new
            {
                message = _env.IsDevelopment() ? exception.Message : "An error occurred",
                type = exception.GetType().Name,
                stackTrace = _env.IsDevelopment() ? exception.StackTrace : null
            }
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsJsonAsync(response);
    }
}
```

### 성능 측정 미들웨어

요청 처리 시간을 측정하는 미들웨어:

```csharp
public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // 느린 요청 경고
            if (elapsedMilliseconds > 1000)
            {
                _logger.LogWarning(
                    "Slow request: {Method} {Path} took {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMilliseconds
                );
            }
            else
            {
                _logger.LogInformation(
                    "{Method} {Path} completed in {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMilliseconds
                );
            }

            // 응답 헤더에 시간 추가
            context.Response.Headers["X-Response-Time-ms"] = elapsedMilliseconds.ToString();
        }
    }
}
```

**전체 파이프라인 구성**:
```csharp
var app = builder.Build();

// 순서가 중요!
app.UseGlobalException();  // 1. 예외 처리
app.UseRequestId();  // 2. 요청 ID
app.UsePerformance();  // 3. 성능 측정
app.UseDetailedLogging();  // 4. 상세 로깅

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

---

## Chapter 3 마무리: 프레임워크의 핵심 이해

축하합니다! Chapter 3를 완료하며 ASP.NET Core의 심장부를 탐험했습니다. 미들웨어 파이프라인으로 요청-응답 흐름을 제어하고, 의존성 주입으로 느슨하게 결합된 코드를 작성하며, 라우팅 시스템으로 복잡한 URL 패턴을 처리하고, 구성 관리와 로깅으로 프로덕션급 애플리케이션의 기반을 다졌습니다.

이제 여러분은 ASP.NET Core가 **어떻게** 작동하는지 깊이 이해하고 있습니다. `builder.Build()`가 서비스 컨테이너를 구성하고, 미들웨어 파이프라인이 요청을 처리하며, 라우팅이 엔드포인트를 찾고, DI가 의존성을 주입하는—전체 흐름을 볼 수 있습니다. 하지만 이 모든 지식을 실제 API 개발에 어떻게 적용할까요?

### 다음 단계: 간결함과 강력함의 만남

**[Chapter 4: Minimal APIs - Node.js Express 개발자를 위한 빠른 시작](./chapter4/index.md)** 에서는 배운 것을 모두 활용하면서도 **Express.js처럼 간결하게** API를 작성합니다.

**컨트롤러 없는 API**: Chapter 2와 3에서 우리는 MVC 패턴과 컨트롤러를 사용했습니다. 하지만 간단한 API에는 과하죠. Minimal APIs는 Express.js의 간결함을 ASP.NET Core에 가져옵니다. `app.MapGet()`, `app.MapPost()`로 라우트를 직접 정의하며, 보일러플레이트를 최소화합니다.

```csharp
// Express.js 스타일
app.MapGet("/api/users/{id}", async (int id, IUserService service) =>
    await service.GetByIdAsync(id));
```

단 3줄의 코드에, Chapter 3에서 배운 DI, 라우팅, 비동기 처리가 모두 들어있습니다.

**강타입의 마법**: Express.js에서는 `req.params.id`를 수동으로 파싱하고 검증해야 합니다. Minimal APIs는 자동으로 타입 변환, 유효성 검사, 모델 바인딩을 수행합니다. `string`을 기대하면 string을, `int`를 기대하면 int를, 복잡한 객체를 기대하면 JSON을 역직렬화합니다—모두 컴파일 타임 타입 안전성과 함께.

**Results 헬퍼**: Express의 `res.json()`, `res.status(404)`처럼, Minimal APIs는 `Results.Ok()`, `Results.NotFound()`, `Results.Problem()` 등의 헬퍼를 제공합니다. 하지만 더 나아가 `TypedResults`를 사용하면 반환 타입이 컴파일 타임에 검증되고, OpenAPI 문서에 자동으로 포함됩니다.

**OpenAPI의 즉시 생성**: Swagger 문서를 수동으로 작성할 필요가 없습니다. 코드에서 타입 정보를 추출하여 자동으로 OpenAPI 스펙을 생성하고, Swagger UI를 제공합니다. 프론트엔드 팀은 즉시 API 문서를 보고 테스트할 수 있습니다.

**완전한 CRUD API**: Chapter 4에서는 완전한 Todo API를 만듭니다. Create, Read, Update, Delete 엔드포인트, 유효성 검사, 에러 처리, 필터링, 페이지네이션—실제 프로덕션 API에 필요한 모든 것을 간결한 Minimal API 스타일로 구현합니다.

Chapter 4를 마치면, 여러분은 Express.js의 간결함과 ASP.NET Core의 강력함을 모두 가진 API를 만들 수 있습니다. 프론트엔드 개발자로서 익숙한 개발 경험에, 백엔드의 타입 안전성, 성능, 엔터프라이즈 기능을 더한 것입니다.

마지막 챕터를 시작할 준비가 되셨나요? [Chapter 4로 이동하세요!](./chapter4/index.md)

---

## 추가 학습 리소스

- [미들웨어 공식 문서](https://docs.microsoft.com/aspnet/core/fundamentals/middleware/)
- [의존성 주입 가이드](https://docs.microsoft.com/aspnet/core/fundamentals/dependency-injection)
- [라우팅 심화](https://docs.microsoft.com/aspnet/core/fundamentals/routing)
- [구성 관리](https://docs.microsoft.com/aspnet/core/fundamentals/configuration/)
- [로깅 가이드](https://docs.microsoft.com/aspnet/core/fundamentals/logging/)
