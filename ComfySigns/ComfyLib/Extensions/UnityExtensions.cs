using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace ComfyLib {
  public static class ComponentExtensions {
    public static bool TryGetComponentInParent<T>(this Component source, out T component) where T : Component {
      component = source.GetComponentInParent<T>();
      return component;
    }
  }

  public static class GameObjectExtensions {
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
      if (!gameObject.TryGetComponent(out T component)) {
        component = gameObject.AddComponent<T>();
      }

      return component;
    }
  }

  public static class ObjectExtensions {
    public static T Ref<T>(this T obj) where T : UnityEngine.Object {
      return obj ? obj : null;
    }
  }

  public static class RectMask2DExtensions {
    public static RectMask2D SetPadding(this RectMask2D rectMask, float left, float top, float right, float bottom) {
      rectMask.padding = new(left, bottom, right, top);
      return rectMask;
    }

    public static RectMask2D SetSoftness(this RectMask2D rectMask, int horizontal, int vertical) {
      rectMask.softness = new(horizontal, vertical);
      return rectMask;
    }
  }

  public static class TMPInputFieldExtensions {
    public static TMP_InputField SetRichText(this TMP_InputField inputField, bool richText) {
      inputField.richText = richText;
      return inputField;
    }

    public static TMP_InputField SetTextViewport(this TMP_InputField inputField, RectTransform viewport) {
      inputField.textViewport = viewport;
      return inputField;
    }
  }

  public static class TextMeshProUGUIExtensions {
    public static TextMeshProUGUI SetColor(this TextMeshProUGUI text, Color color) {
      if (text.color != color) {
        text.color = color;
      }

      return text;
    }

    public static TextMeshProUGUI SetFont(this TextMeshProUGUI text, TMP_FontAsset font) {
      if (text.font != font) {
        text.font = font;
      }

      return text;
    }
  }
}