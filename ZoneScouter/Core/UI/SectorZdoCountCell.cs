namespace ZoneScouter;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public sealed class SectorZdoCountCell {
  public GameObject Cell { get; private set; }

  public Image ZdoCountBackground { get; private set; }
  public TMP_Text ZdoCount { get; private set; }

  public Image SectorBackground { get; private set; }
  public TMP_Text Sector { get; private set; }

  public SectorZdoCountCell(Transform parentTransform) {
    Cell = CreateChildCell(parentTransform);

    ZdoCountBackground = CreateChildBackground(Cell.transform).Image();
    ZdoCount = CreateChildLabel(ZdoCountBackground.transform);

    SectorBackground = CreateChildBackground(Cell.transform).Image();
    Sector = CreateChildLabel(SectorBackground.transform);

    SetCellStyle(setPreferredWidth: true);
  }

  public void SetCellStyle(bool setPreferredWidth = false) {
    ZdoCountBackground.SetColor(CellZdoCountBackgroundImageColor.Value);
    ZdoCount.SetColor(CellZdoCountTextColor.Value);

    if (ZdoCount.fontSize != CellZdoCountTextFontSize.Value || setPreferredWidth) {
      ZdoCount.SetFontSize(CellZdoCountTextFontSize.Value);

      ZdoCount.GetComponent<LayoutElement>()
          .SetFlexible(width: 1f)
          .SetPreferred(width: ZdoCount.GetPreferredValues("12345").x);
    }

    SectorBackground.SetColor(CellSectorBackgroundImageColor.Value);
    Sector.SetColor(CellSectorTextColor.Value);

    if (Sector.fontSize != CellSectorTextFontSize.Value || setPreferredWidth) {
      Sector.SetFontSize(CellSectorTextFontSize.Value);

      Sector.GetComponent<LayoutElement>()
          .SetFlexible(width: 1f)
          .SetPreferred(width: Sector.GetPreferredValues("-123,-123").x);
    }
  }

  GameObject CreateChildCell(Transform parentTransform) {
    GameObject cell = new("Cell", typeof(RectTransform));
    cell.SetParent(parentTransform);

    cell.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetSpacing(0f)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    cell.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.Unconstrained)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    cell.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIBuilder.CreateRoundedCornerSprite(200, 200, 10))
        .SetColor(new(0f, 0f, 0f, 0.3f));

    return cell;
  }

  GameObject CreateChildBackground(Transform parentTransform) {
    GameObject background = new("Background", typeof(RectTransform));
    background.SetParent(parentTransform);

    background.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 4, right: 4, top: 2, bottom: 2)
        .SetChildAlignment(TextAnchor.MiddleCenter);

    background.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIBuilder.CreateRoundedCornerSprite(200, 200, 5))
        .SetColor(Color.clear);

    return background;
  }

  TMP_Text CreateChildLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);
    label.name = "Label";

    label.SetAlignment(TextAlignmentOptions.Center)
        .SetText("123");

    label.gameObject.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f);

    return label;
  }
}
