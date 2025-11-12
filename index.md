---
layout: home

hero:
  name: ASP.NET Core
  text: for Frontend Developers
  tagline: 프론트엔드 개발자를 위한 완벽한 ASP.NET Core 가이드
  image:
    src: /logo.svg
    alt: ASP.NET Core
  actions:
    - theme: brand
      text: Get Started
      link: /part1-csharp-basics/README
    - theme: alt
      text: View on GitHub
      link: https://github.com/SimYunSup/asp-net-for-frontend

features:
  - icon: 🔄
    title: 비교 학습
    details: JavaScript/TypeScript 개념과 C#/.NET 개념을 비교하며 학습하여 빠르게 이해할 수 있습니다.
  - icon: 📊
    title: 프론트엔드 관점
    details: React, Vue, Angular 개발자가 이해하기 쉬운 설명과 예제로 구성되어 있습니다.
  - icon: 🛠️
    title: 실전 중심
    details: 각 챕터마다 실습 프로젝트가 포함되어 실제로 코드를 작성하며 배울 수 있습니다.
  - icon: 🆕
    title: 최신 기술
    details: .NET 9, C# 13/14 최신 기능을 반영하여 현대적인 개발 방법을 익힐 수 있습니다.
  - icon: 🌐
    title: 풀스택 로드맵
    details: 프론트엔드에서 풀스택 개발자로 성장하는 완벽한 경로를 제시합니다.
  - icon: 📚
    title: 체계적인 구성
    details: 28개 챕터 + 7개 부록으로 기초부터 실전까지 단계적으로 학습할 수 있습니다.
---

## 🎯 대상 독자

- **JavaScript/TypeScript** 개발 경험이 있는 프론트엔드 개발자
- **React, Vue, Angular** 등 모던 프론트엔드 프레임워크 사용 경험자
- **Node.js/Express** 백엔드 경험이 있으면 더욱 좋음
- 백엔드 기술을 배워 **풀스택 개발자**로 성장하고 싶은 분
- ASP.NET Core를 **처음 접하거나 체계적으로** 학습하고 싶은 분

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

1. **Part 1부터 시작**: [C# 기초 - 자바스크립트/타입스크립트 개발자 관점](/part1-csharp-basics/README)
2. **각 챕터의 실습 프로젝트를 직접 실행**해보세요
3. **막히는 부분은 커뮤니티에 질문**하세요 (Stack Overflow, Reddit, Discord)

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
