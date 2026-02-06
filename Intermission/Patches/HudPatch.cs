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
  static void AwakePostfix(Hud __instance) {
    SetupHud(__instance);
  }

  static void SetupHud(Hud hud) {
    Transform _panelSeparator = hud.m_loadingProgress.transform.Find("panel_separator");
    HudUtils.SetupTipText(hud.m_loadingTip);
    HudUtils.SetupLoadingImage(hud.m_loadingImage);
    HudUtils.SetupPanelSeparator(_panelSeparator);

    HudUtils.SetLoadingTip(hud.m_loadingTip);
    HudUtils.SetLoadingImage(hud.m_loadingImage);

    hud.m_loadingProgress.transform.Find("TopFade").Ref()?.gameObject.SetActive(false);
    hud.m_loadingProgress.transform.Find("BottomFade").Ref()?.gameObject.SetActive(false);
    hud.m_loadingProgress.transform.Find("text_darken").Ref()?.gameObject.SetActive(false);

    hud.m_teleportingProgress = hud.m_loadingProgress;

    HudUtils.SetupLoadingBackground(hud.transform.Find("LoadingBlack/Bkg").GetComponent<Image>());

    Transform _loadingBlack = hud.transform.Find("LoadingBlack");
    hud.m_loadingImage.transform.SetParent(_loadingBlack, false);
    hud.m_loadingTip.transform.SetParent(_loadingBlack, false);
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
        .MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Player), nameof(Player.ShowTeleportAnimation))),
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Hud), nameof(Hud.m_loadingProgress))))
        .ThrowIfInvalid($"Could not patch Hud.UpdateBlackScreen()! (show-teleport-animation)")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldc_I4_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Hud), nameof(Hud.UpdateShownTip))))
        .InstructionEnumeration();
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
