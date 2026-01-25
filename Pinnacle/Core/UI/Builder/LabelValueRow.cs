namespace Pinnacle;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class LabelValueRow {
  public GameObject Row { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public TextMeshProUGUI Label { get; private set; }
  public ValueCell Value { get; private set; }

  public LabelValueRow(Transform parentTransform) {
    Row = CreateRow(parentTransform);
    RectTransform = Row.GetComponent<RectTransform>();

    Label = CreateLabel(Row.transform);
    Value = new(Row.transform);
  }

  static GameObject CreateRow(Transform parentTransform) {
    GameObject row = new("Row", typeof(RectTransform));
    row.transform.SetParent(parentTransform, worldPositionStays: false);

    row.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 8, right: 8, top: 2, bottom: 2)
        .SetSpacing(12f)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    return row;
  }

  static TextMeshProUGUI CreateLabel(Transform parentTransform) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(parentTransform);

    label
        .SetName("Label")
        .SetAlignment(TextAlignmentOptions.Left)
        .SetText("Label");

    label.gameObject.AddComponent<LayoutElement>()
        .SetPreferred(width: 75f);

    return label;
  }
}
