using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static ZoneScouter.PluginConfig;

namespace ZoneScouter {
  public sealed class SectorInfoPanel {
    public GameObject Panel { get; private set; }

    public ContentRow PositionContent { get; private set; }
    public ValueWithLabel PositionX { get; private set; }
    public ValueWithLabel PositionY { get; private set; }
    public ValueWithLabel PositionZ { get; private set; }

    public ButtonCell CopyPositionButton { get; private set; }

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

      CreateCopyPositionButton();

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

    void CreateCopyPositionButton() {
      CopyPositionButton = new(PositionContent.Row.transform);

      CopyPositionButton.Cell.AddComponent<LayoutElement>()
          .SetIgnoreLayout(true);

      CopyPositionButton.Cell.GetComponent<RectTransform>()
          .SetAnchorMin(new(1f, 0.5f))
          .SetAnchorMax(new(1f, 0.5f))
          .SetPivot(new(0f, 0.5f))
          .SetPosition(new(10f, 0f))
          .SetSizeDelta(new(80f, 0f));

      CopyPositionButton.Label.text = "Copy";
      CopyPositionButton.Button.onClick.AddListener(CopyPositionToClipboard);
    }

    void CopyPositionToClipboard() {
      string text =
          ZInput.instance.Input_GetKey(KeyCode.LeftShift, false)
              ? $"Position (XZY): {PositionX.Value.text} {PositionZ.Value.text} {PositionY.Value.text}"
              : $"Position (XYZ): {PositionX.Value.text} {PositionY.Value.text} {PositionZ.Value.text}";

      GUIUtility.systemCopyBuffer = text;
      Chat.instance.AddString($"Copied to clipboard: {text}");
    }

    public void ToggleCopyButtons(bool toggleOn) {
      CopyPositionButton.Cell.gameObject.SetActive(toggleOn);
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

    public sealed class ContentRow {
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
