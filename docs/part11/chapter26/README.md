# Chapter 26: 프로덕션 고려사항 - 안정적이고 안전한 서비스 운영

## 프로덕션은 시작일 뿐: 진짜 도전은 운영에 있다

Chapter 24에서 컨테이너를 만들고, Chapter 25에서 클라우드에 배포했습니다. 축하합니다! 애플리케이션이 이제 인터넷에서 접근할 수 있습니다. 하지만 배포는 끝이 아닙니다. 오히려 **시작**입니다. 프로덕션 환경에서 애플리케이션을 안정적으로 운영하는 것이 진짜 도전입니다.

"개발에서 잘 작동한다"와 "프로덕션에서 안정적으로 실행된다"는 완전히 다릅니다. 개발 환경에서는 혼자 사용하고, 데이터는 적으며, 실패해도 다시 시작하면 됩니다. 프로덕션은 다릅니다:

- **수천, 수만 명의 동시 사용자**: 부하가 예측 불가능하게 변동합니다.
- **실제 데이터**: 손실되면 안 됩니다. 백업과 복구 전략이 필수입니다.
- **24/7 가용성**: 사용자는 언제나 접근을 기대합니다. 다운타임은 신뢰와 매출 손실입니다.
- **보안 위협**: 악의적인 공격자가 취약점을 노립니다.
- **규정 준수**: GDPR, HIPAA, PCI-DSS 같은 규정을 따라야 할 수 있습니다.

Werner Vogels (Amazon CTO)의 유명한 말이 있습니다: "Everything fails, all the time." 프로덕션 환경에서는 항상 무언가가 실패합니다. 서버가 다운되고, 네트워크가 불안정하며, 디스크가 가득 차고, 외부 API가 응답하지 않습니다. 성공적인 시스템은 **실패를 예상하고 대비**하는 시스템입니다.

이 챕터에서는 프로덕션 환경에서 ASP.NET Core 애플리케이션을 안정적이고 안전하게 운영하기 위한 모든 고려사항을 다룹니다.

## 환경 구성 관리: 개발, 스테이징, 프로덕션

동일한 코드베이스가 여러 환경에서 실행됩니다. 하지만 각 환경은 다른 구성이 필요합니다:

- **개발 (Development)**: 로컬 데이터베이스, 디버그 로깅, 샌드박스 API
- **스테이징 (Staging)**: 프로덕션과 유사, 실제 크기의 데이터, 테스트용
- **프로덕션 (Production)**: 실제 사용자, 실제 데이터, 최소 로깅, 높은 보안

ASP.NET Core는 강력한 구성 시스템을 제공하며, 환경별로 다른 설정을 관리할 수 있습니다.

### appsettings.json 계층 구조

ASP.NET Core는 여러 구성 소스를 **계층적으로** 로드합니다. 나중에 로드된 값이 이전 값을 덮어씁니다:

1. `appsettings.json` (기본 설정)
2. `appsettings.{Environment}.json` (환경별 재정의)
3. User Secrets (개발 환경만)
4. 환경 변수
5. 명령줄 인수

**예제:**

`appsettings.json` (모든 환경에 공통):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AppSettings": {
    "AppName": "MyApp",
    "EnableFeatureX": false
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=myapp;User=dev;Password=dev"
  }
}
```

`appsettings.Development.json` (개발 환경 재정의):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  }
}
```

`appsettings.Production.json` (프로덕션 환경 재정의):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Error"
    }
  },
  "AppSettings": {
    "EnableFeatureX": true
  }
}
```

**중요**: `appsettings.Production.json`에 민감한 정보 (비밀번호, API 키)를 저장하지 마세요. 환경 변수나 Key Vault를 사용합니다.

### 환경 변수로 재정의

환경 변수는 appsettings보다 우선합니다. 계층 구조는 `__` (이중 언더스코어)로 표현합니다:

```bash
# Bash
export ConnectionStrings__DefaultConnection="Server=prod-db;Database=myapp;User=prod;Password=secret"
export AppSettings__EnableFeatureX="true"

# PowerShell
$env:ConnectionStrings__DefaultConnection="Server=prod-db;Database=myapp;User=prod;Password=secret"
$env:AppSettings__EnableFeatureX="true"

# Docker
docker run -e ConnectionStrings__DefaultConnection="..." myapp

# Kubernetes ConfigMap
env:
  - name: ConnectionStrings__DefaultConnection
    value: "Server=prod-db;Database=myapp;..."
```

### Options 패턴: 강타입 구성

구성을 문자열로 접근하는 대신, 강타입 클래스로 바인딩합니다. 이는 타입 안전성, IntelliSense, 검증을 제공합니다.

**1. 설정 클래스 정의:**

```csharp
public class AppSettings
{
    public const string SectionName = "AppSettings";

    public string AppName { get; set; } = string.Empty;
    public bool EnableFeatureX { get; set; }
    public int MaxUploadSizeMB { get; set; } = 10;
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
```

**2. Program.cs에서 등록:**

```csharp
var builder = WebApplication.CreateBuilder(args);

// Options 패턴으로 바인딩
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection(AppSettings.SectionName));

var app = builder.Build();
```

**3. 컨트롤러나 서비스에서 주입:**

```csharp
using Microsoft.Extensions.Options;

public class MyController : ControllerBase
{
    private readonly AppSettings _settings;

    public MyController(IOptions<AppSettings> options)
    {
        _settings = options.Value;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new {
            AppName = _settings.AppName,
            FeatureXEnabled = _settings.EnableFeatureX
        });
    }
}
```

**4. 구성 검증 (선택적이지만 권장):**

```csharp
public class AppSettings : IValidateOptions<AppSettings>
{
    // ... properties ...

    public ValidateOptionsResult Validate(string? name, AppSettings options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.AppName))
            errors.Add("AppName is required");

        if (options.MaxUploadSizeMB <= 0 || options.MaxUploadSizeMB > 100)
            errors.Add("MaxUploadSizeMB must be between 1 and 100");

        return errors.Any()
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}

// Program.cs에서
builder.Services.AddSingleton<IValidateOptions<AppSettings>, AppSettings>();
```

애플리케이션 시작 시 검증이 실패하면, 명확한 오류 메시지와 함께 시작이 중단됩니다.

## 비밀 관리: 절대 Git에 커밋하지 마세요

**황금률: 비밀은 Git에 절대 커밋하지 마세요.**

데이터베이스 비밀번호, API 키, 인증서, 암호화 키—이런 민감한 정보가 Git 히스토리에 들어가면, 영원히 남습니다. Git 히스토리를 완전히 재작성하지 않는 한 제거할 수 없습니다. 그리고 누군가 이미 클론했다면, 손쓸 방법이 없습니다.

### 개발 환경: User Secrets

개발 환경에서는 **User Secrets**를 사용합니다. 비밀이 프로젝트 폴더가 아닌, 사용자 프로필 디렉터리에 저장됩니다.

```bash
# User Secrets 초기화
dotnet user-secrets init

# 비밀 설정
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=myapp;User=dev;Password=devpass"
dotnet user-secrets set "ApiKeys:OpenAI" "sk-xxxxxxxxxxxx"

# 비밀 확인
dotnet user-secrets list

# 비밀 제거
dotnet user-secrets remove "ApiKeys:OpenAI"

# 모든 비밀 삭제
dotnet user-secrets clear
```

User Secrets는 자동으로 로드됩니다 (개발 환경에서만). `IConfiguration`을 통해 접근합니다:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var apiKey = builder.Configuration["ApiKeys:OpenAI"];
```

**제한사항:**
- 개발 환경에만 사용 (프로덕션 아님)
- 암호화되지 않음 (로컬 파일 시스템 보안에 의존)

### 프로덕션: 클라우드 Key Vault / Secrets Manager

프로덕션 환경에서는 클라우드 제공자의 비밀 관리 서비스를 사용합니다.

#### Azure Key Vault

**장점:**
- 암호화된 저장소
- 접근 제어 (Azure AD, Managed Identity)
- 감사 로깅 (누가 언제 접근했는지)
- 자동 회전 지원

**1. Key Vault 생성:**

```bash
az keyvault create \
  --name mykeyvault \
  --resource-group myResourceGroup \
  --location eastus

# 비밀 추가
az keyvault secret set \
  --vault-name mykeyvault \
  --name ConnectionStrings--DefaultConnection \
  --value "Server=prod-db;Database=myapp;User=prod;Password=secretpass"
```

**2. Managed Identity 생성 및 권한 부여:**

```bash
# App Service에 Managed Identity 활성화
az webapp identity assign \
  --name myapp \
  --resource-group myResourceGroup

# Managed Identity에 Key Vault 접근 권한 부여
az keyvault set-policy \
  --name mykeyvault \
  --object-id <managed-identity-principal-id> \
  --secret-permissions get list
```

**3. .NET 애플리케이션에서 통합:**

```bash
# NuGet 패키지 설치
dotnet add package Azure.Identity
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
```

```csharp
// Program.cs
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Key Vault 통합
var keyVaultUri = new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

var app = builder.Build();
```

이제 Key Vault의 비밀을 `IConfiguration`을 통해 투명하게 접근할 수 있습니다. Managed Identity 덕분에 비밀번호나 키가 코드에 없습니다!

**4. 환경 변수로 Key Vault 이름 전달:**

```bash
# Azure App Service 설정
az webapp config appsettings set \
  --name myapp \
  --resource-group myResourceGroup \
  --settings KeyVaultName=mykeyvault
```

#### AWS Secrets Manager

**1. 비밀 생성:**

```bash
aws secretsmanager create-secret \
  --name myapp/ConnectionString \
  --description "Production database connection string" \
  --secret-string "Server=prod-db;Database=myapp;User=prod;Password=secretpass"
```

**2. IAM 권한 부여:**

ECS Task Role이나 EC2 Instance Profile에 `secretsmanager:GetSecretValue` 권한을 부여합니다.

**3. .NET에서 접근:**

```bash
dotnet add package AWSSDK.SecretsManager
```

```csharp
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

public class SecretService
{
    private readonly IAmazonSecretsManager _secretsManager;

    public SecretService(IAmazonSecretsManager secretsManager)
    {
        _secretsManager = secretsManager;
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        var request = new GetSecretValueRequest
        {
            SecretId = secretName
        };

        var response = await _secretsManager.GetSecretValueAsync(request);
        return response.SecretString;
    }
}

// Program.cs
builder.Services.AddAWSService<IAmazonSecretsManager>();
builder.Services.AddSingleton<SecretService>();
```

**자동 회전:**

```bash
aws secretsmanager rotate-secret \
  --secret-id myapp/ConnectionString \
  --rotation-lambda-arn arn:aws:lambda:...
```

#### GCP Secret Manager

**1. 비밀 생성:**

```bash
echo -n "Server=prod-db;Database=myapp;User=prod;Password=secretpass" | \
  gcloud secrets create connection-string --data-file=-
```

**2. 권한 부여:**

```bash
gcloud secrets add-iam-policy-binding connection-string \
  --member="serviceAccount:myapp@my-project.iam.gserviceaccount.com" \
  --role="roles/secretmanager.secretAccessor"
```

**3. .NET에서 접근:**

```bash
dotnet add package Google.Cloud.SecretManager.V1
```

```csharp
using Google.Cloud.SecretManager.V1;

public class SecretService
{
    private readonly SecretManagerServiceClient _client;

    public SecretService()
    {
        _client = SecretManagerServiceClient.Create();
    }

    public string GetSecret(string projectId, string secretId)
    {
        var secretVersionName = new SecretVersionName(projectId, secretId, "latest");
        var response = _client.AccessSecretVersion(secretVersionName);
        return response.Payload.Data.ToStringUtf8();
    }
}
```

## HTTPS와 SSL/TLS: 보안 통신의 기본

프로덕션 환경에서 **HTTPS는 선택이 아닌 필수**입니다. 모든 통신을 암호화하여 중간자 공격을 방지합니다. 2025년 현재, 주요 브라우저는 HTTPS가 아닌 사이트를 "안전하지 않음"으로 표시하며, 일부 기능 (Service Workers, Geolocation)은 HTTPS에서만 작동합니다.

### Let's Encrypt: 무료 SSL 인증서

Let's Encrypt는 무료 SSL/TLS 인증서를 제공하는 비영리 인증 기관입니다. 자동 갱신을 지원하여, 인증서 만료를 걱정할 필요가 없습니다.

**Azure App Service:**

Azure App Service는 커스텀 도메인에 무료 관리 인증서를 제공합니다 (Let's Encrypt 기반):

```bash
az webapp config ssl bind \
  --name myapp \
  --resource-group myResourceGroup \
  --certificate-thumbprint <thumbprint> \
  --ssl-type SNI
```

또는 Azure Portal에서 클릭 몇 번으로 설정할 수 있습니다.

**AWS Certificate Manager (ACM):**

ACM은 무료 SSL 인증서를 제공하며, ALB와 CloudFront에서 사용할 수 있습니다:

```bash
aws acm request-certificate \
  --domain-name myapp.example.com \
  --validation-method DNS

# DNS 검증 레코드 추가 (Route 53)
# 인증서가 발급되면 ALB에 연결
```

**GCP Managed SSL:**

GCP는 Google 관리 SSL 인증서를 제공합니다:

```bash
gcloud compute ssl-certificates create myapp-cert \
  --domains=myapp.example.com \
  --global
```

### HSTS (HTTP Strict Transport Security)

HSTS는 브라우저에게 "이 사이트는 항상 HTTPS를 사용한다"고 알립니다. 한 번 HTTPS로 접속하면, 브라우저는 향후 모든 요청을 자동으로 HTTPS로 변환합니다 (사용자가 `http://`를 입력해도).

```csharp
// Program.cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    await next();
});

// 또는 미들웨어 사용
app.UseHsts();
```

**주의**: HSTS를 활성화하기 전에, HTTPS가 완벽하게 작동하는지 확인하세요. HSTS는 되돌리기 어렵습니다.

### SSL 오프로딩

고부하 환경에서는 SSL/TLS 암호화/복호화가 CPU를 소비합니다. **SSL 오프로딩**은 이 작업을 로드 밸런서에게 맡겨, 애플리케이션 서버의 부담을 줄입니다.

- **Azure**: Application Gateway가 SSL 종료 수행
- **AWS**: Application Load Balancer (ALB)가 SSL 종료 수행
- **GCP**: Cloud Load Balancer가 SSL 종료 수행

애플리케이션은 로드 밸런서로부터 HTTP 요청을 받지만, 클라이언트와는 HTTPS로 통신합니다.

## Rate Limiting: API 남용 방지

Rate Limiting은 특정 시간 내에 사용자나 IP가 보낼 수 있는 요청 수를 제한합니다. 이는 여러 목적을 달성합니다:

- **서비스 거부 공격 (DoS) 방지**: 악의적인 사용자가 서버를 압도하는 것을 막습니다.
- **공정한 리소스 사용**: 모든 사용자가 공평하게 API를 사용하도록 보장합니다.
- **비용 제어**: 외부 API 호출 (비용이 발생하는)을 제한합니다.

### .NET 9의 Rate Limiter 미들웨어

.NET 7부터 `System.Threading.RateLimiting` 네임스페이스가 추가되었으며, .NET 9에서 더욱 강화되었습니다.

**1. 기본 Rate Limiting:**

```csharp
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Rate Limiter 추가
builder.Services.AddRateLimiter(options =>
{
    // Fixed Window: 1분당 100개 요청
    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromMinutes(1);
        options.PermitLimit = 100;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 10;
    });

    // 전역 설정
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
        {
            Window = TimeSpan.FromMinutes(1),
            PermitLimit = 100
        });
    });

    // Rate limit 초과 시 응답
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
    };
});

var app = builder.Build();

app.UseRateLimiter();
```

**2. 엔드포인트별 Rate Limiting:**

```csharp
app.MapGet("/api/data", () => "Data")
    .RequireRateLimiting("fixed");

app.MapPost("/api/expensive", async () =>
{
    // 비용이 많이 드는 작업
    await Task.Delay(1000);
    return Results.Ok("Done");
})
.RequireRateLimiting("strict");

// "strict" 정책 정의
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("strict", options =>
    {
        options.Window = TimeSpan.FromMinutes(1);
        options.PermitLimit = 10; // 매우 제한적
    });
});
```

**3. 사용자별 Rate Limiting (인증 필요):**

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

        // 인증된 사용자는 더 높은 제한
        var permitLimit = userId != "anonymous" ? 1000 : 100;

        return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
        {
            Window = TimeSpan.FromHours(1),
            PermitLimit = permitLimit
        });
    });
});
```

### Rate Limiting 정책 종류

**Fixed Window:**
- 고정된 시간 창 (예: 1분)
- 창이 끝나면 카운터 리셋
- 단순하지만, 창 경계에서 버스트 가능

**Sliding Window:**
- 슬라이딩 시간 창
- 더 부드러운 제한
- 약간 더 복잡

**Token Bucket:**
- 버킷에 토큰이 일정한 속도로 추가됨
- 요청마다 토큰 소비
- 버스트 허용 (버킷이 가득 차면)

**Concurrency Limiter:**
- 동시 요청 수 제한
- 시간이 아닌 동시성 기반

## 오류 처리와 복원력: 실패를 우아하게 다루기

### Global Exception Handler

모든 예외를 일관되게 처리하려면, 글로벌 예외 핸들러를 사용합니다.

**.NET 8+ Built-in Exception Handler:**

```csharp
var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;

        // 로깅
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception occurred");

        // 에러 응답
        var errorResponse = new
        {
            error = "An error occurred while processing your request",
            requestId = context.TraceIdentifier
        };

        // 개발 환경에서는 더 자세한 정보
        if (app.Environment.IsDevelopment())
        {
            errorResponse = new
            {
                error = exception?.Message,
                stackTrace = exception?.StackTrace,
                requestId = context.TraceIdentifier
            };
        }

        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});
```

### Polly: 복원력 패턴의 표준

Polly는 .NET의 복원력 (resilience) 라이브러리입니다. Retry, Circuit Breaker, Timeout, Fallback 같은 패턴을 쉽게 구현할 수 있습니다.

```bash
dotnet add package Polly
dotnet add package Polly.Extensions.Http
```

**1. Retry 정책:**

일시적 오류 (네트워크 블립, 일시적 서비스 불가)를 자동으로 재시도합니다.

```csharp
using Polly;
using Polly.Extensions.Http;

// Program.cs
builder.Services.AddHttpClient("ExternalApi", client =>
{
    client.BaseAddress = new Uri("https://api.external.com");
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError() // 5xx, 408 에러
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 지수 백오프
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
            Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s");
        }
    ));
```

**2. Circuit Breaker:**

연속된 실패가 발생하면 요청을 차단하여, 실패하는 서비스에 부하를 주지 않습니다.

```csharp
builder.Services.AddHttpClient("ExternalApi")
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5, // 5번 실패하면
            durationOfBreak: TimeSpan.FromSeconds(30), // 30초 동안 차단
            onBreak: (result, duration) =>
            {
                Console.WriteLine($"Circuit breaker opened for {duration.TotalSeconds}s");
            },
            onReset: () =>
            {
                Console.WriteLine("Circuit breaker reset");
            }
        ));
```

**3. Timeout:**

응답이 너무 오래 걸리면 취소합니다.

```csharp
builder.Services.AddHttpClient("ExternalApi")
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)));
```

**4. 정책 조합 (Wrap):**

여러 정책을 조합할 수 있습니다. 일반적으로 Timeout → Retry → Circuit Breaker 순서입니다.

```csharp
var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<TimeoutRejectedException>() // Timeout도 재시도
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

builder.Services.AddHttpClient("ExternalApi")
    .AddPolicyHandler(timeoutPolicy)
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);
```

**Fallback:**

모든 재시도가 실패하면, 대체 값을 반환합니다.

```csharp
var fallbackPolicy = Policy<HttpResponseMessage>
    .Handle<Exception>()
    .FallbackAsync(
        fallbackValue: new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"data\": \"cached or default value\"}")
        },
        onFallbackAsync: async (result, context) =>
        {
            Console.WriteLine("Fallback triggered, returning cached value");
            await Task.CompletedTask;
        }
    );
```

## 백업과 재해 복구: 최악의 시나리오 대비

"백업은 했지만 복구는 안 해봤다"는 백업이 없는 것과 같습니다. 재해 복구 (Disaster Recovery, DR) 계획은 필수입니다.

### 핵심 개념

**RTO (Recovery Time Objective):**
- 얼마나 빨리 복구할 수 있는가?
- 예: "서비스는 4시간 이내에 복구되어야 한다"

**RPO (Recovery Point Objective):**
- 얼마나 많은 데이터 손실을 감수할 수 있는가?
- 예: "최대 15분의 데이터 손실은 허용 가능하다"

RTO와 RPO는 비용과 트레이드오프입니다. 더 짧은 RTO/RPO는 더 많은 비용이 듭니다.

### 데이터베이스 백업

**자동 백업:**

모든 주요 클라우드 데이터베이스 서비스는 자동 백업을 제공합니다:

- **Azure SQL Database**: 자동 백업 (7-35일 보관), Point-in-time restore
- **AWS RDS**: 자동 백업 (최대 35일), 자동 스냅샷
- **GCP Cloud SQL**: 자동 백업 및 바이너리 로그

**수동 백업 스크립트:**

```bash
# PostgreSQL
pg_dump -h mydb.postgres.database.azure.com -U myuser -d mydb > backup_$(date +%Y%m%d).sql

# SQL Server
sqlcmd -S mydb.database.windows.net -U myuser -P mypass -Q "BACKUP DATABASE mydb TO URL='https://mystorageaccount.blob.core.windows.net/backups/mydb.bak'"
```

**백업 테스트:**

주기적으로 백업을 복원하여 실제로 작동하는지 확인하세요. 매 분기마다 DR 시뮬레이션을 수행하는 것이 좋습니다.

### 지역 중복성 (Multi-Region)

단일 지역 장애에 대비하려면, 다중 지역 배포를 고려합니다:

- **Active-Passive**: 주 지역이 활성, 보조 지역은 대기 (failover 시 활성화)
- **Active-Active**: 두 지역 모두 활성, 트래픽 분산

**Azure:**
- Azure Traffic Manager로 트래픽 분산
- Geo-replication으로 데이터베이스 복제

**AWS:**
- Route 53 Health Checks로 failover
- RDS Multi-AZ 또는 Cross-Region Read Replicas

**GCP:**
- Cloud Load Balancer로 multi-region 트래픽 분산
- Cloud SQL High Availability

## 보안 체크리스트: OWASP Top 10 대응

OWASP Top 10은 웹 애플리케이션의 가장 심각한 보안 위험 목록입니다. 각각에 대응해야 합니다.

### 1. Injection (SQL Injection, Command Injection)

**위험**: 악의적인 입력으로 데이터베이스 쿼리나 시스템 명령 조작

**방어:**
- ✅ **파라미터화된 쿼리 사용** (Entity Framework는 기본적으로 안전)
- ✅ 원시 SQL은 최소화, 필요 시 파라미터 사용
- ❌ 문자열 결합으로 쿼리 생성 금지

```csharp
// ❌ 취약: SQL Injection 위험
var userId = Request.Query["userId"];
var query = $"SELECT * FROM Users WHERE Id = {userId}";
var users = _context.Users.FromSqlRaw(query).ToList();

// ✅ 안전: 파라미터화
var userId = Request.Query["userId"];
var users = _context.Users
    .FromSqlRaw("SELECT * FROM Users WHERE Id = {0}", userId)
    .ToList();

// ✅ 더 나음: LINQ
var userId = int.Parse(Request.Query["userId"]);
var users = _context.Users.Where(u => u.Id == userId).ToList();
```

### 2. Broken Authentication

**위험**: 약한 인증으로 계정 탈취

**방어:**
- ✅ 강력한 비밀번호 정책 (ASP.NET Core Identity 기본 제공)
- ✅ 2FA (Two-Factor Authentication) 활성화
- ✅ 비밀번호 해싱 (Bcrypt, Argon2, PBKDF2)
- ✅ 세션 타임아웃
- ❌ 평문 비밀번호 저장 금지

```csharp
// ASP.NET Core Identity 비밀번호 정책
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 12;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
});
```

### 3. Sensitive Data Exposure

**위험**: 민감한 데이터 (비밀번호, 신용카드) 노출

**방어:**
- ✅ HTTPS 강제
- ✅ 비밀번호 해싱 (저장 시)
- ✅ 암호화 (at rest, in transit)
- ✅ Key Vault로 비밀 관리
- ❌ 로그에 민감한 데이터 출력 금지

### 4. XML External Entities (XXE)

**위험**: XML 파싱 시 외부 엔티티로 공격

**방어:**
- ✅ XML 외부 엔티티 비활성화
- ✅ JSON 사용 선호 (더 안전)

### 5. Broken Access Control

**위험**: 권한 없는 사용자가 리소스 접근

**방어:**
- ✅ 모든 엔드포인트에 인증/권한 부여
- ✅ 최소 권한 원칙
- ✅ 리소스 기반 권한 부여

```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("/api/users/{id}")]
public async Task<IActionResult> DeleteUser(int id)
{
    // Admin만 삭제 가능
}

[Authorize]
[HttpGet("/api/users/{id}/profile")]
public async Task<IActionResult> GetProfile(int id)
{
    // 자신의 프로필만 접근 가능
    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    if (currentUserId != id && !User.IsInRole("Admin"))
        return Forbid();

    // ...
}
```

### 6. Security Misconfiguration

**위험**: 잘못된 설정으로 인한 취약점

**방어:**
- ✅ 프로덕션에서 디버그 모드 비활성화
- ✅ 불필요한 기능 비활성화
- ✅ 보안 헤더 설정 (다음 섹션)
- ✅ 정기 업데이트

### 7. Cross-Site Scripting (XSS)

**위험**: 악의적인 스크립트 주입

**방어:**
- ✅ Razor는 자동으로 HTML 인코딩
- ✅ `@Html.Raw()` 사용 최소화
- ✅ Content Security Policy (CSP) 헤더

```csharp
// ✅ 안전: 자동 인코딩
<p>@Model.UserInput</p>

// ❌ 위험: 원시 HTML
<p>@Html.Raw(Model.UserInput)</p>
```

### 8. Insecure Deserialization

**위험**: 신뢰할 수 없는 데이터 역직렬화로 원격 코드 실행

**방어:**
- ✅ 입력 검증
- ✅ 타입 검증
- ❌ BinaryFormatter 사용 금지 (알려진 취약점)

### 9. Using Components with Known Vulnerabilities

**위험**: 취약한 라이브러리 사용

**방어:**
- ✅ NuGet 패키지 정기 업데이트
- ✅ `dotnet list package --vulnerable` 실행
- ✅ Dependabot 활성화 (GitHub)

```bash
dotnet list package --vulnerable
# 취약한 패키지 업데이트
dotnet add package <PackageName> --version <LatestVersion>
```

### 10. Insufficient Logging & Monitoring

**위험**: 보안 사고를 감지하지 못함

**방어:**
- ✅ 모든 인증 시도 로깅
- ✅ 권한 부여 실패 로깅
- ✅ 이상 패턴 감지
- ✅ 알림 설정

### 보안 헤더 설정

```csharp
app.Use(async (context, next) =>
{
    // XSS 방어
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

    // CSP
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'");

    // HSTS (이미 다룸)
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

    // Referrer 정책
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

    // Permissions Policy
    context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

    await next();
});
```

## 프로덕션 준비 체크리스트

배포 전에 다음 체크리스트를 확인하세요:

### 환경 구성
- [ ] `appsettings.Production.json`에 민감한 정보 없음
- [ ] 환경 변수 또는 Key Vault로 비밀 관리
- [ ] 로그 레벨 적절히 설정 (Warning 이상)
- [ ] ASPNETCORE_ENVIRONMENT=Production 설정

### 보안
- [ ] HTTPS 강제 활성화
- [ ] HSTS 헤더 설정
- [ ] 보안 헤더 (CSP, X-Frame-Options 등) 설정
- [ ] CORS 정책 정확히 구성
- [ ] Rate Limiting 활성화
- [ ] 취약점 스캔 실행 (`dotnet list package --vulnerable`)

### 성능
- [ ] 응답 압축 활성화 (Gzip, Brotli)
- [ ] 응답 캐싱 설정
- [ ] 데이터베이스 인덱스 확인
- [ ] N+1 쿼리 해결
- [ ] Connection Pooling 확인

### 모니터링
- [ ] Application Insights / CloudWatch / Cloud Monitoring 통합
- [ ] 구조화된 로깅 (Serilog)
- [ ] 헬스 체크 엔드포인트 (`/health`)
- [ ] 알림 규칙 설정 (에러율, 응답 시간)

### 복원력
- [ ] Global Exception Handler 설정
- [ ] Polly로 Retry, Circuit Breaker 구현
- [ ] 외부 API 타임아웃 설정
- [ ] 데이터베이스 백업 자동화
- [ ] DR (재해 복구) 계획 수립

### 인프라
- [ ] 자동 확장 (Auto Scaling) 설정
- [ ] 로드 밸런서 구성
- [ ] SSL/TLS 인증서 설정 및 자동 갱신
- [ ] 다중 리전 고려 (높은 가용성)

## 요약: 프로덕션은 지속적인 노력

프로덕션 배포는 끝이 아닙니다. 지속적인 모니터링, 업데이트, 최적화가 필요합니다. 이 챕터에서 다룬 모든 고려사항을 체크리스트로 만들어, 정기적으로 검토하세요.

**핵심 원칙:**
1. **비밀은 절대 Git에 커밋하지 마세요**
2. **HTTPS는 필수입니다**
3. **모든 것을 로그하고 모니터링하세요**
4. **실패를 예상하고 대비하세요**
5. **보안은 지속적인 과정입니다**

다음 Part 12에서는 지금까지 배운 모든 것을 종합하는 실전 프로젝트를 진행합니다. 전자상거래 플랫폼을 처음부터 끝까지 구축하며, 아키텍처, 코드, 테스트, 배포, 모니터링—전체 생명주기를 경험합니다.

---

## 참고 자료

- [ASP.NET Core Security](https://docs.microsoft.com/aspnet/core/security/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Azure Key Vault](https://docs.microsoft.com/azure/key-vault/)
- [AWS Secrets Manager](https://docs.aws.amazon.com/secretsmanager/)
- [Polly](https://github.com/App-vNext/Polly)
- [.NET Rate Limiting](https://learn.microsoft.com/aspnet/core/performance/rate-limit)
