---
title: "Chapter 19 - 서버 사이드 상태 관리"
---

# Chapter 19: 서버 사이드 상태 관리

## 상태의 복잡성: 프론트엔드를 넘어서

프론트엔드 개발자로서 여러분은 상태 관리에 익숙합니다. React의 `useState`, `useReducer`, Redux의 store, MobX의 observable, Zustand의 간결함—각각은 컴포넌트 간 상태 공유 문제를 해결합니다. 사용자가 입력한 값, API에서 가져온 데이터, UI의 현재 모드... 이 모든 것이 상태입니다.

하지만 서버의 상태는 근본적으로 다릅니다. 단일 사용자가 아닌 **수천 명의 동시 사용자**가 상태를 공유하며, 단일 브라우저 탭이 아닌 **여러 서버 인스턴스**가 상태를 관리합니다. 페이지를 새로고침해도 사라지지 않는 **영속적 상태**가 있으며, 요청이 끝나면 즉시 폐기되는 **일시적 상태**도 있습니다.

이 챕터는 서버 사이드 상태 관리의 모든 측면을 다룹니다. 의존성 주입의 세 가지 수명 주기, 세션과 JWT의 철학적 차이, 다계층 캐싱 전략, 그리고 .NET 9의 HybridCache가 이 모든 것을 어떻게 단순화하는지 배웁니다.

## Part 1: 의존성 주입 수명 주기 - 상태의 범위

### ASP.NET Core 의존성 주입의 세 가지 수명

ASP.NET Core의 의존성 주입(DI) 컨테이너는 세 가지 수명 주기를 제공합니다: **Transient**, **Scoped**, **Singleton**. 각각은 서로 다른 상태 관리 전략을 나타내며, 잘못 선택하면 메모리 누수, 동시성 문제, 성능 저하로 이어질 수 있습니다.

```csharp
// Program.cs에서 서비스 등록
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
```

### Transient: 매번 새로운 인스턴스

**Transient** 서비스는 요청될 때마다 새로운 인스턴스가 생성됩니다.

```csharp
public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body);
}

public class EmailSender : IEmailSender
{
    private readonly SmtpClient _smtpClient;

    public EmailSender(IConfiguration configuration)
    {
        // 매번 새 SmtpClient 생성
        _smtpClient = new SmtpClient(configuration["Smtp:Host"]);
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        await _smtpClient.SendMailAsync(
            new MailMessage("noreply@example.com", to, subject, body));
    }
}

// Program.cs
builder.Services.AddTransient<IEmailSender, EmailSender>();
```

```csharp
// 사용 예시
public class OrderController : ControllerBase
{
    // 이 두 인스턴스는 서로 다름
    private readonly IEmailSender _emailSender1;
    private readonly IEmailSender _emailSender2;

    public OrderController(IEmailSender emailSender1, IEmailSender emailSender2)
    {
        _emailSender1 = emailSender1;
        _emailSender2 = emailSender2; // 새로운 인스턴스!
    }
}
```

**언제 사용하는가:**
- 상태를 가지지 않는 경량 서비스
- 짧은 수명의 작업 (이메일 발송, 데이터 변환, 유효성 검사)
- 스레드 안전성이 보장되지 않는 서비스

**장점:**
- 스레드 안전: 각 사용처가 독립적인 인스턴스를 가짐
- 단순함: 동시성 걱정 없음

**단점:**
- 메모리 오버헤드: 매번 인스턴스 생성 및 GC
- 성능: 생성 비용이 큰 객체에는 부적합

### Scoped: 요청당 하나의 인스턴스

**Scoped** 서비스는 HTTP 요청마다 하나의 인스턴스가 생성되고, 요청이 끝나면 폐기됩니다. 같은 요청 내에서는 같은 인스턴스가 재사용됩니다.

```csharp
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}

// Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
// DbContext는 기본적으로 Scoped
```

```csharp
// 사용 예시
public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        var order = new Order { /* ... */ };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }
}

public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly AppDbContext _context;

    public OrderController(OrderService orderService, AppDbContext context)
    {
        _orderService = orderService;
        _context = context; // OrderService의 context와 같은 인스턴스!
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    {
        var order = await _orderService.CreateOrderAsync(dto);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }
}
```

**언제 사용하는가:**
- 요청 단위로 상태를 유지해야 하는 서비스
- 데이터베이스 컨텍스트 (Entity Framework Core)
- 현재 사용자 정보
- 요청별 로깅 컨텍스트

**장점:**
- 요청 내 일관성: 같은 DbContext로 모든 작업 수행
- 자동 폐기: 요청 끝에 Dispose() 자동 호출
- 적절한 리소스 관리

**단점:**
- 백그라운드 작업에 사용 불가: Scoped는 HTTP 요청 컨텍스트에 의존
- Singleton에서 Scoped를 주입할 수 없음 (수명 주기 불일치)

### Singleton: 애플리케이션당 하나의 인스턴스

**Singleton** 서비스는 애플리케이션이 시작될 때 한 번 생성되고, 종료될 때까지 유지됩니다. 모든 요청과 모든 사용자가 같은 인스턴스를 공유합니다.

```csharp
public class AppSettingsService
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, string> _settings;

    public AppSettingsService(IConfiguration configuration)
    {
        _configuration = configuration;

        // 애플리케이션 시작 시 한 번만 로드
        _settings = _configuration.GetSection("AppSettings")
            .Get<Dictionary<string, string>>() ?? new();
    }

    public string GetSetting(string key)
    {
        return _settings.TryGetValue(key, out var value) ? value : string.Empty;
    }
}

// Program.cs
builder.Services.AddSingleton<AppSettingsService>();
```

**스레드 안전성 보장:**

Singleton은 여러 요청이 동시에 접근할 수 있으므로, 스레드 안전해야 합니다.

```csharp
public class CacheService
{
    private readonly ConcurrentDictionary<string, object> _cache = new();

    public void Set(string key, object value)
    {
        _cache[key] = value; // ConcurrentDictionary는 스레드 안전
    }

    public object? Get(string key)
    {
        _cache.TryGetValue(key, out var value);
        return value;
    }
}

// Program.cs
builder.Services.AddSingleton<CacheService>();
```

**주의: Singleton에서 Scoped 서비스를 사용하지 마세요!**

```csharp
// ❌ 잘못된 예: Singleton이 Scoped를 캡처
public class BadSingletonService
{
    private readonly AppDbContext _context; // Scoped!

    public BadSingletonService(AppDbContext context)
    {
        _context = context; // 첫 번째 요청의 DbContext가 영구 저장됨!
    }
}

// ✅ 올바른 예: IServiceProvider로 필요시 스코프 생성
public class GoodSingletonService
{
    private readonly IServiceProvider _serviceProvider;

    public GoodSingletonService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DoWorkAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 이제 안전하게 사용
        var users = await context.Users.ToListAsync();
    }
}
```

**언제 사용하는가:**
- 상태가 없거나 불변인 서비스
- 설정, 캐시, 로거
- 리소스 풀 (DB 연결 풀)
- 비용이 큰 초기화가 필요한 서비스

**장점:**
- 성능: 한 번만 생성, 메모리 효율적
- 전역 상태: 모든 곳에서 같은 인스턴스 접근

**단점:**
- 스레드 안전성: 수동으로 보장해야 함
- 메모리 누수 위험: 폐기되지 않음
- 테스트 어려움: 전역 상태는 테스트 격리를 방해

### 수명 주기 선택 가이드

| 특성 | Transient | Scoped | Singleton |
|------|-----------|--------|-----------|
| 인스턴스 수 | 요청마다 | 요청당 1개 | 앱당 1개 |
| 수명 | 짧음 | 요청 동안 | 앱 수명 |
| 상태 | 없음 | 요청 범위 | 전역 |
| 스레드 안전 | 자동 | 자동 (요청별) | 수동 |
| 메모리 | 높음 | 중간 | 낮음 |
| 성능 | 생성 오버헤드 | 균형 | 최고 |
| 사용 예 | 유틸리티 | DbContext | 캐시, 설정 |

## Part 2: 세션 관리 - 사용자 상태의 영속성

### 세션이란 무엇인가?

**세션**은 특정 사용자와 연관된 서버 사이드 상태로, 여러 HTTP 요청에 걸쳐 유지됩니다. 로그인 정보, 장바구니, 사용자 설정, 임시 데이터—이런 것들이 세션에 저장됩니다.

HTTP는 상태 비저장(stateless) 프로토콜입니다. 각 요청은 독립적이며, 서버는 이전 요청을 기억하지 않습니다. 세션은 이 제약을 극복하여, 서버가 사용자를 "기억"하게 합니다.

### In-Memory 세션: 단일 서버 환경

가장 단순한 세션은 서버 메모리에 저장됩니다.

```csharp
// Program.cs
builder.Services.AddDistributedMemoryCache(); // 세션을 위한 메모리 스토어
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30분 동안 활동 없으면 만료
    options.Cookie.HttpOnly = true; // XSS 방지
    options.Cookie.IsEssential = true; // GDPR 예외
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS만
});

var app = builder.Build();

app.UseSession(); // 세션 미들웨어 추가
```

```csharp
// 세션 사용
public class CartController : ControllerBase
{
    [HttpPost("add")]
    public IActionResult AddToCart(int productId, int quantity)
    {
        // 세션에서 장바구니 가져오기
        var cart = HttpContext.Session.GetString("Cart");
        var cartItems = string.IsNullOrEmpty(cart)
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(cart);

        // 제품 추가
        cartItems.Add(new CartItem { ProductId = productId, Quantity = quantity });

        // 세션에 저장
        HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));

        return Ok();
    }

    [HttpGet]
    public IActionResult GetCart()
    {
        var cart = HttpContext.Session.GetString("Cart");
        if (string.IsNullOrEmpty(cart))
            return Ok(new List<CartItem>());

        var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cart);
        return Ok(cartItems);
    }

    [HttpPost("clear")]
    public IActionResult ClearCart()
    {
        HttpContext.Session.Remove("Cart");
        return Ok();
    }
}
```

**헬퍼 확장 메서드:**

```csharp
public static class SessionExtensions
{
    public static void SetObject<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T? GetObject<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}

// 사용
HttpContext.Session.SetObject("Cart", cartItems);
var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
```

**문제점:**

In-Memory 세션은 단일 서버에서만 작동합니다. 로드 밸런서 뒤에 여러 서버가 있다면, 사용자가 서버 A에서 로그인하고 다음 요청이 서버 B로 가면 세션을 찾을 수 없습니다.

### Distributed 세션: Redis로 서버 간 공유

**분산 세션**은 모든 서버가 공유하는 외부 저장소(주로 Redis)에 세션을 저장합니다.

```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "MyApp_";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseSession();
```

이제 세션 데이터는 Redis에 저장되며, 모든 서버가 접근할 수 있습니다. 사용자가 어느 서버에 연결되든, 세션을 찾을 수 있습니다.

**성능 고려사항:**

Redis는 네트워크를 거치므로, 메모리 세션보다 느립니다. 세션 접근을 최소화하고, 필요한 데이터만 저장하세요.

```csharp
// ❌ 매 요청마다 세션 읽기
public IActionResult Index()
{
    var user = HttpContext.Session.GetObject<User>("User"); // 네트워크 왕복
    var settings = HttpContext.Session.GetObject<Settings>("Settings"); // 또 다른 왕복
    // ...
}

// ✅ 한 번만 읽고 캐시
public IActionResult Index()
{
    var sessionData = HttpContext.Session.GetObject<SessionData>("SessionData");
    // sessionData에 User, Settings 등이 모두 포함
}
```

### 세션 vs JWT: 두 가지 철학

세션과 JWT는 근본적으로 다른 접근 방식입니다.

#### 세션 방식: 서버가 진실의 소스

```csharp
// 로그인
[HttpPost("login")]
public async Task<IActionResult> Login(LoginDto dto)
{
    var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
    if (user == null)
        return Unauthorized();

    // 세션에 사용자 정보 저장
    HttpContext.Session.SetObject("User", new UserSessionData
    {
        Id = user.Id,
        Email = user.Email,
        Roles = user.Roles
    });

    return Ok();
}

// 인증 필요한 엔드포인트
[HttpGet("profile")]
public IActionResult GetProfile()
{
    var user = HttpContext.Session.GetObject<UserSessionData>("User");
    if (user == null)
        return Unauthorized();

    return Ok(user);
}

// 로그아웃
[HttpPost("logout")]
public IActionResult Logout()
{
    HttpContext.Session.Clear(); // 세션 즉시 삭제
    return Ok();
}
```

**장점:**
- **즉시 무효화**: 세션을 삭제하면 다음 요청은 즉시 실패
- **서버가 완전 제어**: 세션 데이터를 언제든 수정 가능
- **작은 쿠키**: 세션 ID만 전송 (수십 바이트)

**단점:**
- **상태 저장**: 서버/Redis 리소스 필요
- **확장성**: 분산 세션 인프라 필요 (Redis, Sticky Session)

#### JWT 방식: 클라이언트가 상태를 소유

```csharp
// 로그인
[HttpPost("login")]
public async Task<IActionResult> Login(LoginDto dto)
{
    var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
    if (user == null)
        return Unauthorized();

    // JWT 생성
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, string.Join(",", user.Roles))
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(15), // 짧은 수명
        signingCredentials: creds);

    return Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token)
    });
}

// 인증 필요한 엔드포인트
[Authorize] // JWT 미들웨어가 자동 검증
[HttpGet("profile")]
public IActionResult GetProfile()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var email = User.FindFirst(ClaimTypes.Email)?.Value;

    return Ok(new { userId, email });
}
```

**장점:**
- **상태 비저장**: 서버가 아무것도 저장하지 않음
- **확장성**: 수평 확장 간단 (어느 서버든 검증 가능)
- **마이크로서비스**: 토큰만 있으면 모든 서비스 인증 가능

**단점:**
- **즉시 무효화 불가**: 토큰은 만료 시간까지 유효
- **토큰 크기**: 모든 클레임이 포함 (수백 바이트)
- **Refresh Token 복잡성**: Access Token + Refresh Token 패턴 필요

### Refresh Token 패턴: JWT의 보안 강화

JWT의 "즉시 무효화 불가" 문제를 해결하기 위해, **짧은 Access Token + 긴 Refresh Token** 패턴을 사용합니다.

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login(LoginDto dto)
{
    var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
    if (user == null)
        return Unauthorized();

    // Access Token (15분)
    var accessToken = GenerateAccessToken(user);

    // Refresh Token (7일)
    var refreshToken = GenerateRefreshToken();

    // Refresh Token을 데이터베이스에 저장 (즉시 무효화 가능)
    await _context.RefreshTokens.AddAsync(new RefreshToken
    {
        Token = refreshToken,
        UserId = user.Id,
        ExpiresAt = DateTime.UtcNow.AddDays(7)
    });
    await _context.SaveChangesAsync();

    return Ok(new
    {
        accessToken,
        refreshToken
    });
}

[HttpPost("refresh")]
public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
{
    // Refresh Token 검증
    var refreshToken = await _context.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken && !rt.IsRevoked);

    if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
        return Unauthorized();

    var user = await _context.Users.FindAsync(refreshToken.UserId);

    // 새 Access Token 발급
    var newAccessToken = GenerateAccessToken(user);

    return Ok(new { accessToken = newAccessToken });
}

[HttpPost("logout")]
public async Task<IActionResult> Logout(string refreshToken)
{
    // Refresh Token 무효화
    var token = await _context.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

    if (token != null)
    {
        token.IsRevoked = true;
        await _context.SaveChangesAsync();
    }

    return Ok();
}
```

이제 Access Token은 15분만 유효하므로, 탈취되어도 피해가 제한됩니다. Refresh Token은 데이터베이스에 저장되므로, 즉시 무효화할 수 있습니다.

## Part 3: 캐싱 전략 - 성능의 배가수

### 왜 캐싱이 필요한가?

데이터베이스 쿼리는 밀리초(ms) 단위지만, 메모리 접근은 나노초(ns) 단위입니다. **1,000,000배의 차이**입니다. 캐싱을 적절히 적용하면:

- 응답 시간 10배~100배 단축
- 데이터베이스 부하 90% 감소
- 서버 비용 절감
- 사용자 경험 개선

하지만 캐싱은 복잡성을 증가시킵니다. "컴퓨터 과학에서 어려운 것은 두 가지: **캐시 무효화**와 이름 짓기."

### Layer 1: Response Caching (HTTP 캐시)

브라우저나 CDN이 응답을 캐싱하도록 HTTP 헤더를 설정합니다.

```csharp
// Program.cs
builder.Services.AddResponseCaching();

var app = builder.Build();

app.UseResponseCaching();

// 컨트롤러
[ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "id" })]
[HttpGet("{id}")]
public IActionResult GetProduct(int id)
{
    var product = _context.Products.Find(id);
    return Ok(product);
}
```

`Duration = 3600`은 1시간 동안 캐싱합니다. 브라우저는 이 시간 동안 서버에 요청조차 보내지 않습니다.

**ETag를 사용한 조건부 요청:**

```csharp
[HttpGet("{id}")]
public IActionResult GetProduct(int id)
{
    var product = _context.Products.Find(id);
    if (product == null) return NotFound();

    // ETag 생성 (UpdatedAt 기반)
    var etag = $"\"{product.UpdatedAt.Ticks}\"";
    Response.Headers.ETag = etag;

    // 클라이언트의 If-None-Match 헤더 확인
    if (Request.Headers.IfNoneMatch == etag)
    {
        return StatusCode(304); // Not Modified, 본문 없음
    }

    return Ok(product);
}
```

클라이언트가 `If-None-Match: "<etag>"` 헤더를 보내면, ETag가 일치하면 `304 Not Modified`만 응답하고 본문은 보내지 않습니다. 네트워크 대역폭 절감!

### Layer 2: IMemoryCache - 서버 메모리 캐싱

```csharp
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _context;

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product_{id}";

        // 캐시 시도
        if (_cache.TryGetValue(cacheKey, out Product? product))
        {
            return product; // 캐시 히트!
        }

        // 캐시 미스: 데이터베이스 조회
        product = await _context.Products.FindAsync(id);

        if (product != null)
        {
            // 캐시에 저장
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // 5분간 접근 없으면 만료
                .SetAbsoluteExpiration(TimeSpan.FromHours(1)) // 최대 1시간
                .SetSize(1) // 메모리 관리용 크기
                .RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    Console.WriteLine($"Cache evicted: {key}, Reason: {reason}");
                });

            _cache.Set(cacheKey, product, cacheOptions);
        }

        return product;
    }

    public void InvalidateProduct(int id)
    {
        _cache.Remove($"product_{id}");
    }
}

// Program.cs에서 메모리 캐시 크기 제한
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024; // 최대 1024 항목
});
```

**Sliding Expiration vs Absolute Expiration:**

- **Sliding**: 접근할 때마다 만료 시간이 연장됨. "5분간 접근 없으면 만료"
- **Absolute**: 절대 만료 시간. "생성 후 1시간 뒤 무조건 만료"

보통 둘을 조합합니다: "5분간 접근 없으면 만료, 하지만 최대 1시간"

### Layer 3: IDistributedCache - Redis 분산 캐싱

여러 서버가 캐시를 공유합니다.

```csharp
public class ProductService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _context;

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product_{id}";

        // Redis에서 조회
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<Product>(cached);
        }

        // 데이터베이스 조회
        var product = await _context.Products.FindAsync(id);

        if (product != null)
        {
            // Redis에 저장
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }

        return product;
    }
}

// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "MyApp_";
});
```

**성능**: 메모리 캐시보다 느림 (네트워크 왕복), 하지만 서버 간 일관성 보장.

### Layer 4: HybridCache (.NET 9) - 최선의 조합

**HybridCache**는 L1(메모리) + L2(분산 캐시)를 자동으로 관리하며, **Stampede 방지**를 내장합니다.

```bash
dotnet add package Microsoft.Extensions.Caching.Hybrid --prerelease
```

```csharp
public class ProductService
{
    private readonly HybridCache _cache;
    private readonly AppDbContext _context;

    public async Task<Product?> GetProductAsync(int id, CancellationToken token = default)
    {
        return await _cache.GetOrCreateAsync(
            $"product_{id}",
            async cancel =>
            {
                // 팩토리 함수: 캐시 미스 시 실행
                return await _context.Products.FindAsync(new object[] { id }, cancel);
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10), // L2 (Redis) 만료
                LocalCacheExpiration = TimeSpan.FromMinutes(2) // L1 (메모리) 만료
            },
            token);
    }
}

// Program.cs
builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1024 * 1024; // 1MB
    options.MaximumKeyLength = 1024;
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

**작동 방식:**

1. L1 (메모리) 확인 → 히트하면 즉시 반환 (나노초)
2. L1 미스 → L2 (Redis) 확인 → 히트하면 L1에 저장하고 반환 (밀리초)
3. L2 미스 → 팩토리 함수 실행 (데이터베이스 쿼리)
4. 결과를 L1과 L2 모두에 저장

**Stampede 방지:**

여러 요청이 동시에 같은 키를 요청하면, 팩토리 함수는 **한 번만** 실행되고 결과는 모든 요청에 공유됩니다.

```csharp
// 100개 요청이 동시에 product_1을 요청
// HybridCache 없이: 데이터베이스 쿼리 100번
// HybridCache 있으면: 데이터베이스 쿼리 1번, 나머지는 대기 후 결과 공유
```

### 캐시 무효화 전략

**1. Time-based (TTL):**

가장 단순. 일정 시간 후 자동 만료.

```csharp
_cache.Set(key, value, TimeSpan.FromMinutes(10));
```

**적합한 경우**: 정확성보다 성능이 중요하고, 약간 오래된 데이터가 허용되는 경우 (뉴스 피드, 제품 목록).

**2. Event-based:**

데이터 변경 시 명시적으로 캐시 무효화.

```csharp
public async Task UpdateProductAsync(int id, UpdateProductDto dto)
{
    var product = await _context.Products.FindAsync(id);
    // ... 업데이트 로직
    await _context.SaveChangesAsync();

    // 캐시 무효화
    await _cache.RemoveAsync($"product_{id}");
}
```

**적합한 경우**: 정확성이 중요한 경우 (가격, 재고, 사용자 프로필).

**3. Cache Tagging:**

관련 캐시를 그룹으로 무효화.

```csharp
// HybridCache는 태그 지원 (.NET 9)
await _cache.GetOrCreateAsync(
    $"product_{id}",
    factory,
    new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(10),
        Tags = new[] { "products", $"category_{product.CategoryId}" }
    });

// 카테고리의 모든 제품 캐시 무효화
await _cache.RemoveByTagAsync($"category_{categoryId}");
```

## 핵심 교훈

1. **의존성 주입 수명**: Transient (매번), Scoped (요청당), Singleton (앱당)
2. **세션 vs JWT**: 상태 저장 vs 상태 비저장, 즉시 무효화 vs 만료 시간
3. **Refresh Token**: JWT의 보안 강화, Access Token + Refresh Token
4. **다계층 캐싱**: HTTP → 메모리 → 분산 캐시 → HybridCache
5. **캐시 무효화**: Time-based, Event-based, Tag-based

서버 사이드 상태 관리는 프론트엔드보다 복잡하지만, 올바르게 적용하면 성능, 확장성, 유지보수성을 모두 극대화할 수 있습니다. HybridCache 같은 현대적 도구는 복잡성을 크게 줄여줍니다.

다음 챕터에서는 고급 아키텍처 패턴을 배웁니다. Clean Architecture, CQRS, Domain-Driven Design... 대규모 시스템을 설계하는 검증된 패턴들이 기다립니다.
