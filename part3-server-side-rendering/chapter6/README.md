# Chapter 5: Razor 문법 - JSX를 아는 개발자를 위한 가이드

## JSX에서 Razor로: 놀라운 유사성

React로 개발해본 적이 있다면, JSX의 강력함을 이미 경험했을 것입니다. HTML과 JavaScript를 하나의 파일에서 자유롭게 섞어 쓰며, 동적인 UI를 선언적으로 표현하는 방법 말이죠. Vue를 사용해봤다면, 템플릿 문법으로 비슷한 작업을 하는 것에 익숙할 것입니다.

Razor는 ASP.NET Core의 템플릿 엔진으로, JSX와 Vue 템플릿의 개념을 서버 사이드로 가져온 것입니다. 놀랍게도, Razor는 2010년에 처음 등장했습니다—React JSX(2013)보다 3년이나 앞섭니다! Anders Hejlsberg(C#과 TypeScript의 아버지)와 같은 Microsoft 팀이 "HTML 안에 코드를 깔끔하게 섞는 방법"을 먼저 고민했고, 그 영향이 현대 프론트엔드 프레임워크에도 이어진 것입니다.

이 챕터에서는 JSX와 Vue 템플릿을 이미 아는 개발자를 위해, Razor 문법을 빠르게 익힐 수 있도록 직접 비교하며 설명합니다. "아, 이건 JSX의 저것과 같구나!"라는 순간이 반복될 것입니다.

## 핵심 개념: `@` 기호

JSX에서 JavaScript 표현식을 삽입할 때 중괄호 `{}`를 사용합니다. Razor에서는 `@` 기호를 사용합니다. 이것이 가장 근본적인 차이이자, 유일한 차이라고 해도 과언이 아닙니다.

### 표현식 출력하기

**React JSX:**
```jsx
function Greeting({ name, age }) {
  return (
    <div>
      <h1>안녕하세요, {name}님!</h1>
      <p>나이: {age}세</p>
      <p>내년에는 {age + 1}세가 됩니다.</p>
    </div>
  );
}
```

**Razor:**
```razor
@model User

<div>
  <h1>안녕하세요, @Model.Name님!</h1>
  <p>나이: @Model.Age세</p>
  <p>내년에는 @(Model.Age + 1)세가 됩니다.</p>
</div>
```

차이점을 보셨나요?

1. **JSX**: `{expression}` → **Razor**: `@expression` 또는 `@(expression)`
2. **JSX**: props로 데이터 전달 → **Razor**: `@model`로 타입 선언 후 `Model` 프로퍼티로 접근

`@` 뒤에 바로 프로퍼티 이름이 오면 괄호가 필요 없지만, 복잡한 표현식은 `@()`로 감싸야 합니다.

**Vue 템플릿:**
```vue
<template>
  <div>
    <h1>안녕하세요, {{ name }}님!</h1>
    <p>나이: {{ age }}세</p>
    <p>내년에는 {{ age + 1 }}세가 됩니다.</p>
  </div>
</template>
```

**Razor:**
```razor
@model User

<div>
  <h1>안녕하세요, @Model.Name님!</h1>
  <p>나이: @Model.Age세</p>
  <p>내년에는 @(Model.Age + 1)세가 됩니다.</p>
</div>
```

Vue의 `{{ }}` 대신 Razor는 `@`를 사용합니다. 더 간결하죠?

### 자동 HTML 인코딩: XSS 방지

JSX와 Razor 모두 기본적으로 출력을 HTML 인코딩합니다. 이는 XSS(Cross-Site Scripting) 공격을 자동으로 방지합니다.

```jsx
// React - 자동 이스케이프
const userInput = '<script>alert("XSS")</script>';
<div>{userInput}</div>  // 화면에 문자열 그대로 표시됨
```

```razor
@* Razor - 자동 이스케이프 *@
@{
  var userInput = "<script>alert('XSS')</script>";
}
<div>@userInput</div>  @* 화면에 문자열 그대로 표시됨 *@
```

둘 다 `<script>` 태그를 실행하지 않고, 텍스트로 표시합니다. 의도적으로 HTML을 렌더링하려면:

```jsx
// React - 위험한 HTML 렌더링 (신중하게 사용)
<div dangerouslySetInnerHTML={{ __html: htmlString }} />
```

```razor
@* Razor - 원시 HTML 렌더링 (신중하게 사용) *@
<div>@Html.Raw(htmlString)</div>
```

이름에서 알 수 있듯이 (`dangerouslySetInnerHTML`, `Html.Raw`), 이 기능은 신뢰할 수 있는 데이터에만 사용해야 합니다.

## 조건부 렌더링

UI를 조건에 따라 다르게 표시하는 것은 모든 템플릿 엔진의 핵심 기능입니다.

### 단순 조건: if-else

**React JSX:**
```jsx
function WelcomeMessage({ user }) {
  return (
    <div>
      {user.isLoggedIn ? (
        <h1>환영합니다, {user.name}님!</h1>
      ) : (
        <h1>로그인해주세요.</h1>
      )}
    </div>
  );
}
```

**Razor:**
```razor
@model User

<div>
  @if (Model.IsLoggedIn)
  {
    <h1>환영합니다, @Model.Name님!</h1>
  }
  else
  {
    <h1>로그인해주세요.</h1>
  }
</div>
```

Razor는 C#의 `if-else` 문을 그대로 사용합니다. JSX의 삼항 연산자보다 더 읽기 쉽다고 느낄 수 있습니다.

**Vue 템플릿:**
```vue
<template>
  <div>
    <h1 v-if="user.isLoggedIn">환영합니다, {{ user.name }}님!</h1>
    <h1 v-else>로그인해주세요.</h1>
  </div>
</template>
```

**Razor (같은 결과):**
```razor
@model User

<div>
  @if (Model.IsLoggedIn)
  {
    <h1>환영합니다, @Model.Name님!</h1>
  }
  else
  {
    <h1>로그인해주세요.</h1>
  }
</div>
```

### 논리 AND 연산자

**React JSX:**
```jsx
function Notification({ hasNewMessages, messageCount }) {
  return (
    <div>
      {hasNewMessages && (
        <div className="badge">{messageCount}개의 새 메시지</div>
      )}
    </div>
  );
}
```

**Razor:**
```razor
@model NotificationViewModel

<div>
  @if (Model.HasNewMessages)
  {
    <div class="badge">@Model.MessageCount개의 새 메시지</div>
  }
</div>
```

JSX의 `&&` 트릭은 Razor에서 명시적인 `@if`로 표현됩니다. 더 명확하지만 약간 더 장황합니다.

### 다중 조건: switch-case

**React JSX:**
```jsx
function StatusBadge({ status }) {
  let badge;
  switch (status) {
    case 'pending':
      badge = <span className="badge-yellow">대기중</span>;
      break;
    case 'approved':
      badge = <span className="badge-green">승인됨</span>;
      break;
    case 'rejected':
      badge = <span className="badge-red">거절됨</span>;
      break;
    default:
      badge = <span className="badge-gray">알 수 없음</span>;
  }
  return <div>{badge}</div>;
}
```

**Razor:**
```razor
@model OrderViewModel

<div>
  @switch (Model.Status)
  {
    case "pending":
      <span class="badge-yellow">대기중</span>
      break;
    case "approved":
      <span class="badge-green">승인됨</span>
      break;
    case "rejected":
      <span class="badge-red">거절됨</span>
      break;
    default:
      <span class="badge-gray">알 수 없음</span>
      break;
  }
</div>
```

Razor의 `@switch`는 C#의 switch 문법을 그대로 사용하며, 각 case 안에 직접 HTML을 작성할 수 있습니다.

**현대적인 C# switch 표현식 (C# 8+):**
```razor
@model OrderViewModel

<div>
  @{
    var badgeHtml = Model.Status switch
    {
      "pending" => "<span class=\"badge-yellow\">대기중</span>",
      "approved" => "<span class=\"badge-green\">승인됨</span>",
      "rejected" => "<span class=\"badge-red\">거절됨</span>",
      _ => "<span class=\"badge-gray\">알 수 없음</span>"
    };
  }
  @Html.Raw(badgeHtml)
</div>
```

## 리스트 렌더링

배열이나 컬렉션을 반복하여 UI 요소를 생성하는 것은 가장 흔한 작업입니다.

### 기본 반복

**React JSX:**
```jsx
function UserList({ users }) {
  return (
    <ul>
      {users.map(user => (
        <li key={user.id}>
          {user.name} - {user.email}
        </li>
      ))}
    </ul>
  );
}
```

**Razor:**
```razor
@model List<User>

<ul>
  @foreach (var user in Model)
  {
    <li>
      @user.Name - @user.Email
    </li>
  }
</ul>
```

JSX의 `.map()`이 Razor의 `@foreach`로 바뀌었습니다. `key` prop은 Razor에서 자동 처리되므로 명시할 필요가 없습니다(서버 렌더링이므로 React의 reconciliation이 필요 없음).

**Vue 템플릿:**
```vue
<template>
  <ul>
    <li v-for="user in users" :key="user.id">
      {{ user.name }} - {{ user.email }}
    </li>
  </ul>
</template>
```

**Razor (같은 결과):**
```razor
@model List<User>

<ul>
  @foreach (var user in Model)
  {
    <li>
      @user.Name - @user.Email
    </li>
  }
</ul>
```

### 인덱스 접근

**React JSX:**
```jsx
{users.map((user, index) => (
  <li key={user.id}>
    {index + 1}. {user.name}
  </li>
))}
```

**Razor:**
```razor
@{
  var index = 0;
}
@foreach (var user in Model)
{
  <li>
    @(++index). @user.name
  </li>
}
```

또는 LINQ를 사용하여:

```razor
@foreach (var item in Model.Select((user, index) => new { user, index }))
{
  <li>
    @(item.index + 1). @item.user.Name
  </li>
}
```

### 빈 리스트 처리

**React JSX:**
```jsx
function ProductList({ products }) {
  return (
    <div>
      {products.length === 0 ? (
        <p>상품이 없습니다.</p>
      ) : (
        <ul>
          {products.map(product => (
            <li key={product.id}>{product.name}</li>
          ))}
        </ul>
      )}
    </div>
  );
}
```

**Razor:**
```razor
@model List<Product>

<div>
  @if (Model.Count == 0)
  {
    <p>상품이 없습니다.</p>
  }
  else
  {
    <ul>
      @foreach (var product in Model)
      {
        <li>@product.Name</li>
      }
    </ul>
  }
</div>
```

또는 더 간결하게:

```razor
@model List<Product>

<div>
  @if (!Model.Any())
  {
    <p>상품이 없습니다.</p>
  }
  else
  {
    <ul>
      @foreach (var product in Model)
      {
        <li>@product.Name</li>
      }
    </ul>
  }
</div>
```

## Razor 지시문 (Directives)

Razor 지시문은 `@` 뒤에 키워드를 붙여 특별한 기능을 제공합니다. Vue의 `v-` 지시문과 유사한 개념입니다.

### `@model`: 뷰의 타입 선언

React의 props 타입을 선언하는 것과 비슷합니다.

**TypeScript + React:**
```tsx
interface UserProps {
  name: string;
  age: number;
  email: string;
}

function UserProfile(props: UserProps) {
  return <div>{props.name}</div>;
}
```

**Razor:**
```razor
@model User

<div>@Model.Name</div>
```

`@model User`는 이 뷰가 `User` 타입의 모델을 받는다고 선언합니다. IntelliSense(자동완성)와 컴파일 타임 타입 체크를 제공합니다. `@Model.`을 입력하면 IDE가 `User` 클래스의 모든 프로퍼티를 자동완성해줍니다.

### `@using`: 네임스페이스 가져오기

JavaScript의 `import`와 같습니다.

**JavaScript:**
```javascript
import { formatDate, formatCurrency } from './utils';
```

**Razor:**
```razor
@using MyApp.Utilities
@using MyApp.Models

<div>@FormatHelper.FormatDate(Model.CreatedAt)</div>
```

### `@inject`: 의존성 주입

React의 Context나 Angular의 DI와 유사합니다.

**React Context:**
```jsx
import { useContext } from 'react';
import { ThemeContext } from './ThemeContext';

function ThemedButton() {
  const theme = useContext(ThemeContext);
  return <button style={{ color: theme.color }}>버튼</button>;
}
```

**Razor:**
```razor
@inject IConfiguration Config

<div>
  API Key: @Config["ApiSettings:Key"]
</div>
```

`@inject`는 ASP.NET Core의 의존성 주입 컨테이너에서 서비스를 직접 뷰에 주입합니다. 별도의 Provider나 설정 없이 즉시 사용 가능합니다.

### `@section`: 레이아웃의 특정 영역에 콘텐츠 삽입

**React Layout 패턴:**
```jsx
// Layout.jsx
function Layout({ children, header, sidebar }) {
  return (
    <div>
      <header>{header}</header>
      <aside>{sidebar}</aside>
      <main>{children}</main>
    </div>
  );
}

// Page.jsx
<Layout
  header={<h1>제목</h1>}
  sidebar={<nav>...</nav>}
>
  <p>메인 콘텐츠</p>
</Layout>
```

**Razor:**
```razor
@* _Layout.cshtml (레이아웃 파일) *@
<!DOCTYPE html>
<html>
<head>
  <title>@ViewData["Title"]</title>
  @RenderSection("Styles", required: false)
</head>
<body>
  <header>
    @RenderSection("Header", required: false)
  </header>
  <main>
    @RenderBody()  @* children과 동일 *@
  </main>
  <footer>
    @RenderSection("Scripts", required: false)
  </footer>
</body>
</html>

@* Index.cshtml (페이지 파일) *@
@{
  ViewData["Title"] = "홈 페이지";
  Layout = "_Layout";
}

@section Header {
  <h1>환영합니다!</h1>
}

@section Styles {
  <link rel="stylesheet" href="/css/home.css" />
}

<p>메인 콘텐츠가 여기 들어갑니다.</p>

@section Scripts {
  <script src="/js/home.js"></script>
}
```

`@section`은 레이아웃의 특정 "구멍"에 콘텐츠를 삽입하는 메커니즘입니다. 페이지별로 다른 CSS나 JavaScript를 로드하는 데 유용합니다.

## 코드 블록: `@{ }`

JSX에서는 `{}`안에 모든 JavaScript를 작성할 수 있지만, 여러 문장을 작성하려면 IIFE를 사용해야 합니다. Razor는 `@{ }`로 여러 줄의 C# 코드를 작성할 수 있습니다.

**React JSX (IIFE 사용):**
```jsx
function ProductPrice({ price, discount }) {
  return (
    <div>
      {(() => {
        const finalPrice = price * (1 - discount);
        const savings = price - finalPrice;
        return (
          <>
            <span>원가: {price}원</span>
            <span>할인가: {finalPrice}원</span>
            <span>절약: {savings}원</span>
          </>
        );
      })()}
    </div>
  );
}
```

**Razor (코드 블록 사용):**
```razor
@model Product

<div>
  @{
    var finalPrice = Model.Price * (1 - Model.Discount);
    var savings = Model.Price - finalPrice;
  }
  <span>원가: @Model.Price원</span>
  <span>할인가: @finalPrice원</span>
  <span>절약: @savings원</span>
</div>
```

`@{ }` 블록 안의 코드는 실행만 되고 출력되지 않습니다. 변수를 선언하거나, 복잡한 로직을 수행하는 데 사용합니다.

## 레이아웃(Layouts): 마스터 페이지

React의 Layout 컴포넌트와 개념적으로 동일합니다.

**React:**
```jsx
// Layout.jsx
function Layout({ children }) {
  return (
    <div>
      <nav>네비게이션</nav>
      <main>{children}</main>
      <footer>푸터</footer>
    </div>
  );
}

// App.jsx
function App() {
  return (
    <Layout>
      <HomePage />
    </Layout>
  );
}
```

**Razor:**
```razor
@* _Layout.cshtml *@
<!DOCTYPE html>
<html>
<head>
  <title>@ViewData["Title"]</title>
</head>
<body>
  <nav>네비게이션</nav>
  <main>
    @RenderBody()
  </main>
  <footer>푸터</footer>
</body>
</html>

@* Index.cshtml *@
@{
  Layout = "_Layout";
  ViewData["Title"] = "홈 페이지";
}

<h1>홈 페이지 콘텐츠</h1>
```

차이점은 React는 명시적으로 `<Layout>` 컴포넌트로 감싸지만, Razor는 페이지에서 `Layout = "_Layout"`으로 사용할 레이아웃을 지정한다는 것입니다.

### 중첩 레이아웃

복잡한 애플리케이션에서는 레이아웃을 중첩할 수 있습니다.

```razor
@* _Layout.cshtml (기본 레이아웃) *@
<!DOCTYPE html>
<html>
<body>
  <header>사이트 헤더</header>
  @RenderBody()
  <footer>사이트 푸터</footer>
</body>
</html>

@* _AdminLayout.cshtml (관리자 레이아웃) *@
@{
  Layout = "_Layout";  // 기본 레이아웃을 확장
}

<div class="admin-wrapper">
  <aside>관리자 사이드바</aside>
  <main>@RenderBody()</main>
</div>

@* Admin/Index.cshtml (관리자 페이지) *@
@{
  Layout = "_AdminLayout";  // 관리자 레이아웃 사용
}

<h1>관리자 대시보드</h1>
```

결과적으로 페이지는 `_Layout` → `_AdminLayout` → 페이지 콘텐츠 순서로 렌더링됩니다.

## 부분 뷰(Partial Views): 재사용 가능한 조각

React의 컴포넌트와 유사합니다.

**React:**
```jsx
// UserCard.jsx
function UserCard({ user }) {
  return (
    <div className="card">
      <img src={user.avatar} alt={user.name} />
      <h3>{user.name}</h3>
      <p>{user.email}</p>
    </div>
  );
}

// UserList.jsx
function UserList({ users }) {
  return (
    <div>
      {users.map(user => (
        <UserCard key={user.id} user={user} />
      ))}
    </div>
  );
}
```

**Razor:**
```razor
@* _UserCard.cshtml (부분 뷰) *@
@model User

<div class="card">
  <img src="@Model.Avatar" alt="@Model.Name" />
  <h3>@Model.Name</h3>
  <p>@Model.Email</p>
</div>

@* UserList.cshtml *@
@model List<User>

<div>
  @foreach (var user in Model)
  {
    <partial name="_UserCard" model="user" />
  }
</div>
```

또는 `@Html.Partial()` 헬퍼 사용:

```razor
@foreach (var user in Model)
{
  @Html.Partial("_UserCard", user)
}
```

부분 뷰는 관례적으로 `_`로 시작하는 이름을 사용합니다 (예: `_UserCard.cshtml`).

## Tag Helpers: HTML을 위한 특별한 도구

Tag Helpers는 Razor만의 강력한 기능으로, HTML 태그를 확장하여 서버 사이드 기능을 제공합니다.

### `asp-for`: 모델 바인딩

**전통적인 HTML 폼:**
```html
<label for="email">이메일:</label>
<input type="email" id="email" name="email" value="" />
<span class="error"></span>
```

**Tag Helper 사용:**
```razor
@model RegisterViewModel

<label asp-for="Email"></label>
<input asp-for="Email" />
<span asp-validation-for="Email"></span>
```

컴파일 후 결과 (자동 생성):
```html
<label for="Email">이메일</label>
<input type="email" id="Email" name="Email" value=""
       data-val="true"
       data-val-required="이메일은 필수입니다."
       data-val-email="올바른 이메일 형식이 아닙니다." />
<span class="field-validation-valid" data-valmsg-for="Email"></span>
```

Tag Helper가 자동으로:
- `id`와 `name` 속성 생성
- `label` 텍스트를 모델의 `[Display]` 어노테이션에서 가져옴
- 타입에 맞는 `type` 속성 설정 (`email`, `number`, `date` 등)
- 유효성 검사 규칙을 데이터 속성으로 삽입

### `asp-action`, `asp-controller`: 타입 안전한 링크

**전통적인 방식 (문자열 하드코딩):**
```html
<a href="/products/details?id=123">상품 보기</a>
```

문제: 라우트가 변경되면 모든 링크를 수동으로 업데이트해야 함.

**Tag Helper 사용:**
```razor
<a asp-controller="Products" asp-action="Details" asp-route-id="123">
  상품 보기
</a>
```

컴파일러가 `ProductsController`의 `Details` 액션이 존재하는지 확인하며, 라우트 URL을 자동 생성합니다. 리팩토링 안전성이 보장됩니다.

### `asp-validation-summary`: 폼 유효성 검사 요약

```razor
<form method="post">
  <div asp-validation-summary="ModelOnly" class="text-danger"></div>

  <div class="form-group">
    <label asp-for="Email"></label>
    <input asp-for="Email" class="form-control" />
    <span asp-validation-for="Email" class="text-danger"></span>
  </div>

  <div class="form-group">
    <label asp-for="Password"></label>
    <input asp-for="Password" type="password" class="form-control" />
    <span asp-validation-for="Password" class="text-danger"></span>
  </div>

  <button type="submit">로그인</button>
</form>
```

`asp-validation-summary`는 모든 검증 오류의 요약을 표시합니다. React의 폼 라이브러리(React Hook Form, Formik)에서 `errors` 객체를 렌더링하는 것과 유사합니다.

### 환경별 Tag Helper

개발/프로덕션 환경에 따라 다른 콘텐츠를 표시합니다.

```razor
<environment include="Development">
  <link rel="stylesheet" href="~/css/site.css" />
  <script src="~/js/site.js"></script>
</environment>

<environment exclude="Development">
  <link rel="stylesheet" href="~/css/site.min.css" asp-append-version="true" />
  <script src="~/js/site.min.js" asp-append-version="true"></script>
</environment>
```

`asp-append-version="true"`는 파일 내용의 해시를 쿼리 스트링에 추가하여 캐시 무효화를 자동 처리합니다 (Webpack의 `[contenthash]`와 유사).

## View Components: 서버 사이드 컴포넌트

부분 뷰보다 강력한 재사용 가능한 컴포넌트입니다. React 컴포넌트처럼 자체 로직과 상태를 가질 수 있습니다.

**React 컴포넌트:**
```jsx
// RecentPosts.jsx
function RecentPosts() {
  const [posts, setPosts] = useState([]);

  useEffect(() => {
    fetch('/api/posts/recent')
      .then(res => res.json())
      .then(data => setPosts(data));
  }, []);

  return (
    <div className="recent-posts">
      <h3>최근 게시글</h3>
      <ul>
        {posts.map(post => (
          <li key={post.id}>{post.title}</li>
        ))}
      </ul>
    </div>
  );
}
```

**Razor View Component:**

```csharp
// ViewComponents/RecentPostsViewComponent.cs
public class RecentPostsViewComponent : ViewComponent
{
  private readonly IPostService _postService;

  public RecentPostsViewComponent(IPostService postService)
  {
    _postService = postService;
  }

  public async Task<IViewComponentResult> InvokeAsync(int count = 5)
  {
    var posts = await _postService.GetRecentPostsAsync(count);
    return View(posts);
  }
}
```

```razor
@* Views/Shared/Components/RecentPosts/Default.cshtml *@
@model List<Post>

<div class="recent-posts">
  <h3>최근 게시글</h3>
  <ul>
    @foreach (var post in Model)
    {
      <li>@post.Title</li>
    }
  </ul>
</div>
```

**사용 방법:**
```razor
@* 어떤 뷰에서든 사용 가능 *@
<vc:recent-posts count="10"></vc:recent-posts>

@* 또는 *@
@await Component.InvokeAsync("RecentPosts", new { count = 10 })
```

View Component는:
- 의존성 주입을 지원합니다 (생성자 주입)
- 자체적으로 데이터를 가져올 수 있습니다
- 비동기 작업을 수행할 수 있습니다
- 재사용 가능하고 테스트 가능합니다

React 컴포넌트와의 차이점은 서버에서 실행되며, 한 번만 렌더링된다는 것입니다. 클라이언트 사이드 상태나 인터랙션은 없습니다.

## 실습: JSX를 Razor로 변환하기

다음 React 컴포넌트를 Razor로 변환해보세요.

**React 코드:**
```jsx
function BlogPost({ post, author, comments }) {
  const formattedDate = new Date(post.publishedAt).toLocaleDateString();

  return (
    <article className="blog-post">
      <header>
        <h1>{post.title}</h1>
        <div className="meta">
          <span>작성자: {author.name}</span>
          <span>작성일: {formattedDate}</span>
          <span>조회수: {post.views}</span>
        </div>
      </header>

      <div className="content">
        {post.content}
      </div>

      <section className="comments">
        <h2>댓글 ({comments.length})</h2>
        {comments.length === 0 ? (
          <p>첫 댓글을 작성해보세요!</p>
        ) : (
          <ul>
            {comments.map(comment => (
              <li key={comment.id}>
                <strong>{comment.userName}</strong>: {comment.text}
              </li>
            ))}
          </ul>
        )}
      </section>
    </article>
  );
}
```

**Razor 변환:**
```razor
@model BlogPostViewModel

@{
  var formattedDate = Model.Post.PublishedAt.ToString("yyyy-MM-dd");
}

<article class="blog-post">
  <header>
    <h1>@Model.Post.Title</h1>
    <div class="meta">
      <span>작성자: @Model.Author.Name</span>
      <span>작성일: @formattedDate</span>
      <span>조회수: @Model.Post.Views</span>
    </div>
  </header>

  <div class="content">
    @Model.Post.Content
  </div>

  <section class="comments">
    <h2>댓글 (@Model.Comments.Count)</h2>
    @if (Model.Comments.Count == 0)
    {
      <p>첫 댓글을 작성해보세요!</p>
    }
    else
    {
      <ul>
        @foreach (var comment in Model.Comments)
        {
          <li>
            <strong>@comment.UserName</strong>: @comment.Text
          </li>
        }
      </ul>
    }
  </section>
</article>
```

**ViewModel 클래스:**
```csharp
public class BlogPostViewModel
{
  public Post Post { get; set; }
  public Author Author { get; set; }
  public List<Comment> Comments { get; set; }
}
```

## 핵심 차이점 요약

| 개념 | JSX (React) | Razor |
|------|-------------|-------|
| 표현식 삽입 | `{expression}` | `@expression` 또는 `@(expression)` |
| 조건부 렌더링 | `{condition && <div/>}` 또는 `{a ? b : c}` | `@if (condition) { }` |
| 리스트 렌더링 | `.map(item => ...)` | `@foreach (var item in list) { }` |
| 주석 | `{/* comment */}` | `@* comment *@` |
| Props/Model | `props.name` | `@Model.Name` |
| 다중 문장 | IIFE `{(() => { })()}` | `@{ ... }` 블록 |
| 컴포넌트 | `<MyComponent />` | `<vc:my-component />` 또는 `<partial name="_MyPartial" />` |
| 타입 선언 | TypeScript `interface` | `@model MyType` |

## 다음 단계

Razor 문법의 기초를 익혔으니, 이제 실제로 사용해볼 차례입니다. Chapter 6에서는 Razor Pages로 완전한 CRUD 애플리케이션을 만들며, 폼 처리, 데이터 바인딩, 유효성 검사를 직접 경험하게 됩니다.

Razor는 단순한 템플릿 엔진이 아닙니다. 강력한 타입 시스템, 자동 보안 기능, 풍부한 도구 지원을 갖춘 현대적인 UI 렌더링 솔루션입니다. JSX를 아는 여러분은 이미 Razor의 80%를 알고 있습니다. 나머지 20%는 실습을 통해 자연스럽게 체득될 것입니다.

---

## 추가 학습 자료

- [Razor 공식 문서](https://docs.microsoft.com/aspnet/core/mvc/views/razor)
- [Tag Helpers 가이드](https://docs.microsoft.com/aspnet/core/mvc/views/tag-helpers/intro)
- [View Components 상세 가이드](https://docs.microsoft.com/aspnet/core/mvc/views/view-components)
- [Razor 문법 치트시트](https://docs.microsoft.com/aspnet/core/mvc/views/razor#razor-syntax)

다음 챕터: [Chapter 6: Razor Pages - 단순함의 힘](../chapter6/README.md)
