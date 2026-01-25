namespace Pinnacle;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class VectorCell {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public TextMeshProUGUI XLabel { get; private set; }
  public ValueCell XValue { get; private set; }

  public TextMeshProUGUI YLabel { get; private set; }
  public ValueCell YValue { get; private set; }

  public TextMeshProUGUI ZLabel { get; private set; }
  public ValueCell ZValue { get; private set; }

  public VectorCell(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();

    XValue = new(Container.transform);

    XLabel = UIBuilder.CreateTMPLabel(XValue.Container.transform);
    XLabel.transform.SetAsFirstSibling();
    XLabel.alignment = TextAlignmentOptions.Left;
    XLabel.SetText("X");

    XValue.InputField.textComponent.alignment = TextAlignmentOptions.Right;
    XValue.Container.GetComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(XLabel.GetPreferredValues("-99999") + new Vector2(0f, 8f));

    YValue = new(Container.transform);

    YLabel = UIBuilder.CreateTMPLabel(YValue.Container.transform);
    YLabel.transform.SetAsFirstSibling();
    YLabel.alignment = TextAlignmentOptions.Left;
    YLabel.SetText("Y");

    YValue.InputField.textComponent.alignment = TextAlignmentOptions.Right;
    YValue.Container.GetComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(YLabel.GetPreferredValues("-99999") + new Vector2(0f, 8f));

    ZValue = new(Container.transform);

    ZLabel = UIBuilder.CreateTMPLabel(ZValue.Container.transform);
    ZLabel.transform.SetAsFirstSibling();
    ZLabel.alignment = TextAlignmentOptions.Left;
    ZLabel.SetText("Z");

    ZValue.InputField.textComponent.alignment = TextAlignmentOptions.Right;
    ZValue.Container.GetComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(ZLabel.GetPreferredValues("-99999") + new Vector2(0f, 8f));
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("Vector3", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetSpacing(8f)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    return container;
  }
}
