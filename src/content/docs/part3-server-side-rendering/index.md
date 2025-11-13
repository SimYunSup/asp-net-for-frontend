---
title: "Part 3 - 서버 사이드 렌더링 - Razor Pages와 MVC"
---

# Part 3: 서버 사이드 렌더링 - Razor Pages와 MVC

## 프론트엔드 기술을 서버에서: 새로운 관점의 UI 개발

Part 2에서 RESTful API를 구축하는 방법을 배웠다면, 이제 한 걸음 더 나아갈 차례입니다. API는 데이터를 제공하지만, 실제 사용자가 보는 것은 HTML입니다. React, Vue, Angular로 클라이언트 사이드에서 UI를 렌더링하는 데 익숙한 여러분에게, 서버 사이드 렌더링(SSR)은 낯설지만 흥미로운 영역입니다.

"잠깐, 서버에서 HTML을 렌더링한다고? 그건 구식 아닌가요?" 라고 생각할 수 있습니다. PHP, JSP, 오래된 ASP.NET의 시대를 떠올리며 말이죠. 하지만 현대의 서버 사이드 렌더링은 그때와 완전히 다릅니다. Next.js, Nuxt.js, SvelteKit이 왜 SSR을 핵심 기능으로 제공하는지 생각해보세요. SEO, 초기 로딩 성능, 그리고 단순함—이 모든 것이 SSR의 장점입니다.

ASP.NET Core의 Razor Pages와 MVC는 이 SSR 개념을 강력한 타입 시스템과 결합합니다. JSX를 아는 개발자라면 Razor 문법이 놀라울 정도로 친숙하게 느껴질 것입니다. `{expression}`이 `@expression`으로, `.map()`이 `@foreach`로 바뀐 것뿐입니다. 하지만 그 아래에는 컴파일 타임 타입 체크, 자동 HTML 인코딩, 강력한 모델 바인딩이 숨어 있습니다.

### 왜 서버 사이드 렌더링을 배워야 하는가?

프론트엔드 개발자로서 이미 React나 Vue로 훌륭한 UI를 만들 수 있는데, 왜 서버 사이드 렌더링을 배워야 할까요? 실용적인 이유들이 있습니다:

**1. SEO: 검색 엔진이 실제로 보는 것**

구글이 JavaScript를 실행할 수 있다고 해도, 완벽하지 않습니다. 특히 복잡한 SPA에서는 크롤러가 모든 콘텐츠를 인덱싱하지 못할 수 있습니다. 서버에서 렌더링된 HTML은 검색 엔진에 즉시 노출되며, Open Graph 메타 태그도 정확하게 설정할 수 있습니다. 블로그, 전자상거래, 마케팅 사이트라면 이는 필수적입니다.

Next.js가 등장하기 전, React는 SEO가 약점이었습니다. 이제 여러분은 ASP.NET Core로 동일한 문제를 더 간단하게 해결할 수 있습니다. 추가 빌드 도구나 복잡한 설정 없이, Razor Pages만으로 완벽하게 크롤링 가능한 페이지를 만듭니다.

**2. 초기 로딩 성능: Time to First Paint의 중요성**

사용자가 URL을 입력하고 화면에 무언가 나타나기까지의 시간은 사용자 경험에 결정적입니다. SPA는 JavaScript 번들을 다운로드하고, 파싱하고, 실행한 뒤에야 첫 화면을 보여줍니다. 서버 사이드 렌더링은 서버에서 완성된 HTML을 전송하므로, 브라우저는 즉시 렌더링할 수 있습니다.

```
SPA 로딩 과정:
1. HTML 다운로드 (빈 div)
2. JavaScript 번들 다운로드 (수 MB)
3. JavaScript 파싱 및 실행
4. API 호출 → 데이터 대기
5. 첫 화면 렌더링

SSR 로딩 과정:
1. HTML 다운로드 (완성된 페이지)
2. 첫 화면 렌더링 (즉시!)
3. JavaScript hydration (선택적)
```

모바일 네트워크나 저사양 기기에서 이 차이는 극적입니다. 전자상거래 사이트에서 1초의 지연은 7%의 전환율 감소로 이어진다는 연구도 있습니다.

**3. 단순함: 복잡성을 제거하는 힘**

React 앱을 만들려면 얼마나 많은 도구가 필요한지 생각해보세요. Webpack/Vite, Babel, TypeScript, ESLint, Prettier, React Router, Redux/Zustand, Axios/React Query, CSS-in-JS... 각각은 훌륭한 도구지만, 전체를 합치면 압도적인 복잡성이 됩니다. "JavaScript fatigue"라는 말이 괜히 생긴 게 아닙니다.

Razor Pages는 이 모든 것을 하나의 프레임워크로 통합합니다. 라우팅? 폴더 구조만으로 해결됩니다. 상태 관리? 서버 사이드 모델로 충분합니다. API 호출? 필요 없습니다. 페이지 컴포넌트 내에서 직접 데이터베이스에 접근합니다. 빌드 도구? .NET SDK 하나면 됩니다.

간단한 관리자 대시보드, 내부 도구, CRUD 애플리케이션에는 React의 강력함이 과할 수 있습니다. Razor Pages로 몇 시간 만에 만들 수 있는 것을 며칠 동안 설정하고 싶지는 않을 것입니다.

**4. 풀스택 단일 프로젝트: 배포의 단순함**

React + Node.js API 구조는 두 개의 프로젝트, 두 개의 배포, 두 배의 복잡성을 의미합니다. CORS 설정, 환경 변수 동기화, API URL 관리, 별도의 빌드 파이프라인... Razor Pages나 MVC는 모든 것이 하나의 프로젝트입니다. 백엔드 로직과 프론트엔드 뷰가 같은 솔루션에 있으며, 한 번의 배포로 모든 것이 작동합니다.

개발 환경도 간단해집니다. `dotnet run` 하나로 전체 애플리케이션이 실행됩니다. 프론트엔드 서버와 백엔드 서버를 각각 띄우고, 프록시 설정을 확인할 필요가 없습니다.

**5. 하이브리드 접근: 최선의 양쪽 세계**

가장 강력한 점은 선택할 필요가 없다는 것입니다. Razor Pages/MVC로 대부분의 페이지를 만들고, 복잡한 인터랙션이 필요한 부분만 React/Vue 컴포넌트를 삽입할 수 있습니다. 관리자 대시보드는 Razor Pages로, 실시간 대시보드 위젯은 React로 만드는 식입니다.

또는 Razor Pages로 초기 HTML을 렌더링하고, 클라이언트에서 JavaScript로 enhance하는 progressive enhancement 전략도 가능합니다. HTMX, Alpine.js 같은 경량 라이브러리와 조합하면, SPA의 인터랙티비티와 SSR의 단순함을 동시에 얻을 수 있습니다.

### React/Vue 개발자가 보는 Razor: 놀라운 유사성

JSX나 Vue 템플릿을 작성해본 경험이 있다면, Razor는 금방 익숙해질 것입니다. 핵심 개념은 동일합니다: 선언적 UI, 컴포넌트 기반 구조, 데이터 바인딩, 조건부 렌더링, 리스트 렌더링.

**JSX와 Razor 비교:**

```jsx
// React JSX
function UserList({ users }) {
  return (
    <div>
      <h1>사용자 목록</h1>
      {users.length === 0 ? (
        <p>사용자가 없습니다.</p>
      ) : (
        <ul>
          {users.map(user => (
            <li key={user.id}>
              {user.name} - {user.email}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
```

```razor
@* Razor *@
@model List<User>

<div>
  <h1>사용자 목록</h1>
  @if (Model.Count == 0)
  {
    <p>사용자가 없습니다.</p>
  }
  else
  {
    <ul>
      @foreach (var user in Model)
      {
        <li>@user.Name - @user.Email</li>
      }
    </ul>
  }
</div>
```

차이점을 보셨나요? 문법만 약간 다를 뿐, 구조와 흐름은 거의 동일합니다. `{}`가 `@`로, `.map()`이 `@foreach`로 바뀐 것입니다. 더 중요한 것은, Razor는 컴파일 타임에 타입을 검증한다는 것입니다. `@model List<User>`는 이 뷰가 `List<User>` 타입을 받는다는 것을 명시하며, 잘못된 프로퍼티에 접근하면 컴파일 오류가 발생합니다.

### Razor Pages vs MVC: 언제 무엇을 선택할까?

ASP.NET Core는 서버 사이드 렌더링을 위한 두 가지 주요 접근법을 제공합니다. Next.js에 Pages Router와 App Router가 있는 것처럼, ASP.NET Core에는 Razor Pages와 MVC가 있습니다.

**Razor Pages: 단순함과 생산성**

Razor Pages는 페이지 중심적입니다. 각 `.cshtml` 파일은 하나의 페이지를 나타내며, 해당 페이지의 백엔드 로직은 같은 이름의 `.cshtml.cs` 파일(PageModel)에 있습니다. Next.js의 Pages Router와 매우 유사합니다.

```
Pages/
  Index.cshtml          → /
  Index.cshtml.cs
  Products/
    Index.cshtml        → /products
    Index.cshtml.cs
    Details.cshtml      → /products/details?id=1
    Details.cshtml.cs
```

라우팅이 파일 시스템 구조로 자동 결정됩니다. CRUD 애플리케이션, 관리자 패널, 콘텐츠 중심 사이트에 완벽합니다. 컨트롤러를 따로 만들 필요 없이, 페이지와 로직이 함께 있어 코드베이스가 이해하기 쉽습니다.

**MVC: 대규모 애플리케이션의 구조**

MVC(Model-View-Controller)는 관심사를 명확히 분리합니다. 컨트롤러는 요청을 처리하고, 모델은 데이터를 나타내며, 뷰는 UI를 렌더링합니다. 대규모 팀, 복잡한 비즈니스 로직, 재사용 가능한 컴포넌트가 많은 애플리케이션에 적합합니다.

```
Controllers/
  ProductsController.cs  → 모든 제품 관련 로직
Views/
  Products/
    Index.cshtml
    Details.cshtml
    Create.cshtml
Models/
  Product.cs
  ProductViewModel.cs
```

하나의 컨트롤러가 여러 뷰를 처리하며, Areas 기능으로 애플리케이션을 모듈화할 수 있습니다. Admin, API, Public 영역을 명확히 분리하여 대규모 프로젝트를 관리합니다.

**선택 가이드:**

- **Razor Pages를 선택하세요**: 페이지가 명확히 구분되는 애플리케이션, CRUD 중심, 빠른 개발이 필요할 때
- **MVC를 선택하세요**: 복잡한 라우팅, 많은 재사용 로직, 대규모 팀 협업, Areas로 모듈 분리가 필요할 때
- **혼합 사용도 가능합니다**: 같은 프로젝트에서 두 가지를 모두 사용할 수 있습니다!

### Part 3에서 배울 내용

이 파트는 프론트엔드 개발자가 서버 사이드 렌더링의 세계로 자연스럽게 진입할 수 있도록 설계되었습니다. React/Vue의 개념을 Razor로 매핑하며, 각 단계에서 "왜 이렇게 하는가?"를 명확히 합니다.

**Chapter 6: Razor 문법 - JSX를 아는 개발자를 위한 가이드**

Razor 문법의 모든 것을 배웁니다. `@` 기호, 코드 블록, 표현식, 지시문(directives), 레이아웃, 부분 뷰(Partial Views), Tag Helpers... JSX와 직접 비교하며, "React에서는 이렇게 했는데 Razor에서는 어떻게 하지?"라는 질문에 답합니다.

자동 HTML 인코딩은 XSS 공격을 기본적으로 방지하며, 강타입 모델은 런타임 오류를 컴파일 타임에 잡아냅니다. View Components는 React 컴포넌트와 유사하지만, 서버 사이드 로직을 포함할 수 있습니다.

**Chapter 7: Razor Pages - 단순함의 힘**

Next.js Pages Router를 아는 개발자라면 Razor Pages가 매우 친숙할 것입니다. 파일 기반 라우팅, 페이지별 데이터 페칭, 폼 처리, 상태 관리를 배웁니다.

PageModel 클래스는 `OnGet`, `OnPost` 같은 핸들러 메서드를 통해 HTTP 요청을 처리합니다. `[BindProperty]`로 폼 데이터를 자동으로 바인딩하며, 모델 유효성 검사는 데이터 어노테이션만으로 해결됩니다. CSRF 방지, 에러 처리, 리디렉션—모든 것이 프레임워크에 내장되어 있습니다.

실습에서는 완전한 블로그 CRUD 애플리케이션을 만들며, 게시글 목록, 작성, 수정, 삭제, 검색 기능을 구현합니다.

**Chapter 8: MVC 패턴 - 대규모 애플리케이션을 위한 구조**

MVC 아키텍처를 깊이 이해합니다. Flux/Redux를 아는 개발자라면 단방향 데이터 흐름의 개념이 익숙할 것입니다. MVC는 다른 방식이지만, 같은 문제(복잡성 관리)를 해결합니다.

컨트롤러는 사용자 입력을 받아 모델을 업데이트하고, 뷰는 모델을 기반으로 UI를 렌더링합니다. 필터(Filters)는 React의 HOC나 미들웨어와 유사하게 횡단 관심사(인증, 로깅, 캐싱)를 처리합니다.

Areas 기능으로 애플리케이션을 논리적 모듈로 분리하며, 강타입 뷰 모델(ViewModel)은 뷰와 도메인 모델을 분리하여 변경에 강한 아키텍처를 만듭니다.

실습에서는 전자상거래 애플리케이션을 만들며, 상품 카탈로그, 장바구니, 주문 관리, 관리자 영역을 구현합니다.

## 학습 목표

Part 3를 마치면 다음을 할 수 있습니다:

- Razor 문법으로 동적 HTML을 생성할 수 있습니다
- 레이아웃, 부분 뷰, View Components로 재사용 가능한 UI를 구축합니다
- Razor Pages로 페이지 기반 애플리케이션을 빠르게 개발합니다
- 폼 처리, 데이터 바인딩, 유효성 검사를 구현합니다
- MVC 패턴으로 대규모 애플리케이션을 구조화합니다
- 컨트롤러, 뷰, 모델의 역할을 이해하고 효과적으로 분리합니다
- 필터로 횡단 관심사를 처리합니다
- Areas로 애플리케이션을 모듈화합니다
- SEO 친화적이고 접근성 좋은 웹 애플리케이션을 만듭니다

## 챕터 구성

### [Chapter 6: Razor 문법 - JSX를 아는 개발자를 위한 가이드](./chapter6/index.md)

Razor의 모든 것을 JSX/Vue 템플릿과 비교하며 배웁니다.

- `@` 기호와 코드 블록: JSX의 `{}`와 비교
- 조건부 렌더링: `@if`, `@switch` vs `&&`, `? :`
- 리스트 렌더링: `@foreach` vs `.map()`
- Razor 지시문: `@page`, `@model`, `@inject`, `@section`
- 레이아웃과 부분 뷰: React의 Layout 컴포넌트와 비교
- Tag Helpers: HTML을 위한 강력한 도구
- View Components: 서버 사이드 컴포넌트

**핵심 개념**: Razor 표현식, 암묵적/명시적 전환, HTML 인코딩, 강타입 뷰, Tag Helpers, View Components

**실습**: JSX 코드를 Razor로 변환, 동적 UI 렌더링, 재사용 가능한 컴포넌트 생성

### [Chapter 7: Razor Pages - 단순함의 힘](./chapter7/index.md)

Next.js Pages Router와 유사한 Razor Pages로 애플리케이션을 빠르게 개발합니다.

- Razor Pages 모델: 페이지 + PageModel
- 파일 기반 라우팅: Next.js와의 비교
- 핸들러 메서드: OnGet, OnPost, OnPostAsync
- 모델 바인딩: `[BindProperty]`, `[FromQuery]`, `[FromRoute]`
- 폼 처리와 유효성 검사
- TempData, ViewData, ViewBag: 상태 전달 방법
- Anti-forgery 토큰: CSRF 방지
- 페이지 필터와 핸들러

**핵심 개념**: PageModel, 모델 바인딩, POST/Redirect/GET 패턴, 유효성 검사, TempData

**실습**: 블로그 CRUD 애플리케이션 - 게시글 목록/작성/수정/삭제, 검색 기능

### [Chapter 8: MVC 패턴 - 대규모 애플리케이션을 위한 구조](./chapter8/index.md)

전통적이지만 여전히 강력한 MVC로 복잡한 애플리케이션을 구조화합니다.

- MVC 아키텍처: Model, View, Controller의 역할
- 컨트롤러: 요청 처리와 응답 생성
- 액션 메서드와 ActionResult
- 라우팅: 컨벤션 기반 vs 특성 기반
- 강타입 뷰와 뷰 모델(ViewModel)
- 레이아웃, 섹션, 부분 뷰
- 필터: Authorization, Action, Result, Exception
- Areas: 애플리케이션 모듈화
- 모델 바인딩과 유효성 검사

**핵심 개념**: MVC 패턴, 컨트롤러, ActionResult, 라우팅, 필터, Areas, ViewModel

**실습**: 전자상거래 MVC 애플리케이션 - 상품 카탈로그, 장바구니, 주문 관리, 관리자 영역

## React/Vue와 Razor 비교 치트시트

프론트엔드 개발자를 위한 빠른 참조 가이드:

| 개념 | React/JSX | Vue | Razor |
|------|-----------|-----|-------|
| 표현식 | `{user.name}` | `{{ user.name }}` | `@Model.Name` |
| 조건 렌더링 | `{show && <div/>}` | `v-if="show"` | `@if (show) { ... }` |
| 삼항 연산자 | `{x ? a : b}` | `{{ x ? a : b }}` | `@(x ? a : b)` |
| 리스트 렌더링 | `.map(item => ...)` | `v-for="item in items"` | `@foreach (var item in Model) { ... }` |
| 이벤트 핸들링 | `onClick={handler}` | `@click="handler"` | `asp-page-handler="Handler"` |
| Props/Model | `{props.title}` | `{{ props.title }}` | `@Model.Title` |
| 컴포넌트 | `<MyComponent />` | `<MyComponent />` | `<vc:my-component />` |
| 스타일/클래스 | `className={...}` | `:class="..."` | `class="@className"` |

## 실습 프로젝트

각 챕터에는 실전 프로젝트가 포함되어 있습니다.

### Chapter 6 실습: Razor 문법 마스터하기
- JSX 코드를 Razor로 변환하는 연습
- 동적 리스트 렌더링
- 조건부 UI 표시
- Tag Helpers로 폼 생성
- View Components로 재사용 가능한 위젯 만들기

### Chapter 7 실습: 블로그 애플리케이션
Razor Pages로 완전한 블로그 시스템을 만듭니다:
- 게시글 목록 페이지 (페이징 포함)
- 게시글 상세 페이지
- 게시글 작성/수정 페이지 (유효성 검사)
- 게시글 삭제 (확인 페이지)
- 검색 및 태그 필터링
- 댓글 시스템

### Chapter 8 실습: 전자상거래 플랫폼
MVC로 복잡한 전자상거래 애플리케이션을 구축합니다:
- 상품 카탈로그 (카테고리별 필터링, 검색)
- 상품 상세 페이지
- 장바구니 기능 (세션 기반)
- 주문 생성 및 관리
- 관리자 영역 (Areas 사용)
  - 상품 관리 CRUD
  - 주문 관리
  - 대시보드
- 사용자 인증 및 권한 부여 (필터 사용)

## 언제 SSR을, 언제 SPA를 선택할까?

Razor Pages/MVC와 React/Vue 중 어느 것을 선택해야 할지 고민될 수 있습니다. 명확한 가이드라인:

**Razor Pages/MVC를 선택하세요:**
- 콘텐츠 중심 웹사이트 (블로그, 뉴스, 마케팅 사이트)
- SEO가 중요한 서비스
- 간단한 CRUD 애플리케이션
- 관리자 대시보드/내부 도구
- 빠른 개발이 필요할 때
- 적은 JavaScript가 이상적일 때

**React/Vue SPA를 선택하세요:**
- 복잡한 사용자 인터랙션 (드래그 앤 드롭, 실시간 편집)
- 오프라인 지원이 필요한 PWA
- 데스크톱 애플리케이션 같은 UX
- 많은 클라이언트 사이드 상태 관리
- 웹소켓 기반 실시간 업데이트

**하이브리드 접근:**
- Razor Pages로 기본 구조 + Alpine.js/HTMX로 인터랙션
- MVC로 대부분의 페이지 + React 컴포넌트를 특정 섹션에 임베드
- Razor Pages로 초기 렌더링 + Vue로 Progressive Enhancement
- Next.js 같은 메타 프레임워크처럼 SSR과 클라이언트 사이드를 혼합

## 현대적인 도구와의 통합

Razor Pages/MVC는 오래된 기술이 아닙니다. 현대적인 프론트엔드 도구와 완벽하게 통합됩니다:

**Tailwind CSS**: Razor 뷰에서 Tailwind 클래스를 사용할 수 있습니다.

```razor
<div class="flex items-center justify-between p-4 bg-gray-100">
  <h1 class="text-2xl font-bold">@Model.Title</h1>
</div>
```

**HTMX**: JavaScript 없이 AJAX 요청, 웹소켓, SSE를 사용합니다.

```razor
<button hx-post="/api/like" hx-swap="outerHTML">
  좋아요 (@Model.LikeCount)
</button>
```

**Alpine.js**: Razor에 가벼운 인터랙티비티를 추가합니다.

```razor
<div x-data="{ open: false }">
  <button @click="open = !open">토글</button>
  <div x-show="open">@Model.Content</div>
</div>
```

**Vite**: Razor Pages/MVC 프로젝트에서 Vite를 사용하여 빠른 빌드와 HMR을 얻을 수 있습니다.

## 학습 경로

Part 3는 순차적으로 학습하도록 설계되었습니다:

1. **Chapter 6부터 시작하세요**: Razor 문법은 모든 것의 기초입니다. JSX와 비교하며 빠르게 익힐 수 있습니다.

2. **Chapter 7로 실전 경험을**: Razor Pages로 실제 애플리케이션을 만들며 개념을 체화합니다. 대부분의 프로젝트는 Razor Pages만으로 충분합니다.

3. **Chapter 8은 필요할 때**: MVC는 더 복잡한 프로젝트를 위한 것입니다. Razor Pages로 한계를 느낄 때 학습하세요.

## 다음 단계

Part 3를 마치면, 여러분은 서버 사이드 렌더링의 강력함을 이해하게 됩니다. 하지만 여정은 여기서 끝나지 않습니다:

**Part 4: Blazor**에서는 C#으로 프론트엔드를 작성하는 혁명적인 방법을 배웁니다. React의 컴포넌트 모델과 C#의 타입 안정성을 결합한 Blazor는, 서버와 클라이언트의 경계를 허뭅니다.

**Part 5: Entity Framework Core**에서는 데이터베이스 접근을 배웁니다. Razor Pages/MVC는 UI 계층이고, EF Core는 데이터 계층입니다. 둘을 결합하면 완전한 풀스택 애플리케이션이 완성됩니다.

지금 바로 Chapter 6으로 이동하여, 첫 Razor 뷰를 작성해보세요!

---

## 참고 자료

- [공식 ASP.NET Core Razor Pages 문서](https://docs.microsoft.com/aspnet/core/razor-pages/)
- [공식 ASP.NET Core MVC 문서](https://docs.microsoft.com/aspnet/core/mvc/)
- [Razor 문법 참조](https://docs.microsoft.com/aspnet/core/mvc/views/razor)
- [Tag Helpers 가이드](https://docs.microsoft.com/aspnet/core/mvc/views/tag-helpers/)

**예상 학습 시간**: 2-3주 (각 챕터당 5-7일, 실습 포함)
