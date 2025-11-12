# Part 1: C# 기초 - TypeScript 개발자를 위한 새로운 언어

## 새로운 언어, 익숙한 패러다임

JavaScript와 TypeScript로 수년간 개발해온 여러분에게 새로운 프로그래밍 언어를 배운다는 것은 두렵게 느껴질 수 있습니다. "또 다른 문법을 처음부터 배워야 하나?", "기존 지식을 버리고 완전히 새로 시작해야 하나?"라는 걱정이 들 수 있습니다. 하지만 여기 좋은 소식이 있습니다: C#은 생각보다 훨씬 친숙합니다.

TypeScript를 만든 Anders Hejlsberg는 C#의 수석 설계자이기도 합니다. 두 언어는 같은 설계 철학을 공유하며, TypeScript의 많은 기능이 C#에서 영감을 받았습니다. 타입 시스템, 인터페이스, 제네릭, async/await, 람다 표현식—이 모든 것이 C#에서 먼저 등장했고, TypeScript가 JavaScript에 가져온 개념입니다.

### 왜 C#을 배워야 하는가?

프론트엔드 개발자로서 이미 훌륭한 언어(JavaScript/TypeScript)를 알고 있는데, 왜 C#을 배워야 할까요? 몇 가지 실용적인 이유가 있습니다:

**1. 풀스택 개발자로의 확장: 더 넓은 기회**

프론트엔드와 백엔드를 모두 다룰 수 있는 능력은 커리어에 엄청난 가치를 더합니다. Node.js만 아는 것보다, C#과 ASP.NET Core까지 다룰 수 있다면 엔터프라이즈 시장에서 훨씬 더 많은 기회를 얻습니다. 특히 대기업, 금융, 게임 산업에서 C#은 여전히 지배적인 위치를 차지하고 있습니다.

**2. 강력한 타입 시스템: TypeScript를 넘어서**

TypeScript의 타입 시스템은 강력하지만, 컴파일 타임에만 존재합니다. JavaScript로 변환되면 모든 타입 정보가 사라집니다(타입 소거, type erasure). 반면 C#의 타입 정보는 런타임에도 유지되어, 리플렉션(Reflection)을 통해 타입을 검사하고 조작할 수 있습니다. 이는 ORM, 의존성 주입, 직렬화 라이브러리 등에서 강력한 기능을 가능하게 합니다.

```typescript
// TypeScript: 타입 정보는 컴파일 후 사라짐
interface User {
  name: string;
  age: number;
}

// 런타임에 타입 정보 없음
const user: User = JSON.parse(data); // 타입 검증 없이 신뢰
```

```csharp
// C#: 타입 정보가 런타임에 유지됨
public class User {
  public string Name { get; set; }
  public int Age { get; set; }
}

// 런타임에 자동 타입 검증 및 변환
var user = JsonSerializer.Deserialize<User>(data); // 타입 불일치 시 예외 발생
```

**3. 성숙한 생태계: 검증된 도구와 패턴**

JavaScript 생태계는 혁신적이지만, 변화가 너무 빠릅니다. 작년에 배운 라이브러리가 올해는 deprecated되고, 새로운 프레임워크가 매달 등장합니다. 반면 C#과 .NET 생태계는 20년 이상의 역사를 가지며, 안정적이고 검증된 패턴을 제공합니다. Entity Framework Core, ASP.NET Core Identity, SignalR 같은 도구들은 수년간의 프로덕션 사용으로 다듬어졌습니다.

**4. 게임 개발: Unity의 공식 언어**

Unity는 세계에서 가장 인기 있는 게임 엔진 중 하나이며, C#이 유일한 스크립팅 언어입니다. 웹 개발 외에 게임 개발에 관심이 있다면, C#은 필수입니다. React로 UI를 만드는 것과 Unity로 게임을 만드는 것, 두 세계를 모두 다룰 수 있는 개발자는 드뭅니다.

**5. 성능: 타입이 만드는 실제 차이**

JavaScript는 동적 타입 언어로, JIT 컴파일러가 런타임에 최적화를 수행합니다. 하지만 정적 타입 언어인 C#은 컴파일 타임에 이미 많은 최적화가 가능하며, AOT(Ahead-Of-Time) 컴파일을 통해 네이티브 수준의 성능을 달성할 수 있습니다. 특히 CPU 집약적인 작업(이미지 처리, 데이터 분석, 머신러닝)에서 차이가 두드러집니다.

### Part 1에서 배울 내용

이 파트는 TypeScript/JavaScript 개발자가 C#을 빠르게 습득할 수 있도록 설계되었습니다. "완전히 새로운 언어"를 배우는 것이 아니라, "이미 아는 개념을 새로운 문법으로 표현하는 법"을 배우는 것입니다.

**Chapter 1**에서는 TypeScript와 C#의 핵심 개념을 직접 비교합니다. `const`와 `var`, 화살표 함수와 람다, `Promise`와 `Task`, 배열 메서드와 LINQ—이미 익숙한 개념들이 C#에서 어떻게 동작하는지 배웁니다. "아, 이건 TypeScript의 저것과 같구나!"라는 순간이 반복될 것입니다.

**Chapter 2**에서는 C#만의 고유한 특성을 깊이 탐구합니다. 값 타입과 참조 타입의 차이(스택 vs 힙 메모리), 프로퍼티 시스템, 이벤트와 델리게이트, 그리고 최신 C# 13/14의 현대적인 기능들을 배웁니다. 이 장을 마치면, 여러분은 단순히 "C#을 쓸 줄 아는" 수준을 넘어 "C#답게 코드를 작성하는" 수준에 도달합니다.

### 학습 접근법

이 책은 전통적인 프로그래밍 입문서와 다릅니다. 여러분은 이미 숙련된 개발자이므로, "변수란 무엇인가?", "반복문이란?"과 같은 기초적인 설명은 건너뜁니다. 대신:

1. **TypeScript/JavaScript 코드를 먼저 보여줍니다**: 익숙한 코드로 시작합니다.
2. **동등한 C# 코드를 제시합니다**: 같은 작업을 C#으로 어떻게 하는지 보여줍니다.
3. **차이점과 이유를 설명합니다**: 단순히 "이렇게 쓴다"가 아니라 "왜 이렇게 설계되었는가"를 다룹니다.

예를 들어, 배열 필터링을 배울 때:

```typescript
// 이미 아는 방식
const adults = users.filter(u => u.age >= 18);
```

```csharp
// C#으로 같은 작업
var adults = users.Where(u => u.Age >= 18).ToList();
```

"`.filter()`가 `.Where()`로 바뀌었고, `.ToList()`를 붙여야 한다"는 것 이상을 배웁니다. LINQ가 지연 실행(deferred execution)을 사용하는 이유, 이것이 어떻게 메모리 효율성을 제공하는지, 언제 `.ToList()`를 호출해야 하는지를 이해합니다.

## 학습 목표

Part 1을 마치면 다음을 할 수 있습니다:

- TypeScript/JavaScript 개념을 C#으로 자연스럽게 변환할 수 있습니다
- C#의 타입 시스템(값 타입, 참조 타입, nullable 타입)을 이해하고 활용합니다
- LINQ를 사용하여 데이터를 선언적으로 처리합니다
- 패턴 매칭으로 복잡한 조건문을 간결하게 작성합니다
- async/await로 비동기 작업을 처리합니다
- 이벤트와 델리게이트로 느슨하게 결합된 코드를 작성합니다
- 최신 C# 기능(레코드, init 전용 프로퍼티, 파일 스코프 네임스페이스 등)을 활용합니다

## 챕터 구성

### [Chapter 1: TypeScript 개발자를 위한 C# 퀵스타트](./chapter1/README.md)

이 챕터는 여러분이 이미 아는 TypeScript/JavaScript 개념을 C#에 매핑합니다. 새로운 언어를 배운다기보다는, 익숙한 개념의 새로운 표현을 배우는 것입니다.

**1.1 타입 시스템 비교**
- TypeScript vs C#의 타입 시스템 철학
- 타입 소거(Type Erasure) vs 런타임 타입
- 인터페이스, 제네릭, nullable 타입
- 타입 추론과 명시적 타입 선언

**1.2 익숙한 개념, 새로운 문법**
- 변수 선언: `const`/`let` → `var`/`const`
- 함수와 람다: 화살표 함수 → 람다 표현식
- 비동기: `Promise` → `Task`, `async/await`
- 구조 분해: 튜플과 레코드

**1.3 LINQ: 함수형 데이터 처리**
- 배열 메서드(`.map`, `.filter`, `.reduce`) → LINQ
- 메서드 체이닝과 쿼리 구문
- 지연 실행(Deferred Execution)과 즉시 실행
- 복잡한 데이터 변환 예제

**1.4 패턴 매칭과 현대적 기능**
- Switch 표현식과 패턴 매칭
- 레코드 타입과 불변성
- Null 안전성: null 병합 연산자, null 조건부 연산자

**핵심 학습 포인트**: TypeScript에서 C#으로의 개념 매핑, LINQ의 강력함, 함수형 프로그래밍 패턴

### [Chapter 2: C# 고급 기능과 객체지향 프로그래밍](./chapter2/README.md)

TypeScript에서는 명시적이지 않거나 덜 강조되는, C#의 고유한 특성을 깊이 다룹니다. 이 챕터를 마치면 "C#답게" 코드를 작성할 수 있습니다.

**2.1 값 타입과 참조 타입**
- 스택 vs 힙 메모리
- `struct` vs `class`: 언제 무엇을 사용할까?
- Boxing과 Unboxing
- 성능 고려사항

**2.2 프로퍼티: Getter/Setter를 넘어서**
- Auto-property와 표현식 본문
- `init` 전용 프로퍼티: 불변 객체 만들기
- 계산된 프로퍼티
- 프로퍼티 vs 필드: 캡슐화

**2.3 이벤트와 델리게이트**
- 델리게이트: 타입 안전한 함수 포인터
- 이벤트 패턴: Publisher-Subscriber
- `Action<T>`과 `Func<T, TResult>`: 내장 델리게이트
- 이벤트 vs 콜백: 언제 무엇을 쓸까?

**2.4 객체지향 프로그래밍의 철학**
- 인터페이스 vs 추상 클래스
- 의존성 역전 원칙(DIP)과 의존성 주입
- 확장 메서드: 기존 타입에 메서드 추가
- SOLID 원칙을 C#으로 구현

**2.5 고급 LINQ와 함수형 프로그래밍**
- `GroupBy`, `Join`, `SelectMany`: 복잡한 쿼리
- 지연 실행의 함정과 해결법
- LINQ와 Entity Framework: 쿼리가 SQL이 되는 마법
- 사용자 정의 LINQ 연산자

**2.6 최신 C# 기능 (C# 13/14)**
- Primary Constructors: 간결한 클래스 정의
- 파일 스코프 네임스페이스
- Raw 문자열 리터럴
- List Patterns와 Slice Patterns
- 필수 멤버(`required`)

**핵심 학습 포인트**: 메모리 관리, C# 고유의 객체지향 패턴, 최신 언어 기능

## 실습 프로젝트

각 챕터에는 실전 예제가 포함되어 있어, 이론을 즉시 코드로 옮길 수 있습니다.

### Chapter 1 실습: Todo 관리 시스템
- TypeScript의 Todo 앱을 C#으로 변환
- LINQ로 필터링, 정렬, 그룹화
- async/await로 비동기 작업 처리

### Chapter 2 실습: 이벤트 기반 UI 컴포넌트
- 이벤트와 델리게이트로 컴포넌트 간 통신
- 프로퍼티로 반응형 상태 관리
- SOLID 원칙 적용

## TypeScript/JavaScript vs C# 핵심 비교

| 개념 | TypeScript/JavaScript | C# |
|------|----------------------|-----|
| **타입 시스템** | 컴파일 타임 타입 소거 | 런타임 타입 유지 |
| **타입 선언** | `let name: string` | `string name` |
| **함수** | `const fn = (x) => x * 2` | `(int x) => x * 2` |
| **비동기** | `Promise<T>`, `async/await` | `Task<T>`, `async/await` |
| **배열 메서드** | `.map()`, `.filter()`, `.reduce()` | `.Select()`, `.Where()`, `.Aggregate()` |
| **Null 처리** | `?.`, `??` | `?.`, `??` (동일!) |
| **구조 분해** | `const [a, b] = arr` | `var (a, b) = tuple` |
| **인터페이스** | `interface IPerson { }` | `interface IPerson { }` (거의 동일) |
| **제네릭** | `Array<T>`, `Promise<T>` | `List<T>`, `Task<T>` |
| **클래스** | `class User { }` | `class User { }` (거의 동일) |
| **모듈** | ES6 `import/export` | `using`, 네임스페이스 |

## 다음 단계

Part 1에서 C#의 기초를 다졌다면, Part 2로 넘어가 ASP.NET Core로 실제 웹 애플리케이션을 만들어봅시다:

- **[Part 2: ASP.NET Core 기초](../part2-aspnetcore-basics/README.md)** - 첫 번째 API 구축
- **Part 3: 서버 사이드 렌더링** - Razor Pages와 MVC
- **Part 4: Blazor** - C#으로 프론트엔드 작성
- **Part 5: Entity Framework Core** - 데이터베이스와 ORM

## 추가 리소스

### 공식 문서
- [C# 언어 가이드](https://docs.microsoft.com/dotnet/csharp/)
- [.NET API 브라우저](https://docs.microsoft.com/dotnet/api/)
- [C# 언어 명세](https://docs.microsoft.com/dotnet/csharp/language-reference/)

### 온라인 학습
- [Microsoft Learn - C# 학습 경로](https://docs.microsoft.com/learn/paths/csharp-first-steps/)
- [.NET Interactive Notebooks](https://github.com/dotnet/interactive)

### TypeScript 개발자를 위한 가이드
- [TypeScript와 C# 비교](https://aka.ms/typescript-to-csharp)
- [LINQ와 JavaScript 배열 메서드](https://learn.microsoft.com/dotnet/csharp/linq/)

## 시작하기

준비되셨나요? [Chapter 1: TypeScript 개발자를 위한 C# 퀵스타트](./chapter1/README.md)로 시작하세요!

TypeScript의 지식을 C#으로 확장하는 여정이 즐겁고 생산적이기를 바랍니다. 두 언어가 공유하는 철학을 발견하고, 각 언어의 고유한 강점을 이해하면서, 여러분은 더 나은 풀스택 개발자가 될 것입니다.

즐거운 학습 되세요! 🚀
