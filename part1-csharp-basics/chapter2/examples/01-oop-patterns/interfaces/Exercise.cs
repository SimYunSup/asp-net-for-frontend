// 인터페이스 연습 문제 - 아래 TODO를 완성하세요

// TODO 1: ICache<TKey, TValue> 인터페이스와 MemoryCache 구현
public interface ICache<TKey, TValue> where TKey : notnull
{
    // TODO: 메서드 정의
    // - TValue? Get(TKey key)
    // - void Set(TKey key, TValue value)
    // - void Set(TKey key, TValue value, TimeSpan expiration)
    // - bool Remove(TKey key)
    // - void Clear()
    // - bool Contains(TKey key)
}

public class MemoryCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
{
    // TODO: 구현
    // 힌트: Dictionary<TKey, CacheItem<TValue>> 사용
    // CacheItem에는 Value와 ExpirationTime 포함
    throw new NotImplementedException();
}

// TODO 2: Result<T, E> 타입 (제네릭 오류 타입)
public class Result<T, E>
{
    // TODO: Ok/Error 상태, Value, Error 프로퍼티
    // TODO: static 팩토리 메서드 (Ok, Error)
    // TODO: Match 메서드
    throw new NotImplementedException();
}

// TODO 3: 여러 도형 타입 추가
public abstract class Shape
{
    public abstract double CalculateArea();
    public abstract double CalculatePerimeter();
}

// TODO: Circle, Rectangle, Triangle 구현
// TODO: Pentagon, Hexagon 추가

// TODO 4: IValidator<T> 인터페이스와 여러 validator 구현
public interface IValidator<T>
{
    bool IsValid(T value);
    string GetErrorMessage();
}

// TODO: EmailValidator, PasswordValidator, AgeValidator 구현

// ========== 해답 (아래로 스크롤하지 마세요!) ==========

/*

// TODO 1: ICache 구현
public interface ICache<TKey, TValue> where TKey : notnull
{
    TValue? Get(TKey key);
    void Set(TKey key, TValue value);
    void Set(TKey key, TValue value, TimeSpan expiration);
    bool Remove(TKey key);
    void Clear();
    bool Contains(TKey key);
}

public class CacheItem<TValue>
{
    public TValue Value { get; set; }
    public DateTime? ExpirationTime { get; set; }

    public CacheItem(TValue value, DateTime? expirationTime = null)
    {
        Value = value;
        ExpirationTime = expirationTime;
    }

    public bool IsExpired => ExpirationTime.HasValue && DateTime.UtcNow > ExpirationTime;
}

public class MemoryCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, CacheItem<TValue>> _cache = new();

    public TValue? Get(TKey key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (item.IsExpired)
            {
                _cache.Remove(key);
                return default;
            }
            return item.Value;
        }
        return default;
    }

    public void Set(TKey key, TValue value)
    {
        _cache[key] = new CacheItem<TValue>(value);
    }

    public void Set(TKey key, TValue value, TimeSpan expiration)
    {
        _cache[key] = new CacheItem<TValue>(value, DateTime.UtcNow.Add(expiration));
    }

    public bool Remove(TKey key)
    {
        return _cache.Remove(key);
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public bool Contains(TKey key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (item.IsExpired)
            {
                _cache.Remove(key);
                return false;
            }
            return true;
        }
        return false;
    }
}

// TODO 2: Result<T, E> 타입
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
    public static Result<T, E> Err(E error) => new(false, default, error);

    public TResult Match<TResult>(Func<T, TResult> onOk, Func<E, TResult> onError)
    {
        return IsOk ? onOk(Value!) : onError(Error!);
    }

    public void Match(Action<T> onOk, Action<E> onError)
    {
        if (IsOk)
            onOk(Value!);
        else
            onError(Error!);
    }
}

// TODO 3: 도형 추가
public class Circle : Shape
{
    public double Radius { get; init; }

    public Circle(double radius) => Radius = radius;

    public override double CalculateArea() => Math.PI * Radius * Radius;
    public override double CalculatePerimeter() => 2 * Math.PI * Radius;
}

public class Rectangle : Shape
{
    public double Width { get; init; }
    public double Height { get; init; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public override double CalculateArea() => Width * Height;
    public override double CalculatePerimeter() => 2 * (Width + Height);
}

public class Triangle : Shape
{
    public double A { get; init; }
    public double B { get; init; }
    public double C { get; init; }

    public Triangle(double a, double b, double c)
    {
        A = a;
        B = b;
        C = c;
    }

    public override double CalculateArea()
    {
        var s = (A + B + C) / 2;
        return Math.Sqrt(s * (s - A) * (s - B) * (s - C));
    }

    public override double CalculatePerimeter() => A + B + C;
}

public class Pentagon : Shape
{
    public double Side { get; init; }

    public Pentagon(double side) => Side = side;

    public override double CalculateArea() => (Math.Sqrt(25 + 10 * Math.Sqrt(5)) / 4) * Side * Side;
    public override double CalculatePerimeter() => 5 * Side;
}

// TODO 4: Validators
public class EmailValidator : IValidator<string>
{
    public bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Contains('@') && value.Contains('.');
    }

    public string GetErrorMessage() => "Invalid email format";
}

public class PasswordValidator : IValidator<string>
{
    private readonly int _minLength;

    public PasswordValidator(int minLength = 8)
    {
        _minLength = minLength;
    }

    public bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Length >= _minLength &&
               value.Any(char.IsUpper) &&
               value.Any(char.IsLower) &&
               value.Any(char.IsDigit);
    }

    public string GetErrorMessage() =>
        $"Password must be at least {_minLength} characters and contain uppercase, lowercase, and digit";
}

public class AgeValidator : IValidator<int>
{
    private readonly int _minAge;
    private readonly int _maxAge;

    public AgeValidator(int minAge = 0, int maxAge = 150)
    {
        _minAge = minAge;
        _maxAge = maxAge;
    }

    public bool IsValid(int value)
    {
        return value >= _minAge && value <= _maxAge;
    }

    public string GetErrorMessage() => $"Age must be between {_minAge} and {_maxAge}";
}

*/
