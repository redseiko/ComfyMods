namespace Intermission;

using HarmonyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

[HarmonyPatch(typeof(SceneLoader))]
static class SceneLoaderPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(SceneLoader.Start))]
  static void StartPrefix(SceneLoader __instance) {
    SetupSceneLoader(__instance);
  }

  static Image _loadingImage;
  static TMP_Text _loadingText;

  static float _startTime;
  static float _lastImageTime;

  static void SetupSceneLoader(SceneLoader sceneLoader) {
    _loadingImage = sceneLoader.gameLogo.transform.parent.Find("Bkg").GetComponent<Image>();

    if (SceneLoaderUseLoadingImages.Value && CustomAssets.LoadingImageFiles.Count > 0) {
      sceneLoader._showLogos = false;
      sceneLoader._showSaveNotification = false;
      sceneLoader._showHealthWarning = false;

      HudUtils.SetupLoadingBackground(_loadingImage.transform.parent);
      HudUtils.SetupLoadingImage(_loadingImage);
      HudUtils.SetLoadingImage(_loadingImage);
      sceneLoader.ScaleLerpLoadingImage(_loadingImage);

      sceneLoader.gameLogo = _loadingImage.gameObject;
    }

    if (SceneLoaderShowProgressText.Value) {
      TMP_Text sourceLoadingText = sceneLoader.savingNotification.GetComponentInChildren<TMP_Text>();
      _loadingText = UnityEngine.Object.Instantiate(sourceLoadingText, _loadingImage.transform.parent);

      HudUtils.SetupTipText(_loadingText);
      HudUtils.SetLoadingTip(_loadingText);
    }

    if (SceneLoaderCenterProgressIndicator.Value) {
      SetupLoadingIndicator(sceneLoader.loadingIndicator);
    }

    _startTime = Time.time;
    _lastImageTime = _startTime;
  }

  static void SetupLoadingIndicator(LoadingIndicator indicator) {
    indicator.m_showProgressIndicator = true;

    RectTransform rectTransform = indicator.transform.GetComponent<RectTransform>();
    rectTransform.anchorMin = new(0.5f, 0f);
    rectTransform.anchorMax = new(0.5f, 0f);
    rectTransform.pivot = new(0.5f, 0f);
    rectTransform.anchoredPosition = SceneLoaderProgressIndicatorOffset.Value;
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
