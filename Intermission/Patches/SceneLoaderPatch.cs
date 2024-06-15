namespace Intermission;

using HarmonyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

[HarmonyPatch(typeof(SceneLoader))]
static class SceneLoaderPatch {
  static Image _loadingImage;
  static TMP_Text _loadingText;

  static float _startTime;
  static float _lastImageTime;

  [HarmonyPrefix]
  [HarmonyPatch(nameof(SceneLoader.Start))]
  static void StartPrefix(SceneLoader __instance) {
    _loadingImage = __instance.gameLogo.transform.parent.Find("Bkg").GetComponent<Image>();

    if (SceneLoaderUseLoadingImages.Value && CustomAssets.LoadingImageFiles.Count > 0) {
      __instance._showLogos = false;
      __instance._showSaveNotification = false;
      __instance._showHealthWarning = false;

      // set black background as default backing such that the SceneLoader loading screen does not display the default
      // Unity scene behind loading images that do not scale up to the whole screen
      HudUtils.SetupBlackBackground(_loadingImage);

      HudUtils.SetupLoadingImage(_loadingImage);
      HudUtils.SetLoadingImage(_loadingImage);
      __instance.ScaleLerpLoadingImage(_loadingImage);

      __instance.gameLogo = _loadingImage.gameObject;
    }

    if (SceneLoaderShowProgressText.Value) {
      TMP_Text sourceLoadingText = __instance.savingNotification.GetComponentInChildren<TMP_Text>();
      _loadingText = UnityEngine.Object.Instantiate(sourceLoadingText, _loadingImage.transform.parent);

      HudUtils.SetupTipText(_loadingText);
      HudUtils.SetLoadingTip(_loadingText);
    }

    if (SceneLoaderCenterProgressIndicator.Value) {
      SetupLoadingIndicator(LoadingIndicator.s_instance);
    }

    _startTime = Time.time;
    _lastImageTime = _startTime;
  }

  static void SetupLoadingIndicator(LoadingIndicator indicator) {
    indicator.m_showProgressIndicator = true;

    RectTransform rectTransform = indicator.m_background.transform.parent.GetComponent<RectTransform>();
    rectTransform.anchorMin = new(0.5f, 0f);
    rectTransform.anchorMax = new(0.5f, 0f);
    rectTransform.anchoredPosition = new(0f, 250f);
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(SceneLoader.Update))]
  static void UpdatePostfix(SceneLoader __instance) {
    float time = Time.time;

    if (SceneLoaderUseLoadingImages.Value && time - _lastImageTime >= 10f) {
      _lastImageTime = time;

      HudUtils.SetLoadingImage(_loadingImage);
      __instance.ScaleLerpLoadingImage(_loadingImage);
    }

    if (SceneLoaderShowProgressText.Value && _loadingText) {
      _loadingText.text = $"<b>{__instance._fakeProgress * 100:F0}%</b>\n<size=-4>({time - _startTime:F1}s)</size>";
    }
  }
}
