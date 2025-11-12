---
title: "Chapter 22 - 성능 최적화 기법"
---

# Chapter 22: 성능 최적화 기법

## 측정하고, 분석하고, 최적화하라

"조기 최적화는 만악의 근원이다"—Donald Knuth의 이 유명한 경고는 많은 개발자에게 성능 최적화를 미루는 핑계가 되곤 합니다. 하지만 Knuth는 또한 "우리는 97%의 시간에 작은 비효율성을 무시해야 한다"고 말했습니다. 이는 역설적으로 **3%의 중요한 부분은 최적화해야 한다**는 의미입니다.

핵심은 **추측하지 말고 측정하라**입니다. 프론트엔드 개발자로서 여러분은 Chrome DevTools의 Performance 탭으로 렌더링 병목을 찾고, Lighthouse로 성능 점수를 측정하며, webpack bundle analyzer로 번들 크기를 최적화해봤을 것입니다. ASP.NET Core도 동일한 철학을 따르지만, 서버 측 특성에 맞는 강력한 도구들을 제공합니다.

이 장에서는 프로파일링으로 병목을 찾고, 벤치마킹으로 정확히 측정하며, 다양한 최적화 기법을 적용하고, 결과를 검증하는 완전한 사이클을 배웁니다.

## 프로파일링: 병목을 찾아라

프로파일러는 애플리케이션 실행을 분석하여, 어떤 메서드가 시간을 가장 많이 소비하는지, 어디서 메모리 할당이 많이 발생하는지 보여줍니다. Chrome DevTools의 Performance 탭과 유사하지만, 서버 측 코드에 특화되어 있습니다.

### Visual Studio Profiler

Visual Studio에 내장된 프로파일러는 가장 접근하기 쉽고 강력합니다.

**CPU 프로파일링 시작하기:**

1. Visual Studio에서 프로젝트를 엽니다
2. **Debug → Performance Profiler** (Alt+F2)
3. **CPU Usage** 체크
4. **Start** 클릭
5. 애플리케이션을 사용하여 부하 생성 (API 호출, 페이지 탐색 등)
6. **Stop Collection**

결과 화면에서 다음을 볼 수 있습니다:

- **Hot Path**: 가장 많은 시간을 소비한 호출 스택
- **Functions 뷰**: 각 메서드의 실행 시간 (Self Time vs Total Time)
- **Caller/Callee**: 누가 이 메서드를 호출했고, 이 메서드가 누구를 호출했는지
- **Timeline**: 시간대별 CPU 사용률

예를 들어, 다음과 같은 패턴을 발견할 수 있습니다:

```
GetOrders()                      Total: 2500ms   Self: 50ms
└─ _context.Orders.ToListAsync() Total: 2450ms   Self: 2450ms
```

이는 데이터베이스 쿼리가 병목임을 명확히 보여줍니다.

**메모리 프로파일링:**

Memory Usage 프로파일러는 힙 할당을 추적합니다:

1. **Memory Usage** 체크
2. **Start**
3. 부하 생성
4. **Take Snapshot** (여러 번, 시간 간격을 두고)
5. 스냅샷 간 차이를 비교

결과는 다음을 보여줍니다:
- 어떤 타입이 가장 많은 메모리를 소비하는지
- 메모리 누수가 있는지 (GC 후에도 메모리가 증가)
- 어떤 코드 경로가 할당을 유발하는지

```csharp
// 예: 메모리 문제 발견
public async Task<List<ProductDto>> GetProductsAsync()
{
    var products = await _context.Products.ToListAsync(); // 10,000개 Product 엔티티

    // 각 Product를 ProductDto로 변환 - 추가 10,000개 객체 할당!
    return products.Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price
    }).ToList();
}
```

프로파일러는 20,000개의 객체 할당을 보여주며, 이는 최적화 대상입니다:

```csharp
// 최적화: DB에서 직접 DTO로 프로젝션
public async Task<List<ProductDto>> GetProductsAsync()
{
    return await _context.Products
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        })
        .ToListAsync();
    // EF Core가 SQL에서 직접 DTO 생성 - Product 엔티티 할당 제거!
}
```

### dotnet-trace: 프로덕션 프로파일링

Visual Studio 프로파일러는 강력하지만 개발 환경에 제한됩니다. 프로덕션에서 성능 문제가 발생하면 어떻게 할까요? `dotnet-trace`는 실행 중인 애플리케이션을 프로파일링할 수 있습니다.

```bash
# dotnet-trace 설치
dotnet tool install --global dotnet-trace

# 실행 중인 프로세스 찾기
dotnet-trace ps

# 30초 동안 추적
dotnet-trace collect --process-id 12345 --duration 00:00:30

# 생성된 .nettrace 파일을 Visual Studio나 PerfView로 분석
```

낮은 오버헤드로 실행되므로, 프로덕션 환경에서도 안전하게 사용할 수 있습니다. 사용자에게 눈에 띄는 영향을 주지 않습니다.

### dotnet-counters: 실시간 메트릭

프로파일링은 상세한 분석을 제공하지만, 때로는 빠른 개요만 필요할 때가 있습니다. `dotnet-counters`는 실시간 성능 카운터를 표시합니다.

```bash
# dotnet-counters 설치
dotnet tool install --global dotnet-counters

# 실시간 모니터링
dotnet-counters monitor --process-id 12345

# 출력 예시:
# [System.Runtime]
#   CPU Usage (%)                              45
#   Working Set (MB)                          342
#   GC Heap Size (MB)                         128
#   Gen 0 GC Count                          1,234
#   Gen 1 GC Count                            234
#   Gen 2 GC Count                             12
#   Exception Count                            42
#   ThreadPool Thread Count                    16
#   Lock Contention Count                       5
```

이는 Node.js의 `process.memoryUsage()`와 유사하지만 훨씬 더 많은 정보를 제공합니다.

**GC (Garbage Collection) 메트릭 해석:**

- **Gen 0 GC Count**: 짧은 수명의 객체 수집 (빈번함, 빠름)
- **Gen 1 GC**: 중간 수명의 객체 (덜 빈번)
- **Gen 2 GC**: 오래 살아남은 객체 (드묾, 느림)

Gen 2 GC가 자주 발생하면 성능 문제입니다. 대용량 객체가 너무 오래 살아있거나, 메모리 누수가 있을 수 있습니다.

### PerfView: 고급 프로파일링

Microsoft의 PerfView는 무료지만 매우 강력한 프로파일러입니다. ETW (Event Tracing for Windows) 이벤트를 수집하여, .NET 런타임 내부까지 들여다볼 수 있습니다.

PerfView는 학습 곡선이 가파르지만, 다음 시나리오에서 유용합니다:
- GC 일시 중지 시간 분석
- JIT 컴파일 오버헤드 측정
- 스레드 경합 (lock contention) 찾기
- 할당 스택 추적 (어디서 메모리가 할당되는지)

```bash
# PerfView 다운로드: https://github.com/microsoft/perfview

# 수집 시작
PerfView.exe collect -MaxCollectSec:30

# 애플리케이션에 부하 생성

# 수집 중지 (자동으로 30초 후)

# .etl 파일을 PerfView에서 열고 분석
```

## BenchmarkDotNet: 정확한 측정

프로파일링이 "어디가 느린가"를 보여준다면, 벤치마킹은 "얼마나 느린가"를 정확히 측정합니다. 그리고 최적화 전후를 비교하여 실제로 개선되었는지 검증합니다.

### 첫 벤치마크 작성

BenchmarkDotNet은 나노초 단위까지 정확하게 측정하며, JIT 워밍업, GC 영향, 통계적 이상치를 모두 고려합니다.

```bash
dotnet new console -n MyBenchmarks
cd MyBenchmarks
dotnet add package BenchmarkDotNet
```

간단한 벤치마크:

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class StringConcatBenchmark
{
    private const int N = 1000;

    [Benchmark]
    public string UsingPlus()
    {
        string result = "";
        for (int i = 0; i < N; i++)
            result += i.ToString();
        return result;
    }

    [Benchmark]
    public string UsingStringBuilder()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < N; i++)
            sb.Append(i);
        return sb.ToString();
    }

    [Benchmark(Baseline = true)]
    public string UsingStringCreate()
    {
        return string.Create(N * 4, 0, (span, _) =>
        {
            int pos = 0;
            for (int i = 0; i < N; i++)
            {
                if (i.TryFormat(span.Slice(pos), out int written))
                    pos += written;
            }
        });
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<StringConcatBenchmark>();
    }
}
```

**Release 모드로 실행** (Debug는 최적화가 꺼져 있음):

```bash
dotnet run -c Release
```

결과:

```
|              Method |      Mean |    Error |   StdDev | Ratio |   Gen 0 |  Gen 1 |  Allocated | Alloc Ratio |
|-------------------- |----------:|---------:|---------:|------:|--------:|-------:|-----------:|------------:|
|          UsingPlus  | 284.5 μs | 5.2 μs  | 4.8 μs  | 23.17 | 125.00  |  62.50 |  256,120 B |     49.79   |
| UsingStringBuilder  |  12.3 μs | 0.2 μs  | 0.2 μs  |  1.00 |   2.50  |   1.25 |    5,144 B |      1.00   |
|   UsingStringCreate |   8.7 μs | 0.1 μs  | 0.1 μs  |  0.71 |   1.95  |   0.98 |    4,024 B |      0.78   |
```

**해석:**
- **Mean**: 평균 실행 시간. `StringBuilder`가 `+` 연산자보다 23배 빠름!
- **Ratio**: Baseline 대비 비율. `StringCreate`가 29% 더 빠름
- **Gen 0**: Gen 0 GC가 발생한 횟수 (1000회 실행당)
- **Allocated**: 총 메모리 할당량. `+` 연산자는 50배 더 많은 메모리 할당

이제 어떤 방법을 선택해야 할지 명확합니다: `StringBuilder` 또는 `string.Create`.

### 파라미터화된 벤치마크

여러 입력 크기로 테스트하려면 `[Params]`를 사용합니다:

```csharp
[MemoryDiagnoser]
public class JsonSerializationBenchmark
{
    [Params(10, 100, 1000)]
    public int N;

    private List<Product> _products = null!;

    [GlobalSetup]
    public void Setup()
    {
        _products = Enumerable.Range(1, N)
            .Select(i => new Product { Id = i, Name = $"Product {i}", Price = i * 10m })
            .ToList();
    }

    [Benchmark]
    public string SystemTextJson()
    {
        return JsonSerializer.Serialize(_products);
    }

    [Benchmark]
    public string NewtonsoftJson()
    {
        return JsonConvert.SerializeObject(_products);
    }
}
```

결과는 각 N 값에 대해 별도로 표시됩니다:

```
|          Method |    N |       Mean |
|---------------- |----- |-----------:|
| SystemTextJson  |   10 |   12.34 μs |
| NewtonsoftJson  |   10 |   18.56 μs |
| SystemTextJson  |  100 |   95.23 μs |
| NewtonsoftJson  |  100 |  142.67 μs |
| SystemTextJson  | 1000 |  923.45 μs |
| NewtonsoftJson  | 1000 | 1,456.78 μs |
```

`System.Text.Json`이 모든 크기에서 약 35-40% 빠릅니다.

### 실전 예제: Entity Framework 쿼리 최적화

EF Core의 여러 쿼리 방식을 벤치마크로 비교해봅시다:

```csharp
[MemoryDiagnoser]
public class EntityFrameworkBenchmark
{
    private ApplicationDbContext _context = null!;

    [GlobalSetup]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("BenchmarkDb")
            .Options;

        _context = new ApplicationDbContext(options);

        // 테스트 데이터 시드
        if (!_context.Products.Any())
        {
            _context.Products.AddRange(
                Enumerable.Range(1, 1000)
                    .Select(i => new Product
                    {
                        Id = i,
                        Name = $"Product {i}",
                        Price = i * 10m,
                        Category = $"Category {i % 10}"
                    })
            );
            _context.SaveChanges();
        }
    }

    [Benchmark(Baseline = true)]
    public async Task<List<Product>> WithTracking()
    {
        return await _context.Products
            .Where(p => p.Price > 100)
            .ToListAsync();
    }

    [Benchmark]
    public async Task<List<Product>> WithAsNoTracking()
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Price > 100)
            .ToListAsync();
    }

    [Benchmark]
    public async Task<List<ProductDto>> WithProjection()
    {
        return await _context.Products
            .Where(p => p.Price > 100)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })
            .ToListAsync();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
}
```

결과:

```
|           Method |      Mean |   Gen 0 |  Allocated |
|----------------- |----------:|--------:|-----------:|
|     WithTracking | 145.23 μs |  12.500 |   51,240 B |
| WithAsNoTracking | 102.34 μs |   8.125 |   33,456 B |
|   WithProjection |  87.65 μs |   6.250 |   25,672 B |
```

**결론**: `AsNoTracking()` + Projection이 41% 빠르고, 메모리도 50% 절약합니다.

## 비동기 프로그래밍 최적화

ASP.NET Core는 본질적으로 비동기적입니다. 하지만 비동기를 올바르게 사용하지 않으면, 이점을 얻지 못하거나 오히려 성능이 저하될 수 있습니다.

### async/await 모범 사례

**I/O 바운드 작업: 항상 비동기**

```csharp
// ❌ 동기 I/O - 스레드 블로킹
[HttpGet("{id}")]
public IActionResult GetProduct(int id)
{
    var product = _context.Products.Find(id); // 스레드가 DB 응답을 기다리며 블로킹
    return Ok(product);
}

// ✅ 비동기 I/O - 스레드 해제
[HttpGet("{id}")]
public async Task<IActionResult> GetProduct(int id)
{
    var product = await _context.Products.FindAsync(id); // 스레드가 다른 작업 처리 가능
    return Ok(product);
}
```

**왜 중요한가?**

ASP.NET Core의 스레드 풀은 제한적입니다 (보통 CPU 코어당 2개). 모든 스레드가 I/O를 기다리며 블로킹되면, 새 요청을 처리할 스레드가 없습니다. 비동기 I/O는 스레드를 해제하여, 더 많은 동시 요청을 처리할 수 있게 합니다.

**CPU 바운드 작업: async 불필요 (오히려 오버헤드)**

```csharp
// ❌ CPU 바운드 작업에 async - 불필요한 오버헤드
public async Task<int> CalculateFibonacci(int n)
{
    await Task.CompletedTask; // 의미 없음

    if (n <= 1) return n;
    return CalculateFibonacci(n - 1) + CalculateFibonacci(n - 2);
}

// ✅ CPU 바운드는 동기로
public int CalculateFibonacci(int n)
{
    if (n <= 1) return n;
    return CalculateFibonacci(n - 1) + CalculateFibonacci(n - 2);
}

// 만약 장시간 실행된다면, Task.Run으로 스레드 풀에서 실행
public async Task<int> CalculateFibonacciAsync(int n)
{
    return await Task.Run(() => CalculateFibonacci(n));
}
```

**피해야 할 패턴**

```csharp
// ❌ .Result나 .Wait() - 데드락 위험
public IActionResult GetData()
{
    var data = _service.GetDataAsync().Result; // 데드락 가능!
    return Ok(data);
}

// ✅ 끝까지 비동기
public async Task<IActionResult> GetData()
{
    var data = await _service.GetDataAsync();
    return Ok(data);
}

// ❌ async void - 예외를 잡을 수 없음
public async void ProcessOrder(int orderId)
{
    await _orderService.ProcessAsync(orderId); // 예외 발생 시 앱 크래시!
}

// ✅ async Task - 예외 처리 가능
public async Task ProcessOrder(int orderId)
{
    await _orderService.ProcessAsync(orderId);
}
```

### ValueTask: 할당 최적화

`Task<T>`는 힙에 할당되는 참조 타입입니다. 자주 호출되고 대부분 동기적으로 완료되는 메서드(예: 캐시 히트)는 `ValueTask<T>`를 고려하세요.

```csharp
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationDbContext _context;

    // Task<T> 버전: 캐시 히트에도 Task 할당
    public async Task<Product> GetProductAsync_Task(int id)
    {
        if (_cache.TryGetValue(id, out Product cachedProduct))
            return cachedProduct; // 동기 완료지만, Task 할당 발생

        var product = await _context.Products.FindAsync(id);
        _cache.Set(id, product, TimeSpan.FromMinutes(5));
        return product;
    }

    // ValueTask<T> 버전: 캐시 히트 시 할당 없음!
    public ValueTask<Product> GetProductAsync_ValueTask(int id)
    {
        if (_cache.TryGetValue(id, out Product cachedProduct))
            return new ValueTask<Product>(cachedProduct); // 할당 없음!

        return new ValueTask<Product>(FetchFromDbAsync(id));
    }

    private async Task<Product> FetchFromDbAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        _cache.Set(id, product, TimeSpan.FromMinutes(5));
        return product;
    }
}
```

벤치마크:

```
|                Method | CacheHit |      Mean | Allocated |
|---------------------- |--------- |----------:|----------:|
|   GetProductAsync_Task|     True |  12.34 ns |      40 B |
| GetProductAsync_ValueTask|     True |   3.45 ns |       0 B |
|   GetProductAsync_Task|    False | 245.67 μs |     184 B |
| GetProductAsync_ValueTask|    False | 245.23 μs |     144 B |
```

캐시 히트 시 `ValueTask`가 70% 빠르고, 할당이 없습니다!

**주의사항:**
- `ValueTask`는 한 번만 await해야 합니다 (재사용 불가)
- 대부분 동기 완료되는 경우에만 유용
- 복잡도가 증가하므로, 측정 후 결정

### IAsyncEnumerable: 대용량 데이터 스트리밍

전체 데이터를 메모리에 로드하는 대신, 스트리밍하여 메모리 사용을 줄입니다. JavaScript의 AsyncGenerator와 유사합니다.

```csharp
// ❌ 전체 데이터를 메모리에 로드
[HttpGet("products")]
public async Task<ActionResult<List<Product>>> GetAllProducts()
{
    var products = await _context.Products.ToListAsync(); // 100만 개면? OOM!
    return Ok(products);
}

// ✅ 스트리밍으로 청크 단위 전송
[HttpGet("products/stream")]
public async IAsyncEnumerable<Product> StreamProducts()
{
    await foreach (var product in _context.Products.AsAsyncEnumerable())
    {
        yield return product; // 하나씩 클라이언트에 전송
    }
}
```

클라이언트는 `Transfer-Encoding: chunked` 응답을 받으며, 첫 번째 항목이 즉시 도착합니다.

**실제 사용 예: 로그 스트리밍**

```csharp
public class LogService
{
    public async IAsyncEnumerable<LogEntry> StreamLogsAsync(
        DateTime from,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        // 대용량 로그 파일을 한 번에 읽지 않고 스트리밍
        await using var stream = File.OpenRead("app.log");
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (line == null) break;

            var entry = ParseLogEntry(line);
            if (entry.Timestamp >= from)
                yield return entry;
        }
    }
}

// 컨트롤러
[HttpGet("logs/stream")]
public IAsyncEnumerable<LogEntry> StreamLogs([FromQuery] DateTime from)
{
    return _logService.StreamLogsAsync(from, HttpContext.RequestAborted);
}
```

### Channels: 고성능 생산자-소비자 패턴

`System.Threading.Channels`는 비동기 생산자-소비자 큐입니다. RxJS의 Subject와 유사하지만, 백프레셔(backpressure)를 지원합니다.

```csharp
public class ImageProcessingService
{
    private readonly Channel<ImageJob> _channel;
    private readonly ILogger<ImageProcessingService> _logger;

    public ImageProcessingService(ILogger<ImageProcessingService> logger)
    {
        _logger = logger;

        // Bounded channel: 최대 100개 작업 버퍼링
        _channel = Channel.CreateBounded<ImageJob>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait // 가득 차면 생산자 대기
        });
    }

    // API 컨트롤러가 호출: 즉시 반환 (비동기 처리)
    public async ValueTask<Guid> EnqueueImageAsync(Stream imageStream)
    {
        var jobId = Guid.NewGuid();
        var job = new ImageJob { Id = jobId, ImageData = await ReadStreamAsync(imageStream) };

        await _channel.Writer.WriteAsync(job);

        _logger.LogInformation("Image job {JobId} enqueued", jobId);
        return jobId;
    }

    // 백그라운드 서비스가 실행: 큐에서 작업 가져와 처리
    public async Task ProcessJobsAsync(CancellationToken ct)
    {
        await foreach (var job in _channel.Reader.ReadAllAsync(ct))
        {
            try
            {
                await ProcessImageAsync(job);
                _logger.LogInformation("Image job {JobId} completed", job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process image job {JobId}", job.Id);
            }
        }
    }

    private async Task ProcessImageAsync(ImageJob job)
    {
        // 실제 이미지 처리 (리사이징, 워터마크 등)
        await Task.Delay(100); // 시뮬레이션
    }
}

// BackgroundService로 실행
public class ImageProcessorHostedService : BackgroundService
{
    private readonly ImageProcessingService _processor;

    public ImageProcessorHostedService(ImageProcessingService processor)
    {
        _processor = processor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.ProcessJobsAsync(stoppingToken);
    }
}

// Program.cs
services.AddSingleton<ImageProcessingService>();
services.AddHostedService<ImageProcessorHostedService>();
```

**이점:**
- API 엔드포인트는 즉시 반환 (빠른 응답)
- 백그라운드에서 처리 (사용자 경험 향상)
- 백프레셔로 메모리 보호 (큐가 가득 차면 생산자 대기)
- 예외 처리 격리 (한 작업 실패가 다른 작업에 영향 없음)

### ConfigureAwait: 불필요한 컨텍스트 캡처 제거

`await` 시 .NET은 현재 `SynchronizationContext`를 캡처하여, 연속 작업을 원래 컨텍스트에서 실행합니다. 하지만 ASP.NET Core는 요청마다 별도의 컨텍스트가 없으므로, 이 캡처가 불필요합니다.

```csharp
// 라이브러리 코드: ConfigureAwait(false) 권장
public async Task<string> FetchDataAsync(string url)
{
    using var client = new HttpClient();
    var response = await client.GetStringAsync(url)
        .ConfigureAwait(false); // 컨텍스트 캡처 비용 절약

    return ProcessData(response);
}

// 컨트롤러 코드: ConfigureAwait는 일반적으로 불필요
[HttpGet]
public async Task<IActionResult> GetData()
{
    var data = await _service.FetchDataAsync("https://api.example.com");
    // ConfigureAwait(false) 불필요 - ASP.NET Core에는 SynchronizationContext 없음
    return Ok(data);
}
```

**언제 ConfigureAwait(false)를 사용할까?**
- **라이브러리 코드**: 항상 사용 (호출자 환경 독립적)
- **ASP.NET Core 앱 코드**: 일반적으로 불필요
- **UI 앱 (WPF/WinForms)**: 절대 사용 금지 (UI 스레드 필요)

## 데이터베이스 최적화

데이터베이스는 종종 애플리케이션의 병목입니다. Entity Framework Core는 편리하지만, 잘못 사용하면 성능 문제가 발생합니다.

### AsNoTracking(): 읽기 전용 쿼리 최적화

EF Core는 기본적으로 조회한 엔티티를 추적하여, `SaveChanges()` 시 변경된 부분만 업데이트합니다. 하지만 읽기 전용 쿼리에서는 이 추적이 불필요한 오버헤드입니다.

```csharp
// API 엔드포인트: 거의 항상 읽기 전용
[HttpGet]
public async Task<IActionResult> GetProducts()
{
    var products = await _context.Products
        .AsNoTracking() // 20-30% 성능 향상!
        .Where(p => p.IsActive)
        .OrderBy(p => p.Name)
        .ToListAsync();

    return Ok(products);
}

// 수정이 필요한 경우: 추적 활성화 (기본값)
[HttpPut("{id}")]
public async Task<IActionResult> UpdateProduct(int id, ProductDto dto)
{
    var product = await _context.Products.FindAsync(id); // 추적 활성화
    if (product == null) return NotFound();

    product.Price = dto.Price;
    product.Stock = dto.Stock;

    await _context.SaveChangesAsync(); // 변경 감지 작동

    return Ok(product);
}
```

**전역 NoTracking 설정** (대부분의 쿼리가 읽기 전용일 때):

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// 특정 쿼리에서 추적 활성화
var product = await _context.Products
    .AsTracking()
    .FirstOrDefaultAsync(p => p.Id == id);
```

### N+1 쿼리 해결: Include로 Eager Loading

가장 흔한 성능 문제입니다. 프로파일러로 쉽게 발견할 수 있습니다.

```csharp
// ❌ N+1 쿼리 문제
public async Task<List<OrderDto>> GetOrdersWithCustomers_Bad()
{
    var orders = await _context.Orders.ToListAsync(); // 1번 쿼리

    var result = new List<OrderDto>();
    foreach (var order in orders)
    {
        // 각 주문마다 별도 쿼리 실행! (N번 쿼리)
        var customer = await _context.Customers.FindAsync(order.CustomerId);
        result.Add(new OrderDto
        {
            Id = order.Id,
            CustomerName = customer.Name,
            TotalAmount = order.TotalAmount
        });
    }
    // 총 1 + N 개의 쿼리

    return result;
}

// ✅ Eager Loading으로 해결
public async Task<List<OrderDto>> GetOrdersWithCustomers_Good()
{
    var orders = await _context.Orders
        .Include(o => o.Customer) // JOIN으로 한 번에 가져오기
        .AsNoTracking()
        .ToListAsync();
    // 총 1개의 쿼리

    return orders.Select(o => new OrderDto
    {
        Id = o.Id,
        CustomerName = o.Customer.Name,
        TotalAmount = o.TotalAmount
    }).ToList();
}

// ✅ 더 나은 방법: 프로젝션으로 필요한 필드만
public async Task<List<OrderDto>> GetOrdersWithCustomers_Best()
{
    return await _context.Orders
        .Select(o => new OrderDto
        {
            Id = o.Id,
            CustomerName = o.Customer.Name, // EF Core가 자동으로 JOIN
            TotalAmount = o.TotalAmount
        })
        .ToListAsync();
    // 1개의 쿼리, Customer 엔티티 할당 없음
}
```

**중첩 관계**도 Include 가능:

```csharp
var orders = await _context.Orders
    .Include(o => o.Customer)
    .Include(o => o.Items)
        .ThenInclude(i => i.Product) // OrderItem → Product
    .AsNoTracking()
    .ToListAsync();
```

### 인덱스: 쿼리 속도의 핵심

인덱스가 없으면 데이터베이스는 전체 테이블을 스캔해야 합니다.

```csharp
public class Product
{
    public int Id { get; set; }

    [Index] // 단일 컬럼 인덱스
    public string Sku { get; set; } = string.Empty;

    [Index]
    public bool IsActive { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

// OnModelCreating에서 복합 인덱스 설정
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>()
        .HasIndex(p => new { p.Category, p.IsActive })
        .HasDatabaseName("IX_Product_Category_IsActive");

    // WHERE Category = ? AND IsActive = ? 쿼리가 빨라집니다
}
```

**인덱스 전략:**
- `WHERE` 절에 자주 사용되는 컬럼
- `JOIN` 조건 컬럼 (외래 키는 자동으로 인덱스)
- `ORDER BY` 컬럼
- 복합 조건은 복합 인덱스 고려

**주의**: 인덱스는 쓰기 성능을 저하시킵니다. 읽기가 많은 컬럼에만 추가하세요.

### Compiled Queries: 반복 쿼리 최적화

EF Core는 LINQ 쿼리를 SQL로 변환하는 비용이 있습니다. 자주 실행되는 쿼리는 미리 컴파일할 수 있습니다.

```csharp
public class ProductRepository
{
    // Compiled Query 정의
    private static readonly Func<ApplicationDbContext, int, Task<Product?>> _getByIdQuery =
        EF.CompileAsyncQuery((ApplicationDbContext context, int id) =>
            context.Products
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id)
        );

    private readonly ApplicationDbContext _context;

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _getByIdQuery(_context, id);
        // 첫 실행 후 캐시되어, 후속 호출이 빠름
    }
}
```

일반적으로 10-15% 성능 향상입니다. 고빈도 쿼리에만 적용하세요.

## 응답 압축과 캐싱

### 응답 압축: 대역폭 절약

JSON 응답은 텍스트이므로 압축률이 높습니다. Gzip은 보통 60-80%, Brotli는 더 나은 압축을 제공합니다.

```csharp
// Program.cs
services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // HTTPS에서도 압축
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest; // 속도 우선 (Optimal은 더 느림)
});

services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

app.UseResponseCompression(); // 미들웨어 추가 (라우팅 전에)
```

**압축 전후 비교:**

```
원본 JSON 응답: 125 KB
Gzip 압축: 28 KB (77% 감소)
Brotli 압축: 23 KB (82% 감소)
```

느린 네트워크에서 특히 효과적입니다.

**.NET 9의 빌드 타임 압축:**

정적 자산은 빌드 시 미리 압축할 수 있습니다.

```csharp
app.MapStaticAssets(); // .NET 9: 자동 fingerprinting + 사전 압축
```

런타임 압축 오버헤드가 없으며, 파일 이름에 해시가 추가되어 무한 캐싱이 안전합니다.

### HTTP 캐시 헤더: 불필요한 요청 제거

압축은 응답 크기를 줄이지만, 여전히 서버가 처리해야 합니다. HTTP 캐시는 한 단계 더 나아가: 브라우저가 서버에 요청조차 보내지 않게 합니다.

```csharp
[HttpGet("products")]
[ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "category" })]
public async Task<IActionResult> GetProducts(string? category)
{
    var products = await _context.Products
        .Where(p => category == null || p.Category == category)
        .AsNoTracking()
        .ToListAsync();

    // Cache-Control: public, max-age=60
    // Vary: category
    return Ok(products);
}

[HttpGet("user/profile")]
public async Task<IActionResult> GetProfile()
{
    var user = await GetCurrentUserAsync();

    // 사용자별 데이터는 private 캐싱
    Response.Headers.CacheControl = "private, max-age=300";

    return Ok(user);
}

[HttpGet("payment/{id}")]
public async Task<IActionResult> GetPayment(int id)
{
    var payment = await _context.Payments.FindAsync(id);

    // 민감한 데이터는 캐시 금지
    Response.Headers.CacheControl = "no-store";

    return Ok(payment);
}
```

**ETag로 조건부 요청:**

```csharp
[HttpGet("article/{id}")]
public async Task<IActionResult> GetArticle(int id)
{
    var article = await _context.Articles.FindAsync(id);
    if (article == null) return NotFound();

    // ETag 생성 (UpdatedAt 해시)
    var etag = $"\"{article.UpdatedAt:yyyyMMddHHmmss}\"";

    // 클라이언트가 보낸 ETag와 비교
    if (Request.Headers.IfNoneMatch == etag)
    {
        return StatusCode(304); // 304 Not Modified - 본문 없음, 빠름!
    }

    Response.Headers.ETag = etag;
    Response.Headers.CacheControl = "public, max-age=60";

    return Ok(article);
}
```

클라이언트는 다음 요청 시 `If-None-Match: "20250115102030"`을 보내고, 변경이 없으면 304 응답 (본문 없음)을 받습니다.

## Native AOT: 빠른 시작, 작은 footprint

.NET은 전통적으로 JIT 컴파일을 사용합니다. Native AOT는 빌드 시 전체 앱을 네이티브 코드로 컴파일합니다.

**장점:**
- **빠른 시작**: 밀리초 안에 시작 (서버리스/Functions에 이상적)
- **작은 메모리**: JIT 컴파일러가 메모리에 없음
- **작은 배포 크기**: 사용하지 않는 코드 제거 (trimming)

**제약사항:**
- 리플렉션 제한 (AOT는 런타임 타입 정보 제거)
- 동적 코드 생성 불가
- 모든 라이브러리가 지원되지 않음

**언제 사용할까?**
- Azure Functions, AWS Lambda (콜드 스타트 중요)
- 컨테이너 (작은 이미지 크기)
- 마이크로서비스 (많은 인스턴스 → 메모리 절약)

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
</PropertyGroup>
```

```bash
dotnet publish -c Release
```

**결과 비교:**

| 메트릭 | JIT | AOT |
|-------|-----|-----|
| 시작 시간 | 2.5s | 0.08s |
| 메모리 (시작) | 85 MB | 25 MB |
| 배포 크기 | 95 MB | 12 MB |
| 첫 요청 지연 | 250ms | 15ms |

서버리스 환경에서 극적인 개선입니다!

## 성능 최적화 체크리스트

- ✅ **측정**: 추측 대신 프로파일링으로 병목 식별
- ✅ **비동기**: I/O 작업은 항상 비동기로, `.Result` 피하기
- ✅ **데이터베이스**: N+1 해결, AsNoTracking, 인덱스 추가
- ✅ **캐싱**: HTTP 캐시 헤더 설정, ETag 활용
- ✅ **압축**: Brotli/Gzip 활성화, 정적 자산 사전 압축
- ✅ **메모리**: 불필요한 할당 줄이기, ValueTask 고려
- ✅ **검증**: 최적화 전후 벤치마크로 개선 확인

## 마무리

성능 최적화는 예술이자 과학입니다. 측정하지 않고 추측으로 최적화하면 시간을 낭비하거나 코드를 복잡하게 만듭니다. 올바른 접근은:

1. **프로파일링**으로 병목 식별
2. **벤치마크**로 정확히 측정
3. **최적화** 적용
4. **검증**으로 개선 확인
5. **반복**

이제 여러분은 ASP.NET Core 애플리케이션을 빠르고 효율적으로 만드는 모든 도구와 기법을 알고 있습니다. 다음 Chapter 23에서는 프로덕션 환경에서 애플리케이션을 모니터링하고, 문제를 빠르게 감지하는 방법을 배웁니다.
