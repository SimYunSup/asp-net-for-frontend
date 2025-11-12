---
title: "Appendix E - 개발 도구 설정 가이드"
---

# Appendix E: 개발 도구 설정 가이드

프론트엔드 개발자를 위한 .NET 개발 환경 설정 가이드입니다. VS Code, Visual Studio, Rider 등 주요 IDE의 설정과 유용한 확장을 다룹니다.

## 1. Visual Studio Code

### 1.1 필수 확장 프로그램

```bash
# C# 개발
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.csdevkit
code --install-extension ms-dotnettools.vscode-dotnet-runtime

# .NET 관련
code --install-extension ms-dotnettools.vscode-dotnet-pack

# IntelliCode
code --install-extension VisualStudioExptTeam.vscodeintellicode

# NuGet 패키지 관리
code --install-extension jmrog.vscode-nuget-package-manager

# EditorConfig
code --install-extension EditorConfig.EditorConfig

# GitLens
code --install-extension eamodio.gitlens
```

**npm script 비교**
```json
{
  "devDependencies": {
    "eslint": "^8.0.0",
    "prettier": "^3.0.0",
    "@typescript-eslint/parser": "^6.0.0"
  }
}
```

### 1.2 settings.json 설정

**위치**: `.vscode/settings.json`

```json
{
  // C# 포맷팅
  "omnisharp.enableEditorConfigSupport": true,
  "omnisharp.enableRoslynAnalyzers": true,
  "omnisharp.organizeImportsOnFormat": true,

  // 파일 저장 시 자동 포맷
  "editor.formatOnSave": true,
  "editor.codeActionsOnSave": {
    "source.fixAll": true,
    "source.organizeImports": true
  },

  // C# 관련
  "[csharp]": {
    "editor.defaultFormatter": "ms-dotnettools.csharp",
    "editor.tabSize": 4,
    "editor.insertSpaces": true
  },

  // 자동 저장
  "files.autoSave": "onFocusChange",

  // 제외 파일
  "files.exclude": {
    "**/bin": true,
    "**/obj": true,
    "**/.vs": true
  },

  // 탐색기 제외
  "search.exclude": {
    "**/bin": true,
    "**/obj": true
  },

  // IntelliSense
  "editor.suggest.snippetsPreventQuickSuggestions": false,
  "editor.suggestSelection": "first",

  // 터미널
  "terminal.integrated.defaultProfile.windows": "PowerShell",
  "terminal.integrated.defaultProfile.linux": "bash",
  "terminal.integrated.defaultProfile.osx": "zsh"
}
```

### 1.3 tasks.json (빌드 작업)

**위치**: `.vscode/tasks.json`

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "${workspaceFolder}/MyApi.csproj",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile",
      "group": {
        "kind": "build",
        "isDefault": true
      }
    },
    {
      "label": "publish",
      "command": "dotnet",
      "type": "process",
      "args": [
        "publish",
        "${workspaceFolder}/MyApi.csproj",
        "-c",
        "Release",
        "-o",
        "${workspaceFolder}/publish"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "watch",
      "command": "dotnet",
      "type": "process",
      "args": [
        "watch",
        "run",
        "--project",
        "${workspaceFolder}/MyApi.csproj"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "test",
      "command": "dotnet",
      "type": "process",
      "args": [
        "test",
        "${workspaceFolder}/MyApi.Tests.csproj"
      ],
      "problemMatcher": "$msCompile"
    }
  ]
}
```

**package.json scripts 비교**
```json
{
  "scripts": {
    "build": "tsc",
    "watch": "tsc --watch",
    "test": "jest",
    "start": "node dist/index.js"
  }
}
```

### 1.4 launch.json (디버깅)

**위치**: `.vscode/launch.json`

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net8.0/MyApi.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "sourceFileMap": {
        "/Views": "${workspaceFolder}/Views"
      }
    },
    {
      "name": ".NET Core Attach",
      "type": "coreclr",
      "request": "attach"
    },
    {
      "name": ".NET Core Launch (console)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net8.0/MyConsoleApp.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "console": "internalConsole",
      "stopAtEntry": false
    }
  ]
}
```

### 1.5 유용한 단축키

| 작업 | Windows/Linux | macOS | VS Code 명령 |
|------|--------------|-------|-------------|
| 빌드 | `Ctrl+Shift+B` | `Cmd+Shift+B` | Tasks: Run Build Task |
| 디버깅 시작 | `F5` | `F5` | Debug: Start Debugging |
| 디버깅 중지 | `Shift+F5` | `Shift+F5` | Debug: Stop |
| 중단점 토글 | `F9` | `F9` | Debug: Toggle Breakpoint |
| 정의로 이동 | `F12` | `F12` | Go to Definition |
| 참조 찾기 | `Shift+F12` | `Shift+F12` | Find All References |
| 이름 바꾸기 | `F2` | `F2` | Rename Symbol |
| 빠른 수정 | `Ctrl+.` | `Cmd+.` | Quick Fix |
| 코드 포맷 | `Shift+Alt+F` | `Shift+Option+F` | Format Document |

## 2. Visual Studio (Windows/macOS)

### 2.1 필수 워크로드

**Visual Studio Installer에서 설치**:
- ASP.NET 및 웹 개발
- .NET 데스크톱 개발
- Azure 개발 (클라우드 배포 시)
- Node.js 개발 (프론트엔드 통합 시)

### 2.2 유용한 확장 프로그램

```
- ReSharper (유료, 강력한 코드 분석)
- CodeMaid (코드 정리)
- Productivity Power Tools
- Visual Studio IntelliCode
- Web Essentials
- GitFlow
- Markdown Editor
```

### 2.3 설정 권장사항

**도구 → 옵션**

```
텍스트 편집기 → C# → 코드 스타일 → 일반
  - 저장 시 자동 포맷: 활성화
  - 붙여넣기 시 자동 포맷: 활성화
  - using 문 자동 정렬: 활성화

텍스트 편집기 → C# → IntelliSense
  - 완료 후 자동으로 세미콜론 삽입: 활성화
  - 완료 후 자동으로 중괄호 삽입: 활성화

프로젝트 및 솔루션 → 빌드 및 실행
  - 병렬 프로젝트 빌드의 최대 수: CPU 코어 수

디버깅 → 일반
  - 내 코드만 디버그 사용: 비활성화 (외부 라이브러리 디버깅 가능)
  - 소스 서버 지원 사용: 활성화
```

### 2.4 라이브 단위 테스트

```
테스트 → Live Unit Testing → 시작

실시간으로 테스트 실행 결과를 코드 옆에 표시:
✓ 녹색: 테스트 통과
✗ 빨간색: 테스트 실패
- 회색: 커버되지 않음
```

### 2.5 코드 스니펫

**자주 사용하는 스니펫**:
- `ctor` → 생성자
- `prop` → 자동 프로퍼티
- `propfull` → 전체 프로퍼티 (backing field)
- `if` → if 문
- `for` → for 루프
- `foreach` → foreach 루프
- `try` → try-catch 블록
- `class` → 클래스 정의
- `interface` → 인터페이스 정의

**사용자 정의 스니펫 생성**: 도구 → 코드 조각 관리자

## 3. JetBrains Rider

### 3.1 필수 플러그인

```
- .ignore (gitignore 지원)
- GitToolBox
- Rainbow Brackets
- Key Promoter X (단축키 학습)
- String Manipulation
- Material Theme UI
```

### 3.2 설정 권장사항

**File → Settings (Ctrl+Alt+S)**

```
Editor → Code Style → C#
  - 들여쓰기: 4 spaces
  - 줄 바꿈: 120
  - using 정렬: 활성화

Editor → Inspections
  - 모든 C# 검사 활성화
  - 심각도 수준: Warning 이상

Build, Execution, Deployment → Toolset and Build
  - Use MSBuild version: .NET SDK

Tools → Actions on Save
  - Reformat code: 활성화
  - Optimize imports: 활성화
  - Run code cleanup: 활성화
```

### 3.3 유용한 단축키

| 작업 | Windows/Linux | macOS |
|------|--------------|-------|
| 어디서나 검색 | `Shift Shift` | `Shift Shift` |
| 빠른 수정 | `Alt+Enter` | `Option+Enter` |
| 리팩터링 | `Ctrl+Shift+R` | `Cmd+Shift+R` |
| 파일 찾기 | `Ctrl+Shift+N` | `Cmd+Shift+O` |
| 타입 찾기 | `Ctrl+N` | `Cmd+O` |
| 최근 파일 | `Ctrl+E` | `Cmd+E` |
| 구현으로 이동 | `Ctrl+F12` | `Cmd+F12` |
| 테스트 실행 | `Ctrl+T, R` | `Cmd+T, R` |
| 디버깅 | `F5` | `F5` |

## 4. .editorconfig (코드 스타일 통일)

**위치**: 프로젝트 루트 `.editorconfig`

```ini
# 최상위 EditorConfig 파일
root = true

# 모든 파일
[*]
charset = utf-8
indent_style = space
indent_size = 4
insert_final_newline = true
trim_trailing_whitespace = true

# C# 파일
[*.cs]
# 들여쓰기
indent_size = 4
tab_width = 4

# 새 줄 설정
end_of_line = crlf
insert_final_newline = true

# 코드 스타일 규칙
csharp_prefer_braces = true:warning
csharp_prefer_simple_using_statement = true:suggestion
csharp_style_namespace_declarations = file_scoped:warning
csharp_style_prefer_method_group_conversion = true:silent
csharp_style_expression_bodied_methods = false:silent
csharp_style_expression_bodied_constructors = false:silent
csharp_style_expression_bodied_operators = false:silent
csharp_style_expression_bodied_properties = true:silent
csharp_style_expression_bodied_indexers = true:silent
csharp_style_expression_bodied_accessors = true:silent

# var 사용
csharp_style_var_for_built_in_types = false:silent
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = false:silent

# 네이밍 규칙
dotnet_naming_rule.interface_should_be_begins_with_i.severity = warning
dotnet_naming_rule.interface_should_be_begins_with_i.symbols = interface
dotnet_naming_rule.interface_should_be_begins_with_i.style = begins_with_i

dotnet_naming_rule.types_should_be_pascal_case.severity = warning
dotnet_naming_rule.types_should_be_pascal_case.symbols = types
dotnet_naming_rule.types_should_be_pascal_case.style = pascal_case

dotnet_naming_rule.non_field_members_should_be_pascal_case.severity = warning
dotnet_naming_rule.non_field_members_should_be_pascal_case.symbols = non_field_members
dotnet_naming_rule.non_field_members_should_be_pascal_case.style = pascal_case

# 심볼 정의
dotnet_naming_symbols.interface.applicable_kinds = interface
dotnet_naming_symbols.interface.applicable_accessibilities = public, internal, private, protected, protected_internal, private_protected

dotnet_naming_symbols.types.applicable_kinds = class, struct, interface, enum
dotnet_naming_symbols.types.applicable_accessibilities = public, internal, private, protected, protected_internal, private_protected

dotnet_naming_symbols.non_field_members.applicable_kinds = property, event, method
dotnet_naming_symbols.non_field_members.applicable_accessibilities = public, internal, private, protected, protected_internal, private_protected

# 스타일 정의
dotnet_naming_style.pascal_case.capitalization = pascal_case

dotnet_naming_style.begins_with_i.required_prefix = I
dotnet_naming_style.begins_with_i.capitalization = pascal_case

# JSON 파일
[*.json]
indent_size = 2

# YAML 파일
[*.{yml,yaml}]
indent_size = 2

# XML 프로젝트 파일
[*.{csproj,vbproj,vcxproj,vcxproj.filters,proj,projitems,shproj}]
indent_size = 2
```

**.prettierrc 비교**
```json
{
  "semi": true,
  "trailingComma": "es5",
  "singleQuote": true,
  "printWidth": 100,
  "tabWidth": 2
}
```

## 5. 코드 분석 도구

### 5.1 StyleCop Analyzers

```bash
dotnet add package StyleCop.Analyzers
```

**설정**: `stylecop.json`

```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json",
  "settings": {
    "documentationRules": {
      "companyName": "MyCompany",
      "copyrightText": "Copyright (c) {companyName}. All rights reserved.",
      "documentInterfaces": true,
      "documentExposedElements": true,
      "documentInternalElements": false,
      "documentPrivateElements": false
    },
    "orderingRules": {
      "usingDirectivesPlacement": "outsideNamespace",
      "systemUsingDirectivesFirst": true
    },
    "namingRules": {
      "allowCommonHungarianPrefixes": false
    }
  }
}
```

**프로젝트 파일에 추가**:
```xml
<ItemGroup>
  <AdditionalFiles Include="stylecop.json" />
</ItemGroup>
```

**ESLint 비교**
```json
{
  "extends": ["eslint:recommended", "plugin:@typescript-eslint/recommended"],
  "rules": {
    "semi": ["error", "always"],
    "quotes": ["error", "single"]
  }
}
```

### 5.2 SonarAnalyzer

```bash
dotnet add package SonarAnalyzer.CSharp
```

## 6. Git 설정

### 6.1 .gitignore (.NET 프로젝트)

```gitignore
# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Aa]rm/
[Aa]rm64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio cache/options
.vs/
.vscode/

# Rider
.idea/

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# NuGet Packages
*.nupkg
*.snupkg
**/packages/*
!**/packages/build/
*.nuget.props
*.nuget.targets

# User secrets
appsettings.Development.json
appsettings.*.json
!appsettings.json

# Database
*.db
*.db-shm
*.db-wal

# Migration scripts (optional)
# Migrations/

# Publish
publish/
```

### 6.2 .gitattributes

```.gitattributes
* text=auto

*.cs text diff=csharp
*.csproj text diff=csharp
*.sln text eol=crlf

*.sh text eol=lf

*.png binary
*.jpg binary
*.gif binary
*.dll binary
*.exe binary
```

## 7. Docker 개발 환경

### 7.1 Dockerfile

```dockerfile
# 개발 환경
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
WORKDIR /app
COPY . .
RUN dotnet restore
CMD ["dotnet", "watch", "run"]

# 프로덕션 빌드
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MyApi/MyApi.csproj", "MyApi/"]
RUN dotnet restore "MyApi/MyApi.csproj"
COPY . .
WORKDIR "/src/MyApi"
RUN dotnet build "MyApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyApi.csproj" -c Release -o /app/publish

# 런타임
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyApi.dll"]
```

### 7.2 docker-compose.yml

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      target: development
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=MyDb;User=sa;Password=YourStrong@Passw0rd;
    volumes:
      - .:/app
      - /app/bin
      - /app/obj
    depends_on:
      - db
      - redis

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redisdata:/data

volumes:
  sqldata:
  redisdata:
```

## 8. CI/CD 템플릿

### 8.1 GitHub Actions

**.github/workflows/dotnet.yml**

```yaml
name: .NET CI/CD

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 8.0.x

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --configuration Release --no-restore

    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"

    - name: Code Coverage Report
      uses: codecov/codecov-action@v3
      with:
        files: '**/coverage.cobertura.xml'

    - name: Publish
      run: dotnet publish -c Release -o ./publish

    - name: Upload artifact
      uses: actions/upload-artifact@v3
      with:
        name: dotnet-app
        path: ./publish
```

**package.json scripts 비교**
```yaml
name: Node.js CI

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    - uses: actions/setup-node@v4
      with:
        node-version: 20
    - run: npm ci
    - run: npm run build
    - run: npm test
```

## 9. 유용한 터미널 도구

### 9.1 dotnet-script (C# REPL)

```bash
dotnet tool install -g dotnet-script

# C# 스크립트 실행
dotnet script myScript.csx

# REPL 모드
dotnet script
> var x = 10;
> Console.WriteLine(x);
```

### 9.2 dotnet-outdated

```bash
dotnet tool install -g dotnet-outdated-tool

# 오래된 패키지 확인
dotnet outdated

# 자동 업데이트
dotnet outdated -u
```

### 9.3 dotnet-format

```bash
dotnet tool install -g dotnet-format

# 코드 포맷팅
dotnet format

# 검증만
dotnet format --verify-no-changes
```

## 10. 성능 프로파일링

### 10.1 Visual Studio Profiler

```
디버그 → 성능 프로파일러 (Alt+F2)

분석 도구:
- CPU 사용량
- 메모리 사용량
- .NET 개체 할당 추적
- 데이터베이스 쿼리
- 파일 I/O
```

### 10.2 dotnet-trace

```bash
dotnet tool install -g dotnet-trace

# 추적 수집
dotnet trace collect --process-id <PID>

# Speedscope로 변환
dotnet trace convert trace.nettrace --format Speedscope
```

## 요약

이 가이드는 .NET 개발 환경 설정의 모든 측면을 다룹니다:

1. **IDE 설정**: VS Code, Visual Studio, Rider
2. **코드 스타일**: .editorconfig, StyleCop
3. **Git 설정**: .gitignore, .gitattributes
4. **컨테이너**: Docker, docker-compose
5. **CI/CD**: GitHub Actions 템플릿
6. **도구**: 코드 분석, 포맷팅, 프로파일링

프론트엔드 개발자가 익숙한 ESLint, Prettier, package.json scripts와 유사한 .NET 도구들을 비교하여 빠르게 적응할 수 있도록 구성했습니다.
