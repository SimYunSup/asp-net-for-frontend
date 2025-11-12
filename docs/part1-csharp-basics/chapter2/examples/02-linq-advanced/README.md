# 실습: LINQ 고급 쿼리

## 목표

복잡한 LINQ 쿼리를 작성하는 방법을 학습합니다.

## 예제: 복잡한 데이터 분석

### 시나리오

전자상거래 시스템에서 고객, 주문, 제품 데이터를 분석하는 복잡한 쿼리를 작성합니다.

### 데이터 모델

```csharp
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
```

### 복잡한 쿼리 예제

```csharp
// 1. 고객별 총 주문 금액 (최근 3개월)
var customerSales = from customer in customers
                    join order in orders on customer.Id equals order.CustomerId
                    where order.OrderDate >= DateTime.Now.AddMonths(-3)
                    group order by new { customer.Id, customer.Name } into g
                    select new
                    {
                        g.Key.Name,
                        TotalSales = g.Sum(o => o.TotalAmount),
                        OrderCount = g.Count(),
                        AverageOrderValue = g.Average(o => o.TotalAmount)
                    };

// 또는 메서드 구문
var customerSales2 = customers
    .Join(orders,
          c => c.Id,
          o => o.CustomerId,
          (c, o) => new { Customer = c, Order = o })
    .Where(x => x.Order.OrderDate >= DateTime.Now.AddMonths(-3))
    .GroupBy(x => new { x.Customer.Id, x.Customer.Name })
    .Select(g => new
    {
        g.Key.Name,
        TotalSales = g.Sum(x => x.Order.TotalAmount),
        OrderCount = g.Count(),
        AverageOrderValue = g.Average(x => x.Order.TotalAmount)
    });

// 2. 카테고리별 베스트셀러 제품
var bestSellers = from orderItem in orders.SelectMany(o => o.Items)
                  join product in products on orderItem.ProductId equals product.Id
                  group orderItem by new { product.Category, product.Name } into g
                  orderby g.Sum(i => i.Quantity) descending
                  select new
                  {
                      g.Key.Category,
                      g.Key.Name,
                      TotalQuantity = g.Sum(i => i.Quantity),
                      Revenue = g.Sum(i => i.Price * i.Quantity)
                  };

// 3. 월별 매출 추이
var monthlySales = orders
    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
    .Select(g => new
    {
        Year = g.Key.Year,
        Month = g.Key.Month,
        TotalSales = g.Sum(o => o.TotalAmount),
        OrderCount = g.Count(),
        UniqueCustomers = g.Select(o => o.CustomerId).Distinct().Count()
    })
    .OrderBy(x => x.Year)
    .ThenBy(x => x.Month);

// 4. 고객 세그먼테이션 (RFM 분석)
var customerSegmentation = customers
    .GroupJoin(orders,
               c => c.Id,
               o => o.CustomerId,
               (customer, customerOrders) => new
               {
                   Customer = customer,
                   Orders = customerOrders.ToList()
               })
    .Select(x => new
    {
        x.Customer.Name,
        Recency = x.Orders.Any()
            ? (DateTime.Now - x.Orders.Max(o => o.OrderDate)).Days
            : int.MaxValue,
        Frequency = x.Orders.Count(),
        Monetary = x.Orders.Sum(o => o.TotalAmount),
        Segment = x.Orders.Count() switch
        {
            > 10 => "VIP",
            > 5 => "Regular",
            > 0 => "Occasional",
            _ => "New"
        }
    })
    .OrderByDescending(x => x.Monetary);

// 5. 제품 추천: 함께 구매된 제품 찾기
var productRecommendations = from order in orders
                             from item1 in order.Items
                             from item2 in order.Items
                             where item1.ProductId < item2.ProductId
                             group new { item1.ProductId, item2.ProductId } by
                                   new { Product1 = item1.ProductId, Product2 = item2.ProductId } into g
                             select new
                             {
                                 g.Key.Product1,
                                 g.Key.Product2,
                                 PurchasedTogether = g.Count()
                             }
                             into recommendation
                             orderby recommendation.PurchasedTogether descending
                             select recommendation;
```

### 연습 문제

[Exercise.cs](./Exercise.cs) 파일에서 다음을 구현하세요:

1. 도시별 평균 주문 금액과 고객 수 계산
2. 주문이 없는 고객 찾기 (Left Anti Join)
3. 각 카테고리에서 가장 비싼 제품 3개
4. 전월 대비 매출 증가율 계산
5. 고객 생애 가치(LTV) 계산
