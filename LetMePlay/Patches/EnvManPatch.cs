using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using static LetMePlay.PluginConfig;

namespace LetMePlay {
  [HarmonyPatch(typeof(EnvMan))]
  public class EnvManPatch {
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(EnvMan.SetEnv))]
    static IEnumerable<CodeInstruction> SetEnvTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Stfld),
              new CodeMatch(OpCodes.Ldarg_1),
              new CodeMatch(OpCodes.Ldfld, typeof(EnvSetup).GetField(nameof(EnvSetup.m_psystems))))
          .Advance(offset: 2)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<EnvSetup, bool>>(SetEnvDelegate))
          .InstructionEnumeration();
    }

    static bool SetEnvDelegate(EnvSetup envSetup) {
      if (IsModEnabled.Value) {
        if (DisableWeatherSnowParticles.Value
            && (envSetup.m_name == "Snow"
                || envSetup.m_name == "SnowStorm"
                || envSetup.m_name == "Twilight_Snow"
                || envSetup.m_name == "Twilight_SnowStorm")) {
          return false;
        }

        if (DisableWeatherAshParticles.Value
            && (envSetup.m_name == "Ashlands_ashrain"
                || envSetup.m_name == "Ashlands_storm"))
        {
          return false;
        }
      }

      return envSetup.m_psystems != null;
    }
  }
}
