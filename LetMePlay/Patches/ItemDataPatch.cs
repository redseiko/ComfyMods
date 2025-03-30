namespace LetMePlay;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ItemDrop.ItemData))]
static class ItemDataPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(ItemDrop.ItemData.GetIcon))]
  static void GetIconPrefix(ItemDrop.ItemData __instance) {
    if (IsModEnabled.Value) {
      ItemDropUtils.ValidateIcons(__instance, __instance.m_variant, __instance.m_shared.m_icons.Length);
    }
  }
}
