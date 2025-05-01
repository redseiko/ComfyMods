namespace Queryable.Database;

using SQLite;

public sealed class DataObject {
  [PrimaryKey]
  [AutoIncrement]
  public int ObjectId { get; set; }

  // ZDO
  public int PrefabHash { get; set; }
  public int PositionX { get; set; }
  public int PositionY { get; set; }
  public int PositionZ { get; set; }

  // Piece
  public long? CreatorId { get; set; }

  // Atlas
  public long? EpochTimeCreated { get; set; }
  public string? OriginalUid { get; set; }
}
