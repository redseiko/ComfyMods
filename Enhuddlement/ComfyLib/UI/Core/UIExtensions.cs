using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace ComfyLib {
  public static class CanvasGroupExtensions {
    public static CanvasGroup SetAlpha(this CanvasGroup canvasGroup, float alpha) {
      canvasGroup.alpha = alpha;
      return canvasGroup;
    }

    public static CanvasGroup SetBlocksRaycasts(this CanvasGroup canvasGroup, bool blocksRaycasts) {
      canvasGroup.blocksRaycasts = blocksRaycasts;
      return canvasGroup;
    }
  }

  public static class ComponentExtensions {
    public static T SetName<T>(this T component, string name) where T : Component {
      component.gameObject.name = name;
      return component;
    }
  }

  public static class OutlineExtensions {
    public static Outline SetEffectColor(this Outline outline, Color color) {
      outline.effectColor = color;
      return outline;
    }
  }

  public static class RectTransformExtensions {
    public static RectTransform SetAnchorMin(this RectTransform rectTransform, Vector2 anchorMin) {
      rectTransform.anchorMin = anchorMin;
      return rectTransform;
    }

    public static RectTransform SetAnchorMax(this RectTransform rectTransform, Vector2 anchorMax) {
      rectTransform.anchorMax = anchorMax;
      return rectTransform;
    }

    public static RectTransform SetPivot(this RectTransform rectTransform, Vector2 pivot) {
      rectTransform.pivot = pivot;
      return rectTransform;
    }

    public static RectTransform SetPosition(this RectTransform rectTransform, Vector2 position) {
      rectTransform.anchoredPosition = position;
      return rectTransform;
    }

    public static RectTransform SetSizeDelta(this RectTransform rectTransform, Vector2 sizeDelta) {
      rectTransform.sizeDelta = sizeDelta;
      return rectTransform;
    }
  }

  public static class TextMeshProUGUIExtensions {
    public static T SetAlignment<T>(this T tmpText, TextAlignmentOptions alignment) where T : TMP_Text {
      tmpText.alignment = alignment;
      return tmpText;
    }

    public static T SetColor<T>(this T tmpText, Color color) where T : TMP_Text {
      tmpText.color = color;
      return tmpText;
    }

    public static T SetEnableAutoSizing<T>(this T tmpText, bool enableAutoSizing) where T : TMP_Text {
      tmpText.enableAutoSizing = enableAutoSizing;
      return tmpText;
    }

    public static T SetFont<T>(this T tmpText, TMP_FontAsset font) where T : TMP_Text {
      tmpText.font = font;
      return tmpText;
    }

    public static T SetFontSize<T>(this T tmpText, float fontSize) where T : TMP_Text {
      tmpText.fontSize = fontSize;
      return tmpText;
    }

    public static T SetFontMaterial<T>(this T tmpText, Material fontMaterial) where T : TMP_Text {
      tmpText.fontMaterial = fontMaterial;
      return tmpText;
    }

    public static T SetMargin<T>(this T tmpText, Vector4 margin) where T : TMP_Text {
      tmpText.margin = margin;
      return tmpText;
    }

    public static T SetOverflowMode<T>(this T tmpText, TextOverflowModes overflowMode) where T : TMP_Text {
      tmpText.overflowMode = overflowMode;
      return tmpText;
    }

    public static T SetRichText<T>(this T tmpText, bool richText) where T : TMP_Text {
      tmpText.richText = richText;
      return tmpText;
    }

    public static T SetTextWrappingMode<T>(this T tmpText, TextWrappingModes textWrappingMode) where T : TMP_Text {
      tmpText.textWrappingMode = textWrappingMode;
      return tmpText;
    }
  }
}
