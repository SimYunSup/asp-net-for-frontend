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
