---
title: "Chapter 9 - Blazor 소개 - C#으로 작성하는 프론트엔드"
---

# Chapter 9: Blazor 소개 - C#으로 작성하는 프론트엔드

## 브라우저에서 실행되는 C#: 불가능했던 꿈의 실현

2015년, React가 프론트엔드 개발의 패러다임을 바꾸고 있을 때, 백엔드 개발자들은 종종 이런 생각을 했습니다. "C#으로 프론트엔드도 작성할 수 있다면 얼마나 좋을까?" 강력한 타입 시스템, 풍부한 라이브러리, 익숙한 개발 도구—이 모든 것을 브라우저에서도 사용할 수 있다면 말이죠.

하지만 그건 불가능했습니다. 브라우저는 JavaScript만 이해하니까요. Silverlight라는 플러그인 기반 기술이 있었지만, 모바일에서 작동하지 않았고, 결국 2021년 지원이 종료되었습니다. TypeScript는 JavaScript에 타입을 추가했지만, 결국 JavaScript로 컴파일됩니다. C#은 서버에만 머물러야 했습니다.

그런데 2017년, 모든 것을 바꾼 사건이 일어났습니다. WebAssembly가 모든 주요 브라우저에서 지원되기 시작한 것입니다. 이는 웹 플랫폼의 역사에서 JavaScript 이후 가장 혁명적인 변화였습니다. 브라우저가 이제 JavaScript 뿐만 아니라 다른 언어로 작성된 코드도 실행할 수 있게 된 것입니다.

Microsoft의 .NET 팀은 이 기회를 놓치지 않았습니다. "만약 .NET 런타임 자체를 WebAssembly로 컴파일한다면?" 이 대담한 아이디어는 Blazor라는 프레임워크로 탄생했습니다. 2018년 실험적 프로젝트로 시작했고, 2020년 .NET 5에서 프로덕션 레벨로 출시되었습니다. 이제 C#으로 프론트엔드를 작성하는 것이 현실이 되었습니다.

### WebAssembly란 정확히 무엇인가?

WebAssembly(줄여서 Wasm)는 브라우저에서 실행할 수 있는 저수준 바이트코드 형식입니다. JavaScript가 텍스트 기반 스크립트 언어라면, WebAssembly는 이진 형식의 어셈블리 언어입니다. 브라우저는 이 바이트코드를 네이티브 머신 코드로 직접 컴파일하여 실행합니다.

이것이 왜 혁명적일까요? 몇 가지 이유가 있습니다:

**1. 언어 독립성**

WebAssembly는 특정 언어에 종속되지 않습니다. C, C++, Rust, Go, C#—어떤 언어든 WebAssembly로 컴파일할 수 있는 컴파일러만 있으면 브라우저에서 실행할 수 있습니다. 이는 수십 년간 축적된 네이티브 코드 라이브러리를 웹에서 활용할 수 있다는 의미입니다.

예를 들어, OpenCV(컴퓨터 비전 라이브러리)는 C++로 작성되었습니다. 이를 WebAssembly로 컴파일하면 브라우저에서 직접 이미지 처리를 할 수 있습니다. FFmpeg(비디오 처리)도 마찬가지입니다. AutoCAD는 수백만 줄의 C++ 코드를 WebAssembly로 포팅하여 웹에서 CAD 작업을 가능하게 했습니다.

**2. 성능**

JavaScript는 JIT(Just-In-Time) 컴파일로 최적화되지만, 텍스트를 파싱하고 타입을 추론하는 오버헤드가 있습니다. WebAssembly는 이미 컴파일된 바이트코드이므로 파싱 단계를 건너뛸 수 있습니다. 또한 정적 타입이므로 타입 추론이 필요 없습니다.

벤치마크에 따르면, CPU 집약적 작업에서 WebAssembly는 JavaScript보다 30-80% 빠릅니다. 복잡한 수학 계산, 알고리즘 실행, 이미지/비디오 처리, 게임 물리 엔진—이런 작업에서 성능 차이가 극적입니다.

Figma는 렌더링 엔진을 C++에서 WebAssembly로 포팅하여 3배의 성능 향상을 얻었습니다. Google Earth는 WebAssembly로 전환한 후 로딩 속도가 크게 개선되었습니다.

**3. 작은 크기와 빠른 다운로드**

WebAssembly는 이진 형식이므로 텍스트 기반 JavaScript보다 크기가 작습니다. gzip 압축을 적용하면 JavaScript와 비슷하거나 더 작은 크기로 전송할 수 있습니다. 또한 스트리밍 컴파일을 지원하여, 다운로드하면서 동시에 컴파일할 수 있습니다.

**4. 보안**

WebAssembly는 샌드박스 환경에서 실행되며, JavaScript와 동일한 보안 모델을 따릅니다. 파일 시스템에 직접 접근하거나, 네트워크 요청을 임의로 할 수 없습니다. 모든 것이 브라우저의 제어 하에 있습니다.

### .NET과 WebAssembly: 완벽한 조합

.NET 런타임을 WebAssembly로 컴파일한다는 것은 구체적으로 무엇을 의미할까요? Mono라는 .NET의 경량 런타임을 WebAssembly로 포팅한 것입니다. 이 Wasm 버전의 Mono는 약 2-3MB 크기이며, 브라우저가 이를 다운로드하면 C# 코드를 실행할 수 있는 가상 머신이 브라우저 안에 생기는 것입니다.

여기서 중요한 점은 C# 코드가 JavaScript로 변환되는 것이 아니라는 것입니다. TypeScript는 JavaScript로 트랜스파일됩니다. 하지만 Blazor WebAssembly는 C# 바이트코드(DLL)를 그대로 브라우저에 다운로드하고, Wasm 런타임이 이를 해석하여 실행합니다. 완전히 다른 접근 방식입니다.

```
TypeScript → JavaScript (브라우저가 JavaScript 실행)
C# → DLL → WebAssembly 런타임 → 네이티브 코드 (브라우저가 Wasm 실행)
```

이는 몇 가지 흥미로운 결과를 낳습니다:

- **디버깅**: 브라우저 개발자 도구에서 실제 C# 코드를 디버깅할 수 있습니다. 중단점을 설정하고, 변수를 검사하며, 호출 스택을 볼 수 있습니다.
- **NuGet 패키지**: 서버에서 사용하던 NuGet 패키지를 브라우저에서도 사용할 수 있습니다 (플랫폼 독립적인 패키지에 한해).
- **코드 공유**: 백엔드와 프론트엔드에서 같은 C# 코드를 공유할 수 있습니다. DTO, 유효성 검사 로직, 유틸리티 함수—모든 것을 재사용할 수 있습니다.

## Blazor의 세 가지 호스팅 모델: 선택의 자유

Blazor의 가장 독특한 특징은 호스팅 모델을 선택할 수 있다는 것입니다. 같은 컴포넌트 코드를 작성하되, 어디서 실행할지 배포 시점에 결정할 수 있습니다. 이는 React나 Vue에는 없는 유연성입니다.

### 1. Blazor Server: 실시간으로 연결된 UI

Blazor Server는 기술적으로 가장 흥미로운 모델입니다. 사용자가 페이지를 열면, 브라우저와 서버 사이에 SignalR 연결이 만들어집니다. SignalR은 WebSocket을 기반으로 한 실시간 양방향 통신 라이브러리입니다.

**작동 방식:**

1. 사용자가 `https://example.com` 방문
2. 서버가 초기 HTML과 Blazor 클라이언트 JavaScript(아주 작음, ~100KB) 전송
3. Blazor 클라이언트가 서버에 SignalR 연결 설정
4. 사용자가 버튼 클릭
5. 클릭 이벤트가 SignalR을 통해 서버로 전송
6. 서버에서 C# 이벤트 핸들러 실행
7. UI 변경사항(diff)만 클라이언트로 전송
8. Blazor 클라이언트가 DOM 업데이트

이 모델의 핵심은 **UI 로직이 서버에서 실행된다**는 것입니다. 클라이언트는 "멍청한" 렌더러 역할만 합니다. React의 서버 컴포넌트와 비슷하지만, 훨씬 더 인터랙티브합니다.

**장점:**

**빠른 초기 로딩**: 클라이언트가 다운로드할 것이 거의 없습니다. JavaScript 번들, .NET 런타임, 애플리케이션 DLL—아무것도 필요 없습니다. 단지 작은 Blazor 클라이언트 스크립트만 있으면 됩니다. 초기 로딩이 Blazor WebAssembly보다 10배 빠를 수 있습니다.

**전체 .NET API 접근**: 서버에서 실행되므로, 파일 시스템, 데이터베이스, 서드파티 라이브러리—모든 것에 접근할 수 있습니다. Entity Framework Core로 직접 데이터베이스 쿼리를 실행하고, 결과를 UI에 바인딩할 수 있습니다. API 엔드포인트를 만들 필요가 없습니다.

```razor
@page "/users"
@inject ApplicationDbContext DbContext

<h1>사용자 목록</h1>

<ul>
  @foreach (var user in users)
  {
    <li>@user.Name - @user.Email</li>
  }
</ul>

@code {
  private List<User> users = new();

  protected override async Task OnInitializedAsync()
  {
    // 컴포넌트에서 직접 데이터베이스 쿼리!
    users = await DbContext.Users.ToListAsync();
  }
}
```

**강화된 보안**: 비즈니스 로직이 서버에 있으므로, 클라이언트에 노출되지 않습니다. 사용자는 코드를 볼 수 없고, 디컴파일할 수도 없습니다. API 키, 데이터베이스 연결 문자열 같은 비밀이 안전합니다.

**작은 클라이언트 요구사항**: 저사양 기기에서도 잘 작동합니다. 클라이언트는 단지 이벤트를 전송하고 DOM을 업데이트할 뿐이므로, CPU나 메모리가 많이 필요하지 않습니다.

**단점:**

**서버 연결 필수**: 네트워크가 끊기면 앱이 작동하지 않습니다. 지하철이나 비행기에서 오프라인으로 사용할 수 없습니다. SignalR 연결이 끊어지면 재연결을 시도하지만, 그 동안 사용자는 아무것도 할 수 없습니다.

**네트워크 지연**: 모든 사용자 인터랙션이 서버 왕복을 필요로 합니다. 버튼을 클릭하면 서버로 이벤트를 보내고, 응답을 기다려야 합니다. 로컬 네트워크(5ms)에서는 체감이 없지만, 원격 서버(100-300ms)에서는 지연을 느낄 수 있습니다.

**서버 리소스**: 동시 사용자가 많으면 서버 메모리와 CPU가 많이 필요합니다. 각 사용자마다 서버에서 상태를 유지해야 하니까요. 10만 명의 동시 사용자를 지원하려면 상당한 서버 리소스가 필요합니다.

**확장성 고려사항**: 로드 밸런싱이 복잡해집니다. SignalR 연결은 "sticky session"을 요구하므로, 사용자가 항상 같은 서버에 연결되어야 합니다. Redis 백플레인으로 해결할 수 있지만, 추가 복잡성입니다.

**언제 Blazor Server를 선택할까?**

- 인트라넷 애플리케이션 (항상 좋은 네트워크)
- 관리자 대시보드 (동시 사용자가 적음)
- 데이터베이스 집약적 앱 (많은 서버 리소스 필요)
- 빠른 초기 로딩이 중요할 때
- 보안이 최우선일 때 (코드 노출 방지)
- 레거시 .NET 라이브러리를 사용해야 할 때

**예시**: 회사 내부 ERP 시스템, CRM 대시보드, 재고 관리 시스템

### 2. Blazor WebAssembly: 진정한 SPA

Blazor WebAssembly는 React SPA와 개념적으로 가장 유사합니다. 모든 것이 클라이언트에서 실행되며, 서버는 단지 정적 파일을 제공할 뿐입니다.

**작동 방식:**

1. 사용자가 `https://example.com` 방문
2. 서버가 index.html, blazor.webassembly.js, .NET 런타임(Wasm), 애플리케이션 DLL들 전송
3. 브라우저가 .NET 런타임을 WebAssembly로 컴파일하여 실행
4. .NET 런타임이 애플리케이션 DLL을 로드
5. Blazor 앱이 브라우저에서 시작
6. 이후 모든 것이 클라이언트에서 실행 (네트워크 불필요)

초기 다운로드 후에는 네이티브 앱처럼 작동합니다. 서버 통신 없이 모든 UI 업데이트, 라우팅, 상태 관리가 클라이언트에서 일어납니다.

**장점:**

**오프라인 지원**: 한 번 로드되면 네트워크 없이도 작동합니다. Service Worker와 결합하면 완전한 PWA(Progressive Web App)를 만들 수 있습니다. 사용자는 앱을 모바일 홈 화면에 설치하고, 오프라인에서도 사용할 수 있습니다.

```javascript
// service-worker.js (Blazor PWA 템플릿에 포함)
self.addEventListener('fetch', event => {
  event.respondWith(
    caches.match(event.request).then(response => {
      return response || fetch(event.request);
    })
  );
});
```

**서버 부하 없음**: 서버는 정적 파일만 제공합니다. CDN에서 서빙할 수 있으므로, 수백만 사용자를 지원해도 서버 비용이 거의 없습니다. GitHub Pages, Netlify, Vercel 같은 정적 호스팅에 배포할 수 있습니다.

**즉각적인 인터랙션**: 네트워크 왕복이 없으므로 모든 UI 업데이트가 즉시 일어납니다. 버튼 클릭, 폼 입력, 애니메이션—모든 것이 로컬 속도로 실행됩니다.

**독립적인 배포**: 프론트엔드와 백엔드를 별도로 배포할 수 있습니다. API 서버를 업데이트해도 클라이언트 앱은 영향받지 않으며, 그 반대도 마찬가지입니다.

**단점:**

**초기 다운로드 크기**: .NET 런타임 자체가 약 2.5MB (gzip 압축 후)이며, 애플리케이션 DLL과 의존성 라이브러리를 합치면 더 커집니다. 작은 앱도 최소 3-5MB는 됩니다. React 앱(200KB-1MB)보다 훨씬 큽니다.

.NET 7/8/9의 AOT(Ahead-of-Time) 컴파일로 크기를 줄일 수 있지만, 여전히 JavaScript보다 큽니다. 트레이드오프가 있습니다: 큰 초기 로드 vs 이후 빠른 성능.

**브라우저 제약**: WebAssembly는 샌드박스에서 실행되므로, 파일 시스템, 직접 데이터베이스 접근, 네이티브 API를 사용할 수 없습니다. 이런 작업은 JavaScript Interop이나 Web API를 통해 우회해야 합니다.

예를 들어, Entity Framework Core를 사용할 수 없습니다 (데이터베이스 드라이버가 네이티브 코드 필요). 대신 HttpClient로 백엔드 API를 호출해야 합니다.

**NuGet 패키지 호환성**: 모든 NuGet 패키지가 WebAssembly에서 작동하는 것은 아닙니다. 플랫폼 독립적인 순수 C# 라이브러리만 사용할 수 있습니다. 네이티브 바이너리에 의존하는 패키지는 안 됩니다.

**메모리 제한**: 브라우저의 메모리 제한을 받습니다. 수 GB의 데이터를 메모리에 로드하는 작업은 불가능합니다.

**언제 Blazor WebAssembly를 선택할까?**

- 공개 웹 애플리케이션 (누구나 접근)
- 오프라인 지원이 필요할 때
- PWA로 만들고 싶을 때
- 서버 비용을 최소화하고 싶을 때
- 즉각적인 사용자 인터랙션이 중요할 때
- 글로벌 사용자 (CDN 활용)

**예시**: Todo 앱, 노트 앱, 계산기, 그래픽 에디터, 게임

### 3. Blazor Hybrid: 웹을 넘어서

Blazor Hybrid는 웹 기술로 네이티브 앱을 만드는 방식입니다. .NET MAUI(Multi-platform App UI), WPF, Windows Forms 같은 네이티브 프레임워크 안에 Blazor 컴포넌트를 임베드합니다.

**작동 방식:**

Electron을 생각하면 이해하기 쉽습니다. Electron은 Chromium(브라우저)과 Node.js를 앱에 번들로 제공합니다. Blazor Hybrid는 비슷하지만 WebView2(Windows의 Edge 기반 브라우저 컨트롤)와 .NET을 사용합니다.

```csharp
// .NET MAUI 앱 (iOS, Android, Windows, macOS)
public class MainPage : ContentPage
{
  public MainPage()
  {
    Content = new BlazorWebView
    {
      HostPage = "wwwroot/index.html"
    };
  }
}
```

Blazor 컴포넌트는 WebView 안에서 렌더링되지만, 네이티브 API에 완전히 접근할 수 있습니다. JavaScript Interop 대신 C# Interop을 사용하여 카메라, GPS, 파일 시스템, 블루투스 등을 제어합니다.

**장점:**

**크로스 플랫폼**: 같은 Blazor 컴포넌트를 Windows, macOS, Linux, iOS, Android에서 실행할 수 있습니다. 코드를 한 번 작성하고 여러 플랫폼에 배포합니다. React Native와 비슷하지만 더 많은 플랫폼을 지원합니다.

**네이티브 API 접근**: 웹 제약이 없습니다. 파일 시스템에 직접 쓰고, 로컬 데이터베이스(SQLite)를 사용하며, 시스템 트레이 아이콘을 만들고, 백그라운드 서비스를 실행할 수 있습니다.

**웹과 네이티브의 혼합**: 일부 화면은 Blazor 컴포넌트로, 일부는 네이티브 컨트롤(XAML, SwiftUI)로 만들 수 있습니다. 최선의 양쪽 세계를 결합합니다.

**코드 재사용**: 웹 앱, 데스크톱 앱, 모바일 앱에서 같은 UI 컴포넌트를 사용합니다. 비즈니스 로직, 데이터 모델, UI 컴포넌트—모든 것을 공유할 수 있습니다.

**단점:**

**앱 크기**: .NET 런타임과 WebView를 포함하므로 앱 크기가 큽니다. 최소 20-30MB는 됩니다.

**플랫폼별 차이**: 완전히 동일한 경험을 모든 플랫폼에서 제공하기 어렵습니다. iOS와 Android는 다른 디자인 가이드라인을 따르므로, 플랫폼별 조정이 필요할 수 있습니다.

**앱 스토어 심사**: iOS App Store, Google Play에 제출하려면 각 플랫폼의 정책을 따라야 합니다.

**언제 Blazor Hybrid를 선택할까?**

- 웹 앱과 모바일 앱을 동시에 만들고 싶을 때
- 네이티브 기능이 필요할 때 (카메라, GPS, 오프라인 데이터베이스)
- 기존 Blazor 컴포넌트를 재사용하고 싶을 때
- Electron 같은 데스크톱 앱을 만들고 싶지만 .NET을 선호할 때

**예시**: 크로스 플랫폼 노트 앱, 오프라인 POS 시스템, 모바일 데이터 수집 앱

## React/Vue와 Blazor 비교: 컴포넌트 패러다임의 유사성

React나 Vue를 알고 있다면 Blazor가 매우 친숙하게 느껴질 것입니다. Microsoft 팀이 React의 성공적인 패턴을 연구하고 채택했기 때문입니다. 컴포넌트 모델, 단방향 데이터 흐름, 선언적 UI—핵심 개념은 동일합니다.

### 컴포넌트 구조

**React (함수형 컴포넌트):**

```jsx
import { useState } from 'react';

function Counter() {
  const [count, setCount] = useState(0);

  function increment() {
    setCount(count + 1);
  }

  return (
    <div>
      <h1>카운터</h1>
      <p>현재 값: {count}</p>
      <button onClick={increment}>증가</button>
      <button onClick={() => setCount(0)}>리셋</button>
    </div>
  );
}

export default Counter;
```

**Blazor:**

```razor
@page "/counter"

<div>
  <h1>카운터</h1>
  <p>현재 값: @currentCount</p>
  <button @onclick="Increment">증가</button>
  <button @onclick="() => currentCount = 0">리셋</button>
</div>

@code {
  private int currentCount = 0;

  private void Increment()
  {
    currentCount++;
  }
}
```

**핵심 유사점:**

1. **상태 선언**: `useState` vs 클래스 필드 (`private int currentCount`)
2. **이벤트 핸들러**: `onClick` vs `@onclick`
3. **표현식 삽입**: `{count}` vs `@currentCount`
4. **인라인 함수**: `() => setCount(0)` vs `() => currentCount = 0`

**차이점:**

1. **상태 업데이트**: React는 `setCount` 함수 호출 필요, Blazor는 직접 변경 (`currentCount++`)
2. **재렌더링 트리거**: React는 `setState`가 트리거, Blazor는 자동 감지
3. **타입 시스템**: React는 런타임, Blazor는 컴파일 타임

### Props와 Parameters

**React:**

```jsx
// Child.jsx
function Greeting({ name, age, onGreet }) {
  return (
    <div>
      <h2>안녕하세요, {name}님 ({age}세)</h2>
      <button onClick={onGreet}>인사하기</button>
    </div>
  );
}

// Parent.jsx
function App() {
  const handleGreet = () => alert('안녕하세요!');

  return <Greeting name="홍길동" age={30} onGreet={handleGreet} />;
}
```

**Blazor:**

```razor
@* Greeting.razor *@
<div>
  <h2>안녕하세요, @Name님 (@Age세)</h2>
  <button @onclick="OnGreet">인사하기</button>
</div>

@code {
  [Parameter]
  public string Name { get; set; } = string.Empty;

  [Parameter]
  public int Age { get; set; }

  [Parameter]
  public EventCallback OnGreet { get; set; }
}

@* Parent.razor *@
<Greeting Name="홍길동" Age="30" OnGreet="@HandleGreet" />

@code {
  private void HandleGreet()
  {
    // alert은 JavaScript이므로 JS Interop 필요
    // 여기서는 간단히 로그
    Console.WriteLine("안녕하세요!");
  }
}
```

**핵심 유사점:**

1. **Props/Parameters**: 부모에서 자식으로 데이터 전달
2. **이벤트 콜백**: 자식에서 부모로 이벤트 전달
3. **컴포넌트 합성**: 재사용 가능한 UI 블록

**차이점:**

1. **타입 선언**: React는 TypeScript 필요, Blazor는 기본 제공
2. **기본값**: React는 `defaultProps` 또는 매개변수 기본값, Blazor는 프로퍼티 초기화
3. **이벤트 타입**: React는 함수, Blazor는 `EventCallback` (비동기 지원)

### 생명주기

**React (Hooks):**

```jsx
import { useState, useEffect } from 'react';

function UserProfile({ userId }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // 컴포넌트 마운트 또는 userId 변경 시
    setLoading(true);
    fetch(`/api/users/${userId}`)
      .then(res => res.json())
      .then(data => {
        setUser(data);
        setLoading(false);
      });

    // cleanup (언마운트 시)
    return () => {
      console.log('컴포넌트 언마운트');
    };
  }, [userId]);

  if (loading) return <div>로딩 중...</div>;
  if (!user) return <div>사용자 없음</div>;

  return <div>{user.name}</div>;
}
```

**Blazor:**

```razor
@if (loading)
{
  <div>로딩 중...</div>
}
else if (user == null)
{
  <div>사용자 없음</div>
}
else
{
  <div>@user.Name</div>
}

@code {
  [Parameter]
  public int UserId { get; set; }

  private User? user;
  private bool loading = true;

  protected override async Task OnParametersSetAsync()
  {
    // Parameter 변경 시 호출
    loading = true;
    user = await Http.GetFromJsonAsync<User>($"/api/users/{UserId}");
    loading = false;
  }

  public void Dispose()
  {
    // 컴포넌트 언마운트 시
    Console.WriteLine("컴포넌트 언마운트");
  }
}

@implements IDisposable
```

**생명주기 메서드 비교:**

| React Hook | Blazor 메서드 | 호출 시점 |
|-----------|--------------|---------|
| `useEffect(() => {}, [])` | `OnInitialized` / `OnInitializedAsync` | 컴포넌트 처음 생성 |
| `useEffect(() => {}, [deps])` | `OnParametersSet` / `OnParametersSetAsync` | Parameter 변경 |
| `useLayoutEffect` | `OnAfterRender` / `OnAfterRenderAsync` | 렌더링 후 (DOM 접근 가능) |
| `return () => {}` (cleanup) | `Dispose` (IDisposable) | 컴포넌트 제거 |

### 조건부 렌더링과 리스트

**React:**

```jsx
function ProductList({ products }) {
  return (
    <div>
      <h1>상품 목록</h1>
      {products.length === 0 ? (
        <p>상품이 없습니다.</p>
      ) : (
        <ul>
          {products.map(product => (
            <li key={product.id}>
              <h3>{product.name}</h3>
              <p>{product.price}원</p>
              {product.onSale && <span className="badge">할인</span>}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
```

**Blazor:**

```razor
@page "/products"

<div>
  <h1>상품 목록</h1>
  @if (products.Count == 0)
  {
    <p>상품이 없습니다.</p>
  }
  else
  {
    <ul>
      @foreach (var product in products)
      {
        <li>
          <h3>@product.Name</h3>
          <p>@product.Price원</p>
          @if (product.OnSale)
          {
            <span class="badge">할인</span>
          }
        </li>
      }
    </ul>
  }
</div>

@code {
  private List<Product> products = new();
}
```

**핵심 패턴:**

1. **조건부**: `? :` vs `@if`
2. **논리 AND**: `&&` vs 중첩 `@if`
3. **리스트**: `.map()` vs `@foreach`
4. **key prop**: React는 명시적, Blazor는 자동 (서버 렌더링이므로 reconciliation 불필요)

## 프로젝트 생성: 첫 Blazor 애플리케이션

이론은 충분합니다. 실제로 Blazor 앱을 만들어봅시다.

### 사전 준비

.NET 8 SDK 이상이 필요합니다. 설치 확인:

```bash
dotnet --version
# 8.0.0 이상이어야 함
```

### Blazor Server 프로젝트 생성

```bash
# 프로젝트 생성
dotnet new blazorserver -n MyBlazorServerApp
cd MyBlazorServerApp

# 실행
dotnet run
```

브라우저에서 `https://localhost:5001` 열기 (포트는 다를 수 있음).

### Blazor WebAssembly 프로젝트 생성

```bash
# 프로젝트 생성
dotnet new blazorwasm -n MyBlazorWasmApp
cd MyBlazorWasmApp

# 실행
dotnet run
```

### Blazor WebAssembly (ASP.NET Core 호스팅)

더 일반적인 패턴은 Blazor WebAssembly 클라이언트와 ASP.NET Core API 서버를 함께 만드는 것입니다.

```bash
# 호스팅된 프로젝트 생성
dotnet new blazorwasm -ho -n MyHostedApp
cd MyHostedApp
```

이 템플릿은 세 개의 프로젝트를 만듭니다:

```
MyHostedApp/
  Client/         # Blazor WebAssembly 앱
  Server/         # ASP.NET Core API 서버
  Shared/         # 공유 모델 (DTO)
```

`Shared` 프로젝트가 중요합니다. 여기에 정의된 C# 클래스를 클라이언트와 서버 모두에서 사용할 수 있습니다.

```csharp
// Shared/WeatherForecast.cs
public class WeatherForecast
{
  public DateOnly Date { get; set; }
  public int TemperatureC { get; set; }
  public string? Summary { get; set; }
}

// Server/Controllers/WeatherForecastController.cs
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
  [HttpGet]
  public IEnumerable<WeatherForecast> Get()
  {
    // WeatherForecast 타입 반환
  }
}

// Client/Pages/FetchData.razor
@code {
  private WeatherForecast[]? forecasts;

  protected override async Task OnInitializedAsync()
  {
    // 같은 WeatherForecast 타입 사용!
    forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
  }
}
```

타입 불일치 오류가 불가능합니다. 컴파일러가 보장합니다.

## 프로젝트 구조 이해하기

Blazor Server 프로젝트를 열어봅시다.

```
MyBlazorServerApp/
  Pages/
    Counter.razor        # /counter 페이지
    FetchData.razor      # /fetchdata 페이지
    Index.razor          # / (홈) 페이지
  Shared/
    MainLayout.razor     # 레이아웃
    NavMenu.razor        # 네비게이션
  wwwroot/                # 정적 파일 (CSS, JS, 이미지)
    css/
    favicon.ico
  _Imports.razor         # 전역 using 문
  App.razor              # 루트 컴포넌트
  Program.cs             # 애플리케이션 진입점
  appsettings.json       # 구성
```

### 핵심 파일 살펴보기

**`App.razor`** - 라우터 설정

```razor
<Router AppAssembly="@typeof(App).Assembly">
  <Found Context="routeData">
    <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
  </Found>
  <NotFound>
    <PageTitle>Not found</PageTitle>
    <LayoutView Layout="@typeof(MainLayout)">
      <p role="alert">페이지를 찾을 수 없습니다.</p>
    </LayoutView>
  </NotFound>
</Router>
```

이는 React Router의 설정과 유사합니다.

**`Pages/Counter.razor`** - 간단한 카운터

```razor
@page "/counter"

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p role="status">Current count: @currentCount</p>

<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
  private int currentCount = 0;

  private void IncrementCount()
  {
    currentCount++;
  }
}
```

`@page "/counter"` 지시문이 라우팅을 정의합니다. React Router의 `<Route path="/counter">` 와 동일합니다.

**`Program.cs`** - 앱 설정

```csharp
var builder = WebApplication.CreateBuilder(args);

// Blazor 서비스 추가
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 의존성 주입
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// 미들웨어 파이프라인
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();        // SignalR 엔드포인트
app.MapFallbackToPage("/_Host");  // Blazor 호스트 페이지

app.Run();
```

React 앱에는 이런 설정이 없습니다. Blazor는 서버 통합이 기본이므로 미들웨어, 의존성 주입, 라우팅을 설정해야 합니다.

## 개발 경험: Hot Reload와 디버깅

### Hot Reload

.NET 6 이상에서는 Hot Reload가 기본 제공됩니다. 코드를 변경하면 앱을 재시작하지 않고 즉시 반영됩니다.

```bash
dotnet watch run
```

Blazor 컴포넌트의 코드를 변경하면:

1. 변경 감지
2. 재컴파일
3. 브라우저에 자동 적용 (새로고침 없음)

React의 HMR(Hot Module Replacement)과 유사하지만, 더 안정적입니다. 상태도 유지됩니다.

### 브라우저 디버깅

Blazor WebAssembly는 브라우저 개발자 도구에서 디버깅할 수 있습니다.

1. 앱 실행: `dotnet run`
2. Chrome에서 `Shift + Alt + D` (Windows) 또는 `Shift + Cmd + D` (Mac)
3. 디버깅 지침 따라 새 창 열기
4. C# 코드에 중단점 설정
5. 변수 검사, 호출 스택 확인

실제 C# 소스 코드가 브라우저에 표시됩니다!

### Visual Studio / Rider 디버깅

IDE에서 F5를 누르면 일반 .NET 앱처럼 디버깅됩니다:

- 중단점 설정
- 변수 검사
- 호출 스택 탐색
- 즉시 창에서 코드 실행
- 편집 후 계속 (Edit and Continue)

React 앱에서는 Chrome DevTools를 사용해야 하지만, Blazor는 IDE의 강력한 디버거를 그대로 사용할 수 있습니다.

## 다음 단계

이 챕터에서 Blazor의 기초를 다졌습니다:

- WebAssembly의 혁명적인 가능성
- 세 가지 호스팅 모델의 장단점
- React/Vue와의 개념적 유사성
- 프로젝트 생성과 구조
- 개발 환경과 도구

하지만 실제 애플리케이션을 만들려면 더 깊이 파고들어야 합니다. Chapter 10에서는 Blazor 컴포넌트의 모든 측면을 마스터합니다:

- Parameters와 EventCallback로 컴포넌트 통신
- 양방향 바인딩과 폼 처리
- 생명주기 메서드 활용
- JavaScript Interop
- 상태 관리 패턴

실습에서는 완전한 기능의 Todo 애플리케이션을 만들며, React 버전과 직접 비교해봅니다. Blazor의 진짜 강력함을 경험할 준비를 하세요!

---

## 학습 자료

- [공식 Blazor 문서](https://docs.microsoft.com/aspnet/core/blazor/)
- [Blazor University](https://blazor-university.com/) - 무료 온라인 교과서
- [WebAssembly 소개](https://webassembly.org/getting-started/developers-guide/)
- [.NET Blog - Blazor](https://devblogs.microsoft.com/dotnet/category/blazor/)

다음 챕터: [Chapter 10: Blazor 컴포넌트 개발](../chapter10/index.md)
