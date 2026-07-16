namespace PotteryBarn;

public static class CustomFieldUtils {
  public const int HasFieldsHash = -310439593;                        // HasFields
  public const int HasFieldsDestructibleHash = 949312639;             // HasFieldsDestructible
  public const int DestructibleSpawnWhenDestroyedHash = -1786945712;  // Destructible.m_spawnWhenDestroyed

  public static void SetDestructibleFields(Destructible destructible, string spawnWhenDestroyed) {
    ZDO zdo = destructible.m_nview.m_zdo;

    zdo.Set(HasFieldsHash, true);
    zdo.Set(HasFieldsDestructibleHash, true);
    zdo.Set(DestructibleSpawnWhenDestroyedHash, spawnWhenDestroyed);

    destructible.m_nview.LoadFields();
  }
}
