# Chapter 10: Blazor 컴포넌트 개발

## 컴포넌트: 재사용 가능한 UI의 핵심

React를 배울 때 가장 먼저 배우는 개념이 무엇인가요? 바로 컴포넌트입니다. "UI를 독립적이고 재사용 가능한 조각으로 나눈다"는 아이디어는 현대 프론트엔드 개발의 근간입니다. Vue의 단일 파일 컴포넌트(.vue), Angular의 컴포넌트 클래스, Svelte의 컴포넌트—모두 같은 철학을 공유합니다.

Blazor 역시 컴포넌트 기반 아키텍처를 따릅니다. `.razor` 파일 하나가 하나의 컴포넌트이며, 마크업과 로직이 함께 있습니다. React의 JSX와 매우 유사한 경험을 제공하지만, C#의 강력한 타입 시스템과 .NET 생태계의 혜택을 받습니다.

이 챕터에서는 Blazor 컴포넌트의 모든 측면을 마스터합니다. 프론트엔드 개발자로서 이미 알고 있는 개념들을 Blazor 방식으로 재해석하며, React/Vue에서 익숙한 패턴을 C#으로 구현하는 방법을 배웁니다.

## `.razor` 파일 구조: 마크업과 코드의 공존

Blazor 컴포넌트는 `.razor` 확장자를 가진 파일입니다. Vue의 단일 파일 컴포넌트처럼, 한 파일에 템플릿(HTML)과 로직(C#)이 함께 있습니다.

### 기본 구조

::: v-pre
```razor
@* TodoItem.razor *@

@* 1. 마크업 섹션 (템플릿) *@
<div class="todo-item @(todo.IsCompleted ? "completed" : "")">
  <input type="checkbox" checked="@todo.IsCompleted" @onchange="ToggleComplete" />
  <span>@todo.Title</span>
  <button @onclick="OnDelete">삭제</button>
</div>

@* 2. 코드 섹션 *@
@code {
  [Parameter]
  public TodoItemModel Todo { get; set; } = default!;

  [Parameter]
  public EventCallback<int> OnDelete { get; set; }

  private void ToggleComplete()
  {
    todo.IsCompleted = !todo.IsCompleted;
  }
}
```
:::

Vue SFC와 비교:

```vue
<!-- TodoItem.vue -->
<template>
  <div :class="['todo-item', todo.isCompleted ? 'completed' : '']">
    <input type="checkbox" :checked="todo.isCompleted" @change="toggleComplete" />
    <span>{{ todo.title }}</span>
    <button @click="$emit('delete')">삭제</button>
  </div>
</template>

<script>
export default {
  props: {
    todo: Object
  },
  methods: {
    toggleComplete() {
      this.todo.isCompleted = !this.todo.isCompleted;
    }
  }
}
</script>
```

구조적으로 매우 유사합니다. 주요 차이점:

1. **구분자**: Vue는 `<template>`, `<script>` 태그로 분리. Blazor는 `@code {}` 블록으로 분리
2. **Props**: Vue는 `props`, Blazor는 `[Parameter]` 특성
3. **이벤트**: Vue는 `$emit`, Blazor는 `EventCallback`

### Code-behind 패턴 (선택적)

코드가 많아지면 마크업과 분리할 수 있습니다.

```csharp
// TodoItem.razor.cs (코드 비하인드)
public partial class TodoItem : ComponentBase
{
  [Parameter]
  public TodoItemModel Todo { get; set; } = default!;

  [Parameter]
  public EventCallback<int> OnDelete { get; set; }

  private void ToggleComplete()
  {
    Todo.IsCompleted = !Todo.IsCompleted;
  }
}
```

::: v-pre
```razor
@* TodoItem.razor (마크업만) *@
<div class="todo-item @(Todo.IsCompleted ? "completed" : "")">
  <input type="checkbox" checked="@Todo.IsCompleted" @onchange="ToggleComplete" />
  <span>@Todo.Title</span>
  <button @onclick="OnDelete">삭제</button>
</div>
```
:::

`partial` 키워드가 핵심입니다. Razor 컴파일러가 `.razor` 파일을 컴파일할 때 같은 이름의 partial 클래스를 생성하므로, 두 파일이 하나의 클래스로 병합됩니다.

React에서는 마크업과 로직이 항상 함께 있지만, Blazor는 선택할 수 있습니다. 작은 컴포넌트는 `.razor`에 모두 작성하고, 큰 컴포넌트는 분리하는 것이 일반적입니다.

## 로컬 상태 관리: 컴포넌트의 기억

React의 `useState`, Vue의 `ref/reactive`처럼, Blazor 컴포넌트도 자체 상태를 가질 수 있습니다.

### 필드를 사용한 상태

::: v-pre
```razor
@page "/counter"

<h1>카운터</h1>
<p>현재 값: @count</p>
<button @onclick="Increment">증가</button>
<button @onclick="Decrement">감소</button>
<button @onclick="Reset">리셋</button>

@code {
  private int count = 0;  // 상태

  private void Increment() => count++;
  private void Decrement() => count--;
  private void Reset() => count = 0;
}
```
:::

React 버전:

::: v-pre
```jsx
function Counter() {
  const [count, setCount] = useState(0);

  return (
    <div>
      <h1>카운터</h1>
      <p>현재 값: {count}</p>
      <button onClick={() => setCount(count + 1)}>증가</button>
      <button onClick={() => setCount(count - 1)}>감소</button>
      <button onClick={() => setCount(0)}>리셋</button>
    </div>
  );
}
```
:::

**핵심 차이점:**

1. **상태 업데이트**: React는 `setCount` 필수, Blazor는 직접 변경 (`count++`)
2. **재렌더링**: React는 `setCount`가 트리거, Blazor는 이벤트 핸들러 종료 시 자동
3. **타입**: React는 타입 추론 또는 TypeScript, Blazor는 컴파일 타임 타입 (`int`)

### 복잡한 상태: 객체와 리스트

::: v-pre
```razor
@page "/todo"

<h1>할 일 목록</h1>

<input @bind="newTodoTitle" @bind:event="oninput" placeholder="새 할 일" />
<button @onclick="AddTodo">추가</button>

<ul>
  @foreach (var todo in todos)
  {
    <li>
      <input type="checkbox" checked="@todo.IsCompleted"
             @onchange="() => ToggleTodo(todo.Id)" />
      @todo.Title
      <button @onclick="() => DeleteTodo(todo.Id)">삭제</button>
    </li>
  }
</ul>

<p>완료: @todos.Count(t => t.IsCompleted) / @todos.Count</p>

@code {
  private List<TodoItemModel> todos = new();
  private string newTodoTitle = string.Empty;
  private int nextId = 1;

  private void AddTodo()
  {
    if (!string.IsNullOrWhiteSpace(newTodoTitle))
    {
      todos.Add(new TodoItemModel
      {
        Id = nextId++,
        Title = newTodoTitle,
        IsCompleted = false
      });
      newTodoTitle = string.Empty;
    }
  }

  private void ToggleTodo(int id)
  {
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo != null)
    {
      todo.IsCompleted = !todo.IsCompleted;
    }
  }

  private void DeleteTodo(int id)
  {
    todos.RemoveAll(t => t.Id == id);
  }
}

public class TodoItemModel
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public bool IsCompleted { get; set; }
}
```
:::

React 버전 (비교):

::: v-pre
```jsx
function TodoList() {
  const [todos, setTodos] = useState([]);
  const [newTodoTitle, setNewTodoTitle] = useState('');
  const [nextId, setNextId] = useState(1);

  const addTodo = () => {
    if (newTodoTitle.trim()) {
      setTodos([...todos, {
        id: nextId,
        title: newTodoTitle,
        isCompleted: false
      }]);
      setNextId(nextId + 1);
      setNewTodoTitle('');
    }
  };

  const toggleTodo = (id) => {
    setTodos(todos.map(todo =>
      todo.id === id ? { ...todo, isCompleted: !todo.isCompleted } : todo
    ));
  };

  const deleteTodo = (id) => {
    setTodos(todos.filter(todo => todo.id !== id));
  };

  return (
    // JSX...
  );
}
```
:::

**중요한 차이:**

React는 불변성(immutability)을 요구합니다. 상태를 변경하려면 새 객체/배열을 만들어야 합니다 (`[...todos]`, `{...todo}`). Blazor는 직접 변경(mutation)이 가능합니다 (`todos.Add()`, `todo.IsCompleted = !todo.IsCompleted`).

이는 C#의 참조 타입 특성 때문입니다. Blazor는 이벤트 핸들러가 종료될 때 `StateHasChanged()`를 자동 호출하여 재렌더링을 트리거합니다.

### `StateHasChanged()`: 수동 재렌더링

대부분의 경우 자동이지만, 때로는 수동으로 재렌더링을 트리거해야 합니다.

```razor
@code {
  private string message = "대기 중...";

  protected override async Task OnInitializedAsync()
  {
    // 비동기 작업 중간에 UI 업데이트
    message = "데이터 로딩 중...";
    StateHasChanged();  // 즉시 재렌더링

    await Task.Delay(2000);  // 시뮬레이션

    message = "완료!";
    // 메서드 종료 시 자동 재렌더링되므로 StateHasChanged() 불필요
  }
}
```

React의 `forceUpdate()`와 유사하지만, Blazor에서는 거의 필요하지 않습니다.

## Parameters: 부모-자식 간 데이터 전달

React의 props와 정확히 같은 개념입니다.

### 기본 Parameter

::: v-pre
```razor
@* UserCard.razor *@
<div class="user-card">
  <img src="@AvatarUrl" alt="@Name" />
  <h3>@Name</h3>
  <p>@Email</p>
  @if (ShowBio && !string.IsNullOrEmpty(Bio))
  {
    <p class="bio">@Bio</p>
  }
</div>

@code {
  [Parameter]
  public string Name { get; set; } = string.Empty;

  [Parameter]
  public string Email { get; set; } = string.Empty;

  [Parameter]
  public string AvatarUrl { get; set; } = "/images/default-avatar.png";

  [Parameter]
  public string? Bio { get; set; }

  [Parameter]
  public bool ShowBio { get; set; } = true;
}
```
:::

사용법:

::: v-pre
```razor
<UserCard Name="홍길동"
          Email="hong@example.com"
          AvatarUrl="/images/hong.jpg"
          Bio="Blazor 개발자입니다."
          ShowBio="true" />
```
:::

React/TypeScript 버전:

::: v-pre
```tsx
interface UserCardProps {
  name: string;
  email: string;
  avatarUrl?: string;
  bio?: string;
  showBio?: boolean;
}

function UserCard({
  name,
  email,
  avatarUrl = '/images/default-avatar.png',
  bio,
  showBio = true
}: UserCardProps) {
  return (
    <div className="user-card">
      <img src={avatarUrl} alt={name} />
      <h3>{name}</h3>
      <p>{email}</p>
      {showBio && bio && <p className="bio">{bio}</p>}
    </div>
  );
}

// 사용
<UserCard
  name="홍길동"
  email="hong@example.com"
  avatarUrl="/images/hong.jpg"
  bio="Blazor 개발자입니다."
/>
```
:::

**비교:**

1. **타입 선언**: React는 interface, Blazor는 `[Parameter]` 특성
2. **기본값**: React는 매개변수 기본값 또는 `defaultProps`, Blazor는 프로퍼티 초기화
3. **필수/선택**: React는 TypeScript의 `?`, Blazor는 nullable 참조 타입 (`string?`)

### Required Parameter (.NET 7+)

```razor
@code {
  [Parameter, EditorRequired]  // 또는 [Parameter] public required string Name
  public string Name { get; set; } = default!;

  [Parameter, EditorRequired]
  public int UserId { get; set; }
}
```

이 컴포넌트를 사용할 때 `Name`과 `UserId`를 제공하지 않으면 컴파일 경고(IDE에서)가 발생합니다.

### ChildContent: React의 `children`

::: v-pre
```razor
@* Card.razor *@
<div class="card">
  <div class="card-header">
    @Header
  </div>
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
}
```
:::

사용법:

::: v-pre
```razor
<Card>
  <Header>
    <h2>제목</h2>
  </Header>
  <ChildContent>
    <p>본문 내용입니다.</p>
  </ChildContent>
  <Footer>
    <button>확인</button>
  </Footer>
</Card>
```
:::

React 버전:

```jsx
function Card({ header, children, footer }) {
  return (
    React.createElement('div', { className: 'card' },
      header && React.createElement('div', { className: 'card-header' }, header),
      React.createElement('div', { className: 'card-body' }, children),
      footer && React.createElement('div', { className: 'card-footer' }, footer)
    )
  );
}

// 사용 (JSX)
// <Card
//   header={<h2>제목</h2>}
//   footer={<button>확인</button>}
// >
//   <p>본문 내용입니다.</p>
// </Card>
```

`RenderFragment`는 Blazor의 특별한 타입으로, 렌더링할 수 있는 UI 조각을 나타냅니다. React의 `ReactNode`와 유사합니다.

## EventCallback: 자식에서 부모로 이벤트 전달

React의 콜백 prop과 동일한 개념입니다.

### 기본 EventCallback

::: v-pre
```razor
@* DeleteButton.razor *@
<button class="btn btn-danger" @onclick="HandleClick" disabled="@IsDeleting">
  @if (IsDeleting)
  {
    <span>삭제 중...</span>
  }
  else
  {
    <span>삭제</span>
  }
</button>

@code {
  [Parameter]
  public EventCallback OnDelete { get; set; }

  private bool IsDeleting = false;

  private async Task HandleClick()
  {
    IsDeleting = true;
    await OnDelete.InvokeAsync();
    IsDeleting = false;
  }
}
```
:::

사용법:

::: v-pre
```razor
<DeleteButton OnDelete="@DeleteItem" />

@code {
  private async Task DeleteItem()
  {
    await Task.Delay(1000);  // API 호출 시뮬레이션
    // 실제 삭제 로직
  }
}
```
:::

React 버전:

::: v-pre
```jsx
function DeleteButton({ onDelete }) {
  const [isDeleting, setIsDeleting] = useState(false);

  const handleClick = async () => {
    setIsDeleting(true);
    await onDelete();
    setIsDeleting(false);
  };

  return (
    <button onClick={handleClick} disabled={isDeleting}>
      {isDeleting ? <span>삭제 중...</span> : <span>삭제</span>}
    </button>
  );
}

// 사용
<DeleteButton onDelete={async () => {
  await new Promise(resolve => setTimeout(resolve, 1000));
  // 실제 삭제 로직
}} />
```
:::

### EventCallback<T>: 매개변수 전달

::: v-pre
```razor
@* TodoItem.razor *@
<li class="todo-item">
  <input type="checkbox" checked="@Todo.IsCompleted"
         @onchange="() => OnToggle.InvokeAsync(Todo.Id)" />
  <span>@Todo.Title</span>
  <button @onclick="() => OnDelete.InvokeAsync(Todo.Id)">삭제</button>
</li>

@code {
  [Parameter]
  public TodoItemModel Todo { get; set; } = default!;

  [Parameter]
  public EventCallback<int> OnToggle { get; set; }

  [Parameter]
  public EventCallback<int> OnDelete { get; set; }
}
```
:::

부모 컴포넌트:

::: v-pre
```razor
<ul>
  @foreach (var todo in todos)
  {
    <TodoItem Todo="@todo"
              OnToggle="@ToggleTodo"
              OnDelete="@DeleteTodo" />
  }
</ul>

@code {
  private List<TodoItemModel> todos = new();

  private void ToggleTodo(int id)
  {
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo != null)
    {
      todo.IsCompleted = !todo.IsCompleted;
    }
  }

  private void DeleteTodo(int id)
  {
    todos.RemoveAll(t => t.Id == id);
  }
}
```
:::

React에서는 `(id) => onDelete(id)`처럼 콜백을 전달하지만, Blazor는 `EventCallback<int>`로 타입 안정성을 제공합니다.

## 양방향 바인딩: `@bind`

React는 단방향 바인딩만 지원하지만, Vue와 Angular는 양방향 바인딩(`v-model`, `[(ngModel)]`)을 제공합니다. Blazor도 마찬가지입니다.

### 입력 필드 바인딩

::: v-pre
```razor
<input type="text" @bind="username" />
<p>입력한 값: @username</p>

@code {
  private string username = string.Empty;
}
```
:::

이것은 다음과 동일합니다:

::: v-pre
```razor
<input type="text" value="@username"
       @onchange="@((ChangeEventArgs e) => username = e.Value?.ToString() ?? string.Empty)" />
```
:::

React 버전 (수동 양방향 바인딩):

::: v-pre
```jsx
const [username, setUsername] = useState('');

<input type="text" value={username}
       onChange={(e) => setUsername(e.target.value)} />
<p>입력한 값: {username}</p>
```
:::

Blazor의 `@bind`가 훨씬 간결합니다!

### 이벤트 타이밍 지정

기본적으로 `@bind`는 `onchange` 이벤트를 사용합니다 (포커스를 잃을 때 업데이트). 실시간으로 업데이트하려면:

::: v-pre
```razor
<input type="text" @bind="searchQuery" @bind:event="oninput" />
<p>검색 중: @searchQuery</p>

@code {
  private string searchQuery = string.Empty;
}
```
:::

`@bind:event="oninput"`은 키를 누를 때마다 업데이트합니다.

### 체크박스, 셀렉트 등

::: v-pre
```razor
<input type="checkbox" @bind="agreeToTerms" />
<label>약관에 동의합니다</label>

<select @bind="selectedCountry">
  <option value="">선택하세요</option>
  <option value="KR">한국</option>
  <option value="US">미국</option>
  <option value="JP">일본</option>
</select>

<input type="date" @bind="birthDate" />

@code {
  private bool agreeToTerms = false;
  private string selectedCountry = string.Empty;
  private DateTime birthDate = DateTime.Today;
}
```
:::

### 커스텀 컴포넌트에 @bind 지원

자체 컴포넌트에서 `@bind`를 지원하려면 규칙을 따라야 합니다:

::: v-pre
```razor
@* CustomInput.razor *@
<input type="text" value="@Value"
       @oninput="@(e => ValueChanged.InvokeAsync(e.Value?.ToString()))" />

@code {
  [Parameter]
  public string? Value { get; set; }

  [Parameter]
  public EventCallback<string> ValueChanged { get; set; }
}
```
:::

사용법:

::: v-pre
```razor
<CustomInput @bind-Value="myText" />

@code {
  private string myText = string.Empty;
}
```
:::

규칙: `Value` Parameter와 `ValueChanged` EventCallback이 쌍을 이루면 `@bind-Value`를 사용할 수 있습니다.

## 생명주기 메서드: 컴포넌트의 인생

React의 생명주기 메서드(또는 Hooks)처럼, Blazor 컴포넌트도 생성부터 소멸까지 여러 단계를 거칩니다.

### 주요 생명주기 메서드

| 생명주기 | 호출 시점 | React Hook 비교 |
|---------|---------|----------------|
| `SetParametersAsync` | Parameter 설정 전 (매번) | - |
| `OnInitialized` / `OnInitializedAsync` | 컴포넌트 처음 생성 | `useEffect(() => {}, [])` |
| `OnParametersSet` / `OnParametersSetAsync` | Parameter 설정 후 (매번) | `useEffect(() => {}, [deps])` |
| `OnAfterRender` / `OnAfterRenderAsync` | 렌더링 후 | `useLayoutEffect` |
| `Dispose` (IDisposable) | 컴포넌트 제거 | `useEffect return` (cleanup) |

### OnInitialized: 초기화

::: v-pre
```razor
@page "/user/{UserId:int}"
@inject HttpClient Http

@if (user == null)
{
  <p>로딩 중...</p>
}
else
{
  <div>
    <h1>@user.Name</h1>
    <p>@user.Email</p>
  </div>
}

@code {
  [Parameter]
  public int UserId { get; set; }

  private User? user;

  protected override async Task OnInitializedAsync()
  {
    // 컴포넌트가 처음 만들어질 때 한 번만 실행
    user = await Http.GetFromJsonAsync<User>($"/api/users/{UserId}");
  }
}
```
:::

React 버전:

::: v-pre
```jsx
function UserProfile({ userId }) {
  const [user, setUser] = useState(null);

  useEffect(() => {
    fetch(`/api/users/${userId}`)
      .then(res => res.json())
      .then(data => setUser(data));
  }, []);  // 빈 배열: 마운트 시 한 번만

  if (!user) return <p>로딩 중...</p>;

  return (
    <div>
      <h1>{user.name}</h1>
      <p>{user.email}</p>
    </div>
  );
}
```
:::

**주의**: React의 `useEffect([], [])`와 달리, Blazor의 `OnInitializedAsync`는 `UserId` Parameter가 변경되어도 다시 호출되지 않습니다!

### OnParametersSet: Parameter 변경 감지

Parameter가 변경될 때마다 실행되려면:

::: v-pre
```razor
protected override async Task OnParametersSetAsync()
{
  // UserId가 변경될 때마다 실행
  user = await Http.GetFromJsonAsync<User>($"/api/users/{UserId}");
}
```
:::

React 버전:

::: v-pre
```jsx
useEffect(() => {
  fetch(`/api/users/${userId}`)
    .then(res => res.json())
    .then(data => setUser(data));
}, [userId]);  // userId 변경 시 재실행
```
:::

**실행 순서 예시:**

```
1. 컴포넌트 생성 → SetParametersAsync
2. OnInitialized
3. OnParametersSet (첫 번째)
4. (렌더링)
5. OnAfterRender(firstRender: true)
6. [Parameter 변경] → SetParametersAsync
7. OnParametersSet (두 번째)
8. (렌더링)
9. OnAfterRender(firstRender: false)
10. [컴포넌트 제거] → Dispose
```

### OnAfterRender: DOM 접근

렌더링 후 DOM에 접근해야 할 때:

::: v-pre
```razor
@inject IJSRuntime JS

<input @ref="inputElement" />

@code {
  private ElementReference inputElement;

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    if (firstRender)
    {
      // 첫 렌더링 후 input에 포커스
      await JS.InvokeVoidAsync("focusElement", inputElement);
    }
  }
}
```
:::

JavaScript:

```javascript
// wwwroot/js/helpers.js
window.focusElement = (element) => {
  element.focus();
};
```

React 버전:

::: v-pre
```jsx
import { useEffect, useRef } from 'react';

function MyComponent() {
  const inputRef = useRef(null);

  useEffect(() => {
    inputRef.current.focus();
  }, []);

  return <input ref={inputRef} />;
}
```
:::

**중요**: `OnAfterRender`는 서버 사이드 프리렌더링 시에는 호출되지 않을 수 있으므로, `firstRender` 플래그를 확인하세요.

### Dispose: 리소스 정리

::: v-pre
```razor
@page "/live-updates"
@inject IDisposable subscription
@implements IDisposable

<h1>실시간 업데이트</h1>
<p>최신 메시지: @latestMessage</p>

@code {
  private string latestMessage = "";
  private Timer? timer;

  protected override void OnInitialized()
  {
    // 타이머 설정
    timer = new Timer(async _ =>
    {
      latestMessage = $"업데이트: {DateTime.Now:HH:mm:ss}";
      await InvokeAsync(StateHasChanged);  // UI 스레드에서 실행
    }, null, 0, 1000);  // 1초마다
  }

  public void Dispose()
  {
    // 컴포넌트 제거 시 타이머 정리
    timer?.Dispose();
  }
}
```
:::

React 버전:

::: v-pre
```jsx
function LiveUpdates() {
  const [latestMessage, setLatestMessage] = useState('');

  useEffect(() => {
    const timer = setInterval(() => {
      setLatestMessage(`업데이트: ${new Date().toLocaleTimeString()}`);
    }, 1000);

    return () => clearInterval(timer);  // cleanup
  }, []);

  return (
    <div>
      <h1>실시간 업데이트</h1>
      <p>최신 메시지: {latestMessage}</p>
    </div>
  );
}
```
:::

`IDisposable` 인터페이스를 구현하면 Blazor가 컴포넌트 제거 시 `Dispose()`를 자동 호출합니다.

## 폼과 유효성 검사: EditForm의 강력함

React에는 공식 폼 라이브러리가 없어 서드파티(React Hook Form, Formik)를 사용합니다. Blazor는 강력한 폼 시스템이 내장되어 있습니다.

### EditForm 기본 사용

::: v-pre
```razor
@page "/register"

<EditForm Model="@model" OnValidSubmit="@HandleValidSubmit">
  <DataAnnotationsValidator />
  <ValidationSummary />

  <div class="form-group">
    <label for="email">이메일:</label>
    <InputText id="email" @bind-Value="model.Email" class="form-control" />
    <ValidationMessage For="@(() => model.Email)" />
  </div>

  <div class="form-group">
    <label for="password">비밀번호:</label>
    <InputText id="password" @bind-Value="model.Password"
               type="password" class="form-control" />
    <ValidationMessage For="@(() => model.Password)" />
  </div>

  <div class="form-group">
    <label for="age">나이:</label>
    <InputNumber id="age" @bind-Value="model.Age" class="form-control" />
    <ValidationMessage For="@(() => model.Age)" />
  </div>

  <button type="submit" class="btn btn-primary">등록</button>
</EditForm>

@code {
  private RegisterModel model = new();

  private void HandleValidSubmit()
  {
    // 유효성 검사 통과 시에만 호출됨
    Console.WriteLine($"등록: {model.Email}");
  }
}

public class RegisterModel
{
  [Required(ErrorMessage = "이메일은 필수입니다.")]
  [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
  public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "비밀번호는 필수입니다.")]
  [StringLength(100, MinimumLength = 6, ErrorMessage = "비밀번호는 6-100자여야 합니다.")]
  public string Password { get; set; } = string.Empty;

  [Range(18, 120, ErrorMessage = "나이는 18-120 사이여야 합니다.")]
  public int Age { get; set; }
}
```
:::

React Hook Form 버전:

::: v-pre
```jsx
import { useForm } from 'react-hook-form';

function RegisterForm() {
  const { register, handleSubmit, formState: { errors } } = useForm();

  const onSubmit = (data) => {
    console.log(`등록: ${data.email}`);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div className="form-group">
        <label htmlFor="email">이메일:</label>
        <input {...register('email', {
          required: '이메일은 필수입니다.',
          pattern: {
            value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
            message: '올바른 이메일 형식이 아닙니다.'
          }
        })} />
        {errors.email && <span>{errors.email.message}</span>}
      </div>

      <div className="form-group">
        <label htmlFor="password">비밀번호:</label>
        <input type="password" {...register('password', {
          required: '비밀번호는 필수입니다.',
          minLength: {
            value: 6,
            message: '비밀번호는 최소 6자 이상이어야 합니다.'
          }
        })} />
        {errors.password && <span>{errors.password.message}</span>}
      </div>

      <button type="submit">등록</button>
    </form>
  );
}
```
:::

**Blazor의 장점:**

1. **데이터 어노테이션**: 유효성 규칙이 모델 클래스에 선언적으로 정의됨
2. **재사용**: 같은 모델을 백엔드 API에서도 사용 가능 (서버 측 검증 동기화)
3. **타입 안정성**: `For="@(() => model.Email)"`는 컴파일 타임에 검증됨
4. **내장 컴포넌트**: `InputText`, `InputNumber`, `InputDate` 등 제공

### 커스텀 유효성 검사

```csharp
public class RegisterModel : IValidatableObject
{
  [Required]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; } = string.Empty;

  [Required]
  [Compare(nameof(Password), ErrorMessage = "비밀번호가 일치하지 않습니다.")]
  public string ConfirmPassword { get; set; } = string.Empty;

  // 복잡한 커스텀 검증
  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    if (Email.EndsWith("@temp.com"))
    {
      yield return new ValidationResult(
        "임시 이메일은 사용할 수 없습니다.",
        new[] { nameof(Email) }
      );
    }

    if (Password.Contains(Email.Split('@')[0]))
    {
      yield return new ValidationResult(
        "비밀번호에 이메일 아이디가 포함될 수 없습니다.",
        new[] { nameof(Password) }
      );
    }
  }
}
```

### 비동기 유효성 검사 (서버 검증)

::: v-pre
```razor
<EditForm Model="@model" OnValidSubmit="@HandleValidSubmit">
  <DataAnnotationsValidator />
  <ValidationSummary />

  <InputText @bind-Value="model.Username" @bind-Value:event="oninput"
             @onblur="ValidateUsername" />
  <ValidationMessage For="@(() => model.Username)" />

  @if (isCheckingUsername)
  {
    <span>사용자명 확인 중...</span>
  }
  else if (usernameError != null)
  {
    <span class="error">@usernameError</span>
  }

  <button type="submit">등록</button>
</EditForm>

@code {
  private bool isCheckingUsername = false;
  private string? usernameError;

  private async Task ValidateUsername()
  {
    isCheckingUsername = true;
    usernameError = null;

    var isAvailable = await Http.GetFromJsonAsync<bool>(
      $"/api/users/check-username?username={model.Username}"
    );

    if (!isAvailable)
    {
      usernameError = "이미 사용 중인 사용자명입니다.";
    }

    isCheckingUsername = false;
  }
}
```
:::

## JavaScript Interop: 두 세계의 연결

Blazor만으로 모든 것을 할 수는 없습니다. 기존 JavaScript 라이브러리를 사용하거나, 브라우저 API에 접근해야 할 때가 있습니다.

### C#에서 JavaScript 호출

::: v-pre
```razor
@inject IJSRuntime JS

<button @onclick="ShowAlert">알림 표시</button>
<button @onclick="GetWindowSize">윈도우 크기</button>

@code {
  private async Task ShowAlert()
  {
    await JS.InvokeVoidAsync("alert", "안녕하세요!");
  }

  private async Task GetWindowSize()
  {
    var width = await JS.InvokeAsync<int>("eval", "window.innerWidth");
    var height = await JS.InvokeAsync<int>("eval", "window.innerHeight");
    Console.WriteLine($"윈도우 크기: {width}x{height}");
  }
}
```
:::

### JavaScript에서 C# 호출

```csharp
// DotNetHelper.cs
public class DotNetHelper
{
  [JSInvokable]
  public static Task<string> GetMessage()
  {
    return Task.FromResult("C#에서 온 메시지");
  }

  [JSInvokable]
  public static Task<int> Add(int a, int b)
  {
    return Task.FromResult(a + b);
  }
}
```

```javascript
// wwwroot/js/interop.js
window.callDotNet = async () => {
  const message = await DotNet.invokeMethodAsync('MyBlazorApp', 'GetMessage');
  console.log(message);  // "C#에서 온 메시지"

  const sum = await DotNet.invokeMethodAsync('MyBlazorApp', 'Add', 5, 3);
  console.log(sum);  // 8
};
```

### 기존 라이브러리 통합 (Chart.js 예시)

::: v-pre
```razor
@inject IJSRuntime JS

<canvas @ref="chartCanvas" width="400" height="200"></canvas>

@code {
  private ElementReference chartCanvas;

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    if (firstRender)
    {
      await JS.InvokeVoidAsync("initializeChart", chartCanvas, new
      {
        type = "bar",
        data = new
        {
          labels = new[] { "1월", "2월", "3월" },
          datasets = new[]
          {
            new
            {
              label = "매출",
              data = new[] { 12, 19, 3 }
            }
          }
        }
      });
    }
  }
}
```
:::

```javascript
// wwwroot/js/chart-helper.js
window.initializeChart = (canvas, config) => {
  new Chart(canvas, config);
};
```

```html
<!-- index.html -->
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script src="js/chart-helper.js"></script>
```

## 다음 단계

Chapter 10에서 Blazor 컴포넌트의 핵심을 마스터했습니다:

- `.razor` 파일 구조와 code-behind 패턴
- 로컬 상태 관리와 StateHasChanged
- Parameters와 EventCallback로 컴포넌트 통신
- 양방향 바인딩의 편리함
- 생명주기 메서드 활용
- EditForm으로 강력한 폼 유효성 검사
- JavaScript Interop으로 생태계 확장

Chapter 11에서는 프로덕션 수준의 Blazor 애플리케이션을 만드는 고급 패턴을 배웁니다:

- 라우팅과 네비게이션
- 레이아웃과 중첩 레이아웃
- Razor Class Library로 컴포넌트 라이브러리 만들기
- 성능 최적화 (Virtualization, Lazy loading, Prerendering)
- 인증과 권한 부여
- SignalR 통합으로 실시간 데이터 업데이트

실습에서는 실시간 대시보드를 만들며 모든 지식을 종합합니다!

---

## 학습 자료

- [Blazor 컴포넌트 공식 문서](https://docs.microsoft.com/aspnet/core/blazor/components/)
- [데이터 바인딩 가이드](https://docs.microsoft.com/aspnet/core/blazor/components/data-binding)
- [폼과 유효성 검사](https://docs.microsoft.com/aspnet/core/blazor/forms-validation)
- [JavaScript Interop](https://docs.microsoft.com/aspnet/core/blazor/javascript-interoperability/)

다음 챕터: [Chapter 11: Blazor 고급 패턴](../chapter11/README.md)
