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
