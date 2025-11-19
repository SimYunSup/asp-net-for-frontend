---
title: "Chapter 1 - C# 기초 문법 - TypeScript 개발자를 위한 빠른 시작"
---

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

// C# 14: 기본 람다 매개변수
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

이 섹션에서는 실제 코드를 변환하면서 학습합니다.

### 실습 1: Async 패턴 비교 실습

**목표**: Promise/async-await를 Task/async-await로 변환

### 실습 2: Array 메서드를 LINQ로 재작성하기

**목표**: JavaScript 배열 메서드를 LINQ로 변환

---

---

## Chapter 1 마무리: TypeScript에서 C#으로의 첫 걸음

축하합니다! Chapter 1을 완료했습니다. 이제 여러분은 TypeScript/JavaScript의 익숙한 개념들을 C#으로 표현할 수 있습니다. `const`와 `let`이 C#의 변수 선언으로, 화살표 함수가 람다 표현식으로, `Promise`가 `Task`로, 그리고 배열 메서드가 LINQ로 자연스럽게 매핑되는 것을 보았습니다.

하지만 이것은 시작일 뿐입니다. Chapter 1에서 다룬 내용은 두 언어의 공통점에 초점을 맞췄습니다. TypeScript 개발자로서 이미 알고 있는 것을 새로운 문법으로 표현하는 법을 배웠죠. 그러나 C#에는 TypeScript에 없는, 또는 명시적이지 않은 강력한 기능들이 많이 있습니다.

### 다음 단계: C#만의 고유한 세계로

**[Chapter 2: C# 고급 기능과 객체지향 프로그래밍](./chapter2/index.md)** 에서는 C#의 진정한 힘을 경험하게 됩니다:

**메모리의 비밀을 이해하기**: JavaScript는 메모리 관리를 추상화하여 개발자가 신경 쓸 필요가 없게 만듭니다. 하지만 C#에서는 값 타입(Value Types)과 참조 타입(Reference Types)을 구분하며, 스택과 힙 메모리를 이해하면 성능을 크게 최적화할 수 있습니다. `struct`와 `class`의 차이, boxing/unboxing의 비용을 배우면, 왜 일부 .NET 라이브러리가 그토록 빠른지 이해하게 됩니다.

**프로퍼티의 우아함**: TypeScript에서는 getter/setter를 정의하지만, C#의 프로퍼티 시스템은 훨씬 더 정교합니다. `init` 전용 프로퍼티로 불변 객체를 만들고, 계산된 프로퍼티로 로직을 캡슐화하며, auto-property로 보일러플레이트를 제거합니다. React의 `useState`처럼, C# 프로퍼티는 상태 변경을 우아하게 다룹니다.

**이벤트 주도 설계**: React의 props callback, Vue의 emit, Angular의 EventEmitter—모두 이벤트 기반 통신입니다. C#의 이벤트와 델리게이트는 이 패턴을 언어 수준에서 구현하며, 타입 안전성을 보장합니다. Observer 패턴이 언어의 일부가 된 것입니다.

**LINQ의 진정한 힘**: Chapter 1에서 LINQ의 기본을 보았지만, Chapter 2에서는 `GroupBy`, `Join`, `SelectMany` 같은 고급 연산자로 복잡한 데이터 변환을 수행합니다. 더 놀라운 것은, 동일한 LINQ 쿼리가 Entity Framework와 함께 사용되면 SQL로 변환된다는 점입니다. C# 코드를 작성하면 자동으로 데이터베이스 쿼리가 생성됩니다!

**최신 C# 기능**: C# 14는 Primary Constructors, Raw String Literals, List Patterns 같은 현대적 기능을 더욱 발전시켰습니다. 이들은 코드를 더 간결하고 표현력 있게 만들며, TypeScript의 최신 기능들과 비슷한 철학을 공유합니다.

Chapter 2를 마치면, 여러분은 단순히 "TypeScript 개념을 C#으로 번역하는" 수준을 넘어, "C#답게 생각하고 코드를 작성하는" 개발자가 됩니다. 그리고 Part 2에서는 이 모든 지식을 ASP.NET Core로 실제 웹 애플리케이션을 만드는 데 활용합니다.

준비되셨나요? [Chapter 2로 이동하세요!](./chapter2/index.md)

---

## 추가 학습 리소스

- [Microsoft C# 문서](https://docs.microsoft.com/dotnet/csharp/)
- [C# 14의 새로운 기능](https://docs.microsoft.com/dotnet/csharp/whats-new/csharp-14)
- [LINQ 101 샘플](https://docs.microsoft.com/samples/dotnet/try-samples/101-linq-samples/)
- [TypeScript vs C# 비교](https://aka.ms/typescript-to-csharp)


# 실습 3: Async 패턴 비교 - Promise vs Task

## 목표

JavaScript의 Promise/async-await와 C#의 Task/async-await의 유사점과 차이점을 이해합니다.

## 예제 1: 기본 비동기 패턴

### JavaScript/TypeScript

```typescript
// Promise 생성
function delay(ms: number): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms));
}

// async/await 사용
async function fetchUser(id: number): Promise<User> {
  await delay(1000);
  const response = await fetch(`/api/users/${id}`);
  const user = await response.json();
  return user;
}

// Promise 체이닝
fetchUser(1)
  .then(user => console.log(user))
  .catch(error => console.error(error))
  .finally(() => console.log('Done'));
```

### C#

```csharp
// Task 생성
public static Task Delay(int milliseconds)
{
    return Task.Delay(milliseconds);
}

// async/await 사용
public async Task<User> FetchUserAsync(int id)
{
    await Task.Delay(1000);
    var response = await _httpClient.GetAsync($"/api/users/{id}");
    var user = await response.Content.ReadFromJsonAsync<User>();
    return user ?? throw new Exception("User not found");
}

// Task 체이닝
try
{
    var user = await FetchUserAsync(1);
    Console.WriteLine(user);
}
catch (Exception error)
{
    Console.Error.WriteLine(error);
}
finally
{
    Console.WriteLine("Done");
}
```

## 예제 2: 병렬 실행

### JavaScript/TypeScript

```typescript
// Promise.all - 모든 Promise가 완료될 때까지 대기
async function fetchMultipleUsers(): Promise<User[]> {
  const promises = [
    fetchUser(1),
    fetchUser(2),
    fetchUser(3)
  ];

  const users = await Promise.all(promises);
  return users;
}

// Promise.race - 가장 빠른 Promise만
async function fetchFirstAvailable(): Promise<User> {
  const promises = [
    fetchUser(1),
    fetchUser(2),
    fetchUser(3)
  ];

  return Promise.race(promises);
}

// Promise.allSettled - 모든 결과 (성공/실패 포함)
async function fetchAllUsers(): Promise<PromiseSettledResult<User>[]> {
  const promises = [
    fetchUser(1),
    fetchUser(2),
    fetchUser(3)
  ];

  return Promise.allSettled(promises);
}
```

### C#

```csharp
// Task.WhenAll - Promise.all과 동일
public async Task<List<User>> FetchMultipleUsersAsync()
{
    var tasks = new[]
    {
        FetchUserAsync(1),
        FetchUserAsync(2),
        FetchUserAsync(3)
    };

    var users = await Task.WhenAll(tasks);
    return users.ToList();
}

// Task.WhenAny - Promise.race와 동일
public async Task<User> FetchFirstAvailableAsync()
{
    var tasks = new[]
    {
        FetchUserAsync(1),
        FetchUserAsync(2),
        FetchUserAsync(3)
    };

    var completedTask = await Task.WhenAny(tasks);
    return await completedTask; // 완료된 Task의 결과
}

// Promise.allSettled 패턴
public async Task<List<TaskResult<User>>> FetchAllUsersAsync()
{
    var tasks = new[]
    {
        FetchUserAsync(1),
        FetchUserAsync(2),
        FetchUserAsync(3)
    };

    await Task.WhenAll(tasks.Select(async task =>
    {
        try
        {
            await task;
        }
        catch
        {
            // 예외 무시 (이미 Task에 저장됨)
        }
    }));

    // 각 Task의 결과 수집
    var results = tasks.Select(task => task.Status switch
    {
        TaskStatus.RanToCompletion => TaskResult<User>.Success(task.Result),
        TaskStatus.Faulted => TaskResult<User>.Failure(task.Exception!.GetBaseException()),
        TaskStatus.Canceled => TaskResult<User>.Canceled(),
        _ => throw new InvalidOperationException()
    }).ToList();

    return results;
}

public class TaskResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public Exception? Error { get; init; }
    public bool IsCanceled { get; init; }

    public static TaskResult<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static TaskResult<T> Failure(Exception error) =>
        new() { IsSuccess = false, Error = error };

    public static TaskResult<T> Canceled() =>
        new() { IsCanceled = true };
}
```

## 예제 3: 취소 토큰 (JavaScript에는 없는 기능!)

### C#의 CancellationToken

```csharp
// CancellationToken을 사용한 작업 취소
public async Task<User> FetchUserWithCancellationAsync(
    int id,
    CancellationToken cancellationToken = default)
{
    // 취소 체크
    cancellationToken.ThrowIfCancellationRequested();

    await Task.Delay(1000, cancellationToken);

    var response = await _httpClient.GetAsync(
        $"/api/users/{id}",
        cancellationToken);

    var user = await response.Content.ReadFromJsonAsync<User>(
        cancellationToken);

    return user ?? throw new Exception("User not found");
}

// 사용 예제
public async Task Example()
{
    var cts = new CancellationTokenSource();

    // 5초 후 자동 취소
    cts.CancelAfter(TimeSpan.FromSeconds(5));

    try
    {
        var user = await FetchUserWithCancellationAsync(1, cts.Token);
        Console.WriteLine($"User: {user.Name}");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Operation was cancelled");
    }
}

// 수동 취소
public async Task ManualCancellation()
{
    var cts = new CancellationTokenSource();

    var task = Task.Run(async () =>
    {
        await FetchUserWithCancellationAsync(1, cts.Token);
    });

    // 다른 곳에서 취소
    await Task.Delay(2000);
    cts.Cancel(); // 작업 취소

    try
    {
        await task;
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Cancelled!");
    }
}
```

### JavaScript의 AbortController (유사한 개념)

```typescript
// AbortController - CancellationToken과 유사
async function fetchUserWithAbort(
  id: number,
  signal?: AbortSignal
): Promise<User> {
  const response = await fetch(`/api/users/${id}`, { signal });
  return response.json();
}

// 사용
const controller = new AbortController();

// 5초 후 취소
setTimeout(() => controller.abort(), 5000);

try {
  const user = await fetchUserWithAbort(1, controller.signal);
  console.log(user);
} catch (error) {
  if (error.name === 'AbortError') {
    console.log('Fetch aborted');
  }
}
```

## 예제 4: 순차 vs 병렬 실행

### JavaScript/TypeScript

```typescript
// 순차 실행 (느림)
async function sequential() {
  const user1 = await fetchUser(1);  // 1초
  const user2 = await fetchUser(2);  // 1초
  const user3 = await fetchUser(3);  // 1초
  return [user1, user2, user3];      // 총 3초
}

// 병렬 실행 (빠름)
async function parallel() {
  const [user1, user2, user3] = await Promise.all([
    fetchUser(1),
    fetchUser(2),
    fetchUser(3)
  ]);
  return [user1, user2, user3];      // 총 1초
}

// 혼합 (일부 순차, 일부 병렬)
async function mixed() {
  // 먼저 사용자 정보 가져오기
  const user = await fetchUser(1);

  // 그 다음 병렬로 추가 정보 가져오기
  const [posts, comments] = await Promise.all([
    fetchUserPosts(user.id),
    fetchUserComments(user.id)
  ]);

  return { user, posts, comments };
}
```

### C#

```csharp
// 순차 실행 (느림)
public async Task<List<User>> SequentialAsync()
{
    var user1 = await FetchUserAsync(1);  // 1초
    var user2 = await FetchUserAsync(2);  // 1초
    var user3 = await FetchUserAsync(3);  // 1초
    return new List<User> { user1, user2, user3 }; // 총 3초
}

// 병렬 실행 (빠름)
public async Task<List<User>> ParallelAsync()
{
    var tasks = new[]
    {
        FetchUserAsync(1),
        FetchUserAsync(2),
        FetchUserAsync(3)
    };

    var users = await Task.WhenAll(tasks);
    return users.ToList(); // 총 1초
}

// 혼합 (일부 순차, 일부 병렬)
public async Task<UserDetails> MixedAsync()
{
    // 먼저 사용자 정보 가져오기
    var user = await FetchUserAsync(1);

    // 그 다음 병렬로 추가 정보 가져오기
    var postsTask = FetchUserPostsAsync(user.Id);
    var commentsTask = FetchUserCommentsAsync(user.Id);

    await Task.WhenAll(postsTask, commentsTask);

    return new UserDetails
    {
        User = user,
        Posts = postsTask.Result,
        Comments = commentsTask.Result
    };
}
```

## 예제 5: ValueTask (성능 최적화)

### C#의 ValueTask

```csharp
// Task: 항상 힙 할당
public async Task<int> GetValueWithTaskAsync(int id)
{
    await Task.Delay(100);
    return id * 2;
}

// ValueTask: 동기 결과일 때 힙 할당 없음
private readonly Dictionary<int, int> _cache = new();

public async ValueTask<int> GetValueWithValueTaskAsync(int id)
{
    // 캐시에 있으면 동기 반환 (힙 할당 없음!)
    if (_cache.TryGetValue(id, out var cachedValue))
    {
        return cachedValue;
    }

    // 캐시에 없으면 비동기 작업
    await Task.Delay(100);
    var value = id * 2;
    _cache[id] = value;
    return value;
}

// 사용
var result = await GetValueWithValueTaskAsync(1); // 비동기
var result2 = await GetValueWithValueTaskAsync(1); // 동기 (캐시)
```

**언제 ValueTask를 사용할까?**
- 자주 동기적으로 완료되는 경우 (캐시 조회 등)
- 고성능이 중요한 hot path
- 일반적인 경우는 Task 사용

## 예제 6: 에러 처리

### JavaScript/TypeScript

```typescript
// try-catch
async function handleErrors() {
  try {
    const user = await fetchUser(1);
    const posts = await fetchUserPosts(user.id);
    return { user, posts };
  } catch (error) {
    if (error instanceof NetworkError) {
      console.error('Network error:', error);
    } else if (error instanceof ValidationError) {
      console.error('Validation error:', error);
    } else {
      console.error('Unknown error:', error);
    }
    throw error;
  }
}

// Result 패턴
type Result<T, E> =
  | { ok: true; value: T }
  | { ok: false; error: E };

async function fetchUserSafe(id: number): Promise<Result<User, string>> {
  try {
    const user = await fetchUser(id);
    return { ok: true, value: user };
  } catch (error) {
    return { ok: false, error: error.message };
  }
}
```

### C#

```csharp
// try-catch
public async Task<UserWithPosts> HandleErrorsAsync()
{
    try
    {
        var user = await FetchUserAsync(1);
        var posts = await FetchUserPostsAsync(user.Id);
        return new UserWithPosts { User = user, Posts = posts };
    }
    catch (HttpRequestException ex)
    {
        Console.Error.WriteLine($"Network error: {ex.Message}");
        throw;
    }
    catch (ValidationException ex)
    {
        Console.Error.WriteLine($"Validation error: {ex.Message}");
        throw;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Unknown error: {ex.Message}");
        throw;
    }
}

// Result 패턴
public async Task<Result<User, string>> FetchUserSafeAsync(int id)
{
    try
    {
        var user = await FetchUserAsync(id);
        return Result<User, string>.Ok(user);
    }
    catch (Exception ex)
    {
        return Result<User, string>.Fail(ex.Message);
    }
}

public class Result<T, E>
{
    public bool IsOk { get; }
    public T? Value { get; }
    public E? Error { get; }

    private Result(bool isOk, T? value, E? error)
    {
        IsOk = isOk;
        Value = value;
        Error = error;
    }

    public static Result<T, E> Ok(T value) => new(true, value, default);
    public static Result<T, E> Fail(E error) => new(false, default, error);
}
```

## 비교표

| 기능 | JavaScript Promise | C# Task |
|-----|-------------------|---------|
| 기본 생성 | `new Promise(...)` | `Task.Run(...)`, `Task.FromResult(...)` |
| async/await | ✅ | ✅ (거의 동일) |
| 병렬 실행 | `Promise.all()` | `Task.WhenAll()` |
| 경쟁 실행 | `Promise.race()` | `Task.WhenAny()` |
| 모든 결과 | `Promise.allSettled()` | 수동 구현 필요 |
| 취소 | `AbortController` | `CancellationToken` (더 강력) |
| 성능 최적화 | ❌ | `ValueTask` |
| 동기 완료 | `Promise.resolve()` | `Task.FromResult()`, `ValueTask` |

## 연습 문제

다음을 구현해보세요:

1. 여러 API를 병렬로 호출하고 결과를 결합
2. 타임아웃이 있는 비동기 작업
3. 재시도 로직이 있는 비동기 작업
4. 비동기 스트림 (IAsyncEnumerable) 사용


# 실습 4: Array 메서드를 LINQ로 재작성하기

## 목표

JavaScript 배열 메서드를 C# LINQ로 변환하는 방법을 학습합니다.

## 기본 매핑

| JavaScript | C# LINQ | 설명 |
|-----------|---------|------|
| `.filter()` | `.Where()` | 조건에 맞는 요소 필터링 |
| `.map()` | `.Select()` | 요소 변환 |
| `.reduce()` | `.Aggregate()` | 단일 값으로 축약 |
| `.find()` | `.FirstOrDefault()` | 첫 번째 요소 찾기 |
| `.some()` | `.Any()` | 하나라도 조건 만족 |
| `.every()` | `.All()` | 모두 조건 만족 |
| `.sort()` | `.OrderBy()` / `.OrderByDescending()` | 정렬 |
| `.slice()` | `.Skip()` / `.Take()` | 범위 선택 |
| `.concat()` | `.Concat()` | 배열 결합 |
| `.flatMap()` | `.SelectMany()` | 평탄화 + 매핑 |
| `.includes()` | `.Contains()` | 요소 포함 여부 |
| `.indexOf()` | `.IndexOf()` (List) | 인덱스 찾기 |
| `.reverse()` | `.Reverse()` | 역순 |
| `.join()` | `string.Join()` | 문자열 결합 |

## 예제 1: filter와 map

### JavaScript

```javascript
const products = [
  { id: 1, name: 'Laptop', price: 1000, category: 'Electronics' },
  { id: 2, name: 'Mouse', price: 25, category: 'Electronics' },
  { id: 3, name: 'Desk', price: 300, category: 'Furniture' },
  { id: 4, name: 'Chair', price: 200, category: 'Furniture' },
  { id: 5, name: 'Monitor', price: 400, category: 'Electronics' }
];

// filter: 가격이 100 이상인 제품
const expensive = products.filter(p => p.price >= 100);

// map: 제품 이름만 추출
const names = products.map(p => p.name);

// filter + map: 비싼 전자제품의 이름
const expensiveElectronics = products
  .filter(p => p.category === 'Electronics' && p.price >= 100)
  .map(p => p.name);
```

### C#

```csharp
var products = new List<Product>
{
    new() { Id = 1, Name = "Laptop", Price = 1000, Category = "Electronics" },
    new() { Id = 2, Name = "Mouse", Price = 25, Category = "Electronics" },
    new() { Id = 3, Name = "Desk", Price = 300, Category = "Furniture" },
    new() { Id = 4, Name = "Chair", Price = 200, Category = "Furniture" },
    new() { Id = 5, Name = "Monitor", Price = 400, Category = "Electronics" }
};

// Where: 가격이 100 이상인 제품
var expensive = products.Where(p => p.Price >= 100);

// Select: 제품 이름만 추출
var names = products.Select(p => p.Name);

// Where + Select: 비싼 전자제품의 이름
var expensiveElectronics = products
    .Where(p => p.Category == "Electronics" && p.Price >= 100)
    .Select(p => p.Name);

// 쿼리 구문 (SQL과 유사)
var expensiveElectronics2 = from p in products
                             where p.Category == "Electronics" && p.Price >= 100
                             select p.Name;
```

## 예제 2: reduce

### JavaScript

```javascript
const numbers = [1, 2, 3, 4, 5];

// reduce: 합계
const sum = numbers.reduce((acc, n) => acc + n, 0);

// reduce: 최댓값
const max = numbers.reduce((acc, n) => Math.max(acc, n), -Infinity);

// reduce: 객체 생성
const products = [
  { id: 1, name: 'Laptop' },
  { id: 2, name: 'Mouse' }
];

const productMap = products.reduce((acc, p) => {
  acc[p.id] = p.name;
  return acc;
}, {});
```

### C#

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// Aggregate: 합계
var sum = numbers.Aggregate(0, (acc, n) => acc + n);

// 또는 내장 메서드 사용
var sum2 = numbers.Sum();

// Aggregate: 최댓값
var max = numbers.Aggregate((acc, n) => Math.Max(acc, n));

// 또는 내장 메서드 사용
var max2 = numbers.Max();

// Aggregate: Dictionary 생성
var products = new List<Product>
{
    new() { Id = 1, Name = "Laptop" },
    new() { Id = 2, Name = "Mouse" }
};

var productMap = products.Aggregate(
    new Dictionary<int, string>(),
    (acc, p) =>
    {
        acc[p.Id] = p.Name;
        return acc;
    }
);

// 또는 ToDictionary 사용 (더 간단)
var productMap2 = products.ToDictionary(p => p.Id, p => p.Name);
```

## 예제 3: find, some, every

### JavaScript

```javascript
const numbers = [1, 2, 3, 4, 5];

// find: 첫 번째 짝수
const firstEven = numbers.find(n => n % 2 === 0); // 2

// findIndex: 첫 번째 짝수의 인덱스
const firstEvenIndex = numbers.findIndex(n => n % 2 === 0); // 1

// some: 짝수가 하나라도 있는가?
const hasEven = numbers.some(n => n % 2 === 0); // true

// every: 모두 양수인가?
const allPositive = numbers.every(n => n > 0); // true
```

### C#

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// FirstOrDefault: 첫 번째 짝수
var firstEven = numbers.FirstOrDefault(n => n % 2 == 0); // 2

// First: 첫 번째 짝수 (없으면 예외)
var firstEven2 = numbers.First(n => n % 2 == 0); // 2

// FindIndex (List에서만 사용 가능)
var numbersList = numbers.ToList();
var firstEvenIndex = numbersList.FindIndex(n => n % 2 == 0); // 1

// 또는 Select를 사용한 인덱스 찾기
var firstEvenIndex2 = numbers
    .Select((n, index) => new { n, index })
    .FirstOrDefault(x => x.n % 2 == 0)?.index ?? -1;

// Any: 짝수가 하나라도 있는가?
var hasEven = numbers.Any(n => n % 2 == 0); // true

// All: 모두 양수인가?
var allPositive = numbers.All(n => n > 0); // true
```

## 예제 4: 정렬과 그룹화

### JavaScript

```javascript
const products = [
  { id: 1, name: 'Laptop', price: 1000, category: 'Electronics' },
  { id: 2, name: 'Mouse', price: 25, category: 'Electronics' },
  { id: 3, name: 'Desk', price: 300, category: 'Furniture' }
];

// sort: 가격순 정렬
const sortedByPrice = [...products].sort((a, b) => a.price - b.price);

// sort: 이름순 정렬
const sortedByName = [...products].sort((a, b) => a.name.localeCompare(b.name));

// 그룹화 (수동)
const grouped = products.reduce((acc, p) => {
  if (!acc[p.category]) acc[p.category] = [];
  acc[p.category].push(p);
  return acc;
}, {});
```

### C#

```csharp
var products = new List<Product>
{
    new() { Id = 1, Name = "Laptop", Price = 1000, Category = "Electronics" },
    new() { Id = 2, Name = "Mouse", Price = 25, Category = "Electronics" },
    new() { Id = 3, Name = "Desk", Price = 300, Category = "Furniture" }
};

// OrderBy: 가격순 정렬
var sortedByPrice = products.OrderBy(p => p.Price);

// OrderByDescending: 가격 내림차순
var sortedByPriceDesc = products.OrderByDescending(p => p.Price);

// OrderBy: 이름순 정렬
var sortedByName = products.OrderBy(p => p.Name);

// ThenBy: 다중 정렬 (카테고리 → 가격)
var multiSorted = products
    .OrderBy(p => p.Category)
    .ThenBy(p => p.Price);

// GroupBy: 카테고리별 그룹화
var grouped = products.GroupBy(p => p.Category);

foreach (var group in grouped)
{
    Console.WriteLine($"Category: {group.Key}");
    foreach (var product in group)
    {
        Console.WriteLine($"  - {product.Name}: ${product.Price}");
    }
}

// ToDictionary: Dictionary로 변환
var groupedDict = products
    .GroupBy(p => p.Category)
    .ToDictionary(g => g.Key, g => g.ToList());
```

## 예제 5: flatMap과 SelectMany

### JavaScript

```javascript
const orders = [
  { id: 1, items: ['A', 'B'] },
  { id: 2, items: ['C'] },
  { id: 3, items: ['D', 'E', 'F'] }
];

// flatMap: 모든 아이템을 평탄화
const allItems = orders.flatMap(order => order.items);
// ['A', 'B', 'C', 'D', 'E', 'F']

// 중첩 배열 평탄화
const nested = [[1, 2], [3, 4], [5]];
const flattened = nested.flatMap(x => x);
// [1, 2, 3, 4, 5]
```

### C#

```csharp
var orders = new List<Order>
{
    new() { Id = 1, Items = new[] { "A", "B" } },
    new() { Id = 2, Items = new[] { "C" } },
    new() { Id = 3, Items = new[] { "D", "E", "F" } }
};

// SelectMany: 모든 아이템을 평탄화
var allItems = orders.SelectMany(order => order.Items);
// ["A", "B", "C", "D", "E", "F"]

// 중첩 배열 평탄화
var nested = new List<List<int>>
{
    new() { 1, 2 },
    new() { 3, 4 },
    new() { 5 }
};
var flattened = nested.SelectMany(x => x);
// [1, 2, 3, 4, 5]

// SelectMany로 JOIN과 유사한 작업
var users = new List<User>
{
    new() { Id = 1, Name = "John", OrderIds = new[] { 1, 2 } },
    new() { Id = 2, Name = "Jane", OrderIds = new[] { 3 } }
};

// 각 사용자의 모든 주문 ID를 평탄화
var allOrderIds = users.SelectMany(u => u.OrderIds);
// [1, 2, 3]

// 사용자와 주문 ID 쌍
var userOrders = users.SelectMany(
    u => u.OrderIds,
    (user, orderId) => new { user.Name, OrderId = orderId }
);
// [{ Name: "John", OrderId: 1 }, { Name: "John", OrderId: 2 }, { Name: "Jane", OrderId: 3 }]
```

## 예제 6: 페이징과 범위 선택

### JavaScript

```javascript
const numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

// slice: 범위 선택
const slice = numbers.slice(2, 5); // [3, 4, 5]

// 페이징
const pageSize = 3;
const page = 2;
const pageItems = numbers.slice((page - 1) * pageSize, page * pageSize);
// [4, 5, 6]
```

### C#

```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Skip + Take: 범위 선택
var slice = numbers.Skip(2).Take(3); // [3, 4, 5]

// 페이징
var pageSize = 3;
var page = 2;
var pageItems = numbers
    .Skip((page - 1) * pageSize)
    .Take(pageSize);
// [4, 5, 6]

// C# 8+: Range 인덱서 (배열/리스트에서만)
var array = numbers.ToArray();
var slice2 = array[2..5]; // [3, 4, 5]
var lastThree = array[^3..]; // [8, 9, 10]
var allButLast = array[..^1]; // [1, 2, 3, 4, 5, 6, 7, 8, 9]
```

## 예제 7: 복잡한 쿼리

### JavaScript

```javascript
const students = [
  { name: 'Alice', grade: 90, subject: 'Math' },
  { name: 'Bob', grade: 75, subject: 'Math' },
  { name: 'Alice', grade: 85, subject: 'Science' },
  { name: 'Charlie', grade: 95, subject: 'Math' },
  { name: 'Bob', grade: 80, subject: 'Science' }
];

// 수학 점수가 80점 이상인 학생의 이름 (중복 제거)
const result = [...new Set(
  students
    .filter(s => s.subject === 'Math' && s.grade >= 80)
    .map(s => s.name)
)];
// ['Alice', 'Charlie']

// 학생별 평균 점수
const averages = Object.entries(
  students.reduce((acc, s) => {
    if (!acc[s.name]) acc[s.name] = { total: 0, count: 0 };
    acc[s.name].total += s.grade;
    acc[s.name].count++;
    return acc;
  }, {})
).map(([name, data]) => ({
  name,
  average: data.total / data.count
}));
```

### C#

```csharp
var students = new List<Student>
{
    new() { Name = "Alice", Grade = 90, Subject = "Math" },
    new() { Name = "Bob", Grade = 75, Subject = "Math" },
    new() { Name = "Alice", Grade = 85, Subject = "Science" },
    new() { Name = "Charlie", Grade = 95, Subject = "Math" },
    new() { Name = "Bob", Grade = 80, Subject = "Science" }
};

// 수학 점수가 80점 이상인 학생의 이름 (중복 제거)
var result = students
    .Where(s => s.Subject == "Math" && s.Grade >= 80)
    .Select(s => s.Name)
    .Distinct()
    .ToList();
// ["Alice", "Charlie"]

// 쿼리 구문
var result2 = (from s in students
               where s.Subject == "Math" && s.Grade >= 80
               select s.Name).Distinct();

// 학생별 평균 점수
var averages = students
    .GroupBy(s => s.Name)
    .Select(g => new
    {
        Name = g.Key,
        Average = g.Average(s => s.Grade)
    })
    .ToList();

// 쿼리 구문
var averages2 = from s in students
                group s by s.Name into g
                select new
                {
                    Name = g.Key,
                    Average = g.Average(s => s.Grade)
                };
```

## 예제 8: JOIN 작업

### JavaScript

```javascript
const users = [
  { id: 1, name: 'John' },
  { id: 2, name: 'Jane' }
];

const orders = [
  { userId: 1, product: 'Laptop' },
  { userId: 1, product: 'Mouse' },
  { userId: 2, product: 'Desk' }
];

// 수동 JOIN
const userOrders = users.map(user => ({
  ...user,
  orders: orders.filter(o => o.userId === user.id)
}));
```

### C#

```csharp
var users = new List<User>
{
    new() { Id = 1, Name = "John" },
    new() { Id = 2, Name = "Jane" }
};

var orders = new List<Order>
{
    new() { UserId = 1, Product = "Laptop" },
    new() { UserId = 1, Product = "Mouse" },
    new() { UserId = 2, Product = "Desk" }
};

// LINQ Join (내부 조인)
var userOrders = users.Join(
    orders,
    user => user.Id,
    order => order.UserId,
    (user, order) => new { user.Name, order.Product }
);

// 쿼리 구문
var userOrders2 = from user in users
                  join order in orders on user.Id equals order.UserId
                  select new { user.Name, order.Product };

// GroupJoin (왼쪽 외부 조인)
var userWithOrders = users.GroupJoin(
    orders,
    user => user.Id,
    order => order.UserId,
    (user, userOrders) => new
    {
        user.Name,
        Orders = userOrders.Select(o => o.Product).ToList()
    }
);

// 쿼리 구문
var userWithOrders2 = from user in users
                      join order in orders on user.Id equals order.UserId into userOrders
                      select new
                      {
                          user.Name,
                          Orders = userOrders.Select(o => o.Product).ToList()
                      };
```

## 연습 문제

다음을 구현해보세요:

1. 제품 목록에서 카테고리별 총 가격 계산
2. 학생 성적 데이터에서 과목별 최고 점수 학생 찾기
3. 주문 데이터에서 고객별 총 주문 금액 계산 (정렬 포함)
4. 복잡한 필터링 + 그룹화 + 정렬 쿼리

## 유용한 LINQ 메서드 추가

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// Count: 개수
var count = numbers.Count(); // 5
var evenCount = numbers.Count(n => n % 2 == 0); // 2

// Min, Max, Average
var min = numbers.Min(); // 1
var max = numbers.Max(); // 5
var avg = numbers.Average(); // 3

// Distinct: 중복 제거
var duplicates = new[] { 1, 2, 2, 3, 3, 3 };
var unique = duplicates.Distinct(); // [1, 2, 3]

// Union: 합집합
var set1 = new[] { 1, 2, 3 };
var set2 = new[] { 3, 4, 5 };
var union = set1.Union(set2); // [1, 2, 3, 4, 5]

// Intersect: 교집합
var intersection = set1.Intersect(set2); // [3]

// Except: 차집합
var except = set1.Except(set2); // [1, 2]

// Zip: 두 시퀀스 결합
var names = new[] { "Alice", "Bob", "Charlie" };
var ages = new[] { 25, 30, 35 };
var people = names.Zip(ages, (name, age) => new { name, age });

// Chunk: 청크로 분할 (C# 6+)
var chunks = numbers.Chunk(2); // [[1, 2], [3, 4], [5]]
```

## 메서드 체이닝 vs 쿼리 구문

```csharp
// 메서드 체이닝 (Method Syntax)
var result1 = products
    .Where(p => p.Price > 100)
    .OrderBy(p => p.Name)
    .Select(p => p.Name);

// 쿼리 구문 (Query Syntax)
var result2 = from p in products
              where p.Price > 100
              orderby p.Name
              select p.Name;

// 혼합 (복잡한 작업은 메서드 체이닝이 더 유연)
var result3 = (from p in products
               where p.Price > 100
               select p)
              .OrderBy(p => p.Name)
              .Take(10);
```

**권장사항:**
- 간단한 쿼리: 쿼리 구문 (가독성)
- 복잡한 쿼리: 메서드 체이닝 (유연성)
- 팀 컨벤션을 따르세요!


## 연습 문제: Async 패턴

```csharp
// Async 패턴 연습 문제 - 아래 TODO를 완성하세요

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Title { get; set; }
}

public class Comment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Text { get; set; }
}

public class AsyncExercises
{
    private readonly HttpClient _httpClient;

    public AsyncExercises(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // TODO 1: 여러 API를 병렬로 호출하고 결과를 결합
    // 사용자, 게시글, 댓글을 동시에 가져와서 하나의 객체로 반환
    public async Task<UserData> FetchAllUserDataAsync(int userId)
    {
        // 힌트: Task.WhenAll 사용
        throw new NotImplementedException();
    }

    // TODO 2: 타임아웃이 있는 비동기 작업
    // timeoutMs 밀리초 내에 완료되지 않으면 TimeoutException 발생
    public async Task<User> FetchUserWithTimeoutAsync(int userId, int timeoutMs)
    {
        // 힌트: Task.WhenAny + Task.Delay
        // 또는 CancellationTokenSource.CancelAfter
        throw new NotImplementedException();
    }

    // TODO 3: 재시도 로직이 있는 비동기 작업
    // maxRetries 횟수만큼 재시도, 각 재시도 사이에 delayMs 대기
    public async Task<User> FetchUserWithRetryAsync(
        int userId,
        int maxRetries = 3,
        int delayMs = 1000)
    {
        // 힌트: for 루프 + try-catch + Task.Delay
        throw new NotImplementedException();
    }

    // TODO 4: 비동기 스트림 (IAsyncEnumerable) 사용
    // 페이지별로 사용자를 하나씩 yield return
    public async IAsyncEnumerable<User> StreamUsersAsync(int pageSize = 10)
    {
        // 힌트: yield return + await
        throw new NotImplementedException();
    }

    // 헬퍼 메서드들
    private async Task<User> FetchUserAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"/api/users/{userId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User>()
            ?? throw new Exception("User not found");
    }

    private async Task<List<Post>> FetchUserPostsAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"/api/users/{userId}/posts");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Post>>()
            ?? new List<Post>();
    }

    private async Task<List<Comment>> FetchUserCommentsAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"/api/users/{userId}/comments");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Comment>>()
            ?? new List<Comment>();
    }

    private async Task<List<User>> FetchUsersPageAsync(int page, int pageSize)
    {
        var response = await _httpClient.GetAsync($"/api/users?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<User>>()
            ?? new List<User>();
    }
}

public class UserData
{
    public required User User { get; init; }
    public List<Post> Posts { get; init; } = new();
    public List<Comment> Comments { get; init; } = new();
}

// ========== 해답 (아래로 스크롤하지 마세요!) ==========

/*

// TODO 1: 병렬 호출
public async Task<UserData> FetchAllUserDataAsync(int userId)
{
    // 세 가지 작업을 동시에 시작
    var userTask = FetchUserAsync(userId);
    var postsTask = FetchUserPostsAsync(userId);
    var commentsTask = FetchUserCommentsAsync(userId);

    // 모두 완료될 때까지 대기
    await Task.WhenAll(userTask, postsTask, commentsTask);

    return new UserData
    {
        User = userTask.Result,
        Posts = postsTask.Result,
        Comments = commentsTask.Result
    };

    // 또는 더 간결하게
    // var (user, posts, comments) = await (userTask, postsTask, commentsTask);
    // return new UserData { User = user, Posts = posts, Comments = comments };
}

// TODO 2: 타임아웃
public async Task<User> FetchUserWithTimeoutAsync(int userId, int timeoutMs)
{
    using var cts = new CancellationTokenSource();
    cts.CancelAfter(timeoutMs);

    try
    {
        var response = await _httpClient.GetAsync($"/api/users/{userId}", cts.Token);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User>(cts.Token)
            ?? throw new Exception("User not found");
    }
    catch (OperationCanceledException)
    {
        throw new TimeoutException($"Request timed out after {timeoutMs}ms");
    }

    // 또는 Task.WhenAny 사용
    /*
    var fetchTask = FetchUserAsync(userId);
    var timeoutTask = Task.Delay(timeoutMs);

    var completedTask = await Task.WhenAny(fetchTask, timeoutTask);

    if (completedTask == timeoutTask)
    {
        throw new TimeoutException($"Request timed out after {timeoutMs}ms");
    }

    return await fetchTask;
    *//*
}

// TODO 3: 재시도
public async Task<User> FetchUserWithRetryAsync(
    int userId,
    int maxRetries = 3,
    int delayMs = 1000)
{
    Exception? lastException = null;

    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        try
        {
            return await FetchUserAsync(userId);
        }
        catch (Exception ex)
        {
            lastException = ex;
            if (attempt < maxRetries - 1)
            {
                await Task.Delay(delayMs);
            }
        }
    }

    throw new Exception($"Failed after {maxRetries} attempts", lastException);
}

// TODO 4: 비동기 스트림
public async IAsyncEnumerable<User> StreamUsersAsync(int pageSize = 10)
{
    int page = 1;
    bool hasMore = true;

    while (hasMore)
    {
        var users = await FetchUsersPageAsync(page, pageSize);

        if (users.Count == 0)
        {
            hasMore = false;
            yield break;
        }

        foreach (var user in users)
        {
            yield return user;
        }

        hasMore = users.Count == pageSize;
        page++;
    }
}

// 사용 예제
public async Task UseStreamAsync()
{
    await foreach (var user in StreamUsersAsync(10))
    {
        Console.WriteLine($"User: {user.Name}");
        // 각 사용자를 하나씩 처리
    }
}

*/
```


## 연습 문제: LINQ 기초

```csharp
// LINQ 연습 문제 - 아래 TODO를 완성하세요

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public required string Category { get; set; }
}

public class Student
{
    public required string Name { get; set; }
    public required string Subject { get; set; }
    public int Grade { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class LinqExercises
{
    // TODO 1: 카테고리별 총 가격 계산
    // 힌트: GroupBy + Sum
    public Dictionary<string, decimal> GetTotalPriceByCategory(List<Product> products)
    {
        throw new NotImplementedException();
    }

    // TODO 2: 과목별 최고 점수 학생 찾기
    // 힌트: GroupBy + OrderByDescending + First
    public Dictionary<string, Student> GetTopStudentBySubject(List<Student> students)
    {
        throw new NotImplementedException();
    }

    // TODO 3: 고객별 총 주문 금액 계산 및 정렬
    // 힌트: GroupBy + Sum + OrderByDescending
    public List<CustomerOrderSummary> GetCustomerOrderSummary(
        List<Customer> customers,
        List<Order> orders)
    {
        throw new NotImplementedException();
    }

    // TODO 4: 복잡한 쿼리
    // 2000원 이상 구매한 고객의 이름과 주문 수를 주문 수 내림차순으로
    public List<CustomerOrderCount> GetHighValueCustomers(
        List<Customer> customers,
        List<Order> orders,
        decimal minTotalAmount)
    {
        throw new NotImplementedException();
    }
}

public class CustomerOrderSummary
{
    public required string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public int OrderCount { get; set; }
}

public class CustomerOrderCount
{
    public required string CustomerName { get; set; }
    public int OrderCount { get; set; }
}

// ========== 해답 (아래로 스크롤하지 마세요!) ==========

/*

// TODO 1: 카테고리별 총 가격 계산
public Dictionary<string, decimal> GetTotalPriceByCategory(List<Product> products)
{
    return products
        .GroupBy(p => p.Category)
        .ToDictionary(g => g.Key, g => g.Sum(p => p.Price));

    // 또는 쿼리 구문
    // return (from p in products
    //         group p by p.Category into g
    //         select new { Category = g.Key, Total = g.Sum(p => p.Price) })
    //         .ToDictionary(x => x.Category, x => x.Total);
}

// TODO 2: 과목별 최고 점수 학생 찾기
public Dictionary<string, Student> GetTopStudentBySubject(List<Student> students)
{
    return students
        .GroupBy(s => s.Subject)
        .ToDictionary(
            g => g.Key,
            g => g.OrderByDescending(s => s.Grade).First()
        );

    // 또는
    // return students
    //     .GroupBy(s => s.Subject)
    //     .ToDictionary(g => g.Key, g => g.MaxBy(s => s.Grade)!);
}

// TODO 3: 고객별 총 주문 금액 계산 및 정렬
public List<CustomerOrderSummary> GetCustomerOrderSummary(
    List<Customer> customers,
    List<Order> orders)
{
    return customers
        .GroupJoin(
            orders,
            c => c.Id,
            o => o.CustomerId,
            (customer, customerOrders) => new CustomerOrderSummary
            {
                CustomerName = customer.Name,
                TotalAmount = customerOrders.Sum(o => o.Amount),
                OrderCount = customerOrders.Count()
            })
        .OrderByDescending(x => x.TotalAmount)
        .ToList();

    // 또는 쿼리 구문
    // return (from c in customers
    //         join o in orders on c.Id equals o.CustomerId into customerOrders
    //         select new CustomerOrderSummary
    //         {
    //             CustomerName = c.Name,
    //             TotalAmount = customerOrders.Sum(o => o.Amount),
    //             OrderCount = customerOrders.Count()
    //         })
    //         .OrderByDescending(x => x.TotalAmount)
    //         .ToList();
}

// TODO 4: 복잡한 쿼리
public List<CustomerOrderCount> GetHighValueCustomers(
    List<Customer> customers,
    List<Order> orders,
    decimal minTotalAmount)
{
    return customers
        .GroupJoin(
            orders,
            c => c.Id,
            o => o.CustomerId,
            (customer, customerOrders) => new
            {
                Customer = customer,
                Orders = customerOrders.ToList()
            })
        .Where(x => x.Orders.Sum(o => o.Amount) >= minTotalAmount)
        .Select(x => new CustomerOrderCount
        {
            CustomerName = x.Customer.Name,
            OrderCount = x.Orders.Count
        })
        .OrderByDescending(x => x.OrderCount)
        .ToList();
}

*/
```
