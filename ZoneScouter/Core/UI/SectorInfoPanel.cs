namespace ZoneScouter;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public sealed class SectorInfoPanel {
  public GameObject Panel { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public Image Background { get; private set; }

  public ContentRow PositionContent { get; private set; }
  public ValueWithLabel PositionX { get; private set; }
  public ValueWithLabel PositionY { get; private set; }
  public ValueWithLabel PositionZ { get; private set; }

  public ButtonCell CopyPositionButton { get; private set; }

  public ContentRow SectorContent { get; private set; }
  public ValueWithLabel SectorXY { get; private set; }
  public ValueWithLabel SectorZdoCount { get; private set; }

  public ContentRow ZDOManagerContent { get; private set; }
  public ValueWithLabel ZDOManagerNextId { get; private set; }

  public PanelDragger PanelDragger { get; private set; }

  public SectorInfoPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    Background = Panel.GetComponent<Image>();

    PositionContent = new(Panel.transform);

    PositionX = new(PositionContent.Row.transform);
    PositionX.Label.SetText("X");

    PositionY = new(PositionContent.Row.transform);
    PositionY.Label.SetText("Y");

    PositionZ = new(PositionContent.Row.transform);
    PositionZ.Label.SetText("Z");

    CopyPositionButton = CreateCopyPositionButton(PositionContent.Row.transform);

    SectorContent = new(Panel.transform);

    SectorXY = new(SectorContent.Row.transform);
    SectorXY.Label.SetText("Sector");

    SectorZdoCount = new(SectorContent.Row.transform);
    SectorZdoCount.Label.SetText("ZDOs");

    ZDOManagerContent = new(Panel.transform);

    ZDOManagerNextId = new(ZDOManagerContent.Row.transform);
    ZDOManagerNextId.Label.SetText("NextId");

    SetPanelStyle();

    ToggleSectorContent(ShowSectorContent.Value);
    ToggleZDOManagerContent(ShowZDOManagerContent.Value);

    PanelDragger = Panel.AddComponent<PanelDragger>();
    PanelDragger.TargetRectTransform = Panel.GetComponent<RectTransform>();
  }

  public void SetPanelStyle() {
    Background.SetColor(SectorInfoPanelBackgroundColor.Value);

    int fontSize = SectorInfoPanelFontSize.Value;

    PositionX.Label.SetFontSize(fontSize);
    PositionX.Value.SetFontSize(fontSize);
    PositionX.Value.SetColor(PositionValueXTextColor.Value);
    PositionX.FitValueToText("-00000");
    PositionX.Background.SetColor(PositionValueXBackgroundColor.Value);

    PositionY.Label.SetFontSize(fontSize);
    PositionY.Value.SetFontSize(fontSize);
    PositionY.Value.SetColor(PositionValueYTextColor.Value);
    PositionY.FitValueToText("-00000");
    PositionY.Background.SetColor(PositionValueYBackgroundColor.Value);

    PositionZ.Label.SetFontSize(fontSize);
    PositionZ.Value.SetFontSize(fontSize);
    PositionZ.Value.SetColor(PositionValueZTextColor.Value);
    PositionZ.FitValueToText("-00000");
    PositionZ.Background.SetColor(PositionValueZBackgroundColor.Value);

    SectorXY.Label.SetFontSize(fontSize);
    SectorXY.Value.SetFontSize(fontSize);
    SectorXY.Value.SetColor(PositionValueXTextColor.Value);
    SectorXY.FitValueToText("-123,-123");
    SectorXY.Background.SetColor(PositionValueXBackgroundColor.Value);

    SectorZdoCount.Label.SetFontSize(fontSize);
    SectorZdoCount.Value.SetFontSize(fontSize);
    SectorZdoCount.Value.SetColor(PositionValueYTextColor.Value);
    SectorZdoCount.FitValueToText("123456");
    SectorZdoCount.Background.SetColor(PositionValueYBackgroundColor.Value);

    ZDOManagerNextId.Label.SetFontSize(fontSize);
    ZDOManagerNextId.Value.SetFontSize(fontSize);
    ZDOManagerNextId.Value.SetColor(PositionValueZTextColor.Value);
    ZDOManagerNextId.FitValueToText("1234567890");
    ZDOManagerNextId.Background.SetColor(PositionValueZBackgroundColor.Value);
  }

  public void ToggleMenuComponents(bool toggleOn) {
    PanelDragger.enabled = toggleOn;
    CopyPositionButton.Cell.gameObject.SetActive(toggleOn);
  }

  public void ToggleSectorContent(bool toggleOn) {
    SectorContent.Row.SetActive(toggleOn);
  }

  public void ToggleZDOManagerContent(bool toggleOn) {
    ZDOManagerContent.Row.SetActive(toggleOn);
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = new("SectorInfo.Panel", typeof(RectTransform));
    panel.transform.SetParent(parentTransform, worldPositionStays: false);

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
        .SetSprite(UISpriteBuilder.CreateSuperellipse(200, 200, 12))
        .SetColor(SectorInfoPanelBackgroundColor.Value);

    panel.AddComponent<CanvasGroup>()
        .SetBlocksRaycasts(true);

    return panel;
  }

  static ButtonCell CreateCopyPositionButton(Transform parentTransform) {
    ButtonCell buttonCell = new(parentTransform);

    buttonCell.Cell.AddComponent<LayoutElement>()
        .SetIgnoreLayout(true);

    buttonCell.Cell.GetComponent<RectTransform>()
        .SetAnchorMin(new(1f, 0.5f))
        .SetAnchorMax(new(1f, 0.5f))
        .SetPivot(new(0f, 0.5f))
        .SetSizeDelta(new(80f, 0f))
        .SetPosition(new(10f, 0f));

    buttonCell.Label.text = "Copy";

    return buttonCell;
  }

  public sealed class ContentRow {
    public GameObject Row { get; private set; }

    public ContentRow(Transform parentTransform) {
      Row = CreateChildRow(parentTransform);
    }

    static GameObject CreateChildRow(Transform parentTransform) {
      GameObject row = new("Row", typeof(RectTransform));
      row.transform.SetParent(parentTransform, worldPositionStays: false);

      row.AddComponent<HorizontalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: true, height: false)
          .SetSpacing(6f)
          .SetChildAlignment(TextAnchor.MiddleCenter);

      return row;
    }
  }
}
