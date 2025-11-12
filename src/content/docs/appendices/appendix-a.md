---
title: "Appendix A - C# 문법 빠른 참조"
---

# Appendix A: C# 문법 빠른 참조

프론트엔드 개발자를 위한 C# 문법 빠른 참조 가이드입니다. JavaScript/TypeScript와의 비교를 통해 C#의 주요 문법을 빠르게 학습할 수 있습니다.

## 1. 기본 타입과 변수 선언

### 1.1 변수 선언

**C#**
```csharp
// 명시적 타입 선언
string name = "John";
int age = 30;
bool isActive = true;
double price = 99.99;

// 타입 추론 (var)
var userName = "Jane";  // string으로 추론
var count = 10;         // int로 추론

// 상수
const int MaxUsers = 100;
const string ApiUrl = "https://api.example.com";

// 읽기 전용 (런타임에 한 번만 할당)
readonly DateTime createdAt = DateTime.UtcNow;
```

**JavaScript/TypeScript 비교**
```typescript
// JavaScript
let name = "John";
const age = 30;

// TypeScript
let name: string = "John";
const age: number = 30;
const isActive: boolean = true;
```

### 1.2 null 처리

**C#**
```csharp
// Nullable 타입 (C# 8.0+)
string? nullableString = null;  // null 허용
string nonNullableString = "value";  // null 불허

int? nullableInt = null;  // value type도 nullable 가능

// Null 조건 연산자
string? name = user?.Name;  // user가 null이면 name도 null
int length = user?.Name?.Length ?? 0;  // null coalescing

// Null 병합 연산자
string displayName = user?.Name ?? "Guest";

// Null 허용 불가 선언 (C# 8.0+)
#nullable enable
string name = null;  // 컴파일 경고!

// Null 체크
if (user is not null)
{
    Console.WriteLine(user.Name);
}
```

**TypeScript 비교**
```typescript
let nullableString: string | null = null;
let nonNullableString: string = "value";

// Optional chaining
const name = user?.name;
const length = user?.name?.length ?? 0;

// Nullish coalescing
const displayName = user?.name ?? "Guest";
```

## 2. 컬렉션

### 2.1 배열

**C#**
```csharp
// 배열 선언
int[] numbers = new int[] { 1, 2, 3, 4, 5 };
string[] names = { "Alice", "Bob", "Charlie" };  // new 생략 가능

// 다차원 배열
int[,] matrix = new int[3, 3];
matrix[0, 0] = 1;

// 가변 배열 (Jagged Array)
int[][] jaggedArray = new int[3][];
jaggedArray[0] = new int[] { 1, 2 };
jaggedArray[1] = new int[] { 3, 4, 5 };

// 배열 메서드
int length = numbers.Length;
Array.Sort(numbers);
Array.Reverse(numbers);
```

**JavaScript 비교**
```javascript
const numbers = [1, 2, 3, 4, 5];
const names = ["Alice", "Bob", "Charlie"];

// 다차원 배열
const matrix = [[1, 2], [3, 4]];

const length = numbers.length;
numbers.sort();
numbers.reverse();
```

### 2.2 List<T>

**C#**
```csharp
// List 생성 (동적 크기)
var numbers = new List<int> { 1, 2, 3, 4, 5 };
var names = new List<string>();

// 요소 추가
numbers.Add(6);
numbers.AddRange(new[] { 7, 8, 9 });

// 요소 제거
numbers.Remove(3);  // 값 3 제거
numbers.RemoveAt(0);  // 인덱스 0 제거
numbers.RemoveAll(n => n > 5);  // 조건에 맞는 모든 요소 제거

// 요소 접근
int first = numbers[0];
int count = numbers.Count;

// 검색
bool contains = numbers.Contains(5);
int index = numbers.IndexOf(5);
var filtered = numbers.Where(n => n > 3).ToList();
```

**JavaScript 비교**
```javascript
const numbers = [1, 2, 3, 4, 5];

numbers.push(6);
numbers.push(...[7, 8, 9]);

numbers.splice(numbers.indexOf(3), 1);
numbers.shift();

const first = numbers[0];
const count = numbers.length;

const contains = numbers.includes(5);
const index = numbers.indexOf(5);
const filtered = numbers.filter(n => n > 3);
```

### 2.3 Dictionary<TKey, TValue>

**C#**
```csharp
// Dictionary 생성
var userAges = new Dictionary<string, int>
{
    ["Alice"] = 25,
    ["Bob"] = 30,
    ["Charlie"] = 35
};

// 요소 추가
userAges["David"] = 40;
userAges.Add("Eve", 28);

// 요소 접근
int aliceAge = userAges["Alice"];

// 안전한 접근
if (userAges.TryGetValue("Frank", out int frankAge))
{
    Console.WriteLine($"Frank is {frankAge} years old");
}
else
{
    Console.WriteLine("Frank not found");
}

// 키 존재 확인
bool hasAlice = userAges.ContainsKey("Alice");

// 순회
foreach (var kvp in userAges)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}
```

**JavaScript 비교**
```javascript
// Object
const userAges = {
    Alice: 25,
    Bob: 30,
    Charlie: 35
};

// Map
const userAgesMap = new Map([
    ["Alice", 25],
    ["Bob", 30],
    ["Charlie", 35]
]);

userAgesMap.set("David", 40);
const aliceAge = userAgesMap.get("Alice");
const hasAlice = userAgesMap.has("Alice");

for (const [key, value] of userAgesMap) {
    console.log(`${key}: ${value}`);
}
```

## 3. 제어문

### 3.1 조건문

**C#**
```csharp
// if-else
if (age >= 18)
{
    Console.WriteLine("Adult");
}
else if (age >= 13)
{
    Console.WriteLine("Teenager");
}
else
{
    Console.WriteLine("Child");
}

// 삼항 연산자
string category = age >= 18 ? "Adult" : "Minor";

// switch 문 (전통적)
switch (dayOfWeek)
{
    case DayOfWeek.Monday:
    case DayOfWeek.Tuesday:
        Console.WriteLine("Weekday");
        break;
    case DayOfWeek.Saturday:
    case DayOfWeek.Sunday:
        Console.WriteLine("Weekend");
        break;
    default:
        Console.WriteLine("Other");
        break;
}

// switch 표현식 (C# 8.0+)
string dayType = dayOfWeek switch
{
    DayOfWeek.Monday or DayOfWeek.Tuesday or DayOfWeek.Wednesday
        or DayOfWeek.Thursday or DayOfWeek.Friday => "Weekday",
    DayOfWeek.Saturday or DayOfWeek.Sunday => "Weekend",
    _ => "Unknown"
};

// 패턴 매칭 (C# 9.0+)
string GetDiscount(object customer) => customer switch
{
    PremiumCustomer { YearsActive: > 5 } => "30% discount",
    PremiumCustomer => "20% discount",
    RegularCustomer { OrderCount: > 10 } => "10% discount",
    _ => "No discount"
};
```

**JavaScript 비교**
```javascript
if (age >= 18) {
    console.log("Adult");
} else if (age >= 13) {
    console.log("Teenager");
} else {
    console.log("Child");
}

const category = age >= 18 ? "Adult" : "Minor";

switch (dayOfWeek) {
    case "Monday":
    case "Tuesday":
        console.log("Weekday");
        break;
    case "Saturday":
    case "Sunday":
        console.log("Weekend");
        break;
    default:
        console.log("Other");
}
```

### 3.2 반복문

**C#**
```csharp
// for 루프
for (int i = 0; i < 10; i++)
{
    Console.WriteLine(i);
}

// foreach 루프
var names = new[] { "Alice", "Bob", "Charlie" };
foreach (var name in names)
{
    Console.WriteLine(name);
}

// while 루프
int count = 0;
while (count < 5)
{
    Console.WriteLine(count);
    count++;
}

// do-while 루프
do
{
    Console.WriteLine(count);
    count++;
} while (count < 10);

// break와 continue
for (int i = 0; i < 10; i++)
{
    if (i == 5) break;      // 루프 종료
    if (i % 2 == 0) continue;  // 다음 반복으로
    Console.WriteLine(i);
}
```

**JavaScript 비교**
```javascript
for (let i = 0; i < 10; i++) {
    console.log(i);
}

for (const name of names) {
    console.log(name);
}

let count = 0;
while (count < 5) {
    console.log(count);
    count++;
}

do {
    console.log(count);
    count++;
} while (count < 10);
```

## 4. 함수와 메서드

### 4.1 메서드 선언

**C#**
```csharp
// 기본 메서드
public int Add(int a, int b)
{
    return a + b;
}

// void 메서드 (반환값 없음)
public void PrintMessage(string message)
{
    Console.WriteLine(message);
}

// 표현식 본문 메서드 (C# 6.0+)
public int Multiply(int a, int b) => a * b;

// 선택적 매개변수
public void Greet(string name, string greeting = "Hello")
{
    Console.WriteLine($"{greeting}, {name}!");
}

// 명명된 인수
Greet(name: "Alice", greeting: "Hi");
Greet(greeting: "Hey", name: "Bob");

// 매개변수 배열 (params)
public int Sum(params int[] numbers)
{
    return numbers.Sum();
}

int total = Sum(1, 2, 3, 4, 5);  // 가변 인수

// ref와 out 매개변수
public void GetValues(out int x, out int y)
{
    x = 10;
    y = 20;
}

GetValues(out int a, out int b);  // 선언과 동시에 사용
```

**TypeScript 비교**
```typescript
function add(a: number, b: number): number {
    return a + b;
}

function printMessage(message: string): void {
    console.log(message);
}

const multiply = (a: number, b: number): number => a * b;

function greet(name: string, greeting: string = "Hello"): void {
    console.log(`${greeting}, ${name}!`);
}

greet("Alice", "Hi");

function sum(...numbers: number[]): number {
    return numbers.reduce((a, b) => a + b, 0);
}
```

### 4.2 람다 표현식

**C#**
```csharp
// 람다 표현식
Func<int, int, int> add = (a, b) => a + b;
Action<string> print = message => Console.WriteLine(message);

// 여러 줄 람다
Func<int, bool> isEven = n =>
{
    var result = n % 2 == 0;
    Console.WriteLine($"{n} is even: {result}");
    return result;
};

// LINQ와 함께 사용
var numbers = new[] { 1, 2, 3, 4, 5 };
var evenNumbers = numbers.Where(n => n % 2 == 0);
var doubled = numbers.Select(n => n * 2);
var sum = numbers.Aggregate((acc, n) => acc + n);
```

**JavaScript 비교**
```javascript
const add = (a, b) => a + b;
const print = message => console.log(message);

const isEven = n => {
    const result = n % 2 === 0;
    console.log(`${n} is even: ${result}`);
    return result;
};

const evenNumbers = numbers.filter(n => n % 2 === 0);
const doubled = numbers.map(n => n * 2);
const sum = numbers.reduce((acc, n) => acc + n, 0);
```

## 5. 클래스와 객체

### 5.1 클래스 정의

**C#**
```csharp
public class Person
{
    // 필드 (private)
    private string _name;
    private int _age;

    // 프로퍼티 (자동 구현)
    public string Name { get; set; }
    public int Age { get; set; }

    // 읽기 전용 프로퍼티
    public string FullName { get; }

    // 계산된 프로퍼티
    public bool IsAdult => Age >= 18;

    // 프로퍼티 접근자 제어
    public string Email { get; private set; }

    // 생성자
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    // 메서드
    public void Introduce()
    {
        Console.WriteLine($"Hi, I'm {Name}, {Age} years old");
    }

    // 정적 멤버
    public static int TotalPersons { get; private set; }

    public static void IncrementTotal()
    {
        TotalPersons++;
    }
}

// 사용
var person = new Person("Alice", 30);
person.Introduce();
```

**TypeScript 비교**
```typescript
class Person {
    private _name: string;
    private _age: number;

    public name: string;
    public age: number;

    get isAdult(): boolean {
        return this.age >= 18;
    }

    constructor(name: string, age: number) {
        this.name = name;
        this.age = age;
    }

    introduce(): void {
        console.log(`Hi, I'm ${this.name}, ${this.age} years old`);
    }

    static totalPersons: number = 0;

    static incrementTotal(): void {
        this.totalPersons++;
    }
}

const person = new Person("Alice", 30);
person.introduce();
```

### 5.2 상속

**C#**
```csharp
public class Animal
{
    public string Name { get; set; }

    public virtual void MakeSound()
    {
        Console.WriteLine("Some sound");
    }
}

public class Dog : Animal
{
    // 메서드 오버라이드
    public override void MakeSound()
    {
        Console.WriteLine("Woof!");
    }

    // 새로운 메서드
    public void Fetch()
    {
        Console.WriteLine("Fetching...");
    }
}

// 추상 클래스
public abstract class Shape
{
    public abstract double GetArea();

    public virtual void Display()
    {
        Console.WriteLine($"Area: {GetArea()}");
    }
}

public class Circle : Shape
{
    public double Radius { get; set; }

    public override double GetArea()
    {
        return Math.PI * Radius * Radius;
    }
}
```

**TypeScript 비교**
```typescript
class Animal {
    name: string;

    makeSound(): void {
        console.log("Some sound");
    }
}

class Dog extends Animal {
    makeSound(): void {
        console.log("Woof!");
    }

    fetch(): void {
        console.log("Fetching...");
    }
}

abstract class Shape {
    abstract getArea(): number;

    display(): void {
        console.log(`Area: ${this.getArea()}`);
    }
}

class Circle extends Shape {
    radius: number;

    getArea(): number {
        return Math.PI * this.radius * this.radius;
    }
}
```

### 5.3 인터페이스

**C#**
```csharp
public interface IRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class ProductRepository : IRepository<Product>
{
    public async Task<Product> GetByIdAsync(int id)
    {
        // 구현
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        // 구현
    }

    // ... 나머지 메서드 구현
}

// 다중 인터페이스 구현
public interface ILogger
{
    void Log(string message);
}

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class NotificationService : ILogger, IEmailSender
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // 이메일 전송 구현
    }
}
```

**TypeScript 비교**
```typescript
interface IRepository<T> {
    getById(id: number): Promise<T>;
    getAll(): Promise<T[]>;
    add(entity: T): Promise<void>;
    update(entity: T): Promise<void>;
    delete(id: number): Promise<void>;
}

class ProductRepository implements IRepository<Product> {
    async getById(id: number): Promise<Product> {
        // 구현
    }

    async getAll(): Promise<Product[]> {
        // 구현
    }

    // ... 나머지 메서드 구현
}

interface ILogger {
    log(message: string): void;
}

interface IEmailSender {
    sendEmail(to: string, subject: string, body: string): Promise<void>;
}

class NotificationService implements ILogger, IEmailSender {
    log(message: string): void {
        console.log(message);
    }

    async sendEmail(to: string, subject: string, body: string): Promise<void> {
        // 이메일 전송 구현
    }
}
```

## 6. 제네릭

### 6.1 제네릭 클래스와 메서드

**C#**
```csharp
// 제네릭 클래스
public class Stack<T>
{
    private List<T> _items = new List<T>();

    public void Push(T item)
    {
        _items.Add(item);
    }

    public T Pop()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("Stack is empty");

        var item = _items[^1];  // 마지막 요소
        _items.RemoveAt(_items.Count - 1);
        return item;
    }

    public int Count => _items.Count;
}

// 사용
var intStack = new Stack<int>();
intStack.Push(1);
intStack.Push(2);
int value = intStack.Pop();

var stringStack = new Stack<string>();
stringStack.Push("Hello");

// 제네릭 메서드
public T GetFirstOrDefault<T>(IEnumerable<T> items)
{
    return items.FirstOrDefault();
}

var first = GetFirstOrDefault(new[] { 1, 2, 3 });  // int
var firstString = GetFirstOrDefault(new[] { "a", "b" });  // string

// 제네릭 제약 조건
public class Repository<T> where T : class, IEntity, new()
{
    public T Create()
    {
        return new T();  // new() 제약으로 가능
    }
}

// 여러 타입 매개변수
public class Pair<TFirst, TSecond>
{
    public TFirst First { get; set; }
    public TSecond Second { get; set; }
}

var pair = new Pair<string, int>
{
    First = "Age",
    Second = 30
};
```

**TypeScript 비교**
```typescript
class Stack<T> {
    private items: T[] = [];

    push(item: T): void {
        this.items.push(item);
    }

    pop(): T {
        if (this.items.length === 0)
            throw new Error("Stack is empty");

        return this.items.pop()!;
    }

    get count(): number {
        return this.items.length;
    }
}

const intStack = new Stack<number>();
intStack.push(1);

const stringStack = new Stack<string>();
stringStack.push("Hello");

function getFirstOrDefault<T>(items: T[]): T | undefined {
    return items[0];
}

class Pair<TFirst, TSecond> {
    first: TFirst;
    second: TSecond;
}
```

## 7. LINQ (Language Integrated Query)

### 7.1 기본 LINQ 쿼리

**C#**
```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Where (필터링)
var evenNumbers = numbers.Where(n => n % 2 == 0);
// 결과: [2, 4, 6, 8, 10]

// Select (변환)
var doubled = numbers.Select(n => n * 2);
// 결과: [2, 4, 6, 8, 10, 12, 14, 16, 18, 20]

// OrderBy, OrderByDescending (정렬)
var sorted = numbers.OrderByDescending(n => n);

// Take, Skip (페이징)
var firstThree = numbers.Take(3);  // [1, 2, 3]
var skipTwo = numbers.Skip(2);     // [3, 4, 5, ...]

// First, FirstOrDefault
var first = numbers.First();  // 1
var firstEven = numbers.FirstOrDefault(n => n % 2 == 0);  // 2
var firstBig = numbers.FirstOrDefault(n => n > 100);  // 0 (기본값)

// Any, All (조건 확인)
bool hasEven = numbers.Any(n => n % 2 == 0);  // true
bool allPositive = numbers.All(n => n > 0);   // true

// Count, Sum, Average, Min, Max
int count = numbers.Count();
int sum = numbers.Sum();
double average = numbers.Average();
int min = numbers.Min();
int max = numbers.Max();

// Distinct (중복 제거)
var uniqueNumbers = new[] { 1, 2, 2, 3, 3, 4 }.Distinct();
// 결과: [1, 2, 3, 4]
```

**JavaScript 비교**
```javascript
const numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

const evenNumbers = numbers.filter(n => n % 2 === 0);
const doubled = numbers.map(n => n * 2);
const sorted = [...numbers].sort((a, b) => b - a);

const firstThree = numbers.slice(0, 3);
const skipTwo = numbers.slice(2);

const first = numbers[0];
const firstEven = numbers.find(n => n % 2 === 0);

const hasEven = numbers.some(n => n % 2 === 0);
const allPositive = numbers.every(n => n > 0);

const count = numbers.length;
const sum = numbers.reduce((a, b) => a + b, 0);
const average = sum / count;
const min = Math.min(...numbers);
const max = Math.max(...numbers);

const uniqueNumbers = [...new Set([1, 2, 2, 3, 3, 4])];
```

### 7.2 복잡한 LINQ 쿼리

**C#**
```csharp
// 객체 컬렉션
var products = new[]
{
    new { Id = 1, Name = "Laptop", Category = "Electronics", Price = 1200 },
    new { Id = 2, Name = "Mouse", Category = "Electronics", Price = 25 },
    new { Id = 3, Name = "Desk", Category = "Furniture", Price = 300 },
    new { Id = 4, Name = "Chair", Category = "Furniture", Price = 150 }
};

// 체이닝
var result = products
    .Where(p => p.Price > 100)
    .OrderBy(p => p.Category)
    .ThenByDescending(p => p.Price)
    .Select(p => new { p.Name, p.Price });

// GroupBy
var grouped = products
    .GroupBy(p => p.Category)
    .Select(g => new
    {
        Category = g.Key,
        Count = g.Count(),
        AveragePrice = g.Average(p => p.Price)
    });

// Join
var orders = new[]
{
    new { Id = 1, ProductId = 1, Quantity = 2 },
    new { Id = 2, ProductId = 3, Quantity = 1 }
};

var orderDetails = orders.Join(
    products,
    order => order.ProductId,
    product => product.Id,
    (order, product) => new
    {
        OrderId = order.Id,
        ProductName = product.Name,
        Quantity = order.Quantity,
        Total = product.Price * order.Quantity
    });

// SelectMany (평탄화)
var categories = new[]
{
    new { Name = "Electronics", Products = new[] { "Laptop", "Mouse" } },
    new { Name = "Furniture", Products = new[] { "Desk", "Chair" } }
};

var allProducts = categories.SelectMany(c => c.Products);
// 결과: ["Laptop", "Mouse", "Desk", "Chair"]

// Aggregate (누적)
var totalPrice = products.Aggregate(0.0, (total, p) => total + p.Price);
```

**JavaScript 비교**
```javascript
const products = [
    { id: 1, name: "Laptop", category: "Electronics", price: 1200 },
    { id: 2, name: "Mouse", category: "Electronics", price: 25 },
    { id: 3, name: "Desk", category: "Furniture", price: 300 },
    { id: 4, name: "Chair", category: "Furniture", price: 150 }
];

const result = products
    .filter(p => p.price > 100)
    .sort((a, b) => {
        if (a.category !== b.category) return a.category.localeCompare(b.category);
        return b.price - a.price;
    })
    .map(p => ({ name: p.name, price: p.price }));

// GroupBy (라이브러리 없이)
const grouped = Object.values(
    products.reduce((acc, p) => {
        if (!acc[p.category]) {
            acc[p.category] = { category: p.category, items: [] };
        }
        acc[p.category].items.push(p);
        return acc;
    }, {})
).map(g => ({
    category: g.category,
    count: g.items.length,
    averagePrice: g.items.reduce((sum, p) => sum + p.price, 0) / g.items.length
}));

// SelectMany
const allProducts = categories.flatMap(c => c.products);
```

## 8. 비동기 프로그래밍

### 8.1 async/await

**C#**
```csharp
// 비동기 메서드
public async Task<string> FetchDataAsync(string url)
{
    using var client = new HttpClient();
    var response = await client.GetAsync(url);
    var content = await response.Content.ReadAsStringAsync();
    return content;
}

// 반환값이 없는 비동기 메서드
public async Task SaveDataAsync(string data)
{
    await File.WriteAllTextAsync("data.txt", data);
}

// 사용
var data = await FetchDataAsync("https://api.example.com/data");
await SaveDataAsync(data);

// 여러 비동기 작업 동시 실행
var task1 = FetchDataAsync("https://api.example.com/data1");
var task2 = FetchDataAsync("https://api.example.com/data2");
var task3 = FetchDataAsync("https://api.example.com/data3");

await Task.WhenAll(task1, task2, task3);

var results = new[]
{
    await task1,
    await task2,
    await task3
};

// 첫 번째 완료된 작업
var firstCompleted = await Task.WhenAny(task1, task2, task3);
var firstResult = await firstCompleted;

// ConfigureAwait
public async Task ProcessDataAsync()
{
    // UI 컨텍스트로 돌아가지 않음 (성능 향상)
    var data = await FetchDataAsync("url").ConfigureAwait(false);
    ProcessData(data);
}

// 취소 토큰
public async Task LongRunningTaskAsync(CancellationToken cancellationToken)
{
    for (int i = 0; i < 1000; i++)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.Delay(100, cancellationToken);
        // 작업 수행
    }
}

var cts = new CancellationTokenSource();
var task = LongRunningTaskAsync(cts.Token);

// 5초 후 취소
cts.CancelAfter(TimeSpan.FromSeconds(5));
```

**JavaScript 비교**
```javascript
async function fetchData(url) {
    const response = await fetch(url);
    const content = await response.text();
    return content;
}

async function saveData(data) {
    await fs.promises.writeFile("data.txt", data);
}

const data = await fetchData("https://api.example.com/data");
await saveData(data);

// Promise.all
const [result1, result2, result3] = await Promise.all([
    fetchData("https://api.example.com/data1"),
    fetchData("https://api.example.com/data2"),
    fetchData("https://api.example.com/data3")
]);

// Promise.race
const firstResult = await Promise.race([
    fetchData("url1"),
    fetchData("url2"),
    fetchData("url3")
]);

// AbortController
const controller = new AbortController();
const signal = controller.signal;

setTimeout(() => controller.abort(), 5000);

try {
    const response = await fetch(url, { signal });
} catch (error) {
    if (error.name === 'AbortError') {
        console.log('Fetch aborted');
    }
}
```

### 8.2 Task 패턴

**C#**
```csharp
// Task 생성 및 시작
var task = Task.Run(() =>
{
    // CPU 집약적 작업
    return ComputeResult();
});

var result = await task;

// Task.Delay (비동기 대기)
await Task.Delay(TimeSpan.FromSeconds(5));

// Task.FromResult (완료된 Task 반환)
public Task<int> GetCachedValueAsync(string key)
{
    if (_cache.TryGetValue(key, out int value))
    {
        return Task.FromResult(value);  // 이미 완료된 Task
    }

    return FetchFromDatabaseAsync(key);
}

// Task 예외 처리
try
{
    await FetchDataAsync("url");
}
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "Failed to fetch data");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error");
}

// ValueTask (성능 최적화)
public ValueTask<Product> GetProductAsync(int id)
{
    // 캐시에 있으면 할당 없이 즉시 반환
    if (_cache.TryGetValue(id, out var product))
    {
        return new ValueTask<Product>(product);
    }

    // 캐시에 없으면 비동기로 조회
    return new ValueTask<Product>(FetchFromDbAsync(id));
}
```

## 9. 예외 처리

**C#**
```csharp
// 기본 try-catch
try
{
    var result = DivideNumbers(10, 0);
}
catch (DivideByZeroException ex)
{
    Console.WriteLine($"Cannot divide by zero: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// finally
try
{
    var file = File.OpenRead("data.txt");
    // 파일 읽기
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"File not found: {ex.Message}");
}
finally
{
    // 항상 실행 (정리 작업)
    Console.WriteLine("Cleanup");
}

// using (자동 리소스 정리)
using (var file = File.OpenRead("data.txt"))
{
    // 파일 사용
}  // 자동으로 file.Dispose() 호출

// using 선언 (C# 8.0+)
using var file = File.OpenRead("data.txt");
// 메서드 끝에서 자동으로 Dispose

// 사용자 정의 예외
public class ProductNotFoundException : Exception
{
    public int ProductId { get; }

    public ProductNotFoundException(int productId)
        : base($"Product with ID {productId} not found")
    {
        ProductId = productId;
    }
}

throw new ProductNotFoundException(123);

// 예외 필터 (C# 6.0+)
try
{
    await FetchDataAsync(url);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    // 404 에러만 처리
    return null;
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
{
    // 401 에러만 처리
    throw new UnauthorizedAccessException();
}
```

**JavaScript 비교**
```javascript
try {
    const result = divideNumbers(10, 0);
} catch (error) {
    console.log(`Error: ${error.message}`);
}

try {
    const file = fs.openSync("data.txt", "r");
} catch (error) {
    console.log(`File error: ${error.message}`);
} finally {
    console.log("Cleanup");
}

// 사용자 정의 예외
class ProductNotFoundException extends Error {
    constructor(productId) {
        super(`Product with ID ${productId} not found`);
        this.productId = productId;
        this.name = "ProductNotFoundException";
    }
}

throw new ProductNotFoundException(123);
```

## 10. 레코드와 구조체 (C# 9.0+)

### 10.1 레코드

**C#**
```csharp
// 레코드 정의 (불변 객체)
public record Person(string Name, int Age);

// 사용
var person1 = new Person("Alice", 30);
var person2 = new Person("Alice", 30);

// 값 기반 동등성
bool areEqual = person1 == person2;  // true

// with 표현식 (일부 프로퍼티 변경)
var person3 = person1 with { Age = 31 };
// person3: Person("Alice", 31)

// 레코드 분해
var (name, age) = person1;

// 레코드 클래스 (전체 구문)
public record Product
{
    public int Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }

    public Product(int id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
}

// 레코드 상속
public record Employee(string Name, int Age, string Department)
    : Person(Name, Age);
```

**TypeScript 비교**
```typescript
// TypeScript에는 레코드가 없지만 readonly로 비슷하게 구현
interface Person {
    readonly name: string;
    readonly age: number;
}

const person1: Person = { name: "Alice", age: 30 };

// 일부 변경 (spread 연산자)
const person2 = { ...person1, age: 31 };
```

### 10.2 구조체

**C#**
```csharp
// 구조체 (값 타입)
public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public double DistanceFromOrigin()
    {
        return Math.Sqrt(X * X + Y * Y);
    }
}

var point1 = new Point(3, 4);
var point2 = point1;  // 값 복사
point2.X = 10;
// point1.X는 여전히 3

// readonly 구조체 (불변)
public readonly struct ImmutablePoint
{
    public int X { get; }
    public int Y { get; }

    public ImmutablePoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}

// record struct (C# 10.0+)
public record struct Point3D(int X, int Y, int Z);
```

## 11. 패턴 매칭 (C# 7.0+)

**C#**
```csharp
// is 패턴
if (obj is string str)
{
    Console.WriteLine($"String length: {str.Length}");
}

if (obj is int number && number > 0)
{
    Console.WriteLine($"Positive number: {number}");
}

// switch 패턴
string GetDiscount(Customer customer) => customer switch
{
    { IsPremium: true, YearsActive: > 5 } => "30% off",
    { IsPremium: true } => "20% off",
    { OrderCount: > 10 } => "10% off",
    { } => "No discount",
    null => "Invalid customer"
};

// 타입 패턴
string Describe(object obj) => obj switch
{
    int i => $"Integer: {i}",
    string s => $"String: {s}",
    IEnumerable<int> numbers => $"Numbers: {numbers.Count()}",
    null => "null",
    _ => "Unknown type"
};

// 위치 패턴 (튜플, 레코드)
string GetQuadrant(Point point) => point switch
{
    (0, 0) => "Origin",
    (var x, var y) when x > 0 && y > 0 => "Quadrant I",
    (var x, var y) when x < 0 && y > 0 => "Quadrant II",
    (var x, var y) when x < 0 && y < 0 => "Quadrant III",
    (var x, var y) when x > 0 && y < 0 => "Quadrant IV",
    _ => "On axis"
};

// 프로퍼티 패턴
bool IsExpensiveElectronics(Product product) => product switch
{
    { Category: "Electronics", Price: > 1000 } => true,
    _ => false
};

// 리스트 패턴 (C# 11.0+)
string DescribeList(int[] numbers) => numbers switch
{
    [] => "Empty",
    [var x] => $"Single: {x}",
    [var x, var y] => $"Pair: {x}, {y}",
    [var first, .., var last] => $"Multiple: {first}...{last}",
};
```

## 12. 델리게이트와 이벤트

### 12.1 델리게이트

**C#**
```csharp
// 델리게이트 선언
public delegate void MessageHandler(string message);
public delegate int Calculator(int a, int b);

// 델리게이트 사용
MessageHandler handler = message => Console.WriteLine(message);
handler("Hello");

Calculator add = (a, b) => a + b;
int result = add(5, 3);  // 8

// 멀티캐스트 델리게이트
MessageHandler handler1 = msg => Console.WriteLine($"Handler1: {msg}");
MessageHandler handler2 = msg => Console.WriteLine($"Handler2: {msg}");

MessageHandler combined = handler1 + handler2;
combined("Test");  // 두 핸들러 모두 실행

// Func, Action (내장 델리게이트)
Func<int, int, int> multiply = (a, b) => a * b;
Action<string> print = msg => Console.WriteLine(msg);
Predicate<int> isEven = n => n % 2 == 0;

// 콜백 패턴
public void ProcessData(string data, Action<string> onSuccess, Action<Exception> onError)
{
    try
    {
        // 데이터 처리
        onSuccess("Processing completed");
    }
    catch (Exception ex)
    {
        onError(ex);
    }
}

ProcessData("data",
    result => Console.WriteLine(result),
    error => Console.WriteLine($"Error: {error.Message}")
);
```

**JavaScript 비교**
```javascript
const handler = message => console.log(message);
handler("Hello");

const add = (a, b) => a + b;
const result = add(5, 3);

// 콜백
function processData(data, onSuccess, onError) {
    try {
        // 데이터 처리
        onSuccess("Processing completed");
    } catch (error) {
        onError(error);
    }
}

processData("data",
    result => console.log(result),
    error => console.log(`Error: ${error.message}`)
);
```

### 12.2 이벤트

**C#**
```csharp
// 이벤트 정의
public class Button
{
    // 이벤트 선언
    public event EventHandler? Clicked;
    public event EventHandler<MouseEventArgs>? MouseMove;

    public void Click()
    {
        // 이벤트 발생
        Clicked?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnMouseMove(MouseEventArgs e)
    {
        MouseMove?.Invoke(this, e);
    }
}

// 이벤트 구독
var button = new Button();

button.Clicked += (sender, e) =>
{
    Console.WriteLine("Button clicked!");
};

button.Clicked += OnButtonClicked;

void OnButtonClicked(object? sender, EventArgs e)
{
    Console.WriteLine("Another handler");
}

// 이벤트 구독 해제
button.Clicked -= OnButtonClicked;

// 사용자 정의 EventArgs
public class OrderEventArgs : EventArgs
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
}

public class OrderService
{
    public event EventHandler<OrderEventArgs>? OrderCreated;

    public void CreateOrder(Order order)
    {
        // 주문 생성

        OrderCreated?.Invoke(this, new OrderEventArgs
        {
            OrderId = order.Id,
            Amount = order.TotalAmount
        });
    }
}
```

**JavaScript 비교**
```javascript
class Button extends EventTarget {
    click() {
        this.dispatchEvent(new Event('click'));
    }
}

const button = new Button();

button.addEventListener('click', (e) => {
    console.log('Button clicked!');
});

const handler = (e) => {
    console.log('Another handler');
};

button.addEventListener('click', handler);
button.removeEventListener('click', handler);

// Node.js EventEmitter
const EventEmitter = require('events');

class OrderService extends EventEmitter {
    createOrder(order) {
        // 주문 생성

        this.emit('orderCreated', {
            orderId: order.id,
            amount: order.totalAmount
        });
    }
}

const orderService = new OrderService();
orderService.on('orderCreated', (data) => {
    console.log(`Order ${data.orderId} created`);
});
```

## 13. 특성 (Attributes)

**C#**
```csharp
// ASP.NET Core 특성
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        // ...
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        // ...
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<Product>> Create([FromBody] CreateProductDto dto)
    {
        // ...
    }
}

// 검증 특성
public class CreateProductDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }

    [Range(0.01, 10000)]
    public decimal Price { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [RegularExpression(@"^[A-Z]{2}\d{4}$")]
    public string ProductCode { get; set; }
}

// 사용자 정의 특성
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuditAttribute : Attribute
{
    public string Action { get; set; }

    public AuditAttribute(string action)
    {
        Action = action;
    }
}

[Audit("CreateProduct")]
public async Task<Product> CreateAsync(Product product)
{
    // ...
}

// Obsolete 특성
[Obsolete("Use GetProductsAsync instead")]
public List<Product> GetProducts()
{
    // 구식 메서드
}

[Obsolete("This method is deprecated", true)]  // 컴파일 에러
public void OldMethod()
{
    // ...
}
```

**TypeScript 데코레이터 비교**
```typescript
// TypeScript 데코레이터 (실험적 기능)
function Controller(route: string) {
    return function(target: any) {
        target.prototype.route = route;
    };
}

function Get(path: string) {
    return function(target: any, propertyKey: string) {
        // ...
    };
}

@Controller('/api/products')
class ProductsController {
    @Get('/')
    async getAll() {
        // ...
    }

    @Get('/:id')
    async getById(id: number) {
        // ...
    }
}
```

## 14. 확장 메서드

**C#**
```csharp
// 확장 메서드 정의 (static 클래스에 static 메서드)
public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }
}

// 사용
string text = "hello world";
string titleCase = text.ToTitleCase();  // "Hello World"
string truncated = text.Truncate(5);    // "hello"

// IEnumerable 확장
public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        return source.Where(item => item is not null)!;
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
}

// 사용
var names = new[] { "Alice", null, "Bob", null, "Charlie" };
var nonNullNames = names.WhereNotNull();  // ["Alice", "Bob", "Charlie"]

numbers.ForEach(n => Console.WriteLine(n));
```

## 15. 널 허용 참조 형식 (C# 8.0+)

**C#**
```csharp
// 프로젝트 전체 활성화 (.csproj)
// <Nullable>enable</Nullable>

// 또는 파일 단위 활성화
#nullable enable

// Nullable 참조 형식
string nonNullable = "value";  // null 불가
string? nullable = null;        // null 허용

// 컴파일러 경고
string name = null;  // 경고: null을 non-nullable 변수에 할당

// Null 조건 검사
if (name is not null)
{
    Console.WriteLine(name.Length);  // 경고 없음
}

// Null 억제 연산자 (!)
string value = nullable!;  // null이 아님을 보장

// 메서드 매개변수
public void ProcessUser(string name, string? middleName, string lastName)
{
    // name과 lastName은 null이면 안 됨
    // middleName은 null 허용

    Console.WriteLine(name.Length);        // OK
    Console.WriteLine(middleName.Length);  // 경고!
    Console.WriteLine(middleName?.Length); // OK
}

// 프로퍼티
public class User
{
    public string Name { get; set; } = string.Empty;  // null이 아닌 기본값
    public string? MiddleName { get; set; }           // null 허용

    // 생성자에서 초기화 필수
    public string LastName { get; set; }

    public User(string lastName)
    {
        LastName = lastName;  // 생성자에서 반드시 초기화
    }
}

// Nullable 특성
[return: MaybeNull]
public T GetValueOrDefault<T>(string key)
{
    // ...
}

public void SetValue([AllowNull] string value)
{
    // ...
}
```

**TypeScript 비교**
```typescript
// TypeScript strict mode
// "strict": true in tsconfig.json

let nonNullable: string = "value";
let nullable: string | null = null;

// Type guard
if (name !== null) {
    console.log(name.length);
}

// Non-null assertion
const value: string = nullable!;

// Optional parameters
function processUser(name: string, middleName: string | null, lastName: string) {
    console.log(name.length);
    console.log(middleName?.length);
}
```

## 요약

이 빠른 참조 가이드는 프론트엔드 개발자가 C#을 학습할 때 자주 참조할 수 있는 핵심 문법을 다룹니다:

1. **기본 타입과 변수**: var, const, readonly, nullable 타입
2. **컬렉션**: Array, List<T>, Dictionary<TKey, TValue>
3. **제어문**: if-else, switch 표현식, 패턴 매칭, 반복문
4. **함수**: 메서드, 람다 표현식, 확장 메서드
5. **클래스**: 프로퍼티, 생성자, 상속, 인터페이스
6. **제네릭**: 제네릭 클래스, 메서드, 제약 조건
7. **LINQ**: Where, Select, OrderBy, GroupBy, Join 등
8. **비동기**: async/await, Task, ValueTask, 취소 토큰
9. **예외 처리**: try-catch-finally, using, 사용자 정의 예외
10. **레코드와 구조체**: 불변 객체, 값 타입
11. **패턴 매칭**: is 패턴, switch 패턴, 프로퍼티 패턴
12. **델리게이트와 이벤트**: Func, Action, EventHandler
13. **특성**: Validation, Routing, 사용자 정의 특성
14. **확장 메서드**: 기존 타입에 메서드 추가
15. **Nullable 참조 형식**: null 안전성

각 섹션은 C# 코드와 JavaScript/TypeScript 코드를 비교하여 프론트엔드 개발자가 빠르게 이해할 수 있도록 구성되어 있습니다.
