using HarmonyLib;

using ComfyLib;

using static ComfySigns.ComfySigns;
using static ComfySigns.PluginConfig;

namespace ComfySigns {
  [HarmonyPatch(typeof(ZoneSystem))]
  static class ZoneSystemPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZoneSystem.Awake))]
    static void AwakePostfix(ZoneSystem __instance) {
      if (!UseFallbackFonts.Value) {
        return;
      }

      //AddFallbackFont(UIFonts.GetFontAsset(SignDefaultTextFont.Value));
    }
  }
}
