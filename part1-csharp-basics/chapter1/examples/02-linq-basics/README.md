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

[Exercise.cs](./Exercise.cs) 파일에서 다음을 구현하세요:

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
