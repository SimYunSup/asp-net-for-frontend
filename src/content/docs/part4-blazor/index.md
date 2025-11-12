---
title: "Part 4 - Blazor - C#으로 작성하는 프론트엔드"
---

# Part 4: Blazor - C#으로 작성하는 프론트엔드

## 프론트엔드 개발자의 새로운 가능성: 브라우저에서 실행되는 C#

Part 3에서 우리는 서버 사이드 렌더링의 세계를 탐험했습니다. Razor Pages와 MVC를 통해 서버에서 HTML을 생성하고, 사용자에게 완성된 페이지를 전달하는 방법을 배웠습니다. 이는 강력하고 효율적이지만, 한 가지 한계가 있습니다. 풍부한 사용자 인터랙션을 만들려면 여전히 JavaScript가 필요하다는 점입니다.

드래그 앤 드롭, 실시간 폼 유효성 검사, 동적 UI 업데이트, 클라이언트 사이드 라우팅—이런 것들을 구현하려면 전통적으로 JavaScript로 돌아가야 했습니다. React, Vue, Angular 같은 프레임워크들이 바로 이 문제를 해결하기 위해 존재합니다. 하지만 여기서 흥미로운 질문이 생깁니다: "백엔드는 C#으로 작성하는데, 왜 프론트엔드만 JavaScript로 작성해야 할까?"

Blazor는 이 질문에 대한 Microsoft의 답변입니다. C#으로 인터랙티브한 웹 UI를 작성하고, 브라우저에서 실행할 수 있는 풀스택 프레임워크입니다. "불가능하다"고 생각할 수 있습니다. 브라우저는 JavaScript만 실행하니까요. 하지만 WebAssembly의 등장으로 모든 것이 바뀌었습니다.

### WebAssembly: 브라우저의 새로운 시대

2019년, 웹 표준의 역사에서 중요한 순간이 있었습니다. WebAssembly(줄여서 Wasm)가 W3C의 공식 표준이 된 것입니다. WebAssembly는 브라우저에서 실행할 수 있는 저수준 바이트코드 형식으로, JavaScript보다 훨씬 빠르게 실행되며, 어떤 언어로든 컴파일할 수 있습니다.

C, C++, Rust로 작성된 코드를 WebAssembly로 컴파일하면 브라우저에서 네이티브에 가까운 성능으로 실행됩니다. Adobe는 Photoshop을 웹으로 가져올 때 WebAssembly를 사용했고, Figma는 복잡한 그래픽 렌더링에 WebAssembly를 활용합니다. AutoCAD, Unity 게임 엔진, 심지어 Windows 95 에뮬레이터까지 브라우저에서 실행됩니다.

.NET 팀은 이 기술을 보고 생각했습니다: ".NET 런타임 자체를 WebAssembly로 컴파일하면 어떨까?" 그 결과가 Blazor WebAssembly입니다. 브라우저가 .NET 런타임을 다운로드하면, 이후에는 C# 코드가 브라우저에서 직접 실행됩니다. JavaScript 없이 말이죠.

### Blazor의 세 가지 호스팅 모델: 선택의 유연성

Blazor의 가장 혁신적인 점은 호스팅 모델을 선택할 수 있다는 것입니다. 같은 컴포넌트 코드를 작성하되, 어디서 실행할지 결정할 수 있습니다. React는 클라이언트에서 실행되고, Next.js로 서버 사이드 렌더링을 추가할 수 있습니다. Blazor는 더 나아가 세 가지 완전히 다른 방식을 제공합니다.

**1. Blazor Server: SignalR로 연결된 실시간 렌더링**

첫 번째 모델은 Blazor Server입니다. 사용자가 페이지를 열면, 브라우저와 서버 사이에 SignalR(WebSocket 기반) 연결이 만들어집니다. 사용자가 버튼을 클릭하면, 그 이벤트는 서버로 전송되고, 서버에서 C# 코드가 실행되며, UI 변경사항만 브라우저로 다시 전송됩니다.

이 방식의 장점은 명확합니다. 초기 로드가 매우 빠릅니다. JavaScript 번들을 다운로드할 필요가 없으니까요. 또한 전체 .NET API에 접근할 수 있습니다. 파일 시스템, 데이터베이스, 서드파티 라이브러리—서버에서 할 수 있는 모든 것을 할 수 있습니다. 보안도 강화됩니다. 비즈니스 로직이 절대 클라이언트에 노출되지 않으니까요.

하지만 단점도 있습니다. 서버 연결이 필수적입니다. 네트워크가 끊기면 앱이 작동하지 않습니다. 또한 모든 상호작용에 서버 왕복이 필요하므로, 네트워크 지연이 사용자 경험에 직접적인 영향을 미칩니다. 동시 사용자가 많으면 서버 리소스도 많이 필요합니다. 각 사용자마다 서버에서 상태를 유지해야 하니까요.

**2. Blazor WebAssembly: 진정한 클라이언트 사이드 앱**

두 번째 모델은 Blazor WebAssembly입니다. 브라우저가 .NET 런타임과 애플리케이션 DLL을 다운로드하면, 이후 모든 것이 클라이언트에서 실행됩니다. React SPA와 개념적으로 동일하지만, JavaScript 대신 C#으로 작성됩니다.

이 방식의 가장 큰 장점은 오프라인 지원입니다. 한 번 다운로드하면 서버 없이도 작동합니다. PWA(Progressive Web App)로 만들어 모바일 홈 화면에 설치할 수도 있습니다. 서버 부하도 없습니다. 정적 파일만 호스팅하면 되니 CDN에서 서빙할 수 있습니다. 사용자 인터랙션도 즉각적입니다. 네트워크 왕복이 없으니까요.

단점은 초기 다운로드 크기입니다. .NET 런타임 자체가 약 2-3MB이고, 애플리케이션 코드와 라이브러리를 합치면 더 커집니다. 첫 로딩이 React 앱보다 느릴 수 있습니다. 하지만 .NET 7부터 AOT(Ahead-of-Time) 컴파일이 도입되어 런타임 크기가 크게 줄었고, .NET 8에서는 더 최적화되었습니다. 또한 브라우저 제약이 있습니다. 파일 시스템, 데이터베이스 같은 서버 API는 직접 사용할 수 없으며, Web API로 우회해야 합니다.

**3. Blazor Hybrid: 웹을 넘어서**

세 번째 모델은 Blazor Hybrid입니다. .NET MAUI(Multi-platform App UI)나 WPF, Windows Forms 같은 데스크톱 프레임워크 안에서 Blazor 컴포넌트를 실행합니다. 웹 기술로 UI를 만들지만, 네이티브 앱으로 패키징됩니다.

Electron을 생각하면 이해하기 쉽습니다. Electron은 Chromium과 Node.js를 번들로 제공하여 웹 기술로 데스크톱 앱을 만듭니다. Blazor Hybrid는 비슷하지만 .NET 기반입니다. VSCode, Slack, Discord가 Electron으로 만들어진 것처럼, Blazor Hybrid로도 크로스 플랫폼 앱을 만들 수 있습니다.

가장 강력한 점은 코드 공유입니다. 같은 Blazor 컴포넌트를 웹 앱, iOS 앱, Android 앱, Windows 앱, macOS 앱에서 모두 사용할 수 있습니다. UI 로직을 한 번 작성하고 여러 플랫폼에 배포하는 것이죠. 또한 네이티브 API에 완전히 접근할 수 있습니다. 파일 시스템, 블루투스, 카메라, GPS—모든 것이 가능합니다.

### React 개발자가 보는 Blazor: 놀라울 정도로 친숙한

React를 알고 있다면 Blazor가 매우 친숙하게 느껴질 것입니다. Blazor의 컴포넌트 모델은 React에서 직접적인 영감을 받았습니다. Microsoft 팀은 React의 성공적인 패턴을 연구하고, C# 생태계에 맞게 적용했습니다.

**컴포넌트 구조 비교:**

```jsx
// React 컴포넌트
import { useState } from 'react';

function Counter() {
  const [count, setCount] = useState(0);

  const increment = () => setCount(count + 1);

  return (
    <div>
      <h1>카운터</h1>
      <p>현재 카운트: {count}</p>
      <button onClick={increment}>증가</button>
    </div>
  );
}
```

```razor
@* Blazor 컴포넌트 *@
<div>
  <h1>카운터</h1>
  <p>현재 카운트: @currentCount</p>
  <button @onclick="Increment">증가</button>
</div>

@code {
  private int currentCount = 0;

  private void Increment()
  {
    currentCount++;
  }
}
```

구조를 보세요. 거의 동일합니다. JSX의 `{expression}`이 Razor의 `@expression`으로, `onClick`이 `@onclick`으로 바뀐 것뿐입니다. `useState`는 클래스 필드로 대체되며, `setCount`를 호출하는 대신 직접 값을 변경합니다. Blazor가 자동으로 변경을 감지하고 UI를 업데이트합니다.

**Props와 Parameters:**

```jsx
// React Props
function Greeting({ name, onGreet }) {
  return (
    <div>
      <h1>안녕하세요, {name}님!</h1>
      <button onClick={onGreet}>인사하기</button>
    </div>
  );
}

// 사용
<Greeting name="홍길동" onGreet={() => alert('안녕!')} />
```

```razor
@* Blazor Parameters *@
<div>
  <h1>안녕하세요, @Name님!</h1>
  <button @onclick="OnGreet">인사하기</button>
</div>

@code {
  [Parameter]
  public string Name { get; set; } = string.Empty;

  [Parameter]
  public EventCallback OnGreet { get; set; }
}

@* 사용 *@
<Greeting Name="홍길동" OnGreet="@(() => Console.WriteLine("안녕!"))" />
```

React의 props가 Blazor에서는 `[Parameter]` 특성으로 표시된 프로퍼티입니다. 이벤트 콜백도 `EventCallback` 타입으로 동일하게 작동합니다. 차이점은 타입 안정성입니다. `Name`은 `string`이어야 하고, `OnGreet`은 `EventCallback`이어야 합니다. 잘못된 타입을 전달하면 컴파일 시점에 오류가 발생합니다.

**생명주기 메서드:**

```jsx
// React Hooks
import { useEffect } from 'react';

function UserProfile({ userId }) {
  const [user, setUser] = useState(null);

  useEffect(() => {
    fetch(`/api/users/${userId}`)
      .then(res => res.json())
      .then(data => setUser(data));

    // cleanup
    return () => console.log('컴포넌트 언마운트');
  }, [userId]);

  return <div>{user?.name}</div>;
}
```

```razor
@* Blazor 생명주기 *@
@if (user != null)
{
  <div>@user.Name</div>
}

@code {
  [Parameter]
  public int UserId { get; set; }

  private User? user;

  protected override async Task OnParametersSetAsync()
  {
    user = await Http.GetFromJsonAsync<User>($"/api/users/{UserId}");
  }

  public void Dispose()
  {
    Console.WriteLine("컴포넌트 언마운트");
  }
}
```

`useEffect`가 Blazor에서는 생명주기 메서드로 대체됩니다. `OnInitializedAsync`는 컴포넌트가 처음 생성될 때, `OnParametersSetAsync`는 파라미터가 변경될 때 호출됩니다. `useEffect`의 cleanup 함수는 `IDisposable` 인터페이스의 `Dispose` 메서드가 됩니다.

### 왜 Blazor를 배워야 하는가?

프론트엔드 개발자로서 이미 React, Vue, Angular 중 하나는 알고 있을 것입니다. 또 다른 프레임워크를 배워야 할까요? Blazor를 배워야 하는 실용적인 이유들이 있습니다.

**1. 단일 언어 풀스택 개발: 컨텍스트 스위칭 제거**

JavaScript와 C# 사이를 오가는 것은 생각보다 인지 부하가 큽니다. `null` vs `undefined`의 차이, `==` vs `===`의 미묘함, 호이스팅, this 바인딩—JavaScript의 특성들을 기억해야 합니다. 그러다 백엔드 코드로 돌아오면 LINQ, async/await의 다른 동작, 강타입 시스템에 다시 적응해야 합니다.

Blazor는 이 컨텍스트 스위칭을 제거합니다. 프론트엔드와 백엔드를 같은 언어로 작성하니, 같은 패턴, 같은 컨벤션, 같은 도구를 사용합니다. DTO(Data Transfer Object) 클래스를 프론트엔드와 백엔드에서 공유할 수 있습니다. 타입이 항상 동기화됩니다.

```csharp
// 백엔드 API
public class ProductsController : ControllerBase
{
  [HttpGet]
  public async Task<List<Product>> GetProducts()
  {
    return await _db.Products.ToListAsync();
  }
}

// Blazor 프론트엔드 (같은 Product 클래스!)
@code {
  private List<Product> products = new();

  protected override async Task OnInitializedAsync()
  {
    products = await Http.GetFromJsonAsync<List<Product>>("/api/products");
  }
}
```

타입 불일치 오류가 사라집니다. TypeScript를 사용해도 백엔드 C# 타입과 프론트엔드 TypeScript 타입을 동기화해야 하는데, Blazor는 이 문제가 없습니다.

**2. 강력한 타입 시스템: 런타임 오류를 컴파일 타임에**

JavaScript와 TypeScript의 가장 큰 차이는 타입 시스템입니다. Blazor는 C#의 강타입 시스템을 그대로 가져옵니다. 모든 변수, 모든 함수, 모든 컴포넌트 파라미터가 타입을 가집니다. 잘못된 타입을 전달하면 코드가 컴파일되지 않습니다.

```razor
@* 컴파일 오류 예시 *@
<UserProfile UserId="abc" />  @* 오류: string을 int에 할당할 수 없음 *@

@code {
  private void ProcessData(List<string> items)
  {
    // items는 확실히 List<string>
    // null 체크 불필요 (nullable 참조 타입 사용 시)
    var first = items.First();
  }
}
```

TypeScript도 타입 체킹을 제공하지만, 컴파일 타임입니다. 브라우저에서 실행되는 것은 여전히 JavaScript이며, 런타임에 예상치 못한 타입이 나타날 수 있습니다. C#은 런타임에도 타입이 유지됩니다.

**3. 성숙한 생태계와 도구: 엔터프라이즈급 개발**

.NET 생태계는 20년 이상의 역사를 가지고 있습니다. NuGet에는 30만 개 이상의 패키지가 있으며, 대부분이 Blazor에서도 작동합니다. 인증, 로깅, 의존성 주입, 테스팅—모든 것이 표준화되어 있고, Microsoft의 공식 지원을 받습니다.

Visual Studio는 세계에서 가장 강력한 IDE 중 하나입니다. IntelliSense는 단순한 자동완성을 넘어 코드 분석, 리팩토링, 디버깅을 제공합니다. Blazor 컴포넌트 안에서 중단점을 설정하고, 변수를 검사하고, 실시간으로 코드를 수정할 수 있습니다. Hot Reload는 코드를 변경하면 즉시 브라우저에 반영됩니다.

**4. 성능: WebAssembly의 잠재력**

JavaScript는 JIT(Just-In-Time) 컴파일로 빠르지만, 한계가 있습니다. WebAssembly는 AOT(Ahead-of-Time) 컴파일로 네이티브에 가까운 성능을 제공합니다. 복잡한 계산, 데이터 처리, 알고리즘 실행에서 Blazor WebAssembly는 JavaScript보다 빠를 수 있습니다.

벤치마크에 따르면, 정렬 알고리즘, 수학 연산, 이미지 처리 같은 CPU 집약적 작업에서 WebAssembly가 JavaScript보다 2-10배 빠릅니다. 물론 DOM 조작이나 간단한 UI 업데이트에서는 차이가 크지 않지만, 성능이 중요한 부분에서는 큰 장점입니다.

**5. 기업 환경의 현실: .NET 개발자의 풍부함**

많은 기업이 이미 .NET 백엔드를 사용합니다. ASP.NET, WCF, Azure Functions... 백엔드 팀은 C#에 익숙합니다. Blazor를 도입하면 백엔드 개발자도 프론트엔드 개발에 기여할 수 있습니다. 별도의 JavaScript 전문가를 고용할 필요가 줄어듭니다.

역으로, 프론트엔드 개발자인 여러분이 Blazor를 배우면 백엔드 코드베이스에도 쉽게 접근할 수 있습니다. 풀스택 개발자로서의 가치가 높아집니다.

### Blazor vs React/Vue: 언제 무엇을 선택할까?

Blazor가 만능은 아닙니다. 모든 프로젝트에 적합한 것도 아닙니다. 명확한 선택 가이드가 필요합니다.

**Blazor를 선택하세요:**

- **.NET 백엔드와 통합**: 이미 ASP.NET Core API가 있고, 같은 타입과 로직을 공유하고 싶을 때
- **엔터프라이즈 애플리케이션**: 강타입, 명확한 아키텍처, 장기 유지보수가 중요할 때
- **내부 도구와 대시보드**: SEO가 필요 없고, 사용자가 모던 브라우저를 사용할 때
- **데스크톱/모바일 앱 확장**: .NET MAUI로 크로스 플랫폼 앱을 만들 계획이 있을 때
- **팀의 스킬셋**: C# 개발자가 많고, JavaScript 전문성이 부족할 때
- **오프라인 지원**: PWA로 오프라인 작동이 필요할 때 (Blazor WebAssembly)

**React/Vue를 선택하세요:**

- **공개 웹사이트**: SEO와 초기 로딩 성능이 매우 중요할 때 (Next.js, Nuxt.js가 더 성숙함)
- **생태계의 풍부함**: 수많은 JavaScript 라이브러리와 컴포넌트를 활용하고 싶을 때
- **모바일 우선**: React Native, Ionic 같은 모바일 프레임워크가 필요할 때
- **레거시 브라우저**: IE11 같은 오래된 브라우저를 지원해야 할 때
- **큰 커뮤니티**: Stack Overflow, GitHub 예제, 튜토리얼이 더 많을 때
- **빠른 초기 로드**: 초기 다운로드 크기가 절대적으로 작아야 할 때

**하이브리드 접근:**

가장 현명한 선택은 혼합일 수 있습니다. ASP.NET Core API를 백엔드로 사용하고, React/Vue를 프론트엔드로 사용하며, 특정 기능만 Blazor로 구현하는 식입니다. 또는 관리자 대시보드는 Blazor로, 공개 웹사이트는 Next.js로 만드는 전략도 가능합니다.

### Blazor의 현재와 미래

Blazor는 2018년 실험적 프로젝트로 시작했지만, 이제는 프로덕션 준비가 완료되었습니다. .NET 6(2021)에서 공식 릴리스, .NET 7(2022)에서 성능 개선, .NET 8(2023)에서 새로운 렌더링 모델, .NET 9(2024)에서 더욱 강화되었습니다.

Microsoft는 Blazor에 크게 투자하고 있습니다. Azure Portal의 일부 기능이 Blazor로 재작성되고 있으며, Visual Studio의 일부 UI도 Blazor 기반입니다. Stack Overflow의 2023년 설문조사에서 Blazor는 "가장 사랑받는 웹 프레임워크" 상위권에 올랐습니다.

.NET 8에서 도입된 정적 서버 사이드 렌더링(Static SSR)과 스트리밍 렌더링은 Blazor를 Next.js와 동등한 수준으로 끌어올렸습니다. .NET 9의 생성자 주입(Constructor Injection)은 의존성 주입을 더 간결하게 만들었습니다. 앞으로도 계속 개선될 것입니다.

### Part 4에서 배울 내용

이 파트는 React/Vue 개발자가 Blazor의 세계로 자연스럽게 진입할 수 있도록 설계되었습니다. 친숙한 컴포넌트 패러다임에서 시작하여, Blazor만의 독특한 강점을 탐구합니다.

**Chapter 9: Blazor 소개 - C#으로 작성하는 프론트엔드**

Blazor의 기초를 다집니다. 세 가지 호스팅 모델(Server, WebAssembly, Hybrid)을 깊이 이해하고, 각각의 장단점을 비교합니다. 프로젝트를 생성하고, 개발 환경을 설정하며, 첫 Blazor 애플리케이션을 실행합니다.

React/Vue와의 철저한 비교를 통해, 익숙한 개념을 Blazor 방식으로 매핑합니다. 컴포넌트, props, 상태, 이벤트, 생명주기—모든 것이 어떻게 대응되는지 명확히 합니다. Hot Reload, 디버깅, 브라우저 개발자 도구 활용법도 배웁니다.

**Chapter 10: Blazor 컴포넌트 개발**

Blazor 컴포넌트의 모든 것을 배웁니다. `.razor` 파일 구조, `@code` 블록, 로컬 상태 관리부터 시작합니다. `[Parameter]`로 부모-자식 간 데이터를 전달하고, `EventCallback`으로 이벤트를 처리합니다.

`CascadingParameter`는 React의 Context API와 유사하게 깊은 컴포넌트 트리에 데이터를 전달합니다. 양방향 바인딩(`@bind`)은 폼 처리를 간결하게 만듭니다. 생명주기 메서드(`OnInitialized`, `OnParametersSet`, `OnAfterRender`)로 컴포넌트의 생명주기를 제어합니다.

폼과 유효성 검사는 `EditForm` 컴포넌트로 처리합니다. 데이터 어노테이션만으로 복잡한 유효성 규칙을 정의하며, 실시간 피드백을 제공합니다. JavaScript Interop으로 기존 JavaScript 라이브러리를 통합하는 방법도 배웁니다.

실습에서는 완전한 Todo 애플리케이션을 만듭니다. CRUD 작업, 로컬 스토리지 연동, 상태 관리 패턴을 구현하며, React Todo 앱과 직접 비교합니다.

**Chapter 11: Blazor 고급 패턴**

프로덕션 수준의 Blazor 애플리케이션을 만드는 고급 기법을 탐구합니다. 라우팅 시스템(`@page`, 동적 파라미터, 쿼리 문자열)으로 SPA 경험을 만듭니다. `NavLink` 컴포넌트는 React Router의 Link와 유사하게 작동합니다.

레이아웃(`MainLayout.razor`)으로 일관된 UI 구조를 만들고, 중첩 레이아웃으로 복잡한 페이지를 구성합니다. Razor Class Library(RCL)로 재사용 가능한 컴포넌트 라이브러리를 만들어 NuGet으로 배포합니다.

성능 최적화가 중요합니다. 가상화(Virtualization)로 수천 개의 항목을 효율적으로 렌더링하고, `@key` 지시문으로 React의 key prop과 동일한 최적화를 적용합니다. Lazy loading으로 코드를 분할하며, Prerendering으로 SSR과 유사한 초기 로딩을 구현합니다.

인증과 권한 부여는 `AuthorizeView` 컴포넌트로 처리합니다. 로그인 상태에 따라 UI를 조건부 렌더링하며, 역할 기반 접근 제어를 구현합니다.

실습에서는 실시간 대시보드 애플리케이션을 만듭니다. SignalR로 실시간 데이터를 받아 차트를 업데이트하고, 인증이 필요한 페이지를 구현하며, 반응형 레이아웃을 적용합니다.

## 학습 목표

Part 4를 마치면 다음을 할 수 있습니다:

- Blazor의 세 가지 호스팅 모델을 이해하고 프로젝트에 맞게 선택합니다
- Blazor 컴포넌트를 작성하고 재사용 가능한 UI를 구축합니다
- Parameters와 EventCallback으로 컴포넌트 간 통신을 구현합니다
- 생명주기 메서드로 컴포넌트의 동작을 제어합니다
- 폼 처리와 유효성 검사를 구현합니다
- JavaScript Interop으로 기존 라이브러리를 통합합니다
- 라우팅과 네비게이션으로 SPA를 만듭니다
- 성능 최적화 기법을 적용합니다
- 인증과 권한 부여를 구현합니다
- 실시간 데이터 업데이트를 처리합니다
- 프로덕션 수준의 Blazor 애플리케이션을 배포합니다

## 챕터 구성

### [Chapter 9: Blazor 소개 - C#으로 작성하는 프론트엔드](./chapter9/index.md)

Blazor의 기초와 호스팅 모델을 완벽히 이해합니다.

- Blazor란 무엇인가: WebAssembly의 이해
- Blazor Server vs WebAssembly vs Hybrid: 깊이 있는 비교
- 프로젝트 생성과 개발 환경 설정
- React/Vue와 Blazor 비교: 컴포넌트 모델의 유사성
- 첫 Blazor 애플리케이션 만들기
- Hot Reload와 개발 경험
- 디버깅 기법과 도구

**핵심 개념**: WebAssembly, 호스팅 모델, SignalR, .NET 런타임, 컴포넌트 패러다임

**실습**: 세 가지 호스팅 모델로 각각 프로젝트 생성, 간단한 카운터 앱 구현, React와 비교

### [Chapter 10: Blazor 컴포넌트 개발](./chapter10/index.md)

Blazor 컴포넌트의 모든 측면을 마스터합니다.

- `.razor` 파일 구조와 `@code` 블록
- 로컬 상태 관리: 필드와 프로퍼티
- Parameters와 EventCallback: 부모-자식 통신
- CascadingParameter: React Context와 유사한 패턴
- 양방향 바인딩: `@bind` 디렉티브
- 생명주기 메서드: OnInitialized, OnParametersSet, OnAfterRender
- 폼과 유효성 검사: EditForm, 데이터 어노테이션
- JavaScript Interop: IJSRuntime 활용
- 컴포넌트 재사용과 구성

**핵심 개념**: 컴포넌트 생명주기, 데이터 바인딩, 이벤트 처리, 폼 유효성 검사, JS Interop

**실습**: Todo 애플리케이션 - CRUD 작업, 로컬 스토리지, 필터링, 정렬

### [Chapter 11: Blazor 고급 패턴](./chapter11/index.md)

프로덕션 수준의 Blazor 애플리케이션을 만듭니다.

- Blazor 라우팅: `@page`, 동적 파라미터, 쿼리 문자열
- 프로그래밍 방식 네비게이션: NavigationManager
- 레이아웃과 중첩 레이아웃
- Razor Class Library: 재사용 가능한 컴포넌트 라이브러리
- 성능 최적화: Virtualization, `@key`, Lazy loading
- Prerendering과 Static SSR (.NET 8+)
- 인증과 권한 부여: AuthorizeView, 역할 기반 UI
- 상태 관리 패턴: 서비스 기반, Fluxor
- SignalR 통합: 실시간 데이터 업데이트
- 에러 처리와 ErrorBoundary

**핵심 개념**: 라우팅, 레이아웃, RCL, 성능 최적화, 인증, 상태 관리, 실시간 통신

**실습**: 실시간 대시보드 - SignalR 연동, 차트 라이브러리 통합, 인증, 반응형 레이아웃

## React/Vue와 Blazor 비교 치트시트

프론트엔드 개발자를 위한 빠른 참조 가이드:

| 개념 | React | Vue | Blazor |
|------|-------|-----|--------|
| 컴포넌트 파일 | `.jsx` / `.tsx` | `.vue` | `.razor` |
| 상태 선언 | `useState(0)` | `ref(0)` / `reactive({})` | `private int count = 0;` |
| Props | `function Comp({name})` | `defineProps(['name'])` | `[Parameter] public string Name` |
| 이벤트 | `onClick={handler}` | `@click="handler"` | `@onclick="Handler"` |
| 조건 렌더링 | `{show && <div/>}` | `v-if="show"` | `@if (show) { <div/> }` |
| 리스트 렌더링 | `.map(item => ...)` | `v-for="item in items"` | `@foreach (var item in items)` |
| 양방향 바인딩 | `value + onChange` | `v-model` | `@bind` |
| 생명주기 (mount) | `useEffect(() => {}, [])` | `onMounted()` | `OnInitializedAsync()` |
| 생명주기 (update) | `useEffect(() => {}, [dep])` | `watch()` | `OnParametersSetAsync()` |
| 생명주기 (cleanup) | `return () => {}` | `onUnmounted()` | `Dispose()` |
| Context | `useContext()` | `provide/inject` | `CascadingParameter` |
| 라우팅 | `<Route>` (React Router) | `<RouterView>` | `@page "/path"` |
| 네비게이션 | `useNavigate()` | `useRouter()` | `NavigationManager` |

## 실습 프로젝트

각 챕터에는 실전 프로젝트가 포함되어 있습니다.

### Chapter 9 실습: 호스팅 모델 탐험
- Blazor Server 프로젝트 생성 및 실행
- Blazor WebAssembly 프로젝트 생성 및 비교
- 간단한 카운터와 날씨 예보 앱 구현
- React 버전과 Blazor 버전 비교 분석
- Hot Reload와 디버깅 연습

### Chapter 10 실습: Todo 애플리케이션
완전한 기능의 Todo 앱을 Blazor로 만듭니다:
- Todo 항목 추가/삭제/완료 표시
- 필터링 (전체/활성/완료)
- 로컬 스토리지에 데이터 저장
- 편집 모드와 유효성 검사
- 컴포넌트 분리 (TodoList, TodoItem, TodoForm)
- React/Vue Todo 앱과 비교

### Chapter 11 실습: 실시간 대시보드
프로덕션 수준의 대시보드 애플리케이션:
- 실시간 데이터 스트리밍 (SignalR)
- 차트 라이브러리 통합 (Chart.js 또는 ApexCharts)
- 인증 필요 페이지
- 사용자 역할별 UI 표시
- 반응형 레이아웃 (Tailwind CSS 또는 Bootstrap)
- 데이터 가상화 (큰 테이블)
- 로딩 상태와 에러 처리
- PWA 설정 (Blazor WebAssembly)

## Blazor 생태계와 리소스

**공식 리소스:**
- [공식 Blazor 문서](https://docs.microsoft.com/aspnet/core/blazor/)
- [Blazor University](https://blazor-university.com/): 포괄적인 튜토리얼
- [Awesome Blazor](https://github.com/AdrienTorris/awesome-blazor): 큐레이션된 리소스 목록

**컴포넌트 라이브러리:**
- **MudBlazor**: Material Design 기반, 가장 인기 있는 UI 라이브러리
- **Radzen Blazor**: 70+ 컴포넌트, 무료 오픈소스
- **Blazorise**: Bootstrap, Material, Ant Design 지원
- **Syncfusion Blazor**: 엔터프라이즈급, 80+ 컴포넌트 (유료)
- **Telerik UI for Blazor**: 프로페셔널 컴포넌트 (유료)

**상태 관리:**
- **Fluxor**: Redux 패턴을 Blazor에 적용
- **Blazor State**: 경량 상태 관리
- **Cascading Values**: 내장 방식

**유틸리티 라이브러리:**
- **Blazored**: LocalStorage, Toast, Modal 등 유틸리티 모음
- **BlazorStrap**: Bootstrap 컴포넌트
- **MatBlazor**: Material Design 컴포넌트

## 마이그레이션 전략

기존 React/Vue 앱을 Blazor로 마이그레이션할 계획이라면:

**점진적 마이그레이션:**
1. 백엔드 API는 그대로 유지 (ASP.NET Core)
2. 새로운 기능부터 Blazor로 구현
3. 기존 React/Vue 컴포넌트는 JavaScript Interop으로 재사용
4. 중요한 페이지부터 하나씩 재작성
5. 충분히 검증된 후 전체 전환

**하이브리드 접근:**
- 공개 웹사이트: Next.js/Nuxt.js 유지
- 관리자 대시보드: Blazor로 새로 구현
- API 공유: ASP.NET Core로 통합

## 학습 경로

Part 4는 순차적으로 학습하도록 설계되었습니다:

1. **Chapter 9부터 시작하세요**: Blazor의 개념과 호스팅 모델을 이해하는 것이 모든 것의 기초입니다. React/Vue와 비교하며 친숙함을 느껴보세요.

2. **Chapter 10으로 실전 경험을**: 컴포넌트 개발은 Blazor의 핵심입니다. Todo 앱을 만들며 컴포넌트 패턴을 체화하세요.

3. **Chapter 11로 마무리**: 고급 패턴은 프로덕션 앱을 위한 것입니다. 실시간 대시보드를 만들며 모든 것을 종합하세요.

## 다음 단계

Part 4를 마치면, 여러분은 C#으로 풀스택 웹 애플리케이션을 만들 수 있습니다. Blazor 프론트엔드와 ASP.NET Core 백엔드를 하나의 언어로 통합하는 경험은 독특하고 강력합니다.

하지만 여정은 여기서 끝나지 않습니다:

**Part 5: Entity Framework Core**에서는 데이터베이스 접근을 배웁니다. Blazor는 UI 계층이고, EF Core는 데이터 계층입니다. 둘을 결합하면 완전한 데이터 기반 애플리케이션이 완성됩니다.

**Part 6: API 개발**에서는 Blazor WebAssembly와 통신할 RESTful API를 심화 학습합니다. GraphQL, SignalR, gRPC 같은 고급 통신 패턴도 탐구합니다.

**Part 7: 프론트엔드 통합**에서는 Blazor와 React/Vue를 함께 사용하는 하이브리드 전략을 배웁니다. 각 도구의 강점을 활용하는 방법입니다.

지금 바로 Chapter 9로 이동하여, 첫 Blazor 컴포넌트를 작성해보세요! C#으로 버튼을 클릭하고, 상태를 업데이트하며, UI가 자동으로 변경되는 마법을 경험할 시간입니다.

---

## 참고 자료

- [공식 Blazor 문서](https://docs.microsoft.com/aspnet/core/blazor/)
- [Blazor University](https://blazor-university.com/)
- [Awesome Blazor GitHub](https://github.com/AdrienTorris/awesome-blazor)
- [WebAssembly 공식 사이트](https://webassembly.org/)
- [.NET Blog - Blazor 카테고리](https://devblogs.microsoft.com/dotnet/category/blazor/)

**예상 학습 시간**: 2-3주 (각 챕터당 5-7일, 실습 포함)

**선수 지식**: Part 1-2 완료, React/Vue 중 하나의 기본 경험

---

*"The future of web development is not JavaScript or C#. It's the ability to choose the right tool for the right job, and to use them together seamlessly."* - Blazor 개발 팀
