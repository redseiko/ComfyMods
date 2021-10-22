using UnityEngine;
using UnityEngine.UI;

namespace CarbonCopy {
  public static class TextExtensions {
    public static Text SetAlignment(this Text text, TextAnchor alignment) {
      text.alignment = alignment;
      return text;
    }

    public static Text SetFont(this Text text, Font font) {
      text.font = font;
      return text;
    }

    public static Text SetFontSize(this Text text, int fontSize) {
      text.fontSize = fontSize;
      return text;
    }

    public static Text SetText(this Text text, string value) {
      text.text = value;
      return text;
    }
  }
}
