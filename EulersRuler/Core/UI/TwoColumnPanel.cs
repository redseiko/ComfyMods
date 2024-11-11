namespace EulersRuler;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class TwoColumnPanel {
  int _textFontSize = 18;

  public GameObject Panel { get; private set; }

  GameObject _leftColumn = null;
  GameObject _rightColumn = null;

  internal TwoColumnPanel(Transform parent) {
    CreatePanel(parent);
  }

  internal void DestroyPanel() {
    Object.Destroy(Panel);
  }

  internal void SetActive(bool active) {
    if (Panel && Panel.activeSelf != active) {
      Panel.SetActive(active);
    }
  }

  internal TwoColumnPanel SetPosition(Vector2 position) {
    Panel.GetComponent<RectTransform>().anchoredPosition = position;
    return this;
  }

  internal TwoColumnPanel SetAnchors(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot) {
    RectTransform transform = Panel.GetComponent<RectTransform>();
    transform.anchorMin = anchorMin;
    transform.anchorMax = anchorMax;
    transform.pivot = pivot;

    return this;
  }

  public TwoColumnPanel SetFontSize(int fontSize) {
    _textFontSize = fontSize;

    foreach (TMP_Text label in Panel.GetComponentsInChildren<TMP_Text>()) {
      SetFontSize(label, fontSize);
    }

    return this;
  }

  static void SetFontSize(TMP_Text label, int fontSize) {
    label.fontSize = fontSize;
    label.rectTransform.sizeDelta = label.GetPreferredValues("123") + new Vector2(0f, 4f);
  }

  void CreatePanel(Transform parent) {
    Panel = new("TwoColumnPanel", typeof(RectTransform));
    Panel.transform.SetParent(parent, worldPositionStays: false);

    Panel.GetComponent<RectTransform>()
        .SetSizeDelta(Vector2.zero);

    Panel.AddComponent<HorizontalLayoutGroup>()
        .SetSpacing(6f)
        .SetChildAlignment(TextAnchor.UpperCenter);

    Panel.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    _leftColumn = new("LeftColumn", typeof(RectTransform));
    _leftColumn.transform.SetParent(Panel.transform, worldPositionStays: false);

    _leftColumn.GetComponent<RectTransform>()
        .SetSizeDelta(Vector2.zero);

    _leftColumn.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: false)
        .SetChildForceExpand(width: true, height: false)
        .SetChildAlignment(TextAnchor.MiddleLeft)
        .SetSpacing(6f);

    _leftColumn.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    _rightColumn = new("RightColumn", typeof(RectTransform));
    _rightColumn.transform.SetParent(Panel.transform, worldPositionStays: false);

    _rightColumn.GetComponent<RectTransform>()
        .SetSizeDelta(Vector2.zero);

    _rightColumn.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: false)
        .SetChildForceExpand(width: true, height: false)
        .SetChildAlignment(TextAnchor.MiddleLeft)
        .SetSpacing(6f);

    _rightColumn.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);
  }

  public TwoColumnPanel AddPanelRow(out TMP_Text leftText, out TMP_Text rightText) {
    leftText = UIBuilder.CreateTMPLabel(_leftColumn.transform);
    leftText.name = "LeftText";
    leftText.alignment = TextAlignmentOptions.Right;
    SetFontSize(leftText, _textFontSize);

    rightText = UIBuilder.CreateTMPLabel(_rightColumn.transform);
    rightText.name = "RightText";
    rightText.alignment = TextAlignmentOptions.Left;
    SetFontSize(rightText, _textFontSize);

    return this;
  }

  public LabelRow AddLabelRow() {
    LabelRow row = new(_leftColumn.transform, _rightColumn.transform);

    row.LeftLabel.alignment = TextAlignmentOptions.Right;
    SetFontSize(row.LeftLabel, _textFontSize);

    row.RightLabel.alignment = TextAlignmentOptions.Left;
    SetFontSize(row.RightLabel, _textFontSize);

    return row;
  }

  public sealed class LabelRow {
    public TextMeshProUGUI LeftLabel { get; }
    public TextMeshProUGUI RightLabel { get; }

    public LabelRow(Transform leftParentTransform, Transform rightParentTransform) {
      LeftLabel = UIBuilder.CreateTMPLabel(leftParentTransform);
      LeftLabel.name = "LeftLabel";

      RightLabel = UIBuilder.CreateTMPLabel(rightParentTransform);
      RightLabel.name = "RightLabel";
    }

    public void SetActive(bool active) {
      LeftLabel.gameObject.SetActive(active);
      RightLabel.gameObject.SetActive(active);
    }
  }
}
