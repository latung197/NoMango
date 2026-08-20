namespace Wcs.Common.ValueObjects;

public sealed class Priority : StringValueObject<Priority>
{
  public string Label { get; }
  public static readonly Priority High = new("high", "Cao");
  public static readonly Priority Middle = new("middle", "Trung");
  public static readonly Priority Low = new("low", "Thấp");

  static Priority()
  {
    // Register all static instances
    Register(High);
    Register(Middle);
    Register(Low);
  }

  private Priority(string value, string label) : base(value)
  {
    Label = label;
  }
}