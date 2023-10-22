using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static ZoneScouter.PluginConfig;

namespace ZoneScouter {
  public class SectorInfoPanel {
    public GameObject Panel { get; private set; }

    public ContentRow PositionContent { get; private set; }
    public ValueWithLabel PositionX { get; private set; }
    public ValueWithLabel PositionY { get; private set; }
    public ValueWithLabel PositionZ { get; private set; }

    public ContentRow SectorContent { get; private set; }
    public ValueWithLabel SectorXY { get; private set; }
    public ValueWithLabel SectorZdoCount { get; private set; }

    public ContentRow ZdoManagerContent { get; private set; }
    public ValueWithLabel ZdoManagerNextId { get; private set; }

    public PanelDragger PanelDragger { get; private set; }

    public SectorInfoPanel(Transform parentTransform) {
      Panel = CreatePanel(parentTransform);

      PositionContent = new(Panel.transform);

      PositionX = new(PositionContent.Row.transform);
      PositionX.Label.SetText("X");

      PositionY = new(PositionContent.Row.transform);
      PositionY.Label.SetText("Y");

      PositionZ = new(PositionContent.Row.transform);
      PositionZ.Label.SetText("Z");

      SectorContent = new(Panel.transform);

      SectorXY = new(SectorContent.Row.transform);
      SectorXY.Label.SetText("Sector");

      SectorZdoCount = new(SectorContent.Row.transform);
      SectorZdoCount.Label.SetText("ZDOs");

      ZdoManagerContent = new(Panel.transform);

      ZdoManagerNextId = new(ZdoManagerContent.Row.transform);
      ZdoManagerNextId.Label.SetText("NextId");

      SetPanelStyle();
      ToggleZDOManagerContent(ShowZDOManagerContent.Value);

      PanelDragger = Panel.AddComponent<PanelDragger>();
    }

    public void SetPanelStyle() {
      int fontSize = SectorInfoPanelFontSize.Value;

      PositionX.Label.SetFontSize(fontSize);
      PositionX.Value.SetFontSize(fontSize);
      PositionX.Value.SetColor(PositionValueXTextColor.Value);
      PositionX.FitValueToText("-00000");
      PositionX.Row.Image().SetColor(PositionValueXTextColor.Value.SetAlpha(0.1f));

      PositionY.Label.SetFontSize(fontSize);
      PositionY.Value.SetFontSize(fontSize);
      PositionY.Value.SetColor(PositionValueYTextColor.Value);
      PositionY.FitValueToText("-00000");
      PositionY.Row.Image().SetColor(PositionValueYTextColor.Value.SetAlpha(0.1f));

      PositionZ.Label.SetFontSize(fontSize);
      PositionZ.Value.SetFontSize(fontSize);
      PositionZ.Value.SetColor(PositionValueZTextColor.Value);
      PositionZ.FitValueToText("-00000");
      PositionZ.Row.Image().SetColor(PositionValueZTextColor.Value.SetAlpha(0.1f));

      SectorXY.Label.SetFontSize(fontSize);
      SectorXY.Value.SetFontSize(fontSize);
      SectorXY.Value.SetColor(PositionValueXTextColor.Value);
      SectorXY.FitValueToText("-123,-123");
      SectorXY.Row.Image().SetColor(PositionValueXTextColor.Value.SetAlpha(0.1f));

      SectorZdoCount.Label.SetFontSize(fontSize);
      SectorZdoCount.Value.SetFontSize(fontSize);
      SectorZdoCount.Value.SetColor(PositionValueYTextColor.Value);
      SectorZdoCount.FitValueToText("123456");
      SectorZdoCount.Row.Image().SetColor(PositionValueYTextColor.Value.SetAlpha(0.1f));

      ZdoManagerNextId.Label.SetFontSize(fontSize);
      ZdoManagerNextId.Value.SetFontSize(fontSize);
      ZdoManagerNextId.Value.SetColor(PositionValueZTextColor.Value);
      ZdoManagerNextId.FitValueToText("1234567890");
      ZdoManagerNextId.Row.Image().SetColor(PositionValueZTextColor.Value.SetAlpha(0.1f));
    }

    public void ToggleZDOManagerContent(bool toggleOn) {
      ZdoManagerContent.Row?.SetActive(toggleOn);
    }

    GameObject CreatePanel(Transform parentTransform) {
      GameObject panel = new("SectorInfo.Panel", typeof(RectTransform));
      panel.SetParent(parentTransform);

      panel.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: true, height: false)
          .SetPadding(left: 6, right: 6, top: 6, bottom: 6)
          .SetSpacing(6f);

      panel.AddComponent<ContentSizeFitter>()
          .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
          .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

      panel.AddComponent<Image>()
          .SetType(Image.Type.Sliced)
          .SetSprite(UIBuilder.CreateSuperellipse(200, 200, 12))
          .SetColor(SectorInfoPanelBackgroundColor.Value);

      panel.AddComponent<CanvasGroup>()
          .SetBlocksRaycasts(true);

      return panel;
    }

    public class ContentRow {
      public GameObject Row { get; private set; }

      public ContentRow(Transform parentTransform) {
        Row = CreateChildRow(parentTransform);
      }

      GameObject CreateChildRow(Transform parentTransform) {
        GameObject row = new("Row", typeof(RectTransform));
        row.SetParent(parentTransform);

        row.AddComponent<HorizontalLayoutGroup>()
            .SetChildControl(width: true, height: true)
            .SetChildForceExpand(width: true, height: false)
            .SetSpacing(6f)
            .SetChildAlignment(TextAnchor.MiddleCenter);

        return row;
      }
    }
  }
}
