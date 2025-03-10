namespace Atlas;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZoneSystem))]
static class ZoneSystemPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZoneSystem.GenerateLocationsIfNeeded))]
  static bool GenerateLocationsIfNeededPrefix() {
    if (IgnoreGenerateLocationsIfNeeded.Value) {
      return false;
    }

    return true;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZoneSystem.Load))]
  static IEnumerable<CodeInstruction> LoadTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZoneSystem), nameof(ZoneSystem.m_locationVersion))))
        .ThrowIfInvalid("Could not patch ZoneSystem.Load()! (location-version)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(ZoneSystemPatch), nameof(CheckLocationVersionDelegate))))
        .InstructionEnumeration();
  }

  static int CheckLocationVersionDelegate(int locationVersion, ZoneSystem zoneSystem) {
    if (IgnoreLocationVersion.Value) {
      PluginLogger.LogInfo(
          $"File locationVersion is: {locationVersion}, overriding to: {zoneSystem.m_locationVersion}");

      return zoneSystem.m_locationVersion;
    }

    return locationVersion;
  }
}
