namespace Pinnacle;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class LabelCell {
  public GameObject Cell { get; private set; }
  public Image Background { get; private set; }
  public TMP_Text Label { get; private set; }

  public LabelCell(Transform parentTransform) {
    Cell = CreateChildCell(parentTransform);
    Background = Cell.Image();
    Label = CreateChildLabel(Cell.transform);
  }

  GameObject CreateChildCell(Transform parentTransform) {
    GameObject cell = new("Cell", typeof(RectTransform));
    cell.SetParent(parentTransform);

    cell.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 4, right: 4, top: 4, bottom: 4)
        .SetSpacing(4f)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    cell.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIBuilder.CreateRoundedCornerSprite(64, 64, 8))
        .SetColor(new(0.2f, 0.2f, 0.2f, 0.5f));

    cell.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.Unconstrained)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    cell.AddComponent<LayoutElement>()
        .SetPreferred(width: 150f)
        .SetFlexible(width: 1f);

    return cell;
  }

  TMP_Text CreateChildLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);
    label.SetName("Label");

    label.alignment = TextAlignmentOptions.Left;
    label.text = "Label";

    label.gameObject.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f);

    return label;
  }
}
