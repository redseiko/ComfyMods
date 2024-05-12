namespace ComfyLib;

using TMPro;

using UnityEngine;

public static class UIBuilder {
  public static TextMeshProUGUI CreateTMPLabel(Transform parentTransform) {
    TextMeshProUGUI label =
        UnityEngine.Object.Instantiate(UnifiedPopup.instance.bodyText, parentTransform, worldPositionStays: false);

    label.name = "Label";
    label.color = Color.white;
    label.enableAutoSizing = false;
    label.fontSize = 16f;
    label.overflowMode = TextOverflowModes.Overflow;
    label.richText = true;
    label.textWrappingMode = TextWrappingModes.NoWrap;
    label.text = string.Empty;

    return label;
  }

  // List
  // Sprite: item_background
  // Color: 0, 0, 0, 0.565

  // ListItem.Background
  // Sprite: item_background
  // Color: 1, 1, 1, 0.759

  // ListItem.Selected
  // Sprite: item_background
  // Color: 1, 0.641, 1, 0, 1

  // Selectable.Colors
  // NormalColor: 0.3529 0.3529 0.3529 1
  // HighlightedColor: 0.625 0.625 0.625 1
  // PressedColor: 0.8897 0.8897 0.8897 1
  // SelectedColor: 0.625 0.625 0.625 1
  // DisabledColor: 0.3449 0.3449 0.3449 0.502
  // ColorMultiplier: 1
  // FadeDuration: 0.1
}
