namespace ComfyLib;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

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

public static class ColorExtensions {
  public static Color SetAlpha(this Color color, float alpha) {
    color.a = alpha;
    return color;
  }
}

public static class ContentSizeFitterExtensions {
  public static ContentSizeFitter SetHorizontalFit(this ContentSizeFitter fitter, ContentSizeFitter.FitMode fitMode) {
    fitter.horizontalFit = fitMode;
    return fitter;
  }

  public static ContentSizeFitter SetVerticalFit(this ContentSizeFitter fitter, ContentSizeFitter.FitMode fitMode) {
    fitter.verticalFit = fitMode;
    return fitter;
  }
}

public static class LayoutGroupExtensions {
  public static T SetChildAlignment<T>(
      this T layoutGroup, TextAnchor alignment) where T : HorizontalOrVerticalLayoutGroup {
    layoutGroup.childAlignment = alignment;
    return layoutGroup;
  }

  public static T SetChildControl<T>(
      this T layoutGroup, bool? width = null, bool? height = null) where T : HorizontalOrVerticalLayoutGroup {
    if (!width.HasValue && !height.HasValue) {
      throw new ArgumentException("Value for width or height must be provided.");
    }

    if (width.HasValue) {
      layoutGroup.childControlWidth = width.Value;
    }

    if (height.HasValue) {
      layoutGroup.childControlHeight = height.Value;
    }

    return layoutGroup;
  }

  public static T SetChildForceExpand<T>(
      this T layoutGroup, bool? width = null, bool? height = null) where T : HorizontalOrVerticalLayoutGroup {
    if (!width.HasValue && !height.HasValue) {
      throw new ArgumentException("Value for width or height must be provided.");
    }

    if (width.HasValue) {
      layoutGroup.childForceExpandWidth = width.Value;
    }

    if (height.HasValue) {
      layoutGroup.childForceExpandHeight = height.Value;
    }

    return layoutGroup;
  }

  public static T SetPadding<T>(
      this T layoutGroup,
      int? left = null,
      int? right = null,
      int? top = null,
      int? bottom = null)
      where T : HorizontalOrVerticalLayoutGroup {
    if (!left.HasValue && !right.HasValue && !top.HasValue && !bottom.HasValue) {
      throw new ArgumentException("Value for left, right, top or bottom must be provided.");
    }

    if (left.HasValue) {
      layoutGroup.padding.left = left.Value;
    }

    if (right.HasValue) {
      layoutGroup.padding.right = right.Value;
    }

    if (top.HasValue) {
      layoutGroup.padding.top = top.Value;
    }

    if (bottom.HasValue) {
      layoutGroup.padding.bottom = bottom.Value;
    }

    return layoutGroup;
  }

  public static T SetSpacing<T>(this T layoutGroup, float spacing) where T : HorizontalOrVerticalLayoutGroup {
    layoutGroup.spacing = spacing;
    return layoutGroup;
  }
}

public static class ImageExtensions {
  public static Image SetColor(this Image image, Color color) {
    image.color = color;
    return image;
  }

  public static Image SetFillAmount(this Image image, float amount) {
    image.fillAmount = amount;
    return image;
  }

  public static Image SetFillCenter(this Image image, bool fillCenter) {
    image.fillCenter = fillCenter;
    return image;
  }

  public static Image SetFillMethod(this Image image, Image.FillMethod fillMethod) {
    image.fillMethod = fillMethod;
    return image;
  }

  public static Image SetFillOrigin(this Image image, Image.OriginHorizontal origin) {
    image.fillOrigin = (int) origin;
    return image;
  }

  public static Image SetFillOrigin(this Image image, Image.OriginVertical origin) {
    image.fillOrigin = (int) origin;
    return image;
  }

  public static Image SetMaskable(this Image image, bool maskable) {
    image.maskable = maskable;
    return image;
  }

  public static Image SetMaterial(this Image image, Material material) {
    image.material = material;
    return image;
  }

  public static Image SetPixelsPerUnitMultiplier(this Image image, float pixelsPerUnitMultiplier) {
    image.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
    return image;
  }

  public static Image SetPreserveAspect(this Image image, bool preserveAspect) {
    image.preserveAspect = preserveAspect;
    return image;
  }

  public static Image SetRaycastTarget(this Image image, bool raycastTarget) {
    image.raycastTarget = raycastTarget;
    return image;
  }

  public static Image SetSprite(this Image image, Sprite sprite) {
    image.sprite = sprite;
    return image;
  }

  public static Image SetType(this Image image, Image.Type type) {
    image.type = type;
    return image;
  }
}

public static class LayoutElementExtensions {
  public static LayoutElement SetFlexible(
      this LayoutElement layoutElement, float? width = null, float? height = null) {
    if (!width.HasValue && !height.HasValue) {
      throw new ArgumentException("Value for width or height must be provided.");
    }

    if (width.HasValue) {
      layoutElement.flexibleWidth = width.Value;
    }

    if (height.HasValue) {
      layoutElement.flexibleHeight = height.Value;
    }

    return layoutElement;
  }

  public static LayoutElement SetIgnoreLayout(this LayoutElement layoutElement, bool ignoreLayout) {
    layoutElement.ignoreLayout = ignoreLayout;
    return layoutElement;
  }

  public static LayoutElement SetMinimum(
      this LayoutElement layoutElement, float? width = null, float? height = null) {
    if (!width.HasValue && !height.HasValue) {
      throw new ArgumentException("Value for width or height must be provided.");
    }

    if (width.HasValue) {
      layoutElement.minWidth = width.Value;
    }

    if (height.HasValue) {
      layoutElement.minHeight = height.Value;
    }

    return layoutElement;
  }

  public static LayoutElement SetPreferred(
      this LayoutElement layoutElement, float? width = null, float? height = null) {
    if (!width.HasValue && !height.HasValue) {
      throw new ArgumentException("Value for width or height must be provided.");
    }

    if (width.HasValue) {
      layoutElement.preferredWidth = width.Value;
    }

    if (height.HasValue) {
      layoutElement.preferredHeight = height.Value;
    }

    return layoutElement;
  }
}

public static class RectMask2DExtensions {
  public static RectMask2D SetSoftness(this RectMask2D rectMask2d, Vector2Int softness) {
    rectMask2d.softness = softness;
    return rectMask2d;
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

public static class ScrollbarExtensions {
  public static T SetDirection<T>(this T scrollbar, Scrollbar.Direction direction) where T : Scrollbar {
    scrollbar.direction = direction;
    return scrollbar;
  }

  public static T SetHandleRect<T>(this T scrollbar, RectTransform handleRect) where T : Scrollbar {
    scrollbar.handleRect = handleRect;
    return scrollbar;
  }
}

public static class SelectableExtensions {
  public static T SetColors<T>(this T selectable, ColorBlock colors) where T : Selectable {
    selectable.colors = colors;
    return selectable;
  }

  public static T SetImage<T>(this T selectable, Image image) where T : Selectable {
    selectable.image = image;
    return selectable;
  }

  public static T SetInteractable<T>(this T selectable, bool interactable) where T : Selectable {
    selectable.interactable = interactable;
    return selectable;
  }

  public static T SetSpriteState<T>(this T selectable, SpriteState spriteState) where T : Selectable {
    selectable.spriteState = spriteState;
    return selectable;
  }

  public static T SetTargetGraphic<T>(this T selectable, Graphic graphic) where T : Selectable {
    selectable.targetGraphic = graphic;
    return selectable;
  }

  public static T SetTransition<T>(this T selectable, Selectable.Transition transition) where T : Selectable {
    selectable.transition = transition;
    return selectable;
  }

  public static T SetNavigationMode<T>(this T selectable, Navigation.Mode mode) where T : Selectable {
    Navigation navigation = selectable.navigation;
    navigation.mode = mode;
    selectable.navigation = navigation;
    return selectable;
  }
}

public static class SliderExtensions {
  public static T SetDirection<T>(this T slider, Slider.Direction direction) where T : Slider {
    slider.direction = direction;
    return slider;
  }

  public static T SetFillRect<T>(this T slider, RectTransform fillRect) where T : Slider {
    slider.fillRect = fillRect;
    return slider;
  }

  public static T SetHandleRect<T>(this T slider, RectTransform handleRect) where T : Slider {
    slider.handleRect = handleRect;
    return slider;
  }

  public static T SetMaxValue<T>(this T slider, float maxValue) where T : Slider {
    slider.maxValue = maxValue;
    return slider;
  }

  public static T SetMinValue<T>(this T slider, float minValue) where T : Slider {
    slider.minValue = minValue;
    return slider;
  }

  public static T SetWholeNumbers<T>(this T slider, bool wholeNumbers) where T : Slider {
    slider.wholeNumbers = wholeNumbers;
    return slider;
  }
}

public static class ScrollRectExtensions {
  public static T SetContent<T>(this T scrollRect, RectTransform content) where T : ScrollRect {
    scrollRect.content = content;
    return scrollRect;
  }

  public static T SetHorizontal<T>(this T scrollRect, bool horizontal) where T : ScrollRect {
    scrollRect.horizontal = horizontal;
    return scrollRect;
  }

  public static T SetMovementType<T>(this T scrollRect, ScrollRect.MovementType movementType) where T : ScrollRect {
    scrollRect.movementType = movementType;
    return scrollRect;
  }

  public static T SetScrollSensitivity<T>(this T scrollRect, float sensitivity) where T : ScrollRect {
    scrollRect.scrollSensitivity = sensitivity;
    return scrollRect;
  }

  public static T SetVertical<T>(this T scrollRect, bool vertical) where T : ScrollRect {
    scrollRect.vertical = vertical;
    return scrollRect;
  }

  public static T SetVerticalScrollbar<T>(this T scrollRect, Scrollbar verticalScrollbar) where T : ScrollRect {
    scrollRect.verticalScrollbar = verticalScrollbar;
    return scrollRect;
  }

  public static T SetVerticalScrollPosition<T>(this T scrollRect, float position) where T : ScrollRect {
    scrollRect.verticalNormalizedPosition = position;
    return scrollRect;
  }

  public static T SetVerticalScrollbarVisibility<T>(
      this T scrollRect, ScrollRect.ScrollbarVisibility visibility) where T : ScrollRect {
    scrollRect.verticalScrollbarVisibility = visibility;
    return scrollRect;
  }

  public static T SetViewport<T>(this T scrollRect, RectTransform viewport) where T : ScrollRect {
    scrollRect.viewport = viewport;
    return scrollRect;
  }
}

public static class TextMeshProExtensions {
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

public static class Texture2DExtensions {
  public static Texture2D SetFilterMode(this Texture2D texture, FilterMode filterMode) {
    texture.filterMode = filterMode;
    return texture;
  }

  public static Texture2D SetWrapMode(this Texture2D texture, TextureWrapMode wrapMode) {
    texture.wrapMode = wrapMode;
    return texture;
  }
}

public static class ToggleExtensions {
  public static T SetGraphic<T>(this T toggle, Graphic graphic) where T : Toggle {
    toggle.graphic = graphic;
    return toggle;
  }

  public static T SetToggleTransition<T>(this T toggle, Toggle.ToggleTransition toggleTransition) where T : Toggle {
    toggle.toggleTransition = toggleTransition;
    return toggle;
  }
}
