# Chapter 24: 컨테이너화와 Docker - 모든 환경에서 동일하게 실행하기

## "내 컴퓨터에서는 잘 돌아가는데요" - 이 악몽의 종말

프로그래밍에서 가장 악명 높은 문장 중 하나입니다. "내 컴퓨터에서는 잘 돌아가는데요." 개발자의 노트북에서는 완벽하게 작동하던 애플리케이션이 스테이징 서버에서는 이상하게 동작하고, 프로덕션에서는 아예 실행조차 되지 않습니다. 며칠간의 디버깅 끝에 원인을 찾아냅니다: 프로덕션 서버의 .NET 버전이 약간 달랐습니다. 또는 누군가 시스템 라이브러리를 업데이트했습니다. 또는 환경 변수가 설정되지 않았습니다.

이 문제는 새로운 것이 아닙니다. 소프트웨어 배포는 항상 환경 간의 차이로 고통받아 왔습니다. 1990년대에는 "dll hell"이 있었습니다—동일한 DLL의 다른 버전이 시스템에 공존하며 예측할 수 없는 동작을 일으켰습니다. 2000년대에는 의존성 지옥이 있었습니다—한 애플리케이션은 Python 2.7을 요구하고, 다른 것은 Python 3.6을 요구하며, 둘 다 같은 서버에서 실행되어야 했습니다.

가상 머신이 부분적인 해결책이었습니다. 각 애플리케이션이 자신만의 VM에서 실행되며, 완전히 격리되었습니다. 하지만 VM은 무겁습니다. 전체 OS를 부팅해야 하므로 시작 시간이 몇 분이 걸리고, 각 VM은 수 GB의 디스크와 메모리를 소비합니다. 그리고 VM 이미지를 관리하는 것은 여전히 복잡합니다.

**Docker는 이 모든 것을 바꿨습니다.** 2013년 Docker가 등장한 이후, 애플리케이션 배포의 패러다임이 완전히 전환되었습니다. Docker 컨테이너는 VM의 격리성을 제공하면서도, 훨씬 가볍고 빠르며 관리하기 쉽습니다. 이제 개발자의 노트북, CI 서버, 스테이징 환경, 프로덕션 클러스터에서 **완전히 동일한 컨테이너**가 실행됩니다. 환경 차이는 사라집니다.

프론트엔드 개발자로서 여러분은 Node.js 애플리케이션을 Docker로 컨테이너화해본 경험이 있을 것입니다. Dockerfile을 작성하고, `docker build`로 이미지를 만들며, `docker run`으로 실행합니다. ASP.NET Core의 컨테이너화는 놀라울 정도로 유사합니다. 하지만 .NET 특유의 최적화 기회와 모범 사례가 있으며, 이 챕터에서 모두 다룹니다.

## Docker 기초: 컨테이너가 VM과 다른 이유

Docker를 이해하려면, 먼저 컨테이너와 가상 머신의 차이를 명확히 해야 합니다.

**가상 머신 (VM):**

VM은 하이퍼바이저를 통해 물리적 하드웨어를 가상화합니다. 각 VM은 **전체 게스트 OS**를 포함합니다—커널, 시스템 라이브러리, 유틸리티 등. VM이 부팅되면, 실제 컴퓨터처럼 OS 부팅 과정을 거칩니다. 이는 강력한 격리를 제공하지만, 큰 비용이 듭니다:

- **무거움**: 각 VM은 수 GB의 디스크 공간을 차지합니다.
- **느린 시작**: 부팅에 몇 분이 걸립니다.
- **리소스 오버헤드**: 각 게스트 OS가 메모리와 CPU를 소비합니다.

```
+---------------------------------------+
| Host OS                               |
|  +-----------------------------------+|
|  | Hypervisor (VMware, VirtualBox)   ||
|  |  +----------+    +----------+     ||
|  |  | Guest OS |    | Guest OS |     ||
|  |  |  App     |    |  App     |     ||
|  |  +----------+    +----------+     ||
|  +-----------------------------------+|
+---------------------------------------+
| Physical Hardware                     |
+---------------------------------------+
```

**컨테이너:**

컨테이너는 OS 수준에서 가상화합니다. 모든 컨테이너가 **호스트 OS의 커널을 공유**합니다. 각 컨테이너는 자신만의 파일 시스템, 프로세스 공간, 네트워크 인터페이스를 가지지만, 커널은 하나입니다. 이는 극적인 이점을 제공합니다:

- **가벼움**: 컨테이너 이미지는 MB 단위이며, 필요한 것만 포함합니다.
- **빠른 시작**: 밀리초 안에 시작됩니다. OS 부팅이 없습니다.
- **효율성**: 수십, 수백 개의 컨테이너를 동일한 호스트에서 실행할 수 있습니다.

```
+---------------------------------------+
| Host OS                               |
|  +-----------------------------------+|
|  | Docker Engine                     ||
|  |  +------+  +------+  +------+    ||
|  |  | App  |  | App  |  | App  |    ||
|  |  | Libs |  | Libs |  | Libs |    ||
|  |  +------+  +------+  +------+    ||
|  +-----------------------------------+|
+---------------------------------------+
| Physical Hardware                     |
+---------------------------------------+
```

**핵심 차이:**

| 측면 | 가상 머신 | 컨테이너 |
|------|----------|----------|
| OS | 각 VM마다 전체 게스트 OS | 호스트 OS 커널 공유 |
| 크기 | 수 GB | 수십~수백 MB |
| 시작 시간 | 몇 분 | 밀리초~수 초 |
| 성능 | 오버헤드 있음 | 네이티브에 가까움 |
| 격리 | 강력 (하드웨어 수준) | 강력 (프로세스 수준) |
| 포터빌리티 | 제한적 | 매우 높음 |

프론트엔드 개발자에게 비유하자면, VM은 전체 브라우저를 임베딩하는 것과 같고(Electron), 컨테이너는 필요한 런타임만 번들링하는 것과 같습니다(Deno의 단일 실행 파일).

## Docker 아키텍처: 이미지, 컨테이너, 레지스트리

Docker의 핵심 개념 세 가지를 이해해야 합니다:

**Docker 이미지 (Image):**

이미지는 **불변(immutable)의 템플릿**입니다. 애플리케이션 코드, 런타임, 라이브러리, 환경 변수, 설정 파일—실행에 필요한 모든 것을 포함합니다. 이미지는 **레이어**로 구성됩니다. 각 Dockerfile 명령이 하나의 레이어를 생성하며, 레이어는 재사용됩니다.

프론트엔드 비유: 이미지는 `node_modules`와 빌드된 애플리케이션이 함께 패키징된 불변 스냅샷입니다.

**Docker 컨테이너 (Container):**

컨테이너는 **이미지의 실행 인스턴스**입니다. 이미지가 클래스라면, 컨테이너는 객체입니다. 동일한 이미지에서 수백 개의 컨테이너를 생성할 수 있으며, 각각 독립적으로 실행됩니다. 컨테이너는 생성, 시작, 중지, 삭제될 수 있습니다. 컨테이너가 삭제되면, 그 안의 변경 사항은 사라집니다(볼륨을 사용하지 않는 한).

프론트엔드 비유: 컨테이너는 브라우저 탭입니다. 동일한 웹사이트(이미지)를 여러 탭(컨테이너)에서 열 수 있습니다.

**Docker 레지스트리 (Registry):**

레지스트리는 **이미지 저장소**입니다. Git이 코드를 저장하듯, 레지스트리는 Docker 이미지를 저장합니다. 가장 유명한 공개 레지스트리는 Docker Hub입니다. 비공개 레지스트리로는 Azure Container Registry, AWS ECR, GitHub Container Registry 등이 있습니다.

워크플로우:
1. **빌드**: `docker build`로 이미지 생성
2. **푸시**: `docker push`로 레지스트리에 업로드
3. **풀**: `docker pull`로 레지스트리에서 다운로드
4. **실행**: `docker run`으로 컨테이너 시작

```
[Dockerfile] --build--> [Image] --push--> [Registry]
                          |
                          run
                          ↓
                      [Container]
```

## .NET과 Docker: Microsoft의 일급 지원

Microsoft는 Docker를 .NET의 핵심 배포 메커니즘으로 받아들였습니다. .NET Core가 등장하면서(2016년), Linux 지원과 함께 Docker 이미지가 공식적으로 제공되기 시작했습니다. 오늘날 모든 .NET 버전(6, 7, 8, 9)은 공식 Docker 이미지를 가지고 있으며, 매달 보안 업데이트가 제공됩니다.

**.NET 공식 이미지 종류:**

Microsoft는 여러 변형의 이미지를 제공하며, 각각 특정 사용 사례에 최적화되어 있습니다:

**1. SDK 이미지: `mcr.microsoft.com/dotnet/sdk:9.0`**

.NET SDK를 포함합니다: 컴파일러, MSBuild, NuGet, 디버거 등. **빌드**에 사용합니다. 크기가 크므로(~800MB) 프로덕션 배포에는 사용하지 마세요.

**사용 사례:**
- Dockerfile의 빌드 단계
- 로컬 개발 환경
- CI/CD 빌드 서버

**2. ASP.NET Core 런타임 이미지: `mcr.microsoft.com/dotnet/aspnet:9.0`**

ASP.NET Core 런타임만 포함합니다. SDK가 없으므로 크기가 작습니다(~210MB). **프로덕션 배포**에 사용합니다.

**사용 사례:**
- 웹 애플리케이션과 API 실행
- Dockerfile의 최종 런타임 단계

**3. .NET 런타임 이미지: `mcr.microsoft.com/dotnet/runtime:9.0`**

.NET 런타임만 포함합니다. ASP.NET Core 라이브러리가 없으므로 더 작습니다(~190MB). **콘솔 애플리케이션이나 백그라운드 서비스**에 사용합니다.

**4. Alpine 변형: `-alpine` 태그**

Alpine Linux 기반입니다. Alpine은 최소주의 Linux 배포판으로, 매우 작습니다(기본 이미지 ~5MB). .NET Alpine 이미지는 ~110MB입니다.

**장점:**
- 작은 크기 = 빠른 다운로드 = 빠른 배포
- 작은 공격 표면 = 보안 향상

**단점:**
- musl libc 사용 (대부분의 Linux는 glibc 사용)
- 일부 네이티브 의존성이 작동하지 않을 수 있음
- 디버깅 도구가 적음

**사용 사례:**
- 크기가 중요한 경우
- 간단한 애플리케이션 (네이티브 의존성 없음)

**5. Chiseled 이미지: `-jammy-chiseled` 태그**

Ubuntu "Chiseled" 이미지는 최소한의 패키지만 포함합니다. 패키지 관리자(apt)조차 없으며, 쉘도 없습니다. 이는 보안 공격 표면을 극도로 줄입니다.

**장점:**
- 매우 작음 (~100MB)
- 최소 공격 표면 (취약점 수 감소)
- CVE(보안 취약점) 수가 적음

**단점:**
- 디버깅 어려움 (쉘 없음)
- 제한적인 도구

**사용 사례:**
- 보안이 최우선인 프로덕션 환경
- 규제 준수가 중요한 경우

**6. Native AOT 지원 이미지**

Native AOT로 컴파일된 애플리케이션을 위한 초소형 이미지. 런타임조차 필요 없으므로, 기본 OS 이미지만으로 충분합니다.

**이미지 선택 가이드:**

```
개발/빌드: sdk:9.0 (크기 무관, 도구 필요)
ASP.NET Core 프로덕션: aspnet:9.0 (균형)
크기 최적화: aspnet:9.0-alpine (작은 크기, 간단한 앱)
보안 최우선: aspnet:9.0-jammy-chiseled (최소 공격 표면)
콘솔 앱: runtime:9.0
Native AOT: 기본 OS 이미지 (alpine, distroless)
```

## Dockerfile 작성: ASP.NET Core를 위한 Multi-Stage Build

Dockerfile은 이미지를 빌드하기 위한 레시피입니다. 각 명령이 하나의 레이어를 생성합니다.

**기본 Dockerfile (비권장):**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet build -c Release
RUN dotnet publish -c Release -o /app/publish

WORKDIR /app/publish
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

**문제점:**
- 최종 이미지가 SDK를 포함 (~800MB)
- 소스 코드가 이미지에 포함 (보안 위험)
- 빌드 도구가 프로덕션에 불필요하게 포함

**Multi-Stage Build (권장):**

Multi-stage build는 하나의 Dockerfile에서 여러 단계를 정의합니다. 각 단계는 독립적인 `FROM`으로 시작하며, 이전 단계의 결과물을 복사할 수 있습니다. 최종 단계만 이미지로 저장됩니다.

```dockerfile
# ===========================
# Stage 1: Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 프로젝트 파일만 먼저 복사 (레이어 캐싱 최적화)
COPY ["MyApp/MyApp.csproj", "MyApp/"]
RUN dotnet restore "MyApp/MyApp.csproj"

# 나머지 소스 코드 복사
COPY . .
WORKDIR "/src/MyApp"

# Release 빌드
RUN dotnet build "MyApp.csproj" -c Release -o /app/build

# ===========================
# Stage 2: Publish
# ===========================
FROM build AS publish
RUN dotnet publish "MyApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===========================
# Stage 3: Final Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# 빌드 단계에서 발행된 파일만 복사
COPY --from=publish /app/publish .

# Non-root 사용자로 실행 (보안)
USER $APP_UID

# 컨테이너가 수신할 포트
EXPOSE 8080

# 애플리케이션 시작
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

**장점:**
- 최종 이미지는 런타임만 포함 (~210MB)
- 빌드 도구와 소스 코드는 제외
- 깨끗하고 안전한 프로덕션 이미지

**.NET 9의 개선사항:**

.NET 9부터 ASP.NET Core는 기본적으로 포트 8080을 사용하며, non-root 사용자로 실행됩니다. 이전 버전(80 포트, root 사용자)보다 안전합니다.

## 레이어 캐싱 최적화: 빌드 속도 극대화

Docker는 레이어를 캐시합니다. 레이어가 변경되지 않으면, 재사용됩니다. 이를 활용하면 빌드 속도를 극적으로 개선할 수 있습니다.

**비효율적인 순서:**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .  # 모든 파일 복사 - 코드 변경마다 캐시 무효화
RUN dotnet restore
RUN dotnet build -c Release -o /app/build
```

**문제**: 소스 코드 한 줄만 변경해도, `COPY . .` 레이어가 무효화됩니다. 결과적으로 `dotnet restore`도 다시 실행됩니다. NuGet 패키지 복원은 느리므로(수십 초~수 분), 이는 비효율적입니다.

**최적화된 순서:**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. 프로젝트 파일만 먼저 복사
COPY ["MyApp/MyApp.csproj", "MyApp/"]

# 2. Restore (프로젝트 파일이 변경되지 않으면 캐시 히트)
RUN dotnet restore "MyApp/MyApp.csproj"

# 3. 소스 코드 복사 (자주 변경됨)
COPY . .
WORKDIR "/src/MyApp"

# 4. Build (소스 코드만 변경되면 이 단계부터 실행)
RUN dotnet build "MyApp.csproj" -c Release -o /app/build
```

**효과**: `.csproj` 파일이 변경되지 않는 한, `dotnet restore`는 캐시에서 재사용됩니다. 소스 코드만 변경되면, 빌드 단계부터 시작되어 수십 초를 절약합니다.

**npm과 비교:**

Node.js Dockerfile에서 `package.json`을 먼저 복사하고 `npm install`을 실행하는 패턴과 동일합니다:

```dockerfile
# Node.js 패턴
COPY package*.json ./
RUN npm install
COPY . .

# .NET 패턴
COPY ["*.csproj", "./"]
RUN dotnet restore
COPY . .
```

## .dockerignore: 불필요한 파일 제외

`.dockerignore` 파일은 `.gitignore`와 유사합니다. Docker 빌드 컨텍스트에서 제외할 파일과 폴더를 지정합니다. 이는 빌드 속도를 향상시키고, 보안을 강화합니다.

**.dockerignore 예제:**

```
# 빌드 결과물
**/bin/
**/obj/
**/out/
**/publish/

# Visual Studio 관련
.vs/
.vscode/
*.user
*.suo

# Git
.git/
.gitignore
.gitattributes

# 문서
*.md
LICENSE
docs/

# 테스트 결과
**/TestResults/
**/*.trx

# Node.js (프론트엔드 빌드 산출물은 포함할 수 있음)
node_modules/
npm-debug.log

# 기타
.DS_Store
Thumbs.db
```

**효과:**
- Docker 빌드 컨텍스트가 작아짐 (MB → KB)
- 빌드 속도 향상
- 민감한 파일이 이미지에 포함되는 것 방지

## 이미지 크기 최적화: 더 작은 이미지, 더 빠른 배포

이미지 크기는 중요합니다. 작은 이미지는 다운로드가 빠르고, 스토리지 비용이 적으며, 공격 표면이 작습니다.

**최적화 기법:**

**1. Multi-stage build 사용**

이미 다루었습니다. SDK 이미지는 버리고, 런타임 이미지만 사용합니다.

**2. Alpine 또는 Chiseled 이미지 사용**

```dockerfile
# 표준 (~210MB)
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Alpine (~110MB)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine

# Chiseled (~100MB)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-jammy-chiseled
```

**3. 단일 파일 배포와 트리밍**

.NET은 애플리케이션을 단일 실행 파일로 발행할 수 있으며, 사용하지 않는 코드를 제거(trimming)할 수 있습니다:

```xml
<PropertyGroup>
  <PublishSingleFile>true</PublishSingleFile>
  <PublishTrimmed>true</PublishTrimmed>
  <TrimMode>link</TrimMode>
</PropertyGroup>
```

**주의**: 트리밍은 리플렉션을 사용하는 코드를 깨뜨릴 수 있습니다. 철저한 테스트가 필요합니다.

**4. Native AOT 컴파일**

Native AOT는 런타임조차 필요 없는 네이티브 실행 파일을 생성합니다:

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
</PropertyGroup>
```

```dockerfile
# Native AOT용 최소 이미지
FROM alpine:3.19 AS final
WORKDIR /app
COPY --from=publish /app/publish/MyApp .
ENTRYPOINT ["./MyApp"]
```

**결과**: 이미지 크기가 ~20MB까지 줄어듭니다!

**5. 레이어 최소화**

여러 `RUN` 명령을 하나로 결합:

```dockerfile
# 비효율적: 3개 레이어
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get clean

# 효율적: 1개 레이어
RUN apt-get update && \
    apt-get install -y curl && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*
```

**크기 비교:**

| 이미지 유형 | 크기 |
|-----------|-----|
| SDK (dotnet/sdk:9.0) | ~800MB |
| ASP.NET 런타임 (dotnet/aspnet:9.0) | ~210MB |
| Alpine 런타임 (dotnet/aspnet:9.0-alpine) | ~110MB |
| Chiseled 런타임 (dotnet/aspnet:9.0-jammy-chiseled) | ~100MB |
| Native AOT (alpine 기반) | ~20MB |

## Docker Compose: 다중 컨테이너 애플리케이션 관리

실제 애플리케이션은 단일 컨테이너가 아닙니다. API 서버, 데이터베이스, 캐시, 메시지 큐—여러 서비스가 함께 작동합니다. 각각을 수동으로 시작하는 것은 번거롭습니다. **Docker Compose**는 다중 컨테이너 애플리케이션을 YAML 파일로 정의하고, 단일 명령으로 시작합니다.

**docker-compose.yml 예제:**

```yaml
version: '3.8'

services:
  # ASP.NET Core API
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=myapp;Username=postgres;Password=postgres
      - Redis__Configuration=redis:6379
    depends_on:
      - postgres
      - redis
    networks:
      - app-network

  # PostgreSQL 데이터베이스
  postgres:
    image: postgres:16-alpine
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
      - POSTGRES_DB=myapp
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data
    networks:
      - app-network

  # Redis 캐시
  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    networks:
      - app-network

  # (선택) pgAdmin - PostgreSQL 관리 도구
  pgadmin:
    image: dpage/pgadmin4:latest
    environment:
      - PGADMIN_DEFAULT_EMAIL=admin@example.com
      - PGADMIN_DEFAULT_PASSWORD=admin
    ports:
      - "5050:80"
    depends_on:
      - postgres
    networks:
      - app-network

volumes:
  postgres-data:

networks:
  app-network:
    driver: bridge
```

**사용법:**

```bash
# 모든 서비스 시작 (백그라운드)
docker-compose up -d

# 로그 확인
docker-compose logs -f api

# 서비스 중지
docker-compose down

# 볼륨까지 삭제 (데이터베이스 초기화)
docker-compose down -v

# 특정 서비스만 재시작
docker-compose restart api
```

**네트워크:**

Docker Compose는 자동으로 네트워크를 생성하여, 서비스들이 이름으로 서로를 찾을 수 있습니다. API에서 `postgres:5432`로 접속하면, PostgreSQL 컨테이너에 연결됩니다. DNS 해석이 자동입니다.

**볼륨:**

컨테이너는 기본적으로 상태 비저장(stateless)입니다. 컨테이너가 삭제되면 데이터도 사라집니다. **볼륨**은 데이터를 호스트 파일 시스템에 저장하여, 컨테이너가 재시작되어도 데이터가 유지되게 합니다.

```yaml
volumes:
  - postgres-data:/var/lib/postgresql/data
```

`postgres-data` 볼륨은 Docker가 관리하며, 컨테이너 삭제 후에도 유지됩니다.

**개발 환경 최적화:**

개발 중에는 코드 변경을 즉시 반영하고 싶습니다. 볼륨을 사용하여 소스 코드를 마운트할 수 있습니다:

```yaml
api:
  build: .
  volumes:
    - ./src:/app  # 소스 코드를 컨테이너에 마운트
  environment:
    - ASPNETCORE_ENVIRONMENT=Development
```

하지만 .NET은 컴파일 언어이므로, Hot Reload를 위해 `dotnet watch`를 사용해야 합니다:

```dockerfile
# 개발용 Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app
EXPOSE 8080
CMD ["dotnet", "watch", "run", "--urls", "http://+:8080"]
```

이제 소스 코드를 수정하면, 애플리케이션이 자동으로 재빌드되고 재시작됩니다.

## 보안 모범 사례: 안전한 컨테이너 만들기

컨테이너가 안전하지 않으면, 프로덕션에 배포할 수 없습니다. 다음 보안 원칙을 따르세요:

**1. Non-root 사용자로 실행**

기본적으로 컨테이너는 root 사용자로 실행됩니다. 애플리케이션이 취약점에 노출되면, 공격자가 root 권한을 얻습니다. .NET 9 이미지는 기본적으로 non-root 사용자(`app`)를 제공합니다:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Non-root 사용자로 전환
USER $APP_UID

ENTRYPOINT ["dotnet", "MyApp.dll"]
```

**2. 최소한의 베이스 이미지 사용**

적을수록 안전합니다. Chiseled 이미지는 쉘조차 없으므로, 공격자가 컨테이너 내부에서 명령을 실행할 수 없습니다.

**3. 비밀을 이미지에 포함하지 마세요**

절대로 API 키, 비밀번호, 인증서를 Dockerfile에 복사하거나 환경 변수로 하드코딩하지 마세요. 이미지는 불변이므로, 한 번 포함되면 영원히 남습니다.

**올바른 방법:**
- 런타임에 환경 변수로 주입
- Docker Secrets (Swarm) 또는 Kubernetes Secrets 사용
- 클라우드 Key Vault/Secrets Manager 통합

**4. 취약점 스캐닝**

이미지를 빌드한 후, 알려진 보안 취약점을 스캔하세요. 여러 도구가 있습니다:

**Trivy (오픈 소스):**

```bash
# Trivy 설치 (macOS)
brew install trivy

# 이미지 스캔
trivy image myapp:latest

# 높은 심각도 취약점만 표시
trivy image --severity HIGH,CRITICAL myapp:latest
```

**Snyk:**

```bash
snyk container test myapp:latest
```

**Docker Scout (.NET 9 통합):**

Docker Desktop에 내장되어 있으며, 이미지 빌드 시 자동으로 스캔합니다.

**5. 읽기 전용 파일 시스템**

가능하면 컨테이너의 파일 시스템을 읽기 전용으로 실행하세요:

```bash
docker run --read-only myapp:latest
```

임시 파일이 필요하면, tmpfs 마운트를 사용합니다:

```bash
docker run --read-only --tmpfs /tmp myapp:latest
```

## 컨테이너 레지스트리: 이미지 저장과 배포

이미지를 빌드한 후, 어디에 저장할까요? **컨테이너 레지스트리**는 Docker 이미지의 저장소입니다. 여러 선택지가 있습니다:

**1. Docker Hub**

가장 유명한 공개 레지스트리. 무료 티어는 무제한 공개 이미지와 1개의 비공개 이미지를 지원합니다.

```bash
# 로그인
docker login

# 이미지 태그
docker tag myapp:latest myusername/myapp:latest

# 푸시
docker push myusername/myapp:latest

# 풀 (다른 환경에서)
docker pull myusername/myapp:latest
```

**장점:**
- 무료 (공개 이미지)
- 광범위한 사용
- CI/CD 통합 쉬움

**단점:**
- 비공개 이미지 제한
- 회사 정책상 공개 레지스트리 사용 불가할 수 있음

**2. Azure Container Registry (ACR)**

Azure의 프라이빗 레지스트리. Azure 서비스와 긴밀히 통합됩니다.

```bash
# ACR 생성
az acr create --resource-group myRG --name myregistry --sku Basic

# 로그인
az acr login --name myregistry

# 이미지 태그
docker tag myapp:latest myregistry.azurecr.io/myapp:latest

# 푸시
docker push myregistry.azurecr.io/myapp:latest
```

**장점:**
- Azure 서비스와 통합 (App Service, AKS)
- Geo-replication (여러 리전에 복제)
- Managed Identity 지원

**가격:** Basic ($5/월), Standard ($20/월), Premium ($50/월)

**3. AWS Elastic Container Registry (ECR)**

AWS의 프라이빗 레지스트리.

```bash
# ECR 리포지토리 생성
aws ecr create-repository --repository-name myapp

# 로그인 (토큰 만료 주의)
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789.dkr.ecr.us-east-1.amazonaws.com

# 푸시
docker push 123456789.dkr.ecr.us-east-1.amazonaws.com/myapp:latest
```

**장점:**
- AWS 서비스와 통합 (ECS, EKS)
- IAM 권한 관리
- 이미지 스캐닝

**가격:** 스토리지($0.10/GB/월) + 전송($0.09/GB)

**4. GitHub Container Registry (GHCR)**

GitHub의 컨테이너 레지스트리. GitHub Actions와 완벽히 통합됩니다.

```bash
# Personal Access Token으로 로그인
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# 푸시
docker push ghcr.io/username/myapp:latest
```

**장점:**
- 무료 (공개 이미지는 무제한, 비공개는 500MB)
- GitHub Actions 통합
- 코드와 이미지가 함께 관리됨

**가격:** 무료 티어 후 $0.25/GB/월

**5. Google Container Registry (GCR) / Artifact Registry**

GCP의 컨테이너 레지스트리. Artifact Registry가 GCR의 후속입니다.

```bash
# 로그인
gcloud auth configure-docker

# 푸시
docker push gcr.io/my-project/myapp:latest
```

**이미지 태깅 전략:**

태그는 이미지의 버전을 식별합니다. 명확한 태깅 전략을 가지세요:

```bash
# Git 커밋 SHA
myapp:abc1234

# 시맨틱 버전
myapp:1.2.3
myapp:1.2
myapp:1

# 환경
myapp:dev
myapp:staging
myapp:prod

# 날짜 기반
myapp:2025-01-15

# latest (주의: 프로덕션에서 피하세요)
myapp:latest
```

**프로덕션 권장사항:**
- `latest` 태그는 사용하지 마세요 (예측 불가능)
- Git SHA 또는 시맨틱 버전 사용
- 불변 태그 (한 번 푸시하면 덮어쓰지 않음)

## 실습: ASP.NET Core API를 Docker로 컨테이너화하고 최적화하기

이제 실제로 해봅시다. 간단한 ASP.NET Core API를 Docker로 컨테이너화하고, 최적화하며, Docker Compose로 전체 스택을 실행합니다.

**1단계: 샘플 API 생성**

```bash
dotnet new webapi -n MyApi
cd MyApi
```

**2단계: Dockerfile 작성 (최적화됨)**

`Dockerfile`:

```dockerfile
# ===========================
# Stage 1: Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 프로젝트 파일 복사 및 복원 (레이어 캐싱)
COPY ["MyApi.csproj", "./"]
RUN dotnet restore "MyApi.csproj"

# 소스 코드 복사 및 빌드
COPY . .
RUN dotnet build "MyApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===========================
# Stage 2: Publish
# ===========================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MyApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===========================
# Stage 3: Final Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-jammy-chiseled AS final
WORKDIR /app
EXPOSE 8080

# 발행된 파일 복사
COPY --from=publish /app/publish .

# Non-root 사용자로 실행
USER $APP_UID

ENTRYPOINT ["dotnet", "MyApi.dll"]
```

**3단계: .dockerignore 작성**

`.dockerignore`:

```
**/bin/
**/obj/
**/.vs/
**/.vscode/
**/*.user
.git/
.gitignore
README.md
```

**4단계: 이미지 빌드**

```bash
docker build -t myapi:1.0 .
```

**빌드 시간 측정:**

```bash
time docker build -t myapi:1.0 .
# 첫 빌드: ~30-60초
# 캐시된 빌드 (코드만 변경): ~5-10초
```

**5단계: 컨테이너 실행**

```bash
docker run -d -p 5000:8080 --name myapi myapi:1.0

# 로그 확인
docker logs myapi

# 테스트
curl http://localhost:5000/weatherforecast
```

**6단계: 이미지 크기 확인 및 비교**

```bash
docker images myapi
```

**크기 비교 실험:**

표준 vs Alpine vs Chiseled를 비교해보세요:

```bash
# 표준
docker build -t myapi:standard --target final .

# Alpine으로 변경
# Dockerfile에서: FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
docker build -t myapi:alpine .

# Chiseled (현재 Dockerfile)
docker build -t myapi:chiseled .

# 크기 비교
docker images | grep myapi
```

**7단계: Docker Compose로 전체 스택 실행**

`docker-compose.yml`:

```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=myapi;Username=postgres;Password=postgres
    depends_on:
      - postgres
    networks:
      - myapi-network

  postgres:
    image: postgres:16-alpine
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
      - POSTGRES_DB=myapi
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data
    networks:
      - myapi-network

volumes:
  postgres-data:

networks:
  myapi-network:
    driver: bridge
```

**실행:**

```bash
docker-compose up -d

# 로그 확인
docker-compose logs -f api

# 중지 및 정리
docker-compose down -v
```

**8단계: 취약점 스캔**

```bash
# Trivy로 스캔
trivy image myapi:1.0

# 결과 해석 및 대응
```

## 요약 및 다음 단계

이 챕터에서 Docker 컨테이너화의 모든 것을 배웠습니다:

- Docker의 작동 원리와 VM과의 차이
- .NET 공식 이미지 종류와 선택 기준
- Multi-stage build로 최적화된 Dockerfile 작성
- 레이어 캐싱으로 빠른 빌드
- Docker Compose로 다중 컨테이너 관리
- 보안 모범 사례와 취약점 스캔
- 컨테이너 레지스트리 선택과 사용

이제 여러분의 ASP.NET Core 애플리케이션은 어디서나 동일하게 실행됩니다. 개발자의 노트북, CI 서버, 스테이징, 프로덕션—모든 환경에서 같은 컨테이너가 작동합니다. "내 컴퓨터에서는 잘 돌아가는데요"는 이제 역사의 일부가 되었습니다.

**다음 장에서:**

Chapter 25에서는 이 컨테이너를 **어디에 배포할 것인지**를 다룹니다. Azure, AWS, GCP—세 가지 주요 클라우드를 비교하고, 각각의 컨테이너 서비스(App Service, Elastic Beanstalk, Cloud Run, Kubernetes)에 배포하는 방법을 경험합니다. 동일한 컨테이너가 모든 클라우드에서 실행되는 것을 보며, 벤더 종속을 피하는 전략을 배웁니다.

---

## 연습 문제

1. **기본 컨테이너화**: 여러분의 기존 ASP.NET Core 프로젝트를 Docker로 컨테이너화하세요. Multi-stage build를 사용하고, 이미지 크기를 확인하세요.

2. **크기 최적화**: 동일한 애플리케이션을 표준, Alpine, Chiseled 이미지로 각각 빌드하고 크기를 비교하세요. 각 이미지의 장단점을 문서화하세요.

3. **Docker Compose 스택**: API + PostgreSQL + Redis를 Docker Compose로 구성하세요. 데이터베이스 연결과 캐싱이 작동하는지 확인하세요.

4. **개발 환경**: Hot Reload가 작동하는 개발용 Docker Compose 구성을 만드세요. 소스 코드를 변경하면 자동으로 재빌드되어야 합니다.

5. **보안 강화**: 취약점 스캔을 실행하고, 발견된 문제를 수정하세요. Non-root 사용자, 읽기 전용 파일 시스템을 적용하세요.

6. **레지스트리에 푸시**: Docker Hub, GitHub Container Registry, 또는 클라우드 레지스트리(ACR/ECR/GCR)에 이미지를 푸시하세요. 다른 머신에서 풀하여 실행해보세요.

---

## 참고 자료

- [Docker 공식 문서](https://docs.docker.com/)
- [.NET Docker 이미지](https://hub.docker.com/_/microsoft-dotnet)
- [Docker Compose 문서](https://docs.docker.com/compose/)
- [Multi-stage Builds](https://docs.docker.com/build/building/multi-stage/)
- [Trivy 취약점 스캐너](https://github.com/aquasecurity/trivy)
- [Docker 보안 모범 사례](https://docs.docker.com/develop/security-best-practices/)
- [.NET 컨테이너 이미지](https://learn.microsoft.com/dotnet/core/docker/introduction)
- [Chiseled Ubuntu 이미지](https://devblogs.microsoft.com/dotnet/dotnet-6-is-now-in-ubuntu-2204/)
