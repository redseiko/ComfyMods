namespace ZoneScouter;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public sealed class ValueWithLabel {
  public GameObject Row { get; private set; }
  public TMP_Text Value { get; private set; }
  public TMP_Text Label { get; private set; }

  public ValueWithLabel(Transform parentTransform) {
    Row = CreateChildRow(parentTransform);
    Value = CreateChildValue(Row.transform);
    Label = CreateChildLabel(Row.transform);
  }

  public ValueWithLabel FitValueToText(string longestText) {
    Value.GetOrAddComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(width: Value.GetPreferredValues(longestText).x);

    return this;
  }

  GameObject CreateChildRow(Transform parentTransform) {
    GameObject row = new("Row", typeof(RectTransform));
    row.SetParent(parentTransform);

    row.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 8, right: 8, top: 4, bottom: 4)
        .SetSpacing(8f);

    row.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIBuilder.CreateRoundedCornerSprite(200, 200, 5))
        .SetColor(new(0f, 0f, 0f, 0.1f));

    return row;
  }

  TMP_Text CreateChildValue(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);
    label.name = "Value";

    label
        .SetFontSize(SectorInfoPanelFontSize.Value)
        .SetAlignment(TextAlignmentOptions.TopRight)
        .SetText("0");

    label.gameObject.AddComponent<LayoutElement>()
        .SetPreferred(width: 50f);

    return label;
  }

  TMP_Text CreateChildLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);
    label.name = "Label";

    label
        .SetFontSize(SectorInfoPanelFontSize.Value)
        .SetAlignment(TextAlignmentOptions.TopLeft)
        .SetText("X");

    return label;
  }
}
