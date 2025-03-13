﻿namespace Pinnacle;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Game))]
static class GamePatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Game.UpdateNoMap))]
  static IEnumerable<CodeInstruction> UpdateNoMapTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Br),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Minimap), nameof(Minimap.SetMapMode))))
        .ThrowIfInvalid("Could not patch Game.UpdateNoMap()! (set-map-mode)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GamePatch), nameof(SetMapModeDelegate))))
        .InstructionEnumeration();
  }

  static Minimap.MapMode SetMapModeDelegate(Minimap.MapMode mapMode) {
    if (IsModEnabled.Value && !Game.m_noMap && Minimap.m_instance.m_mode == Minimap.MapMode.Large) {
      return Minimap.MapMode.Large;
    }

    return mapMode;
  }
}
