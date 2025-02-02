namespace Entitlement.Database;

using SQLite;

public sealed class PlayerTitle {
  [PrimaryKey]
  [AutoIncrement]
  public int TitleId { get; set; }

  public string TitleName { get; set; }
}
