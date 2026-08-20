# String Value Object Pattern

## Tổng quan

`StringValueObject<T>` là abstract base class để tạo type-safe value objects với các constants được định nghĩa trước, map đến string trong database.

## Mục đích

- ✅ Type-safe constants (thay vì string literals)
- ✅ Tự động validation
- ✅ Dễ dàng extend
- ✅ EF Core integration (tự động convert sang string trong DB)

## Cách sử dụng

### 1. Tạo Value Object mới

```csharp
namespace Wcs.Common.ValueObjects;

public sealed class ProductType : StringValueObject<ProductType>
{
    // Định nghĩa các constants
    public static readonly ProductType Electronic = new("Electronic");
    public static readonly ProductType Clothing = new("Clothing");
    public static readonly ProductType Food = new("Food");

    // Static constructor để register các giá trị
    static ProductType()
    {
        Register("Electronic", Electronic);
        Register("Clothing", Clothing);
        Register("Food", Food);
    }

    // Private constructor
    private ProductType(string value) : base(value)
    {
    }
}
```

### 2. Sử dụng trong Entity/DbModel

```csharp
public class ProductDbModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProductType Type { get; set; } = ProductType.Electronic;
}
```

### 3. Cấu hình EF Core

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<ProductDbModel>(entity =>
    {
        entity.Property(p => p.Type)
            .HasConversion(
                v => v.Value,                        // Convert to string (for DB)
                v => ProductType.FromString(v));     // Convert from string (from DB)
    });
}
```

## API của StringValueObject

### Static Methods

#### `FromString(string value)`
Convert string sang value object instance. Throw exception nếu không tìm thấy.

```csharp
var productType = ProductType.FromString("Electronic"); // Returns ProductType.Electronic
var invalid = ProductType.FromString("Invalid");       // Throws ArgumentException
```

#### `TryFromString(string? value, out T? instance)`
Try convert string sang value object. Return false nếu không tìm thấy.

```csharp
if (ProductType.TryFromString("Electronic", out var type))
{
    // Use type
}
```

#### `GetAll()`
Lấy tất cả registered values.

```csharp
var allTypes = ProductType.GetAll(); // Returns IEnumerable<ProductType>
```

### Instance Properties/Methods

#### `Value`
String value của instance.

```csharp
var type = ProductType.Electronic;
string value = type.Value; // "Electronic"
```

#### `ToString()`
Return string value.

```csharp
var type = ProductType.Electronic;
string str = type.ToString(); // "Electronic"
```

### Operators

#### `==` và `!=`
So sánh equality dựa trên Value.

```csharp
var type1 = ProductType.Electronic;
var type2 = ProductType.FromString("Electronic");
bool equal = type1 == type2; // true
```

## Ví dụ thực tế: StageArea

```csharp
public sealed class StageArea : StringValueObject<StageArea>
{
    public static readonly StageArea Chip = new("Chip");
    public static readonly StageArea Large = new("Large");
    public static readonly StageArea Waste = new("Waste");

    static StageArea()
    {
        Register("Chip", Chip);
        Register("Large", Large);
        Register("Waste", Waste);
    }

    private StageArea(string value) : base(value)
    {
    }
}

// Usage
var stage = new Stage("STAGE_01", StageArea.Chip);
var area = StageArea.FromString("Large");
```

## Lợi ích so với Enum

| Feature | Enum | StringValueObject |
|---------|------|-------------------|
| Type safety | ✅ | ✅ |
| IntelliSense | ✅ | ✅ |
| Extensibility | ❌ (khó extend) | ✅ (dễ extend) |
| Methods/Logic | ⚠️ (limited) | ✅ (full class) |
| Validation | ❌ | ✅ (FromString validates) |
| Database storage | Int/String | String (flexible) |

## Khi nào nên dùng?

✅ **Nên dùng StringValueObject khi:**
- Cần validate values
- Có thể cần extend trong tương lai
- Cần business logic trong value object
- Cần linh hoạt về database storage

❌ **Nên dùng Enum khi:**
- Values cố định, không thay đổi
- Không cần validation phức tạp
- Performance critical (enum nhanh hơn một chút)

## Best Practices

1. **Always use static constructor** để register values
2. **Private constructor** để prevent instantiation bên ngoài
3. **Sealed class** để prevent further inheritance (trừ khi thực sự cần)
4. **Use `FromString`** trong EF Core converters
5. **Use constants** trong code (không dùng string literals)

## Migration từ String literals

**Trước:**
```csharp
public string Area { get; set; } = "Chip"; // Không type-safe
```

**Sau:**
```csharp
public StageArea Area { get; set; } = StageArea.Chip; // Type-safe
```

## Extension Methods cho EF Core (Optional)

Có thể tạo extension method để simplify EF Core configuration:

```csharp
public static class ModelBuilderExtensions
{
    public static PropertyBuilder<T> HasStringValueObjectConversion<T>(
        this PropertyBuilder<T> propertyBuilder)
        where T : StringValueObject<T>
    {
        return propertyBuilder.HasConversion(
            v => v.Value,
            v => T.FromString(v));
    }
}

// Usage
entity.Property(p => p.Type).HasStringValueObjectConversion<ProductType>();
```
