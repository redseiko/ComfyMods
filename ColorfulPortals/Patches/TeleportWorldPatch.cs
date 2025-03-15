namespace ColorfulPortals;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(TeleportWorld))]
static class TeleportWorldPatch {
  public static readonly int PortalWoodHashCode = "portal_wood".GetStableHashCode();

  [HarmonyPostfix]
  [HarmonyPatch(nameof(TeleportWorld.Awake))]
  static void AwakePostfix(TeleportWorld __instance) {
    if (IsModEnabled.Value && __instance) {
      AddTeleportWorldColor(__instance.m_nview);
    }
  }

  static void AddTeleportWorldColor(ZNetView netView) {
    if (netView
        && netView.IsValid()
        && netView.m_zdo.m_prefab == PortalWoodHashCode
        && !netView.TryGetComponent(out TeleportWorldColor _)) {
      netView.gameObject.AddComponent<TeleportWorldColor>();
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(TeleportWorld.GetHoverText))]
  static void GetHoverTextPostfix(TeleportWorld __instance, ref string __result) {
    if (!IsModEnabled.Value || !ShowChangeColorHoverText.Value || !__instance) {
      return;
    }

    __result =
        string.Format(
            "{0}\n[<color={1}>{2}</color>] Change color to: <color=#{3}>#{3}</color>",
            __result,
            "#FFA726",
            ChangePortalColorShortcut.Value,
            TargetPortalColor.Value.GetColorHtmlString());
  }
}
