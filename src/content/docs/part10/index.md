---
title: "Part 10 - 성능 최적화와 모니터링"
---

# Part 10: 성능 최적화와 모니터링

## 빠른 것은 좋은 것: 성능이 사용자 경험을 만든다

Part 9까지 여러분은 견고하고 테스트된 애플리케이션을 만드는 방법을 배웠습니다. 코드는 아름답게 구조화되어 있고, 테스트는 모든 기능을 검증하며, 아키텍처는 확장 가능합니다. 하지만 프로덕션에 배포하고 나면 새로운 질문이 생깁니다: **이 애플리케이션은 얼마나 빠른가? 수천 명의 동시 사용자를 처리할 수 있는가? 병목 지점은 어디인가?**

성능은 기능성만큼 중요합니다. 연구에 따르면, 페이지 로딩이 1초 지연될 때마다 전환율이 7% 감소하고, 사용자의 40%는 3초 이상 기다리지 않고 사이트를 떠납니다. 느린 API는 좌절감을 주고, 긴 응답 시간은 신뢰를 떨어뜨립니다. Amazon은 100ms의 지연이 매출의 1%를 감소시킨다고 보고했습니다. 성능은 비즈니스 성과에 직접적인 영향을 미칩니다.

하지만 성능 최적화에는 함정이 있습니다. Donald Knuth의 유명한 말처럼, "조기 최적화는 만악의 근원이다." 측정하지 않고 추측으로 최적화하면, 실제로는 영향이 미미한 부분에 시간을 낭비하거나, 더 나쁘게는 코드를 복잡하게 만들어 유지보수성을 해칩니다. 올바른 접근은 **측정 → 분석 → 최적화 → 검증**의 사이클입니다.

Part 10에서는 ASP.NET Core 애플리케이션의 성능을 측정하고, 병목을 식별하며, 효율적으로 최적화하는 방법을 배웁니다. 그리고 프로덕션 환경에서 애플리케이션의 건강을 모니터링하고, 문제를 빠르게 감지하며, 근본 원인을 추적하는 방법을 마스터합니다.

### 성능의 여러 차원: 무엇을 최적화할 것인가

성능은 단일한 개념이 아닙니다. 여러 측면이 있으며, 때로는 서로 트레이드오프 관계에 있습니다.

**응답 시간 (Latency)**

사용자가 요청을 보낸 후 첫 번째 바이트를 받기까지의 시간입니다. 프론트엔드 개발자로서 가장 친숙한 메트릭일 것입니다. API 응답이 빠를수록, 사용자 경험이 좋습니다. 목표는 대부분의 요청이 200ms 이내, 99%가 1초 이내에 완료되는 것입니다.

**처리량 (Throughput)**

단위 시간당 처리할 수 있는 요청 수입니다. 초당 요청 수(RPS)나 분당 거래 수(TPM)로 측정됩니다. 처리량이 높을수록, 더 많은 사용자를 동시에 처리할 수 있습니다.

**리소스 사용률 (Resource Utilization)**

CPU, 메모리, 네트워크, 디스크를 얼마나 효율적으로 사용하는가입니다. 같은 작업을 더 적은 리소스로 처리할수록, 비용이 절감되고 확장성이 향상됩니다. 클라우드 환경에서는 리소스 사용이 곧 비용이므로, 이는 직접적인 절감 효과를 가져옵니다.

**확장성 (Scalability)**

부하가 증가할 때 얼마나 잘 대응하는가입니다. 수평 확장(서버 추가)이나 수직 확장(더 강력한 서버)으로 성능이 선형적으로 향상되는가? 아니면 어느 지점에서 병목이 발생하는가?

이 모든 차원을 동시에 최적화할 수는 없습니다. 예를 들어, 캐싱은 응답 시간을 극적으로 줄이지만 메모리 사용률을 증가시킵니다. 비동기 처리는 처리량을 높이지만 각 요청의 응답 시간은 약간 증가할 수 있습니다. 중요한 것은 **여러분의 애플리케이션에 가장 중요한 메트릭을 파악하고, 그것을 우선적으로 최적화**하는 것입니다.

### 프로파일링: 추측하지 말고 측정하라

"이 함수가 느린 것 같아"는 최적화의 출발점이 되어서는 안 됩니다. 실제로 측정해보면, 예상과 다른 곳이 병목일 때가 많습니다. 프로파일러는 애플리케이션의 실행을 분석하여, 어떤 메서드가 가장 많은 시간을 소비하는지, 어떤 부분이 메모리를 많이 할당하는지 보여줍니다.

**.NET 프로파일링 도구**

**Visual Studio Profiler**: Visual Studio에 내장된 강력한 프로파일러로, CPU 사용, 메모리 할당, .NET 객체 할당을 시각화합니다. 타임라인 뷰는 메서드 호출의 깊이와 시간을 보여주며, Hot Path 분석은 가장 많은 시간을 소비하는 코드 경로를 강조합니다.

**dotnet-trace**: 명령줄 도구로, 프로덕션 환경에서도 사용할 수 있습니다. .NET 이벤트를 수집하여 나중에 분석할 수 있으며, 오버헤드가 낮아 실행 중인 서비스에 안전하게 사용할 수 있습니다.

```bash
# 프로세스 추적 시작
dotnet-trace collect --process-id <pid> --duration 00:00:30

# 생성된 .nettrace 파일을 Visual Studio나 PerfView로 분석
```

**dotnet-counters**: 실시간 성능 카운터를 표시합니다. CPU 사용률, 메모리, GC 힙 크기, 예외 수 등을 실시간으로 모니터링합니다.

```bash
dotnet-counters monitor --process-id <pid>
```

**PerfView**: Microsoft의 무료 고급 프로파일러로, CPU와 메모리 프로파일링, ETW(Event Tracing for Windows) 이벤트 수집을 지원합니다. 복잡하지만 매우 강력합니다.

**프로파일링의 일반적인 발견**

프로파일링을 하면 흔히 발견하는 패턴들이 있습니다:

- **N+1 쿼리**: Entity Framework를 잘못 사용하면, 하나의 엔티티를 가져온 후 각 관련 엔티티를 개별 쿼리로 가져옵니다. 100개의 게시글이 있다면 101개의 쿼리가 실행됩니다.

```csharp
// 나쁜 예: N+1 쿼리
var posts = await _context.Posts.ToListAsync();
foreach (var post in posts)
{
    // 각 post마다 별도의 쿼리 실행!
    var author = await _context.Users.FindAsync(post.AuthorId);
}

// 좋은 예: Eager Loading
var posts = await _context.Posts
    .Include(p => p.Author)
    .ToListAsync();
```

- **동기 I/O**: 비동기 메서드를 사용하지 않으면, 스레드가 I/O를 기다리며 블로킹됩니다. 이는 처리량을 크게 감소시킵니다.

- **과도한 메모리 할당**: 루프 안에서 객체를 반복적으로 생성하면, GC 압력이 증가하여 성능이 저하됩니다.

- **불필요한 직렬화**: 매 요청마다 복잡한 객체를 JSON으로 직렬화하는 것은 비용이 큽니다. 캐싱을 고려하세요.

### 벤치마킹: 정확한 성능 측정

프로파일링이 "어디가 느린가"를 보여준다면, 벤치마킹은 "얼마나 느린가"를 정확히 측정합니다. 그리고 최적화 전후를 비교하여 실제로 개선되었는지 확인합니다.

**BenchmarkDotNet: 마이크로벤치마킹의 표준**

BenchmarkDotNet은 .NET의 사실상 표준 벤치마킹 라이브러리입니다. 나노초 단위까지 정확하게 측정하며, JIT 워밍업, GC 영향, 통계적 이상치를 모두 고려합니다.

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class StringConcatBenchmark
{
    [Benchmark]
    public string UsingPlus()
    {
        string result = "";
        for (int i = 0; i < 1000; i++)
            result += i.ToString();
        return result;
    }

    [Benchmark]
    public string UsingStringBuilder()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 1000; i++)
            sb.Append(i);
        return sb.ToString();
    }
}

// Program.cs
BenchmarkRunner.Run<StringConcatBenchmark>();
```

결과는 다음과 같이 표시됩니다:

```
|              Method |      Mean |    Error |   StdDev |   Gen 0 |  Allocated |
|-------------------- |----------:|---------:|---------:|--------:|-----------:|
|          UsingPlus  | 284.5 μs | 5.2 μs  | 4.8 μs  | 125.00  |  256,120 B |
| UsingStringBuilder  |  12.3 μs | 0.2 μs  | 0.2 μs  |   2.50  |    5,144 B |
```

StringBuilder가 23배 빠르고, 메모리 할당은 1/50입니다! 이것이 측정의 힘입니다.

**[MemoryDiagnoser]**: 메모리 할당을 측정합니다. GC Gen 0/1/2 수집 횟수도 보여줍니다.

**[Params]**: 여러 매개변수로 벤치마크를 실행합니다.

```csharp
[Params(10, 100, 1000)]
public int N;
```

**벤치마킹 주의사항**

- **현실적인 시나리오 테스트**: 인위적인 마이크로벤치마크는 오해를 불러일으킬 수 있습니다. 실제 사용 패턴을 반영하세요.
- **여러 번 실행**: 단일 실행은 노이즈에 영향받습니다. BenchmarkDotNet은 자동으로 여러 번 실행하고 통계를 계산합니다.
- **Release 모드**: 벤치마크는 항상 Release 빌드로 실행하세요. Debug 빌드는 최적화가 꺼져 있어 비현실적입니다.

### 비동기 프로그래밍: 처리량의 비밀

ASP.NET Core는 기본적으로 비동기입니다. 하지만 비동기를 올바르게 사용하지 않으면, 이점을 얻지 못하거나 오히려 성능이 저하될 수 있습니다.

**async/await의 작동 원리**

프론트엔드에서 `async/await`는 JavaScript 이벤트 루프를 차단하지 않습니다. .NET에서도 비슷하지만, 세부 사항은 다릅니다. `await`를 만나면, 현재 스레드는 다른 작업을 처리할 수 있도록 반환됩니다. I/O가 완료되면, 스레드 풀에서 사용 가능한 스레드가 계속 실행합니다.

이것이 중요한 이유: 동기 I/O는 스레드를 블로킹합니다. ASP.NET Core의 스레드 풀은 제한되어 있으므로(일반적으로 CPU 코어당 2개), 모든 스레드가 블로킹되면 새 요청을 처리할 수 없습니다. 비동기 I/O는 스레드를 해제하여, 더 많은 동시 요청을 처리할 수 있게 합니다.

**모범 사례**

```csharp
// 나쁜 예: 동기 I/O
public IActionResult GetProduct(int id)
{
    var product = _context.Products.Find(id); // 스레드 블로킹!
    return Ok(product);
}

// 좋은 예: 비동기 I/O
public async Task<IActionResult> GetProduct(int id)
{
    var product = await _context.Products.FindAsync(id);
    return Ok(product);
}
```

**피해야 할 패턴**

- **`.Result`나 `.Wait()`**: 비동기 메서드를 동기적으로 호출하면 데드락이 발생할 수 있습니다.
- **async void**: 이벤트 핸들러를 제외하고는 사용하지 마세요. 예외를 잡을 수 없습니다.
- **불필요한 async**: CPU 바운드 작업에 async를 사용하면 오버헤드만 증가합니다.

**ValueTask vs Task**

자주 호출되고 대부분 동기적으로 완료되는 메서드(예: 캐시 히트)는 `ValueTask<T>`를 고려하세요. `Task<T>`는 힙에 할당되지만, `ValueTask<T>`는 성공 경로에서 할당을 피할 수 있습니다.

```csharp
public async ValueTask<Product> GetProductAsync(int id)
{
    if (_cache.TryGetValue(id, out Product product))
        return product; // 할당 없음!

    product = await _db.Products.FindAsync(id);
    _cache.Set(id, product);
    return product;
}
```

**ConfigureAwait: 컨텍스트 캡처 제어**

`ConfigureAwait(false)`는 라이브러리 코드에서 성능을 개선하는 패턴입니다. `await` 시 .NET은 현재 `SynchronizationContext`나 `TaskScheduler`를 캡처하여, 연속 작업을 원래 컨텍스트에서 실행합니다. 하지만 ASP.NET Core는 요청마다 별도의 컨텍스트가 없으므로, 이 캡처가 불필요합니다.

```csharp
// 라이브러리 코드: ConfigureAwait(false) 권장
public async Task<string> FetchDataAsync()
{
    using var client = new HttpClient();
    var response = await client.GetStringAsync("https://api.example.com/data")
        .ConfigureAwait(false); // 컨텍스트 캡처 비용 절약

    return ProcessData(response);
}

// 컨트롤러 코드: ConfigureAwait는 일반적으로 불필요
public async Task<IActionResult> GetData()
{
    var data = await _service.FetchDataAsync(); // ConfigureAwait(false) 불필요
    return Ok(data);
}
```

**언제 ConfigureAwait(false)를 사용할까?**
- **라이브러리 코드**: 항상 사용하여 호출자에게 성능 이점 제공
- **ASP.NET Core 컨트롤러**: 불필요 (SynchronizationContext가 없음)
- **UI 스레드 필요**: 절대 사용 금지 (WPF/WinForms)

**IAsyncEnumerable: 비동기 스트림**

대용량 데이터를 한 번에 메모리에 로드하지 않고 스트리밍할 때 `IAsyncEnumerable<T>`를 사용합니다. 프론트엔드의 Observable이나 Generator 함수와 유사합니다.

```csharp
// 전통적인 방법: 모든 데이터를 메모리에 로드
public async Task<List<Product>> GetAllProductsAsync()
{
    return await _context.Products.ToListAsync(); // 메모리 부담!
}

// IAsyncEnumerable: 스트리밍
public async IAsyncEnumerable<Product> GetProductsStreamAsync()
{
    await foreach (var product in _context.Products.AsAsyncEnumerable())
    {
        // 각 항목을 개별적으로 yield
        yield return product;
    }
}
```

컨트롤러에서 사용:

```csharp
[HttpGet("stream")]
public IAsyncEnumerable<Product> StreamProducts()
{
    // 클라이언트는 청크 단위로 데이터를 받음
    return _repository.GetProductsStreamAsync();
}
```

클라이언트는 `Transfer-Encoding: chunked` 응답을 받으며, 첫 번째 항목이 즉시 도착합니다. 100만 개의 레코드를 기다릴 필요가 없습니다.

**언제 IAsyncEnumerable을 사용할까?**
- 대용량 데이터셋 (수천 개 이상의 항목)
- 실시간 데이터 피드 (로그 스트리밍, 모니터링)
- 메모리 제약이 있는 환경

**Channels: 생산자-소비자 패턴**

`System.Threading.Channels`는 고성능 비동기 생산자-소비자 큐입니다. 프론트엔드의 RxJS Subject와 유사하지만, 백프레셔(backpressure)를 지원합니다.

```csharp
using System.Threading.Channels;

public class OrderProcessor
{
    private readonly Channel<Order> _channel;

    public OrderProcessor()
    {
        // Bounded channel: 최대 100개 항목 버퍼링
        _channel = Channel.CreateBounded<Order>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait // 가득 차면 생산자 대기
        });
    }

    // 생산자: 주문을 채널에 추가
    public async Task EnqueueOrderAsync(Order order)
    {
        await _channel.Writer.WriteAsync(order);
    }

    // 소비자: 백그라운드에서 주문 처리
    public async Task ProcessOrdersAsync(CancellationToken ct)
    {
        await foreach (var order in _channel.Reader.ReadAllAsync(ct))
        {
            await ProcessOrderAsync(order);
        }
    }

    private async Task ProcessOrderAsync(Order order)
    {
        // 실제 주문 처리 로직
        await Task.Delay(100); // 시뮬레이션
    }
}
```

백그라운드 서비스로 실행:

```csharp
public class OrderProcessorService : BackgroundService
{
    private readonly OrderProcessor _processor;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await _processor.ProcessOrdersAsync(ct);
    }
}

// Program.cs
services.AddSingleton<OrderProcessor>();
services.AddHostedService<OrderProcessorService>();
```

**언제 Channels를 사용할까?**
- 비동기 생산자-소비자 패턴
- 백그라운드 작업 큐
- 속도 제한 (rate limiting)
- 이벤트 스트림 처리

Channels는 `BlockingCollection`이나 `ConcurrentQueue`보다 비동기 시나리오에서 훨씬 효율적입니다.

### 응답 압축: 대역폭 절약

압축은 저비용 고효율 최적화입니다. JSON 응답은 텍스트이므로 압축률이 높습니다. Gzip은 보통 60-80% 크기를 줄이며, Brotli는 더 나은 압축률을 제공합니다.

```csharp
services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
```

**.NET 9+의 빌드 타임 압축**

정적 자산(CSS, JS)은 빌드 시 미리 압축할 수 있습니다. 런타임 오버헤드가 없으며, CPU를 절약합니다.

```csharp
app.MapStaticAssets(); // .NET 9+: 자동 fingerprinting + 사전 압축
```

`MapStaticAssets`는 각 파일의 해시를 URL에 추가하여, 파일이 변경되면 URL도 변경되게 합니다. 따라서 무한 캐싱이 안전합니다.

### 응답 캐싱: 불필요한 작업 피하기

압축은 응답 크기를 줄이지만, 여전히 서버가 응답을 생성해야 합니다. 응답 캐싱은 한 단계 더 나아가: **동일한 요청에 대해 응답을 재사용**하여, 서버 작업을 완전히 건너뜁니다. 프론트엔드에서 SWR이나 React Query의 캐싱과 유사하지만, HTTP 프로토콜 레벨에서 작동합니다.

**Response Caching 미들웨어**

ASP.NET Core는 서버 측 응답 캐싱을 제공합니다:

```csharp
// Program.cs
services.AddResponseCaching();

app.UseResponseCaching(); // 라우팅 전에 추가
app.MapControllers();
```

컨트롤러에서 캐싱 정책을 지정:

```csharp
[HttpGet("products")]
[ResponseCache(Duration = 60)] // 60초 캐싱
public IActionResult GetProducts()
{
    var products = _context.Products.ToList();
    return Ok(products);
}
```

이제 동일한 요청이 60초 이내에 들어오면, 서버는 메모리에서 캐시된 응답을 반환하며, 데이터베이스 쿼리를 건너뜁니다.

**HTTP 캐시 헤더: 브라우저 캐싱 제어**

HTTP 프로토콜은 클라이언트 측 캐싱을 위한 여러 헤더를 정의합니다. 이를 올바르게 설정하면, 브라우저가 서버에 요청조차 보내지 않습니다.

**Cache-Control: 캐싱 정책의 핵심**

`Cache-Control` 헤더는 응답이 캐시될 수 있는지, 얼마나 오래 캐시될지를 지정합니다:

```csharp
[HttpGet("products/{id}")]
public IActionResult GetProduct(int id)
{
    var product = _context.Products.Find(id);

    // Cache-Control 헤더 설정
    Response.Headers.CacheControl = "public, max-age=300"; // 5분 캐싱

    return Ok(product);
}
```

주요 Cache-Control 지시어:
- **public**: 모든 캐시(브라우저, CDN)가 저장 가능
- **private**: 브라우저만 캐시 가능, CDN은 불가
- **no-cache**: 캐시하지만, 사용 전 서버에 재검증 필요
- **no-store**: 전혀 캐시하지 않음 (민감한 데이터)
- **max-age=seconds**: 캐시 유효 시간

```csharp
// 정적 데이터: 긴 캐싱
Response.Headers.CacheControl = "public, max-age=86400"; // 24시간

// 사용자 데이터: 짧은 캐싱
Response.Headers.CacheControl = "private, max-age=60"; // 1분

// 민감한 데이터: 캐시 금지
Response.Headers.CacheControl = "no-store";
```

**ETag: 효율적인 검증**

`ETag` (Entity Tag)는 리소스의 "지문"입니다. 내용이 변경되면 ETag도 변경됩니다. 클라이언트는 `If-None-Match` 헤더로 ETag를 보내며, 서버는 변경 여부를 확인합니다.

```csharp
[HttpGet("products/{id}")]
public IActionResult GetProduct(int id)
{
    var product = _context.Products.Find(id);
    if (product == null) return NotFound();

    // ETag 생성 (간단한 예: LastModified 해시)
    var etag = $"\"{product.LastModified:yyyyMMddHHmmss}\"";

    // 클라이언트가 보낸 ETag 확인
    if (Request.Headers.IfNoneMatch == etag)
    {
        return StatusCode(304); // 304 Not Modified - 본문 없음
    }

    Response.Headers.ETag = etag;
    Response.Headers.CacheControl = "public, max-age=60";

    return Ok(product);
}
```

304 응답은 본문이 없으므로 매우 빠릅니다. 클라이언트는 기존 캐시를 재사용합니다.

**Last-Modified: 시간 기반 검증**

`Last-Modified` 헤더는 리소스가 마지막으로 수정된 시간을 나타냅니다. 클라이언트는 `If-Modified-Since` 헤더로 이를 보내며, 서버는 변경 여부를 확인합니다.

```csharp
[HttpGet("articles/{id}")]
public IActionResult GetArticle(int id)
{
    var article = _context.Articles.Find(id);
    if (article == null) return NotFound();

    var lastModified = article.UpdatedAt;

    // 클라이언트가 보낸 If-Modified-Since 확인
    if (Request.Headers.IfModifiedSince.Any())
    {
        var ifModifiedSince = DateTimeOffset.Parse(
            Request.Headers.IfModifiedSince.ToString()
        );

        if (lastModified <= ifModifiedSince)
        {
            return StatusCode(304); // 304 Not Modified
        }
    }

    Response.Headers.LastModified = lastModified.ToString("R"); // RFC1123 형식
    Response.Headers.CacheControl = "public, max-age=300";

    return Ok(article);
}
```

**VaryBy: 조건부 캐싱**

동일한 URL이지만 헤더나 쿼리에 따라 다른 응답을 반환할 때, `VaryBy`를 사용합니다:

```csharp
[HttpGet("products")]
[ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "category", "sort" })]
public IActionResult GetProducts(string category, string sort)
{
    // category와 sort 조합마다 별도 캐시 항목
    var products = _context.Products
        .Where(p => p.Category == category)
        .OrderBy(p => sort == "price" ? p.Price : p.Name)
        .ToList();

    return Ok(products);
}

[HttpGet("content")]
[ResponseCache(Duration = 300, VaryByHeader = "Accept-Language")]
public IActionResult GetContent()
{
    // Accept-Language 헤더마다 별도 캐시 (다국어 지원)
    var language = Request.Headers.AcceptLanguage.ToString();
    var content = GetLocalizedContent(language);

    return Ok(content);
}
```

**캐싱 전략 가이드**

```csharp
// 불변 데이터 (이미지, 정적 자산)
Response.Headers.CacheControl = "public, max-age=31536000, immutable"; // 1년

// 자주 변경되는 데이터 (뉴스 피드)
Response.Headers.CacheControl = "public, max-age=60"; // 1분

// 사용자별 데이터 (프로필)
Response.Headers.CacheControl = "private, max-age=300"; // 5분

// 실시간 데이터 (주식 시세)
Response.Headers.CacheControl = "no-cache"; // 항상 재검증

// 민감한 데이터 (결제 정보)
Response.Headers.CacheControl = "no-store"; // 캐시 금지
```

### 데이터베이스 최적화: 쿼리 효율 높이기

데이터베이스는 종종 애플리케이션의 병목입니다. 느린 쿼리 하나가 전체 응답 시간을 지배할 수 있습니다. Entity Framework Core는 편리하지만, 잘못 사용하면 성능 문제가 발생합니다.

**AsNoTracking(): 읽기 전용 쿼리 최적화**

EF Core는 기본적으로 조회한 엔티티를 **추적(tracking)**합니다. 변경 감지를 위해 원본 값을 메모리에 보관하며, `SaveChanges()` 시 변경된 부분만 업데이트합니다. 하지만 읽기 전용 쿼리에서는 이 추적이 불필요한 오버헤드입니다.

```csharp
// 기본: 추적 활성화 (읽기 + 쓰기 시나리오)
var products = await _context.Products.ToListAsync();
products[0].Price = 99.99m;
await _context.SaveChanges(); // 변경 감지 및 업데이트

// 읽기 전용: 추적 비활성화로 성능 향상
var products = await _context.Products
    .AsNoTracking()
    .ToListAsync();
// 20-30% 더 빠르고, 메모리 사용량 절감
```

**성능 이점**:
- **메모리 절약**: 원본 값을 저장하지 않음
- **CPU 절약**: 변경 감지 로직 실행 안 함
- **쿼리 속도 향상**: 객체 구체화(materialization)가 더 빠름

```csharp
// API 엔드포인트: 거의 항상 읽기 전용
[HttpGet]
public async Task<IActionResult> GetProducts()
{
    var products = await _context.Products
        .AsNoTracking() // 추적 불필요
        .Where(p => p.IsActive)
        .ToListAsync();

    return Ok(products);
}

// 수정이 필요한 경우: 추적 활성화
[HttpPut("{id}")]
public async Task<IActionResult> UpdateProduct(int id, ProductDto dto)
{
    var product = await _context.Products.FindAsync(id); // 추적 활성화
    if (product == null) return NotFound();

    product.Price = dto.Price;
    await _context.SaveChangesAsync(); // 변경 감지 작동

    return Ok(product);
}
```

**전역 NoTracking 설정**

대부분의 쿼리가 읽기 전용이라면, 전역으로 NoTracking을 설정하고 필요할 때만 추적을 활성화할 수 있습니다:

```csharp
// Program.cs
services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // 기본값 변경
});

// 특정 쿼리에서 추적 활성화
var product = await _context.Products
    .AsTracking() // 명시적으로 추적 활성화
    .FirstOrDefaultAsync(p => p.Id == id);
```

**N+1 쿼리 해결: Include로 Eager Loading**

이미 프로파일링 섹션에서 언급했지만, N+1 쿼리는 가장 흔한 성능 문제입니다:

```csharp
// ❌ N+1 쿼리 문제
var orders = await _context.Orders.ToListAsync(); // 1번 쿼리
foreach (var order in orders)
{
    // 각 주문마다 별도 쿼리 실행! (N번 쿼리)
    var customer = await _context.Customers.FindAsync(order.CustomerId);
    var items = await _context.OrderItems.Where(i => i.OrderId == order.Id).ToListAsync();
}
// 총 1 + N + N = 1 + 2N 개의 쿼리

// ✅ Eager Loading으로 해결
var orders = await _context.Orders
    .Include(o => o.Customer) // JOIN으로 한 번에 가져오기
    .Include(o => o.Items)
        .ThenInclude(i => i.Product) // 중첩 관계도 가능
    .AsNoTracking() // 읽기 전용이면 추적 비활성화
    .ToListAsync();
// 총 1개의 쿼리 (또는 최적화된 2-3개)
```

**Select로 필요한 필드만 가져오기**

전체 엔티티를 가져오는 대신, 필요한 필드만 프로젝션하면 네트워크 대역폭과 메모리를 절약할 수 있습니다:

```csharp
// ❌ 전체 엔티티 조회 (모든 필드 포함)
var products = await _context.Products.ToListAsync();

// ✅ 필요한 필드만 선택
var products = await _context.Products
    .Select(p => new ProductListDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price
        // Description, CreatedAt 등 불필요한 필드 제외
    })
    .ToListAsync();
```

**인덱싱: 쿼리 속도의 핵심**

인덱스가 없으면 데이터베이스는 전체 테이블을 스캔해야 합니다. 100만 개의 레코드가 있다면 치명적입니다.

```csharp
public class Product
{
    public int Id { get; set; }

    [Index] // 단일 컬럼 인덱스
    public string Sku { get; set; }

    [Index] // 자주 필터링되는 필드
    public bool IsActive { get; set; }

    public string Name { get; set; }
    public decimal Price { get; set; }
}

// FluentAPI로 복합 인덱스 설정
modelBuilder.Entity<Product>()
    .HasIndex(p => new { p.Category, p.IsActive }); // 두 필드를 함께 조회할 때 유용
```

**Connection Pooling: 연결 재사용**

데이터베이스 연결을 열고 닫는 것은 비용이 큽니다. Connection Pooling은 연결을 재사용하여 오버헤드를 제거합니다. EF Core는 기본적으로 활성화되어 있지만, 풀 크기를 조정할 수 있습니다:

```csharp
// 연결 문자열에서 풀 설정
"Server=...;Database=...;Max Pool Size=100;Min Pool Size=10;"
```

고부하 시나리오에서는 풀 크기를 늘려야 할 수 있지만, 너무 크면 데이터베이스 서버에 부담을 줍니다.

**데이터베이스 최적화 체크리스트**
- ✅ 읽기 전용 쿼리는 `.AsNoTracking()` 사용
- ✅ N+1 쿼리 확인 및 `.Include()`로 해결
- ✅ 필요한 필드만 `.Select()`로 프로젝션
- ✅ 자주 조회/필터링되는 컬럼에 인덱스 추가
- ✅ 복합 조건 쿼리는 복합 인덱스 고려
- ✅ Connection Pooling 설정 확인

### Native AOT: 빠른 시작, 작은 footprint

.NET은 전통적으로 JIT(Just-In-Time) 컴파일을 사용합니다. 애플리케이션이 시작되면, IL(Intermediate Language) 코드가 네이티브 코드로 컴파일됩니다. 이는 런타임 최적화를 가능하게 하지만, 시작 시간이 느리고 메모리 사용이 많습니다.

Native AOT(Ahead-of-Time) 컴파일은 다릅니다. 빌드 시 전체 애플리케이션을 네이티브 코드로 컴파일합니다. 결과:

- **빠른 시작 시간**: JIT 컴파일이 없으므로, 밀리초 안에 시작됩니다.
- **작은 메모리 footprint**: JIT 컴파일러와 IL 코드가 메모리에 없습니다.
- **작은 배포 크기**: 사용하지 않는 코드가 제거됩니다(trimming).

이상적인 사용 사례:
- **서버리스/Functions**: 콜드 스타트가 중요할 때
- **컨테이너**: 작은 이미지 크기가 중요할 때
- **마이크로서비스**: 많은 인스턴스를 실행할 때 메모리 절약

제약 사항:
- 모든 API가 지원되지 않음(리플렉션, 동적 코드 생성 제한)
- 빌드 시간 증가
- 런타임 최적화 불가(JIT보다 느릴 수 있음)

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
</PropertyGroup>
```

### 모니터링: 프로덕션의 가시성

개발 환경에서는 모든 것이 잘 작동합니다. 하지만 프로덕션에서는 예상치 못한 문제가 발생합니다. 사용자가 "사이트가 느려요"라고 불평할 때, 무엇이 문제인지 어떻게 알 수 있을까요? 모니터링은 애플리케이션의 건강과 성능을 실시간으로 추적하여, 문제를 빠르게 감지하고 원인을 파악하게 해줍니다.

**Application Insights: Azure의 강력한 모니터링**

Application Insights는 Azure의 애플리케이션 성능 관리(APM) 서비스입니다. 요청, 의존성, 예외, 사용자 지정 이벤트를 자동으로 수집하며, 강력한 쿼리와 시각화를 제공합니다.

```csharp
services.AddApplicationInsightsTelemetry();
```

단 한 줄로 다음을 얻습니다:
- **요청 추적**: 모든 HTTP 요청의 응답 시간, 상태 코드, URL
- **의존성 추적**: 데이터베이스 쿼리, HTTP 호출, Redis 명령
- **예외 로깅**: 처리되지 않은 예외의 스택 트레이스
- **성능 카운터**: CPU, 메모리, GC 통계

Application Insights 포털에서 다음을 할 수 있습니다:
- **Application Map**: 서비스 간 의존성을 시각화
- **Live Metrics**: 실시간 메트릭과 로그
- **Failures**: 실패한 요청과 예외 분석
- **Performance**: 느린 요청 식별

**Kusto Query Language (KQL)**로 강력한 쿼리를 작성할 수 있습니다:

```kql
requests
| where timestamp > ago(1h)
| summarize count(), avg(duration) by name
| order by avg_duration desc
```

"지난 1시간의 요청을 엔드포인트별로 그룹화하고, 평균 응답 시간으로 정렬"

**구조화된 로깅: Serilog의 우수성**

전통적인 로깅은 문자열입니다: `logger.LogInformation("User {userId} logged in")`. 하지만 이는 검색과 분석이 어렵습니다.

구조화된 로깅은 데이터를 필드로 저장합니다. Serilog는 .NET의 대표적인 구조화 로깅 라이브러리입니다.

```csharp
Log.Information("Order {OrderId} created by user {UserId} with total {Total:C}",
    order.Id, user.Id, order.Total);
```

이는 다음과 같은 구조로 저장됩니다:

```json
{
  "timestamp": "2025-01-15T10:30:00Z",
  "level": "Information",
  "messageTemplate": "Order {OrderId} created by user {UserId} with total {Total}",
  "properties": {
    "OrderId": 12345,
    "UserId": 678,
    "Total": 99.99
  }
}
```

이제 "Total이 1000달러 이상인 모든 주문"을 쉽게 쿼리할 수 있습니다.

**Serilog Sinks**: 로그를 다양한 대상으로 보낼 수 있습니다:
- Console, File (개발)
- Azure Application Insights
- Elasticsearch
- Seq: 개발자 친화적 로그 검색 UI

**Enricher**: 모든 로그에 추가 정보를 자동으로 포함합니다:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .CreateLogger();
```

### 분산 추적: 마이크로서비스의 가시성

모놀리스에서는 요청이 하나의 프로세스 내에서 처리됩니다. 하지만 마이크로서비스에서는 하나의 사용자 요청이 여러 서비스를 거칩니다. "주문 생성"이 Order Service → Inventory Service → Payment Service → Notification Service를 거친다면, 어디서 지연이 발생하는지 어떻게 알 수 있을까요?

분산 추적은 요청에 고유 ID를 부여하고, 각 서비스가 이를 전달합니다. 모든 서비스의 로그를 이 ID로 연관지으면, 전체 요청 흐름을 시각화할 수 있습니다.

**OpenTelemetry: 벤더 중립적 표준**

OpenTelemetry는 분산 추적과 메트릭의 표준입니다. 다양한 백엔드(Jaeger, Zipkin, Azure Monitor)와 호환됩니다.

```csharp
services.AddOpenTelemetry()
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation()
        .AddJaegerExporter());
```

이제 모든 HTTP 요청, HTTP 클라이언트 호출, SQL 쿼리가 자동으로 추적됩니다. Jaeger UI에서 다음과 같은 시각화를 볼 수 있습니다:

```
Order Service  [========================================] 450ms
  ├─ Inventory Service  [==========] 100ms
  ├─ Payment Service    [====================] 200ms
  └─ Notification Service [====] 50ms
```

이제 Payment Service가 병목임이 명확합니다.

### 헬스 체크: 시스템의 심장박동

애플리케이션이 실행 중이라고 해서 건강한 것은 아닙니다. 데이터베이스 연결이 끊어졌거나, 디스크가 가득 찼거나, 중요한 의존성이 실패했을 수 있습니다. 헬스 체크는 이를 감지합니다.

```csharp
services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddRedis(Configuration["Redis:ConnectionString"])
    .AddUrlGroup(new Uri("https://api.thirdparty.com/health"), "Third Party API");

app.MapHealthChecks("/health");
```

`/health` 엔드포인트를 호출하면:

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": {
    "DbContext": { "status": "Healthy" },
    "Redis": { "status": "Healthy" },
    "Third Party API": { "status": "Degraded", "description": "Slow response" }
  }
}
```

Kubernetes나 로드 밸런서는 이 엔드포인트를 주기적으로 호출하여, 인스턴스가 건강하지 않으면 트래픽을 중단합니다.

### Part 10에서 배울 내용

이제 여러분은 애플리케이션의 성능을 측정하고 최적화하며, 프로덕션 환경에서 모니터링하는 방법을 배우게 될 것입니다.

**Chapter 22: 성능 최적화 기법**

프로파일링 도구(Visual Studio Profiler, dotnet-trace, PerfView)로 병목을 식별합니다. BenchmarkDotNet으로 정확한 벤치마크를 작성하며, 최적화 전후를 비교합니다.

비동기 프로그래밍의 모범 사례를 배우며, `ValueTask`와 `IAsyncEnumerable`을 활용합니다. 응답 압축(Gzip, Brotli)을 설정하고, .NET 9+의 빌드 타임 압축을 경험합니다.

데이터베이스 최적화(N+1 해결, 인덱싱, Connection Pooling)를 다루며, Native AOT의 장단점을 이해합니다. 실습에서는 느린 API를 프로파일링하여 병목을 찾고, 최적화하며, 벤치마크로 개선을 검증합니다.

**Chapter 23: 모니터링과 로깅**

Application Insights를 통합하여 요청, 의존성, 예외를 자동 추적합니다. KQL로 강력한 쿼리를 작성하며, 대시보드를 구성합니다.

Serilog로 구조화된 로깅을 구현하며, Enricher로 컨텍스트를 추가합니다. OpenTelemetry로 분산 추적을 설정하고, Jaeger나 Zipkin으로 요청 흐름을 시각화합니다.

헬스 체크를 구현하여 데이터베이스, 캐시, 외부 API의 상태를 모니터링합니다. Prometheus와 Grafana로 메트릭 대시보드를 구축하며, 알림 규칙을 설정합니다.

실습에서는 마이크로서비스 시나리오에서 분산 추적을 적용하고, Application Insights 대시보드를 구성하며, 성능 저하 시 자동 알림을 받도록 설정합니다.

## 학습 목표

Part 10을 마치면 다음을 할 수 있습니다:

- 프로파일링 도구로 성능 병목을 식별할 수 있습니다
- BenchmarkDotNet으로 정확한 벤치마크를 작성하고 해석합니다
- 비동기 프로그래밍 모범 사례를 적용하여 처리량을 향상시킵니다
- 응답 압축을 설정하고 대역폭을 절약합니다
- 데이터베이스 쿼리를 최적화하고 N+1 문제를 해결합니다
- Native AOT의 장단점을 이해하고 적절히 사용합니다
- Application Insights를 통합하여 프로덕션 모니터링을 구축합니다
- Serilog로 구조화된 로깅을 구현합니다
- OpenTelemetry로 분산 추적을 설정하고 요청 흐름을 시각화합니다
- 헬스 체크를 구현하고 자동 장애 감지를 설정합니다
- Prometheus와 Grafana로 메트릭 대시보드를 구축합니다
- KQL로 로그를 쿼리하고 분석합니다

## 챕터 구성

### [Chapter 22: 성능 최적화 기법](./chapter22/)

애플리케이션의 성능을 측정하고, 병목을 찾으며, 효율적으로 최적화하는 모든 기법을 배웁니다.

**프로파일링과 벤치마킹:**
- Visual Studio Profiler
  - CPU 프로파일링
  - 메모리 프로파일링
  - Hot Path 분석
- dotnet-trace와 dotnet-counters
  - 프로덕션 환경 프로파일링
  - 실시간 성능 카운터
- PerfView: 고급 프로파일링
  - ETW 이벤트 수집
  - GC 분석
- BenchmarkDotNet
  - 마이크로벤치마크 작성
  - `[MemoryDiagnoser]`, `[Params]`
  - 결과 해석과 비교

**응답 압축:**
- Gzip vs Brotli: 압축률과 CPU 사용
- 압축 레벨 선택
- HTTPS에서의 압축 고려사항
- .NET 9+의 빌드 타임 압축
  - `MapStaticAssets`: 자동 fingerprinting
  - 사전 압축된 파일 제공

**응답 캐싱:**
- Response Caching 미들웨어
- HTTP 캐시 헤더
  - Cache-Control, ETag, Last-Modified
- 조건부 요청: 304 Not Modified
- VaryBy: 쿼리, 헤더별 캐싱

**데이터베이스 최적화:**
- N+1 쿼리 해결
  - `Include()`, `ThenInclude()`
  - Explicit Loading vs Lazy Loading
- 인덱싱 전략
  - EF Core Indexes
  - 복합 인덱스
- Connection Pooling
- Compiled Queries: 반복 쿼리 최적화
- `AsNoTracking()`: 읽기 전용 쿼리

**비동기 프로그래밍:**
- async/await 모범 사례
  - I/O 바운드 vs CPU 바운드
  - ConfigureAwait 이해
- ValueTask vs Task
  - 할당 최적화
  - 언제 사용할까?
- IAsyncEnumerable: 비동기 스트림
  - 대용량 데이터 스트리밍
  - `await foreach`
- Channels: 생산자-소비자 패턴

**Native AOT:**
- AOT vs JIT: 트레이드오프
- 시작 시간과 메모리 footprint 개선
- 제약 사항과 호환성
- 트리밍(Trimming)과 크기 최적화
- 서버리스와 컨테이너 시나리오

**핵심 개념**: 프로파일링, 벤치마킹, 비동기 최적화, 압축, Native AOT

**실습**:
- 느린 API 프로파일링하고 병목 식별
- N+1 쿼리 해결하고 성능 개선 측정
- BenchmarkDotNet으로 최적화 전후 비교
- Native AOT로 콜드 스타트 개선

### [Chapter 23: 모니터링과 로깅](./chapter23/)

프로덕션 환경에서 애플리케이션의 건강을 유지하고, 문제를 빠르게 감지하며, 근본 원인을 추적합니다.

**Application Insights:**
- Azure 통합과 설정
- 자동 텔레메트리 수집
  - 요청, 의존성, 예외
  - 성능 카운터
- 사용자 지정 이벤트와 메트릭
  - `TelemetryClient`
  - 커스텀 차원과 메트릭
- Application Map: 서비스 의존성 시각화
- Live Metrics: 실시간 대시보드
- Failures 분석: 예외와 실패한 요청
- Performance: 느린 요청 식별
- Kusto Query Language (KQL)
  - 쿼리 작성과 집계
  - 시계열 분석
  - 이상 감지

**구조화된 로깅:**
- Serilog 통합
  - 구조화된 로그 작성
  - 메시지 템플릿
- Enrichers
  - `FromLogContext`: 동적 컨텍스트
  - `WithMachineName`, `WithThreadId`
  - 커스텀 Enricher 작성
- Sinks
  - Console, File
  - Application Insights
  - Elasticsearch, Seq
- 로그 레벨과 필터링
  - Verbose, Debug, Information, Warning, Error, Fatal
  - 환경별 로그 레벨

**분산 추적:**
- OpenTelemetry 소개
  - 벤더 중립적 표준
  - Traces, Metrics, Logs
- Activity와 ActivitySource (.NET)
  - 커스텀 Span 생성
  - 태그와 이벤트 추가
- 자동 계측 (Instrumentation)
  - ASP.NET Core, HttpClient, SQL Client
- Exporters
  - Jaeger: 로컬 개발
  - Zipkin: 대안
  - Azure Monitor
- 분산 추적 시각화
  - 요청 흐름 타임라인
  - 병목 식별

**메트릭과 대시보드:**
- Prometheus 통합
  - 메트릭 노출 (`/metrics` 엔드포인트)
  - Counter, Gauge, Histogram, Summary
- Grafana 대시보드
  - 시계열 그래프
  - 알림 규칙
- Kestrel 연결 메트릭 (.NET 9+)
  - 동시 연결 수
  - 요청 큐 길이
- 커스텀 메트릭 생성
  - Business 메트릭 (주문 수, 매출)

**헬스 체크:**
- 헬스 체크 엔드포인트
- 기본 제공 체크
  - DbContext, Redis, URL
- 커스텀 헬스 체크 작성
- Degraded vs Unhealthy
- Kubernetes Liveness/Readiness Probes

**알림과 경고:**
- Application Insights Alerts
  - 메트릭 기반 알림
  - 로그 쿼리 기반 알림
- 오류율 모니터링
- 성능 저하 감지
- 알림 채널 (이메일, Slack, PagerDuty)

**핵심 개념**: Application Insights, Serilog, OpenTelemetry, Prometheus, 헬스 체크

**실습**:
- Application Insights 통합하고 대시보드 구성
- Serilog로 구조화된 로깅 구현
- OpenTelemetry로 마이크로서비스 간 추적 설정
- Prometheus + Grafana 대시보드 구축
- 성능 저하 시 자동 알림 설정

## 성능 최적화 체크리스트

Part 10을 학습하며 다음 원칙들을 내재화하세요:

**측정:**
- [ ] 추측 대신 프로파일링으로 병목 식별
- [ ] 최적화 전후 벤치마크로 검증
- [ ] 프로덕션 환경에서 실제 사용 패턴 측정

**비동기:**
- [ ] I/O 작업은 항상 비동기로
- [ ] `.Result`나 `.Wait()` 피하기
- [ ] CPU 바운드 작업에는 async 불필요

**데이터베이스:**
- [ ] N+1 쿼리 해결 (Include, Eager Loading)
- [ ] 읽기 전용 쿼리는 AsNoTracking
- [ ] 적절한 인덱스 설정
- [ ] Connection Pooling 활용

**캐싱:**
- [ ] HTTP 캐시 헤더 설정
- [ ] 정적 자산은 장기 캐싱
- [ ] 메모리/분산 캐시 적절히 사용
- [ ] 캐시 무효화 전략 명확히

**압축:**
- [ ] Brotli/Gzip 압축 활성화
- [ ] 압축 레벨 최적화 (성능 vs 크기)
- [ ] 정적 자산 사전 압축

**모니터링:**
- [ ] 모든 프로덕션 환경에 모니터링 통합
- [ ] 구조화된 로깅 사용
- [ ] 분산 추적으로 요청 흐름 시각화
- [ ] 헬스 체크로 자동 장애 감지
- [ ] 중요 메트릭에 알림 설정

## 다음 단계

Part 10을 마치면, 여러분은 고성능 애플리케이션을 만들고, 프로덕션 환경에서 모니터링하며, 문제를 빠르게 해결할 수 있습니다. 측정, 최적화, 모니터링—이 사이클을 반복하며 시스템을 지속적으로 개선할 수 있습니다.

**Part 11: 배포와 DevOps**에서는 애플리케이션을 실제 프로덕션 환경에 배포하는 방법을 배웁니다. Docker 컨테이너화, Kubernetes 오케스트레이션, CI/CD 파이프라인, Azure 클라우드 배포... 완전한 DevOps 워크플로우를 마스터하게 될 것입니다.

지금 바로 Chapter 22로 이동하여, 첫 프로파일링을 시작해보세요!

---

## 참고 자료

- [Performance Best Practices in ASP.NET Core](https://docs.microsoft.com/aspnet/core/performance/performance-best-practices)
- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [Application Insights Documentation](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [Serilog](https://serilog.net/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/instrumentation/net/)
- [dotnet-trace](https://docs.microsoft.com/dotnet/core/diagnostics/dotnet-trace)
- [Native AOT Deployment](https://docs.microsoft.com/dotnet/core/deploying/native-aot)
- [Prometheus](https://prometheus.io/)
- [Grafana](https://grafana.com/)
- [Health Checks in ASP.NET Core](https://docs.microsoft.com/aspnet/core/host-and-deploy/health-checks)

**예상 학습 시간**: 2-3주 (실습 포함)
