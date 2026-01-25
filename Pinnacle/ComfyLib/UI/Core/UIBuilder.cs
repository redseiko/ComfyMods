namespace ComfyLib;

using GUIFramework;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public static class UIBuilder {
  public static TextMeshProUGUI CreateTMPLabel(Transform parentTransform) {
    TextMeshProUGUI label =
        UnityEngine.Object.Instantiate(UnifiedPopup.instance.bodyText, parentTransform, worldPositionStays: false);

    label
        .SetName("Label")
        .SetFontSize(16f)
        .SetRichText(true)
        .SetColor(Color.white)
        .SetEnableAutoSizing(false)
        .SetTextWrappingMode(TextWrappingModes.NoWrap)
        .SetOverflowMode(TextOverflowModes.Overflow)
        .SetText(string.Empty);

    return label;
  }

  public static TextMeshProUGUI CreateTMPHeaderLabel(Transform parentTransform) {
    TextMeshProUGUI label =
        UnityEngine.Object.Instantiate(UnifiedPopup.instance.headerText, parentTransform, worldPositionStays: false);

    label
        .SetName("Label")
        .SetFontSize(32f)
        .SetRichText(true)
        .SetEnableAutoSizing(false)
        .SetTextWrappingMode(TextWrappingModes.NoWrap)
        .SetOverflowMode(TextOverflowModes.Overflow)
        .SetText(string.Empty);

    return label;
  }

  public static GameObject CreateRowSpacer(Transform parentTransform) {
    GameObject spacer = new($"{parentTransform.name}.Spacer", typeof(RectTransform));
    spacer.SetParent(parentTransform);

    spacer.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f);

    return spacer;
  }

  public static GuiInputField CreateInputField(Transform parentTransform) {
    GuiInputField inputField =
        UnityEngine.Object.Instantiate(TextInput.instance.m_inputField, parentTransform, worldPositionStays: false);

    inputField.SetName("Value");

    inputField.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(120f, 40f));

    inputField.GetComponentInChildren<RectMask2D>()
        .SetPadding(Vector4.zero);

    return inputField;
  }
}
