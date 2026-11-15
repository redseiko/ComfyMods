namespace Intermission;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Hud))]
static class HudPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.Awake))]
  static void AwakePostfix(Hud __instance) {
    LoadingManager.SetupHud(__instance);
  }

  static bool _loadingScreenState;

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Hud.UpdateBlackScreen))]
  static void UpdateBlackScreenPrefix(Hud __instance) {
    _loadingScreenState = __instance.m_loadingProgress.activeInHierarchy;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.UpdateBlackScreen))]
  static void UpdateBlackScreenPostfix(Hud __instance) {
    if (!_loadingScreenState && __instance.m_loadingProgress.activeInHierarchy) {
      LoadingManager.SetLoadingImage(__instance.m_loadingImage);
      LoadingManager.SetLoadingTip(__instance.m_loadingTip);

      __instance.ScaleLerpLoadingImage(__instance.m_loadingImage);
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Hud.UpdateBlackScreen))]
  static IEnumerable<CodeInstruction> UpdateBlackScreenTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Player), nameof(Player.ShowTeleportAnimation))),
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Hud), nameof(Hud.m_loadingProgress))))
        .ThrowIfInvalid($"Could not patch Hud.UpdateBlackScreen()! (show-teleport-animation)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(HudPatch), nameof(ShowTeleportAnimationDelegate))))
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Hud), nameof(Hud.UpdateProgressIndicator))),
            new CodeMatch(OpCodes.Ret),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Hud), nameof(Hud.m_loadingProgress))),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))))
        .ThrowIfInvalid($"Could not patch Hud.UpdateBlackScreen()! (set-loading-progress-active)")
        .Advance(offset: 5)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(HudPatch), nameof(ShowLoadingProgressDelegate))))
        .InstructionEnumeration();
  }

  static bool ShowTeleportAnimationDelegate(bool showTeleportAnimation, Hud hud) {
    if (!showTeleportAnimation) {
      return false;
    }

    if (ShowLoadingScreenOnTeleport.Value) {
      hud.UpdateShownTip(false);
      return false;
    }

    return true;
  }

  static bool ShowLoadingProgressDelegate(bool _, Player player) {
    if (CinematicsManager.IsPlaying()) {
      return false;
    }

    if (player && player.IsDead()) {
      return ShowLoadingScreenImmediatelyOnDeath.Value;
    }

    return true;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Hud.UpdateShownTip))]
  static IEnumerable<CodeInstruction> UpdateShownTipTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Hud), nameof(Hud.m_currentLoadingTipIndex))),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Stloc_1))
        .ThrowIfInvalid($"Could not patch Hud.UpdateShownTip()! (loading-tip-set-text)")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HudPatch), nameof(LoadingTipSetTextDelegate))))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "tip:"),
            new CodeMatch(OpCodes.Ldloc_1),
            new CodeMatch(
                OpCodes.Call,
                AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))))
        .ThrowIfInvalid($"Could not patch Hud.UpdateShownTip()! (loading-tip-ignore-zlog)")
        .CreateLabelOffset(offset: 4, out Label logLabel)
        .ExtractLabels(out List<Label> ldstrLabels)
        .Insert(new CodeInstruction(OpCodes.Br, logLabel))
        .AddLabels(ldstrLabels)
        .Advance(offset: 1)
        .InstructionEnumeration();
  }

  static string LoadingTipSetTextDelegate(string text) {
    if (CustomAssets.GetRandomLoadingTip(out string loadingTip)) {
      return loadingTip;
    }

    return text;
  }
}
