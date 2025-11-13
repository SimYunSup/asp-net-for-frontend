---
title: "Chapter 2 - 객체지향 프로그래밍과 고급 기능"
---

# Chapter 2: 객체지향 프로그래밍과 고급 기능

## 개요

Chapter 1에서 C#의 기본 문법과 TypeScript와의 유사점을 배웠다면, 이제 한 단계 더 깊이 들어갈 차례입니다. 이 챕터에서는 C#의 객체지향 프로그래밍 심화와 JavaScript/TypeScript에는 없거나 다르게 구현된 고급 기능들을 다룹니다.

프론트엔드 개발자로서 여러분은 객체지향 프로그래밍(OOP)의 기본 개념에 익숙할 것입니다. TypeScript의 클래스, 인터페이스, 상속 등은 모두 OOP의 핵심 요소입니다. 하지만 C#은 20년 이상 OOP를 중심으로 발전해 온 언어로, 더 깊이 있고 정교한 OOP 기능을 제공합니다.

이 챕터에서 다루는 주제들은 처음에는 다소 낯설 수 있습니다. 값 타입과 참조 타입의 차이, 이벤트 시스템, 그리고 고급 LINQ 활용은 JavaScript에는 없는 개념이거나 완전히 다른 방식으로 구현됩니다. 하지만 이러한 기능들을 이해하면, 대규모 애플리케이션을 더 효율적이고 안전하게 개발할 수 있습니다.

특히 값 타입과 참조 타입의 차이는 성능과 메모리 관리에 직접적인 영향을 미치므로, 고성능 애플리케이션을 개발할 때 필수적인 지식입니다. 이벤트 시스템은 느슨하게 결합된 아키텍처를 구축하는 데 핵심적이며, LINQ의 고급 기능은 복잡한 데이터 처리를 간결하게 표현할 수 있게 합니다.

또한 C# 13과 14의 최신 기능들을 소개합니다. 이 기능들은 언어를 더욱 간결하고 표현력 있게 만들어, 현대적인 개발 패턴을 더 자연스럽게 구현할 수 있게 합니다.

---

## 2.1 값 타입(Value Types) vs 참조 타입(Reference Types)

메모리 관리는 애플리케이션 성능의 핵심입니다. JavaScript 개발자로서 여러분은 대부분의 메모리 관리를 가비지 컬렉터에 맡기고, 원시 타입(primitives)과 객체의 차이 정도만 인식하면 됩니다. 하지만 C#은 더 세밀한 메모리 제어를 제공합니다.

C#의 타입 시스템은 값 타입(Value Types)과 참조 타입(Reference Types)으로 나뉩니다. 이 구분은 단순한 개념적 차이가 아니라, 메모리 할당 위치(스택 vs 힙), 복사 동작(값 복사 vs 참조 복사), 그리고 성능에 직접적인 영향을 미칩니다.

JavaScript에도 원시 타입(number, string, boolean 등)과 객체가 있지만, C#의 값 타입과 참조 타입은 더 명시적이고 강력합니다. C#에서는 `struct` 키워드로 커스텀 값 타입을 만들 수 있으며, 이는 고성능 시나리오에서 매우 유용합니다. 예를 들어, 게임 개발이나 실시간 데이터 처리에서 수백만 개의 객체를 다룰 때, 값 타입을 사용하면 가비지 컬렉션 부담을 크게 줄일 수 있습니다.

이 섹션을 마치면, 언제 `class`를 사용하고 언제 `struct`를 사용해야 하는지, 그리고 각 선택이 성능에 어떤 영향을 미치는지 이해하게 될 것입니다.

### JavaScript의 메모리 모델

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

### C#의 메모리 모델

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

---

## 2.2 프로퍼티(Properties): getter/setter의 진화된 형태

객체의 상태를 관리하는 방법은 프로그래밍 언어마다 다릅니다. JavaScript에서는 단순히 객체의 속성에 직접 접근하거나, getter/setter 함수를 사용합니다. TypeScript는 ES5의 getter/setter 문법을 지원하여, 속성처럼 보이지만 실제로는 메서드인 접근자를 만들 수 있게 합니다.

C#의 프로퍼티(Properties)는 이러한 개념을 언어 수준에서 일급 시민(first-class citizen)으로 승격시킨 것입니다. 프로퍼티는 필드처럼 보이지만, 실제로는 메서드로 구현되어 캡슐화의 이점을 제공합니다. 읽기 전용, 쓰기 전용, 또는 읽기-쓰기 프로퍼티를 간결하게 정의할 수 있으며, 자동 구현 프로퍼티(Auto-Implemented Properties)는 불필요한 보일러플레이트 코드를 제거합니다.

C#의 프로퍼티는 단순한 syntactic sugar가 아닙니다. `init` 접근자(C# 9.0), `required` 키워드(C# 11), 그리고 최신 `field` 키워드(C# 14)까지, 지속적으로 개선되어 불변성을 강제하고 안전한 객체 초기화를 보장합니다. 이는 React나 Vue에서 불변 상태 관리가 중요한 것처럼, 서버 사이드 애플리케이션에서도 중요한 패턴입니다.

### JavaScript/TypeScript getter/setter

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

### C# 프로퍼티

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

### C# 14: field 키워드 (새로운 기능!)

```csharp
// C# 14: field 키워드로 backing field 접근
public class User
{
    public string Name
    {
        get => field;
        set => field = value?.Trim() ?? throw new ArgumentNullException();
    }

    // 더 간결한 유효성 검사
    public int Age
    {
        get => field;
        set
        {
            if (value < 0 || value > 150)
                throw new ArgumentOutOfRangeException(nameof(Age));
            field = value;
        }
    }
}
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

---

## 2.3 이벤트(Events)와 델리게이트(Delegates)

이벤트 기반 프로그래밍은 프론트엔드 개발의 핵심입니다. 사용자의 클릭, 입력, 스크롤 등 모든 인터랙션은 이벤트로 처리됩니다. JavaScript의 이벤트 시스템은 간단하고 직관적입니다. `addEventListener`로 이벤트를 구독하고, 이벤트가 발생하면 등록된 콜백이 실행됩니다.

C#의 이벤트 시스템은 이보다 훨씬 더 정교하고 타입 안전합니다. 델리게이트(Delegates)라는 타입 안전한 함수 포인터를 기반으로 하며, 이벤트는 델리게이트를 래핑하여 캡슐화를 제공합니다. 이는 단순히 콜백 함수를 등록하는 것을 넘어, 강타입의 이벤트 시그니처를 정의하고, 누가 이벤트를 발생시킬 수 있는지 제어할 수 있게 합니다.

C#의 이벤트 패턴은 Observer 디자인 패턴의 언어 수준 구현입니다. 이벤트 발행자(publisher)와 구독자(subscriber) 간의 느슨한 결합을 제공하여, 대규모 애플리케이션에서 컴포넌트 간 통신을 깔끔하게 처리할 수 있습니다. React의 props drilling이나 Redux의 액션-리듀서 패턴과 유사하게, C#의 이벤트는 상태 변경을 전파하는 강력한 메커니즘을 제공합니다.

프론트엔드에서 RxJS나 EventEmitter를 사용해본 경험이 있다면, C#의 이벤트 시스템이 매우 익숙하게 느껴질 것입니다. 실제로 RxJS의 Observable 패턴은 C#의 이벤트와 LINQ에서 영감을 받았습니다.

### JavaScript의 이벤트

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

### C# 델리게이트

```csharp
// 델리게이트 선언 (함수 타입 정의)
public delegate void ClickHandler(object sender, EventArgs e);

// 또는 내장 델리게이트 사용
// Action<T>: 반환값 없음
// Func<T, TResult>: 반환값 있음
// EventHandler<T>: 이벤트용
```

### C# 이벤트

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

### 실전 예제: 데이터 변경 알림

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

public class DataChangedEventArgs<T> : EventArgs
{
    public T OldValue { get; }
    public T NewValue { get; }

    public DataChangedEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
```

---

## 2.4 객체지향 프로그래밍: 더 엄격한 세계

TypeScript는 JavaScript에 타입 안정성을 추가했지만, 객체지향 프로그래밍(OOP)의 많은 측면은 여전히 런타임에만 강제됩니다. 예를 들어, TypeScript의 `private`과 `protected` 키워드는 컴파일 타임에만 체크되고, JavaScript로 컴파일된 후에는 완전히 사라집니다. 이는 실제로는 보호되지 않는다는 의미입니다.

C#의 OOP는 훨씬 더 엄격하고 강력합니다. 접근 제한자는 런타임에도 강제되며, 클래스 상속 시 명시적으로 `virtual`, `override`, `sealed` 등의 키워드를 사용해야 합니다. 이는 처음에는 번거롭게 느껴질 수 있지만, 대규모 코드베이스에서 예상치 못한 동작을 방지하고 명확한 인터페이스를 제공합니다.

C#의 OOP 설계는 "명시적이고 의도적"이라는 철학을 따릅니다. 메서드를 오버라이드하려면 명시적으로 `virtual`로 표시해야 하고, 오버라이드하는 쪽에서도 `override` 키워드를 사용해야 합니다. 이는 우연한 오버라이드를 방지하고, 코드를 읽는 사람에게 명확한 의도를 전달합니다.

또한 C#은 여러 수준의 접근 제한자를 제공합니다. TypeScript의 `public`, `private`, `protected`뿐만 아니라, `internal`, `protected internal`, `private protected` 등 더 세밀한 제어가 가능합니다. 이는 특히 대규모 프로젝트나 라이브러리를 개발할 때 유용합니다.

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

### 상속과 다형성

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
    public void Eat()
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

### 인터페이스와 추상 클래스 심화

**인터페이스 vs 추상 클래스 선택 기준:**

| 기준 | 인터페이스 | 추상 클래스 |
|------|-----------|-----------|
| 다중 상속 | ✅ 가능 | ❌ 불가능 (단일 상속만) |
| 구현 포함 | C# 8.0+에서만 | ✅ 가능 |
| 필드 포함 | ❌ 불가능 | ✅ 가능 |
| 생성자 | ❌ 없음 | ✅ 있음 |
| 접근 제한자 | 모두 public | ✅ 다양하게 설정 |
| 용도 | 계약 정의 | 공통 기능 + 계약 |

**추상 클래스 활용 패턴:**
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

## 2.5 LINQ 고급 활용

Chapter 1에서 LINQ의 기본 메서드(`Where`, `Select`, `First` 등)를 배웠다면, 이제 LINQ의 진정한 힘을 발휘할 차례입니다. JavaScript 배열 메서드로는 구현하기 어렵거나 여러 단계를 거쳐야 하는 복잡한 데이터 처리를, LINQ는 단일 표현식으로 간결하게 작성할 수 있게 합니다.

`GroupBy`, `Join`, `SelectMany` 등의 고급 LINQ 메서드는 데이터베이스의 SQL 쿼리와 유사한 강력한 데이터 변환 기능을 제공합니다. 실제로 Entity Framework Core와 함께 사용하면, 이 LINQ 표현식들이 자동으로 최적화된 SQL 쿼리로 변환됩니다. 이는 백엔드 개발에서 매우 강력한 추상화 계층을 제공합니다.

프론트엔드 개발자라면 데이터를 그룹화하거나 조인하는 작업을 자주 합니다. 예를 들어, 주문 목록을 사용자별로 그룹화하거나, 사용자 정보와 주문 정보를 결합하는 등의 작업입니다. JavaScript에서는 `reduce`나 복잡한 루프를 사용해야 하지만, LINQ는 이를 선언적이고 읽기 쉬운 방식으로 표현합니다.

또한 LINQ의 지연 실행(Deferred Execution) 개념은 성능 최적화에 중요합니다. LINQ 쿼리는 실제로 데이터가 필요할 때까지 실행되지 않으며, 여러 LINQ 연산을 체이닝해도 중간 컬렉션을 생성하지 않습니다. 이는 React의 Virtual DOM이나 Vue의 반응성 시스템처럼, 불필요한 연산을 최소화합니다.

### GroupBy와 집계

```csharp
var products = new List<Product>
{
    new() { Id = 1, Name = "Laptop", Price = 1000, Category = "Electronics" },
    new() { Id = 2, Name = "Mouse", Price = 25, Category = "Electronics" },
    new() { Id = 3, Name = "Desk", Price = 300, Category = "Furniture" },
};

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

// 카테고리별 총 가격
var categoryTotals = products
    .GroupBy(p => p.Category)
    .Select(g => new
    {
        Category = g.Key,
        Total = g.Sum(p => p.Price),
        Count = g.Count(),
        Average = g.Average(p => p.Price)
    });

// 쿼리 구문
var categoryTotals2 = from p in products
                       group p by p.Category into g
                       select new
                       {
                           Category = g.Key,
                           Total = g.Sum(p => p.Price),
                           Count = g.Count()
                       };
```

### Join 작업

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
```

### SelectMany (flatMap)

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

// 사용자와 주문 ID 쌍
var users = new List<User>
{
    new() { Id = 1, Name = "John", OrderIds = new[] { 1, 2 } },
    new() { Id = 2, Name = "Jane", OrderIds = new[] { 3 } }
};

var userOrders = users.SelectMany(
    u => u.OrderIds,
    (user, orderId) => new { user.Name, OrderId = orderId }
);
```

### 복잡한 쿼리 예제

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

// 학생별 평균 점수
var averages = students
    .GroupBy(s => s.Name)
    .Select(g => new
    {
        Name = g.Key,
        Average = g.Average(s => s.Grade)
    })
    .ToList();
```

---

## 2.6 C# 13 & 14의 최신 기능 (2025 기준)

프로그래밍 언어는 끊임없이 진화합니다. JavaScript가 ES6에서 화살표 함수, Promise, 클래스를 도입하여 혁신적으로 개선되었듯이, C#도 매년 새로운 기능을 추가하며 더 간결하고 표현력 있는 언어로 발전하고 있습니다.

C# 13(2024년 11월 출시)과 C# 14(2025년 11월 출시)는 개발자 생산성을 크게 향상시키는 기능들을 추가했습니다. 이 기능들은 보일러플레이트 코드를 줄이고, 더 안전한 패턴을 권장하며, 현대적인 개발 스타일을 더 자연스럽게 표현할 수 있게 합니다.

프론트엔드 개발자라면 TypeScript가 매 버전마다 새로운 기능을 추가하는 것에 익숙할 것입니다. Optional Chaining(`?.`), Nullish Coalescing(`??`), Template Literal Types 등은 모두 최근에 추가된 기능입니다. 마찬가지로 C#도 지속적으로 개선되고 있으며, 많은 경우 TypeScript보다 먼저 새로운 기능을 도입합니다.

이 섹션에서 소개하는 기능들은 최신 C# 프로젝트에서 즉시 사용할 수 있으며, 코드를 더 간결하고 안전하게 만들어줍니다. 특히 `field` 키워드와 기본 람다 매개변수는 일상적인 코드 작성에서 큰 편의성을 제공합니다.

### C# 13 기능

**1. 기본 람다 매개변수**
```csharp
// C# 13: 람다 표현식에 기본 매개변수 지원
var greet = (string name = "Guest") => $"Hello, {name}!";

Console.WriteLine(greet());          // Hello, Guest!
Console.WriteLine(greet("John"));    // Hello, John!

// LINQ에서 활용
var increment = (int x, int step = 1) => x + step;
var numbers = new[] { 1, 2, 3, 4, 5 };
var incremented = numbers.Select(n => increment(n));     // [2, 3, 4, 5, 6]
```

**2. 향상된 패턴 매칭**
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
```

### C# 14 기능 (2025년 11월 릴리스)

**1. Extension Members (확장 멤버)**
```csharp
// C# 14: 확장 프로퍼티 및 연산자
public static class IntExtensions
{
    // 확장 프로퍼티
    public static bool IsEven(this int value) => value % 2 == 0;

    // 정적 확장 메서드
    public static int Parse(this string value) => int.Parse(value);
}

// 사용
int num = 10;
bool even = num.IsEven(); // true

string str = "123";
int parsed = str.Parse(); // 123
```

**2. First-Class Span Support**
```csharp
// C# 14: Span<T>에 대한 향상된 지원
Span<int> numbers = stackalloc int[] { 1, 2, 3, 4, 5 };

// 더 자연스러운 Span 사용
var filtered = numbers.Where(n => n > 2); // Span에서 직접 LINQ 사용 가능

// 제네릭 타입 추론 개선
void ProcessSpan<T>(Span<T> span) where T : struct
{
    // Span이 extension method receiver로 사용 가능
}
```

**3. field 키워드**
```csharp
// C# 14: 프로퍼티에서 backing field 직접 접근
public class User
{
    public string Name
    {
        get => field;
        set => field = value?.Trim() ?? throw new ArgumentNullException();
    }

    // 유효성 검사가 더 간결해짐
    public int Age
    {
        get => field;
        set
        {
            if (value < 0 || value > 150)
                throw new ArgumentOutOfRangeException();
            field = value;
        }
    }
}
```

**4. Null-Conditional Assignment**
```csharp
// C# 14: null 조건부 할당
User? user = GetUser();

// 이전 방식
if (user != null)
{
    user.Name = "John";
}

// C# 14 방식
user?.Name = "John"; // user가 null이 아닐 때만 할당
```

**5. Partial Constructors and Events**
```csharp
// C# 14: 생성자와 이벤트도 partial 가능
public partial class User
{
    partial User(string name);

    partial event EventHandler NameChanged;
}

public partial class User
{
    partial User(string name)
    {
        Name = name;
    }

    partial event EventHandler NameChanged
    {
        add { /* 구현 */ }
        remove { /* 구현 */ }
    }
}
```

**6. Unbound Generic Types in nameof**
```csharp
// C# 14: nameof에서 제네릭 타입 사용
var typeName = nameof(List<>); // "List"

// 이전에는 불가능했던 패턴
public static string GetTypeName<T>()
{
    return nameof(T); // 제네릭 타입 파라미터의 이름
}
```

**7. Compound Operator Overloading**
```csharp
// C# 14: +=, *=, /= 등의 복합 연산자 오버로딩
public class Vector
{
    public double X { get; set; }
    public double Y { get; set; }

    // += 오버로딩
    public static Vector operator +(Vector a, Vector b) =>
        new Vector { X = a.X + b.X, Y = a.Y + b.Y };

    // *= 오버로딩
    public static Vector operator *(Vector v, double scalar) =>
        new Vector { X = v.X * scalar, Y = v.Y * scalar };
}

var v1 = new Vector { X = 1, Y = 2 };
v1 += new Vector { X = 3, Y = 4 }; // 이제 가능!
v1 *= 2; // 이것도 가능!
```

---

## 2.7 실습: 고급 패턴 연습

이 섹션에서는 실제 코드를 작성하면서 고급 패턴을 학습합니다.

### 실습 1: OOP 패턴 구현

**목표**: 객체지향 디자인 패턴 적용

### 실습 2: LINQ 고급 쿼리

**목표**: 복잡한 LINQ 쿼리 작성

### 실습 3: 이벤트와 델리게이트

**목표**: 이벤트 기반 프로그래밍

---

## Part 1 정리

Part 1을 완료했습니다! 다음 내용을 학습했습니다:

**Chapter 1:**
- TypeScript와 C#의 타입 시스템 차이
- 기본 문법 (람다, async/await, 구조 분해)
- LINQ 기초
- 패턴 매칭

**Chapter 2:**
- 값 타입 vs 참조 타입
- 프로퍼티와 이벤트
- 객체지향 프로그래밍 심화
- LINQ 고급
- C# 13 & 14 최신 기능

### 다음 단계

Part 2에서는 ASP.NET Core의 기초를 다룹니다:
- ASP.NET Core 소개와 개발 환경
- 첫 번째 애플리케이션 구축
- 미들웨어와 의존성 주입
- Minimal APIs

---

## 추가 학습 리소스

- [Microsoft C# 문서](https://docs.microsoft.com/dotnet/csharp/)
- [C# 14의 새로운 기능](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)
- [LINQ 문서](https://docs.microsoft.com/dotnet/csharp/programming-guide/concepts/linq/)
- [디자인 패턴 in C#](https://refactoring.guru/design-patterns/csharp)


## 연습 문제: Component to Class

```csharp
// 연습 문제: 아래 TODO를 완성하세요

using System.Net.Http.Json;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; }
}

public class UserManager
{
    private readonly HttpClient _httpClient;
    private readonly List<User> _users;

    public event EventHandler? UsersChanged;
    public IReadOnlyList<User> Users => _users.AsReadOnly();

    public UserManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _users = new List<User>();
    }

    // TODO 1: 사용자 삭제 기능 구현
    public async Task DeleteUserAsync(int userId)
    {
        // 힌트:
        // 1. HTTP DELETE 요청 보내기: _httpClient.DeleteAsync($"/api/users/{userId}")
        // 2. 성공하면 _users 리스트에서 제거
        // 3. UsersChanged 이벤트 발생
        throw new NotImplementedException();
    }

    // TODO 2: 사용자 이름으로 검색 (대소문자 구분 없이)
    public IEnumerable<User> SearchUsers(string query)
    {
        // 힌트: LINQ의 Where + Contains 사용
        // user.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
        throw new NotImplementedException();
    }

    // TODO 3: 사용자 정렬
    public enum SortBy { Name, Email }

    public IEnumerable<User> GetSortedUsers(SortBy sortBy, bool descending = false)
    {
        // 힌트:
        // LINQ의 OrderBy / OrderByDescending 사용
        // switch 표현식으로 sortBy 처리
        throw new NotImplementedException();
    }

    // TODO 4: 페이징
    public IEnumerable<User> GetUsersByPage(int page, int pageSize)
    {
        // 힌트:
        // LINQ의 Skip과 Take 사용
        // Skip((page - 1) * pageSize).Take(pageSize)
        throw new NotImplementedException();
    }

    public int GetTotalPages(int pageSize)
    {
        // 힌트: (int)Math.Ceiling((double)_users.Count / pageSize)
        throw new NotImplementedException();
    }
}

// ========== 해답 (아래로 스크롤하지 마세요!) ==========

/*

// TODO 1: 사용자 삭제 기능 구현
public async Task DeleteUserAsync(int userId)
{
    try
    {
        var response = await _httpClient.DeleteAsync($"/api/users/{userId}");
        response.EnsureSuccessStatusCode();

        var removedCount = _users.RemoveAll(u => u.Id == userId);
        if (removedCount > 0)
        {
            UsersChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    catch (HttpRequestException ex)
    {
        throw new Exception($"Failed to delete user: {ex.Message}", ex);
    }
}

// TODO 2: 사용자 이름으로 검색
public IEnumerable<User> SearchUsers(string query)
{
    if (string.IsNullOrWhiteSpace(query))
        return _users;

    return _users.Where(u =>
        u.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
        u.Email.Contains(query, StringComparison.OrdinalIgnoreCase)
    );
}

// TODO 3: 사용자 정렬
public IEnumerable<User> GetSortedUsers(SortBy sortBy, bool descending = false)
{
    var query = sortBy switch
    {
        SortBy.Name => descending
            ? _users.OrderByDescending(u => u.Name)
            : _users.OrderBy(u => u.Name),
        SortBy.Email => descending
            ? _users.OrderByDescending(u => u.Email)
            : _users.OrderBy(u => u.Email),
        _ => _users.AsEnumerable()
    };

    return query;
}

// TODO 4: 페이징
public IEnumerable<User> GetUsersByPage(int page, int pageSize)
{
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 10;

    return _users
        .Skip((page - 1) * pageSize)
        .Take(pageSize);
}

public int GetTotalPages(int pageSize)
{
    if (pageSize < 1) pageSize = 10;
    return (int)Math.Ceiling((double)_users.Count / pageSize);
}

*/
```


## 연습 문제: Interfaces

```csharp
// 인터페이스 연습 문제 - 아래 TODO를 완성하세요

// TODO 1: ICache<TKey, TValue> 인터페이스와 MemoryCache 구현
public interface ICache<TKey, TValue> where TKey : notnull
{
    // TODO: 메서드 정의
    // - TValue? Get(TKey key)
    // - void Set(TKey key, TValue value)
    // - void Set(TKey key, TValue value, TimeSpan expiration)
    // - bool Remove(TKey key)
    // - void Clear()
    // - bool Contains(TKey key)
}

public class MemoryCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
{
    // TODO: 구현
    // 힌트: Dictionary<TKey, CacheItem<TValue>> 사용
    // CacheItem에는 Value와 ExpirationTime 포함
    throw new NotImplementedException();
}

// TODO 2: Result<T, E> 타입 (제네릭 오류 타입)
public class Result<T, E>
{
    // TODO: Ok/Error 상태, Value, Error 프로퍼티
    // TODO: static 팩토리 메서드 (Ok, Error)
    // TODO: Match 메서드
    throw new NotImplementedException();
}

// TODO 3: 여러 도형 타입 추가
public abstract class Shape
{
    public abstract double CalculateArea();
    public abstract double CalculatePerimeter();
}

// TODO: Circle, Rectangle, Triangle 구현
// TODO: Pentagon, Hexagon 추가

// TODO 4: IValidator<T> 인터페이스와 여러 validator 구현
public interface IValidator<T>
{
    bool IsValid(T value);
    string GetErrorMessage();
}

// TODO: EmailValidator, PasswordValidator, AgeValidator 구현

// ========== 해답 (아래로 스크롤하지 마세요!) ==========

/*

// TODO 1: ICache 구현
public interface ICache<TKey, TValue> where TKey : notnull
{
    TValue? Get(TKey key);
    void Set(TKey key, TValue value);
    void Set(TKey key, TValue value, TimeSpan expiration);
    bool Remove(TKey key);
    void Clear();
    bool Contains(TKey key);
}

public class CacheItem<TValue>
{
    public TValue Value { get; set; }
    public DateTime? ExpirationTime { get; set; }

    public CacheItem(TValue value, DateTime? expirationTime = null)
    {
        Value = value;
        ExpirationTime = expirationTime;
    }

    public bool IsExpired => ExpirationTime.HasValue && DateTime.UtcNow > ExpirationTime;
}

public class MemoryCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, CacheItem<TValue>> _cache = new();

    public TValue? Get(TKey key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (item.IsExpired)
            {
                _cache.Remove(key);
                return default;
            }
            return item.Value;
        }
        return default;
    }

    public void Set(TKey key, TValue value)
    {
        _cache[key] = new CacheItem<TValue>(value);
    }

    public void Set(TKey key, TValue value, TimeSpan expiration)
    {
        _cache[key] = new CacheItem<TValue>(value, DateTime.UtcNow.Add(expiration));
    }

    public bool Remove(TKey key)
    {
        return _cache.Remove(key);
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public bool Contains(TKey key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (item.IsExpired)
            {
                _cache.Remove(key);
                return false;
            }
            return true;
        }
        return false;
    }
}

// TODO 2: Result<T, E> 타입
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
    public static Result<T, E> Err(E error) => new(false, default, error);

    public TResult Match<TResult>(Func<T, TResult> onOk, Func<E, TResult> onError)
    {
        return IsOk ? onOk(Value!) : onError(Error!);
    }

    public void Match(Action<T> onOk, Action<E> onError)
    {
        if (IsOk)
            onOk(Value!);
        else
            onError(Error!);
    }
}

// TODO 3: 도형 추가
public class Circle : Shape
{
    public double Radius { get; init; }

    public Circle(double radius) => Radius = radius;

    public override double CalculateArea() => Math.PI * Radius * Radius;
    public override double CalculatePerimeter() => 2 * Math.PI * Radius;
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

    public override double CalculateArea() => Width * Height;
    public override double CalculatePerimeter() => 2 * (Width + Height);
}

public class Triangle : Shape
{
    public double A { get; init; }
    public double B { get; init; }
    public double C { get; init; }

    public Triangle(double a, double b, double c)
    {
        A = a;
        B = b;
        C = c;
    }

    public override double CalculateArea()
    {
        var s = (A + B + C) / 2;
        return Math.Sqrt(s * (s - A) * (s - B) * (s - C));
    }

    public override double CalculatePerimeter() => A + B + C;
}

public class Pentagon : Shape
{
    public double Side { get; init; }

    public Pentagon(double side) => Side = side;

    public override double CalculateArea() => (Math.Sqrt(25 + 10 * Math.Sqrt(5)) / 4) * Side * Side;
    public override double CalculatePerimeter() => 5 * Side;
}

// TODO 4: Validators
public class EmailValidator : IValidator<string>
{
    public bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Contains('@') && value.Contains('.');
    }

    public string GetErrorMessage() => "Invalid email format";
}

public class PasswordValidator : IValidator<string>
{
    private readonly int _minLength;

    public PasswordValidator(int minLength = 8)
    {
        _minLength = minLength;
    }

    public bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Length >= _minLength &&
               value.Any(char.IsUpper) &&
               value.Any(char.IsLower) &&
               value.Any(char.IsDigit);
    }

    public string GetErrorMessage() =>
        $"Password must be at least {_minLength} characters and contain uppercase, lowercase, and digit";
}

public class AgeValidator : IValidator<int>
{
    private readonly int _minAge;
    private readonly int _maxAge;

    public AgeValidator(int minAge = 0, int maxAge = 150)
    {
        _minAge = minAge;
        _maxAge = maxAge;
    }

    public bool IsValid(int value)
    {
        return value >= _minAge && value <= _maxAge;
    }

    public string GetErrorMessage() => $"Age must be between {_minAge} and {_maxAge}";
}

*/
```


## 연습 문제: LINQ 고급

```csharp
// LINQ 고급 연습 문제

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string City { get; set; }
    public DateTime JoinDate { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public decimal Price { get; set; }
}

public class LinqAdvancedExercises
{
    // TODO 1: 도시별 평균 주문 금액과 고객 수
    public List<CitySummary> GetCitySummary(
        List<Customer> customers,
        List<Order> orders)
    {
        // 힌트: GroupJoin + Select
        throw new NotImplementedException();
    }

    // TODO 2: 주문이 없는 고객 찾기 (Left Anti Join)
    public List<Customer> GetCustomersWithoutOrders(
        List<Customer> customers,
        List<Order> orders)
    {
        // 힌트: GroupJoin + Where
        throw new NotImplementedException();
    }

    // TODO 3: 각 카테고리에서 가장 비싼 제품 3개
    public List<TopProduct> GetTop3ProductsByCategory(List<Product> products)
    {
        // 힌트: GroupBy + SelectMany + OrderByDescending + Take
        throw new NotImplementedException();
    }

    // TODO 4: 전월 대비 매출 증가율
    public List<MonthlySalesGrowth> GetMonthlySalesGrowth(List<Order> orders)
    {
        // 힌트: GroupBy + OrderBy + Zip 또는 윈도우 함수 스타일
        throw new NotImplementedException();
    }

    // TODO 5: 고객 생애 가치(LTV) 계산
    public List<CustomerLTV> CalculateCustomerLTV(
        List<Customer> customers,
        List<Order> orders)
    {
        // LTV = 총 주문 금액 / 가입 기간(월)
        // 힌트: GroupJoin + Select
        throw new NotImplementedException();
    }
}

public class CitySummary
{
    public required string City { get; set; }
    public int CustomerCount { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal TotalSales { get; set; }
}

public class TopProduct
{
    public required string Category { get; set; }
    public required string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Rank { get; set; }
}

public class MonthlySalesGrowth
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Sales { get; set; }
    public decimal? GrowthRate { get; set; } // null for first month
}

public class CustomerLTV
{
    public required string CustomerName { get; set; }
    public decimal TotalSpent { get; set; }
    public int MonthsActive { get; set; }
    public decimal LTVPerMonth { get; set; }
}

// ========== 해답 (아래로 스크롤하지 마세요!) ==========

/*

// TODO 1: 도시별 평균 주문 금액과 고객 수
public List<CitySummary> GetCitySummary(
    List<Customer> customers,
    List<Order> orders)
{
    return customers
        .GroupJoin(orders,
                   c => c.Id,
                   o => o.CustomerId,
                   (customer, customerOrders) => new
                   {
                       customer.City,
                       Orders = customerOrders.ToList()
                   })
        .GroupBy(x => x.City)
        .Select(g => new CitySummary
        {
            City = g.Key,
            CustomerCount = g.Count(),
            AverageOrderValue = g.SelectMany(x => x.Orders).Any()
                ? g.SelectMany(x => x.Orders).Average(o => o.TotalAmount)
                : 0,
            TotalSales = g.SelectMany(x => x.Orders).Sum(o => o.TotalAmount)
        })
        .ToList();
}

// TODO 2: 주문이 없는 고객
public List<Customer> GetCustomersWithoutOrders(
    List<Customer> customers,
    List<Order> orders)
{
    return customers
        .GroupJoin(orders,
                   c => c.Id,
                   o => o.CustomerId,
                   (customer, customerOrders) => new
                   {
                       Customer = customer,
                       HasOrders = customerOrders.Any()
                   })
        .Where(x => !x.HasOrders)
        .Select(x => x.Customer)
        .ToList();
}

// TODO 3: 카테고리별 Top 3 제품
public List<TopProduct> GetTop3ProductsByCategory(List<Product> products)
{
    return products
        .GroupBy(p => p.Category)
        .SelectMany(g => g
            .OrderByDescending(p => p.Price)
            .Take(3)
            .Select((p, index) => new TopProduct
            {
                Category = g.Key,
                ProductName = p.Name,
                Price = p.Price,
                Rank = index + 1
            }))
        .ToList();
}

// TODO 4: 전월 대비 매출 증가율
public List<MonthlySalesGrowth> GetMonthlySalesGrowth(List<Order> orders)
{
    var monthlySales = orders
        .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
        .Select(g => new
        {
            g.Key.Year,
            g.Key.Month,
            Sales = g.Sum(o => o.TotalAmount)
        })
        .OrderBy(x => x.Year)
        .ThenBy(x => x.Month)
        .ToList();

    var result = new List<MonthlySalesGrowth>();

    for (int i = 0; i < monthlySales.Count; i++)
    {
        var current = monthlySales[i];
        decimal? growthRate = null;

        if (i > 0)
        {
            var previous = monthlySales[i - 1];
            growthRate = previous.Sales > 0
                ? ((current.Sales - previous.Sales) / previous.Sales) * 100
                : null;
        }

        result.Add(new MonthlySalesGrowth
        {
            Year = current.Year,
            Month = current.Month,
            Sales = current.Sales,
            GrowthRate = growthRate
        });
    }

    return result;
}

// TODO 5: 고객 LTV
public List<CustomerLTV> CalculateCustomerLTV(
    List<Customer> customers,
    List<Order> orders)
{
    var now = DateTime.Now;

    return customers
        .GroupJoin(orders,
                   c => c.Id,
                   o => o.CustomerId,
                   (customer, customerOrders) => new
                   {
                       Customer = customer,
                       Orders = customerOrders.ToList()
                   })
        .Select(x =>
        {
            var totalSpent = x.Orders.Sum(o => o.TotalAmount);
            var monthsActive = Math.Max(1,
                (int)((now - x.Customer.JoinDate).TotalDays / 30));

            return new CustomerLTV
            {
                CustomerName = x.Customer.Name,
                TotalSpent = totalSpent,
                MonthsActive = monthsActive,
                LTVPerMonth = totalSpent / monthsActive
            };
        })
        .OrderByDescending(x => x.LTVPerMonth)
        .ToList();
}

*/
```


## 연습 문제: Events and Delegates

```csharp
// 이벤트와 델리게이트 연습 문제

// TODO 1: 파일 업로드 진행 상태 이벤트 시스템
public class FileUploadEventArgs : EventArgs
{
    public required string FileName { get; set; }
    public long BytesUploaded { get; set; }
    public long TotalBytes { get; set; }
    public int ProgressPercentage => (int)((BytesUploaded * 100) / TotalBytes);
}

public class FileUploader
{
    // TODO: 이벤트 선언
    // - UploadStarted
    // - ProgressChanged
    // - UploadCompleted
    // - UploadFailed

    public async Task UploadFileAsync(string filePath)
    {
        // TODO: 파일 업로드 시뮬레이션 + 이벤트 발생
        throw new NotImplementedException();
    }
}

// TODO 2: 승인 프로세스 이벤트 체인
public enum ApprovalStatus
{
    Pending,
    ManagerApproved,
    DirectorApproved,
    Approved,
    Rejected
}

public class ApprovalEventArgs : EventArgs
{
    public int RequestId { get; set; }
    public ApprovalStatus Status { get; set; }
    public required string ApprovedBy { get; set; }
    public required string Comments { get; set; }
}

public class ApprovalProcess
{
    // TODO: 승인 단계별 이벤트 선언

    public void SubmitRequest(int requestId)
    {
        // TODO: 승인 프로세스 시작
        throw new NotImplementedException();
    }
}

// TODO 3: 실시간 채팅 메시지 시스템
public class ChatMessage
{
    public required string Sender { get; set; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; set; }
}

public delegate void MessageReceivedDelegate(ChatMessage message);
public delegate bool MessageFilterDelegate(ChatMessage message);

public class ChatRoom
{
    // TODO: 델리게이트 기반 메시지 시스템 구현
    // - AddMessageHandler: 메시지 핸들러 추가
    // - AddMessageFilter: 메시지 필터 추가 (욕설 필터 등)
    // - SendMessage: 메시지 전송 (필터 통과 후 핸들러 호출)

    throw new NotImplementedException();
}

// TODO 4: 주식 가격 변동 알림
public class StockPriceEventArgs : EventArgs
{
    public required string Symbol { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public decimal ChangePercentage =>
        OldPrice > 0 ? ((NewPrice - OldPrice) / OldPrice) * 100 : 0;
}

public class StockMarket
{
    // TODO: 이벤트 선언
    // - PriceChanged: 가격 변동 시
    // - SignificantChange: 5% 이상 변동 시
    // - ThresholdReached: 특정 가격 도달 시

    public void UpdatePrice(string symbol, decimal newPrice)
    {
        // TODO: 가격 업데이트 + 이벤트 발생
        throw new NotImplementedException();
    }
}

// ========== 해답 (아래로 스크롤하지 마세요!) ==========

/*

// TODO 1: 파일 업로드
public class FileUploader
{
    public event EventHandler<FileUploadEventArgs>? UploadStarted;
    public event EventHandler<FileUploadEventArgs>? ProgressChanged;
    public event EventHandler<FileUploadEventArgs>? UploadCompleted;
    public event EventHandler<string>? UploadFailed;

    public async Task UploadFileAsync(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var totalBytes = new FileInfo(filePath).Length;

        try
        {
            // 업로드 시작
            OnUploadStarted(new FileUploadEventArgs
            {
                FileName = fileName,
                BytesUploaded = 0,
                TotalBytes = totalBytes
            });

            // 진행 상태 시뮬레이션
            for (long uploaded = 0; uploaded <= totalBytes; uploaded += totalBytes / 10)
            {
                await Task.Delay(100); // 업로드 시뮬레이션

                OnProgressChanged(new FileUploadEventArgs
                {
                    FileName = fileName,
                    BytesUploaded = Math.Min(uploaded, totalBytes),
                    TotalBytes = totalBytes
                });
            }

            // 완료
            OnUploadCompleted(new FileUploadEventArgs
            {
                FileName = fileName,
                BytesUploaded = totalBytes,
                TotalBytes = totalBytes
            });
        }
        catch (Exception ex)
        {
            OnUploadFailed(ex.Message);
        }
    }

    protected virtual void OnUploadStarted(FileUploadEventArgs e) =>
        UploadStarted?.Invoke(this, e);

    protected virtual void OnProgressChanged(FileUploadEventArgs e) =>
        ProgressChanged?.Invoke(this, e);

    protected virtual void OnUploadCompleted(FileUploadEventArgs e) =>
        UploadCompleted?.Invoke(this, e);

    protected virtual void OnUploadFailed(string error) =>
        UploadFailed?.Invoke(this, error);
}

// TODO 2: 승인 프로세스
public class ApprovalProcess
{
    public event EventHandler<ApprovalEventArgs>? ApprovalRequested;
    public event EventHandler<ApprovalEventArgs>? ManagerApproval;
    public event EventHandler<ApprovalEventArgs>? DirectorApproval;
    public event EventHandler<ApprovalEventArgs>? FinalApproval;
    public event EventHandler<ApprovalEventArgs>? Rejected;

    public void SubmitRequest(int requestId)
    {
        OnApprovalRequested(new ApprovalEventArgs
        {
            RequestId = requestId,
            Status = ApprovalStatus.Pending,
            ApprovedBy = "System",
            Comments = "Request submitted"
        });

        // 시뮬레이션: 자동 승인 플로우
        Task.Run(async () =>
        {
            await Task.Delay(1000);
            ApproveByManager(requestId, "Manager1", "Approved by manager");

            await Task.Delay(1000);
            ApproveByDirector(requestId, "Director1", "Approved by director");

            await Task.Delay(1000);
            FinalApprove(requestId, "CEO", "Final approval");
        });
    }

    private void ApproveByManager(int requestId, string approver, string comments)
    {
        OnManagerApproval(new ApprovalEventArgs
        {
            RequestId = requestId,
            Status = ApprovalStatus.ManagerApproved,
            ApprovedBy = approver,
            Comments = comments
        });
    }

    private void ApproveByDirector(int requestId, string approver, string comments)
    {
        OnDirectorApproval(new ApprovalEventArgs
        {
            RequestId = requestId,
            Status = ApprovalStatus.DirectorApproved,
            ApprovedBy = approver,
            Comments = comments
        });
    }

    private void FinalApprove(int requestId, string approver, string comments)
    {
        OnFinalApproval(new ApprovalEventArgs
        {
            RequestId = requestId,
            Status = ApprovalStatus.Approved,
            ApprovedBy = approver,
            Comments = comments
        });
    }

    protected virtual void OnApprovalRequested(ApprovalEventArgs e) =>
        ApprovalRequested?.Invoke(this, e);

    protected virtual void OnManagerApproval(ApprovalEventArgs e) =>
        ManagerApproval?.Invoke(this, e);

    protected virtual void OnDirectorApproval(ApprovalEventArgs e) =>
        DirectorApproval?.Invoke(this, e);

    protected virtual void OnFinalApproval(ApprovalEventArgs e) =>
        FinalApproval?.Invoke(this, e);
}

// TODO 3: 채팅 시스템
public class ChatRoom
{
    private MessageReceivedDelegate? _messageHandlers;
    private MessageFilterDelegate? _messageFilters;

    public void AddMessageHandler(MessageReceivedDelegate handler)
    {
        _messageHandlers += handler;
    }

    public void RemoveMessageHandler(MessageReceivedDelegate handler)
    {
        _messageHandlers -= handler;
    }

    public void AddMessageFilter(MessageFilterDelegate filter)
    {
        _messageFilters += filter;
    }

    public void SendMessage(string sender, string content)
    {
        var message = new ChatMessage
        {
            Sender = sender,
            Content = content,
            Timestamp = DateTime.Now
        };

        // 필터 체크
        if (_messageFilters != null)
        {
            foreach (MessageFilterDelegate filter in _messageFilters.GetInvocationList())
            {
                if (!filter(message))
                {
                    Console.WriteLine($"Message blocked by filter: {content}");
                    return;
                }
            }
        }

        // 핸들러 호출
        _messageHandlers?.Invoke(message);
    }
}

// TODO 4: 주식 시장
public class StockMarket
{
    private readonly Dictionary<string, decimal> _prices = new();

    public event EventHandler<StockPriceEventArgs>? PriceChanged;
    public event EventHandler<StockPriceEventArgs>? SignificantChange;
    public event EventHandler<StockPriceEventArgs>? ThresholdReached;

    public void UpdatePrice(string symbol, decimal newPrice)
    {
        var oldPrice = _prices.GetValueOrDefault(symbol, newPrice);
        _prices[symbol] = newPrice;

        var eventArgs = new StockPriceEventArgs
        {
            Symbol = symbol,
            OldPrice = oldPrice,
            NewPrice = newPrice
        };

        OnPriceChanged(eventArgs);

        if (Math.Abs(eventArgs.ChangePercentage) >= 5)
        {
            OnSignificantChange(eventArgs);
        }
    }

    protected virtual void OnPriceChanged(StockPriceEventArgs e) =>
        PriceChanged?.Invoke(this, e);

    protected virtual void OnSignificantChange(StockPriceEventArgs e) =>
        SignificantChange?.Invoke(this, e);

    protected virtual void OnThresholdReached(StockPriceEventArgs e) =>
        ThresholdReached?.Invoke(this, e);
}

*/
```
