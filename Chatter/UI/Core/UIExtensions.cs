using System;
using System.Collections.Generic;
using System.Linq;

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

  public static class ColorExtensions {
    public static Color SetAlpha(this Color color, float alpha) {
      color.a = alpha;
      return color;
    }
  }

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

  public static class GameObjectExtensions {
    public static T SetEnabled<T>(this T behaviour, bool enabled) where T : Behaviour {
      behaviour.enabled = enabled;
      return behaviour;
    }

    public static GameObject SetName(this GameObject gameObject, string name) {
      gameObject.name = name;
      return gameObject;
    }

    public static T SetName<T>(this T component, string name) where T : Component {
      component.gameObject.name = name;
      return component;
    }

    public static GameObject SetParent(
        this GameObject gameObject, Transform transform, bool worldPositionStays = false) {
      gameObject.transform.SetParent(transform, worldPositionStays);
      return gameObject;
    }

    public static IEnumerable<GameObject> Children(this GameObject gameObject) {
      return gameObject
          ? gameObject.transform.Cast<Transform>().Select(t => t.gameObject)
          : Enumerable.Empty<GameObject>();
    }

    public static Button Button(this GameObject gameObject) {
      return gameObject ? gameObject.GetComponent<Button>() : null;
    }

    public static Image Image(this GameObject gameObject) {
      return gameObject ? gameObject.GetComponent<Image>() : null;
    }

    public static LayoutElement LayoutElement(this GameObject gameObject) {
      return gameObject ? gameObject.GetComponent<LayoutElement>() : null;
    }

    public static RectTransform RectTransform(this GameObject gameObject) {
      return gameObject ? gameObject.GetComponent<RectTransform>() : null;
    }

    public static Text Text(this GameObject gameObject) {
      return gameObject ? gameObject.GetComponent<Text>() : null;
    }
  }

  public static class GridLayoutGroupExtensions {
    public static GridLayoutGroup SetCellSize(this GridLayoutGroup layoutGroup, Vector2 cellSize) {
      layoutGroup.cellSize = cellSize;
      return layoutGroup;
    }

    public static GridLayoutGroup SetConstraint(
        this GridLayoutGroup layoutGroup, GridLayoutGroup.Constraint constraint) {
      layoutGroup.constraint = constraint;
      return layoutGroup;
    }

    public static GridLayoutGroup SetConstraintCount(this GridLayoutGroup layoutGroup, int constraintCount) {
      layoutGroup.constraintCount = constraintCount;
      return layoutGroup;
    }

    public static GridLayoutGroup SetStartAxis(this GridLayoutGroup layoutGroup, GridLayoutGroup.Axis startAxis) {
      layoutGroup.startAxis = startAxis;
      return layoutGroup;
    }

    public static GridLayoutGroup SetStartCorner(
        this GridLayoutGroup layoutGroup, GridLayoutGroup.Corner startCorner) {
      layoutGroup.startCorner = startCorner;
      return layoutGroup;
    }

    public static GridLayoutGroup SetPadding(
        this GridLayoutGroup layoutGroup,
        int? left = null,
        int? right = null,
        int? top = null,
        int? bottom = null) {
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

    public static GridLayoutGroup SetSpacing(this GridLayoutGroup layoutGroup, Vector2 spacing) {
      layoutGroup.spacing = spacing;
      return layoutGroup;
    }
  }

  public static class HorizontalLayoutGroupExtensions {
    public static HorizontalLayoutGroup SetChildControl(
        this HorizontalLayoutGroup layoutGroup, bool? width = null, bool? height = null) {
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

    public static HorizontalLayoutGroup SetChildForceExpand(
        this HorizontalLayoutGroup layoutGroup, bool? width = null, bool? height = null) {
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

    public static HorizontalLayoutGroup SetChildAlignment(
        this HorizontalLayoutGroup layoutGroup, TextAnchor alignment) {
      layoutGroup.childAlignment = alignment;
      return layoutGroup;
    }

    public static HorizontalLayoutGroup SetPadding(
        this HorizontalLayoutGroup layoutGroup,
        int? left = null,
        int? right = null,
        int? top = null,
        int? bottom = null) {
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

    public static HorizontalLayoutGroup SetSpacing(this HorizontalLayoutGroup layoutGroup, float spacing) {
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

  public static class InputFieldExtensions {
    public static InputField SetTextComponent(this InputField inputField, Text textComponent) {
      inputField.textComponent = textComponent;
      return inputField;
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

  public static class MaskExtensions {
    public static Mask SetShowMaskGraphic(this Mask mask, bool showMaskGraphic) {
      mask.showMaskGraphic = showMaskGraphic;
      return mask;
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

    public static T SetTargetGraphic<T>(this T selectable, Graphic graphic) where T : Selectable {
      selectable.targetGraphic = graphic;
      return selectable;
    }

    public static T SetTransition<T>(this T selectable, Selectable.Transition transition) where T : Selectable{
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

  public static class ShadowExtensions {
    public static Shadow SetEffectColor(this Shadow shadow, Color effectColor) {
      shadow.effectColor = effectColor;
      return shadow;
    }

    public static Shadow SetEffectDistance(this Shadow shadow, Vector2 effectDistance) {
      shadow.effectDistance = effectDistance;
      return shadow;
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
  }

  public static class SpriteExtensions {
    public static Sprite SetName(this Sprite sprite, string name) {
      sprite.name = name;
      return sprite;
    }
  }

  public static class TextExtensions {
    public static Text SetAlignment(this Text text, TextAnchor alignment) {
      text.alignment = alignment;
      return text;
    }

    public static Text SetColor(this Text text, Color color) {
      text.color = color;
      return text;
    }

    public static Text SetFont(this Text text, Font font) {
      text.font = font;
      return text;
    }

    public static Text SetFontStyle(this Text text, FontStyle fontStyle) {
      text.fontStyle = fontStyle;
      return text;
    }

    public static Text SetFontSize(this Text text, int fontSize) {
      text.fontSize = fontSize;
      return text;
    }

    public static Text SetResizeTextForBestFit(this Text text, bool resizeTextForBestFit) {
      text.resizeTextForBestFit = resizeTextForBestFit;
      return text;
    }

    public static Text SetSupportRichText(this Text text, bool supportRichText) {
      text.supportRichText = supportRichText;
      return text;
    }

    public static Text SetText(this Text text, string value) {
      text.text = value;
      return text;
    }
  }

  public static class Texture2DExtensions {
    public static Texture2D SetName(this Texture2D texture, string name) {
      texture.name = name;
      return texture;
    }

    public static Texture2D SetWrapMode(this Texture2D texture, TextureWrapMode wrapMode) {
      texture.wrapMode = wrapMode;
      return texture;
    }

    public static Texture2D SetFilterMode(this Texture2D texture, FilterMode filterMode) {
      texture.filterMode = filterMode;
      return texture;
    }
  }

  public static class ToggleExtensions {
    public static Toggle SetIsOn(this Toggle toggle, bool isOn) {
      toggle.SetIsOnWithoutNotify(isOn);
      toggle.onValueChanged?.Invoke(isOn);

      return toggle;
    }
  }

  public static class VerticalLayoutGroupExtensions {
    public static VerticalLayoutGroup SetChildControl(
        this VerticalLayoutGroup layoutGroup, bool? width = null, bool? height = null) {
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

    public static VerticalLayoutGroup SetChildForceExpand(
        this VerticalLayoutGroup layoutGroup, bool? width = null, bool? height = null) {
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

    public static VerticalLayoutGroup SetChildAlignment(this VerticalLayoutGroup layoutGroup, TextAnchor alignment) {
      layoutGroup.childAlignment = alignment;
      return layoutGroup;
    }

    public static VerticalLayoutGroup SetPadding(
        this VerticalLayoutGroup layoutGroup,
        int? left = null,
        int? right = null,
        int? top = null,
        int? bottom = null) {
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

    public static VerticalLayoutGroup SetSpacing(this VerticalLayoutGroup layoutGroup, float spacing) {
      layoutGroup.spacing = spacing;
      return layoutGroup;
    }
  }
}