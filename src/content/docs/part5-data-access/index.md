---
title: "Part 5 - 데이터 액세스 - Entity Framework Core"
---

# Part 5: 데이터 액세스 - Entity Framework Core

## 타입 안전한 데이터베이스 작업: ORM의 새로운 표준

Part 4에서 우리는 Blazor를 통해 C#으로 풍부한 인터랙티브 UI를 만드는 방법을 배웠습니다. 이제 모든 웹 애플리케이션의 핵심인 데이터 계층으로 내려갈 차례입니다. 아무리 아름다운 UI를 만들어도, 결국 대부분의 앱은 데이터를 저장하고, 조회하고, 수정하고, 삭제하는 작업으로 귀결됩니다. 사용자 정보, 게시글, 주문 내역, 제품 카탈로그—모든 것이 데이터베이스에 저장됩니다.

전통적으로 데이터베이스 작업은 SQL 쿼리를 문자열로 작성하는 것을 의미했습니다. Node.js 개발자라면 이런 코드에 익숙할 것입니다:

```javascript
// 원시 SQL 쿼리
const users = await db.query('SELECT * FROM Users WHERE Age > $1', [18]);

// 오타가 있어도 런타임에만 발견됨
const posts = await db.query('SELECT * FROM Pots WHERE UserId = $1', [userId]);
// 'Pots'는 'Posts'의 오타지만, 실행하기 전까지 모름
```

문제는 명확합니다. SQL은 문자열이기 때문에 컴파일러가 검증할 수 없습니다. 테이블 이름을 잘못 입력해도, 컬럼이 존재하지 않아도, 타입이 맞지 않아도 코드는 성공적으로 컴파일됩니다. 오류는 런타임에, 그것도 사용자가 그 기능을 사용할 때만 발견됩니다. 프로덕션 환경에서 말이죠.

이것이 바로 ORM(Object-Relational Mapping)이 등장한 이유입니다. ORM은 데이터베이스 테이블을 프로그래밍 언어의 객체로 매핑하여, SQL을 직접 작성하지 않고도 데이터를 다룰 수 있게 합니다. JavaScript/TypeScript 개발자라면 Prisma, TypeORM, Sequelize를 사용해봤을 것입니다. .NET 생태계에서는 Entity Framework Core(줄여서 EF Core)가 그 역할을 합니다.

### ORM의 진화: JavaScript에서 .NET까지

ORM은 새로운 개념이 아닙니다. Java의 Hibernate(2001년), Python의 Django ORM(2005년), Ruby의 ActiveRecord(2004년)—거의 모든 현대 프레임워크가 ORM을 제공합니다. 하지만 ORM들의 철학과 접근 방식은 크게 다릅니다. JavaScript 생태계의 주요 ORM들과 EF Core를 비교하며 각각의 특징을 이해해봅시다.

**1. Sequelize: 전통적인 Active Record 패턴**

Sequelize는 Node.js에서 가장 오래된 ORM 중 하나입니다. Active Record 패턴을 따르며, 모델이 데이터베이스 로직과 밀접하게 결합되어 있습니다.

```javascript
// Sequelize 모델 정의
const User = sequelize.define('User', {
  firstName: {
    type: DataTypes.STRING,
    allowNull: false
  },
  email: {
    type: DataTypes.STRING,
    unique: true
  }
});

// 쿼리 실행
const users = await User.findAll({
  where: { firstName: 'John' },
  include: [{ model: Post }]
});
```

Sequelize의 강점은 명시적입니다. 모든 것을 코드로 정의하며, 데이터베이스 스키마를 프로그래밍적으로 관리합니다. 하지만 타입 안정성이 약합니다. TypeScript를 사용하더라도 많은 부분이 `any` 타입이거나 복잡한 제네릭으로 래핑됩니다. 쿼리 결과의 타입을 보장하기 어렵고, 관계를 탐색할 때 타입 추론이 잘 작동하지 않습니다.

**2. TypeORM: 데코레이터 기반의 현대적 접근**

TypeORM은 TypeScript의 데코레이터 기능을 활용하여 더 우아한 모델 정의를 제공합니다. Java의 JPA나 C#의 EF Core에서 영감을 받았습니다.

```typescript
// TypeORM 엔티티 정의
@Entity()
class User {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  firstName: string;

  @Column({ unique: true })
  email: string;

  @OneToMany(() => Post, post => post.user)
  posts: Post[];
}

// 쿼리 실행
const users = await userRepository.find({
  where: { firstName: 'John' },
  relations: ['posts']
});
```

TypeORM은 타입 안정성이 훨씬 좋습니다. 엔티티는 실제 TypeScript 클래스이고, 프로퍼티는 명확한 타입을 가집니다. 하지만 여전히 한계가 있습니다. `find()` 메서드의 옵션 객체는 타입 체크가 약하고, 복잡한 쿼리에서는 `QueryBuilder`를 사용해야 하는데 이는 문자열 기반입니다. 또한 데코레이터는 실험적 기능이므로 tsconfig에서 활성화해야 합니다.

**3. Prisma: 스키마 우선의 혁신**

Prisma는 2019년에 등장하여 ORM의 패러다임을 바꿨습니다. 별도의 스키마 언어(Prisma Schema Language)로 모델을 정의하고, 거기서 TypeScript 클라이언트를 자동 생성합니다.

```prisma
// schema.prisma
model User {
  id        Int      @id @default(autoincrement())
  firstName String
  email     String   @unique
  posts     Post[]
}

model Post {
  id      Int    @id @default(autoincrement())
  title   String
  userId  Int
  user    User   @relation(fields: [userId], references: [id])
}
```

```typescript
// 자동 생성된 타입 안전 클라이언트
const users = await prisma.user.findMany({
  where: { firstName: 'John' },
  include: { posts: true }
});
// users의 타입: User & { posts: Post[] }
```

Prisma의 가장 큰 혁신은 완벽한 타입 안정성입니다. 모든 쿼리, 모든 필드, 모든 관계가 정확하게 타입 체크됩니다. IDE의 자동완성이 완벽하게 작동하고, 존재하지 않는 필드를 참조하면 컴파일 에러가 발생합니다. 마이그레이션도 스키마에서 자동 생성되며, Prisma Studio라는 GUI 도구도 제공합니다.

하지만 Prisma에도 트레이드오프가 있습니다. 빌드 시간이 길어집니다. 스키마가 변경될 때마다 클라이언트를 재생성해야 하니까요. 또한 스키마 언어를 새로 배워야 하며, 복잡한 쿼리나 원시 SQL이 필요한 경우에는 한계가 있습니다.

### Entity Framework Core: 타입 안정성과 강력함의 결합

EF Core는 Prisma의 타입 안정성과 TypeORM의 유연성을 결합하면서, C#의 강력한 타입 시스템과 LINQ를 활용합니다. 2016년에 처음 출시되어 .NET Core의 핵심 컴포넌트가 되었으며, 현재 버전 8(2023년)은 성능과 기능 면에서 최고의 ORM 중 하나로 평가받습니다.

EF Core의 핵심 철학은 "Code First, but flexible"입니다. 코드로 모델을 정의하되, 필요하면 데이터베이스에서 역생성(Database First)할 수도 있습니다. 마이그레이션으로 스키마를 관리하되, 원시 SQL도 자유롭게 사용할 수 있습니다. 추상화 위에서 작업하되, 필요하면 낮은 레벨로 내려갈 수 있습니다.

```csharp
// EF Core 엔티티 정의
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Post> Posts { get; set; } = new();
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

// DbContext: 데이터베이스 세션
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
}

// LINQ를 통한 타입 안전 쿼리
var users = await context.Users
    .Where(u => u.FirstName == "John")
    .Include(u => u.Posts)
    .ToListAsync();
```

이 코드를 보면 몇 가지가 눈에 띕니다. 첫째, 엔티티는 평범한 C# 클래스입니다(POCO: Plain Old CLR Object). 특별한 베이스 클래스를 상속하거나 인터페이스를 구현할 필요가 없습니다. 둘째, 타입이 명확합니다. `Id`는 `int`, `FirstName`은 `string`, `Posts`는 `List<Post>`—모두 컴파일 타임에 체크됩니다.

가장 강력한 부분은 쿼리입니다. LINQ(Language Integrated Query)를 사용하기 때문에 쿼리 자체가 C# 코드입니다. 문자열이 아닙니다. `Where`, `Select`, `OrderBy`, `Include`—모든 메서드가 강타입이고, 잘못된 필드명을 사용하면 컴파일 에러가 발생합니다. IDE가 자동완성을 제공하고, 리팩토링 도구가 완벽하게 작동합니다.

### 왜 ORM을 배워야 하는가? 원시 SQL의 한계

"SQL을 알고 있는데 왜 ORM을 배워야 하나요?"라는 질문은 자주 듣습니다. 특히 SQL에 능숙한 개발자일수록 ORM이 불필요한 추상화처럼 느껴질 수 있습니다. 하지만 현대 애플리케이션 개발에서 ORM이 필수가 된 이유는 명확합니다.

**1. 타입 안정성: 컴파일 타임 오류 vs 런타임 오류**

원시 SQL의 가장 큰 문제는 타입 안정성의 부재입니다. 다음 Node.js 코드를 보세요:

```javascript
// 원시 SQL: 런타임에만 오류 발견
async function getUser(id) {
  const result = await db.query(
    'SELECT id, first_name, emial FROM users WHERE id = $1',
    [id]
  );
  return result.rows[0];
}

// 사용하는 곳
const user = await getUser(123);
console.log(user.email); // undefined! 'emial'은 오타
```

`email`을 `emial`로 잘못 입력했습니다. 하지만 코드는 정상적으로 실행되고, 쿼리도 성공합니다. 단지 `user.email`이 `undefined`일 뿐입니다. 이 버그는 사용자가 이메일을 사용하려 할 때만 발견되며, 원인을 추적하기 어렵습니다. 로그를 보면 "email이 undefined"라고 나올 뿐, SQL 쿼리의 오타를 지적하지 않습니다.

EF Core에서는 이것이 불가능합니다:

```csharp
// EF Core: 컴파일 타임에 오류 발견
var user = await context.Users
    .Select(u => new { u.Id, u.FirstName, u.Emial }) // 컴파일 에러!
    .FirstOrDefaultAsync(u => u.Id == 123);
// Error: 'User' does not contain a definition for 'Emial'
```

IDE가 즉시 빨간 밑줄을 표시하고, 컴파일조차 되지 않습니다. 오타는 작성하는 순간 발견되며, 런타임까지 갈 필요가 없습니다.

**2. SQL 인젝션 방지: 자동 파라미터화**

SQL 인젝션은 OWASP Top 10에서 항상 상위권에 있는 보안 취약점입니다. 사용자 입력을 쿼리에 직접 삽입하면 공격자가 임의의 SQL을 실행할 수 있습니다.

```javascript
// 위험한 코드: SQL 인젝션 취약점
async function searchUsers(name) {
  const query = `SELECT * FROM users WHERE name = '${name}'`;
  return await db.query(query);
}

// 공격 시나리오
await searchUsers("John' OR '1'='1");
// SELECT * FROM users WHERE name = 'John' OR '1'='1'
// 모든 사용자 반환!

await searchUsers("John'; DROP TABLE users; --");
// 데이터베이스 삭제!
```

파라미터화를 사용하면 막을 수 있지만, 개발자가 항상 기억해야 합니다. ORM은 이를 자동화합니다. EF Core에서는 SQL 인젝션이 원천적으로 불가능합니다:

```csharp
// 안전한 코드: 자동 파라미터화
var users = await context.Users
    .Where(u => u.Name == name) // 항상 파라미터화됨
    .ToListAsync();
// SQL: SELECT * FROM Users WHERE Name = @p0
// 파라미터: @p0 = 'John'' OR ''1''=''1'
// 결과: 리터럴 문자열로 처리되어 일치하는 사용자 없음
```

사용자 입력이 항상 파라미터로 전달되므로, SQL 구조를 변경할 수 없습니다. 개발자가 의식적으로 노력하지 않아도 안전합니다.

**3. 데이터베이스 독립성: Write Once, Run Anywhere**

프로젝트 초기에는 SQLite나 PostgreSQL을 사용하다가, 나중에 SQL Server나 MySQL로 변경해야 할 수 있습니다. 원시 SQL로 작성하면 데이터베이스마다 문법이 달라 수백 개의 쿼리를 수정해야 할 수 있습니다.

```sql
-- PostgreSQL
SELECT * FROM users LIMIT 10 OFFSET 20;

-- SQL Server
SELECT * FROM users ORDER BY id OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY;

-- MySQL
SELECT * FROM users LIMIT 10 OFFSET 20;
```

페이징 문법만 해도 이렇게 다릅니다. 날짜 함수, 문자열 함수, JSON 지원은 더 복잡합니다. EF Core는 이를 추상화합니다:

```csharp
// 모든 데이터베이스에서 동일한 코드
var users = await context.Users
    .Skip(20)
    .Take(10)
    .ToListAsync();
```

`Skip`과 `Take`는 각 데이터베이스의 적절한 SQL로 변환됩니다. 코드는 변경하지 않고 `appsettings.json`의 연결 문자열만 바꾸면 다른 데이터베이스로 전환됩니다.

**4. 복잡한 관계 탐색: Lazy Loading과 Eager Loading**

관계형 데이터베이스의 핵심은 테이블 간의 관계입니다. 하지만 원시 SQL로 관계를 다루는 것은 번거롭습니다. N+1 쿼리 문제는 너무나 흔합니다.

```javascript
// N+1 쿼리 문제
const users = await db.query('SELECT * FROM users');
for (const user of users) {
  const posts = await db.query('SELECT * FROM posts WHERE user_id = $1', [user.id]);
  user.posts = posts;
}
// 사용자가 100명이면 101번의 쿼리!
```

JOIN을 사용하면 해결되지만, 중첩된 관계에서는 복잡해집니다. EF Core는 이를 우아하게 처리합니다:

```csharp
// Eager Loading: 한 번의 쿼리로 모든 데이터
var users = await context.Users
    .Include(u => u.Posts)
        .ThenInclude(p => p.Comments)
            .ThenInclude(c => c.Author)
    .ToListAsync();
// 적절한 JOIN 쿼리 자동 생성
```

`Include`는 관련 데이터를 같이 로드하라는 명시적 지시입니다. EF Core는 최적의 JOIN 쿼리를 생성하여 한 번(또는 최소한의 횟수)에 모든 데이터를 가져옵니다.

**5. 변경 추적: 자동 업데이트 감지**

데이터를 수정하고 저장하는 것은 모든 애플리케이션의 기본입니다. 원시 SQL로는 어떤 필드가 변경되었는지 수동으로 추적해야 합니다.

```javascript
// 수동 업데이트: 모든 필드 나열
async function updateUser(id, updates) {
  await db.query(
    'UPDATE users SET first_name = $1, last_name = $2, email = $3, updated_at = $4 WHERE id = $5',
    [updates.firstName, updates.lastName, updates.email, new Date(), id]
  );
}
```

필드가 추가될 때마다 업데이트 쿼리를 수정해야 합니다. EF Core는 변경을 자동으로 추적합니다:

```csharp
// 자동 변경 추적
var user = await context.Users.FindAsync(id);
user.FirstName = "Jane"; // 변경 기록
user.Email = "jane@example.com"; // 변경 기록
// user.LastName은 변경 안 함

await context.SaveChangesAsync();
// UPDATE Users SET FirstName = @p0, Email = @p1 WHERE Id = @p2
// 변경된 필드만 업데이트!
```

어떤 프로퍼티가 변경되었는지 자동으로 감지하고, 필요한 필드만 업데이트하는 효율적인 SQL을 생성합니다.

### 마이그레이션: 데이터베이스 스키마의 버전 관리

코드는 Git으로 버전 관리합니다. 하지만 데이터베이스 스키마는 어떻게 관리할까요? 개발 환경에서 테이블을 추가했는데, 프로덕션에도 같은 변경을 적용하려면? 새로운 팀원이 합류했을 때 로컬 데이터베이스를 어떻게 설정할까요?

전통적인 접근은 SQL 스크립트를 수동으로 작성하고 관리하는 것입니다. `001_create_users_table.sql`, `002_add_email_column.sql` 같은 파일들을 만들고, 어떤 스크립트가 실행되었는지 추적합니다. 하지만 이는 오류가 발생하기 쉽습니다. 순서를 잘못 실행하거나, 이미 실행된 스크립트를 다시 실행하거나, 환경마다 다른 상태가 되기 쉽습니다.

Node.js 생태계의 Knex.js나 TypeORM도 마이그레이션 시스템을 제공합니다. 하지만 대부분 JavaScript로 마이그레이션 파일을 작성해야 합니다:

```javascript
// Knex.js 마이그레이션
exports.up = function(knex) {
  return knex.schema.createTable('users', function(table) {
    table.increments('id');
    table.string('first_name');
    table.string('email').unique();
    table.timestamps();
  });
};

exports.down = function(knex) {
  return knex.schema.dropTable('users');
};
```

이 방식의 문제는 모델과 마이그레이션이 분리되어 있다는 것입니다. 엔티티 클래스에 필드를 추가하면, 마이그레이션 파일도 별도로 작성해야 합니다. 실수로 둘이 맞지 않으면 런타임 에러가 발생합니다.

EF Core의 마이그레이션은 다릅니다. **Code-First** 접근으로, 코드에서 모델을 정의하면 마이그레이션이 자동 생성됩니다.

```csharp
// 1. 엔티티 클래스에 새 프로퍼티 추가
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } // 새로 추가!
}

// 2. 마이그레이션 생성 (CLI)
// $ dotnet ef migrations add AddCreatedAtToUser

// 3. 자동 생성된 마이그레이션 파일
public partial class AddCreatedAtToUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Users",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Users");
    }
}

// 4. 데이터베이스에 적용
// $ dotnet ef database update
```

모델의 변경사항을 자동으로 감지하고, 적절한 SQL을 생성합니다. `Up` 메서드는 변경을 적용하고, `Down` 메서드는 롤백합니다. Git 커밋처럼 마이그레이션을 쌓아가며, 언제든 원하는 시점으로 되돌릴 수 있습니다.

더 강력한 점은 팀 협업입니다. 마이그레이션 파일을 Git에 커밋하면, 팀원들이 단순히 `dotnet ef database update`를 실행하는 것만으로 동일한 스키마를 갖게 됩니다. CI/CD 파이프라인에 통합하면 배포 시 자동으로 프로덕션 데이터베이스를 업데이트할 수 있습니다.

### LINQ to Entities: 데이터베이스를 위한 함수형 프로그래밍

Part 1에서 LINQ(Language Integrated Query)를 배웠습니다. JavaScript의 배열 메서드와 유사하지만, 더 강력하고 일관된 API를 제공한다고 말씀드렸습니다. EF Core에서 LINQ는 진정한 힘을 발휘합니다. 메모리의 컬렉션뿐만 아니라, 데이터베이스에도 동일한 쿼리를 사용할 수 있습니다.

```csharp
// 메모리 컬렉션 쿼리
var numbers = new List<int> { 1, 2, 3, 4, 5 };
var evenNumbers = numbers.Where(n => n % 2 == 0).ToList();

// 데이터베이스 쿼리 (거의 동일한 문법!)
var activeUsers = await context.Users
    .Where(u => u.IsActive)
    .ToListAsync();
```

차이는 `ToList()`가 `ToListAsync()`로 바뀐 것뿐입니다. 하지만 내부 동작은 완전히 다릅니다. 첫 번째는 메모리에서 필터링하고, 두 번째는 SQL 쿼리로 변환되어 데이터베이스에서 실행됩니다.

이것이 가능한 이유는 LINQ가 **표현식 트리(Expression Tree)**로 작동하기 때문입니다. `Where(u => u.IsActive)`의 람다 표현식은 즉시 실행되지 않고, 데이터 구조로 변환됩니다. EF Core는 이 표현식 트리를 분석하여 SQL로 번역합니다.

```csharp
// 복잡한 LINQ 쿼리
var result = await context.Users
    .Where(u => u.Age >= 18)
    .OrderBy(u => u.LastName)
    .ThenBy(u => u.FirstName)
    .Select(u => new {
        u.Id,
        FullName = u.FirstName + " " + u.LastName,
        PostCount = u.Posts.Count
    })
    .Take(10)
    .ToListAsync();

// 생성된 SQL (PostgreSQL)
// SELECT u.Id,
//        u.FirstName || ' ' || u.LastName AS FullName,
//        (SELECT COUNT(*) FROM Posts p WHERE p.UserId = u.Id) AS PostCount
// FROM Users u
// WHERE u.Age >= 18
// ORDER BY u.LastName, u.FirstName
// LIMIT 10
```

LINQ 체인이 하나의 최적화된 SQL 쿼리로 변환됩니다. 중간 단계에서 데이터를 가져오지 않습니다. `Where`, `OrderBy`, `Select`, `Take`가 모두 하나의 쿼리로 결합됩니다. 이를 **지연 실행(Deferred Execution)**이라고 합니다.

JavaScript의 Prisma도 비슷한 체이닝을 지원하지만, 타입 추론과 최적화 면에서 차이가 있습니다:

```typescript
// Prisma 쿼리
const result = await prisma.user.findMany({
  where: { age: { gte: 18 } },
  orderBy: [{ lastName: 'asc' }, { firstName: 'asc' }],
  select: {
    id: true,
    firstName: true,
    lastName: true,
    _count: { select: { posts: true } }
  },
  take: 10
});
```

Prisma의 쿼리는 객체 리터럴로 표현됩니다. 타입 안전하지만, LINQ의 함수형 체이닝만큼 직관적이지는 않습니다. 특히 동적 쿼리를 구성할 때 LINQ가 더 유연합니다:

```csharp
// 동적 쿼리 구성 (LINQ)
var query = context.Users.AsQueryable();

if (!string.IsNullOrEmpty(searchTerm))
{
    query = query.Where(u => u.FirstName.Contains(searchTerm)
                          || u.LastName.Contains(searchTerm));
}

if (minAge.HasValue)
{
    query = query.Where(u => u.Age >= minAge.Value);
}

if (sortByName)
{
    query = query.OrderBy(u => u.FirstName);
}

var users = await query.Take(20).ToListAsync();
```

조건에 따라 쿼리를 점진적으로 구성할 수 있습니다. 각 `Where`와 `OrderBy`는 쿼리 객체를 수정하며, 최종적으로 `ToListAsync()`가 호출될 때만 SQL이 실행됩니다.

### 성능 최적화: N+1 문제와 AsNoTracking

ORM의 가장 큰 비판은 "느리다"는 것입니다. 잘못 사용하면 맞는 말입니다. 하지만 올바르게 사용하면 ORM은 수동 SQL만큼, 때로는 더 빠를 수 있습니다. 핵심은 ORM이 어떻게 작동하는지 이해하는 것입니다.

**N+1 쿼리 문제: ORM의 고전적 함정**

가장 흔한 성능 문제는 N+1 쿼리입니다. 컬렉션을 순회하며 각 항목의 관련 데이터를 로드할 때 발생합니다.

```csharp
// 나쁜 예: N+1 쿼리 문제
var users = await context.Users.ToListAsync(); // 1번 쿼리
foreach (var user in users)
{
    Console.WriteLine($"{user.FirstName}: {user.Posts.Count} posts");
    // 각 사용자마다 1번씩 추가 쿼리! (N번)
}
// 총 N+1번의 쿼리
```

첫 번째 쿼리로 사용자 목록을 가져온 후, 각 사용자의 `Posts`에 접근할 때마다 추가 쿼리가 실행됩니다. 사용자가 100명이면 101번의 쿼리가 발생합니다. 데이터베이스 왕복이 반복되므로 극도로 느립니다.

해결책은 **Eager Loading**입니다:

```csharp
// 좋은 예: Eager Loading
var users = await context.Users
    .Include(u => u.Posts) // 관련 데이터를 미리 로드
    .ToListAsync();
// 단 1번(또는 2번)의 쿼리로 모든 데이터 로드

foreach (var user in users)
{
    Console.WriteLine($"{user.FirstName}: {user.Posts.Count} posts");
    // 추가 쿼리 없음!
}
```

`Include`는 관련 엔티티를 JOIN으로 함께 가져오라는 명시적 지시입니다. EF Core는 적절한 JOIN 쿼리를 생성하거나, 대량의 데이터에서는 별도의 쿼리로 분할하여 최적화합니다(Split Query).

**변경 추적 비용: 읽기 전용 쿼리 최적화**

EF Core는 기본적으로 조회한 모든 엔티티를 추적합니다. 프로퍼티 변경을 감지하여 `SaveChanges()` 시 적절한 UPDATE 쿼리를 생성하기 위해서입니다. 하지만 단순히 읽기만 하는 경우, 이 추적은 불필요한 오버헤드입니다.

```csharp
// 기본: 변경 추적 활성화 (쓰기 작업 위해)
var user = await context.Users.FirstAsync(u => u.Id == 123);
user.FirstName = "Jane"; // 변경 추적
await context.SaveChangesAsync(); // UPDATE 쿼리 생성

// 읽기 전용: 변경 추적 비활성화
var users = await context.Users
    .AsNoTracking() // 추적하지 않음
    .ToListAsync();
// 30-40% 더 빠르며, 메모리 사용량 감소
```

`AsNoTracking()`을 사용하면 EF Core는 엔티티를 추적하지 않습니다. 조회 성능이 크게 향상되며, 메모리도 절약됩니다. API 엔드포인트처럼 데이터를 읽어서 반환하기만 하는 경우 항상 사용해야 합니다.

**프로젝션: 필요한 데이터만 조회**

ORM의 또 다른 함정은 필요 이상의 데이터를 조회하는 것입니다. `SELECT *`는 편리하지만, 사용하지 않는 컬럼까지 전송하므로 비효율적입니다.

```csharp
// 비효율적: 모든 컬럼 조회
var users = await context.Users
    .Include(u => u.Posts)
    .ToListAsync();
// User와 Post의 모든 필드 로드

// 효율적: 필요한 데이터만 프로젝션
var users = await context.Users
    .Select(u => new {
        u.Id,
        u.FirstName,
        u.LastName,
        PostCount = u.Posts.Count // 집계만 필요
    })
    .ToListAsync();
// 훨씬 적은 데이터 전송
```

`Select`를 사용한 프로젝션은 필요한 컬럼만 조회하는 SQL을 생성합니다. 특히 큰 BLOB 필드나 텍스트 필드가 있을 때 성능 차이가 극적입니다.

### Repository 패턴: 추상화의 딜레마

ORM을 사용하다 보면 "Repository 패턴"을 들어봤을 것입니다. 데이터 액세스 로직을 추상화하는 디자인 패턴으로, 많은 튜토리얼과 책에서 권장합니다.

```csharp
// Repository 인터페이스
public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
}

// 구현
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    // ... 나머지 메서드
}
```

이 패턴의 장점은 명확합니다. 데이터 액세스 로직을 캡슐화하고, 테스트를 쉽게 만들며, 나중에 다른 ORM이나 데이터 소스로 교체할 수 있습니다. 하지만 EF Core와 함께 사용할 때는 논란이 있습니다.

**찬성 측 주장:**

1. **테스트 용이성**: Repository를 모킹하여 단위 테스트를 작성할 수 있습니다.
2. **비즈니스 로직 분리**: 컨트롤러가 DbContext를 직접 알 필요가 없습니다.
3. **재사용성**: 공통 쿼리를 Repository 메서드로 캡슐화합니다.

**반대 측 주장:**

1. **불필요한 추상화**: DbContext 자체가 이미 Repository와 Unit of Work 패턴입니다.
2. **LINQ 제한**: Repository 인터페이스는 LINQ의 유연성을 잃게 만듭니다.
3. **Generic Repository의 함정**: `IRepository<T>`는 모든 엔티티에 동일한 메서드를 제공하지만, 실제로는 엔티티마다 다른 쿼리가 필요합니다.

Microsoft의 공식 입장은 중립적입니다. Repository 패턴이 팀의 요구사항에 맞으면 사용하고, 그렇지 않으면 DbContext를 직접 사용해도 된다고 합니다. 현대의 경향은 **CQRS(Command Query Responsibility Segregation)**와 **MediatR** 같은 패턴으로 이동하고 있으며, 이는 Repository보다 더 명확한 책임 분리를 제공합니다.

### 다중 데이터베이스 지원: 진정한 크로스 플랫폼

EF Core의 강력한 점 중 하나는 다양한 데이터베이스를 지원한다는 것입니다. 동일한 코드로 다양한 RDBMS에서 작동하며, 심지어 NoSQL도 지원합니다.

**지원하는 주요 데이터베이스:**

- **SQL Server**: Microsoft의 주력 데이터베이스, 완벽한 지원
- **PostgreSQL**: 오픈 소스 커뮤니티에서 선호, Npgsql 제공자
- **MySQL/MariaDB**: 널리 사용되는 오픈 소스 데이터베이스
- **SQLite**: 로컬 개발과 모바일 앱에 적합
- **Oracle**: 엔터프라이즈 환경
- **Cosmos DB**: Azure의 NoSQL 데이터베이스
- **In-Memory**: 테스트용 메모리 데이터베이스

데이터베이스를 변경하려면 NuGet 패키지와 연결 문자열만 바꾸면 됩니다:

```csharp
// PostgreSQL 사용
// Install-Package Npgsql.EntityFrameworkCore.PostgreSQL

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// SQLite로 변경
// Install-Package Microsoft.EntityFrameworkCore.Sqlite

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
```

대부분의 쿼리는 수정 없이 작동합니다. 데이터베이스별 차이는 EF Core가 처리합니다. 물론 100% 호환되지는 않습니다. 전문 검색, JSON 함수, 특정 데이터 타입은 데이터베이스마다 다를 수 있습니다. 하지만 80-90%의 일반적인 쿼리는 포팅 작업 없이 동작합니다.

### Part 5에서 배울 내용

이 Part에서 우리는 Entity Framework Core의 깊이 있는 세계로 들어갑니다. 단순히 "데이터베이스에서 데이터 가져오기"를 넘어, 현대적인 데이터 액세스 계층을 설계하고 최적화하는 방법을 배웁니다.

**Chapter 12: Entity Framework Core 기초**

먼저 기초부터 탄탄히 다집니다. DbContext와 Entity 클래스의 관계, Code-First 마이그레이션의 작동 원리, LINQ to Entities의 기본 사용법을 배웁니다. 간단한 블로그 애플리케이션을 만들며 CRUD 작업을 직접 구현해봅니다. Prisma나 TypeORM을 사용해봤다면 빠르게 익숙해질 것이고, ORM이 처음이라면 왜 이것이 현대 개발의 표준인지 이해하게 될 것입니다.

```csharp
// DbContext 정의부터
public class BlogContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
}

// 마이그레이션 생성과 적용
// $ dotnet ef migrations add InitialCreate
// $ dotnet ef database update

// 쿼리 작성까지
var recentPosts = await context.Posts
    .Where(p => p.PublishedAt > DateTime.UtcNow.AddDays(-7))
    .Include(p => p.Comments)
    .OrderByDescending(p => p.PublishedAt)
    .ToListAsync();
```

**Chapter 13: Entity Framework Core 고급**

기초를 마스터했다면 이제 실전입니다. 복잡한 쿼리 작성, N+1 문제 해결, 인덱싱 전략, Compiled Queries를 통한 성능 최적화를 다룹니다. Repository 패턴의 장단점을 논의하고, 언제 사용하고 언제 피해야 하는지 배웁니다. 여러 데이터베이스를 지원하는 방법과, Cosmos DB 같은 NoSQL과의 통합도 살펴봅니다.

```csharp
// 복잡한 쿼리와 최적화
var statistics = await context.Users
    .AsNoTracking()
    .Select(u => new UserStats
    {
        UserId = u.Id,
        UserName = u.FirstName + " " + u.LastName,
        TotalPosts = u.Posts.Count,
        TotalComments = u.Posts.SelectMany(p => p.Comments).Count(),
        AvgPostLength = u.Posts.Average(p => p.Content.Length),
        MostRecentPost = u.Posts
            .OrderByDescending(p => p.PublishedAt)
            .Select(p => p.Title)
            .FirstOrDefault()
    })
    .Where(s => s.TotalPosts > 10)
    .OrderByDescending(s => s.TotalComments)
    .Take(20)
    .ToListAsync();
// 복잡하지만 여전히 타입 안전!
```

이 Part를 마치면, 프론트엔드 개발자에서 진정한 풀스택 개발자로 진화합니다. 데이터베이스는 더 이상 블랙박스가 아니라, 정확히 제어할 수 있는 도구가 됩니다. TypeScript로 타입 안전한 프론트엔드를 만들던 것처럼, 이제 C#과 EF Core로 타입 안전한 데이터 계층을 만들 수 있습니다.

JavaScript의 Prisma가 ORM의 새로운 방향을 제시했다면, EF Core는 성숙하고 검증된 강력함을 제공합니다. 두 세계의 장점을 이해하고, 프로젝트에 맞는 도구를 선택할 수 있는 안목을 갖추게 될 것입니다.

---

## 챕터 구성

### [Chapter 12: Entity Framework Core 기초](./chapter12/index.md)
DbContext와 Entity 클래스, Code-First 마이그레이션, LINQ to Entities 기본, 관계 설정, CRUD 작업을 실습과 함께 배웁니다.

### [Chapter 13: Entity Framework Core 고급](./chapter13/index.md)
복잡한 쿼리 기법, 성능 최적화, N+1 문제 해결, Repository 패턴, 다중 데이터베이스 지원, 벌크 작업을 마스터합니다.

---

## 학습 목표

이 Part를 완료하면 다음을 할 수 있습니다:

1. **ORM의 본질 이해**: JavaScript/TypeScript ORM과 비교하며 EF Core의 장단점을 설명할 수 있습니다.

2. **Code-First 개발**: 엔티티 클래스로 데이터베이스 스키마를 정의하고, 마이그레이션으로 버전 관리할 수 있습니다.

3. **타입 안전한 쿼리**: LINQ를 사용하여 복잡한 쿼리를 작성하고, SQL 인젝션 없이 데이터베이스를 조작할 수 있습니다.

4. **관계 탐색**: 일대다, 다대다 관계를 설정하고, Eager/Lazy Loading으로 효율적으로 로드할 수 있습니다.

5. **성능 최적화**: N+1 문제를 식별하고 해결하며, AsNoTracking과 프로젝션으로 쿼리를 최적화할 수 있습니다.

6. **아키텍처 패턴**: Repository 패턴, Unit of Work 패턴의 적용 시점을 판단할 수 있습니다.

7. **다중 데이터베이스**: PostgreSQL, MySQL, SQLite, SQL Server를 자유롭게 전환하며 사용할 수 있습니다.

---

## 사전 준비

Part 5를 시작하기 전에 필요한 것들:

- **Part 1 완료**: C# 기본 문법과 LINQ에 대한 이해
- **Part 2 완료**: ASP.NET Core 기초와 의존성 주입
- **데이터베이스 기본 지식**: SQL의 기본적인 개념 (SELECT, INSERT, UPDATE, DELETE, JOIN)
- **개발 환경**:
  - .NET 8 SDK 이상
  - 데이터베이스 (PostgreSQL, SQL Server, MySQL, 또는 SQLite)
  - EF Core CLI 도구: `dotnet tool install --global dotnet-ef`
  - 데이터베이스 클라이언트 (pgAdmin, SQL Server Management Studio, DBeaver 등)

---

## 추천 학습 순서

1. **Chapter 12부터 순서대로**: 기초 없이 고급으로 가면 혼란스럽습니다.

2. **코드를 직접 작성**: 읽기만 하지 말고, 간단한 프로젝트를 만들며 따라해보세요. 블로그, TODO 앱, 간단한 전자상거래 등.

3. **SQL 비교**: EF Core가 생성하는 SQL을 확인하세요. `options.LogTo(Console.WriteLine)`로 쿼리를 출력할 수 있습니다.

4. **마이그레이션 실습**: 스키마를 수정하고 마이그레이션을 생성하며, 롤백도 해보세요. 실수를 두려워하지 마세요.

5. **성능 측정**: 같은 쿼리를 여러 방식으로 작성하고, 어떤 SQL이 생성되는지, 얼마나 빠른지 비교해보세요.

6. **JavaScript ORM과 비교**: Prisma나 TypeORM을 알고 있다면, 동일한 기능을 EF Core로 구현하며 차이를 느껴보세요.

---

데이터베이스는 모든 애플리케이션의 심장입니다. EF Core를 마스터하면, 여러분의 애플리케이션은 단순한 UI를 넘어 진정한 가치를 제공하는 시스템이 됩니다. 타입 안전성, 성능, 유지보수성—모든 것을 갖춘 데이터 계층을 구축할 준비가 되었나요? 시작해봅시다!
