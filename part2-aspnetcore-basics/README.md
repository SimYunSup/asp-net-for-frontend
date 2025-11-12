# Part 2: ASP.NET Core 기초 - 새로운 패러다임의 이해

이 파트에서는 ASP.NET Core의 핵심 개념과 아키텍처를 프론트엔드 개발자 관점에서 학습합니다. Node.js, Express.js, React 등 익숙한 기술과 비교하며 ASP.NET Core를 이해합니다.

## 학습 목표

이 파트를 마치면 다음을 할 수 있습니다:
- ASP.NET Core 프로젝트를 생성하고 실행할 수 있습니다
- 미들웨어 파이프라인을 이해하고 커스텀 미들웨어를 작성할 수 있습니다
- 의존성 주입(DI)을 활용하여 느슨하게 결합된 코드를 작성할 수 있습니다
- Minimal APIs로 RESTful API를 빠르게 구축할 수 있습니다
- 구성 관리와 로깅을 효과적으로 활용할 수 있습니다

## 챕터 구성

### [Chapter 2: ASP.NET Core 소개와 개발 환경 설정](./chapter2/README.md)
- ASP.NET Core란 무엇인가?
- .NET Framework에서 .NET Core로의 진화
- VS Code + C# Dev Kit으로 개발 환경 구성
- dotnet CLI 마스터하기
- 첫 번째 "Hello World" API 만들기

**핵심 개념**: 크로스 플랫폼, Kestrel, Hot Reload, .csproj

### [Chapter 3: ASP.NET Core의 핵심 아키텍처](./chapter3/README.md)
- 요청-응답 파이프라인과 미들웨어
- 의존성 주입(DI)의 강력한 활용
- 라우팅 시스템 (컨벤션 vs 특성)
- 구성 관리: appsettings.json, 환경 변수, 사용자 시크릿
- 구조화된 로깅과 모니터링

**핵심 개념**: Middleware, DI Container, Service Lifetime, Configuration, ILogger

### [Chapter 4: Minimal APIs - Node.js Express 개발자를 위한 빠른 시작](./chapter4/README.md)
- Minimal APIs 소개: 컨트롤러 없는 API
- HTTP 메서드 매핑과 라우팅
- 의존성 주입을 통한 서비스 사용
- Results 헬퍼로 응답 생성
- OpenAPI/Swagger 자동 문서화
- 완전한 CRUD API 구현

**핵심 개념**: MapGet/Post/Put/Delete, Results, TypedResults, OpenAPI

## 실습 프로젝트

각 챕터에는 실습 예제가 포함되어 있습니다. 직접 코드를 작성하며 개념을 익히세요.

### Chapter 2 실습: "Hello World"에서 실제 API까지
간단한 Todo API를 만들며 ASP.NET Core의 기본을 익힙니다.

### Chapter 3 실습: 미들웨어 파이프라인 구축
커스텀 미들웨어를 작성하여 요청 ID 추적, 성능 측정, 에러 처리를 구현합니다.

### Chapter 4 실습: RESTful API 완성
Minimal APIs로 완전한 CRUD 엔드포인트, 유효성 검사, 에러 처리를 갖춘 API를 만듭니다.

## Express.js와 ASP.NET Core 비교

프론트엔드 개발자가 익숙한 Express.js와 ASP.NET Core를 비교합니다.

| 개념 | Express.js | ASP.NET Core |
|------|------------|--------------|
| 프로젝트 생성 | `npm init` | `dotnet new webapi` |
| 의존성 설치 | `npm install express` | `dotnet add package` |
| 개발 서버 | `npm run dev` | `dotnet watch run` |
| 라우팅 | `app.get('/users', handler)` | `app.MapGet("/users", handler)` |
| 미들웨어 | `app.use(middleware)` | `app.Use(middleware)` |
| 의존성 주입 | 수동 구현 필요 | 내장 DI 컨테이너 |
| 환경 변수 | `.env` 파일 | `appsettings.json`, 환경 변수 |

## 다음 단계

Part 2를 완료하면 다음 파트로 이동하세요:
- **Part 3**: 서버 사이드 렌더링 - Razor Pages와 MVC
- **Part 4**: Blazor - C#으로 작성하는 프론트엔드
- **Part 5**: Entity Framework Core - 데이터 액세스

## 추가 리소스

- [ASP.NET Core 공식 문서](https://docs.microsoft.com/aspnet/core)
- [.NET CLI 가이드](https://docs.microsoft.com/dotnet/core/tools/)
- [Minimal APIs 가이드](https://docs.microsoft.com/aspnet/core/fundamentals/minimal-apis)

즐거운 학습 되세요! 🚀
