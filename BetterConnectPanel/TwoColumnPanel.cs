using UnityEngine;
using UnityEngine.UI;

namespace BetterConnectPanel {
  public class TwoColumnPanel {
    public GameObject Panel { get; private set; }
    public GameObject LeftColumn { get; private set; }
    public GameObject RightColumn { get; private set; }

    private readonly Font _textFont;
    private int _textFontSize;

    public TwoColumnPanel(string name, Transform parent, Font textFont, int textFontSize) {
      _textFont = textFont;
      _textFontSize = textFontSize;

      CreatePanel(name, parent);
    }

    public void DestroyPanel() {
      Object.Destroy(Panel);
    }

    public TwoColumnPanel SetActive(bool active) {
      if (Panel && Panel.activeSelf != active) {
        Panel.SetActive(active);
      }

      return this;
    }

    public TwoColumnPanel SetPosition(Vector2 position) {
      if (Panel && Panel.TryGetComponent(out RectTransform transform)) {
        transform.anchoredPosition = position;
      }

      return this;
    }

    public TwoColumnPanel SetTextFontSize(int textFontSize) {
      _textFontSize = textFontSize;

      foreach (Text text in Panel.GetComponentsInChildren<Text>()) {
        text.fontSize = textFontSize;
      }

      return this;
    }

    public TwoColumnPanel SetBackgroundColor(Color color) {
      if (Panel && Panel.TryGetComponent(out Image image)) {
        image.color = color;
      }

      return this;
    }

    void CreatePanel(string name, Transform parent) {
      Panel = new(name, typeof(RectTransform));
      Panel.transform.SetParent(parent, worldPositionStays: false);

      RectTransform transform = Panel.GetComponent<RectTransform>();
      transform.anchorMin = Vector2.zero;
      transform.anchorMax = Vector2.zero;
      transform.pivot = Vector2.zero;
      transform.anchoredPosition = Vector2.zero;

      Image image = Panel.AddComponent<Image>();
      image.color = new Color32(0, 0, 0, 48);

      HorizontalLayoutGroup panelLayout = Panel.AddComponent<HorizontalLayoutGroup>();
      panelLayout.padding = new RectOffset(5, 5, 5, 5);
      panelLayout.spacing = 10f;

      ContentSizeFitter panelFitter = Panel.AddComponent<ContentSizeFitter>();
      panelFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
      panelFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // MinSize;

      LeftColumn = new("LeftColumn", typeof(RectTransform));
      LeftColumn.transform.SetParent(Panel.transform, worldPositionStays: false);

      VerticalLayoutGroup leftLayout = LeftColumn.AddComponent<VerticalLayoutGroup>();
      leftLayout.childControlWidth = true;
      leftLayout.childControlHeight = true;
      leftLayout.childForceExpandWidth = true;
      leftLayout.childForceExpandHeight = false;
      leftLayout.childAlignment = TextAnchor.MiddleLeft;
      leftLayout.spacing = 4f;

      ContentSizeFitter leftFitter = LeftColumn.AddComponent<ContentSizeFitter>();
      leftFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
      leftFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

      RightColumn = new("RightColumn", typeof(RectTransform));
      RightColumn.transform.SetParent(Panel.transform, worldPositionStays: false);

      VerticalLayoutGroup rightLayout = RightColumn.AddComponent<VerticalLayoutGroup>();
      rightLayout.childControlWidth = true;
      rightLayout.childControlHeight = true;
      rightLayout.childForceExpandWidth = true;
      rightLayout.childForceExpandHeight = false;
      rightLayout.childAlignment = TextAnchor.MiddleRight;
      rightLayout.spacing = 4f;

      ContentSizeFitter rightFitter = RightColumn.AddComponent<ContentSizeFitter>();
      rightFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
      rightFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public TwoColumnPanel AddPanelRow(string leftName, string rightName, out PanelRow panelRow) {
      panelRow = new PanelRow(leftName, rightName);
      return AddPanelRow(panelRow);
    }

    public TwoColumnPanel AddPanelRow(PanelRow panelRow) {
      panelRow.LeftText.transform.SetParent(LeftColumn.transform, worldPositionStays: false);
      panelRow.LeftText.font = _textFont;
      panelRow.LeftText.fontSize = _textFontSize;

      panelRow.RightText.transform.SetParent(RightColumn.transform, worldPositionStays: false);
      panelRow.RightText.font = _textFont;
      panelRow.RightText.fontSize = _textFontSize;

      return this;
    }

    public class PanelRow {
      public Text LeftText { get; }
      public Text RightText { get; }

      public PanelRow(string leftName, string rightName) {
        GameObject leftSide = new(leftName, typeof(RectTransform));

        LeftText = leftSide.AddComponent<Text>();
        LeftText.alignment = TextAnchor.MiddleRight;
        LeftText.horizontalOverflow = HorizontalWrapMode.Overflow;
        LeftText.text = leftName;

        Outline leftOutline = leftSide.AddComponent<Outline>();
        leftOutline.effectColor = Color.black;
        leftOutline.effectDistance = new Vector2(1, -1);

        GameObject rightSide = new(rightName, typeof(RectTransform));

        RightText = rightSide.AddComponent<Text>();
        RightText.alignment = TextAnchor.MiddleLeft;
        RightText.horizontalOverflow = HorizontalWrapMode.Wrap;
        RightText.text = rightName;

        Outline rightOutline = rightSide.AddComponent<Outline>();
        rightOutline.effectColor = Color.black;
        rightOutline.effectDistance = new Vector2(1, -1);
      }
    }
  }
}
