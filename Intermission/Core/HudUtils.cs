namespace Intermission;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public static class HudUtils {
  static TMP_Text _cachedTipText;
  static Image _cachedLoadingImage;
  static Image _cachedLoadingBackground;
  static Transform _cachedPanelSeparator;

  public static void SetLoadingImage(Image loadingImage) {
    if (loadingImage && CustomAssets.GetRandomLoadingImage(out Sprite loadingImageSprite)) {
      loadingImage.SetSprite(loadingImageSprite);
    }
  }

  public static void SetLoadingTip(TMP_Text tipText) {
    if (tipText && CustomAssets.GetRandomLoadingTip(out string loadingTipText)) {
      tipText.SetText(loadingTipText);
    }
  }

  public static void SetupLoadingImage(Image loadingImage = default) {
    if (loadingImage) {
      _cachedLoadingImage = loadingImage;
    } else if (_cachedLoadingImage) {
      loadingImage = _cachedLoadingImage;
    } else {
      Intermission.LogError($"Could not find a LoadingImage to setup!");
    }

    loadingImage
        .SetType(Image.Type.Simple)
        .SetColor(LoadingImageBaseColor.Value)
        .SetPreserveAspect(true);
  }

  public static void SetupLoadingBackground(Transform parentTransform) {
    if (!_cachedLoadingBackground) {
      GameObject background = new("Background", typeof(RectTransform));
      _cachedLoadingBackground = background.AddComponent<Image>();
    }

    _cachedLoadingBackground.transform.SetParent(parentTransform, worldPositionStays: false);
    _cachedLoadingBackground.transform.SetAsFirstSibling();

    _cachedLoadingBackground.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    SetupLoadingBackground();
  }

  public static void SetupLoadingBackground(Image loadingBackground = default) {
    if (loadingBackground) {
      _cachedLoadingBackground = loadingBackground;
    } else if (_cachedLoadingBackground) {
      loadingBackground = _cachedLoadingBackground;
    } else {
      Intermission.LogError($"Could not find a LoadingBackground to setup!");
    }

    loadingBackground.Ref()?
        .SetType(Image.Type.Simple)
        .SetColor(LoadingScreenBackgroundColor.Value);
  }

  public static void SetupPanelSeparator(Transform panelSeparator = default) {
    if (panelSeparator) {
      _cachedPanelSeparator = panelSeparator;
    } else if (_cachedPanelSeparator) {
      panelSeparator = _cachedPanelSeparator;
    } else {
      Intermission.LogError($"Could not find a PanelSeparator to setup!");
    }

    panelSeparator.gameObject.SetActive(LoadingScreenShowPanelSeparator.Value);
    panelSeparator.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0f))
        .SetAnchorMax(new(0.5f, 0f))
        .SetPosition(LoadingScreenPanelSeparatorPosition.Value);
  }

  public static void SetupTipText(TMP_Text tipText = default) {
    if (tipText) {
      _cachedTipText = tipText;
    } else if (_cachedTipText) {
      tipText = _cachedTipText;
    } else {
      Intermission.LogError($"Could not find a TipText to setup!");
    }

    tipText
        .SetAlignment(TextAlignmentOptions.Top)
        .SetTextWrappingMode(TextWrappingModes.Normal)
        .SetFontSize(LoadingTipTextFontSize.Value)
        .SetColor(LoadingTipTextColor.Value);

    tipText.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.right)
        .SetPosition(LoadingTipTextPosition.Value)
        .SetSizeDelta(new(-50f, 78f));
  }
}
