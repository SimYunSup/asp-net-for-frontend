# Chapter 20: 고급 아키텍처 패턴 - 복잡성을 제어하는 설계

## 아키텍처의 필요성: 왜 패턴이 중요한가?

처음 만드는 애플리케이션은 단순합니다. 몇 개의 컨트롤러, 몇 개의 서비스, Entity Framework로 데이터베이스 접근—모든 것이 명확합니다. 하지만 시간이 지나면서 요구사항이 늘어나고, 팀이 커지며, 코드베이스가 수만 줄로 증가합니다. 이때 초기의 단순한 구조는 한계를 드러냅니다.

"이 로직은 어디에 있어야 하지?", "이 클래스가 너무 많은 일을 하는 것 같은데?", "테스트를 어떻게 작성하지?", "변경이 전체 시스템에 영향을 미쳐"—이런 질문들이 더 자주 등장합니다. 이것이 아키텍처 패턴이 필요한 이유입니다. 패턴은 검증된 해결책으로, 복잡성을 관리하고 변경에 대응하며 팀 간 소통을 원활하게 합니다.

이 챕터는 Clean Architecture, CQRS, DDD, Modular Monolith 같은 고급 패턴을 다룹니다. 프론트엔드에서 경험한 컴포넌트 기반 아키텍처, 상태 관리 패턴, 단방향 데이터 흐름—이런 개념들은 서버 아키텍처와 놀라울 정도로 유사합니다.

## Part 1: Clean Architecture - 계층의 분리

### Clean Architecture란 무엇인가?

**Clean Architecture**(Robert C. Martin, "Uncle Bob")는 시스템을 여러 계층으로 분리하고, 의존성이 항상 **내부를 향하도록** 강제합니다. 핵심 아이디어는 **비즈니스 로직을 프레임워크, 데이터베이스, UI로부터 독립시키는 것**입니다.

React의 관심사 분리(컴포넌트, 훅, 상태)와 유사하게, Clean Architecture는 각 계층이 명확한 책임을 가지도록 합니다.

### 계층 구조: 동심원의 법칙

Clean Architecture는 4개의 계층으로 구성됩니다:

```
┌─────────────────────────────────────┐
│   Presentation (Web API, UI)        │  ← 가장 외부
│   ┌─────────────────────────────┐   │
│   │   Infrastructure             │   │  ← 데이터베이스, 외부 서비스
│   │   ┌─────────────────────┐   │   │
│   │   │   Application       │   │   │  ← 유즈 케이스
│   │   │   ┌─────────────┐   │   │   │
│   │   │   │   Domain    │   │   │   │  ← 비즈니스 로직 (가장 내부)
│   │   │   └─────────────┘   │   │   │
│   │   └─────────────────────┘   │   │
│   └─────────────────────────────┘   │
└─────────────────────────────────────┘

의존성 방향: 외부 → 내부 (항상!)
```

**의존성 규칙**: 외부 계층은 내부 계층을 알 수 있지만, 내부 계층은 외부를 몰라야 합니다. Domain Layer는 Entity Framework, ASP.NET Core, 어떤 UI 프레임워크도 알지 못합니다.

### 1. Domain Layer: 비즈니스의 핵심

**Domain Layer**는 비즈니스 규칙과 엔티티를 포함합니다. 프레임워크, 데이터베이스, UI로부터 완전히 독립적입니다.

```csharp
// Domain/Entities/Order.cs
public class Order
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // 팩토리 메서드
    public static Order Create(int customerId)
    {
        return new Order
        {
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    // 비즈니스 규칙: 최소 주문 금액
    public void AddItem(Product product, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive");

        var item = new OrderItem(product.Id, product.Price, quantity);
        _items.Add(item);

        RecalculateTotal();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be confirmed");

        if (TotalAmount < 10)
            throw new DomainException("Minimum order amount is $10");

        Status = OrderStatus.Confirmed;
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.Subtotal);
    }
}

public class OrderItem
{
    public int ProductId { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }
    public decimal Subtotal => UnitPrice * Quantity;

    public OrderItem(int productId, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
```

**핵심 특징:**
- **프레임워크 독립적**: `[Key]`, `[Required]` 같은 어트리뷰트 없음
- **비즈니스 규칙 캡슐화**: "최소 주문 금액 $10"은 Domain에 존재
- **불변성**: setter는 private, 상태 변경은 메서드로만

### 2. Application Layer: 유즈 케이스

**Application Layer**는 애플리케이션의 유즈 케이스를 구현합니다. Domain을 오케스트레이션하지만, 비즈니스 규칙은 포함하지 않습니다.

```csharp
// Application/Orders/Commands/CreateOrder/CreateOrderCommand.cs
public record CreateOrderCommand(int CustomerId, List<CreateOrderItemDto> Items) : IRequest<int>;

public record CreateOrderItemDto(int ProductId, int Quantity);

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Domain 엔티티 생성
        var order = Order.Create(request.CustomerId);

        // 2. 제품 조회 및 주문 항목 추가
        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new NotFoundException($"Product {item.ProductId} not found");

            order.AddItem(product, item.Quantity);
        }

        // 3. 주문 확인 (비즈니스 규칙 검증)
        order.Confirm();

        // 4. 저장
        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
```

**인터페이스 정의** (Application Layer에 위치):

```csharp
// Application/Common/Interfaces/IOrderRepository.cs
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task AddAsync(Order order);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 3. Infrastructure Layer: 외부 세계와의 연결

**Infrastructure Layer**는 데이터베이스, 외부 API, 파일 시스템 등 외부 리소스를 다룹니다. Application Layer의 인터페이스를 **구현**합니다.

```csharp
// Infrastructure/Persistence/Repositories/OrderRepository.cs
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }
}

// Infrastructure/Persistence/AppDbContext.cs
public class AppDbContext : DbContext, IUnitOfWork
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EF Core 설정은 여기에
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            entity.OwnsMany(o => o.Items, items =>
            {
                items.WithOwner().HasForeignKey("OrderId");
                items.Property<int>("Id");
                items.HasKey("Id");
            });
        });
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

### 4. Presentation Layer: API 엔드포인트

**Presentation Layer**는 외부 세계와의 인터페이스입니다. HTTP 요청을 받아 Application Layer의 커맨드/쿼리로 변환합니다.

```csharp
// WebApi/Controllers/OrdersController.cs
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateOrder(CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(
            request.CustomerId,
            request.Items.Select(i => new CreateOrderItemDto(i.ProductId, i.Quantity)).ToList());

        var orderId = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetOrder), new { id = orderId }, orderId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var query = new GetOrderQuery(id);
        var order = await _mediator.Send(query);

        return order == null ? NotFound() : Ok(order);
    }
}

// 요청/응답 DTO
public record CreateOrderRequest(int CustomerId, List<CreateOrderItemRequest> Items);
public record CreateOrderItemRequest(int ProductId, int Quantity);
```

### 프로젝트 구조

```
Solution/
├── Domain/
│   ├── Entities/
│   │   ├── Order.cs
│   │   ├── Product.cs
│   │   └── Customer.cs
│   ├── Exceptions/
│   │   └── DomainException.cs
│   └── Domain.csproj
├── Application/
│   ├── Orders/
│   │   ├── Commands/
│   │   │   └── CreateOrder/
│   │   │       ├── CreateOrderCommand.cs
│   │   │       └── CreateOrderCommandHandler.cs
│   │   └── Queries/
│   │       └── GetOrder/
│   │           ├── GetOrderQuery.cs
│   │           └── GetOrderQueryHandler.cs
│   ├── Common/
│   │   └── Interfaces/
│   │       ├── IOrderRepository.cs
│   │       └── IUnitOfWork.cs
│   └── Application.csproj (참조: Domain)
├── Infrastructure/
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   └── Repositories/
│   │       └── OrderRepository.cs
│   ├── Services/
│   │   └── EmailService.cs
│   └── Infrastructure.csproj (참조: Application)
└── WebApi/
    ├── Controllers/
    │   └── OrdersController.cs
    ├── Program.cs
    └── WebApi.csproj (참조: Infrastructure)
```

### 의존성 역전 원칙 (DIP)

Application Layer는 `IOrderRepository` 인터페이스를 정의하지만, Infrastructure Layer가 구현합니다. 이것이 **의존성 역전**입니다. Application은 Infrastructure를 모르지만, Infrastructure는 Application의 인터페이스를 알고 구현합니다.

```csharp
// Program.cs에서 의존성 주입
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
```

## Part 2: CQRS - Command와 Query의 분리

### CQRS란 무엇인가?

**CQRS**(Command Query Responsibility Segregation)는 **데이터 쓰기(Command)**와 **데이터 읽기(Query)**를 별도의 모델로 분리하는 패턴입니다.

전통적인 CRUD에서는 같은 모델(Entity)로 읽기와 쓰기를 모두 처리합니다. 하지만 실제로는 읽기와 쓰기의 요구사항이 다릅니다:

- **읽기**: 빠른 응답, 복잡한 조인, DTO로 투영, 캐싱
- **쓰기**: 유효성 검사, 비즈니스 규칙, 트랜잭션, 이벤트 발생

CQRS는 이 둘을 명확히 분리하여 각각 최적화할 수 있게 합니다.

### MediatR을 통한 CQRS 구현

**MediatR**은 .NET의 메디에이터 패턴 라이브러리로, CQRS를 우아하게 구현할 수 있게 합니다.

```bash
dotnet add package MediatR
```

```csharp
// Program.cs
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

**Command: 데이터 수정**

```csharp
// Application/Orders/Commands/CreateOrder/CreateOrderCommand.cs
public record CreateOrderCommand(int CustomerId, List<CreateOrderItemDto> Items)
    : IRequest<Result<int>>; // Result<int>를 반환

public class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Result<int>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<int>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = Order.Create(request.CustomerId);

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    return Result<int>.Failure($"Product {item.ProductId} not found");

                order.AddItem(product, item.Quantity);
            }

            order.Confirm();

            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(order.Id);
        }
        catch (DomainException ex)
        {
            return Result<int>.Failure(ex.Message);
        }
    }
}
```

**Query: 데이터 조회**

```csharp
// Application/Orders/Queries/GetOrder/GetOrderQuery.cs
public record GetOrderQuery(int OrderId) : IRequest<OrderDto?>;

public record OrderDto(
    int Id,
    int CustomerId,
    string Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    List<OrderItemDto> Items);

public record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal Subtotal);

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto?>
{
    private readonly AppDbContext _context;

    public async Task<OrderDto?> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken)
    {
        // EF Core로 직접 쿼리 (Repository 없음)
        return await _context.Orders
            .Where(o => o.Id == request.OrderId)
            .Select(o => new OrderDto(
                o.Id,
                o.CustomerId,
                o.Status.ToString(),
                o.TotalAmount,
                o.CreatedAt,
                o.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.Product.Name,
                    i.Quantity,
                    i.Subtotal)).ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
```

**Controller에서 사용:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<int>> CreateOrder(CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(request.CustomerId, request.Items);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetOrder), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var query = new GetOrderQuery(id);
        var order = await _mediator.Send(query);

        return order == null ? NotFound() : Ok(order);
    }
}
```

### Pipeline Behaviors: 횡단 관심사

MediatR의 강력한 기능은 **Pipeline Behaviors**입니다. 모든 커맨드/쿼리를 가로채서 로깅, 유효성 검사, 트랜잭션을 자동으로 적용할 수 있습니다.

**로깅 Behavior:**

```csharp
public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Handling {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        _logger.LogInformation("{RequestName} handled in {ElapsedMs}ms",
            requestName, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
```

**유효성 검사 Behavior (FluentValidation):**

```bash
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
```

```csharp
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}

// Program.cs
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

이제 모든 커맨드는 자동으로 로깅되고 검증됩니다!

## Part 3: Domain-Driven Design 기초

### DDD의 핵심 개념

**Domain-Driven Design**(Eric Evans)은 복잡한 도메인 로직을 소프트웨어로 모델링하는 접근 방식입니다.

**1. Entity: 식별자를 가진 객체**

```csharp
public class Order // Entity
{
    public int Id { get; private set; } // 식별자
    // ...

    // 같은 Id면 같은 Order
    public override bool Equals(object? obj)
    {
        return obj is Order order && order.Id == Id;
    }
}
```

**2. Value Object: 불변 객체**

```csharp
public record Money // Value Object
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative");
        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");

        return new Money(Amount + other.Amount, Currency);
    }
}

// 사용
var price1 = new Money(10.50m, "USD");
var price2 = new Money(5.25m, "USD");
var total = price1.Add(price2); // new Money(15.75m, "USD")
```

Value Object는 불변이며, 값으로 비교됩니다. `record`가 완벽하게 맞습니다!

**3. Aggregate: 일관성 경계**

```csharp
public class Order // Aggregate Root
{
    private readonly List<OrderItem> _items = new();

    // 외부에서 OrderItem을 직접 수정할 수 없음
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public void AddItem(Product product, int quantity)
    {
        // 비즈니스 규칙 검증
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot add items to confirmed order");

        _items.Add(new OrderItem(product.Id, product.Price, quantity));
        RecalculateTotal();
    }

    // Aggregate 내부의 일관성 보장
    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.Subtotal);
    }
}
```

Aggregate는 **일관성이 보장되어야 하는 객체들의 클러스터**입니다. Aggregate Root(Order)를 통해서만 내부 객체(OrderItem)를 수정할 수 있습니다.

**4. Domain Events: 도메인 내 통신**

```csharp
// Domain/Events/OrderConfirmedEvent.cs
public record OrderConfirmedEvent(int OrderId, int CustomerId, decimal TotalAmount);

public class Order
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be confirmed");

        Status = OrderStatus.Confirmed;

        // Domain Event 발생
        _domainEvents.Add(new OrderConfirmedEvent(Id, CustomerId, TotalAmount));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

// Infrastructure/Persistence/AppDbContext.cs
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    // 변경 사항 저장
    var result = await base.SaveChangesAsync(cancellationToken);

    // Domain Events 발행
    var events = ChangeTracker.Entries<Order>()
        .SelectMany(e => e.Entity.DomainEvents)
        .ToList();

    foreach (var domainEvent in events)
    {
        await _mediator.Publish(domainEvent, cancellationToken);
    }

    // Events 클리어
    foreach (var entity in ChangeTracker.Entries<Order>().Select(e => e.Entity))
    {
        entity.ClearDomainEvents();
    }

    return result;
}

// Application/Orders/EventHandlers/OrderConfirmedEventHandler.cs
public class OrderConfirmedEventHandler : INotificationHandler<OrderConfirmedEvent>
{
    private readonly IEmailService _emailService;

    public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
    {
        // 주문 확인 이메일 발송
        await _emailService.SendOrderConfirmationAsync(
            notification.CustomerId,
            notification.OrderId);
    }
}
```

Domain Events는 Aggregate 간 느슨한 결합을 가능하게 합니다. `Order`는 이메일 발송을 알 필요 없이, 단지 "주문이 확인됨" 이벤트를 발생시킵니다.

## Part 4: Modular Monolith - 마이크로서비스의 이점을 모놀리스에서

### Modular Monolith란?

**Modular Monolith**는 단일 배포 단위(모놀리스)이지만, 내부적으로 독립적인 모듈로 구성된 아키텍처입니다. 각 모듈은 마이크로서비스처럼 명확한 경계를 가지며, 다른 모듈과 인터페이스를 통해서만 통신합니다.

```
Monolith.dll
├── Modules/
│   ├── Orders/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Api/
│   ├── Customers/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Api/
│   └── Catalog/
│       ├── Domain/
│       ├── Application/
│       ├── Infrastructure/
│       └── Api/
└── Shared/
    └── Common/
```

**모듈 간 통신:**

```csharp
// Modules/Orders/Application/IntegrationEvents/OrderConfirmedIntegrationEvent.cs
public record OrderConfirmedIntegrationEvent(int OrderId, int CustomerId, decimal TotalAmount);

// 발행 (Orders 모듈)
public class OrderConfirmedEventHandler : INotificationHandler<OrderConfirmedEvent>
{
    private readonly IEventBus _eventBus;

    public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
    {
        // Integration Event 발행
        await _eventBus.PublishAsync(new OrderConfirmedIntegrationEvent(
            notification.OrderId,
            notification.CustomerId,
            notification.TotalAmount));
    }
}

// 구독 (Customers 모듈)
public class OrderConfirmedIntegrationEventHandler
    : IIntegrationEventHandler<OrderConfirmedIntegrationEvent>
{
    private readonly ICustomerRepository _customerRepository;

    public async Task Handle(OrderConfirmedIntegrationEvent @event)
    {
        // 고객의 총 주문 금액 업데이트
        var customer = await _customerRepository.GetByIdAsync(@event.CustomerId);
        customer.AddOrderTotal(@event.TotalAmount);
        await _customerRepository.SaveAsync();
    }
}
```

각 모듈은 자신의 데이터베이스 스키마를 가질 수도 있고(같은 데이터베이스 내 다른 스키마), 나중에 별도의 데이터베이스로 분리할 수도 있습니다. Modular Monolith는 마이크로서비스로의 점진적 전환을 가능하게 합니다.

## 핵심 교훈

1. **Clean Architecture**: 의존성은 항상 내부를 향한다 (Domain ← Application ← Infrastructure ← Presentation)
2. **CQRS**: 읽기와 쓰기를 분리하여 각각 최적화
3. **MediatR**: Pipeline Behaviors로 횡단 관심사 자동화
4. **DDD**: Entity, Value Object, Aggregate, Domain Events로 복잡한 도메인 모델링
5. **Modular Monolith**: 마이크로서비스의 이점을 단일 배포 단위에서

이 패턴들은 복잡성을 제어하는 검증된 방법입니다. 하지만 모든 프로젝트에 적용할 필요는 없습니다. 간단한 CRUD 애플리케이션에는 과도합니다. 팀의 크기, 도메인의 복잡도, 변경 빈도를 고려하여 적절한 패턴을 선택하세요.

다음 챕터에서는 테스트를 다룹니다. 단위 테스트, 통합 테스트, E2E 테스트—Clean Architecture와 CQRS가 테스트를 얼마나 쉽게 만드는지 경험하게 될 것입니다.
