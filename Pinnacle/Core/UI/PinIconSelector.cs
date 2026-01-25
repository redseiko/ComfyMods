namespace Pinnacle;

using System;
using System.Collections.Generic;
using System.Linq;

using ComfyLib;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class PinIconSelector {
  public UnityEvent<Minimap.PinType> OnPinIconClick { get; private set; } = new();

  public GameObject Grid { get; private set; }
  public GridLayoutGroup GridLayoutGroup { get; private set; }

  public List<GameObject> Icons { get; } = [];
  public Dictionary<Minimap.PinType, GameObject> IconsByType { get; } = [];

  public PinIconSelector(Transform parentTransform) {
    Grid = CreateChildGrid(parentTransform);
    GridLayoutGroup = Grid.GetComponent<GridLayoutGroup>();

    CreateIcons(Grid.transform);
  }

  void CreateIcons(Transform parentTransform) {
    foreach (Minimap.PinType pinType in Enum.GetValues(typeof(Minimap.PinType))) {
      Sprite sprite = Minimap.m_instance.GetSprite(pinType);

      if (!sprite) {
        continue;
      }

      GameObject icon = CreateChildIcon(parentTransform);

      Icons.Add(icon);
      IconsByType[pinType] = icon;

      icon.Image()
          .SetSprite(sprite);

      Button button =
          icon.AddComponent<Button>()
              .SetNavigationMode(Navigation.Mode.None)
              .SetTargetGraphic(icon.Image())
              .SetTransition(Selectable.Transition.ColorTint)
              .SetColors(ButtonColorBlock.Value);

      button.onClick.AddListener(() => OnPinIconClick.Invoke(pinType));
    }
  }

  public void UpdateIcons(Minimap.PinType pinType) {
    string spriteName = Minimap.m_instance.GetSprite(pinType).Ref()?.name;

    foreach (Image icon in Icons.Select(i => i.Image())) {
      icon.SetColor(
          icon.sprite.name == spriteName
              ? ButtonColorBlock.Value.selectedColor
              : ButtonColorBlock.Value.normalColor);
    }
  }

  public void SetIconSize(Vector2 sizeDelta) {
    foreach (LayoutElement layout in Icons.Select(icon => icon.LayoutElement())) {
      layout.SetFlexible(width: sizeDelta.x, height: sizeDelta.y);
    }

    Grid.GetComponent<GridLayoutGroup>()
        .SetCellSize(sizeDelta);
  }

  GameObject CreateChildGrid(Transform parentTransform) {
    GameObject grid = new("PinIconSelector.Grid", typeof(RectTransform));
    grid.SetParent(parentTransform);

    grid.AddComponent<GridLayoutGroup>()
        .SetCellSize(new(25f, 25f))
        .SetSpacing(new(8f, 8f))
        .SetConstraint(GridLayoutGroup.Constraint.FixedRowCount)
        .SetConstraintCount(2)
        .SetStartAxis(GridLayoutGroup.Axis.Horizontal)
        .SetStartCorner(GridLayoutGroup.Corner.UpperLeft);

    grid.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f);

    return grid;
  }

  GameObject CreateChildIcon(Transform parentTransform) {
    GameObject icon = new("Icon", typeof(RectTransform));
    icon.SetParent(parentTransform);

    icon.AddComponent<Image>()
        .SetType(Image.Type.Simple)
        .SetPreserveAspect(true);

    icon.AddComponent<LayoutElement>()
        .SetPreferred(width: 25f, height: 25f);

    return icon;
  }

  static readonly Lazy<ColorBlock> ButtonColorBlock =
      new(() =>
        new() {
          normalColor = new Color(1f, 1f, 1f, 0.8f),
          highlightedColor = new Color(0.565f, 0.792f, 0.976f),
          disabledColor = new Color(0f, 0f, 0f, 0.5f),
          pressedColor = new Color(0.647f, 0.839f, 0.655f),
          selectedColor = new Color(1f, 0.878f, 0.51f),
          colorMultiplier = 1f,
          fadeDuration = 0.25f,
        });
}
