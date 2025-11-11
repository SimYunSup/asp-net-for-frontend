# 실습 2: TypeScript 인터페이스를 C# 인터페이스로 변환

## 목표

TypeScript의 타입 시스템과 인터페이스를 C#으로 변환하는 방법을 학습합니다.

## 예제 1: 기본 인터페이스

### TypeScript (Before)

```typescript
// types.ts
interface Product {
  id: number;
  name: string;
  price: number;
  description?: string;  // optional
  tags: string[];
}

interface ProductRepository {
  getAll(): Promise<Product[]>;
  getById(id: number): Promise<Product | null>;
  create(product: Omit<Product, 'id'>): Promise<Product>;
  update(id: number, product: Partial<Product>): Promise<Product>;
  delete(id: number): Promise<void>;
}

// implementation
class ApiProductRepository implements ProductRepository {
  constructor(private baseUrl: string) {}

  async getAll(): Promise<Product[]> {
    const response = await fetch(`${this.baseUrl}/products`);
    return response.json();
  }

  async getById(id: number): Promise<Product | null> {
    const response = await fetch(`${this.baseUrl}/products/${id}`);
    if (!response.ok) return null;
    return response.json();
  }

  async create(product: Omit<Product, 'id'>): Promise<Product> {
    const response = await fetch(`${this.baseUrl}/products`, {
      method: 'POST',
      body: JSON.stringify(product)
    });
    return response.json();
  }

  async update(id: number, product: Partial<Product>): Promise<Product> {
    const response = await fetch(`${this.baseUrl}/products/${id}`, {
      method: 'PATCH',
      body: JSON.stringify(product)
    });
    return response.json();
  }

  async delete(id: number): Promise<void> {
    await fetch(`${this.baseUrl}/products/${id}`, {
      method: 'DELETE'
    });
  }
}
```

### C# (After)

```csharp
// Product.cs
public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }  // nullable = optional
    public List<string> Tags { get; set; } = new();
}

// CreateProductDto.cs - Omit<Product, 'id'>와 유사
public class CreateProductDto
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new();
}

// UpdateProductDto.cs - Partial<Product>와 유사
public class UpdateProductDto
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}

// IProductRepository.cs
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(CreateProductDto product);
    Task<Product> UpdateAsync(int id, UpdateProductDto product);
    Task DeleteAsync(int id);
}

// ApiProductRepository.cs
public class ApiProductRepository : IProductRepository
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ApiProductRepository(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/products");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Product>>()
            ?? new List<Product>();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/products/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task<Product> CreateAsync(CreateProductDto product)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/products", product);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>()
            ?? throw new Exception("Failed to create product");
    }

    public async Task<Product> UpdateAsync(int id, UpdateProductDto product)
    {
        var response = await _httpClient.PatchAsJsonAsync($"{_baseUrl}/products/{id}", product);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>()
            ?? throw new Exception("Failed to update product");
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/products/{id}");
        response.EnsureSuccessStatusCode();
    }
}
```

## 예제 2: 제네릭 인터페이스

### TypeScript (Before)

```typescript
interface Repository<T> {
  findAll(): Promise<T[]>;
  findById(id: number): Promise<T | null>;
  create(item: T): Promise<T>;
  update(id: number, item: T): Promise<T>;
  delete(id: number): Promise<void>;
}

interface Identifiable {
  id: number;
}

class GenericRepository<T extends Identifiable> implements Repository<T> {
  constructor(private endpoint: string) {}

  async findAll(): Promise<T[]> {
    const response = await fetch(this.endpoint);
    return response.json();
  }

  async findById(id: number): Promise<T | null> {
    const response = await fetch(`${this.endpoint}/${id}`);
    if (!response.ok) return null;
    return response.json();
  }

  async create(item: T): Promise<T> {
    const response = await fetch(this.endpoint, {
      method: 'POST',
      body: JSON.stringify(item)
    });
    return response.json();
  }

  async update(id: number, item: T): Promise<T> {
    const response = await fetch(`${this.endpoint}/${id}`, {
      method: 'PUT',
      body: JSON.stringify(item)
    });
    return response.json();
  }

  async delete(id: number): Promise<void> {
    await fetch(`${this.endpoint}/${id}`, { method: 'DELETE' });
  }
}
```

### C# (After)

```csharp
// IRepository.cs
public interface IRepository<T> where T : IIdentifiable
{
    Task<List<T>> FindAllAsync();
    Task<T?> FindByIdAsync(int id);
    Task<T> CreateAsync(T item);
    Task<T> UpdateAsync(int id, T item);
    Task DeleteAsync(int id);
}

// IIdentifiable.cs
public interface IIdentifiable
{
    int Id { get; set; }
}

// GenericRepository.cs
public class GenericRepository<T> : IRepository<T>
    where T : class, IIdentifiable
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;

    public GenericRepository(HttpClient httpClient, string endpoint)
    {
        _httpClient = httpClient;
        _endpoint = endpoint;
    }

    public async Task<List<T>> FindAllAsync()
    {
        var response = await _httpClient.GetAsync(_endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<T>>()
            ?? new List<T>();
    }

    public async Task<T?> FindByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{_endpoint}/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T> CreateAsync(T item)
    {
        var response = await _httpClient.PostAsJsonAsync(_endpoint, item);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new Exception("Failed to create item");
    }

    public async Task<T> UpdateAsync(int id, T item)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_endpoint}/{id}", item);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new Exception("Failed to update item");
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_endpoint}/{id}");
        response.EnsureSuccessStatusCode();
    }
}

// 사용 예제
public class Product : IIdentifiable
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
}

public class User : IIdentifiable
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}

// 사용
var productRepo = new GenericRepository<Product>(httpClient, "/api/products");
var products = await productRepo.FindAllAsync();

var userRepo = new GenericRepository<User>(httpClient, "/api/users");
var users = await userRepo.FindAllAsync();
```

## 예제 3: 유니온 타입과 판별된 유니온

### TypeScript (Before)

```typescript
// 유니온 타입
type Status = 'pending' | 'approved' | 'rejected';

type Result<T> =
  | { success: true; data: T }
  | { success: false; error: string };

function processResult<T>(result: Result<T>): void {
  if (result.success) {
    console.log(result.data);
  } else {
    console.error(result.error);
  }
}

// 판별된 유니온
type Shape =
  | { kind: 'circle'; radius: number }
  | { kind: 'rectangle'; width: number; height: number }
  | { kind: 'triangle'; base: number; height: number };

function calculateArea(shape: Shape): number {
  switch (shape.kind) {
    case 'circle':
      return Math.PI * shape.radius ** 2;
    case 'rectangle':
      return shape.width * shape.height;
    case 'triangle':
      return (shape.base * shape.height) / 2;
  }
}
```

### C# (After)

```csharp
// Enum으로 변환 (문자열 리터럴 유니온)
public enum Status
{
    Pending,
    Approved,
    Rejected
}

// Result 패턴 (제네릭 + 판별 프로퍼티)
public class Result<T>
{
    public bool Success { get; }
    public T? Data { get; }
    public string? Error { get; }

    private Result(bool success, T? data, string? error)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    public static Result<T> Ok(T data) => new(true, data, null);
    public static Result<T> Fail(string error) => new(false, default, error);

    // 패턴 매칭 지원
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, TResult> onFailure)
    {
        return Success ? onSuccess(Data!) : onFailure(Error!);
    }
}

// 사용
void ProcessResult<T>(Result<T> result)
{
    if (result.Success)
    {
        Console.WriteLine(result.Data);
    }
    else
    {
        Console.Error.WriteLine(result.Error);
    }

    // 또는 Match 사용
    result.Match(
        data => { Console.WriteLine(data); return true; },
        error => { Console.Error.WriteLine(error); return false; }
    );
}

// 판별된 유니온: 추상 클래스 + 파생 클래스
public abstract class Shape
{
    public abstract double CalculateArea();
}

public class Circle : Shape
{
    public double Radius { get; init; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }
}

public class Rectangle : Shape
{
    public double Width { get; init; }
    public double Height { get; init; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public override double CalculateArea()
    {
        return Width * Height;
    }
}

public class Triangle : Shape
{
    public double Base { get; init; }
    public double Height { get; init; }

    public Triangle(double @base, double height)
    {
        Base = @base;
        Height = height;
    }

    public override double CalculateArea()
    {
        return (Base * Height) / 2;
    }
}

// 패턴 매칭을 사용한 면적 계산
double CalculateArea(Shape shape)
{
    return shape switch
    {
        Circle c => Math.PI * c.Radius * c.Radius,
        Rectangle r => r.Width * r.Height,
        Triangle t => (t.Base * t.Height) / 2,
        _ => throw new ArgumentException("Unknown shape")
    };

    // 또는 다형성 사용
    // return shape.CalculateArea();
}
```

## 주요 변환 포인트

| TypeScript | C# | 비고 |
|-----------|-----|------|
| `interface` | `interface` 또는 `class` | C#은 런타임에도 존재 |
| `?` (optional) | `?` (nullable) | 유사하지만 의미 다름 |
| `Omit<T, K>` | 별도 DTO 클래스 | C#에는 직접 지원 없음 |
| `Partial<T>` | 모든 프로퍼티 nullable | C#에는 직접 지원 없음 |
| `T extends X` | `where T : X` | 제네릭 제약 조건 |
| `type Union = A \| B` | `enum` 또는 상속 | 패턴에 따라 다름 |
| 판별된 유니온 | 추상 클래스 + 파생 | 다형성 활용 |

## 연습 문제

[Exercise.cs](./Exercise.cs) 파일에서 다음을 구현하세요:

1. `ICache<TKey, TValue>` 인터페이스와 `MemoryCache` 구현
2. `Result<T, E>` 타입 (성공/실패에 대한 제네릭 오류 타입)
3. 여러 도형 타입 추가 (Pentagon, Hexagon 등)
4. `IValidator<T>` 인터페이스와 여러 validator 구현
