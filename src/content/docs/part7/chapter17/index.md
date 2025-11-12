---
title: "Chapter 17 - 실시간 통신과 백그라운드 처리"
---

# Chapter 17: 실시간 통신과 백그라운드 처리

## 프로덕션의 현실: 동기 세계를 넘어서

지금까지 만든 API는 대부분 동기적입니다. 클라이언트가 요청을 보내면, 서버가 즉시 처리하고 응답합니다. 데이터베이스 쿼리, 비즈니스 로직 실행, JSON 직렬화—모든 것이 HTTP 요청-응답 사이클 내에서 일어납니다. 이는 대부분의 경우 완벽하게 작동하지만, 현실 세계의 애플리케이션은 더 복잡한 요구사항을 가지고 있습니다.

사용자가 대용량 파일을 업로드하면 어떻게 해야 할까요? 바이러스 검사, 포맷 변환, 썸네일 생성... 이 모든 것을 HTTP 요청 내에서 처리하면 사용자는 몇 분을 기다려야 하며, 브라우저는 타임아웃될 수 있습니다. 게다가 서버 리소스가 묶여, 다른 요청을 처리할 수 없게 됩니다.

주식 거래 대시보드는 어떻게 만들어야 할까요? 가격은 초 단위로 변합니다. 클라이언트가 1초마다 서버에 "새 가격 있어?"라고 물어보는 것(폴링)은 비효율적입니다. 서버가 능동적으로 클라이언트에게 가격 변동을 푸시할 수 있다면 얼마나 좋을까요?

매일 자정에 리포트를 생성해야 한다면? 누군가 자정에 API를 호출해야 할까요? 아니면 서버가 스스로 작업을 예약하고 실행할 수 있어야 할까요?

이 챕터는 이러한 현실적인 요구사항을 해결합니다. **실시간 통신**으로 서버에서 클라이언트로 데이터를 능동적으로 푸시하고, **백그라운드 처리**로 긴 작업을 HTTP 요청에서 분리하며, **메시지 큐**로 마이크로서비스 간 통신을 느슨하게 결합합니다.

## Part 1: 실시간 통신 - 서버에서 클라이언트로

### 실시간의 진화: 폴링에서 WebSocket까지

HTTP는 요청-응답 프로토콜입니다. 클라이언트가 먼저 요청을 보내야만, 서버가 응답할 수 있습니다. 서버가 새로운 데이터를 클라이언트에게 보내고 싶어도, 클라이언트의 요청이 없으면 불가능합니다. 이 근본적인 제약이 실시간 통신을 어렵게 만듭니다.

**1단계: 단순 폴링(Short Polling)**

가장 단순한 해결책은 클라이언트가 주기적으로 서버에 물어보는 것입니다.

```typescript
// 3초마다 서버에 새 메시지 확인
setInterval(async () => {
  const response = await fetch('/api/messages/new');
  const messages = await response.json();
  if (messages.length > 0) {
    displayMessages(messages);
  }
}, 3000);
```

이는 구현이 쉽지만, 엄청난 비효율입니다. 새 메시지가 없어도 3초마다 HTTP 요청이 발생합니다. 1,000명의 사용자가 있다면, 서버는 초당 333개의 요청을 처리해야 하며, 그중 대부분은 "없어"라는 응답만 받습니다. 네트워크 대역폭, 서버 CPU, 데이터베이스 연결—모든 것이 낭비됩니다.

게다가 실시간성도 떨어집니다. 메시지가 도착한 직후 폴링이 방금 지나갔다면, 사용자는 최대 3초를 기다려야 합니다. 폴링 간격을 줄이면 실시간성은 개선되지만, 서버 부하는 기하급수적으로 증가합니다.

**2단계: 롱 폴링(Long Polling)**

롱 폴링은 이를 개선합니다. 클라이언트가 요청을 보내면, 서버는 **새 데이터가 있을 때까지** 응답을 보류합니다.

```csharp
[HttpGet("messages/long-poll")]
public async Task<IActionResult> LongPoll(CancellationToken cancellationToken)
{
    // 최대 30초 대기
    var timeout = TimeSpan.FromSeconds(30);
    var started = DateTime.UtcNow;

    while (DateTime.UtcNow - started < timeout)
    {
        var messages = await GetNewMessagesAsync();
        if (messages.Any())
        {
            return Ok(messages); // 새 데이터 있음, 즉시 응답
        }

        // 100ms 대기 후 재확인
        await Task.Delay(100, cancellationToken);
    }

    // 타임아웃, 빈 응답
    return Ok(Array.Empty<Message>());
}
```

```typescript
async function longPoll() {
  while (true) {
    const response = await fetch('/api/messages/long-poll');
    const messages = await response.json();

    if (messages.length > 0) {
      displayMessages(messages);
    }

    // 즉시 다음 요청
  }
}

longPoll();
```

롱 폴링은 실시간성을 크게 향상시킵니다. 새 메시지가 도착하면 거의 즉시(100ms 이내) 클라이언트에 전달됩니다. 불필요한 요청도 줄어듭니다. 하지만 여전히 각 업데이트마다 HTTP 요청-응답 사이클을 거쳐야 하며, 서버는 각 연결마다 리소스를 할당해야 합니다.

**3단계: Server-Sent Events (SSE)**

SSE는 HTTP의 근본적인 제약을 우회합니다. 연결을 계속 열어두고, 서버가 일방적으로 데이터를 스트리밍합니다.

```csharp
[HttpGet("messages/stream")]
public async Task StreamMessages(CancellationToken cancellationToken)
{
    Response.Headers.Add("Content-Type", "text/event-stream");
    Response.Headers.Add("Cache-Control", "no-cache");
    Response.Headers.Add("Connection", "keep-alive");

    while (!cancellationToken.IsCancellationRequested)
    {
        var messages = await GetNewMessagesAsync();
        foreach (var message in messages)
        {
            var json = JsonSerializer.Serialize(message);
            await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }

        await Task.Delay(1000, cancellationToken);
    }
}
```

```typescript
const eventSource = new EventSource('/api/messages/stream');

eventSource.onmessage = (event) => {
  const message = JSON.parse(event.data);
  displayMessage(message);
};

eventSource.onerror = (error) => {
  console.error('SSE Error:', error);
  eventSource.close();
};
```

SSE는 단순하고 효율적입니다. HTTP를 그대로 사용하므로, 프록시나 방화벽과의 호환성이 좋습니다. 브라우저의 네이티브 `EventSource` API가 재연결을 자동으로 처리합니다.

하지만 SSE는 **단방향**입니다. 서버에서 클라이언트로만 데이터를 보낼 수 있습니다. 클라이언트가 서버로 메시지를 보내려면 별도의 HTTP POST 요청이 필요합니다. 채팅 애플리케이션처럼 양방향 통신이 필요한 경우, 이는 불편합니다.

**4단계: WebSocket - 완전한 양방향 통신**

WebSocket은 HTTP를 업그레이드하여 완전한 양방향 연결을 만듭니다. 초기 핸드셰이크는 HTTP로 이루어지지만, 이후 연결은 WebSocket 프로토콜로 전환됩니다.

```csharp
app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws/chat")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await HandleChatAsync(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

async Task HandleChatAsync(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];

    while (webSocket.State == WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            await webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closing",
                CancellationToken.None);
        }
        else
        {
            // 받은 메시지를 모든 클라이언트에 브로드캐스트
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await BroadcastToAllClientsAsync(message);
        }
    }
}
```

```typescript
const ws = new WebSocket('ws://localhost:5000/ws/chat');

ws.onopen = () => {
  console.log('WebSocket Connected');
  ws.send(JSON.stringify({ type: 'join', user: 'Alice' }));
};

ws.onmessage = (event) => {
  const message = JSON.parse(event.data);
  displayMessage(message);
};

ws.onerror = (error) => {
  console.error('WebSocket Error:', error);
};

ws.onclose = () => {
  console.log('WebSocket Closed');
  // 재연결 로직
  setTimeout(() => {
    // 재연결 시도
  }, 1000);
};

// 메시지 전송
function sendMessage(text) {
  ws.send(JSON.stringify({ type: 'message', text }));
}
```

WebSocket은 완전한 양방향이며, 오버헤드가 매우 낮습니다. HTTP 헤더를 매번 보낼 필요가 없으며, TCP 연결을 재사용합니다. 하지만 직접 구현하기에는 복잡합니다. 연결 관리, 재연결 로직, 하트비트, 메시지 직렬화, 에러 처리, 보안... 모든 것을 직접 만들어야 합니다.

### SignalR: WebSocket의 복잡성을 숨기다

**SignalR**은 실시간 통신의 모든 복잡성을 추상화합니다. WebSocket이 가능하면 사용하고, 불가능하면 자동으로 SSE나 롱 폴링으로 폴백합니다. 재연결은 자동이며, 메시지 직렬화는 투명하고, 연결 관리는 프레임워크가 처리합니다.

```csharp
public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendToUser(string targetUser, string message)
    {
        await Clients.User(targetUser).SendAsync("ReceiveMessage", "Private", message);
    }

    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("UserJoined", Context.User?.Identity?.Name);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SendAsync("UserConnected", Context.User?.Identity?.Name);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.SendAsync("UserDisconnected", Context.User?.Identity?.Name);
        await base.OnDisconnectedAsync(exception);
    }
}

// Program.cs
builder.Services.AddSignalR();
app.MapHub<ChatHub>("/chatHub");
```

```typescript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect() // 자동 재연결!
    .build();

connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user}: ${message}`);
    addMessageToUI(user, message);
});

connection.on("UserJoined", (userName) => {
    console.log(`${userName} joined the chat`);
});

connection.onreconnecting((error) => {
    console.warn("Reconnecting...", error);
    showReconnectingIndicator();
});

connection.onreconnected((connectionId) => {
    console.log("Reconnected:", connectionId);
    hideReconnectingIndicator();
});

await connection.start();

// 메시지 전송
await connection.invoke("SendMessage", "Alice", "Hello, World!");

// 방 참가
await connection.invoke("JoinRoom", "developers");
```

SignalR의 장점은 명확합니다:

- **자동 전송 협상**: 최선의 방법을 자동으로 선택
- **자동 재연결**: 네트워크 끊김 시 자동 복구
- **타입 안전성**: Strongly-typed Hub로 컴파일 타임 검증
- **확장성**: Redis backplane, Azure SignalR Service로 무한 확장
- **보안**: ASP.NET Core 인증/권한 부여 통합

### Strongly-Typed Hub: 컴파일 타임 안전성

SignalR의 가장 강력한 기능 중 하나는 강타입 Hub입니다.

```csharp
// 클라이언트 메서드 인터페이스
public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
    Task UserJoined(string userName);
    Task UserLeft(string userName);
    Task TypingIndicator(string userName, bool isTyping);
}

public class ChatHub : Hub<IChatClient>
{
    private readonly ILogger<ChatHub> _logger;
    private readonly ChatService _chatService;

    public ChatHub(ILogger<ChatHub> logger, ChatService chatService)
    {
        _logger = logger;
        _chatService = chatService;
    }

    public async Task SendMessage(string message)
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";

        // 타입 안전한 호출 - 인텔리센스 작동!
        await Clients.All.ReceiveMessage(userName, message);

        // 데이터베이스에 저장
        await _chatService.SaveMessageAsync(userName, message);
    }

    public async Task SetTyping(bool isTyping)
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";

        // Others는 호출자를 제외한 모든 클라이언트
        await Clients.Others.TypingIndicator(userName, isTyping);
    }

    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        await Clients.Group(roomName).UserJoined(userName);

        _logger.LogInformation("{UserName} joined room {RoomName}", userName, roomName);
    }

    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        await Clients.Group(roomName).UserLeft(userName);
    }

    public override async Task OnConnectedAsync()
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        _logger.LogInformation("{UserName} connected: {ConnectionId}",
            userName, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        _logger.LogInformation("{UserName} disconnected: {ConnectionId}",
            userName, Context.ConnectionId);

        if (exception != null)
        {
            _logger.LogError(exception, "Connection error for {UserName}", userName);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
```

인터페이스를 통해 클라이언트 메서드가 명확히 정의되며, 오타나 잘못된 파라미터는 컴파일 에러로 즉시 발견됩니다.

### SignalR 확장성: Redis Backplane과 Azure SignalR Service

단일 서버에서 SignalR은 완벽하게 작동합니다. 하지만 로드 밸런서 뒤에 여러 서버가 있다면 문제가 생깁니다. 사용자 A는 서버 1에, 사용자 B는 서버 2에 연결되어 있습니다. A가 메시지를 보내면 서버 1의 메모리에만 있으므로, 서버 2에 연결된 B는 받을 수 없습니다.

**Redis Backplane**이 이를 해결합니다. 모든 서버가 Redis를 공유 메시지 버스로 사용합니다. 서버 1이 메시지를 받으면 Redis에 발행하고, 모든 서버가 구독하여 자신의 클라이언트에게 전달합니다.

```csharp
// NuGet: Microsoft.AspNetCore.SignalR.StackExchangeRedis
builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379", options =>
    {
        options.Configuration.ChannelPrefix = "MyApp.SignalR";
    });
```

이제 사용자가 어느 서버에 연결되어 있든, 모든 메시지를 받을 수 있습니다.

**Azure SignalR Service**는 한 단계 더 나아갑니다. 완전 관리형 서비스로, 연결 관리를 Azure에 위임합니다. 여러분의 ASP.NET Core 앱은 클라이언트 연결을 직접 처리하지 않고, Azure SignalR Service에 메시지를 보내기만 하면 됩니다. Azure는 수십만 개의 동시 연결을 처리하며, 자동으로 확장합니다.

```csharp
builder.Services.AddSignalR()
    .AddAzureSignalR(options =>
    {
        options.ConnectionString = builder.Configuration["Azure:SignalR:ConnectionString"];
    });
```

코드 변경이 거의 없이, 무한 확장 가능한 실시간 통신을 얻습니다.

### 실전 패턴: 실시간 대시보드

실시간 통신의 대표적인 사용 사례는 대시보드입니다. 서버 메트릭, 주문 통계, 사용자 활동... 이 모든 것이 실시간으로 업데이트되어야 합니다.

```csharp
public interface IDashboardClient
{
    Task UpdateMetrics(ServerMetrics metrics);
    Task NewOrder(Order order);
    Task UserActivity(string userName, string activity);
}

public class DashboardHub : Hub<IDashboardClient>
{
    [Authorize(Roles = "Admin")]
    public async Task SubscribeToMetrics()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "metrics-subscribers");
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task SubscribeToOrders()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "order-subscribers");
    }
}

// 백그라운드 서비스에서 메트릭 푸시
public class MetricsCollectorService : BackgroundService
{
    private readonly IHubContext<DashboardHub, IDashboardClient> _hubContext;
    private readonly IMetricsProvider _metricsProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var metrics = await _metricsProvider.GetCurrentMetricsAsync();

            // 모든 구독자에게 푸시
            await _hubContext.Clients
                .Group("metrics-subscribers")
                .UpdateMetrics(metrics);

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}

// 주문 생성 시 실시간 알림
public class OrderService
{
    private readonly IHubContext<DashboardHub, IDashboardClient> _hubContext;

    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        var order = new Order { /* ... */ };
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        // SignalR로 실시간 알림
        await _hubContext.Clients
            .Group("order-subscribers")
            .NewOrder(order);

        return order;
    }
}
```

```typescript
// React 대시보드
function DashboardMetrics() {
  const [metrics, setMetrics] = useState<ServerMetrics | null>(null);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('/dashboardHub', {
        accessTokenFactory: () => getJwtToken()
      })
      .withAutomaticReconnect()
      .build();

    newConnection.on('UpdateMetrics', (newMetrics: ServerMetrics) => {
      setMetrics(newMetrics);
    });

    newConnection.start().then(() => {
      newConnection.invoke('SubscribeToMetrics');
    });

    setConnection(newConnection);

    return () => {
      newConnection.stop();
    };
  }, []);

  if (!metrics) return <div>Loading...</div>;

  return (
    <div className="metrics-dashboard">
      <MetricCard label="CPU" value={`${metrics.cpuUsage}%`} />
      <MetricCard label="Memory" value={`${metrics.memoryUsage}%`} />
      <MetricCard label="Active Connections" value={metrics.activeConnections} />
    </div>
  );
}
```

## Part 2: 백그라운드 처리 - 사용자를 기다리게 하지 마라

### 왜 백그라운드 처리가 필요한가?

HTTP 요청에는 타임아웃이 있습니다. 브라우저는 보통 30초~2분 후 요청을 포기합니다. 프록시나 로드 밸런서는 더 짧은 타임아웃을 가질 수 있습니다. 게다가 사용자는 기다리기 싫어합니다. 버튼을 클릭한 후 30초 동안 로딩 스피너를 보는 것은 끔찍한 경험입니다.

일부 작업은 본질적으로 느립니다:

- **이미지/비디오 처리**: 리사이징, 포맷 변환, 썸네일 생성, 워터마크 추가
- **대량 데이터 처리**: 수천 개의 레코드 업데이트, CSV 가져오기/내보내기
- **외부 API 호출**: 결제 처리, 이메일 발송, SMS 전송, 외부 서비스 연동
- **복잡한 계산**: 리포트 생성, 통계 분석, AI/ML 추론
- **주기적 작업**: 일일 리포트, 데이터 동기화, 캐시 갱신, 정리 작업

이런 작업을 HTTP 요청 내에서 처리하면:

1. **사용자 경험 저하**: 긴 대기 시간, 타임아웃
2. **서버 리소스 낭비**: 스레드가 묶여 다른 요청 처리 불가
3. **에러 처리 어려움**: 네트워크 끊김 시 작업 유실
4. **확장성 문제**: 긴 요청이 많으면 서버 과부하

백그라운드 처리는 이를 해결합니다. HTTP 요청은 작업을 큐에 추가하고 즉시 응답합니다. 백그라운드 워커가 큐에서 작업을 꺼내 처리하며, 완료되면 사용자에게 알립니다(이메일, 푸시 알림, SignalR).

### IHostedService: 지속 실행 서비스

`IHostedService`는 ASP.NET Core 애플리케이션과 함께 시작되고 종료되는 백그라운드 작업을 정의합니다. 주기적인 작업, 데이터 동기화, 모니터링 등에 사용됩니다.

```csharp
public class DataSyncService : BackgroundService
{
    private readonly ILogger<DataSyncService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DataSyncService(ILogger<DataSyncService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Data Sync Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // IServiceProvider에서 스코프 생성 (DbContext는 스코프 서비스)
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var externalApi = scope.ServiceProvider.GetRequiredService<IExternalApiClient>();

                // 외부 API에서 데이터 가져오기
                var externalData = await externalApi.GetLatestDataAsync();

                // 데이터베이스 업데이트
                await SyncDataAsync(dbContext, externalData);

                _logger.LogInformation("Data sync completed. Next sync in 5 minutes.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during data sync");
            }

            // 5분 대기
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }

        _logger.LogInformation("Data Sync Service stopped");
    }

    private async Task SyncDataAsync(AppDbContext context, ExternalData data)
    {
        // 동기화 로직
        foreach (var item in data.Items)
        {
            var existing = await context.Products.FindAsync(item.Id);
            if (existing == null)
            {
                context.Products.Add(new Product
                {
                    ExternalId = item.Id,
                    Name = item.Name,
                    Price = item.Price
                });
            }
            else
            {
                existing.Name = item.Name;
                existing.Price = item.Price;
            }
        }

        await context.SaveChangesAsync();
    }
}

// Program.cs
builder.Services.AddHostedService<DataSyncService>();
```

`BackgroundService`는 `IHostedService`의 기본 구현으로, `ExecuteAsync` 메서드만 오버라이드하면 됩니다. `CancellationToken`을 통해 정상 종료를 지원하며, 애플리케이션이 종료될 때 자동으로 취소됩니다.

**중요한 점**: `IHostedService`는 싱글톤입니다. `DbContext` 같은 스코프 서비스를 직접 주입할 수 없습니다. `IServiceProvider`를 주입하여 필요할 때 스코프를 생성해야 합니다.

### 작업 큐 패턴: 즉시 응답, 나중에 처리

사용자가 트리거하는 긴 작업은 작업 큐로 처리합니다. HTTP 요청 핸들러는 작업을 큐에 추가하고 즉시 응답하며, 백그라운드 서비스가 큐에서 작업을 꺼내 처리합니다.

```csharp
// 작업 큐 인터페이스
public interface IBackgroundTaskQueue
{
    void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
    Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
}

// 메모리 기반 큐 구현
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _queue;

    public BackgroundTaskQueue(int capacity)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
    }

    public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
    {
        if (workItem == null)
            throw new ArgumentNullException(nameof(workItem));

        _queue.Writer.TryWrite(workItem);
    }

    public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}

// 백그라운드 워커
public class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<QueuedHostedService> _logger;

    public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued Hosted Service is running");

        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _taskQueue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing work item");
            }
        }

        _logger.LogInformation("Queued Hosted Service is stopping");
    }
}

// Program.cs
builder.Services.AddSingleton<IBackgroundTaskQueue>(_ => new BackgroundTaskQueue(100));
builder.Services.AddHostedService<QueuedHostedService>();
```

**사용 예시: 이미지 업로드 및 처리**

```csharp
[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IWebHostEnvironment _env;
    private readonly IHubContext<ProgressHub> _hubContext;

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        // 원본 파일 저장
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var uploadPath = Path.Combine(_env.WebRootPath, "uploads", fileName);

        using (var stream = new FileStream(uploadPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 백그라운드 작업 큐에 추가
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _taskQueue.QueueBackgroundWorkItem(async token =>
        {
            await ProcessImageAsync(uploadPath, fileName, userId, token);
        });

        return Ok(new
        {
            message = "Image uploaded successfully. Processing in background...",
            fileName
        });
    }

    private async Task ProcessImageAsync(string originalPath, string fileName, string? userId, CancellationToken token)
    {
        try
        {
            // SignalR로 진행 상태 알림
            await _hubContext.Clients.User(userId!).SendAsync("ProgressUpdate", new
            {
                fileName,
                status = "processing",
                progress = 0
            }, token);

            // 썸네일 생성
            var thumbnailPath = Path.Combine(_env.WebRootPath, "thumbnails", fileName);
            await GenerateThumbnailAsync(originalPath, thumbnailPath, token);

            await _hubContext.Clients.User(userId!).SendAsync("ProgressUpdate", new
            {
                fileName,
                status = "processing",
                progress = 50
            }, token);

            // 여러 크기 생성
            var sizes = new[] { 200, 400, 800 };
            foreach (var size in sizes)
            {
                var resizedPath = Path.Combine(_env.WebRootPath, "resized", $"{size}_{fileName}");
                await ResizeImageAsync(originalPath, resizedPath, size, token);
            }

            // 완료
            await _hubContext.Clients.User(userId!).SendAsync("ProgressUpdate", new
            {
                fileName,
                status = "completed",
                progress = 100
            }, token);
        }
        catch (Exception ex)
        {
            await _hubContext.Clients.User(userId!).SendAsync("ProgressUpdate", new
            {
                fileName,
                status = "failed",
                error = ex.Message
            }, token);
        }
    }
}
```

### Hangfire: 프로덕션급 백그라운드 작업

작업 큐 패턴은 간단하지만, 한계가 있습니다. 서버가 재시작되면 메모리 큐의 작업이 유실됩니다. 재시도 로직, 스케줄링, 진행 상태 추적... 모든 것을 직접 구현해야 합니다.

**Hangfire**는 이 모든 문제를 해결하는 프로덕션급 라이브러리입니다. 작업을 데이터베이스에 영속화하므로, 서버 재시작 시에도 유실되지 않습니다. 자동 재시도, 스케줄링, 작업 체인, 대시보드 UI를 내장하고 있습니다.

```bash
dotnet add package Hangfire.AspNetCore
dotnet add package Hangfire.SqlServer
```

```csharp
// Program.cs
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer();

var app = builder.Build();

// Hangfire 대시보드 (개발 환경에서만)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}
```

**Fire-and-Forget: 즉시 실행**

```csharp
// 즉시 백그라운드에서 실행
BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget job!"));

// 실제 사용 예시
BackgroundJob.Enqueue<IEmailService>(x => x.SendWelcomeEmailAsync(userId));
```

**Delayed: 지연 실행**

```csharp
// 30분 후 실행
BackgroundJob.Schedule<IEmailService>(
    x => x.SendReminderEmailAsync(userId),
    TimeSpan.FromMinutes(30));

// 특정 시각에 실행
BackgroundJob.Schedule<IReportService>(
    x => x.GenerateMonthlyReportAsync(),
    DateTimeOffset.UtcNow.AddDays(1).Date); // 다음 자정
```

**Recurring: 반복 실행**

```csharp
// 매일 자정에 실행 (Cron 표현식)
RecurringJob.AddOrUpdate<IReportService>(
    "daily-report",
    x => x.GenerateDailyReportAsync(),
    Cron.Daily(0, 0)); // 매일 00:00

// 매 시간 실행
RecurringJob.AddOrUpdate<IDataSyncService>(
    "hourly-sync",
    x => x.SyncDataAsync(),
    Cron.Hourly());

// 매주 월요일 9시
RecurringJob.AddOrUpdate<IMaintenanceService>(
    "weekly-maintenance",
    x => x.PerformMaintenanceAsync(),
    Cron.Weekly(DayOfWeek.Monday, 9));

// 커스텀 Cron 표현식
RecurringJob.AddOrUpdate<IBackupService>(
    "backup",
    x => x.BackupDatabaseAsync(),
    "0 2 * * *"); // 매일 02:00
```

**Continuation: 작업 체인**

```csharp
// 첫 번째 작업
var jobId = BackgroundJob.Enqueue<IImageService>(x => x.UploadImageAsync(fileName));

// 첫 번째 작업이 완료된 후 실행
BackgroundJob.ContinueJobWith<IImageService>(
    jobId,
    x => x.GenerateThumbnailAsync(fileName));

// 복잡한 체인
var upload = BackgroundJob.Enqueue<IImageService>(x => x.UploadImageAsync(fileName));
var resize = BackgroundJob.ContinueJobWith<IImageService>(upload, x => x.ResizeImageAsync(fileName));
var watermark = BackgroundJob.ContinueJobWith<IImageService>(resize, x => x.AddWatermarkAsync(fileName));
BackgroundJob.ContinueJobWith<INotificationService>(watermark, x => x.NotifyUserAsync(userId, "Image processed"));
```

**재시도와 에러 처리**

Hangfire는 실패한 작업을 자동으로 재시도합니다. 기본적으로 10번까지 재시도하며, 지수 백오프를 사용합니다.

```csharp
public class EmailService : IEmailService
{
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // 이메일 전송 로직
        // 예외 발생 시 자동으로 재시도
    }

    [AutomaticRetry(Attempts = 0)] // 재시도 안 함
    public async Task SendCriticalEmailAsync(string to, string subject, string body)
    {
        // 재시도하면 안 되는 작업 (예: 결제 알림)
    }
}
```

**Hangfire 대시보드**

Hangfire는 웹 기반 대시보드를 제공하여, 모든 작업의 상태를 실시간으로 모니터링할 수 있습니다.

- **Enqueued**: 대기 중인 작업
- **Processing**: 현재 실행 중인 작업
- **Succeeded**: 성공한 작업
- **Failed**: 실패한 작업 (재시도 대기)
- **Deleted**: 삭제된 작업
- **Scheduled**: 예약된 작업
- **Recurring**: 반복 작업

각 작업의 실행 기록, 파라미터, 예외 메시지를 확인할 수 있으며, 수동으로 재시도하거나 삭제할 수도 있습니다.

### 메시지 큐: 마이크로서비스 간 통신

마이크로서비스 아키텍처에서 서비스 간 통신은 어떻게 해야 할까요? 직접 HTTP 호출은 강한 결합을 만듭니다. 서비스 A가 서비스 B를 호출하려면 B의 URL을 알아야 하고, B가 실행 중이어야 하며, 네트워크가 연결되어 있어야 합니다. B가 느리면 A도 느려지고, B가 다운되면 A도 실패합니다.

**메시지 큐**는 이를 해결합니다. 서비스 A는 메시지를 큐에 발행하고, 서비스 B는 큐에서 메시지를 구독합니다. 둘은 서로를 알 필요가 없으며, 시간적으로도 분리됩니다. B가 잠시 다운되어도 메시지는 큐에 남아 있다가, B가 복구되면 처리됩니다.

**RabbitMQ 통합**

```bash
dotnet add package RabbitMQ.Client
```

```csharp
public interface IMessageBus
{
    void Publish<T>(string exchange, string routingKey, T message);
    void Subscribe<T>(string exchange, string routingKey, Func<T, Task> handler);
}

public class RabbitMQMessageBus : IMessageBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQMessageBus(string hostname)
    {
        var factory = new ConnectionFactory() { HostName = hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Publish<T>(string exchange, string routingKey, T message)
    {
        _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(exchange, routingKey, null, body);
    }

    public void Subscribe<T>(string exchange, string routingKey, Func<T, Task> handler)
    {
        _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queueName, exchange, routingKey);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<T>(json);

            if (message != null)
            {
                await handler(message);
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queueName, autoAck: false, consumer);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
```

**이벤트 기반 주문 시스템**

```csharp
// 주문 생성 이벤트
public record OrderCreatedEvent(int OrderId, int UserId, decimal TotalAmount);

// 주문 서비스
public class OrderService
{
    private readonly AppDbContext _context;
    private readonly IMessageBus _messageBus;

    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        var order = new Order
        {
            UserId = dto.UserId,
            TotalAmount = dto.TotalAmount,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // 이벤트 발행
        _messageBus.Publish("orders", "order.created", new OrderCreatedEvent(
            order.Id,
            order.UserId,
            order.TotalAmount));

        return order;
    }
}

// 이메일 서비스 (별도 프로세스/서버)
public class EmailWorker : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IEmailService _emailService;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageBus.Subscribe<OrderCreatedEvent>("orders", "order.created", async evt =>
        {
            await _emailService.SendOrderConfirmationAsync(evt.UserId, evt.OrderId);
        });

        return Task.CompletedTask;
    }
}

// 재고 서비스 (별도 프로세스/서버)
public class InventoryWorker : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IInventoryService _inventoryService;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageBus.Subscribe<OrderCreatedEvent>("orders", "order.created", async evt =>
        {
            await _inventoryService.ReserveStockAsync(evt.OrderId);
        });

        return Task.CompletedTask;
    }
}
```

이제 주문이 생성되면, 이메일 발송과 재고 예약이 독립적으로 처리됩니다. 이메일 서버가 다운되어도 주문은 정상적으로 생성되며, 이메일은 서버가 복구된 후 발송됩니다.

## 핵심 교훈

1. **실시간 통신**: 폴링 < 롱 폴링 < SSE < WebSocket < SignalR
2. **백그라운드 처리**: 긴 작업은 HTTP 요청에서 분리
3. **작업 큐**: 즉시 응답, 나중에 처리
4. **Hangfire**: 영속화, 재시도, 스케줄링, 대시보드
5. **메시지 큐**: 느슨한 결합, 확장 가능한 마이크로서비스

프로덕션 환경에서 견고한 시스템을 만들려면, 이 모든 패턴을 적재적소에 사용해야 합니다. 실시간 알림은 SignalR로, 긴 작업은 Hangfire로, 서비스 간 통신은 메시지 큐로—각각의 강점을 이해하고 활용하는 것이 진정한 아키텍트의 역량입니다.

다음 챕터에서는 타입 안전한 API 클라이언트 생성과 캐싱 전략을 배웁니다. 서버와 클라이언트의 완벽한 통합으로, 프론트엔드와 백엔드의 경계가 사라지는 경험을 하게 될 것입니다.
