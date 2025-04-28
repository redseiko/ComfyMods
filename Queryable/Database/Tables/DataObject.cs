namespace Queryable.Database;

using SQLite;

public sealed class DataObject {
  [PrimaryKey]
  [AutoIncrement]
  public int ObjectId { get; set; }

  public int PrefabHash { get; set; }
  public int PositionX { get; set; }
  public int PositionY { get; set; }
  public int PositionZ { get; set; }
}
