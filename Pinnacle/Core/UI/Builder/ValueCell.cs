namespace Pinnacle;

using System;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ValueCell {
  public GameObject Cell { get; private set; }
  public Image Background { get; private set; }
  public TMP_InputField InputField { get; private set; }

  public ValueCell(Transform parentTransform) {
    Cell = CreateChildCell(parentTransform);
    Background = Cell.Image();

    InputField = CreateChildInputField(Cell.transform);
    InputField.SetTargetGraphic(Background);
  }

  GameObject CreateChildCell(Transform parentTransform) {
    GameObject cell = new("Cell", typeof(RectTransform));
    cell.SetParent(parentTransform);

    cell.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIBuilder.CreateRoundedCornerSprite(64, 64, 8))
        .SetColor(new(0.5f, 0.5f, 0.5f, 0.5f));

    cell.AddComponent<RectMask2D>();
    cell.AddComponent<LayoutElement>();

    return cell;
  }

  TMP_InputField CreateChildInputField(Transform parentTransform) {
    GameObject row = new("InputField", typeof(RectTransform));
    row.SetParent(parentTransform);

    row.RectTransform()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetSizeDelta(Vector2.zero)
        .SetPosition(Vector2.zero);

    TMP_Text label = UIBuilder.CreateTMPLabel(row.transform);
    label.SetName("InputField.Text");

    label.richText = false;
    label.alignment = TextAlignmentOptions.Left;
    label.text = "InputField.Text";

    TMP_InputField inputField = parentTransform.gameObject.AddComponent<TMP_InputField>();
    inputField.textComponent = label;
    inputField.textViewport = row.GetComponent<RectTransform>();
    inputField.transition = Selectable.Transition.ColorTint;
    inputField.colors = InputFieldColorBlock.Value;
    inputField.onFocusSelectAll = false;
    inputField.SetNavigationMode(Navigation.Mode.None);

    // row.AddComponent<DisableHighlightOnSelect>();

    return inputField;
  }

  static readonly Lazy<ColorBlock> InputFieldColorBlock =
      new(() =>
        new() {
          normalColor = new Color(1f, 1f, 1f, 0.9f),
          highlightedColor = new Color(0.565f, 0.792f, 0.976f),
          disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.8f),
          pressedColor = new Color(0.647f, 0.839f, 0.655f),
          selectedColor = new Color(1f, 0.878f, 0.51f),
          colorMultiplier = 1f,
          fadeDuration = 0.25f,
        });
}
