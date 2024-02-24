namespace ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ListItemCell {
  public GameObject Cell { get; private set; }
  public LayoutElement Layout { get; private set; }
  public Image Background { get; private set; }
  public TMP_Text Label { get; private set; }
  public Button Button { get; private set; }

  public ListItemCell(Transform parentTransform) {
    Cell = CreateChildCell(parentTransform);
    Layout = Cell.GetComponent<LayoutElement>();
    Background = Cell.GetComponent<Image>();
    Label = CreateChildLabel(Cell.transform);
    Button = CreateChildButton(Cell, Background);
  }

  GameObject CreateChildCell(Transform parentTransform) {
    GameObject cell = new("ListItem", typeof(RectTransform));
    cell.transform.SetParent(parentTransform, worldPositionStays: false);

    cell.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.zero)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    cell.AddComponent<Image>()
        .SetType(Image.Type.Filled)
        .SetColor(Color.white);

    cell.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(height: 30f);

    return cell;
  }

  TMP_Text CreateChildLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);

    label.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(-8f, -4f));

    label.text = "ListItem";
    label.fontSize = 18f;
    label.alignment = TextAlignmentOptions.Left;
    label.textWrappingMode = TextWrappingModes.NoWrap;
    label.overflowMode = TextOverflowModes.Overflow;

    return label;
  }

  Button CreateChildButton(GameObject cell, Graphic graphic) {
    Button button = cell.AddComponent<Button>();

    button
        .SetTargetGraphic(graphic)
        .SetTransition(Selectable.Transition.ColorTint)
        .SetColors(ListItemColors);

    return button;
  }

  public static readonly ColorBlock ListItemColors =
      new() {
        normalColor = Color.clear,
        highlightedColor = new(1f, 0.75f, 0f),
        selectedColor = new(1f, 0.75f, 0f),
        pressedColor = new(1f, 0.67f, 0.11f),
        disabledColor = Color.gray,
        colorMultiplier = 1f,
        fadeDuration = 0.15f,
      };
}
