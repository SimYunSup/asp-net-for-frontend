# Chapter 12: Entity Framework Core 기초

## 첫 번째 쿼리에서 프로덕션까지: EF Core 마스터하기

새로운 기술을 배울 때 가장 중요한 것은 견고한 기초입니다. Part 5 서론에서 우리는 ORM의 철학과 EF Core가 JavaScript 생태계의 ORM들과 어떻게 다른지 탐구했습니다. 이제 이론에서 실천으로 넘어갈 시간입니다. 이 챕터에서는 EF Core의 핵심 개념을 하나씩 배우고, 실제로 작동하는 애플리케이션을 만들어봅니다.

프론트엔드 개발자라면 새로운 React 라이브러리나 Vue 플러그인을 배울 때 공식 문서의 "Getting Started"를 따라가본 경험이 있을 것입니다. 간단한 카운터 앱, TODO 리스트를 만들며 개념을 익히는 과정이죠. 이 챕터도 마찬가지입니다. 하지만 여기서는 단순히 "이렇게 하면 됩니다"가 아니라, **왜** 그렇게 하는지, 내부적으로 **무슨 일이** 일어나는지 이해하는 데 중점을 둡니다.

### DbContext: 데이터베이스 세션의 중심

EF Core의 모든 것은 `DbContext`에서 시작됩니다. 이 클래스는 애플리케이션과 데이터베이스 사이의 다리 역할을 하며, 여러 역할을 동시에 수행합니다. JavaScript 생태계와 비교하면 다음과 같이 이해할 수 있습니다:

- **Prisma Client**: Prisma의 `PrismaClient`와 유사하게, 데이터베이스와 통신하는 주요 인터페이스입니다.
- **TypeORM DataSource**: TypeORM의 `DataSource`처럼 연결과 설정을 관리합니다.
- **Sequelize Instance**: Sequelize의 인스턴스와 비슷하게 트랜잭션과 쿼리를 처리합니다.

하지만 `DbContext`는 이들을 넘어 더 깊은 통합을 제공합니다. **Unit of Work** 패턴과 **Repository** 패턴을 동시에 구현하며, 변경 추적(Change Tracking) 시스템을 내장합니다. 이 모든 것이 하나의 일관된 API로 제공됩니다.

```csharp
// 가장 간단한 DbContext
public class BlogContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=blog.db");
    }
}
```

이 코드는 놀라울 정도로 간단하지만, 많은 일을 합니다. `DbSet<Post>`는 데이터베이스의 `Posts` 테이블을 나타내며, 이를 통해 쿼리하고 수정할 수 있습니다. `OnConfiguring`은 어떤 데이터베이스를 사용할지 정의합니다. SQLite를 사용하는 이유는 간단합니다—별도의 서버 설치 없이 파일 하나로 시작할 수 있으니까요.

**의존성 주입과 DbContext**

실제 애플리케이션에서는 `OnConfiguring`을 사용하지 않습니다. 연결 문자열을 코드에 하드코딩하는 것은 좋지 않은 관행이며, 테스트도 어렵게 만듭니다. ASP.NET Core의 의존성 주입(DI) 시스템과 통합하는 것이 표준입니다.

```csharp
// Program.cs에서 DbContext 등록
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// appsettings.json에서 연결 문자열
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=blog.db"
  }
}

// 컨트롤러나 서비스에서 주입받아 사용
public class PostsController : ControllerBase
{
    private readonly BlogContext _context;

    public PostsController(BlogContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Post>>> GetPosts()
    {
        return await _context.Posts.ToListAsync();
    }
}
```

이 패턴은 React의 Context API나 Angular의 의존성 주입과 개념적으로 유사합니다. 컴포넌트가 직접 전역 상태를 import하지 않고 주입받는 것처럼, 컨트롤러도 `DbContext`를 직접 생성하지 않고 주입받습니다. 이는 테스트 가능성, 생명주기 관리, 그리고 관심사의 분리를 보장합니다.

ASP.NET Core는 요청마다 새로운 `DbContext` 인스턴스를 생성합니다(Scoped lifetime). 요청이 시작되면 Context가 생성되고, 요청이 끝나면 자동으로 폐기됩니다. 이는 동시성 문제를 방지하고, 각 요청이 독립적인 데이터베이스 세션을 갖도록 보장합니다. JavaScript의 미들웨어 체인에서 각 요청이 독립적인 컨텍스트를 갖는 것과 비슷합니다.

### Entity 클래스: 단순한 C# 클래스가 테이블이 되는 마법

Entity는 데이터베이스 테이블을 나타내는 C# 클래스입니다. 하지만 특별한 베이스 클래스를 상속하거나 인터페이스를 구현할 필요가 없습니다. 그냥 평범한 클래스(POCO: Plain Old CLR Object)입니다. 이는 TypeORM의 데코레이터 기반 접근보다 깔끔하고, Sequelize의 `define()` 메서드보다 직관적입니다.

```csharp
// 가장 간단한 Entity
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

이것만으로 충분합니다. EF Core는 관례(Convention)를 통해 많은 것을 유추합니다:

- `Id` 또는 `{ClassName}Id` 프로퍼티는 자동으로 Primary Key가 됩니다.
- Primary Key가 `int`나 `long`이면 자동으로 Identity(자동 증가)가 설정됩니다.
- `string` 프로퍼티는 `nvarchar(max)` 또는 `text` 컬럼이 됩니다.
- 프로퍼티 이름이 컬럼 이름이 됩니다.
- 클래스 이름의 복수형이 테이블 이름이 됩니다 (`Post` → `Posts`).

하지만 관례를 따르지 않고 명시적으로 설정할 수도 있습니다. Data Annotations(특성)나 Fluent API를 사용합니다.

**Data Annotations: 간단하고 직관적인 설정**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("BlogPosts")] // 테이블 이름 명시
public class Post
{
    [Key] // Primary Key 명시 (Id가 아닌 경우)
    public int PostId { get; set; }

    [Required] // NOT NULL
    [MaxLength(200)] // nvarchar(200)
    public string Title { get; set; } = string.Empty;

    [Column(TypeName = "text")] // 컬럼 타입 명시
    public string Content { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
}
```

Data Annotations는 TypeORM의 데코레이터와 매우 유사합니다. 클래스 정의를 보는 것만으로 스키마를 이해할 수 있습니다. 하지만 복잡한 설정에는 한계가 있습니다. 예를 들어, 복합 키(Composite Key), 인덱스, 다대다 관계의 세세한 설정은 어렵습니다.

**Fluent API: 강력하고 유연한 설정**

더 복잡한 매핑은 Fluent API를 사용합니다. `DbContext`의 `OnModelCreating` 메서드를 오버라이드하여 설정합니다.

```csharp
public class BlogContext : DbContext
{
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            // 테이블 이름 설정
            entity.ToTable("BlogPosts");

            // Primary Key 설정
            entity.HasKey(e => e.PostId);

            // 프로퍼티 설정
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Content)
                .HasColumnType("text");

            // 인덱스 생성
            entity.HasIndex(e => e.CreatedAt);

            // 기본값 설정
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
```

Fluent API는 더 장황하지만, 모든 것을 제어할 수 있습니다. 특히 복잡한 비즈니스 로직이나 레거시 데이터베이스와 통합할 때 필수적입니다. Prisma의 스키마 파일과 비슷한 역할을 하지만, C# 코드로 작성되어 타입 안전하고 리팩토링에 강합니다.

두 접근 방식은 함께 사용할 수 있습니다. 간단한 설정은 Data Annotations로, 복잡한 설정은 Fluent API로 하는 것이 일반적입니다. Fluent API의 설정이 항상 우선순위를 가지므로, 필요하면 특성을 오버라이드할 수 있습니다.

### 관계 설정: 테이블을 연결하는 법

관계형 데이터베이스의 핵심은 이름 그대로 "관계"입니다. 블로그에는 게시글이 있고, 게시글에는 댓글이 있고, 댓글은 사용자가 작성합니다. 이런 관계를 코드로 표현하는 것이 ORM의 중요한 역할입니다.

**일대다 관계 (One-to-Many)**

가장 흔한 관계입니다. 한 게시글에는 여러 댓글이 있지만, 각 댓글은 하나의 게시글에만 속합니다.

```csharp
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    // Navigation Property: 이 게시글의 모든 댓글
    public List<Comment> Comments { get; set; } = new();
}

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;

    // Foreign Key: 어떤 게시글에 속하는가
    public int PostId { get; set; }

    // Navigation Property: 이 댓글이 속한 게시글
    public Post Post { get; set; } = null!;
}
```

여기서 몇 가지 주목할 점이 있습니다:

1. **Navigation Property**: `Post.Comments`와 `Comment.Post`는 실제 데이터베이스 컬럼이 아닙니다. EF Core가 관계를 탐색할 때 사용하는 프로퍼티입니다. React의 props처럼, 데이터를 탐색하는 경로입니다.

2. **Foreign Key**: `Comment.PostId`는 실제 데이터베이스의 Foreign Key 컬럼입니다. EF Core는 관례로 `{NavigationProperty}Id` 패턴을 인식합니다.

3. **초기화**: `Comments = new()`는 null 참조 오류를 방지합니다. C# 11의 target-typed new 표현식입니다. `Post = null!`의 `null!`은 "나는 이것이 null이 아님을 알고 있다"는 컴파일러 힌트입니다. EF Core가 로드할 때 채워줄 것을 알기 때문입니다.

이 정의만으로 EF Core는 적절한 Foreign Key 제약 조건을 생성합니다. 명시적으로 설정하고 싶다면 Fluent API를 사용합니다:

```csharp
modelBuilder.Entity<Comment>()
    .HasOne(c => c.Post) // Comment는 하나의 Post를 가짐
    .WithMany(p => p.Comments) // Post는 여러 Comment를 가짐
    .HasForeignKey(c => c.PostId) // Foreign Key 명시
    .OnDelete(DeleteBehavior.Cascade); // 게시글 삭제 시 댓글도 삭제
```

**다대다 관계 (Many-to-Many)**

다대다는 더 복잡합니다. 게시글에는 여러 태그가 있고, 각 태그도 여러 게시글에 사용됩니다. 전통적으로는 중간 테이블(Join Table)이 필요합니다.

```csharp
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    // Navigation Property: 이 게시글의 모든 태그
    public List<Tag> Tags { get; set; } = new();
}

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation Property: 이 태그를 사용하는 모든 게시글
    public List<Post> Posts { get; set; } = new();
}
```

EF Core 5.0부터는 중간 엔티티 없이 다대다를 정의할 수 있습니다. EF Core가 자동으로 `PostTag` 조인 테이블을 생성합니다. 하지만 중간 테이블에 추가 데이터(생성 날짜, 정렬 순서 등)를 저장하려면 명시적으로 정의해야 합니다:

```csharp
public class PostTag
{
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;

    public DateTime AssignedAt { get; set; } // 추가 데이터
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public List<PostTag> PostTags { get; set; } = new();
}

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<PostTag> PostTags { get; set; } = new();
}

// Fluent API 설정
modelBuilder.Entity<PostTag>()
    .HasKey(pt => new { pt.PostId, pt.TagId }); // 복합 키

modelBuilder.Entity<PostTag>()
    .HasOne(pt => pt.Post)
    .WithMany(p => p.PostTags)
    .HasForeignKey(pt => pt.PostId);

modelBuilder.Entity<PostTag>()
    .HasOne(pt => pt.Tag)
    .WithMany(t => t.PostTags)
    .HasForeignKey(pt => pt.TagId);
```

이는 TypeORM의 `@ManyToMany` 데코레이터나 Prisma의 implicit many-to-many 관계와 유사하지만, 더 명시적입니다. 명시성은 복잡성을 가져오지만, 정확히 무슨 일이 일어나는지 이해하고 제어할 수 있게 합니다.

### Code-First 마이그레이션: 데이터베이스 스키마의 Git

코드로 모델을 정의했으니 이제 데이터베이스를 만들 차례입니다. EF Core의 마이그레이션 시스템은 코드의 변경사항을 추적하고, 데이터베이스 스키마를 동기화합니다. Git이 코드 변경을 추적하듯, 마이그레이션은 스키마 변경을 추적합니다.

**첫 번째 마이그레이션 생성**

터미널에서 프로젝트 디렉토리로 이동하여 다음 명령을 실행합니다:

```bash
dotnet ef migrations add InitialCreate
```

이 명령은 현재 모델의 스냅샷과 비교하여 변경사항을 감지하고, 마이그레이션 파일을 생성합니다. 첫 마이그레이션이므로 모든 테이블을 생성하는 코드가 만들어집니다.

```csharp
// Migrations/20231215_InitialCreate.cs
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Posts",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Content = table.Column<string>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Posts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Comments",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Content = table.Column<string>(type: "TEXT", nullable: false),
                PostId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Comments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Comments_Posts_PostId",
                    column: x => x.PostId,
                    principalTable: "Posts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Comments");
        migrationBuilder.DropTable(name: "Posts");
    }
}
```

`Up` 메서드는 변경을 적용하고, `Down` 메서드는 롤백합니다. SQL DDL(Data Definition Language)을 C# API로 표현한 것입니다. 데이터베이스 독립적이므로, SQLite용으로 작성했지만 PostgreSQL이나 SQL Server에서도 작동합니다.

**마이그레이션 적용**

```bash
dotnet ef database update
```

이 명령은 아직 적용되지 않은 모든 마이그레이션을 순서대로 실행합니다. 데이터베이스가 없으면 생성하고, 테이블을 만들고, 제약 조건을 설정합니다. `__EFMigrationsHistory` 테이블에 어떤 마이그레이션이 적용되었는지 기록되어, 중복 실행을 방지합니다.

**모델 변경과 추가 마이그레이션**

개발하다 보면 모델을 자주 변경합니다. 새 필드를 추가하거나, 관계를 수정하거나, 인덱스를 추가합니다. 각 변경마다 새 마이그레이션을 생성합니다.

```csharp
// Post 클래스에 새 프로퍼티 추가
public class Post
{
    // ... 기존 프로퍼티들
    public int ViewCount { get; set; } // 새로 추가
    public bool IsPublished { get; set; } // 새로 추가
}

// 마이그레이션 생성
// $ dotnet ef migrations add AddViewCountAndIsPublished

// 자동 생성된 마이그레이션
public partial class AddViewCountAndIsPublished : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ViewCount",
            table: "Posts",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<bool>(
            name: "IsPublished",
            table: "Posts",
            type: "INTEGER",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "ViewCount", table: "Posts");
        migrationBuilder.DropColumn(name: "IsPublished", table: "Posts");
    }
}
```

EF Core는 이전 모델 스냅샷과 현재 모델을 비교하여 무엇이 변경되었는지 정확히 감지합니다. 기존 데이터가 있어도 안전하게 컬럼을 추가하며, 기본값을 설정하여 null이 될 수 없는 필드도 추가할 수 있습니다.

**마이그레이션 롤백**

실수했거나 변경을 되돌리고 싶다면 이전 마이그레이션으로 롤백할 수 있습니다:

```bash
# 마지막 마이그레이션 취소
dotnet ef database update PreviousMigrationName

# 모든 마이그레이션 취소 (데이터베이스 삭제 제외)
dotnet ef database update 0

# 마이그레이션 파일 자체를 삭제
dotnet ef migrations remove
```

`Down` 메서드가 실행되어 변경사항을 되돌립니다. 하지만 주의해야 합니다. 컬럼을 삭제하면 그 컬럼의 데이터도 사라지니까요. 프로덕션에서는 롤백보다는 새로운 마이그레이션으로 수정하는 것이 안전합니다.

**시드 데이터: 초기 데이터 삽입**

개발이나 테스트를 위해 초기 데이터가 필요할 때가 많습니다. 관리자 계정, 기본 카테고리, 샘플 데이터 등입니다. `OnModelCreating`에서 시드 데이터를 정의할 수 있습니다:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Tag>().HasData(
        new Tag { Id = 1, Name = "ASP.NET Core" },
        new Tag { Id = 2, Name = "Entity Framework" },
        new Tag { Id = 3, Name = "Blazor" }
    );

    modelBuilder.Entity<Post>().HasData(
        new Post
        {
            Id = 1,
            Title = "Welcome to EF Core",
            Content = "This is the first post...",
            CreatedAt = new DateTime(2024, 1, 1),
            ViewCount = 0,
            IsPublished = true
        }
    );
}
```

`HasData`는 마이그레이션에 `INSERT` 문을 추가합니다. 다음 마이그레이션을 생성할 때 자동으로 포함됩니다. 데이터베이스가 이미 해당 데이터를 가지고 있으면(Primary Key 기준) 건너뛰므로, 여러 번 실행해도 안전합니다.

### LINQ to Entities: 타입 안전한 쿼리의 힘

마이그레이션으로 데이터베이스를 준비했으니 이제 데이터를 조회할 차례입니다. EF Core에서 쿼리를 작성하는 주요 방법은 LINQ(Language Integrated Query)입니다. Part 1에서 LINQ의 기초를 배웠지만, 여기서는 데이터베이스 컨텍스트에서 어떻게 작동하는지 깊이 있게 다룹니다.

**기본 쿼리: Where, OrderBy, Select**

가장 기본적인 쿼리부터 시작해봅시다. JavaScript의 배열 메서드와 비교하며 이해하기 쉽습니다.

```csharp
// JavaScript 배열 메서드
const posts = allPosts
    .filter(p => p.isPublished)
    .sort((a, b) => b.createdAt - a.createdAt)
    .slice(0, 10);

// EF Core LINQ
var posts = await context.Posts
    .Where(p => p.IsPublished)
    .OrderByDescending(p => p.CreatedAt)
    .Take(10)
    .ToListAsync();
```

문법이 놀라울 정도로 유사합니다. 하지만 근본적인 차이가 있습니다. JavaScript 코드는 모든 데이터를 메모리로 가져온 후 필터링하지만, EF Core는 SQL 쿼리로 변환하여 데이터베이스에서 필터링합니다.

```sql
-- 생성된 SQL
SELECT p.Id, p.Title, p.Content, p.CreatedAt, p.ViewCount, p.IsPublished
FROM Posts p
WHERE p.IsPublished = 1
ORDER BY p.CreatedAt DESC
LIMIT 10
```

`ToListAsync()`가 호출되는 시점에 SQL이 실행됩니다. 그 전까지는 쿼리 객체만 구성됩니다. 이를 **지연 실행(Deferred Execution)**이라고 하며, 동적 쿼리 구성을 가능하게 합니다.

**프로젝션: 필요한 데이터만 선택**

종종 엔티티의 모든 프로퍼티가 아니라 일부만 필요합니다. API 응답을 위해 특정 필드만 반환하거나, 집계 값을 계산할 때입니다. `Select`를 사용하여 프로젝션합니다:

```csharp
// 익명 타입으로 프로젝션
var postSummaries = await context.Posts
    .Where(p => p.IsPublished)
    .Select(p => new
    {
        p.Id,
        p.Title,
        p.CreatedAt,
        CommentCount = p.Comments.Count
    })
    .ToListAsync();

// 생성된 SQL
// SELECT p.Id, p.Title, p.CreatedAt,
//        (SELECT COUNT(*) FROM Comments c WHERE c.PostId = p.Id) AS CommentCount
// FROM Posts p
// WHERE p.IsPublished = 1
```

프로젝션은 여러 장점이 있습니다:

1. **성능**: 필요한 컬럼만 조회하므로 데이터 전송량이 줄어듭니다.
2. **메모리**: 작은 객체만 메모리에 로드되므로 효율적입니다.
3. **보안**: 민감한 필드(비밀번호 해시 등)를 제외할 수 있습니다.
4. **명확성**: API 응답 구조가 명시적으로 정의됩니다.

DTO(Data Transfer Object) 클래스를 정의하여 명명된 타입으로 프로젝션할 수도 있습니다:

```csharp
public class PostSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int CommentCount { get; set; }
}

var postSummaries = await context.Posts
    .Where(p => p.IsPublished)
    .Select(p => new PostSummaryDto
    {
        Id = p.Id,
        Title = p.Title,
        CreatedAt = p.CreatedAt,
        CommentCount = p.Comments.Count
    })
    .ToListAsync();
```

이는 타입 안전성을 유지하면서 재사용 가능한 구조를 만듭니다. API 컨트롤러에서 DTO를 반환하면 Swagger/OpenAPI 문서에도 정확한 스키마가 나타납니다.

**관계 탐색: Include와 ThenInclude**

Entity의 Navigation Property를 통해 관련 데이터를 로드할 수 있습니다. 하지만 기본적으로 EF Core는 관련 데이터를 자동으로 로드하지 않습니다. 명시적으로 `Include`해야 합니다.

```csharp
// Comments를 포함하지 않음
var post = await context.Posts.FirstAsync(p => p.Id == 1);
Console.WriteLine(post.Comments.Count); // 0 (로드되지 않음)

// Comments를 명시적으로 포함
var postWithComments = await context.Posts
    .Include(p => p.Comments)
    .FirstAsync(p => p.Id == 1);
Console.WriteLine(postWithComments.Comments.Count); // 실제 댓글 수
```

`Include`는 JOIN 쿼리를 생성하거나 별도의 쿼리로 관련 데이터를 로드합니다(Split Query). 중첩된 관계는 `ThenInclude`로 계속 탐색할 수 있습니다:

```csharp
// 게시글 → 댓글 → 댓글 작성자
var posts = await context.Posts
    .Include(p => p.Comments)
        .ThenInclude(c => c.Author)
    .ToListAsync();

// 각 게시글의 댓글과 그 댓글의 작성자까지 로드됨
foreach (var post in posts)
{
    foreach (var comment in post.Comments)
    {
        Console.WriteLine($"{comment.Content} - by {comment.Author.Name}");
    }
}
```

하지만 조심해야 합니다. `Include`를 과도하게 사용하면 **Cartesian Explosion** 문제가 발생할 수 있습니다. 게시글 10개, 각각 댓글 5개, 각 댓글에 태그 3개가 있다면, JOIN으로 인해 150개의 행이 반환됩니다. EF Core는 이를 메모리에서 다시 조립하지만, 데이터 전송량과 처리 시간이 기하급수적으로 증가합니다.

이럴 때는 **Split Query**를 사용합니다:

```csharp
var posts = await context.Posts
    .Include(p => p.Comments)
        .ThenInclude(c => c.Author)
    .Include(p => p.Tags)
    .AsSplitQuery() // 별도의 쿼리로 분할
    .ToListAsync();

// 생성된 쿼리:
// 1. SELECT * FROM Posts WHERE ...
// 2. SELECT * FROM Comments WHERE PostId IN (...)
// 3. SELECT * FROM Authors WHERE Id IN (...)
// 4. SELECT * FROM Tags WHERE PostId IN (...)
```

각 `Include`가 별도의 쿼리로 실행되어 Cartesian Explosion을 방지합니다. 다만 여러 번의 데이터베이스 왕복이 발생하므로, 네트워크 지연이 있는 환경에서는 오히려 느릴 수 있습니다. 트레이드오프를 이해하고 상황에 맞게 선택해야 합니다.

### 변경 추적과 저장: SaveChanges의 마법

EF Core의 가장 강력한 기능 중 하나는 **변경 추적(Change Tracking)**입니다. 데이터베이스에서 조회한 엔티티의 변경사항을 자동으로 감지하고, `SaveChanges`를 호출하면 적절한 SQL을 생성합니다.

**추가 (Insert)**

```csharp
// 새 게시글 생성
var post = new Post
{
    Title = "New Post",
    Content = "Content here...",
    CreatedAt = DateTime.UtcNow,
    IsPublished = true
};

// Context에 추가
context.Posts.Add(post);

// 데이터베이스에 저장
await context.SaveChangesAsync();

// 이제 post.Id는 데이터베이스에서 생성된 값을 가짐
Console.WriteLine($"Created post with ID: {post.Id}");
```

`Add` 메서드는 엔티티를 **Added** 상태로 표시합니다. `SaveChangesAsync`가 호출되면 EF Core는 `INSERT` 쿼리를 생성하고 실행합니다. Identity 컬럼의 값은 자동으로 엔티티에 다시 할당되어, 즉시 사용할 수 있습니다.

**수정 (Update)**

```csharp
// 게시글 조회
var post = await context.Posts.FindAsync(1);

// 프로퍼티 수정
post.Title = "Updated Title";
post.ViewCount += 1;

// 저장
await context.SaveChangesAsync();
// UPDATE Posts SET Title = @p0, ViewCount = @p1 WHERE Id = 1
```

`Find`나 LINQ 쿼리로 조회한 엔티티는 자동으로 추적됩니다. 프로퍼티를 변경하면 EF Core가 감지하고, `SaveChanges` 시 변경된 필드만 포함하는 `UPDATE` 쿼리를 생성합니다. `Update` 메서드를 명시적으로 호출할 필요가 없습니다.

만약 추적되지 않는 엔티티를 수정하려면(예: API 요청으로 받은 데이터) `Update` 메서드를 사용합니다:

```csharp
// API에서 받은 데이터 (추적되지 않음)
var updatedPost = new Post
{
    Id = 1,
    Title = "New Title",
    Content = "New Content",
    // ... 모든 필드
};

context.Posts.Update(updatedPost);
await context.SaveChangesAsync();
// UPDATE Posts SET Title = @p0, Content = @p1, ... WHERE Id = 1
```

이 경우 모든 필드가 업데이트됩니다. 부분 업데이트를 원하면 먼저 조회하거나, `Attach`와 프로퍼티별 수정을 사용합니다:

```csharp
var post = new Post { Id = 1 };
context.Attach(post);
post.Title = "New Title"; // 이 필드만 변경됨
context.Entry(post).Property(p => p.Title).IsModified = true;
await context.SaveChangesAsync();
// UPDATE Posts SET Title = @p0 WHERE Id = 1
```

**삭제 (Delete)**

```csharp
// 게시글 조회 후 삭제
var post = await context.Posts.FindAsync(1);
context.Posts.Remove(post);
await context.SaveChangesAsync();
// DELETE FROM Posts WHERE Id = 1

// 조회 없이 삭제 (효율적)
var postToDelete = new Post { Id = 1 };
context.Posts.Remove(postToDelete);
await context.SaveChangesAsync();
```

`Remove`는 엔티티를 **Deleted** 상태로 표시하고, `SaveChanges` 시 `DELETE` 쿼리를 실행합니다. Foreign Key 제약 조건에 따라 관련 데이터도 함께 삭제되거나(Cascade Delete) 오류가 발생할 수 있습니다.

**트랜잭션: 모두 성공하거나 모두 실패**

`SaveChanges`는 자동으로 트랜잭션 안에서 실행됩니다. 여러 변경사항이 있어도 모두 성공하거나 모두 롤백됩니다.

```csharp
var post = new Post { Title = "New Post", /* ... */ };
context.Posts.Add(post);

var comment = new Comment { PostId = post.Id, Content = "First!" };
context.Comments.Add(comment);

await context.SaveChangesAsync();
// 두 INSERT가 하나의 트랜잭션으로 실행됨
// 하나라도 실패하면 둘 다 롤백
```

명시적 트랜잭션이 필요하면 `BeginTransaction`을 사용합니다:

```csharp
using var transaction = await context.Database.BeginTransactionAsync();
try
{
    var post = new Post { /* ... */ };
    context.Posts.Add(post);
    await context.SaveChangesAsync();

    // 외부 API 호출 등 다른 작업
    await SendNotificationAsync(post);

    var comment = new Comment { PostId = post.Id, /* ... */ };
    context.Comments.Add(comment);
    await context.SaveChangesAsync();

    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

명시적 트랜잭션은 여러 `SaveChanges` 호출이나 데이터베이스 외부 작업을 하나의 원자적 단위로 묶을 때 사용합니다.

### 실습: 블로그 데이터베이스 설계와 구현

이론을 충분히 배웠으니 이제 직접 만들어봅시다. 간단한 블로그 시스템의 데이터베이스를 설계하고, EF Core로 구현합니다.

**요구사항:**

- **게시글(Post)**: 제목, 내용, 작성일, 조회수, 공개 여부
- **댓글(Comment)**: 내용, 작성일, 작성자 이름
- **태그(Tag)**: 이름
- **관계**: 게시글 ↔ 댓글 (일대다), 게시글 ↔ 태그 (다대다)

**1단계: 엔티티 클래스 정의**

```csharp
// Entities/Post.cs
public class Post
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int ViewCount { get; set; }
    public bool IsPublished { get; set; }

    // Navigation Properties
    public List<Comment> Comments { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();
}

// Entities/Comment.cs
public class Comment
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string AuthorName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Key & Navigation
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;
}

// Entities/Tag.cs
public class Tag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    // Navigation Property
    public List<Post> Posts { get; set; } = new();
}
```

**2단계: DbContext 정의**

```csharp
// Data/BlogContext.cs
public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 인덱스 설정
        modelBuilder.Entity<Post>()
            .HasIndex(p => p.CreatedAt);

        modelBuilder.Entity<Post>()
            .HasIndex(p => p.IsPublished);

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        // 시드 데이터
        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "ASP.NET Core" },
            new Tag { Id = 2, Name = "EF Core" },
            new Tag { Id = 3, Name = "C#" }
        );
    }
}
```

**3단계: Program.cs에서 등록**

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=blog.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

**4단계: 마이그레이션 생성 및 적용**

```bash
# 마이그레이션 생성
dotnet ef migrations add InitialCreate

# 데이터베이스 업데이트
dotnet ef database update
```

**5단계: API 컨트롤러 구현**

```csharp
// Controllers/PostsController.cs
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly BlogContext _context;

    public PostsController(BlogContext context)
    {
        _context = context;
    }

    // GET: api/posts
    [HttpGet]
    public async Task<ActionResult<List<PostDto>>> GetPosts()
    {
        var posts = await _context.Posts
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                CreatedAt = p.CreatedAt,
                ViewCount = p.ViewCount,
                CommentCount = p.Comments.Count
            })
            .ToListAsync();

        return posts;
    }

    // GET: api/posts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PostDetailDto>> GetPost(int id)
    {
        var post = await _context.Posts
            .Include(p => p.Comments)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return NotFound();

        // 조회수 증가
        post.ViewCount++;
        await _context.SaveChangesAsync();

        var dto = new PostDetailDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            ViewCount = post.ViewCount,
            Comments = post.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                AuthorName = c.AuthorName,
                CreatedAt = c.CreatedAt
            }).ToList(),
            Tags = post.Tags.Select(t => t.Name).ToList()
        };

        return dto;
    }

    // POST: api/posts
    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost(CreatePostDto dto)
    {
        var post = new Post
        {
            Title = dto.Title,
            Content = dto.Content,
            IsPublished = dto.IsPublished,
            CreatedAt = DateTime.UtcNow
        };

        // 태그 연결
        if (dto.TagIds?.Any() == true)
        {
            var tags = await _context.Tags
                .Where(t => dto.TagIds.Contains(t.Id))
                .ToListAsync();
            post.Tags = tags;
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    // PUT: api/posts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, UpdatePostDto dto)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return NotFound();

        post.Title = dto.Title;
        post.Content = dto.Content;
        post.IsPublished = dto.IsPublished;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/posts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return NotFound();

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
```

**DTOs (Data Transfer Objects)**

```csharp
// DTOs/PostDto.cs
public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ViewCount { get; set; }
    public int CommentCount { get; set; }
}

public class PostDetailDto : PostDto
{
    public string Content { get; set; } = string.Empty;
    public List<CommentDto> Comments { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreatePostDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public bool IsPublished { get; set; }
    public List<int>? TagIds { get; set; }
}

public class UpdatePostDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public bool IsPublished { get; set; }
}
```

**테스트**

```bash
# 애플리케이션 실행
dotnet run

# Swagger UI로 이동
# https://localhost:5001/swagger

# 또는 curl로 테스트
curl -X POST https://localhost:5001/api/posts \
  -H "Content-Type: application/json" \
  -d '{
    "title": "My First Post",
    "content": "Hello EF Core!",
    "isPublished": true,
    "tagIds": [1, 2]
  }'

curl https://localhost:5001/api/posts

curl https://localhost:5001/api/posts/1
```

이 실습을 통해 다음을 경험했습니다:

1. **엔티티 정의**: Data Annotations로 제약 조건 설정
2. **관계 설정**: 일대다와 다대다 관계
3. **DbContext**: 엔티티를 하나로 묶고 설정 적용
4. **마이그레이션**: Code-First로 데이터베이스 생성
5. **CRUD 작업**: LINQ 쿼리와 SaveChanges
6. **DTO 패턴**: 엔티티와 API 응답 분리

### 다음 단계: Chapter 13으로

Chapter 12에서 EF Core의 기초를 탄탄히 다졌습니다. DbContext와 Entity, 마이그레이션, LINQ 쿼리, 변경 추적—이 모든 것이 이제 익숙해졌을 것입니다. 하지만 이것은 시작일 뿐입니다.

실제 프로덕션 애플리케이션에서는 더 복잡한 시나리오에 직면합니다. 수백만 개의 행을 가진 테이블, 복잡한 비즈니스 로직, 성능 요구사항, 여러 데이터베이스 지원—Chapter 13에서는 이런 고급 주제들을 다룹니다.

- **복잡한 쿼리**: GroupBy, Join, 서브쿼리, window functions
- **성능 최적화**: N+1 문제 해결, AsNoTracking, Compiled Queries
- **Repository 패턴**: 언제 사용하고 언제 피해야 하는가
- **다중 데이터베이스**: PostgreSQL, MySQL, SQL Server 전환
- **벌크 작업**: 대량 INSERT/UPDATE/DELETE 최적화

여러분은 이제 EF Core의 기초를 마스터했습니다. 다음 챕터에서는 진짜 실력을 발휘할 차례입니다. 준비되셨나요?
