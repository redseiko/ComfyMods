namespace ZoneScouter;

using System;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ButtonCell {
  public GameObject Cell { get; private set; }
  public Image Background { get; private set; }
  public TMP_Text Label { get; private set; }
  public Button Button { get; private set; }

  public ButtonCell(Transform parentTransform) {
    Cell = CreateChildCell(parentTransform);
    Background = CreateChildBackground(Cell.transform).GetComponent<Image>();
    Label = CreateChildLabel(Background.transform);

    Button = Cell.AddComponent<Button>();
    Button
        .SetNavigationMode(Navigation.Mode.None)
        .SetTargetGraphic(Background)
        .SetColors(_buttonColorBlock.Value);
  }

  GameObject CreateChildCell(Transform parentTransform) {
    GameObject cell = new("Cell", typeof(RectTransform));
    cell.transform.SetParent(parentTransform, worldPositionStays: false);

    cell.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetSpacing(0f)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    cell.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.Unconstrained)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    return cell;
  }

  GameObject CreateChildBackground(Transform parentTransform) {
    GameObject background = new("Background", typeof(RectTransform));
    background.transform.SetParent(parentTransform, worldPositionStays: false);

    background.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 4, right: 4, top: 2, bottom: 2)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    background.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIBuilder.CreateRoundedCornerSprite(200, 200, 5))
        .SetColor(Color.white);

    return background;
  }

  TMP_Text CreateChildLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);
    label.name = "Label";

    label.SetAlignment(TextAlignmentOptions.Center)
        .SetText("Button");

    label.gameObject.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f);

    return label;
  }

  static readonly Lazy<ColorBlock> _buttonColorBlock =
      new(() =>
        new() {
          normalColor = new Color(0f, 0f, 0f, 0.3f),
          highlightedColor = new Color(0.565f, 0.792f, 0.976f, 0.3f),
          disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.3f),
          pressedColor = new Color(1f, 0.878f, 0.51f, 0.3f),
          selectedColor = new Color(0.647f, 0.839f, 0.655f, 0.3f),
          colorMultiplier = 1f,
          fadeDuration = 0.1f,
        });
}
