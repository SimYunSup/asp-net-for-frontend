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
