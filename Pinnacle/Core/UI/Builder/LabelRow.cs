namespace Pinnacle;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class LabelRow {
  public GameObject Row { get; private set; }
  public TMP_Text Label { get; private set; }

  public LabelRow(Transform parentTransform) {
    Row = CreateChildRow(parentTransform);
    Label = CreateChildLabel(Row.transform);
  }

  GameObject CreateChildRow(Transform parentTransform) {
    GameObject row = new("Row", typeof(RectTransform));
    row.SetParent(parentTransform);

    row.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 8, right: 8, top: 2, bottom: 2)
        .SetSpacing(12f)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    return row;
  }

  TMP_Text CreateChildLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);
    label.SetName("Label");

    label.alignment = TextAlignmentOptions.Left;
    label.text = "Label";

    label.gameObject.AddComponent<LayoutElement>();

    return label;
  }
}
