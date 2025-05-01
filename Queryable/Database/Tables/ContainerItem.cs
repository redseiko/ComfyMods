namespace Queryable.Database;

using SQLite;

public sealed class ContainerItem {
  [PrimaryKey]
  [AutoIncrement]
  public int ItemId { get; set; }

  public int ObjectId { get; set; }

  public string Name { get; set; }
  public int Stack { get; set; }
  public float Durability { get; set; }
  public int GridPositionX { get; set; }
  public int GridPositionY { get; set; }
  public bool IsEquipped { get; set; }
  public int Quality { get; set; }
  public int Variant { get; set; }
  public long CrafterId { get; set; }
  public string CrafterName { get; set; }
  public string CustomDataJson { get; set; }
  public int WorldLevel { get; set; }
  public bool IsPickedUp { get; set; }
}
