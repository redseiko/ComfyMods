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
    Player player = __instance.m_playerInstance.GetComponent<Player>();
    DyeManager.SetLocalPlayer(player);

    if (IsModEnabled.Value && player) {
      DyeManager.SetPlayerZDOHairColor(player);
      DyeManager.SetPlayerHairItem(player);
      DyeManager.SetPlayerBeardItem(player);
      DyeManager.SetCharacterPreviewPosition();
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.ShowCharacterSelection))]
  static void ShowCharacterSelectionPostfix() {
    DyeManager.SetCharacterPreviewPosition();
  }
}
