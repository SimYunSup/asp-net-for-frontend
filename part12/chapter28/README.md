# Chapter 28: 모범 사례 종합

## 프로덕션 수준의 코드를 위한 체크리스트

전자상거래 플랫폼을 구축했습니다. 기능은 완벽하게 작동하고, 테스트도 통과하며, 배포도 성공했습니다. 하지만 "작동하는 코드"와 "프로덕션 수준의 코드" 사이에는 중요한 차이가 있습니다. 프로덕션 코드는 단순히 기능을 수행하는 것을 넘어, **유지보수 가능하고, 안전하며, 확장 가능하고, 팀에서 협업하기 좋은** 코드입니다.

이 챕터는 지금까지 배운 모든 것의 종합입니다. 개별 기술을 넘어, 어떻게 그것들을 **올바르게** 사용하는지—모범 사례, 안티패턴, 트레이드오프—를 배웁니다.

## 코드 품질

### StyleCop과 Roslyn Analyzers

일관된 코딩 스타일은 팀 협업의 기본입니다:

```xml
<!-- .editorconfig -->
root = true

[*.cs]
# 들여쓰기
indent_style = space
indent_size = 4

# 명명 규칙
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.severity = warning
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.symbols = interface
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.style = begins_with_i

# 코드 스타일
csharp_prefer_braces = true:warning
csharp_using_directive_placement = outside_namespace:warning

# 불필요한 using 제거
dotnet_diagnostic.IDE0005.severity = warning

# var 사용 규칙
csharp_style_var_for_built_in_types = false:warning
csharp_style_var_when_type_is_apparent = true:suggestion

# Null 검사
csharp_style_conditional_delegate_call = true:warning
```

**.csproj에 Analyzers 추가:**

```xml
<ItemGroup>
  <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556" />
  <PackageReference Include="SonarAnalyzer.CSharp" Version="9.16.0.82469" />
  <PackageReference Include="Roslynator.Analyzers" Version="4.7.0" />
</ItemGroup>

<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>
```

### 코드 리뷰 체크리스트

**기능:**
- [ ] 요구사항을 정확히 구현했는가?
- [ ] 엣지 케이스를 처리하는가?
- [ ] 에러 처리가 적절한가?

**테스트:**
- [ ] 단위 테스트가 있는가?
- [ ] 테스트 커버리지가 충분한가?
- [ ] 테스트 이름이 명확한가?

**보안:**
- [ ] 입력 유효성 검사가 있는가?
- [ ] SQL Injection 위험이 없는가?
- [ ] 인증/권한 검사가 올바른가?
- [ ] 비밀 정보가 코드에 하드코딩되지 않았는가?

**성능:**
- [ ] N+1 쿼리가 없는가?
- [ ] 불필요한 메모리 할당이 없는가?
- [ ] 비동기 I/O를 사용하는가?

**유지보수성:**
- [ ] 코드가 읽기 쉬운가?
- [ ] SOLID 원칙을 따르는가?
- [ ] 적절한 추상화 수준인가?
- [ ] 주석이 "왜"를 설명하는가 (무엇이 아닌)?

## 보안 모범 사례

### OWASP Top 10 대응

**1. Broken Access Control**

```csharp
// ❌ 나쁜 예: 권한 검사 없음
[HttpGet("{id}")]
public async Task<IActionResult> GetOrder(int id)
{
    var order = await _context.Orders.FindAsync(id);
    return Ok(order); // 다른 사용자의 주문도 조회 가능!
}

// ✅ 좋은 예: 소유권 검증
[HttpGet("{id}")]
[Authorize]
public async Task<IActionResult> GetOrder(int id)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var order = await _context.Orders.FindAsync(id);

    if (order == null)
        return NotFound();

    if (order.CustomerId != userId && !User.IsInRole("Admin"))
        return Forbid();

    return Ok(order);
}
```

**2. SQL Injection 방지**

```csharp
// ❌ 나쁜 예: 문자열 연결
var query = $"SELECT * FROM Products WHERE Name LIKE '%{searchTerm}%'"; // SQL Injection 위험!

// ✅ 좋은 예: 파라미터화된 쿼리
var products = await _context.Products
    .Where(p => p.Name.Contains(searchTerm))
    .ToListAsync();

// 또는 FromSqlRaw 사용 시
var products = await _context.Products
    .FromSqlRaw("SELECT * FROM Products WHERE Name LIKE {0}", $"%{searchTerm}%")
    .ToListAsync();
```

**3. XSS (Cross-Site Scripting) 방지**

```csharp
// Razor는 자동으로 HTML 인코딩
<p>@Model.UserInput</p> <!-- 안전 -->

<!-- 명시적으로 인코딩 비활성화 (주의!) -->
<p>@Html.Raw(Model.TrustedHtml)</p> <!-- 신뢰할 수 있는 소스만 -->

// API에서는 Content Security Policy 헤더 설정
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});
```

**4. 비밀 정보 관리**

```csharp
// ❌ 나쁜 예: appsettings.json에 비밀 저장
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod;Database=eshop;User=sa;Password=SuperSecret123;"
  }
}

// ✅ 좋은 예: Azure Key Vault 또는 환경 변수
// Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential()
);

// 또는 환경 변수
var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

// User Secrets (개발 환경)
dotnet user-secrets set "Stripe:ApiKey" "sk_test_..."
```

### Rate Limiting

```csharp
// .NET 7+ 내장 Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

app.UseRateLimiter();

// 엔드포인트별 제한
[EnableRateLimiting("fixed")]
[HttpPost("login")]
public async Task<IActionResult> Login(LoginRequest request)
{
    // 로그인 로직
}
```

## 성능 모범 사례 체크리스트

**데이터베이스:**
- [ ] `AsNoTracking()` 사용 (읽기 전용 쿼리)
- [ ] N+1 쿼리 해결 (`Include` 또는 프로젝션)
- [ ] 적절한 인덱스 추가
- [ ] 페이지네이션 구현 (대용량 데이터)
- [ ] Connection pooling 활성화

**비동기:**
- [ ] I/O 작업은 모두 `async/await`
- [ ] `.Result` 또는 `.Wait()` 사용 안 함
- [ ] `ValueTask` 고려 (핫패스)

**캐싱:**
- [ ] 자주 조회되는 데이터 캐싱
- [ ] 캐시 무효화 전략 명확히
- [ ] 분산 캐시 (Redis) 활용

**메모리:**
- [ ] `using` 문 사용 (IDisposable)
- [ ] 대용량 컬렉션은 스트리밍 (`IAsyncEnumerable`)
- [ ] 불필요한 객체 할당 최소화

## API 설계 모범 사례

### RESTful 원칙

```csharp
// ✅ 리소스 중심 URL
GET    /api/products          // 상품 목록
GET    /api/products/{id}     // 상품 상세
POST   /api/products          // 상품 생성
PUT    /api/products/{id}     // 상품 전체 수정
PATCH  /api/products/{id}     // 상품 부분 수정
DELETE /api/products/{id}     // 상품 삭제

// 중첩 리소스
GET    /api/products/{id}/reviews
POST   /api/products/{id}/reviews

// ❌ 동사 사용 안 함
POST /api/createProduct
GET  /api/getAllProducts
```

### Versioning

```csharp
// URL 버저닝
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public IActionResult GetV1()
    {
        // V1 응답
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    public IActionResult GetV2()
    {
        // V2 응답 (새 필드 추가 등)
    }
}

// Program.cs
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
```

### API 문서화

```csharp
// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EShop API",
        Version = "v1",
        Description = "E-Commerce Platform API",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@eshop.com"
        }
    });

    // XML 주석 포함
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // JWT 인증 추가
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
});

// 컨트롤러에 XML 주석
/// <summary>
/// 상품 목록을 조회합니다.
/// </summary>
/// <param name="categoryId">카테고리 ID (선택적)</param>
/// <param name="pageNumber">페이지 번호 (기본값: 1)</param>
/// <returns>페이지네이션된 상품 목록</returns>
/// <response code="200">성공</response>
/// <response code="400">잘못된 요청</response>
[HttpGet]
[ProducesResponseType(typeof(PaginatedList<ProductDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> GetProducts(int? categoryId, int pageNumber = 1)
{
    // ...
}
```

## 에러 처리 전략

### 일관된 에러 응답

```csharp
// 에러 응답 DTO
public record ApiError
{
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public int Status { get; init; }
    public string? Detail { get; init; }
    public Dictionary<string, string[]>? Errors { get; init; }
    public string TraceId { get; init; } = string.Empty;
}

// 전역 예외 처리 미들웨어
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var (status, title, detail) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found", exception.Message),
            ValidationException => (StatusCodes.Status400BadRequest, "Validation error", exception.Message),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error", "An error occurred while processing your request")
        };

        var response = new ApiError
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = title,
            Status = status,
            Detail = detail,
            TraceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;

        await context.Response.WriteAsJsonAsync(response);
    }
}
```

## 테스팅 전략

### 테스팅 피라미드 비율

```
E2E Tests (5-10%)           [==]
Integration Tests (20-30%)  [======]
Unit Tests (60-70%)         [==============]
```

**무엇을 테스트할까?**

**단위 테스트 우선순위:**
1. 비즈니스 로직 (도메인 엔티티, Value Objects)
2. Command/Query 핸들러
3. 검증 로직 (Validators)
4. 복잡한 알고리즘

**통합 테스트 우선순위:**
1. API 엔드포인트 (주요 플로우)
2. 데이터베이스 작업
3. 외부 서비스 통합

**E2E 테스트 우선순위:**
1. 핵심 사용자 플로우 (주문, 결제)
2. 중요한 비즈니스 프로세스

## 문서화

### README.md 구조

```markdown
# EShop - E-Commerce Platform

간결한 설명

## Features

- 주요 기능 목록
- Bullet points

## Tech Stack

- .NET 9
- PostgreSQL
- Redis
- Stripe

## Getting Started

### Prerequisites

- .NET 9 SDK
- Docker Desktop

### Installation

\`\`\`bash
git clone https://github.com/username/eshop.git
cd eshop
dotnet restore
\`\`\`

### Configuration

\`\`\`bash
cp appsettings.example.json appsettings.json
# Edit appsettings.json
\`\`\`

### Running

\`\`\`bash
docker-compose up -d
dotnet run --project EShop.API
\`\`\`

## Architecture

- Clean Architecture 다이어그램
- 폴더 구조 설명

## API Documentation

Swagger: http://localhost:5000/swagger

## Testing

\`\`\`bash
dotnet test
\`\`\`

## Deployment

Docker, Kubernetes 배포 가이드

## Contributing

기여 가이드라인

## License

MIT
```

## DevOps 모범 사례

### Infrastructure as Code

```hcl
# Terraform (Azure)
resource "azurerm_app_service_plan" "eshop" {
  name                = "eshop-plan"
  location            = azurerm_resource_group.eshop.location
  resource_group_name = azurerm_resource_group.eshop.name
  kind                = "Linux"
  reserved            = true

  sku {
    tier = "Standard"
    size = "S1"
  }
}

resource "azurerm_app_service" "eshop_api" {
  name                = "eshop-api"
  location            = azurerm_resource_group.eshop.location
  resource_group_name = azurerm_resource_group.eshop.name
  app_service_plan_id = azurerm_app_service_plan.eshop.id

  site_config {
    linux_fx_version = "DOCKER|myusername/eshop:latest"
    always_on        = true
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
    "ConnectionStrings__DefaultConnection" = data.azurerm_key_vault_secret.db_connection.value
  }
}
```

### 환경별 구성

```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=eshop_dev"
  }
}

// appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
  // ConnectionStrings는 환경 변수에서
}
```

## 모니터링 골든 시그널

**1. Latency (지연 시간)**
- P50, P90, P95, P99 추적
- 목표: P95 < 200ms

**2. Traffic (트래픽)**
- 초당 요청 수 (RPS)
- 동시 연결 수

**3. Errors (에러율)**
- 4xx, 5xx 응답 비율
- 목표: 에러율 < 1%

**4. Saturation (포화도)**
- CPU, 메모리, 디스크 사용률
- 목표: CPU < 70%, 메모리 < 80%

## 팀 협업

### Git 브랜치 전략

```
main (프로덕션)
  └─ develop (개발)
      ├─ feature/add-payment-integration
      ├─ feature/user-reviews
      └─ bugfix/order-total-calculation
```

**Commit Message 규칙:**

```
feat: Add Stripe payment integration
fix: Correct order total calculation
docs: Update API documentation
test: Add unit tests for OrderService
refactor: Extract payment logic to separate service
perf: Optimize product query with indexes
```

## 최종 체크리스트

프로덕션 배포 전:

**기능:**
- [ ] 모든 요구사항 구현 완료
- [ ] 엣지 케이스 처리
- [ ] 에러 처리 완비

**보안:**
- [ ] 입력 유효성 검사
- [ ] 인증/권한 검증
- [ ] 비밀 정보 보호 (Key Vault)
- [ ] HTTPS 강제
- [ ] CORS 설정
- [ ] Rate limiting

**성능:**
- [ ] 부하 테스트 완료
- [ ] 응답 시간 < 200ms (P95)
- [ ] 캐싱 전략 구현
- [ ] 데이터베이스 최적화

**품질:**
- [ ] 테스트 커버리지 > 80%
- [ ] 모든 테스트 통과
- [ ] 코드 리뷰 완료
- [ ] Static analysis 통과

**모니터링:**
- [ ] Application Insights 설정
- [ ] 로깅 구성
- [ ] 알림 설정
- [ ] 헬스 체크 구현

**문서:**
- [ ] README.md 작성
- [ ] API 문서 (Swagger)
- [ ] 배포 가이드
- [ ] 아키텍처 다이어그램

**배포:**
- [ ] CI/CD 파이프라인 구성
- [ ] 롤백 계획 수립
- [ ] 데이터베이스 마이그레이션 전략
- [ ] 환경 변수 설정

## 마무리

이제 여러분은 단순히 "작동하는 코드"를 넘어, **프로덕션 수준의 코드**를 작성할 수 있습니다. 모범 사례는 처음에는 번거롭게 느껴질 수 있지만, 시간이 지나면서 그 가치를 깨닫게 됩니다. 버그가 적고, 유지보수가 쉬우며, 팀원들이 이해하기 쉬운 코드—이것이 진정한 전문가의 코드입니다.

**기억하세요:**
- 측정하지 않으면 개선할 수 없다
- 보안은 선택이 아닌 필수다
- 테스트는 문서이자 안전망이다
- 간단한 것이 복잡한 것을 이긴다
- 팀을 위한 코드를 작성하라

여러분의 ASP.NET Core 여정을 축하합니다!
