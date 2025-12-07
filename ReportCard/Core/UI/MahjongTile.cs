namespace ReportCard;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class MahjongTile {
  public const float TileWidth = 70f;
  public const float TileHeight = 80f;
  public MahjongTileInfo Info { get; private set; }
  public GameObject Container { get; }
  public RectTransform RectTransform { get; }
  public Image Background { get; }
  public TextMeshProUGUI FaceText { get; }
  public Button Button { get; }

  public MahjongTile(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    Background = Container.GetComponent<Image>();
    FaceText = CreateFaceText(Container.transform);
    Button = CreateButton(Container, Background);

    FaceText.gameObject.SetActive(false);
  }

  public void SetTile(MahjongTileInfo info) {
    Info = info;
    FaceText.SetText(MahjongTileHelper.GetFormattedTileText(info));
    FaceText.gameObject.SetActive(true);
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("MahjongTile", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("button"))
        .SetColor(Color.white);

    container.GetComponent<RectTransform>()
        .SetSizeDelta(new(TileWidth, TileHeight));

    return container;
  }

  static TextMeshProUGUI CreateFaceText(Transform parentTransform) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(parentTransform);
    label.name = "FaceText";

    label
        .SetAlignment(TextAlignmentOptions.Center)
        .SetFontSize(20f);

    label.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    return label;
  }

  static Button CreateButton(GameObject container, Image targetGraphic) {
    Button button = container.AddComponent<Button>();

    button
        .SetTargetGraphic(targetGraphic)
        .SetTransition(Selectable.Transition.SpriteSwap)
        .SetSpriteState(
            new() {
              disabledSprite = UIResources.GetSprite("button_disabled"),
              highlightedSprite = UIResources.GetSprite("button_highlight"),
              pressedSprite = UIResources.GetSprite("button_pressed"),
              selectedSprite = UIResources.GetSprite("button_highlight"),
            });

    return button;
  }
}
