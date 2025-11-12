# Part 9: 테스팅 전략

## 신뢰할 수 있는 코드: 테스트로 만드는 안심

Part 8까지 여러분은 복잡한 시스템을 설계하는 방법을 배웠습니다. Clean Architecture로 계층을 분리하고, CQRS로 읽기와 쓰기를 나누며, DDD로 비즈니스 로직을 표현했습니다. 이제 여러분의 애플리케이션은 잘 구조화되어 있고, 코드는 아름답게 조직되어 있습니다. 하지만 한 가지 질문이 남습니다: **이 코드가 실제로 작동한다고 어떻게 확신할 수 있을까요?**

수동으로 매번 테스트할 수는 없습니다. 새 기능을 추가할 때마다, 기존 기능이 여전히 작동하는지 확인하려면 수십 개의 시나리오를 클릭하고 입력해야 합니다. 누군가의 코드 수정이 전혀 관련 없어 보이는 기능을 망가뜨릴 수 있습니다. 프로덕션에 배포하기 전까지는 버그를 발견하지 못할 수도 있습니다. 이것이 바로 자동화된 테스트가 필요한 이유입니다.

테스트는 단순히 버그를 찾는 것 이상입니다. 테스트는 **리팩토링의 안전망**입니다. 코드를 개선하고 싶지만, 무언가를 망가뜨릴까 두려워 주저한 적이 있나요? 포괄적인 테스트 스위트가 있다면, 자신 있게 리팩토링할 수 있습니다. 테스트가 통과하면, 기능이 여전히 작동합니다. 테스트는 또한 **문서**입니다. 코드가 무엇을 해야 하는지 설명하는 주석보다, 실제로 작동하는 예제인 테스트가 더 정확합니다.

프론트엔드 개발자로서 Jest, Vitest, React Testing Library, Cypress를 사용해봤을 것입니다. 컴포넌트를 렌더링하고, 버튼을 클릭하며, 올바른 텍스트가 나타나는지 확인했습니다. ASP.NET Core의 테스팅도 유사한 철학을 따르지만, 서버 사이드 특성을 고려한 도구들이 있습니다. Part 9에서는 단위 테스트부터 E2E 테스트까지, 신뢰할 수 있는 시스템을 만드는 모든 테스팅 전략을 배웁니다.

### 테스팅 피라미드: 균형 잡힌 접근

모든 테스트가 동일한 가치를 제공하는 것은 아닙니다. 일부는 빠르고 저렴하며 안정적입니다. 다른 것들은 느리고 비싸며 불안정합니다(flaky). 테스팅 피라미드는 어떤 유형의 테스트를 얼마나 작성해야 하는지 안내합니다.

```
           /\
          /  \  E2E Tests (소수)
         /    \
        /------\
       / Integr \  Integration Tests (중간)
      /  -ation  \
     /------------\
    / Unit  Tests  \  Unit Tests (다수)
   /----------------\
```

**피라미드의 기반: 단위 테스트**

피라미드의 가장 넓은 부분은 단위 테스트입니다. 이들은 개별 메서드나 클래스를 격리하여 테스트합니다. 의존성은 목(mock)으로 대체되므로, 데이터베이스나 외부 서비스 없이 실행됩니다. 덕분에 **매우 빠릅니다**—수천 개의 단위 테스트가 몇 초 안에 실행될 수 있습니다. 또한 **안정적입니다**—네트워크나 데이터베이스 상태에 영향을 받지 않습니다.

단위 테스트는 비즈니스 로직, 검증, 계산, 알고리즘에 완벽합니다. "이 할인 계산이 올바른가?", "이 유효성 검사가 잘못된 입력을 거부하는가?"—이런 질문에 답합니다.

```csharp
[Fact]
public void AddItem_WithValidProduct_AddsToCart()
{
    // Arrange
    var cart = new ShoppingCart();
    var product = new Product { Id = 1, Name = "Book", Price = 10m };

    // Act
    cart.AddItem(product, quantity: 2);

    // Assert
    Assert.Single(cart.Items);
    Assert.Equal(2, cart.Items[0].Quantity);
    Assert.Equal(20m, cart.TotalAmount);
}
```

이 테스트는 밀리초 안에 실행되며, 데이터베이스가 필요 없습니다. `ShoppingCart` 클래스의 로직만 검증합니다.

**피라미드의 중간: 통합 테스트**

통합 테스트는 여러 컴포넌트가 함께 작동하는지 확인합니다. 데이터베이스, 캐시, 외부 API—실제 의존성을 사용합니다(또는 테스트 대역을 사용). 단위 테스트보다 느리지만, **더 현실적입니다**. "API 엔드포인트가 실제로 데이터베이스에서 데이터를 가져오는가?", "인증 미들웨어가 제대로 작동하는가?"—이런 질문에 답합니다.

ASP.NET Core는 통합 테스트를 위한 `WebApplicationFactory`를 제공합니다. 이것은 메모리 내에서 전체 애플리케이션을 호스팅하므로, 실제 HTTP 요청을 보낼 수 있습니다.

```csharp
public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsOkWithProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotEmpty(products);
    }
}
```

이 테스트는 실제 HTTP 요청, 라우팅, 컨트롤러, 서비스, 데이터베이스(테스트용)를 모두 거칩니다.

**피라미드의 정점: E2E 테스트**

E2E(End-to-End) 테스트는 사용자 관점에서 애플리케이션 전체를 테스트합니다. 브라우저를 자동화하여 실제 사용자처럼 클릭하고 입력하며, 결과를 확인합니다. 가장 **현실적이지만**, 가장 **느리고 불안정합니다**. 네트워크 지연, 애니메이션, 비동기 로딩—모든 것이 테스트를 flaky하게 만들 수 있습니다.

E2E 테스트는 중요한 사용자 흐름에만 사용합니다: "사용자가 로그인하고 제품을 장바구니에 추가하고 결제할 수 있는가?" 같은 핵심 시나리오입니다.

Playwright for .NET이나 Selenium WebDriver로 E2E 테스트를 작성할 수 있습니다.

```csharp
[Fact]
public async Task UserCanPlaceOrder()
{
    await _page.GotoAsync("https://localhost:5001");
    await _page.ClickAsync("text=Login");
    await _page.FillAsync("#email", "user@example.com");
    await _page.FillAsync("#password", "password");
    await _page.ClickAsync("#login-button");

    await _page.ClickAsync("text=Add to Cart");
    await _page.ClickAsync("text=Checkout");

    await Expect(_page.Locator("text=Order Confirmed")).ToBeVisibleAsync();
}
```

**최적의 비율**

구글은 70/20/10 비율을 권장합니다: 70% 단위 테스트, 20% 통합 테스트, 10% E2E 테스트. 이는 절대적 규칙은 아니지만 좋은 출발점입니다. 프로젝트의 특성에 따라 조정할 수 있습니다.

### xUnit: .NET의 사실상 표준

프론트엔드에 Jest가 있다면, .NET에는 xUnit이 있습니다. xUnit은 .NET 팀과 커뮤니티가 선호하는 테스트 프레임워크로, ASP.NET Core 프로젝트 템플릿의 기본값입니다. NUnit이나 MSTest도 있지만, xUnit이 가장 현대적이고 확장 가능합니다.

**xUnit vs Jest: 개념적 유사성**

Jest에 익숙하다면, xUnit은 금방 이해할 수 있습니다. 많은 개념이 직접 대응됩니다:

| Jest | xUnit |
|------|-------|
| `describe()` | 클래스로 그룹화 |
| `test()` / `it()` | `[Fact]` |
| `test.each()` | `[Theory]` + `[InlineData]` |
| `beforeEach()` | 생성자 |
| `afterEach()` | `Dispose()` |
| `expect(x).toBe(y)` | `Assert.Equal(y, x)` |

**기본 테스트 구조**

```csharp
public class CalculatorTests
{
    [Fact]
    public void Add_TwoNumbers_ReturnsSum()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(2, 3);

        // Assert
        Assert.Equal(5, result);
    }
}
```

`[Fact]`는 Jest의 `test()`와 같습니다. AAA (Arrange-Act-Assert) 패턴은 테스트를 명확하게 구조화합니다.

**Theory: 파라미터화된 테스트**

같은 로직을 여러 입력값으로 테스트하려면, `[Theory]`를 사용합니다. Jest의 `test.each()`와 유사합니다.

```csharp
[Theory]
[InlineData(2, 3, 5)]
[InlineData(0, 0, 0)]
[InlineData(-1, 1, 0)]
[InlineData(100, 200, 300)]
public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    // Arrange
    var calculator = new Calculator();

    // Act
    var result = calculator.Add(a, b);

    // Assert
    Assert.Equal(expected, result);
}
```

하나의 테스트 메서드로 네 가지 시나리오를 검증합니다. 각 `[InlineData]`는 별도의 테스트 케이스로 실행됩니다.

**Fixtures: 테스트 간 공유**

여러 테스트가 같은 설정을 필요로 한다면, `IClassFixture`를 사용합니다. Jest의 `beforeAll()`과 유사하지만 더 명시적입니다.

```csharp
public class DatabaseFixture : IDisposable
{
    public ApplicationDbContext DbContext { get; }

    public DatabaseFixture()
    {
        DbContext = CreateInMemoryDatabase();
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}

public class ProductRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepositoryTests(DatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public async Task GetById_ExistingProduct_ReturnsProduct()
    {
        // 테스트 구현...
    }
}
```

데이터베이스는 클래스의 모든 테스트에서 공유되지만, 각 테스트 메서드 사이에 격리됩니다.

### Moq: 의존성을 목(Mock)으로 대체하기

단위 테스트의 핵심은 격리입니다. 테스트하려는 클래스만 실제 인스턴스를 사용하고, 의존성은 모두 목으로 대체합니다. 프론트엔드에서 `jest.fn()`이나 `vi.fn()`을 사용했다면, .NET의 Moq는 같은 역할을 합니다.

**왜 목이 필요한가?**

`OrderService`를 테스트한다고 가정해봅시다. 이 서비스는 `IOrderRepository`, `IEmailService`, `IPaymentGateway`에 의존합니다. 실제 구현을 사용하면:

- 데이터베이스가 필요합니다 (느림)
- 실제 이메일이 발송됩니다 (부작용)
- 실제 결제가 처리됩니다 (비용 발생!)

목을 사용하면, 이 모든 의존성을 가짜로 대체하여 `OrderService`의 로직만 테스트할 수 있습니다.

**Moq 기본 사용법**

```csharp
[Fact]
public async Task CreateOrder_ValidOrder_SendsConfirmationEmail()
{
    // Arrange
    var mockRepository = new Mock<IOrderRepository>();
    var mockEmailService = new Mock<IEmailService>();

    var orderService = new OrderService(
        mockRepository.Object,
        mockEmailService.Object
    );

    var order = new Order { UserId = 1, Items = new List<OrderItem> { ... } };

    // Act
    await orderService.CreateOrderAsync(order);

    // Assert
    mockEmailService.Verify(
        e => e.SendOrderConfirmationAsync(It.IsAny<int>(), It.IsAny<Order>()),
        Times.Once
    );
}
```

`Mock<T>`는 인터페이스의 가짜 구현을 만듭니다. `Verify()`는 특정 메서드가 호출되었는지 확인합니다. Jest의 `expect(mockFn).toHaveBeenCalledWith()`와 유사합니다.

**Setup: 목의 동작 정의**

목이 특정 값을 반환하도록 설정할 수 있습니다:

```csharp
mockRepository
    .Setup(r => r.GetByIdAsync(1))
    .ReturnsAsync(new Order { Id = 1, UserId = 123 });

mockPaymentGateway
    .Setup(p => p.ProcessPaymentAsync(It.IsAny<decimal>()))
    .ReturnsAsync(new PaymentResult { Success = true, TransactionId = "ABC123" });
```

`It.IsAny<T>()`는 모든 값을 매칭합니다. Jest의 `expect.any()`와 유사합니다.

**예외 시뮬레이션**

실패 시나리오도 테스트해야 합니다. 목을 사용하면 예외를 쉽게 시뮬레이션할 수 있습니다:

```csharp
mockPaymentGateway
    .Setup(p => p.ProcessPaymentAsync(It.IsAny<decimal>()))
    .ThrowsAsync(new PaymentFailedException("Insufficient funds"));

// 이제 OrderService가 이 예외를 어떻게 처리하는지 테스트
```

### FluentAssertions: 가독성 높은 검증

xUnit의 기본 `Assert` 클래스는 강력하지만, 때로는 읽기 어렵습니다. FluentAssertions는 자연어에 가까운 문법으로 더 표현력 있는 검증을 작성할 수 있게 해줍니다. Jest의 `expect()` 체이닝과 유사한 경험을 제공합니다.

**기본 Assert vs FluentAssertions**

```csharp
// xUnit Assert
Assert.Equal(expected, actual);
Assert.True(list.Count > 0);
Assert.Throws<ArgumentException>(() => method());

// FluentAssertions
actual.Should().Be(expected);
list.Should().NotBeEmpty();
Action act = () => method();
act.Should().Throw<ArgumentException>();
```

FluentAssertions는 더 읽기 쉽고, 실패 메시지도 더 명확합니다.

**컬렉션 검증**

```csharp
var products = await _repository.GetAllAsync();

// 풍부한 컬렉션 검증
products.Should().NotBeNull()
    .And.HaveCount(3)
    .And.Contain(p => p.Name == "Book")
    .And.OnlyContain(p => p.Price > 0);

// 특정 순서 검증
products.Should().BeInAscendingOrder(p => p.Name);

// 복잡한 객체 비교
products.Should().BeEquivalentTo(new[]
{
    new { Name = "Book", Price = 10m },
    new { Name = "Pen", Price = 2m }
}, options => options.ExcludingMissingMembers());
```

**예외 검증 개선**

```csharp
// 예외 메시지와 내부 예외까지 검증
Action act = () => service.CreateOrder(null);

act.Should().Throw<ArgumentNullException>()
    .WithMessage("*order*")
    .And.ParamName.Should().Be("order");

// 비동기 예외 검증
Func<Task> act = async () => await service.ProcessPaymentAsync(-100);

await act.Should().ThrowAsync<InvalidOperationException>()
    .WithMessage("Amount must be positive");
```

**복잡한 객체 비교**

```csharp
var actualOrder = await _repository.GetByIdAsync(1);

// 특정 속성만 비교
actualOrder.Should().BeEquivalentTo(expectedOrder, options => options
    .Excluding(o => o.CreatedAt)
    .Excluding(o => o.Id));

// 중첩된 객체 검증
order.Items.Should().AllSatisfy(item =>
{
    item.Quantity.Should().BeGreaterThan(0);
    item.Price.Should().BePositive();
    item.Product.Should().NotBeNull();
});
```

**시간 관련 검증**

```csharp
var createdAt = order.CreatedAt;

// 시간 범위 검증 (타임존 고려)
createdAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

// 날짜 부분만 검증
createdAt.Should().BeSameDateAs(DateTime.Today);
```

FluentAssertions를 설치하려면:

```bash
dotnet add package FluentAssertions
```

그리고 테스트 파일에서 `using FluentAssertions;`를 추가하면 모든 객체에 `.Should()` 메서드가 확장됩니다.

### WebApplicationFactory: 실제처럼 테스트하기

통합 테스트는 단위 테스트와 달리, 실제 애플리케이션의 동작을 확인합니다. 하지만 테스트마다 서버를 시작하고 종료하는 것은 느립니다. ASP.NET Core의 `WebApplicationFactory`는 이 문제를 해결합니다. 메모리 내에서 애플리케이션을 호스팅하므로, 실제 HTTP 요청을 빠르게 테스트할 수 있습니다.

**기본 설정**

```csharp
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/products");
        response.EnsureSuccessStatusCode();
    }
}
```

`Program`은 `Program.cs`의 클래스로, .NET 6+ 에서 최상위 문장으로 작성됩니다. 테스트에서 접근하려면 `Program.cs`의 끝에 `public partial class Program {}`를 추가해야 합니다.

**서비스 오버라이드: 테스트용 구성**

프로덕션 서비스를 테스트용으로 교체할 수 있습니다. 예를 들어, 실제 데이터베이스 대신 메모리 내 데이터베이스를 사용:

```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 기존 DbContext 제거
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // 인메모리 데이터베이스로 교체
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // 실제 이메일 서비스 대신 목으로 교체
            services.AddScoped<IEmailService, FakeEmailService>();
        });
    }
}
```

이제 각 테스트는 깨끗한 인메모리 데이터베이스에서 시작하며, 실제 이메일을 발송하지 않습니다.

**인증된 요청 테스트**

`[Authorize]` 특성이 있는 엔드포인트를 테스트하려면, 인증 토큰이 필요합니다. 테스트에서는 진짜 로그인 대신, 가짜 인증을 주입할 수 있습니다:

```csharp
builder.ConfigureTestServices(services =>
{
    services.AddAuthentication("Test")
        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
});

// 테스트에서
_client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Test");
```

`TestAuthHandler`는 모든 요청을 인증된 것으로 간주합니다.

### Blazor 컴포넌트 테스트: bUnit의 힘

Blazor 컴포넌트를 테스트하려면 어떻게 해야 할까요? 컴포넌트는 렌더링되어야 하고, 사용자 이벤트를 시뮬레이션해야 하며, DOM을 검증해야 합니다. React Testing Library에 익숙하다면, bUnit은 거의 동일한 철학을 따릅니다.

**bUnit 기본 사용**

```csharp
using Bunit;
using Xunit;

public class CounterTests : TestContext
{
    [Fact]
    public void Counter_ClickButton_IncrementsCount()
    {
        // Arrange
        var cut = RenderComponent<Counter>();

        // Assert - 초기 상태
        cut.Find("p").MarkupMatches("<p>Current count: 0</p>");

        // Act - 버튼 클릭
        cut.Find("button").Click();

        // Assert - 업데이트된 상태
        cut.Find("p").MarkupMatches("<p>Current count: 1</p>");
    }
}
```

`RenderComponent<T>()`는 컴포넌트를 렌더링하고, `Find()`는 CSS 선택자로 요소를 찾으며, `Click()`은 클릭 이벤트를 시뮬레이션합니다. React Testing Library의 `render()`, `getByText()`, `fireEvent.click()`과 거의 동일합니다.

**파라미터 전달**

컴포넌트에 파라미터를 전달하려면:

```csharp
var cut = RenderComponent<ProductCard>(parameters => parameters
    .Add(p => p.ProductName, "Test Product")
    .Add(p => p.Price, 99.99m)
    .Add(p => p.OnAddToCart, () => Console.WriteLine("Added"))
);
```

**서비스 주입**

컴포넌트가 `@inject IProductService ProductService`를 사용한다면, 테스트에서 목을 주입할 수 있습니다:

```csharp
var mockProductService = new Mock<IProductService>();
mockProductService.Setup(s => s.GetProductsAsync())
    .ReturnsAsync(new List<Product> { ... });

Services.AddSingleton(mockProductService.Object);

var cut = RenderComponent<ProductList>();
```

### 테스트 주도 개발(TDD): 코드보다 테스트를 먼저

TDD는 개발 방식의 전환입니다. 일반적으로는 코드를 작성하고 나서 테스트를 추가하지만, TDD는 반대입니다: **먼저 실패하는 테스트를 작성하고, 그 테스트를 통과시키는 최소한의 코드를 작성한 후, 리팩토링합니다**.

Red → Green → Refactor의 사이클:

1. **Red**: 실패하는 테스트를 작성합니다. 아직 기능이 구현되지 않았으므로 당연히 실패합니다.
2. **Green**: 테스트를 통과시키는 최소한의 코드를 작성합니다. 완벽하지 않아도 됩니다.
3. **Refactor**: 테스트를 유지하면서 코드를 개선합니다. 테스트가 안전망입니다.

**TDD 예제: 할인 계산기**

먼저 테스트를 작성합니다:

```csharp
[Fact]
public void ApplyDiscount_TenPercent_ReducesPrice()
{
    // Arrange
    var calculator = new DiscountCalculator();

    // Act
    var result = calculator.ApplyDiscount(100m, 10);

    // Assert
    Assert.Equal(90m, result);
}
```

이 테스트는 컴파일조차 되지 않습니다. `DiscountCalculator`가 없으니까요. 이제 최소한의 코드를 작성합니다:

```csharp
public class DiscountCalculator
{
    public decimal ApplyDiscount(decimal price, int discountPercent)
    {
        return price * (100 - discountPercent) / 100;
    }
}
```

테스트가 통과합니다! 이제 더 많은 시나리오를 추가하며 반복합니다:

```csharp
[Theory]
[InlineData(100, 10, 90)]
[InlineData(50, 20, 40)]
[InlineData(200, 0, 200)]
[InlineData(100, 100, 0)]
public void ApplyDiscount_VariousInputs_CalculatesCorrectly(
    decimal price, int discount, decimal expected)
{
    var calculator = new DiscountCalculator();
    var result = calculator.ApplyDiscount(price, discount);
    Assert.Equal(expected, result);
}
```

예외 케이스도 테스트합니다:

```csharp
[Fact]
public void ApplyDiscount_NegativePrice_ThrowsException()
{
    var calculator = new DiscountCalculator();

    Assert.Throws<ArgumentException>(() =>
        calculator.ApplyDiscount(-10, 10));
}
```

이 테스트는 실패합니다. 구현을 추가합니다:

```csharp
public decimal ApplyDiscount(decimal price, int discountPercent)
{
    if (price < 0)
        throw new ArgumentException("Price cannot be negative", nameof(price));

    return price * (100 - discountPercent) / 100;
}
```

TDD의 이점:
- **테스트 커버리지가 자연스럽게 100%에 가까워집니다**: 모든 코드가 테스트에서 시작되므로.
- **설계가 개선됩니다**: 테스트 가능한 코드를 작성하려면, 의존성을 명확히 하고 결합을 느슨하게 해야 합니다.
- **자신감**: 모든 기능이 테스트로 검증되므로, 리팩토링이 안전합니다.

### 테스트 커버리지: 수치의 함정

테스트 커버리지는 코드의 몇 퍼센트가 테스트에 의해 실행되는지 측정합니다. Coverlet은 .NET의 대표적인 커버리지 도구입니다.

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

보고서는 어떤 라인이 실행되었고, 어떤 분기가 테스트되지 않았는지 보여줍니다. 하지만 주의하세요: **100% 커버리지가 버그 없음을 보장하지는 않습니다**. 커버리지는 코드가 실행되었음을 의미할 뿐, 올바르게 동작한다는 것은 아닙니다.

```csharp
// 이 코드는 100% 커버리지를 가질 수 있지만, 테스트는 의미 없음
public int Add(int a, int b)
{
    return a - b; // 버그!
}

[Fact]
public void Add_Test()
{
    var result = Add(5, 3);
    // Assert가 없음! 그냥 실행만 됨
}
```

커버리지 목표는 80-90%가 합리적입니다. 100%를 추구하면, 의미 없는 테스트를 작성하게 되는 경향이 있습니다(getter/setter 테스트 등).

### 비동기 테스트: async/await 올바르게 다루기

ASP.NET Core는 본질적으로 비동기적입니다. 대부분의 데이터베이스 호출, HTTP 요청, I/O 작업은 `async/await`를 사용합니다. 비동기 코드를 테스트할 때는 몇 가지 주의사항이 있습니다.

**기본 비동기 테스트**

xUnit은 비동기 테스트를 완벽하게 지원합니다. 테스트 메서드를 `async Task`로 선언하면 됩니다:

```csharp
[Fact]
public async Task GetProductAsync_ExistingId_ReturnsProduct()
{
    // Arrange
    var repository = new ProductRepository(_dbContext);

    // Act
    var product = await repository.GetByIdAsync(1);

    // Assert
    Assert.NotNull(product);
    Assert.Equal("Book", product.Name);
}
```

**async void는 절대 사용하지 마세요**

테스트 메서드는 반드시 `Task`를 반환해야 합니다. `async void`는 xUnit이 테스트 완료를 감지할 수 없어 불안정한 결과를 초래합니다:

```csharp
// ❌ 절대 하지 마세요
[Fact]
public async void BadTest()
{
    await SomeAsyncMethod();
    // 테스트가 완료되기 전에 xUnit이 종료할 수 있음
}

// ✅ 올바른 방법
[Fact]
public async Task GoodTest()
{
    await SomeAsyncMethod();
}
```

**비동기 예외 테스트**

비동기 메서드의 예외를 테스트할 때는 `Assert.ThrowsAsync`를 사용합니다:

```csharp
[Fact]
public async Task DeleteProductAsync_NonExistentId_ThrowsException()
{
    var repository = new ProductRepository(_dbContext);

    // ✅ 비동기 예외 검증
    await Assert.ThrowsAsync<NotFoundException>(
        async () => await repository.DeleteAsync(999)
    );
}

// FluentAssertions 사용 시
[Fact]
public async Task ProcessPaymentAsync_InsufficientFunds_ThrowsException()
{
    var service = new PaymentService();

    Func<Task> act = async () => await service.ProcessPaymentAsync(-100);

    await act.Should().ThrowAsync<InvalidOperationException>()
        .WithMessage("Amount must be positive");
}
```

**Task 반환 메서드 테스트**

`Task<T>`를 반환하지만 `await`하지 않는 메서드를 테스트할 때 주의하세요:

```csharp
// ❌ 잘못된 방법 - Task를 await하지 않음
[Fact]
public void BadAsyncTest()
{
    var task = _repository.GetByIdAsync(1);
    // Task가 완료되지 않은 상태로 테스트 종료
}

// ✅ 올바른 방법
[Fact]
public async Task GoodAsyncTest()
{
    var product = await _repository.GetByIdAsync(1);
    Assert.NotNull(product);
}
```

**병렬 비동기 작업 테스트**

여러 비동기 작업을 병렬로 실행하는 코드를 테스트할 때는 `Task.WhenAll`을 사용합니다:

```csharp
[Fact]
public async Task GetMultipleProducts_ParallelExecution_ReturnsAll()
{
    var repository = new ProductRepository(_dbContext);

    // Act - 3개의 제품을 병렬로 가져오기
    var tasks = new[]
    {
        repository.GetByIdAsync(1),
        repository.GetByIdAsync(2),
        repository.GetByIdAsync(3)
    };

    var products = await Task.WhenAll(tasks);

    // Assert
    Assert.Equal(3, products.Length);
    Assert.All(products, p => Assert.NotNull(p));
}
```

**타임아웃 설정**

무한 대기를 방지하기 위해 타임아웃을 설정할 수 있습니다:

```csharp
[Fact(Timeout = 5000)] // 5초 타임아웃
public async Task SlowOperation_CompletesWithinTimeout()
{
    var service = new DataProcessingService();

    var result = await service.ProcessLargeDatasetAsync();

    Assert.NotNull(result);
}
```

**ConfigureAwait in Tests**

테스트 코드에서는 일반적으로 `ConfigureAwait(false)`를 사용할 필요가 없습니다. 테스트는 SynchronizationContext가 없는 환경에서 실행되므로, 컨텍스트 전환 오버헤드가 없습니다:

```csharp
[Fact]
public async Task TestMethod()
{
    // 테스트에서는 ConfigureAwait(false) 불필요
    var result = await _service.GetDataAsync();
    Assert.NotNull(result);
}
```

### Part 9에서 배울 내용

이제 여러분은 ASP.NET Core 애플리케이션을 테스트하는 모든 방법을 배우게 될 것입니다.

**Chapter 21: 단위 테스트와 통합 테스트**

xUnit으로 단위 테스트를 작성하며, AAA 패턴, Theory, Fixtures를 마스터합니다. Moq로 의존성을 목으로 대체하고, 비즈니스 로직을 격리하여 테스트합니다.

WebApplicationFactory로 통합 테스트를 작성하며, 실제 HTTP 요청과 응답을 검증합니다. 인메모리 데이터베이스를 사용하여 빠른 테스트를 유지하며, 인증된 엔드포인트도 테스트합니다.

bUnit으로 Blazor 컴포넌트를 테스트하며, React Testing Library와 유사한 패턴을 경험합니다. Playwright와 Selenium으로 E2E 테스트를 작성하며, 사용자 시나리오를 자동화합니다.

TDD로 개발하며, Red-Green-Refactor 사이클을 경험합니다. 테스트 커버리지를 측정하며, 의미 있는 테스트를 작성하는 방법을 배웁니다.

실습에서는 전체 주문 시스템을 TDD로 구축하며, 단위 테스트부터 E2E 테스트까지 모든 계층을 커버합니다.

## 학습 목표

Part 9를 마치면 다음을 할 수 있습니다:

- xUnit으로 단위 테스트를 작성하고 AAA 패턴을 적용할 수 있습니다
- Theory와 InlineData로 파라미터화된 테스트를 만듭니다
- Moq로 의존성을 목으로 대체하고 동작을 검증합니다
- WebApplicationFactory로 API 통합 테스트를 작성합니다
- 인메모리 데이터베이스를 사용하여 빠른 데이터 액세스 테스트를 만듭니다
- 인증된 엔드포인트를 테스트할 수 있습니다
- bUnit으로 Blazor 컴포넌트를 테스트합니다
- Playwright나 Selenium으로 E2E 테스트를 작성합니다
- TDD 사이클을 이해하고 적용할 수 있습니다
- 테스트 커버리지를 측정하고 의미 있게 해석합니다
- FluentAssertions로 가독성 높은 검증을 작성합니다

## 챕터 구성

### Chapter 21: 단위 테스트와 통합 테스트

신뢰할 수 있는 코드를 만드는 모든 테스팅 기법을 마스터합니다.

**단위 테스트 기초:**
- xUnit 시작하기
  - `[Fact]`: 단일 테스트
  - `[Theory]`: 파라미터화된 테스트
  - `[InlineData]`, `[MemberData]`, `[ClassData]`
- AAA 패턴: Arrange, Act, Assert
- 테스트 격리와 독립성
- Fixtures와 공유 컨텍스트
  - `IClassFixture<T>`
  - `ICollectionFixture<T>`
- 생명주기와 Setup/Teardown

**Moq을 통한 목 객체:**
- 인터페이스 모킹
  - `Mock<T>` 생성
  - `Setup()`: 동작 정의
  - `Returns()`, `ReturnsAsync()`: 반환 값 설정
- 메서드 호출 검증
  - `Verify()`: 호출 확인
  - `Times.Once`, `Times.Never`, `Times.Exactly(n)`
- 매개변수 매칭
  - `It.IsAny<T>()`: 모든 값
  - `It.Is<T>(predicate)`: 조건부 매칭
  - `It.IsInRange<T>()`: 범위 매칭
- 예외 시뮬레이션
  - `Throws()`, `ThrowsAsync()`
- Callback과 순차적 반환
- Strict vs Loose Mocks
- 모킹 안티패턴과 주의사항

**통합 테스트:**
- WebApplicationFactory 기초
  - 메모리 내 애플리케이션 호스팅
  - `CreateClient()`: HTTP 클라이언트 생성
- 서비스 오버라이드
  - `ConfigureWebHost()`: 구성 변경
  - 테스트용 서비스 등록
- 인메모리 데이터베이스
  - SQLite In-Memory
  - EF Core InMemoryDatabase
  - 데이터 시딩과 초기화
- 인증된 요청 테스트
  - 테스트 인증 핸들러
  - JWT 토큰 생성
  - 사용자 클레임 설정
- API 응답 검증
  - 상태 코드
  - 응답 본문 (JSON)
  - 헤더 검증

**Blazor 컴포넌트 테스트:**
- bUnit 소개
  - React Testing Library와 비교
- 컴포넌트 렌더링
  - `RenderComponent<T>()`
  - 파라미터 전달
- DOM 검증
  - `Find()`, `FindAll()`: CSS 선택자
  - `MarkupMatches()`: HTML 매칭
- 이벤트 시뮬레이션
  - `Click()`, `Input()`, `Submit()`
- 비동기 상태 테스트
  - `WaitForState()`, `WaitForAssertion()`
- 서비스 주입 모킹

**E2E 테스트:**
- Playwright for .NET
  - 브라우저 자동화
  - 페이지 객체 패턴
  - 대기와 동기화
  - 스크린샷과 비디오 녹화
- Selenium WebDriver (대안)
- 안정적인 E2E 테스트 작성
  - Flaky 테스트 방지
  - 명시적 대기 vs 암묵적 대기
  - 재시도 로직

**테스트 주도 개발 (TDD):**
- Red-Green-Refactor 사이클
- TDD의 이점과 한계
- 실전 TDD 예제
- 레거시 코드에 테스트 추가

**테스트 커버리지:**
- Coverlet: 커버리지 도구
- 보고서 생성과 분석
- 커버리지 목표 설정
- 커버리지의 의미와 한계

**핵심 개념**: xUnit, Moq, WebApplicationFactory, bUnit, TDD, 테스트 커버리지

**실습**:
- 비즈니스 로직 단위 테스트 (할인, 재고 관리)
- API 통합 테스트 (주문, 결제)
- Blazor 컴포넌트 테스트 (장바구니 UI)
- E2E 테스트 (사용자 주문 플로우)
- TDD로 새 기능 구현 (쿠폰 시스템)

## 테스팅 모범 사례 체크리스트

Part 9를 학습하며 다음 원칙들을 내재화하세요:

**단위 테스트:**
- [ ] AAA 패턴을 일관되게 사용
- [ ] 테스트 메서드 이름은 명확하고 서술적으로 (`Method_Scenario_ExpectedResult`)
- [ ] 하나의 테스트는 하나의 것만 검증
- [ ] 테스트는 독립적이고 순서에 무관
- [ ] 매직 넘버 대신 명명된 상수 사용
- [ ] 테스트는 빠르게 (밀리초 단위)

**Moq:**
- [ ] 인터페이스만 모킹 (구현 클래스는 모킹하지 않기)
- [ ] 과도한 모킹 피하기 (통합 테스트가 더 적합할 수 있음)
- [ ] `Verify()`로 중요한 상호작용 검증
- [ ] `Setup()`은 필요한 것만 최소한으로

**통합 테스트:**
- [ ] 실제 의존성 vs 테스트 대역 균형
- [ ] 각 테스트는 깨끗한 데이터베이스 상태에서 시작
- [ ] 트랜잭션 롤백이나 데이터베이스 재생성으로 격리
- [ ] API 계약 테스트 (상태 코드, 응답 스키마)

**E2E 테스트:**
- [ ] 핵심 사용자 플로우만 테스트
- [ ] 명시적 대기 사용 (임의의 sleep 피하기)
- [ ] 페이지 객체 패턴으로 유지보수성 향상
- [ ] 실패 시 스크린샷 캡처
- [ ] Flaky 테스트는 즉시 수정하거나 제거

**TDD:**
- [ ] Red (실패) → Green (통과) → Refactor
- [ ] 최소한의 코드로 테스트 통과
- [ ] 리팩토링 시 테스트는 변경하지 않기

**일반:**
- [ ] 커버리지는 수단이지 목표가 아님
- [ ] 의미 있는 Assert 사용
- [ ] FluentAssertions로 가독성 향상
- [ ] CI/CD에 테스트 통합

## 실습 프로젝트

### 종합 실습: TDD로 주문 시스템 구축

완전한 주문 시스템을 TDD로 처음부터 만들며, 모든 테스팅 기법을 적용합니다:

**도메인 로직 (단위 테스트):**
- `Order` Aggregate 테스트
  - 항목 추가/제거
  - 총액 계산
  - 상태 전환 (Pending → Confirmed → Shipped)
  - 도메인 규칙 검증 (최소 주문 금액, 재고 확인)
- `DiscountService` 테스트
  - 쿠폰 적용
  - 다중 할인 규칙
  - 만료 확인

**Application Layer (단위 테스트 + Moq):**
- `CreateOrderCommandHandler` 테스트
  - Repository와 EmailService 모킹
  - 성공 시나리오
  - 실패 시나리오 (재고 부족, 결제 실패)
  - 도메인 이벤트 발행 검증

**API (통합 테스트):**
- `POST /api/orders` 테스트
  - 유효한 주문 생성
  - 인증되지 않은 요청 거부 (401)
  - 유효성 검사 실패 (400)
  - 생성된 주문 반환 (201)
- `GET /api/orders/{id}` 테스트
  - 존재하는 주문 조회
  - 존재하지 않는 주문 (404)
  - 타인의 주문 조회 차단 (403)

**Blazor UI (bUnit):**
- `OrderList` 컴포넌트 테스트
  - 주문 목록 렌더링
  - 로딩 상태 표시
  - 빈 상태 메시지
- `CheckoutForm` 컴포넌트 테스트
  - 폼 제출
  - 유효성 검사 메시지
  - 성공 시 리다이렉트

**E2E (Playwright):**
- 전체 주문 플로우
  - 로그인
  - 제품 검색 및 장바구니 추가
  - 수량 변경
  - 체크아웃
  - 주문 확인 페이지 검증

**커버리지 목표**: 전체 80% 이상, 비즈니스 로직 95% 이상

## 다음 단계

Part 9를 마치면, 여러분은 테스트로 보호되는 신뢰할 수 있는 코드를 작성할 수 있습니다. 단위 테스트, 통합 테스트, E2E 테스트—각각의 역할을 이해하고 적절히 조합하여 견고한 테스트 스위트를 만들 수 있습니다.

**Part 10: 성능 최적화와 모니터링**에서는 애플리케이션의 성능을 측정하고 개선하는 방법을 배웁니다. 프로파일링, 벤치마킹, 캐싱, 비동기 프로그래밍... 빠르고 효율적인 애플리케이션을 만드는 모든 기법을 마스터하게 될 것입니다.

지금 바로 Chapter 21로 이동하여, 첫 단위 테스트를 작성해보세요!

---

## 참고 자료

- [xUnit Documentation](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [Integration Tests in ASP.NET Core](https://docs.microsoft.com/aspnet/core/test/integration-tests)
- [bUnit Documentation](https://bunit.dev/)
- [Playwright for .NET](https://playwright.dev/dotnet/)
- [Test-Driven Development by Example (Kent Beck)](https://www.amazon.com/Test-Driven-Development-Kent-Beck/dp/0321146530)
- [FluentAssertions](https://fluentassertions.com/)
- [Coverlet](https://github.com/coverlet-coverage/coverlet)
- [Testing Best Practices](https://docs.microsoft.com/dotnet/core/testing/unit-testing-best-practices)

**예상 학습 시간**: 2-3주 (실습 포함)
