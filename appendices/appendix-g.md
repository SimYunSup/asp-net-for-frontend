# Appendix G: 용어집

ASP.NET Core 및 .NET 개발에서 자주 사용하는 용어를 프론트엔드 개발자 관점에서 설명합니다.

## A

### Action
컨트롤러의 메서드로, HTTP 요청을 처리합니다.
**JavaScript 비교**: Express.js의 route handler
```csharp
[HttpGet]
public IActionResult GetProducts() { ... }
```

### Action Result
Action이 반환하는 결과입니다. `Ok()`, `NotFound()`, `BadRequest()` 등이 있습니다.
**JavaScript 비교**: Express의 `res.json()`, `res.status()`

### AOT (Ahead-of-Time Compilation)
런타임 이전에 미리 네이티브 코드로 컴파일하는 방식입니다. 빠른 시작 시간과 작은 메모리 사용량이 장점입니다.
**JavaScript 비교**: Bun의 사전 컴파일, Deno의 컴파일 실행 파일

### API Controller
`[ApiController]` 특성이 적용된 컨트롤러로, Web API 개발에 최적화되어 있습니다.
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase { ... }
```

### ASP.NET Core
Microsoft의 오픈소스 크로스플랫폼 웹 프레임워크입니다.
**JavaScript 비교**: Express.js, Fastify, Koa

### Attribute
클래스, 메서드, 프로퍼티에 메타데이터를 추가하는 방법입니다.
**JavaScript 비교**: TypeScript decorators (실험적)
```csharp
[HttpGet]
[Authorize]
[Route("api/products")]
```

### async/await
비동기 프로그래밍을 위한 키워드입니다.
**JavaScript 비교**: JavaScript의 async/await와 동일한 개념
```csharp
public async Task<Product> GetProductAsync(int id)
{
    return await _repository.GetByIdAsync(id);
}
```

## B

### Background Service
백그라운드에서 실행되는 장기 실행 작업입니다.
**JavaScript 비교**: Node.js worker threads, Bull queue
```csharp
public class TimedHostedService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) { ... }
}
```

### Blazor
C#으로 인터랙티브 웹 UI를 구축하는 프레임워크입니다.
**JavaScript 비교**: React, Vue, Svelte
- Blazor Server: Server-side rendering (Next.js SSR과 유사)
- Blazor WebAssembly: Client-side SPA (React SPA와 유사)

### Builder Pattern
객체 생성을 단계적으로 구성하는 패턴입니다.
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
```

## C

### CancellationToken
비동기 작업을 취소하는 메커니즘입니다.
**JavaScript 비교**: AbortController/AbortSignal
```csharp
public async Task<Data> FetchDataAsync(CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    // ...
}
```

### CLR (Common Language Runtime)
.NET 프로그램을 실행하는 런타임 환경입니다.
**JavaScript 비교**: V8 엔진, Node.js 런타임

### Configuration
appsettings.json, 환경 변수 등에서 설정을 읽어옵니다.
**JavaScript 비교**: dotenv, config 패키지
```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

### Controller
MVC 패턴의 C로, 요청을 처리하고 응답을 반환합니다.
**JavaScript 비교**: Express의 router/controller
```csharp
public class ProductsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() { ... }
}
```

### CORS (Cross-Origin Resource Sharing)
교차 출처 리소스 공유를 허용하는 정책입니다.
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin());
});
```

### CQRS (Command Query Responsibility Segregation)
명령(쓰기)과 쿼리(읽기)를 분리하는 패턴입니다.
**JavaScript 비교**: Redux의 actions와 selectors 분리와 유사

### CSRF (Cross-Site Request Forgery)
사이트 간 요청 위조 공격입니다. ASP.NET Core는 자동으로 방어합니다.
```csharp
[ValidateAntiForgeryToken]
public IActionResult Create([FromBody] Product product) { ... }
```

## D

### Data Annotations
모델 검증 및 데이터베이스 매핑을 위한 특성입니다.
```csharp
public class Product
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Range(0, 10000)]
    public decimal Price { get; set; }
}
```

### DbContext
Entity Framework Core에서 데이터베이스 세션을 나타냅니다.
**JavaScript 비교**: Prisma Client, TypeORM DataSource
```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
}
```

### Dependency Injection (DI)
의존성 주입 패턴입니다. ASP.NET Core에 내장되어 있습니다.
**JavaScript 비교**: InversifyJS, tsyringe
```csharp
builder.Services.AddScoped<IProductService, ProductService>();
```

### DTO (Data Transfer Object)
계층 간 데이터 전송을 위한 객체입니다.
```csharp
public class CreateProductDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

## E

### EF Core (Entity Framework Core)
.NET의 ORM(Object-Relational Mapper)입니다.
**JavaScript 비교**: Prisma, TypeORM, Sequelize
```csharp
var products = await _context.Products
    .Where(p => p.Price > 100)
    .ToListAsync();
```

### Endpoint
HTTP 요청을 처리하는 엔드포인트입니다.
```csharp
app.MapGet("/api/products", async (ApplicationDbContext db) =>
    await db.Products.ToListAsync());
```

### Entity
데이터베이스 테이블에 매핑되는 클래스입니다.
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

### Environment
개발, 스테이징, 프로덕션 환경을 구분합니다.
**JavaScript 비교**: NODE_ENV
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
```

### Extension Method
기존 타입에 새 메서드를 추가하는 방법입니다.
```csharp
public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
}
```

## F

### Filter
요청 파이프라인에서 특정 시점에 실행되는 로직입니다.
**JavaScript 비교**: Express middleware
```csharp
public class LogActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { ... }
    public void OnActionExecuted(ActionExecutedContext context) { ... }
}
```

### FluentValidation
유창한 API로 검증 규칙을 정의하는 라이브러리입니다.
**JavaScript 비교**: Joi, Yup
```csharp
public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Price).GreaterThan(0);
    }
}
```

## G

### Generic
타입 매개변수를 사용하는 클래스나 메서드입니다.
**JavaScript 비교**: TypeScript generics
::: v-pre
```csharp
public class Repository<T> where T : class
{
    public async Task<T> GetByIdAsync(int id) { ... }
}
```
:::

### gRPC
고성능 RPC 프레임워크입니다.
**JavaScript 비교**: gRPC-node
```csharp
public class ProductService : Products.ProductsBase
{
    public override async Task<ProductResponse> GetProduct(
        ProductRequest request, ServerCallContext context) { ... }
}
```

## H

### Hangfire
백그라운드 작업 스케줄링 라이브러리입니다.
**JavaScript 비교**: Bull, Agenda
```csharp
BackgroundJob.Enqueue(() => SendEmailAsync(email));
RecurringJob.AddOrUpdate("daily-report", () => GenerateReportAsync(), Cron.Daily);
```

### Health Check
애플리케이션의 상태를 확인하는 엔드포인트입니다.
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

app.MapHealthChecks("/health");
```

### Hosted Service
애플리케이션 수명 주기 동안 실행되는 백그라운드 서비스입니다.
```csharp
public class MyHostedService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) { ... }
    public Task StopAsync(CancellationToken cancellationToken) { ... }
}
```

### HTTP Client
HTTP 요청을 보내는 클라이언트입니다.
**JavaScript 비교**: axios, fetch
```csharp
var response = await _httpClient.GetAsync("https://api.example.com/data");
var data = await response.Content.ReadFromJsonAsync<Data>();
```

## I

### IActionResult
Action의 반환 타입입니다.
```csharp
public IActionResult Get(int id)
{
    if (product == null) return NotFound();
    return Ok(product);
}
```

### IL (Intermediate Language)
.NET의 중간 언어입니다.
**JavaScript 비교**: JavaScript bytecode (V8)

### Interface
클래스가 구현해야 하는 계약입니다.
**JavaScript 비교**: TypeScript interface
```csharp
public interface IProductRepository
{
    Task<Product> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
}
```

### IoC (Inversion of Control)
제어의 역전 원칙입니다. DI 컨테이너가 객체 생성을 관리합니다.

## J

### JIT (Just-in-Time Compilation)
런타임에 IL 코드를 네이티브 코드로 컴파일합니다.
**JavaScript 비교**: V8의 JIT 컴파일

### JSON Serialization
객체를 JSON으로 변환하거나 그 반대입니다.
```csharp
var json = JsonSerializer.Serialize(product);
var product = JsonSerializer.Deserialize<Product>(json);
```

### JWT (JSON Web Token)
인증에 사용되는 토큰 형식입니다.
**JavaScript 비교**: jsonwebtoken 패키지
```csharp
var token = new JwtSecurityToken(
    issuer: "myapp",
    audience: "users",
    claims: claims,
    expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: credentials);
```

## K

### Kestrel
ASP.NET Core의 기본 웹 서버입니다.
**JavaScript 비교**: Node.js의 http 모듈

## L

### Lambda Expression
익명 함수를 만드는 표현식입니다.
**JavaScript 비교**: Arrow functions
```csharp
var evenNumbers = numbers.Where(n => n % 2 == 0);
```

### LINQ (Language Integrated Query)
컬렉션을 쿼리하는 언어 통합 쿼리입니다.
**JavaScript 비교**: Array methods (map, filter, reduce)
```csharp
var result = products
    .Where(p => p.Price > 100)
    .OrderBy(p => p.Name)
    .Select(p => p.Name)
    .ToList();
```

### Logging
애플리케이션 이벤트를 기록합니다.
**JavaScript 비교**: winston, pino
```csharp
_logger.LogInformation("Processing product {ProductId}", productId);
```

## M

### MediatR
중재자 패턴을 구현한 라이브러리입니다. CQRS에 자주 사용됩니다.
::: v-pre
```csharp
public class CreateProductCommand : IRequest<int>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, ...) { ... }
}
```
:::

### Middleware
요청 파이프라인의 구성 요소입니다.
**JavaScript 비교**: Express middleware
```csharp
app.Use(async (context, next) =>
{
    // 요청 처리 전
    await next.Invoke();
    // 응답 처리 후
});
```

### Migration
데이터베이스 스키마 변경을 코드로 관리합니다.
**JavaScript 비교**: Prisma migrations, TypeORM migrations
```bash
dotnet ef migrations add AddProductTable
dotnet ef database update
```

### Minimal API
간결한 구문으로 API를 정의합니다.
**JavaScript 비교**: Express.js의 간결한 라우팅
```csharp
app.MapGet("/api/products", async (ApplicationDbContext db) =>
    await db.Products.ToListAsync());
```

### Model Binding
HTTP 요청 데이터를 메서드 매개변수에 자동으로 바인딩합니다.
```csharp
public IActionResult Create([FromBody] CreateProductDto dto) { ... }
```

### Model Validation
데이터 검증을 자동으로 수행합니다.
```csharp
if (!ModelState.IsValid)
{
    return BadRequest(ModelState);
}
```

## N

### Namespace
코드를 논리적으로 그룹화합니다.
**JavaScript 비교**: ES modules
```csharp
namespace MyApp.Controllers
{
    public class ProductsController { ... }
}
```

### NuGet
.NET의 패키지 관리자입니다.
**JavaScript 비교**: npm, yarn, pnpm
```bash
dotnet add package Newtonsoft.Json
```

### Nullable Reference Types
null 안전성을 제공합니다.
**JavaScript 비교**: TypeScript strict null checks
```csharp
string? nullableString = null;  // OK
string nonNullableString = null;  // 컴파일 경고
```

## O

### ORM (Object-Relational Mapping)
객체와 데이터베이스 테이블을 매핑합니다.
**예**: Entity Framework Core
**JavaScript 비교**: Prisma, TypeORM

### Options Pattern
강타입 구성을 위한 패턴입니다.
```csharp
public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
}

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));
```

## P

### Package
NuGet 패키지입니다.
**JavaScript 비교**: npm package

### Polly
복원력(resilience) 패턴을 구현한 라이브러리입니다.
**JavaScript 비교**: axios-retry
```csharp
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

await retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));
```

### Program.cs
애플리케이션의 진입점입니다.
**JavaScript 비교**: index.js, main.js

### Property
클래스의 프로퍼티입니다.
```csharp
public class Product
{
    public int Id { get; set; }  // Auto-property
    public string Name { get; set; }
}
```

## Q

### Query String
URL의 쿼리 파라미터입니다.
```csharp
[HttpGet]
public IActionResult Get([FromQuery] int page, [FromQuery] int pageSize)
{
    // GET /api/products?page=1&pageSize=10
}
```

## R

### Razor
ASP.NET의 뷰 엔진입니다.
**JavaScript 비교**: JSX, Vue templates
```cshtml
@model Product
<h1>@Model.Name</h1>
<p>Price: @Model.Price</p>
```

### Record
불변 객체를 쉽게 만드는 타입입니다.
```csharp
public record Product(int Id, string Name, decimal Price);
```

### Redis
인메모리 데이터 저장소입니다. 캐싱에 자주 사용됩니다.
**JavaScript 비교**: node-redis, ioredis

### Refit
타입 안전 HTTP 클라이언트 라이브러리입니다.
**JavaScript 비교**: axios with TypeScript
::: v-pre
```csharp
public interface IGitHubApi
{
    [Get("/users/{username}")]
    Task<User> GetUserAsync(string username);
}
```
:::

### Repository Pattern
데이터 액세스 로직을 캡슐화하는 패턴입니다.
```csharp
public interface IProductRepository
{
    Task<Product> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
}
```

### Response Caching
HTTP 응답을 캐싱합니다.
```csharp
[ResponseCache(Duration = 60)]
public IActionResult Get()
{
    return Ok(products);
}
```

### Route
URL 패턴을 정의합니다.
```csharp
[Route("api/[controller]")]
[HttpGet("{id}")]
public IActionResult Get(int id) { ... }
```

## S

### Scoped
요청당 하나의 인스턴스를 생성하는 DI 수명입니다.
```csharp
builder.Services.AddScoped<IProductService, ProductService>();
```

### Seed Data
데이터베이스 초기 데이터입니다.
```csharp
modelBuilder.Entity<Product>().HasData(
    new Product { Id = 1, Name = "Laptop", Price = 1200 }
);
```

### Serilog
구조화된 로깅 라이브러리입니다.
**JavaScript 비교**: winston, pino
```csharp
Log.Information("Processing product {ProductId} for user {UserId}",
    productId, userId);
```

### SignalR
실시간 양방향 통신을 지원합니다.
**JavaScript 비교**: Socket.io
```csharp
public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
```

### Singleton
애플리케이션 수명 동안 하나의 인스턴스만 생성하는 DI 수명입니다.
```csharp
builder.Services.AddSingleton<ICacheService, CacheService>();
```

### Swagger
OpenAPI 문서를 생성하고 시각화합니다.
**JavaScript 비교**: swagger-ui-express
```csharp
builder.Services.AddSwaggerGen();
app.UseSwagger();
app.UseSwaggerUI();
```

## T

### Task
비동기 작업을 나타냅니다.
**JavaScript 비교**: Promise
::: v-pre
```csharp
public async Task<Product> GetProductAsync(int id)
{
    return await _repository.GetByIdAsync(id);
}
```
:::

### TDD (Test-Driven Development)
테스트 주도 개발 방법론입니다.

### Transient
매번 새로운 인스턴스를 생성하는 DI 수명입니다.
```csharp
builder.Services.AddTransient<IEmailService, EmailService>();
```

## U

### Unit of Work
트랜잭션 경계를 관리하는 패턴입니다.
```csharp
public interface IUnitOfWork
{
    IProductRepository Products { get; }
    Task<int> SaveChangesAsync();
}
```

### User Secrets
개발 환경에서 비밀 정보를 안전하게 저장합니다.
**JavaScript 비교**: .env 파일
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=..."
```

## V

### Validation
데이터 검증입니다.
```csharp
[Required]
[StringLength(100)]
public string Name { get; set; }
```

### ValueTask
성능 최적화된 Task 대안입니다.
::: v-pre
```csharp
public ValueTask<Product> GetProductAsync(int id)
{
    if (_cache.TryGetValue(id, out var product))
        return new ValueTask<Product>(product);  // 할당 없음

    return new ValueTask<Product>(FetchFromDbAsync(id));
}
```
:::

## W

### Web API
RESTful API를 구축하는 프레임워크입니다.
**JavaScript 비교**: Express.js REST API

### WebSocket
양방향 통신 프로토콜입니다.
**JavaScript 비교**: ws 패키지

## X

### xUnit
.NET의 테스트 프레임워크입니다.
**JavaScript 비교**: Jest, Mocha
```csharp
[Fact]
public void Add_ShouldReturnSum()
{
    var result = Calculator.Add(2, 3);
    Assert.Equal(5, result);
}
```

### XML Documentation
코드 문서화 주석입니다.
```csharp
/// <summary>
/// Gets a product by ID
/// </summary>
/// <param name="id">The product ID</param>
/// <returns>The product</returns>
public async Task<Product> GetProductAsync(int id) { ... }
```

## 약어 정리

| 약어 | 전체 이름 | 설명 |
|------|----------|------|
| AOT | Ahead-of-Time Compilation | 사전 컴파일 |
| API | Application Programming Interface | 애플리케이션 프로그래밍 인터페이스 |
| CLR | Common Language Runtime | 공통 언어 런타임 |
| CORS | Cross-Origin Resource Sharing | 교차 출처 리소스 공유 |
| CQRS | Command Query Responsibility Segregation | 명령 쿼리 책임 분리 |
| CSRF | Cross-Site Request Forgery | 사이트 간 요청 위조 |
| DI | Dependency Injection | 의존성 주입 |
| DTO | Data Transfer Object | 데이터 전송 객체 |
| EF Core | Entity Framework Core | Entity Framework Core |
| gRPC | gRPC Remote Procedure Calls | gRPC 원격 프로시저 호출 |
| HTTP | Hypertext Transfer Protocol | 하이퍼텍스트 전송 프로토콜 |
| IL | Intermediate Language | 중간 언어 |
| IoC | Inversion of Control | 제어의 역전 |
| JIT | Just-in-Time Compilation | 적시 컴파일 |
| JWT | JSON Web Token | JSON 웹 토큰 |
| LINQ | Language Integrated Query | 언어 통합 쿼리 |
| MVC | Model-View-Controller | 모델-뷰-컨트롤러 |
| ORM | Object-Relational Mapping | 객체 관계 매핑 |
| REST | Representational State Transfer | 표현 상태 전이 |
| RPC | Remote Procedure Call | 원격 프로시저 호출 |
| SPA | Single Page Application | 단일 페이지 애플리케이션 |
| SQL | Structured Query Language | 구조화 쿼리 언어 |
| TDD | Test-Driven Development | 테스트 주도 개발 |
| UI | User Interface | 사용자 인터페이스 |
| URI | Uniform Resource Identifier | 통합 자원 식별자 |
| URL | Uniform Resource Locator | 통합 자원 위치 지정자 |
| XSS | Cross-Site Scripting | 사이트 간 스크립팅 |

## 프론트엔드 개발자를 위한 용어 매핑

| .NET 용어 | JavaScript/Node.js 용어 |
|----------|------------------------|
| ASP.NET Core | Express.js, Fastify |
| Entity Framework Core | Prisma, TypeORM |
| LINQ | Array methods (map, filter, reduce) |
| async/await | async/await |
| Task | Promise |
| NuGet | npm, yarn |
| namespace | ES modules |
| interface | TypeScript interface |
| Blazor | React, Vue |
| SignalR | Socket.io |
| Middleware | Express middleware |
| Dependency Injection | InversifyJS |
| xUnit | Jest, Mocha |
| Swagger | swagger-ui-express |
| Hangfire | Bull, Agenda |
| Serilog | winston, pino |
| Refit | axios |
| FluentValidation | Joi, Yup |

## 요약

이 용어집은 ASP.NET Core 개발에서 자주 사용하는 용어를 다룹니다:

1. **핵심 개념**: DI, Middleware, Controller, Action
2. **데이터**: EF Core, LINQ, Repository
3. **비동기**: async/await, Task, CancellationToken
4. **보안**: JWT, CORS, CSRF
5. **테스트**: xUnit, Moq, Integration Testing
6. **아키텍처**: Clean Architecture, CQRS, DDD

각 용어는 프론트엔드 개발자가 익숙한 JavaScript/Node.js 용어와 비교하여 빠르게 이해할 수 있도록 구성했습니다.
