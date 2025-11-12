# Chapter 25: 클라우드 배포 - Azure, AWS, GCP 비교

## 클라우드 선택: 가장 중요한 기술적 의사결정

컨테이너를 만들었습니다. 이제 어디에 배포할까요? 이 질문은 단순해 보이지만, 향후 몇 년간 여러분의 애플리케이션 아키텍처, 운영 비용, 팀의 생산성, 심지어 비즈니스 전략까지 결정합니다. 클라우드 제공자를 바꾸는 것은 가능하지만 비용이 큽니다. 따라서 처음부터 올바른 선택을 하는 것이 중요합니다.

2025년 현재, 클라우드 시장은 세 거인이 지배합니다: **Amazon Web Services (AWS)**, **Microsoft Azure**, **Google Cloud Platform (GCP)**. 이 세 플랫폼은 전체 클라우드 인프라 시장의 약 65%를 차지합니다 (AWS 32%, Azure 23%, GCP 10%, Gartner 2024). 나머지는 Alibaba Cloud, IBM Cloud, Oracle Cloud 등 여러 플레이어가 나눠 가집니다.

프론트엔드 개발자로서 여러분은 Vercel, Netlify, Cloudflare Pages 같은 특화된 플랫폼에 익숙할 것입니다. 이들은 정적 사이트와 서버리스 함수에 최적화되어 있으며, 설정이 간단하고 DX(Developer Experience)가 뛰어납니다. 하지만 풀스택 ASP.NET Core 애플리케이션은 다릅니다. 데이터베이스가 필요하고, 백그라운드 작업을 실행하며, 파일을 저장하고, 캐시를 관리합니다. 이런 복잡한 워크로드는 범용 클라우드 플랫폼이 필요합니다.

이 챕터에서는 세 주요 클라우드 제공자를 **편견 없이, 균등하게** 비교합니다. Microsoft 제품인 .NET을 사용한다고 해서 Azure가 자동으로 최선의 선택은 아닙니다. 각 플랫폼의 장단점을 객관적으로 평가하고, 여러분의 상황에 맞는 선택을 할 수 있도록 돕습니다.

## 클라우드 제공자 개관: 역사와 철학

세 플랫폼은 각각 다른 배경에서 출발했으며, 그 DNA가 오늘날의 서비스 설계에 반영되어 있습니다.

### Amazon Web Services (AWS): 선구자의 방대한 생태계

**탄생 배경 (2006년):**

AWS는 Amazon.com의 내부 인프라에서 시작되었습니다. Amazon은 2000년대 초반 자사의 전자상거래 플랫폼을 확장하며 대규모 분산 시스템 구축 경험을 쌓았습니다. 그들은 이 인프라를 외부에 제공하면 비즈니스가 될 것이라 판단했고, 2006년 EC2와 S3를 출시했습니다. 이는 클라우드 컴퓨팅의 시작이었습니다.

**철학:**

AWS의 철학은 **"빌딩 블록"**입니다. 매우 세분화된 서비스를 제공하여, 사용자가 레고 블록처럼 조합해 원하는 아키텍처를 만듭니다. 이는 엄청난 유연성을 제공하지만, 학습 곡선이 가파릅니다. 2025년 현재 AWS는 200개 이상의 서비스를 제공합니다.

**특징:**
- **시장 리더**: 가장 큰 시장 점유율, 가장 성숙한 생태계
- **방대한 서비스**: 거의 모든 사용 사례를 위한 서비스 존재
- **글로벌 인프라**: 31개 리전, 99개 가용 영역 (2024년 기준)
- **엔터프라이즈 채택**: Fortune 500 기업의 대다수가 사용
- **복잡성**: 선택의 폭이 넓지만 초보자에게는 압도적

**주요 서비스:**
- **컴퓨팅**: EC2 (VM), Lambda (서버리스), ECS/EKS (컨테이너)
- **스토리지**: S3 (객체), EBS (블록), EFS (파일)
- **데이터베이스**: RDS (관계형), DynamoDB (NoSQL), Aurora (고성능 MySQL/PostgreSQL)
- **네트워킹**: VPC, Route 53 (DNS), CloudFront (CDN)

### Microsoft Azure: 엔터프라이즈와 .NET의 자연스러운 파트너

**탄생 배경 (2010년):**

Azure는 Microsoft의 클라우드 전략의 일환으로 시작되었습니다. 초기에는 "Windows Azure"로 Windows 중심이었지만, Satya Nadella가 CEO가 된 후 (2014년) "Microsoft loves Linux"라는 문화 전환과 함께 개방형 클라우드로 진화했습니다. 오늘날 Azure에서 실행되는 워크로드의 절반 이상이 Linux입니다.

**철학:**

Azure의 철학은 **"통합과 일관성"**입니다. Microsoft 생태계 (Windows Server, Active Directory, Visual Studio, Office 365)와의 깊은 통합을 제공하며, 하이브리드 클라우드 (온프레미스 + 클라우드)를 일급 시민으로 취급합니다. Azure Arc는 멀티 클라우드와 엣지까지 Azure 관리를 확장합니다.

**특징:**
- **.NET 통합**: Visual Studio에서 직접 배포, Azure SDK는 .NET 우선
- **엔터프라이즈 친화적**: Active Directory 통합, 복잡한 규정 준수
- **하이브리드 클라우드**: Azure Stack으로 온프레미스에 Azure 서비스 제공
- **AI와 데이터**: Azure OpenAI Service, Azure AI Studio로 최첨단 AI
- **가격**: 일부 서비스는 AWS보다 비쌀 수 있음

**주요 서비스:**
- **컴퓨팅**: Virtual Machines, Azure Functions, Container Apps, AKS
- **스토리지**: Blob Storage, Azure Files, Managed Disks
- **데이터베이스**: Azure SQL Database, Cosmos DB (다중 모델 NoSQL), PostgreSQL/MySQL Flexible Server
- **네트워킹**: Virtual Network, Azure Front Door (CDN), Azure DNS

### Google Cloud Platform (GCP): 데이터와 혁신의 선도자

**탄생 배경 (2011년):**

GCP는 Google의 내부 인프라 기술을 상용화한 것입니다. Google은 2000년대부터 BigTable, MapReduce, Borg (Kubernetes의 전신) 같은 혁신적 기술을 개발했습니다. GCP는 이런 기술을 세계에 제공하며, 특히 데이터 분석과 머신러닝에 강점을 가집니다.

**철학:**

GCP의 철학은 **"혁신과 간결함"**입니다. 최첨단 기술을 빠르게 도입하며 (Kubernetes, TensorFlow, Spanner), 사용자 경험을 단순하게 유지하려 노력합니다. 서비스 수는 AWS보다 적지만, 각 서비스가 더 포괄적입니다. 가격 모델도 투명하고 간단합니다.

**특징:**
- **Kubernetes 원조**: GKE는 가장 성숙한 관리형 Kubernetes
- **네트워크 우수성**: Google의 글로벌 네트워크로 낮은 지연 시간
- **데이터 분석**: BigQuery는 업계 최고의 데이터 웨어하우스
- **가격 투명성**: 단순한 가격 구조, 지속 사용 할인 자동 적용
- **시장 점유율**: AWS, Azure보다 작음 (약 10%)

**주요 서비스:**
- **컴퓨팅**: Compute Engine (VM), Cloud Functions, Cloud Run, GKE
- **스토리지**: Cloud Storage (객체), Persistent Disk, Filestore
- **데이터베이스**: Cloud SQL, Firestore (NoSQL), Spanner (글로벌 분산 SQL)
- **네트워킹**: VPC, Cloud CDN, Cloud DNS

## .NET 지원 비교: 누가 .NET을 더 잘 지원하는가?

.NET은 Microsoft 제품이므로, Azure가 당연히 최고의 .NET 지원을 제공할 것이라 예상할 수 있습니다. 이는 대체로 맞지만, AWS와 GCP도 훌륭한 .NET 지원을 제공합니다.

### Azure: .NET의 고향

**장점:**
- **네이티브 통합**: Azure Portal에서 .NET 버전을 바로 선택할 수 있습니다.
- **Visual Studio 통합**: 프로젝트에서 마우스 우클릭 → "Publish to Azure"로 즉시 배포
- **Azure SDK for .NET**: 모든 Azure 서비스를 위한 고품질 .NET 라이브러리, NuGet에서 바로 설치
- **문서와 샘플**: .NET 중심의 문서, 대부분의 예제가 C#으로 제공됨
- **빠른 업데이트**: 새 .NET 버전이 출시되면 Azure가 가장 먼저 지원
- **.NET Aspire**: Azure 전용 로컬 개발 경험 개선 도구

**예제 - Azure App Service 배포:**
```bash
# Azure CLI로 .NET 9 웹앱 생성
az webapp create \
  --resource-group myResourceGroup \
  --plan myAppServicePlan \
  --name myapp \
  --runtime "DOTNET|9.0"

# Visual Studio에서 직접 배포 (GUI)
# 또는 GitHub Actions
```

### AWS: 완전한 기능, 약간의 추가 작업

**장점:**
- **AWS SDK for .NET**: 매우 성숙하고 잘 관리됨, 모든 AWS 서비스 지원
- **Elastic Beanstalk**: .NET 플랫폼 기본 제공, 간단한 배포
- **Lambda**: .NET 7/8/9 런타임 공식 지원, Native AOT로 콜드 스타트 개선
- **툴링**: AWS Toolkit for Visual Studio, Rider용 플러그인
- **문서**: .NET 가이드와 샘플 풍부 (C# 우선은 아니지만 충분)

**단점:**
- Azure만큼 "네이티브"하지는 않음. 예를 들어, Elastic Beanstalk 배포는 추가 구성 필요
- 일부 AWS 서비스는 Java/Python 중심 문서 (하지만 .NET SDK 존재)

**예제 - AWS Lambda .NET 함수:**
```bash
# AWS Lambda 템플릿 설치
dotnet new install Amazon.Lambda.Templates

# Lambda 함수 생성
dotnet new lambda.EmptyFunction -n MyFunction

# 배포
cd MyFunction/src/MyFunction
dotnet lambda deploy-function MyFunction
```

### GCP: 잘 작동하지만 덜 강조됨

**장점:**
- **.NET 런타임 지원**: App Engine, Cloud Run, Cloud Functions 모두 .NET 지원
- **Google Cloud Client Libraries**: .NET용 클라이언트 라이브러리 제공, 품질 우수
- **컨테이너 중심**: Docker 이미지를 사용하므로 .NET 버전에 제한 없음
- **문서**: .NET 가이드 존재, 충분히 사용 가능

**단점:**
- Azure/AWS만큼 .NET이 강조되지 않음. 대부분의 예제가 Node.js, Python, Go
- 네이티브 IDE 통합 없음 (Visual Studio에서 직접 배포 불가)
- 커뮤니티 리소스가 상대적으로 적음

**예제 - GCP Cloud Run 배포:**
```bash
# Dockerfile 필요 (이미 Chapter 24에서 작성)
# gcloud CLI로 빌드 및 배포
gcloud run deploy myapp \
  --source . \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated
```

**결론:**

- **Azure**: .NET 개발자에게 가장 마찰 없는 경험
- **AWS**: 완전한 .NET 지원, 약간의 추가 학습 필요
- **GCP**: 잘 작동하지만 .NET이 일급 시민은 아님

하지만 중요한 것은: **세 플랫폼 모두 프로덕션급 .NET 워크로드를 훌륭하게 실행합니다.** 특히 Docker 컨테이너를 사용하면, .NET 지원의 차이는 거의 없어집니다.

## Platform-Native 배포: 가장 간단한 시작

각 클라우드는 코드를 푸시하기만 하면 플랫폼이 나머지를 처리하는 PaaS (Platform as a Service)를 제공합니다. 인프라 관리 없이 빠르게 시작할 수 있습니다.

### Azure App Service: .NET을 위한 최적화

**개요:**

Azure App Service는 웹 애플리케이션과 API를 위한 완전 관리형 플랫폼입니다. .NET, Node.js, Python, Java, PHP를 지원하며, 특히 .NET에 최적화되어 있습니다.

**주요 기능:**
- **자동 확장**: 트래픽에 따라 인스턴스 자동 증감
- **배포 슬롯**: Blue-Green 배포 (무중단 배포)
- **통합 모니터링**: Application Insights 자동 통합
- **커스텀 도메인과 SSL**: Let's Encrypt 무료 인증서
- **VNet 통합**: 프라이빗 리소스 접근

**가격 (2025년 기준):**
- **Free**: 공유 인프라, 60분/일 CPU, 1GB 메모리, 테스트용
- **Basic B1**: $13/월, 전용 VM, 1코어, 1.75GB RAM, 소규모 프로덕션
- **Standard S1**: $70/월, 자동 확장, 배포 슬롯, 중간 규모
- **Premium P1v3**: $150/월, 고성능, VNet 통합, 대규모

**배포 방법:**

**1. Azure CLI:**
```bash
# 리소스 그룹 생성
az group create --name myResourceGroup --location eastus

# App Service Plan 생성
az appservice plan create \
  --name myAppServicePlan \
  --resource-group myResourceGroup \
  --sku B1 \
  --is-linux

# 웹앱 생성
az webapp create \
  --resource-group myResourceGroup \
  --plan myAppServicePlan \
  --name myuniqueappname \
  --runtime "DOTNET|9.0"

# Git 배포 설정
az webapp deployment source config-local-git \
  --name myuniqueappname \
  --resource-group myResourceGroup

# 코드 푸시
git remote add azure <git-url>
git push azure main
```

**2. Visual Studio 직접 배포:**
- 프로젝트 우클릭 → "Publish"
- "Azure" 선택 → "Azure App Service (Linux/Windows)"
- 구독 선택, 리소스 생성/선택
- "Publish" 버튼 클릭

**3. GitHub Actions:**
```yaml
name: Deploy to Azure App Service

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'

      - name: Build
        run: dotnet publish -c Release -o ${{env.DOTNET_ROOT}}/myapp

      - name: Deploy to Azure Web App
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'myuniqueappname'
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ${{env.DOTNET_ROOT}}/myapp
```

**배포 슬롯 (Blue-Green 배포):**

```bash
# Staging 슬롯 생성
az webapp deployment slot create \
  --name myuniqueappname \
  --resource-group myResourceGroup \
  --slot staging

# Staging에 배포
az webapp deployment source config \
  --name myuniqueappname \
  --resource-group myResourceGroup \
  --slot staging \
  --repo-url <your-repo> \
  --branch main

# 스왑 (즉시 프로덕션으로)
az webapp deployment slot swap \
  --name myuniqueappname \
  --resource-group myResourceGroup \
  --slot staging \
  --target-slot production
```

**장점:**
- .NET 개발자에게 가장 간단한 경험
- Visual Studio에서 클릭 몇 번으로 배포
- 자동 확장, 배포 슬롯, 모니터링 모두 기본 제공

**단점:**
- Windows App Service는 비쌈 (Linux 사용 권장)
- 컨테이너 사용 시 일부 제약 (App Service for Containers는 별도)
- 복잡한 아키텍처에는 제한적

### AWS Elastic Beanstalk: 자동화된 인프라 관리

**개요:**

Elastic Beanstalk는 AWS의 PaaS입니다. EC2, 로드 밸런서, Auto Scaling, CloudWatch를 자동으로 구성하며, 사용자는 코드만 신경 씁니다. Azure App Service보다 더 많은 제어권을 제공하지만, 그만큼 설정이 복잡합니다.

**주요 기능:**
- **다양한 플랫폼**: .NET, Node.js, Python, Java, Go, Docker
- **완전한 제어**: 생성된 EC2 인스턴스에 SSH 접근 가능
- **통합 서비스**: RDS, S3, DynamoDB 쉽게 연결
- **환경 복제**: 스테이징 환경을 쉽게 복제
- **롤링 업데이트**: 무중단 배포

**가격:**

Elastic Beanstalk 자체는 무료입니다. 사용한 AWS 리소스(EC2, Load Balancer 등)에 대해서만 비용이 발생합니다.
- **t3.micro EC2** (1vCPU, 1GB RAM): ~$8/월
- **t3.small EC2** (2vCPU, 2GB RAM): ~$17/월
- **Application Load Balancer**: ~$16/월 + 데이터 전송

**배포 방법:**

**1. EB CLI:**
```bash
# EB CLI 설치
pip install awsebcli

# 초기화
eb init -p "64bit Amazon Linux 2023 v3.0 running .NET 8" myapp

# 환경 생성 및 배포
eb create myapp-env --single  # 단일 인스턴스 (개발용)
# 또는
eb create myapp-env --elb-type application  # 로드 밸런서 포함 (프로덕션용)

# 코드 변경 후 재배포
eb deploy

# 환경 열기
eb open
```

**2. .ebextensions 설정:**

프로젝트 루트에 `.ebextensions/` 폴더를 만들고 설정 파일을 추가합니다:

`.ebextensions/01_app.config`:
```yaml
option_settings:
  aws:elasticbeanstalk:application:environment:
    ASPNETCORE_ENVIRONMENT: Production
    ConnectionStrings__DefaultConnection: "your-connection-string"

  aws:elasticbeanstalk:environment:proxy:
    ProxyServer: nginx

  aws:autoscaling:launchconfiguration:
    InstanceType: t3.small
```

**3. GitHub Actions:**
```yaml
name: Deploy to Elastic Beanstalk

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'

      - name: Publish
        run: dotnet publish -c Release -o ./publish

      - name: Generate deployment package
        run: zip -r deploy.zip ./publish

      - name: Deploy to EB
        uses: einaregilsson/beanstalk-deploy@v21
        with:
          aws_access_key: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws_secret_key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          application_name: myapp
          environment_name: myapp-env
          version_label: ${{ github.sha }}
          region: us-east-1
          deployment_package: deploy.zip
```

**환경 구성:**

```bash
# 환경 변수 설정
eb setenv ASPNETCORE_ENVIRONMENT=Production ConnectionStrings__DefaultConnection="..."

# Auto Scaling 설정
eb scale 3  # 최소 3개 인스턴스

# 로그 확인
eb logs

# SSH 접속
eb ssh
```

**장점:**
- EC2, 로드 밸런서 등 기본 AWS 서비스를 자동 구성
- 생성된 리소스에 완전한 접근 권한 (SSH, 설정 변경)
- 무료 (리소스 비용만 지불)
- 롤링 업데이트로 무중단 배포

**단점:**
- Azure App Service보다 초기 설정이 복잡
- .NET 지원이 네이티브하지 않음 (구성 필요)
- 학습 곡선이 있음 (AWS 생태계 이해 필요)

### GCP App Engine: 간결함과 자동 확장

**개요:**

App Engine은 GCP의 원조 PaaS입니다 (2008년 출시). 두 가지 환경을 제공합니다:
- **Standard Environment**: 샌드박스 환경, 빠른 확장, 제한적 런타임
- **Flexible Environment**: Docker 컨테이너 기반, 더 많은 제어

.NET은 **Flexible Environment**에서만 지원됩니다.

**주요 기능:**
- **자동 확장**: 트래픽에 따라 0에서 무제한으로 확장
- **트래픽 분할**: 카나리 배포, A/B 테스트
- **버전 관리**: 여러 버전 동시 실행, 즉시 전환
- **통합 로깅**: Cloud Logging 자동 연동

**가격:**
- **Flexible Environment**: 최소 1개 인스턴스 항상 실행
  - **F1**: 1vCPU, 0.6GB RAM: ~$40/월
  - **F2**: 2vCPU, 1.2GB RAM: ~$100/월
- 0으로 축소 불가 (비용 절감 제한)

**배포 방법:**

**1. app.yaml 작성:**

프로젝트 루트에 `app.yaml`:
```yaml
runtime: custom
env: flex

env_variables:
  ASPNETCORE_ENVIRONMENT: "Production"
  ConnectionStrings__DefaultConnection: "your-connection-string"

automatic_scaling:
  min_num_instances: 1
  max_num_instances: 10
  cpu_utilization:
    target_utilization: 0.8

resources:
  cpu: 2
  memory_gb: 2
  disk_size_gb: 10
```

**2. Dockerfile 필요:**

App Engine Flexible은 Dockerfile을 사용합니다 (Chapter 24 참조).

**3. 배포:**

```bash
# gcloud CLI로 배포
gcloud app deploy

# 특정 버전으로 배포 (트래픽 받지 않음)
gcloud app deploy --no-promote --version v2

# 트래픽 전환
gcloud app services set-traffic default --splits v2=1.0

# 트래픽 분할 (카나리)
gcloud app services set-traffic default --splits v1=0.9,v2=0.1

# 로그 확인
gcloud app logs tail -s default

# 브라우저에서 열기
gcloud app browse
```

**4. GitHub Actions:**
```yaml
name: Deploy to App Engine

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Authenticate to Google Cloud
        uses: google-github-actions/auth@v1
        with:
          credentials_json: ${{ secrets.GCP_SA_KEY }}

      - name: Deploy to App Engine
        run: gcloud app deploy --quiet
```

**장점:**
- 매우 간단한 배포 (gcloud app deploy)
- 자동 확장이 뛰어남 (트래픽 급증에 빠른 대응)
- 트래픽 분할로 카나리 배포 쉬움
- 가격 투명성

**단점:**
- Flexible Environment는 최소 1개 인스턴스 항상 실행 (비용)
- Standard Environment는 .NET 미지원
- Dockerfile 필수 (추가 작업)
- Azure/AWS만큼 .NET 친화적이지 않음

### Platform-Native 비교 요약

| 측면 | Azure App Service | AWS Elastic Beanstalk | GCP App Engine |
|------|-------------------|----------------------|----------------|
| **.NET 지원** | ⭐⭐⭐⭐⭐ 최고 | ⭐⭐⭐⭐ 우수 | ⭐⭐⭐ 양호 |
| **배포 간편성** | ⭐⭐⭐⭐⭐ 매우 쉬움 | ⭐⭐⭐ 보통 | ⭐⭐⭐⭐ 쉬움 |
| **가격** | $13-150/월 | $8-50/월 (+ LB) | $40-100/월 |
| **자동 확장** | ✅ 우수 | ✅ 우수 | ✅ 최고 |
| **무중단 배포** | ✅ 배포 슬롯 | ✅ 롤링 업데이트 | ✅ 트래픽 분할 |
| **컨트롤** | ⭐⭐⭐ 제한적 | ⭐⭐⭐⭐ 많음 | ⭐⭐⭐ 제한적 |
| **최소 비용** | $13/월 (B1) | $8/월 (t3.micro) | $40/월 (F1) |
| **0으로 축소** | ❌ (Free 티어 제외) | ❌ | ❌ |

**추천:**
- **.NET 개발자, 빠른 시작**: **Azure App Service**
- **AWS 생태계, 비용 최적화**: **AWS Elastic Beanstalk**
- **간결함, 트래픽 분할**: **GCP App Engine**

## Serverless Containers: 서버 관리 없이 컨테이너 실행

Platform-Native PaaS는 편리하지만 제한적입니다. 특정 런타임, 특정 구성만 지원합니다. 컨테이너는 완전한 유연성을 제공하지만, Kubernetes 같은 오케스트레이션은 복잡합니다. **Serverless Containers**는 그 중간입니다: Docker 이미지를 푸시하면, 플랫폼이 실행하고 확장합니다. 서버 관리가 없습니다.

### Azure Container Apps: KEDA 기반 자동 확장

**개요:**

Azure Container Apps (ACA)는 2022년 GA된 Azure의 서버리스 컨테이너 플랫폼입니다. Kubernetes 기반이지만, 사용자는 Kubernetes를 전혀 몰라도 됩니다. KEDA (Kubernetes Event Driven Autoscaling)로 HTTP 요청뿐 아니라 큐, 이벤트, 스케줄 등 다양한 트리거로 확장합니다.

**주요 기능:**
- **0으로 축소**: 트래픽이 없으면 인스턴스 0개 (비용 절감)
- **Dapr 통합**: 마이크로서비스 빌딩 블록 (서비스 간 호출, pub/sub, 상태 관리)
- **Revisions**: 버전 관리, 트래픽 분할
- **VNet 통합**: 프라이빗 리소스 접근
- **Managed Identity**: 비밀 없는 Azure 서비스 접근

**가격:**
- **vCPU-second**: $0.000012/초 (~$0.043/시간)
- **Memory GB-second**: $0.0000014/초
- **요청**: $0.40/백만 요청
- **예제**: 1vCPU, 2GB, 100만 요청/월, 평균 500ms = 약 **$15-25/월**
- 0으로 축소 시 비용 없음!

**배포 방법:**

**1. Azure CLI:**
```bash
# Container Apps 환경 생성 (한 번만)
az containerapp env create \
  --name myenv \
  --resource-group myResourceGroup \
  --location eastus

# 컨테이너 앱 생성 (ACR에서 이미지 가져오기)
az containerapp create \
  --name myapp \
  --resource-group myResourceGroup \
  --environment myenv \
  --image myregistry.azurecr.io/myapp:latest \
  --target-port 8080 \
  --ingress external \
  --min-replicas 0 \
  --max-replicas 10 \
  --cpu 1.0 --memory 2.0Gi \
  --env-vars "ASPNETCORE_ENVIRONMENT=Production" "ConnectionStrings__DefaultConnection=secretref:connstring"

# 비밀 추가
az containerapp secret set \
  --name myapp \
  --resource-group myResourceGroup \
  --secrets connstring="your-connection-string"
```

**2. YAML 기반 배포:**

`containerapp.yaml`:
```yaml
properties:
  configuration:
    ingress:
      external: true
      targetPort: 8080
    secrets:
      - name: connstring
        value: "your-connection-string"
    registries:
      - server: myregistry.azurecr.io
        identity: system
  template:
    containers:
      - name: myapp
        image: myregistry.azurecr.io/myapp:latest
        resources:
          cpu: 1.0
          memory: 2Gi
        env:
          - name: ASPNETCORE_ENVIRONMENT
            value: "Production"
          - name: ConnectionStrings__DefaultConnection
            secretRef: connstring
    scale:
      minReplicas: 0
      maxReplicas: 10
      rules:
        - name: http-rule
          http:
            metadata:
              concurrentRequests: "100"
```

```bash
az containerapp create --resource-group myResourceGroup --environment myenv --name myapp --yaml containerapp.yaml
```

**3. GitHub Actions:**
```yaml
name: Deploy to Azure Container Apps

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Log in to Azure
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Build and push image
        run: |
          az acr build --registry myregistry --image myapp:${{ github.sha }} .

      - name: Deploy to Container Apps
        run: |
          az containerapp update \
            --name myapp \
            --resource-group myResourceGroup \
            --image myregistry.azurecr.io/myapp:${{ github.sha }}
```

**리비전과 트래픽 분할:**

```bash
# 새 리비전 배포 (트래픽 받지 않음)
az containerapp update \
  --name myapp \
  --resource-group myResourceGroup \
  --image myregistry.azurecr.io/myapp:v2 \
  --revision-suffix v2

# 트래픽 분할 (카나리)
az containerapp ingress traffic set \
  --name myapp \
  --resource-group myResourceGroup \
  --revision-weight myapp--v1=90 myapp--v2=10

# v2로 완전 전환
az containerapp ingress traffic set \
  --name myapp \
  --resource-group myResourceGroup \
  --revision-weight myapp--v2=100
```

**장점:**
- 0으로 축소로 비용 절감
- KEDA로 다양한 확장 트리거 (HTTP, 큐, Cron, Kafka 등)
- Dapr 통합으로 마이크로서비스 패턴 쉬움
- Managed Identity로 비밀 관리 간소화

**단점:**
- 상대적으로 새로운 서비스 (2022년 GA)
- 복잡한 네트워킹 시나리오에서 제한
- Kubernetes를 직접 사용하는 것보다는 제한적

### AWS ECS Fargate: 태스크 기반 컨테이너 실행

**개요:**

AWS ECS (Elastic Container Service)는 AWS의 컨테이너 오케스트레이션 서비스입니다. 두 가지 런치 타입이 있습니다:
- **EC2**: 직접 관리하는 EC2 인스턴스에서 컨테이너 실행
- **Fargate**: 서버리스, AWS가 인프라 관리

**Fargate 주요 기능:**
- **태스크 정의**: 컨테이너 사양을 JSON으로 정의
- **서비스**: 태스크의 원하는 수를 유지, 로드 밸런싱
- **자동 확장**: Target Tracking으로 CPU/메모리 기반 확장
- **CloudWatch 통합**: 로그와 메트릭 자동 수집

**가격 (US East, 2025):**
- **vCPU**: $0.04048/시간
- **Memory**: $0.004445/GB/시간
- **예제**: 1vCPU, 2GB = $0.04048 + $0.00889 = ~$35/월 (항상 실행)
- **0으로 축소 가능** (Service의 Desired Count = 0)

**배포 방법:**

**1. 태스크 정의 생성:**

`task-definition.json`:
```json
{
  "family": "myapp",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "1024",
  "memory": "2048",
  "containerDefinitions": [
    {
      "name": "myapp",
      "image": "123456789.dkr.ecr.us-east-1.amazonaws.com/myapp:latest",
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        }
      ],
      "secrets": [
        {
          "name": "ConnectionStrings__DefaultConnection",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789:secret:myapp/connstring"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/myapp",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "ecs"
        }
      }
    }
  ]
}
```

**2. AWS CLI로 배포:**

```bash
# ECR에 이미지 푸시 (사전 작업)
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789.dkr.ecr.us-east-1.amazonaws.com
docker tag myapp:latest 123456789.dkr.ecr.us-east-1.amazonaws.com/myapp:latest
docker push 123456789.dkr.ecr.us-east-1.amazonaws.com/myapp:latest

# 태스크 정의 등록
aws ecs register-task-definition --cli-input-json file://task-definition.json

# 클러스터 생성 (한 번만)
aws ecs create-cluster --cluster-name myapp-cluster

# 서비스 생성
aws ecs create-service \
  --cluster myapp-cluster \
  --service-name myapp-service \
  --task-definition myapp \
  --desired-count 2 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxx],securityGroups=[sg-xxx],assignPublicIp=ENABLED}" \
  --load-balancers "targetGroupArn=arn:aws:elasticloadbalancing:...,containerName=myapp,containerPort=8080"

# 서비스 업데이트 (새 이미지 배포)
aws ecs update-service \
  --cluster myapp-cluster \
  --service myapp-service \
  --force-new-deployment
```

**3. GitHub Actions:**
```yaml
name: Deploy to ECS Fargate

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v2
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: us-east-1

      - name: Login to Amazon ECR
        id: login-ecr
        uses: aws-actions/amazon-ecr-login@v1

      - name: Build, tag, and push image
        env:
          ECR_REGISTRY: ${{ steps.login-ecr.outputs.registry }}
          IMAGE_TAG: ${{ github.sha }}
        run: |
          docker build -t $ECR_REGISTRY/myapp:$IMAGE_TAG .
          docker push $ECR_REGISTRY/myapp:$IMAGE_TAG

      - name: Deploy to ECS
        uses: aws-actions/amazon-ecs-deploy-task-definition@v1
        with:
          task-definition: task-definition.json
          service: myapp-service
          cluster: myapp-cluster
          wait-for-service-stability: true
```

**Auto Scaling 설정:**

```bash
# Target Tracking 정책 생성
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --scalable-dimension ecs:service:DesiredCount \
  --resource-id service/myapp-cluster/myapp-service \
  --min-capacity 1 \
  --max-capacity 10

aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --scalable-dimension ecs:service:DesiredCount \
  --resource-id service/myapp-cluster/myapp-service \
  --policy-name cpu-target-tracking \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration file://scaling-policy.json
```

`scaling-policy.json`:
```json
{
  "TargetValue": 75.0,
  "PredefinedMetricSpecification": {
    "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
  },
  "ScaleOutCooldown": 60,
  "ScaleInCooldown": 300
}
```

**장점:**
- AWS 생태계와 깊은 통합 (ALB, CloudWatch, Secrets Manager)
- 세밀한 제어 (태스크 정의로 모든 것 구성)
- 성숙한 서비스 (2017년부터)
- IAM 역할로 세밀한 권한 관리

**단점:**
- 설정이 복잡 (태스크 정의, 서비스, 네트워킹, 로드 밸런서)
- Azure Container Apps만큼 간단하지 않음
- 0으로 축소는 수동 설정 필요

### GCP Cloud Run: 가장 간단한 서버리스 컨테이너

**개요:**

Cloud Run은 GCP의 서버리스 컨테이너 플랫폼입니다. Knative 기반이며, "stateless HTTP 컨테이너를 실행하는 가장 간단한 방법"을 표방합니다. 놀라울 정도로 단순합니다.

**주요 기능:**
- **0으로 축소**: 요청이 없으면 인스턴스 0개
- **빠른 콜드 스타트**: 평균 ~1초 (최적화 시 수백 ms)
- **자동 HTTPS**: 커스텀 도메인에 무료 SSL
- **동시성 제어**: 컨테이너당 동시 요청 수 설정
- **버전 관리**: 트래픽 분할로 카나리 배포

**가격 (US, 2025):**
- **vCPU**: $0.00002400/vCPU-second
- **Memory**: $0.00000250/GiB-second
- **요청**: $0.40/백만 요청
- **무료 티어**: 월 200만 요청, 360,000 vCPU-seconds, 180,000 GiB-seconds
- **예제**: 1vCPU, 2GB, 100만 요청, 평균 500ms = 약 **$10/월** (무료 티어 포함)

**배포 방법:**

**1. gcloud CLI (가장 간단):**

```bash
# Dockerfile이 있는 디렉터리에서
gcloud run deploy myapp \
  --source . \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated \
  --min-instances 0 \
  --max-instances 10 \
  --cpu 1 \
  --memory 2Gi \
  --port 8080 \
  --set-env-vars "ASPNETCORE_ENVIRONMENT=Production" \
  --set-secrets "ConnectionStrings__DefaultConnection=connstring:latest"

# 또는 이미 빌드된 이미지 사용
gcloud run deploy myapp \
  --image gcr.io/my-project/myapp:latest \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated
```

단 두 줄로 배포 완료! Dockerfile을 빌드하고, 레지스트리에 푸시하며, 서비스를 생성하고, HTTPS 엔드포인트를 제공합니다.

**2. YAML 기반 배포:**

`service.yaml`:
```yaml
apiVersion: serving.knative.dev/v1
kind: Service
metadata:
  name: myapp
spec:
  template:
    metadata:
      annotations:
        autoscaling.knative.dev/minScale: "0"
        autoscaling.knative.dev/maxScale: "10"
    spec:
      containers:
        - image: gcr.io/my-project/myapp:latest
          ports:
            - containerPort: 8080
          env:
            - name: ASPNETCORE_ENVIRONMENT
              value: "Production"
            - name: ConnectionStrings__DefaultConnection
              valueFrom:
                secretKeyRef:
                  name: connstring
                  key: latest
          resources:
            limits:
              cpu: "1"
              memory: "2Gi"
```

```bash
gcloud run services replace service.yaml
```

**3. GitHub Actions:**
```yaml
name: Deploy to Cloud Run

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Authenticate to Google Cloud
        uses: google-github-actions/auth@v1
        with:
          credentials_json: ${{ secrets.GCP_SA_KEY }}

      - name: Deploy to Cloud Run
        uses: google-github-actions/deploy-cloudrun@v1
        with:
          service: myapp
          region: us-central1
          source: .
          env_vars: |
            ASPNETCORE_ENVIRONMENT=Production
          secrets: |
            ConnectionStrings__DefaultConnection=connstring:latest
```

**트래픽 분할 (카나리 배포):**

```bash
# 새 리비전 배포 (트래픽 받지 않음)
gcloud run deploy myapp \
  --image gcr.io/my-project/myapp:v2 \
  --no-traffic \
  --tag v2

# 트래픽 분할
gcloud run services update-traffic myapp \
  --to-revisions=v1=90,v2=10

# v2로 완전 전환
gcloud run services update-traffic myapp \
  --to-latest
```

**동시성과 확장 설정:**

```bash
# 컨테이너당 최대 80개 동시 요청
gcloud run services update myapp \
  --concurrency 80 \
  --min-instances 0 \
  --max-instances 100
```

**장점:**
- **가장 간단한 배포**: `gcloud run deploy --source .` 한 줄
- **저렴한 가격**: 무료 티어 관대, 사용한 만큼만 지불
- **빠른 확장**: 수백 개 인스턴스로 몇 초 안에 확장
- **자동 HTTPS**: 커스텀 도메인에 무료 SSL
- **0으로 축소**: 비용 절감

**단점:**
- **Stateless만**: HTTP 요청-응답만 지원 (WebSocket, gRPC는 제한적)
- **실행 시간 제한**: 최대 60분 (대부분의 API는 충분)
- **네트워킹 제한**: VPC 직접 연결은 추가 구성 필요

### Serverless Containers 비교 요약

| 측면 | Azure Container Apps | AWS ECS Fargate | GCP Cloud Run |
|------|---------------------|----------------|---------------|
| **배포 간편성** | ⭐⭐⭐⭐ 쉬움 | ⭐⭐⭐ 보통 | ⭐⭐⭐⭐⭐ 가장 쉬움 |
| **가격** | ~$15-25/월 | ~$35/월 | ~$10/월 |
| **0으로 축소** | ✅ 자동 | ⚠️ 수동 설정 | ✅ 자동 |
| **콜드 스타트** | ~2-5초 | ~5-10초 | ~1초 |
| **확장 트리거** | HTTP, 큐, Cron 등 | CPU, 메모리 | HTTP만 |
| **마이크로서비스** | ⭐⭐⭐⭐⭐ Dapr 통합 | ⭐⭐⭐⭐ 서비스 메시 | ⭐⭐⭐ 제한적 |
| **네트워킹** | VNet 통합 | VPC 통합 | VPC Connector |
| **상태 지원** | ⚠️ HTTP만 | ⚠️ HTTP만 | ⚠️ HTTP만 |
| **최대 실행 시간** | 제한 없음 | 제한 없음 | 60분 |

**추천:**
- **마이크로서비스, 이벤트 기반**: **Azure Container Apps**
- **AWS 생태계, 세밀한 제어**: **AWS ECS Fargate**
- **간결함, 비용 최적화, 빠른 시작**: **GCP Cloud Run**

## Kubernetes: 궁극의 컨테이너 오케스트레이션

Serverless containers는 간단하지만 제한적입니다. 복잡한 마이크로서비스 아키텍처, 상태 저장 애플리케이션, 커스텀 네트워킹, 고급 배포 전략이 필요하다면 **Kubernetes**가 필요합니다. Kubernetes는 학습 곡선이 가파르지만, 무한한 유연성을 제공합니다.

세 클라우드 모두 관리형 Kubernetes 서비스를 제공하며, 각각 장단점이 있습니다.

### Azure Kubernetes Service (AKS): Azure 통합과 엔터프라이즈 기능

**개요:**

AKS는 Azure의 관리형 Kubernetes입니다. 컨트롤 플레인 (마스터 노드)은 무료이며, 워커 노드 VM에 대해서만 비용이 발생합니다.

**주요 기능:**
- **Azure 통합**: Azure AD, Key Vault, Monitor, Policy 깊은 통합
- **Managed Identity**: Pod가 Azure 리소스에 비밀 없이 접근
- **자동 업그레이드**: Kubernetes 버전 자동 업그레이드 옵션
- **노드 풀**: CPU, GPU, 고메모리 등 다양한 노드 타입 혼합
- **Virtual Nodes**: 서버리스 노드 (Azure Container Instances 기반)

**가격:**
- **컨트롤 플레인**: 무료
- **워커 노드**: VM 가격 (예: Standard_D2s_v3 = ~$70/월)
- **Uptime SLA**: $73/월 (99.95% SLA, 프로덕션 권장)

**배포 방법:**

**1. AKS 클러스터 생성:**
```bash
# 리소스 그룹 생성
az group create --name myResourceGroup --location eastus

# AKS 클러스터 생성
az aks create \
  --resource-group myResourceGroup \
  --name myAKSCluster \
  --node-count 3 \
  --node-vm-size Standard_D2s_v3 \
  --enable-managed-identity \
  --generate-ssh-keys \
  --network-plugin azure \
  --enable-addons monitoring

# kubectl 자격 증명 가져오기
az aks get-credentials --resource-group myResourceGroup --name myAKSCluster

# 클러스터 확인
kubectl get nodes
```

**2. ACR 통합:**
```bash
# ACR에 AKS 접근 권한 부여
az aks update \
  --resource-group myResourceGroup \
  --name myAKSCluster \
  --attach-acr myregistry
```

**3. 애플리케이션 배포:**

`deployment.yaml`:
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp
spec:
  replicas: 3
  selector:
    matchLabels:
      app: myapp
  template:
    metadata:
      labels:
        app: myapp
    spec:
      containers:
      - name: myapp
        image: myregistry.azurecr.io/myapp:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: db-secret
              key: connectionstring
        resources:
          requests:
            memory: "512Mi"
            cpu: "500m"
          limits:
            memory: "1Gi"
            cpu: "1000m"
---
apiVersion: v1
kind: Service
metadata:
  name: myapp-service
spec:
  type: LoadBalancer
  selector:
    app: myapp
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8080
```

```bash
# 비밀 생성
kubectl create secret generic db-secret --from-literal=connectionstring="your-connection-string"

# 배포
kubectl apply -f deployment.yaml

# 서비스 외부 IP 확인
kubectl get service myapp-service
```

**4. Ingress Controller (NGINX):**

```bash
# NGINX Ingress Controller 설치 (Helm)
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update
helm install ingress-nginx ingress-nginx/ingress-nginx \
  --set controller.service.annotations."service\.beta\.kubernetes\.io/azure-load-balancer-health-probe-request-path"=/healthz

# Ingress 리소스
kubectl apply -f ingress.yaml
```

`ingress.yaml`:
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: myapp-ingress
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /
    cert-manager.io/cluster-issuer: letsencrypt-prod
spec:
  ingressClassName: nginx
  tls:
  - hosts:
    - myapp.example.com
    secretName: myapp-tls
  rules:
  - host: myapp.example.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: myapp-service
            port:
              number: 80
```

**5. Auto Scaling:**

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: myapp-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: myapp
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 80
```

```bash
# 노드 자동 확장 활성화
az aks update \
  --resource-group myResourceGroup \
  --name myAKSCluster \
  --enable-cluster-autoscaler \
  --min-count 1 \
  --max-count 5
```

**장점:**
- Azure 서비스와 깊은 통합 (Key Vault, Monitor, AD)
- Managed Identity로 간편한 인증
- 엔터프라이즈 기능 (Policy, Security Center)
- 컨트롤 플레인 무료

**단점:**
- AWS EKS, GCP GKE보다 새로운 서비스 (일부 기능 부족)
- Kubernetes 지식 필요
- 초기 설정 복잡

### AWS Elastic Kubernetes Service (EKS): 엔터프라이즈급 Kubernetes

**개요:**

EKS는 AWS의 관리형 Kubernetes입니다. 가장 엄격한 보안과 규정 준수 요구사항을 충족하도록 설계되었습니다.

**주요 기능:**
- **AWS 통합**: IAM, Secrets Manager, CloudWatch, VPC 깊은 통합
- **Fargate 지원**: 노드 없이 Pod 실행 (서버리스)
- **보안**: AWS Nitro System, Bottlerocket OS
- **글로벌**: 모든 AWS 리전 지원

**가격:**
- **컨트롤 플레인**: $0.10/시간 (~$73/월)
- **워커 노드**: EC2 가격 (예: t3.medium = ~$30/월)
- **Fargate**: vCPU/memory 초당 요금

**배포 방법:**

**1. eksctl로 클러스터 생성 (가장 쉬움):**
```bash
# eksctl 설치
brew install weaveworks/tap/eksctl  # macOS
# 또는 https://eksctl.io/introduction/#installation

# 클러스터 생성 (10-15분 소요)
eksctl create cluster \
  --name myeks-cluster \
  --region us-east-1 \
  --nodegroup-name standard-workers \
  --node-type t3.medium \
  --nodes 3 \
  --nodes-min 1 \
  --nodes-max 5 \
  --managed

# kubectl 자격 증명 자동 구성됨
kubectl get nodes
```

**2. 애플리케이션 배포:**

EKS도 표준 Kubernetes이므로, 동일한 YAML을 사용합니다 (AKS와 거의 동일).

```bash
# ECR 로그인
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789.dkr.ecr.us-east-1.amazonaws.com

# Secrets Manager에서 비밀 가져오기 (CSI Driver)
kubectl apply -f https://raw.githubusercontent.com/kubernetes-sigs/secrets-store-csi-driver/main/deploy/rbac-secretproviderclass.yaml

# 배포
kubectl apply -f deployment.yaml
```

**3. ALB Ingress Controller:**

AWS는 Application Load Balancer (ALB)를 Ingress로 사용합니다:

```bash
# AWS Load Balancer Controller 설치
helm repo add eks https://aws.github.io/eks-charts
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=myeks-cluster \
  --set serviceAccount.create=true \
  --set serviceAccount.name=aws-load-balancer-controller
```

`ingress.yaml`:
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: myapp-ingress
  annotations:
    kubernetes.io/ingress.class: alb
    alb.ingress.kubernetes.io/scheme: internet-facing
    alb.ingress.kubernetes.io/target-type: ip
    alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:...
spec:
  rules:
  - host: myapp.example.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: myapp-service
            port:
              number: 80
```

**4. Fargate 프로필 (서버리스 노드):**

```bash
eksctl create fargateprofile \
  --cluster myeks-cluster \
  --name myapp-profile \
  --namespace default
```

이제 `default` 네임스페이스의 Pod는 Fargate (서버리스)에서 실행됩니다!

**장점:**
- 가장 엄격한 보안 (Nitro, Bottlerocket)
- Fargate 지원으로 노드 관리 불필요
- AWS 생태계와 깊은 통합
- 글로벌 가용성

**단점:**
- 컨트롤 플레인 비용 ($73/월)
- 초기 설정 복잡
- IAM 권한 관리 복잡

### Google Kubernetes Engine (GKE): 가장 성숙한 Kubernetes

**개요:**

GKE는 Kubernetes의 원조입니다 (Google이 Kubernetes를 만들었습니다). 가장 성숙하고, 가장 많은 기능을 제공합니다.

**주요 기능:**
- **Autopilot 모드**: 완전 관리형, 노드 관리 불필요, Pod 기반 요금
- **Standard 모드**: 전통적인 노드 풀 관리
- **빠른 업데이트**: Kubernetes 새 버전을 가장 먼저 지원
- **Workload Identity**: Pod가 GCP 리소스에 안전하게 접근
- **GKE Enterprise**: 멀티 클러스터 관리, 서비스 메시

**가격:**
- **Autopilot**: Pod 리소스 사용량 기반, $0.000044/vCPU-second, $0.000005/GB-second
- **Standard**: 컨트롤 플레인 무료 (단일 리전), 노드는 Compute Engine 가격
- **예제 (Autopilot)**: 3개 Pod (각 1vCPU, 2GB) 항상 실행 = ~$80/월

**배포 방법:**

**1. GKE Autopilot 클러스터 생성 (권장):**
```bash
# Autopilot 클러스터 생성
gcloud container clusters create-auto myapp-cluster \
  --region us-central1

# kubectl 자격 증명 가져오기
gcloud container clusters get-credentials myapp-cluster --region us-central1

# 클러스터 확인
kubectl get nodes  # Autopilot은 노드를 직접 보여주지 않음
```

**2. Standard 클러스터 (더 많은 제어):**
```bash
gcloud container clusters create myapp-cluster \
  --zone us-central1-a \
  --num-nodes 3 \
  --machine-type e2-medium \
  --enable-autoscaling \
  --min-nodes 1 \
  --max-nodes 5
```

**3. 애플리케이션 배포:**

GKE도 표준 Kubernetes이므로, 동일한 YAML 사용:

```bash
# GCR 인증 (자동으로 구성됨)
# 배포
kubectl apply -f deployment.yaml
```

**4. Ingress와 Let's Encrypt:**

```bash
# Google Cloud Load Balancer가 자동으로 생성됨
kubectl apply -f ingress.yaml
```

`ingress.yaml`:
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: myapp-ingress
  annotations:
    kubernetes.io/ingress.class: "gce"
    cert-manager.io/cluster-issuer: "letsencrypt-prod"
spec:
  tls:
  - hosts:
    - myapp.example.com
    secretName: myapp-tls
  rules:
  - host: myapp.example.com
    http:
      paths:
      - path: /*
        pathType: ImplementationSpecific
        backend:
          service:
            name: myapp-service
            port:
              number: 80
```

**5. Workload Identity (GCP 서비스 접근):**

```bash
# Workload Identity 활성화
gcloud container clusters update myapp-cluster \
  --workload-pool=my-project.svc.id.goog

# Kubernetes Service Account 생성
kubectl create serviceaccount myapp-ksa

# GCP Service Account와 바인딩
gcloud iam service-accounts add-iam-policy-binding \
  myapp-sa@my-project.iam.gserviceaccount.com \
  --role roles/iam.workloadIdentityUser \
  --member "serviceAccount:my-project.svc.id.goog[default/myapp-ksa]"

# Pod에서 사용
kubectl annotate serviceaccount myapp-ksa \
  iam.gke.io/gcp-service-account=myapp-sa@my-project.iam.gserviceaccount.com
```

이제 Pod는 비밀 없이 GCP 서비스 (Cloud SQL, Storage 등)에 접근할 수 있습니다!

**장점:**
- **Autopilot**: 노드 관리 불필요, 완전 서버리스
- **가장 성숙**: Kubernetes 신기능을 가장 먼저 지원
- **네트워크 성능**: Google의 글로벌 네트워크
- **컨트롤 플레인 무료** (Standard 모드)

**단점:**
- Autopilot은 제어 제한 (노드 접근 불가)
- Standard 모드는 노드 관리 필요
- GCP 시장 점유율 작음 (커뮤니티 리소스 적음)

### Kubernetes 비교 요약

| 측면 | Azure AKS | AWS EKS | GCP GKE |
|------|----------|---------|---------|
| **컨트롤 플레인 비용** | 무료 | $73/월 | 무료 (Standard) |
| **노드 관리** | 필수 | 필수 (Fargate 제외) | Autopilot: 불필요 |
| **클라우드 통합** | ⭐⭐⭐⭐⭐ Azure 최고 | ⭐⭐⭐⭐⭐ AWS 최고 | ⭐⭐⭐⭐⭐ GCP 최고 |
| **Kubernetes 성숙도** | ⭐⭐⭐⭐ 양호 | ⭐⭐⭐⭐ 우수 | ⭐⭐⭐⭐⭐ 최고 |
| **서버리스 옵션** | Virtual Nodes | Fargate | Autopilot |
| **학습 곡선** | 보통 | 가파름 | 보통 (Autopilot 쉬움) |
| **최소 비용/월** | ~$70 (노드) | ~$73 + 노드 | ~$30 (노드, Standard) |

**추천:**
- **Azure 생태계**: **Azure AKS**
- **AWS 생태계, 엔터프라이즈**: **AWS EKS**
- **간결함, 최신 Kubernetes**: **GCP GKE Autopilot**

## 의사결정 매트릭스: 어떤 클라우드를, 어떤 서비스를?

수많은 옵션이 있습니다. 어떻게 선택할까요? 다음 의사결정 트리를 따르세요:

### 1단계: 클라우드 제공자 선택

**Azure를 선택하는 경우:**
- ✅ 이미 Microsoft 생태계 사용 중 (Office 365, Active Directory, Azure AD)
- ✅ .NET 중심 개발팀
- ✅ 하이브리드 클라우드 필요 (온프레미스 + 클라우드)
- ✅ Visual Studio에서 직접 배포하고 싶음
- ✅ Enterprise 지원과 규정 준수 중요

**AWS를 선택하는 경우:**
- ✅ 가장 큰 서비스 선택지 원함
- ✅ 스타트업, 빠른 확장 예상
- ✅ 비용 최적화 중요 (Spot, Reserved Instances)
- ✅ 글로벌 인프라 최대 필요
- ✅ 이미 AWS 사용 중

**GCP를 선택하는 경우:**
- ✅ Kubernetes를 주력으로 사용
- ✅ 데이터 분석, 머신러닝 중요 (BigQuery, Vertex AI)
- ✅ 간결함과 투명한 가격 선호
- ✅ 네트워크 성능 최우선 (글로벌 앱)
- ✅ 최신 기술 빠르게 도입

### 2단계: 배포 모델 선택

**질문 1: 컨테이너를 사용하는가?**
- **아니오** → Platform-Native PaaS
  - Azure: App Service
  - AWS: Elastic Beanstalk
  - GCP: App Engine
- **예** → 다음 질문으로

**질문 2: Kubernetes가 필요한가?**
- **복잡한 마이크로서비스, 상태 저장, 커스텀 네트워킹** → Yes → Kubernetes
  - Azure: AKS
  - AWS: EKS
  - GCP: GKE (Autopilot 권장)
- **간단한 stateless API** → No → Serverless Containers

**질문 3: 비용 vs 기능?**
- **최저 비용, 간결함** → GCP Cloud Run
- **마이크로서비스, 이벤트 기반** → Azure Container Apps
- **AWS 생태계, 세밀한 제어** → AWS ECS Fargate

### 의사결정 플로우차트

```
시작
├── .NET 중심 팀? → Yes → Azure
├── 최대 서비스 선택? → Yes → AWS
├── 간결함, 최신 기술? → Yes → GCP
└── 벤더 중립? → 컨테이너 + Kubernetes

배포 모델
├── 빠른 시작, 간단한 앱? → PaaS (App Service, Elastic Beanstalk, App Engine)
├── 컨테이너, 간단한 API? → Serverless Containers (Cloud Run, Container Apps, Fargate)
└── 복잡한 마이크로서비스? → Kubernetes (AKS, EKS, GKE)
```

## 실습: 동일한 앱을 세 클라우드에 배포하고 비교하기

이론은 충분합니다. 실제로 해봅시다. 간단한 ASP.NET Core API를 Azure, AWS, GCP에 모두 배포하고, 비용, 성능, 배포 경험을 비교합니다.

**샘플 애플리케이션:**

```bash
dotnet new webapi -n MultiCloudApi
cd MultiCloudApi
```

**Dockerfile (Chapter 24 참조):**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MultiCloudApi.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0-jammy-chiseled AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "MultiCloudApi.dll"]
```

**1. Azure Container Apps:**

```bash
# 빌드 및 배포 (한 줄!)
az containerapp up \
  --name multicloud-api-azure \
  --resource-group multicloud-rg \
  --location eastus \
  --source .
```

**2. GCP Cloud Run:**

```bash
# 빌드 및 배포 (한 줄!)
gcloud run deploy multicloud-api-gcp \
  --source . \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated
```

**3. AWS ECS Fargate:**

(더 복잡하지만 완전히 자동화 가능)

```bash
# ECR 생성, 이미지 빌드, 태스크 정의, 서비스 생성 등...
# (스크립트로 자동화 권장)
```

**벤치마크:**

```bash
# Apache Bench로 부하 테스트
ab -n 10000 -c 100 https://multicloud-api-azure.azurecontainerapps.io/weatherforecast
ab -n 10000 -c 100 https://multicloud-api-gcp-xxx.run.app/weatherforecast
ab -n 10000 -c 100 https://multicloud-api-aws-alb-xxx.us-east-1.elb.amazonaws.com/weatherforecast
```

**비교 메트릭:**
- 배포 시간
- 평균 응답 시간
- 처리량 (RPS)
- 콜드 스타트 시간
- 월 비용 (100만 요청 기준)

## 요약: 클라우드 선택의 지혜

완벽한 클라우드는 없습니다. 각각 장단점이 있으며, **여러분의 상황에 맞는 선택**이 최선입니다.

**핵심 교훈:**

1. **.NET이라고 Azure가 필수는 아닙니다.** AWS와 GCP도 훌륭한 .NET 지원을 제공합니다.

2. **컨테이너는 이식성을 제공합니다.** Docker 이미지는 어떤 클라우드에도 배포할 수 있어, 벤더 종속을 완화합니다.

3. **간단하게 시작하세요.** Serverless Containers (Cloud Run, Container Apps)로 시작하고, 필요할 때 Kubernetes로 이동합니다.

4. **비용은 변동적입니다.** 무료 티어를 활용하고, 실제 사용 패턴을 모니터링하며, 리소스를 최적화합니다.

5. **다중 클라우드 전략을 고려하세요.** 주 워크로드는 한 클라우드에, 재해 복구는 다른 클라우드에 배치할 수 있습니다.

**다음 장에서:**

Chapter 26에서는 프로덕션 환경의 안정성과 보안을 보장하는 모든 요소를 다룹니다. 환경 구성 관리, 비밀 관리 (Key Vault, Secrets Manager), Rate Limiting, Circuit Breaker, 재해 복구—프로덕션 체크리스트의 모든 항목을 완성합니다.

---

## 연습 문제

1. **세 클라우드 비교**: 동일한 ASP.NET Core 앱을 Azure, AWS, GCP에 배포하고 배포 경험, 비용, 성능을 비교하는 보고서를 작성하세요.

2. **비용 최적화**: 각 클라우드에서 동일한 워크로드를 실행하는 최저 비용 구성을 찾으세요. 무료 티어, 예약 인스턴스, Spot 인스턴스를 고려하세요.

3. **CI/CD 파이프라인**: GitHub Actions로 단일 저장소에서 세 클라우드에 자동 배포하는 파이프라인을 구축하세요.

4. **마이그레이션 계획**: 현재 Azure에서 실행 중인 애플리케이션을 AWS 또는 GCP로 마이그레이션하는 단계별 계획을 작성하세요. 어떤 서비스를 어디로 매핑할까요?

5. **Kubernetes 실습**: GKE Autopilot에 마이크로서비스 애플리케이션 (API + DB + Redis)을 배포하고, Ingress, Let's Encrypt, Auto Scaling을 구성하세요.

---

## 참고 자료

**Azure:**
- [Azure App Service 문서](https://docs.microsoft.com/azure/app-service/)
- [Azure Container Apps 문서](https://docs.microsoft.com/azure/container-apps/)
- [Azure Kubernetes Service 문서](https://docs.microsoft.com/azure/aks/)

**AWS:**
- [AWS Elastic Beanstalk 문서](https://docs.aws.amazon.com/elasticbeanstalk/)
- [AWS ECS 문서](https://docs.aws.amazon.com/ecs/)
- [AWS EKS 문서](https://docs.aws.amazon.com/eks/)

**GCP:**
- [GCP App Engine 문서](https://cloud.google.com/appengine/docs)
- [GCP Cloud Run 문서](https://cloud.google.com/run/docs)
- [GCP GKE 문서](https://cloud.google.com/kubernetes-engine/docs)

**비교:**
- [Cloud Provider Comparison (2025)](https://www.gartner.com/reviews/market/public-cloud-infrastructure-professional-managed-services/compare/aws-vs-azure-vs-google-cloud)
- [Kubernetes on Cloud: AKS vs EKS vs GKE](https://www.cncf.io/blog/2024/kubernetes-cloud-comparison)
