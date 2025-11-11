# Chapter 1: 프론트엔드 개발자가 알아야 할 C# 핵심 개념

## 개요

이 챕터에서는 JavaScript/TypeScript 개발자가 C#을 학습할 때 알아야 할 핵심 개념들을 다룹니다. 익숙한 개념과의 비교를 통해 빠르게 C#의 핵심을 이해할 수 있도록 구성했습니다.

---

## 1.1 타입 시스템: TypeScript와 C#의 차이점

### 정적 타입 vs 동적 타입의 근본적 차이

**TypeScript의 타입 시스템:**
```typescript
// TypeScript: 컴파일 시간에만 타입 체크
let message: string = "Hello";
message = 123; // 컴파일 에러, 하지만 런타임에는 영향 없음 (JS로 변환 후)
```

**C#의 타입 시스템:**
```csharp
// C#: 컴파일 타임과 런타임 모두에서 타입 강제
string message = "Hello";
message = 123; // 컴파일 에러 - 절대 실행되지 않음
```

**핵심 차이점:**
- TypeScript: 타입 정보가 컴파일 후 사라짐 (타입 삭제)
- C#: 타입 정보가 런타임에도 유지됨 (리플렉션 가능)

### C#의 컴파일 타임 타입 체킹

C#은 컴파일러가 모든 타입을 엄격하게 검사합니다:

```csharp
// 암묵적 타입 변환이 제한적
int number = 10;
string text = number; // 에러! 명시적 변환 필요

// 명시적 변환 필요
string text = number.ToString();
```

### Nullable 참조 타입과 TypeScript의 strict mode 비교

**TypeScript의 strictNullChecks:**
```typescript
// tsconfig.json: "strictNullChecks": true
let name: string = "John";
name = null; // 에러!

let optionalName: string | null = "John";
optionalName = null; // OK
```

**C#의 Nullable 참조 타입 (C# 8.0+):**
```csharp
// .csproj에서 <Nullable>enable</Nullable> 설정

// Non-nullable 참조 타입 (기본)
string name = "John";
name = null; // 경고!

// Nullable 참조 타입
string? optionalName = "John";
optionalName = null; // OK

// Null 검사
if (optionalName != null)
{
    Console.WriteLine(optionalName.Length); // 안전
}

// Null 병합 연산자
string displayName = optionalName ?? "Guest";

// Null 조건부 연산자
int? length = optionalName?.Length;
```

**주요 차이점:**
| 특징 | TypeScript | C# |
|------|-----------|-----|
| 기본 설정 | opt-in (strictNullChecks) | opt-in (Nullable 활성화) |
| Null 표현 | `null \| undefined` | `null` (nullable types) |
| 연산자 | `?.`, `??`, `!` | `?.`, `??`, `!` (유사) |
| 런타임 체크 | 없음 (타입 삭제) | 있음 (NullReferenceException) |

### 제네릭: `<T>` 사용법의 유사점과 차이점

**TypeScript 제네릭:**
```typescript
// 기본 제네릭
function identity<T>(arg: T): T {
    return arg;
}

// 제네릭 인터페이스
interface Repository<T> {
    getById(id: number): T;
    getAll(): T[];
}

// 제네릭 클래스
class DataStore<T> {
    private items: T[] = [];

    add(item: T): void {
        this.items.push(item);
    }
}
```

**C# 제네릭:**
```csharp
// 기본 제네릭
public T Identity<T>(T arg)
{
    return arg;
}

// 제네릭 인터페이스
public interface IRepository<T>
{
    T GetById(int id);
    List<T> GetAll();
}

// 제네릭 클래스
public class DataStore<T>
{
    private List<T> items = new();

    public void Add(T item)
    {
        items.Add(item);
    }
}

// 제네릭 제약 조건 (TypeScript보다 강력)
public class EntityRepository<T> where T : class, IEntity, new()
{
    public T Create()
    {
        return new T(); // 'new()' 제약으로 인스턴스 생성 가능
    }
}
```

**제네릭 제약 조건 종류:**
```csharp
// where T : struct        - 값 타입만
// where T : class         - 참조 타입만
// where T : notnull       - null이 아닌 타입
// where T : unmanaged     - unmanaged 타입만
// where T : new()         - 매개변수 없는 생성자 필요
// where T : BaseClass     - 특정 클래스 상속
// where T : IInterface    - 특정 인터페이스 구현
// where T : U             - 다른 타입 매개변수로부터 파생

public class Example<T, U>
    where T : class, IComparable<T>, new()
    where U : struct
{
    // T는 참조 타입, IComparable 구현, 기본 생성자 필요
    // U는 값 타입
}
```

### 인터페이스와 추상 클래스의 실전 활용

**TypeScript 인터페이스:**
```typescript
// TypeScript: 인터페이스는 컴파일 후 사라짐
interface IUser {
    id: number;
    name: string;
    email: string;
    greet(): void;
}

// 구현
class User implements IUser {
    constructor(
        public id: number,
        public name: string,
        public email: string
    ) {}

    greet(): void {
        console.log(`Hello, ${this.name}`);
    }
}
```

**C# 인터페이스:**
```csharp
// C#: 인터페이스는 런타임에도 존재
public interface IUser
{
    int Id { get; }
    string Name { get; set; }
    string Email { get; set; }
    void Greet();
}

// 구현
public class User : IUser
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string Email { get; set; }

    public User(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public void Greet()
    {
        Console.WriteLine($"Hello, {Name}");
    }
}

// C# 8.0+: 인터페이스 기본 구현
public interface ILogger
{
    void Log(string message);

    // 기본 구현
    void LogError(string message)
    {
        Log($"ERROR: {message}");
    }
}
```

**추상 클래스 (TypeScript):**
```typescript
abstract class BaseRepository<T> {
    abstract getAll(): Promise<T[]>;
    abstract getById(id: number): Promise<T>;

    // 구현된 메서드
    async exists(id: number): Promise<boolean> {
        const item = await this.getById(id);
        return item !== null;
    }
}
```

**추상 클래스 (C#):**
```csharp
public abstract class BaseRepository<T> where T : class
{
    // 추상 메서드 (구현 필수)
    public abstract Task<List<T>> GetAllAsync();
    public abstract Task<T?> GetByIdAsync(int id);

    // 구현된 메서드
    public async Task<bool> ExistsAsync(int id)
    {
        var item = await GetByIdAsync(id);
        return item != null;
    }

    // 추상 프로퍼티
    protected abstract string TableName { get; }

    // 가상 메서드 (오버라이드 가능하지만 필수 아님)
    public virtual void Validate(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
    }
}
```

**인터페이스 vs 추상 클래스 선택 기준:**

| 기준 | 인터페이스 | 추상 클래스 |
|------|-----------|-----------|
| 다중 상속 | ✅ 가능 | ❌ 불가능 (단일 상속만) |
| 구현 포함 | C# 8.0+에서만 | ✅ 가능 |
| 필드 포함 | ❌ 불가능 | ✅ 가능 |
| 생성자 | ❌ 없음 | ✅ 있음 |
| 접근 제한자 | 모두 public | ✅ 다양하게 설정 |
| 용도 | 계약 정의 | 공통 기능 + 계약 |

---

## 1.2 JavaScript/TypeScript에 없는 C# 개념

### 값 타입(Value Types) vs 참조 타입(Reference Types)

**JavaScript의 메모리 모델:**
```javascript
// JavaScript: 원시 타입은 값 복사, 객체는 참조 복사
let a = 10;
let b = a;  // 값 복사
b = 20;
console.log(a); // 10

let obj1 = { x: 10 };
let obj2 = obj1;  // 참조 복사
obj2.x = 20;
console.log(obj1.x); // 20
```

**C#의 메모리 모델:**
```csharp
// 값 타입 (struct, int, bool, enum 등)
int a = 10;
int b = a;  // 값 복사
b = 20;
Console.WriteLine(a); // 10

// 참조 타입 (class)
class Point
{
    public int X { get; set; }
}

Point p1 = new Point { X = 10 };
Point p2 = p1;  // 참조 복사
p2.X = 20;
Console.WriteLine(p1.X); // 20

// struct (값 타입)
struct PointStruct
{
    public int X { get; set; }
}

PointStruct ps1 = new PointStruct { X = 10 };
PointStruct ps2 = ps1;  // 값 복사
ps2.X = 20;
Console.WriteLine(ps1.X); // 10 (독립적인 복사본)
```

**메모리 할당:**
- **값 타입**: 스택(Stack)에 저장 - 빠르지만 크기 제한
- **참조 타입**: 힙(Heap)에 저장 - 느리지만 크기 유연

### struct와 class의 차이점과 성능 영향

**class (참조 타입):**
```csharp
// 힙 할당, 참조 의미론
public class PersonClass
{
    public string Name { get; set; }
    public int Age { get; set; }

    public PersonClass(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

// 사용
PersonClass person1 = new PersonClass("John", 30);  // 힙 할당
PersonClass person2 = person1;  // 참조 복사
person2.Age = 31;
Console.WriteLine(person1.Age); // 31
```

**struct (값 타입):**
```csharp
// 스택 할당, 값 의미론
public struct PersonStruct
{
    public string Name { get; set; }
    public int Age { get; set; }

    public PersonStruct(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

// 사용
PersonStruct person1 = new PersonStruct("John", 30);  // 스택 할당
PersonStruct person2 = person1;  // 값 복사
person2.Age = 31;
Console.WriteLine(person1.Age); // 30
```

**언제 struct를 사용할까?**
1. 작은 데이터 구조 (16바이트 이하 권장)
2. 불변성이 중요한 경우
3. 빈번한 생성/소멸이 일어나는 경우
4. 컬렉션에 많이 저장되는 경우

```csharp
// 좋은 struct 예제
public readonly struct Point2D
{
    public double X { get; init; }
    public double Y { get; init; }

    public Point2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double DistanceFromOrigin() => Math.Sqrt(X * X + Y * Y);
}

// readonly struct: 불변성 보장
```

### 프로퍼티(Properties): getter/setter의 진화된 형태

**JavaScript/TypeScript getter/setter:**
```typescript
class User {
    private _name: string;

    get name(): string {
        return this._name;
    }

    set name(value: string) {
        if (!value) throw new Error("Name is required");
        this._name = value;
    }
}
```

**C# 프로퍼티:**
```csharp
// 자동 구현 프로퍼티 (가장 간단)
public class User
{
    public string Name { get; set; }  // 컴파일러가 backing field 자동 생성
    public int Age { get; set; }
}

// 읽기 전용 프로퍼티
public class User
{
    public string Name { get; }  // 생성자에서만 설정 가능

    public User(string name)
    {
        Name = name;
    }
}

// init 접근자 (C# 9.0+) - 객체 초기화 시에만 설정
public class User
{
    public string Name { get; init; }
    public int Age { get; init; }
}

var user = new User { Name = "John", Age = 30 };
// user.Name = "Jane"; // 에러! init만 가능

// 계산된 프로퍼티
public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    // Expression-bodied property
    public string FullName => $"{FirstName} {LastName}";

    // 또는
    public string FullName
    {
        get => $"{FirstName} {LastName}";
    }
}

// Backing field를 사용한 프로퍼티 (유효성 검사)
public class User
{
    private string _name;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name is required");
            _name = value;
        }
    }
}

// Required 프로퍼티 (C# 11+)
public class User
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public int? Age { get; init; }
}

// var user = new User(); // 에러! Name과 Email 필수
var user = new User { Name = "John", Email = "john@example.com" }; // OK
```

**프로퍼티 접근 제한자:**
```csharp
public class BankAccount
{
    // Public get, private set
    public decimal Balance { get; private set; }

    // Protected set
    public string AccountNumber { get; protected set; }

    // Internal get
    internal string InternalId { get; set; }

    public void Deposit(decimal amount)
    {
        if (amount > 0)
            Balance += amount;
    }
}
```

### 이벤트(Events)와 델리게이트(Delegates)

**JavaScript의 이벤트:**
```javascript
class Button {
    constructor() {
        this.listeners = [];
    }

    addEventListener(callback) {
        this.listeners.push(callback);
    }

    click() {
        this.listeners.forEach(listener => listener());
    }
}
```

**C# 델리게이트:**
```csharp
// 델리게이트 선언 (함수 타입 정의)
public delegate void ClickHandler(object sender, EventArgs e);

// 또는 내장 델리게이트 사용
// Action<T>: 반환값 없음
// Func<T, TResult>: 반환값 있음
// EventHandler<T>: 이벤트용
```

**C# 이벤트:**
```csharp
public class Button
{
    // 이벤트 선언
    public event EventHandler<EventArgs> Click;

    // 또는 커스텀 EventArgs
    public event EventHandler<ButtonClickEventArgs> CustomClick;

    // 이벤트 발생 메서드
    protected virtual void OnClick()
    {
        // Null 조건부 호출
        Click?.Invoke(this, EventArgs.Empty);
    }

    public void PerformClick()
    {
        OnClick();
    }
}

public class ButtonClickEventArgs : EventArgs
{
    public int X { get; set; }
    public int Y { get; set; }
}

// 사용
var button = new Button();

// 이벤트 구독 (+=)
button.Click += (sender, e) =>
{
    Console.WriteLine("Button clicked!");
};

button.Click += Button_Click;

void Button_Click(object sender, EventArgs e)
{
    Console.WriteLine("Another handler");
}

// 이벤트 해제 (-=)
button.Click -= Button_Click;

// 이벤트 발생
button.PerformClick();
```

**실전 예제: 데이터 변경 알림**
```csharp
public class DataStore<T>
{
    private T _data;

    // 데이터 변경 전/후 이벤트
    public event EventHandler<DataChangingEventArgs<T>> DataChanging;
    public event EventHandler<DataChangedEventArgs<T>> DataChanged;

    public T Data
    {
        get => _data;
        set
        {
            var changingArgs = new DataChangingEventArgs<T>(_data, value);
            DataChanging?.Invoke(this, changingArgs);

            if (!changingArgs.Cancel)
            {
                var oldValue = _data;
                _data = value;
                DataChanged?.Invoke(this, new DataChangedEventArgs<T>(oldValue, value));
            }
        }
    }
}

public class DataChangingEventArgs<T> : EventArgs
{
    public T OldValue { get; }
    public T NewValue { get; }
    public bool Cancel { get; set; }

    public DataChangingEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
```

### LINQ: SQL과 함수형 프로그래밍의 만남

**JavaScript 배열 메서드:**
```javascript
const numbers = [1, 2, 3, 4, 5];

const result = numbers
    .filter(n => n % 2 === 0)
    .map(n => n * 2)
    .reduce((sum, n) => sum + n, 0);
```

**C# LINQ:**
```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// 메서드 구문 (Method Syntax)
var result = numbers
    .Where(n => n % 2 == 0)
    .Select(n => n * 2)
    .Sum();

// 쿼리 구문 (Query Syntax) - SQL과 유사
var result2 = (from n in numbers
               where n % 2 == 0
               select n * 2).Sum();
```

**LINQ 주요 메서드:**
```csharp
var products = new List<Product>
{
    new Product { Id = 1, Name = "Laptop", Price = 1000, Category = "Electronics" },
    new Product { Id = 2, Name = "Mouse", Price = 25, Category = "Electronics" },
    new Product { Id = 3, Name = "Desk", Price = 300, Category = "Furniture" },
};

// Where - filter와 동일
var expensive = products.Where(p => p.Price > 100);

// Select - map과 동일
var names = products.Select(p => p.Name);

// OrderBy / OrderByDescending - sort
var sorted = products.OrderBy(p => p.Price);
var sortedDesc = products.OrderByDescending(p => p.Price);

// First / FirstOrDefault - find
var first = products.First(p => p.Price > 100);
var firstOrNull = products.FirstOrDefault(p => p.Price > 5000); // null if not found

// Any / All - some / every
bool hasExpensive = products.Any(p => p.Price > 100);
bool allExpensive = products.All(p => p.Price > 10);

// GroupBy - groupBy
var grouped = products.GroupBy(p => p.Category);
foreach (var group in grouped)
{
    Console.WriteLine($"{group.Key}: {group.Count()} items");
}

// Aggregate - reduce
var total = products.Aggregate(0m, (sum, p) => sum + p.Price);

// Join - SQL JOIN과 유사
var categories = new List<Category>
{
    new Category { Id = 1, Name = "Electronics" },
    new Category { Id = 2, Name = "Furniture" }
};

var joined = products.Join(
    categories,
    product => product.Category,
    category => category.Name,
    (product, category) => new { product.Name, Category = category.Name }
);

// SelectMany - flatMap
var orders = new List<Order>
{
    new Order { Items = new[] { "A", "B" } },
    new Order { Items = new[] { "C", "D" } }
};

var allItems = orders.SelectMany(o => o.Items); // ["A", "B", "C", "D"]

// Take / Skip - 페이징
var page1 = products.Skip(0).Take(10);
var page2 = products.Skip(10).Take(10);

// Distinct - unique values
var categories = products.Select(p => p.Category).Distinct();
```

**복잡한 쿼리 예제:**
```csharp
// 카테고리별 평균 가격이 100 이상인 제품들
var result = products
    .GroupBy(p => p.Category)
    .Where(g => g.Average(p => p.Price) >= 100)
    .SelectMany(g => g)
    .OrderBy(p => p.Name);

// 쿼리 구문
var result2 = from p in products
              group p by p.Category into g
              where g.Average(p => p.Price) >= 100
              from product in g
              orderby product.Name
              select product;
```

**LINQ to Objects vs LINQ to Entities (EF Core):**
```csharp
// LINQ to Objects: 메모리에서 실행
var memoryResult = products
    .Where(p => p.Price > 100)  // 메모리에서 필터링
    .ToList();

// LINQ to Entities: SQL로 변환되어 DB에서 실행
var dbResult = dbContext.Products
    .Where(p => p.Price > 100)  // SQL: WHERE Price > 100
    .ToList();
```

---

## 1.3 익숙한 개념, 다른 문법

### 화살표 함수 vs 람다 표현식

**JavaScript/TypeScript:**
```typescript
// 화살표 함수
const add = (a: number, b: number) => a + b;

const square = (x: number) => {
    const result = x * x;
    return result;
};

// 배열 메서드와 함께
numbers.map(n => n * 2);
numbers.filter(n => n > 10);
```

**C# 람다:**
```csharp
// 람다 표현식
Func<int, int, int> add = (a, b) => a + b;

Func<int, int> square = x =>
{
    var result = x * x;
    return result;
};

// LINQ와 함께
numbers.Select(n => n * 2);
numbers.Where(n => n > 10);

// 델리게이트 타입
// Action: 반환값 없음
Action<string> print = message => Console.WriteLine(message);

// Func: 반환값 있음
Func<int, int> double = x => x * 2;
Func<int, int, int> add = (a, b) => a + b;

// Predicate: bool 반환
Predicate<int> isEven = n => n % 2 == 0;

// C# 10+: 자연스러운 타입 추론
var multiply = (int x, int y) => x * y;

// C# 13: 기본 람다 매개변수 (새로운 기능!)
var increment = (int x, int step = 1) => x + step;
Console.WriteLine(increment(5));    // 6
Console.WriteLine(increment(5, 2)); // 7
```

**람다 vs 로컬 함수:**
```csharp
public class Calculator
{
    public int Calculate(int[] numbers)
    {
        // 람다 표현식
        Func<int, int> double = x => x * 2;

        // 로컬 함수 (더 효율적, 재귀 가능)
        int Triple(int x)
        {
            return x * 3;
        }

        // 재귀 로컬 함수
        int Factorial(int n)
        {
            return n <= 1 ? 1 : n * Factorial(n - 1);
        }

        return numbers.Select(double).Sum() + Triple(10);
    }
}
```

### Promise/async-await vs Task/async-await

**JavaScript Promise:**
```javascript
// Promise
function fetchUser(id) {
    return fetch(`/api/users/${id}`)
        .then(response => response.json())
        .then(user => user);
}

// async/await
async function fetchUser(id) {
    const response = await fetch(`/api/users/${id}`);
    const user = await response.json();
    return user;
}

// 병렬 실행
async function fetchMultipleUsers() {
    const [user1, user2] = await Promise.all([
        fetchUser(1),
        fetchUser(2)
    ]);
}
```

**C# Task:**
```csharp
// Task (Promise와 유사)
public Task<User> FetchUserAsync(int id)
{
    return httpClient.GetFromJsonAsync<User>($"/api/users/{id}");
}

// async/await (거의 동일한 구문!)
public async Task<User> FetchUserAsync(int id)
{
    var response = await httpClient.GetAsync($"/api/users/{id}");
    var user = await response.Content.ReadFromJsonAsync<User>();
    return user;
}

// 병렬 실행
public async Task<(User, User)> FetchMultipleUsersAsync()
{
    var task1 = FetchUserAsync(1);
    var task2 = FetchUserAsync(2);

    // await Task.WhenAll - Promise.all과 동일
    await Task.WhenAll(task1, task2);

    return (task1.Result, task2.Result);

    // 또는
    var users = await Task.WhenAll(task1, task2);
    return (users[0], users[1]);
}

// 예외 처리
public async Task<User> SafeFetchUserAsync(int id)
{
    try
    {
        return await FetchUserAsync(id);
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return null;
    }
}
```

**Task vs ValueTask:**
```csharp
// Task: 항상 힙 할당
public async Task<int> GetValueAsync()
{
    await Task.Delay(100);
    return 42;
}

// ValueTask: 동기 결과일 때 힙 할당 없음 (성능 최적화)
public async ValueTask<int> GetCachedValueAsync(int id)
{
    if (cache.TryGetValue(id, out var value))
    {
        return value; // 동기 반환, 힙 할당 없음
    }

    var result = await FetchFromDatabaseAsync(id);
    cache[id] = result;
    return result;
}
```

**취소 토큰 (CancellationToken):**
```csharp
public async Task<User> FetchUserAsync(int id, CancellationToken cancellationToken)
{
    var response = await httpClient.GetAsync($"/api/users/{id}", cancellationToken);
    return await response.Content.ReadFromJsonAsync<User>(cancellationToken);
}

// 사용
var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(5)); // 5초 후 취소

try
{
    var user = await FetchUserAsync(1, cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request cancelled");
}
```

### 구조 분해 할당과 패턴 매칭

**JavaScript 구조 분해:**
```javascript
// 객체 구조 분해
const { name, age } = user;

// 배열 구조 분해
const [first, second] = array;

// 함수 매개변수
function greet({ name, age }) {
    console.log(`${name} is ${age} years old`);
}
```

**C# 구조 분해 (Deconstruction):**
```csharp
// 튜플 구조 분해
var (name, age) = GetUser();

(string name, int age) GetUser()
{
    return ("John", 30);
}

// 여러 값 반환 (튜플)
public (string Name, int Age, string Email) GetUserInfo(int id)
{
    return ("John", 30, "john@example.com");
}

var (name, age, email) = GetUserInfo(1);

// 일부만 필요할 때
var (name, _, email) = GetUserInfo(1); // age 무시

// 커스텀 타입 구조 분해
public class User
{
    public string Name { get; set; }
    public int Age { get; set; }

    // Deconstruct 메서드
    public void Deconstruct(out string name, out int age)
    {
        name = Name;
        age = Age;
    }
}

var user = new User { Name = "John", Age = 30 };
var (userName, userAge) = user; // Deconstruct 호출
```

**C# 패턴 매칭 (JavaScript보다 강력):**
```csharp
// Switch 표현식
string GetDiscount(int age) => age switch
{
    < 18 => "Student discount",
    >= 65 => "Senior discount",
    _ => "No discount"
};

// 타입 패턴
object obj = "Hello";
string message = obj switch
{
    string s => $"String: {s}",
    int i => $"Integer: {i}",
    null => "Null",
    _ => "Unknown"
};

// 프로퍼티 패턴
User user = GetUser();
string category = user switch
{
    { Age: < 18 } => "Child",
    { Age: >= 18, Age: < 65 } => "Adult",
    { Age: >= 65 } => "Senior",
    _ => "Unknown"
};

// 위치 패턴 (튜플)
(int, int) point = (0, 0);
string quadrant = point switch
{
    (0, 0) => "Origin",
    (> 0, > 0) => "Quadrant 1",
    (< 0, > 0) => "Quadrant 2",
    (< 0, < 0) => "Quadrant 3",
    (> 0, < 0) => "Quadrant 4",
    _ => "On axis"
};

// 리스트 패턴 (C# 11+)
int[] numbers = { 1, 2, 3 };
string description = numbers switch
{
    [] => "Empty",
    [1] => "Single element: 1",
    [1, 2] => "Two elements: 1, 2",
    [1, 2, 3] => "Three elements: 1, 2, 3",
    [1, .., 10] => "Starts with 1, ends with 10",
    _ => "Other"
};

// is 패턴
if (obj is string s)
{
    Console.WriteLine(s.Length); // s는 여기서 string 타입
}

if (user is { Age: >= 18, Name: var name })
{
    Console.WriteLine($"{name} is an adult");
}
```

### 모듈 시스템: ES6 modules vs C# namespaces

**JavaScript/TypeScript 모듈:**
```typescript
// user.ts
export class User {
    constructor(public name: string) {}
}

export function createUser(name: string): User {
    return new User(name);
}

export const DEFAULT_USER = new User("Guest");

// main.ts
import { User, createUser, DEFAULT_USER } from './user';
import * as UserModule from './user';
```

**C# 네임스페이스와 using:**
```csharp
// User.cs
namespace MyApp.Models
{
    public class User
    {
        public string Name { get; set; }

        public User(string name)
        {
            Name = name;
        }
    }

    public static class UserFactory
    {
        public static User CreateUser(string name)
        {
            return new User(name);
        }

        public static readonly User DefaultUser = new("Guest");
    }
}

// Program.cs
using MyApp.Models;

var user = new User("John");
var defaultUser = UserFactory.DefaultUser;

// 별칭 사용
using UserModel = MyApp.Models.User;

// C# 10+: 파일 스코프 네임스페이스 (더 간결)
namespace MyApp.Models;

public class User
{
    public string Name { get; set; }
}

// C# 10+: Global using (프로젝트 전체에 적용)
// GlobalUsings.cs
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using MyApp.Models;
```

**네임스페이스 vs 모듈 차이점:**
| 특징 | ES6 Modules | C# Namespaces |
|------|-------------|---------------|
| 파일 관계 | 1 파일 = 1 모듈 | 여러 파일이 같은 네임스페이스 가능 |
| Import 방식 | 명시적 import | using 지시문 |
| 기본 접근성 | export 필요 | public/internal |
| 로딩 | 동적 import 가능 | 컴파일 타임에 해결 |

### `var`, `let`, `const` vs `var`, `let`, `const`

**JavaScript/TypeScript:**
```typescript
var x = 10;        // 함수 스코프, 호이스팅
let y = 20;        // 블록 스코프, 재할당 가능
const z = 30;      // 블록 스코프, 재할당 불가

// 객체는 const여도 내용 변경 가능
const obj = { x: 10 };
obj.x = 20; // OK
```

**C#:**
```csharp
// var: 타입 추론 (JavaScript와 완전히 다름!)
var x = 10;        // int로 추론, 이후 타입 변경 불가
var name = "John"; // string으로 추론

// x = "text"; // 에러! int 타입으로 이미 결정됨

// 명시적 타입
int y = 20;
string text = "Hello";

// const: 컴파일 타임 상수 (값이 코드에 포함됨)
const int MaxItems = 100;
const string AppName = "MyApp";

// readonly: 런타임 상수 (생성자에서만 할당 가능)
public class Config
{
    public readonly string ConnectionString;

    public Config(string connString)
    {
        ConnectionString = connString;
        // ConnectionString = "new"; // 에러! 생성자 외부에서 불가
    }
}

// C# 9+: Target-typed new
List<string> names = new(); // new List<string>()와 동일

// C# 12+: Primary constructors
public class User(string name, int age)
{
    public string Name => name;
    public int Age => age;
}
```

**스코프 규칙:**
```csharp
public void Example()
{
    // C#의 모든 변수는 블록 스코프
    if (true)
    {
        var x = 10;
        int y = 20;
    }

    // Console.WriteLine(x); // 에러! 스코프 밖

    // 같은 스코프에서 재선언 불가
    var name = "John";
    // var name = "Jane"; // 에러!
}
```

---

## 1.4 객체지향 프로그래밍: 더 엄격한 세계

### 클래스 기반 OOP의 전통적 접근

**TypeScript:**
```typescript
class Animal {
    name: string;

    constructor(name: string) {
        this.name = name;
    }

    makeSound(): void {
        console.log("Some sound");
    }
}
```

**C#:**
```csharp
public class Animal
{
    // 필드 (private이 기본 권장)
    private string _name;

    // 프로퍼티 (public 권장)
    public string Name
    {
        get => _name;
        set => _name = value;
    }

    // 자동 구현 프로퍼티 (더 간결)
    public int Age { get; set; }

    // 생성자
    public Animal(string name)
    {
        _name = name;
    }

    // 메서드
    public virtual void MakeSound()
    {
        Console.WriteLine("Some sound");
    }
}
```

### 접근 제한자: `public`, `private`, `protected`, `internal`

```csharp
public class AccessModifierExample
{
    // public: 어디서나 접근 가능
    public string PublicField = "public";

    // private: 같은 클래스 내에서만 (기본값)
    private string _privateField = "private";

    // protected: 같은 클래스 + 파생 클래스
    protected string ProtectedField = "protected";

    // internal: 같은 어셈블리(프로젝트) 내에서만
    internal string InternalField = "internal";

    // protected internal: protected OR internal (합집합)
    protected internal string ProtectedInternalField = "protected internal";

    // private protected: protected AND internal (교집합) - C# 7.2+
    private protected string PrivateProtectedField = "private protected";
}

public class DerivedClass : AccessModifierExample
{
    public void Test()
    {
        var x = PublicField;        // OK
        // var y = _privateField;   // 에러!
        var z = ProtectedField;     // OK
        var w = InternalField;      // OK (같은 어셈블리)
    }
}
```

**TypeScript와의 비교:**
| 접근 제한자 | TypeScript | C# |
|------------|-----------|-----|
| public | ✅ (기본값) | ✅ |
| private | ✅ (컴파일만) | ✅ (런타임도) |
| protected | ✅ | ✅ |
| internal | ❌ | ✅ (어셈블리 레벨) |
| private protected | ❌ | ✅ |

### 상속과 다형성: TypeScript보다 강력한 제약

**TypeScript:**
```typescript
class Animal {
    makeSound(): void {
        console.log("Some sound");
    }
}

class Dog extends Animal {
    makeSound(): void {
        console.log("Woof!");
    }
}
```

**C#:**
```csharp
// 기본 클래스
public class Animal
{
    // virtual: 오버라이드 가능
    public virtual void MakeSound()
    {
        Console.WriteLine("Some sound");
    }

    // sealed 메서드: 더 이상 오버라이드 불가
    public sealed void Eat()
    {
        Console.WriteLine("Eating...");
    }
}

// 파생 클래스
public class Dog : Animal
{
    // override: 명시적으로 오버라이드
    public override void MakeSound()
    {
        Console.WriteLine("Woof!");
    }

    // new: 기본 메서드 숨김 (shadowing)
    public new void Eat()
    {
        Console.WriteLine("Dog eating...");
    }
}

// sealed 클래스: 상속 불가
public sealed class FinalClass
{
    // 더 이상 상속할 수 없음
}

// abstract 클래스
public abstract class Shape
{
    // 추상 메서드: 구현 필수
    public abstract double GetArea();

    // 일반 메서드
    public void Display()
    {
        Console.WriteLine($"Area: {GetArea()}");
    }
}

public class Circle : Shape
{
    public double Radius { get; set; }

    // 추상 메서드 구현 필수
    public override double GetArea()
    {
        return Math.PI * Radius * Radius;
    }
}
```

**다형성 예제:**
```csharp
// 다형성: 기본 클래스 타입으로 파생 클래스 인스턴스 참조
Animal animal = new Dog();
animal.MakeSound(); // "Woof!" (Dog의 메서드 호출)

// 타입 캐스팅
Dog dog = (Dog)animal;  // 명시적 캐스트

// 안전한 캐스팅
if (animal is Dog d)
{
    d.MakeSound(); // Dog의 메서드
}

// as 연산자
Dog? maybeDog = animal as Dog;
if (maybeDog != null)
{
    maybeDog.MakeSound();
}
```

### 인터페이스 구현의 명시성

```csharp
// 인터페이스 정의
public interface IRepository<T>
{
    T GetById(int id);
    void Add(T item);
    void Update(T item);
    void Delete(int id);
}

public interface ILoggable
{
    void Log(string message);
}

// 다중 인터페이스 구현
public class UserRepository : IRepository<User>, ILoggable
{
    public User GetById(int id)
    {
        // 구현
        return new User();
    }

    public void Add(User item)
    {
        // 구현
    }

    public void Update(User item)
    {
        // 구현
    }

    public void Delete(int id)
    {
        // 구현
    }

    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}

// 명시적 인터페이스 구현
public class MultiRepository : IRepository<User>, IRepository<Product>
{
    // IRepository<User> 구현
    User IRepository<User>.GetById(int id)
    {
        return new User();
    }

    void IRepository<User>.Add(User item) { }
    void IRepository<User>.Update(User item) { }
    void IRepository<User>.Delete(int id) { }

    // IRepository<Product> 구현
    Product IRepository<Product>.GetById(int id)
    {
        return new Product();
    }

    void IRepository<Product>.Add(Product item) { }
    void IRepository<Product>.Update(Product item) { }
    void IRepository<Product>.Delete(int id) { }
}

// 사용
var repo = new MultiRepository();
var user = ((IRepository<User>)repo).GetById(1);
var product = ((IRepository<Product>)repo).GetById(1);
```

### 추상 클래스 활용 패턴

```csharp
// 추상 클래스: 공통 기능 + 추상 메서드
public abstract class BaseController
{
    protected ILogger Logger { get; }

    protected BaseController(ILogger logger)
    {
        Logger = logger;
    }

    // 추상 메서드
    protected abstract string GetControllerName();

    // 템플릿 메서드 패턴
    public void HandleRequest()
    {
        Logger.Log($"Handling request in {GetControllerName()}");
        BeforeAction();
        ExecuteAction();
        AfterAction();
    }

    protected virtual void BeforeAction()
    {
        Logger.Log("Before action");
    }

    protected abstract void ExecuteAction();

    protected virtual void AfterAction()
    {
        Logger.Log("After action");
    }
}

public class UserController : BaseController
{
    public UserController(ILogger logger) : base(logger)
    {
    }

    protected override string GetControllerName() => "UserController";

    protected override void ExecuteAction()
    {
        Logger.Log("Executing user action");
    }

    // BeforeAction과 AfterAction은 선택적 오버라이드
}
```

---

## 1.5 C# 13의 최신 기능 (2025 기준)

### 기본 람다 매개변수

```csharp
// C# 13: 람다 표현식에 기본 매개변수 지원
var greet = (string name = "Guest") => $"Hello, {name}!";

Console.WriteLine(greet());          // Hello, Guest!
Console.WriteLine(greet("John"));    // Hello, John!

// LINQ에서 활용
var increment = (int x, int step = 1) => x + step;
var numbers = new[] { 1, 2, 3, 4, 5 };
var incremented = numbers.Select(n => increment(n));     // [2, 3, 4, 5, 6]
var incrementedBy2 = numbers.Select(n => increment(n, 2)); // [3, 4, 5, 6, 7]
```

### 향상된 패턴 매칭

```csharp
// 개선된 리스트 패턴
int[] numbers = { 1, 2, 3, 4, 5 };

var result = numbers switch
{
    [1, ..] => "Starts with 1",
    [.., 5] => "Ends with 5",
    [1, .., 5] => "Starts with 1 and ends with 5",
    [var first, .., var last] => $"First: {first}, Last: {last}",
    _ => "Other"
};

// Span 패턴 매칭 개선
ReadOnlySpan<char> text = "Hello";
var greeting = text switch
{
    ['H', 'e', 'l', 'l', 'o'] => "Hello!",
    ['H', 'i'] => "Hi!",
    _ => "Unknown"
};
```

### ref struct와 성능 최적화

```csharp
// ref struct: 스택에만 존재, 힙 할당 없음
public ref struct SpanBuffer
{
    private Span<byte> _buffer;

    public SpanBuffer(Span<byte> buffer)
    {
        _buffer = buffer;
    }

    public void Write(byte value, int index)
    {
        _buffer[index] = value;
    }
}

// 사용
Span<byte> buffer = stackalloc byte[1024]; // 스택 할당
var spanBuffer = new SpanBuffer(buffer);
spanBuffer.Write(42, 0);

// ref struct는 다음에서 사용 불가:
// - 박싱 불가 (object로 변환 불가)
// - 인터페이스 구현 불가
// - async 메서드의 await 경계를 넘을 수 없음
// - 람다나 로컬 함수에서 캡처 불가
```

### 간결한 using 선언

```csharp
// 전통적인 using
void TraditionalUsing()
{
    using (var file = File.OpenRead("data.txt"))
    {
        // file 사용
    } // 여기서 file.Dispose() 호출
}

// C# 8+: using 선언 (더 간결)
void SimplifiedUsing()
{
    using var file = File.OpenRead("data.txt");
    // file 사용

    // 메서드 끝에서 자동으로 file.Dispose() 호출
}

// 여러 리소스
void MultipleUsing()
{
    using var file1 = File.OpenRead("data1.txt");
    using var file2 = File.OpenRead("data2.txt");
    using var file3 = File.OpenRead("data3.txt");

    // 모두 메서드 끝에서 역순으로 Dispose
}
```

---

## 1.6 실습: 프론트엔드 개발자를 위한 C# 코드 변환 연습

이 섹션에서는 실제 코드를 변환하면서 학습합니다. `examples/` 폴더의 코드를 참고하세요.

### 실습 1: React 컴포넌트 로직을 C# 클래스로 변환하기

**목표**: React의 상태 관리와 로직을 C# 클래스로 변환
- [예제 코드 보기](./examples/01-component-to-class/)

### 실습 2: TypeScript 인터페이스를 C# 인터페이스로

**목표**: TypeScript의 타입 시스템을 C#으로 변환
- [예제 코드 보기](./examples/02-interfaces/)

### 실습 3: Async 패턴 비교 실습

**목표**: Promise/async-await를 Task/async-await로 변환
- [예제 코드 보기](./examples/03-async-patterns/)

### 실습 4: Array 메서드를 LINQ로 재작성하기

**목표**: JavaScript 배열 메서드를 LINQ로 변환
- [예제 코드 보기](./examples/04-linq-exercises/)

---

## 다음 챕터 예고

Chapter 2에서는 ASP.NET Core의 기초를 다룹니다:
- 개발 환경 설정
- 첫 번째 ASP.NET Core 애플리케이션
- 프로젝트 구조 이해

---

## 추가 학습 리소스

- [Microsoft C# 문서](https://docs.microsoft.com/dotnet/csharp/)
- [C# 13의 새로운 기능](https://docs.microsoft.com/dotnet/csharp/whats-new/csharp-13)
- [LINQ 101 샘플](https://docs.microsoft.com/samples/dotnet/try-samples/101-linq-samples/)
