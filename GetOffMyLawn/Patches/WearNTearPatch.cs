namespace GetOffMyLawn;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(WearNTear))]
static class WearNTearPatch {
  public static readonly long PieceHealthDamageThreshold = 100_000L;

  [HarmonyPrefix]
  [HarmonyPatch(nameof(WearNTear.ApplyDamage), new[] { typeof(float), typeof(HitData) })]
  static bool ApplyDamagePrefix(WearNTear __instance, float damage, HitData hitData, ref bool __result) {
    if (IsModEnabled.Value && EnablePieceHealthDamageThreshold.Value) {
      __result = ApplyDamageDelegate(__instance, damage, hitData);
      return false;
    }

    return true;
  }

  public static bool ApplyDamageDelegate(WearNTear wearNTear, float damage, HitData hitData) {
    float health = wearNTear.m_nview.m_zdo.GetFloat(ZDOVars.s_health, wearNTear.m_health);

    if (health <= 0f || health >= PieceHealthDamageThreshold) {
      return false;
    }

    wearNTear.m_nview.m_zdo.Set(ZDOVars.s_health, health);

    if (health <= 0f) {
      wearNTear.Destroy(hitData);
    } else {
      ZRoutedRpc.s_instance.InvokeRoutedRPC(
          ZRoutedRpc.Everybody, "RPC_HealthChanged", wearNTear.m_nview.m_zdo.m_uid, health);
    }

    return true;
  }
}
