---
title: "Chapter 15 - API 보안과 인증 - 안전한 API 구축하기"
---

# Chapter 15: API 보안과 인증 - 안전한 API 구축하기

## 보안은 선택이 아닌 필수

Chapter 14에서 RESTful API를 구축하는 방법을 배웠습니다. 하지만 인증과 권한 부여 없이 공개된 API는 재앙입니다. 누구나 데이터를 읽고, 수정하고, 삭제할 수 있습니다. 악의적인 사용자는 시스템을 남용하고, 민감한 정보를 탈취하며, 서비스를 마비시킬 수 있습니다.

2023년, API 관련 보안 사고는 전체 데이터 유출의 83%를 차지했습니다 (Gartner). API는 애플리케이션의 가장 취약한 진입점이며, 가장 많이 공격받는 대상입니다. Peloton은 인증되지 않은 API로 사용자 데이터가 노출되었고, Facebook은 부적절한 권한 관리로 수백만 사용자의 정보가 유출되었습니다.

보안은 "나중에 추가"할 수 있는 기능이 아닙니다. 처음부터 설계되어야 합니다. 이 챕터에서는 ASP.NET Core API를 안전하게 만드는 모든 방법을 다룹니다: JWT 인증, OAuth 2.0, 역할 및 클레임 기반 권한 부여, API 키, Rate Limiting 등.

## JWT (JSON Web Token) 인증: 현대적이고 무상태적인 인증

JWT는 현대 API 인증의 사실상 표준입니다. 서버는 상태를 유지하지 않으며 (stateless), 토큰 자체가 모든 정보를 포함합니다. 이는 확장성과 마이크로서비스 아키텍처에 이상적입니다.

### JWT 구조 이해하기

JWT는 세 부분으로 구성됩니다: **Header.Payload.Signature**

**Header** (헤더):
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```
알고리즘 (HS256, RS256 등)과 타입을 지정합니다.

**Payload** (페이로드):
```json
{
  "sub": "1234567890",
  "name": "John Doe",
  "email": "john@example.com",
  "role": "Admin",
  "exp": 1735689600,
  "iat": 1735686000
}
```
클레임(claims)이라 불리는 사용자 정보를 포함합니다. 표준 클레임 (`sub`, `exp`, `iat`)과 커스텀 클레임을 사용할 수 있습니다.

**Signature** (서명):
```
HMACSHA256(
  base64UrlEncode(header) + "." + base64UrlEncode(payload),
  secret
)
```
헤더와 페이로드를 비밀 키로 서명하여, 토큰이 변조되지 않았음을 보장합니다.

**최종 JWT:**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwicm9sZSI6IkFkbWluIiwiZXhwIjoxNzM1Njg5NjAwfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

### ASP.NET Core에서 JWT 인증 구현

**1. NuGet 패키지 설치:**
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
```

**2. JWT 설정 (appsettings.json):**
```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "https://yourapp.com",
    "Audience": "https://yourapp.com",
    "ExpiryMinutes": 60
  }
}
```

**중요**: 프로덕션에서는 `SecretKey`를 환경 변수나 Key Vault에 저장하세요!

**3. JWT 서비스 구성 (Program.cs):**
```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT 설정 바인딩
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

// Authentication 서비스 추가
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // 만료 시간 정확히 적용
    };

    // JWT 이벤트 (선택적)
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token validated for {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// 미들웨어 순서 중요!
app.UseAuthentication(); // 먼저
app.UseAuthorization();  // 그 다음

app.MapControllers();
app.Run();
```

**4. JWT 토큰 생성 서비스:**
```csharp
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role) // 역할 추가
        };

        // 커스텀 클레임 추가 가능
        if (!string.IsNullOrEmpty(user.Department))
            claims.Add(new Claim("department", user.Department));

        if (!int.TryParse(jwtSettings["ExpiryMinutes"], out var expiryMinutes))
            expiryMinutes = 60;

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false // 만료된 토큰도 검증 (Refresh Token 시나리오)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}

// Program.cs에 등록
builder.Services.AddScoped<ITokenService, TokenService>();
```

**5. 로그인 엔드포인트:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public AuthController(ITokenService tokenService, IUserService userService)
    {
        _tokenService = tokenService;
        _userService = userService;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        // 사용자 검증
        var user = await _userService.ValidateCredentialsAsync(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Access Token 생성
        var accessToken = _tokenService.GenerateAccessToken(user);

        // Refresh Token 생성
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Refresh Token 저장 (데이터베이스)
        await _userService.SaveRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600, // 60분
            TokenType = "Bearer"
        });
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        // 만료된 Access Token에서 사용자 정보 추출
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return Unauthorized(new { message = "Invalid access token" });

        var userId = int.Parse(principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "0");

        // Refresh Token 검증
        var isValidRefreshToken = await _userService.ValidateRefreshTokenAsync(userId, request.RefreshToken);
        if (!isValidRefreshToken)
            return Unauthorized(new { message = "Invalid refresh token" });

        // 사용자 정보 조회
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        // 새 토큰 생성
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // 새 Refresh Token 저장
        await _userService.SaveRefreshTokenAsync(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(7));

        return Ok(new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 3600,
            TokenType = "Bearer"
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "0");
        await _userService.RevokeRefreshTokensAsync(userId);
        return Ok(new { message = "Logged out successfully" });
    }
}
```

**6. 보호된 엔드포인트:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // 전체 컨트롤러에 인증 필요
public class ProductsController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous] // 이 엔드포인트는 인증 불필요
    public IActionResult GetProducts()
    {
        return Ok(new[] { "Product 1", "Product 2" });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Admin 역할만 접근 가능
    public IActionResult CreateProduct([FromBody] CreateProductDto dto)
    {
        // 현재 사용자 정보 접근
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var userName = User.Identity?.Name;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // 제품 생성 로직
        return CreatedAtAction(nameof(GetProduct), new { id = 1 }, dto);
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        return Ok(new { id, name = "Product 1" });
    }
}
```

**7. 클라이언트에서 JWT 사용:**
```javascript
// 프론트엔드 (React, Vue 등)
// 1. 로그인
const response = await fetch('https://api.example.com/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email: 'user@example.com', password: 'password' })
});

const { accessToken, refreshToken } = await response.json();

// 2. 로컬 스토리지에 저장
localStorage.setItem('accessToken', accessToken);
localStorage.setItem('refreshToken', refreshToken);

// 3. API 요청 시 헤더에 포함
const productsResponse = await fetch('https://api.example.com/api/products', {
  headers: {
    'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
  }
});

// 4. 401 에러 시 Refresh Token으로 갱신
if (productsResponse.status === 401) {
  const refreshResponse = await fetch('https://api.example.com/api/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      accessToken: localStorage.getItem('accessToken'),
      refreshToken: localStorage.getItem('refreshToken')
    })
  });

  const { accessToken: newAccessToken, refreshToken: newRefreshToken } = await refreshResponse.json();
  localStorage.setItem('accessToken', newAccessToken);
  localStorage.setItem('refreshToken', newRefreshToken);

  // 원래 요청 재시도
}
```

### JWT 취소 메커니즘

JWT의 주요 단점 중 하나는 한번 발급되면 만료 시간까지 유효하다는 것입니다. 사용자가 로그아웃하거나 계정이 비활성화되어도 토큰은 여전히 작동합니다. 이를 해결하기 위한 방법이 **토큰 블랙리스트**입니다.

**블랙리스트 인터페이스:**
```csharp
public interface ITokenBlacklist
{
    Task RevokeTokenAsync(string jti, DateTime expiry);
    Task<bool> IsRevokedAsync(string jti);
}

public class RedisTokenBlacklist : ITokenBlacklist
{
    private readonly IConnectionMultiplexer _redis;

    public RedisTokenBlacklist(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task RevokeTokenAsync(string jti, DateTime expiry)
    {
        var db = _redis.GetDatabase();
        var ttl = expiry - DateTime.UtcNow;

        if (ttl > TimeSpan.Zero)
        {
            await db.StringSetAsync($"blacklist:{jti}", "revoked", ttl);
        }
    }

    public async Task<bool> IsRevokedAsync(string jti)
    {
        var db = _redis.GetDatabase();
        return await db.KeyExistsAsync($"blacklist:{jti}");
    }
}
```

**JWT 검증 시 블랙리스트 확인:**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    // ... 기존 설정 ...

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var tokenBlacklist = context.HttpContext.RequestServices
                .GetRequiredService<ITokenBlacklist>();

            var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (jti != null && await tokenBlacklist.IsRevokedAsync(jti))
            {
                context.Fail("Token has been revoked");
            }
        }
    };
});
```

**로그아웃 시 토큰 블랙리스트 추가:**
```csharp
[HttpPost("logout")]
[Authorize]
public async Task<IActionResult> Logout()
{
    var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
    var exp = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

    if (jti != null && exp != null)
    {
        var expiryDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).DateTime;
        await _tokenBlacklist.RevokeTokenAsync(jti, expiryDate);
    }

    var userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "0");
    await _userService.RevokeRefreshTokensAsync(userId);

    return Ok(new { message = "Logged out successfully" });
}
```

**주의사항:**
- 블랙리스트는 메모리나 Redis 같은 빠른 저장소에 보관해야 합니다
- 만료된 토큰은 자동으로 블랙리스트에서 제거됩니다 (TTL 활용)
- 짧은 Access Token 만료 시간을 사용하면 블랙리스트 크기를 줄일 수 있습니다

### JWT 보안 모범 사례

**✅ 해야 할 것:**
- HTTPS 사용 (토큰 가로채기 방지)
- 짧은 만료 시간 (15-60분)
- Refresh Token 사용 (장기 세션 유지)
- 강력한 비밀 키 (최소 256비트)
- 토큰에 민감한 정보 넣지 않기 (JWT는 암호화되지 않음, Base64 인코딩만)

**❌ 하지 말아야 할 것:**
- LocalStorage에 Refresh Token 저장 (XSS 공격 위험, HttpOnly 쿠키 사용 권장)
- GET 요청에 토큰 포함 (URL 로그에 노출)
- 만료 검증 생략
- 약한 비밀 키 사용

## ASP.NET Core Identity: 완전한 사용자 관리 시스템

JWT는 토큰 생성/검증만 다룹니다. 사용자 등록, 비밀번호 해싱, 이메일 확인, 2FA 등은 직접 구현해야 합니다. **ASP.NET Core Identity**는 이 모든 것을 제공하는 완전한 회원 관리 시스템입니다.

### Identity 설정

**1. NuGet 패키지:**
```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

**2. 사용자 엔티티 정의:**
```csharp
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser<int> // int를 ID 타입으로 사용
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

**3. DbContext 수정:**
```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // 다른 DbSet들...
}
```

**4. Identity 구성 (Program.cs):**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    // 비밀번호 정책
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // 잠금 정책
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // 사용자 설정
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true; // 이메일 확인 필수
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders(); // 이메일 확인, 비밀번호 재설정 토큰
```

**5. 마이그레이션:**
```bash
dotnet ef migrations add AddIdentity
dotnet ef database update
```

### 회원가입과 로그인

```csharp
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        // 사용자 생성
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            // 프로덕션: 보안을 위해 일반적인 메시지 사용
            // return BadRequest(new { message = "Registration failed" });

            // 개발: 자세한 에러 정보 제공
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        // 역할 할당
        await _userManager.AddToRoleAsync(user, "User");

        // 이메일 확인 토큰 생성
        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // 이메일 전송 (실제 구현 필요)
        var confirmationLink = Url.Action(
            nameof(ConfirmEmail),
            "Account",
            new { userId = user.Id, token = emailConfirmationToken },
            Request.Scheme);

        await _emailService.SendEmailAsync(
            user.Email,
            "Confirm your email",
            $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>");

        return Ok(new RegisterResponse
        {
            Message = "User registered successfully. Please check your email to confirm your account.",
            UserId = user.Id
        });
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(int userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
            return Ok(new { message = "Email confirmed successfully" });

        return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        // 이메일 확인 여부 체크
        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Unauthorized(new { message = "Email not confirmed" });

        // 비밀번호 검증
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Refresh Token 저장
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 3600,
                TokenType = "Bearer"
            });
        }

        if (result.IsLockedOut)
            return Unauthorized(new { message = "Account locked due to multiple failed login attempts" });

        return Unauthorized(new { message = "Invalid credentials" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // 보안상 사용자 존재 여부를 노출하지 않음
            return Ok(new { message = "If the email exists, a password reset link has been sent" });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink = Url.Action(
            nameof(ResetPassword),
            "Account",
            new { email = user.Email, token },
            Request.Scheme);

        await _emailService.SendEmailAsync(
            user.Email,
            "Reset your password",
            $"Reset your password by clicking <a href='{resetLink}'>here</a>");

        return Ok(new { message = "If the email exists, a password reset link has been sent" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(new { message = "Invalid request" });

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (result.Succeeded)
            return Ok(new { message = "Password reset successfully" });

        return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
    }
}
```

### 2FA (Two-Factor Authentication)

```csharp
[HttpPost("enable-2fa")]
[Authorize]
public async Task<ActionResult<Enable2FAResponse>> Enable2FA()
{
    var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    var user = await _userManager.FindByIdAsync(userId);

    if (user == null)
        return NotFound();

    // Authenticator 키 생성
    var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
    if (string.IsNullOrEmpty(unformattedKey))
    {
        await _userManager.ResetAuthenticatorKeyAsync(user);
        unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
    }

    // QR 코드용 URI 생성
    var email = await _userManager.GetEmailAsync(user);
    var authenticatorUri = $"otpauth://totp/MyApp:{email}?secret={unformattedKey}&issuer=MyApp&digits=6";

    return Ok(new Enable2FAResponse
    {
        SharedKey = unformattedKey,
        AuthenticatorUri = authenticatorUri
    });
}

[HttpPost("verify-2fa")]
[Authorize]
public async Task<IActionResult> Verify2FA([FromBody] Verify2FARequest request)
{
    var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    var user = await _userManager.FindByIdAsync(userId);

    if (user == null)
        return NotFound();

    // 코드 검증
    var isValid = await _userManager.VerifyTwoFactorTokenAsync(
        user,
        _userManager.Options.Tokens.AuthenticatorTokenProvider,
        request.Code);

    if (isValid)
    {
        await _userManager.SetTwoFactorEnabledAsync(user, true);
        return Ok(new { message = "2FA enabled successfully" });
    }

    return BadRequest(new { message = "Invalid code" });
}
```

## 권한 부여 패턴: 누가 무엇을 할 수 있는가

인증 (Authentication)은 "당신이 누구인가"를 확인합니다. 권한 부여 (Authorization)는 "당신이 무엇을 할 수 있는가"를 결정합니다.

### 역할 기반 권한 부여 (RBAC)

가장 간단한 방법입니다. 사용자에게 역할 (Admin, User, Manager)을 할당하고, 역할별로 접근을 제어합니다.

```csharp
// 역할 생성
var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
if (!await roleManager.RoleExistsAsync("Admin"))
    await roleManager.CreateAsync(new IdentityRole<int>("Admin"));

if (!await roleManager.RoleExistsAsync("User"))
    await roleManager.CreateAsync(new IdentityRole<int>("User"));

// 사용자에게 역할 할당
await _userManager.AddToRoleAsync(user, "Admin");

// 엔드포인트 보호
[Authorize(Roles = "Admin")]
[HttpDelete("api/products/{id}")]
public IActionResult DeleteProduct(int id)
{
    // Admin만 접근 가능
}

[Authorize(Roles = "Admin,Manager")]
[HttpPut("api/products/{id}")]
public IActionResult UpdateProduct(int id)
{
    // Admin 또는 Manager 접근 가능
}
```

### 클레임 기반 권한 부여

클레임은 사용자에 대한 정보 조각입니다. 역할보다 세밀한 제어가 가능합니다.

```csharp
// 클레임 추가
await _userManager.AddClaimAsync(user, new Claim("Department", "IT"));
await _userManager.AddClaimAsync(user, new Claim("EmployeeNumber", "12345"));
await _userManager.AddClaimAsync(user, new Claim("CanEditProducts", "true"));

// 클레임 기반 권한 부여
[Authorize(Policy = "CanEditProducts")]
[HttpPut("api/products/{id}")]
public IActionResult UpdateProduct(int id)
{
    // CanEditProducts 클레임이 있는 사용자만 접근
}

// Program.cs에서 정책 정의
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanEditProducts", policy =>
        policy.RequireClaim("CanEditProducts", "true"));

    options.AddPolicy("ITDepartment", policy =>
        policy.RequireClaim("Department", "IT"));

    options.AddPolicy("SeniorEmployee", policy =>
        policy.RequireAssertion(context =>
        {
            var employeeNumber = context.User.FindFirst("EmployeeNumber")?.Value;
            return employeeNumber != null && int.Parse(employeeNumber) < 10000;
        }));
});
```

### 정책 기반 권한 부여

더 복잡한 로직을 정책으로 캡슐화합니다.

```csharp
// 요구사항 정의
public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }

    public MinimumAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}

// 핸들러 구현
public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumAgeRequirement requirement)
    {
        var dateOfBirthClaim = context.User.FindFirst(c => c.Type == "DateOfBirth");

        if (dateOfBirthClaim == null)
            return Task.CompletedTask;

        var dateOfBirth = DateTime.Parse(dateOfBirthClaim.Value);
        var age = DateTime.Today.Year - dateOfBirth.Year;

        if (dateOfBirth > DateTime.Today.AddYears(-age))
            age--;

        if (age >= requirement.MinimumAge)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

// Program.cs에 등록
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AtLeast18", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(18)));
});

// 사용
[Authorize(Policy = "AtLeast18")]
[HttpPost("api/orders/alcohol")]
public IActionResult OrderAlcohol()
{
    // 18세 이상만 접근 가능
}
```

### 리소스 기반 권한 부여

"이 사용자가 *이 특정 리소스*를 수정할 수 있는가?" 같은 질문에 답합니다.

```csharp
// 요구사항
public class SameAuthorRequirement : IAuthorizationRequirement { }

// 핸들러
public class DocumentAuthorizationHandler : AuthorizationHandler<SameAuthorRequirement, Document>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameAuthorRequirement requirement,
        Document resource)
    {
        var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (resource.AuthorId.ToString() == userId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

// 컨트롤러에서 사용
[HttpPut("api/documents/{id}")]
public async Task<IActionResult> UpdateDocument(int id, [FromBody] UpdateDocumentDto dto)
{
    var document = await _context.Documents.FindAsync(id);
    if (document == null)
        return NotFound();

    // 리소스 기반 권한 부여
    var authResult = await _authorizationService.AuthorizeAsync(
        User,
        document,
        new SameAuthorRequirement());

    if (!authResult.Succeeded)
        return Forbid();

    // 업데이트 로직
    return Ok();
}
```

## API 보안 모범 사례 체크리스트

### 인증과 권한
- [ ] JWT 비밀 키는 환경 변수나 Key Vault에 저장
- [ ] Access Token 만료 시간은 짧게 (15-60분)
- [ ] Refresh Token 사용으로 장기 세션 지원
- [ ] HTTPS 강제 (모든 통신 암호화)
- [ ] 비밀번호 해싱 (Identity는 자동)
- [ ] 2FA 지원

### 입력 검증
- [ ] 모든 입력 유효성 검사 (Data Annotations, FluentValidation)
- [ ] SQL Injection 방지 (파라미터화된 쿼리, EF Core 사용)
- [ ] XSS 방지 (출력 인코딩, CSP 헤더)
- [ ] CSRF 방지 (SameSite 쿠키, Anti-forgery 토큰)

### Rate Limiting
- [ ] API Rate Limiting 구현 (.NET 9 기능 활용)
- [ ] IP 기반 및 사용자 기반 제한
- [ ] 429 Too Many Requests 응답

### 로깅과 모니터링
- [ ] 모든 인증 시도 로깅
- [ ] 권한 부여 실패 로깅
- [ ] 이상 패턴 감지 (brute force, 비정상적 접근)
- [ ] 민감한 정보 로그에서 제외

## 요약

이 챕터에서 API 보안의 모든 측면을 다루었습니다:
- JWT로 무상태 인증 구현
- ASP.NET Core Identity로 완전한 회원 관리
- OAuth 2.0으로 외부 제공자 통합
- 역할, 클레임, 정책 기반 권한 부여
- 리소스 기반 권한 부여
- 보안 모범 사례

보안은 한 번 설정하고 끝나는 것이 아닙니다. 지속적인 업데이트와 모니터링이 필요합니다. 다음 Chapter 16에서는 GraphQL과 SignalR로 더 고급 API 패턴을 탐구합니다.

## 참고 자료

- [ASP.NET Core Security](https://docs.microsoft.com/aspnet/core/security/)
- [JWT.io](https://jwt.io/)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [OAuth 2.0](https://oauth.net/2/)
- [OWASP API Security](https://owasp.org/www-project-api-security/)
