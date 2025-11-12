# Appendix D: 유용한 NuGet 패키지 모음

실무 ASP.NET Core 개발에서 자주 사용하는 NuGet 패키지를 카테고리별로 정리했습니다. npm 패키지와의 비교를 통해 프론트엔드 개발자가 빠르게 이해할 수 있도록 구성했습니다.

## 1. 데이터베이스 및 ORM

### 1.1 Entity Framework Core

```bash
# 핵심 패키지
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design

# SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# MySQL
dotnet add package Pomelo.EntityFrameworkCore.MySql

# SQLite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# In-Memory (테스트용)
dotnet add package Microsoft.EntityFrameworkCore.InMemory

# 도구
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

**npm 비교**
```bash
npm install prisma @prisma/client
npm install typeorm mysql2
npm install sequelize pg
```

### 1.2 Dapper (Micro ORM)

```bash
dotnet add package Dapper
```

**사용 예제**
```csharp
using Dapper;

var products = await connection.QueryAsync<Product>(
    "SELECT * FROM Products WHERE CategoryId = @CategoryId",
    new { CategoryId = 1 });

// INSERT
await connection.ExecuteAsync(
    "INSERT INTO Products (Name, Price) VALUES (@Name, @Price)",
    new { Name = "Laptop", Price = 1200 });
```

**npm 비교**: 없음 (SQL 쿼리 빌더 직접 작성)

## 2. API 문서화

### 2.1 Swagger/OpenAPI

```bash
# Swashbuckle (가장 인기)
dotnet add package Swashbuckle.AspNetCore

# NSwag (TypeScript 클라이언트 생성 가능)
dotnet add package NSwag.AspNetCore
```

**설정 예제**
```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "API documentation"
    });

    // JWT 인증 추가
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
});

app.UseSwagger();
app.UseSwaggerUI();
```

**npm 비교**
```bash
npm install swagger-ui-express swagger-jsdoc
npm install @nestjs/swagger  # NestJS
```

## 3. 인증 및 권한

### 3.1 ASP.NET Core Identity

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
```

### 3.2 JWT

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
```

**설정 예제**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
```

**npm 비교**
```bash
npm install jsonwebtoken bcryptjs
npm install passport passport-jwt
```

### 3.3 OAuth 제공자

```bash
# Google
dotnet add package Microsoft.AspNetCore.Authentication.Google

# Facebook
dotnet add package Microsoft.AspNetCore.Authentication.Facebook

# Microsoft
dotnet add package Microsoft.AspNetCore.Authentication.MicrosoftAccount

# Twitter
dotnet add package Microsoft.AspNetCore.Authentication.Twitter
```

**npm 비교**
```bash
npm install passport-google-oauth20
npm install passport-facebook
```

## 4. 데이터 검증

### 4.1 FluentValidation

```bash
dotnet add package FluentValidation
dotnet add package FluentValidation.AspNetCore
```

**사용 예제**
```csharp
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThan(1000000).WithMessage("Price must be less than 1,000,000");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format");
    }
}

// 등록
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();
```

**npm 비교**
```bash
npm install joi
npm install yup
npm install class-validator class-transformer
```

## 5. 로깅

### 5.1 Serilog

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Seq
dotnet add package Serilog.Enrichers.Environment
```

**설정 예제**
```csharp
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});
```

**npm 비교**
```bash
npm install winston
npm install pino pino-pretty
npm install bunyan
```

### 5.2 NLog

```bash
dotnet add package NLog.Web.AspNetCore
```

## 6. HTTP 클라이언트

### 6.1 Refit (Type-safe HTTP Client)

```bash
dotnet add package Refit
dotnet add package Refit.HttpClientFactory
```

**사용 예제**
```csharp
public interface IGitHubApi
{
    [Get("/users/{username}")]
    Task<User> GetUserAsync(string username);

    [Post("/repos/{owner}/{repo}/issues")]
    Task<Issue> CreateIssueAsync(string owner, string repo, [Body] CreateIssueRequest request);
}

// 등록
builder.Services.AddRefitClient<IGitHubApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.github.com"));

// 사용
var user = await githubApi.GetUserAsync("octocat");
```

**npm 비교**
```bash
npm install axios
npm install ky
npm install @tanstack/react-query  # API 클라이언트 + 상태 관리
```

### 6.2 RestSharp

```bash
dotnet add package RestSharp
```

**사용 예제**
```csharp
var client = new RestClient("https://api.example.com");
var request = new RestRequest("/users/{id}", Method.Get);
request.AddUrlSegment("id", 123);

var response = await client.ExecuteAsync<User>(request);
```

## 7. JSON 처리

### 7.1 Newtonsoft.Json

```bash
dotnet add package Newtonsoft.Json
```

**사용 예제**
```csharp
var json = JsonConvert.SerializeObject(product);
var product = JsonConvert.DeserializeObject<Product>(json);

// 설정
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });
```

**npm 비교**: JSON.parse/JSON.stringify (내장)

## 8. 매핑

### 8.1 AutoMapper

```bash
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

**사용 예제**
```csharp
// 프로필 정의
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();

        // 복잡한 매핑
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Items.Sum(i => i.Price * i.Quantity)));
    }
}

// 등록
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 사용
var productDto = _mapper.Map<ProductDto>(product);
var products = _mapper.Map<List<ProductDto>>(productList);
```

**npm 비교**
```bash
npm install class-transformer
npm install automapper-core @automapper/classes
```

## 9. 캐싱

### 9.1 Redis

```bash
# StackExchange.Redis (가장 인기)
dotnet add package StackExchange.Redis
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

**설정 예제**
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "MyApp_";
});

// 사용
await _cache.SetStringAsync("key", "value", new DistributedCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
});

var value = await _cache.GetStringAsync("key");
```

**npm 비교**
```bash
npm install redis
npm install ioredis
```

### 9.2 Memory Cache

```bash
# 내장 패키지 (추가 설치 불필요)
builder.Services.AddMemoryCache();
```

## 10. 백그라운드 작업

### 10.1 Hangfire

```bash
dotnet add package Hangfire.AspNetCore
dotnet add package Hangfire.SqlServer
dotnet add package Hangfire.PostgreSql
```

**설정 예제**
```csharp
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

app.UseHangfireDashboard("/hangfire");

// 백그라운드 작업
BackgroundJob.Enqueue(() => SendEmailAsync(email));
BackgroundJob.Schedule(() => GenerateReportAsync(), TimeSpan.FromDays(1));
RecurringJob.AddOrUpdate("daily-cleanup", () => CleanupOldDataAsync(), Cron.Daily);
```

**npm 비교**
```bash
npm install bull  # Redis 기반 큐
npm install agenda  # MongoDB 기반 스케줄러
npm install node-cron
```

### 10.2 Quartz.NET

```bash
dotnet add package Quartz
dotnet add package Quartz.Extensions.Hosting
```

## 11. 실시간 통신

### 11.1 SignalR

```bash
# 내장 (추가 설치 불필요)
builder.Services.AddSignalR();

# Redis 백플레인 (스케일 아웃)
dotnet add package Microsoft.AspNetCore.SignalR.StackExchangeRedis
```

**npm 비교**
```bash
npm install socket.io
npm install ws
```

## 12. 테스트

### 12.1 xUnit

```bash
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk
```

### 12.2 Moq

```bash
dotnet add package Moq
```

**사용 예제**
```csharp
var mockRepo = new Mock<IProductRepository>();
mockRepo.Setup(r => r.GetByIdAsync(1))
    .ReturnsAsync(new Product { Id = 1, Name = "Laptop" });

var service = new ProductService(mockRepo.Object);
var product = await service.GetProductAsync(1);

mockRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
```

**npm 비교**
```bash
npm install jest @types/jest
npm install vitest
npm install mocha chai sinon
```

### 12.3 FluentAssertions

```bash
dotnet add package FluentAssertions
```

**사용 예제**
```csharp
product.Name.Should().Be("Laptop");
product.Price.Should().BeGreaterThan(0);
products.Should().HaveCount(5);
products.Should().Contain(p => p.Name == "Laptop");

Action act = () => service.CreateProduct(null);
act.Should().Throw<ArgumentNullException>();
```

**npm 비교**
```bash
npm install chai
npm install @testing-library/jest-dom
```

### 12.4 WebApplicationFactory (Integration Testing)

```bash
dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

### 12.5 Playwright (E2E Testing)

```bash
dotnet add package Microsoft.Playwright
```

**npm 비교**
```bash
npm install @playwright/test
npm install cypress
```

## 13. 모니터링 및 APM

### 13.1 Application Insights

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

### 13.2 OpenTelemetry

```bash
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Exporter.Jaeger
```

### 13.3 Prometheus

```bash
dotnet add package prometheus-net
dotnet add package prometheus-net.AspNetCore
```

**설정 예제**
```csharp
using Prometheus;

app.UseMetricServer();  // /metrics 엔드포인트
app.UseHttpMetrics();   // HTTP 메트릭 수집
```

**npm 비교**
```bash
npm install prom-client
npm install @opentelemetry/sdk-node
```

## 14. 메시징

### 14.1 MassTransit (RabbitMQ, Azure Service Bus 등)

```bash
dotnet add package MassTransit
dotnet add package MassTransit.RabbitMQ
dotnet add package MassTransit.Azure.ServiceBus.Core
```

**npm 비교**
```bash
npm install amqplib  # RabbitMQ
npm install kafkajs  # Kafka
npm install @azure/service-bus
```

### 14.2 RabbitMQ.Client

```bash
dotnet add package RabbitMQ.Client
```

## 15. 문서 생성

### 15.1 iTextSharp (PDF)

```bash
dotnet add package itext7
```

### 15.2 ClosedXML (Excel)

```bash
dotnet add package ClosedXML
```

**사용 예제**
```csharp
using var workbook = new XLWorkbook();
var worksheet = workbook.Worksheets.Add("Products");

worksheet.Cell(1, 1).Value = "Name";
worksheet.Cell(1, 2).Value = "Price";

int row = 2;
foreach (var product in products)
{
    worksheet.Cell(row, 1).Value = product.Name;
    worksheet.Cell(row, 2).Value = product.Price;
    row++;
}

workbook.SaveAs("products.xlsx");
```

**npm 비교**
```bash
npm install exceljs
npm install pdfkit
```

## 16. 이미지 처리

### 16.1 ImageSharp

```bash
dotnet add package SixLabors.ImageSharp
dotnet add package SixLabors.ImageSharp.Web
```

**사용 예제**
```csharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

using var image = await Image.LoadAsync(stream);

// 리사이즈
image.Mutate(x => x.Resize(new ResizeOptions
{
    Mode = ResizeMode.Max,
    Size = new Size(800, 600)
}));

// 썸네일
image.Mutate(x => x.Resize(200, 200));

await image.SaveAsync("thumbnail.jpg");
```

**npm 비교**
```bash
npm install sharp
npm install jimp
```

## 17. 날짜 및 시간

### 17.1 NodaTime

```bash
dotnet add package NodaTime
dotnet add package NodaTime.Serialization.SystemTextJson
```

**사용 예제**
```csharp
var now = SystemClock.Instance.GetCurrentInstant();
var zonedNow = now.InZone(DateTimeZoneProviders.Tzdb["America/New_York"]);

var localDate = new LocalDate(2024, 12, 25);
var localTime = new LocalTime(14, 30, 0);
```

**npm 비교**
```bash
npm install dayjs
npm install date-fns
npm install luxon
```

## 18. 구성 관리

### 18.1 Azure Key Vault

```bash
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

### 18.2 환경 변수

```bash
# 내장 (추가 설치 불필요)
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
```

## 19. Rate Limiting

### 19.1 ASP.NET Core Rate Limiting (.NET 7+)

```bash
# 내장 (추가 설치 불필요)
```

**설정 예제**
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});

app.UseRateLimiter();
```

### 19.2 AspNetCoreRateLimit (레거시)

```bash
dotnet add package AspNetCoreRateLimit
```

**npm 비교**
```bash
npm install express-rate-limit
npm install rate-limiter-flexible
```

## 20. 보안

### 20.1 OWASP 보안

```bash
# 헤더 보안
dotnet add package NWebsec.AspNetCore.Middleware

# CSRF 보호 (내장)
# XSS 방지 (내장)
```

**설정 예제**
```csharp
app.UseXContentTypeOptions();
app.UseReferrerPolicy(opts => opts.NoReferrer());
app.UseXXssProtection(options => options.EnabledWithBlockMode());
app.UseXfo(options => options.Deny());
```

### 20.2 데이터 보호

```bash
# 내장
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"./keys/"))
    .SetApplicationName("MyApp");
```

## 21. 유틸리티

### 21.1 Polly (복원력 패턴)

```bash
dotnet add package Polly
dotnet add package Microsoft.Extensions.Http.Polly
```

**사용 예제**
```csharp
builder.Services.AddHttpClient<IMyService, MyService>()
    .AddTransientHttpErrorPolicy(builder =>
        builder.WaitAndRetryAsync(new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(4)
        }))
    .AddTransientHttpErrorPolicy(builder =>
        builder.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
```

**npm 비교**
```bash
npm install axios-retry
npm install opossum  # Circuit Breaker
```

### 21.2 Bogus (가짜 데이터 생성)

```bash
dotnet add package Bogus
```

**사용 예제**
```csharp
var faker = new Faker<Product>()
    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
    .RuleFor(p => p.Price, f => f.Random.Decimal(10, 1000))
    .RuleFor(p => p.Description, f => f.Lorem.Paragraph());

var products = faker.Generate(100);
```

**npm 비교**
```bash
npm install @faker-js/faker
npm install chance
```

### 21.3 Humanizer

```bash
dotnet add package Humanizer
```

**사용 예제**
```csharp
"PascalCase".Humanize() // "Pascal case"
"2024-12-25".ToDateTime().Humanize() // "tomorrow"
TimeSpan.FromDays(1).Humanize() // "1 day"
123456.ToWords() // "one hundred and twenty-three thousand four hundred and fifty-six"
```

## 22. 성능

### 22.1 BenchmarkDotNet

```bash
dotnet add package BenchmarkDotNet
```

**사용 예제**
```csharp
[MemoryDiagnoser]
public class StringConcatBenchmark
{
    [Benchmark]
    public string UsingStringBuilder()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 1000; i++)
            sb.Append(i);
        return sb.ToString();
    }

    [Benchmark]
    public string UsingStringConcat()
    {
        var result = "";
        for (int i = 0; i < 1000; i++)
            result += i;
        return result;
    }
}

// 실행
BenchmarkRunner.Run<StringConcatBenchmark>();
```

## 23. GraphQL

### 23.1 HotChocolate

```bash
dotnet add package HotChocolate.AspNetCore
dotnet add package HotChocolate.Data.EntityFramework
```

**npm 비교**
```bash
npm install @apollo/server graphql
npm install @nestjs/graphql @nestjs/apollo
```

## 24. gRPC

```bash
dotnet add package Grpc.AspNetCore
dotnet add package Grpc.Tools
dotnet add package Google.Protobuf
```

**npm 비교**
```bash
npm install @grpc/grpc-js @grpc/proto-loader
```

## 25. 패키지 설치 스크립트 예제

### 25.1 기본 Web API 프로젝트

```bash
# 프로젝트 생성
dotnet new webapi -n MyApi

cd MyApi

# 필수 패키지
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation.AspNetCore
dotnet add package Serilog.AspNetCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# 개발용 패키지
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### 25.2 엔터프라이즈 프로젝트

```bash
# 위 기본 패키지 + 추가
dotnet add package MediatR
dotnet add package Hangfire.AspNetCore
dotnet add package StackExchange.Redis
dotnet add package Refit.HttpClientFactory
dotnet add package Polly
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package prometheus-net.AspNetCore
```

## 요약

이 가이드는 ASP.NET Core 개발에서 가장 많이 사용하는 NuGet 패키지를 다룹니다:

**핵심 카테고리**:
- 데이터베이스: EF Core, Dapper
- API: Swagger, Refit
- 인증: JWT, Identity, OAuth
- 로깅: Serilog, NLog
- 테스트: xUnit, Moq, FluentAssertions
- 백그라운드: Hangfire, Quartz.NET
- 캐싱: Redis, Memory Cache
- 매핑: AutoMapper
- 검증: FluentValidation
- 모니터링: Application Insights, Prometheus

각 패키지는 npm 생태계의 대응 패키지와 비교하여 프론트엔드 개발자가 빠르게 이해할 수 있도록 구성했습니다.
