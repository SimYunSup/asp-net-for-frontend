# Chapter 7: MVC 패턴 - 대규모 애플리케이션을 위한 구조

## Flux/Redux를 아는 개발자를 위한 MVC

React 생태계에서 Redux, Zustand, MobX 같은 상태 관리 라이브러리를 사용해봤다면, 왜 이런 도구가 필요한지 이미 알고 있을 것입니다. 애플리케이션이 커지면서 컴포넌트 간에 props를 무한정 전달하고(props drilling), 어디서 상태가 변경되는지 추적하기 어려워지며, 버그가 발생했을 때 원인을 찾기가 점점 힘들어집니다.

MVC(Model-View-Controller)는 이와 같은 문제를 1970년대부터 해결해온 아키텍처 패턴입니다. Facebook이 Flux 패턴을 발표하기 40년 전부터 말이죠! 물론 MVC와 Flux/Redux는 다른 패턴이지만, 둘 다 같은 목표를 가지고 있습니다: **복잡성을 관리하고, 관심사를 분리하며, 예측 가능한 데이터 흐름을 만드는 것**.

이 챕터에서는 Redux를 아는 개발자를 위해, MVC 패턴을 Flux/Redux와 비교하며 설명합니다. "아, 이건 Redux의 저것과 비슷하구나!"라는 순간이 있을 것이고, "MVC는 이 부분을 다르게 접근하네?"라는 발견도 있을 것입니다.

## MVC의 핵심: 관심사의 분리

### Redux/Flux 아키텍처 (복습)

React 애플리케이션에서 Redux를 사용할 때의 구조를 떠올려보세요:

```
┌─────────────┐
│    View     │  (React 컴포넌트)
│  (UI 렌더링)  │
└─────────────┘
      ↓ dispatch
┌─────────────┐
│   Actions   │  (액션 생성자)
└─────────────┘
      ↓
┌─────────────┐
│  Reducers   │  (상태 업데이트 로직)
└─────────────┘
      ↓
┌─────────────┐
│    Store    │  (전역 상태)
└─────────────┘
      ↓ subscribe
    View로 다시
```

단방향 데이터 흐름: View → Action → Reducer → Store → View

### MVC 아키텍처

```
┌─────────────┐
│    View     │  (Razor 템플릿)
│  (UI 렌더링)  │
└─────────────┘
      ↑ renders
┌─────────────┐
│ Controller  │  (요청 처리, 흐름 제어)
└─────────────┘
      ↕ reads/writes
┌─────────────┐
│    Model    │  (데이터 + 비즈니스 로직)
└─────────────┘
```

**사용자 요청 흐름:**
1. 사용자가 URL 요청 또는 폼 제출
2. **Controller**가 요청을 받아 어떤 액션을 수행할지 결정
3. **Model**을 통해 데이터를 읽거나 쓰기
4. 적절한 **View**를 선택하고 Model 데이터 전달
5. View가 HTML 렌더링하여 사용자에게 응답

### 비교: Redux vs MVC

| 개념 | Redux | MVC |
|------|-------|-----|
| 데이터/상태 | Store | Model |
| 상태 변경 로직 | Reducer | Controller + Model |
| UI 렌더링 | React 컴포넌트 | View (Razor) |
| 사용자 액션 | Action Dispatch | Controller Action |
| 데이터 흐름 | 단방향 (View→Action→Reducer→Store→View) | 요청-응답 (User→Controller→Model→View→User) |

핵심 차이:
- **Redux**: 클라이언트 사이드, 실시간 상태 관리, 불변성 중시
- **MVC**: 서버 사이드, 요청당 생명주기, 각 요청마다 새로운 인스턴스

## Model: 데이터와 비즈니스 로직의 집합소

Redux의 Store + Reducer를 합친 개념과 유사합니다.

### Redux Store/Reducer

```typescript
// Redux: State + Logic
interface ProductState {
  products: Product[];
  loading: boolean;
  error: string | null;
}

const initialState: ProductState = {
  products: [],
  loading: false,
  error: null,
};

function productReducer(state = initialState, action: Action): ProductState {
  switch (action.type) {
    case 'FETCH_PRODUCTS_SUCCESS':
      return { ...state, products: action.payload, loading: false };
    case 'ADD_PRODUCT':
      return { ...state, products: [...state.products, action.payload] };
    default:
      return state;
  }
}
```

### MVC Model

MVC에서는 두 종류의 Model이 있습니다:

**1. 도메인 모델 (Domain Model)**: 데이터베이스 엔티티

```csharp
// Models/Product.cs
public class Product
{
  public int Id { get; set; }

  [Required]
  [StringLength(200)]
  public string Name { get; set; }

  [Required]
  [Column(TypeName = "decimal(18,2)")]
  public decimal Price { get; set; }

  [Required]
  public int Stock { get; set; }

  [StringLength(1000)]
  public string Description { get; set; }

  public int CategoryId { get; set; }
  public Category Category { get; set; }  // 관계

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

**2. 뷰 모델 (ViewModel)**: 뷰를 위한 특화된 모델

```csharp
// ViewModels/ProductViewModel.cs
public class ProductViewModel
{
  public int Id { get; set; }
  public string Name { get; set; }
  public decimal Price { get; set; }
  public string CategoryName { get; set; }
  public bool IsInStock => Stock > 0;
  public int Stock { get; set; }

  // 뷰에 필요한 추가 정보
  public string FormattedPrice => $"₩{Price:N0}";
  public string StockStatus => Stock > 10 ? "충분" : Stock > 0 ? "부족" : "품절";
}
```

ViewModel은 React의 selector나 derived state와 유사합니다:

```typescript
// Redux selector (비슷한 개념)
const selectProductsWithStock = createSelector(
  (state) => state.products,
  (products) => products.map(p => ({
    ...p,
    isInStock: p.stock > 0,
    formattedPrice: `₩${p.price.toLocaleString()}`,
  }))
);
```

## Controller: Redux Action Creator + Thunk의 서버 버전

Controller는 사용자 요청을 받아 처리하는 클래스입니다. Redux의 Action Creator와 Thunk(비동기 액션)를 합친 것과 유사합니다.

### Redux Action Creator + Thunk

```typescript
// Redux: 동기 액션
const addProduct = (product: Product) => ({
  type: 'ADD_PRODUCT',
  payload: product,
});

// Redux: 비동기 액션 (Thunk)
const fetchProducts = () => async (dispatch: Dispatch) => {
  dispatch({ type: 'FETCH_PRODUCTS_REQUEST' });
  try {
    const response = await fetch('/api/products');
    const products = await response.json();
    dispatch({ type: 'FETCH_PRODUCTS_SUCCESS', payload: products });
  } catch (error) {
    dispatch({ type: 'FETCH_PRODUCTS_FAILURE', payload: error.message });
  }
};
```

### MVC Controller

```csharp
// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;

public class ProductsController : Controller
{
  private readonly ApplicationDbContext _db;
  private readonly ILogger<ProductsController> _logger;

  // 의존성 주입
  public ProductsController(ApplicationDbContext db, ILogger<ProductsController> logger)
  {
    _db = db;
    _logger = logger;
  }

  // GET: /Products
  public async Task<IActionResult> Index()
  {
    // 데이터 조회 (Model)
    var products = await _db.Products
      .Include(p => p.Category)
      .ToListAsync();

    // ViewModel로 변환
    var viewModel = products.Select(p => new ProductViewModel
    {
      Id = p.Id,
      Name = p.Name,
      Price = p.Price,
      CategoryName = p.Category.Name,
      Stock = p.Stock
    }).ToList();

    // View에 데이터 전달
    return View(viewModel);
  }

  // GET: /Products/Details/5
  public async Task<IActionResult> Details(int id)
  {
    var product = await _db.Products
      .Include(p => p.Category)
      .FirstOrDefaultAsync(p => p.Id == id);

    if (product == null)
    {
      return NotFound();  // 404 응답
    }

    return View(product);
  }

  // GET: /Products/Create
  public IActionResult Create()
  {
    // 카테고리 목록을 ViewBag에 담아 전달
    ViewBag.Categories = _db.Categories.ToList();
    return View();
  }

  // POST: /Products/Create
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(Product product)
  {
    if (!ModelState.IsValid)
    {
      ViewBag.Categories = _db.Categories.ToList();
      return View(product);  // 유효성 검사 실패 시 폼 다시 표시
    }

    _db.Products.Add(product);
    await _db.SaveChangesAsync();

    _logger.LogInformation($"상품 생성됨: {product.Name}");

    TempData["Success"] = "상품이 성공적으로 등록되었습니다!";
    return RedirectToAction(nameof(Index));  // PRG 패턴
  }

  // POST: /Products/Delete/5
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    var product = await _db.Products.FindAsync(id);
    if (product != null)
    {
      _db.Products.Remove(product);
      await _db.SaveChangesAsync();
      TempData["Success"] = "상품이 삭제되었습니다.";
    }

    return RedirectToAction(nameof(Index));
  }
}
```

**Controller의 역할:**
1. **라우팅**: URL을 액션 메서드로 매핑
2. **입력 검증**: ModelState로 자동 유효성 검사
3. **비즈니스 로직 호출**: 서비스나 Repository 사용
4. **뷰 선택**: 어떤 View를 렌더링할지 결정
5. **데이터 전달**: ViewModel을 View에 전달

## View: React 컴포넌트의 서버 버전

View는 Controller가 전달한 데이터를 받아 HTML을 렌더링합니다.

### React 컴포넌트

```tsx
// ProductList.tsx
interface ProductListProps {
  products: Product[];
}

const ProductList: React.FC<ProductListProps> = ({ products }) => {
  return (
    <div>
      <h1>상품 목록</h1>
      {products.map(product => (
        <div key={product.id}>
          <h3>{product.name}</h3>
          <p>{product.price}원</p>
          <Link to={`/products/${product.id}`}>상세보기</Link>
        </div>
      ))}
    </div>
  );
};
```

### MVC View

```razor
@* Views/Products/Index.cshtml *@
@model List<ProductViewModel>

<div>
  <h1>상품 목록</h1>

  <div class="mb-3">
    <a asp-action="Create" class="btn btn-primary">새 상품 등록</a>
  </div>

  @if (!Model.Any())
  {
    <p>등록된 상품이 없습니다.</p>
  }
  else
  {
    <div class="row">
      @foreach (var product in Model)
      {
        <div class="col-md-4 mb-3">
          <div class="card">
            <div class="card-body">
              <h3 class="card-title">@product.Name</h3>
              <p class="card-text">@product.FormattedPrice</p>
              <p class="text-muted">@product.CategoryName</p>
              <p>
                <span class="badge @(product.IsInStock ? "bg-success" : "bg-danger")">
                  @product.StockStatus
                </span>
              </p>
              <a asp-action="Details" asp-route-id="@product.Id" class="btn btn-primary">
                상세보기
              </a>
              <a asp-action="Edit" asp-route-id="@product.Id" class="btn btn-secondary">
                수정
              </a>
            </div>
          </div>
        </div>
      }
    </div>
  }
</div>
```

**강타입 뷰의 장점:**
- `@model List<ProductViewModel>`: 뷰가 받는 데이터 타입 명시
- IntelliSense/자동완성: `@product.` 입력 시 모든 프로퍼티 표시
- 컴파일 타임 체크: 잘못된 프로퍼티 접근 시 컴파일 오류

## 라우팅: URL을 액션 메서드로 매핑

### 컨벤션 기반 라우팅 (전통적 방식)

```csharp
// Program.cs
app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}"
);
```

이 패턴은 다음과 같이 해석됩니다:
- `/` → `HomeController.Index()`
- `/Products` → `ProductsController.Index()`
- `/Products/Details/5` → `ProductsController.Details(5)`
- `/Admin/Users/Edit/10` → `Admin.UsersController.Edit(10)`

React Router와 비교:
```tsx
// React Router
<Routes>
  <Route path="/" element={<Home />} />
  <Route path="/products" element={<ProductList />} />
  <Route path="/products/:id" element={<ProductDetails />} />
</Routes>
```

### 특성 기반 라우팅 (현대적 방식)

```csharp
[Route("products")]
public class ProductsController : Controller
{
  // GET: /products
  [HttpGet]
  public async Task<IActionResult> Index()
  {
    // ...
  }

  // GET: /products/5
  [HttpGet("{id}")]
  public async Task<IActionResult> Details(int id)
  {
    // ...
  }

  // POST: /products
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create([FromBody] Product product)
  {
    // ...
  }

  // PUT: /products/5
  [HttpPut("{id}")]
  public async Task<IActionResult> Update(int id, [FromBody] Product product)
  {
    // ...
  }

  // DELETE: /products/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    // ...
  }
}
```

RESTful API와 완벽하게 매칭되며, HTTP 메서드를 명시적으로 지정합니다.

## 필터 (Filters): React HOC/미들웨어의 서버 버전

Redux 미들웨어나 React의 HOC(Higher-Order Components)처럼, 필터는 횡단 관심사(cross-cutting concerns)를 처리합니다.

### Redux 미들웨어

```typescript
// Redux 로깅 미들웨어
const loggerMiddleware: Middleware = store => next => action => {
  console.log('Dispatching:', action);
  const result = next(action);
  console.log('Next state:', store.getState());
  return result;
};
```

### MVC 필터

ASP.NET Core MVC는 5가지 필터 타입을 제공합니다:

#### 1. Authorization 필터: 인증/권한 검사

```csharp
// 커스텀 권한 필터
public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
{
  public void OnAuthorization(AuthorizationFilterContext context)
  {
    var user = context.HttpContext.User;

    if (!user.Identity.IsAuthenticated)
    {
      context.Result = new RedirectToActionResult("Login", "Account", null);
      return;
    }

    if (!user.IsInRole("Admin"))
    {
      context.Result = new ForbidResult();
    }
  }
}

// 사용
[AdminOnly]
public class AdminController : Controller
{
  public IActionResult Dashboard()
  {
    return View();
  }
}
```

React에서는:
```tsx
// React HOC
const withAuth = (allowedRoles: string[]) => (Component: React.FC) => {
  return (props) => {
    const { user } = useAuth();
    if (!user) return <Navigate to="/login" />;
    if (!allowedRoles.includes(user.role)) return <Forbidden />;
    return <Component {...props} />;
  };
};

const AdminDashboard = withAuth(['admin'])(Dashboard);
```

#### 2. Action 필터: 액션 실행 전후에 로직 추가

```csharp
public class LogActionFilter : IActionFilter
{
  private readonly ILogger<LogActionFilter> _logger;

  public LogActionFilter(ILogger<LogActionFilter> logger)
  {
    _logger = logger;
  }

  public void OnActionExecuting(ActionExecutingContext context)
  {
    _logger.LogInformation($"액션 실행 시작: {context.ActionDescriptor.DisplayName}");
  }

  public void OnActionExecuted(ActionExecutedContext context)
  {
    _logger.LogInformation($"액션 실행 완료: {context.ActionDescriptor.DisplayName}");
  }
}

// 전역 적용
builder.Services.AddControllersWithViews(options =>
{
  options.Filters.Add<LogActionFilter>();
});

// 또는 특정 액션/컨트롤러에만 적용
[ServiceFilter(typeof(LogActionFilter))]
public IActionResult Index()
{
  return View();
}
```

#### 3. Result 필터: 뷰 렌더링 전후에 로직 추가

```csharp
public class AddHeaderAttribute : ResultFilterAttribute
{
  public override void OnResultExecuting(ResultExecutingContext context)
  {
    context.HttpContext.Response.Headers.Add("X-Custom-Header", "MyValue");
    base.OnResultExecuting(context);
  }
}

[AddHeader]
public IActionResult About()
{
  return View();
}
```

#### 4. Exception 필터: 예외 처리

```csharp
public class CustomExceptionFilter : IExceptionFilter
{
  private readonly ILogger<CustomExceptionFilter> _logger;

  public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
  {
    _logger = logger;
  }

  public void OnException(ExceptionContext context)
  {
    _logger.LogError(context.Exception, "처리되지 않은 예외 발생");

    if (context.Exception is NotFoundException)
    {
      context.Result = new NotFoundObjectResult(new
      {
        error = "리소스를 찾을 수 없습니다",
        message = context.Exception.Message
      });
      context.ExceptionHandled = true;
    }
    else if (context.Exception is UnauthorizedAccessException)
    {
      context.Result = new UnauthorizedResult();
      context.ExceptionHandled = true;
    }
  }
}
```

React의 Error Boundary와 유사:
```tsx
class ErrorBoundary extends React.Component {
  componentDidCatch(error, errorInfo) {
    logError(error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      return <ErrorPage />;
    }
    return this.props.children;
  }
}
```

#### 5. Resource 필터: 성능 최적화 (캐싱 등)

```csharp
public class CacheResourceFilter : IResourceFilter
{
  private readonly IMemoryCache _cache;

  public CacheResourceFilter(IMemoryCache cache)
  {
    _cache = cache;
  }

  public void OnResourceExecuting(ResourceExecutingContext context)
  {
    var cacheKey = context.HttpContext.Request.Path.ToString();

    if (_cache.TryGetValue(cacheKey, out var cachedResult))
    {
      context.Result = cachedResult as IActionResult;
    }
  }

  public void OnResourceExecuted(ResourceExecutedContext context)
  {
    if (context.Result is ViewResult viewResult)
    {
      var cacheKey = context.HttpContext.Request.Path.ToString();
      _cache.Set(cacheKey, context.Result, TimeSpan.FromMinutes(10));
    }
  }
}
```

### 필터 실행 순서

```
Authorization Filter
    ↓
Resource Filter (Before)
    ↓
Action Filter (Before)
    ↓
[액션 메서드 실행]
    ↓
Action Filter (After)
    ↓
Exception Filter (예외 발생 시)
    ↓
Result Filter (Before)
    ↓
[뷰 렌더링]
    ↓
Result Filter (After)
    ↓
Resource Filter (After)
```

## Areas: 애플리케이션 모듈화

대규모 애플리케이션을 논리적 모듈로 분리하는 기능입니다. React의 폴더 구조 기반 모듈화와 유사하지만, 라우팅 수준에서 지원됩니다.

### 프로젝트 구조

```
Areas/
  Admin/
    Controllers/
      ProductsController.cs
      UsersController.cs
    Views/
      Products/
        Index.cshtml
        Edit.cshtml
      Users/
        Index.cshtml
    Models/
      AdminViewModel.cs

  Customer/
    Controllers/
      OrdersController.cs
    Views/
      Orders/
        Index.cshtml
        Details.cshtml

Controllers/  (메인 영역)
  HomeController.cs
  AccountController.cs

Views/
  Home/
    Index.cshtml
  Shared/
    _Layout.cshtml
```

### Area 컨트롤러 정의

```csharp
// Areas/Admin/Controllers/ProductsController.cs
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
  // GET: /Admin/Products
  public IActionResult Index()
  {
    return View();
  }

  // GET: /Admin/Products/Edit/5
  public IActionResult Edit(int id)
  {
    return View();
  }
}
```

### Area 라우팅 설정

```csharp
// Program.cs
app.MapControllerRoute(
  name: "areas",
  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}"
);
```

URL 구조:
- `/` → 메인 영역 `HomeController.Index()`
- `/Admin/Products` → Admin 영역 `ProductsController.Index()`
- `/Customer/Orders` → Customer 영역 `OrdersController.Index()`

### Area 간 링크

```razor
@* 같은 Area 내 링크 *@
<a asp-action="Index" asp-controller="Products">상품 관리</a>

@* 다른 Area로 링크 *@
<a asp-area="Admin" asp-action="Dashboard" asp-controller="Home">
  관리자 대시보드
</a>

@* 메인 영역으로 링크 *@
<a asp-area="" asp-action="Index" asp-controller="Home">홈</a>
```

React에서는 폴더로 구분:
```
src/
  features/
    admin/
      products/
        ProductList.tsx
      users/
        UserList.tsx
    customer/
      orders/
        OrderList.tsx
```

## 실습: 전자상거래 MVC 애플리케이션

배운 내용을 종합하여 전자상거래 시스템을 만들어봅시다.

### 1. 도메인 모델

```csharp
// Models/Product.cs
public class Product
{
  public int Id { get; set; }
  [Required] public string Name { get; set; }
  [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }
  public int Stock { get; set; }
  public int CategoryId { get; set; }
  public Category Category { get; set; }
}

// Models/Category.cs
public class Category
{
  public int Id { get; set; }
  [Required] public string Name { get; set; }
  public ICollection<Product> Products { get; set; }
}

// Models/Cart.cs
public class Cart
{
  public int Id { get; set; }
  public string UserId { get; set; }
  public List<CartItem> Items { get; set; } = new();

  public decimal Total => Items.Sum(i => i.Subtotal);
}

public class CartItem
{
  public int Id { get; set; }
  public int CartId { get; set; }
  public int ProductId { get; set; }
  public Product Product { get; set; }
  public int Quantity { get; set; }
  public decimal Subtotal => Product.Price * Quantity;
}

// Models/Order.cs
public class Order
{
  public int Id { get; set; }
  public string UserId { get; set; }
  public DateTime OrderDate { get; set; } = DateTime.UtcNow;
  public OrderStatus Status { get; set; }
  public List<OrderItem> Items { get; set; }
  public decimal Total => Items.Sum(i => i.Subtotal);
}

public enum OrderStatus
{
  Pending,
  Confirmed,
  Shipped,
  Delivered,
  Cancelled
}
```

### 2. 상품 카탈로그 컨트롤러

```csharp
// Controllers/ProductsController.cs
public class ProductsController : Controller
{
  private readonly ApplicationDbContext _db;

  public ProductsController(ApplicationDbContext db)
  {
    _db = db;
  }

  // GET: /Products?categoryId=1&search=laptop
  public async Task<IActionResult> Index(int? categoryId, string search)
  {
    var query = _db.Products.Include(p => p.Category).AsQueryable();

    if (categoryId.HasValue)
    {
      query = query.Where(p => p.CategoryId == categoryId.Value);
    }

    if (!string.IsNullOrWhiteSpace(search))
    {
      query = query.Where(p => p.Name.Contains(search) || p.Category.Name.Contains(search));
    }

    var products = await query.ToListAsync();
    ViewBag.Categories = await _db.Categories.ToListAsync();
    ViewBag.SelectedCategory = categoryId;
    ViewBag.SearchQuery = search;

    return View(products);
  }

  // GET: /Products/Details/5
  public async Task<IActionResult> Details(int id)
  {
    var product = await _db.Products
      .Include(p => p.Category)
      .FirstOrDefaultAsync(p => p.Id == id);

    if (product == null)
    {
      return NotFound();
    }

    return View(product);
  }
}
```

### 3. 상품 목록 뷰

```razor
@* Views/Products/Index.cshtml *@
@model List<Product>

<h1>상품 카탈로그</h1>

<div class="row mb-3">
  <div class="col-md-8">
    <form method="get" class="row g-2">
      <div class="col-auto">
        <select name="categoryId" class="form-select" onchange="this.form.submit()">
          <option value="">전체 카테고리</option>
          @foreach (var category in (List<Category>)ViewBag.Categories)
          {
            <option value="@category.Id" selected="@(ViewBag.SelectedCategory == category.Id)">
              @category.Name
            </option>
          }
        </select>
      </div>
      <div class="col">
        <input type="text" name="search" value="@ViewBag.SearchQuery"
               class="form-control" placeholder="상품 검색..." />
      </div>
      <div class="col-auto">
        <button type="submit" class="btn btn-primary">검색</button>
      </div>
    </form>
  </div>
</div>

@if (!Model.Any())
{
  <div class="alert alert-info">검색 결과가 없습니다.</div>
}
else
{
  <div class="row">
    @foreach (var product in Model)
    {
      <div class="col-md-4 mb-4">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">@product.Name</h5>
            <p class="text-muted">@product.Category.Name</p>
            <p class="h4 text-primary">₩@product.Price.ToString("N0")</p>
            <p class="@(product.Stock > 0 ? "text-success" : "text-danger")">
              @(product.Stock > 0 ? $"재고: {product.Stock}개" : "품절")
            </p>
          </div>
          <div class="card-footer">
            <a asp-action="Details" asp-route-id="@product.Id"
               class="btn btn-sm btn-primary">상세보기</a>
            @if (product.Stock > 0)
            {
              <form method="post" asp-controller="Cart" asp-action="Add" class="d-inline">
                <input type="hidden" name="productId" value="@product.Id" />
                <button type="submit" class="btn btn-sm btn-success">장바구니에 추가</button>
              </form>
            }
          </div>
        </div>
      </div>
    }
  </div>
}
```

### 4. 장바구니 컨트롤러 (세션 기반)

```csharp
// Controllers/CartController.cs
public class CartController : Controller
{
  private readonly ApplicationDbContext _db;

  public CartController(ApplicationDbContext db)
  {
    _db = db;
  }

  // GET: /Cart
  public async Task<IActionResult> Index()
  {
    var cart = await GetCartAsync();
    return View(cart);
  }

  // POST: /Cart/Add
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Add(int productId)
  {
    var product = await _db.Products.FindAsync(productId);
    if (product == null || product.Stock == 0)
    {
      TempData["Error"] = "상품을 찾을 수 없거나 품절입니다.";
      return RedirectToAction("Index", "Products");
    }

    var cart = await GetCartAsync();
    var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

    if (existingItem != null)
    {
      existingItem.Quantity++;
    }
    else
    {
      cart.Items.Add(new CartItem
      {
        ProductId = productId,
        Product = product,
        Quantity = 1
      });
    }

    await SaveCartAsync(cart);

    TempData["Success"] = "장바구니에 추가되었습니다.";
    return RedirectToAction("Index", "Products");
  }

  // POST: /Cart/UpdateQuantity
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
  {
    if (quantity < 1)
    {
      return await Remove(productId);
    }

    var cart = await GetCartAsync();
    var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

    if (item != null)
    {
      item.Quantity = quantity;
      await SaveCartAsync(cart);
    }

    return RedirectToAction(nameof(Index));
  }

  // POST: /Cart/Remove
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Remove(int productId)
  {
    var cart = await GetCartAsync();
    cart.Items.RemoveAll(i => i.ProductId == productId);
    await SaveCartAsync(cart);

    return RedirectToAction(nameof(Index));
  }

  // 세션에서 장바구니 가져오기
  private async Task<Cart> GetCartAsync()
  {
    var cartJson = HttpContext.Session.GetString("Cart");

    if (string.IsNullOrEmpty(cartJson))
    {
      return new Cart();
    }

    var cart = JsonSerializer.Deserialize<Cart>(cartJson);

    // Product 정보를 DB에서 새로 로드 (세션에는 ID만 저장)
    foreach (var item in cart.Items)
    {
      item.Product = await _db.Products.FindAsync(item.ProductId);
    }

    return cart;
  }

  private async Task SaveCartAsync(Cart cart)
  {
    // Product 객체는 제외하고 직렬화 (순환 참조 방지)
    var simplifiedCart = new Cart
    {
      Items = cart.Items.Select(i => new CartItem
      {
        ProductId = i.ProductId,
        Quantity = i.Quantity
      }).ToList()
    };

    var cartJson = JsonSerializer.Serialize(simplifiedCart);
    HttpContext.Session.SetString("Cart", cartJson);
  }
}
```

### 5. 관리자 영역 (Areas 사용)

```csharp
// Areas/Admin/Controllers/ProductsController.cs
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
  private readonly ApplicationDbContext _db;

  public ProductsController(ApplicationDbContext db)
  {
    _db = db;
  }

  // GET: /Admin/Products
  public async Task<IActionResult> Index()
  {
    var products = await _db.Products.Include(p => p.Category).ToListAsync();
    return View(products);
  }

  // GET: /Admin/Products/Create
  public IActionResult Create()
  {
    ViewBag.Categories = _db.Categories.ToList();
    return View();
  }

  // POST: /Admin/Products/Create
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(Product product)
  {
    if (!ModelState.IsValid)
    {
      ViewBag.Categories = _db.Categories.ToList();
      return View(product);
    }

    _db.Products.Add(product);
    await _db.SaveChangesAsync();

    TempData["Success"] = "상품이 등록되었습니다.";
    return RedirectToAction(nameof(Index));
  }

  // Similar Edit, Delete actions...
}
```

## Redux vs MVC: 언제 무엇을 사용할까?

| 시나리오 | Redux (SPA) | MVC (서버 사이드) |
|----------|-------------|-------------------|
| 복잡한 클라이언트 상태 | ✅ 최적 | ❌ 적합하지 않음 |
| 실시간 UI 업데이트 | ✅ 최적 | ❌ 적합하지 않음 |
| SEO가 중요한 사이트 | ⚠️ SSR 필요 | ✅ 기본 지원 |
| 간단한 CRUD | ⚠️ 과할 수 있음 | ✅ 최적 |
| 오프라인 지원 | ✅ 최적 | ❌ 불가능 |
| 팀 규모가 큼 | ✅ 명확한 구조 | ✅ 명확한 구조 |
| 빠른 프로토타입 | ❌ 설정 복잡 | ✅ 빠름 |

## 다음 단계

MVC 패턴으로 대규모 애플리케이션을 구조화하는 방법을 배웠습니다. 이제 여러분은:
- 관심사를 명확히 분리할 수 있습니다 (Model, View, Controller)
- 필터로 횡단 관심사를 처리할 수 있습니다
- Areas로 애플리케이션을 모듈화할 수 있습니다
- RESTful 라우팅과 강타입 뷰를 활용할 수 있습니다

**Part 4: Blazor**에서는 C#으로 프론트엔드를 작성하는 혁명적인 방법을 배웁니다. MVC가 서버에서 HTML을 렌더링한다면, Blazor는 브라우저에서 C#을 실행합니다. React의 컴포넌트 모델과 C#의 타입 안전성을 결합한 Blazor는, 진정한 풀스택 C# 개발을 가능하게 합니다.

---

## 참고 자료

- [ASP.NET Core MVC 공식 문서](https://docs.microsoft.com/aspnet/core/mvc/)
- [컨트롤러 가이드](https://docs.microsoft.com/aspnet/core/mvc/controllers/actions)
- [필터 상세 가이드](https://docs.microsoft.com/aspnet/core/mvc/controllers/filters)
- [Areas 가이드](https://docs.microsoft.com/aspnet/core/mvc/controllers/areas)
- [라우팅 가이드](https://docs.microsoft.com/aspnet/core/mvc/controllers/routing)

다음 파트: [Part 4: Blazor - C#으로 작성하는 프론트엔드](../../part4-blazor/README.md)
