using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static Pinnacle.PluginConfig;

namespace Pinnacle {
  public sealed class PinFilterPanel {
    public GameObject Panel { get; private set; }
    public PanelDragger PanelDragger { get; private set; }

    public PinIconSelector PinIconSelector { get; private set; }

    public PinFilterPanel(Transform parentTransform) {
      Panel = CreateChildPanel(parentTransform);

      PanelDragger = CreateChildPanelDragger(Panel.transform).AddComponent<PanelDragger>();
      PanelDragger.TargetRectTransform = Panel.RectTransform();

      PinIconSelector = new(Panel.transform);

      PinIconSelector.GridLayoutGroup
          .SetConstraint(GridLayoutGroup.Constraint.FixedColumnCount)
          .SetConstraintCount(2)
          .SetStartAxis(GridLayoutGroup.Axis.Vertical);

      PinIconSelector.OnPinIconClicked += ProcessOnPinIconClicked;

      SetPanelStyle();
    }

    void ProcessOnPinIconClicked(object sender, Minimap.PinType pinType) {
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

    GameObject CreateChildPanel(Transform parentTransform) {
      GameObject panel = new("PinFilter.Panel", typeof(RectTransform));
      panel.SetParent(parentTransform);

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
          .SetSprite(UIBuilder.CreateSuperellipse(200, 200, 10))
          .SetColor(new(0f, 0f, 0f, 0.9f));

      panel.AddComponent<CanvasGroup>()
          .SetBlocksRaycasts(true);

      return panel;
    }

    GameObject CreateChildPanelDragger(Transform parentTransform) {
      GameObject dragger = new("Dragger", typeof(RectTransform));
      dragger.SetParent(parentTransform);

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
}
