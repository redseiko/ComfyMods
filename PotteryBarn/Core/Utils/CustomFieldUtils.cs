namespace PotteryBarn;

public static class CustomFieldUtils {
  public static readonly int HasFieldsHashCode = "HasFields".GetStableHashCode();
  public static readonly int HasFieldsDestructibleHashCode = "HasFieldsDestructible".GetStableHashCode();
  public static readonly int DestructibleSpawnWhenDestroyedHashCode =
      "Destructible.m_spawnWhenDestroyed".GetStableHashCode();

  public static void SetDestructibleFields(Destructible destructible, string spawnWhenDestroyed) {
    ZDO zdo = destructible.m_nview.m_zdo;
    zdo.Set(HasFieldsHashCode, true);
    zdo.Set(HasFieldsDestructibleHashCode, true);
    zdo.Set(DestructibleSpawnWhenDestroyedHashCode, spawnWhenDestroyed);

    destructible.m_nview.LoadFields();
  }
}
