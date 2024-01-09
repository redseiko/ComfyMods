using System;

using UnityEngine;
using UnityEngine.UI;

namespace ComfyLib {
  public static class ContentSizeFitterExtensions {
    public static ContentSizeFitter SetHorizontalFit(
        this ContentSizeFitter fitter, ContentSizeFitter.FitMode fitMode) {
      fitter.horizontalFit = fitMode;
      return fitter;
    }

    public static ContentSizeFitter SetVerticalFit(this ContentSizeFitter fitter, ContentSizeFitter.FitMode fitMode) {
      fitter.verticalFit = fitMode;
      return fitter;
    }
  }

  public static class ImageExtensions {
    public static Image SetColor(this Image image, Color color) {
      image.color = color;
      return image;
    }

    public static Image SetMaskable(this Image image, bool maskable) {
      image.maskable = maskable;
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

    public static LayoutElement SetPreferred(this LayoutElement layoutElement, Vector2 sizeDelta) {
      layoutElement.preferredWidth = sizeDelta.x;
      layoutElement.preferredHeight = sizeDelta.y;

      return layoutElement;
    }

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

    public static LayoutElement SetIgnoreLayout(this LayoutElement layoutElement, bool ignoreLayout) {
      layoutElement.ignoreLayout = ignoreLayout;
      return layoutElement;
    }
  }

  public static class LayoutGroupExtensions {
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

    public static T SetChildAlignment<T>(
        this T layoutGroup, TextAnchor alignment) where T : HorizontalOrVerticalLayoutGroup {
      layoutGroup.childAlignment = alignment;
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

  public static class OutlineExtensions {
    public static Outline SetEffectColor(this Outline outline, Color effectColor) {
      outline.effectColor = effectColor;
      return outline;
    }

    public static Outline SetEffectDistance(this Outline outline, Vector2 effectDistance) {
      outline.effectDistance = effectDistance;
      return outline;
    }

    public static Outline SetUseGraphicAlpha(this Outline outline, bool useGraphicAlpha) {
      outline.useGraphicAlpha = useGraphicAlpha;
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

  public static class ScrollRectExtensions {
    public static ScrollRect SetScrollSensitivity(this ScrollRect scrollRect, float sensitivity) {
      scrollRect.scrollSensitivity = sensitivity;
      return scrollRect;
    }

    public static ScrollRect SetVerticalScrollPosition(this ScrollRect scrollRect, float position) {
      scrollRect.verticalNormalizedPosition = position;
      return scrollRect;
    }

    public static ScrollRect SetViewport(this ScrollRect scrollRect, RectTransform viewport) {
      scrollRect.viewport = viewport;
      return scrollRect;
    }

    public static ScrollRect SetContent(this ScrollRect scrollRect, RectTransform content) {
      scrollRect.content = content;
      return scrollRect;
    }

    public static ScrollRect SetHorizontal(this ScrollRect scrollRect, bool horizontal) {
      scrollRect.horizontal = horizontal;
      return scrollRect;
    }

    public static ScrollRect SetVertical(this ScrollRect scrollRect, bool vertical) {
      scrollRect.vertical = vertical;
      return scrollRect;
    }

    public static ScrollRect SetMovementType(this ScrollRect scrollRect, ScrollRect.MovementType movementType) {
      scrollRect.movementType = movementType;
      return scrollRect;
    }
  }
}
