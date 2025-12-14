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
  public MahjongTileDragger Dragger { get; }

  public MahjongTile(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();

    CreateShadow(Container.transform);
    Image borderImage = CreateBorder(Container.transform);
    Image faceImage = CreateFace(borderImage.transform);

    Background = faceImage;
    FaceText = CreateFaceText(faceImage.transform);
    Button = CreateButton(faceImage.gameObject, faceImage);

    Container.AddComponent<CanvasGroup>();
    Dragger = Container.AddComponent<MahjongTileDragger>();
    Dragger.Initialize(this);

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
    container.GetComponent<RectTransform>().SetSizeDelta(new(TileWidth + 3, TileHeight + 3));
    return container;
  }

  static Image CreateShadow(Transform parentTransform) {
    GameObject shadow = new("Shadow", typeof(RectTransform));
    shadow.transform.SetParent(parentTransform, false);

    Image image = shadow.AddComponent<Image>();
    image.SetType(Image.Type.Sliced)
        .SetSprite(MahjongTileResources.TileSprite)
        .SetColor(MahjongTileResources.ShadowColor);

    shadow.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPosition(new(3, -3))
        .SetSizeDelta(new(0, 0));

    return image;
  }

  static Image CreateBorder(Transform parentTransform) {
    GameObject border = new("Border", typeof(RectTransform));
    border.transform.SetParent(parentTransform, false);

    Image image = border.AddComponent<Image>();
    image.SetType(Image.Type.Sliced)
        .SetSprite(MahjongTileResources.TileSprite)
        .SetColor(MahjongTileResources.BorderColor);

    border.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(-3, -3));

    return image;
  }

  static Image CreateFace(Transform parentTransform) {
    GameObject face = new("Face", typeof(RectTransform));
    face.transform.SetParent(parentTransform, false);

    Image image = face.AddComponent<Image>();
    image.SetType(Image.Type.Sliced)
        .SetSprite(MahjongTileResources.TileSprite)
        .SetColor(MahjongTileResources.TileColors.normalColor);

    face.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(-4, -4));

    return image;
  }

  static TextMeshProUGUI CreateFaceText(Transform parentTransform) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(parentTransform);
    label.name = "FaceText";
    label.SetAlignment(TextAlignmentOptions.Center).SetFontSize(20f);
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
    button.SetTargetGraphic(targetGraphic)
        .SetTransition(Selectable.Transition.ColorTint)
        .SetColors(MahjongTileResources.TileColors);
    return button;
  }
}
