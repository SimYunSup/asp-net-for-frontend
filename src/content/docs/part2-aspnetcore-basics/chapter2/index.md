---
title: "Chapter 3 - ASP.NET Core 소개와 개발 환경 설정"
---

# Chapter 3: ASP.NET Core 소개와 개발 환경 설정

## 3.1 ASP.NET Core란 무엇인가?

### .NET Framework에서 .NET Core로의 진화

웹 개발의 역사를 살펴보면, 기술은 항상 시대의 요구에 맞춰 진화해왔습니다. JavaScript가 단순한 브라우저 스크립팅 언어에서 Node.js를 통해 서버 사이드 플랫폼으로 확장된 것처럼, .NET 역시 근본적인 변화를 겪었습니다. 이 변화의 중심에 ASP.NET Core가 있습니다.

2002년, Microsoft는 .NET Framework와 함께 ASP.NET을 출시했습니다. 당시에는 혁신적인 기술이었지만, Windows에서만 실행되고 무거운 IIS(Internet Information Services) 웹 서버에 의존해야 한다는 근본적인 한계가 있었습니다. 프론트엔드 개발자인 여러분이 Node.js나 Python으로 웹 서버를 작성하면 어떤 운영체제에서든 실행할 수 있는 것과 대조적이었죠.

2016년, Microsoft는 이러한 한계를 극복하기 위해 ASP.NET Core를 완전히 새롭게 재설계했습니다. 이는 단순한 업데이트가 아니라, 지난 15년간의 웹 개발 경험을 바탕으로 처음부터 다시 만든 것입니다. 마치 Angular가 AngularJS에서 완전히 재작성된 것처럼, ASP.NET Core는 레거시 코드를 과감히 버리고 현대 웹 개발의 요구사항에 맞춰 재탄생했습니다.

### 크로스 플랫폼 웹 프레임워크의 의미

ASP.NET Core의 가장 큰 변화는 크로스 플랫폼 지원입니다. 이제 Windows, macOS, Linux 어디서든 ASP.NET Core 애플리케이션을 개발하고 실행할 수 있습니다. 이것이 프론트엔드 개발자에게 왜 중요할까요?

Node.js로 개발할 때를 떠올려보세요. MacBook에서 개발하고, Linux 서버에 배포하며, CI/CD 파이프라인은 Ubuntu 컨테이너에서 실행됩니다. 이 모든 것이 자연스럽게 작동하죠. ASP.NET Core도 이제 동일한 경험을 제공합니다. 로컬 Mac에서 `dotnet run`으로 개발 서버를 시작하고, Docker 컨테이너로 패키징하여, Kubernetes 클러스터에 배포할 수 있습니다.

더 나아가, ASP.NET Core는 자체 내장 웹 서버인 Kestrel을 포함합니다. Kestrel은 경량이면서도 고성능으로, TechEmpower 벤치마크에서 상위권을 차지할 정도로 빠릅니다. Express.js의 내장 서버처럼 개발 중에는 독립적으로 실행되고, 프로덕션에서는 Nginx나 Apache 같은 리버스 프록시 뒤에 배치할 수 있습니다.

### .NET 8 (LTS)과 .NET 9의 차이점

.NET은 매년 11월 새 버전을 출시하며, 짝수 버전(6, 8, 10...)은 LTS(Long Term Support)로 3년간 지원됩니다. 홀수 버전(7, 9, 11...)은 표준 지원으로 18개월간 지원됩니다. 이는 Node.js의 버전 정책과 유사합니다.

.NET 8은 2023년 11월에 출시된 LTS 버전으로, 프로덕션 환경에서 안정성이 검증되었습니다. 주요 특징으로는:

- **성능 향상**: JSON 직렬화 속도가 크게 개선되었고, 가비지 컬렉션 최적화로 메모리 사용량이 줄었습니다
- **Native AOT 지원**: 애플리케이션을 네이티브 바이너리로 컴파일하여 시작 시간을 극적으로 단축시킬 수 있습니다
- **새로운 Identity API**: 사용자 인증을 위한 최소한의 API 엔드포인트를 제공합니다

.NET 9는 2024년 11월에 출시되었으며, 다음과 같은 혁신적인 기능을 추가했습니다:

- **HybridCache**: 메모리 캐시와 분산 캐시를 통합한 새로운 캐싱 API로, cache stampede 문제를 자동으로 방지합니다
- **OpenAPI 내장 지원**: 별도 패키지 없이 Swagger 문서를 자동 생성할 수 있습니다
- **개선된 Rate Limiting**: 더 유연하고 강력한 요청 제한 기능을 제공합니다
- **Keyed Services**: 동일한 인터페이스의 여러 구현체를 키로 구분하여 등록할 수 있습니다

프로덕션 환경이라면 .NET 8 LTS를 권장하지만, 최신 기능을 실험하고 싶다면 .NET 9를 사용해도 좋습니다. 두 버전 간의 마이그레이션은 비교적 간단하며, 대부분의 코드가 호환됩니다.

### Express.js, NestJS와 비교: 왜 ASP.NET Core인가?

프론트엔드 개발자로서 이미 Node.js 생태계에 익숙하다면, "왜 ASP.NET Core를 배워야 하는가?"라는 질문을 가질 수 있습니다. 각 프레임워크를 비교해보겠습니다.

**Express.js**는 미니멀하고 유연한 프레임워크입니다. 빠르게 시작할 수 있고, 커뮤니티 생태계가 풍부하지만, 구조에 대한 강제가 거의 없어 프로젝트가 커질수록 일관성을 유지하기 어렵습니다. 미들웨어 체인, 라우팅, 에러 처리 등을 모두 직접 구성해야 합니다.

```javascript
// Express.js 예제
const express = require('express');
const app = express();

app.get('/api/users/:id', (req, res) => {
  const id = req.params.id;
  // 타입 안정성 없음, 런타임 오류 가능
  res.json({ id, name: 'John' });
});
```

**NestJS**는 Angular에서 영감을 받아 TypeScript로 작성된 프레임워크로, 의존성 주입, 데코레이터, 모듈 시스템을 제공합니다. 대규모 애플리케이션에 적합한 구조를 강제하며, ASP.NET Core와 많은 개념을 공유합니다.

```typescript
// NestJS 예제
@Controller('users')
export class UsersController {
  constructor(private usersService: UsersService) {}

  @Get(':id')
  findOne(@Param('id') id: string) {
    return this.usersService.findOne(id);
  }
}
```

**ASP.NET Core**는 NestJS의 구조적 장점을 가지면서도, 컴파일 언어인 C#의 성능과 타입 안정성을 제공합니다. 런타임 오류를 컴파일 타임에 잡을 수 있고, IDE의 강력한 IntelliSense 지원을 받습니다.

```csharp
// ASP.NET Core 예제
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _usersService.FindOneAsync(id);
        return user ?? NotFound();
    }
}
```

ASP.NET Core를 선택해야 하는 이유는 다음과 같습니다:

1. **성능**: Kestrel 웹 서버는 TechEmpower 벤치마크에서 Node.js보다 2-3배 빠른 처리량을 보여줍니다
2. **타입 안정성**: 컴파일 타임 타입 체킹으로 많은 버그를 사전에 방지합니다
3. **통합 생태계**: Entity Framework Core(ORM), Identity(인증), SignalR(실시간 통신) 등 모든 것이 표준화되어 있습니다
4. **엔터프라이즈 지원**: Microsoft의 공식 지원과 장기 지원(LTS) 정책으로 안정성이 보장됩니다
5. **현대적인 개발 경험**: Hot Reload, LINQ, async/await 등 개발자 경험이 뛰어납니다

물론 Node.js도 훌륭한 선택이며, JavaScript/TypeScript 전용 팀이라면 NestJS가 더 적합할 수 있습니다. 하지만 성능이 중요하거나, 강타입 시스템을 선호하거나, 대규모 엔터프라이즈 환경이라면 ASP.NET Core는 매우 강력한 대안입니다.

## 3.2 개발 환경 선택과 설정

### Visual Studio 2022 vs Rider vs VS Code: 각각의 장단점

ASP.NET Core 개발을 시작하려면 먼저 개발 환경을 선택해야 합니다. 프론트엔드 개발자에게 익숙한 VS Code부터 전문적인 IDE까지, 선택지가 다양합니다.

**Visual Studio 2022**는 Microsoft의 공식 통합 개발 환경입니다. Windows와 Mac 버전이 있으며, C# 개발을 위한 최고의 도구입니다. 강력한 디버거, 프로파일러, 데이터베이스 도구, 시각적 디자이너 등 모든 것이 포함되어 있습니다.

장점:
- 가장 강력한 IntelliSense와 코드 완성
- 통합된 디버깅 경험 (중단점, 조사식, 즉시 실행 창)
- Entity Framework Core 시각적 디자이너
- NuGet 패키지 관리 UI
- Azure 통합 배포 기능

단점:
- 무겁고 리소스를 많이 사용 (메모리 4GB 이상 권장)
- Community 에디션은 무료지만 Professional/Enterprise는 유료
- 시작 시간이 느림
- Windows에서 가장 잘 작동 (Mac 버전은 기능이 제한적)

**JetBrains Rider**는 유료 IDE이지만, 많은 개발자들이 최고의 크로스 플랫폼 .NET IDE로 꼽습니다. IntelliJ 플랫폼 기반으로, WebStorm이나 PyCharm을 사용해본 적이 있다면 익숙할 것입니다.

장점:
- 진정한 크로스 플랫폼 (Windows, Mac, Linux에서 동일한 경험)
- 빠르고 반응성이 좋음
- 뛰어난 리팩토링 도구
- 데이터베이스 도구 내장
- Docker, Kubernetes 통합
- Git 통합이 뛰어남

단점:
- 유료 (개인 라이선스 월 $14.90)
- 일부 Visual Studio 전용 기능 부족 (예: Visual Designers)
- 메모리 사용량이 적지 않음

**Visual Studio Code**는 프론트엔드 개발자에게 가장 익숙한 선택지입니다. 경량이고 빠르며, C# Dev Kit 확장을 통해 .NET 개발을 지원합니다.

장점:
- 가볍고 빠른 시작
- 프론트엔드 개발과 백엔드 개발을 하나의 에디터에서
- 무료이며 오픈소스
- 풍부한 확장 생태계
- 통합 터미널로 dotnet CLI 사용 용이
- Git 통합

단점:
- IDE만큼 강력한 IntelliSense는 아님
- 디버깅 경험이 덜 직관적
- 프로젝트 파일(.csproj) 편집이 번거로움
- 고급 리팩토링 도구 부족

프론트엔드 개발자라면 **VS Code + C# Dev Kit**으로 시작하는 것을 권장합니다. 익숙한 환경에서 학습 곡선을 줄일 수 있고, 나중에 필요하다면 Rider나 Visual Studio로 전환할 수 있습니다.

### VS Code + C# Dev Kit: 프론트엔드 개발자에게 익숙한 선택

VS Code로 ASP.NET Core 개발을 시작하는 것은 매우 간단합니다. 다음 확장을 설치하세요:

1. **C# Dev Kit** (Microsoft 공식): C# 언어 지원, IntelliSense, 디버깅을 제공합니다
2. **C#** (기본 언어 지원)
3. **REST Client** 또는 **Thunder Client**: API 테스트용
4. **NuGet Gallery**: NuGet 패키지 검색 및 설치

설치 후, VS Code는 자동으로 .NET SDK를 감지하고, `.csproj` 파일이 있는 프로젝트를 인식합니다. `Ctrl+Shift+P` (Mac: `Cmd+Shift+P`)를 눌러 명령 팔레트에서 ".NET"을 입력하면 다양한 명령을 사용할 수 있습니다.

디버깅은 `F5` 키로 시작할 수 있으며, VS Code는 자동으로 `.vscode/launch.json`과 `.vscode/tasks.json`을 생성합니다. 이는 프론트엔드 프로젝트에서 사용하는 것과 동일한 방식입니다.

```json
// .vscode/launch.json 예제
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net9.0/MyApp.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      }
    }
  ]
}
```

### .NET CLI 마스터하기: npm처럼 사용하기

프론트엔드 개발자에게 `npm`이나 `yarn`은 필수 도구입니다. .NET에서는 `dotnet` CLI가 동일한 역할을 합니다. 놀랍게도, 많은 명령이 직관적으로 대응됩니다.

**프로젝트 생성**:
```bash
# npm init
npm init

# dotnet equivalent
dotnet new webapi -n MyApi
dotnet new webapp -n MyWebApp
dotnet new blazorserver -n MyBlazorApp
```

**의존성 설치**:
```bash
# npm install express
npm install express

# dotnet equivalent
dotnet add package Microsoft.EntityFrameworkCore
```

**스크립트 실행**:
```bash
# npm run dev
npm run dev

# dotnet equivalent
dotnet run
dotnet watch run  # Hot Reload 포함
```

**빌드**:
```bash
# npm run build
npm run build

# dotnet equivalent
dotnet build
dotnet publish -c Release  # 프로덕션 빌드
```

**테스트 실행**:
```bash
# npm test
npm test

# dotnet equivalent
dotnet test
```

**전역 도구 설치**:
```bash
# npm install -g typescript
npm install -g typescript

# dotnet equivalent
dotnet tool install -g dotnet-ef  # Entity Framework CLI
dotnet tool install -g dotnet-aspnet-codegenerator
```

특히 `dotnet watch`는 파일 변경을 감지하여 자동으로 재시작하는데, Vite나 webpack-dev-server의 Hot Reload와 유사합니다. .NET 6부터는 코드를 변경하면 재컴파일 없이 즉시 반영되는 Hot Reload를 지원합니다.

자주 사용하는 명령어들:
- `dotnet new`: 새 프로젝트나 파일 생성
- `dotnet restore`: 의존성 복원 (보통 자동으로 실행됨)
- `dotnet build`: 프로젝트 빌드
- `dotnet run`: 애플리케이션 실행
- `dotnet watch`: 파일 변경 감지 실행
- `dotnet test`: 테스트 실행
- `dotnet publish`: 배포용 빌드
- `dotnet ef`: Entity Framework 마이그레이션

### 프로젝트 구조 이해: `.csproj` vs `package.json`

Node.js 프로젝트의 `package.json`은 의존성, 스크립트, 메타데이터를 정의합니다. .NET의 `.csproj` 파일은 이와 유사하지만 XML 형식입니다.

**package.json 예제**:
```json
{
  "name": "my-app",
  "version": "1.0.0",
  "scripts": {
    "dev": "vite",
    "build": "vite build"
  },
  "dependencies": {
    "express": "^4.18.0",
    "lodash": "^4.17.21"
  },
  "devDependencies": {
    "typescript": "^5.0.0"
  }
}
```

**.csproj 예제**:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>
</Project>
```

주요 차이점:
- **타겟 프레임워크**: `package.json`의 `engines` 필드와 유사하게, `<TargetFramework>`는 사용할 .NET 버전을 지정합니다
- **의존성**: `<PackageReference>`는 NuGet 패키지를 나타내며, npm 패키지와 동일한 개념입니다
- **스크립트 없음**: .NET CLI가 표준 명령(`run`, `build`, `test`)을 제공하므로 별도 스크립트가 필요 없습니다
- **자동 포함**: `.csproj`는 모든 `.cs` 파일을 자동으로 포함하므로 명시할 필요가 없습니다

프론트엔드 프로젝트와 달리 `.csproj` 파일을 직접 편집하는 경우는 드뭅니다. 대부분의 작업은 `dotnet` CLI로 수행할 수 있습니다:

```bash
# package.json에 의존성 추가
npm install lodash

# .csproj에 의존성 추가
dotnet add package Newtonsoft.Json
```

솔루션 파일(`.sln`)은 여러 프로젝트를 그룹화하는 컨테이너입니다. 프론트엔드 모노레포의 워크스페이스와 유사한 개념으로, 관련된 여러 프로젝트(API, 웹앱, 라이브러리 등)를 하나로 묶어 관리합니다.

## 3.3 첫 번째 ASP.NET Core 애플리케이션

### `dotnet new` 템플릿 탐색

프론트엔드에서 `create-react-app`, `create-next-app`, `vue create`로 프로젝트를 시작하듯, .NET에서는 `dotnet new` 명령을 사용합니다. 다양한 템플릿이 제공되며, 각각 특정 시나리오에 최적화되어 있습니다.

사용 가능한 템플릿 목록 보기:
```bash
dotnet new list
```

주요 웹 템플릿:
- **`webapi`**: RESTful API 프로젝트 (컨트롤러 기반)
- **`web`**: ASP.NET Core Empty (빈 프로젝트)
- **`webapp`**: Razor Pages 웹 애플리케이션
- **`mvc`**: MVC 패턴 웹 애플리케이션
- **`blazorserver`**: Blazor Server 앱
- **`blazorwasm`**: Blazor WebAssembly 앱
- **`react`**, **`angular`**, **`vue`**: SPA 통합 템플릿

첫 API 프로젝트 생성:
```bash
dotnet new webapi -n MyFirstApi
cd MyFirstApi
dotnet run
```

이 명령은 다음 구조의 프로젝트를 생성합니다:
```
MyFirstApi/
├── Controllers/
│   └── WeatherForecastController.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── MyFirstApi.csproj
```

### Program.cs: 애플리케이션의 진입점

`Program.cs`는 ASP.NET Core 애플리케이션의 진입점으로, Node.js의 `index.js` 또는 `server.js`와 유사합니다. .NET 6부터는 "최소 호스팅 모델"이 도입되어 코드가 매우 간결해졌습니다.

**전형적인 Program.cs**:
```csharp
var builder = WebApplication.CreateBuilder(args);

// 서비스 등록 (의존성 주입 컨테이너 설정)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 미들웨어 파이프라인 구성
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

이를 Express.js와 비교해보겠습니다:

**Express.js 예제**:
```javascript
const express = require('express');
const app = express();

// 미들웨어 등록
app.use(express.json());
app.use(express.static('public'));

// 라우트 정의
app.get('/api/users', (req, res) => {
  res.json([{ id: 1, name: 'John' }]);
});

// 서버 시작
app.listen(3000, () => {
  console.log('Server running on port 3000');
});
```

ASP.NET Core의 구조는 두 단계로 나뉩니다:

1. **Builder 단계** (`WebApplicationBuilder`): 서비스를 등록하고 애플리케이션을 구성합니다. 의존성 주입, 로깅, 구성 등을 설정합니다.

2. **App 단계** (`WebApplication`): 미들웨어 파이프라인을 구성하고 HTTP 요청 처리 방식을 정의합니다.

이 분리는 관심사를 명확히 구분합니다. Builder에서는 "무엇을 사용할 것인가"를 정의하고, App에서는 "어떻게 처리할 것인가"를 정의합니다.

### 개발 서버 실행: `dotnet run` vs `dotnet watch`

애플리케이션을 실행하는 방법은 두 가지입니다:

**dotnet run**: 한 번 실행하고 코드 변경 시 수동으로 재시작해야 합니다.
```bash
dotnet run
```

실행하면 다음과 같은 출력을 볼 수 있습니다:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

기본적으로 HTTP(5000)와 HTTPS(5001) 두 포트에서 실행됩니다. 브라우저에서 `https://localhost:5001/swagger`를 열면 자동 생성된 API 문서를 볼 수 있습니다.

**dotnet watch**: 파일 변경을 감지하여 자동으로 재시작하거나 Hot Reload를 적용합니다.
```bash
dotnet watch run
```

`dotnet watch`는 다음과 같은 변경을 감지합니다:
- `.cs` 파일 변경
- `.cshtml` 파일 변경 (Razor 뷰)
- `appsettings.json` 변경

.NET 6 이상에서는 대부분의 코드 변경이 Hot Reload로 즉시 반영됩니다. 메서드 내부 로직을 변경하면 재시작 없이 업데이트되지만, 새 클래스 추가나 구조적 변경은 재시작이 필요할 수 있습니다.

환경 변수로 포트를 변경할 수 있습니다:
```bash
# Linux/Mac
export ASPNETCORE_URLS="http://localhost:3000"
dotnet run

# Windows PowerShell
$env:ASPNETCORE_URLS="http://localhost:3000"
dotnet run
```

또는 `Properties/launchSettings.json`에서 설정할 수 있습니다:
```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:3000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### Hot Reload: Vite HMR과 비교

프론트엔드 개발자라면 Vite의 Hot Module Replacement(HMR)에 익숙할 것입니다. 코드를 저장하면 브라우저가 전체 페이지를 새로고침하지 않고 변경된 부분만 즉시 업데이트됩니다.

ASP.NET Core의 Hot Reload는 유사하지만 서버 사이드에서 작동합니다:

**Vite HMR (클라이언트 사이드)**:
- React 컴포넌트를 수정하면 해당 컴포넌트만 교체
- 상태가 유지됨
- 매우 빠름 (밀리초 단위)

**ASP.NET Core Hot Reload (서버 사이드)**:
- C# 메서드를 수정하면 해당 코드만 교체
- 실행 중인 요청은 영향받지 않음
- 새 요청부터 업데이트된 코드 실행
- 빠름 (1-2초)

Hot Reload가 지원되는 변경:
- ✅ 메서드 본문 수정
- ✅ 새 메서드 추가
- ✅ 속성 값 변경
- ✅ Lambda 표현식 수정
- ✅ Razor 뷰 수정

Hot Reload가 지원되지 않는 변경 (재시작 필요):
- ❌ 새 클래스 추가
- ❌ 새 의존성 추가
- ❌ Program.cs 변경 (미들웨어 파이프라인)
- ❌ 인터페이스 시그니처 변경

실제로 개발하면서 대부분의 변경은 Hot Reload로 처리되므로, 개발 경험이 매우 빠릅니다. Blazor 개발 시에는 서버와 클라이언트 양쪽에서 Hot Reload가 작동하여, 프론트엔드와 거의 동일한 경험을 제공합니다.

## 3.4 프로젝트 구조 해부

### Solution과 Project의 관계

Node.js에서는 보통 하나의 `package.json`이 하나의 애플리케이션을 나타냅니다. Monorepo를 사용한다면 여러 패키지를 워크스페이스로 관리하겠죠.

.NET에서는 이 구조가 명확하게 정의되어 있습니다:

- **Project** (`.csproj`): 빌드 가능한 단위로, npm 패키지에 해당합니다
- **Solution** (`.sln`): 여러 프로젝트를 그룹화하는 컨테이너로, 모노레포의 워크스페이스에 해당합니다

전형적인 솔루션 구조:
```
MySolution/
├── MySolution.sln
├── src/
│   ├── MySolution.Api/
│   │   ├── Controllers/
│   │   ├── Program.cs
│   │   └── MySolution.Api.csproj
│   ├── MySolution.Core/
│   │   ├── Models/
│   │   ├── Interfaces/
│   │   └── MySolution.Core.csproj
│   └── MySolution.Infrastructure/
│       ├── Data/
│       ├── Services/
│       └── MySolution.Infrastructure.csproj
└── tests/
    └── MySolution.Tests/
        ├── UnitTests/
        └── MySolution.Tests.csproj
```

이 구조는 Clean Architecture나 Domain-Driven Design을 따르는 경우 일반적입니다:

- **Api**: API 엔드포인트와 컨트롤러 (프레젠테이션 레이어)
- **Core**: 비즈니스 로직과 도메인 모델 (도메인 레이어)
- **Infrastructure**: 데이터베이스, 외부 서비스 (인프라 레이어)
- **Tests**: 단위 테스트와 통합 테스트

프로젝트 간 참조 관계:
```
Api → Core
Api → Infrastructure
Infrastructure → Core
Tests → Api, Core, Infrastructure
```

솔루션과 프로젝트 생성 예제:
```bash
# 솔루션 생성
dotnet new sln -n MySolution

# 프로젝트 생성
dotnet new webapi -n MySolution.Api -o src/MySolution.Api
dotnet new classlib -n MySolution.Core -o src/MySolution.Core
dotnet new classlib -n MySolution.Infrastructure -o src/MySolution.Infrastructure
dotnet new xunit -n MySolution.Tests -o tests/MySolution.Tests

# 솔루션에 프로젝트 추가
dotnet sln add src/MySolution.Api
dotnet sln add src/MySolution.Core
dotnet sln add src/MySolution.Infrastructure
dotnet sln add tests/MySolution.Tests

# 프로젝트 간 참조 추가
dotnet add src/MySolution.Api reference src/MySolution.Core
dotnet add src/MySolution.Api reference src/MySolution.Infrastructure
dotnet add src/MySolution.Infrastructure reference src/MySolution.Core
```

솔루션 레벨에서 빌드하면 모든 프로젝트가 올바른 순서로 빌드됩니다:
```bash
dotnet build  # 솔루션 디렉토리에서 실행
```

### 의존성 관리: NuGet vs npm

NuGet은 .NET의 패키지 관리자로, npm과 매우 유사합니다.

**패키지 설치**:
```bash
# npm
npm install lodash

# NuGet
dotnet add package Newtonsoft.Json
```

**특정 버전 설치**:
```bash
# npm
npm install lodash@4.17.21

# NuGet
dotnet add package Newtonsoft.Json --version 13.0.3
```

**패키지 제거**:
```bash
# npm
npm uninstall lodash

# NuGet
dotnet remove package Newtonsoft.Json
```

**패키지 업데이트**:
```bash
# npm
npm update

# NuGet
dotnet list package --outdated  # 오래된 패키지 확인
dotnet add package <PackageName>  # 최신 버전으로 업데이트
```

주요 차이점:

1. **중앙 저장소**: npm은 `node_modules` 폴더에 패키지를 저장하지만, NuGet은 전역 캐시(`~/.nuget/packages`)에 저장하여 디스크 공간을 절약합니다.

2. **Lock 파일**: npm은 `package-lock.json`을 사용하지만, .NET은 프로젝트 파일에 정확한 버전을 명시하고 빌드 시 `obj/project.assets.json`을 생성합니다.

3. **의존성 복원**: npm은 `npm install`로 의존성을 설치하지만, .NET은 `dotnet restore`를 사용합니다. 대부분의 경우 자동으로 실행되므로 명시적으로 호출할 필요가 없습니다.

4. **Private Registry**: npm은 `.npmrc`로 설정하고, NuGet은 `nuget.config`로 설정합니다.

**nuget.config 예제**:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="MyPrivateFeed" value="https://myfeed.company.com/v3/index.json" />
  </packageSources>
</configuration>
```

인기 있는 NuGet 패키지:
- **Entity Framework Core**: ORM (Prisma, TypeORM과 유사)
- **Swashbuckle.AspNetCore**: OpenAPI/Swagger 문서 생성
- **Serilog**: 구조화된 로깅
- **FluentValidation**: 유효성 검사
- **AutoMapper**: 객체 매핑
- **MediatR**: CQRS 패턴 구현
- **Polly**: 복원력 패턴 (재시도, 서킷 브레이커)
- **xUnit**: 단위 테스트 프레임워크

### 빌드 프로세스 이해하기

프론트엔드 빌드 프로세스는 보통 다음과 같습니다:
```
소스 코드 → Babel/TypeScript → Bundler (Webpack/Vite) → 최적화 → 배포 파일
```

.NET의 빌드 프로세스:
```
소스 코드 (.cs) → C# 컴파일러 (Roslyn) → IL (중간 언어) → JIT/AOT → 네이티브 코드
```

**빌드 구성**:
- **Debug**: 디버깅 정보 포함, 최적화 없음, 빠른 빌드
- **Release**: 최적화됨, 디버깅 정보 최소화, 프로덕션용

```bash
# Debug 빌드 (기본값)
dotnet build

# Release 빌드
dotnet build -c Release

# 게시 (배포용 빌드)
dotnet publish -c Release -o ./publish
```

빌드 출력 위치:
```
MyProject/
├── bin/
│   ├── Debug/
│   │   └── net9.0/
│   │       ├── MyProject.dll
│   │       ├── MyProject.pdb (디버그 심볼)
│   │       └── appsettings.json
│   └── Release/
│       └── net9.0/
└── obj/  (임시 빌드 파일)
```

`obj` 폴더는 Node.js의 `.cache` 폴더처럼 중간 빌드 결과물을 저장합니다. `.gitignore`에 포함되어야 합니다.

**빌드 최적화 옵션**:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>

    <!-- Release 빌드 최적화 -->
    <PublishTrimmed>true</PublishTrimmed>  <!-- 사용하지 않는 코드 제거 -->
    <PublishAot>true</PublishAot>  <!-- Native AOT 컴파일 -->

    <!-- 디버그 설정 -->
    <DebugType>portable</DebugType>
    <DebugSymbols>true</DebugSymbols>
  </PropertyGroup>
</Project>
```

### 디버깅 환경 구성

VS Code에서 디버깅을 시작하려면 `F5`를 누르거나 디버그 패널에서 "Start Debugging"을 선택합니다.

**.vscode/launch.json** 설정:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net9.0/MyApi.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "stopAtEntry": false,
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "http://localhost:5000"
      },
      "sourceFileMap": {
        "/Views": "${workspaceFolder}/Views"
      }
    },
    {
      "name": ".NET Core Attach",
      "type": "coreclr",
      "request": "attach"
    }
  ]
}
```

주요 기능:
- **중단점 (Breakpoints)**: 코드 라인 왼쪽을 클릭하여 설정
- **조사식 (Watch)**: 변수나 표현식의 값을 실시간으로 확인
- **호출 스택 (Call Stack)**: 현재 실행 경로 추적
- **즉시 실행 창 (Debug Console)**: 디버그 중 코드 실행

**조건부 중단점**:
중단점을 우클릭하여 조건을 설정할 수 있습니다.
```csharp
// 특정 조건에서만 중단
public IActionResult GetUser(int id)
{
    // id == 5일 때만 중단되도록 설정
    var user = _service.GetUser(id);
    return Ok(user);
}
```

**로그 포인트**:
중단점 대신 로그를 출력할 수 있습니다. 프로덕션 환경에서 유용합니다.

디버깅 팁:
- `Ctrl+Shift+P` → "Omnisharp: Select Project"로 디버그할 프로젝트 선택
- `F10`: Step Over (다음 라인)
- `F11`: Step Into (함수 내부로)
- `Shift+F11`: Step Out (함수 밖으로)
- `F5`: Continue (다음 중단점까지)

## 3.5 실습: "Hello World"에서 실제 API까지

### 간단한 REST API 엔드포인트 생성

이제 배운 내용을 실습해보겠습니다. 간단한 할 일(Todo) API를 만들어봅시다.

**1단계: 프로젝트 생성**
```bash
dotnet new webapi -n TodoApi
cd TodoApi
dotnet watch run
```

**2단계: 모델 정의**

`Models/Todo.cs` 파일을 생성합니다:
```csharp
namespace TodoApi.Models;

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

이는 TypeScript의 인터페이스와 유사합니다:
```typescript
interface Todo {
  id: number;
  title: string;
  isCompleted: boolean;
  createdAt: Date;
}
```

**3단계: 컨트롤러 생성**

`Controllers/TodosController.cs`:
```csharp
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    // 간단한 인메모리 저장소
    private static readonly List<Todo> Todos = new();
    private static int _nextId = 1;

    // GET: api/todos
    [HttpGet]
    public ActionResult<IEnumerable<Todo>> GetAll()
    {
        return Ok(Todos);
    }

    // GET: api/todos/5
    [HttpGet("{id}")]
    public ActionResult<Todo> GetById(int id)
    {
        var todo = Todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
        {
            return NotFound(new { message = $"Todo with id {id} not found" });
        }
        return Ok(todo);
    }

    // POST: api/todos
    [HttpPost]
    public ActionResult<Todo> Create(CreateTodoDto dto)
    {
        var todo = new Todo
        {
            Id = _nextId++,
            Title = dto.Title,
            IsCompleted = false
        };

        Todos.Add(todo);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    // PUT: api/todos/5
    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateTodoDto dto)
    {
        var todo = Todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
        {
            return NotFound();
        }

        todo.Title = dto.Title;
        todo.IsCompleted = dto.IsCompleted;

        return NoContent();
    }

    // DELETE: api/todos/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var todo = Todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
        {
            return NotFound();
        }

        Todos.Remove(todo);
        return NoContent();
    }
}

// DTO (Data Transfer Objects)
public record CreateTodoDto(string Title);
public record UpdateTodoDto(string Title, bool IsCompleted);
```

Express.js로 작성하면:
```javascript
app.get('/api/todos', (req, res) => {
  res.json(todos);
});

app.get('/api/todos/:id', (req, res) => {
  const todo = todos.find(t => t.id === parseInt(req.params.id));
  if (!todo) {
    return res.status(404).json({ message: 'Not found' });
  }
  res.json(todo);
});

app.post('/api/todos', (req, res) => {
  const todo = {
    id: nextId++,
    title: req.body.title,
    isCompleted: false,
    createdAt: new Date()
  };
  todos.push(todo);
  res.status(201).json(todo);
});
```

ASP.NET Core의 장점:
- **타입 안정성**: 컴파일 타임에 모든 타입 오류를 잡습니다
- **자동 바인딩**: `[FromBody]`, `[FromRoute]` 없이도 자동으로 요청 데이터를 바인딩합니다
- **상태 코드 헬퍼**: `Ok()`, `NotFound()`, `CreatedAtAction()` 등으로 명확하게 응답 생성
- **OpenAPI 자동 생성**: Swagger가 자동으로 API 문서를 만듭니다

### Postman/Thunder Client로 테스트

VS Code에서 Thunder Client 확장을 설치하거나, REST Client 확장을 사용할 수 있습니다.

**REST Client 예제** (`test.http` 파일):
```http
@baseUrl = https://localhost:5001

### Get all todos
GET {{baseUrl}}/api/todos

### Get todo by id
GET {{baseUrl}}/api/todos/1

### Create new todo
POST {{baseUrl}}/api/todos
Content-Type: application/json

{
  "title": "Learn ASP.NET Core"
}

### Update todo
PUT {{baseUrl}}/api/todos/1
Content-Type: application/json

{
  "title": "Master ASP.NET Core",
  "isCompleted": true
}

### Delete todo
DELETE {{baseUrl}}/api/todos/1
```

VS Code에서 이 파일을 열고 각 요청 위의 "Send Request"를 클릭하면 실행됩니다.

**Swagger UI 사용**:
브라우저에서 `https://localhost:5001/swagger`를 열면 자동 생성된 API 문서를 볼 수 있습니다. 여기서 직접 API를 테스트할 수도 있습니다.

### 에러 처리 기초

실제 애플리케이션에서는 에러 처리가 중요합니다. ASP.NET Core는 개발 환경과 프로덕션 환경에서 다르게 에러를 처리합니다.

**Program.cs에 에러 처리 미들웨어 추가**:
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 개발 환경: 상세한 에러 정보
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();  // 상세한 예외 페이지
}
else
{
    // 프로덕션: 일반적인 에러 응답
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 에러 처리 엔드포인트
app.MapGet("/error", () => Results.Problem("An error occurred"));

app.Run();
```

**글로벌 예외 처리**:
커스텀 미들웨어를 만들어 모든 예외를 일관되게 처리할 수 있습니다.

`Middleware/GlobalExceptionHandler.cs`:
```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred",
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
```

`Program.cs`에 등록:
```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ...

app.UseExceptionHandler();  // 미들웨어로 추가
```

이제 예외가 발생하면 일관된 JSON 응답이 반환됩니다:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "An error occurred",
  "status": 500,
  "detail": "Object reference not set to an instance of an object."
}
```

**유효성 검사 에러**:
ASP.NET Core는 자동으로 모델 유효성 검사를 수행하고, 실패 시 400 Bad Request를 반환합니다.

```csharp
public record CreateTodoDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; init; } = string.Empty;
}
```

유효성 검사 실패 시 자동 응답:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "Title is required"
    ]
  }
}
```

---

## Chapter 3 마무리: 첫 번째 API의 완성

축하합니다! Chapter 3을 완료하며 첫 번째 ASP.NET Core API를 만들었습니다. .NET SDK 설치부터 시작해서, `dotnet new webapi`로 프로젝트를 생성하고, Hot Reload로 빠르게 개발하며, Swagger로 API를 문서화하고, 에러 처리까지 구현했습니다. 이제 여러분은 간단하지만 완전히 동작하는 웹 API를 만들 수 있습니다.

하지만 "Hello World"를 넘어 **프로덕션급 애플리케이션**을 만들려면, ASP.NET Core의 내부 작동 방식을 이해해야 합니다. Chapter 3에서 우리는 `builder.Build()`와 `app.Run()`을 사용했지만, 그 사이에서 무슨 일이 일어나는지는 깊이 다루지 않았습니다. 미들웨어는 어떻게 작동하나요? 의존성 주입은 어떻게 구현되나요? 요청이 어떻게 올바른 엔드포인트로 라우팅되나요?

### 다음 단계: 프레임워크의 심장부로

**[Chapter 4: ASP.NET Core의 핵심 아키텍처](../chapter3/index.md)** 에서는 프레임워크의 내부 동작을 깊이 파고듭니다. 단순히 "이렇게 쓴다"가 아니라 "왜 이렇게 설계되었는가"를 이해합니다.

**미들웨어 파이프라인: Express.js를 넘어서**: Express.js의 미들웨어를 사용해봤다면 개념은 익숙합니다. 하지만 ASP.NET Core의 미들웨어는 더욱 정교합니다. 요청과 응답을 모두 가로챌 수 있고, 조건부로 파이프라인을 분기하며, 타입 안전성을 보장합니다. 커스텀 미들웨어로 요청 ID 추적, 성능 측정, 전역 에러 처리를 구현하면서, Express의 `app.use()`가 ASP.NET Core에서 어떻게 발전했는지 봅니다.

**의존성 주입: Angular의 DI를 서버에서**: Angular의 의존성 주입 시스템을 좋아한다면, ASP.NET Core의 DI는 더 마음에 들 것입니다. `Transient`, `Scoped`, `Singleton` 라이프타임으로 서비스를 정밀하게 제어하고, 인터페이스 기반 설계로 테스트 가능한 코드를 작성합니다. `@Injectable()`에서 `services.AddScoped<T>()`로, 프론트엔드의 모범 사례가 백엔드에서 어떻게 구현되는지 배웁니다.

**라우팅의 마법**: React Router의 경로 매칭, Vue Router의 동적 라우트—이 모든 개념이 ASP.NET Core에도 있습니다. 하지만 서버 라우팅은 클라이언트와 다른 고려사항이 있습니다. 라우트 제약 조건, 라우트 값, 엔드포인트 메타데이터를 활용하여 복잡한 API 구조를 우아하게 처리합니다.

**구성 관리: .env를 넘어서**: Node.js의 `.env` 파일은 간단하지만 한계가 있습니다. ASP.NET Core는 `appsettings.json`, 환경 변수, 사용자 시크릿, Azure Key Vault 등 여러 구성 소스를 계층적으로 결합합니다. 개발 환경에서는 로컬 설정을, 프로덕션에서는 환경 변수를, 민감한 데이터는 Key Vault에서—모두 동일한 인터페이스로 접근합니다.

**구조화된 로깅: console.log의 진화**: `console.log()`는 개발에 유용하지만, 프로덕션 모니터링에는 부족합니다. ASP.NET Core의 `ILogger`는 로그 레벨, 구조화된 데이터, 필터링, 다양한 출력 대상(콘솔, 파일, Application Insights, Elasticsearch)을 지원합니다. 로그를 단순한 텍스트가 아닌 쿼리 가능한 데이터로 다룹니다.

Chapter 4를 마치면, 여러분은 ASP.NET Core가 단순한 웹 프레임워크가 아닌, 엔터프라이즈급 애플리케이션을 위한 완전한 플랫폼임을 이해하게 됩니다. 그리고 이 모든 강력한 기능을 Chapter 5의 Minimal APIs에서 간결하게 활용하는 방법을 배웁니다.

준비되셨나요? [Chapter 4로 이동하세요!](../chapter3/index.md)

---

## 추가 학습 리소스

- [ASP.NET Core 공식 문서](https://docs.microsoft.com/aspnet/core)
- [dotnet CLI 가이드](https://docs.microsoft.com/dotnet/core/tools/)
- [Swagger/OpenAPI 가이드](https://docs.microsoft.com/aspnet/core/tutorials/web-api-help-pages-using-swagger)
- [Hot Reload 가이드](https://docs.microsoft.com/dotnet/core/tools/dotnet-watch)
