# 프론트엔드 개발자를 위한 ASP.NET Core 완벽 가이드
## Frontend Developer's Complete Guide to ASP.NET Core

<p align="center">
  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet" alt=".NET 9.0">
  <img src="https://img.shields.io/badge/C%23-13%20%7C%2014-239120?style=flat-square&logo=csharp" alt="C# 13/14">
  <img src="https://img.shields.io/badge/ASP.NET_Core-9.0-512BD4?style=flat-square" alt="ASP.NET Core 9.0">
</p>

---

## 📖 소개

JavaScript, TypeScript, React, Vue, Angular에 익숙한 프론트엔드 개발자를 위한 ASP.NET Core 완벽 학습 가이드입니다.

이 가이드는 프론트엔드 개발자의 관점에서 ASP.NET Core를 이해하고 활용할 수 있도록, 익숙한 개념과 비교하며 단계적으로 학습할 수 있게 구성되었습니다.

## 🎯 이 가이드의 특징

- **🔄 비교 학습**: JavaScript/TypeScript 개념과 C#/.NET 개념을 비교하며 학습
- **📊 프론트엔드 관점**: React, Vue, Angular 개발자가 이해하기 쉬운 설명
- **🛠️ 실전 중심**: 각 챕터마다 실습 프로젝트 포함
- **🆕 최신 기술**: .NET 9, C# 13/14 최신 기능 반영 (2025년 기준)
- **🌐 풀스택 로드맵**: 프론트엔드에서 풀스택 개발자로 성장하는 완벽한 경로

## 👥 대상 독자

- JavaScript/TypeScript 개발 경험이 있는 프론트엔드 개발자
- React, Vue, Angular 등 모던 프론트엔드 프레임워크 사용 경험자
- Node.js/Express 백엔드 경험이 있으면 더욱 좋음
- 백엔드 기술을 배워 풀스택 개발자로 성장하고 싶은 분
- ASP.NET Core를 처음 접하거나 체계적으로 학습하고 싶은 분

## 🗺️ 학습 로드맵

**총 28개 챕터 + 7개 부록** | **예상 학습 기간: 4-6개월**

```
Part 1-2  (1-2개월) → C# 기초 & ASP.NET Core 핵심
Part 3-4  (2-3개월) → 서버 사이드 렌더링 & Blazor
Part 5-6  (1-2개월) → 데이터베이스 & API 개발
Part 7-8  (1-2개월) → 실시간 통신 & 고급 패턴
Part 9-11 (1개월)   → 테스팅, 성능, 배포
Part 12   (2-4주)   → 실전 프로젝트
```

## 📚 목차

### [**Part 1: C# 기초 - 자바스크립트/타입스크립트 개발자 관점**](./part1-csharp-basics/README.md)
- **[Chapter 1: C# 기초 문법 - TypeScript 개발자를 위한 빠른 시작](./part1-csharp-basics/README.md#chapter-1-c-기초-문법---typescript-개발자를-위한-빠른-시작)**
  - 타입 시스템, 람다 표현식, async/await, LINQ, 패턴 매칭
- **[Chapter 2: 객체지향 프로그래밍과 고급 기능](./part1-csharp-basics/README.md#chapter-2-객체지향-프로그래밍과-고급-기능)**
  - 값 타입 vs 참조 타입, Properties, Events, LINQ 고급, C# 13/14 최신 기능

### [**Part 2: ASP.NET Core 기초 - 새로운 패러다임의 이해**](./part2-aspnetcore-basics/README.md)
- **[Chapter 3: ASP.NET Core 소개와 개발 환경 설정](./part2-aspnetcore-basics/README.md#chapter-3-aspnet-core-소개와-개발-환경-설정)**
  - .NET 소개, 개발 환경 구성, 첫 번째 애플리케이션
- **[Chapter 4: ASP.NET Core의 핵심 아키텍처](./part2-aspnetcore-basics/README.md#chapter-4-aspnet-core의-핵심-아키텍처)**
  - 미들웨어 파이프라인, 의존성 주입, 라우팅, 구성 관리, 로깅
- **[Chapter 5: Minimal APIs - Node.js Express 개발자를 위한 빠른 시작](./part2-aspnetcore-basics/README.md#chapter-5-minimal-apis---nodejs-express-개발자를-위한-빠른-시작)**
  - Express.js 스타일의 빠른 API 개발

### [**Part 3: 서버 사이드 렌더링 - Razor Pages와 MVC**](./part3-server-side-rendering/README.md)
- **[Chapter 6: Razor 문법 - JSX를 아는 개발자를 위한 가이드](./part3-server-side-rendering/README.md#chapter-6-razor-문법---jsx를-아는-개발자를-위한-가이드)**
  - JSX vs Razor, 문법 비교, Tag Helpers
- **[Chapter 7: Razor Pages - 단순함의 힘](./part3-server-side-rendering/README.md#chapter-7-razor-pages---단순함의-힘)**
  - Next.js Pages Router와 유사한 페이지 기반 라우팅
- **[Chapter 8: MVC 패턴 - 대규모 애플리케이션을 위한 구조](./part3-server-side-rendering/README.md#chapter-8-mvc-패턴---대규모-애플리케이션을-위한-구조)**
  - Model-View-Controller 아키텍처, Filters, Areas

### [**Part 4: Blazor - 프론트엔드 개발자의 친숙한 영역**](./part4-blazor/README.md)
- **[Chapter 9: Blazor 소개 - C#으로 작성하는 프론트엔드](./part4-blazor/README.md#chapter-9-blazor-소개---c으로-작성하는-프론트엔드)**
  - React/Vue/Angular의 C# 대안, 호스팅 모델 비교
- **[Chapter 10: Blazor 컴포넌트 개발](./part4-blazor/README.md#chapter-10-blazor-컴포넌트-개발)**
  - Props, 상태 관리, 생명주기, 폼, JavaScript Interop
- **[Chapter 11: Blazor 고급 패턴](./part4-blazor/README.md#chapter-11-blazor-고급-패턴)**
  - 라우팅, 레이아웃, 컴포넌트 라이브러리, 성능 최적화, 인증

### [**Part 5: 데이터 액세스 - Entity Framework Core**](./part5-data-access/README.md)
- **[Chapter 12: Entity Framework Core 기초](./part5-data-access/README.md#chapter-12-entity-framework-core-기초)**
  - ORM 소개, Prisma/TypeORM 비교, DbContext, 마이그레이션, LINQ 쿼리
- **[Chapter 13: Entity Framework Core 고급](./part5-data-access/README.md#chapter-13-entity-framework-core-고급)**
  - 고급 쿼리, 성능 최적화, Repository 패턴, 다중 DB 지원

### [**Part 6: API 개발 - RESTful에서 GraphQL까지**](./part6/README.md)
- **[Chapter 14: RESTful API 설계와 구현](./part6/README.md#chapter-14-restful-api-설계와-구현)**
  - REST 원칙, 컨트롤러 기반 API, 버전 관리, OpenAPI/Swagger, CORS
- **[Chapter 15: API 보안과 인증](./part6/README.md#chapter-15-api-보안과-인증)**
  - JWT 인증, ASP.NET Core Identity, OAuth 2.0, 권한 부여 패턴
- **[Chapter 16: GraphQL과 SignalR](./part6/README.md#chapter-16-graphql과-signalr)**
  - Hot Chocolate, SignalR 실시간 통신

### [**Part 7: 프로덕션 준비 - 실시간 통신과 클라이언트 통합**](./part7/README.md)
- **[Chapter 17: 실시간 통신과 백그라운드 처리](./part7/README.md#chapter-17-실시간-통신과-백그라운드-처리)**
  - Socket.io vs SignalR, SSE, WebSocket, Hangfire, 메시지 큐
- **[Chapter 18: API 클라이언트 패턴](./part7/README.md#chapter-18-api-클라이언트-패턴)**
  - NSwag, Kiota, React Query 통합, 캐싱 전략

### [**Part 8: 상태 관리와 패턴**](./part8/README.md)
- **[Chapter 19: 서버 사이드 상태 관리](./part8/README.md#chapter-19-서버-사이드-상태-관리)**
  - 세션 관리, 캐싱 전략, HybridCache (.NET 9)
- **[Chapter 20: 고급 아키텍처 패턴](./part8/README.md#chapter-20-고급-아키텍처-패턴)**
  - Clean Architecture, CQRS, DDD, Microservices

### [**Part 9: 테스팅 전략**](./part9/README.md)
- **[Chapter 21: 단위 테스트와 통합 테스트](./part9/README.md#chapter-21-단위-테스트와-통합-테스트)**
  - xUnit, Moq, WebApplicationFactory, bUnit, Playwright, TDD

### [**Part 10: 성능 최적화와 모니터링**](./part10/README.md)
- **[Chapter 22: 성능 최적화 기법](./part10/README.md#chapter-22-성능-최적화-기법)**
  - 프로파일링, 응답 압축, 캐싱, DB 최적화, Native AOT
- **[Chapter 23: 모니터링과 로깅](./part10/README.md#chapter-23-모니터링과-로깅)**
  - Application Insights, Serilog, OpenTelemetry, Prometheus/Grafana

### [**Part 11: 배포와 DevOps**](./part11/README.md)
- **[Chapter 24: 컨테이너화와 Docker](./part11/README.md#chapter-24-컨테이너화와-docker)**
  - Dockerfile, Multi-stage 빌드, Docker Compose
- **[Chapter 25: 클라우드 배포 - Azure 중심](./part11/README.md#chapter-25-클라우드-배포---azure-중심)**
  - Azure App Service, Container Apps, AKS, Functions, CI/CD
- **[Chapter 26: 프로덕션 고려사항](./part11/README.md#chapter-26-프로덕션-고려사항)**
  - 환경 구성, HTTPS, Rate Limiting, 보안, 백업

### [**Part 12: 실전 프로젝트와 모범 사례**](./part12/README.md)
- **[Chapter 27: 전자상거래 플랫폼 구축 (종합 프로젝트)](./part12/README.md#chapter-27-전자상거래-플랫폼-구축-종합-프로젝트)**
  - 실전 프로젝트: 백엔드 API, 인증, 프론트엔드 통합, 결제, 검색, 배포
- **[Chapter 28: 모범 사례 종합](./part12/README.md#chapter-28-모범-사례-종합)**
  - 코드 품질, 보안, 성능, 유지보수성, 확장성

### [**부록 (Appendices)**](./appendices/README.md)
- **[Appendix A: C# 치트 시트 - JavaScript/TypeScript 개발자용](./appendices/README.md#appendix-a-c-치트-시트---javascripttypescript-개발자용)**
- **[Appendix B: ASP.NET Core 프로젝트 템플릿 가이드](./appendices/README.md#appendix-b-aspnet-core-프로젝트-템플릿-가이드)**
- **[Appendix C: 유용한 NuGet 패키지 모음](./appendices/README.md#appendix-c-유용한-nuget-패키지-모음)**
- **[Appendix D: 도구와 확장 프로그램](./appendices/README.md#appendix-d-도구와-확장-프로그램)**
- **[Appendix E: 학습 리소스와 커뮤니티](./appendices/README.md#appendix-e-학습-리소스와-커뮤니티)**
- **[Appendix F: 마이그레이션 가이드](./appendices/README.md#appendix-f-마이그레이션-가이드)**
- **[Appendix G: 문제 해결 가이드](./appendices/README.md#appendix-g-문제-해결-가이드)**

---

## 🚀 시작하기

### 필요한 사전 지식

- ✅ JavaScript/TypeScript 기본 문법
- ✅ React, Vue, Angular 중 하나 이상의 프레임워크 경험
- ✅ HTTP, REST API 기본 개념
- ✅ Git 기본 사용법
- 🔵 Node.js/Express 경험 (선택사항이지만 도움됨)

### 개발 환경 준비

1. **.NET SDK 설치**
   ```bash
   # Windows (winget)
   winget install Microsoft.DotNet.SDK.9

   # macOS (Homebrew)
   brew install dotnet-sdk

   # Linux (Ubuntu/Debian)
   sudo apt-get update
   sudo apt-get install -y dotnet-sdk-9.0
   ```

2. **코드 에디터**
   - [Visual Studio Code](https://code.visualstudio.com/) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) (추천)
   - [JetBrains Rider](https://www.jetbrains.com/rider/)
   - [Visual Studio 2022](https://visualstudio.microsoft.com/)

3. **설치 확인**
   ```bash
   dotnet --version  # 9.0 이상 확인
   ```

### 첫 단계

1. **Part 1부터 시작**: [C# 기초 - 자바스크립트/타입스크립트 개발자 관점](./part1-csharp-basics/README.md)
2. **각 챕터의 실습 프로젝트를 직접 실행**해보세요
3. **막히는 부분은 커뮤니티에 질문**하세요 (Stack Overflow, Reddit, Discord)

---

## 💡 학습 팁

### 효과적인 학습 방법

1. **비교하며 학습하기**: 이미 알고 있는 JavaScript/TypeScript 개념과 비교하며 C#을 이해하세요
2. **작은 프로젝트부터**: To-do 앱, 블로그, 간단한 API부터 시작하세요
3. **공식 문서 활용**: [Microsoft Learn](https://learn.microsoft.com/dotnet/)은 훌륭한 무료 리소스입니다
4. **커뮤니티 참여**: 막힐 때 주저하지 말고 질문하세요

### 각 파트별 예상 학습 시간

| Part | 내용 | 예상 시간 |
|------|------|----------|
| Part 1-2 | C# 기초 & ASP.NET Core 핵심 | 1-2개월 |
| Part 3-4 | SSR & Blazor | 2-3개월 |
| Part 5-6 | 데이터베이스 & API | 1-2개월 |
| Part 7-8 | 실시간 통신 & 패턴 | 1-2개월 |
| Part 9-11 | 테스팅, 성능, 배포 | 1개월 |
| Part 12 | 실전 프로젝트 | 2-4주 |

**총 예상 기간**: 4-6개월 (주 10-15시간 학습 기준)

---

## 🤝 기여하기

이 가이드는 오픈소스 프로젝트입니다. 기여를 환영합니다!

### 기여 방법

1. 오타, 잘못된 정보 수정
2. 예제 코드 개선
3. 새로운 실습 프로젝트 추가
4. 번역 (한글 ↔ 영어)
5. 이슈 리포팅

자세한 내용은 [CONTRIBUTING.md](./CONTRIBUTING.md)를 참고하세요.

---

## 📞 커뮤니티 & 지원

### 질문하기

- **GitHub Issues**: 버그 리포트, 개선 제안
- **GitHub Discussions**: 일반적인 질문, 토론
- **Stack Overflow**: `asp.net-core` 태그 사용

### 유용한 링크

- [Microsoft Learn - .NET](https://learn.microsoft.com/dotnet/)
- [ASP.NET Core 공식 문서](https://learn.microsoft.com/aspnet/core/)
- [C# 공식 문서](https://learn.microsoft.com/dotnet/csharp/)
- [.NET Blog](https://devblogs.microsoft.com/dotnet/)

### 한국 커뮤니티

- [한국 .NET 개발자 그룹](https://www.facebook.com/groups/dotnetkorea/)
- [ASP.NET Korea](https://forum.dotnetdev.kr/)

---

## 📄 라이선스

이 프로젝트는 [MIT License](./LICENSE)로 배포됩니다.

---

## 🙏 감사의 말

이 가이드는 JavaScript/TypeScript 프론트엔드 개발자들이 ASP.NET Core를 더 쉽게 배울 수 있도록 하기 위해 만들어졌습니다.

여러분의 프론트엔드 경험은 이미 훌륭한 출발점입니다. 이제 그 위에 강력한 백엔드 기술을 쌓아올릴 차례입니다.

**행운을 빕니다! 🚀**

---

<p align="center">
  Made with ❤️ for Frontend Developers learning ASP.NET Core
</p>
