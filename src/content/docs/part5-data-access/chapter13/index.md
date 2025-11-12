---
title: "Chapter 13 - Entity Framework Core 고급"
---

# Chapter 13: Entity Framework Core 고급

## 프로덕션 수준의 데이터 액세스: 성능, 패턴, 최적화

Chapter 12에서 EF Core의 기초를 배웠습니다. 엔티티를 정의하고, 마이그레이션을 생성하고, LINQ로 쿼리하고, 데이터를 저장하는 방법을 익혔습니다. 이것만으로도 간단한 애플리케이션을 만들기에 충분합니다. 하지만 프로덕션 환경은 다릅니다. 수백만 개의 행, 복잡한 비즈니스 로직, 엄격한 성능 요구사항, 레거시 시스템과의 통합—이런 현실적인 문제들을 해결하려면 고급 기법이 필요합니다.

이 챕터는 EF Core의 심화 과정입니다. "작동하는 코드"에서 "효율적이고 유지보수 가능한 코드"로 진화하는 방법을 배웁니다. 프론트엔드 개발에 비유하자면, React의 기본 hooks를 배운 후 useMemo, useCallback, React.memo로 최적화하고, Suspense와 Code Splitting으로 로딩 성능을 개선하며, 복잡한 상태 관리 패턴을 적용하는 것과 같습니다.

### 복잡한 쿼리: GroupBy, Join, 그리고 그 너머

실제 애플리케이션의 쿼리는 단순한 SELECT, WHERE, ORDER BY를 넘어섭니다. 데이터를 집계하고, 여러 테이블을 조인하고, 서브쿼리를 사용하고, 통계를 계산해야 합니다. SQL로는 익숙한 작업들이지만, LINQ로 표현하는 방법을 배워야 합니다.

**GroupBy: 데이터 그룹화와 집계**

JavaScript의 `reduce()`나 lodash의 `groupBy`를 사용해본 경험이 있을 것입니다. LINQ의 `GroupBy`는 비슷하지만 훨씬 강력합니다. SQL의 `GROUP BY`와 직접 매핑되어 데이터베이스에서 집계를 수행합니다.

```csharp
// 각 태그별 게시글 수 계산
var tagStats = await context.Tags
    .Select(t => new
    {
        TagName = t.Name,
        PostCount = t.Posts.Count,
        TotalViews = t.Posts.Sum(p => p.ViewCount)
    })
    .OrderByDescending(t => t.PostCount)
    .ToListAsync();

// 생성된 SQL
// SELECT t.Name AS TagName,
//        COUNT(p.Id) AS PostCount,
//        SUM(p.ViewCount) AS TotalViews
// FROM Tags t
// LEFT JOIN Posts p ON t.Id = p.TagId
// GROUP BY t.Name
// ORDER BY PostCount DESC
```

더 복잡한 시나리오도 가능합니다. 월별 게시글 통계를 계산해봅시다:

```csharp
// 월별 게시글 수와 평균 조회수
var monthlyStats = await context.Posts
    .Where(p => p.CreatedAt >= DateTime.UtcNow.AddMonths(-12))
    .GroupBy(p => new
    {
        Year = p.CreatedAt.Year,
        Month = p.CreatedAt.Month
    })
    .Select(g => new
    {
        Year = g.Key.Year,
        Month = g.Key.Month,
        PostCount = g.Count(),
        AvgViews = g.Average(p => p.ViewCount),
        TotalComments = g.Sum(p => p.Comments.Count)
    })
    .OrderByDescending(s => s.Year)
    .ThenByDescending(s => s.Month)
    .ToListAsync();
```

이것은 JavaScript로는 다단계 `reduce`와 `map`을 중첩해야 하는 복잡한 작업입니다. LINQ는 선언적으로 표현하며, EF Core는 효율적인 SQL로 변환합니다.

**프로젝션과 익명 타입의 힘**

프로덕션 애플리케이션에서는 엔티티 전체를 로드하는 것보다 필요한 데이터만 선택하는 것이 중요합니다. 특히 대시보드나 리포트처럼 여러 테이블의 데이터를 결합할 때입니다.

```csharp
// 사용자별 활동 통계
var userStats = await context.Users
    .Select(u => new
    {
        UserId = u.Id,
        UserName = u.FirstName + " " + u.LastName,

        // 게시글 통계
        TotalPosts = u.Posts.Count,
        PublishedPosts = u.Posts.Count(p => p.IsPublished),
        TotalPostViews = u.Posts.Sum(p => p.ViewCount),

        // 댓글 통계
        TotalComments = u.Comments.Count,
        RecentComments = u.Comments
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(c => new { c.Id, c.Content, c.CreatedAt })
            .ToList(),

        // 최근 활동
        LastPostDate = u.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => p.CreatedAt)
            .FirstOrDefault(),

        LastCommentDate = u.Comments
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.CreatedAt)
            .FirstOrDefault(),

        // 평균 게시글 길이
        AvgPostLength = u.Posts.Any()
            ? u.Posts.Average(p => p.Content.Length)
            : 0
    })
    .Where(s => s.TotalPosts > 0 || s.TotalComments > 0)
    .OrderByDescending(s => s.TotalPostViews)
    .Take(20)
    .ToListAsync();
```

이 단일 쿼리는 여러 집계를 수행하고, 서브쿼리를 사용하며, 조건부 로직을 포함합니다. JavaScript에서는 여러 API 호출을 병렬로 실행하고 결과를 조합해야 할 작업을, LINQ는 하나의 표현식으로 처리합니다.

**명시적 Join: 복잡한 관계 처리**

대부분의 경우 Navigation Property와 `Include`로 충분하지만, 때로는 명시적 JOIN이 필요합니다. 특히 다대다 관계의 중간 테이블에 접근하거나, 여러 조건으로 조인할 때입니다.

```csharp
// 게시글과 댓글을 조인하여 작성자별 통계
var authorCommentStats = await context.Posts
    .Join(
        context.Comments,
        post => post.Id,
        comment => comment.PostId,
        (post, comment) => new
        {
            PostTitle = post.Title,
            CommentAuthor = comment.AuthorName,
            CommentContent = comment.Content,
            CommentDate = comment.CreatedAt
        }
    )
    .GroupBy(x => x.CommentAuthor)
    .Select(g => new
    {
        Author = g.Key,
        CommentCount = g.Count(),
        PostsCommentedOn = g.Select(x => x.PostTitle).Distinct().Count()
    })
    .OrderByDescending(x => x.CommentCount)
    .ToListAsync();
```

LINQ의 `Join`은 SQL의 `INNER JOIN`에 해당합니다. `GroupJoin`은 `LEFT JOIN`과 유사하며, 일대다 관계에서 유용합니다.

**서브쿼리와 Exists: 복잡한 조건**

"댓글이 5개 이상인 게시글", "특정 태그를 가진 게시글", "최근 30일 동안 댓글이 없는 게시글"—이런 조건들은 서브쿼리나 EXISTS를 사용합니다.

```csharp
// 댓글이 10개 이상이고 최근 7일 이내에 댓글이 달린 인기 게시글
var popularPosts = await context.Posts
    .Where(p =>
        p.Comments.Count >= 10 &&
        p.Comments.Any(c => c.CreatedAt >= DateTime.UtcNow.AddDays(-7))
    )
    .Select(p => new
    {
        p.Id,
        p.Title,
        CommentCount = p.Comments.Count,
        LatestCommentDate = p.Comments.Max(c => c.CreatedAt)
    })
    .ToListAsync();

// 특정 사용자가 댓글을 단 게시글
var postsCommentedByUser = await context.Posts
    .Where(p => p.Comments.Any(c => c.AuthorName == "John"))
    .ToListAsync();

// 댓글이 전혀 없는 게시글
var postsWithoutComments = await context.Posts
    .Where(p => !p.Comments.Any())
    .ToListAsync();
```

`Any`는 SQL의 `EXISTS`로, `Count() > 0`보다 효율적입니다. EXISTS는 첫 번째 일치를 찾으면 중단하지만, COUNT는 모든 행을 세기 때문입니다.

**원시 SQL: 마지막 수단**

LINQ로 표현하기 어렵거나, 데이터베이스 특정 기능이 필요하거나, 성능 최적화를 위해 정확한 SQL을 제어해야 할 때는 원시 SQL을 사용합니다.

```csharp
// 원시 SQL 쿼리
var posts = await context.Posts
    .FromSqlRaw("SELECT * FROM Posts WHERE ViewCount > {0}", 1000)
    .ToListAsync();

// Stored Procedure 호출
var results = await context.Posts
    .FromSqlRaw("EXEC GetTopPosts @Count = {0}", 10)
    .ToListAsync();

// 복잡한 집계 쿼리 (Window Functions 등)
var rankedPosts = await context.Database
    .SqlQuery<PostRankDto>($@"
        SELECT
            Id,
            Title,
            ViewCount,
            RANK() OVER (ORDER BY ViewCount DESC) as Rank,
            PERCENT_RANK() OVER (ORDER BY ViewCount DESC) as PercentRank
        FROM Posts
        WHERE IsPublished = 1
    ")
    .ToListAsync();
```

`FromSqlRaw`는 파라미터화를 자동으로 처리하여 SQL 인젝션을 방지합니다. LINQ와 결합할 수도 있습니다—원시 SQL로 기본 쿼리를 작성하고, `Where`, `OrderBy`, `Take` 등을 추가로 적용할 수 있습니다.

### 성능 최적화: 느린 쿼리를 빠르게

ORM의 가장 큰 비판은 "느리다"는 것입니다. 하지만 문제는 ORM 자체가 아니라 사용 방법입니다. EF Core는 제대로 사용하면 수동 SQL만큼, 때로는 더 빠를 수 있습니다. 핵심은 무슨 일이 일어나는지 이해하고, 병목 지점을 식별하고, 적절한 최적화를 적용하는 것입니다.

**N+1 쿼리 문제: 가장 흔한 성능 킬러**

N+1 문제는 ORM 초보자가 가장 자주 만드는 실수입니다. 컬렉션을 순회하며 각 항목의 관련 데이터를 로드할 때 발생합니다.

```csharp
// 나쁜 예: N+1 쿼리 문제
var posts = await context.Posts.Take(100).ToListAsync();
// 1번 쿼리: SELECT * FROM Posts LIMIT 100

foreach (var post in posts)
{
    Console.WriteLine($"{post.Title}: {post.Comments.Count} comments");
    // 각 게시글마다 1번씩 추가 쿼리! (100번)
    // SELECT * FROM Comments WHERE PostId = 1
    // SELECT * FROM Comments WHERE PostId = 2
    // ...
}
// 총 101번의 쿼리!
```

100개의 게시글에 대해 101번의 쿼리가 실행됩니다. 각 쿼리는 데이터베이스 왕복을 의미하며, 네트워크 지연이 누적됩니다. 해결책은 **Eager Loading**입니다:

```csharp
// 좋은 예: Eager Loading
var posts = await context.Posts
    .Include(p => p.Comments)
    .Take(100)
    .ToListAsync();
// 단 1번(또는 2번)의 쿼리로 모든 데이터 로드

foreach (var post in posts)
{
    Console.WriteLine($"{post.Title}: {post.Comments.Count} comments");
    // 추가 쿼리 없음!
}
```

`Include`는 관련 데이터를 미리 로드합니다. EF Core는 JOIN을 사용하거나 별도의 쿼리로 분할하여 최적화합니다. React Query나 Apollo Client의 prefetching과 비슷한 개념입니다—필요한 데이터를 미리 가져와 추가 네트워크 요청을 방지합니다.

**Split Query vs Single Query: Cartesian Explosion 방지**

여러 컬렉션을 `Include`하면 JOIN으로 인해 행이 기하급수적으로 증가할 수 있습니다(Cartesian Explosion).

```csharp
// Single Query: 모든 Include를 하나의 JOIN으로
var posts = await context.Posts
    .Include(p => p.Comments)
    .Include(p => p.Tags)
    .ToListAsync();
// SELECT * FROM Posts p
// LEFT JOIN Comments c ON p.Id = c.PostId
// LEFT JOIN PostTags pt ON p.Id = pt.PostId
// LEFT JOIN Tags t ON pt.TagId = t.Id

// 게시글 10개, 각각 댓글 5개, 태그 3개
// = 10 * 5 * 3 = 150개의 행 반환!
```

게시글 데이터가 중복되어 네트워크 대역폭을 낭비하고, EF Core가 메모리에서 다시 조립해야 합니다. **Split Query**는 이를 방지합니다:

```csharp
// Split Query: 각 Include를 별도 쿼리로
var posts = await context.Posts
    .Include(p => p.Comments)
    .Include(p => p.Tags)
    .AsSplitQuery()
    .ToListAsync();
// SELECT * FROM Posts
// SELECT * FROM Comments WHERE PostId IN (...)
// SELECT * FROM Tags WHERE ...
```

별도의 쿼리로 실행되어 중복이 없습니다. 하지만 여러 번의 데이터베이스 왕복이 필요하므로, 네트워크 지연이 큰 환경에서는 오히려 느릴 수 있습니다. 트레이드오프를 이해하고 상황에 맞게 선택해야 합니다.

EF Core 5부터는 전역 설정도 가능합니다:

```csharp
// DbContext에서 기본 동작 설정
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
}
```

**AsNoTracking: 읽기 전용 쿼리 최적화**

EF Core는 기본적으로 조회한 엔티티를 추적하여 변경을 감지합니다. 하지만 API 응답처럼 데이터를 읽기만 하는 경우, 이 추적은 불필요한 오버헤드입니다.

```csharp
// 기본: 변경 추적 활성화
var posts = await context.Posts.ToListAsync();
// 메모리에 스냅샷 저장, 변경 감지 준비

// 읽기 전용: 변경 추적 비활성화
var posts = await context.Posts
    .AsNoTracking()
    .ToListAsync();
// 스냅샷 저장 안 함, 30-40% 더 빠름
```

벤치마크 결과를 보면 차이가 명확합니다:

```
| Method         | Mean     | Allocated |
|--------------- |---------:|----------:|
| WithTracking   | 150.2 ms |   12.5 MB |
| WithNoTracking |  98.7 ms |    8.2 MB |
```

API 엔드포인트에서는 거의 항상 `AsNoTracking()`을 사용해야 합니다. 전역 설정도 가능합니다:

```csharp
// 모든 쿼리를 기본적으로 NoTracking으로
public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
}
```

단, 이후 엔티티를 수정하고 `SaveChanges`를 호출하려면 명시적으로 `AsTracking()`을 사용하거나 `Attach` 후 수정해야 합니다.

**프로젝션의 힘: SELECT only what you need**

엔티티 전체를 로드하는 것은 편리하지만 비효율적입니다. 특히 큰 텍스트 필드나 BLOB를 포함하는 경우입니다.

```csharp
// 비효율적: 모든 컬럼 로드
var posts = await context.Posts
    .Where(p => p.IsPublished)
    .ToListAsync();
// SELECT * FROM Posts WHERE IsPublished = 1
// Content 필드 (수십 KB)도 로드

// 효율적: 필요한 컬럼만 프로젝션
var posts = await context.Posts
    .Where(p => p.IsPublished)
    .Select(p => new
    {
        p.Id,
        p.Title,
        p.CreatedAt,
        CommentCount = p.Comments.Count
    })
    .ToListAsync();
// SELECT p.Id, p.Title, p.CreatedAt,
//        (SELECT COUNT(*) FROM Comments WHERE PostId = p.Id)
// FROM Posts p
// WHERE IsPublished = 1
```

네트워크 대역폭, 메모리 사용량, 역직렬화 시간이 모두 줄어듭니다. React의 useMemo나 Vue의 computed처럼, 필요한 데이터만 계산하는 것이 항상 빠릅니다.

**Compiled Queries: 반복 쿼리 최적화**

동일한 구조의 쿼리를 반복 실행하면, LINQ 표현식을 SQL로 변환하는 오버헤드가 누적됩니다. **Compiled Query**는 이를 미리 컴파일하여 캐시합니다.

```csharp
// 일반 쿼리 (매번 컴파일)
public async Task<Post?> GetPostById(int id)
{
    return await context.Posts
        .Include(p => p.Comments)
        .FirstOrDefaultAsync(p => p.Id == id);
}

// Compiled Query (한 번만 컴파일)
private static readonly Func<BlogContext, int, Task<Post?>> GetPostByIdCompiled =
    EF.CompileAsyncQuery((BlogContext context, int id) =>
        context.Posts
            .Include(p => p.Comments)
            .FirstOrDefault(p => p.Id == id));

public async Task<Post?> GetPostByIdOptimized(int id)
{
    return await GetPostByIdCompiled(context, id);
}
```

벤치마크:

```
| Method          | Mean     |
|---------------- |---------:|
| Normal          | 2.45 ms  |
| Compiled        | 1.82 ms  |
```

약 25% 성능 향상입니다. API 핫 패스(hot path)처럼 초당 수천 번 호출되는 쿼리에서 의미가 있습니다.

**인덱싱 전략: 데이터베이스 수준 최적화**

EF Core는 애플리케이션 레벨의 최적화를 제공하지만, 데이터베이스 수준의 최적화도 중요합니다. 가장 효과적인 것은 인덱스입니다.

```csharp
// Fluent API로 인덱스 생성
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // 단일 컬럼 인덱스
    modelBuilder.Entity<Post>()
        .HasIndex(p => p.CreatedAt);

    modelBuilder.Entity<Post>()
        .HasIndex(p => p.IsPublished);

    // 복합 인덱스 (WHERE절에 자주 함께 사용되는 컬럼)
    modelBuilder.Entity<Post>()
        .HasIndex(p => new { p.IsPublished, p.CreatedAt });

    // Unique 인덱스
    modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();

    // Filtered 인덱스 (조건부 인덱스)
    modelBuilder.Entity<Post>()
        .HasIndex(p => p.CreatedAt)
        .HasFilter("IsPublished = 1");
}
```

인덱스는 조회 성능을 극적으로 향상시킵니다. 하지만 공짜 점심은 없습니다. 인덱스는 저장 공간을 차지하고, INSERT/UPDATE/DELETE 시 추가 작업이 필요합니다. 트레이드오프를 이해하고 실제 쿼리 패턴을 분석하여 적용해야 합니다.

**쿼리 분석: 병목 지점 찾기**

최적화의 첫 단계는 측정입니다. EF Core는 생성된 SQL을 로깅하여 분석할 수 있습니다.

```csharp
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}

// 또는 코드에서 직접 로깅
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseSqlite("...")
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging() // 파라미터 값도 로깅 (개발 환경만!)
        .EnableDetailedErrors();
}
```

실행 시 콘솔에 SQL이 출력됩니다:

```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (3ms) [Parameters=[@__p_0='10'], CommandType='Text']
      SELECT p.Id, p.Title, p.Content, p.CreatedAt
      FROM Posts p
      WHERE p.IsPublished = 1
      ORDER BY p.CreatedAt DESC
      LIMIT @__p_0
```

실행 시간을 확인하고, 느린 쿼리를 식별하고, `EXPLAIN ANALYZE`(PostgreSQL) 또는 `SET STATISTICS TIME ON`(SQL Server)으로 실행 계획을 분석합니다.

### Repository 패턴과 Unit of Work: 추상화의 딜레마

ORM에 대한 거의 모든 튜토리얼이 Repository 패턴을 가르칩니다. 데이터 액세스 로직을 추상화하여 비즈니스 로직과 분리하는 디자인 패턴입니다. 하지만 EF Core 커뮤니티에서는 논란이 있습니다. "DbContext 자체가 이미 Repository와 Unit of Work 패턴이다"라는 주장입니다.

**전통적인 Repository 패턴**

먼저 Repository 패턴이 무엇인지 이해해봅시다:

```csharp
// 인터페이스 정의
public interface IPostRepository
{
    Task<Post?> GetByIdAsync(int id);
    Task<List<Post>> GetAllAsync();
    Task<List<Post>> GetPublishedAsync();
    Task<Post> AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(int id);
}

// 구현
public class PostRepository : IPostRepository
{
    private readonly BlogContext _context;

    public PostRepository(BlogContext context)
    {
        _context = context;
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _context.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Post>> GetPublishedAsync()
    {
        return await _context.Posts
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Post> AddAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    // ... 나머지 메서드
}

// 서비스에서 사용
public class PostService
{
    private readonly IPostRepository _repository;

    public PostService(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<PostDto> GetPostAsync(int id)
    {
        var post = await _repository.GetByIdAsync(id);
        // DTO 변환 등 비즈니스 로직
        return new PostDto { /* ... */ };
    }
}
```

**장점:**

1. **테스트 용이성**: 인터페이스를 모킹하여 단위 테스트를 작성할 수 있습니다.
   ```csharp
   var mockRepo = new Mock<IPostRepository>();
   mockRepo.Setup(r => r.GetByIdAsync(1))
           .ReturnsAsync(new Post { Id = 1, Title = "Test" });
   var service = new PostService(mockRepo.Object);
   ```

2. **비즈니스 로직 분리**: 컨트롤러나 서비스가 DbContext를 직접 알 필요가 없습니다.

3. **재사용성**: 공통 쿼리를 Repository 메서드로 캡슐화하여 여러 곳에서 사용할 수 있습니다.

4. **ORM 독립성**: 나중에 EF Core를 Dapper나 다른 ORM으로 교체할 수 있습니다(이론상).

**단점:**

1. **불필요한 추상화**: DbContext 자체가 이미 Repository 패턴입니다. `DbSet<T>`는 Repository이고, `SaveChanges()`는 Unit of Work입니다. 한 번 더 감싸는 것은 간접 레이어를 추가할 뿐입니다.

2. **LINQ의 유연성 상실**: Repository 인터페이스는 미리 정의된 메서드만 제공합니다. 동적 쿼리나 특수한 조건은 새 메서드를 계속 추가해야 합니다.
   ```csharp
   Task<List<Post>> GetByTagAsync(string tag);
   Task<List<Post>> GetByAuthorAsync(int authorId);
   Task<List<Post>> GetByDateRangeAsync(DateTime start, DateTime end);
   // 조합이 늘어날수록 메서드 폭발!
   ```

3. **Generic Repository의 함정**: 모든 엔티티에 동일한 메서드를 제공하는 `IRepository<T>`는 실제로는 거의 쓸모없습니다. 각 엔티티는 고유한 쿼리 패턴을 가지니까요.
   ```csharp
   public interface IRepository<T>
   {
       Task<T> GetByIdAsync(int id);
       Task<List<T>> GetAllAsync();
       // ... 모든 엔티티가 이 메서드를 필요로 하나요? 아닙니다.
   }
   ```

4. **ORM 교체의 환상**: "나중에 다른 ORM으로 바꿀 수 있다"는 주장은 현실적이지 않습니다. EF Core의 LINQ, Change Tracking, Navigation Property는 깊이 통합되어 있어, 단순히 Repository 구현만 바꾼다고 되지 않습니다.

**Microsoft의 공식 입장**

[EF Core 공식 문서](https://learn.microsoft.com/en-us/ef/core/miscellaneous/testing/choosing-a-testing-strategy#repository-pattern)는 다음과 같이 말합니다:

> "DbContext already implements the repository and unit of work patterns, so layering additional abstractions on top of it does not provide much value."

즉, DbContext를 직접 사용하는 것을 권장합니다. 하지만 팀의 코딩 스타일, 테스트 전략, 기존 아키텍처에 따라 Repository가 적합할 수도 있습니다. **절대적인 정답은 없습니다.**

**현대적 대안: CQRS와 MediatR**

Repository 패턴의 문제를 해결하는 현대적 접근은 **CQRS (Command Query Responsibility Segregation)**와 **MediatR** 패턴입니다.

```csharp
// 쿼리 정의 (Request)
public class GetPostByIdQuery : IRequest<PostDto>
{
    public int Id { get; set; }
}

// 핸들러 정의 (Handler)
public class GetPostByIdHandler : IRequestHandler<GetPostByIdQuery, PostDto>
{
    private readonly BlogContext _context;

    public GetPostByIdHandler(BlogContext context)
    {
        _context = context;
    }

    public async Task<PostDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (post == null)
            throw new NotFoundException($"Post {request.Id} not found");

        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            // ... 매핑
        };
    }
}

// 컨트롤러에서 사용
[HttpGet("{id}")]
public async Task<ActionResult<PostDto>> GetPost(int id)
{
    var query = new GetPostByIdQuery { Id = id };
    var result = await _mediator.Send(query);
    return result;
}
```

이 접근은 여러 장점이 있습니다:

1. **단일 책임**: 각 핸들러는 하나의 쿼리/명령만 처리합니다.
2. **LINQ 유연성 유지**: 핸들러 내부에서 원하는 대로 LINQ를 작성할 수 있습니다.
3. **테스트 용이성**: 핸들러를 독립적으로 테스트할 수 있습니다.
4. **파이프라인**: MediatR의 Behavior로 로깅, 유효성 검사, 트랜잭션을 자동화할 수 있습니다.

Clean Architecture나 Vertical Slice Architecture를 추구한다면 이 방향을 고려하세요.

### 다중 데이터베이스 지원: 진정한 크로스 플랫폼

EF Core의 강력한 점 중 하나는 다양한 데이터베이스를 지원한다는 것입니다. 개발은 SQLite로, 프로덕션은 PostgreSQL로, 온프레미스 고객은 SQL Server로—코드 변경 없이 가능합니다.

**주요 데이터베이스 제공자**

```bash
# SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# MySQL
dotnet add package Pomelo.EntityFrameworkCore.MySql

# SQLite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# Cosmos DB
dotnet add package Microsoft.EntityFrameworkCore.Cosmos

# In-Memory (테스트용)
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

**설정 전환**

```csharp
// appsettings.json에서 데이터베이스 선택
{
  "Database": "PostgreSQL",
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=blog;Username=user;Password=pass",
    "SqlServer": "Server=localhost;Database=blog;Trusted_Connection=True;",
    "MySql": "Server=localhost;Database=blog;User=root;Password=pass;"
  }
}

// Program.cs에서 동적 설정
var databaseType = builder.Configuration["Database"];
var connectionString = builder.Configuration.GetConnectionString(databaseType);

builder.Services.AddDbContext<BlogContext>(options =>
{
    switch (databaseType)
    {
        case "PostgreSQL":
            options.UseNpgsql(connectionString);
            break;
        case "SqlServer":
            options.UseSqlServer(connectionString);
            break;
        case "MySql":
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            break;
        case "Sqlite":
            options.UseSqlite(connectionString);
            break;
        default:
            throw new InvalidOperationException($"Unsupported database: {databaseType}");
    }
});
```

**데이터베이스별 차이점**

대부분의 쿼리는 변경 없이 작동하지만, 일부 기능은 데이터베이스별로 다릅니다:

1. **자동 증가(Identity)**:
   - SQL Server: `IDENTITY(1,1)`
   - PostgreSQL: `SERIAL` 또는 `GENERATED ALWAYS AS IDENTITY`
   - MySQL: `AUTO_INCREMENT`
   - SQLite: `AUTOINCREMENT`

   EF Core가 자동 변환합니다.

2. **날짜/시간**:
   ```csharp
   // DateTime.UtcNow는 모든 데이터베이스에서 작동
   var recentPosts = await context.Posts
       .Where(p => p.CreatedAt >= DateTime.UtcNow.AddDays(-7))
       .ToListAsync();

   // 하지만 데이터베이스 함수는 다름
   // SQL Server: GETUTCDATE()
   // PostgreSQL: NOW() AT TIME ZONE 'UTC'
   // MySQL: UTC_TIMESTAMP()
   ```

3. **문자열 함수**:
   ```csharp
   // EF.Functions로 데이터베이스 특정 함수 사용
   var posts = await context.Posts
       .Where(p => EF.Functions.Like(p.Title, "%ASP.NET%"))
       .ToListAsync();

   // PostgreSQL의 전문 검색
   var posts = await context.Posts
       .Where(p => EF.Functions.ToTsVector("english", p.Content)
                                .Matches(EF.Functions.PlainToTsQuery("entity framework")))
       .ToListAsync();
   ```

4. **JSON 지원**:
   ```csharp
   // PostgreSQL, MySQL 8.0+, SQL Server 2016+는 JSON 지원
   public class Post
   {
       public int Id { get; set; }
       public string Title { get; set; } = string.Empty;
       public Dictionary<string, string> Metadata { get; set; } = new();
   }

   // PostgreSQL에서 JSON 쿼리
   var posts = await context.Posts
       .Where(p => EF.Functions.JsonContains(p.Metadata, "{\"featured\": true}"))
       .ToListAsync();
   ```

5. **대소문자 구분**:
   - PostgreSQL: 기본적으로 대소문자 구분 (case-sensitive)
   - MySQL: 기본적으로 대소문자 무시 (case-insensitive, 설정에 따라 다름)
   - SQL Server: Collation에 따라 다름
   - SQLite: 기본적으로 대소문자 무시

   ```csharp
   // 대소문자 무시 검색 (모든 DB에서 작동)
   var posts = await context.Posts
       .Where(p => p.Title.ToLower() == searchTerm.ToLower())
       .ToListAsync();
   ```

**마이그레이션과 다중 데이터베이스**

각 데이터베이스마다 별도의 마이그레이션을 유지하는 것이 가장 안전합니다:

```bash
# PostgreSQL용 마이그레이션
dotnet ef migrations add InitialCreate --context BlogContext -- --database PostgreSQL

# SQL Server용 마이그레이션
dotnet ef migrations add InitialCreate --context BlogContext -- --database SqlServer
```

또는 조건부 마이그레이션:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    if (Database.IsNpgsql())
    {
        // PostgreSQL 특정 설정
        modelBuilder.Entity<Post>()
            .HasIndex(p => p.Content)
            .HasMethod("gin") // GIN 인덱스
            .IsTsVectorExpressionIndex("english");
    }
    else if (Database.IsSqlServer())
    {
        // SQL Server 특정 설정
        modelBuilder.Entity<Post>()
            .HasIndex(p => p.Content)
            .IncludeProperties(p => p.Title); // Covering Index
    }
}
```

### Cosmos DB: NoSQL과의 만남

EF Core는 전통적인 RDBMS뿐만 아니라 Azure Cosmos DB (NoSQL)도 지원합니다. 동일한 LINQ 쿼리로 문서 데이터베이스를 조작할 수 있습니다.

```csharp
// Cosmos DB 제공자 설치
// dotnet add package Microsoft.EntityFrameworkCore.Cosmos

// DbContext 설정
public class CosmosContext : DbContext
{
    public DbSet<BlogPost> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseCosmos(
            accountEndpoint: "https://myaccount.documents.azure.com:443/",
            accountKey: "...",
            databaseName: "BlogDb");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 컨테이너 이름 설정
        modelBuilder.Entity<BlogPost>()
            .ToContainer("Posts")
            .HasPartitionKey(p => p.Category);

        // 내장 컬렉션
        modelBuilder.Entity<BlogPost>()
            .OwnsMany(p => p.Comments);
    }
}

// 쿼리 사용
var posts = await context.Posts
    .Where(p => p.Category == "Technology")
    .OrderByDescending(p => p.CreatedAt)
    .Take(10)
    .ToListAsync();
```

Cosmos DB는 SQL API를 제공하므로 많은 LINQ 쿼리가 작동하지만, JOIN이나 복잡한 집계는 제한적입니다. 문서 데이터베이스의 특성상 비정규화(denormalization)가 권장됩니다.

### 벌크 작업 최적화: 대량 데이터 처리

일반적인 CRUD 작업은 EF Core가 잘 처리하지만, 수천 또는 수만 개의 레코드를 한 번에 삽입, 업데이트, 삭제해야 할 때는 다른 접근이 필요합니다.

**비효율적인 방법: 루프**

```csharp
// 나쁜 예: 각 항목마다 INSERT
foreach (var post in importedPosts) // 10,000개
{
    context.Posts.Add(post);
    await context.SaveChangesAsync(); // 10,000번의 쿼리!
}
```

각 `SaveChanges`는 데이터베이스 왕복을 의미하며, 트랜잭션 오버헤드도 누적됩니다. 10,000개 삽입에 몇 분이 걸릴 수 있습니다.

**개선: 배치 처리**

```csharp
// 더 나은 방법: 배치로 묶기
const int batchSize = 1000;
for (int i = 0; i < importedPosts.Count; i += batchSize)
{
    var batch = importedPosts.Skip(i).Take(batchSize);
    context.Posts.AddRange(batch);
    await context.SaveChangesAsync();
    context.ChangeTracker.Clear(); // 메모리 해제
}
```

`AddRange`와 하나의 `SaveChanges`로 1000개를 한 번에 삽입합니다. 10배 빠릅니다.

**최적: EFCore.BulkExtensions**

더 나은 성능을 원하면 서드파티 라이브러리를 사용합니다:

```bash
dotnet add package EFCore.BulkExtensions
```

```csharp
// 벌크 삽입: 네이티브 BULK INSERT 사용
await context.BulkInsertAsync(importedPosts);

// 벌크 업데이트
await context.BulkUpdateAsync(postsToUpdate);

// 벌크 삭제
await context.BulkDeleteAsync(postsToDelete);

// 조건부 벌크 업데이트
await context.Posts
    .Where(p => p.ViewCount < 10)
    .BatchUpdateAsync(p => new Post { IsPopular = false });
```

네이티브 데이터베이스 기능(SQL Server의 `BULK INSERT`, PostgreSQL의 `COPY`)을 사용하여 수십 배 빠릅니다. 100,000개 삽입이 몇 초 안에 완료됩니다.

**원시 SQL로 직접 벌크 작업**

EF Core 7부터는 `ExecuteUpdate`와 `ExecuteDelete`가 내장되어 있습니다:

```csharp
// 조건에 맞는 모든 행 업데이트 (단일 SQL)
await context.Posts
    .Where(p => p.ViewCount < 10)
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(p => p.IsPopular, false)
        .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));
// UPDATE Posts SET IsPopular = 0, UpdatedAt = @p0 WHERE ViewCount < 10

// 조건에 맞는 모든 행 삭제 (단일 SQL)
await context.Posts
    .Where(p => p.CreatedAt < DateTime.UtcNow.AddYears(-5))
    .ExecuteDeleteAsync();
// DELETE FROM Posts WHERE CreatedAt < @p0
```

Change Tracker를 거치지 않으므로 매우 빠르지만, 엔티티 이벤트나 변경 추적이 작동하지 않습니다. 대량 작업에는 완벽합니다.

### 실전 팁: 프로덕션에서 배운 교훈

마지막으로, 수년간의 EF Core 프로덕션 경험에서 배운 실용적인 조언들을 공유합니다.

**1. Connection Resilience: 네트워크는 항상 실패한다**

프로덕션 환경에서 데이터베이스 연결은 일시적으로 실패할 수 있습니다. EF Core의 재시도 정책을 활성화하세요:

```csharp
builder.Services.AddDbContext<BlogContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));
```

**2. Connection Pooling: 연결 재사용**

ADO.NET은 기본적으로 connection pooling을 제공하지만, 제대로 설정해야 합니다:

```
// Connection String
Server=localhost;Database=blog;Max Pool Size=100;Min Pool Size=10;
```

`using`이나 DI로 DbContext를 제대로 폐기해야 연결이 풀로 반환됩니다.

**3. DbContext Lifetime: Scoped가 기본**

ASP.NET Core에서 DbContext는 Scoped lifetime을 사용해야 합니다. Singleton은 절대 안 됩니다(thread-safe하지 않음).

```csharp
// 올바른 설정
builder.Services.AddDbContext<BlogContext>(ServiceLifetime.Scoped); // 기본값

// 잘못된 설정
builder.Services.AddSingleton<BlogContext>(); // 절대 금지!
```

**4. 프로덕션에서 마이그레이션: 자동 vs 수동**

개발 환경에서는 `dotnet ef database update`가 편리하지만, 프로덕션에서는 신중해야 합니다. 두 가지 접근이 있습니다:

**자동 마이그레이션** (작은 팀, 작은 앱):
```csharp
// Program.cs
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
    await context.Database.MigrateAsync(); // 시작 시 자동 적용
}
```

**수동 마이그레이션** (대규모, 엔터프라이즈):
```bash
# 마이그레이션 SQL 스크립트 생성
dotnet ef migrations script --idempotent --output migration.sql

# DBA가 검토 후 수동 실행
```

**5. 환경별 시드 데이터 관리**

개발용 테스트 데이터와 프로덕션용 초기 데이터를 분리하세요:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    if (_environment.IsDevelopment())
    {
        // 개발용 시드 데이터
        modelBuilder.Entity<Post>().HasData(
            new Post { Id = 1, Title = "Test Post 1", /* ... */ },
            new Post { Id = 2, Title = "Test Post 2", /* ... */ }
        );
    }
    else
    {
        // 프로덕션용 필수 데이터만
        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "Announcement" }
        );
    }
}
```

**6. Soft Delete: 데이터 보존**

실제로 삭제하지 않고 플래그로 표시하는 Soft Delete 패턴:

```csharp
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

// Global Query Filter로 자동 필터링
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasQueryFilter(p => !p.IsDeleted);
}

// 이제 모든 쿼리에서 삭제된 항목은 자동으로 제외됨
var posts = await context.Posts.ToListAsync(); // WHERE IsDeleted = 0 자동 추가

// 삭제된 항목 포함하려면
var allPosts = await context.Posts.IgnoreQueryFilters().ToListAsync();
```

**7. Audit Fields: 변경 이력 추적**

누가, 언제 생성/수정했는지 자동으로 기록:

```csharp
public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class Post : AuditableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

// SaveChanges 오버라이드로 자동 설정
public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var entries = ChangeTracker.Entries<AuditableEntity>();
    var currentUser = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
    var now = DateTime.UtcNow;

    foreach (var entry in entries)
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.CreatedAt = now;
            entry.Entity.CreatedBy = currentUser;
        }
        else if (entry.State == EntityState.Modified)
        {
            entry.Entity.UpdatedAt = now;
            entry.Entity.UpdatedBy = currentUser;
        }
    }

    return base.SaveChangesAsync(cancellationToken);
}
```

### 마치며: 데이터 계층 마스터하기

Chapter 13에서 우리는 EF Core의 고급 기능을 깊이 있게 탐구했습니다. 복잡한 쿼리 작성, 성능 최적화, 아키텍처 패턴, 다중 데이터베이스 지원, 벌크 작업—프로덕션 환경에서 만나는 현실적인 문제들을 해결하는 방법을 배웠습니다.

EF Core는 단순한 ORM이 아닙니다. 강력한 타입 시스템, LINQ의 표현력, 그리고 .NET 생태계와의 깊은 통합을 통해, 데이터 액세스 계층을 우아하고 효율적으로 만드는 도구입니다. JavaScript의 Prisma가 새로운 가능성을 제시했다면, EF Core는 성숙하고 검증된 강력함을 제공합니다.

하지만 기술은 도구일 뿐입니다. 중요한 것은 문제를 이해하고, 트레이드오프를 평가하고, 상황에 맞는 최선의 솔루션을 선택하는 능력입니다. N+1 문제를 식별하고, AsNoTracking을 언제 사용할지 알고, Repository 패턴의 장단점을 이해하고, 성능 병목을 측정하는 것—이런 것들이 진짜 실력입니다.

여러분은 이제 Part 5를 완료했습니다. C#과 ASP.NET Core로 백엔드를 만들고, EF Core로 데이터 계층을 구축하는 방법을 배웠습니다. 프론트엔드 개발자에서 시작하여, 이제 진정한 풀스택 .NET 개발자가 되었습니다.

다음 Part에서는 API 개발로 넘어갑니다. RESTful API 설계, 인증과 권한 부여, GraphQL, SignalR—프론트엔드와 백엔드를 연결하는 모든 것을 배웁니다. 여러분의 React 또는 Vue 앱이 여러분의 ASP.NET Core API와 통신하는 완전한 풀스택 애플리케이션을 만들 준비가 되었나요?

계속 나아갑시다!
