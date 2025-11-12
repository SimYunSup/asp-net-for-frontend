# 실습: 이벤트와 델리게이트

## 목표

C#의 이벤트 시스템을 이해하고 실전에서 활용하는 방법을 학습합니다.

## 예제 1: 기본 이벤트 패턴

### 시나리오: 주문 처리 시스템

```csharp
// 이벤트 인자 정의
public class OrderEventArgs : EventArgs
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
}

// 발행자 (Publisher)
public class OrderService
{
    // 이벤트 선언
    public event EventHandler<OrderEventArgs>? OrderPlaced;
    public event EventHandler<OrderEventArgs>? OrderCancelled;
    public event EventHandler<OrderEventArgs>? OrderShipped;

    public void PlaceOrder(int orderId, decimal amount)
    {
        // 비즈니스 로직
        Console.WriteLine($"Processing order {orderId}...");

        // 이벤트 발생
        OnOrderPlaced(new OrderEventArgs
        {
            OrderId = orderId,
            Amount = amount,
            OrderDate = DateTime.Now
        });
    }

    public void CancelOrder(int orderId)
    {
        OnOrderCancelled(new OrderEventArgs
        {
            OrderId = orderId,
            OrderDate = DateTime.Now
        });
    }

    protected virtual void OnOrderPlaced(OrderEventArgs e)
    {
        OrderPlaced?.Invoke(this, e);
    }

    protected virtual void OnOrderCancelled(OrderEventArgs e)
    {
        OrderCancelled?.Invoke(this, e);
    }

    protected virtual void OnOrderShipped(OrderEventArgs e)
    {
        OrderShipped?.Invoke(this, e);
    }
}

// 구독자 (Subscriber)
public class EmailNotificationService
{
    public void Subscribe(OrderService orderService)
    {
        orderService.OrderPlaced += OnOrderPlaced;
        orderService.OrderCancelled += OnOrderCancelled;
        orderService.OrderShipped += OnOrderShipped;
    }

    public void Unsubscribe(OrderService orderService)
    {
        orderService.OrderPlaced -= OnOrderPlaced;
        orderService.OrderCancelled -= OnOrderCancelled;
        orderService.OrderShipped -= OnOrderShipped;
    }

    private void OnOrderPlaced(object? sender, OrderEventArgs e)
    {
        Console.WriteLine($"[Email] Order {e.OrderId} placed. Sending confirmation...");
    }

    private void OnOrderCancelled(object? sender, OrderEventArgs e)
    {
        Console.WriteLine($"[Email] Order {e.OrderId} cancelled. Notifying customer...");
    }

    private void OnOrderShipped(object? sender, OrderEventArgs e)
    {
        Console.WriteLine($"[Email] Order {e.OrderId} shipped. Sending tracking info...");
    }
}

// 사용
var orderService = new OrderService();
var emailService = new EmailNotificationService();

emailService.Subscribe(orderService);

orderService.PlaceOrder(123, 99.99m);
// Output: Processing order 123...
//         [Email] Order 123 placed. Sending confirmation...
```

## 예제 2: 델리게이트 체인

```csharp
// 델리게이트 정의
public delegate void ProcessDataDelegate(string data);

public class DataProcessor
{
    public void ProcessData(string data, ProcessDataDelegate processor)
    {
        Console.WriteLine($"Processing: {data}");
        processor?.Invoke(data);
    }
}

// 사용
var processor = new DataProcessor();

ProcessDataDelegate chain = null;

// 델리게이트 체인 구성
chain += data => Console.WriteLine($"Step 1: Validate {data}");
chain += data => Console.WriteLine($"Step 2: Transform {data}");
chain += data => Console.WriteLine($"Step 3: Save {data}");

processor.ProcessData("User Data", chain);
// Output:
// Processing: User Data
// Step 1: Validate User Data
// Step 2: Transform User Data
// Step 3: Save User Data
```

## 예제 3: 커스텀 이벤트 접근자

```csharp
public class AdvancedEventPublisher
{
    private EventHandler<string>? _dataChanged;

    // 커스텀 이벤트 접근자
    public event EventHandler<string> DataChanged
    {
        add
        {
            Console.WriteLine("Subscriber added");
            _dataChanged += value;
        }
        remove
        {
            Console.WriteLine("Subscriber removed");
            _dataChanged -= value;
        }
    }

    public void ChangeData(string newData)
    {
        _dataChanged?.Invoke(this, newData);
    }
}
```

## 예제 4: 약한 이벤트 패턴 (메모리 누수 방지)

```csharp
public class WeakEventManager<TEventArgs> where TEventArgs : EventArgs
{
    private readonly List<WeakReference> _listeners = new();

    public void AddListener(EventHandler<TEventArgs> listener)
    {
        _listeners.Add(new WeakReference(listener));
    }

    public void RemoveListener(EventHandler<TEventArgs> listener)
    {
        _listeners.RemoveAll(wr =>
        {
            var target = wr.Target as EventHandler<TEventArgs>;
            return target == null || target == listener;
        });
    }

    public void RaiseEvent(object sender, TEventArgs args)
    {
        var deadRefs = new List<WeakReference>();

        foreach (var weakRef in _listeners)
        {
            if (weakRef.Target is EventHandler<TEventArgs> handler)
            {
                handler(sender, args);
            }
            else
            {
                deadRefs.Add(weakRef);
            }
        }

        // 죽은 참조 제거
        foreach (var deadRef in deadRefs)
        {
            _listeners.Remove(deadRef);
        }
    }
}
```

## 예제 5: 비동기 이벤트 핸들러

```csharp
public class AsyncEventPublisher
{
    public event Func<object, EventArgs, Task>? AsyncEvent;

    public async Task RaiseEventAsync(EventArgs args)
    {
        var handlers = AsyncEvent;
        if (handlers == null) return;

        // 모든 핸들러를 병렬로 실행
        var tasks = handlers
            .GetInvocationList()
            .Cast<Func<object, EventArgs, Task>>()
            .Select(handler => handler(this, args));

        await Task.WhenAll(tasks);
    }
}

// 사용
var publisher = new AsyncEventPublisher();

publisher.AsyncEvent += async (sender, args) =>
{
    await Task.Delay(1000);
    Console.WriteLine("Handler 1 completed");
};

publisher.AsyncEvent += async (sender, args) =>
{
    await Task.Delay(500);
    Console.WriteLine("Handler 2 completed");
};

await publisher.RaiseEventAsync(EventArgs.Empty);
```

## React와 비교

### React의 이벤트 시스템

```typescript
// React
import { useState } from 'react';

function OrderComponent() {
  const [orders, setOrders] = useState([]);

  const handleOrderPlaced = (order) => {
    console.log('Order placed:', order);
    setOrders([...orders, order]);
  };

  return <OrderForm onOrderPlaced={handleOrderPlaced} />;
}
```

### C#의 이벤트 시스템

```csharp
// C#
public class OrderComponent
{
    private readonly List<Order> _orders = new();
    private readonly OrderService _orderService;

    public OrderComponent(OrderService orderService)
    {
        _orderService = orderService;
        _orderService.OrderPlaced += HandleOrderPlaced;
    }

    private void HandleOrderPlaced(object? sender, OrderEventArgs e)
    {
        Console.WriteLine($"Order placed: {e.OrderId}");
        _orders.Add(new Order { Id = e.OrderId, Amount = e.Amount });
    }
}
```

## 연습 문제

[Exercise.cs](./Exercise.cs) 파일에서 다음을 구현하세요:

1. 파일 업로드 진행 상태를 알리는 이벤트 시스템
2. 여러 단계의 승인 프로세스 (이벤트 체인)
3. 실시간 채팅 메시지 시스템 (델리게이트 사용)
4. 주식 가격 변동 알림 시스템
