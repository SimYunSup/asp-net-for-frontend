# Chapter 23: 모니터링과 로깅

## 프로덕션의 가시성: 보이지 않으면 관리할 수 없다

개발 환경에서는 모든 것이 완벽하게 작동합니다. 디버거로 코드를 단계별로 실행하고, 로그를 콘솔에서 즉시 확인하며, 문제가 발생하면 즉시 수정합니다. 하지만 프로덕션은 다릅니다. 수천 명의 사용자가 동시에 접속하고, 예상치 못한 데이터가 들어오며, 네트워크는 불안정하고, 외부 API는 가끔 실패합니다.

"사이트가 느려요"—사용자의 이 불평에 어떻게 대응하시겠습니까? 어느 API가 느린지, 어떤 데이터베이스 쿼리가 병목인지, 언제부터 느려졌는지... 이 모든 질문에 답하려면 **가시성(Observability)**이 필요합니다. 프론트엔드에서 Sentry로 에러를 추적하고, Google Analytics로 사용자 행동을 분석하며, Lighthouse로 성능을 측정했듯이, 백엔드에도 포괄적인 모니터링이 필수입니다.

이 장에서는 Application Insights로 자동 텔레메트리를 수집하고, Serilog로 구조화된 로그를 작성하며, OpenTelemetry로 분산 추적을 구현하고, Prometheus와 Grafana로 메트릭 대시보드를 구축하는 방법을 배웁니다.

## Application Insights: 포괄적인 APM 솔루션

Application Insights는 Azure의 애플리케이션 성능 관리(APM) 서비스입니다. 단 몇 줄의 코드로 요청, 의존성, 예외, 성능 카운터를 자동으로 수집하며, 강력한 쿼리와 시각화를 제공합니다. 프론트엔드의 Datadog이나 New Relic과 유사하지만, .NET과 긴밀히 통합되어 있습니다.

### 설정과 통합

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Application Insights 추가 - 단 한 줄!
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
```

`appsettings.json`에 연결 문자열 추가:

```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=...;IngestionEndpoint=https://...;LiveEndpoint=https://..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  }
}
```

이제 자동으로 다음이 수집됩니다:
- **HTTP 요청**: 모든 API 호출의 URL, 메서드, 응답 코드, 지연 시간
- **의존성**: 데이터베이스 쿼리, HTTP 클라이언트 호출, Redis 명령
- **예외**: 처리되지 않은 예외의 스택 트레이스
- **성능 카운터**: CPU, 메모리, GC 통계
- **로그**: `ILogger`로 작성한 모든 로그

### 사용자 정의 이벤트와 메트릭

자동 텔레메트리 외에도, 비즈니스 메트릭을 추적할 수 있습니다:

```csharp
public class OrderService
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<OrderService> _logger;

    public OrderService(TelemetryClient telemetryClient, ILogger<OrderService> logger)
    {
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 주문 처리 로직
            var order = new Order
            {
                UserId = request.UserId,
                Items = request.Items,
                TotalAmount = request.Items.Sum(i => i.Price * i.Quantity)
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            stopwatch.Stop();

            // 사용자 정의 메트릭: 주문 금액
            _telemetryClient.TrackMetric("Order.TotalAmount", order.TotalAmount);

            // 사용자 정의 이벤트: 주문 생성
            _telemetryClient.TrackEvent("OrderCreated", new Dictionary<string, string>
            {
                { "OrderId", order.Id.ToString() },
                { "UserId", request.UserId.ToString() },
                { "ItemCount", request.Items.Count.ToString() }
            }, new Dictionary<string, double>
            {
                { "TotalAmount", (double)order.TotalAmount },
                { "ProcessingTime", stopwatch.ElapsedMilliseconds }
            });

            _logger.LogInformation("Order {OrderId} created for user {UserId} with total {TotalAmount:C}",
                order.Id, request.UserId, order.TotalAmount);

            return order;
        }
        catch (Exception ex)
        {
            // 사용자 정의 예외 추적 (컨텍스트 추가)
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "UserId", request.UserId.ToString() },
                { "Operation", "CreateOrder" }
            });

            _logger.LogError(ex, "Failed to create order for user {UserId}", request.UserId);
            throw;
        }
    }

    public async Task ProcessPaymentAsync(int orderId, string cardToken)
    {
        // 종속성 추적 (외부 API 호출)
        using var operation = _telemetryClient.StartOperation<DependencyTelemetry>("ProcessPayment");
        operation.Telemetry.Type = "Payment Gateway";
        operation.Telemetry.Target = "https://payment.example.com";

        try
        {
            // 실제 결제 처리
            var result = await _paymentGateway.ChargeAsync(cardToken, amount);

            operation.Telemetry.Success = result.Success;
            operation.Telemetry.ResultCode = result.StatusCode;

            return result;
        }
        catch (Exception ex)
        {
            operation.Telemetry.Success = false;
            _telemetryClient.TrackException(ex);
            throw;
        }
    }
}
```

### Application Insights 포털

Azure Portal의 Application Insights에서 다음을 볼 수 있습니다:

**1. Application Map**: 서비스 간 의존성 시각화

```
┌─────────────┐
│ Frontend    │
└──────┬──────┘
       │ 1200 req/min
       │ avg 45ms
       ▼
┌─────────────┐     ┌──────────────┐
│ API Server  ├────►│ SQL Database │
└──────┬──────┘     └──────────────┘
       │              650 queries/min
       │              avg 12ms
       │
       │ ┌──────────────────┐
       └►│ Payment Gateway  │
         └──────────────────┘
           120 calls/min
           avg 180ms (느림!)
```

Payment Gateway가 병목임이 즉시 보입니다.

**2. Live Metrics**: 실시간 대시보드

- 초당 요청 수 (실시간 그래프)
- 실패한 요청 (빨간색 강조)
- 현재 서버 메모리, CPU
- 실시간 로그 스트림

디버깅 중이거나 배포 직후 모니터링할 때 유용합니다.

**3. Failures**: 실패 분석

모든 예외와 실패한 요청을 집계하여 표시:
- 가장 흔한 예외 타입
- 영향받은 사용자 수
- 시간대별 실패율
- 스택 트레이스와 요청 세부 정보

**4. Performance**: 느린 요청 식별

- 엔드포인트별 평균 응답 시간
- P50, P90, P95, P99 백분위수
- 느린 의존성 (DB 쿼리, HTTP 호출)
- 시간대별 성능 추세

### Kusto Query Language (KQL)

Application Insights의 진정한 힘은 KQL입니다. SQL과 유사하지만 시계열 데이터에 특화되어 있습니다.

**가장 느린 10개의 API 엔드포인트:**

```kql
requests
| where timestamp > ago(1h)
| summarize count(), avg(duration), percentile(duration, 95) by name
| order by avg_duration desc
| take 10
```

**시간대별 오류율:**

```kql
requests
| where timestamp > ago(24h)
| summarize
    total = count(),
    failed = countif(success == false)
    by bin(timestamp, 1h)
| project timestamp, errorRate = (failed * 100.0) / total
| render timechart
```

**사용자 정의 이벤트 분석 (주문 금액 분포):**

```kql
customEvents
| where name == "OrderCreated"
| where timestamp > ago(7d)
| extend totalAmount = todouble(customMeasurements.TotalAmount)
| summarize
    count = count(),
    avg_amount = avg(totalAmount),
    total_revenue = sum(totalAmount)
    by bin(timestamp, 1d)
| render timechart
```

**가장 느린 데이터베이스 쿼리:**

```kql
dependencies
| where type == "SQL"
| where timestamp > ago(1h)
| where duration > 100 // 100ms 이상
| summarize count(), avg(duration), max(duration) by name, target
| order by avg_duration desc
| take 20
```

**특정 사용자의 요청 추적:**

```kql
union requests, dependencies, traces, exceptions
| where timestamp > ago(1h)
| where customDimensions.UserId == "12345"
| order by timestamp asc
| project timestamp, itemType, name, duration, message
```

이제 한 사용자의 전체 요청 흐름을 시간순으로 볼 수 있습니다.

## Serilog: 구조화된 로깅

전통적인 로깅은 문자열입니다: `logger.LogInformation("User 123 logged in")`. 하지만 이는 검색과 분석이 어렵습니다. 구조화된 로깅은 데이터를 필드로 저장하여, 쿼리 가능하게 만듭니다.

### Serilog 설정

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.ApplicationInsights
```

```csharp
// Program.cs
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog 구성
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "MyApp")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("logs/app-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .WriteTo.ApplicationInsights(
        builder.Configuration["ApplicationInsights:ConnectionString"],
        TelemetryConverter.Traces)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting web application");

    // 앱 구성 및 실행
    var app = builder.Build();
    app.MapControllers();
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

### 구조화된 로그 작성

```csharp
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        // 구조화된 로깅: 필드로 저장
        _logger.LogInformation(
            "Creating order for user {UserId} with {ItemCount} items, total {TotalAmount:C}",
            request.UserId,
            request.Items.Count,
            request.TotalAmount
        );

        try
        {
            var order = await _orderService.CreateOrderAsync(request);

            _logger.LogInformation(
                "Order {OrderId} created successfully for user {UserId}",
                order.Id,
                request.UserId
            );

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (PaymentFailedException ex)
        {
            _logger.LogWarning(ex,
                "Payment failed for user {UserId} with card {CardLast4}",
                request.UserId,
                request.CardToken.Substring(request.CardToken.Length - 4)
            );

            return BadRequest(new { error = "Payment processing failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create order for user {UserId}",
                request.UserId
            );

            return StatusCode(500);
        }
    }
}
```

이는 다음과 같은 JSON으로 저장됩니다:

```json
{
  "timestamp": "2025-01-15T10:30:00.123Z",
  "level": "Information",
  "messageTemplate": "Creating order for user {UserId} with {ItemCount} items, total {TotalAmount}",
  "message": "Creating order for user 123 with 5 items, total $99.99",
  "properties": {
    "UserId": 123,
    "ItemCount": 5,
    "TotalAmount": 99.99,
    "MachineName": "web-server-01",
    "ThreadId": 42,
    "Application": "MyApp"
  }
}
```

이제 "TotalAmount > 1000인 모든 주문"을 쉽게 쿼리할 수 있습니다.

### LogContext로 동적 컨텍스트 추가

요청마다 공통 정보를 모든 로그에 자동으로 추가하려면 `LogContext`를 사용합니다:

```csharp
// 미들웨어로 요청마다 컨텍스트 추가
app.Use(async (context, next) =>
{
    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var requestId = context.TraceIdentifier;

    using (LogContext.PushProperty("UserId", userId))
    using (LogContext.PushProperty("RequestId", requestId))
    using (LogContext.PushProperty("UserAgent", context.Request.Headers.UserAgent.ToString()))
    {
        await next();
    }
});

// 이제 컨트롤러에서
_logger.LogInformation("Processing order");
// 자동으로 UserId, RequestId, UserAgent가 포함됨!
```

### 커스텀 Enricher

모든 로그에 환경 변수나 커스텀 정보를 추가할 수 있습니다:

```csharp
public class EnvironmentEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

        logEvent.AddPropertyIfAbsent(factory.CreateProperty("Environment", environment));
        logEvent.AddPropertyIfAbsent(factory.CreateProperty("Version", version));
    }
}

// 등록
Log.Logger = new LoggerConfiguration()
    .Enrich.With<EnvironmentEnricher>()
    .WriteTo.Console()
    .CreateLogger();
```

## OpenTelemetry: 분산 추적

마이크로서비스 아키텍처에서는 하나의 사용자 요청이 여러 서비스를 거칩니다. "주문 생성"이 Order Service → Inventory Service → Payment Service → Notification Service를 거친다면, 어디서 지연이 발생하는지 어떻게 알 수 있을까요?

분산 추적은 요청에 고유 ID (Trace ID)를 부여하고, 각 서비스가 자신의 작업 (Span)을 기록합니다. 모든 Span을 연결하면 전체 요청 흐름을 시각화할 수 있습니다.

### OpenTelemetry 설정

```bash
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.SqlClient
dotnet add package OpenTelemetry.Exporter.Jaeger
```

```csharp
// Program.cs
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("OrderService", serviceVersion: "1.0.0"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation(options =>
        {
            // HTTP 요청 자동 추적
            options.RecordException = true;
        })
        .AddHttpClientInstrumentation(options =>
        {
            // HttpClient 호출 자동 추적
            options.RecordException = true;
        })
        .AddSqlClientInstrumentation(options =>
        {
            // SQL 쿼리 자동 추적
            options.SetDbStatementForText = true;
            options.RecordException = true;
        })
        .AddJaegerExporter(options =>
        {
            // Jaeger로 추적 데이터 전송
            options.AgentHost = "localhost";
            options.AgentPort = 6831;
        })
    );
```

이제 모든 HTTP 요청, HTTP 클라이언트 호출, SQL 쿼리가 자동으로 추적됩니다!

### 커스텀 Span 생성

비즈니스 로직에 커스텀 Span을 추가하여 더 상세한 추적을 할 수 있습니다:

```csharp
public class OrderService
{
    private static readonly ActivitySource ActivitySource = new("OrderService");

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        // 커스텀 Span 시작
        using var activity = ActivitySource.StartActivity("CreateOrder", ActivityKind.Internal);

        // 태그 추가 (필터링과 검색에 유용)
        activity?.SetTag("order.userId", request.UserId);
        activity?.SetTag("order.itemCount", request.Items.Count);
        activity?.SetTag("order.totalAmount", request.TotalAmount);

        try
        {
            // 1. 재고 확인
            using (var checkInventoryActivity = ActivitySource.StartActivity("CheckInventory"))
            {
                await CheckInventoryAsync(request.Items);
                checkInventoryActivity?.SetTag("inventory.available", true);
            }

            // 2. 결제 처리
            using (var processPaymentActivity = ActivitySource.StartActivity("ProcessPayment"))
            {
                var paymentResult = await ProcessPaymentAsync(request);
                processPaymentActivity?.SetTag("payment.success", paymentResult.Success);
                processPaymentActivity?.SetTag("payment.transactionId", paymentResult.TransactionId);
            }

            // 3. 주문 생성
            var order = new Order { /* ... */ };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // 이벤트 추가 (타임라인에 마커)
            activity?.AddEvent(new ActivityEvent("OrderCreated", tags: new ActivityTagsCollection
            {
                { "orderId", order.Id }
            }));

            activity?.SetTag("order.id", order.Id);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return order;
        }
        catch (Exception ex)
        {
            // 예외 기록
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }
}

// ActivitySource를 OpenTelemetry에 등록
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("OrderService") // 여기!
        .AddAspNetCoreInstrumentation()
        // ...
    );
```

### Jaeger UI로 추적 시각화

Jaeger를 Docker로 실행:

```bash
docker run -d --name jaeger \
  -p 6831:6831/udp \
  -p 16686:16686 \
  jaegertracing/all-in-one:latest
```

http://localhost:16686 에서 Jaeger UI를 열면, 다음과 같은 타임라인을 볼 수 있습니다:

```
Trace ID: 1a2b3c4d5e6f7g8h9i0j
Duration: 450ms

OrderService: POST /api/orders                            [========================================] 450ms
  ├─ CreateOrder                                          [====================================]     420ms
  │  ├─ CheckInventory                                    [====]                                     50ms
  │  │  └─ SQL: SELECT * FROM Products WHERE Id IN (...)  [==]                                       20ms
  │  ├─ ProcessPayment                                    [====================]                    200ms
  │  │  └─ HTTP POST https://payment.example.com/charge   [==================]                     180ms
  │  └─ SQL: INSERT INTO Orders (...)                     [=]                                        10ms
  └─ Event: OrderCreated (320ms)
```

이제 Payment Service의 HTTP 호출이 180ms로 가장 느리다는 것이 명확합니다.

**분산 추적 예: 마이크로서비스 간**

Order Service에서 Inventory Service를 HTTP로 호출할 때, Trace ID가 자동으로 전파됩니다:

```csharp
// Order Service
public async Task<bool> CheckInventoryAsync(List<OrderItem> items)
{
    using var activity = ActivitySource.StartActivity("CheckInventory");

    // HttpClient는 자동으로 Trace ID를 헤더에 추가
    var response = await _httpClient.PostAsJsonAsync(
        "http://inventory-service/api/inventory/check",
        items
    );

    return response.IsSuccessStatusCode;
}

// Inventory Service (다른 프로세스)
[HttpPost("check")]
public async Task<IActionResult> CheckInventory(List<OrderItem> items)
{
    // ASP.NET Core는 자동으로 Trace ID를 헤더에서 추출
    // 같은 Trace ID로 Span 생성

    using var activity = ActivitySource.StartActivity("CheckInventoryInDatabase");

    var available = await _inventoryService.CheckAsync(items);
    return Ok(new { available });
}
```

Jaeger에서 보면 두 서비스가 하나의 Trace로 연결되어 표시됩니다:

```
Order Service → Inventory Service
[=======================================] 100ms
                [====]                     20ms
```

## Prometheus와 Grafana: 메트릭 대시보드

메트릭은 애플리케이션의 수치적 측정입니다: 요청 수, 응답 시간, 메모리 사용량, 활성 사용자 수 등. 시계열 데이터로 저장되어, 추세를 파악하고 이상을 감지할 수 있습니다.

### Prometheus 통합

```bash
dotnet add package prometheus-net.AspNetCore
```

```csharp
// Program.cs
using Prometheus;

var app = builder.Build();

// HTTP 요청 메트릭 자동 수집
app.UseHttpMetrics();

app.MapControllers();

// /metrics 엔드포인트 노출 (Prometheus가 스크랩)
app.MapMetrics();

app.Run();
```

이제 http://localhost:5000/metrics 에 접속하면 Prometheus 형식의 메트릭을 볼 수 있습니다:

```
# HELP http_requests_received_total Total number of HTTP requests received
# TYPE http_requests_received_total counter
http_requests_received_total{method="GET",code="200",controller="Products",action="GetAll"} 1234

# HELP http_request_duration_seconds HTTP request duration in seconds
# TYPE http_request_duration_seconds histogram
http_request_duration_seconds_bucket{method="GET",code="200",le="0.1"} 1000
http_request_duration_seconds_bucket{method="GET",code="200",le="0.5"} 1200
http_request_duration_seconds_bucket{method="GET",code="200",le="1"} 1230
http_request_duration_seconds_sum{method="GET",code="200"} 85.3
http_request_duration_seconds_count{method="GET",code="200"} 1234
```

### 커스텀 메트릭 생성

비즈니스 메트릭을 추적할 수 있습니다:

```csharp
public class OrderService
{
    // Counter: 누적 증가만 가능 (주문 수, 에러 수 등)
    private static readonly Counter OrdersCreated = Metrics
        .CreateCounter("orders_created_total", "Total number of orders created",
            new CounterConfiguration
            {
                LabelNames = new[] { "status", "payment_method" }
            });

    // Gauge: 증가/감소 가능 (활성 사용자, 큐 길이 등)
    private static readonly Gauge ActiveOrders = Metrics
        .CreateGauge("orders_active", "Number of orders being processed");

    // Histogram: 값의 분포 (응답 시간, 주문 금액 등)
    private static readonly Histogram OrderAmount = Metrics
        .CreateHistogram("order_amount_dollars", "Order amount in dollars",
            new HistogramConfiguration
            {
                Buckets = new[] { 10, 50, 100, 200, 500, 1000, 5000 }
            });

    // Summary: 백분위수 (P50, P90, P99)
    private static readonly Summary OrderProcessingTime = Metrics
        .CreateSummary("order_processing_duration_seconds", "Order processing time",
            new SummaryConfiguration
            {
                Objectives = new[]
                {
                    new QuantileEpsilonPair(0.5, 0.05),  // P50
                    new QuantileEpsilonPair(0.9, 0.01),  // P90
                    new QuantileEpsilonPair(0.99, 0.001) // P99
                }
            });

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        using (ActiveOrders.TrackInProgress()) // 진행 중 +1, 완료 시 -1
        using (OrderProcessingTime.NewTimer()) // 자동으로 실행 시간 측정
        {
            try
            {
                var order = await ProcessOrderAsync(request);

                // Counter 증가
                OrdersCreated.WithLabels(
                    status: "success",
                    payment_method: request.PaymentMethod
                ).Inc();

                // Histogram 기록
                OrderAmount.Observe((double)order.TotalAmount);

                return order;
            }
            catch (Exception ex)
            {
                OrdersCreated.WithLabels(
                    status: "failed",
                    payment_method: request.PaymentMethod
                ).Inc();

                throw;
            }
        }
    }
}
```

### Prometheus 서버 설정

Prometheus를 Docker로 실행하고, 앱을 스크랩하도록 설정합니다:

```yaml
# prometheus.yml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'myapp'
    static_configs:
      - targets: ['host.docker.internal:5000']
```

```bash
docker run -d --name prometheus \
  -p 9090:9090 \
  -v $(pwd)/prometheus.yml:/etc/prometheus/prometheus.yml \
  prom/prometheus
```

http://localhost:9090 에서 Prometheus UI를 열어 쿼리할 수 있습니다:

```promql
# 초당 요청 수
rate(http_requests_received_total[5m])

# 평균 응답 시간
rate(http_request_duration_seconds_sum[5m]) / rate(http_request_duration_seconds_count[5m])

# P95 응답 시간
histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))

# 오류율
sum(rate(http_requests_received_total{code=~"5.."}[5m])) / sum(rate(http_requests_received_total[5m]))
```

### Grafana 대시보드

Grafana는 Prometheus 데이터를 시각화합니다:

```bash
docker run -d --name grafana \
  -p 3000:3000 \
  grafana/grafana
```

http://localhost:3000 (admin/admin)에서 다음을 수행:

1. **Data Source 추가**: Configuration → Data Sources → Add Prometheus (http://host.docker.internal:9090)
2. **Dashboard 생성**: Create → Dashboard → Add new panel

**대시보드 예시: API 성능**

- **패널 1**: 초당 요청 수 (RPS)
  ```promql
  sum(rate(http_requests_received_total[5m]))
  ```

- **패널 2**: 평균 응답 시간
  ```promql
  rate(http_request_duration_seconds_sum[5m]) / rate(http_request_duration_seconds_count[5m])
  ```

- **패널 3**: 엔드포인트별 응답 시간 (Heatmap)
  ```promql
  histogram_quantile(0.95, sum(rate(http_request_duration_seconds_bucket[5m])) by (le, controller, action))
  ```

- **패널 4**: 오류율
  ```promql
  sum(rate(http_requests_received_total{code=~"5.."}[5m])) / sum(rate(http_requests_received_total[5m])) * 100
  ```

- **패널 5**: 활성 주문 수
  ```promql
  orders_active
  ```

- **패널 6**: 주문 금액 분포
  ```promql
  histogram_quantile(0.5, rate(order_amount_dollars_bucket[5m]))
  ```

**알림 규칙 설정:**

1. 패널 설정 → Alert 탭
2. 조건 설정: "오류율이 1%를 초과하면"
   ```
   sum(rate(http_requests_received_total{code=~"5.."}[5m])) / sum(rate(http_requests_received_total[5m])) > 0.01
   ```
3. 알림 채널 설정: Email, Slack, PagerDuty 등

## 헬스 체크: 시스템의 심장박동

애플리케이션이 실행 중이라고 해서 건강한 것은 아닙니다. 데이터베이스 연결이 끊어졌거나, 디스크가 가득 찼거나, 중요한 의존성이 실패했을 수 있습니다.

```csharp
// Program.cs
services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .AddRedis(
        Configuration["Redis:ConnectionString"],
        name: "redis",
        timeout: TimeSpan.FromSeconds(5))
    .AddUrlGroup(
        new Uri("https://api.thirdparty.com/health"),
        name: "third-party-api",
        timeout: TimeSpan.FromSeconds(10))
    .AddCheck<CustomHealthCheck>("custom-check");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => true // 모든 체크
});
```

**커스텀 헬스 체크:**

```csharp
public class DiskSpaceHealthCheck : IHealthCheck
{
    private readonly long _thresholdBytes = 1024 * 1024 * 1024; // 1 GB

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var drive = DriveInfo.GetDrives().First(d => d.Name == "/");
        var freeSpace = drive.AvailableFreeSpace;

        if (freeSpace < _thresholdBytes)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                $"Disk space is low: {freeSpace / 1024 / 1024} MB remaining"
            ));
        }

        if (freeSpace < _thresholdBytes * 2)
        {
            return Task.FromResult(HealthCheckResult.Degraded(
                $"Disk space is running low: {freeSpace / 1024 / 1024} MB remaining"
            ));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            $"Disk space is sufficient: {freeSpace / 1024 / 1024} MB remaining"
        ));
    }
}

services.AddHealthChecks()
    .AddCheck<DiskSpaceHealthCheck>("disk-space", tags: new[] { "ready" });
```

**응답 예시:**

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": {
    "database": {
      "status": "Healthy",
      "description": "Database connection is healthy",
      "duration": "00:00:00.0234567"
    },
    "redis": {
      "status": "Healthy",
      "duration": "00:00:00.0123456"
    },
    "third-party-api": {
      "status": "Degraded",
      "description": "Response time is slow",
      "duration": "00:00:00.5123456"
    },
    "disk-space": {
      "status": "Healthy",
      "description": "Disk space is sufficient: 50000 MB remaining",
      "duration": "00:00:00.0001234"
    }
  }
}
```

**Kubernetes 통합:**

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: myapp
spec:
  containers:
  - name: myapp
    image: myapp:latest
    livenessProbe:
      httpGet:
        path: /health/live
        port: 8080
      initialDelaySeconds: 30
      periodSeconds: 10
    readinessProbe:
      httpGet:
        path: /health/ready
        port: 8080
      initialDelaySeconds: 10
      periodSeconds: 5
```

Kubernetes는 `/health/ready`가 실패하면 트래픽을 중단하고, `/health/live`가 실패하면 컨테이너를 재시작합니다.

## .NET 9: Kestrel 연결 메트릭

.NET 9는 Kestrel의 연결 메트릭을 노출합니다:

```csharp
// Program.cs (.NET 9)
builder.Services.AddMetrics();

var app = builder.Build();

// 자동으로 다음 메트릭 노출:
// - kestrel.active_connections: 현재 활성 연결 수
// - kestrel.connection_duration: 연결 지속 시간
// - kestrel.rejected_connections: 거부된 연결 수
// - kestrel.queued_connections: 큐에 대기 중인 연결 수
// - kestrel.queued_requests: 큐에 대기 중인 요청 수
// - kestrel.upgraded_connections: WebSocket 등으로 업그레이드된 연결 수
```

Prometheus로 수집하고 Grafana로 시각화하여, 연결 포화 상태를 모니터링할 수 있습니다.

## 모니터링 모범 사례

- ✅ **골든 시그널 추적**: Latency, Traffic, Errors, Saturation
- ✅ **구조화된 로깅**: 쿼리 가능한 필드로 저장
- ✅ **분산 추적**: 마이크로서비스 간 요청 흐름 시각화
- ✅ **메트릭 대시보드**: 추세와 이상 감지
- ✅ **헬스 체크**: 자동 장애 감지와 복구
- ✅ **알림 설정**: 중요 메트릭 임계값 초과 시 통보
- ✅ **On-Call 문화**: 알림에 대한 명확한 대응 절차

## 마무리

"보이지 않으면 관리할 수 없다"—이 원칙은 프로덕션 환경에서 특히 중요합니다. 이제 여러분은 포괄적인 가시성을 갖추었습니다:

- **Application Insights**로 자동 텔레메트리 수집
- **Serilog**로 구조화된 로그 작성
- **OpenTelemetry**로 분산 추적 구현
- **Prometheus + Grafana**로 메트릭 대시보드 구축
- **헬스 체크**로 자동 장애 감지

사용자가 "사이트가 느려요"라고 불평하면, 이제 정확히 어느 엔드포인트가, 어떤 데이터베이스 쿼리가, 언제부터 느려졌는지 알 수 있습니다. 그리고 문제가 발생하기 전에 알림을 받아, 사용자가 영향받기 전에 해결할 수 있습니다.

Part 10을 마무리하면서, 여러분은 이제 성능을 측정하고 최적화하며, 프로덕션에서 모니터링하는 완전한 사이클을 마스터했습니다. 다음 Part 11에서는 애플리케이션을 실제 프로덕션에 배포하는 방법—Docker, Kubernetes, Azure—을 배웁니다.
