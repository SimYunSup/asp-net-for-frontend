// LINQ 연습 문제 - 아래 TODO를 완성하세요

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public required string Category { get; set; }
}

public class Student
{
    public required string Name { get; set; }
    public required string Subject { get; set; }
    public int Grade { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class LinqExercises
{
    // TODO 1: 카테고리별 총 가격 계산
    // 힌트: GroupBy + Sum
    public Dictionary<string, decimal> GetTotalPriceByCategory(List<Product> products)
    {
        throw new NotImplementedException();
    }

    // TODO 2: 과목별 최고 점수 학생 찾기
    // 힌트: GroupBy + OrderByDescending + First
    public Dictionary<string, Student> GetTopStudentBySubject(List<Student> students)
    {
        throw new NotImplementedException();
    }

    // TODO 3: 고객별 총 주문 금액 계산 및 정렬
    // 힌트: GroupBy + Sum + OrderByDescending
    public List<CustomerOrderSummary> GetCustomerOrderSummary(
        List<Customer> customers,
        List<Order> orders)
    {
        throw new NotImplementedException();
    }

    // TODO 4: 복잡한 쿼리
    // 2000원 이상 구매한 고객의 이름과 주문 수를 주문 수 내림차순으로
    public List<CustomerOrderCount> GetHighValueCustomers(
        List<Customer> customers,
        List<Order> orders,
        decimal minTotalAmount)
    {
        throw new NotImplementedException();
    }
}

public class CustomerOrderSummary
{
    public required string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public int OrderCount { get; set; }
}

public class CustomerOrderCount
{
    public required string CustomerName { get; set; }
    public int OrderCount { get; set; }
}

// ========== 해답 (아래로 스크롤하지 마세요!) ==========

/*

// TODO 1: 카테고리별 총 가격 계산
public Dictionary<string, decimal> GetTotalPriceByCategory(List<Product> products)
{
    return products
        .GroupBy(p => p.Category)
        .ToDictionary(g => g.Key, g => g.Sum(p => p.Price));

    // 또는 쿼리 구문
    // return (from p in products
    //         group p by p.Category into g
    //         select new { Category = g.Key, Total = g.Sum(p => p.Price) })
    //         .ToDictionary(x => x.Category, x => x.Total);
}

// TODO 2: 과목별 최고 점수 학생 찾기
public Dictionary<string, Student> GetTopStudentBySubject(List<Student> students)
{
    return students
        .GroupBy(s => s.Subject)
        .ToDictionary(
            g => g.Key,
            g => g.OrderByDescending(s => s.Grade).First()
        );

    // 또는
    // return students
    //     .GroupBy(s => s.Subject)
    //     .ToDictionary(g => g.Key, g => g.MaxBy(s => s.Grade)!);
}

// TODO 3: 고객별 총 주문 금액 계산 및 정렬
public List<CustomerOrderSummary> GetCustomerOrderSummary(
    List<Customer> customers,
    List<Order> orders)
{
    return customers
        .GroupJoin(
            orders,
            c => c.Id,
            o => o.CustomerId,
            (customer, customerOrders) => new CustomerOrderSummary
            {
                CustomerName = customer.Name,
                TotalAmount = customerOrders.Sum(o => o.Amount),
                OrderCount = customerOrders.Count()
            })
        .OrderByDescending(x => x.TotalAmount)
        .ToList();

    // 또는 쿼리 구문
    // return (from c in customers
    //         join o in orders on c.Id equals o.CustomerId into customerOrders
    //         select new CustomerOrderSummary
    //         {
    //             CustomerName = c.Name,
    //             TotalAmount = customerOrders.Sum(o => o.Amount),
    //             OrderCount = customerOrders.Count()
    //         })
    //         .OrderByDescending(x => x.TotalAmount)
    //         .ToList();
}

// TODO 4: 복잡한 쿼리
public List<CustomerOrderCount> GetHighValueCustomers(
    List<Customer> customers,
    List<Order> orders,
    decimal minTotalAmount)
{
    return customers
        .GroupJoin(
            orders,
            c => c.Id,
            o => o.CustomerId,
            (customer, customerOrders) => new
            {
                Customer = customer,
                Orders = customerOrders.ToList()
            })
        .Where(x => x.Orders.Sum(o => o.Amount) >= minTotalAmount)
        .Select(x => new CustomerOrderCount
        {
            CustomerName = x.Customer.Name,
            OrderCount = x.Orders.Count
        })
        .OrderByDescending(x => x.OrderCount)
        .ToList();
}

*/
