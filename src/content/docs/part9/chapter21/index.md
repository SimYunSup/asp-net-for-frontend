---
title: "Chapter 21 - 단위 테스트와 통합 테스트"
---

# Chapter 21: 단위 테스트와 통합 테스트

## 신뢰할 수 있는 코드의 기반

프론트엔드 개발자로서 Jest나 Vitest로 `describe()`, `it()`, `expect()`를 작성하며 테스트의 가치를 경험했을 것입니다. 버튼을 클릭했을 때 올바른 함수가 호출되는지, API 호출이 성공했을 때 UI가 업데이트되는지—이런 것들을 자동으로 검증하는 테스트 덕분에 리팩토링도, 새 기능 추가도 자신 있게 할 수 있었습니다.

ASP.NET Core의 테스팅 생태계는 JavaScript와 유사한 철학을 따르지만, 서버 사이드의 복잡성을 고려한 강력한 도구들을 제공합니다. xUnit은 Jest처럼 직관적이면서도 더 강력한 타입 안전성을 제공하고, Moq는 `jest.fn()`보다 더 정교한 모킹을 가능하게 하며, WebApplicationFactory는 실제 HTTP 요청을 메모리 내에서 테스트할 수 있게 해줍니다.

이 장에서는 단위 테스트부터 통합 테스트, Blazor 컴포넌트 테스트, 그리고 E2E 테스트까지—신뢰할 수 있는 ASP.NET Core 애플리케이션을 만드는 모든 테스팅 전략을 배웁니다.

## xUnit 기초: [Fact]와 [Theory]

xUnit은 .NET 생태계의 사실상 표준 테스트 프레임워크입니다. ASP.NET Core 팀이 만든 프로젝트 템플릿에도 기본으로 포함되어 있으며, 현대적이고 확장 가능한 아키텍처를 갖추고 있습니다.

### 첫 단위 테스트 작성하기

먼저 테스트 프로젝트를 생성합니다:

```bash
dotnet new xunit -n MyApp.Tests
cd MyApp.Tests
dotnet add reference ../MyApp/MyApp.csproj
```

xUnit 프로젝트는 자동으로 필요한 패키지를 포함합니다:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.0" />
  <PackageReference Include="xunit" Version="2.9.0" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
</ItemGroup>
```

가장 간단한 테스트부터 시작해봅시다. 쇼핑 카트에 상품을 추가하는 로직을 테스트합니다:

```csharp
// Domain/ShoppingCart.cs
public class ShoppingCart
{
    private readonly List<CartItem> _items = new();

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(i => i.Price * i.Quantity);

    public void AddItem(Product product, int quantity)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            });
        }
    }

    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
            _items.Remove(item);
    }

    public void Clear() => _items.Clear();
}

public class CartItem
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

이제 테스트를 작성합니다. xUnit에서는 `[Fact]` 특성으로 테스트 메서드를 표시합니다:

```csharp
// Tests/Domain/ShoppingCartTests.cs
using Xunit;

public class ShoppingCartTests
{
    [Fact]
    public void AddItem_WithValidProduct_AddsToCart()
    {
        // Arrange - 테스트 준비
        var cart = new ShoppingCart();
        var product = new Product { Id = 1, Name = "Book", Price = 10m };

        // Act - 테스트 실행
        cart.AddItem(product, quantity: 2);

        // Assert - 결과 검증
        Assert.Single(cart.Items);
        Assert.Equal(2, cart.Items[0].Quantity);
        Assert.Equal(20m, cart.TotalAmount);
    }

    [Fact]
    public void AddItem_ExistingProduct_IncreasesQuantity()
    {
        // Arrange
        var cart = new ShoppingCart();
        var product = new Product { Id = 1, Name = "Book", Price = 10m };
        cart.AddItem(product, 2);

        // Act - 같은 상품을 다시 추가
        cart.AddItem(product, 3);

        // Assert - 수량이 증가해야 함
        Assert.Single(cart.Items); // 여전히 하나의 항목
        Assert.Equal(5, cart.Items[0].Quantity); // 수량은 2 + 3 = 5
        Assert.Equal(50m, cart.TotalAmount);
    }

    [Fact]
    public void AddItem_WithNullProduct_ThrowsArgumentNullException()
    {
        // Arrange
        var cart = new ShoppingCart();

        // Act & Assert - 예외 검증
        var exception = Assert.Throws<ArgumentNullException>(
            () => cart.AddItem(null!, 1)
        );

        Assert.Equal("product", exception.ParamName);
    }

    [Fact]
    public void AddItem_WithZeroQuantity_ThrowsArgumentException()
    {
        // Arrange
        var cart = new ShoppingCart();
        var product = new Product { Id = 1, Name = "Book", Price = 10m };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => cart.AddItem(product, 0)
        );

        Assert.Equal("quantity", exception.ParamName);
        Assert.Contains("positive", exception.Message);
    }

    [Fact]
    public void RemoveItem_ExistingProduct_RemovesFromCart()
    {
        // Arrange
        var cart = new ShoppingCart();
        var product = new Product { Id = 1, Name = "Book", Price = 10m };
        cart.AddItem(product, 2);

        // Act
        cart.RemoveItem(1);

        // Assert
        Assert.Empty(cart.Items);
        Assert.Equal(0m, cart.TotalAmount);
    }
}
```

테스트를 실행합니다:

```bash
dotnet test
```

Jest와 비교해보면 구조가 매우 유사합니다:

| Jest | xUnit |
|------|-------|
| `describe("ShoppingCart", ...)` | `public class ShoppingCartTests` |
| `test("adds item", ...)` | `[Fact] public void AddItem_...()` |
| `expect(cart.items).toHaveLength(1)` | `Assert.Single(cart.Items)` |
| `expect(total).toBe(20)` | `Assert.Equal(20m, cart.TotalAmount)` |

**AAA 패턴**은 테스트를 명확하게 구조화하는 보편적인 방식입니다:
- **Arrange**: 테스트에 필요한 객체와 데이터를 준비합니다
- **Act**: 테스트하려는 동작을 실행합니다
- **Assert**: 결과가 예상과 일치하는지 검증합니다

### Theory: 파라미터화된 테스트

같은 로직을 여러 입력값으로 테스트하려면 `[Theory]`와 `[InlineData]`를 사용합니다. Jest의 `test.each()`와 유사합니다:

```csharp
public class DiscountCalculatorTests
{
    [Theory]
    [InlineData(100, 10, 90)]      // 100원에 10% 할인 = 90원
    [InlineData(50, 20, 40)]       // 50원에 20% 할인 = 40원
    [InlineData(200, 0, 200)]      // 0% 할인 = 원가
    [InlineData(100, 100, 0)]      // 100% 할인 = 무료
    public void ApplyDiscount_VariousInputs_CalculatesCorrectly(
        decimal price, int discountPercent, decimal expected)
    {
        // Arrange
        var calculator = new DiscountCalculator();

        // Act
        var result = calculator.ApplyDiscount(price, discountPercent);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(-10, 10)]  // 음수 가격
    [InlineData(100, -5)]  // 음수 할인율
    [InlineData(100, 150)] // 100% 초과 할인
    public void ApplyDiscount_InvalidInputs_ThrowsException(
        decimal price, int discountPercent)
    {
        // Arrange
        var calculator = new DiscountCalculator();

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => calculator.ApplyDiscount(price, discountPercent)
        );
    }
}
```

각 `[InlineData]` 속성은 별도의 테스트 케이스로 실행됩니다. 테스트 실행 결과는 각각 개별적으로 표시됩니다.

더 복잡한 데이터는 `[MemberData]`나 `[ClassData]`를 사용할 수 있습니다:

```csharp
public class OrderValidatorTests
{
    public static IEnumerable<object[]> ValidOrders()
    {
        yield return new object[]
        {
            new Order { Items = new[] { new OrderItem { Quantity = 1, Price = 10m } } }
        };
        yield return new object[]
        {
            new Order { Items = new[] { new OrderItem { Quantity = 5, Price = 20m } } }
        };
    }

    [Theory]
    [MemberData(nameof(ValidOrders))]
    public void Validate_ValidOrder_ReturnsTrue(Order order)
    {
        var validator = new OrderValidator();

        var result = validator.Validate(order);

        Assert.True(result.IsValid);
    }
}
```

### Fixtures: 테스트 간 공유 리소스

여러 테스트가 동일한 설정을 필요로 한다면 `IClassFixture<T>`를 사용합니다. Jest의 `beforeAll()`과 유사하지만 더 명시적입니다:

```csharp
// Fixtures/DatabaseFixture.cs
public class DatabaseFixture : IDisposable
{
    public ApplicationDbContext DbContext { get; }

    public DatabaseFixture()
    {
        // 테스트용 인메모리 데이터베이스 생성
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        DbContext = new ApplicationDbContext(options);

        // 테스트 데이터 시드
        DbContext.Products.AddRange(
            new Product { Id = 1, Name = "Book", Price = 10m, Stock = 100 },
            new Product { Id = 2, Name = "Pen", Price = 2m, Stock = 50 },
            new Product { Id = 3, Name = "Notebook", Price = 5m, Stock = 200 }
        );
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}

// Tests/Repositories/ProductRepositoryTests.cs
public class ProductRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests(DatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _repository = new ProductRepository(_dbContext);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
    {
        // Act
        var product = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(product);
        Assert.Equal("Book", product.Name);
        Assert.Equal(10m, product.Price);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentProduct_ReturnsNull()
    {
        // Act
        var product = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(product);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Act
        var products = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, products.Count);
    }
}
```

`DatabaseFixture`는 클래스의 모든 테스트가 시작되기 전에 한 번 생성되고, 모든 테스트가 끝난 후 `Dispose()`가 호출됩니다. 각 테스트 메서드는 생성자를 통해 fixture를 주입받습니다.

여러 테스트 클래스가 같은 fixture를 공유하려면 `ICollectionFixture<T>`를 사용합니다:

```csharp
[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // 이 클래스는 마커일 뿐, 코드는 필요 없음
}

[Collection("Database collection")]
public class ProductRepositoryTests
{
    // ...
}

[Collection("Database collection")]
public class OrderRepositoryTests
{
    // 같은 DatabaseFixture를 공유
}
```

## Moq: 의존성을 목으로 대체하기

단위 테스트의 핵심은 **격리**입니다. 테스트하려는 클래스만 실제로 사용하고, 의존성은 모두 목(mock)으로 대체하여 외부 요인의 영향을 제거합니다.

### Moq 설치와 기본 사용

```bash
dotnet add package Moq
```

`OrderService`를 테스트한다고 가정해봅시다. 이 서비스는 여러 의존성을 가집니다:

```csharp
// Services/OrderService.cs
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(Order order);
    Task UpdateAsync(Order order);
}

public interface IEmailService
{
    Task SendOrderConfirmationAsync(int userId, Order order);
}

public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPaymentAsync(decimal amount, string cardToken);
}

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailService _emailService;
    private readonly IPaymentGateway _paymentGateway;

    public OrderService(
        IOrderRepository orderRepository,
        IEmailService emailService,
        IPaymentGateway paymentGateway)
    {
        _orderRepository = orderRepository;
        _emailService = emailService;
        _paymentGateway = paymentGateway;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        // 결제 처리
        var paymentResult = await _paymentGateway.ProcessPaymentAsync(
            request.TotalAmount,
            request.CardToken
        );

        if (!paymentResult.Success)
            throw new PaymentFailedException("Payment processing failed");

        // 주문 생성
        var order = new Order
        {
            UserId = request.UserId,
            Items = request.Items,
            TotalAmount = request.TotalAmount,
            PaymentTransactionId = paymentResult.TransactionId,
            Status = OrderStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        var createdOrder = await _orderRepository.CreateAsync(order);

        // 확인 이메일 발송
        await _emailService.SendOrderConfirmationAsync(request.UserId, createdOrder);

        return createdOrder;
    }
}
```

실제 구현을 사용하면 데이터베이스에 연결하고, 실제 이메일을 발송하며, 실제 결제가 처리됩니다. 테스트에서는 이런 일이 일어나서는 안 됩니다. Moq로 가짜 구현을 만듭니다:

```csharp
using Moq;
using Xunit;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_SuccessfulPayment_CreatesOrderAndSendsEmail()
    {
        // Arrange - 목 객체 생성
        var mockRepository = new Mock<IOrderRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockPaymentGateway = new Mock<IPaymentGateway>();

        // Setup: 결제 성공 시뮬레이션
        mockPaymentGateway
            .Setup(p => p.ProcessPaymentAsync(It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(new PaymentResult
            {
                Success = true,
                TransactionId = "TXN123"
            });

        // Setup: 주문 저장 시뮬레이션
        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order order) =>
            {
                order.Id = 1; // ID 할당 시뮬레이션
                return order;
            });

        var orderService = new OrderService(
            mockRepository.Object,
            mockEmailService.Object,
            mockPaymentGateway.Object
        );

        var request = new CreateOrderRequest
        {
            UserId = 123,
            Items = new List<OrderItem>
            {
                new() { ProductId = 1, Quantity = 2, Price = 10m }
            },
            TotalAmount = 20m,
            CardToken = "tok_visa"
        };

        // Act
        var result = await orderService.CreateOrderAsync(request);

        // Assert - 결과 검증
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(OrderStatus.Confirmed, result.Status);
        Assert.Equal("TXN123", result.PaymentTransactionId);

        // Assert - 메서드 호출 검증
        mockPaymentGateway.Verify(
            p => p.ProcessPaymentAsync(20m, "tok_visa"),
            Times.Once
        );

        mockRepository.Verify(
            r => r.CreateAsync(It.Is<Order>(o =>
                o.UserId == 123 &&
                o.TotalAmount == 20m &&
                o.Status == OrderStatus.Confirmed
            )),
            Times.Once
        );

        mockEmailService.Verify(
            e => e.SendOrderConfirmationAsync(123, It.IsAny<Order>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateOrderAsync_PaymentFails_ThrowsException()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockPaymentGateway = new Mock<IPaymentGateway>();

        // Setup: 결제 실패 시뮬레이션
        mockPaymentGateway
            .Setup(p => p.ProcessPaymentAsync(It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(new PaymentResult { Success = false });

        var orderService = new OrderService(
            mockRepository.Object,
            mockEmailService.Object,
            mockPaymentGateway.Object
        );

        var request = new CreateOrderRequest
        {
            UserId = 123,
            TotalAmount = 20m,
            CardToken = "tok_invalid"
        };

        // Act & Assert - 예외 검증
        await Assert.ThrowsAsync<PaymentFailedException>(
            async () => await orderService.CreateOrderAsync(request)
        );

        // Assert - 주문이 생성되지 않았는지 확인
        mockRepository.Verify(
            r => r.CreateAsync(It.IsAny<Order>()),
            Times.Never
        );

        // Assert - 이메일이 발송되지 않았는지 확인
        mockEmailService.Verify(
            e => e.SendOrderConfirmationAsync(It.IsAny<int>(), It.IsAny<Order>()),
            Times.Never
        );
    }
}
```

### Moq의 핵심 기능

**1. Setup: 메서드의 반환 값 정의**

```csharp
// 특정 매개변수에 대한 반환 값
mock.Setup(x => x.GetById(1))
    .Returns(new Product { Id = 1, Name = "Book" });

// 모든 매개변수에 대한 반환 값
mock.Setup(x => x.GetById(It.IsAny<int>()))
    .Returns(new Product { Id = 0, Name = "Default" });

// 비동기 메서드
mock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
    .ReturnsAsync(new Product { Id = 1 });

// 매개변수에 따라 다른 반환 값
mock.Setup(x => x.GetById(It.Is<int>(id => id > 0)))
    .Returns((int id) => new Product { Id = id });
```

**2. Verify: 메서드 호출 검증**

```csharp
// 정확히 한 번 호출되었는지
mock.Verify(x => x.Save(It.IsAny<Product>()), Times.Once);

// 호출되지 않았는지
mock.Verify(x => x.Delete(It.IsAny<int>()), Times.Never);

// 정확히 N번 호출되었는지
mock.Verify(x => x.Update(It.IsAny<Product>()), Times.Exactly(3));

// 최소/최대 호출 횟수
mock.Verify(x => x.Log(It.IsAny<string>()), Times.AtLeastOnce);
mock.Verify(x => x.SendEmail(It.IsAny<string>()), Times.AtMost(5));
```

**3. It.Is: 매개변수 조건 검증**

```csharp
// 특정 조건을 만족하는 매개변수
mock.Verify(x => x.CreateOrder(
    It.Is<Order>(o => o.TotalAmount > 100 && o.UserId == 123)
), Times.Once);

// 범위 검증
mock.Verify(x => x.ApplyDiscount(
    It.IsInRange(10m, 100m, Moq.Range.Inclusive)
), Times.Once);

// 문자열 패턴 매칭
mock.Verify(x => x.SendEmail(
    It.IsRegex(@".*@example\.com")
), Times.Once);
```

**4. Callback: 메서드 호출 시 추가 동작**

```csharp
var capturedOrder = default(Order);

mock.Setup(x => x.CreateAsync(It.IsAny<Order>()))
    .Callback<Order>(order => capturedOrder = order)
    .ReturnsAsync((Order o) => o);

await service.CreateOrderAsync(request);

// Callback에서 캡처한 값을 검증
Assert.NotNull(capturedOrder);
Assert.Equal(123, capturedOrder.UserId);
```

**5. Throws: 예외 시뮬레이션**

```csharp
// 동기 메서드 예외
mock.Setup(x => x.GetById(999))
    .Throws<NotFoundException>();

// 비동기 메서드 예외
mock.Setup(x => x.GetByIdAsync(999))
    .ThrowsAsync(new NotFoundException("Product not found"));

// 특정 조건에서만 예외
mock.Setup(x => x.ProcessPayment(It.Is<decimal>(amount => amount < 0)))
    .Throws<ArgumentException>();
```

**6. 순차적 반환 값**

```csharp
// 호출 순서에 따라 다른 값 반환
var mock = new Mock<IStockService>();

mock.SetupSequence(x => x.GetStockLevel(1))
    .Returns(10)   // 첫 호출
    .Returns(5)    // 두 번째 호출
    .Returns(0);   // 세 번째 호출

Assert.Equal(10, mock.Object.GetStockLevel(1));
Assert.Equal(5, mock.Object.GetStockLevel(1));
Assert.Equal(0, mock.Object.GetStockLevel(1));
```

## FluentAssertions: 가독성 높은 검증

xUnit의 `Assert` 클래스는 강력하지만, FluentAssertions를 사용하면 더 읽기 쉬운 테스트를 작성할 수 있습니다:

```bash
dotnet add package FluentAssertions
```

```csharp
using FluentAssertions;

public class ProductServiceTests
{
    [Fact]
    public async Task GetProductsAsync_ReturnsProducts()
    {
        // Arrange
        var service = new ProductService();

        // Act
        var products = await service.GetProductsAsync();

        // Assert - FluentAssertions 스타일
        products.Should().NotBeNull()
            .And.HaveCount(3)
            .And.Contain(p => p.Name == "Book")
            .And.OnlyContain(p => p.Price > 0);

        products.Should().BeInAscendingOrder(p => p.Name);
    }

    [Fact]
    public void CreateOrder_InvalidInput_ThrowsException()
    {
        var service = new OrderService();

        // 예외 검증이 더 자연스러움
        Action act = () => service.CreateOrder(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*order*")
            .And.ParamName.Should().Be("order");
    }

    [Fact]
    public async Task ProcessOrder_UpdatesTimestamp()
    {
        var order = new Order { CreatedAt = DateTime.UtcNow };

        await Task.Delay(100);
        order.UpdatedAt = DateTime.UtcNow;

        // 시간 관련 검증
        order.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        order.UpdatedAt.Should().BeAfter(order.CreatedAt);
    }

    [Fact]
    public void CalculateTotal_ComplexOrder_ReturnsCorrectAmount()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new() { Price = 10m, Quantity = 2 },
                new() { Price = 5m, Quantity = 3 }
            },
            DiscountPercent = 10
        };

        var total = order.CalculateTotal();

        // 소수점 비교 (부동소수점 오차 고려)
        total.Should().BeApproximately(31.5m, 0.01m);
    }
}
```

## WebApplicationFactory: 통합 테스트의 핵심

단위 테스트는 개별 클래스를 검증하지만, 통합 테스트는 여러 컴포넌트가 함께 작동하는지 확인합니다. ASP.NET Core의 `WebApplicationFactory`는 메모리 내에서 전체 애플리케이션을 호스팅하므로, 실제 HTTP 요청을 빠르게 테스트할 수 있습니다.

### 기본 설정

먼저 패키지를 설치합니다:

```bash
dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

`Program.cs`를 테스트에서 접근 가능하게 만들어야 합니다. .NET 6+는 최상위 문장을 사용하므로, 파일 끝에 다음을 추가합니다:

```csharp
// Program.cs 끝에
public partial class Program { }
```

이제 통합 테스트를 작성합니다:

```csharp
// Tests/Integration/ProductsApiTests.cs
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

public class ProductsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessAndProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode(); // 200 OK

        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();

        products.Should().NotBeNull()
            .And.NotBeEmpty();
    }

    [Fact]
    public async Task GetProduct_ExistingId_ReturnsProduct()
    {
        // Act
        var response = await _client.GetAsync("/api/products/1");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetProduct_NonExistentId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/products/999");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProduct_ValidData_ReturnsCreated()
    {
        // Arrange
        var newProduct = new CreateProductDto
        {
            Name = "New Book",
            Price = 15.99m,
            Stock = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be("New Book");

        // Location 헤더 검증
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/products/");
    }
}
```

### 사용자 정의 WebApplicationFactory

프로덕션 서비스를 테스트용으로 교체하려면 `WebApplicationFactory`를 상속합니다:

```csharp
// Tests/Integration/CustomWebApplicationFactory.cs
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. 기존 DbContext 제거
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // 2. 인메모리 데이터베이스로 교체
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // 3. 외부 서비스를 목으로 교체
            services.Remove(services.Single(d => d.ServiceType == typeof(IEmailService)));
            services.AddScoped<IEmailService, FakeEmailService>();

            // 4. 데이터 시딩
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });

        builder.UseEnvironment("Testing");
    }

    private static void SeedTestData(ApplicationDbContext db)
    {
        if (!db.Products.Any())
        {
            db.Products.AddRange(
                new Product { Id = 1, Name = "Test Book", Price = 10m, Stock = 100 },
                new Product { Id = 2, Name = "Test Pen", Price = 2m, Stock = 50 }
            );
            db.SaveChanges();
        }
    }
}

// 가짜 이메일 서비스
public class FakeEmailService : IEmailService
{
    public List<EmailMessage> SentEmails { get; } = new();

    public Task SendOrderConfirmationAsync(int userId, Order order)
    {
        SentEmails.Add(new EmailMessage
        {
            UserId = userId,
            Subject = "Order Confirmation",
            OrderId = order.Id
        });
        return Task.CompletedTask;
    }
}
```

이제 테스트에서 사용합니다:

```csharp
public class OrdersApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OrdersApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_SendsConfirmationEmail()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = 123,
            Items = new[] { new OrderItem { ProductId = 1, Quantity = 2 } }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.EnsureSuccessStatusCode();

        // 이메일이 발송되었는지 확인
        var emailService = _factory.Services
            .GetRequiredService<IEmailService>() as FakeEmailService;

        emailService!.SentEmails.Should().ContainSingle()
            .Which.UserId.Should().Be(123);
    }
}
```

### 인증된 요청 테스트

`[Authorize]` 특성이 있는 엔드포인트를 테스트하려면 인증을 추가해야 합니다. 테스트용 인증 핸들러를 만듭니다:

```csharp
// Tests/Infrastructure/TestAuthHandler.cs
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Name, "test@example.com"),
            new Claim(ClaimTypes.Role, "User")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

// CustomWebApplicationFactory에 추가
protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.ConfigureTestServices(services =>
    {
        services.AddAuthentication("Test")
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
    });
}
```

테스트에서 인증 헤더를 설정합니다:

```csharp
[Fact]
public async Task GetMyOrders_Authenticated_ReturnsOrders()
{
    // Arrange
    _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Test");

    // Act
    var response = await _client.GetAsync("/api/orders/my");

    // Assert
    response.EnsureSuccessStatusCode();

    var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
    orders.Should().NotBeNull();
}

[Fact]
public async Task GetMyOrders_NotAuthenticated_ReturnsUnauthorized()
{
    // Act - 인증 헤더 없이 요청
    var response = await _client.GetAsync("/api/orders/my");

    // Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
}
```

## 비동기 테스트: async/await 올바르게 다루기

ASP.NET Core는 본질적으로 비동기적입니다. 비동기 코드를 테스트할 때는 몇 가지 주의사항이 있습니다.

```csharp
public class AsyncTests
{
    // ✅ 올바른 방법: async Task 반환
    [Fact]
    public async Task GetDataAsync_ReturnsData()
    {
        var service = new DataService();

        var result = await service.GetDataAsync();

        result.Should().NotBeNull();
    }

    // ❌ 잘못된 방법: async void
    // xUnit이 완료를 감지할 수 없어 불안정한 테스트
    [Fact]
    public async void BadAsyncTest()
    {
        await Task.Delay(100);
    }

    // 비동기 예외 테스트
    [Fact]
    public async Task DeleteAsync_NonExistent_ThrowsException()
    {
        var repository = new ProductRepository();

        // Assert.ThrowsAsync 사용
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await repository.DeleteAsync(999)
        );

        // FluentAssertions 스타일
        Func<Task> act = async () => await repository.DeleteAsync(999);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*not found*");
    }

    // 병렬 실행 테스트
    [Fact]
    public async Task GetMultiple_ParallelExecution_ReturnsAll()
    {
        var repository = new ProductRepository();

        // 병렬로 여러 제품 가져오기
        var tasks = Enumerable.Range(1, 5)
            .Select(id => repository.GetByIdAsync(id))
            .ToArray();

        var products = await Task.WhenAll(tasks);

        products.Should().HaveCount(5)
            .And.OnlyContain(p => p != null);
    }

    // 타임아웃 설정
    [Fact(Timeout = 5000)] // 5초 타임아웃
    public async Task LongOperation_CompletesInTime()
    {
        var service = new ProcessingService();

        var result = await service.ProcessLargeDataAsync();

        result.Should().NotBeNull();
    }
}
```

## bUnit: Blazor 컴포넌트 테스트

Blazor 컴포넌트를 테스트하려면 bUnit을 사용합니다. React Testing Library와 매우 유사한 철학을 따릅니다:

```bash
dotnet add package bUnit
dotnet add package bUnit.web
```

간단한 Counter 컴포넌트를 테스트해봅시다:

```razor
@* Components/Counter.razor *@
<div>
    <p>Current count: @currentCount</p>
    <button class="btn btn-primary" @onclick="IncrementCount">Click me</button>
</div>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

테스트는 다음과 같습니다:

```csharp
using Bunit;
using Xunit;

public class CounterTests : TestContext
{
    [Fact]
    public void Counter_InitialState_ShowsZero()
    {
        // Arrange & Act
        var cut = RenderComponent<Counter>();

        // Assert
        cut.Find("p").MarkupMatches("<p>Current count: 0</p>");
    }

    [Fact]
    public void Counter_ClickButton_IncrementsCount()
    {
        // Arrange
        var cut = RenderComponent<Counter>();

        // Act
        cut.Find("button").Click();

        // Assert
        cut.Find("p").MarkupMatches("<p>Current count: 1</p>");
    }

    [Fact]
    public void Counter_MultipleClicks_IncrementsMultipleTimes()
    {
        // Arrange
        var cut = RenderComponent<Counter>();
        var button = cut.Find("button");

        // Act
        button.Click();
        button.Click();
        button.Click();

        // Assert
        cut.Find("p").TextContent.Should().Be("Current count: 3");
    }
}
```

더 복잡한 컴포넌트 테스트:

```razor
@* Components/ProductCard.razor *@
@inject IProductService ProductService

<div class="card">
    <h3>@Product.Name</h3>
    <p>Price: $@Product.Price</p>
    <p>Stock: @Product.Stock</p>
    <button class="btn btn-primary" @onclick="HandleAddToCart" disabled="@(Product.Stock == 0)">
        Add to Cart
    </button>
</div>

@code {
    [Parameter] public Product Product { get; set; } = null!;
    [Parameter] public EventCallback<Product> OnAddToCart { get; set; }

    private async Task HandleAddToCart()
    {
        await OnAddToCart.InvokeAsync(Product);
    }
}
```

```csharp
public class ProductCardTests : TestContext
{
    [Fact]
    public void ProductCard_RendersProductInfo()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Book",
            Price = 19.99m,
            Stock = 10
        };

        // Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Product, product)
        );

        // Assert
        cut.Find("h3").TextContent.Should().Be("Test Book");
        cut.Find("p").TextContent.Should().Contain("$19.99");
        cut.Find("p").TextContent.Should().Contain("Stock: 10");
    }

    [Fact]
    public void ProductCard_OutOfStock_DisablesButton()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Book", Price = 10m, Stock = 0 };

        // Act
        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Product, product)
        );

        // Assert
        var button = cut.Find("button");
        button.HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ProductCard_ClickAddToCart_InvokesCallback()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Book", Price = 10m, Stock = 10 };
        Product? capturedProduct = null;

        var cut = RenderComponent<ProductCard>(parameters => parameters
            .Add(p => p.Product, product)
            .Add(p => p.OnAddToCart, EventCallback.Factory.Create<Product>(
                this, p => capturedProduct = p))
        );

        // Act
        cut.Find("button").Click();

        // Assert
        capturedProduct.Should().NotBeNull();
        capturedProduct!.Id.Should().Be(1);
    }

    [Fact]
    public void ProductList_WithMockedService_RendersProducts()
    {
        // Arrange - 서비스 모킹
        var mockService = new Mock<IProductService>();
        mockService.Setup(s => s.GetProductsAsync())
            .ReturnsAsync(new List<Product>
            {
                new() { Id = 1, Name = "Book", Price = 10m, Stock = 10 },
                new() { Id = 2, Name = "Pen", Price = 2m, Stock = 50 }
            });

        Services.AddSingleton(mockService.Object);

        // Act
        var cut = RenderComponent<ProductList>();

        // Assert
        var cards = cut.FindAll(".card");
        cards.Should().HaveCount(2);
    }
}
```

## Playwright: E2E 테스트

E2E 테스트는 사용자 관점에서 애플리케이션 전체를 테스트합니다. Playwright for .NET은 Chromium, Firefox, WebKit을 자동화할 수 있습니다:

```bash
dotnet add package Microsoft.Playwright
dotnet add package Microsoft.Playwright.NUnit
pwsh bin/Debug/net8.0/playwright.ps1 install
```

```csharp
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[Parallelizable(ParallelScope.Self)]
public class OrderE2ETests : PageTest
{
    [Test]
    public async Task User_CanPlaceOrder()
    {
        // Arrange - 애플리케이션으로 이동
        await Page.GotoAsync("https://localhost:5001");

        // Act - 로그인
        await Page.ClickAsync("text=Login");
        await Page.FillAsync("#email", "test@example.com");
        await Page.FillAsync("#password", "password123");
        await Page.ClickAsync("#login-button");

        // Act - 제품 검색 및 추가
        await Page.FillAsync("#search", "book");
        await Page.PressAsync("#search", "Enter");

        await Page.ClickAsync("text=Add to Cart");

        // Act - 장바구니 확인
        await Page.ClickAsync("text=Cart");

        // Assert - 장바구니에 상품이 있는지
        await Expect(Page.Locator(".cart-item")).ToHaveCountAsync(1);
        await Expect(Page.Locator(".cart-total")).ToContainTextAsync("$");

        // Act - 체크아웃
        await Page.ClickAsync("text=Checkout");

        await Page.FillAsync("#card-number", "4242424242424242");
        await Page.FillAsync("#card-expiry", "12/25");
        await Page.FillAsync("#card-cvc", "123");

        await Page.ClickAsync("text=Place Order");

        // Assert - 주문 확인
        await Expect(Page.Locator("h1")).ToContainTextAsync("Order Confirmed");
        await Expect(Page.Locator(".order-id")).ToBeVisibleAsync();
    }

    [Test]
    public async Task User_CannotCheckout_WithEmptyCart()
    {
        await Page.GotoAsync("https://localhost:5001");

        // 빈 장바구니로 체크아웃 시도
        await Page.ClickAsync("text=Cart");

        // 체크아웃 버튼이 비활성화되어 있어야 함
        var checkoutButton = Page.Locator("text=Checkout");
        await Expect(checkoutButton).ToBeDisabledAsync();
    }

    [Test]
    public async Task User_CanSearch_AndFilterProducts()
    {
        await Page.GotoAsync("https://localhost:5001/products");

        // 검색
        await Page.FillAsync("#search", "book");
        await Page.PressAsync("#search", "Enter");

        // 결과 확인
        var products = Page.Locator(".product-card");
        (await products.CountAsync()).Should().BeGreaterThan(0);

        // 모든 결과에 "book"이 포함되어야 함
        var productNames = await products.Locator("h3").AllTextContentsAsync();
        productNames.Should().OnlyContain(name =>
            name.Contains("book", StringComparison.OrdinalIgnoreCase));

        // 가격 필터
        await Page.ClickAsync("#price-filter");
        await Page.ClickAsync("text=Under $20");

        // 스크린샷 캡처 (실패 시 디버깅용)
        await Page.ScreenshotAsync(new() { Path = "test-results/search-results.png" });
    }
}
```

## 테스트 주도 개발(TDD): Red-Green-Refactor

TDD는 테스트를 먼저 작성하고, 그 테스트를 통과시키는 최소한의 코드를 작성한 후, 리팩토링하는 개발 방식입니다.

할인 시스템을 TDD로 구현해봅시다:

**Red: 실패하는 테스트 작성**

```csharp
public class DiscountServiceTests
{
    [Theory]
    [InlineData(100, 10, 90)]
    [InlineData(50, 20, 40)]
    [InlineData(200, 0, 200)]
    public void ApplyPercentageDiscount_CalculatesCorrectly(
        decimal price, int percent, decimal expected)
    {
        // Arrange
        var service = new DiscountService();

        // Act
        var result = service.ApplyPercentageDiscount(price, percent);

        // Assert
        result.Should().Be(expected);
    }
}
```

이 테스트는 `DiscountService`가 존재하지 않으므로 컴파일조차 되지 않습니다.

**Green: 최소한의 코드로 통과**

```csharp
public class DiscountService
{
    public decimal ApplyPercentageDiscount(decimal price, int percent)
    {
        return price * (100 - percent) / 100;
    }
}
```

테스트가 통과합니다!

**Red: 더 많은 시나리오 추가**

```csharp
[Fact]
public void ApplyPercentageDiscount_NegativePrice_ThrowsException()
{
    var service = new DiscountService();

    Action act = () => service.ApplyPercentageDiscount(-10, 10);

    act.Should().Throw<ArgumentException>()
        .WithMessage("*positive*");
}

[Fact]
public void ApplyPercentageDiscount_InvalidPercent_ThrowsException()
{
    var service = new DiscountService();

    Action act = () => service.ApplyPercentageDiscount(100, -5);

    act.Should().Throw<ArgumentException>();
}
```

테스트가 실패합니다.

**Green: 검증 로직 추가**

```csharp
public decimal ApplyPercentageDiscount(decimal price, int percent)
{
    if (price < 0)
        throw new ArgumentException("Price must be positive", nameof(price));
    if (percent < 0 || percent > 100)
        throw new ArgumentException("Percent must be between 0 and 100", nameof(percent));

    return price * (100 - percent) / 100;
}
```

**Refactor: 코드 개선**

```csharp
public class DiscountService
{
    public decimal ApplyPercentageDiscount(decimal price, int percent)
    {
        ValidatePrice(price);
        ValidatePercent(percent);

        return CalculateDiscountedPrice(price, percent);
    }

    private static void ValidatePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price must be positive", nameof(price));
    }

    private static void ValidatePercent(int percent)
    {
        if (percent < 0 || percent > 100)
            throw new ArgumentException(
                "Percent must be between 0 and 100",
                nameof(percent)
            );
    }

    private static decimal CalculateDiscountedPrice(decimal price, int percent)
    {
        return price * (100 - percent) / 100;
    }
}
```

테스트가 여전히 통과합니다. 리팩토링은 테스트를 변경하지 않고 코드 구조만 개선합니다.

## 테스트 커버리지: 측정과 해석

Coverlet으로 코드 커버리지를 측정할 수 있습니다:

```bash
dotnet add package coverlet.collector
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

보고서를 HTML로 보려면 ReportGenerator를 사용합니다:

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:coverage.opencover.xml -targetdir:coveragereport -reporttypes:Html
```

`coveragereport/index.html`을 열면 어떤 코드가 테스트되었는지 시각적으로 확인할 수 있습니다.

**커버리지 해석 주의사항:**

100% 커버리지가 버그 없음을 보장하지는 않습니다. 중요한 것은 **의미 있는 테스트**입니다:

```csharp
// ❌ 100% 커버리지지만 의미 없는 테스트
[Fact]
public void Add_Test()
{
    var result = calculator.Add(2, 3);
    // Assert 없음! 그냥 실행만 함
}

// ✅ 80% 커버리지지만 의미 있는 테스트
[Theory]
[InlineData(2, 3, 5)]
[InlineData(-1, 1, 0)]
[InlineData(0, 0, 0)]
public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    var result = calculator.Add(a, b);
    Assert.Equal(expected, result);
}
```

목표는 80-90% 정도가 적절합니다. Getter/Setter, 간단한 DTO 같은 것까지 테스트하려고 100%를 추구하면 오히려 생산성이 떨어집니다.

## 모범 사례와 안티패턴

**단위 테스트 모범 사례:**

```csharp
// ✅ 테스트 이름은 명확하고 서술적으로
[Fact]
public void CreateOrder_WithInsufficientStock_ThrowsOutOfStockException()

// ❌ 모호한 이름
[Fact]
public void Test1()

// ✅ 하나의 테스트는 하나의 것만 검증
[Fact]
public void AddItem_IncreasesItemCount()
{
    cart.AddItem(product, 1);
    Assert.Equal(1, cart.Items.Count);
}

// ❌ 여러 것을 검증 (분리해야 함)
[Fact]
public void CartOperations_Work()
{
    cart.AddItem(product1, 1);
    Assert.Equal(1, cart.Items.Count);

    cart.AddItem(product2, 2);
    Assert.Equal(2, cart.Items.Count);

    cart.RemoveItem(product1.Id);
    Assert.Equal(1, cart.Items.Count);
}

// ✅ 매직 넘버 대신 명명된 상수
const decimal StandardPrice = 10m;
const int DefaultQuantity = 2;

// ✅ 테스트는 독립적이고 순서에 무관
[Fact]
public void Test1() { /* 다른 테스트에 의존하지 않음 */ }

[Fact]
public void Test2() { /* Test1의 결과에 의존하지 않음 */ }
```

**통합 테스트 모범 사례:**

```csharp
// ✅ 각 테스트는 깨끗한 상태에서 시작
public class OrdersApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrdersApiTests(CustomWebApplicationFactory factory)
    {
        // 각 테스트마다 새 클라이언트 (격리)
        _client = factory.CreateClient();
    }

    // ✅ API 계약 테스트 (상태 코드, 응답 스키마)
    [Fact]
    public async Task CreateOrder_ValidRequest_Returns201WithLocation()
    {
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.Id.Should().BeGreaterThan(0);
    }
}
```

## 실전 예제: 주문 시스템 완전 테스트

전체 주문 플로우를 테스트하는 종합 예제입니다:

```csharp
// 1. 도메인 로직 단위 테스트
public class OrderTests
{
    [Fact]
    public void Order_CalculateTotalAmount_ReturnsCorrectSum()
    {
        var order = new Order();
        order.AddItem(new OrderItem { Price = 10m, Quantity = 2 });
        order.AddItem(new OrderItem { Price = 5m, Quantity = 3 });

        order.TotalAmount.Should().Be(35m);
    }

    [Fact]
    public void Order_Confirm_ChangesStatusToConfirmed()
    {
        var order = new Order { Status = OrderStatus.Pending };

        order.Confirm();

        order.Status.Should().Be(OrderStatus.Confirmed);
        order.ConfirmedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Order_Confirm_AlreadyConfirmed_ThrowsException()
    {
        var order = new Order { Status = OrderStatus.Confirmed };

        Action act = () => order.Confirm();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already confirmed*");
    }
}

// 2. Application Layer 테스트 (Moq 사용)
public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidOrder_CreatesOrderAndSendsEmail()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockStockService = new Mock<IStockService>();

        mockStockService
            .Setup(s => s.CheckStockAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => { o.Id = 1; return o; });

        var handler = new CreateOrderCommandHandler(
            mockRepository.Object,
            mockEmailService.Object,
            mockStockService.Object
        );

        var command = new CreateOrderCommand
        {
            UserId = 123,
            Items = new[] { new OrderItem { ProductId = 1, Quantity = 2, Price = 10m } }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);

        mockRepository.Verify(r => r.CreateAsync(It.Is<Order>(o =>
            o.UserId == 123 &&
            o.TotalAmount == 20m
        )), Times.Once);

        mockEmailService.Verify(e =>
            e.SendOrderConfirmationAsync(123, It.IsAny<Order>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_InsufficientStock_ThrowsException()
    {
        var mockStockService = new Mock<IStockService>();
        mockStockService
            .Setup(s => s.CheckStockAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        var handler = new CreateOrderCommandHandler(
            Mock.Of<IOrderRepository>(),
            Mock.Of<IEmailService>(),
            mockStockService.Object
        );

        var command = new CreateOrderCommand { Items = new[] { new OrderItem() } };

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<OutOfStockException>();
    }
}

// 3. API 통합 테스트
public class OrdersApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrdersApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task POST_CreateOrder_ValidRequest_Returns201()
    {
        var request = new CreateOrderRequest
        {
            UserId = 123,
            Items = new[]
            {
                new OrderItemDto { ProductId = 1, Quantity = 2 }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.Id.Should().BeGreaterThan(0);
        order.Status.Should().Be("Confirmed");
    }

    [Fact]
    public async Task GET_GetOrder_ExistingId_Returns200()
    {
        // Arrange - 먼저 주문 생성
        var createResponse = await _client.PostAsJsonAsync("/api/orders", new CreateOrderRequest
        {
            UserId = 123,
            Items = new[] { new OrderItemDto { ProductId = 1, Quantity = 1 } }
        });
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

        // Act
        var getResponse = await _client.GetAsync($"/api/orders/{createdOrder!.Id}");

        // Assert
        getResponse.EnsureSuccessStatusCode();
        var order = await getResponse.Content.ReadFromJsonAsync<OrderDto>();
        order!.Id.Should().Be(createdOrder.Id);
    }

    [Fact]
    public async Task GET_GetOrder_NonExistent_Returns404()
    {
        var response = await _client.GetAsync("/api/orders/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
```

## 마무리

이제 여러분은 ASP.NET Core 애플리케이션을 포괄적으로 테스트할 수 있습니다:

- **xUnit**으로 단위 테스트를 작성하고, AAA 패턴으로 명확하게 구조화합니다
- **Moq**로 의존성을 모킹하여 격리된 테스트를 만듭니다
- **WebApplicationFactory**로 실제 HTTP 요청을 메모리 내에서 테스트합니다
- **bUnit**으로 Blazor 컴포넌트를 React Testing Library처럼 테스트합니다
- **Playwright**로 E2E 테스트를 작성하여 사용자 시나리오를 자동화합니다
- **TDD**로 개발하여 테스트 커버리지와 코드 품질을 동시에 달성합니다

테스트는 단순히 버그를 찾는 것이 아닙니다. 테스트는 **자신감**입니다. 리팩토링할 때, 새 기능을 추가할 때, 프로덕션에 배포할 때—포괄적인 테스트 스위트가 있다면 두려움 없이 진행할 수 있습니다.

다음 Part 10에서는 성능 최적화와 모니터링을 배우며, 빠르고 효율적인 애플리케이션을 만드는 방법을 익힙니다.
