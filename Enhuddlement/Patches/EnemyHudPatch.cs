namespace Enhuddlement;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(EnemyHud))]
static class EnemyHudPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(EnemyHud.ShowHud))]
  static void ShowHudPrefix(ref EnemyHud __instance, ref Character c, ref bool __state) {
    __state = __instance.m_huds.ContainsKey(c);
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(EnemyHud.ShowHud))]
  static void ShowHudPostfix(ref EnemyHud __instance, ref Character c, ref bool isMount, ref bool __state) {
    if (!IsModEnabled.Value || __state || !__instance.m_huds.TryGetValue(c, out EnemyHud.HudData hudData)) {
      return;
    }

    if (c.IsPlayer()) {
      EnemyHudManager.SetupPlayerHud(hudData);
    } else if (c.IsBoss()) {
      EnemyHudManager.SetupBossHud(hudData);
    } else if (isMount) {
      // Nothing.
    } else {
      EnemyHudManager.SetupEnemyHud(hudData);
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(EnemyHud.UpdateHuds))]
  static bool UpdateHudsPrefix(ref EnemyHud __instance, ref Player player, ref Sadle sadle, float dt) {
    if (IsModEnabled.Value) {
      EnemyHudManager.UpdateHuds(ref __instance, ref player, ref sadle, dt);
      return false;
    }

    return true;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(EnemyHud.LateUpdate))]
  static IEnumerable<CodeInstruction> LateUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        // (character == localPlayer)
        .MatchStartForward(
            new CodeMatch(OpCodes.Stloc_3),
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Ldloc_1),
            new CodeMatch(OpCodes.Call))
        .Advance(offset: 3)
        .ThrowIfInvalid($"Could not patch EnemyHud.LateUpdate()! (character-local-player-equality)")
        .SetInstructionAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(EnemyHudPatch), nameof(CharacterLocalPlayerEqualityDelegate))))
        .InstructionEnumeration();
  }

  static bool CharacterLocalPlayerEqualityDelegate(Character character, Player player) {
    if (PlayerHudShowLocalPlayer.Value) {
      return false;
    }

    return character == player;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(EnemyHud.TestShow))]
  static void TestShowPostfix(Character c, ref bool __result) {
    if (__result && c == Player.m_localPlayer) {
      __result = IsModEnabled.Value && PlayerHudShowLocalPlayer.Value;
    }
  }
}
