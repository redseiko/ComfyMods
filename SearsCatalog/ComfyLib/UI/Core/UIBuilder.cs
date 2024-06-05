namespace ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public static class UIBuilder {
  public static TextMeshProUGUI CreateTMPLabel(Transform parentTransform) {
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

  public static TextMeshProUGUI CreateTMPHeaderLabel(Transform parentTransform) {
    TextMeshProUGUI label =
        UnityEngine.Object.Instantiate(UnifiedPopup.instance.headerText, parentTransform, worldPositionStays: false);

    label.name = "Label";
    label.fontSize = 32f;
    label.richText = true;
    label.enableAutoSizing = false;
    label.textWrappingMode = TextWrappingModes.NoWrap;
    label.overflowMode = TextOverflowModes.Overflow;
    label.text = string.Empty;

    return label;
  }

  public static GameObject CreateResizer(Transform parentTransform) {
    GameObject resizer = new("Resizer", typeof(RectTransform));
    resizer.transform.SetParent(parentTransform, worldPositionStays: false);

    resizer.AddComponent<LayoutElement>()
        .SetIgnoreLayout(true);

    resizer.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.right)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.right)
        .SetSizeDelta(new(42.5f, 42.5f))
        .SetPosition(new(10f, -10f));

    resizer.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("button"))
        .SetColor(new(1f, 1f, 1f, 0.95f));

    resizer.AddComponent<Shadow>()
        .SetEffectDistance(new(2f, -2f));

    resizer.AddComponent<CanvasGroup>()
        .SetAlpha(0f);

    TextMeshProUGUI icon = CreateTMPLabel(resizer.transform);
    icon.SetName("Icon");

    icon.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetSizeDelta(Vector2.zero);

    icon
        .SetAlignment(TextAlignmentOptions.Center)
        .SetFontSize(24f)
        .SetOverflowMode(TextOverflowModes.Overflow)
        .SetText("<rotate=-45>\u2194</rotate>");

    return resizer;
  }
}
