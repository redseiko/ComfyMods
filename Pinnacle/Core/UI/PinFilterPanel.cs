namespace Pinnacle;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public sealed class PinFilterPanel {
  public GameObject Panel { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public PanelDragger PanelDragger { get; private set; }

  public PinIconSelector PinIconSelector { get; private set; }

  public PinFilterPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();

    PanelDragger = CreateDragger(Panel.transform).AddComponent<PanelDragger>();
    PanelDragger.TargetRectTransform = RectTransform;

    PinIconSelector = new(Panel.transform);

    PinIconSelector.GridLayoutGroup
        .SetConstraint(GridLayoutGroup.Constraint.FixedColumnCount)
        .SetConstraintCount(2)
        .SetStartAxis(GridLayoutGroup.Axis.Vertical);

    PinIconSelector.OnPinIconClick.AddListener(ProcessOnPinIconClick);

    SetPanelStyle();
  }

  void ProcessOnPinIconClick(Minimap.PinType pinType) {
    Minimap.m_instance.Ref()?.ToggleIconFilter(pinType);
  }

  public void SetPanelStyle() {
    PinIconSelector.GridLayoutGroup.SetCellSize(
        new(PinFilterPanelGridIconSize.Value, PinFilterPanelGridIconSize.Value));
  }

  public void UpdatePinIconFilters() {
    foreach (Minimap.PinType pinType in PinIconSelector.IconsByType.Keys) {
      PinIconSelector.IconsByType[pinType]
          .Image()
          .Ref()?
          .SetColor(Minimap.m_instance.m_visibleIconTypes[(int) pinType] ? Color.white : Color.gray);
    }
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = new("PinFilterPanel", typeof(RectTransform));
    panel.transform.SetParent(parentTransform, worldPositionStays: false);

    panel.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 8, right: 8, top: 8, bottom: 8)
        .SetSpacing(4f);

    panel.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    panel.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UISpriteBuilder.CreateSuperellipse(200, 200, 10))
        .SetColor(new(0f, 0f, 0f, 0.9f));

    panel.AddComponent<Canvas>();
    panel.AddComponent<GraphicRaycaster>();

    panel.AddComponent<CanvasGroup>()
        .SetBlocksRaycasts(true);

    return panel;
  }

  static GameObject CreateDragger(Transform parentTransform) {
    GameObject dragger = new("Dragger", typeof(RectTransform));
    dragger.transform.SetParent(parentTransform, worldPositionStays: false);

    dragger.AddComponent<LayoutElement>()
        .SetIgnoreLayout(true);

    dragger.RectTransform()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetSizeDelta(Vector2.zero);

    dragger.AddComponent<Image>()
        .SetColor(Color.clear);

    return dragger;
  }
}
