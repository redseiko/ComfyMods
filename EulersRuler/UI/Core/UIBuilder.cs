using TMPro;

using UnityEngine;

namespace ComfyLib {
  public static class UIBuilder {
    public static TMP_Text CreateTMPLabel(Transform parentTransform) {
      TMP_Text label = UnityEngine.Object.Instantiate(UnifiedPopup.instance.bodyText, parentTransform);
      label.name = "Label";

      label.enableAutoSizing = false;
      label.fontSize = 16f;
      label.overflowMode = TextOverflowModes.Overflow;
      label.textWrappingMode = TextWrappingModes.NoWrap;
      label.color = Color.white;
      label.text = string.Empty;

      return label;
    }
  }
}
