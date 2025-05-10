namespace Pinnacle;

using System;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ToggleCell {
  public GameObject Cell { get; private set; }
  public TMP_Text Label { get; private set; }
  public Image Checkbox { get; private set; }
  public Image Checkmark { get; private set; }
  public Toggle Toggle { get; private set; }

  public ToggleCell(Transform parentTransform) {
    Cell = CreateChildCell(parentTransform);
    Label = CreateChildLabel(Cell.transform);
    Checkbox = CreateChildCheckbox(Cell.transform).Image();
    Checkmark = CreateChildCheckmark(Checkbox.transform).Image();

    Toggle = Cell.AddComponent<Toggle>();
    Toggle.SetTransition(Selectable.Transition.ColorTint)
        .SetNavigationMode(Navigation.Mode.None)
        .SetTargetGraphic(Checkbox)
        .SetColors(ToggleColorBlock.Value);
    Toggle.graphic = Checkmark;
    Toggle.toggleTransition = Toggle.ToggleTransition.Fade;
  }

  GameObject CreateChildCell(Transform parentTransform) {
    GameObject cell = new("Cell", typeof(RectTransform));
    cell.SetParent(parentTransform);

    cell.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 8, right: 8, top: 4, bottom: 4)
        .SetSpacing(8f)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    cell.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIBuilder.CreateRoundedCornerSprite(64, 64, 8))
        .SetColor(new(0.5f, 0.5f, 0.5f, 0.5f));

    cell.AddComponent<Shadow>()
        .SetEffectDistance(new(2, -2));

    cell.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    return cell;
  }

  GameObject CreateChildCheckbox(Transform parentTransform) {
    GameObject checkbox = new("Toggle.Checkbox", typeof(RectTransform));
    checkbox.SetParent(parentTransform);

    checkbox.AddComponent<Image>()
        .SetType(Image.Type.Filled)
        .SetSprite(UIBuilder.CreateRoundedCornerSprite(64, 64, 10))
        .SetColor(new(0.5f, 0.5f, 0.5f, 0.9f))
        .SetPreserveAspect(true);

    checkbox.AddComponent<Shadow>()
        .SetEffectDistance(new(1, -1));

    checkbox.AddComponent<GridLayoutGroup>()
        .SetCellSize(new(12f, 12f))
        .SetPadding(left: 4, right: 4, top: 4, bottom: 4)
        .SetConstraint(GridLayoutGroup.Constraint.FixedColumnCount)
        .SetConstraintCount(1)
        .SetStartAxis(GridLayoutGroup.Axis.Horizontal)
        .SetStartCorner(GridLayoutGroup.Corner.UpperLeft);

    checkbox.AddComponent<LayoutElement>()
        .SetPreferred(width: 16f, height: 16f);

    return checkbox;
  }

  GameObject CreateChildCheckmark(Transform parentTransform) {
    GameObject checkmark = new("Toggle.Checkmark", typeof(RectTransform));
    checkmark.SetParent(parentTransform);

    checkmark.AddComponent<Image>()
        .SetType(Image.Type.Filled)
        .SetSprite(UIBuilder.CreateRoundedCornerSprite(64, 64, 6))
        .SetColor(new(0.565f, 0.792f, 0.976f, 0.9f))
        .SetPreserveAspect(true);

    checkmark.AddComponent<Shadow>()
        .SetEffectDistance(new(1, -1));

    checkmark.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f, height: 1f);

    return checkmark;
  }

  TMP_Text CreateChildLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);
    label.SetName("Toggle.Label");

    label.alignment = TextAlignmentOptions.Center;
    label.text = "Toggle";

    return label;
  }

  static readonly Lazy<ColorBlock> ToggleColorBlock =
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
