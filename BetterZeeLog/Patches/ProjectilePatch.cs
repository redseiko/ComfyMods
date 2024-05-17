namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Projectile))]
static class ProjectilePatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Projectile.FixedUpdate))]
  static IEnumerable<CodeInstruction> FixedUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    if (CheckProjectFixedUpdateZeroVelocity.Value) {
      return new CodeMatcher(instructions)
          .Start()
          .MatchStartForward(
              new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Projectile), nameof(Projectile.m_vel))),
              new CodeMatch(
                  OpCodes.Call,
                  AccessTools.Method(typeof(Quaternion), nameof(Quaternion.LookRotation), [typeof(Vector3)])),
              new CodeMatch(
                  OpCodes.Callvirt, AccessTools.PropertySetter(typeof(Transform), nameof(Transform.rotation))))
          .ThrowIfInvalid($"Could not patch Projectile.FixedUpdate()! (LookRotation)")
          .Advance(offset: 1)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate(LookRotationDelegate))
          .InstructionEnumeration();
    }

    return instructions;
  }

  static Quaternion LookRotationDelegate(Vector3 velocity) {
    if (velocity == Vector3.zero) {
      return Quaternion.identity;
    }

    return Quaternion.LookRotation(velocity);
  }
}
