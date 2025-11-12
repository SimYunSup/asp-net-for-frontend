# Chapter 1: C# 기초 문법 - TypeScript 개발자를 위한 빠른 시작

## 개요

프론트엔드 개발자로서 JavaScript와 TypeScript를 다루며, 여러분은 이미 강력한 프로그래밍 패러다임을 경험했습니다. TypeScript의 타입 시스템, async/await 패턴, 고차 함수, 그리고 최신 ES6+ 문법은 현대적인 웹 개발의 핵심입니다. 이제 C#을 배우려고 할 때, 완전히 새로운 언어를 처음부터 배워야 한다는 두려움을 가질 수 있습니다. 하지만 놀랍게도, C#과 TypeScript는 많은 공통점을 공유합니다.

이 챕터에서는 JavaScript/TypeScript 개발자가 C#을 빠르게 시작할 수 있도록 기본 문법과 익숙한 개념들을 비교합니다. 이미 알고 있는 TypeScript 개념을 C#에 매핑하여, 학습 곡선을 크게 줄일 수 있습니다. 두 언어 모두 C 계열 문법을 사용하고, 강타입 시스템을 지원하며, 객체지향과 함수형 프로그래밍 패러다임을 모두 수용합니다.

실제로 TypeScript의 많은 기능은 C#에서 영감을 받았습니다. Anders Hejlsberg라는 동일한 설계자가 두 언어를 모두 만들었기 때문입니다. 그는 C#의 수석 설계자였으며, 이후 TypeScript를 만들었습니다. 따라서 TypeScript 개발자가 C#을 배우는 것은 새로운 언어를 배우는 것이라기보다, 이미 알고 있는 개념을 더 강력하고 성숙한 플랫폼에 적용하는 것에 가깝습니다.

이 챕터를 마치면, 여러분은 C# 코드를 읽고 이해할 수 있을 뿐만 아니라, 간단한 API나 비즈니스 로직을 직접 작성할 수 있게 됩니다. TypeScript에서 사용하던 패턴들을 C#에서 어떻게 표현하는지 알게 되고, C#만의 강력한 기능들도 발견하게 될 것입니다.

---

## 1.1 타입 시스템: TypeScript와 C#의 차이점

타입 시스템은 프로그래밍 언어의 근간입니다. JavaScript에서 TypeScript로 전환했을 때, 타입 안정성이 얼마나 큰 생산성 향상을 가져오는지 경험했을 것입니다. IDE의 자동 완성, 컴파일 타임 오류 감지, 리팩토링의 안정성 등은 대규모 애플리케이션을 개발할 때 필수적입니다.

C#의 타입 시스템은 TypeScript보다 더 오래되었고, 더 깊이 통합되어 있습니다. TypeScript가 JavaScript 위에 타입 레이어를 추가한 것이라면, C#은 처음부터 강타입 언어로 설계되었습니다. 이 근본적인 차이는 타입 시스템의 작동 방식에 중요한 영향을 미칩니다.

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

### 인터페이스 기초

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

---

## 1.2 익숙한 개념, 다른 문법

프로그래밍 언어는 표현 방식은 다르지만, 핵심 개념은 놀랍도록 유사합니다. 함수형 프로그래밍의 핵심인 일급 함수, 비동기 프로그래밍, 구조 분해 할당, 모듈 시스템 등은 현대적인 언어라면 대부분 지원하는 기능입니다. JavaScript/TypeScript 개발자로서 여러분은 이미 이러한 개념에 익숙합니다. 이제 C#에서 동일한 개념을 어떻게 표현하는지만 배우면 됩니다.

이 섹션에서는 여러분이 일상적으로 사용하는 JavaScript/TypeScript 패턴을 C#으로 변환하는 방법을 다룹니다. 화살표 함수는 람다 표현식으로, Promise는 Task로, async/await는 그대로 async/await로 사용됩니다. 문법은 조금 다르지만, 개념과 사용 패턴은 거의 동일합니다.

### 화살표 함수 vs 람다 표현식

함수형 프로그래밍의 핵심은 함수를 값처럼 다루는 것입니다. JavaScript의 화살표 함수(`=>`)는 간결한 함수 표현식을 제공하며, 특히 배열 메서드(`map`, `filter`, `reduce`)와 함께 사용할 때 강력합니다. C#의 람다 표현식도 정확히 동일한 목적을 가지고 있으며, LINQ(Language Integrated Query)와 함께 사용할 때 그 진가를 발휘합니다.

흥미롭게도, C#은 JavaScript보다 먼저 람다 표현식을 도입했습니다. C# 3.0(2007년)에서 람다가 추가되었고, ES6(2015년)에서 화살표 함수가 추가되었습니다. 따라서 JavaScript의 화살표 함수는 C#의 람다 표현식에서 영감을 받았다고 볼 수 있습니다.

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

### 구조 분해 할당 (Deconstruction)

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

**C# 구조 분해:**
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

### `var`, `let`, `const` 비교

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

---

## 1.3 LINQ 기초 - JavaScript 배열 메서드와 비교

LINQ(Language Integrated Query)는 C#의 가장 강력하고 혁신적인 기능 중 하나입니다. JavaScript 개발자라면 배열 메서드인 `map`, `filter`, `reduce`, `find` 등을 매일 사용할 것입니다. 이 메서드들은 함수형 프로그래밍의 핵심 개념을 JavaScript에 도입하여, 데이터 변환과 처리를 선언적이고 읽기 쉽게 만들었습니다.

LINQ는 이러한 개념을 언어 수준에서 통합한 것으로, 단순히 배열뿐만 아니라 데이터베이스 쿼리, XML 파싱, 그리고 모든 종류의 컬렉션에 대해 동일한 문법을 사용할 수 있게 합니다. LINQ는 2007년 C# 3.0에서 도입되었는데, 이는 ES5의 배열 메서드(2009년)보다 2년 앞선 것입니다. 실제로 JavaScript의 배열 메서드는 LINQ와 함수형 프로그래밍 언어들에서 영감을 받았습니다.

LINQ의 놀라운 점은 "통합 쿼리"라는 이름 그대로, 다양한 데이터 소스를 동일한 방식으로 쿼리할 수 있다는 것입니다. 메모리 내 컬렉션을 다루는 코드와 SQL 데이터베이스를 쿼리하는 코드가 거의 동일한 문법을 사용합니다. 이는 Entity Framework Core와 결합될 때 특히 강력한데, LINQ 표현식이 자동으로 SQL 쿼리로 변환됩니다.

프론트엔드 개발자로서 여러분은 이미 LINQ의 핵심 개념을 알고 있습니다. 이제 JavaScript 배열 메서드를 LINQ 메서드로 매핑하는 것만 배우면 됩니다.

### 기본 LINQ 메서드

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

### JavaScript 메서드 → LINQ 매핑

| JavaScript | C# LINQ | 설명 |
|-----------|---------|------|
| `.filter()` | `.Where()` | 조건에 맞는 요소 필터링 |
| `.map()` | `.Select()` | 요소 변환 |
| `.reduce()` | `.Aggregate()` | 단일 값으로 축약 |
| `.find()` | `.FirstOrDefault()` | 첫 번째 요소 찾기 |
| `.some()` | `.Any()` | 하나라도 조건 만족 |
| `.every()` | `.All()` | 모두 조건 만족 |
| `.sort()` | `.OrderBy()` | 정렬 |
| `.slice()` | `.Skip().Take()` | 범위 선택 |

### LINQ 기본 예제

```csharp
var products = new List<Product>
{
    new() { Id = 1, Name = "Laptop", Price = 1000, Category = "Electronics" },
    new() { Id = 2, Name = "Mouse", Price = 25, Category = "Electronics" },
    new() { Id = 3, Name = "Desk", Price = 300, Category = "Furniture" },
};

// Where - filter와 동일
var expensive = products.Where(p => p.Price > 100);

// Select - map과 동일
var names = products.Select(p => p.Name);

// OrderBy / OrderByDescending
var sorted = products.OrderBy(p => p.Price);

// First / FirstOrDefault
var first = products.FirstOrDefault(p => p.Price > 100);

// Any / All
bool hasExpensive = products.Any(p => p.Price > 100);
bool allExpensive = products.All(p => p.Price > 10);

// Count
int count = products.Count(p => p.Price > 100);

// Sum, Average, Min, Max
decimal total = products.Sum(p => p.Price);
decimal avg = products.Average(p => p.Price);

// Take / Skip (페이징)
var page1 = products.Skip(0).Take(10);
var page2 = products.Skip(10).Take(10);
```

---

## 1.4 패턴 매칭 기초

패턴 매칭은 현대 프로그래밍 언어의 중요한 기능으로, 복잡한 조건문을 간결하고 읽기 쉽게 표현할 수 있게 합니다. JavaScript/TypeScript에서는 switch 문이나 if-else 체인을 사용하여 조건부 로직을 작성하지만, 이는 종종 장황하고 실수하기 쉽습니다.

C#의 패턴 매칭은 훨씬 더 강력하고 표현력이 뛰어납니다. 단순히 값을 비교하는 것을 넘어, 타입 체크, 속성 검사, 범위 확인 등을 하나의 표현식으로 수행할 수 있습니다. C# 8.0부터 도입된 Switch 표현식은 패턴 매칭을 더욱 간결하게 만들었으며, C# 9과 10에서 계속 개선되었습니다.

JavaScript에는 아직 진정한 패턴 매칭이 없지만, TC39 제안(Pattern Matching Proposal)으로 논의되고 있습니다. 이는 C#과 다른 언어들의 패턴 매칭에서 영감을 받은 것입니다. 따라서 C#의 패턴 매칭을 배우는 것은 JavaScript의 미래를 미리 경험하는 것이기도 합니다.

**C# Switch 표현식:**
```csharp
// 기본 switch 표현식
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

// is 패턴
if (obj is string s)
{
    Console.WriteLine(s.Length); // s는 여기서 string 타입
}
```

---

## 1.5 실습: 기초 문법 변환 연습

이 섹션에서는 실제 코드를 변환하면서 학습합니다. `examples/` 폴더의 코드를 참고하세요.

### 실습 1: Async 패턴 비교 실습

**목표**: Promise/async-await를 Task/async-await로 변환
- [예제 코드 보기](./examples/01-async-patterns/)

### 실습 2: Array 메서드를 LINQ로 재작성하기

**목표**: JavaScript 배열 메서드를 LINQ로 변환
- [예제 코드 보기](./examples/02-linq-basics/)

---

## 다음 챕터 예고

Chapter 2에서는 C#의 고급 기능을 다룹니다:
- 객체지향 프로그래밍 심화
- 이벤트와 델리게이트
- 값 타입 vs 참조 타입
- LINQ 고급 활용

---

## 추가 학습 리소스

- [Microsoft C# 문서](https://docs.microsoft.com/dotnet/csharp/)
- [C# 13의 새로운 기능](https://docs.microsoft.com/dotnet/csharp/whats-new/csharp-13)
- [LINQ 101 샘플](https://docs.microsoft.com/samples/dotnet/try-samples/101-linq-samples/)
