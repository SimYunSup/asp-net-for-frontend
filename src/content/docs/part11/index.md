---
title: "Part 11 - 배포와 DevOps - 프로덕션으로 가는 마지막 단계"
---

# Part 11: 배포와 DevOps - 프로덕션으로 가는 마지막 단계

## 코드는 실행될 때 가치를 만든다

Part 10까지 여러분은 빠르고 안정적이며 모니터링 가능한 애플리케이션을 만드는 방법을 배웠습니다. 코드는 아름답고, 테스트는 완벽하며, 성능은 최적화되어 있습니다. 하지만 여전히 개발 환경의 localhost:5000에서만 실행되고 있다면, 그것은 아직 **가치를 만들지 못하고 있습니다**. 사용자는 여러분의 localhost에 접근할 수 없습니다. 실제 세계에서 가치를 창출하려면, 애플리케이션을 **프로덕션 환경에 배포**해야 합니다.

하지만 배포는 단순히 코드를 서버에 복사하는 것이 아닙니다. 프로덕션 배포는 여러 복잡한 질문에 답해야 합니다:

- **어디에 배포할 것인가?** 자체 서버? 클라우드? 어떤 클라우드 제공자?
- **어떻게 배포할 것인가?** FTP? Git? Docker? Kubernetes?
- **어떻게 환경을 관리할 것인가?** 개발, 스테이징, 프로덕션 환경의 차이를 어떻게 다룰까?
- **어떻게 비밀을 보호할 것인가?** 데이터베이스 연결 문자열, API 키를 안전하게 관리하려면?
- **어떻게 무중단 배포할 것인가?** 사용자에게 영향 없이 새 버전을 배포하려면?
- **어떻게 롤백할 것인가?** 새 배포에 문제가 생기면 어떻게 빠르게 이전 버전으로 돌아갈까?
- **어떻게 확장할 것인가?** 트래픽이 10배 증가하면 어떻게 대응할까?

Part 11에서는 이 모든 질문에 답합니다. 현대적인 DevOps 워크플로우를 마스터하고, 여러분의 ASP.NET Core 애플리케이션을 실제 사용자에게 전달하는 방법을 배웁니다.

### DevOps 문화: 개발자의 책임은 배포까지

전통적으로 개발과 운영은 분리되어 있었습니다. 개발자는 코드를 작성하고, 운영팀은 그것을 배포하고 관리했습니다. "내 컴퓨터에서는 잘 돌아가는데요"는 악명 높은 변명이었습니다. 개발 환경과 프로덕션 환경의 차이 때문에, 개발에서는 완벽하던 코드가 프로덕션에서 실패하곤 했습니다.

DevOps는 이 장벽을 허물니다. 개발자는 운영을 이해해야 하고, 운영팀은 개발을 이해해야 합니다. 더 나아가, **개발자가 직접 배포하고 모니터링하는 것**이 표준이 되었습니다. "You build it, you run it"—Amazon의 CTO Werner Vogels의 유명한 말입니다.

프론트엔드 개발자로서 여러분은 이미 이 문화에 익숙할 것입니다. Vercel에 Next.js 앱을 배포하고, Netlify에 정적 사이트를 올리며, GitHub Actions로 자동 배포를 설정해본 경험이 있을 것입니다. ASP.NET Core 배포도 비슷하지만, 백엔드 애플리케이션 특유의 고려사항이 있습니다: 데이터베이스 마이그레이션, 환경 변수 관리, 상태 유지 세션, 백그라운드 작업 등.

### 컨테이너 혁명: "내 컴퓨터에서는 잘 돌아가는데요" 문제의 해결

2013년 Docker가 등장하기 전, 애플리케이션 배포는 악몽이었습니다. 각 서버마다 런타임을 설치하고, 의존성을 구성하며, 환경 변수를 설정해야 했습니다. 개발 환경은 Windows, 스테이징은 Linux, 프로덕션은 다른 Linux 배포판일 수 있었습니다. Python 버전, 라이브러리 버전, 시스템 패키지—모든 것이 미묘하게 달랐고, 예상치 못한 문제가 발생했습니다.

Docker는 이 모든 것을 **컨테이너**라는 표준화된 단위로 패키징합니다. 컨테이너는 애플리케이션과 그것의 모든 의존성—런타임, 라이브러리, 시스템 도구—을 하나의 불변(immutable) 이미지로 묶습니다. 이 이미지는 어디서나 동일하게 실행됩니다. 개발자의 노트북에서, CI 서버에서, 스테이징 환경에서, 프로덕션 클러스터에서—모두 같은 이미지, 같은 동작입니다.

프론트엔드 개발자에게 비유하자면, npm package가 의존성을 관리하는 것처럼, Docker는 **전체 환경**을 관리합니다. `package.json`과 `package-lock.json`이 Node.js 의존성을 정확히 재현하듯, `Dockerfile`은 전체 실행 환경을 정확히 재현합니다.

**컨테이너의 이점:**

**일관성**: 개발, 테스트, 프로덕션 환경이 동일합니다. "내 컴퓨터에서는"이라는 말은 이제 통하지 않습니다.

**이식성**: 어떤 클라우드, 어떤 서버에도 배포할 수 있습니다. Azure에서 AWS로, 또는 자체 서버로 이동하는 것이 간단합니다.

**격리**: 각 컨테이너는 독립된 환경에서 실행됩니다. 한 애플리케이션이 .NET 6을 사용하고 다른 것이 .NET 9를 사용해도 충돌하지 않습니다.

**효율성**: 가상 머신보다 훨씬 가볍습니다. 전체 OS를 부팅하지 않고, 커널을 공유하므로 메모리와 시작 시간이 극적으로 개선됩니다.

**확장성**: 동일한 이미지를 수백, 수천 개의 인스턴스로 쉽게 확장할 수 있습니다. Kubernetes 같은 오케스트레이션 도구가 자동으로 관리합니다.

### .NET과 Docker: 완벽한 조화

.NET은 Docker와 환상적으로 잘 작동합니다. Microsoft는 공식 Docker 이미지를 제공하며, ASP.NET Core는 처음부터 컨테이너를 염두에 두고 설계되었습니다.

**.NET 공식 이미지 종류:**

**`mcr.microsoft.com/dotnet/aspnet:9.0`**: ASP.NET Core 런타임만 포함. 애플리케이션 실행에 최적화되어 있으며, SDK가 없어 크기가 작습니다. 프로덕션 배포에 사용합니다.

**`mcr.microsoft.com/dotnet/sdk:9.0`**: .NET SDK 포함. 빌드와 개발에 사용합니다. 컴파일러, 디버거, 도구가 모두 포함되어 있어 크기가 큽니다.

**Alpine 변형**: `mcr.microsoft.com/dotnet/aspnet:9.0-alpine`. Alpine Linux 기반으로 매우 작습니다(~110MB). 하지만 일부 네이티브 의존성 문제가 있을 수 있습니다.

**Chiseled 이미지**: Ubuntu Chiseled는 최소한의 패키지만 포함하여 보안 공격 표면을 줄입니다. `mcr.microsoft.com/dotnet/aspnet:9.0-jammy-chiseled`.

**Native AOT 이미지**: AOT 컴파일된 애플리케이션용 초소형 이미지.

**Multi-stage Build 패턴:**

Docker의 multi-stage build는 필수입니다. 하나의 Dockerfile에서 빌드와 실행을 분리하여, 최종 이미지에는 런타임만 포함됩니다:

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyApp.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

빌드 단계는 SDK 이미지(~800MB)를 사용하지만, 최종 런타임 이미지는 ASP.NET 런타임만(~210MB) 포함합니다. 결과적으로 배포 크기가 크게 줄어듭니다.

### 클라우드 배포: 어디에, 어떻게?

컨테이너를 만들었다면, 이제 어디에 배포할지 결정해야 합니다. 오늘날 대부분의 애플리케이션은 클라우드에 배포됩니다. 클라우드는 인프라 관리 부담을 줄이고, 탄력적인 확장을 가능하게 하며, 전 세계 데이터 센터에 배포할 수 있게 합니다.

하지만 **어떤 클라우드**를 선택할까요? 그리고 **어떤 서비스**를 사용할까요?

### 클라우드 제공자 비교: Azure, AWS, GCP

세 가지 주요 클라우드 제공자는 각각 장단점이 있습니다. .NET 개발자에게 Microsoft Azure가 자연스러운 선택처럼 보이지만, AWS와 GCP도 훌륭한 .NET 지원을 제공합니다. 선택은 여러분의 요구사항, 기존 인프라, 비용, 팀의 전문성에 달려 있습니다.

**Microsoft Azure**

**장점:**
- **.NET 통합의 깊이**: Azure는 Microsoft 제품이므로, .NET과의 통합이 가장 깊습니다. Visual Studio에서 직접 배포할 수 있고, Azure SDK는 .NET 우선입니다.
- **Enterprise 지원**: Active Directory 통합, Azure DevOps, 기업 수준의 지원과 SLA.
- **Hybrid Cloud**: Azure Arc로 온프레미스와 클라우드를 통합 관리할 수 있습니다.
- **학습 곡선**: .NET 개발자에게 익숙한 용어와 패턴을 사용합니다.

**단점:**
- **가격**: 일부 서비스는 AWS보다 비쌀 수 있습니다.
- **시장 점유율**: AWS보다 작아, 커뮤니티 리소스가 상대적으로 적습니다.

**적합한 경우:**
- Microsoft 생태계에 이미 투자한 기업
- .NET을 주력으로 사용하는 팀
- Enterprise 기능(Active Directory, 하이브리드 클라우드)이 필요한 경우

**Amazon Web Services (AWS)**

**장점:**
- **시장 리더**: 가장 큰 시장 점유율, 가장 성숙한 서비스, 광범위한 커뮤니티
- **서비스 폭**: 가장 많은 서비스를 제공합니다(200개 이상). 거의 모든 사용 사례를 지원합니다.
- **가격 경쟁력**: 예약 인스턴스와 Spot 인스턴스로 비용을 크게 절감할 수 있습니다.
- **글로벌 인프라**: 가장 많은 리전과 가용 영역을 보유합니다.

**단점:**
- **.NET 통합**: Azure보다 덜 매끄럽지만, 여전히 우수합니다. AWS SDK for .NET은 잘 유지되고 있습니다.
- **복잡성**: 서비스가 너무 많아 선택이 어려울 수 있습니다. 학습 곡선이 가파릅니다.

**적합한 경우:**
- 스타트업과 빠르게 성장하는 기업
- 다양한 서비스와 유연성이 필요한 경우
- 비용 최적화가 중요한 경우
- 이미 AWS를 사용 중인 조직

**Google Cloud Platform (GCP)**

**장점:**
- **혁신**: Kubernetes(GKE의 기원), BigQuery, TensorFlow 등 최첨단 기술을 선도합니다.
- **네트워킹**: Google의 글로벌 네트워크를 사용하여 뛰어난 성능과 낮은 지연 시간을 제공합니다.
- **가격 투명성**: 간단한 가격 구조, 자동 할인(지속 사용 할인).
- **데이터 분석**: BigQuery, Dataflow 등 데이터 처리 도구가 뛰어납니다.

**단점:**
- **시장 점유율**: AWS, Azure보다 작아, 일부 지역에서는 지원이 제한적입니다.
- **.NET 지원**: Azure나 AWS보다 덜 강조되지만, 충분히 사용 가능합니다.
- **Enterprise 기능**: Azure나 AWS보다 Enterprise 지원이 덜 성숙할 수 있습니다.

**적합한 경우:**
- Kubernetes를 주력으로 사용하는 경우
- 데이터 분석과 머신러닝이 중요한 경우
- 네트워킹 성능이 중요한 글로벌 애플리케이션
- 혁신적인 기술을 빠르게 도입하려는 팀

**다중 클라우드 전략**

많은 조직이 **단일 클라우드에 종속되는 것을 피하기 위해** 다중 클라우드 전략을 채택합니다. 예를 들어, 주 애플리케이션은 AWS에, 데이터 분석은 GCP에, 기업 통합은 Azure에 배포할 수 있습니다. 또는 재해 복구를 위해 두 클라우드에 동시 배포할 수 있습니다.

Docker와 Kubernetes는 다중 클라우드를 가능하게 합니다. 동일한 컨테이너 이미지를 어떤 클라우드에도 배포할 수 있으므로, 벤더 종속을 완화합니다.

### 배포 모델 스펙트럼: 관리 수준 vs 유연성

클라우드 배포는 스펙트럼입니다. 한쪽 끝에는 **완전 관리형 PaaS**(Platform as a Service)가 있어 배포가 간단하지만 제어가 제한적입니다. 다른 끝에는 **IaaS**(Infrastructure as a Service)가 있어 완전한 제어를 제공하지만 관리 부담이 큽니다.

**스펙트럼:**

```
완전 관리형 ←――――――――――――――――――――――――――――→ 완전 제어
(간단, 제한적)                                    (복잡, 유연)

Serverless → PaaS → Container PaaS → Kubernetes → VMs
Functions     App Service  Container Apps         AKS       Virtual Machines
Lambda        Elastic Beanstalk  ECS Fargate      EKS       EC2
Cloud Functions  App Engine      Cloud Run        GKE       Compute Engine
```

**Serverless Functions**: 코드만 배포, 인프라 관리 없음. 이벤트 기반, 자동 확장. Azure Functions, AWS Lambda, GCP Cloud Functions.

**PaaS**: 코드를 푸시하면 플랫폼이 나머지를 처리. Azure App Service, AWS Elastic Beanstalk, GCP App Engine.

**Container PaaS**: 컨테이너를 배포하면 플랫폼이 오케스트레이션. Azure Container Apps, AWS ECS Fargate, GCP Cloud Run.

**Kubernetes**: 완전한 컨테이너 오케스트레이션, 복잡하지만 강력함. Azure AKS, AWS EKS, GCP GKE.

**VMs**: 완전한 제어, 완전한 책임. Azure Virtual Machines, AWS EC2, GCP Compute Engine.

대부분의 ASP.NET Core 애플리케이션은 **PaaS** 또는 **Container PaaS**가 최적의 선택입니다. 관리 부담이 적으면서도 충분한 유연성을 제공합니다. Kubernetes는 대규모 마이크로서비스나 복잡한 요구사항이 있을 때 고려합니다.

### CI/CD 파이프라인: 자동화된 배포의 핵심

한 번 배포하는 것은 간단합니다. 하지만 매일 여러 번 배포하려면? 수동 배포는 오류가 발생하기 쉽고, 시간이 오래 걸리며, 일관성이 없습니다. **CI/CD**(Continuous Integration/Continuous Deployment) 파이프라인은 이를 자동화합니다.

**CI/CD의 단계:**

1. **Code Push**: 개발자가 코드를 Git 저장소에 푸시합니다.
2. **Build**: CI 서버가 자동으로 코드를 빌드합니다.
3. **Test**: 자동화된 테스트(단위, 통합, E2E)를 실행합니다.
4. **Build Container**: Docker 이미지를 빌드하고 레지스트리에 푸시합니다.
5. **Deploy**: 이미지를 스테이징 또는 프로덕션에 배포합니다.
6. **Verify**: 헬스 체크와 스모크 테스트를 실행합니다.

프론트엔드 개발자라면 Vercel이나 Netlify의 자동 배포에 익숙할 것입니다. Git에 푸시하면 자동으로 빌드되고 배포됩니다. ASP.NET Core의 CI/CD도 동일한 패턴을 따르지만, 백엔드 특유의 단계가 추가됩니다: 데이터베이스 마이그레이션, 환경별 구성, Blue-Green 배포 등.

**인기 있는 CI/CD 도구:**

- **GitHub Actions**: GitHub에 통합되어 있어 설정이 간단합니다. .NET 빌드 액션이 기본 제공됩니다.
- **Azure DevOps**: Microsoft의 종합 DevOps 플랫폼. Azure와 긴밀히 통합됩니다.
- **GitLab CI/CD**: GitLab 저장소와 통합. 자체 호스팅 가능.
- **Jenkins**: 오픈 소스, 고도로 커스터마이징 가능. 학습 곡선이 가파릅니다.
- **AWS CodePipeline**: AWS 서비스와 통합.
- **GCP Cloud Build**: GCP 서비스와 통합.

대부분의 경우 **GitHub Actions**가 최선의 선택입니다. 무료 티어가 관대하고, 설정이 간단하며, 모든 클라우드 제공자와 잘 작동합니다.

### Infrastructure as Code: 인프라를 코드로 관리하기

과거에는 서버를 수동으로 설정했습니다. SSH로 접속하여 패키지를 설치하고, 방화벽을 구성하며, 로드 밸런서를 설정했습니다. 이는 느리고, 오류가 발생하기 쉬우며, 재현이 어렵습니다. "서버가 어떻게 설정되었는지 아무도 모른다"는 악몽이 현실이었습니다.

**Infrastructure as Code**(IaC)는 인프라 구성을 코드 파일로 정의합니다. 이 코드를 실행하면 인프라가 자동으로 프로비저닝됩니다. 코드이므로, Git에 저장하고, 리뷰하며, 재사용할 수 있습니다.

**주요 IaC 도구:**

**Terraform**: 가장 인기 있는 IaC 도구. 모든 주요 클라우드를 지원합니다. 선언적 구문(HCL)을 사용하며, 변경 사항을 미리 볼 수 있습니다(plan).

**Azure Bicep**: Azure의 네이티브 IaC 언어. ARM 템플릿보다 간결하고 읽기 쉽습니다. Azure 전용입니다.

**AWS CloudFormation**: AWS의 네이티브 IaC. YAML/JSON으로 작성하며, AWS 서비스와 깊이 통합됩니다.

**Pulumi**: 실제 프로그래밍 언어(C#, TypeScript, Python)로 인프라를 정의합니다. 타입 안전성과 IDE 지원이 뛰어납니다.

.NET 개발자에게는 **Pulumi with C#**이 특히 매력적입니다. 익숙한 C# 구문으로 인프라를 정의할 수 있습니다:

```csharp
var resourceGroup = new ResourceGroup("my-rg");

var appServicePlan = new Plan("my-plan", new PlanArgs
{
    ResourceGroupName = resourceGroup.Name,
    Kind = "Linux",
    Reserved = true,
    Sku = new SkuDescriptionArgs
    {
        Tier = "Basic",
        Size = "B1"
    }
});

var app = new WebApp("my-app", new WebAppArgs
{
    ResourceGroupName = resourceGroup.Name,
    ServerFarmId = appServicePlan.Id,
    SiteConfig = new SiteConfigArgs
    {
        LinuxFxVersion = "DOTNETCORE|9.0"
    }
});
```

이 코드를 실행하면 Azure에 리소스 그룹, App Service Plan, Web App이 생성됩니다. 코드이므로 버전 관리, 리뷰, 재사용이 가능합니다.

### 환경 관리: Development, Staging, Production

애플리케이션은 여러 환경을 거칩니다:

- **Development**: 개발자의 로컬 환경. 빠른 반복, 디버깅 도구.
- **Staging**: 프로덕션과 거의 동일한 환경. 최종 테스트와 QA.
- **Production**: 실제 사용자가 접근하는 환경. 안정성과 성능이 최우선.

각 환경은 다른 구성이 필요합니다:

- **데이터베이스**: 개발은 SQLite, 스테이징과 프로덕션은 PostgreSQL/SQL Server.
- **로그 레벨**: 개발은 Debug, 프로덕션은 Warning 이상.
- **외부 서비스**: 개발은 샌드박스 API, 프로덕션은 실제 API.
- **비밀**: 개발은 하드코딩 가능, 프로덕션은 Key Vault/Secrets Manager.

ASP.NET Core는 `appsettings.{Environment}.json` 패턴으로 이를 지원합니다:

```
appsettings.json          // 기본 설정
appsettings.Development.json  // 개발 환경 재정의
appsettings.Staging.json      // 스테이징 환경 재정의
appsettings.Production.json   // 프로덕션 환경 재정의
```

환경 변수 `ASPNETCORE_ENVIRONMENT`가 현재 환경을 결정합니다.

### 보안: 비밀 관리와 최소 권한 원칙

배포에서 가장 중요한 것은 **보안**입니다. 데이터베이스 연결 문자열, API 키, 인증서—이런 비밀들이 노출되면 치명적입니다.

**절대 하지 말아야 할 것:**
- ❌ 비밀을 Git에 커밋 (`.env` 파일도 마찬가지)
- ❌ 소스 코드에 하드코딩
- ❌ 로그에 비밀 출력
- ❌ 모든 환경에서 동일한 비밀 사용

**올바른 비밀 관리:**

**개발 환경**: User Secrets (dotnet user-secrets). 로컬 개발자 머신에만 저장되며, Git에 커밋되지 않습니다.

**프로덕션 환경**: 클라우드 Key Vault/Secrets Manager
- **Azure Key Vault**: Azure의 비밀 관리 서비스
- **AWS Secrets Manager**: AWS의 비밀 관리 서비스
- **GCP Secret Manager**: GCP의 비밀 관리 서비스

**Managed Identity/Service Account**: 애플리케이션이 자격 증명 없이 클라우드 리소스에 접근할 수 있게 합니다. 비밀번호를 관리할 필요가 없으며, 자동으로 회전됩니다.

```csharp
// Azure Managed Identity를 사용하여 Key Vault 접근
var client = new SecretClient(
    new Uri("https://myvault.vault.azure.net/"),
    new DefaultAzureCredential() // 자동으로 Managed Identity 사용
);

var secret = await client.GetSecretAsync("ConnectionString");
```

### 무중단 배포: Blue-Green과 Canary

프로덕션에 새 버전을 배포할 때, 서비스 중단은 용납되지 않습니다. 사용자는 24/7 접근을 기대합니다. **무중단 배포** 전략은 이를 가능하게 합니다.

**Blue-Green 배포:**

두 개의 동일한 환경을 유지합니다: Blue(현재 프로덕션)와 Green(새 버전). 새 버전을 Green에 배포하고 테스트한 후, 로드 밸런서를 Green으로 전환합니다. 문제가 생기면 즉시 Blue로 롤백합니다.

Azure App Service의 배포 슬롯이 이 패턴을 구현합니다:

```bash
# 새 버전을 staging 슬롯에 배포
az webapp deployment source config-zip --resource-group myRG --name myApp --slot staging --src app.zip

# Staging에서 테스트
curl https://myapp-staging.azurewebsites.net

# 문제 없으면 스왑
az webapp deployment slot swap --resource-group myRG --name myApp --slot staging --target-slot production
```

스왑은 몇 초 안에 완료되며, 사용자 연결이 끊어지지 않습니다.

**Canary 배포:**

새 버전을 모든 사용자에게 한 번에 배포하는 대신, 작은 비율(예: 5%)의 사용자에게만 먼저 배포합니다. 문제가 없으면 점진적으로 비율을 높입니다(10%, 25%, 50%, 100%). 문제가 발생하면 영향 받은 사용자가 적으므로, 피해를 최소화할 수 있습니다.

Kubernetes와 Istio/Linkerd 같은 서비스 메시는 Canary 배포를 정교하게 제어할 수 있습니다.

### 모니터링과 알림: 배포 후가 시작

배포가 끝나면 일이 끝난 것이 아닙니다. 오히려 **시작**입니다. 프로덕션에서 무슨 일이 일어나는지 지속적으로 모니터링해야 합니다. Part 10에서 배운 Application Insights, Prometheus, Grafana가 이제 진가를 발휘합니다.

**배포 후 체크리스트:**
- ✅ 헬스 체크 엔드포인트가 200 OK를 반환하는가?
- ✅ 에러율이 정상 범위인가?
- ✅ 응답 시간이 SLA를 충족하는가?
- ✅ 데이터베이스 연결이 정상인가?
- ✅ 외부 API 의존성이 작동하는가?

**알림 설정:**
- 🔴 Critical: 서비스 다운, 에러율 급증
- 🟠 Warning: 응답 시간 증가, CPU 사용률 높음
- 🟢 Info: 배포 완료, 스케일 업/다운

Slack, PagerDuty, 이메일로 알림을 받아, 문제를 즉시 인지하고 대응할 수 있습니다.

### Part 11에서 배울 내용

이제 여러분은 ASP.NET Core 애플리케이션을 프로덕션에 배포하는 전체 여정을 시작합니다.

**Chapter 24: 컨테이너화와 Docker**

Docker의 기초부터 시작하여, ASP.NET Core 애플리케이션을 컨테이너화하는 방법을 배웁니다. Dockerfile 작성, multi-stage 빌드, 이미지 최적화, 보안 모범 사례를 다룹니다. Docker Compose로 다중 컨테이너 애플리케이션을 로컬에서 실행하며, 컨테이너 레지스트리에 이미지를 푸시하는 방법을 익힙니다.

Alpine, Chiseled, Native AOT 이미지 변형을 비교하고, 각각의 장단점을 이해합니다. 실습에서는 전체 ASP.NET Core 애플리케이션(API + 데이터베이스 + Redis)을 Docker Compose로 구성하고 실행합니다.

**Chapter 25: 클라우드 배포 - Azure, AWS, GCP 비교**

가장 중요한 장입니다. 동일한 ASP.NET Core 애플리케이션을 세 가지 주요 클라우드에 배포하며, 각 플랫폼의 장단점을 직접 경험합니다.

**Platform-Native 배포**: Azure App Service, AWS Elastic Beanstalk, GCP App Engine을 비교하고, 각각에 배포합니다. 가격, 성능, 관리 편의성을 비교합니다.

**Serverless Containers**: Azure Container Apps, AWS ECS Fargate, GCP Cloud Run을 사용하여 컨테이너를 서버리스로 실행합니다. 자동 확장, 콜드 스타트, 가격 모델을 비교합니다.

**Kubernetes**: Azure AKS, AWS EKS, GCP GKE에 동일한 애플리케이션을 배포합니다. Helm 차트를 작성하고, Ingress Controller를 설정하며, 자동 확장을 구성합니다.

**Serverless Functions**: Azure Functions, AWS Lambda, GCP Cloud Functions에 .NET을 배포합니다. Native AOT로 콜드 스타트를 개선하고, 각 플랫폼의 트리거와 통합을 경험합니다.

**CI/CD**: GitHub Actions로 세 클라우드에 자동 배포하는 파이프라인을 구축합니다. 환경별 배포, 승인 단계, 롤백 전략을 구현합니다.

**의사결정 매트릭스**: 어떤 클라우드를, 어떤 서비스를 선택할지에 대한 명확한 가이드를 제공합니다. 비용, 성능, 복잡성, 팀 전문성을 고려한 의사결정 트리를 제공합니다.

실습에서는 동일한 워크로드를 세 클라우드에 배포하고, 비용과 성능을 벤치마킹합니다. 이를 통해 여러분의 사용 사례에 최적의 플랫폼을 선택할 수 있습니다.

**Chapter 26: 프로덕션 고려사항**

프로덕션 환경의 안정성과 보안을 보장하는 모든 요소를 다룹니다.

**환경 구성 관리**: appsettings.json, 환경 변수, Key Vault를 조합하여 안전하고 유연한 구성 관리를 구현합니다. Managed Identity로 비밀 없는 인증을 설정합니다.

**HTTPS와 SSL/TLS**: 인증서 관리, Let's Encrypt 자동 갱신, HSTS 설정, SSL 오프로딩을 배웁니다.

**Rate Limiting**: .NET 9의 Rate Limiter 미들웨어로 API를 보호합니다. Fixed Window, Sliding Window, Token Bucket 정책을 구현하고, IP와 사용자 기반 제한을 적용합니다.

**오류 처리와 복원력**: Global Exception Handler로 일관된 오류 응답을 제공합니다. Polly로 Circuit Breaker, Retry, Fallback 패턴을 구현하여 외부 의존성 실패에 대응합니다.

**백업과 재해 복구**: 데이터베이스 백업 전략, 지역 중복성, 재해 복구 계획을 수립합니다. RTO(Recovery Time Objective)와 RPO(Recovery Point Objective)를 정의합니다.

**보안 체크리스트**: OWASP Top 10에 대응하는 구체적인 조치를 취합니다. 보안 헤더(CSP, HSTS, X-Frame-Options)를 설정하고, 의존성 취약점을 스캔하며, 정기 보안 감사를 수행합니다.

실습에서는 프로덕션 준비 체크리스트를 작성하고, 실제 애플리케이션에 모든 보안 조치를 적용합니다. 침투 테스트 도구로 취약점을 발견하고 수정하는 경험을 합니다.

## 학습 목표

Part 11을 마치면 다음을 할 수 있습니다:

- Docker로 ASP.NET Core 애플리케이션을 컨테이너화합니다
- Dockerfile을 작성하고 multi-stage 빌드를 구현합니다
- 컨테이너 이미지를 최적화하고 보안을 강화합니다
- Docker Compose로 다중 컨테이너 애플리케이션을 관리합니다
- Azure, AWS, GCP의 차이를 이해하고 적절한 플랫폼을 선택합니다
- 각 클라우드의 주요 서비스에 애플리케이션을 배포합니다
- Kubernetes 클러스터에 애플리케이션을 배포하고 관리합니다
- GitHub Actions로 CI/CD 파이프라인을 구축합니다
- Infrastructure as Code로 인프라를 관리합니다
- 환경별 구성을 안전하게 관리합니다
- Key Vault/Secrets Manager로 비밀을 보호합니다
- Blue-Green 배포로 무중단 배포를 구현합니다
- Rate Limiting과 Circuit Breaker로 애플리케이션을 보호합니다
- 프로덕션 보안 체크리스트를 적용합니다
- 재해 복구 계획을 수립합니다

## 챕터 구성

### Chapter 24: 컨테이너화와 Docker

Docker의 기초부터 고급 최적화까지, ASP.NET Core를 컨테이너화하는 모든 것을 배웁니다.

**Docker 기초:**
- 컨테이너 vs 가상 머신
- Docker 아키텍처 (이미지, 컨테이너, 레지스트리)
- .NET 개발자를 위한 Docker 개념
- Node.js 컨테이너와의 비교

**Dockerfile 작성:**
- .NET 공식 이미지 종류 (aspnet, sdk, alpine, chiseled)
- Multi-stage build 패턴
- 레이어 캐싱 최적화
- .dockerignore 활용

**컨테이너 최적화:**
- 이미지 크기 최소화
- 빌드 시간 단축
- Alpine vs Chiseled vs 표준 이미지
- Native AOT 이미지

**Docker Compose:**
- 다중 컨테이너 애플리케이션 정의
- 서비스 간 네트워킹
- 볼륨과 데이터 지속성
- 개발 환경 구성

**보안 모범 사례:**
- Non-root 사용자 실행
- 취약점 스캐닝 (Trivy, Snyk)
- 멀티 스테이지로 공격 표면 최소화
- 비밀 관리 (Docker Secrets)

**컨테이너 레지스트리:**
- Docker Hub
- Azure Container Registry
- AWS Elastic Container Registry
- GitHub Container Registry
- 이미지 태깅 전략

**핵심 개념**: Docker, 컨테이너화, Multi-stage build, 이미지 최적화

**실습**:
- ASP.NET Core API를 Docker 이미지로 빌드
- Docker Compose로 API + PostgreSQL + Redis 스택 실행
- 이미지 크기를 500MB에서 100MB로 최적화
- 컨테이너 레지스트리에 푸시하고 다른 환경에서 실행

### Chapter 25: 클라우드 배포 - Azure, AWS, GCP 비교

세 가지 주요 클라우드 플랫폼에 동일한 애플리케이션을 배포하며, 각각의 장단점을 직접 경험합니다.

**클라우드 플랫폼 개요:**
- Azure, AWS, GCP 비교
- 시장 점유율과 생태계
- .NET 지원 수준
- 가격 모델 비교

**Platform-Native 배포:**
- **Azure App Service**
  - Web App 생성과 배포
  - 배포 슬롯 (Blue-Green)
  - 자동 확장 규칙
  - 가격 티어 비교
- **AWS Elastic Beanstalk**
  - 환경 생성과 구성
  - .NET 플랫폼 설정
  - 로드 밸런서와 Auto Scaling
  - CLI 배포 (eb deploy)
- **GCP App Engine**
  - app.yaml 구성
  - 유연한 환경 vs 표준 환경
  - 트래픽 분할
  - gcloud 배포

**Serverless Containers:**
- **Azure Container Apps**
  - 서버리스 컨테이너 개념
  - KEDA 기반 auto-scaling
  - Dapr 통합
  - 가격 (vCPU-second)
- **AWS ECS Fargate**
  - 태스크 정의 (Task Definition)
  - 서비스와 클러스터
  - ALB 통합
  - CloudWatch 로그
- **GCP Cloud Run**
  - 완전 관리형 컨테이너
  - 요청 기반 auto-scaling
  - 콜드 스타트 최적화
  - 비용 효율성

**Kubernetes 배포:**
- **Azure AKS**
  - 클러스터 생성
  - kubectl과 Helm
  - Azure Monitor 통합
  - Managed Identity
- **AWS EKS**
  - eksctl로 클러스터 생성
  - ALB Ingress Controller
  - CloudWatch Container Insights
  - IAM Roles for Service Accounts
- **GCP GKE**
  - Autopilot vs Standard 모드
  - Google Cloud Load Balancer
  - Workload Identity
  - GKE 가격 최적화

**Serverless Functions:**
- **Azure Functions**
  - Isolated worker model
  - HTTP 트리거
  - Native AOT 지원
  - Durable Functions
- **AWS Lambda**
  - .NET 7+ 런타임
  - API Gateway 통합
  - Lambda Layers
  - 콜드 스타트 벤치마크
- **GCP Cloud Functions**
  - .NET 지원 현황
  - 트리거 종류
  - Cloud Run Functions

**CI/CD 파이프라인:**
- **GitHub Actions**
  - 세 클라우드 배포 워크플로우
  - Secrets 관리
  - 환경별 배포 (dev, staging, prod)
  - 승인 단계 (Approvals)
- **Azure DevOps**
  - 파이프라인 구성 (YAML)
  - Release 관리
  - Azure 통합
- **AWS CodePipeline**
  - 빌드, 테스트, 배포 단계
  - CodeBuild와 CodeDeploy
- **GCP Cloud Build**
  - cloudbuild.yaml
  - Artifact Registry 통합

**Infrastructure as Code:**
- **Terraform**
  - 세 클라우드 리소스 정의
  - 모듈화와 재사용
  - State 관리
- **Azure Bicep**
  - ARM 템플릿 대안
  - 타입 안전성
- **Pulumi with C#**
  - 실제 프로그래밍 언어로 인프라 정의
  - 타입 체크와 IntelliSense

**의사결정 매트릭스:**
- 비용 비교 (동일 워크로드)
- 성능 벤치마크
- 관리 복잡성
- 팀 전문성 고려
- 벤더 종속 vs 이식성
- 의사결정 트리

**다중 클라우드 전략:**
- 단일 vs 다중 클라우드
- 재해 복구를 위한 다중 클라우드
- Kubernetes의 이식성 활용
- 비용 최적화 전략

**핵심 개념**: Multi-cloud, PaaS, Serverless containers, Kubernetes, CI/CD

**실습**:
- 동일한 ASP.NET Core API를 Azure, AWS, GCP에 배포
- 각 클라우드의 Serverless container 서비스 비교
- GitHub Actions로 세 클라우드 자동 배포
- 비용과 성능 벤치마크 수행
- 의사결정 매트릭스로 최적 플랫폼 선택

### Chapter 26: 프로덕션 고려사항

프로덕션 환경에서 안정성, 보안, 성능을 보장하는 모든 요소를 다룹니다.

**환경 구성 관리:**
- appsettings.json 계층 구조
- 환경 변수 우선순위
- Options 패턴
- 구성 검증 (IValidateOptions)

**비밀 관리:**
- User Secrets (개발)
- **Azure Key Vault**
  - Key Vault 생성
  - Managed Identity 통합
  - .NET 애플리케이션에서 접근
  - 비밀 회전
- **AWS Secrets Manager**
  - Secret 생성과 관리
  - IAM 권한 설정
  - .NET SDK 사용
  - 자동 회전
- **GCP Secret Manager**
  - Secret 생성
  - Service Account 권한
  - .NET 클라이언트 라이브러리
  - 버전 관리

**HTTPS와 SSL/TLS:**
- 인증서 획득 (Let's Encrypt)
- Kestrel HTTPS 구성
- HSTS (HTTP Strict Transport Security)
- SSL 오프로딩 (로드 밸런서)
- 인증서 자동 갱신

**Rate Limiting:**
- .NET 9 Rate Limiter 미들웨어
- 정책 종류
  - Fixed Window
  - Sliding Window
  - Token Bucket
  - Concurrency Limiter
- IP 기반 제한
- 사용자 기반 제한
- 엔드포인트별 제한
- Rate limit 응답 (429 Too Many Requests)

**오류 처리와 복원력:**
- Global Exception Handler
  - 일관된 오류 응답
  - 로깅과 추적
- Polly 복원력 패턴
  - **Retry**: 일시적 오류 재시도
  - **Circuit Breaker**: 연속 실패 시 차단
  - **Timeout**: 응답 시간 제한
  - **Fallback**: 실패 시 대체 값
  - **Bulkhead Isolation**: 리소스 격리
- 정책 조합 (Retry + Circuit Breaker)

**백업과 재해 복구:**
- 데이터베이스 백업 전략
  - 자동 백업 스케줄
  - Point-in-time recovery
  - 백업 테스트
- 애플리케이션 상태 백업
- 지역 중복성 (Multi-region)
- 재해 복구 계획 (DR Plan)
  - RTO (Recovery Time Objective)
  - RPO (Recovery Point Objective)
  - Failover 테스트

**보안 체크리스트:**
- OWASP Top 10 대응
  - Injection: 파라미터화된 쿼리
  - XSS: 출력 인코딩
  - CSRF: Anti-forgery 토큰
  - Insecure Deserialization: JSON 검증
- 보안 헤더 설정
  - Content-Security-Policy
  - X-Frame-Options
  - X-Content-Type-Options
  - Referrer-Policy
- 의존성 취약점 스캐닝
  - dotnet list package --vulnerable
  - Dependabot
  - Snyk
- 정기 보안 감사
  - 침투 테스트
  - 코드 리뷰
  - 보안 교육

**성능과 확장성:**
- 수평 vs 수직 확장
- 상태 비저장 설계
- 분산 캐시 (Redis)
- CDN 활용
- 데이터베이스 읽기 복제본
- Connection pooling

**모니터링과 알림:**
- 프로덕션 모니터링 필수 요소
- 헬스 체크 엔드포인트
- Uptime 모니터링 (Pingdom, UptimeRobot)
- 알림 규칙
  - 서비스 다운
  - 높은 에러율
  - 느린 응답 시간
  - 리소스 고갈
- 온콜(On-call) 로테이션

**핵심 개념**: 비밀 관리, Rate limiting, Circuit breaker, 재해 복구, 보안 강화

**실습**:
- Key Vault/Secrets Manager로 비밀 관리
- Rate Limiter로 API 보호
- Polly로 Circuit Breaker 구현
- 보안 헤더 설정 및 검증
- 침투 테스트 도구로 취약점 발견 및 수정
- 재해 복구 시나리오 시뮬레이션

## 배포 체크리스트

Part 11을 학습하며 다음 원칙들을 내재화하세요:

**컨테이너:**
- [ ] Dockerfile은 multi-stage build 사용
- [ ] 최종 이미지는 aspnet 런타임만 포함
- [ ] .dockerignore로 불필요한 파일 제외
- [ ] Non-root 사용자로 실행
- [ ] 취약점 스캐닝 자동화

**클라우드 선택:**
- [ ] 요구사항에 맞는 클라우드 선택 (비용, 성능, 전문성)
- [ ] 벤더 종속 위험 평가
- [ ] 다중 클라우드 전략 고려
- [ ] 리전과 가용 영역 선택

**CI/CD:**
- [ ] 모든 배포는 자동화
- [ ] 테스트 통과 후에만 배포
- [ ] 환경별 승인 단계 (프로덕션)
- [ ] 롤백 계획 준비

**보안:**
- [ ] 비밀은 절대 Git에 커밋 금지
- [ ] Key Vault/Secrets Manager 사용
- [ ] Managed Identity/Service Account 활용
- [ ] HTTPS 강제, HSTS 활성화
- [ ] Rate limiting 적용
- [ ] 보안 헤더 설정

**복원력:**
- [ ] Circuit Breaker로 외부 의존성 보호
- [ ] Retry 정책으로 일시적 오류 처리
- [ ] Timeout으로 무한 대기 방지
- [ ] 헬스 체크 엔드포인트 구현

**모니터링:**
- [ ] 프로덕션 모니터링 필수
- [ ] 알림 규칙 설정 (에러, 성능)
- [ ] 로그 집중화 (중앙 로깅)
- [ ] 배포 후 즉시 확인

**재해 복구:**
- [ ] 백업 자동화 및 테스트
- [ ] RTO/RPO 정의
- [ ] 다중 리전 고려
- [ ] Failover 절차 문서화

## 다음 단계

Part 11을 마치면, 여러분은 ASP.NET Core 애플리케이션을 프로덕션에 배포하고, 안전하게 운영하며, 문제에 빠르게 대응할 수 있습니다. 컨테이너, 클라우드, CI/CD—현대적인 DevOps 워크플로우를 마스터했습니다.

**Part 12: 실전 프로젝트와 모범 사례**에서는 지금까지 배운 모든 것을 하나의 종합 프로젝트에 적용합니다. 전자상거래 플랫폼을 처음부터 끝까지 구축하며, 아키텍처 설계, 코드 작성, 테스트, 배포, 모니터링—전체 생명주기를 경험합니다. 그리고 프로덕션 환경에서 배운 모범 사례를 종합하여, 여러분만의 프로젝트에 적용할 수 있는 가이드를 얻게 될 것입니다.

지금 바로 Chapter 24로 이동하여, 첫 Docker 이미지를 빌드해보세요!

---

## 참고 자료

**Docker:**
- [Docker Documentation](https://docs.docker.com/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Multi-stage Builds](https://docs.docker.com/build/building/multi-stage/)

**클라우드 배포:**
- [Azure App Service](https://docs.microsoft.com/azure/app-service/)
- [AWS Elastic Beanstalk](https://docs.aws.amazon.com/elasticbeanstalk/)
- [GCP App Engine](https://cloud.google.com/appengine/docs)
- [Azure Container Apps](https://docs.microsoft.com/azure/container-apps/)
- [AWS ECS Fargate](https://docs.aws.amazon.com/ecs/)
- [GCP Cloud Run](https://cloud.google.com/run/docs)

**Kubernetes:**
- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Azure AKS](https://docs.microsoft.com/azure/aks/)
- [AWS EKS](https://docs.aws.amazon.com/eks/)
- [GCP GKE](https://cloud.google.com/kubernetes-engine/docs)
- [Helm](https://helm.sh/docs/)

**CI/CD:**
- [GitHub Actions](https://docs.github.com/actions)
- [Azure DevOps](https://docs.microsoft.com/azure/devops/)
- [AWS CodePipeline](https://docs.aws.amazon.com/codepipeline/)
- [GCP Cloud Build](https://cloud.google.com/build/docs)

**Infrastructure as Code:**
- [Terraform](https://www.terraform.io/docs)
- [Azure Bicep](https://docs.microsoft.com/azure/azure-resource-manager/bicep/)
- [Pulumi](https://www.pulumi.com/docs/)
- [AWS CloudFormation](https://docs.aws.amazon.com/cloudformation/)

**보안:**
- [Azure Key Vault](https://docs.microsoft.com/azure/key-vault/)
- [AWS Secrets Manager](https://docs.aws.amazon.com/secretsmanager/)
- [GCP Secret Manager](https://cloud.google.com/secret-manager/docs)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Polly](https://github.com/App-vNext/Polly)

**모니터링:**
- [Application Insights](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [AWS CloudWatch](https://docs.aws.amazon.com/cloudwatch/)
- [GCP Cloud Monitoring](https://cloud.google.com/monitoring/docs)
- [Prometheus](https://prometheus.io/docs/)
- [Grafana](https://grafana.com/docs/)

**예상 학습 시간**: 3-4주 (실습 포함)
