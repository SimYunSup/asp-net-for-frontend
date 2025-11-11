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

[Exercise.cs](./Exercise.cs) 파일에서 다음을 구현하세요:

1. 여러 API를 병렬로 호출하고 결과를 결합
2. 타임아웃이 있는 비동기 작업
3. 재시도 로직이 있는 비동기 작업
4. 비동기 스트림 (IAsyncEnumerable) 사용
