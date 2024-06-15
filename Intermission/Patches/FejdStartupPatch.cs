namespace Intermission;

using HarmonyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.Awake))]
  static void AwakePostfix(ref FejdStartup __instance) {
    Image loadingImage = __instance.m_loading.transform.Find("Bkg").GetComponent<Image>();

    // set black background as default backing such that the FejdStartup screen does not display the main menu behind
    // loading images that do not scale up to the whole screen, and to avoid a visual discrepancy when the Hud screen
    // takes over (since the Hud screen does have a black background built-in already)
    HudUtils.SetupBlackBackground(loadingImage);

    // FejdStartup.m_menuAnimator locks the vanilla TMP_Text UI state, so work-around is to clone it to a new object.
    TMP_Text sourceLoadingText = __instance.m_loading.transform.Find("Text").GetComponent<TMP_Text>();
    TMP_Text loadingText = UnityEngine.Object.Instantiate(sourceLoadingText, __instance.m_loading.transform);
    loadingText.name = sourceLoadingText.name;
    UnityEngine.Object.Destroy(sourceLoadingText.gameObject);

    Transform panelSeparator =
        UnityEngine.Object.Instantiate(
            __instance.m_menuList.transform.Find("ornament"), __instance.m_loading.transform);
    HudUtils.SetupTipText(loadingText);
    HudUtils.SetupLoadingImage(loadingImage);
    HudUtils.SetupPanelSeparator(panelSeparator);
    HudUtils.SetLoadingTip(loadingText);
    HudUtils.SetLoadingImage(loadingImage);
  }
}
