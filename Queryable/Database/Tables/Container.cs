namespace Queryable.Database;

using SQLite;

public sealed class Container {
  [PrimaryKey]
  [AutoIncrement]
  public int ContainerId { get; set; }

  public int PrefabHash { get; set; }

  public int PositionX { get; set; }
  public int PositionY { get; set; }
  public int PositionZ { get; set; }

  public long CreatorId { get; set; }
}
