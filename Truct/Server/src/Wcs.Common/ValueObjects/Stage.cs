namespace Wcs.Common.ValueObjects;

public sealed class StageArea : StringValueObject<StageArea>
{
  public string Label { get; }
  public static readonly StageArea Chip = new("Chip", "Chip");
  public static readonly StageArea Large = new("Large", "Large");
  public static readonly StageArea Waste = new("Waste", "Waste");
  public static readonly StageArea Material = new("Material", "Material");
  public static readonly StageArea Stock = new("Stock", "Stock");

  static StageArea()
  {
    // Register all static instances
    Register(Chip);
    Register(Large);
    Register(Waste);
    Register(Material);
    Register(Stock);
  }

  private StageArea(string value, string label) : base(value)
  {
    Label = label;
  }
}