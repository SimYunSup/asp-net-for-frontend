---
title: "Appendix B - .NET CLI 명령어 참조"
---

# Appendix B: .NET CLI 명령어 참조

.NET CLI(Command Line Interface)는 .NET 애플리케이션 개발, 빌드, 테스트, 배포에 필요한 모든 명령어를 제공합니다. 이 참조 가이드는 실무에서 가장 많이 사용하는 CLI 명령어를 다룹니다.

## 1. 기본 명령어

### 1.1 버전 및 정보 확인

```bash
# .NET SDK 버전 확인
dotnet --version
# 출력 예: 8.0.100

# 설치된 모든 SDK 버전 나열
dotnet --list-sdks
# 출력 예:
# 6.0.420 [C:\Program Files\dotnet\sdk]
# 7.0.407 [C:\Program Files\dotnet\sdk]
# 8.0.100 [C:\Program Files\dotnet\sdk]

# 설치된 모든 런타임 나열
dotnet --list-runtimes
# 출력 예:
# Microsoft.AspNetCore.App 8.0.0 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
# Microsoft.NETCore.App 8.0.0 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]

# .NET 정보 전체 출력
dotnet --info
```

**npm 비교**
```bash
node --version
npm --version
npm list -g --depth=0
```

### 1.2 도움말

```bash
# 전체 도움말
dotnet --help

# 특정 명령어 도움말
dotnet new --help
dotnet build --help
dotnet run --help
```

## 2. 프로젝트 생성 및 관리

### 2.1 프로젝트 템플릿

```bash
# 사용 가능한 모든 템플릿 나열
dotnet new list

# 주요 템플릿:
# console          - Console Application
# classlib         - Class Library
# webapi           - ASP.NET Core Web API
# mvc              - ASP.NET Core MVC
# webapp           - ASP.NET Core Web App (Razor Pages)
# blazorserver     - Blazor Server App
# blazorwasm       - Blazor WebAssembly App
# worker           - Worker Service
# xunit            - xUnit Test Project
# nunit            - NUnit Test Project

# 특정 템플릿 검색
dotnet new list webapi
```

### 2.2 프로젝트 생성

```bash
# 콘솔 애플리케이션 생성
dotnet new console -n MyConsoleApp

# Web API 프로젝트 생성
dotnet new webapi -n MyApi

# 최소 API (minimal API) 생성
dotnet new webapi -n MyMinimalApi --use-minimal-apis

# MVC 프로젝트 생성
dotnet new mvc -n MyMvcApp

# Blazor Server 프로젝트 생성
dotnet new blazorserver -n MyBlazorApp

# 특정 프레임워크 버전 지정
dotnet new webapi -n MyApi -f net8.0

# 인증 포함하여 생성
dotnet new webapi -n MyApi --auth Individual

# 현재 디렉터리에 프로젝트 생성
dotnet new webapi

# HTTPS 비활성화
dotnet new webapi -n MyApi --no-https
```

**npm 비교**
```bash
npm init -y
npx create-react-app my-app
npx create-next-app my-next-app
```

### 2.3 솔루션 관리

```bash
# 새 솔루션 생성
dotnet new sln -n MySolution

# 현재 디렉터리에 솔루션 생성
dotnet new sln

# 프로젝트를 솔루션에 추가
dotnet sln add ./src/MyApi/MyApi.csproj
dotnet sln add ./tests/MyApi.Tests/MyApi.Tests.csproj

# 여러 프로젝트 한 번에 추가
dotnet sln add **/*.csproj

# 솔루션에서 프로젝트 제거
dotnet sln remove ./src/MyApi/MyApi.csproj

# 솔루션의 프로젝트 목록 보기
dotnet sln list

# 폴더 구조 예시
# MySolution/
# ├── MySolution.sln
# ├── src/
# │   ├── MyApi/
# │   │   └── MyApi.csproj
# │   └── MyApi.Core/
# │       └── MyApi.Core.csproj
# └── tests/
#     └── MyApi.Tests/
#         └── MyApi.Tests.csproj

# 전체 솔루션 빌드
dotnet build MySolution.sln

# 전체 솔루션 실행 (startup project)
dotnet run --project ./src/MyApi/MyApi.csproj
```

### 2.4 프로젝트 참조

```bash
# 프로젝트 참조 추가
cd MyApi
dotnet add reference ../MyApi.Core/MyApi.Core.csproj

# 여러 참조 추가
dotnet add reference ../MyApi.Core/MyApi.Core.csproj ../MyApi.Data/MyApi.Data.csproj

# 참조 제거
dotnet remove reference ../MyApi.Core/MyApi.Core.csproj

# 프로젝트 참조 목록 보기
dotnet list reference
```

## 3. NuGet 패키지 관리

### 3.1 패키지 추가 및 제거

```bash
# 패키지 설치
dotnet add package Newtonsoft.Json

# 특정 버전 설치
dotnet add package Newtonsoft.Json --version 13.0.3

# 최신 프리릴리스 버전 설치
dotnet add package Newtonsoft.Json --prerelease

# 패키지 제거
dotnet remove package Newtonsoft.Json

# 설치된 패키지 목록 보기
dotnet list package

# 업데이트 가능한 패키지 보기
dotnet list package --outdated

# 취약점 있는 패키지 보기
dotnet list package --vulnerable

# 사용하지 않는(Deprecated) 패키지 보기
dotnet list package --deprecated
```

**npm 비교**
```bash
npm install express
npm install express@4.18.2
npm install express@latest
npm uninstall express
npm list
npm outdated
npm audit
```

### 3.2 패키지 복원

```bash
# 모든 프로젝트 종속성 복원
dotnet restore

# 특정 프로젝트 복원
dotnet restore MyApi.csproj

# 특정 소스에서 복원
dotnet restore --source https://api.nuget.org/v3/index.json

# 패키지 캐시 정리
dotnet nuget locals all --clear

# 캐시 위치 확인
dotnet nuget locals all --list
```

**npm 비교**
```bash
npm install
npm ci
npm cache clean --force
```

### 3.3 패키지 검색

```bash
# 패키지 검색
dotnet tool search Swashbuckle

# NuGet.org에서 검색 (브라우저)
# https://www.nuget.org/packages
```

## 4. 빌드 및 실행

### 4.1 빌드

```bash
# 프로젝트 빌드
dotnet build

# Release 모드로 빌드
dotnet build --configuration Release
dotnet build -c Release

# 특정 프로젝트 빌드
dotnet build MyApi.csproj

# 빌드 출력 디렉터리 지정
dotnet build --output ./build

# 빌드 전 복원 생략 (빠른 빌드)
dotnet build --no-restore

# 상세 로그 출력
dotnet build --verbosity detailed
dotnet build -v d

# 로그 레벨:
# q[uiet], m[inimal], n[ormal], d[etailed], diag[nostic]

# 병렬 빌드 비활성화
dotnet build --no-incremental
```

**npm 비교**
```bash
npm run build
npm run build:prod
```

### 4.2 실행

```bash
# 프로젝트 실행 (빌드 포함)
dotnet run

# 특정 프로젝트 실행
dotnet run --project ./src/MyApi/MyApi.csproj

# Release 모드로 실행
dotnet run --configuration Release

# 빌드 없이 실행 (이미 빌드된 경우)
dotnet run --no-build

# 환경 변수 설정하여 실행
ASPNETCORE_ENVIRONMENT=Production dotnet run

# 명령줄 인수 전달
dotnet run -- --arg1 value1 --arg2 value2

# 특정 DLL 실행
dotnet MyApi.dll

# 특정 포트로 실행
dotnet run --urls "http://localhost:5000;https://localhost:5001"
```

**npm 비교**
```bash
npm start
npm run dev
NODE_ENV=production npm start
```

### 4.3 감시 모드 (Hot Reload)

```bash
# 파일 변경 감시 및 자동 재시작
dotnet watch run

# 특정 프로젝트 감시
dotnet watch --project ./src/MyApi/MyApi.csproj run

# Hot Reload (코드 변경 시 재시작 없이 반영)
dotnet watch

# Blazor Hot Reload
cd BlazorApp
dotnet watch
```

**npm 비교**
```bash
npm run dev    # Next.js, Vite 등
nodemon app.js
```

### 4.4 게시 (Publish)

```bash
# 프로젝트 게시 (프로덕션 배포용)
dotnet publish

# Release 모드로 게시
dotnet publish -c Release

# 특정 출력 디렉터리로 게시
dotnet publish -c Release -o ./publish

# 자체 포함 배포 (Self-contained)
dotnet publish -c Release -r linux-x64 --self-contained

# 런타임 종속 배포 (Framework-dependent)
dotnet publish -c Release -r linux-x64 --self-contained false

# 단일 파일 실행 파일 생성
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

# ReadyToRun (R2R) 컴파일 사용
dotnet publish -c Release /p:PublishReadyToRun=true

# 트리밍 활성화 (앱 크기 감소)
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishTrimmed=true

# 주요 런타임 식별자 (RID):
# - win-x64, win-x86, win-arm64
# - linux-x64, linux-arm, linux-arm64
# - osx-x64, osx-arm64
```

**npm 비교**
```bash
npm run build
npm run build:prod
```

## 5. 테스트

### 5.1 테스트 실행

```bash
# 모든 테스트 실행
dotnet test

# 특정 테스트 프로젝트 실행
dotnet test MyApi.Tests.csproj

# 상세 로그와 함께 실행
dotnet test --logger "console;verbosity=detailed"

# 특정 테스트만 실행 (필터)
dotnet test --filter "FullyQualifiedName~ProductController"
dotnet test --filter "Category=Integration"

# 병렬 실행 비활성화
dotnet test --parallel false

# 테스트 결과 파일 생성
dotnet test --logger "trx;LogFileName=test-results.trx"

# 코드 커버리지 수집
dotnet test --collect:"XPlat Code Coverage"

# 코드 커버리지 도구 (coverlet) 사용
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

**npm 비교**
```bash
npm test
npm run test:unit
npm run test:integration
npm test -- --coverage
```

### 5.2 테스트 프로젝트 생성

```bash
# xUnit 테스트 프로젝트 생성
dotnet new xunit -n MyApi.Tests

# NUnit 테스트 프로젝트 생성
dotnet new nunit -n MyApi.Tests

# MSTest 프로젝트 생성
dotnet new mstest -n MyApi.Tests

# 테스트 프로젝트를 솔루션에 추가
dotnet sln add MyApi.Tests/MyApi.Tests.csproj

# 테스트 프로젝트에서 원본 프로젝트 참조
cd MyApi.Tests
dotnet add reference ../MyApi/MyApi.csproj
```

## 6. 도구 (Global Tools)

### 6.1 전역 도구 관리

```bash
# 전역 도구 설치
dotnet tool install -g dotnet-ef

# 특정 버전 설치
dotnet tool install -g dotnet-ef --version 8.0.0

# 전역 도구 업데이트
dotnet tool update -g dotnet-ef

# 전역 도구 제거
dotnet tool uninstall -g dotnet-ef

# 설치된 전역 도구 목록
dotnet tool list -g
```

**npm 비교**
```bash
npm install -g typescript
npm update -g typescript
npm uninstall -g typescript
npm list -g --depth=0
```

### 6.2 로컬 도구 (Local Tools)

```bash
# 로컬 도구 매니페스트 생성
dotnet new tool-manifest

# 로컬 도구 설치
dotnet tool install dotnet-ef

# 로컬 도구 실행
dotnet tool run dotnet-ef
# 또는
dotnet ef

# 로컬 도구 목록
dotnet tool list

# 로컬 도구 복원 (CI/CD에서 유용)
dotnet tool restore
```

### 6.3 인기 있는 .NET 도구

```bash
# Entity Framework Core CLI
dotnet tool install -g dotnet-ef

# ASP.NET Core 코드 생성기
dotnet tool install -g dotnet-aspnet-codegenerator

# 사용자 비밀 관리
dotnet tool install -g dotnet-user-secrets

# SQL Server CLI
dotnet tool install -g dotnet-sql-cache

# Report Generator (코드 커버리지)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Swagger/OpenAPI CLI
dotnet tool install -g Swashbuckle.AspNetCore.Cli
```

## 7. Entity Framework Core

### 7.1 마이그레이션

```bash
# 마이그레이션 생성
dotnet ef migrations add InitialCreate

# 특정 컨텍스트 지정
dotnet ef migrations add InitialCreate --context ApplicationDbContext

# 특정 프로젝트에서 실행
dotnet ef migrations add InitialCreate --project ./src/MyApi.Data

# 마이그레이션 목록 보기
dotnet ef migrations list

# 마이그레이션 제거 (마지막 마이그레이션)
dotnet ef migrations remove

# 마이그레이션 SQL 스크립트 생성
dotnet ef migrations script

# 특정 마이그레이션부터 SQL 생성
dotnet ef migrations script InitialCreate

# 멱등성 스크립트 생성 (여러 번 실행 가능)
dotnet ef migrations script --idempotent

# 마이그레이션 SQL을 파일로 저장
dotnet ef migrations script -o migrations.sql
```

### 7.2 데이터베이스 관리

```bash
# 데이터베이스 업데이트 (마이그레이션 적용)
dotnet ef database update

# 특정 마이그레이션까지 업데이트
dotnet ef database update InitialCreate

# 모든 마이그레이션 롤백
dotnet ef database update 0

# 데이터베이스 삭제
dotnet ef database drop

# 강제 삭제 (확인 없이)
dotnet ef database drop --force

# 데이터베이스 정보 보기
dotnet ef dbcontext info

# DbContext 목록 보기
dotnet ef dbcontext list
```

### 7.3 스캐폴딩 (Reverse Engineering)

```bash
# 기존 데이터베이스에서 모델 생성
dotnet ef dbcontext scaffold "Server=localhost;Database=MyDb;User=sa;Password=Pass123;" Microsoft.EntityFrameworkCore.SqlServer

# 특정 테이블만 스캐폴드
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer --table Products --table Orders

# 출력 디렉터리 지정
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models

# 컨텍스트 디렉터리 지정
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer --context-dir Data

# Data Annotations 사용
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer --data-annotations

# 강제 덮어쓰기
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer --force
```

## 8. 사용자 비밀 (User Secrets)

```bash
# 사용자 비밀 초기화
dotnet user-secrets init

# 비밀 설정
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=MyDb;"

# JSON 형식으로 설정
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."

# 비밀 목록 보기
dotnet user-secrets list

# 특정 비밀 제거
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"

# 모든 비밀 제거
dotnet user-secrets clear

# 사용자 비밀 파일 위치:
# Windows: %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
# macOS/Linux: ~/.microsoft/usersecrets/<user_secrets_id>/secrets.json
```

**npm 비교**
```bash
# .env 파일 사용
echo "DATABASE_URL=postgres://..." > .env
```

## 9. 개발 인증서

```bash
# HTTPS 개발 인증서 신뢰
dotnet dev-certs https --trust

# 인증서 생성
dotnet dev-certs https

# 기존 인증서 확인
dotnet dev-certs https --check

# 인증서 삭제
dotnet dev-certs https --clean

# 인증서를 파일로 내보내기
dotnet dev-certs https --export-path ./certificate.pfx --password MyPassword123
```

## 10. 코드 정리 및 포맷팅

### 10.1 코드 포맷터

```bash
# 코드 포맷 확인 (변경 없이)
dotnet format --verify-no-changes

# 코드 자동 포맷팅
dotnet format

# 특정 프로젝트/솔루션 포맷
dotnet format MyApi.sln

# 공백만 수정
dotnet format --fix-whitespace

# 스타일 규칙 적용
dotnet format --fix-style

# 분석기 규칙 적용
dotnet format --fix-analyzers

# 심각도 수준 지정
dotnet format --severity info
```

**npm 비교**
```bash
npx prettier --check .
npx prettier --write .
npx eslint .
npx eslint . --fix
```

### 10.2 코드 분석

```bash
# 코드 분석 실행
dotnet build /p:EnableNETAnalyzers=true

# 경고를 오류로 처리
dotnet build /p:TreatWarningsAsErrors=true

# 특정 경고 무시
dotnet build /p:NoWarn=CS1591,CS1572
```

## 11. 성능 및 진단

### 11.1 카운터 모니터링

```bash
# dotnet-counters 도구 설치
dotnet tool install -g dotnet-counters

# 실행 중인 .NET 프로세스 목록
dotnet-counters ps

# 카운터 모니터링
dotnet-counters monitor --process-id <PID>

# 특정 카운터 모니터링
dotnet-counters monitor --process-id <PID> --counters System.Runtime,Microsoft.AspNetCore.Hosting

# 카운터를 파일로 수집
dotnet-counters collect --process-id <PID> --output counters.json
```

### 11.2 메모리 덤프

```bash
# dotnet-dump 도구 설치
dotnet tool install -g dotnet-dump

# 메모리 덤프 생성
dotnet-dump collect --process-id <PID>

# 덤프 분석
dotnet-dump analyze <dump_file>
```

### 11.3 추적 (Trace)

```bash
# dotnet-trace 도구 설치
dotnet tool install -g dotnet-trace

# 추적 수집
dotnet-trace collect --process-id <PID>

# 특정 이벤트 추적
dotnet-trace collect --process-id <PID> --providers Microsoft-AspNetCore-Server-Kestrel

# 추적을 Speedscope 형식으로 변환
dotnet-trace convert trace.nettrace --format Speedscope
```

## 12. 워크로드 (Workloads)

```bash
# 설치 가능한 워크로드 목록
dotnet workload search

# 워크로드 설치
dotnet workload install maui
dotnet workload install wasm-tools

# 설치된 워크로드 목록
dotnet workload list

# 워크로드 업데이트
dotnet workload update

# 워크로드 제거
dotnet workload uninstall maui

# 워크로드 복원
dotnet workload restore
```

## 13. NuGet 소스 관리

```bash
# NuGet 소스 목록
dotnet nuget list source

# NuGet 소스 추가
dotnet nuget add source https://myget.org/F/myfeed/api/v3/index.json --name MyGetFeed

# 인증이 필요한 소스 추가
dotnet nuget add source https://pkgs.dev.azure.com/myorg/_packaging/myfeed/nuget/v3/index.json \
  --name AzureArtifacts \
  --username myusername \
  --password mypassword

# NuGet 소스 제거
dotnet nuget remove source MyGetFeed

# NuGet 소스 비활성화
dotnet nuget disable source MyGetFeed

# NuGet 소스 활성화
dotnet nuget enable source MyGetFeed
```

## 14. 정리 (Clean)

```bash
# 빌드 출력 정리
dotnet clean

# Release 모드 정리
dotnet clean --configuration Release

# 솔루션 전체 정리
dotnet clean MySolution.sln
```

## 15. 유용한 조합 명령어

### 15.1 새 프로젝트 설정

```bash
# 전체 솔루션 구조 생성
mkdir MyApp && cd MyApp
dotnet new sln

mkdir src tests
cd src
dotnet new webapi -n MyApp.Api
dotnet new classlib -n MyApp.Core
dotnet new classlib -n MyApp.Data

cd ../tests
dotnet new xunit -n MyApp.Api.Tests

cd ..
dotnet sln add src/MyApp.Api/MyApp.Api.csproj
dotnet sln add src/MyApp.Core/MyApp.Core.csproj
dotnet sln add src/MyApp.Data/MyApp.Data.csproj
dotnet sln add tests/MyApp.Api.Tests/MyApp.Api.Tests.csproj

cd src/MyApp.Api
dotnet add reference ../MyApp.Core/MyApp.Core.csproj
dotnet add reference ../MyApp.Data/MyApp.Data.csproj

cd ../../tests/MyApp.Api.Tests
dotnet add reference ../../src/MyApp.Api/MyApp.Api.csproj
```

### 15.2 빠른 API 개발

```bash
# 프로젝트 생성 및 실행
dotnet new webapi -n QuickApi && cd QuickApi
dotnet add package Swashbuckle.AspNetCore
dotnet watch run
```

### 15.3 CI/CD 파이프라인 명령어

```bash
# 일반적인 CI/CD 단계
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build --verbosity normal
dotnet publish --configuration Release --no-build --output ./publish
```

## 16. 환경 변수

```bash
# ASP.NET Core 환경 설정
export ASPNETCORE_ENVIRONMENT=Production  # Linux/macOS
set ASPNETCORE_ENVIRONMENT=Production     # Windows CMD
$env:ASPNETCORE_ENVIRONMENT="Production"  # PowerShell

# 주요 환경 변수:
# - ASPNETCORE_ENVIRONMENT: Development, Staging, Production
# - ASPNETCORE_URLS: 수신 URL (예: http://localhost:5000)
# - DOTNET_ENVIRONMENT: .NET 일반 환경
# - DOTNET_CLI_TELEMETRY_OPTOUT: 원격 분석 비활성화 (1)

# 로깅 레벨 설정
export Logging__LogLevel__Default=Debug
export Logging__LogLevel__Microsoft=Warning
```

## 17. 빠른 참조 치트시트

### 프로젝트 생성
```bash
dotnet new webapi -n MyApi
dotnet new console -n MyApp
dotnet new classlib -n MyLibrary
```

### 패키지 관리
```bash
dotnet add package PackageName
dotnet remove package PackageName
dotnet list package
dotnet restore
```

### 빌드 및 실행
```bash
dotnet build
dotnet run
dotnet watch run
dotnet publish -c Release
```

### 테스트
```bash
dotnet test
dotnet test --filter "FullyQualifiedName~Test"
dotnet test --collect:"XPlat Code Coverage"
```

### Entity Framework
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
dotnet ef migrations remove
dotnet ef migrations list
```

### 도구
```bash
dotnet tool install -g ToolName
dotnet tool list -g
dotnet tool update -g ToolName
```

## npm과의 비교 요약

| 작업 | npm | dotnet |
|------|-----|--------|
| 프로젝트 초기화 | `npm init` | `dotnet new <template>` |
| 패키지 설치 | `npm install <package>` | `dotnet add package <package>` |
| 패키지 제거 | `npm uninstall <package>` | `dotnet remove package <package>` |
| 의존성 설치 | `npm install` | `dotnet restore` |
| 빌드 | `npm run build` | `dotnet build` |
| 실행 | `npm start` | `dotnet run` |
| 개발 모드 | `npm run dev` | `dotnet watch run` |
| 테스트 | `npm test` | `dotnet test` |
| 전역 도구 설치 | `npm install -g <tool>` | `dotnet tool install -g <tool>` |
| 버전 확인 | `npm --version` | `dotnet --version` |

이 참조 가이드는 일상적인 .NET 개발에서 필요한 대부분의 CLI 명령어를 다룹니다. 각 명령어의 상세한 옵션은 `dotnet <command> --help`를 통해 확인할 수 있습니다.
