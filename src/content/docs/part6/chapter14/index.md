---
title: "Chapter 14 - RESTful API 설계와 구현"
---

# Chapter 14: RESTful API 설계와 구현

## REST API의 본질: 리소스 중심 설계

RESTful API는 현대 웹 애플리케이션의 핵심입니다. 프론트엔드와 백엔드를 분리하고, 모바일 앱, 서드파티 통합, 마이크로서비스 간 통신을 가능하게 합니다. Express.js나 NestJS로 API를 만들어본 경험이 있다면, ASP.NET Core의 접근 방식은 친숙하면서도 더 강력한 타입 안전성을 제공합니다.

REST (Representational State Transfer)는 아키텍처 스타일이지 프로토콜이 아닙니다. Roy Fielding이 2000년 박사 논문에서 정의한 이후, 웹 API의 사실상 표준이 되었습니다. REST의 핵심 원칙은:

1. **리소스 중심 (Resource-Oriented)**: URL은 동사가 아닌 명사로 리소스를 표현합니다.
2. **HTTP 메서드 활용**: GET, POST, PUT, PATCH, DELETE로 작업을 표현합니다.
3. **무상태 (Stateless)**: 각 요청은 독립적이며, 서버는 클라이언트 상태를 저장하지 않습니다.
4. **표준 상태 코드**: 200, 201, 400, 404, 500 등으로 결과를 명확히 전달합니다.
5. **HATEOAS (선택적)**: 응답에 관련 리소스의 링크를 포함합니다.

이 챕터에서는 ASP.NET Core로 RESTful API를 설계하고 구현하는 모든 것을 다룹니다.

## 학습 목표

이 챕터를 마치면 다음을 할 수 있습니다:

- REST 원칙을 이해하고 올바른 API 설계를 할 수 있습니다
- `[ApiController]` 특성과 라우팅 템플릿을 사용합니다
- HTTP 메서드별로 적절한 액션 메서드를 구현합니다
- 모델 바인딩으로 요청 데이터를 타입 안전하게 받습니다
- 상태 코드와 응답 형식을 올바르게 반환합니다
- API 버전 관리 전략을 구현합니다
- OpenAPI/Swagger로 API 문서를 자동 생성합니다
- CORS를 구성하여 SPA와 통합합니다

## 주요 내용 개요

### 1. REST API 원칙과 설계
- 리소스 중심 URL 설계
- HTTP 메서드의 적절한 사용 (GET, POST, PUT, PATCH, DELETE)
- 상태 코드의 의미 (2xx, 4xx, 5xx)
- Express.js API와의 패턴 비교
- REST vs RPC vs GraphQL

### 2. 컨트롤러 기반 API
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        // 구현
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        // 구현
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        // 구현
    }

    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
    {
        // 구현
    }

    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        // 구현
    }
}
```

### 3. 모델 바인딩
- `[FromBody]`: 요청 본문에서 JSON 바인딩
- `[FromQuery]`: 쿼리 문자열에서 바인딩
- `[FromRoute]`: URL 경로에서 바인딩
- `[FromHeader]`: 헤더에서 바인딩
- `[FromForm]`: 폼 데이터에서 바인딩

### 4. 응답 형식화
- `ActionResult<T>`: 타입 안전한 응답
- Content Negotiation (JSON, XML)
- 커스텀 포맷터
- System.Text.Json vs Newtonsoft.Json

### 5. 유효성 검사
- Data Annotations (`[Required]`, `[StringLength]`, `[Range]`)
- FluentValidation
- 모델 상태 검증
- 커스텀 검증 로직

### 6. 에러 처리
- ProblemDetails (RFC 7807)
- Global Exception Handler
- 일관된 에러 응답 형식
- HTTP 상태 코드 가이드

### 7. API 버전 관리
- URL 기반 버전 관리: `/api/v1/products`
- 헤더 기반 버전 관리
- 쿼리 문자열 기반
- Microsoft.AspNetCore.Mvc.Versioning

### 8. API 문서화
- OpenAPI/Swagger 통합 (.NET 9+ 내장 지원)
- XML 주석 활용
- 예제 응답 정의
- Swagger UI 커스터마이징

### 9. CORS 구성
- CORS의 필요성
- 정책 정의와 적용
- Preflight 요청 처리
- 보안 고려사항

### 10. 페이징, 필터링, 정렬
- 쿼리 문자열 파라미터
- 커서 기반 vs 오프셋 기반 페이징
- 필터링 전략
- 정렬 구현

## 실습 프로젝트

### 전자상거래 REST API
이 챕터의 실습으로 전자상거래 API를 구축합니다:

**리소스:**
- Products (제품)
- Categories (카테고리)
- Orders (주문)
- Users (사용자)

**엔드포인트 예제:**
```
GET    /api/v1/products              - 제품 목록 (페이징, 필터링)
GET    /api/v1/products/{id}         - 제품 상세
POST   /api/v1/products              - 제품 생성 (Admin)
PUT    /api/v1/products/{id}         - 제품 수정 (Admin)
DELETE /api/v1/products/{id}         - 제품 삭제 (Admin)
GET    /api/v1/products/{id}/reviews - 제품 리뷰 목록
POST   /api/v1/products/{id}/reviews - 리뷰 작성
GET    /api/v1/categories            - 카테고리 목록
GET    /api/v1/orders                - 내 주문 목록
POST   /api/v1/orders                - 주문 생성
GET    /api/v1/orders/{id}           - 주문 상세
```

## 코드 예제: 완전한 CRUD API

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 제품 목록을 가져옵니다 (페이징, 필터링 지원)
    /// </summary>
    /// <param name="page">페이지 번호 (기본값: 1)</param>
    /// <param name="pageSize">페이지당 항목 수 (기본값: 10)</param>
    /// <param name="category">카테고리 필터 (선택)</param>
    /// <param name="search">검색어 (선택)</param>
    /// <returns>제품 목록</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? category = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();

        // 필터링
        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category == category);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

        // 총 개수 (CancellationToken 전달)
        var totalCount = await query.CountAsync(cancellationToken);

        // 페이징 (CancellationToken 전달)
        var products = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<ProductDto>
        {
            Items = products,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return Ok(result);
    }

    /// <summary>
    /// 특정 제품을 가져옵니다
    /// </summary>
    /// <param name="id">제품 ID</param>
    /// <returns>제품 정보</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(int id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", id);
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Product not found",
                Detail = $"Product with ID {id} does not exist"
            });
        }

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category,
            ImageUrl = product.ImageUrl
        };

        return Ok(dto);
    }

    /// <summary>
    /// 새 제품을 생성합니다
    /// </summary>
    /// <param name="dto">제품 생성 정보</param>
    /// <returns>생성된 제품</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Category = dto.Category,
            ImageUrl = dto.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product created with ID {ProductId}", product.Id);

        var resultDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category,
            ImageUrl = product.ImageUrl
        };

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, resultDto);
    }

    /// <summary>
    /// 제품을 수정합니다
    /// </summary>
    /// <param name="id">제품 ID</param>
    /// <param name="dto">수정할 정보</param>
    /// <returns>수정된 제품</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Category = dto.Category;
        product.ImageUrl = dto.ImageUrl;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Product {ProductId} updated", id);

        var resultDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category,
            ImageUrl = product.ImageUrl
        };

        return Ok(resultDto);
    }

    /// <summary>
    /// 제품을 삭제합니다
    /// </summary>
    /// <param name="id">제품 ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product {ProductId} deleted", id);

        return NoContent();
    }
}
```

## DTOs (Data Transfer Objects)

```csharp
public record ProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Category { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
}

public record CreateProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string Description { get; init; } = string.Empty;

    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; init; }

    [Required]
    public string Category { get; init; } = string.Empty;

    [Url]
    public string? ImageUrl { get; init; }
}

public record UpdateProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string Description { get; init; } = string.Empty;

    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; init; }

    [Required]
    public string Category { get; init; } = string.Empty;

    [Url]
    public string? ImageUrl { get; init; }
}

public record PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}
```

## 다음 단계

이 챕터의 기초를 닦았다면, 다음 단계로:

1. **Chapter 15**로 이동하여 API 보안과 인증을 배웁니다 (JWT, OAuth, API 키)
2. **Chapter 16**에서 GraphQL과 SignalR로 더 고급 API 패턴을 탐구합니다
3. 실제 프로젝트에 배운 내용을 적용하고, Postman이나 Thunder Client로 테스트합니다

## 연습 문제

1. 완전한 CRUD API를 직접 구현해보세요 (블로그, TODO, 또는 원하는 도메인)
2. 페이징, 필터링, 정렬을 모두 지원하는 엔드포인트를 만드세요
3. Swagger UI를 커스터마이징하고, XML 주석으로 문서를 풍부하게 만드세요
4. API 버전 관리를 구현하고, v1과 v2를 동시에 제공해보세요
5. CORS를 구성하고, React나 Vue 프론트엔드와 통합해보세요

## 참고 자료

- [ASP.NET Core Web API 문서](https://docs.microsoft.com/aspnet/core/web-api/)
- [RESTful API 설계 가이드](https://restfulapi.net/)
- [HTTP 상태 코드](https://developer.mozilla.org/docs/Web/HTTP/Status)
- [OpenAPI Specification](https://swagger.io/specification/)
- [API Versioning in ASP.NET Core](https://github.com/dotnet/aspnet-api-versioning)
