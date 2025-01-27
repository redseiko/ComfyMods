namespace Instaloot;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Ragdoll))]
static class RagdollPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Ragdoll.Awake))]
  static IEnumerable<CodeInstruction> AwakeTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Ragdoll), nameof(Ragdoll.m_ttl))))
        .ThrowIfInvalid("Could not patch Ragdoll.Awake()! (destroy-now-ttl)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Ragdoll), nameof(Ragdoll.m_nview))),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RagdollPatch), nameof(DestroyNowDelegate))))
        .InstructionEnumeration();
  }

  static float DestroyNowDelegate(float ttl, ZNetView netView) {
    if (IsModEnabled.Value && !RagdollManager.HasCustomFieldTtl(netView)) {
      return 0f;
    }

    return ttl;
  }
}
