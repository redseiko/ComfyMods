namespace DyeHard;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.SetupObjectDB))]
  static void SetupObjectDbPostfix(FejdStartup __instance) {
    BindCustomizationConfig(
        __instance.GetComponent<ObjectDB>(),
        __instance.m_newCharacterPanel.GetComponent<PlayerCustomizaton>());
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.SetupCharacterPreview))]
  static void SetupCharacterPreviewPostfix(FejdStartup __instance) {
    DyeManager.SetLocalPlayer(__instance.m_playerInstance.GetComponent<Player>());

    if (IsModEnabled.Value) {
      DyeManager.SetPlayerZDOHairColor();
      DyeManager.SetPlayerHairItem();
      DyeManager.SetPlayerBeardItem();
      DyeManager.SetCharacterPreviewPosition();
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.ShowCharacterSelection))]
  static void ShowCharacterSelectionPostfix() {
    DyeManager.SetCharacterPreviewPosition();
  }
}
