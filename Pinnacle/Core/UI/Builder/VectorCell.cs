namespace Pinnacle;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class VectorCell {
  public GameObject Cell { get; private set; }

  public TMP_Text XLabel { get; private set; }
  public ValueCell XValue { get; private set; }

  public TMP_Text YLabel { get; private set; }
  public ValueCell YValue { get; private set; }

  public TMP_Text ZLabel { get; private set; }
  public ValueCell ZValue { get; private set; }

  public VectorCell(Transform parentTransform) {
    Cell = CreateChildCell(parentTransform);

    XValue = new(Cell.transform);

    XLabel = UIBuilder.CreateTMPLabel(XValue.Cell.transform);
    XLabel.transform.SetAsFirstSibling();
    XLabel.alignment = TextAlignmentOptions.Left;
    XLabel.SetText("X");

    XValue.InputField.textComponent.alignment = TextAlignmentOptions.Right;
    XValue.Cell.GetComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(XLabel.GetPreferredValues("-99999") + new Vector2(0f, 8f));

    YValue = new(Cell.transform);

    YLabel = UIBuilder.CreateTMPLabel(YValue.Cell.transform);
    YLabel.transform.SetAsFirstSibling();
    YLabel.alignment = TextAlignmentOptions.Left;
    YLabel.SetText("Y");

    YValue.InputField.textComponent.alignment = TextAlignmentOptions.Right;
    YValue.Cell.GetComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(YLabel.GetPreferredValues("-99999") + new Vector2(0f, 8f));

    ZValue = new(Cell.transform);

    ZLabel = UIBuilder.CreateTMPLabel(ZValue.Cell.transform);
    ZLabel.transform.SetAsFirstSibling();
    ZLabel.alignment = TextAlignmentOptions.Left;
    ZLabel.SetText("Z");

    ZValue.InputField.textComponent.alignment = TextAlignmentOptions.Right;
    ZValue.Cell.GetComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(ZLabel.GetPreferredValues("-99999") + new Vector2(0f, 8f));
  }

  GameObject CreateChildCell(Transform parentTransform) {
    GameObject cell = new("Cell", typeof(RectTransform));
    cell.SetParent(parentTransform);

    cell.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetSpacing(8f)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    return cell;
  }
}
