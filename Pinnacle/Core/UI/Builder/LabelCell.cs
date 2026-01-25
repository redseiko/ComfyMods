namespace Pinnacle;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class LabelCell {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public Image Background { get; private set; }
  public TextMeshProUGUI Label { get; private set; }

  public LabelCell(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    Background = Container.GetComponent<Image>();
    Label = CreateLabel(Container.transform);
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("Container", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 4, right: 4, top: 4, bottom: 4)
        .SetSpacing(4f)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    container.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UISpriteBuilder.CreateRoundedCornerSprite(64, 64, 8))
        .SetColor(new(0.2f, 0.2f, 0.2f, 0.5f));

    container.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.Unconstrained)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    container.AddComponent<LayoutElement>()
        .SetPreferred(width: 150f)
        .SetFlexible(width: 1f);

    return container;
  }

  static TextMeshProUGUI CreateLabel(Transform parentTransform) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(parentTransform);

    label
        .SetName("Label")
        .SetAlignment(TextAlignmentOptions.Left)
        .SetText("Label");

    label.gameObject.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f);

    return label;
  }
}
