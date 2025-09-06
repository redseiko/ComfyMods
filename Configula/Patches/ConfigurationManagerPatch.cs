namespace Configula;

using System.Collections.Generic;
using System.Reflection.Emit;

using ConfigurationManager;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(ConfigurationManager))]
static class ConfigurationManagerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ConfigurationManager.CalculateWindowRect))]
  static IEnumerable<CodeInstruction> CalculateWindowRectTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Screen), "get_width")),
            new CodeMatch(OpCodes.Ldc_I4))
        .ThrowIfInvalid("Could not patch CalculateWindowRect() width value!")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(ConfigurationManagerPatch), nameof(GetWidthDelegate))))
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Screen), "get_height")),
            new CodeMatch(OpCodes.Ldc_I4_S),
            new CodeMatch(OpCodes.Sub))
        .ThrowIfInvalid("Could not patch CalculateWindowRect() height value!")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(ConfigurationManagerPatch), nameof(GetHeightDelegate))))
        .InstructionEnumeration();
  }

  static int GetWidthDelegate(int width) {
    return IsModEnabled.Value ? WindowWidth.Value : width;
  }

  static int GetHeightDelegate(int heightOffset) {
    return IsModEnabled.Value ? WindowHeightOffset.Value : heightOffset;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ConfigurationManager.DrawTooltip))]
  static bool DrawTooltipPrefix(Rect area) {
    if (IsModEnabled.Value && UseAlternateDrawTooltip.Value) {
      ConfigulaManager.DrawTooltip(area);
      return false;
    }

    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ConfigurationManager.OnGUI))]
  static void OnGUIPrefix() {
    if (IsModEnabled.Value && Event.current.type == EventType.Repaint) {
      GUI.tooltip = string.Empty;
    }
  }
}
