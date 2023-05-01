using HarmonyLib;

using ComfyLib;

using static ComfySigns.ComfySigns;
using static ComfySigns.PluginConfig;

namespace ComfySigns.Patches {
  [HarmonyPatch(typeof(ZoneSystem))]
  public class ZoneSystemPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZoneSystem.Awake))]
    public static void ZoneSystemAwakePostfix(ZoneSystem __instance) {
      if (!UseFallbackFonts.Value) {
        return;
      }
      AddFallbackFont(UIFonts.GetFontAsset(SignDefaultTextFont.Value));
    }
  }
}
