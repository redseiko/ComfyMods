namespace Intermission;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch(typeof(Hud))]
static class HudPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.Awake))]
  static void AwakePostfix(ref Hud __instance) {
    Transform _panelSeparator = __instance.m_loadingProgress.transform.Find("panel_separator");
    HudUtils.SetupTipText(__instance.m_loadingTip);
    HudUtils.SetupLoadingImage(__instance.m_loadingImage);
    HudUtils.SetupPanelSeparator(_panelSeparator);

    HudUtils.SetLoadingTip(__instance.m_loadingTip);
    HudUtils.SetLoadingImage(__instance.m_loadingImage);

    __instance.m_loadingProgress.transform.Find("TopFade").Ref()?.gameObject.SetActive(false);
    __instance.m_loadingProgress.transform.Find("BottomFade").Ref()?.gameObject.SetActive(false);
    __instance.m_loadingProgress.transform.Find("text_darken").Ref()?.gameObject.SetActive(false);

    __instance.m_teleportingProgress = __instance.m_loadingProgress;

    HudUtils.SetupLoadingBackground(__instance.transform.Find("LoadingBlack/Bkg").GetComponent<Image>());

    Transform _loadingBlack = __instance.transform.Find("LoadingBlack");
    __instance.m_loadingImage.transform.SetParent(_loadingBlack, false);
    __instance.m_loadingTip.transform.SetParent(_loadingBlack, false);
    _panelSeparator.SetParent(_loadingBlack, false);
  }

  static bool _loadingScreenState;

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Hud.UpdateBlackScreen))]
  static void UpdateBlackScreenPrefix(ref Hud __instance) {
    _loadingScreenState = __instance.m_loadingImage.gameObject.activeInHierarchy;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.UpdateBlackScreen))]
  static void UpdateBlackScreenPostfix(Hud __instance) {
    if (!_loadingScreenState && __instance.m_loadingScreen.gameObject.activeInHierarchy) {
        HudUtils.SetLoadingImage(__instance.m_loadingImage);
        HudUtils.SetLoadingTip(__instance.m_loadingTip);

        __instance.ScaleLerpLoadingImage(__instance.m_loadingImage);
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Hud.UpdateBlackScreen))]
  static IEnumerable<CodeInstruction> UpdateBlackScreenTranspiler1(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .DeclareLocal(generator, typeof(bool), out LocalBuilder isTeleportingLocal)
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Stloc_2))
        .ThrowIfInvalid($"Could not patch Hud.UpdateBlackScreen()! (teleport-respawn-label)")
        .CreateLabel(out Label respawnLabel)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Hud), nameof(Hud.m_teleportingProgress))),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))),
            new CodeMatch(OpCodes.Ret))
        .ThrowIfInvalid($"Could not patch Hud.UpdateBlackScreen()! (teleport-respawn-branch)")
        .Advance(offset: 3)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_I4_1),
            new CodeInstruction(OpCodes.Stloc, isTeleportingLocal),
            new CodeInstruction(OpCodes.Br, respawnLabel))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Hud), nameof(Hud.m_teleportingProgress))),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))))
        .ThrowIfInvalid($"Could not patch Hud.UpdateBlackScreen()! (teleport-respawn-set-active)")
        .Advance(offset: 1)
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldloc, isTeleportingLocal))
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Hud.UpdateBlackScreen))]
  static IEnumerable<CodeInstruction> UpdateBlackScreenTranspiler2(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Hud), nameof(Hud.m_currentLoadingTipIndex))),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Stloc_S))
        .ThrowIfInvalid($"Could not patch Hud.UpdateBlackScreen()! (loading-tip-set-text)")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HudPatch), nameof(LoadingTipSetTextDelegate))))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "tip:"),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(
                OpCodes.Call,
                AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))))
        .ThrowIfInvalid($"Could not patch Hud.UpdateBlackScreen()! (loading-tip-ignore-zlog)")
        .CreateLabelOffset(offset: 4, out Label logLabel)
        .InsertAndAdvance(new CodeInstruction(OpCodes.Br, logLabel))
        .InstructionEnumeration();
  }

  static string LoadingTipSetTextDelegate(string text) {
    if (CustomAssets.GetRandomLoadingTip(out string loadingTip)) {
      return loadingTip;
    }

    return text;
  }
}
