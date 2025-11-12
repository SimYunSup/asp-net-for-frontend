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

## 📖 문서 사이트

이 가이드는 Astro Starlight로 빌드되어 정적 사이트로 배포할 수 있습니다.

### 로컬에서 문서 사이트 실행하기

```bash
# 의존성 설치
pnpm install

# 개발 서버 시작
pnpm dev

# 프로덕션 빌드
pnpm build

# 빌드 미리보기
pnpm preview
```

개발 서버는 `http://localhost:4321/asp-net-for-frontend/`에서 실행됩니다.

### GitHub Pages 배포

이 리포지토리는 GitHub Actions를 통해 자동으로 GitHub Pages에 배포됩니다.

**배포 설정 방법:**

1. GitHub 리포지토리의 Settings > Pages로 이동
2. Source를 "GitHub Actions"로 선택
3. `main` 브랜치에 push하면 자동으로 배포됩니다

배포된 사이트는 `https://<username>.github.io/asp-net-for-frontend/`에서 확인할 수 있습니다.

**커스텀 도메인 사용 시:**

`astro.config.mjs` 파일에서 `base` 값을 `'/'`로 변경하세요:

```javascript
export default defineConfig({
  base: '/', // 커스텀 도메인 사용 시
  // ...
})
```

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

1. **Part 1부터 시작**: C# 기초 - 자바스크립트/타입스크립트 개발자 관점
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
4. 이슈 리포팅

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
- [닷넷데브 포럼](https://forum.dotnetdev.kr/)

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
