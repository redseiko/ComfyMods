namespace ComfyLib;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public static class UIBuilder {
  public static TextMeshProUGUI CreateTMPLabel(Transform parentTransform) {
    TextMeshProUGUI label = UnityEngine.Object.Instantiate(UnifiedPopup.instance.bodyText, parentTransform);
    label.name = "Label";

    label.enableAutoSizing = false;
    label.fontSize = 16f;
    label.overflowMode = TextOverflowModes.Overflow;
    label.textWrappingMode = TextWrappingModes.NoWrap;
    label.color = Color.white;
    label.text = string.Empty;

    return label;
  }

  public static GameObject CreateRowSpacer(Transform parentTransform) {
    GameObject spacer = new($"{parentTransform.name}.Spacer", typeof(RectTransform));
    spacer.transform.SetParent(parentTransform, worldPositionStays: false);

    spacer.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f);

    return spacer;
  }

  static readonly Lazy<TextGenerator> CachedTextGenerator = new();

  public static float GetTextPreferredWidth(Text text) {
    return GetTextPreferredWidth(text, text.text);
  }

  public static float GetTextPreferredWidth(Text text, string value) {
    return CachedTextGenerator.Value.GetPreferredWidth(
        value, text.GetGenerationSettings(text.rectTransform.rect.size));
  }
}
