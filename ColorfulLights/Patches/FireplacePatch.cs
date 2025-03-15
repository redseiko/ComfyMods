namespace ColorfulLights;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Fireplace))]
static class FireplacePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Fireplace.Awake))]
  static void AwakePostfix(Fireplace __instance) {
    if (IsModEnabled.Value && __instance) {
      AddFireplaceColor(__instance.m_nview);
    }
  }

  static void AddFireplaceColor(ZNetView netView) {
    if (netView && netView.IsValid() && !netView.TryGetComponent(out FireplaceColor _)) {
      netView.gameObject.AddComponent<FireplaceColor>();
    }
  }

  static readonly string _changeColorHoverTextTemplate =
      "{0}\n<size={4}>[<color={1}>{2}</color>] Change fire color to: <color=#{3}>#{3}</color></size>";

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Fireplace.GetHoverText))]
  static void GetHoverTextPostfix(Fireplace __instance, ref string __result) {
    if (!IsModEnabled.Value || !ShowChangeColorHoverText.Value || !__instance) {
      return;
    }

    __result =
        Localization.instance.Localize(
            string.Format(
                _changeColorHoverTextTemplate,
                __result,
                "#FFA726",
                ChangeColorActionShortcut.Value,
                TargetFireplaceColor.Value.GetColorHtmlString(),
                ColorPromptFontSize.Value));
  }
}
