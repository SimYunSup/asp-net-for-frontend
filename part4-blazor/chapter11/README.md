# Chapter 11: Blazor 고급 패턴

## 프로덕션을 향한 여정: 고급 기법의 필요성

Chapter 9와 10에서 Blazor의 기초를 다졌습니다. 컴포넌트를 만들고, 상태를 관리하며, 폼을 처리할 수 있게 되었습니다. 하지만 실제 프로덕션 애플리케이션을 만들려면 더 많은 것이 필요합니다.

사용자가 `/products/123`을 입력했을 때 올바른 페이지로 라우팅하려면? 수천 개의 항목을 효율적으로 렌더링하려면? 인증된 사용자만 특정 페이지에 접근하게 하려면? SignalR로 실시간 데이터를 받아 UI를 업데이트하려면?

이 챕터에서는 이런 실전 질문들에 답합니다. React Router, React Virtualized, NextAuth 같은 React 생태계의 도구들이 해결하는 문제를 Blazor에서는 어떻게 다루는지 배웁니다. 프로덕션 수준의 애플리케이션을 만드는 고급 패턴과 최적화 기법을 마스터할 시간입니다.

## 라우팅: SPA의 핵심

React Router, Vue Router가 클라이언트 사이드 라우팅의 표준이듯, Blazor도 강력한 라우팅 시스템을 내장하고 있습니다.

### `@page` 지시문: 선언적 라우팅

Next.js의 파일 시스템 라우팅과 유사하게, Blazor는 `@page` 지시문으로 라우트를 정의합니다.

::: v-pre
```razor
@* Pages/Products.razor *@
@page "/products"

<h1>상품 목록</h1>
```
:::

사용자가 `/products`를 방문하면 이 컴포넌트가 렌더링됩니다.

### 동적 라우트 매개변수

::: v-pre
```razor
@* Pages/ProductDetail.razor *@
@page "/products/{id:int}"

<h1>상품 상세</h1>
<p>상품 ID: @Id</p>

@code {
  [Parameter]
  public int Id { get; set; }

  protected override async Task OnParametersSetAsync()
  {
    // Id가 변경될 때마다 데이터 로드
    await LoadProduct(Id);
  }
}
```
:::

`{id:int}`는 라우트 제약입니다. 정수만 허용되며, 타입 안정성을 보장합니다.

React Router 버전:

```jsx
// App.jsx
<Routes>
  <Route path="/products/:id" element={<ProductDetail />} />
</Routes>

// ProductDetail.jsx
import { useParams } from 'react-router-dom';

function ProductDetail() {
  const { id } = useParams();

  useEffect(() => {
    loadProduct(id);
  }, [id]);

  return (
    <div>
      <h1>상품 상세</h1>
      <p>상품 ID: {id}</p>
    </div>
  );
}
```

**차이점:**

1. **타입 안정성**: Blazor는 `{id:int}`로 타입 강제, React는 항상 문자열
2. **선언 위치**: Blazor는 컴포넌트 자체, React는 중앙 라우트 설정
3. **매개변수 접근**: Blazor는 `[Parameter]` 프로퍼티, React는 `useParams` 훅

### 라우트 제약 조건

```razor
@page "/blog/{year:int}/{month:int}/{day:int}/{slug}"

@code {
  [Parameter]
  public int Year { get; set; }

  [Parameter]
  public int Month { get; set; }

  [Parameter]
  public int Day { get; set; }

  [Parameter]
  public string Slug { get; set; } = string.Empty;
}
```

URL: `/blog/2024/11/12/blazor-introduction`

사용 가능한 제약:
- `:int`, `:long`, `:float`, `:double`, `:decimal`
- `:bool`
- `:datetime`
- `:guid`
- `:regex(pattern)`
- `:length(min,max)`

### 선택적 매개변수

::: v-pre
```razor
@page "/search/{category?}"

<h1>검색</h1>
@if (!string.IsNullOrEmpty(Category))
{
  <p>카테고리: @Category</p>
}

@code {
  [Parameter]
  public string? Category { get; set; }
}
```
:::

`/search`와 `/search/books` 모두 매칭됩니다.

### 쿼리 문자열

::: v-pre
```razor
@page "/products"
@inject NavigationManager Navigation

<h1>상품 목록</h1>
<p>페이지: @currentPage, 정렬: @sortBy</p>

@code {
  private int currentPage = 1;
  private string sortBy = "name";

  protected override void OnInitialized()
  {
    var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
    if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("page", out var pageValues))
    {
      int.TryParse(pageValues, out currentPage);
    }
    if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("sort", out var sortValues))
    {
      sortBy = sortValues;
    }
  }
}
```
:::

URL: `/products?page=2&sort=price`

.NET 7+에서는 `[SupplyParameterFromQuery]` 사용:

```razor
@code {
  [Parameter]
  [SupplyParameterFromQuery(Name = "page")]
  public int CurrentPage { get; set; } = 1;

  [Parameter]
  [SupplyParameterFromQuery(Name = "sort")]
  public string SortBy { get; set; } = "name";
}
```

훨씬 간결합니다!

### 프로그래밍 방식 네비게이션

::: v-pre
```razor
@inject NavigationManager Navigation

<button @onclick="GoToProduct">상품 보기</button>
<button @onclick="GoBack">뒤로</button>

@code {
  private void GoToProduct()
  {
    Navigation.NavigateTo("/products/123");
  }

  private void GoBack()
  {
    Navigation.NavigateTo(Navigation.Uri, replace: true);
  }

  private async Task SaveAndRedirect()
  {
    await SaveData();
    Navigation.NavigateTo("/success");
  }
}
```
:::

React Router 버전:

```jsx
import { useNavigate } from 'react-router-dom';

function MyComponent() {
  const navigate = useNavigate();

  const goToProduct = () => {
    navigate('/products/123');
  };

  const goBack = () => {
    navigate(-1);
  };

  const saveAndRedirect = async () => {
    await saveData();
    navigate('/success');
  };

  return (
    <button onClick={goToProduct}>상품 보기</button>
  );
}
```

### NavLink 컴포넌트: 활성 링크 스타일링

::: v-pre
```razor
<nav>
  <NavLink href="/" Match="NavLinkMatch.All">홈</NavLink>
  <NavLink href="/products">상품</NavLink>
  <NavLink href="/about">소개</NavLink>
</nav>

<style>
  .active {
    font-weight: bold;
    color: blue;
  }
</style>
```
:::

현재 경로와 일치하면 자동으로 `active` 클래스가 추가됩니다.

React Router 버전:

```jsx
import { NavLink } from 'react-router-dom';

<nav>
  <NavLink to="/" className={({ isActive }) => isActive ? 'active' : ''}>
    홈
  </NavLink>
  <NavLink to="/products" className={({ isActive }) => isActive ? 'active' : ''}>
    상품
  </NavLink>
</nav>
```

Blazor는 CSS 클래스 자동 추가가 더 간단합니다.

## 레이아웃: 일관된 UI 구조

React의 Layout 컴포넌트와 유사하게, Blazor는 레이아웃을 통해 여러 페이지에서 공통 UI를 공유합니다.

### MainLayout 생성

::: v-pre
```razor
@* Shared/MainLayout.razor *@
@inherits LayoutComponentBase

<div class="page">
  <header>
    <nav>
      <NavLink href="/">홈</NavLink>
      <NavLink href="/products">상품</NavLink>
      <NavLink href="/about">소개</NavLink>
    </nav>
  </header>

  <main>
    @Body  @* 페이지 콘텐츠가 여기 렌더링됨 *@
  </main>

  <footer>
    <p>&copy; 2024 My Blazor App</p>
  </footer>
</div>

<style>
  .page {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
  }

  main {
    flex: 1;
  }
</style>
```
:::

페이지에서 사용:

::: v-pre
```razor
@page "/products"
@layout MainLayout  @* 명시적 지정 (선택적) *@

<h1>상품 목록</h1>
```
:::

또는 `_Imports.razor`에서 전역 설정:

```razor
@* _Imports.razor *@
@layout MainLayout
```

### 중첩 레이아웃

복잡한 애플리케이션에서는 레이아웃을 중첩할 수 있습니다.

::: v-pre
```razor
@* Shared/AdminLayout.razor *@
@inherits LayoutComponentBase
@layout MainLayout  @* MainLayout을 상속 *@

<div class="admin-container">
  <aside class="sidebar">
    <NavLink href="/admin/dashboard">대시보드</NavLink>
    <NavLink href="/admin/users">사용자 관리</NavLink>
    <NavLink href="/admin/settings">설정</NavLink>
  </aside>

  <div class="admin-content">
    @Body
  </div>
</div>
```
:::

관리자 페이지:

::: v-pre
```razor
@page "/admin/dashboard"
@layout AdminLayout

<h1>관리자 대시보드</h1>
```
:::

렌더링 결과: `MainLayout` → `AdminLayout` → 페이지 콘텐츠

### 동적 레이아웃 전환

::: v-pre
```razor
@* App.razor *@
<Router AppAssembly="@typeof(App).Assembly">
  <Found Context="routeData">
    <RouteView RouteData="@routeData" DefaultLayout="@GetLayout(routeData)" />
  </Found>
</Router>

@code {
  private Type GetLayout(RouteData routeData)
  {
    // 관리자 페이지는 AdminLayout, 나머지는 MainLayout
    if (routeData.PageType.Namespace?.Contains("Admin") == true)
    {
      return typeof(AdminLayout);
    }
    return typeof(MainLayout);
  }
}
```
:::

## Razor Class Library: 재사용 가능한 컴포넌트 라이브러리

React에서 npm 패키지로 컴포넌트를 공유하듯, Blazor는 Razor Class Library(RCL)로 컴포넌트를 패키징합니다.

### RCL 생성

```bash
dotnet new razorclasslib -n MyComponentLibrary
cd MyComponentLibrary
```

구조:

```
MyComponentLibrary/
  Component1.razor
  ExampleJsInterop.cs
  wwwroot/
    background.png
    exampleJsInterop.js
```

### 컴포넌트 작성

::: v-pre
```razor
@* Card.razor *@
<div class="card @CssClass">
  @if (Header != null)
  {
    <div class="card-header">
      @Header
    </div>
  }
  <div class="card-body">
    @ChildContent
  </div>
  @if (Footer != null)
  {
    <div class="card-footer">
      @Footer
    </div>
  }
</div>

@code {
  [Parameter]
  public RenderFragment? Header { get; set; }

  [Parameter]
  public RenderFragment? ChildContent { get; set; }

  [Parameter]
  public RenderFragment? Footer { get; set; }

  [Parameter]
  public string CssClass { get; set; } = string.Empty;
}
```
:::

### NuGet 패키지로 배포

```bash
dotnet pack -c Release
dotnet nuget push bin/Release/MyComponentLibrary.1.0.0.nupkg -s https://api.nuget.org/v3/index.json
```

### 라이브러리 사용

```bash
dotnet add package MyComponentLibrary
```

::: v-pre
```razor
@using MyComponentLibrary

<Card CssClass="my-card">
  <Header>
    <h2>제목</h2>
  </Header>
  <ChildContent>
    <p>본문</p>
  </ChildContent>
  <Footer>
    <button>확인</button>
  </Footer>
</Card>
```
:::

**인기 있는 Blazor 컴포넌트 라이브러리:**

- **MudBlazor**: Material Design 기반, 80+ 컴포넌트
- **Radzen Blazor**: 무료 오픈소스, 70+ 컴포넌트
- **Blazorise**: Bootstrap/Material/Ant Design 지원
- **Syncfusion**: 엔터프라이즈급 (유료)

## 성능 최적화: 빠른 애플리케이션 만들기

React에서 `React.memo`, `useMemo`, `useCallback`으로 최적화하듯, Blazor에도 여러 최적화 기법이 있습니다.

### Virtualization: 큰 리스트 렌더링

React Virtualized나 react-window와 동일한 개념입니다.

::: v-pre
```razor
@using Microsoft.AspNetCore.Components.Web.Virtualization

<h1>가상화된 리스트</h1>

<Virtualize Items="@allItems" Context="item">
  <div class="item">
    <h3>@item.Title</h3>
    <p>@item.Description</p>
  </div>
</Virtualize>

@code {
  private List<Item> allItems = Enumerable.Range(1, 100000)
    .Select(i => new Item
    {
      Id = i,
      Title = $"항목 {i}",
      Description = $"설명 {i}"
    })
    .ToList();
}
```
:::

화면에 보이는 항목만 렌더링하므로, 10만 개의 항목도 부드럽게 스크롤됩니다.

**비동기 데이터 로딩:**

::: v-pre
```razor
<Virtualize ItemsProvider="@LoadItems" Context="item">
  <ItemContent>
    <div>@item.Title</div>
  </ItemContent>
  <Placeholder>
    <div>로딩 중...</div>
  </Placeholder>
</Virtualize>

@code {
  private async ValueTask<ItemsProviderResult<Item>> LoadItems(
    ItemsProviderRequest request)
  {
    // request.StartIndex와 request.Count를 사용하여 페이징
    var items = await Http.GetFromJsonAsync<Item[]>(
      $"/api/items?start={request.StartIndex}&count={request.Count}"
    );

    return new ItemsProviderResult<Item>(items, totalItemCount: 100000);
  }
}
```
:::

스크롤할 때마다 필요한 데이터만 서버에서 가져옵니다. 무한 스크롤의 최적화된 버전입니다.

### `@key` 지시문: React의 key prop

리스트 렌더링 시 각 항목을 고유하게 식별합니다.

::: v-pre
```razor
<ul>
  @foreach (var todo in todos)
  {
    <li @key="todo.Id">  @* Id로 항목 식별 *@
      <TodoItem Todo="@todo" />
    </li>
  }
</ul>
```
:::

React 버전:

```jsx
<ul>
  {todos.map(todo => (
    <li key={todo.id}>
      <TodoItem todo={todo} />
    </li>
  ))}
</ul>
```

`@key`가 없으면 Blazor는 항목의 순서로만 식별하여, 리스트 중간에 항목을 추가/삭제할 때 비효율적입니다.

### Lazy Loading: 코드 분할

React의 `React.lazy`와 유사하게, Blazor는 어셈블리를 지연 로드할 수 있습니다.

```csharp
// Program.cs
builder.Services.AddScoped(sp => new HttpClient
{
  BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Lazy load assemblies
var assemblies = new[] { "MyApp.AdminModule.dll" };
```

::: v-pre
```razor
@* App.razor *@
<Router AppAssembly="@typeof(App).Assembly"
        AdditionalAssemblies="@lazyLoadedAssemblies">
  <Navigating>
    <div>페이지 로딩 중...</div>
  </Navigating>
  <Found Context="routeData">
    <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
  </Found>
</Router>

@code {
  private List<Assembly> lazyLoadedAssemblies = new();

  protected override async Task OnInitializedAsync()
  {
    // 필요할 때만 AdminModule 로드
    if (isAdmin)
    {
      var assemblies = await LazyLoadAssembly(new[] { "MyApp.AdminModule.dll" });
      lazyLoadedAssemblies.AddRange(assemblies);
    }
  }
}
```
:::

### Prerendering: SSR 같은 초기 로딩

Blazor WebAssembly는 초기 로딩이 느릴 수 있습니다. Prerendering은 서버에서 초기 HTML을 렌더링하여 전송합니다.

```csharp
// Program.cs (Server 프로젝트)
app.MapFallbackToPage("/_Host");
```

::: v-pre
```razor
@* Pages/_Host.cshtml *@
@page "/"
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html>
<head>
  <title>My Blazor App</title>
  <base href="~/" />
</head>
<body>
  <component type="typeof(App)" render-mode="WebAssemblyPrerendered" />
  <script src="_framework/blazor.webassembly.js"></script>
</body>
</html>
```
:::

`render-mode="WebAssemblyPrerendered"`:

1. 서버에서 초기 HTML 렌더링
2. 브라우저가 HTML 즉시 표시 (빠른 First Paint)
3. WebAssembly 런타임 다운로드 및 로드
4. Blazor가 "hydration" 수행 (정적 HTML을 인터랙티브하게 만듦)

Next.js의 SSR과 거의 동일한 경험입니다.

## 인증과 권한 부여: 보안 애플리케이션

React에서 NextAuth나 Auth0를 사용하듯, Blazor는 ASP.NET Core Identity와 통합됩니다.

### AuthorizeView 컴포넌트

::: v-pre
```razor
<AuthorizeView>
  <Authorized>
    <h1>환영합니다, @context.User.Identity.Name님!</h1>
    <button @onclick="Logout">로그아웃</button>
  </Authorized>
  <NotAuthorized>
    <h1>로그인이 필요합니다.</h1>
    <a href="/login">로그인</a>
  </NotAuthorized>
</AuthorizeView>
```
:::

React에서 조건부 렌더링으로 하던 것을 컴포넌트로 제공합니다.

### 역할 기반 UI

::: v-pre
```razor
<AuthorizeView Roles="Admin, Manager">
  <Authorized>
    <NavLink href="/admin">관리자 페이지</NavLink>
  </Authorized>
</AuthorizeView>

<AuthorizeView Policy="CanDeleteUsers">
  <Authorized>
    <button @onclick="DeleteUser">사용자 삭제</button>
  </Authorized>
  <NotAuthorized>
    <p>권한이 없습니다.</p>
  </NotAuthorized>
</AuthorizeView>
```
:::

### 페이지 수준 권한

::: v-pre
```razor
@page "/admin/dashboard"
@attribute [Authorize(Roles = "Admin")]

<h1>관리자 대시보드</h1>
```
:::

권한이 없는 사용자가 접근하면 자동으로 로그인 페이지로 리디렉션됩니다.

### 사용자 정보 접근

::: v-pre
```razor
@inject AuthenticationStateProvider AuthStateProvider

<p>사용자: @userName</p>
<p>이메일: @userEmail</p>

@code {
  private string userName = "";
  private string userEmail = "";

  protected override async Task OnInitializedAsync()
  {
    var authState = await AuthStateProvider.GetAuthenticationStateAsync();
    var user = authState.User;

    if (user.Identity?.IsAuthenticated == true)
    {
      userName = user.Identity.Name ?? "";
      userEmail = user.FindFirst(c => c.Type == "email")?.Value ?? "";
    }
  }
}
```
:::

## SignalR 통합: 실시간 데이터 업데이트

Blazor Server는 기본적으로 SignalR을 사용하지만, Blazor WebAssembly도 SignalR Hub에 연결하여 실시간 통신을 할 수 있습니다.

### SignalR Hub 생성 (서버)

```csharp
// Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
  public async Task SendMessage(string user, string message)
  {
    // 모든 클라이언트에게 메시지 브로드캐스트
    await Clients.All.SendAsync("ReceiveMessage", user, message);
  }

  public async Task SendToGroup(string groupName, string user, string message)
  {
    await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
  }

  public async Task JoinGroup(string groupName)
  {
    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    await Clients.Group(groupName).SendAsync("SystemMessage",
      $"{Context.User?.Identity?.Name} joined the group");
  }
}
```

```csharp
// Program.cs
builder.Services.AddSignalR();

app.MapHub<ChatHub>("/chathub");
```

### Blazor 클라이언트에서 연결

::: v-pre
```razor
@page "/chat"
@inject NavigationManager Navigation
@using Microsoft.AspNetCore.SignalR.Client
@implements IAsyncDisposable

<div class="chat-container">
  <div class="messages">
    @foreach (var message in messages)
    {
      <div class="message">
        <strong>@message.User:</strong> @message.Text
      </div>
    }
  </div>

  <div class="input-area">
    <input @bind="currentMessage" @bind:event="oninput"
           @onkeyup="@(e => e.Key == "Enter" ? Send() : Task.CompletedTask)" />
    <button @onclick="Send" disabled="@(!IsConnected)">전송</button>
  </div>
</div>

@code {
  private HubConnection? hubConnection;
  private List<ChatMessage> messages = new();
  private string currentMessage = "";
  private string userName = "익명";

  protected override async Task OnInitializedAsync()
  {
    // SignalR 연결 설정
    hubConnection = new HubConnectionBuilder()
      .WithUrl(Navigation.ToAbsoluteUri("/chathub"))
      .Build();

    // 메시지 수신 핸들러
    hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
    {
      messages.Add(new ChatMessage { User = user, Text = message });
      StateHasChanged();  // UI 업데이트
    });

    await hubConnection.StartAsync();
  }

  private async Task Send()
  {
    if (hubConnection != null && !string.IsNullOrWhiteSpace(currentMessage))
    {
      await hubConnection.SendAsync("SendMessage", userName, currentMessage);
      currentMessage = "";
    }
  }

  private bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

  public async ValueTask DisposeAsync()
  {
    if (hubConnection != null)
    {
      await hubConnection.DisposeAsync();
    }
  }

  private class ChatMessage
  {
    public string User { get; set; } = "";
    public string Text { get; set; } = "";
  }
}
```
:::

### 실시간 대시보드 예시

::: v-pre
```razor
@page "/dashboard"
@inject HubConnection HubConnection
@implements IAsyncDisposable

<h1>실시간 대시보드</h1>

<div class="stats">
  <div class="stat-card">
    <h3>활성 사용자</h3>
    <p class="big-number">@activeUsers</p>
  </div>
  <div class="stat-card">
    <h3>오늘 매출</h3>
    <p class="big-number">@todaySales.ToString("C")</p>
  </div>
  <div class="stat-card">
    <h3>처리 중인 주문</h3>
    <p class="big-number">@pendingOrders</p>
  </div>
</div>

<h2>최근 주문</h2>
<table>
  <thead>
    <tr>
      <th>주문번호</th>
      <th>고객</th>
      <th>금액</th>
      <th>상태</th>
    </tr>
  </thead>
  <tbody>
    @foreach (var order in recentOrders.Take(10))
    {
      <tr>
        <td>@order.Id</td>
        <td>@order.CustomerName</td>
        <td>@order.Amount.ToString("C")</td>
        <td>@order.Status</td>
      </tr>
    }
  </tbody>
</table>

@code {
  private int activeUsers = 0;
  private decimal todaySales = 0;
  private int pendingOrders = 0;
  private List<Order> recentOrders = new();

  protected override async Task OnInitializedAsync()
  {
    HubConnection = new HubConnectionBuilder()
      .WithUrl(Navigation.ToAbsoluteUri("/dashboardhub"))
      .WithAutomaticReconnect()  // 자동 재연결
      .Build();

    // 통계 업데이트 수신
    HubConnection.On<DashboardStats>("UpdateStats", stats =>
    {
      activeUsers = stats.ActiveUsers;
      todaySales = stats.TodaySales;
      pendingOrders = stats.PendingOrders;
      StateHasChanged();
    });

    // 새 주문 수신
    HubConnection.On<Order>("NewOrder", order =>
    {
      recentOrders.Insert(0, order);
      if (recentOrders.Count > 100)
      {
        recentOrders = recentOrders.Take(100).ToList();
      }
      StateHasChanged();
    });

    await HubConnection.StartAsync();

    // 초기 데이터 로드
    var initialStats = await Http.GetFromJsonAsync<DashboardStats>("/api/dashboard/stats");
    activeUsers = initialStats.ActiveUsers;
    todaySales = initialStats.TodaySales;
    pendingOrders = initialStats.PendingOrders;
  }

  public async ValueTask DisposeAsync()
  {
    if (HubConnection != null)
    {
      await HubConnection.DisposeAsync();
    }
  }
}
```
:::

## 에러 처리: ErrorBoundary

React의 Error Boundary와 동일한 개념입니다.

::: v-pre
```razor
<ErrorBoundary>
  <ChildContent>
    <MyComponent />  @* 여기서 오류가 발생하면 ErrorContent 표시 *@
  </ChildContent>
  <ErrorContent Context="exception">
    <div class="error-message">
      <h2>오류가 발생했습니다</h2>
      <p>@exception.Message</p>
      <button @onclick="@(() => exception.Recover())">다시 시도</button>
    </div>
  </ErrorContent>
</ErrorBoundary>
```
:::

전역 에러 경계:

::: v-pre
```razor
@* App.razor *@
<Router AppAssembly="@typeof(App).Assembly">
  <Found Context="routeData">
    <ErrorBoundary>
      <ChildContent>
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
      </ChildContent>
      <ErrorContent Context="exception">
        <div class="error-page">
          <h1>앗! 문제가 발생했습니다</h1>
          <p>예상치 못한 오류가 발생했습니다.</p>
          <details>
            <summary>기술 세부정보</summary>
            <pre>@exception.ToString()</pre>
          </details>
          <a href="/">홈으로 돌아가기</a>
        </div>
      </ErrorContent>
    </ErrorBoundary>
  </Found>
</Router>
```
:::

## 상태 관리 패턴: 복잡한 상태 다루기

React에서 Redux, Zustand, Jotai를 사용하듯, Blazor에도 여러 상태 관리 전략이 있습니다.

### 1. 서비스 기반 상태 관리

```csharp
// Services/AppState.cs
public class AppState
{
  private string currentUser = "";
  public string CurrentUser
  {
    get => currentUser;
    set
    {
      currentUser = value;
      NotifyStateChanged();
    }
  }

  private int cartItemCount = 0;
  public int CartItemCount
  {
    get => cartItemCount;
    set
    {
      cartItemCount = value;
      NotifyStateChanged();
    }
  }

  public event Action? OnChange;

  private void NotifyStateChanged() => OnChange?.Invoke();
}
```

```csharp
// Program.cs
builder.Services.AddScoped<AppState>();
```

컴포넌트에서 사용:

::: v-pre
```razor
@inject AppState AppState
@implements IDisposable

<p>사용자: @AppState.CurrentUser</p>
<p>장바구니: @AppState.CartItemCount개</p>

<button @onclick="AddToCart">장바구니에 추가</button>

@code {
  protected override void OnInitialized()
  {
    AppState.OnChange += StateHasChanged;
  }

  private void AddToCart()
  {
    AppState.CartItemCount++;
  }

  public void Dispose()
  {
    AppState.OnChange -= StateHasChanged;
  }
}
```
:::

### 2. Fluxor: Redux 패턴

```bash
dotnet add package Fluxor.Blazor.Web
```

```csharp
// Store/Counter/CounterState.cs
public record CounterState
{
  public int Count { get; init; }
}

// Store/Counter/CounterActions.cs
public record IncrementCounterAction;
public record DecrementCounterAction;

// Store/Counter/CounterReducer.cs
public static class CounterReducers
{
  [ReducerMethod]
  public static CounterState OnIncrement(CounterState state, IncrementCounterAction action)
    => state with { Count = state.Count + 1 };

  [ReducerMethod]
  public static CounterState OnDecrement(CounterState state, DecrementCounterAction action)
    => state with { Count = state.Count - 1 };
}
```

::: v-pre
```razor
@page "/fluxor-counter"
@using Fluxor
@inject IState<CounterState> CounterState
@inject IDispatcher Dispatcher

<h1>Fluxor 카운터</h1>
<p>현재 값: @CounterState.Value.Count</p>
<button @onclick="Increment">증가</button>
<button @onclick="Decrement">감소</button>

@code {
  private void Increment() => Dispatcher.Dispatch(new IncrementCounterAction());
  private void Decrement() => Dispatcher.Dispatch(new DecrementCounterAction());
}
```
:::

Redux 개발 경험과 거의 동일합니다!

## 실전 프로젝트: 실시간 대시보드 완성

Part 4의 모든 지식을 종합하여 실시간 대시보드를 만들어봅시다.

**기능:**
- SignalR로 실시간 데이터 업데이트
- 인증 필요 (관리자만 접근)
- 차트 라이브러리 통합 (Chart.js)
- 가상화된 주문 목록
- 반응형 레이아웃

이 프로젝트는 별도의 실습 폴더에서 단계별로 구현합니다.

## 마무리: Blazor 마스터로의 여정

Part 4를 완료했습니다! C#으로 프론트엔드를 작성하는 혁명적인 경험을 했습니다.

**배운 것:**

- Chapter 9: Blazor의 기초, WebAssembly의 혁명, 호스팅 모델
- Chapter 10: 컴포넌트 개발, 상태 관리, 폼 유효성 검사, JS Interop
- Chapter 11: 라우팅, 레이아웃, 성능 최적화, 인증, SignalR, 상태 관리 패턴

**다음 단계:**

Part 5에서는 Entity Framework Core로 데이터베이스를 다룹니다. Blazor UI와 EF Core 데이터 계층을 결합하면 완전한 풀스택 애플리케이션이 완성됩니다.

Part 6에서는 RESTful API, GraphQL, SignalR을 깊이 탐구합니다.

Part 7에서는 Blazor와 React/Vue를 함께 사용하는 하이브리드 전략을 배웁니다.

프로덕션 수준의 Blazor 애플리케이션을 만들 준비가 되었습니다. 계속 학습하고, 실험하고, 빌드하세요!

---

## 학습 자료

- [Blazor 라우팅 공식 문서](https://docs.microsoft.com/aspnet/core/blazor/fundamentals/routing)
- [Blazor 레이아웃 가이드](https://docs.microsoft.com/aspnet/core/blazor/layouts)
- [성능 최적화 모범 사례](https://docs.microsoft.com/aspnet/core/blazor/performance)
- [SignalR with Blazor](https://docs.microsoft.com/aspnet/core/blazor/tutorials/signalr-blazor)
- [Fluxor GitHub](https://github.com/mrpmorris/Fluxor)

이전 챕터: [Chapter 10: Blazor 컴포넌트 개발](../chapter10/README.md)
