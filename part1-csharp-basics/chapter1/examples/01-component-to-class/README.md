# 실습 1: React 컴포넌트 로직을 C# 클래스로 변환하기

## 목표

React 컴포넌트의 상태 관리와 비즈니스 로직을 C# 클래스로 변환하는 방법을 학습합니다.

## React 컴포넌트 (Before)

```typescript
// UserManager.tsx
import { useState, useEffect } from 'react';

interface User {
  id: number;
  name: string;
  email: string;
  isActive: boolean;
}

export function UserManager() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadUsers();
  }, []);

  async function loadUsers() {
    setLoading(true);
    setError(null);

    try {
      const response = await fetch('/api/users');
      if (!response.ok) throw new Error('Failed to load users');
      const data = await response.json();
      setUsers(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  async function addUser(name: string, email: string) {
    try {
      const response = await fetch('/api/users', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name, email, isActive: true })
      });

      if (!response.ok) throw new Error('Failed to add user');
      const newUser = await response.json();
      setUsers([...users, newUser]);
    } catch (err) {
      setError(err.message);
    }
  }

  async function toggleUserStatus(userId: number) {
    const user = users.find(u => u.id === userId);
    if (!user) return;

    try {
      const response = await fetch(`/api/users/${userId}`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ isActive: !user.isActive })
      });

      if (!response.ok) throw new Error('Failed to update user');
      const updatedUser = await response.json();
      setUsers(users.map(u => u.id === userId ? updatedUser : u));
    } catch (err) {
      setError(err.message);
    }
  }

  function getActiveUsers(): User[] {
    return users.filter(u => u.isActive);
  }

  function getInactiveUsers(): User[] {
    return users.filter(u => !u.isActive);
  }

  // ... JSX rendering
}
```

## C# 클래스 (After)

```csharp
// User.cs
public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; }
}

// UserManager.cs
public class UserManager
{
    private readonly HttpClient _httpClient;
    private readonly List<User> _users;
    private bool _loading;
    private string? _error;

    // 이벤트: 상태 변경 알림 (React의 setState 효과)
    public event EventHandler? UsersChanged;
    public event EventHandler<bool>? LoadingChanged;
    public event EventHandler<string?>? ErrorChanged;

    // 프로퍼티: React의 state와 유사
    public IReadOnlyList<User> Users => _users.AsReadOnly();

    public bool Loading
    {
        get => _loading;
        private set
        {
            _loading = value;
            LoadingChanged?.Invoke(this, value);
        }
    }

    public string? Error
    {
        get => _error;
        private set
        {
            _error = value;
            ErrorChanged?.Invoke(this, value);
        }
    }

    public UserManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _users = new List<User>();
    }

    // useEffect 대신 명시적 호출
    public async Task InitializeAsync()
    {
        await LoadUsersAsync();
    }

    public async Task LoadUsersAsync()
    {
        Loading = true;
        Error = null;

        try
        {
            var response = await _httpClient.GetAsync("/api/users");
            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<List<User>>();
            if (users != null)
            {
                _users.Clear();
                _users.AddRange(users);
                UsersChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (HttpRequestException ex)
        {
            Error = $"Failed to load users: {ex.Message}";
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task AddUserAsync(string name, string email)
    {
        try
        {
            var newUser = new { name, email, isActive = true };
            var response = await _httpClient.PostAsJsonAsync("/api/users", newUser);
            response.EnsureSuccessStatusCode();

            var addedUser = await response.Content.ReadFromJsonAsync<User>();
            if (addedUser != null)
            {
                _users.Add(addedUser);
                UsersChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (HttpRequestException ex)
        {
            Error = $"Failed to add user: {ex.Message}";
        }
    }

    public async Task ToggleUserStatusAsync(int userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        if (user == null) return;

        try
        {
            var update = new { isActive = !user.IsActive };
            var response = await _httpClient.PatchAsJsonAsync($"/api/users/{userId}", update);
            response.EnsureSuccessStatusCode();

            var updatedUser = await response.Content.ReadFromJsonAsync<User>();
            if (updatedUser != null)
            {
                var index = _users.FindIndex(u => u.Id == userId);
                if (index >= 0)
                {
                    _users[index] = updatedUser;
                    UsersChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Error = $"Failed to update user: {ex.Message}";
        }
    }

    // LINQ 사용 - JavaScript의 filter와 유사
    public IEnumerable<User> GetActiveUsers()
    {
        return _users.Where(u => u.IsActive);
    }

    public IEnumerable<User> GetInactiveUsers()
    {
        return _users.Where(u => !u.IsActive);
    }

    // 추가 유틸리티 메서드
    public User? GetUserById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public int GetActiveUserCount()
    {
        return _users.Count(u => u.IsActive);
    }
}
```

## 사용 예제

```csharp
// Program.cs 또는 서비스에서 사용
public class UserService
{
    private readonly UserManager _userManager;

    public UserService(HttpClient httpClient)
    {
        _userManager = new UserManager(httpClient);

        // 이벤트 구독 (React의 useEffect와 유사)
        _userManager.UsersChanged += OnUsersChanged;
        _userManager.LoadingChanged += OnLoadingChanged;
        _userManager.ErrorChanged += OnErrorChanged;
    }

    private void OnUsersChanged(object? sender, EventArgs e)
    {
        Console.WriteLine($"Users updated. Total: {_userManager.Users.Count}");
    }

    private void OnLoadingChanged(object? sender, bool loading)
    {
        Console.WriteLine($"Loading: {loading}");
    }

    private void OnErrorChanged(object? sender, string? error)
    {
        if (error != null)
        {
            Console.WriteLine($"Error: {error}");
        }
    }

    public async Task RunAsync()
    {
        // 초기화
        await _userManager.InitializeAsync();

        // 사용자 추가
        await _userManager.AddUserAsync("John Doe", "john@example.com");

        // 활성 사용자 조회
        var activeUsers = _userManager.GetActiveUsers();
        foreach (var user in activeUsers)
        {
            Console.WriteLine($"{user.Name} ({user.Email})");
        }

        // 사용자 상태 토글
        if (_userManager.Users.Any())
        {
            await _userManager.ToggleUserStatusAsync(_userManager.Users[0].Id);
        }
    }
}
```

## 주요 변환 포인트

### 1. State → Properties + Events

**React:**
```typescript
const [users, setUsers] = useState<User[]>([]);
```

**C#:**
```csharp
private readonly List<User> _users;
public IReadOnlyList<User> Users => _users.AsReadOnly();
public event EventHandler? UsersChanged;
```

### 2. useEffect → 명시적 메서드 호출

**React:**
```typescript
useEffect(() => {
  loadUsers();
}, []);
```

**C#:**
```csharp
public async Task InitializeAsync()
{
    await LoadUsersAsync();
}
```

### 3. 배열 메서드 → LINQ

**React:**
```typescript
users.filter(u => u.isActive)
```

**C#:**
```csharp
_users.Where(u => u.IsActive)
```

### 4. async/await → Task/async-await

**React:**
```typescript
async function loadUsers() { ... }
```

**C#:**
```csharp
public async Task LoadUsersAsync() { ... }
```

## 연습 문제

1. `UserManager`에 사용자 삭제 기능(`DeleteUserAsync`) 추가
2. 사용자 이름으로 검색하는 `SearchUsers(string query)` 메서드 구현
3. 사용자 정렬 기능 추가 (이름순, 이메일순)
4. 페이징 기능 구현 (`GetUsersByPage(int page, int pageSize)`)

## 해답 예시

[Exercise.cs](./Exercise.cs) 파일에서 직접 구현해보세요!
