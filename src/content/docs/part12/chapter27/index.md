---
title: "Chapter 27 - 전자상거래 플랫폼 구축 (종합 프로젝트)"
---

# Chapter 27: 전자상거래 플랫폼 구축 (종합 프로젝트)

## 모든 것을 하나로: EShop 프로젝트

이제 여러분이 기다려온 순간입니다. 지금까지 배운 모든 개념—API 설계, 인증, 데이터베이스, 실시간 통신, 백그라운드 작업, 테스팅, 성능 최적화—을 하나의 완전한 애플리케이션으로 통합합니다. 단순한 예제가 아니라, **실제 프로덕션에 배포할 수 있는 수준**의 전자상거래 플랫폼을 만듭니다.

프론트엔드 개발자로서 여러분은 이미 복잡한 애플리케이션을 만들어본 경험이 있을 것입니다. React나 Vue로 상태 관리, 라우팅, API 통합을 다루며, 점점 더 복잡한 요구사항을 해결해왔습니다. 백엔드도 마찬가지입니다. 개별 기능을 아는 것과 그것들을 조화롭게 통합하는 것은 다릅니다. 이 챕터에서는 **통합의 예술**을 배웁니다.

## 프로젝트 요구사항

우리가 만들 전자상거래 플랫폼 "EShop"의 요구사항입니다:

**사용자 기능:**
- 회원가입 및 로그인 (이메일/비밀번호, Google OAuth)
- 상품 검색 및 필터링 (카테고리, 가격, 평점)
- 상품 상세 정보 및 리뷰 조회
- 장바구니 관리
- 주문 및 결제 (Stripe 통합)
- 주문 내역 조회
- 실시간 재고 알림

**관리자 기능:**
- 상품 CRUD (생성, 조회, 수정, 삭제)
- 카테고리 관리
- 주문 관리 및 상태 업데이트
- 재고 관리
- 실시간 대시보드 (매출, 주문 수, 활성 사용자)

**비기능적 요구사항:**
- 동시 사용자 1,000명 처리
- API 응답 시간 200ms 이하 (P95)
- 99.9% 가용성
- 포괄적인 테스트 커버리지 (80% 이상)
- 보안 모범 사례 준수
- 포괄적인 로깅과 모니터링

## 아키텍처 설계

**Clean Architecture** 구조를 채택합니다:

```
src/
├── EShop.Domain/              # 도메인 계층 (비즈니스 규칙)
│   ├── Entities/
│   │   ├── Product.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   ├── Customer.cs
│   │   ├── Category.cs
│   │   ├── Review.cs
│   │   └── Cart.cs
│   ├── ValueObjects/
│   │   ├── Money.cs
│   │   ├── Address.cs
│   │   └── Email.cs
│   ├── Enums/
│   │   ├── OrderStatus.cs
│   │   └── PaymentStatus.cs
│   ├── Exceptions/
│   │   ├── DomainException.cs
│   │   └── InsufficientStockException.cs
│   └── Events/
│       ├── OrderCreatedEvent.cs
│       └── ProductStockChangedEvent.cs
│
├── EShop.Application/         # 애플리케이션 계층 (Use Cases)
│   ├── Common/
│   │   ├── Interfaces/
│   │   │   ├── IApplicationDbContext.cs
│   │   │   ├── IDateTime.cs
│   │   │   └── ICurrentUserService.cs
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs
│   │   └── Behaviors/
│   │       ├── ValidationBehavior.cs
│   │       └── LoggingBehavior.cs
│   ├── Products/
│   │   ├── Commands/
│   │   │   ├── CreateProduct/
│   │   │   ├── UpdateProduct/
│   │   │   └── DeleteProduct/
│   │   └── Queries/
│   │       ├── GetProducts/
│   │       └── GetProductDetail/
│   ├── Orders/
│   │   ├── Commands/
│   │   │   ├── CreateOrder/
│   │   │   ├── UpdateOrderStatus/
│   │   │   └── CancelOrder/
│   │   └── Queries/
│   │       ├── GetOrders/
│   │       └── GetOrderDetail/
│   ├── Carts/
│   │   ├── Commands/
│   │   │   ├── AddToCart/
│   │   │   ├── RemoveFromCart/
│   │   │   └── UpdateCartItem/
│   │   └── Queries/
│   │       └── GetCart/
│   └── Services/
│       ├── IPaymentService.cs
│       ├── IEmailService.cs
│       └── IStorageService.cs
│
├── EShop.Infrastructure/      # 인프라 계층 (외부 의존성)
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations/
│   │   ├── Migrations/
│   │   └── Repositories/
│   ├── Services/
│   │   ├── StripePaymentService.cs
│   │   ├── SendGridEmailService.cs
│   │   ├── BlobStorageService.cs
│   │   └── DateTimeService.cs
│   ├── BackgroundJobs/
│   │   ├── EmailJob.cs
│   │   └── DailyReportJob.cs
│   └── Caching/
│       └── RedisCacheService.cs
│
└── EShop.API/                 # 프레젠테이션 계층 (API)
    ├── Controllers/
    │   ├── ProductsController.cs
    │   ├── OrdersController.cs
    │   ├── CartsController.cs
    │   └── AuthController.cs
    ├── Hubs/
    │   ├── NotificationHub.cs
    │   └── AdminDashboardHub.cs
    ├── Middleware/
    │   ├── ExceptionHandlingMiddleware.cs
    │   └── RequestLoggingMiddleware.cs
    ├── Filters/
    │   └── ApiExceptionFilterAttribute.cs
    └── Program.cs
```

**의존성 방향**: API → Application → Domain ← Infrastructure

Domain 계층은 다른 계층을 전혀 참조하지 않습니다. Application은 Domain만 참조합니다. Infrastructure와 API는 모든 계층을 참조할 수 있지만, Domain을 직접 수정하지 않습니다.

## Phase 1: 프로젝트 초기 설정

### 솔루션 생성

```bash
# 솔루션 생성
dotnet new sln -n EShop

# 프로젝트 생성
dotnet new classlib -n EShop.Domain
dotnet new classlib -n EShop.Application
dotnet new classlib -n EShop.Infrastructure
dotnet new webapi -n EShop.API

# 솔루션에 프로젝트 추가
dotnet sln add EShop.Domain/EShop.Domain.csproj
dotnet sln add EShop.Application/EShop.Application.csproj
dotnet sln add EShop.Infrastructure/EShop.Infrastructure.csproj
dotnet sln add EShop.API/EShop.API.csproj

# 프로젝트 참조 설정
cd EShop.Application
dotnet add reference ../EShop.Domain/EShop.Domain.csproj

cd ../EShop.Infrastructure
dotnet add reference ../EShop.Application/EShop.Application.csproj

cd ../EShop.API
dotnet add reference ../EShop.Application/EShop.Application.csproj
dotnet add reference ../EShop.Infrastructure/EShop.Infrastructure.csproj
```

### 필수 패키지 설치

```bash
# EShop.Application
cd EShop.Application
dotnet add package MediatR
dotnet add package AutoMapper
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions

# EShop.Infrastructure
cd ../EShop.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Hangfire
dotnet add package Hangfire.PostgreSql
dotnet add package StackExchangeRedis
dotnet add package Stripe.net

# EShop.API
cd ../EShop.API
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Authentication.Google
dotnet add package Swashbuckle.AspNetCore
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
```

## Phase 2: 도메인 모델 정의

### Product 엔티티

```csharp
// EShop.Domain/Entities/Product.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // 네비게이션 속성
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    // 계산된 속성
    public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;

    // 도메인 메서드
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (Stock < quantity)
            throw new InsufficientStockException($"Insufficient stock for product {Name}. Available: {Stock}, Requested: {quantity}");

        Stock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        Stock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### Order와 OrderItem 엔티티

```csharp
// EShop.Domain/Entities/Order.cs
public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public Customer Customer { get; set; } = null!;

    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public string? StripePaymentIntentId { get; set; }
    public string ShippingAddressLine1 { get; set; } = string.Empty;
    public string ShippingAddressLine2 { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingPostalCode { get; set; } = string.Empty;
    public string ShippingCountry { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    // 도메인 메서드
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm order in {Status} status");

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException($"Cannot ship order in {Status} status");

        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException($"Cannot deliver order in {Status} status");

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel delivered order");

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }

    public static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
    }
}

// EShop.Domain/Entities/OrderItem.cs
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string ProductName { get; set; } = string.Empty; // 주문 시점의 이름 저장
    public decimal UnitPrice { get; set; } // 주문 시점의 가격 저장
    public int Quantity { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity;
}
```

### Value Objects

```csharp
// EShop.Domain/ValueObjects/Money.cs
public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add different currencies: {Currency} and {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract different currencies: {Currency} and {other.Currency}");

        return new Money(Amount - other.Amount, Currency);
    }

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
}

// EShop.Domain/ValueObjects/Address.cs
public record Address
{
    public string Line1 { get; init; } = string.Empty;
    public string Line2 { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;

    public Address(string line1, string city, string postalCode, string country, string line2 = "")
    {
        if (string.IsNullOrWhiteSpace(line1))
            throw new ArgumentException("Address line 1 is required", nameof(line1));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required", nameof(city));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code is required", nameof(postalCode));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country is required", nameof(country));

        Line1 = line1;
        Line2 = line2;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    public override string ToString()
    {
        var line2Part = string.IsNullOrWhiteSpace(Line2) ? "" : $"{Line2}, ";
        return $"{Line1}, {line2Part}{City}, {PostalCode}, {Country}";
    }
}
```

## Phase 3: 데이터베이스 설정

### DbContext 구성

```csharp
// EShop.Infrastructure/Persistence/ApplicationDbContext.cs
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Product 구성
        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => new { e.IsActive, e.CategoryId });

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Order 구성
        builder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OrderItem 구성
        builder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Review 구성
        builder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => new { e.ProductId, e.Rating });
        });

        // 시드 데이터
        SeedData(builder);
    }

    private static void SeedData(ModelBuilder builder)
    {
        // 카테고리 시드
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
            new Category { Id = 2, Name = "Books", Description = "Physical and digital books" },
            new Category { Id = 3, Name = "Clothing", Description = "Apparel and fashion" },
            new Category { Id = 4, Name = "Home & Garden", Description = "Home improvement and gardening" }
        );

        // 상품 시드
        builder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Wireless Headphones",
                Description = "Premium noise-cancelling wireless headphones",
                Price = 299.99m,
                Stock = 50,
                CategoryId = 1,
                ImageUrl = "/images/products/headphones.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "Programming in C#",
                Description = "Comprehensive guide to C# programming",
                Price = 49.99m,
                Stock = 100,
                CategoryId = 2,
                ImageUrl = "/images/products/csharp-book.jpg",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
```

### 마이그레이션 생성

```bash
# Infrastructure 프로젝트에서
dotnet ef migrations add InitialCreate --startup-project ../EShop.API --output-dir Persistence/Migrations

# 데이터베이스 업데이트
dotnet ef database update --startup-project ../EShop.API
```

## Phase 4: CQRS with MediatR

### CreateProduct Command

```csharp
// EShop.Application/Products/Commands/CreateProduct/CreateProductCommand.cs
public record CreateProductCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public int CategoryId { get; init; }
    public IFormFile? Image { get; init; }
}

// CreateProductCommandValidator.cs
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(v => v.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(v => v.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");

        RuleFor(v => v.CategoryId)
            .GreaterThan(0).WithMessage("Category is required");
    }
}

// CreateProductCommandHandler.cs
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly ApplicationDbContext _context;
    private readonly IStorageService _storageService;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        ApplicationDbContext context,
        IStorageService storageService,
        ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product: {ProductName}", request.Name);

        // 이미지 업로드
        string? imageUrl = null;
        if (request.Image != null)
        {
            imageUrl = await _storageService.UploadFileAsync(
                request.Image,
                "products",
                cancellationToken
            );
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            ImageUrl = imageUrl ?? "/images/products/default.jpg",
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} created successfully", product.Id);

        return product.Id;
    }
}
```

### GetProducts Query

```csharp
// EShop.Application/Products/Queries/GetProducts/GetProductsQuery.cs
public record GetProductsQuery : IRequest<PaginatedList<ProductDto>>
{
    public int? CategoryId { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

// ProductDto.cs
public record ProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public double AverageRating { get; init; }
    public int ReviewCount { get; init; }
}

// GetProductsQueryHandler.cs
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public GetProductsQueryHandler(
        ApplicationDbContext context,
        IMapper mapper,
        IMemoryCache cache)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<PaginatedList<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"products_{request.CategoryId}_{request.MinPrice}_{request.MaxPrice}_{request.SearchTerm}_{request.SortBy}_{request.PageNumber}_{request.PageSize}";

        if (_cache.TryGetValue(cacheKey, out PaginatedList<ProductDto>? cachedResult))
        {
            return cachedResult!;
        }

        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Reviews)
            .Where(p => p.IsActive);

        // 필터링
        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(p =>
                p.Name.Contains(request.SearchTerm) ||
                p.Description.Contains(request.SearchTerm));

        // 정렬
        query = request.SortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name" => query.OrderBy(p => p.Name),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderBy(p => p.Id)
        };

        // 프로젝션 (메모리 절약)
        var productDtos = query.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl,
            CategoryName = p.Category.Name,
            AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
            ReviewCount = p.Reviews.Count
        });

        var result = await PaginatedList<ProductDto>.CreateAsync(
            productDtos,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        // 캐시 저장 (5분)
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
}

// PaginatedList.cs
public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
```

## Phase 5: 주문 생성 워크플로우

### CreateOrder Command (복잡한 비즈니스 로직)

```csharp
// EShop.Application/Orders/Commands/CreateOrder/CreateOrderCommand.cs
public record CreateOrderCommand : IRequest<int>
{
    public string CustomerId { get; init; } = string.Empty;
    public List<OrderItemDto> Items { get; init; } = new();
    public AddressDto ShippingAddress { get; init; } = null!;
    public string PaymentMethodId { get; init; } = string.Empty; // Stripe Payment Method ID
}

public record OrderItemDto
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}

public record AddressDto
{
    public string Line1 { get; init; } = string.Empty;
    public string Line2 { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}

// CreateOrderCommandHandler.cs
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly ApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;
    private readonly IMediator _mediator;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        ApplicationDbContext context,
        IPaymentService paymentService,
        IEmailService emailService,
        IMediator mediator,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _context = context;
        _paymentService = paymentService;
        _emailService = emailService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);

            // 1. 상품 조회 및 재고 확인
            var productIds = request.Items.Select(i => i.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            if (products.Count != productIds.Count)
                throw new NotFoundException("One or more products not found");

            // 2. 재고 확인 및 차감 (낙관적 동시성 제어)
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var itemDto in request.Items)
            {
                var product = products.First(p => p.Id == itemDto.ProductId);

                try
                {
                    product.DecreaseStock(itemDto.Quantity);
                }
                catch (InsufficientStockException ex)
                {
                    _logger.LogWarning(ex, "Insufficient stock for product {ProductId}", itemDto.ProductId);
                    throw;
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = itemDto.Quantity
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.TotalPrice;
            }

            // 3. 결제 처리
            _logger.LogInformation("Processing payment for amount {Amount}", totalAmount);

            var paymentResult = await _paymentService.CreatePaymentIntentAsync(
                totalAmount,
                request.PaymentMethodId,
                request.CustomerId,
                cancellationToken
            );

            if (!paymentResult.Success)
            {
                _logger.LogError("Payment failed: {Error}", paymentResult.ErrorMessage);
                throw new PaymentFailedException(paymentResult.ErrorMessage);
            }

            // 4. 주문 생성
            var order = new Order
            {
                OrderNumber = Order.GenerateOrderNumber(),
                CustomerId = request.CustomerId,
                TotalAmount = totalAmount,
                Status = OrderStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                StripePaymentIntentId = paymentResult.PaymentIntentId,
                ShippingAddressLine1 = request.ShippingAddress.Line1,
                ShippingAddressLine2 = request.ShippingAddress.Line2,
                ShippingCity = request.ShippingAddress.City,
                ShippingPostalCode = request.ShippingAddress.PostalCode,
                ShippingCountry = request.ShippingAddress.Country,
                Items = orderItems,
                CreatedAt = DateTime.UtcNow
            };

            order.Confirm();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} created successfully", order.Id);

            // 5. 이메일 발송 (백그라운드)
            BackgroundJob.Enqueue(() =>
                _emailService.SendOrderConfirmationAsync(order.Id));

            // 6. 도메인 이벤트 발행 (SignalR 알림 등)
            await _mediator.Publish(new OrderCreatedEvent { OrderId = order.Id }, cancellationToken);

            return order.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to create order for customer {CustomerId}", request.CustomerId);
            throw;
        }
    }
}
```

## Phase 6: Stripe 결제 통합

### Payment Service 구현

```csharp
// EShop.Application/Services/IPaymentService.cs
public interface IPaymentService
{
    Task<PaymentResult> CreatePaymentIntentAsync(
        decimal amount,
        string paymentMethodId,
        string customerId,
        CancellationToken cancellationToken = default);

    Task<PaymentResult> ConfirmPaymentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default);

    Task<bool> RefundPaymentAsync(
        string paymentIntentId,
        decimal? amount = null,
        CancellationToken cancellationToken = default);
}

public record PaymentResult
{
    public bool Success { get; init; }
    public string? PaymentIntentId { get; init; }
    public string? ErrorMessage { get; init; }
}

// EShop.Infrastructure/Services/StripePaymentService.cs
public class StripePaymentService : IPaymentService
{
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(ILogger<StripePaymentService> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentResult> CreatePaymentIntentAsync(
        decimal amount,
        string paymentMethodId,
        string customerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe는 센트 단위
                Currency = "usd",
                PaymentMethod = paymentMethodId,
                Customer = customerId,
                Confirm = true,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never"
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation("Payment intent {PaymentIntentId} created successfully", paymentIntent.Id);

            return new PaymentResult
            {
                Success = paymentIntent.Status == "succeeded",
                PaymentIntentId = paymentIntent.Id
            };
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe payment failed");

            return new PaymentResult
            {
                Success = false,
                ErrorMessage = ex.StripeError.Message
            };
        }
    }

    public async Task<PaymentResult> ConfirmPaymentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.ConfirmAsync(paymentIntentId, cancellationToken: cancellationToken);

            return new PaymentResult
            {
                Success = paymentIntent.Status == "succeeded",
                PaymentIntentId = paymentIntent.Id
            };
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to confirm payment {PaymentIntentId}", paymentIntentId);

            return new PaymentResult
            {
                Success = false,
                ErrorMessage = ex.StripeError.Message
            };
        }
    }

    public async Task<bool> RefundPaymentAsync(
        string paymentIntentId,
        decimal? amount = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            if (amount.HasValue)
                options.Amount = (long)(amount.Value * 100);

            var service = new RefundService();
            var refund = await service.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation("Refund {RefundId} created for payment {PaymentIntentId}", refund.Id, paymentIntentId);

            return refund.Status == "succeeded";
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to refund payment {PaymentIntentId}", paymentIntentId);
            return false;
        }
    }
}
```

### Webhook 처리 (결제 상태 동기화)

```csharp
// EShop.API/Controllers/WebhooksController.cs
[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<WebhooksController> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"].ToString();
        var webhookSecret = _configuration["Stripe:WebhookSecret"];

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                webhookSecret
            );

            _logger.LogInformation("Processing Stripe webhook: {EventType}", stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    await HandlePaymentSucceededAsync(paymentIntent!);
                    break;

                case Events.PaymentIntentPaymentFailed:
                    var failedPayment = stripeEvent.Data.Object as PaymentIntent;
                    await HandlePaymentFailedAsync(failedPayment!);
                    break;

                case Events.ChargeRefunded:
                    var refund = stripeEvent.Data.Object as Charge;
                    await HandleRefundAsync(refund!);
                    break;
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            return BadRequest();
        }
    }

    private async Task HandlePaymentSucceededAsync(PaymentIntent paymentIntent)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.StripePaymentIntentId == paymentIntent.Id);

        if (order != null)
        {
            order.PaymentStatus = PaymentStatus.Paid;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} payment confirmed", order.Id);
        }
    }

    private async Task HandlePaymentFailedAsync(PaymentIntent paymentIntent)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.StripePaymentIntentId == paymentIntent.Id);

        if (order != null)
        {
            order.PaymentStatus = PaymentStatus.Failed;
            await _context.SaveChangesAsync();

            _logger.LogWarning("Order {OrderId} payment failed", order.Id);
        }
    }

    private async Task HandleRefundAsync(Charge charge)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.StripePaymentIntentId == charge.PaymentIntentId);

        if (order != null)
        {
            order.PaymentStatus = PaymentStatus.Refunded;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} refunded", order.Id);
        }
    }
}
```

## Phase 7: SignalR 실시간 알림

### Notification Hub

```csharp
// EShop.API/Hubs/NotificationHub.cs
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            // 사용자별 그룹에 추가
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("User {UserId} connected to notification hub", userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("User {UserId} disconnected from notification hub", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // 상품 재고 업데이트 구독
    public async Task SubscribeToProduct(int productId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"product_{productId}");
        _logger.LogInformation("Connection {ConnectionId} subscribed to product {ProductId}",
            Context.ConnectionId, productId);
    }

    public async Task UnsubscribeFromProduct(int productId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"product_{productId}");
    }
}

// Program.cs에 SignalR 추가
builder.Services.AddSignalR();
app.MapHub<NotificationHub>("/hubs/notifications");
```

### 주문 상태 변경 시 실시간 알림

```csharp
// EShop.Application/Orders/Commands/UpdateOrderStatus/UpdateOrderStatusCommandHandler.cs
public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Unit>
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(
        ApplicationDbContext context,
        IHubContext<NotificationHub> hubContext,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _context = context;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellationToken);

        if (order == null)
            throw new NotFoundException(nameof(Order), request.OrderId);

        var previousStatus = order.Status;

        // 상태 전환
        switch (request.NewStatus)
        {
            case OrderStatus.Shipped:
                order.Ship();
                break;
            case OrderStatus.Delivered:
                order.Deliver();
                break;
            case OrderStatus.Cancelled:
                order.Cancel(request.Reason ?? "No reason provided");
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} status changed from {PreviousStatus} to {NewStatus}",
            order.Id, previousStatus, order.Status);

        // SignalR 실시간 알림
        await _hubContext.Clients
            .Group($"user_{order.CustomerId}")
            .SendAsync("OrderStatusChanged", new
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                Message = GetStatusMessage(order.Status)
            }, cancellationToken);

        return Unit.Value;
    }

    private static string GetStatusMessage(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Shipped => "Your order has been shipped!",
            OrderStatus.Delivered => "Your order has been delivered!",
            OrderStatus.Cancelled => "Your order has been cancelled.",
            _ => "Order status updated."
        };
    }
}
```

### Admin Dashboard 실시간 메트릭

```csharp
// EShop.API/Hubs/AdminDashboardHub.cs
public class AdminDashboardHub : Hub
{
    private readonly ApplicationDbContext _context;

    public AdminDashboardHub(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        await base.OnConnectedAsync();
    }

    [Authorize(Roles = "Admin")]
    public async Task<DashboardStats> GetDashboardStats()
    {
        var today = DateTime.UtcNow.Date;

        var stats = new DashboardStats
        {
            TodayOrders = await _context.Orders.CountAsync(o => o.CreatedAt >= today),
            TodayRevenue = await _context.Orders
                .Where(o => o.CreatedAt >= today && o.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(o => o.TotalAmount),
            ActiveUsers = await _context.Users.CountAsync(u => u.LastActiveAt >= DateTime.UtcNow.AddMinutes(-15)),
            LowStockProducts = await _context.Products.CountAsync(p => p.Stock < 10)
        };

        return stats;
    }
}

// 백그라운드 작업으로 주기적으로 메트릭 푸시
public class DashboardMetricsJob
{
    private readonly IHubContext<AdminDashboardHub> _hubContext;
    private readonly ApplicationDbContext _context;

    [AutomaticRetry(Attempts = 0)]
    public async Task PushDashboardMetrics()
    {
        var stats = await CalculateStatsAsync();

        await _hubContext.Clients.Group("Admins")
            .SendAsync("DashboardStatsUpdated", stats);
    }

    private async Task<DashboardStats> CalculateStatsAsync()
    {
        // 메트릭 계산...
    }
}

// Hangfire로 1분마다 실행
RecurringJob.AddOrUpdate<DashboardMetricsJob>(
    "dashboard-metrics",
    job => job.PushDashboardMetrics(),
    Cron.Minutely
);
```

## Phase 8: 백그라운드 작업 (Hangfire)

### Email Service와 작업 큐

```csharp
// EShop.Infrastructure/Services/SendGridEmailService.cs
public class SendGridEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(int orderId)
    {
        // 주문 정보 조회
        // SendGrid API 호출
        // 이메일 발송

        _logger.LogInformation("Order confirmation email sent for order {OrderId}", orderId);
    }

    public async Task SendShippingNotificationAsync(int orderId)
    {
        // 배송 알림 이메일 발송
    }

    public async Task SendWelcomeEmailAsync(string userId)
    {
        // 환영 이메일 발송
    }
}

// Hangfire 작업
public class EmailBackgroundJob
{
    private readonly IEmailService _emailService;
    private readonly ApplicationDbContext _context;

    public EmailBackgroundJob(IEmailService emailService, ApplicationDbContext context)
    {
        _emailService = emailService;
        _context = context;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task SendOrderConfirmation(int orderId)
    {
        await _emailService.SendOrderConfirmationAsync(orderId);
    }

    [AutomaticRetry(Attempts = 2)]
    public async Task SendDailyReport()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);

        var report = new
        {
            TotalOrders = await _context.Orders.CountAsync(o => o.CreatedAt >= yesterday && o.CreatedAt < yesterday.AddDays(1)),
            TotalRevenue = await _context.Orders
                .Where(o => o.CreatedAt >= yesterday && o.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(o => o.TotalAmount),
            NewCustomers = await _context.Users.CountAsync(u => u.CreatedAt >= yesterday)
        };

        // 관리자에게 리포트 이메일 발송
    }
}

// Program.cs에서 Hangfire 설정
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// 반복 작업 등록
RecurringJob.AddOrUpdate<EmailBackgroundJob>(
    "daily-report",
    job => job.SendDailyReport(),
    Cron.Daily(9) // 매일 오전 9시
);
```

## Phase 9: Redis 캐싱

### Distributed Cache 설정

```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "EShop:";
});

// 캐싱 서비스
public class CachedProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachedProductService> _logger;

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var cacheKey = $"product_{id}";

        // 캐시 확인
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            _logger.LogInformation("Cache hit for product {ProductId}", id);
            return JsonSerializer.Deserialize<ProductDto>(cachedData);
        }

        // DB에서 조회
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return null;

        var dto = MapToDto(product);

        // 캐시 저장 (10분)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(dto),
            cacheOptions
        );

        _logger.LogInformation("Cached product {ProductId}", id);

        return dto;
    }

    public async Task InvalidateProductCacheAsync(int id)
    {
        await _cache.RemoveAsync($"product_{id}");
        _logger.LogInformation("Invalidated cache for product {ProductId}", id);
    }
}
```

## Phase 10: 포괄적인 테스팅

### 단위 테스트 (도메인 로직)

```csharp
// EShop.Tests/Domain/OrderTests.cs
public class OrderTests
{
    [Fact]
    public void Confirm_PendingOrder_SetsConfirmedStatus()
    {
        // Arrange
        var order = new Order
        {
            Status = OrderStatus.Pending
        };

        // Act
        order.Confirm();

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.ConfirmedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Confirm_AlreadyConfirmedOrder_ThrowsException()
    {
        // Arrange
        var order = new Order
        {
            Status = OrderStatus.Confirmed
        };

        // Act
        Action act = () => order.Confirm();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Confirmed*");
    }

    [Fact]
    public void GenerateOrderNumber_ReturnsUniqueNumber()
    {
        // Act
        var orderNumber1 = Order.GenerateOrderNumber();
        var orderNumber2 = Order.GenerateOrderNumber();

        // Assert
        orderNumber1.Should().NotBe(orderNumber2);
        orderNumber1.Should().MatchRegex(@"^ORD-\d{8}-[A-F0-9]{8}$");
    }
}

// EShop.Tests/Domain/ProductTests.cs
public class ProductTests
{
    [Theory]
    [InlineData(10, 5, 5)]
    [InlineData(100, 50, 50)]
    [InlineData(1, 1, 0)]
    public void DecreaseStock_ValidQuantity_DecreasesStock(int initialStock, int quantity, int expectedStock)
    {
        // Arrange
        var product = new Product { Stock = initialStock };

        // Act
        product.DecreaseStock(quantity);

        // Assert
        product.Stock.Should().Be(expectedStock);
    }

    [Fact]
    public void DecreaseStock_InsufficientStock_ThrowsException()
    {
        // Arrange
        var product = new Product { Name = "Test Product", Stock = 5 };

        // Act
        Action act = () => product.DecreaseStock(10);

        // Assert
        act.Should().Throw<InsufficientStockException>()
            .WithMessage("*Test Product*")
            .WithMessage("*Available: 5*")
            .WithMessage("*Requested: 10*");
    }
}
```

### 통합 테스트 (API 엔드포인트)

```csharp
// EShop.IntegrationTests/ProductsControllerTests.cs
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessAndProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PaginatedList<ProductDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateProduct_Authorized_ReturnsCreated()
    {
        // Arrange
        var token = await _factory.GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var newProduct = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Stock = 10,
            CategoryId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var productId = await response.Content.ReadFromJsonAsync<int>();
        productId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateProduct_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var newProduct = new CreateProductCommand
        {
            Name = "Test Product",
            Price = 99.99m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
```

### E2E 테스트 (주문 플로우)

```csharp
// EShop.E2ETests/OrderFlowTests.cs
public class OrderFlowTests : PageTest
{
    [Test]
    public async Task User_CanPlaceOrder_EndToEnd()
    {
        // 1. 홈페이지 방문
        await Page.GotoAsync("https://localhost:5001");

        // 2. 로그인
        await Page.ClickAsync("text=Login");
        await Page.FillAsync("#email", "test@example.com");
        await Page.FillAsync("#password", "Test123!");
        await Page.ClickAsync("#login-button");

        await Expect(Page.Locator("text=Welcome")).ToBeVisibleAsync();

        // 3. 상품 검색
        await Page.FillAsync("#search", "headphones");
        await Page.PressAsync("#search", "Enter");

        // 4. 상품 상세 페이지
        await Page.ClickAsync("text=Wireless Headphones");

        // 5. 장바구니에 추가
        await Page.ClickAsync("text=Add to Cart");
        await Expect(Page.Locator(".toast-success")).ToContainTextAsync("Added to cart");

        // 6. 장바구니 확인
        await Page.ClickAsync("text=Cart");
        await Expect(Page.Locator(".cart-item")).ToHaveCountAsync(1);
        await Expect(Page.Locator(".cart-total")).ToContainTextAsync("$299.99");

        // 7. 체크아웃
        await Page.ClickAsync("text=Checkout");

        // 8. 배송 정보 입력
        await Page.FillAsync("#address-line1", "123 Main St");
        await Page.FillAsync("#city", "Seattle");
        await Page.FillAsync("#postal-code", "98101");
        await Page.FillAsync("#country", "USA");

        // 9. 결제 정보 입력 (Stripe Test Card)
        await Page.FillAsync("#card-number", "4242424242424242");
        await Page.FillAsync("#card-expiry", "12/25");
        await Page.FillAsync("#card-cvc", "123");

        // 10. 주문 완료
        await Page.ClickAsync("text=Place Order");

        // 11. 주문 확인 페이지
        await Expect(Page.Locator("h1")).ToContainTextAsync("Order Confirmed");
        await Expect(Page.Locator(".order-number")).ToBeVisibleAsync();

        // 스크린샷 캡처
        await Page.ScreenshotAsync(new() { Path = "order-confirmed.png" });
    }
}
```

## Phase 11: 배포

### Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 프로젝트 파일 복사 및 복원
COPY ["EShop.API/EShop.API.csproj", "EShop.API/"]
COPY ["EShop.Application/EShop.Application.csproj", "EShop.Application/"]
COPY ["EShop.Domain/EShop.Domain.csproj", "EShop.Domain/"]
COPY ["EShop.Infrastructure/EShop.Infrastructure.csproj", "EShop.Infrastructure/"]

RUN dotnet restore "EShop.API/EShop.API.csproj"

# 소스 코드 복사 및 빌드
COPY . .
WORKDIR "/src/EShop.API"
RUN dotnet build "EShop.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "EShop.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# 비root 사용자 생성
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "EShop.API.dll"]
```

### Docker Compose (개발 환경)

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=eshop;Username=postgres;Password=postgres
      - ConnectionStrings__Redis=redis:6379
      - Stripe__ApiKey=${STRIPE_API_KEY}
    depends_on:
      - postgres
      - redis

  postgres:
    image: postgres:16
    environment:
      - POSTGRES_DB=eshop
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data

volumes:
  postgres-data:
  redis-data:
```

### GitHub Actions CI/CD

```yaml
# .github/workflows/deploy.yml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore --configuration Release

    - name: Test
      run: dotnet test --no-build --verbosity normal --configuration Release

    - name: Login to Docker Hub
      uses: docker/login-action@v3
      with:
        username: ${{ secrets.DOCKER_USERNAME }}
        password: ${{ secrets.DOCKER_PASSWORD }}

    - name: Build and push Docker image
      uses: docker/build-push-action@v5
      with:
        context: .
        push: true
        tags: myusername/eshop:latest,myusername/eshop:${{ github.sha }}

    - name: Deploy to Azure App Service
      uses: azure/webapps-deploy@v3
      with:
        app-name: 'eshop-api'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        images: 'myusername/eshop:${{ github.sha }}'
```

## 프로젝트 완성

축하합니다! 이제 여러분은 완전한 전자상거래 플랫폼을 구축했습니다:

**구현한 기능:**
- ✅ Clean Architecture + CQRS 구조
- ✅ 인증과 권한 (JWT + OAuth)
- ✅ 복잡한 비즈니스 로직 (주문 처리, 재고 관리)
- ✅ Stripe 결제 통합
- ✅ SignalR 실시간 알림
- ✅ Hangfire 백그라운드 작업
- ✅ Redis 캐싱
- ✅ 포괄적인 테스팅 (단위, 통합, E2E)
- ✅ Docker 컨테이너화
- ✅ CI/CD 파이프라인

이 프로젝트는 **포트폴리오 프로젝트**로 활용할 수 있으며, 실제 프로덕션 환경에 배포하여 실사용자를 서비스할 수 있는 수준입니다.

**다음 단계:**
1. 프론트엔드 추가 (React, Vue, Blazor)
2. 관리자 대시보드 개선
3. 추천 시스템 (ML.NET)
4. 마이크로서비스로 분리 (선택적)
5. Kubernetes 배포 (확장성)

여러분은 이제 ASP.NET Core로 실전 수준의 애플리케이션을 만들 수 있는 개발자입니다!

