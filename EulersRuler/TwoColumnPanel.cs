using UnityEngine;
using UnityEngine.UI;

namespace EulersRuler {
  public class TwoColumnPanel {
     readonly Font _textFont = null;
     int _textFontSize = 18;

     GameObject _panel = null;
     GameObject _leftColumn = null;
     GameObject _rightColumn = null;

    internal TwoColumnPanel(Transform parent, Font textFont) {
      _textFont = textFont;
      CreatePanel(parent);
    }

    internal void DestroyPanel() {
      Object.Destroy(_panel);
    }

    internal void SetActive(bool active) {
      if (_panel && _panel.activeSelf != active) {
        _panel.SetActive(active);
      }
    }

    internal TwoColumnPanel SetPosition(Vector2 position) {
      _panel.GetComponent<RectTransform>().anchoredPosition = position;
      return this;
    }

    internal TwoColumnPanel SetAnchors(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot) {
      RectTransform transform = _panel.GetComponent<RectTransform>();
      transform.anchorMin = anchorMin;
      transform.anchorMax = anchorMax;
      transform.pivot = pivot;

      return this;
    }

    internal TwoColumnPanel SetFontSize(int fontSize) {
      _textFontSize = fontSize;

      foreach (Text text in _panel.GetComponentsInChildren<Text>()) {
        text.fontSize = fontSize;
      }

      return this;
    }

    void CreatePanel(Transform parent) {
      _panel = new("TwoColumnPanel", typeof(RectTransform));
      _panel.transform.SetParent(parent, worldPositionStays: false);

      RectTransform transform = _panel.GetComponent<RectTransform>();
      transform.anchorMin = Vector2.zero;
      transform.anchorMax = Vector2.zero;
      transform.pivot = Vector2.zero;
      transform.anchoredPosition = Vector2.zero;

      HorizontalLayoutGroup panelLayout = _panel.AddComponent<HorizontalLayoutGroup>();
      panelLayout.spacing = 8f;

      ContentSizeFitter panelFitter = _panel.AddComponent<ContentSizeFitter>();
      panelFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
      panelFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;

      _leftColumn = new("LeftColumn", typeof(RectTransform));
      _leftColumn.transform.SetParent(_panel.transform, worldPositionStays: false);

      VerticalLayoutGroup leftLayout = _leftColumn.AddComponent<VerticalLayoutGroup>();
      leftLayout.childControlWidth = true;
      leftLayout.childControlHeight = true;
      leftLayout.childForceExpandWidth = true;
      leftLayout.childForceExpandHeight = false;
      leftLayout.childAlignment = TextAnchor.MiddleLeft;
      leftLayout.spacing = 6f;

      ContentSizeFitter leftFitter = _leftColumn.AddComponent<ContentSizeFitter>();
      leftFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
      leftFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

      _rightColumn = new("RightColumn", typeof(RectTransform));
      _rightColumn.transform.SetParent(_panel.transform, worldPositionStays: false);

      VerticalLayoutGroup rightLayout = _rightColumn.AddComponent<VerticalLayoutGroup>();
      rightLayout.childControlWidth = true;
      rightLayout.childControlHeight = true;
      rightLayout.childForceExpandWidth = true;
      rightLayout.childForceExpandHeight = false;
      rightLayout.childAlignment = TextAnchor.MiddleRight;
      rightLayout.spacing = 6f;

      ContentSizeFitter rightFitter = _rightColumn.AddComponent<ContentSizeFitter>();
      rightFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
      rightFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public TwoColumnPanel AddPanelRow(out Text leftText, out Text rightText) {
      GameObject leftSide = new("LeftText", typeof(RectTransform));
      leftSide.transform.SetParent(_leftColumn.transform, worldPositionStays: false);

      leftText = leftSide.AddComponent<Text>();
      leftText.alignment = TextAnchor.MiddleRight;
      leftText.horizontalOverflow = HorizontalWrapMode.Overflow;
      leftText.font = _textFont;
      leftText.fontSize = _textFontSize;
      leftText.text = "LeftText";

      Outline leftOutline = leftSide.AddComponent<Outline>();
      leftOutline.effectColor = Color.black;
      leftOutline.effectDistance = new Vector2(1, -1);

      GameObject rightSide = new("RightText", typeof(RectTransform));
      rightSide.transform.SetParent(_rightColumn.transform, worldPositionStays: false);

      rightText = rightSide.AddComponent<Text>();
      rightText.alignment = TextAnchor.MiddleLeft;
      rightText.horizontalOverflow = HorizontalWrapMode.Wrap;
      rightText.font = _textFont;
      rightText.fontSize = _textFontSize;
      rightText.text = "RightText";

      Outline rightOutline = rightSide.AddComponent<Outline>();
      rightOutline.effectColor = Color.black;
      rightOutline.effectDistance = new Vector2(1, -1);

      return this;
    }
  }
}
