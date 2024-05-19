namespace Atlas;

public static class CustomFieldUtils {
  public static readonly int HasFieldsHashCode = "HasFields".GetStableHashCode();

  public static readonly int HasFieldsFireplaceHashCode = "HasFieldsFireplace".GetStableHashCode();
  public static readonly int FireplaceIgniteChanceHashCode = "Fireplace.m_igniteChance".GetStableHashCode();
  public static readonly int FireplaceIgniteSpreadHashCode = "Fireplace.m_igniteSpread".GetStableHashCode();

  public static void SetFireplaceFields(ZDO zdo, float igniteChance, int igniteSpread) {
    zdo.Set(HasFieldsHashCode, true);
    zdo.Set(HasFieldsFireplaceHashCode, true);
    zdo.Set(FireplaceIgniteChanceHashCode, 0f);
    zdo.Set(FireplaceIgniteSpreadHashCode, 0);
  }

  public static readonly int HasFieldsWearNTearHashCode = "HasFieldsWearNTear".GetStableHashCode();
  public static readonly int WearNTearBurnableHashCode = "WearNTear.m_burnable".GetStableHashCode();

  public static void SetWearNTearFields(ZDO zdo, bool burnable) {
    zdo.Set(HasFieldsHashCode, true);
    zdo.Set(HasFieldsWearNTearHashCode, true);
    zdo.Set(WearNTearBurnableHashCode, burnable);
  }
}
