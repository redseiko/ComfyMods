namespace Queryable.Database;

using SQLite;

public sealed class ItemDropItem {
  [PrimaryKey]
  [AutoIncrement]
  public int ItemId { get; set; }

  public int ObjectId { get; set; }
  public int? ItemDropId { get; set; }
  public int? ItemStandId { get; set; }
  public int? ArmorStandId { get; set; }

  public string? Name { get; set; }
  public float? Durability { get; set; }
  public int? Stack { get; set; }
  public int? Quality { get; set; }
  public int? Variant { get; set; }
  public long? CrafterId { get; set; }
  public string? CrafterName { get; set; }
  public string? CustomDataJson { get; set; }
  public int? WorldLevel { get; set; }
  public bool? IsPickedUp { get; set; }

  public bool IsNull() {
    return
        Durability == null
        && Stack == null
        && Quality == null
        && Variant == null
        && CrafterId == null
        && CrafterName == null
        && CustomDataJson == null
        && WorldLevel == null
        && IsPickedUp == null;
  }
}
