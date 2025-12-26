namespace ReportCard;

using ComfyLib;
using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class MahjongTile {
  public const float TileWidth = 70f;
  public const float TileHeight = 80f;

  public MahjongTileInfo Info { get; private set; }
  public GameObject Container { get; }
  public RectTransform RectTransform { get; }
  public CanvasGroup CanvasGroup {  get; }
  public Image Background { get; }
  public Image FaceImage { get; }
  public Button Button { get; }
  public Action<MahjongTile> OnTileClicked;

  public MahjongTile(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    CanvasGroup = Container.GetComponent<CanvasGroup>();
    Background = Container.GetComponent<Image>();
    FaceImage = CreateFaceImage(Container.transform);
    Button = CreateButton(Container, Background);
    Button.onClick.AddListener(ProcessButtonClick);

    FaceImage.gameObject.SetActive(false);
  }

  void ProcessButtonClick() {
    OnTileClicked?.Invoke(this);
  }

  public void SetTile(MahjongTileInfo info) {
    Info = info;
    FaceImage.sprite = MahjongTileTextureGenerator.GetTileSprite(info);
    FaceImage.gameObject.SetActive(true);
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("MahjongTile", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(MahjongTileResources.TileSprite)
        .SetColor(MahjongTileResources.TileColors.normalColor);

    container.AddComponent<CanvasGroup>();

    container.GetComponent<RectTransform>()
        .SetSizeDelta(new(TileWidth, TileHeight));

    return container;
  }

  static Image CreateFaceImage(Transform parentTransform) {
    GameObject obj = new("FaceImage", typeof(RectTransform));
    obj.transform.SetParent(parentTransform, false);

    Image image = obj.AddComponent<Image>();
    image.raycastTarget = false;

    image.rectTransform
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(56, 64));

    return image;
  }
  
  static Button CreateButton(GameObject container, Image targetGraphic) {
    Button button = container.AddComponent<Button>();

    button
        .SetTargetGraphic(targetGraphic)
        .SetTransition(Selectable.Transition.ColorTint)
        .SetColors(MahjongTileResources.TileColors);

    return button;
  }

  public void AnimateSpawn(Vector2 startPosition, Vector2 endPosition, Action onComplete) {
    RectTransform.anchoredPosition = startPosition;
    
    RectTransform.TweenKill();
    RectTransform
        .TweenAnchoredPosition(endPosition, 0.3f)
        .SetEase(Ease.OutBack)
        .OnComplete(onComplete)
        .TweenStart();
  }

  public void AnimateDiscard(Action onComplete) {
    Vector2 currentPosition = RectTransform.anchoredPosition;
    Vector2 targetPosition = new Vector2(currentPosition.x, currentPosition.y + 100f);

    RectTransform.TweenKill();
    RectTransform
        .TweenAnchoredPosition(targetPosition, 0.3f)
        .SetEase(Ease.InBack)
        .TweenStart();

    CanvasGroup
        .TweenFade(0f, 0.3f)
        .OnComplete(onComplete)
        .TweenStart();
  }
}
