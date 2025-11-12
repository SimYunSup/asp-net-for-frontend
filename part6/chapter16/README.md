# Chapter 16: GraphQL, gRPC, SignalR - 현대적 통신 패러다임

## REST를 넘어서: 세 가지 혁신적 접근

REST API는 지난 20년간 웹 개발의 표준이었습니다. 단순하고, 직관적이며, HTTP의 본질을 활용한 아키텍처입니다. 하지만 소프트웨어 요구사항이 진화하면서, REST만으로는 해결하기 어려운 문제들이 등장했습니다. 모바일 환경에서의 네트워크 효율성, 마이크로서비스 간의 고성능 통신, 실시간 양방향 데이터 전송—각각은 서로 다른 해결책을 필요로 합니다.

이 챕터에서는 REST의 한계를 넘어서는 세 가지 기술을 마스터합니다. **GraphQL**은 클라이언트가 필요한 데이터를 정확히 요청할 수 있게 하여 과소/과다 페칭 문제를 해결합니다. **gRPC**는 Protocol Buffers와 HTTP/2를 기반으로 REST보다 훨씬 빠른 마이크로서비스 간 통신을 가능하게 합니다. **SignalR**은 WebSocket을 추상화하여 실시간 양방향 통신을 단순하게 만듭니다.

세 기술은 서로 경쟁하는 것이 아니라 보완적입니다. GraphQL은 외부 클라이언트(웹, 모바일)를 위한 공개 API에 적합하고, gRPC는 내부 서비스 간 통신에 이상적이며, SignalR은 실시간 업데이트가 필요한 곳에서 빛을 발합니다. 현대적인 애플리케이션은 이 모든 것을 적재적소에 사용합니다.

## GraphQL: 클라이언트 중심의 데이터 페칭

### REST의 한계: 과소 페칭과 과다 페칭

프론트엔드 개발자로서 여러분은 이런 경험이 있을 것입니다. 사용자 프로필 페이지를 만드는데, 필요한 데이터는 사용자 이름, 프로필 사진, 최근 게시글 5개입니다. REST API는 어떻게 응답할까요?

**시나리오 1: 과다 페칭(Over-fetching)**

```
GET /api/users/123
```

응답에는 사용자의 *모든* 정보가 포함됩니다: 이름, 이메일, 전화번호, 주소, 생년월일, 가입일, 마지막 로그인 시각, 설정 객체, 권한 목록... 하지만 실제로 필요한 것은 이름과 프로필 사진뿐입니다. 나머지 90%의 데이터는 네트워크 대역폭을 낭비하고, JSON 파싱 시간을 늘리며, 모바일에서는 데이터 요금으로 직결됩니다.

**시나리오 2: 과소 페칭(Under-fetching)**

필요한 최근 게시글은 별도의 엔드포인트입니다:

```
GET /api/users/123/posts?limit=5
```

이제 두 번의 HTTP 요청이 필요합니다. 각 요청은 TCP 핸드셰이크, TLS 협상, HTTP 헤더 전송을 동반합니다. 모바일 네트워크에서 왕복 시간(RTT)이 100ms라면, 두 요청은 최소 200ms가 걸립니다. 게시글마다 작성자 정보도 필요하다면? 추가로 5번의 요청. 총 7번의 왕복으로 700ms—거의 1초입니다.

REST는 이 딜레마에서 벗어날 방법이 없습니다. 엔드포인트를 더 세분화하면 요청 횟수가 늘어나고, 통합하면 불필요한 데이터가 늘어납니다. 물론 특정 화면을 위한 커스텀 엔드포인트(`/api/users/123/profile-summary`)를 만들 수 있지만, 이는 프론트엔드의 모든 화면마다 백엔드 엔드포인트를 추가해야 함을 의미합니다. 프론트엔드가 변경될 때마다 백엔드도 수정—이는 확장 가능한 방식이 아닙니다.

### GraphQL의 혁신: 클라이언트가 스키마를 쿼리한다

GraphQL은 근본적으로 다른 접근을 취합니다. 서버는 가능한 모든 데이터와 관계를 **스키마**로 정의하고, 클라이언트는 필요한 것만 **쿼리**로 요청합니다.

```graphql
query {
  user(id: 123) {
    name
    profilePicture
    posts(limit: 5) {
      title
      createdAt
    }
  }
}
```

이 하나의 요청으로 정확히 필요한 데이터만 받습니다:

```json
{
  "data": {
    "user": {
      "name": "홍길동",
      "profilePicture": "https://cdn.example.com/avatar.jpg",
      "posts": [
        { "title": "GraphQL 소개", "createdAt": "2025-01-15T10:00:00Z" },
        { "title": "gRPC vs REST", "createdAt": "2025-01-14T15:30:00Z" }
      ]
    }
  }
}
```

과다 페칭도, 과소 페칭도 없습니다. 요청 구조가 응답 구조와 정확히 일치합니다. 프론트엔드 개발자가 프로필 사진 대신 배너 이미지를 표시하고 싶다면? 쿼리만 수정하면 됩니다. 백엔드 변경 없이. 스키마에 이미 `bannerImage` 필드가 있다면 즉시 사용 가능합니다.

### GraphQL의 타입 시스템: 컴파일 타임 안전성

GraphQL은 강타입 스키마 언어를 사용합니다. 모든 타입, 필드, 관계가 명시적으로 정의됩니다.

```graphql
type User {
  id: ID!
  name: String!
  email: String!
  profilePicture: String
  posts(limit: Int, offset: Int): [Post!]!
  createdAt: DateTime!
}

type Post {
  id: ID!
  title: String!
  content: String!
  author: User!
  comments: [Comment!]!
  createdAt: DateTime!
  updatedAt: DateTime
}

type Query {
  user(id: ID!): User
  users(first: Int, after: String): UserConnection!
  post(id: ID!): Post
  posts(authorId: ID, first: Int): [Post!]!
}

type Mutation {
  createPost(input: CreatePostInput!): Post!
  updatePost(id: ID!, input: UpdatePostInput!): Post!
  deletePost(id: ID!): Boolean!
}
```

`!`는 null이 아님(non-nullable)을 의미합니다. `String!`은 항상 문자열이지만, `String`은 null일 수 있습니다. `[Post!]!`은 "null이 아닌 Post들의 null이 아닌 배열"입니다. 배열 자체도 null이 아니고, 배열의 각 요소도 null이 아닙니다. 이는 TypeScript보다도 정확한 타입 표현입니다.

이 스키마는 자동으로 **문서**가 됩니다. GraphQL Playground, Apollo Studio, Altair 같은 도구들은 스키마를 읽어 자동완성, 실시간 검증, 대화형 문서를 제공합니다. API 문서를 별도로 작성하고 동기화할 필요가 없습니다. 스키마가 곧 진실의 단일 소스(Single Source of Truth)입니다.

### Hot Chocolate: ASP.NET Core의 GraphQL 라이브러리

ASP.NET Core에서 GraphQL을 구현하는 가장 강력한 라이브러리는 **Hot Chocolate**입니다. Code-First와 Schema-First 접근을 모두 지원하며, .NET의 타입 시스템과 완벽하게 통합됩니다.

**Code-First 접근**: C# 클래스에서 스키마 생성

```csharp
// 엔티티 모델
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public DateTime CreatedAt { get; set; }

    // 내비게이션 프로퍼티
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// Query 타입 정의
public class Query
{
    public async Task<User?> GetUser(
        int id,
        [Service] AppDbContext context)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUsers([Service] AppDbContext context)
    {
        return context.Users;
    }

    public async Task<Post?> GetPost(
        int id,
        [Service] AppDbContext context)
    {
        return await context.Posts
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}

// Mutation 타입 정의
public class Mutation
{
    public async Task<Post> CreatePost(
        CreatePostInput input,
        [Service] AppDbContext context,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var post = new Post
        {
            Title = input.Title,
            Content = input.Content,
            AuthorId = userId,
            CreatedAt = DateTime.UtcNow
        };

        context.Posts.Add(post);
        await context.SaveChangesAsync();

        return post;
    }

    public async Task<Post> UpdatePost(
        int id,
        UpdatePostInput input,
        [Service] AppDbContext context,
        ClaimsPrincipal claimsPrincipal)
    {
        var post = await context.Posts.FindAsync(id);
        if (post == null) throw new GraphQLException("Post not found");

        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (post.AuthorId != userId)
            throw new GraphQLException("Unauthorized");

        post.Title = input.Title ?? post.Title;
        post.Content = input.Content ?? post.Content;
        post.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return post;
    }
}

// Input 타입
public record CreatePostInput(string Title, string Content);
public record UpdatePostInput(string? Title, string? Content);

// Program.cs에서 설정
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();

app.MapGraphQL();
```

이 코드는 완전한 GraphQL API를 생성합니다. Hot Chocolate은 C# 타입을 분석하여 자동으로 GraphQL 스키마를 만듭니다. `IQueryable<User>`는 필터링, 정렬, 페이징을 지원하는 쿼리가 되며, `UsePaging`, `UseFiltering`, `UseSorting` 어트리뷰트로 이를 활성화합니다.

### DataLoader: N+1 쿼리 문제 해결

GraphQL의 가장 큰 함정은 **N+1 쿼리 문제**입니다. 다음 쿼리를 생각해보세요:

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

순진한 구현은 이렇게 작동합니다:

1. 모든 게시글을 가져옴: `SELECT * FROM Posts` (1번의 쿼리)
2. 각 게시글의 작성자를 가져옴: `SELECT * FROM Users WHERE Id = ?` (N번의 쿼리)

10개의 게시글이 있다면 11번의 쿼리, 100개라면 101번의 쿼리. 데이터베이스가 감당할 수 없습니다.

**DataLoader**는 이 문제를 해결합니다. 같은 요청 내에서 데이터 로드를 배칭(batching)하고 캐싱합니다:

```csharp
public class UserDataLoader : BatchDataLoader<int, User>
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public UserDataLoader(
        IDbContextFactory<AppDbContext> dbContextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<int, User>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // 한 번에 모든 사용자를 가져옴
        var users = await context.Users
            .Where(u => keys.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        return users;
    }
}

// Post 타입 확장
[ExtendObjectType(typeof(Post))]
public class PostResolvers
{
    public async Task<User> GetAuthor(
        [Parent] Post post,
        UserDataLoader userDataLoader)
    {
        return await userDataLoader.LoadAsync(post.AuthorId);
    }
}

// Program.cs에서 등록
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<PostResolvers>()
    .AddDataLoader<UserDataLoader>();
```

이제 쿼리는 2번의 데이터베이스 호출로 최적화됩니다:

1. `SELECT * FROM Posts`
2. `SELECT * FROM Users WHERE Id IN (1, 2, 3, ...)`

DataLoader는 자동으로 요청을 수집하고, 배치로 실행하며, 같은 요청 내에서 결과를 캐시합니다. 같은 사용자가 여러 게시글의 작성자라면, 한 번만 로드되고 재사용됩니다.

### Subscription: 실시간 GraphQL

GraphQL의 **Subscription**은 실시간 데이터 업데이트를 위한 메커니즘입니다. WebSocket을 통해 서버에서 클라이언트로 데이터를 푸시합니다.

```csharp
public class Subscription
{
    [Subscribe]
    [Topic("PostCreated")]
    public Post OnPostCreated([EventMessage] Post post) => post;

    [Subscribe]
    [Topic("CommentAdded_{postId}")]
    public Comment OnCommentAdded(
        int postId,
        [EventMessage] Comment comment) => comment;
}

public class Mutation
{
    public async Task<Post> CreatePost(
        CreatePostInput input,
        [Service] AppDbContext context,
        [Service] ITopicEventSender eventSender,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var post = new Post
        {
            Title = input.Title,
            Content = input.Content,
            AuthorId = userId,
            CreatedAt = DateTime.UtcNow
        };

        context.Posts.Add(post);
        await context.SaveChangesAsync();

        // Subscription 트리거
        await eventSender.SendAsync("PostCreated", post);

        return post;
    }
}

// Program.cs에서 Subscription 활성화
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddInMemorySubscriptions();

app.UseWebSockets();
app.MapGraphQL();
```

클라이언트는 새 게시글을 실시간으로 받을 수 있습니다:

```graphql
subscription {
  onPostCreated {
    id
    title
    author {
      name
    }
    createdAt
  }
}
```

게시글이 생성될 때마다, 구독 중인 모든 클라이언트에게 즉시 푸시됩니다.

### GraphQL의 보안: 인증과 권한 부여

GraphQL에서 보안은 Resolver 레벨에서 적용됩니다. Hot Chocolate은 ASP.NET Core의 인증/권한 부여 시스템과 완벽하게 통합됩니다.

```csharp
public class Query
{
    [Authorize] // 인증 필요
    public async Task<User> GetMe(
        [Service] AppDbContext context,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return await context.Users.FindAsync(userId);
    }

    [Authorize(Roles = "Admin")] // 관리자만
    public IQueryable<User> GetAllUsers([Service] AppDbContext context)
    {
        return context.Users;
    }
}

public class Mutation
{
    [Authorize]
    public async Task<Post> UpdatePost(
        int id,
        UpdatePostInput input,
        [Service] AppDbContext context,
        [Service] IAuthorizationService authorizationService,
        ClaimsPrincipal claimsPrincipal)
    {
        var post = await context.Posts.FindAsync(id);
        if (post == null) throw new GraphQLException("Post not found");

        // 리소스 기반 권한 부여
        var authResult = await authorizationService.AuthorizeAsync(
            claimsPrincipal, post, "EditPost");

        if (!authResult.Succeeded)
            throw new GraphQLException("Unauthorized");

        post.Title = input.Title ?? post.Title;
        post.Content = input.Content ?? post.Content;
        post.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return post;
    }
}
```

## gRPC: 고성능 마이크로서비스 통신

### REST의 또 다른 한계: 성능과 타입 안전성

REST는 사람이 읽을 수 있는 JSON을 HTTP/1.1로 전송합니다. 이는 개발과 디버깅을 쉽게 하지만, 성능에는 한계가 있습니다. JSON은 텍스트 기반이므로 파싱이 느리고, HTTP/1.1은 요청당 하나의 TCP 연결을 사용하므로 다중 요청에서 비효율적입니다.

마이크로서비스 아키텍처에서 이는 심각한 문제가 됩니다. 하나의 사용자 요청이 내부적으로 수십 개의 서비스 간 호출을 유발합니다. 각 호출마다 JSON 직렬화/역직렬화, HTTP 헤더 파싱, TCP 연결 관리... 지연 시간이 누적되며 시스템 전체의 성능을 저하시킵니다.

또 다른 문제는 **타입 안전성 부재**입니다. REST API를 호출하는 서비스는 응답 구조를 추측해야 합니다. TypeScript 인터페이스를 수동으로 작성하거나, OpenAPI 스키마에서 생성할 수 있지만, 이는 런타임에 검증되지 않습니다. 서버가 응답 구조를 변경하면, 클라이언트는 프로덕션에서 에러를 만날 때까지 알 수 없습니다.

### gRPC의 혁신: Protocol Buffers + HTTP/2

**gRPC**(Google Remote Procedure Call)는 Google이 내부 마이크로서비스를 위해 개발하고 2015년에 오픈소스로 공개한 RPC 프레임워크입니다. 두 가지 핵심 기술을 기반으로 합니다:

**1. Protocol Buffers (Protobuf)**: 바이너리 직렬화 형식

JSON과 달리 Protobuf는 바이너리입니다. 데이터를 컴팩트하게 인코딩하여 크기가 작고, 파싱이 빠릅니다. 벤치마크에 따르면 JSON보다 3-10배 빠르며, 크기는 절반 이하입니다.

더 중요한 것은 **스키마 기반**이라는 점입니다. `.proto` 파일에 메시지 구조를 정의하면, 컴파일러가 강타입 코드를 생성합니다. C#, Java, Go, Python, JavaScript 등 모든 언어에서 같은 스키마를 사용하여, 컴파일 타임 타입 안전성을 보장합니다.

```protobuf
// user.proto
syntax = "proto3";

package myapp;

message User {
  int32 id = 1;
  string name = 2;
  string email = 3;
  string profile_picture = 4;
  google.protobuf.Timestamp created_at = 5;
}

message GetUserRequest {
  int32 id = 1;
}

message GetUserResponse {
  User user = 1;
}

service UserService {
  rpc GetUser(GetUserRequest) returns (GetUserResponse);
  rpc ListUsers(ListUsersRequest) returns (ListUsersResponse);
  rpc CreateUser(CreateUserRequest) returns (User);
}
```

이 `.proto` 파일에서 C# 코드가 자동 생성됩니다. `User`, `GetUserRequest`, `GetUserResponse` 클래스와 `UserService` 기본 클래스가 포함됩니다.

**2. HTTP/2**: 다중화와 스트리밍

HTTP/1.1은 요청-응답 모델입니다. 하나의 요청이 완료될 때까지 같은 연결로 다른 요청을 보낼 수 없습니다(HOL blocking). HTTP/2는 다중화(multiplexing)를 지원하여, 하나의 TCP 연결로 여러 요청을 동시에 처리합니다.

또한 HTTP/2는 스트리밍을 지원합니다. 클라이언트가 여러 요청을 스트림으로 보내거나, 서버가 여러 응답을 스트림으로 반환하거나, 양방향 스트리밍도 가능합니다. 이는 REST로는 불가능한 패턴입니다.

### gRPC의 네 가지 통신 패턴

gRPC는 REST의 단순한 요청-응답을 넘어, 네 가지 통신 패턴을 지원합니다:

**1. Unary RPC**: 단일 요청, 단일 응답 (REST와 유사)

```protobuf
rpc GetUser(GetUserRequest) returns (User);
```

```csharp
// 서버 구현
public class UserService : UserServiceBase
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<User> GetUser(
        GetUserRequest request,
        ServerCallContext context)
    {
        var user = await _context.Users.FindAsync(request.Id);
        if (user == null)
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        return new User
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            ProfilePicture = user.ProfilePicture ?? "",
            CreatedAt = Timestamp.FromDateTime(user.CreatedAt.ToUniversalTime())
        };
    }
}

// 클라이언트 호출
var client = new UserService.UserServiceClient(channel);
var response = await client.GetUserAsync(new GetUserRequest { Id = 123 });
Console.WriteLine($"User: {response.Name}");
```

**2. Server Streaming RPC**: 단일 요청, 스트림 응답

서버가 데이터를 여러 번에 걸쳐 전송합니다. 큰 데이터를 분할하거나, 진행 상황을 보고하는 데 유용합니다.

```protobuf
rpc ListUsers(ListUsersRequest) returns (stream User);
```

```csharp
// 서버 구현
public override async Task ListUsers(
    ListUsersRequest request,
    IServerStreamWriter<User> responseStream,
    ServerCallContext context)
{
    var users = _context.Users.AsAsyncEnumerable();

    await foreach (var user in users.WithCancellation(context.CancellationToken))
    {
        await responseStream.WriteAsync(new User
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        });
    }
}

// 클라이언트 호출
var call = client.ListUsers(new ListUsersRequest());
await foreach (var user in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"User: {user.Name}");
}
```

**3. Client Streaming RPC**: 스트림 요청, 단일 응답

클라이언트가 데이터를 여러 번 전송하고, 서버는 모두 받은 후 하나의 응답을 반환합니다. 파일 업로드나 배치 작업에 유용합니다.

```protobuf
rpc CreateUsers(stream CreateUserRequest) returns (CreateUsersResponse);
```

```csharp
// 서버 구현
public override async Task<CreateUsersResponse> CreateUsers(
    IAsyncStreamReader<CreateUserRequest> requestStream,
    ServerCallContext context)
{
    int count = 0;

    await foreach (var request in requestStream.ReadAllAsync())
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        count++;
    }

    await _context.SaveChangesAsync();

    return new CreateUsersResponse { CreatedCount = count };
}

// 클라이언트 호출
var call = client.CreateUsers();
foreach (var userData in userDataList)
{
    await call.RequestStream.WriteAsync(new CreateUserRequest
    {
        Name = userData.Name,
        Email = userData.Email
    });
}
await call.RequestStream.CompleteAsync();
var response = await call;
Console.WriteLine($"Created {response.CreatedCount} users");
```

**4. Bidirectional Streaming RPC**: 스트림 요청, 스트림 응답

클라이언트와 서버가 동시에 데이터를 주고받습니다. 실시간 채팅, 게임 서버, 라이브 스트리밍에 이상적입니다.

```protobuf
rpc Chat(stream ChatMessage) returns (stream ChatMessage);
```

```csharp
// 서버 구현
public override async Task Chat(
    IAsyncStreamReader<ChatMessage> requestStream,
    IServerStreamWriter<ChatMessage> responseStream,
    ServerCallContext context)
{
    // 백그라운드에서 다른 클라이언트의 메시지를 전달
    var broadcastTask = Task.Run(async () =>
    {
        await foreach (var message in _messageHub.Subscribe())
        {
            await responseStream.WriteAsync(message);
        }
    });

    // 클라이언트의 메시지를 받아 브로드캐스트
    await foreach (var message in requestStream.ReadAllAsync())
    {
        await _messageHub.Publish(message);
    }

    await broadcastTask;
}

// 클라이언트 호출
var call = client.Chat();

// 메시지 수신
var receiveTask = Task.Run(async () =>
{
    await foreach (var message in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"{message.User}: {message.Text}");
    }
});

// 메시지 전송
await call.RequestStream.WriteAsync(new ChatMessage
{
    User = "홍길동",
    Text = "안녕하세요!"
});

await call.RequestStream.CompleteAsync();
await receiveTask;
```

### ASP.NET Core에서 gRPC 서비스 구현

ASP.NET Core는 gRPC를 일급 시민으로 지원합니다. Kestrel 웹 서버가 HTTP/2를 네이티브로 지원하므로, 별도의 프록시 없이 gRPC를 호스팅할 수 있습니다.

**1. .proto 파일 정의**

```protobuf
// Protos/product.proto
syntax = "proto3";

option csharp_namespace = "MyApp.Grpc";

package product;

import "google/protobuf/timestamp.proto";
import "google/protobuf/empty.proto";

message Product {
  int32 id = 1;
  string name = 2;
  string description = 3;
  double price = 4;
  int32 stock = 5;
  google.protobuf.Timestamp created_at = 6;
}

message GetProductRequest {
  int32 id = 1;
}

message ListProductsRequest {
  int32 page_size = 1;
  int32 page = 2;
  string category = 3;
}

message ListProductsResponse {
  repeated Product products = 1;
  int32 total_count = 2;
}

message CreateProductRequest {
  string name = 1;
  string description = 2;
  double price = 3;
  int32 stock = 4;
}

service ProductService {
  rpc GetProduct(GetProductRequest) returns (Product);
  rpc ListProducts(ListProductsRequest) returns (ListProductsResponse);
  rpc CreateProduct(CreateProductRequest) returns (Product);
  rpc DeleteProduct(GetProductRequest) returns (google.protobuf.Empty);

  // 스트리밍: 재고 업데이트 실시간 모니터링
  rpc WatchStock(google.protobuf.Empty) returns (stream Product);
}
```

**2. .csproj에 Protobuf 추가**

```xml
<ItemGroup>
  <Protobuf Include="Protos\product.proto" GrpcServices="Server" />
</ItemGroup>

<ItemGroup>
  <PackageReference Include="Grpc.AspNetCore" Version="2.60.0" />
</ItemGroup>
```

빌드 시 자동으로 C# 코드가 생성됩니다.

**3. 서비스 구현**

```csharp
public class ProductService : ProductServiceBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task<Product> GetProduct(
        GetProductRequest request,
        ServerCallContext context)
    {
        var product = await _context.Products.FindAsync(request.Id);

        if (product == null)
        {
            throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Product with ID {request.Id} not found"));
        }

        return MapToGrpcProduct(product);
    }

    public override async Task<ListProductsResponse> ListProducts(
        ListProductsRequest request,
        ServerCallContext context)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(request.Category))
        {
            query = query.Where(p => p.Category == request.Category);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var response = new ListProductsResponse
        {
            TotalCount = totalCount
        };

        response.Products.AddRange(products.Select(MapToGrpcProduct));

        return response;
    }

    public override async Task<Product> CreateProduct(
        CreateProductRequest request,
        ServerCallContext context)
    {
        var product = new ProductEntity
        {
            Name = request.Name,
            Description = request.Description,
            Price = (decimal)request.Price,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created product {ProductId}: {ProductName}",
            product.Id, product.Name);

        return MapToGrpcProduct(product);
    }

    public override async Task<Empty> DeleteProduct(
        GetProductRequest request,
        ServerCallContext context)
    {
        var product = await _context.Products.FindAsync(request.Id);

        if (product == null)
        {
            throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Product with ID {request.Id} not found"));
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return new Empty();
    }

    public override async Task WatchStock(
        Empty request,
        IServerStreamWriter<Product> responseStream,
        ServerCallContext context)
    {
        // 실시간 재고 업데이트를 스트리밍
        // 실제로는 메시지 큐나 Change Data Capture를 사용
        while (!context.CancellationToken.IsCancellationRequested)
        {
            var lowStockProducts = await _context.Products
                .Where(p => p.Stock < 10)
                .ToListAsync();

            foreach (var product in lowStockProducts)
            {
                await responseStream.WriteAsync(MapToGrpcProduct(product));
            }

            await Task.Delay(TimeSpan.FromSeconds(5), context.CancellationToken);
        }
    }

    private static Product MapToGrpcProduct(ProductEntity entity)
    {
        return new Product
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = (double)entity.Price,
            Stock = entity.Stock,
            CreatedAt = Timestamp.FromDateTime(entity.CreatedAt.ToUniversalTime())
        };
    }
}
```

**4. Program.cs에서 등록**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

var app = builder.Build();

app.MapGrpcService<ProductService>();

// gRPC reflection (개발 환경에서 유용)
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();
```

### gRPC 클라이언트: 타입 안전한 호출

gRPC 클라이언트도 같은 `.proto` 파일에서 생성됩니다. 타입 안전하며, IntelliSense가 완벽하게 작동합니다.

```csharp
// 클라이언트 프로젝트 .csproj
<ItemGroup>
  <Protobuf Include="Protos\product.proto" GrpcServices="Client" />
</ItemGroup>

// 클라이언트 코드
var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new ProductService.ProductServiceClient(channel);

// Unary 호출
var product = await client.GetProductAsync(new GetProductRequest { Id = 123 });
Console.WriteLine($"Product: {product.Name}, Price: {product.Price}");

// 페이징된 목록
var listResponse = await client.ListProductsAsync(new ListProductsRequest
{
    Page = 1,
    PageSize = 10,
    Category = "Electronics"
});

Console.WriteLine($"Total: {listResponse.TotalCount} products");
foreach (var p in listResponse.Products)
{
    Console.WriteLine($"- {p.Name}: ${p.Price}");
}

// Server streaming
var call = client.WatchStock(new Empty());
await foreach (var lowStockProduct in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"Low stock alert: {lowStockProduct.Name} ({lowStockProduct.Stock} left)");
}
```

### gRPC vs REST: 언제 무엇을 선택할까?

gRPC가 성능에서 우월하지만, 모든 경우에 적합한 것은 아닙니다.

**gRPC를 선택하세요:**

- **마이크로서비스 간 내부 통신**: 가장 이상적인 사용 사례입니다. 높은 처리량, 낮은 지연 시간, 강타입 계약이 모두 중요합니다.
- **실시간 스트리밍**: 양방향 스트리밍이 필요한 경우 (채팅, 게임, IoT 텔레메트리).
- **폴리글랏 환경**: 여러 언어로 작성된 서비스가 통신해야 하며, 타입 안전성이 중요할 때.
- **모바일 클라이언트**: 네트워크 효율성이 중요하고, Protobuf의 작은 크기가 이점입니다.
- **성능이 핵심**: 밀리초 단위의 지연 시간이 중요한 금융, 게임, 실시간 분석 시스템.

**REST를 선택하세요:**

- **공개 API**: 브라우저에서 직접 호출하거나, 서드파티 통합이 필요할 때. gRPC는 브라우저 지원이 제한적입니다(gRPC-Web 필요).
- **캐싱이 중요**: HTTP 캐싱(CDN, 브라우저 캐시)을 활용하고 싶을 때.
- **디버깅과 탐색**: cURL, Postman, 브라우저 개발자 도구로 쉽게 테스트하고 싶을 때.
- **팀의 익숙함**: 팀이 REST에 익숙하고, gRPC 학습 곡선을 감수할 이유가 부족할 때.

**하이브리드 접근**: 많은 조직이 두 가지를 함께 사용합니다. 내부 서비스는 gRPC로, 공개 API는 REST나 GraphQL로 제공합니다.

### gRPC-Web: 브라우저에서 gRPC 사용하기

브라우저는 HTTP/2 Trailers를 지원하지 않아 네이티브 gRPC를 호출할 수 없습니다. **gRPC-Web**은 이를 해결하는 프로토콜로, HTTP/1.1이나 HTTP/2로 gRPC를 호출할 수 있게 합니다.

```csharp
// Program.cs
builder.Services.AddGrpc();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});

var app = builder.Build();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.UseCors();

app.MapGrpcService<ProductService>().EnableGrpcWeb();
```

TypeScript 클라이언트:

```typescript
import { ProductServiceClient } from './generated/product_grpc_web_pb';
import { GetProductRequest } from './generated/product_pb';

const client = new ProductServiceClient('https://localhost:5001', null, null);

const request = new GetProductRequest();
request.setId(123);

client.getProduct(request, {}, (err, response) => {
  if (err) {
    console.error(err);
  } else {
    console.log(`Product: ${response.getName()}, Price: ${response.getPrice()}`);
  }
});
```

## SignalR: 실시간 양방향 통신의 단순함

### WebSocket의 복잡성

실시간 기능을 구현해본 적이 있나요? 채팅, 라이브 알림, 협업 도구, 실시간 대시보드... 사용자는 페이지를 새로고침하지 않고도 최신 데이터를 보기를 기대합니다. WebSocket은 이를 위한 표준 기술이지만, 직접 구현하면 복잡합니다.

연결 관리: 사용자가 언제 연결하고 언제 끊어지는지 추적해야 합니다. 네트워크 끊김, 브라우저 탭 전환, 모바일 앱 백그라운드 전환—모든 경우를 처리해야 합니다.

재연결 로직: 네트워크가 일시적으로 끊어졌을 때 자동으로 재연결해야 합니다. Exponential backoff를 구현하여 서버를 압도하지 않으면서도 빠르게 복구해야 합니다.

메시지 직렬화: JSON? Binary? Protocol Buffers? 일관된 형식을 선택하고 양쪽에서 동일하게 구현해야 합니다.

브라우저 호환성: 모든 브라우저가 WebSocket을 지원하지 않습니다. 오래된 브라우저나 엄격한 프록시 환경에서는 폴백이 필요합니다.

### SignalR의 혁신: 추상화와 폴백

**SignalR**은 이 모든 복잡성을 숨깁니다. ASP.NET Core에 내장된 실시간 통신 라이브러리로, 다음을 제공합니다:

**자동 전송 협상**: 클라이언트와 서버가 최선의 전송 방식을 자동으로 선택합니다.

1. **WebSocket**: 가능하면 항상 선호됩니다. 양방향, 낮은 오버헤드.
2. **Server-Sent Events (SSE)**: WebSocket이 불가능하면 사용. 서버→클라이언트 단방향.
3. **Long Polling**: 모두 실패하면 폴백. HTTP 요청을 길게 열어둠.

개발자는 이를 신경 쓸 필요 없습니다. SignalR이 알아서 처리합니다.

**자동 재연결**: 연결이 끊어지면 자동으로 재연결을 시도합니다. Exponential backoff가 내장되어 있습니다.

**타입 안전한 허브**: C#의 강타입을 활용하여, 메서드 호출과 파라미터가 컴파일 타임에 검증됩니다.

**확장성**: Azure SignalR Service나 Redis backplane을 사용하여 여러 서버로 확장 가능합니다.

### SignalR Hub: 실시간 통신의 중심

SignalR의 핵심 개념은 **Hub**입니다. Hub는 클라이언트와 서버가 메서드를 호출할 수 있는 고수준 파이프라인입니다.

```csharp
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    // 클라이언트가 호출할 수 있는 메서드
    public async Task SendMessage(string user, string message)
    {
        _logger.LogInformation("{User} sent: {Message}", user, message);

        // 모든 연결된 클라이언트에게 메시지 전송
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendToUser(string targetUser, string message)
    {
        // 특정 사용자에게만 전송
        await Clients.User(targetUser).SendAsync("ReceiveMessage", "Private", message);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await Clients.Group(groupName).SendAsync(
            "SystemMessage",
            $"{Context.User?.Identity?.Name} joined {groupName}");
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        await Clients.Group(groupName).SendAsync(
            "SystemMessage",
            $"{Context.User?.Identity?.Name} left {groupName}");
    }

    public async Task SendToGroup(string groupName, string message)
    {
        await Clients.Group(groupName).SendAsync(
            "ReceiveMessage",
            Context.User?.Identity?.Name ?? "Anonymous",
            message);
    }

    // 연결 수명 주기
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}

// Program.cs
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<ChatHub>("/chatHub");
```

### SignalR 클라이언트: JavaScript/TypeScript

SignalR 클라이언트 라이브러리는 여러 언어로 제공됩니다. JavaScript/TypeScript가 가장 일반적입니다.

```typescript
import * as signalR from "@microsoft/signalr";

// 연결 생성
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", {
        accessTokenFactory: () => getJwtToken() // JWT 인증
    })
    .withAutomaticReconnect() // 자동 재연결
    .configureLogging(signalR.LogLevel.Information)
    .build();

// 서버에서 메시지 수신
connection.on("ReceiveMessage", (user: string, message: string) => {
    console.log(`${user}: ${message}`);
    addMessageToUI(user, message);
});

connection.on("SystemMessage", (message: string) => {
    console.log(`[System] ${message}`);
    addSystemMessageToUI(message);
});

// 연결 시작
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected");
    } catch (err) {
        console.error("SignalR Connection Error:", err);
        setTimeout(start, 5000); // 5초 후 재시도
    }
}

// 재연결 이벤트
connection.onreconnecting((error) => {
    console.warn("SignalR Reconnecting:", error);
    showReconnectingUI();
});

connection.onreconnected((connectionId) => {
    console.log("SignalR Reconnected:", connectionId);
    hideReconnectingUI();
});

connection.onclose((error) => {
    console.error("SignalR Connection Closed:", error);
    showDisconnectedUI();
});

start();

// 서버 메서드 호출
document.getElementById("sendButton")?.addEventListener("click", async () => {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;

    try {
        await connection.invoke("SendMessage", user, message);
    } catch (err) {
        console.error("Send Error:", err);
    }
});

// 그룹 참가
async function joinRoom(roomName: string) {
    try {
        await connection.invoke("JoinGroup", roomName);
        console.log(`Joined room: ${roomName}`);
    } catch (err) {
        console.error("Join Room Error:", err);
    }
}

// 그룹에 메시지 전송
async function sendToRoom(roomName: string, message: string) {
    try {
        await connection.invoke("SendToGroup", roomName, message);
    } catch (err) {
        console.error("Send To Group Error:", err);
    }
}
```

### 강타입 Hub: IHubContext와 강타입 클라이언트

SignalR은 강타입 Hub를 지원하여, 컴파일 타임에 메서드 시그니처를 검증할 수 있습니다.

```csharp
// 클라이언트 메서드 인터페이스
public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
    Task SystemMessage(string message);
    Task UserJoined(string user);
    Task UserLeft(string user);
}

// 강타입 Hub
public class ChatHub : Hub<IChatClient>
{
    public async Task SendMessage(string user, string message)
    {
        // 타입 안전한 호출
        await Clients.All.ReceiveMessage(user, message);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).UserJoined(Context.User?.Identity?.Name ?? "Anonymous");
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SystemMessage($"{Context.User?.Identity?.Name} connected");
        await base.OnConnectedAsync();
    }
}
```

백그라운드 서비스에서 Hub에 메시지를 보낼 수도 있습니다:

```csharp
public class NotificationService : BackgroundService
{
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;

    public NotificationService(IHubContext<ChatHub, IChatClient> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // 모든 연결된 클라이언트에게 시스템 메시지 전송
            await _hubContext.Clients.All.SystemMessage(
                $"Server time: {DateTime.UtcNow:HH:mm:ss}");

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}

// Program.cs
builder.Services.AddHostedService<NotificationService>();
```

### SignalR의 고급 기능

**1. 그룹 관리: 채널별 메시지 격리**

그룹은 연결을 논리적으로 묶는 메커니즘입니다. 채팅방, 알림 채널, 협업 세션을 모델링할 수 있습니다.

```csharp
public class ChatHub : Hub<IChatClient>
{
    private static readonly Dictionary<string, HashSet<string>> _roomUsers = new();

    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

        lock (_roomUsers)
        {
            if (!_roomUsers.ContainsKey(roomName))
                _roomUsers[roomName] = new HashSet<string>();

            _roomUsers[roomName].Add(Context.ConnectionId);
        }

        // 방의 다른 사용자에게 알림
        await Clients.Group(roomName).UserJoined(Context.User?.Identity?.Name ?? "Anonymous");

        // 현재 방 사용자 목록 전송
        var userCount = _roomUsers[roomName].Count;
        await Clients.Caller.SystemMessage($"Room '{roomName}' has {userCount} users");
    }

    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

        lock (_roomUsers)
        {
            if (_roomUsers.ContainsKey(roomName))
            {
                _roomUsers[roomName].Remove(Context.ConnectionId);
                if (_roomUsers[roomName].Count == 0)
                    _roomUsers.Remove(roomName);
            }
        }

        await Clients.Group(roomName).UserLeft(Context.User?.Identity?.Name ?? "Anonymous");
    }

    public async Task SendToRoom(string roomName, string message)
    {
        await Clients.Group(roomName).ReceiveMessage(
            Context.User?.Identity?.Name ?? "Anonymous",
            message);
    }
}
```

**2. 인증과 권한 부여**

SignalR은 ASP.NET Core의 인증 시스템과 통합됩니다.

```csharp
[Authorize] // 인증된 사용자만 연결 가능
public class SecureChatHub : Hub<IChatClient>
{
    [Authorize(Roles = "Admin")]
    public async Task BroadcastAnnouncement(string message)
    {
        await Clients.All.SystemMessage($"[Admin Announcement] {message}");
    }

    public async Task SendPrivateMessage(string targetUserId, string message)
    {
        // 특정 사용자에게만 전송
        await Clients.User(targetUserId).ReceiveMessage(
            Context.User?.Identity?.Name ?? "Anonymous",
            message);
    }
}

// Program.cs
builder.Services.AddSignalR();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* JWT 설정 */ });

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<SecureChatHub>("/secureChat");
```

클라이언트에서 JWT 토큰 전달:

```typescript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/secureChat", {
        accessTokenFactory: () => {
            // localStorage나 쿠키에서 JWT 가져오기
            return localStorage.getItem("jwt_token") || "";
        }
    })
    .withAutomaticReconnect()
    .build();
```

**3. 확장성: Redis Backplane**

여러 서버에서 SignalR을 실행할 때, 메시지를 모든 서버에 브로드캐스트해야 합니다. Redis backplane이 이를 처리합니다.

```csharp
// NuGet: Microsoft.AspNetCore.SignalR.StackExchangeRedis
builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379", options =>
    {
        options.Configuration.ChannelPrefix = "MyApp.SignalR";
    });
```

이제 사용자가 서버 A에 연결되어 있어도, 서버 B의 Hub에서 보낸 메시지를 받을 수 있습니다.

**4. Azure SignalR Service: 완전 관리형 확장**

수십만 개의 동시 연결을 처리해야 한다면, Azure SignalR Service를 사용할 수 있습니다. 코드 변경 없이 무한 확장됩니다.

```csharp
builder.Services.AddSignalR()
    .AddAzureSignalR(options =>
    {
        options.ConnectionString = builder.Configuration["Azure:SignalR:ConnectionString"];
    });
```

## 세 기술의 조화: 실전 시나리오

현대적인 애플리케이션은 이 세 기술을 함께 사용합니다. 각각의 강점을 적재적소에 활용하는 것이 핵심입니다.

### 시나리오 1: 전자상거래 플랫폼

**REST API**: 제품 목록, 주문 생성, 결제 처리—표준 CRUD 작업
**GraphQL**: 모바일 앱용 API—제품, 리뷰, 추천, 재고를 하나의 쿼리로
**gRPC**: 재고 서비스, 결제 서비스, 배송 서비스 간 내부 통신
**SignalR**: 실시간 주문 상태 업데이트, 재고 부족 알림

### 시나리오 2: 협업 도구 (Notion, Figma)

**GraphQL**: 문서, 사용자, 권한, 댓글 조회—복잡한 쿼리
**gRPC**: 검색 서비스, 파일 저장소 서비스 간 통신
**SignalR**: 실시간 동시 편집, 커서 위치 공유, 변경 사항 동기화

### 시나리오 3: 금융 거래 플랫폼

**gRPC**: 주문 실행 서비스, 리스크 관리 서비스 간 초저지연 통신
**SignalR**: 실시간 가격 피드, 주문 체결 알림
**REST**: 백오피스 관리 API, 리포트 생성

## 실습 프로젝트: 실시간 협업 문서 편집기

이제 세 기술을 모두 활용하는 프로젝트를 만들어봅시다.

### 아키텍처

- **GraphQL API**: 문서 조회, 생성, 수정 (복잡한 쿼리)
- **gRPC**: 검색 서비스와 통신 (전문 검색 엔진)
- **SignalR**: 실시간 동시 편집 (변경 사항 동기화)

### GraphQL 스키마와 구현

```csharp
public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
}

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Document> GetDocuments([Service] AppDbContext context)
    {
        return context.Documents.Include(d => d.Owner);
    }

    public async Task<Document?> GetDocument(
        int id,
        [Service] AppDbContext context)
    {
        return await context.Documents
            .Include(d => d.Owner)
            .Include(d => d.Shares)
            .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}

public class Mutation
{
    public async Task<Document> CreateDocument(
        string title,
        [Service] AppDbContext context,
        [Service] ITopicEventSender eventSender,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var document = new Document
        {
            Title = title,
            Content = "",
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Documents.Add(document);
        await context.SaveChangesAsync();

        await eventSender.SendAsync("DocumentCreated", document);

        return document;
    }

    public async Task<Document> UpdateDocument(
        int id,
        string? title,
        string? content,
        [Service] AppDbContext context,
        [Service] IHubContext<DocumentHub, IDocumentClient> hubContext,
        ClaimsPrincipal claimsPrincipal)
    {
        var document = await context.Documents.FindAsync(id);
        if (document == null) throw new GraphQLException("Document not found");

        var userId = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (document.OwnerId != userId)
            throw new GraphQLException("Unauthorized");

        if (title != null) document.Title = title;
        if (content != null) document.Content = content;
        document.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        // SignalR로 다른 사용자에게 알림
        await hubContext.Clients.Group($"document_{id}")
            .DocumentUpdated(id, content ?? document.Content);

        return document;
    }
}

public class Subscription
{
    [Subscribe]
    [Topic("DocumentCreated")]
    public Document OnDocumentCreated([EventMessage] Document document) => document;
}
```

### SignalR Hub: 실시간 편집

```csharp
public interface IDocumentClient
{
    Task DocumentUpdated(int documentId, string content);
    Task UserJoinedDocument(int documentId, string userName);
    Task UserLeftDocument(int documentId, string userName);
    Task CursorMoved(int documentId, string userName, int position);
}

[Authorize]
public class DocumentHub : Hub<IDocumentClient>
{
    private readonly AppDbContext _context;

    public DocumentHub(AppDbContext context)
    {
        _context = context;
    }

    public async Task JoinDocument(int documentId)
    {
        // 권한 확인
        var document = await _context.Documents.FindAsync(documentId);
        if (document == null) return;

        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var hasAccess = document.OwnerId == userId ||
            await _context.DocumentShares.AnyAsync(s => s.DocumentId == documentId && s.UserId == userId);

        if (!hasAccess)
            throw new HubException("Unauthorized");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"document_{documentId}");

        await Clients.Group($"document_{documentId}")
            .UserJoinedDocument(documentId, Context.User.Identity!.Name!);
    }

    public async Task LeaveDocument(int documentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"document_{documentId}");

        await Clients.Group($"document_{documentId}")
            .UserLeftDocument(documentId, Context.User!.Identity!.Name!);
    }

    public async Task SendEdit(int documentId, string content)
    {
        // 다른 사용자에게 변경 사항 전파 (자신 제외)
        await Clients.OthersInGroup($"document_{documentId}")
            .DocumentUpdated(documentId, content);

        // 데이터베이스 업데이트는 debounce되어야 함 (실제로는 별도 로직)
    }

    public async Task SendCursorPosition(int documentId, int position)
    {
        await Clients.OthersInGroup($"document_{documentId}")
            .CursorMoved(documentId, Context.User!.Identity!.Name!, position);
    }
}
```

### gRPC 검색 서비스

```protobuf
// search.proto
syntax = "proto3";

service SearchService {
  rpc SearchDocuments(SearchRequest) returns (SearchResponse);
  rpc IndexDocument(IndexRequest) returns (IndexResponse);
}

message SearchRequest {
  string query = 1;
  int32 user_id = 2;
  int32 limit = 3;
}

message SearchResponse {
  repeated DocumentResult results = 1;
}

message DocumentResult {
  int32 id = 1;
  string title = 2;
  string snippet = 3;
  float score = 4;
}

message IndexRequest {
  int32 document_id = 1;
  string title = 2;
  string content = 3;
}

message IndexResponse {
  bool success = 1;
}
```

```csharp
// 검색 서비스 구현
public class SearchService : SearchServiceBase
{
    private readonly ISearchEngine _searchEngine; // Elasticsearch, Azure Search 등

    public override async Task<SearchResponse> SearchDocuments(
        SearchRequest request,
        ServerCallContext context)
    {
        var results = await _searchEngine.SearchAsync(request.Query, request.UserId, request.Limit);

        var response = new SearchResponse();
        response.Results.AddRange(results.Select(r => new DocumentResult
        {
            Id = r.Id,
            Title = r.Title,
            Snippet = r.Snippet,
            Score = r.Score
        }));

        return response;
    }

    public override async Task<IndexResponse> IndexDocument(
        IndexRequest request,
        ServerCallContext context)
    {
        await _searchEngine.IndexAsync(request.DocumentId, request.Title, request.Content);

        return new IndexResponse { Success = true };
    }
}

// GraphQL Mutation에서 gRPC 호출
public class Mutation
{
    public async Task<Document> CreateDocument(
        string title,
        [Service] AppDbContext context,
        [Service] SearchService.SearchServiceClient searchClient)
    {
        var document = new Document { /* ... */ };
        context.Documents.Add(document);
        await context.SaveChangesAsync();

        // gRPC로 검색 인덱싱
        await searchClient.IndexDocumentAsync(new IndexRequest
        {
            DocumentId = document.Id,
            Title = document.Title,
            Content = document.Content
        });

        return document;
    }
}
```

### TypeScript 클라이언트: 모든 것의 통합

```typescript
import { ApolloClient, InMemoryCache, gql, useQuery, useMutation, useSubscription } from '@apollo/client';
import * as signalR from '@microsoft/signalr';

// GraphQL 클라이언트
const apolloClient = new ApolloClient({
  uri: 'https://localhost:5001/graphql',
  cache: new InMemoryCache(),
  headers: {
    authorization: `Bearer ${getJwtToken()}`
  }
});

// SignalR 연결
const hubConnection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:5001/documentHub', {
    accessTokenFactory: () => getJwtToken()
  })
  .withAutomaticReconnect()
  .build();

// GraphQL 쿼리
const GET_DOCUMENT = gql`
  query GetDocument($id: Int!) {
    document(id: $id) {
      id
      title
      content
      owner {
        name
        email
      }
      shares {
        user {
          name
        }
        permission
      }
    }
  }
`;

// GraphQL Mutation
const UPDATE_DOCUMENT = gql`
  mutation UpdateDocument($id: Int!, $title: String, $content: String) {
    updateDocument(id: $id, title: $title, content: $content) {
      id
      title
      content
      updatedAt
    }
  }
`;

// GraphQL Subscription
const DOCUMENT_CREATED = gql`
  subscription OnDocumentCreated {
    onDocumentCreated {
      id
      title
      owner {
        name
      }
    }
  }
`;

// React 컴포넌트
function DocumentEditor({ documentId }: { documentId: number }) {
  const [content, setContent] = useState('');

  // GraphQL로 문서 로드
  const { data, loading } = useQuery(GET_DOCUMENT, {
    variables: { id: documentId }
  });

  const [updateDocument] = useMutation(UPDATE_DOCUMENT);

  // GraphQL Subscription으로 새 문서 알림
  useSubscription(DOCUMENT_CREATED, {
    onData: ({ data }) => {
      console.log('New document:', data.onDocumentCreated);
      showNotification(`New document: ${data.onDocumentCreated.title}`);
    }
  });

  useEffect(() => {
    // SignalR 연결
    hubConnection.start().then(() => {
      hubConnection.invoke('JoinDocument', documentId);
    });

    // SignalR로 실시간 업데이트 수신
    hubConnection.on('DocumentUpdated', (docId, newContent) => {
      if (docId === documentId) {
        setContent(newContent);
      }
    });

    hubConnection.on('UserJoinedDocument', (docId, userName) => {
      console.log(`${userName} joined document ${docId}`);
      showUserJoined(userName);
    });

    hubConnection.on('CursorMoved', (docId, userName, position) => {
      showRemoteCursor(userName, position);
    });

    return () => {
      hubConnection.invoke('LeaveDocument', documentId);
    };
  }, [documentId]);

  // 로컬 편집
  const handleContentChange = (newContent: string) => {
    setContent(newContent);

    // SignalR로 실시간 전송
    hubConnection.invoke('SendEdit', documentId, newContent);

    // GraphQL로 저장 (debounced)
    debouncedSave(documentId, newContent);
  };

  const debouncedSave = useMemo(
    () => debounce((id: number, content: string) => {
      updateDocument({ variables: { id, content } });
    }, 1000),
    [updateDocument]
  );

  const handleCursorMove = (position: number) => {
    hubConnection.invoke('SendCursorPosition', documentId, position);
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div>
      <h1>{data.document.title}</h1>
      <textarea
        value={content}
        onChange={(e) => handleContentChange(e.target.value)}
        onMouseUp={(e) => handleCursorMove(e.currentTarget.selectionStart)}
      />
    </div>
  );
}
```

## 핵심 교훈

1. **GraphQL**: 클라이언트가 데이터를 제어할 때—모바일, 다양한 화면, 빠른 변화
2. **gRPC**: 성능이 핵심일 때—마이크로서비스, 내부 통신, 실시간 스트리밍
3. **SignalR**: 실시간 업데이트가 필요할 때—채팅, 알림, 협업, 대시보드
4. **하이브리드**: 현대 애플리케이션은 모든 것을 사용합니다. 적재적소에.

REST는 여전히 유효하며 많은 경우 최선의 선택입니다. 하지만 이제 여러분은 더 많은 도구를 갖게 되었습니다. 문제에 맞는 도구를 선택하는 지혜가 진정한 아키텍트를 만듭니다.

다음 챕터에서는 이 모든 API를 프론트엔드와 연결하는 고급 패턴을 배웁니다. 타입 안전한 클라이언트 생성, 캐싱 전략, 에러 처리, 재시도 로직—완전한 풀스택 개발자로의 여정이 계속됩니다.
