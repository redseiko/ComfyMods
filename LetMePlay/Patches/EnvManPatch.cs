namespace LetMePlay;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using ComfyLib;

using static PluginConfig;

[HarmonyPatch(typeof(EnvMan))]
static class EnvManPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(EnvMan.SetEnv))]
  static IEnumerable<CodeInstruction> SetEnvTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            new(OpCodes.Ldfld, AccessTools.Field(typeof(EnvSetup), nameof(EnvSetup.m_psystems))),
            new(OpCodes.Ldc_I4_1),
            new(OpCodes.Call, AccessTools.Method(typeof(EnvMan), nameof(EnvMan.SetParticleArrayEnabled))))
        .ThrowIfInvalid($"Could not patch EnvMan.SetEnv()! (set-particle-array-enabled-true")
        .ExtractLabels(out List<Label> setLabels)
        .CreateLabelOffset(offset: 5, out Label skipLabel)
        .Insert(
            new(OpCodes.Ldarg_1),
            new(OpCodes.Call, AccessTools.Method(typeof(EnvManPatch), nameof(SetEnvDelegate))),
            new(OpCodes.Brfalse, skipLabel))
        .AddLabels(setLabels)
        .InstructionEnumeration();
  }

  static bool SetEnvDelegate(EnvSetup envSetup) {
    if (IsModEnabled.Value) {
      if (DisableWeatherSnowParticles.Value && EnvManagerUtils.IsSnowWeather(envSetup.m_name)) {
        return false;
      }

      if (DisableWeatherAshParticles.Value && EnvManagerUtils.IsAshWeather(envSetup.m_name)) {
        return false;
      }
    }

    return true;
  }
}
