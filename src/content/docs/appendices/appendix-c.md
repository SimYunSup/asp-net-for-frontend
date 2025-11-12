---
title: "Appendix C - Entity Framework Core 마이그레이션 가이드"
---

# Appendix C: Entity Framework Core 마이그레이션 가이드

Entity Framework Core 마이그레이션은 데이터베이스 스키마를 코드로 관리하는 강력한 도구입니다. 이 가이드는 마이그레이션의 모든 측면을 실무 중심으로 다룹니다.

## 1. 마이그레이션 기본 개념

### 1.1 마이그레이션이란?

마이그레이션은 데이터베이스 스키마 변경사항을 코드로 버전 관리하는 방식입니다. 프론트엔드 개발자에게는 Prisma Migrate나 TypeORM 마이그레이션과 유사한 개념입니다.

**Prisma 비교**
```javascript
// Prisma: schema.prisma 수정 후
npx prisma migrate dev --name add_user_table
npx prisma migrate deploy

// EF Core: 모델 클래스 수정 후
dotnet ef migrations add AddUserTable
dotnet ef database update
```

### 1.2 마이그레이션 작동 방식

```
1. 모델 클래스 변경
   ↓
2. 마이그레이션 생성 (dotnet ef migrations add)
   ↓
3. 마이그레이션 파일 생성 (Up/Down 메서드)
   ↓
4. 데이터베이스 업데이트 (dotnet ef database update)
   ↓
5. __EFMigrationsHistory 테이블에 기록
```

## 2. 초기 설정

### 2.1 EF Core CLI 도구 설치

```bash
# 전역 설치
dotnet tool install --global dotnet-ef

# 버전 확인
dotnet ef --version
# 출력 예: Entity Framework Core .NET Command-line Tools 8.0.0

# 업데이트
dotnet tool update --global dotnet-ef
```

### 2.2 필수 패키지 설치

```bash
# 기본 EF Core 패키지
dotnet add package Microsoft.EntityFrameworkCore

# SQL Server 프로바이더
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# 디자인 타임 도구 (마이그레이션 생성에 필요)
dotnet add package Microsoft.EntityFrameworkCore.Design

# PostgreSQL 사용 시
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# MySQL 사용 시
dotnet add package Pomelo.EntityFrameworkCore.MySql
```

### 2.3 DbContext 설정

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 모델 구성
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });
    }
}
```

**Program.cs 등록**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## 3. 마이그레이션 생성

### 3.1 첫 번째 마이그레이션

```bash
# 초기 마이그레이션 생성
dotnet ef migrations add InitialCreate

# 생성되는 파일:
# Migrations/
# ├── 20240101120000_InitialCreate.cs
# ├── 20240101120000_InitialCreate.Designer.cs
# └── ApplicationDbContextModelSnapshot.cs
```

**생성된 마이그레이션 파일**
```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Products");
    }
}
```

### 3.2 추가 마이그레이션

```bash
# 새 칼럼 추가 마이그레이션
dotnet ef migrations add AddProductDescription

# 외래 키 추가 마이그레이션
dotnet ef migrations add AddCategoryToProduct

# 인덱스 추가 마이그레이션
dotnet ef migrations add AddIndexOnProductName
```

**예제: 칼럼 추가 마이그레이션**
```csharp
public partial class AddProductDescription : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "Products",
            type: "nvarchar(1000)",
            maxLength: 1000,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Description",
            table: "Products");
    }
}
```

### 3.3 출력 디렉터리 지정

```bash
# 특정 디렉터리에 마이그레이션 생성
dotnet ef migrations add AddUserTable --output-dir Data/Migrations

# 특정 프로젝트에서 마이그레이션 생성
dotnet ef migrations add AddUserTable --project ./src/MyApp.Data --startup-project ./src/MyApp.Api
```

## 4. 데이터베이스 업데이트

### 4.1 마이그레이션 적용

```bash
# 최신 마이그레이션까지 업데이트
dotnet ef database update

# 특정 마이그레이션까지 업데이트
dotnet ef database update AddProductDescription

# 모든 마이그레이션 롤백 (데이터베이스 초기화)
dotnet ef database update 0

# 연결 문자열 명시적으로 지정
dotnet ef database update -- --ConnectionStrings:DefaultConnection "Server=localhost;Database=MyDb;User=sa;Password=Pass123;"
```

### 4.2 런타임 마이그레이션

애플리케이션 시작 시 자동으로 마이그레이션을 적용할 수 있습니다.

```csharp
// Program.cs
var app = builder.Build();

// 개발 환경에서만 자동 마이그레이션
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();
```

**프로덕션 환경 고려사항**
```csharp
// 더 안전한 접근 방식
public static async Task ApplyMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            logger.LogInformation("Applying {Count} pending migrations", pendingMigrations.Count());
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully");
        }
        else
        {
            logger.LogInformation("No pending migrations");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error applying migrations");
        throw;
    }
}

// 사용
await ApplyMigrationsAsync(app);
```

## 5. SQL 스크립트 생성

### 5.1 마이그레이션 SQL 생성

```bash
# 모든 마이그레이션의 SQL 스크립트 생성
dotnet ef migrations script

# 특정 마이그레이션부터 SQL 생성
dotnet ef migrations script InitialCreate

# 특정 범위의 마이그레이션 SQL 생성
dotnet ef migrations script InitialCreate AddProductDescription

# 최신 마이그레이션만 SQL 생성
dotnet ef migrations script AddProductDescription AddCategoryToProduct

# 멱등성 스크립트 (여러 번 실행 가능)
dotnet ef migrations script --idempotent

# SQL 파일로 저장
dotnet ef migrations script --output migrations.sql
dotnet ef migrations script --idempotent --output idempotent-migrations.sql
```

**생성된 SQL 예제 (멱등성)**
```sql
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240101120000_InitialCreate')
BEGIN
    CREATE TABLE [Products] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
    );

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240101120000_InitialCreate', N'8.0.0');
END;
GO
```

### 5.2 프로덕션 배포 전략

```bash
# CI/CD 파이프라인에서 사용
# 1. SQL 스크립트 생성
dotnet ef migrations script --idempotent --output deploy/migrations.sql

# 2. DBA 검토 후 수동 실행
# 또는
# 3. 배포 파이프라인에서 자동 실행 (sqlcmd, Azure SQL 등)
```

## 6. 마이그레이션 관리

### 6.1 마이그레이션 목록 확인

```bash
# 모든 마이그레이션 목록
dotnet ef migrations list

# 출력 예:
# 20240101120000_InitialCreate (Applied)
# 20240102140000_AddProductDescription (Applied)
# 20240103160000_AddCategoryToProduct (Pending)
```

### 6.2 마이그레이션 제거

```bash
# 마지막 마이그레이션 제거 (아직 적용되지 않은 경우)
dotnet ef migrations remove

# 강제 제거 (이미 적용된 경우)
dotnet ef migrations remove --force

# 제거 전 데이터베이스 롤백 필요
dotnet ef database update PreviousMigration
dotnet ef migrations remove
```

### 6.3 마이그레이션 되돌리기

```bash
# 특정 마이그레이션으로 롤백
dotnet ef database update AddProductDescription

# 이후 불필요한 마이그레이션 파일 삭제
dotnet ef migrations remove
```

## 7. 복잡한 마이그레이션

### 7.1 데이터 시드 (Seed Data)

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 테이블 생성
    migrationBuilder.CreateTable(
        name: "Categories",
        columns: table => new
        {
            Id = table.Column<int>(nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            Name = table.Column<string>(maxLength: 100, nullable: false)
        });

    // 시드 데이터 삽입
    migrationBuilder.InsertData(
        table: "Categories",
        columns: new[] { "Name" },
        values: new object[,]
        {
            { "Electronics" },
            { "Clothing" },
            { "Books" }
        });
}
```

**OnModelCreating에서 시드 데이터 정의 (권장)**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Category>().HasData(
        new Category { Id = 1, Name = "Electronics" },
        new Category { Id = 2, Name = "Clothing" },
        new Category { Id = 3, Name = "Books" }
    );
}

// 마이그레이션 생성
// dotnet ef migrations add SeedCategories
```

### 7.2 칼럼 이름 변경 (데이터 보존)

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 단순 RenameColumn (데이터 보존됨)
    migrationBuilder.RenameColumn(
        name: "OldColumnName",
        table: "Products",
        newName: "NewColumnName");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn(
        name: "NewColumnName",
        table: "Products",
        newName: "OldColumnName");
}
```

### 7.3 테이블 이름 변경

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameTable(
        name: "OldTableName",
        newName: "NewTableName");
}
```

### 7.4 복잡한 데이터 변환

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. 새 칼럼 추가
    migrationBuilder.AddColumn<string>(
        name: "FullName",
        table: "Users",
        nullable: true);

    // 2. 데이터 변환 (SQL 실행)
    migrationBuilder.Sql(@"
        UPDATE Users
        SET FullName = FirstName + ' ' + LastName
        WHERE FullName IS NULL
    ");

    // 3. Not Null 제약 조건 추가
    migrationBuilder.AlterColumn<string>(
        name: "FullName",
        table: "Users",
        nullable: false);

    // 4. 기존 칼럼 제거
    migrationBuilder.DropColumn(name: "FirstName", table: "Users");
    migrationBuilder.DropColumn(name: "LastName", table: "Users");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    // 롤백 로직
    migrationBuilder.AddColumn<string>(name: "FirstName", table: "Users", nullable: false, defaultValue: "");
    migrationBuilder.AddColumn<string>(name: "LastName", table: "Users", nullable: false, defaultValue: "");

    migrationBuilder.Sql(@"
        UPDATE Users
        SET FirstName = SUBSTRING(FullName, 1, CHARINDEX(' ', FullName) - 1),
            LastName = SUBSTRING(FullName, CHARINDEX(' ', FullName) + 1, LEN(FullName))
    ");

    migrationBuilder.DropColumn(name: "FullName", table: "Users");
}
```

### 7.5 사용자 정의 SQL 실행

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 인덱스 생성
    migrationBuilder.Sql(@"
        CREATE INDEX IX_Products_Name_Price
        ON Products (Name, Price)
        INCLUDE (Description)
    ");

    // 저장 프로시저 생성
    migrationBuilder.Sql(@"
        CREATE PROCEDURE GetTopProducts
            @Count INT
        AS
        BEGIN
            SELECT TOP (@Count) *
            FROM Products
            ORDER BY Price DESC
        END
    ");

    // 뷰 생성
    migrationBuilder.Sql(@"
        CREATE VIEW ProductSummary AS
        SELECT
            p.Id,
            p.Name,
            p.Price,
            c.Name AS CategoryName
        FROM Products p
        INNER JOIN Categories c ON p.CategoryId = c.Id
    ");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql("DROP INDEX IX_Products_Name_Price ON Products");
    migrationBuilder.Sql("DROP PROCEDURE GetTopProducts");
    migrationBuilder.Sql("DROP VIEW ProductSummary");
}
```

## 8. 다중 DbContext

### 8.1 여러 DbContext 관리

```bash
# 특정 컨텍스트 지정
dotnet ef migrations add InitialCreate --context ApplicationDbContext
dotnet ef migrations add InitialCreate --context IdentityDbContext

# 데이터베이스 업데이트
dotnet ef database update --context ApplicationDbContext
dotnet ef database update --context IdentityDbContext
```

**마이그레이션 디렉터리 분리**
```csharp
public class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            connectionString,
            options => options.MigrationsAssembly("MyApp.Data"));
    }
}
```

## 9. 고급 시나리오

### 9.1 마이그레이션 번들 (EF Core 6.0+)

```bash
# 마이그레이션 번들 생성 (독립 실행 파일)
dotnet ef migrations bundle

# 특정 런타임용 번들 생성
dotnet ef migrations bundle --runtime linux-x64

# 자체 포함 번들
dotnet ef migrations bundle --runtime linux-x64 --self-contained

# 번들 실행
./efbundle
./efbundle --connection "Server=prod-server;Database=MyDb;..."

# Docker 컨테이너에서 사용
FROM mcr.microsoft.com/dotnet/runtime:8.0
COPY efbundle /app/efbundle
ENTRYPOINT ["/app/efbundle"]
```

### 9.2 마이그레이션 히스토리 테이블 커스터마이징

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer(
        connectionString,
        options => options.MigrationsHistoryTable("_MyMigrationsHistory", "audit"));
}
```

### 9.3 프로덕션 데이터 마이그레이션 전략

```csharp
// 1. Blue-Green 배포 방식
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1단계: 새 칼럼 추가 (nullable)
    migrationBuilder.AddColumn<string>(
        name: "NewColumn",
        table: "Users",
        nullable: true);
}

// 2단계: 애플리케이션 코드 업데이트 (두 칼럼 모두 사용)

// 3단계: 데이터 마이그레이션
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql("UPDATE Users SET NewColumn = OldColumn");
}

// 4단계: Not Null 제약 조건 추가
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AlterColumn<string>(
        name: "NewColumn",
        table: "Users",
        nullable: false);
}

// 5단계: 기존 칼럼 제거
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(name: "OldColumn", table: "Users");
}
```

## 10. 문제 해결

### 10.1 일반적인 오류

**오류: "Build failed"**
```bash
# 해결: 프로젝트를 먼저 빌드
dotnet build
dotnet ef migrations add MyMigration
```

**오류: "More than one DbContext was found"**
```bash
# 해결: 컨텍스트 명시
dotnet ef migrations add MyMigration --context ApplicationDbContext
```

**오류: "Unable to create an object of type 'DbContext'"**
```csharp
// 해결: IDesignTimeDbContextFactory 구현
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=MyDb;Trusted_Connection=True;");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
```

### 10.2 마이그레이션 충돌 해결

```bash
# 1. 최신 코드 가져오기
git pull

# 2. 충돌하는 마이그레이션 제거
dotnet ef migrations remove

# 3. 데이터베이스를 이전 마이그레이션으로 롤백
dotnet ef database update PreviousMigration

# 4. 최신 코드로 새 마이그레이션 생성
dotnet ef migrations add MyMigration

# 5. 데이터베이스 업데이트
dotnet ef database update
```

### 10.3 마이그레이션 히스토리 수동 조정

```sql
-- 마이그레이션 히스토리 확인
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;

-- 잘못된 마이그레이션 항목 제거
DELETE FROM __EFMigrationsHistory
WHERE MigrationId = '20240101120000_WrongMigration';

-- 마이그레이션 항목 수동 추가 (이미 적용된 경우)
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20240101120000_InitialCreate', '8.0.0');
```

## 11. 모범 사례

### 11.1 마이그레이션 명명 규칙

```bash
# ✅ Good: 설명적이고 명확한 이름
dotnet ef migrations add AddUserEmailColumn
dotnet ef migrations add CreateOrdersTable
dotnet ef migrations add AddIndexOnProductName
dotnet ef migrations add RenameCustomerToUser

# ❌ Bad: 모호하거나 일반적인 이름
dotnet ef migrations add Update
dotnet ef migrations add Fix
dotnet ef migrations add Migration1
```

### 11.2 마이그레이션 검토 체크리스트

- [ ] Up 메서드가 올바르게 구현되었는가?
- [ ] Down 메서드가 변경사항을 완전히 되돌리는가?
- [ ] 기존 데이터가 손실되지 않는가?
- [ ] 프로덕션 환경에서 안전하게 적용 가능한가?
- [ ] 롤백 가능한가?
- [ ] 성능에 영향을 주는 변경사항이 있는가? (대용량 테이블 스캔 등)
- [ ] 다운타임이 필요한가?

### 11.3 마이그레이션 전략

**개발 환경**
```bash
# 자주 마이그레이션 생성 및 적용
dotnet ef migrations add SmallChange
dotnet ef database update
```

**스테이징 환경**
```bash
# SQL 스크립트 생성 및 검토
dotnet ef migrations script --idempotent --output staging-migrations.sql
# DBA 또는 팀 리더 검토 후 적용
```

**프로덕션 환경**
```bash
# 1. 백업 먼저!
# 2. 멱등성 스크립트 생성
dotnet ef migrations script --idempotent --output prod-migrations.sql
# 3. 검토 및 테스트 (staging에서)
# 4. 프로덕션 적용
# 5. 롤백 계획 준비
```

## 12. Prisma/TypeORM과의 비교

### 12.1 Prisma Migrate

```bash
# Prisma
npx prisma migrate dev --name add_user_table
npx prisma migrate deploy
npx prisma migrate reset

# EF Core 동등한 명령어
dotnet ef migrations add AddUserTable
dotnet ef database update
dotnet ef database update 0 && dotnet ef database update
```

### 12.2 TypeORM Migrations

```typescript
// TypeORM: 마이그레이션 생성
npm run typeorm migration:generate -- -n AddUserTable
npm run typeorm migration:run
npm run typeorm migration:revert

// EF Core
dotnet ef migrations add AddUserTable
dotnet ef database update
dotnet ef database update PreviousMigration
```

### 12.3 Sequelize Migrations

```bash
# Sequelize
npx sequelize-cli migration:generate --name add-user-table
npx sequelize-cli db:migrate
npx sequelize-cli db:migrate:undo

# EF Core
dotnet ef migrations add AddUserTable
dotnet ef database update
dotnet ef database update PreviousMigration
```

## 요약

Entity Framework Core 마이그레이션의 핵심 포인트:

1. **마이그레이션 라이프사이클**: 생성 → 검토 → 적용 → 관리
2. **멱등성 스크립트**: 프로덕션 배포에는 `--idempotent` 사용
3. **데이터 보존**: 칼럼/테이블 변경 시 기존 데이터 보존 전략 수립
4. **롤백 계획**: 항상 Down 메서드 구현 및 롤백 테스트
5. **단계적 배포**: 큰 변경은 여러 마이그레이션으로 분할
6. **검토 프로세스**: 프로덕션 적용 전 SQL 스크립트 검토
7. **백업**: 프로덕션 마이그레이션 전 반드시 백업

프론트엔드 개발자에게는 Prisma, TypeORM과 유사한 개념이지만, EF Core는 더 강력한 엔터프라이즈 기능(복잡한 SQL 실행, 마이그레이션 번들 등)을 제공합니다.
