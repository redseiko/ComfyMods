namespace ComfyLib;

using System;
using TMPro;

using UnityEngine;

public static class UIBuilder {
  public static TextMeshProUGUI CreateLabel(Transform parentTransform) {
    TextMeshProUGUI label =
        UnityEngine.Object.Instantiate(UnifiedPopup.instance.bodyText, parentTransform, worldPositionStays: false);

    label.name = "Label";
    label.fontSize = 16f;
    label.richText = true;
    label.color = Color.white;
    label.enableAutoSizing = false;
    label.textWrappingMode = TextWrappingModes.NoWrap;
    label.overflowMode = TextOverflowModes.Overflow;
    label.text = string.Empty;

    return label;
  }
}
