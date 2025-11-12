# 프론트엔드 개발자를 위한 ASP.NET Core 완벽 가이드
## Frontend Developer's Complete Guide to ASP.NET Core

---

## **Part 1: C# 기초 - 자바스크립트/타입스크립트 개발자 관점**

📖 **[Part 1 시작하기](./part1-csharp-basics/README.md)**

### **Chapter 1: C# 기초 문법 - TypeScript 개발자를 위한 빠른 시작**

#### 1.1 타입 시스템: TypeScript와 C#의 차이점
- 정적 타입 vs 동적 타입의 근본적 차이
- C#의 컴파일 타임 타입 체킹
- nullable 참조 타입과 TypeScript의 strict mode 비교
- 제네릭: `<T>` 사용법의 유사점과 차이점
- 인터페이스 기초

#### 1.2 익숙한 개념, 다른 문법
- 화살표 함수 vs 람다 표현식
- Promise/async-await vs Task/async-await
- 구조 분해 할당과 패턴 매칭
- 모듈 시스템: ES6 modules vs C# namespaces
- `var`, `let`, `const` 비교

#### 1.3 LINQ 기초 - JavaScript 배열 메서드와 비교
- 기본 LINQ 메서드 (Where, Select, OrderBy 등)
- JavaScript 메서드 → LINQ 매핑
- LINQ 기본 예제

#### 1.4 패턴 매칭 기초
- Switch 표현식
- is 패턴
- 타입 패턴

#### 1.5 실습: 기초 문법 변환 연습
- Async 패턴 비교 실습
- Array 메서드를 LINQ로 재작성하기

---

### **Chapter 2: 객체지향 프로그래밍과 고급 기능**

#### 2.1 값 타입 vs 참조 타입
- JavaScript의 메모리 모델과 비교
- struct와 class의 차이점과 성능 영향
- 언제 struct를 사용할까?

#### 2.2 프로퍼티(Properties): getter/setter의 진화된 형태
- 자동 구현 프로퍼티
- init 접근자
- Required 프로퍼티
- C# 14: field 키워드

#### 2.3 이벤트(Events)와 델리게이트(Delegates)
- JavaScript 이벤트 시스템과 비교
- 델리게이트 기초
- 이벤트 패턴
- 실전 예제

#### 2.4 객체지향 프로그래밍: 더 엄격한 세계
- 접근 제한자
- 상속과 다형성
- 인터페이스와 추상 클래스 심화
- 디자인 패턴 적용

#### 2.5 LINQ 고급 활용
- GroupBy와 집계
- Join 작업
- SelectMany (flatMap)
- 복잡한 쿼리 예제

#### 2.6 C# 13 & 14의 최신 기능 (2025 기준)
- C# 13: 기본 람다 매개변수, 향상된 패턴 매칭
- C# 14: Extension Members, First-Class Span Support, field 키워드
- C# 14: Null-Conditional Assignment, Partial Constructors
- C# 14: Compound Operator Overloading

#### 2.7 실습: 고급 패턴 연습
- OOP 패턴 구현
- LINQ 고급 쿼리
- 이벤트와 델리게이트

---

## **Part 2: ASP.NET Core 기초 - 새로운 패러다임의 이해**

📖 **[Part 2 시작하기](./part2-aspnetcore-basics/README.md)**

### **Chapter 3: ASP.NET Core 소개와 개발 환경 설정**

#### 2.1 ASP.NET Core란 무엇인가?
- .NET Framework에서 .NET Core로의 진화
- 크로스 플랫폼 웹 프레임워크의 의미
- .NET 8 (LTS)과 .NET 9의 차이점
- Express.js, NestJS와 비교: 왜 ASP.NET Core인가?

#### 2.2 개발 환경 선택과 설정
- Visual Studio 2022 vs Rider vs VS Code: 각각의 장단점
- VS Code + C# Dev Kit: 프론트엔드 개발자에게 익숙한 선택
- .NET CLI 마스터하기: npm처럼 사용하기
- 프로젝트 구조 이해: `.csproj` vs `package.json`

#### 2.3 첫 번째 ASP.NET Core 애플리케이션
- `dotnet new` 템플릿 탐색
- Program.cs: 애플리케이션의 진입점
- 개발 서버 실행: `dotnet run` vs `dotnet watch`
- Hot Reload: Vite HMR과 비교

#### 2.4 프로젝트 구조 해부
- Solution과 Project의 관계
- 의존성 관리: NuGet vs npm
- 빌드 프로세스 이해하기
- 디버깅 환경 구성

#### 2.5 실습: "Hello World"에서 실제 API까지
- 간단한 REST API 엔드포인트 생성
- Postman/Thunder Client로 테스트
- 에러 처리 기초

---

### **Chapter 4: ASP.NET Core의 핵심 아키텍처**

#### 3.1 요청-응답 파이프라인: Express 미들웨어와의 비교
- 미들웨어 파이프라인의 실행 순서
- `Use`, `Run`, `Map` 메서드의 차이
- 미들웨어 체인과 next() 개념
- 실행 순서의 중요성: 인증 → 라우팅 → 엔드포인트

#### 3.2 의존성 주입(DI): Angular에서 본 것과 비슷하지만 더 강력한
- IoC 컨테이너의 개념
- 서비스 생명주기: Singleton, Scoped, Transient
- 생성자 주입 패턴
- Keyed Services (.NET 9 신기능)
- React Context API vs ASP.NET Core DI

#### 3.3 라우팅 시스템
- 컨벤션 기반 라우팅
- 특성 기반 라우팅(Attribute Routing)
- React Router와의 개념적 유사성
- 라우트 제약 조건과 매개변수

#### 3.4 구성 관리(Configuration)
- appsettings.json: package.json과는 다른 역할
- 환경별 구성: Development, Staging, Production
- 환경 변수와 사용자 시크릿
- Options 패턴: 강타입 구성

#### 3.5 로깅과 모니터링
- 구조화된 로깅(Structured Logging)
- 로그 레벨과 필터링
- Application Insights 통합
- Serilog와 서드파티 로깅 프레임워크

#### 3.6 실습: 미들웨어 파이프라인 구축
- 커스텀 미들웨어 작성
- 로깅 미들웨어 구현
- 에러 처리 미들웨어
- 성능 측정 미들웨어

---

### **Chapter 5: Minimal APIs - Node.js Express 개발자를 위한 빠른 시작**

#### 4.1 Minimal APIs 소개
- 컨트롤러 없는 API: Express.js 스타일
- 언제 Minimal APIs를 사용할까?
- 마이크로서비스에 최적화된 접근

#### 4.2 엔드포인트 정의와 라우팅
```csharp
app.MapGet("/api/users", () => { ... });
app.MapPost("/api/users", (User user) => { ... });
```
- HTTP 메서드 매핑
- 라우트 매개변수와 쿼리 문자열
- 요청 본문 바인딩

#### 4.3 의존성 주입과 서비스 사용
- 람다 매개변수를 통한 DI
- `[FromServices]` 특성
- 데이터베이스 컨텍스트 주입

#### 4.4 응답 형식과 상태 코드
- `Results` 헬퍼: `Ok()`, `NotFound()`, `BadRequest()`
- TypedResults를 활용한 타입 안전성
- 커스텀 응답 생성

#### 4.5 OpenAPI/Swagger 통합
- 자동 API 문서 생성
- `.NET 9의 내장 OpenAPI 지원`
- API 테스팅과 문서화

#### 4.6 실습: RESTful API 완성
- CRUD 엔드포인트 구현
- 유효성 검사 추가
- 에러 응답 표준화

---

## **Part 3: 서버 사이드 렌더링 - Razor Pages와 MVC**

📖 **[Part 3 시작하기](./part3-server-side-rendering/README.md)**

### **Chapter 6: Razor 문법 - JSX를 아는 개발자를 위한 가이드**

#### 5.1 Razor 문법 기초
- `@` 기호: JSX의 `{}`와 유사한 역할
- C# 코드 블록: `@{ ... }`
- 표현식과 문장의 차이
- HTML 인코딩: XSS 방지

#### 5.2 JSX vs Razor: 비교 분석
- 조건부 렌더링: `&&`, `? :` vs `@if`
- 리스트 렌더링: `.map()` vs `@foreach`
- 컴포넌트 합성: React 컴포넌트 vs Razor 컴포넌트
- Props vs Model

#### 5.3 Razor 지시문(Directives)
- `@page`: 페이지 정의
- `@model`: 뷰 모델 바인딩
- `@inject`: 의존성 주입
- `@using`: 네임스페이스 가져오기
- `@section`: 섹션 정의

#### 5.4 레이아웃과 부분 뷰
- `_Layout.cshtml`: 마스터 페이지
- React의 Layout 컴포넌트와 비교
- Partial Views: 재사용 가능한 UI 조각
- View Components: 더 강력한 컴포넌트

#### 5.5 Tag Helpers: HTML을 위한 특별한 도구
- `asp-for`, `asp-action`, `asp-controller`
- 폼 생성과 유효성 검사
- 환경별 조건부 렌더링

#### 5.6 실습: Razor 문법 마스터하기
- JSX 코드를 Razor로 변환
- 동적 리스트 렌더링
- 폼 처리 예제

---

### **Chapter 7: Razor Pages - 단순함의 힘**

#### 6.1 Razor Pages 모델
- 페이지 기반 라우팅: Next.js Pages Router와 유사
- PageModel 클래스: 백엔드 로직의 집합소
- MVVM 패턴과의 관계

#### 6.2 라우팅과 페이지 구조
- 폴더 기반 라우팅
- 동적 라우트 매개변수
- 라우트 제약 조건

#### 6.3 폼 처리와 데이터 바인딩
- `[BindProperty]` 특성
- OnGet, OnPost 핸들러 메서드
- 모델 유효성 검사
- Anti-forgery 토큰: CSRF 방지

#### 6.4 상태 관리
- TempData, ViewData, ViewBag
- React의 useState와 차이점
- 서버 사이드 상태의 특성

#### 6.5 실습: 블로그 CRUD 애플리케이션
- 게시글 목록 페이지
- 게시글 작성/수정 페이지
- 삭제 확인 페이지
- 검색 기능 구현

---

### **Chapter 8: MVC 패턴 - 대규모 애플리케이션을 위한 구조**

#### 7.1 MVC 아키텍처 이해
- Model, View, Controller의 역할
- Flux/Redux와 MVC의 차이점
- 언제 MVC를, 언제 Razor Pages를 사용할까?

#### 7.2 컨트롤러(Controllers)
- 컨트롤러 클래스 구조
- 액션 메서드와 라우팅
- `IActionResult` 반환 타입
- 컨트롤러의 책임과 범위

#### 7.3 뷰(Views)
- 강타입 뷰: `@model` 지시문
- 뷰 모델(ViewModel) 패턴
- 레이아웃과 섹션
- 부분 뷰의 활용

#### 7.4 모델(Models)
- 도메인 모델 vs 뷰 모델
- 데이터 어노테이션을 통한 유효성 검사
- 모델 바인딩의 작동 원리

#### 7.5 필터(Filters)
- Authorization 필터
- Action 필터: 로깅, 캐싱
- Exception 필터: 에러 처리
- React HOC/Decorators와 비교

#### 7.6 Areas: 애플리케이션 모듈화
- Admin, API, Public 영역 분리
- 영역 기반 라우팅
- 대규모 프로젝트 구조화

#### 7.7 실습: 전자상거래 MVC 애플리케이션
- 상품 카탈로그 (Controller + View)
- 장바구니 기능
- 주문 관리 페이지
- 관리자 영역 구현

---

## **Part 4: Blazor - 프론트엔드 개발자의 친숙한 영역**

📖 **[Part 4 시작하기](./part4-blazor/README.md)**

### **Chapter 9: Blazor 소개 - C#으로 작성하는 프론트엔드**

#### 8.1 Blazor란 무엇인가?
- React/Vue/Angular의 C# 대안
- WebAssembly의 이해
- Blazor의 세 가지 호스팅 모델: Server, WebAssembly, Hybrid

#### 8.2 호스팅 모델 비교와 선택
- **Blazor Server**: SignalR 기반, 실시간 연결
  - 장점: 빠른 초기 로드, 전체 .NET API 접근
  - 단점: 서버 연결 필수, 지연 시간
- **Blazor WebAssembly**: 브라우저에서 .NET 실행
  - 장점: 오프라인 가능, PWA 지원
  - 단점: 초기 다운로드 크기, 제한된 API
- **Blazor Hybrid**: .NET MAUI와 통합
  - 웹/모바일/데스크톱 코드 공유

#### 8.3 React/Vue와 Blazor 비교
| 개념 | React | Vue | Blazor |
|------|-------|-----|--------|
| 컴포넌트 | .jsx | .vue | .razor |
| 상태 관리 | useState | ref/reactive | @code { } |
| Props | props | props | [Parameter] |
| 이벤트 | onClick | @click | @onclick |
| 조건부 렌더링 | && / ? : | v-if | @if |
| 리스트 렌더링 | .map() | v-for | @foreach |
| 생명주기 | useEffect | onMounted | OnInitialized |

#### 8.4 개발 환경 설정
- Blazor 프로젝트 템플릿
- Hot Reload와 개발 경험
- 브라우저 개발자 도구 활용

---

### **Chapter 10: Blazor 컴포넌트 개발**

#### 9.1 컴포넌트 기초
- .razor 파일 구조
- `@code` 블록: 컴포넌트 로직
- 생성자 주입 (Constructor Injection) - .NET 9 신기능
- 컴포넌트 재사용성

#### 9.2 Props와 Parameters
```csharp
[Parameter]
public string Title { get; set; }

[Parameter]
public EventCallback<string> OnClick { get; set; }
```
- 매개변수 정의와 전달
- CascadingParameter: React Context와 유사
- EventCallback: 자식→부모 통신

#### 9.3 상태 관리
- 컴포넌트 상태: 로컬 변수
- `StateHasChanged()`: React의 forceUpdate와 유사
- 양방향 바인딩: `@bind`
- Redux 대신: Fluxor, Blazor State Management

#### 9.4 생명주기 메서드
- `OnInitialized` / `OnInitializedAsync`
- `OnParametersSet` / `OnParametersSetAsync`
- `OnAfterRender` / `OnAfterRenderAsync`
- `Dispose`: 리소스 정리 (useEffect cleanup과 유사)

#### 9.5 폼과 유효성 검사
- `EditForm` 컴포넌트
- 데이터 어노테이션 유효성 검사
- 커스텀 유효성 검사 로직
- `ValidationSummary`, `ValidationMessage`

#### 9.6 JavaScript Interop
- C#에서 JavaScript 호출: `IJSRuntime`
- JavaScript에서 C# 호출
- 기존 JavaScript 라이브러리 통합
- 언제 JS Interop을 사용할까?

#### 9.7 실습: Todo 애플리케이션 (Blazor로)
- 컴포넌트 구조 설계
- CRUD 기능 구현
- 로컬 스토리지 연동
- 상태 관리 패턴 적용

---

### **Chapter 11: Blazor 고급 패턴**

#### 10.1 Blazor 라우팅
- `@page` 지시문
- 동적 라우트 매개변수
- 쿼리 문자열
- NavLink 컴포넌트: React Router Link와 유사
- 프로그래밍 방식 네비게이션

#### 10.2 Blazor 레이아웃
- `MainLayout.razor`
- 중첩 레이아웃
- 동적 레이아웃 전환

#### 10.3 컴포넌트 라이브러리 생성
- Razor Class Library (RCL)
- NuGet 패키지로 배포
- 재사용 가능한 UI 컴포넌트 세트

#### 10.4 성능 최적화
- 가상화(Virtualization): 큰 리스트 렌더링
- `@key` 지시문: React의 key prop과 동일
- Lazy loading: 코드 분할
- Prerendering: SSR과 유사

#### 10.5 인증과 권한 부여
- `AuthorizeView` 컴포넌트
- 사용자 정보 접근
- 역할 기반 UI 렌더링
- 커스텀 인증 상태 제공자

#### 10.6 실습: 대시보드 애플리케이션
- 차트 라이브러리 통합
- 실시간 데이터 업데이트 (SignalR)
- 인증이 필요한 페이지
- 반응형 레이아웃

---

## **Part 5: 데이터 액세스 - Entity Framework Core**

📖 **[Part 5 시작하기](./part5-data-access/README.md)**

### **Chapter 12: Entity Framework Core 기초**

#### 11.1 ORM 소개
- SQL vs ORM
- Prisma, TypeORM, Sequelize와 비교
- EF Core의 위치

#### 11.2 DbContext와 Entity 클래스
- DbContext: 데이터베이스 세션
- Entity 클래스: 테이블 매핑
- DbSet 속성
- 관계 설정: 일대다, 다대다

#### 11.3 Code-First vs Database-First
- 마이그레이션 기반 개발
- `dotnet ef` CLI 도구
- 마이그레이션 생성과 적용
- 초기 데이터(Seed Data) 설정

#### 11.4 쿼리 작성
- LINQ to Entities
- 지연 로딩, 즉시 로딩, 명시적 로딩
- `.Include()`, `.ThenInclude()`
- 원시 SQL 쿼리

#### 11.5 변경 추적과 저장
- `Add()`, `Update()`, `Remove()`
- `SaveChanges()` / `SaveChangesAsync()`
- 트랜잭션 관리
- 동시성 제어

#### 11.6 실습: 블로그 데이터베이스 설계
- Entity 클래스 정의
- 관계 설정 (Post, Comment, User)
- 마이그레이션 생성
- CRUD 작업 구현

---

### **Chapter 13: Entity Framework Core 고급**

#### 12.1 고급 쿼리 기법
- 프로젝션과 익명 타입
- 그룹화와 집계
- 페이징: Skip, Take
- 필터링과 정렬

#### 12.2 성능 최적화
- N+1 쿼리 문제 해결
- AsNoTracking: 읽기 전용 쿼리
- Compiled Queries
- 인덱스 설정

#### 12.3 Repository 패턴
- Generic Repository 구현
- Unit of Work 패턴
- 언제 사용하고 언제 사용하지 말아야 할까?

#### 12.4 다중 데이터베이스 지원
- SQL Server, PostgreSQL, MySQL, SQLite
- 데이터베이스 제공자 구성
- 데이터베이스별 차이점

#### 12.5 EF Core와 NoSQL
- Cosmos DB 제공자
- MongoDB 통합 고려사항

#### 12.6 실습: 복잡한 쿼리 시나리오
- 다중 조인 쿼리
- 서브쿼리와 CTE
- 저장 프로시저 호출
- 벌크 작업 최적화

---

## **Part 6: API 개발 - RESTful에서 GraphQL까지**

📖 **[Part 6 시작하기](./part6/README.md)**

### **Chapter 14: RESTful API 설계와 구현**

#### 13.1 REST API 원칙
- 리소스 중심 설계
- HTTP 메서드의 적절한 사용
- 상태 코드의 의미
- Express.js API와의 패턴 비교

#### 13.2 컨트롤러 기반 API
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
```
- ApiController 특성
- 라우트 템플릿
- 액션 메서드 정의
- 모델 바인딩 소스: `[FromBody]`, `[FromQuery]`, `[FromRoute]`

#### 13.3 응답 형식화
- Content Negotiation
- JSON 직렬화: System.Text.Json
- XML 지원
- 커스텀 포맷터

#### 13.4 API 버전 관리
- URL 기반 버전 관리: `/api/v1/products`
- 헤더 기반 버전 관리
- Microsoft.AspNetCore.Mvc.Versioning

#### 13.5 API 문서화
- OpenAPI/Swagger 통합 (.NET 9 내장 지원)
- XML 주석 활용
- 예제 응답 정의
- Swagger UI 커스터마이징

#### 13.6 CORS 구성
- SPA와의 통신을 위한 CORS
- 정책 정의와 적용
- Preflight 요청 처리

#### 13.7 실습: 전자상거래 REST API
- 제품 목록 API
- 검색과 필터링
- 페이징 구현
- API 문서 자동 생성

---

### **Chapter 15: API 보안과 인증**

#### 14.1 JWT 인증 구현
- JWT 토큰 구조 이해
- 토큰 생성과 검증
- Refresh Token 패턴
- Bearer Token 인증 설정

#### 14.2 ASP.NET Core Identity
- Identity 프레임워크 소개
- 사용자 등록과 로그인
- 비밀번호 해싱과 보안
- 이메일 확인과 2FA

#### 14.3 OAuth 2.0과 OpenID Connect
- 외부 인증 제공자 통합 (Google, Facebook, Microsoft)
- Authorization Code Flow
- PAR (Pushed Authorization Requests) - .NET 9 지원
- 토큰 관리

#### 14.4 권한 부여 패턴
- 역할 기반 권한 부여(RBAC)
- 클레임 기반 권한 부여
- 정책 기반 권한 부여
- 리소스 기반 권한 부여

#### 14.5 API 보안 모범 사례
- HTTPS 강제
- Rate Limiting (.NET 9 개선사항)
- API 키 관리
- 입력 유효성 검사
- SQL Injection, XSS 방지

#### 14.6 실습: 보안이 적용된 API
- JWT 기반 인증 구현
- 역할별 엔드포인트 접근 제어
- Refresh Token 흐름
- API Key 인증 추가

---

### **Chapter 16: GraphQL과 SignalR**

#### 15.1 GraphQL in ASP.NET Core
- Hot Chocolate 라이브러리
- Schema 정의
- Query와 Mutation
- DataLoader 패턴: N+1 문제 해결
- REST vs GraphQL: 언제 무엇을 사용할까?

#### 15.2 SignalR - 실시간 통신
- SignalR Hub 개념
- WebSocket 기반 통신
- 클라이언트 라이브러리 (JavaScript, React, Angular)
- 그룹과 브로드캐스트

#### 15.3 SignalR 패턴
- 채팅 애플리케이션
- 실시간 알림
- 실시간 대시보드
- 진행률 추적

#### 15.4 실습: 실시간 협업 도구
- SignalR Hub 구현
- React 클라이언트 통합
- 사용자 상태 추적
- 메시지 브로드캐스팅

---

## **Part 7: 프로덕션 준비 - 실시간 통신과 클라이언트 통합**

📖 **[Part 7 시작하기](./part7/README.md)**

### **Chapter 17: 실시간 통신과 백그라운드 처리**

#### 16.1 실시간 통신 패턴
- Polling, Long Polling, Server-Sent Events, WebSocket 비교
- 각 패턴의 장단점과 선택 기준
- **Socket.io vs SignalR: Node.js 개발자를 위한 상세 비교**
  - 코드 비교: 거의 1대1 대응
  - 타입 안전성, 성능, 확장성 차이점
  - 마이그레이션 가이드

#### 16.2 Server-Sent Events (SSE)
- SSE의 개념과 사용 사례
- ASP.NET Core에서 SSE 구현
- 실시간 알림, 로그 스트리밍
- 브라우저 호환성과 폴백

#### 16.3 WebSocket 직접 사용
- `System.Net.WebSockets` 네임스페이스
- 연결 관리와 메시지 송수신
- 재연결 로직과 하트비트
- 에러 처리

#### 16.4 SignalR 고급 패턴
- Strongly-typed Hub
- 그룹 관리와 동적 그룹
- Redis backplane으로 확장
- Azure SignalR Service

#### 16.5 백그라운드 작업 처리
- `IHostedService`와 `BackgroundService`
- 작업 큐 패턴: `IBackgroundTaskQueue`
- 정상 종료(graceful shutdown)
- Node.js Worker Threads와 비교

#### 16.6 Hangfire: 고급 백그라운드 작업
- Fire-and-forget, Delayed, Recurring, Continuation
- 자동 재시도와 지수 백오프
- 대시보드 UI로 작업 모니터링
- Cron 표현식과 스케줄링

#### 16.7 메시지 큐와 이벤트 기반 아키텍처
- RabbitMQ: AMQP 프로토콜
- Azure Service Bus: 클라우드 메시징
- 메시지 발행/구독 패턴
- 느슨한 결합과 마이크로서비스

#### 16.8 실습: 실시간 대시보드와 백그라운드 작업
- SignalR로 실시간 메트릭 업데이트
- Hangfire로 이미지 처리 작업
- 메시지 큐로 이벤트 기반 주문 시스템

---

### **Chapter 18: API 클라이언트 패턴**

#### 17.1 타입 안전한 API 클라이언트
- NSwag: OpenAPI에서 TypeScript 클라이언트 생성
- Kiota: Microsoft의 API 클라이언트 생성기
- 자동 생성 vs 수동 작성

#### 17.2 프론트엔드 상태 관리와 API 통합
- React Query와 ASP.NET Core API
- Redux Toolkit (RTK Query)
- Vue의 Pinia와 API 호출
- NgRx Effects (Angular)

#### 17.3 Optimistic UI 업데이트
- 낙관적 업데이트 패턴
- 실패 시 롤백
- 서버 조정

#### 17.4 캐싱 전략
- 브라우저 캐싱
- 서버 사이드 캐싱: IMemoryCache, IDistributedCache
- HybridCache (.NET 9 신기능): stampede 방지
- HTTP 캐시 헤더

#### 17.5 실습: API 클라이언트 라이브러리
- NSwag로 TypeScript 클라이언트 생성
- React Query 통합
- 에러 처리와 재시도 로직
- 캐싱 정책 구성

---

## **Part 8: 상태 관리와 패턴**

📖 **[Part 8 시작하기](./part8/README.md)**

### **Chapter 19: 서버 사이드 상태 관리**

#### 18.1 상태의 종류
- 요청 범위 상태 (Scoped)
- 애플리케이션 상태 (Singleton)
- 사용자 세션 상태
- 분산 캐시

#### 18.2 세션 관리
- In-Memory 세션
- 분산 세션: Redis 통합
- 쿠키 기반 세션
- 세션 vs JWT: 선택 기준

#### 18.3 캐싱 전략
- 메모리 캐시: `IMemoryCache`
- 분산 캐시: `IDistributedCache`
- HybridCache (.NET 9): 통합 API
- 캐시 무효화 패턴
- Cache-Aside, Write-Through

#### 18.4 TempData와 ViewData
- TempData: 리디렉션 간 데이터 전달
- ViewData와 ViewBag
- 사용 시나리오와 제한사항

#### 18.5 실습: 장바구니 세션 관리
- 세션 기반 장바구니
- Redis를 통한 분산 세션
- 영속적 장바구니 (DB 저장)

---

### **Chapter 20: 고급 아키텍처 패턴**

#### 19.1 Clean Architecture in ASP.NET Core
- 계층 분리: Domain, Application, Infrastructure, Presentation
- 의존성 규칙
- SOLID 원칙 적용
- 프로젝트 구조 예제

#### 19.2 CQRS 패턴
- Command와 Query 분리
- MediatR 라이브러리
- 읽기/쓰기 모델 분리
- Event Sourcing과의 결합

#### 19.3 Repository와 Unit of Work
- Generic Repository 구현
- 언제 사용하고 언제 피해야 할까?
- EF Core의 DbContext는 이미 Unit of Work

#### 19.4 DDD (Domain-Driven Design) 기초
- Aggregate, Entity, Value Object
- Domain Events
- Bounded Context

#### 19.5 Microservices 아키텍처
- 모놀리스에서 마이크로서비스로
- 서비스 간 통신: REST, gRPC
- API Gateway: YARP (Yet Another Reverse Proxy)
- 서비스 디스커버리

#### 19.6 실습: Modular Monolith
- 모듈별 프로젝트 분리
- 모듈 간 통신: .NET Channels
- 점진적 마이크로서비스 전환

---

## **Part 9: 테스팅 전략**

📖 **[Part 9 시작하기](./part9/README.md)**

### **Chapter 21: 단위 테스트와 통합 테스트**

#### 20.1 테스트 프레임워크
- xUnit: .NET 표준
- NUnit과 MSTest
- Jest/Vitest와 비교

#### 20.2 단위 테스트 작성
```csharp
[Fact]
public void GetProduct_ReturnsProduct()
{
    // Arrange, Act, Assert
}
```
- AAA 패턴
- Moq을 통한 목 객체
- 의존성 주입 테스트

#### 20.3 통합 테스트
- WebApplicationFactory
- 테스트 데이터베이스 설정
- HTTP 클라이언트 테스트
- 인증이 필요한 엔드포인트 테스트

#### 20.4 Blazor 컴포넌트 테스트
- bUnit 라이브러리
- 컴포넌트 렌더링 테스트
- 이벤트 트리거
- React Testing Library와 비교

#### 20.5 E2E 테스트
- Playwright for .NET
- Selenium WebDriver
- UI 자동화

#### 20.6 테스트 커버리지
- Coverlet을 통한 코드 커버리지
- Visual Studio Code Coverage
- 커버리지 목표 설정

#### 20.7 실습: 테스트 주도 개발(TDD)
- API 엔드포인트 TDD
- 비즈니스 로직 단위 테스트
- 통합 테스트 시나리오

---

## **Part 10: 성능 최적화와 모니터링**

📖 **[Part 10 시작하기](./part10/README.md)**

### **Chapter 22: 성능 최적화 기법**

#### 21.1 프로파일링과 벤치마킹
- dotnet-trace, dotnet-counters
- BenchmarkDotNet
- Visual Studio Profiler
- 성능 병목 지점 식별

#### 21.2 응답 압축
- Gzip, Brotli 압축
- 정적 자산 압축 (.NET 9 빌드 타임 압축)
- `MapStaticAssets`: 자동 fingerprinting

#### 21.3 응답 캐싱
- Response Caching 미들웨어
- HTTP 캐시 헤더
- 조건부 요청: ETag, Last-Modified

#### 21.4 데이터베이스 최적화
- N+1 쿼리 해결
- 인덱싱 전략
- Connection Pooling
- Compiled Queries

#### 21.5 비동기 프로그래밍
- async/await 모범 사례
- ValueTask vs Task
- 비동기 스트림: IAsyncEnumerable

#### 21.6 Native AOT (Ahead-of-Time 컴파일)
- 빠른 시작 시간
- 작은 메모리 풋프린트
- 제약 사항과 호환성
- 언제 Native AOT를 사용할까?

#### 21.7 실습: 성능 최적화 프로젝트
- 느린 엔드포인트 식별
- 캐싱 적용
- 쿼리 최적화
- 벤치마크로 개선 측정

---

### **Chapter 23: 모니터링과 로깅**

#### 22.1 Application Insights
- Azure Application Insights 통합
- 요청 추적
- 의존성 추적
- 사용자 지정 이벤트

#### 22.2 구조화된 로깅
- Serilog 통합
- 로그 enrichment
- 컨텍스트 정보 추가

#### 22.3 분산 추적
- OpenTelemetry
- ActivitySource (.NET 9 지원 확대)
- 마이크로서비스 간 추적
- Jaeger, Zipkin 통합

#### 22.4 메트릭과 대시보드
- Prometheus와 Grafana
- 커스텀 메트릭 생성
- Kestrel 연결 메트릭 (.NET 9 개선)
- 헬스 체크 엔드포인트

#### 22.5 알림과 경고
- 오류율 알림
- 성능 저하 감지
- 애플리케이션 상태 모니터링

#### 22.6 실습: 종합 모니터링 구성
- Application Insights 설정
- 커스텀 이벤트 로깅
- 대시보드 구성
- 알림 규칙 설정

---

## **Part 11: 배포와 DevOps**

📖 **[Part 11 시작하기](./part11/README.md)**

### **Chapter 24: 컨테이너화와 Docker**

#### 23.1 Docker 기초
- Dockerfile 작성
- Multi-stage 빌드
- .NET 공식 이미지 선택
  - `mcr.microsoft.com/dotnet/aspnet:9.0`
  - Alpine, Chiseled, AOT 변형

#### 23.2 컨테이너 최적화
- 이미지 크기 최소화
- 레이어 캐싱
- .dockerignore 활용
- 보안 모범 사례: Non-root 사용자

#### 23.3 Docker Compose
- 다중 컨테이너 애플리케이션
- 데이터베이스 컨테이너 통합
- 개발 환경 구성
- 볼륨과 네트워크

#### 23.4 컨테이너 레지스트리
- Docker Hub
- Azure Container Registry
- GitHub Container Registry
- 이미지 태깅 전략

#### 23.5 실습: ASP.NET Core 앱 Docker화
- Dockerfile 작성
- 이미지 빌드와 실행
- Docker Compose로 전체 스택 실행
- 레지스트리에 푸시

---

### **Chapter 25: 클라우드 배포 - Azure 중심**

#### 24.1 Azure App Service
- 웹앱 배포
- 배포 슬롯: Blue-Green 배포
- Zero-downtime 배포 (.NET 9 지원)
- 환경 변수와 설정

#### 24.2 Azure Container Apps
- 서버리스 컨테이너
- 자동 확장
- KEDA 기반 스케일링
- 마이크로서비스 호스팅

#### 24.3 Azure Kubernetes Service (AKS)
- Kubernetes 기초
- Deployment와 Service
- Ingress Controller
- Helm 차트 배포

#### 24.4 Azure Functions
- Serverless ASP.NET Core
- HTTP 트리거
- Native AOT로 콜드 스타트 개선

#### 24.5 Azure DevOps와 GitHub Actions
- CI/CD 파이프라인
- 자동 빌드와 테스트
- 환경별 배포
- 비밀 관리

#### 24.6 인프라스트럭처 as 코드
- Bicep 템플릿
- Terraform
- 리소스 프로비저닝 자동화

#### 24.7 실습: 전체 CI/CD 파이프라인
- GitHub Actions 워크플로우
- 자동 테스트 실행
- Docker 이미지 빌드
- Azure App Service 배포

---

### **Chapter 26: 프로덕션 고려사항**

#### 25.1 환경 구성 관리
- appsettings.Production.json
- Azure Key Vault 통합
- 관리 ID(Managed Identity)
- 비밀 회전(Secret Rotation)

#### 25.2 HTTPS와 SSL/TLS
- 인증서 관리
- Let's Encrypt 통합
- HSTS (HTTP Strict Transport Security)
- SSL 오프로딩

#### 25.3 Rate Limiting과 Throttling
- .NET 9 Rate Limiter 미들웨어
- 정책 정의: Fixed Window, Sliding Window, Token Bucket
- IP 기반 제한
- 사용자 기반 제한

#### 25.4 오류 처리와 복원력
- Global Exception Handler
- Circuit Breaker 패턴: Polly
- Retry 정책
- Fallback 전략

#### 25.5 백업과 재해 복구
- 데이터베이스 백업 전략
- 지역 중복성
- 재해 복구 계획

#### 25.6 보안 체크리스트
- OWASP Top 10 대응
- 보안 헤더 설정
- 의존성 취약점 스캐닝
- 정기 보안 감사

#### 25.7 실습: 프로덕션 준비 체크리스트
- 환경 변수 검증
- 로깅 레벨 조정
- 성능 테스트
- 보안 헤더 적용

---

## **Part 12: 실전 프로젝트와 모범 사례**

📖 **[Part 12 시작하기](./part12/README.md)**

### **Chapter 27: 전자상거래 플랫폼 구축 (종합 프로젝트)**

#### 26.1 프로젝트 개요와 요구사항
- 기능 명세
- 아키텍처 결정
- 기술 스택 선택

#### 26.2 백엔드 API 개발
- RESTful API 설계
- Entity Framework Core 모델링
- 리포지토리 패턴 적용
- 비즈니스 로직 계층

#### 26.3 인증과 권한
- JWT 인증 구현
- 역할 기반 권한 (고객, 판매자, 관리자)
- 외부 OAuth 로그인

#### 26.4 프론트엔드 통합
- React 또는 Blazor 선택
- 쇼핑 카트 구현
- 결제 흐름
- 주문 관리

#### 26.5 결제 통합
- Stripe API 통합
- 웹훅 처리
- 환불과 취소

#### 26.6 검색 기능
- Azure Cognitive Search
- Elasticsearch 통합
- 전문 검색과 필터링

#### 26.7 배포와 모니터링
- Azure 인프라 설정
- CI/CD 파이프라인
- Application Insights 통합

---

### **Chapter 28: 모범 사례 종합**

#### 27.1 코드 품질
- 코딩 표준과 StyleCop
- .editorconfig 설정
- 코드 리뷰 체크리스트
- 정적 분석: Roslyn Analyzers

#### 27.2 보안 모범 사례
- 입력 유효성 검사
- 출력 인코딩
- 파라미터화된 쿼리
- 비밀 정보 관리

#### 27.3 성능 모범 사례
- 비동기 프로그래밍
- 캐싱 전략
- 데이터베이스 쿼리 최적화
- 프로파일링과 측정

#### 27.4 유지보수성
- SOLID 원칙 적용
- 의존성 주입 활용
- 테스트 가능한 코드
- 명확한 네이밍과 주석

#### 27.5 확장성 고려사항
- 수평 확장 vs 수직 확장
- 상태 비저장 설계
- 데이터베이스 샤딩
- 읽기 복제본

#### 27.6 프론트엔드 개발자를 위한 팁
- C# 관용구 익히기
- .NET 생태계 탐색
- 커뮤니티 리소스 활용
- 지속적인 학습

---

## **부록**

### **Appendix A: C# 치트 시트 - JavaScript/TypeScript 개발자용**

#### A.1 문법 비교표
- 변수 선언
- 함수/메서드 정의
- 비동기 패턴
- 컬렉션 조작
- 오류 처리

#### A.2 타입 변환 가이드
- JavaScript 타입 → C# 타입
- 공통 인터페이스 패턴
- 유틸리티 타입 비교

#### A.3 LINQ 치트 시트
- Array 메서드 → LINQ 메서드
- 예제 코드 모음

---

### **Appendix B: ASP.NET Core 프로젝트 템플릿 가이드**

#### B.1 공식 템플릿 종류
- Web API
- Web App (MVC)
- Web App (Razor Pages)
- Blazor Server
- Blazor WebAssembly
- React, Angular, Vue 통합 템플릿

#### B.2 커스텀 템플릿 생성
- 팀 표준 템플릿
- NuGet 패키지로 배포

---

### **Appendix C: 유용한 NuGet 패키지 모음**

#### C.1 데이터 액세스
- Entity Framework Core
- Dapper
- MongoDB.Driver

#### C.2 API 관련
- Swashbuckle (Swagger)
- NSwag
- FluentValidation

#### C.3 테스팅
- xUnit
- Moq
- FluentAssertions
- bUnit (Blazor)

#### C.4 유틸리티
- AutoMapper
- MediatR
- Polly
- Serilog

---

### **Appendix D: 도구와 확장 프로그램**

#### D.1 Visual Studio Code 확장
- C# Dev Kit
- REST Client
- Docker
- GitLens

#### D.2 CLI 도구
- dotnet CLI 명령어 전체 목록
- Entity Framework Core CLI
- 사용자 시크릿 관리

#### D.3 브라우저 도구
- Browser Developer Tools
- Blazor 디버깅

---

### **Appendix E: 학습 리소스와 커뮤니티**

#### E.1 공식 문서
- Microsoft Learn
- ASP.NET Core 문서
- .NET API 브라우저

#### E.2 온라인 강좌
- Microsoft Learn 경로
- Pluralsight
- Udemy 추천 강좌

#### E.3 책과 블로그
- 추천 서적 목록
- 인기 블로그와 뉴스레터
- YouTube 채널

#### E.4 커뮤니티
- Stack Overflow
- Reddit (r/dotnet, r/csharp, r/Blazor)
- Discord 서버
- 한국 .NET 개발자 커뮤니티

---

### **Appendix F: 마이그레이션 가이드**

#### F.1 Node.js/Express에서 ASP.NET Core로
- 아키텍처 비교
- 공통 패턴 매핑
- 단계별 마이그레이션 전략

#### F.2 레거시 ASP.NET에서 ASP.NET Core로
- Web Forms에서 Blazor/Razor Pages로
- ASP.NET MVC에서 ASP.NET Core MVC로
- 단계적 마이그레이션

---

### **Appendix G: 문제 해결 가이드**

#### G.1 흔한 오류와 해결책
- 의존성 주입 오류
- 라우팅 문제
- CORS 오류
- Entity Framework 마이그레이션 문제

#### G.2 디버깅 팁
- 중단점 활용
- 조건부 중단점
- 로그 분석
- 프로파일러 사용

---

### **용어 사전**
- ASP.NET Core 주요 용어 한영 대조표
- 약어 풀이
- 개념 색인

---

### **참고문헌**
- 공식 문서 링크
- 주요 블로그 포스트
- 관련 서적
- 컨퍼런스 발표 자료

---

## **맺음말: 프론트엔드 개발자에서 풀스택 .NET 개발자로**

### 학습 로드맵 요약
- 1-2개월: C# 기초와 ASP.NET Core 핵심 (Part 1-2)
- 2-3개월: 서버 사이드 렌더링과 Blazor (Part 3-4)
- 1-2개월: 데이터베이스와 API 개발 (Part 5-6)
- 1-2개월: 프론트엔드 통합과 고급 패턴 (Part 7-8)
- 1개월: 테스팅, 성능, 배포 (Part 9-11)
- 2-4주: 실전 프로젝트 (Part 12)

### 다음 단계
- 실제 프로젝트 시작하기
- 오픈 소스 기여
- .NET 커뮤니티 참여
- 계속되는 학습

### 최종 조언
- 프론트엔드 지식은 자산이다
- 점진적으로 학습하라
- 실습이 가장 중요하다
- 커뮤니티를 활용하라

---

**총 27개 챕터, 약 800-1000페이지 분량 예상**

이 책은 JavaScript/TypeScript 백그라운드를 가진 프론트엔드 개발자가 ASP.NET Core를 효과적으로 학습할 수 있도록 설계되었습니다. 익숙한 개념에서 시작하여 점진적으로 .NET 생태계의 강력한 기능들을 탐구하며, 실전 프로젝트를 통해 완전한 풀스택 개발자로 성장할 수 있는 로드맵을 제공합니다.