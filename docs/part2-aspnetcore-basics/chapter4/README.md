# Chapter 4: Minimal APIs - Node.js Express 개발자를 위한 빠른 시작

## 4.1 Minimal APIs 소개

### 컨트롤러 없는 API: Express.js 스타일

지금까지 살펴본 컨트롤러 기반 API는 구조화되고 강력하지만, 간단한 API를 만들 때는 다소 복잡하게 느껴질 수 있습니다. 특히 Express.js처럼 빠르게 엔드포인트를 추가하고 싶을 때는 더욱 그렇습니다.

.NET 6부터 도입된 Minimal APIs는 이러한 니즈를 정확히 충족시킵니다. 컨트롤러 클래스 없이, `Program.cs`에서 직접 라우트를 정의할 수 있습니다. Express.js의 간결함과 ASP.NET Core의 강력함을 결합한 것입니다.

**Express.js 예제**:
```javascript
const express = require('express');
const app = express();

app.use(express.json());

app.get('/api/users', (req, res) => {
  res.json([{ id: 1, name: 'Alice' }, { id: 2, name: 'Bob' }]);
});

app.get('/api/users/:id', (req, res) => {
  const id = parseInt(req.params.id);
  res.json({ id, name: 'Alice' });
});

app.post('/api/users', (req, res) => {
  const user = req.body;
  res.status(201).json({ id: 3, ...user });
});

app.listen(3000, () => console.log('Server running on port 3000'));
```

**ASP.NET Core Minimal APIs 예제**:
```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/users", () =>
{
    return new[] { new { id = 1, name = "Alice" }, new { id = 2, name = "Bob" } };
});

app.MapGet("/api/users/{id}", (int id) =>
{
    return new { id, name = "Alice" };
});

app.MapPost("/api/users", (User user) =>
{
    return Results.Created($"/api/users/{3}", new { id = 3, name = user.Name });
});

app.Run();

record User(string Name, string Email);
```

놀랍도록 유사하죠? Minimal APIs는 Express.js의 간결함을 ASP.NET Core에 가져왔습니다. 하지만 여전히 강력한 타입 시스템, 의존성 주입, 자동 바인딩, OpenAPI 생성 등의 이점을 누릴 수 있습니다.

### 언제 Minimal APIs를 사용할까?

Minimal APIs와 컨트롤러 기반 API 중 어느 것을 선택해야 할까요? 각각의 장단점을 이해하고 적절한 상황에서 사용하는 것이 중요합니다.

**Minimal APIs가 적합한 경우**:

1. **마이크로서비스**: 작고 집중된 서비스에 완벽합니다
   ```csharp
   // 날씨 서비스 마이크로서비스
   var app = builder.Build();

   app.MapGet("/weather", async (IWeatherService weather) =>
   {
       return await weather.GetForecastAsync();
   });

   app.MapGet("/weather/{city}", async (string city, IWeatherService weather) =>
   {
       return await weather.GetForecastForCityAsync(city);
   });

   app.Run();
   ```

2. **프로토타입과 빠른 개발**: MVP나 PoC를 빠르게 만들 때
   ```csharp
   // 간단한 URL 단축 서비스
   var urls = new Dictionary<string, string>();

   app.MapPost("/shorten", (string url) =>
   {
       var shortCode = Guid.NewGuid().ToString()[..8];
       urls[shortCode] = url;
       return new { shortCode, url };
   });

   app.MapGet("/{code}", (string code) =>
   {
       return urls.TryGetValue(code, out var url)
           ? Results.Redirect(url)
           : Results.NotFound();
   });
   ```

3. **간단한 API**: 엔드포인트가 10개 미만인 API
4. **서버리스 함수**: Azure Functions, AWS Lambda에 배포할 때
5. **학습 목적**: ASP.NET Core를 처음 배울 때

**컨트롤러가 더 적합한 경우**:

1. **대규모 API**: 수십 개 이상의 엔드포인트가 있을 때
2. **복잡한 비즈니스 로직**: 여러 레이어와 서비스가 필요한 경우
3. **필터가 많이 필요할 때**: Authorization, Validation, Caching 등
4. **기존 프로젝트**: 이미 컨트롤러 기반으로 구축된 경우

실제로는 두 가지를 혼합할 수도 있습니다:
```csharp
// Minimal APIs로 헬스 체크, 간단한 유틸리티 엔드포인트
app.MapGet("/health", () => "OK");
app.MapGet("/version", () => new { version = "1.0.0" });

// 컨트롤러로 복잡한 비즈니스 API
app.MapControllers();
```

### 마이크로서비스에 최적화된 접근

Minimal APIs는 마이크로서비스 아키텍처에 완벽하게 맞습니다. 각 서비스가 작고 집중되어 있으며, 빠르게 시작하고 적은 메모리를 사용합니다.

**주문 서비스 예제**:
```csharp
var builder = WebApplication.CreateBuilder(args);

// 서비스 등록
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Orders")));

var app = builder.Build();

// 주문 생성
app.MapPost("/orders", async (CreateOrderRequest request, IOrderService orderService) =>
{
    var order = await orderService.CreateAsync(request);
    return Results.Created($"/orders/{order.Id}", order);
});

// 주문 조회
app.MapGet("/orders/{id}", async (int id, IOrderService orderService) =>
{
    var order = await orderService.GetByIdAsync(id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
});

// 주문 목록
app.MapGet("/orders", async (int page, int pageSize, IOrderService orderService) =>
{
    var orders = await orderService.GetPagedAsync(page, pageSize);
    return Results.Ok(orders);
});

// 주문 취소
app.MapDelete("/orders/{id}", async (int id, IOrderService orderService) =>
{
    var success = await orderService.CancelAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
});

app.Run();
```

이 서비스는 다음과 같은 이점이 있습니다:
- **빠른 시작 시간**: 몇 밀리초 내에 시작
- **작은 메모리 풋프린트**: 컨트롤러보다 적은 리소스 사용
- **명확한 엔드포인트**: 모든 라우트가 한눈에 보임
- **쉬운 배포**: 컨테이너화와 오케스트레이션에 적합

## 4.2 엔드포인트 정의와 라우팅

### HTTP 메서드 매핑

Minimal APIs는 모든 HTTP 메서드를 지원하며, 각각에 대응하는 메서드를 제공합니다.

```csharp
// GET 요청
app.MapGet("/products", () => "Get all products");

// POST 요청
app.MapPost("/products", (Product product) => "Create product");

// PUT 요청
app.MapPut("/products/{id}", (int id, Product product) => "Update product");

// DELETE 요청
app.MapDelete("/products/{id}", (int id) => "Delete product");

// PATCH 요청
app.MapPatch("/products/{id}", (int id, ProductPatch patch) => "Patch product");

// 여러 메서드 허용
app.MapMethods("/products/{id}", new[] { "HEAD", "OPTIONS" },
    (int id) => Results.Ok());

// 모든 HTTP 메서드 허용 (권장하지 않음)
app.Map("/debug", () => "Any HTTP method");
```

Express.js와 비교:
```javascript
app.get('/products', (req, res) => res.send('Get all'));
app.post('/products', (req, res) => res.send('Create'));
app.put('/products/:id', (req, res) => res.send('Update'));
app.delete('/products/:id', (req, res) => res.send('Delete'));
app.patch('/products/:id', (req, res) => res.send('Patch'));
```

### 라우트 매개변수와 쿼리 문자열

**라우트 매개변수**:
```csharp
// 단일 매개변수
app.MapGet("/users/{id}", (int id) =>
{
    return new { id, name = "User" };
});

// 여러 매개변수
app.MapGet("/posts/{year}/{month}/{slug}", (int year, int month, string slug) =>
{
    return new { year, month, slug };
});

// 선택적 매개변수
app.MapGet("/search/{term?}", (string? term) =>
{
    return term is null
        ? "Show all"
        : $"Search for: {term}";
});

// 타입 제약
app.MapGet("/products/{id:int}", (int id) => $"Product {id}");
app.MapGet("/products/{id:int:min(1)}", (int id) => $"Product {id}");
app.MapGet("/products/{slug:alpha}", (string slug) => $"Product {slug}");
```

**쿼리 문자열**:
```csharp
// 개별 쿼리 매개변수
app.MapGet("/products", (int? page, int? pageSize, string? sort) =>
{
    var currentPage = page ?? 1;
    var size = pageSize ?? 10;
    return new { page = currentPage, pageSize = size, sort };
});

// DTO로 바인딩
app.MapGet("/search", ([AsParameters] SearchQuery query) =>
{
    return new { query.Term, query.Page, query.PageSize };
});

record SearchQuery(string? Term, int Page = 1, int PageSize = 10);
```

URL: `/products?page=2&pageSize=20&sort=name`

Express.js와 비교:
```javascript
// Express.js
app.get('/products', (req, res) => {
  const page = parseInt(req.query.page) || 1;
  const pageSize = parseInt(req.query.pageSize) || 10;
  const sort = req.query.sort;
  res.json({ page, pageSize, sort });
});
```

ASP.NET Core의 장점은 자동 타입 변환입니다. 문자열을 수동으로 파싱할 필요가 없고, 타입이 맞지 않으면 자동으로 400 Bad Request를 반환합니다.

### 요청 본문 바인딩

POST나 PUT 요청의 본문은 자동으로 객체로 바인딩됩니다.

**JSON 본문**:
```csharp
app.MapPost("/products", (Product product) =>
{
    // product 객체가 자동으로 역직렬화됨
    return Results.Created($"/products/{product.Id}", product);
});

record Product(int Id, string Name, decimal Price);
```

요청 예제:
```json
POST /products
Content-Type: application/json

{
  "id": 1,
  "name": "Laptop",
  "price": 999.99
}
```

**복잡한 바인딩**:
```csharp
// 라우트 매개변수 + 본문
app.MapPut("/products/{id}", (int id, Product product) =>
{
    if (id != product.Id)
    {
        return Results.BadRequest("ID mismatch");
    }
    // 업데이트 로직...
    return Results.NoContent();
});

// 쿼리 + 본문
app.MapPost("/products", (bool draft, Product product) =>
{
    return draft
        ? Results.Ok(new { message = "Saved as draft", product })
        : Results.Created($"/products/{product.Id}", product);
});
```

**명시적 바인딩 소스**:
때로는 명시적으로 바인딩 소스를 지정해야 합니다.
```csharp
app.MapPost("/users",
    ([FromBody] User user,
     [FromHeader(Name = "X-API-Key")] string apiKey,
     [FromQuery] bool sendEmail) =>
{
    // user는 본문에서
    // apiKey는 헤더에서
    // sendEmail은 쿼리에서
    return Results.Ok();
});
```

**폼 데이터**:
```csharp
app.MapPost("/upload", async (IFormFile file) =>
{
    if (file.Length == 0)
    {
        return Results.BadRequest("No file uploaded");
    }

    var path = Path.Combine("uploads", file.FileName);
    using var stream = File.Create(path);
    await file.CopyToAsync(stream);

    return Results.Ok(new { fileName = file.FileName, size = file.Length });
});

// 여러 파일
app.MapPost("/upload-multiple", async (IFormFileCollection files) =>
{
    var uploadedFiles = new List<string>();

    foreach (var file in files)
    {
        var path = Path.Combine("uploads", file.FileName);
        using var stream = File.Create(path);
        await file.CopyToAsync(stream);
        uploadedFiles.Add(file.FileName);
    }

    return Results.Ok(new { count = uploadedFiles.Count, files = uploadedFiles });
});
```

## 4.3 의존성 주입과 서비스 사용

Minimal APIs에서도 의존성 주입을 완벽하게 지원합니다. 엔드포인트 핸들러의 매개변수로 서비스를 요청하면 자동으로 주입됩니다.

### 람다 매개변수를 통한 DI

```csharp
// 서비스 등록
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ILogger<Program>, Logger<Program>>();

// 엔드포인트에서 사용
app.MapGet("/products", async (IProductService productService, ILogger<Program> logger) =>
{
    logger.LogInformation("Fetching all products");
    var products = await productService.GetAllAsync();
    return Results.Ok(products);
});

app.MapGet("/products/{id}", async (int id, IProductService productService) =>
{
    var product = await productService.GetByIdAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.MapPost("/products", async (Product product, IProductService productService) =>
{
    var created = await productService.CreateAsync(product);
    return Results.Created($"/products/{created.Id}", created);
});
```

ASP.NET Core는 매개변수의 타입을 보고 자동으로 판단합니다:
- **서비스 타입** (인터페이스, 등록된 클래스): DI 컨테이너에서 주입
- **특수 타입** (`HttpContext`, `HttpRequest`, `HttpResponse`, `ClaimsPrincipal` 등): 자동 제공
- **값 타입이나 간단한 타입**: 라우트 매개변수나 쿼리 문자열에서 바인딩
- **복잡한 타입**: 요청 본문에서 역직렬화

**여러 서비스 주입**:
```csharp
app.MapPost("/orders", async (
    Order order,
    IOrderService orderService,
    IPaymentService paymentService,
    IEmailService emailService,
    ILogger<Program> logger) =>
{
    logger.LogInformation("Creating order {OrderId}", order.Id);

    try
    {
        // 1. 결제 처리
        var paymentResult = await paymentService.ProcessAsync(order.Total);
        if (!paymentResult.Success)
        {
            return Results.BadRequest("Payment failed");
        }

        // 2. 주문 저장
        var created = await orderService.CreateAsync(order);

        // 3. 이메일 발송
        await emailService.SendOrderConfirmationAsync(created);

        return Results.Created($"/orders/{created.Id}", created);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error creating order");
        return Results.Problem("An error occurred");
    }
});
```

### `[FromServices]` 특성

일반적으로 필요하지 않지만, 명시적으로 DI를 나타내고 싶거나 모호성을 해결할 때 사용합니다.

```csharp
app.MapGet("/products/{id}",
    async (int id, [FromServices] IProductService productService) =>
{
    var product = await productService.GetByIdAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});
```

### 데이터베이스 컨텍스트 주입

Entity Framework Core의 DbContext도 직접 주입할 수 있습니다.

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// 직접 DbContext 사용
app.MapGet("/users", async (AppDbContext db) =>
{
    return await db.Users.ToListAsync();
});

app.MapGet("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/users", async (User user, AppDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null)
    {
        return Results.NotFound();
    }

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
```

Prisma (Node.js)와 비교:
```typescript
// Prisma
app.get('/users', async (req, res) => {
  const users = await prisma.user.findMany();
  res.json(users);
});

app.post('/users', async (req, res) => {
  const user = await prisma.user.create({ data: req.body });
  res.status(201).json(user);
});
```

## 4.4 응답 형식과 상태 코드

### `Results` 헬퍼: `Ok()`, `NotFound()`, `BadRequest()`

`Results` 클래스는 다양한 HTTP 응답을 생성하는 헬퍼 메서드를 제공합니다. Express.js의 `res.status()`, `res.json()` 등과 유사합니다.

**기본 응답**:
```csharp
// 200 OK
app.MapGet("/ok", () => Results.Ok(new { message = "Success" }));

// 201 Created
app.MapPost("/users", (User user) =>
    Results.Created($"/users/{user.Id}", user));

// 204 No Content
app.MapDelete("/users/{id}", (int id) => Results.NoContent());

// 400 Bad Request
app.MapPost("/validate", (Data data) =>
{
    if (string.IsNullOrEmpty(data.Name))
    {
        return Results.BadRequest("Name is required");
    }
    return Results.Ok(data);
});

// 401 Unauthorized
app.MapGet("/protected", () => Results.Unauthorized());

// 403 Forbidden
app.MapGet("/admin", () => Results.Forbid());

// 404 Not Found
app.MapGet("/users/{id}", (int id) => Results.NotFound());

// 409 Conflict
app.MapPost("/users", (User user) =>
    Results.Conflict("User already exists"));

// 500 Internal Server Error
app.MapGet("/error", () => Results.Problem("Something went wrong"));
```

Express.js와 비교:
```javascript
app.get('/ok', (req, res) => res.json({ message: 'Success' }));
app.post('/users', (req, res) => res.status(201).json(user));
app.delete('/users/:id', (req, res) => res.sendStatus(204));
app.post('/validate', (req, res) => res.status(400).send('Name is required'));
app.get('/protected', (req, res) => res.sendStatus(401));
app.get('/users/:id', (req, res) => res.sendStatus(404));
```

**리디렉션**:
```csharp
// 302 Found (임시 리디렉션)
app.MapGet("/old-url", () => Results.Redirect("/new-url"));

// 301 Moved Permanently (영구 리디렉션)
app.MapGet("/old-url", () => Results.RedirectPermanent("/new-url"));

// 다른 호스트로 리디렉션
app.MapGet("/external", () => Results.Redirect("https://example.com"));
```

**파일 응답**:
```csharp
// 파일 다운로드
app.MapGet("/download/{fileName}", (string fileName) =>
{
    var filePath = Path.Combine("files", fileName);
    if (!File.Exists(filePath))
    {
        return Results.NotFound();
    }
    return Results.File(filePath, "application/octet-stream", fileName);
});

// 이미지 응답
app.MapGet("/images/{id}", (int id) =>
{
    var imagePath = $"images/{id}.jpg";
    return Results.File(imagePath, "image/jpeg");
});

// 스트리밍
app.MapGet("/stream", async (HttpContext context) =>
{
    context.Response.ContentType = "text/plain";
    for (int i = 0; i < 10; i++)
    {
        await context.Response.WriteAsync($"Line {i}\n");
        await context.Response.Body.FlushAsync();
        await Task.Delay(1000);
    }
});
```

### TypedResults를 활용한 타입 안전성

.NET 7부터는 `TypedResults`를 사용하여 컴파일 타임 타입 안전성을 확보할 수 있습니다.

```csharp
// 일반 Results (런타임 타입)
app.MapGet("/users/{id}", (int id) =>
{
    var user = GetUser(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

// TypedResults (컴파일 타임 타입)
app.MapGet("/users/{id}", Results<Ok<User>, NotFound> (int id) =>
{
    var user = GetUser(id);
    return user is not null ? TypedResults.Ok(user) : TypedResults.NotFound();
});
```

`TypedResults`의 장점:
1. **컴파일 타임 타입 체크**: 반환 타입이 정확히 명시됨
2. **OpenAPI 정확도**: Swagger 문서에 정확한 응답 타입 표시
3. **IDE 지원**: 더 나은 IntelliSense와 자동완성

**실제 예제**:
```csharp
app.MapGet("/products/{id}", async Task<Results<Ok<Product>, NotFound>> (
    int id,
    IProductService productService) =>
{
    var product = await productService.GetByIdAsync(id);
    return product is not null
        ? TypedResults.Ok(product)
        : TypedResults.NotFound();
});

app.MapPost("/products", async Task<Results<Created<Product>, BadRequest<string>>> (
    Product product,
    IProductService productService) =>
{
    if (string.IsNullOrEmpty(product.Name))
    {
        return TypedResults.BadRequest("Name is required");
    }

    var created = await productService.CreateAsync(product);
    return TypedResults.Created($"/products/{created.Id}", created);
});
```

### 커스텀 응답 생성

표준 응답 외에 완전히 커스텀한 응답을 만들 수도 있습니다.

```csharp
// HttpContext를 직접 사용
app.MapGet("/custom", async (HttpContext context) =>
{
    context.Response.StatusCode = 200;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new
    {
        timestamp = DateTime.UtcNow,
        message = "Custom response"
    });
});

// 커스텀 헤더
app.MapGet("/with-headers", () =>
{
    var response = Results.Ok(new { data = "value" });
    // Results는 불변이므로 HttpContext를 직접 조작
    return Results.Ok(new { data = "value" });
});

app.MapGet("/with-headers2", (HttpContext context) =>
{
    context.Response.Headers["X-Custom-Header"] = "MyValue";
    context.Response.Headers["X-Request-Id"] = Guid.NewGuid().ToString();
    return Results.Ok(new { data = "value" });
});
```

**ProblemDetails 표준 응답**:
```csharp
app.MapGet("/problem", () =>
    Results.Problem(
        title: "An error occurred",
        detail: "The resource you requested could not be found",
        statusCode: 404,
        instance: "/problem"
    ));

// 커스텀 ProblemDetails
app.MapPost("/validate", (Product product) =>
{
    var errors = new Dictionary<string, string[]>
    {
        ["name"] = new[] { "Name is required" },
        ["price"] = new[] { "Price must be greater than 0" }
    };

    return Results.ValidationProblem(errors, title: "Validation failed");
});
```

ProblemDetails 응답:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "An error occurred",
  "status": 404,
  "detail": "The resource you requested could not be found",
  "instance": "/problem"
}
```

## 4.5 OpenAPI/Swagger 통합

### 자동 API 문서 생성

Minimal APIs는 OpenAPI 사양을 자동으로 생성하여 Swagger UI로 시각화할 수 있습니다.

**기본 설정**:
```csharp
var builder = WebApplication.CreateBuilder(args);

// .NET 8까지
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI (개발 환경에서만)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/products", () => new[] { "Product1", "Product2" });

app.Run();
```

브라우저에서 `/swagger`를 열면 자동 생성된 API 문서를 볼 수 있습니다.

**문서화 개선**:
```csharp
app.MapGet("/products/{id}", async (int id, IProductService service) =>
{
    var product = await service.GetByIdAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProduct")  // 작업 이름
.WithDescription("Get a product by ID")  // 설명
.WithSummary("Get Product")  // 요약
.WithTags("Products")  // 태그로 그룹화
.Produces<Product>(200)  // 200 응답 타입
.Produces(404);  // 404 응답

app.MapPost("/products", async (Product product, IProductService service) =>
{
    var created = await service.CreateAsync(product);
    return Results.Created($"/products/{created.Id}", created);
})
.WithName("CreateProduct")
.WithTags("Products")
.Accepts<Product>("application/json")  // 입력 타입
.Produces<Product>(201)
.Produces<ProblemDetails>(400);
```

**엔드포인트 그룹화**:
```csharp
var products = app.MapGroup("/api/products")
    .WithTags("Products")
    .WithOpenApi();

products.MapGet("/", async (IProductService service) =>
    await service.GetAllAsync());

products.MapGet("/{id}", async (int id, IProductService service) =>
{
    var product = await service.GetByIdAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

products.MapPost("/", async (Product product, IProductService service) =>
{
    var created = await service.CreateAsync(product);
    return Results.Created($"/api/products/{created.Id}", created);
});

products.MapPut("/{id}", async (int id, Product product, IProductService service) =>
{
    if (id != product.Id) return Results.BadRequest();
    await service.UpdateAsync(product);
    return Results.NoContent();
});

products.MapDelete("/{id}", async (int id, IProductService service) =>
{
    await service.DeleteAsync(id);
    return Results.NoContent();
});
```

### .NET 9의 내장 OpenAPI 지원

.NET 9부터는 별도 패키지 없이 OpenAPI를 지원합니다.

```csharp
var builder = WebApplication.CreateBuilder(args);

// .NET 9: 내장 OpenAPI 지원
builder.Services.AddOpenApi();

var app = builder.Build();

// OpenAPI JSON 엔드포인트
app.MapOpenApi();

// Scalar UI (Swagger UI보다 현대적)
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app.MapGet("/products", () => new[] { "Product1", "Product2" })
    .WithOpenApi(operation =>
    {
        operation.Summary = "Get all products";
        operation.Description = "Returns a list of all available products";
        return operation;
    });

app.Run();
```

`.NET 9의 개선사항`:
- 더 빠른 시작 시간
- 더 작은 메모리 사용량
- TypedResults와의 완벽한 통합
- 더 정확한 스키마 생성

### API 테스팅과 문서화

Swagger UI에서 직접 API를 테스트할 수 있지만, 프로그래밍 방식으로도 가능합니다.

**HTTP 파일로 테스트** (`.http` or `.rest` 파일):
```http
@baseUrl = https://localhost:5001/api

### Get all products
GET {{baseUrl}}/products

### Get product by ID
GET {{baseUrl}}/products/1

### Create product
POST {{baseUrl}}/products
Content-Type: application/json

{
  "name": "New Product",
  "price": 29.99,
  "categoryId": 1
}

### Update product
PUT {{baseUrl}}/products/1
Content-Type: application/json

{
  "id": 1,
  "name": "Updated Product",
  "price": 39.99,
  "categoryId": 1
}

### Delete product
DELETE {{baseUrl}}/products/1
```

**통합 테스트**:
```csharp
public class ProductsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreated()
    {
        // Arrange
        var product = new Product { Name = "Test", Price = 19.99m };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", product);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Product>();
        Assert.Equal(product.Name, created.Name);
    }
}
```

## 4.6 실습: RESTful API 완성

이제 배운 내용을 종합하여 완전한 RESTful API를 만들어봅시다.

### CRUD 엔드포인트 구현

**Todo API 전체 구현**:

```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 서비스 등록
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoDb"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Todo 엔드포인트 그룹
var todos = app.MapGroup("/api/todos").WithTags("Todos");

// GET /api/todos - 전체 조회
todos.MapGet("/", async (TodoDbContext db, int? page, int? pageSize) =>
{
    var currentPage = page ?? 1;
    var size = pageSize ?? 10;

    var totalCount = await db.Todos.CountAsync();
    var items = await db.Todos
        .OrderByDescending(t => t.CreatedAt)
        .Skip((currentPage - 1) * size)
        .Take(size)
        .ToListAsync();

    return Results.Ok(new
    {
        page = currentPage,
        pageSize = size,
        totalCount,
        totalPages = (int)Math.Ceiling(totalCount / (double)size),
        items
    });
})
.WithSummary("Get all todos")
.Produces<PaginatedResponse<Todo>>(200);

// GET /api/todos/{id} - 단일 조회
todos.MapGet("/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
})
.WithSummary("Get todo by ID")
.Produces<Todo>(200)
.Produces(404);

// POST /api/todos - 생성
todos.MapPost("/", async (CreateTodoDto dto, TodoDbContext db) =>
{
    // 유효성 검사
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(dto);

    if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
    {
        var errors = validationResults.ToDictionary(
            vr => vr.MemberNames.First(),
            vr => new[] { vr.ErrorMessage! }
        );
        return Results.ValidationProblem(errors);
    }

    var todo = new Todo
    {
        Title = dto.Title,
        Description = dto.Description,
        IsCompleted = false,
        CreatedAt = DateTime.UtcNow
    };

    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/api/todos/{todo.Id}", todo);
})
.WithSummary("Create a new todo")
.Produces<Todo>(201)
.Produces<ProblemDetails>(400);

// PUT /api/todos/{id} - 수정
todos.MapPut("/{id}", async (int id, UpdateTodoDto dto, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }

    todo.Title = dto.Title;
    todo.Description = dto.Description;
    todo.IsCompleted = dto.IsCompleted;
    todo.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.Ok(todo);
})
.WithSummary("Update a todo")
.Produces<Todo>(200)
.Produces(404);

// PATCH /api/todos/{id}/complete - 완료 토글
todos.MapPatch("/{id}/complete", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }

    todo.IsCompleted = !todo.IsCompleted;
    todo.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.Ok(todo);
})
.WithSummary("Toggle todo completion status")
.Produces<Todo>(200)
.Produces(404);

// DELETE /api/todos/{id} - 삭제
todos.MapDelete("/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithSummary("Delete a todo")
.Produces(204)
.Produces(404);

// GET /api/todos/search - 검색
todos.MapGet("/search", async (string? q, TodoDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(q))
    {
        return Results.BadRequest("Search query is required");
    }

    var results = await db.Todos
        .Where(t => t.Title.Contains(q) || (t.Description != null && t.Description.Contains(q)))
        .ToListAsync();

    return Results.Ok(results);
})
.WithSummary("Search todos")
.Produces<List<Todo>>(200)
.Produces(400);

app.Run();

// 모델
public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// DTOs
public record CreateTodoDto(
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    string Title,

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    string? Description
);

public record UpdateTodoDto(
    [Required] string Title,
    string? Description,
    bool IsCompleted
);

public record PaginatedResponse<T>(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    List<T> Items
);

// DbContext
public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}
```

### 유효성 검사 추가

Data Annotations를 사용한 자동 유효성 검사:

```csharp
public record CreateProductDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
    public decimal Price { get; init; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CategoryId { get; init; }

    [EmailAddress]
    public string? ContactEmail { get; init; }

    [Url]
    public string? WebsiteUrl { get; init; }
}
```

수동 유효성 검사:
```csharp
app.MapPost("/products", async (CreateProductDto dto, IProductService service) =>
{
    // 커스텀 유효성 검사
    if (await service.ProductExistsAsync(dto.Name))
    {
        return Results.Problem(
            statusCode: 409,
            title: "Product already exists",
            detail: $"A product with the name '{dto.Name}' already exists"
        );
    }

    var product = await service.CreateAsync(dto);
    return Results.Created($"/products/{product.Id}", product);
});
```

FluentValidation 사용:
```csharp
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 100)
            .Must(BeValidProductName).WithMessage("Product name contains invalid characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Invalid category");
    }

    private bool BeValidProductName(string name)
    {
        return !name.Contains("forbidden");
    }
}
```

### 에러 응답 표준화

일관된 에러 응답 형식을 만들어봅시다:

```csharp
public record ApiError(
    string Type,
    string Title,
    int Status,
    string Detail,
    Dictionary<string, string[]>? Errors = null
);

public static class ApiErrorExtensions
{
    public static IResult ToValidationProblem(this List<ValidationResult> validationResults)
    {
        var errors = validationResults
            .GroupBy(vr => vr.MemberNames.First())
            .ToDictionary(
                g => g.Key,
                g => g.Select(vr => vr.ErrorMessage!).ToArray()
            );

        return Results.ValidationProblem(errors, title: "Validation failed");
    }

    public static IResult ToNotFound(string resource, object id)
    {
        return Results.Problem(
            statusCode: 404,
            title: "Resource not found",
            detail: $"{resource} with ID {id} was not found"
        );
    }

    public static IResult ToConflict(string message)
    {
        return Results.Problem(
            statusCode: 409,
            title: "Conflict",
            detail: message
        );
    }
}

// 사용
app.MapGet("/products/{id}", async (int id, IProductService service) =>
{
    var product = await service.GetByIdAsync(id);
    return product is not null
        ? Results.Ok(product)
        : ApiErrorExtensions.ToNotFound("Product", id);
});
```

---

이것으로 Chapter 4가 완료되었습니다! Minimal APIs를 통해 Express.js처럼 간결하면서도 ASP.NET Core의 강력한 기능을 활용하는 API를 만드는 방법을 배웠습니다.

## 다음 단계

Part 3에서는 서버 사이드 렌더링인 Razor Pages와 MVC를 다룹니다:
- Razor 문법과 JSX의 비교
- Razor Pages로 서버 렌더링 웹 앱 만들기
- MVC 패턴의 이해
- 폼 처리와 유효성 검사

실습을 통해 각 개념을 확실히 익히세요!
