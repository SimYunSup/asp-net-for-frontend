# Part 6: API 개발 - RESTful에서 GraphQL까지

## 데이터를 연결하는 다리: 현대적인 API 설계의 모든 것

프론트엔드 개발자로서 여러분은 이미 수많은 API를 소비해왔습니다. `fetch()`나 `axios`를 사용하여 데이터를 가져오고, POST 요청으로 폼을 제출하며, WebSocket으로 실시간 업데이트를 받아왔을 것입니다. 하지만 API를 *사용하는 것*과 *설계하고 구축하는 것*은 완전히 다른 영역입니다. Part 6에서는 여러분이 그동안 클라이언트 입장에서 사용해온 API를, 이제 서버 입장에서 직접 만들어보는 여정을 시작합니다.

"좋은 API"와 "나쁜 API"의 차이를 경험으로 알고 계실 것입니다. 일관성 없는 엔드포인트 이름, 예측 불가능한 에러 응답, 불명확한 상태 코드, 과도한 또는 부족한 데이터... 클라이언트 개발자로서 겪었던 모든 불편함은 사실 서버 측 설계의 문제였습니다. 이제 여러분이 그 반대편에 서서, 프론트엔드 개발자들이 사용하기 좋은 API를 만들 차례입니다. 그리고 프론트엔드 경험이 있다는 것은, 오히려 여러분에게 큰 장점이 됩니다. 클라이언트가 무엇을 필요로 하는지, 어떤 설계가 편리한지를 이미 알고 있기 때문입니다.

### API의 진화: REST에서 GraphQL, 그리고 실시간 통신까지

웹 개발의 역사는 API 패러다임의 진화와 함께 해왔습니다. 2000년대 초반, SOAP와 XML이 지배하던 시대에는 API를 호출하는 것만으로도 엄청난 보일러플레이트 코드가 필요했습니다. 그러다 2000년대 중반, Roy Fielding의 REST 아키텍처가 등장하며 모든 것이 바뀌었습니다. HTTP의 본질을 활용한 REST는 단순하고 직관적이었으며, 빠르게 산업 표준이 되었습니다.

하지만 REST에도 한계가 있었습니다. 과소 페칭(under-fetching)과 과다 페칭(over-fetching) 문제입니다. 사용자 프로필을 가져오는데 불필요한 필드가 수십 개 포함되거나(`/api/users/1`이 모든 것을 반환), 반대로 필요한 데이터를 얻기 위해 여러 번 요청해야 하는 경우(`/api/users/1`, `/api/users/1/posts`, `/api/users/1/comments`...)가 그것입니다. 모바일 네트워크에서 이는 치명적인 성능 저하로 이어졌습니다.

2015년, Facebook은 이 문제에 대한 해답으로 GraphQL을 오픈소스로 공개했습니다. 클라이언트가 정확히 필요한 데이터만 요청할 수 있게 하는 쿼리 언어. 하나의 요청으로 연관된 모든 데이터를 가져올 수 있으며, 불필요한 필드는 제외할 수 있습니다. GitHub, Shopify, Airbnb 같은 대기업들이 GraphQL을 채택하며, 이는 새로운 표준으로 자리 잡기 시작했습니다.

그리고 실시간 웹의 시대가 왔습니다. 채팅 애플리케이션, 협업 도구, 라이브 대시보드, 실시간 알림... 사용자들은 페이지를 새로고침하지 않고도 최신 데이터를 보기를 기대합니다. HTTP의 요청-응답 모델로는 이를 효율적으로 구현할 수 없습니다. WebSocket, Server-Sent Events, SignalR 같은 기술들이 이 간격을 메웁니다.

ASP.NET Core는 이 모든 패러다임을 훌륭하게 지원합니다. RESTful API를 위한 컨트롤러와 Minimal APIs, GraphQL을 위한 Hot Chocolate 라이브러리, 실시간 통신을 위한 SignalR. Part 6에서는 이 모든 것을 배우며, 각각의 장단점과 적합한 사용 사례를 이해하게 될 것입니다.

### 왜 ASP.NET Core로 API를 만들어야 하는가?

Node.js와 Express.js는 JavaScript 개발자에게 자연스러운 선택입니다. 같은 언어로 프론트엔드와 백엔드를 작성할 수 있다는 것은 분명한 장점입니다. 하지만 ASP.NET Core는 Node.js가 제공하지 못하는 몇 가지 결정적인 이점을 가지고 있습니다.

**1. 강력한 타입 시스템: 컴파일 타임에 잡는 버그**

JavaScript는 동적 타입 언어입니다. TypeScript가 이를 크게 개선했지만, 런타임과 컴파일 타임의 경계는 여전히 모호합니다. API 엔드포인트가 `{ name: string, age: number }`를 반환한다고 타입을 정의했지만, 실제로 데이터베이스에서 `null`이 나오면 어떻게 될까요? TypeScript는 이를 잡지 못하고, 프로덕션에서 에러가 발생합니다.

C#과 ASP.NET Core는 다릅니다. 컴파일러는 모든 타입을 검증하며, 데이터베이스 모델부터 API 응답까지 전체 파이프라인이 타입 안전합니다. `User` 엔티티가 `Email` 프로퍼티를 가지고 있다면, 이는 컴파일 타임에 보장됩니다. 리팩토링할 때도 컴파일러가 모든 사용처를 찾아주므로, 놓치는 부분이 없습니다.

```csharp
// C#: 컴파일 타임 타입 안전성
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

[HttpGet("{id}")]
public ActionResult<User> GetUser(int id)
{
    var user = _context.Users.Find(id);
    if (user == null) return NotFound();
    return user; // 컴파일러가 타입을 보장
}
```

```typescript
// TypeScript: 런타임 타입 불일치 가능
interface User {
  id: number;
  name: string;
  email: string;
}

app.get('/users/:id', (req, res) => {
  const user = db.users.find(req.params.id); // 실제로 null일 수 있음
  res.json(user); // 타입 체크 우회
});
```

**2. 성능: 네이티브 코드의 속도**

Node.js는 싱글 스레드 이벤트 루프로 작동합니다. I/O 바운드 작업에는 훌륭하지만, CPU 집약적인 작업에는 한계가 있습니다. ASP.NET Core는 .NET 런타임 위에서 실행되며, JIT(Just-In-Time) 컴파일이나 Native AOT(Ahead-of-Time) 컴파일을 통해 네이티브 코드 수준의 성능을 냅니다.

TechEmpower의 벤치마크에서 ASP.NET Core는 지속적으로 상위권을 차지합니다. 같은 하드웨어에서 Node.js보다 2-3배 많은 요청을 처리할 수 있으며, 메모리 사용량도 적습니다. 대규모 트래픽을 처리해야 하는 서비스에서 이는 인프라 비용의 직접적인 절감으로 이어집니다.

**3. 내장된 기능: 별도의 라이브러리가 필요 없는 풍부함**

Node.js 생태계는 거대하지만, 그만큼 파편화되어 있습니다. 유효성 검사는 Joi? Yup? Zod? 인증은 Passport? Auth0? 로깅은 Winston? Pino? Bunyan? 각 라이브러리는 서로 다른 API와 설정 방식을 가지고 있으며, 호환성 문제도 빈번합니다.

ASP.NET Core는 대부분의 기능을 프레임워크에 내장하고 있습니다. 모델 유효성 검사는 데이터 어노테이션으로, 인증은 ASP.NET Core Identity로, 로깅은 `ILogger`로, API 문서화는 OpenAPI로—모든 것이 일관된 방식으로 작동하며, Microsoft의 공식 지원을 받습니다. 라이브러리를 찾고 비교하는 시간을 실제 비즈니스 로직을 작성하는 데 사용할 수 있습니다.

**4. 엔터프라이즈급 도구: Visual Studio와 Rider의 강력함**

VS Code는 훌륭한 에디터지만, 완전한 IDE는 아닙니다. Visual Studio 2022나 JetBrains Rider는 리팩토링, 디버깅, 프로파일링, 코드 분석에서 차원이 다른 경험을 제공합니다. "Extract Interface", "Rename Symbol", "Find All References"—이 모든 것이 프로젝트 전체에서 타입 안전하게 작동합니다. 디버거는 복잡한 비동기 코드의 흐름을 시각화하며, 프로파일러는 성능 병목을 정확히 찾아냅니다.

**5. 보안: 기본적으로 안전한 설계**

SQL Injection, XSS, CSRF... 웹 보안 취약점의 대부분은 프레임워크가 기본적으로 방어해줄 수 있는 것들입니다. ASP.NET Core는 이를 기본값으로 제공합니다. Entity Framework Core는 파라미터화된 쿼리를 강제하며, Razor는 HTML을 자동으로 인코딩하고, Anti-forgery 토큰은 자동으로 검증됩니다. 개발자가 의도적으로 우회하지 않는 한, 보안 취약점이 들어갈 여지가 적습니다.

### REST API 설계: 원칙에서 실전까지

REST는 단순히 HTTP 엔드포인트를 만드는 것이 아닙니다. 리소스 중심의 사고, 적절한 HTTP 메서드 사용, 의미 있는 상태 코드, 일관된 URL 구조... 이 모든 것이 REST의 본질입니다. 하지만 현실에서 "RESTful"이라고 주장하는 API 중 상당수는 실제로는 "HTTP를 사용하는 RPC"에 불과합니다.

진정한 RESTful API는 자기 서술적(self-descriptive)이고, 상태 비저장(stateless)이며, 캐시 가능(cacheable)하고, 계층화된(layered) 시스템입니다. URL은 리소스를 나타내고, HTTP 메서드는 동작을 나타냅니다. `/api/users/123`는 ID가 123인 사용자 리소스를 의미하며, `GET`은 조회, `PUT`은 전체 수정, `PATCH`는 부분 수정, `DELETE`는 삭제를 나타냅니다.

하지만 이론과 실전 사이에는 간극이 있습니다. 예를 들어, "사용자 비밀번호 재설정 이메일 전송"은 어떤 HTTP 메서드를 사용해야 할까요? 리소스가 아니라 행동(action)이므로 REST 원칙에 완벽히 맞지 않습니다. 이런 경우 `/api/users/123/send-password-reset` POST를 사용하는 것이 실용적입니다.

**상태 코드의 미묘함: 200, 201, 204의 차이**

프론트엔드 개발자로서 `200 OK`만 보면 성공이라고 생각하기 쉽습니다. 하지만 HTTP 상태 코드는 훨씬 풍부한 의미 체계를 가지고 있습니다.

- `200 OK`: 요청이 성공했으며, 응답 본문에 데이터가 있음
- `201 Created`: 새 리소스가 생성되었으며, `Location` 헤더에 URL이 있음
- `204 No Content`: 성공했지만 반환할 데이터가 없음 (예: DELETE 성공)
- `400 Bad Request`: 클라이언트의 요청이 잘못됨 (유효성 검사 실패 등)
- `401 Unauthorized`: 인증이 필요함 (실제로는 "Unauthenticated"를 의미)
- `403 Forbidden`: 인증은 되었지만 권한이 없음
- `404 Not Found`: 리소스를 찾을 수 없음
- `409 Conflict`: 요청이 현재 서버 상태와 충돌 (예: 이미 존재하는 이메일)
- `422 Unprocessable Entity`: 문법은 맞지만 의미적으로 처리 불가
- `500 Internal Server Error`: 서버 내부 오류

각 상태 코드를 적절히 사용하면, 클라이언트는 별도의 응답 본문을 파싱하지 않고도 요청의 결과를 이해할 수 있습니다. 또한 캐싱, 재시도 로직, 에러 처리를 상태 코드 기반으로 자동화할 수 있습니다.

**API 버전 관리: 변화를 수용하는 전략**

API는 시간이 지나며 진화합니다. 새 필드가 추가되고, 기존 동작이 변경되며, 더 나은 설계가 발견됩니다. 하지만 기존 클라이언트를 망가뜨릴 수는 없습니다. 모바일 앱은 사용자가 업데이트하지 않으면 오래된 버전이 계속 사용되며, 서드파티 통합은 수정이 어렵습니다.

API 버전 관리는 이 문제의 해답입니다. 여러 전략이 있습니다:

**URL 기반 버전 관리** (`/api/v1/users`, `/api/v2/users`): 가장 명시적이고 직관적입니다. 버전이 URL의 일부이므로, 브라우저에서도 쉽게 테스트할 수 있습니다. 단점은 URL 구조가 길어진다는 것입니다.

**헤더 기반 버전 관리** (`Accept: application/vnd.myapi.v2+json`): URL은 깔끔하게 유지되지만, 버전을 확인하려면 헤더를 봐야 합니다. API 탐색이 조금 덜 직관적입니다.

**쿼리 문자열 버전 관리** (`/api/users?version=2`): 간단하지만 캐싱에 문제가 생길 수 있습니다.

ASP.NET Core는 `Microsoft.AspNetCore.Mvc.Versioning` 패키지로 모든 방식을 지원하며, 한 프로젝트에서 여러 버전의 API를 동시에 제공할 수 있습니다. 중요한 것은 **breaking change를 도입할 때만 메이저 버전을 올리고**, **하위 호환성을 유지하는 변경은 같은 버전 내에서 처리**하는 원칙입니다.

### API 보안: 신뢰할 수 없는 세상에서 안전하게

인터넷은 적대적인 환경입니다. 여러분의 API는 공개되는 순간, 자동화된 봇, 악의적인 사용자, 보안 취약점을 찾는 스크립트의 표적이 됩니다. 보안은 선택이 아니라 필수이며, "나중에 추가하면 되지"라는 생각은 치명적입니다. 보안은 아키텍처의 기초부터 고려되어야 합니다.

**인증(Authentication) vs 권한 부여(Authorization): 혼동하기 쉬운 개념**

많은 개발자들이 이 두 용어를 혼용하지만, 이들은 명확히 다릅니다.

**인증(Authentication)**은 "당신은 누구인가?"를 묻습니다. 사용자가 실제로 그들이 주장하는 사람인지 확인하는 과정입니다. 비밀번호, 생체 인식, OAuth 토큰, JWT—모두 인증 메커니즘입니다.

**권한 부여(Authorization)**는 "당신은 무엇을 할 수 있는가?"를 묻습니다. 인증된 사용자가 특정 리소스에 접근할 권한이 있는지 확인합니다. 일반 사용자는 자신의 프로필만 수정할 수 있지만, 관리자는 모든 사용자의 프로필을 수정할 수 있습니다.

ASP.NET Core는 이 둘을 명확히 분리합니다. 인증은 `[Authorize]` 특성과 인증 미들웨어로, 권한 부여는 정책(Policy) 기반 시스템으로 처리됩니다.

**JWT(JSON Web Token): 상태 비저장 인증의 표준**

전통적인 세션 기반 인증은 서버에 상태를 저장합니다. 사용자가 로그인하면 서버는 세션 ID를 생성하고, 이를 메모리나 데이터베이스에 저장합니다. 클라이언트는 쿠키로 세션 ID를 받아, 이후 요청마다 전송합니다. 서버는 세션 저장소를 조회하여 사용자를 식별합니다.

하지만 이 방식은 확장성에 문제가 있습니다. 로드 밸런서 뒤에 여러 서버가 있다면, 세션 저장소를 공유해야 합니다. Redis 같은 분산 캐시를 사용할 수 있지만, 이는 추가적인 인프라와 복잡성을 의미합니다.

JWT는 다른 접근을 취합니다. 서버는 사용자 정보를 JSON으로 인코딩하고, 비밀 키로 서명하여 토큰을 생성합니다. 이 토큰을 클라이언트에 전달하면, 클라이언트는 이후 요청마다 `Authorization: Bearer <token>` 헤더로 토큰을 전송합니다. 서버는 서명을 검증하여 토큰이 위조되지 않았음을 확인하고, 토큰 내부의 정보(claims)를 신뢰합니다.

핵심은 **서버가 상태를 저장하지 않는다**는 것입니다. 토큰 자체가 모든 필요한 정보를 담고 있으므로, 어떤 서버가 요청을 받든 세션 저장소를 조회할 필요가 없습니다. 이는 수평 확장을 단순하게 만듭니다.

```
JWT 구조:
header.payload.signature

예:
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c

디코딩하면:
Header: { "alg": "HS256", "typ": "JWT" }
Payload: { "sub": "1234567890", "name": "John Doe", "iat": 1516239022 }
Signature: HMACSHA256(base64(header) + "." + base64(payload), secret)
```

하지만 JWT에도 단점이 있습니다. 토큰을 발급한 후에는 **취소할 수 없습니다**. 토큰은 만료 시간(`exp` claim)까지 유효하므로, 사용자를 즉시 로그아웃시킬 방법이 없습니다. 이를 해결하기 위해 짧은 수명의 Access Token과 긴 수명의 Refresh Token을 조합하는 패턴을 사용합니다.

**Refresh Token 패턴: 보안과 편의성의 균형**

사용자 경험을 위해서는 토큰이 오래 유효해야 합니다. 매 15분마다 다시 로그인하라고 요구할 수는 없습니다. 하지만 보안을 위해서는 토큰이 짧게 유효해야 합니다. 토큰이 탈취되면 공격자가 해당 시간 동안 사용자인 척할 수 있기 때문입니다.

Refresh Token은 이 딜레마를 해결합니다:

1. **Access Token**: 짧은 수명(15분), API 요청에 사용
2. **Refresh Token**: 긴 수명(7일~30일), 새 Access Token을 얻는 데만 사용

사용자가 로그인하면 두 토큰을 모두 받습니다. Access Token으로 API를 호출하다가 만료되면, Refresh Token으로 `/api/auth/refresh` 엔드포인트를 호출하여 새 Access Token을 받습니다. Refresh Token이 만료되면 그때 다시 로그인합니다.

이 방식의 장점은, Refresh Token은 서버 데이터베이스에 저장할 수 있다는 것입니다. 사용자를 즉시 로그아웃시키려면 해당 Refresh Token을 무효화하면 됩니다. Access Token은 여전히 만료될 때까지 유효하지만(최대 15분), 새로운 Access Token을 발급받을 수 없으므로 사실상 로그아웃됩니다.

**역할 기반 권한 부여(RBAC): 가장 일반적인 모델**

권한 부여의 가장 단순한 형태는 역할(Role)입니다. 사용자에게 "Admin", "Manager", "User" 같은 역할을 부여하고, 각 엔드포인트는 특정 역할을 요구합니다.

```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("api/users/{id}")]
public IActionResult DeleteUser(int id)
{
    // 관리자만 사용자를 삭제할 수 있음
}
```

이는 직관적이고 구현하기 쉽지만, 유연성이 부족합니다. "판매 관리자는 자신의 팀의 주문만 볼 수 있다"같은 복잡한 규칙은 역할만으로 표현하기 어렵습니다.

**클레임 기반 권한 부여(Claims-Based): 더 세밀한 제어**

클레임(Claim)은 사용자에 대한 사실(fact)입니다. "이메일이 확인됨", "프리미엄 구독 중", "미국 지역" 같은 것들입니다. 역할도 사실 클레임의 일종입니다(`Role: Admin`).

클레임 기반 권한 부여는 더 세밀한 제어를 가능하게 합니다.

```csharp
[Authorize(Policy = "CanManageOrders")]
[HttpGet("api/orders")]
public IActionResult GetOrders()
{
    // "CanManageOrders" 정책을 만족하는 사용자만 접근
}

// Startup.cs 또는 Program.cs에서 정책 정의
services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageOrders", policy =>
        policy.RequireClaim("Department", "Sales", "Management")
              .RequireRole("Manager"));
});
```

**리소스 기반 권한 부여: 가장 유연한 모델**

때로는 권한이 사용자의 속성뿐만 아니라 **리소스 자체**에도 의존합니다. "사용자는 자신이 작성한 게시글만 수정할 수 있다"는 규칙을 생각해보세요. 이는 사용자의 역할이나 클레임만으로 판단할 수 없으며, 실제 게시글의 작성자를 확인해야 합니다.

ASP.NET Core는 `IAuthorizationHandler`를 통해 이를 지원합니다.

```csharp
public class PostAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Post>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Post resource)
    {
        if (requirement.Name == "Edit" &&
            resource.AuthorId == context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

// 컨트롤러에서 사용
var post = await _context.Posts.FindAsync(id);
var authResult = await _authorizationService.AuthorizeAsync(User, post, "Edit");
if (!authResult.Succeeded)
{
    return Forbid();
}
```

### GraphQL: 데이터 페칭의 새로운 패러다임

REST API를 사용하다 보면 불편한 점들이 있습니다. 블로그 게시글과 작성자 정보, 댓글 목록을 모두 가져오려면 여러 요청이 필요합니다: `/api/posts/1`, `/api/users/123`, `/api/posts/1/comments`. 네트워크 왕복(round-trip)이 많아질수록 성능은 저하됩니다. 특히 모바일 네트워크에서는 지연 시간이 치명적입니다.

반대 문제도 있습니다. `/api/users/1`이 사용자의 모든 정보를 반환하는데, 실제로는 이름과 프로필 사진만 필요할 때가 있습니다. 불필요한 데이터를 전송하는 것은 대역폭 낭비이며, 모바일에서는 비용으로 직결됩니다.

GraphQL은 이 두 문제를 동시에 해결합니다. 클라이언트가 정확히 필요한 데이터 구조를 쿼리로 명시하면, 서버는 그에 맞춰 응답합니다. 하나의 요청으로 여러 리소스를 가져올 수 있으며, 각 리소스의 필요한 필드만 선택할 수 있습니다.

```graphql
# GraphQL 쿼리: 한 번의 요청으로 모든 데이터
query {
  post(id: 1) {
    title
    content
    author {
      name
      profilePicture
    }
    comments {
      text
      createdAt
      author {
        name
      }
    }
  }
}

# 응답: 요청한 구조 그대로
{
  "data": {
    "post": {
      "title": "GraphQL 소개",
      "content": "...",
      "author": {
        "name": "홍길동",
        "profilePicture": "https://..."
      },
      "comments": [
        {
          "text": "좋은 글입니다!",
          "createdAt": "2025-01-15T10:30:00Z",
          "author": { "name": "김철수" }
        }
      ]
    }
  }
}
```

이는 REST로는 불가능한 유연성입니다. 프론트엔드가 변경되어 새로운 필드가 필요해도, 백엔드 API를 수정할 필요가 없습니다. 스키마에 이미 해당 필드가 있다면, 쿼리만 조정하면 됩니다.

**GraphQL의 타입 시스템: 자동 검증과 문서화**

GraphQL의 핵심은 강력한 타입 시스템입니다. 스키마는 가능한 모든 쿼리와 각 필드의 타입을 정의합니다.

```graphql
type Post {
  id: ID!
  title: String!
  content: String!
  author: User!
  comments: [Comment!]!
  createdAt: DateTime!
}

type User {
  id: ID!
  name: String!
  email: String!
  posts: [Post!]!
}

type Query {
  post(id: ID!): Post
  posts(first: Int, after: String): PostConnection!
  user(id: ID!): User
}
```

`!`는 null이 아님을 의미하며, `[Comment!]!`는 "null이 아닌 Comment들의 null이 아닌 배열"을 의미합니다. 이는 TypeScript보다 더 정확한 타입 표현입니다.

이 스키마는 자동으로 문서가 됩니다. GraphQL Playground나 Apollo Studio 같은 도구는 스키마를 읽어 자동완성, 검증, 문서화를 제공합니다. 프론트엔드 개발자는 별도의 API 문서를 읽을 필요 없이, 쿼리를 작성하며 바로 가능한 필드를 확인할 수 있습니다.

**N+1 문제: GraphQL의 숨겨진 함정**

GraphQL은 강력하지만, 잘못 구현하면 성능 재앙이 될 수 있습니다. 가장 흔한 문제는 N+1 쿼리입니다.

```graphql
query {
  posts {
    title
    author {
      name
    }
  }
}
```

단순해 보이는 이 쿼리가 어떻게 실행될까요? 먼저 모든 게시글을 가져오는 쿼리(`SELECT * FROM Posts`)가 실행됩니다. 10개의 게시글이 있다면, 이제 각 게시글의 작성자를 가져오는 쿼리가 10번 실행됩니다(`SELECT * FROM Users WHERE Id = ?`). 총 11번의 쿼리—이것이 N+1 문제입니다.

100개의 게시글이 있다면? 101번의 쿼리. 데이터베이스가 버티지 못합니다.

해결책은 **DataLoader**입니다. DataLoader는 같은 요청 내에서 데이터베이스 호출을 배칭(batching)하고 캐싱합니다. 위 쿼리는 2번의 쿼리로 최적화됩니다:

1. `SELECT * FROM Posts`
2. `SELECT * FROM Users WHERE Id IN (1, 2, 3, ...)`

Hot Chocolate(ASP.NET Core의 GraphQL 라이브러리)은 DataLoader를 내장하고 있으며, 간단한 설정만으로 N+1 문제를 해결할 수 있습니다.

**GraphQL vs REST: 언제 무엇을 선택할까?**

GraphQL이 모든 면에서 REST보다 우월한 것은 아닙니다. 각각의 적합한 사용 사례가 있습니다.

**GraphQL을 선택하세요:**
- 모바일 앱이나 대역폭이 제한된 환경
- 다양한 클라이언트(웹, 모바일, 태블릿)가 서로 다른 데이터를 요구
- 프론트엔드가 빠르게 변화하며 백엔드 수정을 최소화하고 싶을 때
- 복잡한 중첩 관계의 데이터를 자주 조회
- 강타입 API가 중요한 프로젝트

**REST를 선택하세요:**
- 단순한 CRUD 작업이 주를 이룰 때
- HTTP 캐싱을 적극 활용하고 싶을 때 (GraphQL은 POST를 주로 사용하므로 캐싱이 어려움)
- 파일 업로드/다운로드가 많을 때
- 서드파티와의 통합이 중요할 때 (REST가 더 보편적)
- 팀이 GraphQL에 익숙하지 않을 때 (학습 곡선이 있음)

많은 조직은 하이브리드 접근을 취합니다. 주요 API는 GraphQL로, 파일 업로드나 웹훅 같은 특수 케이스는 REST로 제공하는 식입니다.

### SignalR: 실시간 웹의 단순함

WebSocket을 직접 구현해본 적이 있나요? 연결 관리, 재연결 로직, 메시지 직렬화, 에러 처리... 생각보다 복잡합니다. 게다가 모든 브라우저가 WebSocket을 지원하는 것도 아니며, 일부 방화벽과 프록시는 WebSocket 연결을 차단합니다.

SignalR은 이 모든 복잡성을 숨깁니다. 실시간 양방향 통신을 위한 고수준 추상화로, 가능한 최선의 전송 방식을 자동으로 선택합니다. WebSocket이 가능하면 WebSocket을, 아니면 Server-Sent Events를, 그것도 안 되면 Long Polling을 사용합니다. 개발자는 이를 신경 쓸 필요 없이, 단순히 메서드를 호출하고 이벤트를 수신하면 됩니다.

```csharp
// 서버: SignalR Hub
public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        // 모든 연결된 클라이언트에게 메시지 전송
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

// 클라이언트 (JavaScript)
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.on("ReceiveMessage", (user, message) => {
    // 메시지 수신 시 처리
    console.log(`${user}: ${message}`);
});

connection.start();
connection.invoke("SendMessage", "홍길동", "안녕하세요!");
```

이 코드는 완전한 실시간 채팅을 구현합니다. 연결 관리, 재연결, 에러 처리는 SignalR이 알아서 합니다.

**SignalR의 유연한 메시징: 누구에게 보낼 것인가**

SignalR의 강력함은 메시징의 유연성에 있습니다.

- `Clients.All`: 모든 연결된 클라이언트에게
- `Clients.Caller`: 메서드를 호출한 클라이언트에게만
- `Clients.Others`: 호출자를 제외한 모든 클라이언트에게
- `Clients.Client(connectionId)`: 특정 연결에게
- `Clients.User(userId)`: 특정 사용자의 모든 연결에게 (여러 탭/기기)
- `Clients.Group(groupName)`: 특정 그룹의 모든 클라이언트에게

그룹 기능은 특히 강력합니다. 채팅방, 알림 채널, 실시간 협업 세션을 그룹으로 모델링할 수 있습니다.

```csharp
public class ChatHub : Hub
{
    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("UserJoined", Context.User.Identity.Name);
    }

    public async Task SendMessageToRoom(string roomName, string message)
    {
        await Clients.Group(roomName).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
    }
}
```

### Part 6에서 배울 내용

이제 여러분은 API 개발의 전체 스펙트럼을 이해하게 될 것입니다. REST의 원칙과 실전, 보안의 다층적 접근, GraphQL의 혁신적 데이터 페칭, SignalR의 실시간 통신—각각은 독립적으로도 가치 있지만, 함께 사용될 때 진정한 힘을 발휘합니다.

**Chapter 14: RESTful API 설계와 구현**

REST의 이론부터 실전까지 모든 것을 배웁니다. 리소스 중심 설계, HTTP 메서드의 의미론, 상태 코드의 적절한 사용, URL 구조 설계, API 버전 관리, CORS 설정, OpenAPI/Swagger를 통한 자동 문서화... Express.js로 API를 만들어본 경험이 있다면, ASP.NET Core의 강타입 접근이 얼마나 생산성을 높이는지 놀라게 될 것입니다.

컨트롤러 기반 API와 Minimal APIs를 모두 다루며, 각각의 장단점과 적합한 사용 사례를 이해합니다. 모델 바인딩, 유효성 검사, 응답 형식화, Content Negotiation—모든 것이 프레임워크에 내장되어 있으며, 일관된 방식으로 작동합니다.

실습에서는 완전한 전자상거래 REST API를 만들며, 제품 목록, 검색, 필터링, 페이징, 정렬을 구현합니다. OpenAPI 문서는 자동으로 생성되며, Swagger UI로 바로 테스트할 수 있습니다.

**Chapter 15: API 보안과 인증**

보안은 복잡하지만 피할 수 없는 주제입니다. 이 챕터에서는 JWT 인증을 처음부터 구현하며, 토큰 생성, 검증, Refresh Token 패턴을 마스터합니다. ASP.NET Core Identity로 사용자 등록, 로그인, 비밀번호 해싱, 이메일 확인, 2단계 인증을 구현합니다.

OAuth 2.0과 OpenID Connect를 통해 Google, Facebook, Microsoft 계정으로 로그인하는 기능을 추가합니다. 역할 기반, 클레임 기반, 정책 기반, 리소스 기반 권한 부여를 모두 다루며, 각각의 적합한 사용 사례를 이해합니다.

Rate Limiting으로 무차별 대입 공격을 방지하고, HTTPS를 강제하며, OWASP Top 10 보안 취약점을 하나씩 대응합니다. 실습에서는 보안이 완전히 적용된 API를 만들며, 인증된 사용자만 자신의 리소스를 관리할 수 있도록 합니다.

**Chapter 16: GraphQL과 SignalR**

Hot Chocolate 라이브러리로 GraphQL API를 처음부터 만듭니다. 스키마 정의, Query와 Mutation, Subscription(실시간 업데이트), DataLoader를 통한 N+1 문제 해결, 페이징, 필터링, 정렬... GraphQL의 모든 측면을 다룹니다.

SignalR로 실시간 채팅 애플리케이션을 만들며, Hub 개념, 클라이언트-서버 통신, 그룹 관리, 연결 수명 주기를 이해합니다. React, Vue, Angular 클라이언트와의 통합도 다루며, 실시간 알림, 대시보드, 협업 도구를 구현하는 패턴을 배웁니다.

실습에서는 GraphQL과 SignalR을 결합하여, 실시간 협업 도구를 만듭니다. 사용자가 문서를 편집하면 다른 사용자에게 즉시 반영되며, GraphQL로 복잡한 데이터를 효율적으로 가져옵니다.

## 학습 목표

Part 6를 마치면 다음을 할 수 있습니다:

- RESTful API 설계 원칙을 이해하고 적용할 수 있습니다
- 적절한 HTTP 메서드와 상태 코드를 사용하여 일관된 API를 만듭니다
- 컨트롤러 기반 API와 Minimal APIs를 모두 구현할 수 있습니다
- API 버전 관리 전략을 적용합니다
- OpenAPI/Swagger로 API를 자동으로 문서화합니다
- JWT 기반 인증을 처음부터 구현할 수 있습니다
- OAuth 2.0으로 외부 로그인을 통합합니다
- 역할, 클레임, 정책 기반 권한 부여를 적용합니다
- GraphQL 스키마를 설계하고 구현할 수 있습니다
- DataLoader로 N+1 쿼리 문제를 해결합니다
- SignalR Hub를 만들어 실시간 양방향 통신을 구현합니다
- OWASP Top 10 보안 취약점을 이해하고 방어할 수 있습니다
- Rate Limiting, CORS, HTTPS를 적절히 설정합니다

## 챕터 구성

### [Chapter 14: RESTful API 설계와 구현](./chapter14/)

REST의 원칙부터 실전까지, 프론트엔드 개발자가 사용하기 좋은 API를 만드는 모든 방법을 배웁니다.

- REST 아키텍처 원칙과 제약 조건
- 리소스 중심 설계: URL 구조와 명명 규칙
- HTTP 메서드의 의미론: GET, POST, PUT, PATCH, DELETE
- 상태 코드의 적절한 사용: 2xx, 4xx, 5xx
- 컨트롤러 기반 API: `[ApiController]`, `[Route]`
- Minimal APIs: Express.js 스타일의 간결함
- 모델 바인딩: `[FromBody]`, `[FromQuery]`, `[FromRoute]`, `[FromHeader]`
- 응답 형식화: Content Negotiation, JSON 직렬화
- API 버전 관리: URL, 헤더, 쿼리 문자열 기반
- OpenAPI/Swagger 통합: 자동 문서 생성
- CORS 구성: SPA와의 안전한 통신
- 유효성 검사: Data Annotations, FluentValidation
- 에러 처리: ProblemDetails 표준

**핵심 개념**: REST 원칙, HTTP 시맨틱, API 설계 패턴, OpenAPI, CORS

**실습**: 전자상거래 REST API - 제품 목록/상세/생성/수정/삭제, 검색, 필터링, 페이징, 정렬

### [Chapter 15: API 보안과 인증](./chapter15/)

보안을 아키텍처의 기초부터 고려하여, 안전한 API를 만드는 모든 기법을 배웁니다.

- 인증 vs 권한 부여: 개념과 차이점
- JWT(JSON Web Token): 구조, 생성, 검증
- Refresh Token 패턴: Access Token + Refresh Token
- ASP.NET Core Identity: 사용자 관리 프레임워크
- 비밀번호 해싱: PBKDF2, Bcrypt, Argon2
- OAuth 2.0과 OpenID Connect: 외부 로그인 통합
- Google, Facebook, Microsoft 계정 로그인
- 역할 기반 권한 부여(RBAC): `[Authorize(Roles = "Admin")]`
- 클레임 기반 권한 부여: 정책(Policy) 시스템
- 리소스 기반 권한 부여: `IAuthorizationHandler`
- HTTPS 강제: HSTS, SSL/TLS
- Rate Limiting: 무차별 대입 공격 방지
- API 키 인증: 서드파티 통합
- OWASP Top 10: SQL Injection, XSS, CSRF, 등

**핵심 개념**: JWT, OAuth 2.0, ASP.NET Core Identity, 권한 부여 정책, Rate Limiting

**실습**: 보안이 적용된 블로그 API - JWT 로그인, Refresh Token, 역할별 권한, 소유자만 수정/삭제 가능

### [Chapter 16: GraphQL, gRPC, SignalR](./chapter16/)

데이터 페칭의 혁신(GraphQL)과 실시간 통신의 단순함(SignalR)을 마스터합니다.

- GraphQL 소개: REST의 한계와 GraphQL의 해결책
- Hot Chocolate 라이브러리: ASP.NET Core용 GraphQL
- 스키마 정의: Type, Query, Mutation, Subscription
- Resolver: 데이터를 가져오는 로직
- DataLoader: N+1 쿼리 문제 해결
- 페이징: Cursor-based, Offset-based
- 필터링과 정렬: 동적 쿼리 구성
- Mutation: 데이터 수정 작업
- Subscription: 실시간 업데이트 (WebSocket)
- GraphQL vs REST: 언제 무엇을 선택할까?
- SignalR 소개: 실시간 양방향 통신
- Hub: SignalR의 핵심 개념
- 클라이언트-서버 메시지 전송
- 그룹: 채널별 메시지 관리
- JavaScript, React, Angular 클라이언트 통합
- 실시간 알림, 채팅, 대시보드 패턴

**핵심 개념**: GraphQL 스키마, Resolver, DataLoader, SignalR Hub, 실시간 통신

**실습**: GraphQL API로 복잡한 쿼리 구현 + SignalR 실시간 채팅 + 실시간 협업 도구

## 실습 프로젝트

각 챕터에는 실전 프로젝트가 포함되어 있습니다.

### Chapter 14 실습: 전자상거래 REST API

완전한 RESTful API를 처음부터 만듭니다:
- 제품 목록 조회 (페이징, 정렬, 필터링)
- 제품 상세 조회
- 제품 생성/수정/삭제 (관리자 전용)
- 카테고리별 제품 조회
- 검색 기능 (이름, 설명, 카테고리)
- API 버전 관리 (v1, v2)
- Swagger UI를 통한 대화형 문서
- CORS 설정으로 React 앱과 통합
- 유효성 검사 및 에러 처리

### Chapter 15 실습: 보안이 적용된 블로그 API

인증과 권한 부여를 완전히 구현합니다:
- 사용자 등록 (이메일 확인)
- JWT 로그인 및 Refresh Token
- Google OAuth 로그인
- 게시글 CRUD (작성자만 수정/삭제 가능)
- 댓글 기능 (인증된 사용자만 작성 가능)
- 관리자 영역 (모든 게시글 관리 가능)
- Rate Limiting (로그인 시도 제한)
- HTTPS 강제 및 보안 헤더
- API 키를 통한 서드파티 통합

### Chapter 16 실습: 실시간 협업 도구

GraphQL과 SignalR을 결합한 현대적인 애플리케이션:
- GraphQL API로 문서, 사용자, 댓글 조회
- 복잡한 중첩 쿼리 (문서 + 작성자 + 댓글 + 댓글 작성자)
- DataLoader로 N+1 문제 해결
- Mutation으로 문서 생성/수정
- SignalR Hub로 실시간 협업
  - 사용자가 문서를 편집하면 다른 사용자에게 즉시 반영
  - 현재 편집 중인 사용자 표시
  - 실시간 커서 위치 공유
- React 클라이언트와 통합
- GraphQL Playground로 API 탐색

## API 설계 모범 사례 체크리스트

Part 6를 학습하며 다음 원칙들을 내재화하세요:

**RESTful 설계:**
- [ ] URL은 리소스를 나타내고, 동사가 아닌 명사를 사용
- [ ] HTTP 메서드로 동작을 표현 (GET=조회, POST=생성, PUT=전체 수정, PATCH=부분 수정, DELETE=삭제)
- [ ] 상태 코드를 의미에 맞게 사용
- [ ] 복수형 명사 사용 (`/api/users`, not `/api/user`)
- [ ] 계층 구조를 URL로 표현 (`/api/users/123/posts`)
- [ ] 필터링, 정렬, 페이징은 쿼리 문자열 사용
- [ ] 버전 관리 전략 수립
- [ ] HATEOAS 고려 (선택적)

**보안:**
- [ ] 모든 민감한 엔드포인트에 인증 적용
- [ ] 권한 부여로 리소스 접근 제어
- [ ] HTTPS 강제 (프로덕션)
- [ ] Rate Limiting으로 남용 방지
- [ ] CORS를 적절히 구성
- [ ] SQL Injection 방지 (파라미터화된 쿼리)
- [ ] XSS 방지 (출력 인코딩)
- [ ] CSRF 방지 (Anti-forgery 토큰)
- [ ] 민감한 정보를 로그에 기록하지 않기
- [ ] 에러 메시지에서 내부 정보 노출하지 않기

**GraphQL:**
- [ ] 스키마를 명확하고 일관되게 설계
- [ ] Null 가능성을 명시 (`!` 사용)
- [ ] DataLoader로 N+1 문제 해결
- [ ] 쿼리 복잡도 제한 (무한 중첩 방지)
- [ ] 페이징 구현 (큰 리스트 조회 시)
- [ ] 인증과 권한 부여를 Resolver 레벨에서 적용

**SignalR:**
- [ ] Hub 메서드는 빠르게 실행되어야 함 (긴 작업은 백그라운드로)
- [ ] 재연결 로직 구현 (클라이언트)
- [ ] 에러 처리 및 로깅
- [ ] 확장성 고려 (Azure SignalR Service, Redis backplane)
- [ ] 인증된 사용자만 연결 허용
- [ ] 그룹 관리 주의 (메모리 누수 방지)

## 다음 단계

Part 6를 마치면, 여러분은 프로덕션급 API를 설계하고 구축할 수 있게 됩니다. REST의 원칙, 보안의 다층적 접근, GraphQL의 혁신, SignalR의 실시간성—이 모든 것을 마스터했습니다.

**Part 7**에서는 API를 넘어, 프론트엔드와 백엔드를 연결하는 고급 패턴을 배웁니다. 타입 안전한 API 클라이언트 생성, 실시간 통신 패턴, 백그라운드 작업 처리, 캐싱 전략... 완전한 풀스택 개발자로 성장하는 여정이 계속됩니다.

지금 바로 Chapter 14로 이동하여, 첫 RESTful API를 만들어보세요!

---

## 참고 자료

- [RESTful API 설계 가이드](https://docs.microsoft.com/aspnet/core/web-api/)
- [ASP.NET Core 보안 문서](https://docs.microsoft.com/aspnet/core/security/)
- [Hot Chocolate GraphQL](https://chillicream.com/docs/hotchocolate/)
- [SignalR 공식 문서](https://docs.microsoft.com/aspnet/core/signalr/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [JWT 소개](https://jwt.io/introduction)
- [OAuth 2.0 명세](https://oauth.net/2/)

**예상 학습 시간**: 3-4주 (각 챕터당 7-10일, 실습 포함)
