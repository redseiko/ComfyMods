namespace Queryable.Database;

using SQLite;

public sealed class PrefabHash {
  [PrimaryKey]
  public int Hash { get; set; }

  public string PrefabName { get; set; }
}
