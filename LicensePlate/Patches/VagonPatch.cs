namespace LicensePlate;

using HarmonyLib;

using System;

using static PluginConfig;

[HarmonyPatch(typeof(Vagon))]
static class VagonPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Vagon.Awake))]
  static void AwakePostfix(ref Vagon __instance) {
    if (IsModEnabled.Value && ShowCartNames.Value) {
      __instance.gameObject.AddComponent<VagonName>();
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Vagon.Interact))]
  static bool InteractPrefix(ref Vagon __instance, ref bool __result, bool hold, bool alt) {
    if (alt
        && IsModEnabled.Value
        && ShowCartNames.Value
        && PrivateArea.CheckAccess(__instance.transform.position)
        && __instance.m_nview
        && __instance.m_nview.IsValid()
        && __instance.m_nview.IsOwner()
        && __instance.TryGetComponent(out VagonName vagonName)) {
      TextInput.m_instance.RequestText(vagonName, "$hud_rename", 64);

      __result = true;
      return false;
    }

    return true;
  }

  static readonly Lazy<string> _renameText =
      new(() =>
          Localization.m_instance.Localize("\n[<color=yellow><b>$KEY_AltPlace + $KEY_Use</b></color>] $hud_rename"));

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Vagon.GetHoverText))]
  static void GetHoverTextPostfix(ref Vagon __instance, ref string __result) {
    if (IsModEnabled.Value && ShowCartNames.Value && __instance.m_nview && __instance.m_nview.IsValid()) {
      __result += _renameText.Value;
    }
  }
}
