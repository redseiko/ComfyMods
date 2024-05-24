namespace ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ButtonCell {
  public GameObject Cell { get; private set; }
  public TMP_Text Label { get; private set; }
  public Button Button { get; private set; }

  public ButtonCell(Transform parentTransform) {
    Cell = CreateChildCell(parentTransform);
    Label = CreateChildLabel(Cell.transform);
    Button = CreateChildButton(Cell);
  }

  static GameObject CreateChildCell(Transform parentTransform) {
    GameObject cell = new("Button", typeof(RectTransform));
    cell.transform.SetParent(parentTransform, worldPositionStays: false);

    cell.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("button"))
        .SetColor(Color.white);

    cell.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(120f, 45f));

    return cell;
  }

  static TMP_Text CreateChildLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);

    label
        .SetFontSize(16f)
        .SetAlignment(TextAlignmentOptions.Center)
        .SetText("Button");

    label.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    return label;
  }

  static Button CreateChildButton(GameObject cell) {
    Button button = cell.AddComponent<Button>();

    button
        .SetTransition(Selectable.Transition.SpriteSwap)
        .SetSpriteState(
            new SpriteState() {
              disabledSprite = UIResources.GetSprite("button_disabled"),
              highlightedSprite = UIResources.GetSprite("button_highlight"),
              pressedSprite = UIResources.GetSprite("button_pressed"),
              selectedSprite = UIResources.GetSprite("button_highlight")
            });

    return button;
  }
}
