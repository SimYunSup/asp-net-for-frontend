---
title: "Chapter 7 - Razor Pages - 단순함의 힘"
---

# Chapter 7: Razor Pages - 단순함의 힘

## Next.js Pages Router의 서버 버전

Next.js를 사용해본 적이 있다면, Pages Router의 우아함을 이미 경험했을 것입니다. `pages/` 폴더에 파일을 만들면 자동으로 라우트가 생성되고, `getServerSideProps`나 `getStaticProps`로 데이터를 페칭하는 그 방식 말이죠. 파일 시스템이 곧 라우팅이며, 각 페이지는 자체적인 데이터 로직을 가집니다.

Razor Pages는 이 개념을 2016년에 이미 구현했습니다—Next.js가 등장하기 1년 전입니다! 놀랍게도 두 프레임워크는 거의 같은 문제를 거의 같은 방식으로 해결했습니다. 차이점은 Razor Pages는 서버에서만 실행되며, C#의 강력한 타입 시스템을 활용한다는 것입니다.

이 챕터에서는 Next.js Pages Router를 아는 개발자를 위해, Razor Pages를 빠르게 익힐 수 있도록 직접 비교하며 설명합니다. "아, Next.js의 그것과 같구나!"라는 순간이 반복될 것입니다.

## 파일 기반 라우팅: 폴더 구조가 곧 URL

가장 먼저 눈에 띄는 유사점은 파일 기반 라우팅입니다.

### Next.js Pages Router

```
pages/
  index.tsx           → /
  about.tsx           → /about
  products/
    index.tsx         → /products
    [id].tsx          → /products/123
  blog/
    [slug].tsx        → /blog/my-post
```

### Razor Pages

```
Pages/
  Index.cshtml        → /
  About.cshtml        → /about
  Products/
    Index.cshtml      → /products
    Details.cshtml    → /products/details?id=123
  Blog/
    Post.cshtml       → /blog/post?slug=my-post
```

핵심 차이점:
1. **확장자**: `.tsx` → `.cshtml`
2. **동적 라우트**: Next.js는 `[param]` 문법, Razor Pages는 쿼리 스트링 또는 라우트 템플릿 사용

Razor Pages의 동적 라우트 템플릿을 사용하면 Next.js와 더 유사해집니다:

```razor
@* Pages/Products/Details.cshtml *@
@page "/products/{id:int}"
@model ProductDetailsModel

<h1>상품 #@Model.ProductId</h1>
```

이제 `/products/123`처럼 깔끔한 URL을 사용할 수 있습니다.

## PageModel: getServerSideProps의 C# 버전

Next.js에서 페이지 컴포넌트는 UI를 렌더링하고, `getServerSideProps`는 서버에서 데이터를 가져옵니다.

### Next.js 패턴

```tsx
// pages/products/[id].tsx
interface ProductPageProps {
  product: Product;
}

export default function ProductPage({ product }: ProductPageProps) {
  return (
    <div>
      <h1>{product.name}</h1>
      <p>{product.description}</p>
      <p>가격: {product.price}원</p>
    </div>
  );
}

export async function getServerSideProps(context: GetServerSidePropsContext) {
  const { id } = context.params!;
  const res = await fetch(`https://api.example.com/products/${id}`);
  const product = await res.json();

  return {
    props: {
      product,
    },
  };
}
```

### Razor Pages 패턴

```csharp
// Pages/Products/Details.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Pages.Products
{
  public class DetailsModel : PageModel
  {
    private readonly IProductService _productService;

    public DetailsModel(IProductService productService)
    {
      _productService = productService;
    }

    public Product Product { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
      Product = await _productService.GetProductByIdAsync(id);

      if (Product == null)
      {
        return NotFound();
      }

      return Page();
    }
  }
}
```

```razor
@* Pages/Products/Details.cshtml *@
@page
@model DetailsModel

<div>
  <h1>@Model.Product.Name</h1>
  <p>@Model.Product.Description</p>
  <p>가격: @Model.Product.Price원</p>
</div>
```

**비교:**

| Next.js | Razor Pages |
|---------|-------------|
| `getServerSideProps` | `OnGetAsync` 메서드 |
| 반환값의 `props` 객체 | PageModel의 public 프로퍼티 |
| 컴포넌트 props로 접근 | Razor 뷰에서 `@Model.Property`로 접근 |
| `notFound: true` 반환 | `return NotFound()` |
| `redirect: { ... }` 반환 | `return RedirectToPage(...)` |

핵심 차이는 Next.js는 함수형이고 Razor Pages는 객체 지향적이라는 것입니다. `OnGetAsync`는 클래스의 메서드이며, 의존성 주입을 생성자로 받습니다.

## 폼 처리: POST 요청과 데이터 바인딩

Next.js에서 폼을 처리하려면 API 라우트를 만들거나, 서버 액션을 사용합니다. Razor Pages는 같은 페이지에서 GET과 POST를 모두 처리할 수 있습니다.

### Next.js 패턴

```tsx
// pages/contact.tsx
import { useState } from 'react';

export default function ContactPage() {
  const [formData, setFormData] = useState({ name: '', email: '', message: '' });
  const [errors, setErrors] = useState({});

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const res = await fetch('/api/contact', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData),
    });

    if (res.ok) {
      alert('메시지가 전송되었습니다!');
    } else {
      const errors = await res.json();
      setErrors(errors);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label htmlFor="name">이름:</label>
        <input
          id="name"
          value={formData.name}
          onChange={e => setFormData({...formData, name: e.target.value})}
        />
        {errors.name && <span>{errors.name}</span>}
      </div>
      {/* email, message 필드 생략 */}
      <button type="submit">전송</button>
    </form>
  );
}
```

```typescript
// pages/api/contact.ts
export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  if (req.method === 'POST') {
    const { name, email, message } = req.body;

    // 유효성 검사
    const errors = {};
    if (!name) errors.name = '이름은 필수입니다';
    if (!email) errors.email = '이메일은 필수입니다';

    if (Object.keys(errors).length > 0) {
      return res.status(400).json(errors);
    }

    // 저장 로직...
    return res.status(200).json({ success: true });
  }
}
```

두 파일(페이지 + API 라우트), 클라이언트 상태 관리, fetch 호출, 수동 유효성 검사...

### Razor Pages 패턴

```csharp
// Pages/Contact.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Pages
{
  public class ContactModel : PageModel
  {
    private readonly IEmailService _emailService;

    public ContactModel(IEmailService emailService)
    {
      _emailService = emailService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
      [Required(ErrorMessage = "이름은 필수입니다")]
      public string Name { get; set; }

      [Required(ErrorMessage = "이메일은 필수입니다")]
      [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다")]
      public string Email { get; set; }

      [Required(ErrorMessage = "메시지는 필수입니다")]
      [StringLength(1000, ErrorMessage = "메시지는 1000자를 넘을 수 없습니다")]
      public string Message { get; set; }
    }

    public void OnGet()
    {
      // GET 요청 처리 (페이지 로드)
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        return Page();  // 유효성 검사 실패 시 폼 다시 표시
      }

      await _emailService.SendContactEmailAsync(Input.Name, Input.Email, Input.Message);

      TempData["Message"] = "메시지가 전송되었습니다!";
      return RedirectToPage();  // PRG 패턴
    }
  }
}
```

```razor
@* Pages/Contact.cshtml *@
@page
@model ContactModel

<h1>문의하기</h1>

@if (TempData["Message"] != null)
{
  <div class="alert alert-success">@TempData["Message"]</div>
}

<form method="post">
  <div class="form-group">
    <label asp-for="Input.Name"></label>
    <input asp-for="Input.Name" class="form-control" />
    <span asp-validation-for="Input.Name" class="text-danger"></span>
  </div>

  <div class="form-group">
    <label asp-for="Input.Email"></label>
    <input asp-for="Input.Email" class="form-control" />
    <span asp-validation-for="Input.Email" class="text-danger"></span>
  </div>

  <div class="form-group">
    <label asp-for="Input.Message"></label>
    <textarea asp-for="Input.Message" class="form-control" rows="5"></textarea>
    <span asp-validation-for="Input.Message" class="text-danger"></span>
  </div>

  <button type="submit" class="btn btn-primary">전송</button>
</form>

@section Scripts {
  <partial name="_ValidationScriptsPartial" />
}
```

**Razor Pages의 장점:**

1. **한 파일에 모든 것**: API 라우트가 필요 없음
2. **자동 모델 바인딩**: `[BindProperty]`만 추가하면 폼 데이터가 자동으로 객체로 변환
3. **선언적 유효성 검사**: 데이터 어노테이션으로 규칙 정의, 자동 검증
4. **PRG 패턴**: Post/Redirect/Get으로 새로고침 시 중복 제출 방지
5. **클라이언트 상태 불필요**: 서버가 모든 것을 처리
6. **Tag Helpers**: `asp-for`가 자동으로 label, input, validation 연결

## 핸들러 메서드: 다양한 액션 처리

Razor Pages는 하나의 페이지에서 여러 액션을 처리할 수 있습니다.

### 기본 핸들러: OnGet, OnPost

```csharp
public class ProductsModel : PageModel
{
  // GET /products
  public void OnGet()
  {
    // 페이지 로드 시 실행
  }

  // POST /products
  public IActionResult OnPost()
  {
    // 폼 제출 시 실행
  }
}
```

### 명명된 핸들러: 여러 버튼/액션

```razor
@* Edit.cshtml *@
<form method="post">
  <input asp-for="Product.Name" />
  <button type="submit" asp-page-handler="Save">저장</button>
  <button type="submit" asp-page-handler="Delete">삭제</button>
</form>
```

```csharp
// Edit.cshtml.cs
public async Task<IActionResult> OnPostSaveAsync()
{
  // 저장 로직
  return RedirectToPage("./Index");
}

public async Task<IActionResult> OnPostDeleteAsync()
{
  // 삭제 로직
  return RedirectToPage("./Index");
}
```

URL: `POST /edit?handler=save` 또는 `POST /edit?handler=delete`

Next.js에서는 이를 위해 별도의 API 엔드포인트나 복잡한 로직이 필요하지만, Razor Pages는 핸들러 메서드 하나면 충분합니다.

## 모델 바인딩: 자동 데이터 변환

Razor Pages의 강력한 기능 중 하나는 자동 모델 바인딩입니다. HTTP 요청의 다양한 부분(쿼리 문자열, 라우트 데이터, 폼 데이터, 헤더)을 C# 객체로 자동 변환합니다.

### 쿼리 문자열 바인딩

```csharp
// Pages/Search.cshtml.cs
public class SearchModel : PageModel
{
  [BindProperty(SupportsGet = true)]
  public string Query { get; set; }

  [BindProperty(SupportsGet = true)]
  public int Page { get; set; } = 1;

  public void OnGet()
  {
    // URL: /search?query=laptop&page=2
    // Query = "laptop", Page = 2
  }
}
```

Next.js에서는:
```tsx
export default function SearchPage() {
  const router = useRouter();
  const { query, page } = router.query;  // 문자열, 타입 불안정
  // ...
}
```

Razor Pages는 타입 안전하며, 자동 변환(`string` → `int`)까지 수행합니다.

### 라우트 데이터 바인딩

```razor
@* Pages/Products/Edit.cshtml *@
@page "/products/edit/{id:int}"
@model EditModel
```

```csharp
public class EditModel : PageModel
{
  public async Task OnGetAsync(int id)  // URL에서 자동 추출
  {
    // id는 라우트의 {id} 값
  }
}
```

### 폼 데이터 바인딩

```csharp
public class CreateModel : PageModel
{
  [BindProperty]
  public Product Product { get; set; }

  public async Task<IActionResult> OnPostAsync()
  {
    // 폼의 모든 필드가 Product 객체로 자동 매핑됨
    await _db.Products.AddAsync(Product);
    await _db.SaveChangesAsync();
    return RedirectToPage("./Index");
  }
}
```

```razor
<form method="post">
  <input asp-for="Product.Name" />
  <input asp-for="Product.Price" type="number" />
  <textarea asp-for="Product.Description"></textarea>
  <button type="submit">생성</button>
</form>
```

폼 필드 이름이 `Product.Name`, `Product.Price`로 자동 생성되며, POST 시 `Product` 객체로 자동 바인딩됩니다.

## 유효성 검사: 데이터 어노테이션의 강력함

Next.js에서 유효성 검사를 하려면 Zod, Yup 같은 라이브러리를 사용하거나 수동으로 검증 로직을 작성해야 합니다. Razor Pages는 데이터 어노테이션으로 선언적으로 규칙을 정의하며, 서버와 클라이언트 모두에서 자동 검증됩니다.

### 유효성 검사 규칙 정의

```csharp
public class RegisterModel : PageModel
{
  [BindProperty]
  public InputModel Input { get; set; }

  public class InputModel
  {
    [Required(ErrorMessage = "이메일은 필수입니다")]
    [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다")]
    [Display(Name = "이메일 주소")]
    public string Email { get; set; }

    [Required(ErrorMessage = "비밀번호는 필수입니다")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "비밀번호는 6-100자여야 합니다")]
    [DataType(DataType.Password)]
    [Display(Name = "비밀번호")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "비밀번호 확인")]
    [Compare("Password", ErrorMessage = "비밀번호가 일치하지 않습니다")]
    public string ConfirmPassword { get; set; }

    [Range(18, 120, ErrorMessage = "나이는 18-120 사이여야 합니다")]
    public int Age { get; set; }

    [Url(ErrorMessage = "올바른 URL 형식이 아닙니다")]
    public string Website { get; set; }
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
    {
      return Page();  // 검증 실패 시 에러 메시지와 함께 폼 다시 표시
    }

    // 유효성 검사 통과, 사용자 등록 로직...
    return RedirectToPage("./Success");
  }
}
```

### 뷰에서 검증 메시지 표시

```razor
<form method="post">
  <div asp-validation-summary="ModelOnly" class="text-danger"></div>

  <div class="form-group">
    <label asp-for="Input.Email"></label>
    <input asp-for="Input.Email" class="form-control" />
    <span asp-validation-for="Input.Email" class="text-danger"></span>
  </div>

  <div class="form-group">
    <label asp-for="Input.Password"></label>
    <input asp-for="Input.Password" class="form-control" />
    <span asp-validation-for="Input.Password" class="text-danger"></span>
  </div>

  <div class="form-group">
    <label asp-for="Input.ConfirmPassword"></label>
    <input asp-for="Input.ConfirmPassword" class="form-control" />
    <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>
  </div>

  <button type="submit">회원가입</button>
</form>

@section Scripts {
  <partial name="_ValidationScriptsPartial" />
}
```

**자동으로 제공되는 것:**
- 서버 사이드 검증 (`ModelState.IsValid`)
- 클라이언트 사이드 검증 (jQuery Unobtrusive Validation)
- 필드별 에러 메시지
- 전체 에러 요약
- HTML5 속성 자동 생성 (`required`, `type="email"` 등)

### 커스텀 유효성 검사

```csharp
public class UniqueEmailAttribute : ValidationAttribute
{
  protected override ValidationResult IsValid(object value, ValidationContext validationContext)
  {
    var email = value as string;
    var dbContext = validationContext.GetService<ApplicationDbContext>();

    if (dbContext.Users.Any(u => u.Email == email))
    {
      return new ValidationResult("이미 사용 중인 이메일입니다");
    }

    return ValidationResult.Success;
  }
}

// 사용
public class InputModel
{
  [Required]
  [EmailAddress]
  [UniqueEmail]  // 커스텀 검증
  public string Email { get; set; }
}
```

## 상태 관리: TempData, ViewData, ViewBag

React의 `useState`, `useContext`와 달리, 서버 사이드에서는 다른 방식의 상태 관리가 필요합니다.

### TempData: 리디렉션 간 데이터 전달

**PRG (Post/Redirect/Get) 패턴:**

```csharp
public async Task<IActionResult> OnPostAsync()
{
  if (!ModelState.IsValid)
  {
    return Page();
  }

  await _db.SaveChangesAsync();

  TempData["SuccessMessage"] = "상품이 성공적으로 생성되었습니다!";
  return RedirectToPage("./Index");
}
```

```razor
@* Index.cshtml *@
@if (TempData["SuccessMessage"] != null)
{
  <div class="alert alert-success">
    @TempData["SuccessMessage"]
  </div>
}
```

`TempData`는 한 요청에만 유지되며, 읽으면 자동으로 삭제됩니다. 리디렉션 후 일회성 메시지를 표시하는 데 완벽합니다.

### ViewData: 페이지 제목 등 메타데이터

```csharp
public void OnGet()
{
  ViewData["Title"] = "상품 목록";
  ViewData["PageDescription"] = "모든 상품을 확인하세요";
}
```

```razor
@{
  ViewData["Title"] = "상품 목록";
}

<h1>@ViewData["Title"]</h1>
<p>@ViewData["PageDescription"]</p>
```

레이아웃에서 사용:
```razor
@* _Layout.cshtml *@
<title>@ViewData["Title"] - My App</title>
```

### ViewBag: ViewData의 동적 버전

```csharp
public void OnGet()
{
  ViewBag.Title = "상품 목록";
  ViewBag.Categories = new[] { "전자제품", "의류", "식품" };
}
```

```razor
<h1>@ViewBag.Title</h1>
@foreach (var category in ViewBag.Categories)
{
  <span>@category</span>
}
```

`ViewBag`은 `dynamic` 타입이므로 타입 안전성이 없습니다. 가능하면 강타입 `@Model`을 사용하는 것이 좋습니다.

## 보안: Anti-Forgery 토큰 (CSRF 방지)

Razor Pages는 기본적으로 모든 POST 요청에 CSRF 방지 기능을 제공합니다.

```razor
<form method="post">
  @* Anti-forgery 토큰 자동 생성 *@
  <input asp-for="Name" />
  <button type="submit">전송</button>
</form>
```

렌더링된 HTML:
```html
<form method="post">
  <input name="__RequestVerificationToken" type="hidden" value="복잡한_토큰_값" />
  <input name="Name" />
  <button type="submit">전송</button>
</form>
```

POST 요청 시 토큰이 자동 검증되며, 일치하지 않으면 400 Bad Request를 반환합니다. 추가 코드 없이 CSRF 공격을 방지합니다.

AJAX 요청의 경우:
```javascript
// JavaScript
const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

fetch('/api/data', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'RequestVerificationToken': token
  },
  body: JSON.stringify(data)
});
```

## 페이지 필터: 횡단 관심사 처리

React의 HOC(Higher-Order Components)나 미들웨어처럼, 페이지 필터는 여러 페이지에 공통 로직을 적용합니다.

```csharp
// Filters/LogPageFilter.cs
public class LogPageFilter : IPageFilter
{
  private readonly ILogger<LogPageFilter> _logger;

  public LogPageFilter(ILogger<LogPageFilter> logger)
  {
    _logger = logger;
  }

  public void OnPageHandlerSelected(PageHandlerSelectedContext context)
  {
    _logger.LogInformation($"페이지 핸들러 선택: {context.HandlerMethod?.Name}");
  }

  public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
  {
    _logger.LogInformation($"페이지 핸들러 실행 시작: {context.HandlerMethod?.Name}");
  }

  public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
  {
    _logger.LogInformation($"페이지 핸들러 실행 완료: {context.HandlerMethod?.Name}");
  }
}
```

전역 등록:
```csharp
// Program.cs
builder.Services.AddRazorPages()
  .AddMvcOptions(options =>
  {
    options.Filters.Add<LogPageFilter>();
  });
```

또는 특정 페이지에만 적용:
```csharp
[TypeFilter(typeof(LogPageFilter))]
public class IndexModel : PageModel
{
  // ...
}
```

## 실습: 블로그 CRUD 애플리케이션

이제 배운 내용을 종합하여 완전한 블로그 애플리케이션을 만들어봅시다.

### 1. 모델 정의

```csharp
// Models/Post.cs
public class Post
{
  public int Id { get; set; }

  [Required]
  [StringLength(200)]
  public string Title { get; set; }

  [Required]
  public string Content { get; set; }

  [Required]
  [StringLength(100)]
  public string Author { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  [StringLength(500)]
  public string Tags { get; set; }
}
```

### 2. 게시글 목록 페이지

```csharp
// Pages/Blog/Index.cshtml.cs
public class IndexModel : PageModel
{
  private readonly ApplicationDbContext _db;

  public IndexModel(ApplicationDbContext db)
  {
    _db = db;
  }

  public List<Post> Posts { get; set; }

  [BindProperty(SupportsGet = true)]
  public string SearchQuery { get; set; }

  public async Task OnGetAsync()
  {
    var query = _db.Posts.AsQueryable();

    if (!string.IsNullOrWhiteSpace(SearchQuery))
    {
      query = query.Where(p => p.Title.Contains(SearchQuery) || p.Content.Contains(SearchQuery));
    }

    Posts = await query
      .OrderByDescending(p => p.CreatedAt)
      .ToListAsync();
  }
}
```

```razor
@* Pages/Blog/Index.cshtml *@
@page
@model IndexModel

<h1>블로그 게시글</h1>

<div class="mb-3">
  <a asp-page="./Create" class="btn btn-primary">새 게시글 작성</a>
</div>

<form method="get" class="mb-3">
  <div class="input-group">
    <input asp-for="SearchQuery" class="form-control" placeholder="검색..." />
    <button type="submit" class="btn btn-secondary">검색</button>
  </div>
</form>

@if (!Model.Posts.Any())
{
  <p>게시글이 없습니다.</p>
}
else
{
  <div class="row">
    @foreach (var post in Model.Posts)
    {
      <div class="col-md-6 mb-3">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">@post.Title</h5>
            <p class="card-text">@post.Content.Substring(0, Math.Min(100, post.Content.Length))...</p>
            <p class="text-muted">
              <small>작성자: @post.Author | @post.CreatedAt.ToString("yyyy-MM-dd")</small>
            </p>
            <a asp-page="./Details" asp-route-id="@post.Id" class="btn btn-sm btn-primary">더보기</a>
            <a asp-page="./Edit" asp-route-id="@post.Id" class="btn btn-sm btn-secondary">수정</a>
            <a asp-page="./Delete" asp-route-id="@post.Id" class="btn btn-sm btn-danger">삭제</a>
          </div>
        </div>
      </div>
    }
  </div>
}
```

### 3. 게시글 작성 페이지

```csharp
// Pages/Blog/Create.cshtml.cs
public class CreateModel : PageModel
{
  private readonly ApplicationDbContext _db;

  public CreateModel(ApplicationDbContext db)
  {
    _db = db;
  }

  [BindProperty]
  public Post Post { get; set; }

  public void OnGet()
  {
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
    {
      return Page();
    }

    _db.Posts.Add(Post);
    await _db.SaveChangesAsync();

    TempData["SuccessMessage"] = "게시글이 성공적으로 작성되었습니다!";
    return RedirectToPage("./Index");
  }
}
```

```razor
@* Pages/Blog/Create.cshtml *@
@page
@model CreateModel

<h1>새 게시글 작성</h1>

<form method="post">
  <div asp-validation-summary="ModelOnly" class="text-danger"></div>

  <div class="form-group">
    <label asp-for="Post.Title"></label>
    <input asp-for="Post.Title" class="form-control" />
    <span asp-validation-for="Post.Title" class="text-danger"></span>
  </div>

  <div class="form-group">
    <label asp-for="Post.Author"></label>
    <input asp-for="Post.Author" class="form-control" />
    <span asp-validation-for="Post.Author" class="text-danger"></span>
  </div>

  <div class="form-group">
    <label asp-for="Post.Content"></label>
    <textarea asp-for="Post.Content" class="form-control" rows="10"></textarea>
    <span asp-validation-for="Post.Content" class="text-danger"></span>
  </div>

  <div class="form-group">
    <label asp-for="Post.Tags"></label>
    <input asp-for="Post.Tags" class="form-control" placeholder="쉼표로 구분" />
    <span asp-validation-for="Post.Tags" class="text-danger"></span>
  </div>

  <button type="submit" class="btn btn-primary">작성</button>
  <a asp-page="./Index" class="btn btn-secondary">취소</a>
</form>

@section Scripts {
  <partial name="_ValidationScriptsPartial" />
}
```

### 4. 게시글 수정 페이지

```csharp
// Pages/Blog/Edit.cshtml.cs
public class EditModel : PageModel
{
  private readonly ApplicationDbContext _db;

  public EditModel(ApplicationDbContext db)
  {
    _db = db;
  }

  [BindProperty]
  public Post Post { get; set; }

  public async Task<IActionResult> OnGetAsync(int id)
  {
    Post = await _db.Posts.FindAsync(id);

    if (Post == null)
    {
      return NotFound();
    }

    return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
    {
      return Page();
    }

    Post.UpdatedAt = DateTime.UtcNow;
    _db.Attach(Post).State = EntityState.Modified;

    try
    {
      await _db.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!await _db.Posts.AnyAsync(p => p.Id == Post.Id))
      {
        return NotFound();
      }
      throw;
    }

    TempData["SuccessMessage"] = "게시글이 수정되었습니다!";
    return RedirectToPage("./Index");
  }
}
```

```razor
@* Pages/Blog/Edit.cshtml *@
@page "{id:int}"
@model EditModel

<h1>게시글 수정</h1>

<form method="post">
  <input type="hidden" asp-for="Post.Id" />
  <input type="hidden" asp-for="Post.CreatedAt" />

  @* Create.cshtml과 동일한 폼 필드들 *@

  <button type="submit" class="btn btn-primary">저장</button>
  <a asp-page="./Index" class="btn btn-secondary">취소</a>
</form>

@section Scripts {
  <partial name="_ValidationScriptsPartial" />
}
```

### 5. 게시글 삭제 페이지

```csharp
// Pages/Blog/Delete.cshtml.cs
public class DeleteModel : PageModel
{
  private readonly ApplicationDbContext _db;

  public DeleteModel(ApplicationDbContext db)
  {
    _db = db;
  }

  [BindProperty]
  public Post Post { get; set; }

  public async Task<IActionResult> OnGetAsync(int id)
  {
    Post = await _db.Posts.FindAsync(id);

    if (Post == null)
    {
      return NotFound();
    }

    return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    var post = await _db.Posts.FindAsync(Post.Id);

    if (post != null)
    {
      _db.Posts.Remove(post);
      await _db.SaveChangesAsync();
    }

    TempData["SuccessMessage"] = "게시글이 삭제되었습니다.";
    return RedirectToPage("./Index");
  }
}
```

```razor
@* Pages/Blog/Delete.cshtml *@
@page "{id:int}"
@model DeleteModel

<h1>게시글 삭제</h1>

<div class="alert alert-warning">
  <p>정말로 이 게시글을 삭제하시겠습니까? 이 작업은 되돌릴 수 없습니다.</p>
</div>

<div class="card">
  <div class="card-body">
    <h5 class="card-title">@Model.Post.Title</h5>
    <p class="card-text">@Model.Post.Content</p>
    <p class="text-muted">작성자: @Model.Post.Author</p>
  </div>
</div>

<form method="post" class="mt-3">
  <input type="hidden" asp-for="Post.Id" />
  <button type="submit" class="btn btn-danger">삭제</button>
  <a asp-page="./Index" class="btn btn-secondary">취소</a>
</form>
```

## Next.js와 Razor Pages 비교 요약

| 개념 | Next.js Pages Router | Razor Pages |
|------|----------------------|-------------|
| 파일 기반 라우팅 | `pages/` 폴더 | `Pages/` 폴더 |
| 동적 라우트 | `[param].tsx` | `@page "{param:type}"` |
| 데이터 페칭 | `getServerSideProps` | `OnGetAsync` 메서드 |
| 폼 처리 | API 라우트 + 클라이언트 상태 | 같은 페이지의 `OnPostAsync` |
| 유효성 검사 | Zod/Yup + 수동 로직 | 데이터 어노테이션 + 자동 검증 |
| Props/Model | `props` 객체 | `@Model` 프로퍼티 |
| 리디렉션 | `redirect: { ... }` | `RedirectToPage(...)` |
| 404 처리 | `notFound: true` | `return NotFound()` |
| 레이아웃 | 컴포넌트로 감싸기 | `Layout = "_Layout"` 선언 |

## 다음 단계

Razor Pages로 간단하고 생산적인 웹 애플리케이션을 만들 수 있게 되었습니다. 하지만 대규모 프로젝트나 복잡한 비즈니스 로직에는 더 구조화된 접근이 필요할 수 있습니다.

**Chapter 8: MVC 패턴**에서는 Model-View-Controller 아키텍처를 배웁니다. 관심사를 명확히 분리하고, 재사용 가능한 컴포넌트를 만들며, 팀 협업에 적합한 구조를 설계하는 방법을 익히게 됩니다.

---

## 참고 자료

- [Razor Pages 공식 문서](https://docs.microsoft.com/aspnet/core/razor-pages/)
- [모델 바인딩 가이드](https://docs.microsoft.com/aspnet/core/mvc/models/model-binding)
- [유효성 검사 가이드](https://docs.microsoft.com/aspnet/core/mvc/models/validation)
- [TempData, ViewData, ViewBag 비교](https://docs.microsoft.com/aspnet/core/mvc/views/overview)

다음 챕터: [Chapter 8: MVC 패턴 - 대규모 애플리케이션을 위한 구조](../chapter8/index.md)
