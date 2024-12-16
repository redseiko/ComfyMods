namespace ComfyLib;

using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class ColorSquare {
  public GameObject Container { get; }
  public RectTransform RectTransform { get; }

  public GameObject Square { get; }
  public Image SquareImage { get; }
  public Texture2D SquareTexture { get; }

  readonly SquareInteraction _squareInteraction;

  public ColorSquare(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();

    Square = CreateSquare(Container.transform);
    SquareImage = Square.GetComponent<Image>();
    SquareTexture = SquareImage.sprite.texture;

    _squareInteraction = Square.AddComponent<SquareInteraction>();
    _squareInteraction.OnClick += OnSquareClick;
  }

  void OnSquareClick(object sender, Vector2 position) {
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        RectTransform, position, null, out Vector2 localPoint);

    Rect r = RectTransform.rect;

    float x = (((localPoint.x - r.x) * SquareTexture.width) / r.width);
    float y = (((localPoint.y - r.y) * SquareTexture.height) / r.height);

    ZLog.Log($"SquareClick: {position} --> {localPoint} --> {x},{y}");
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("ColorSquare", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(150f, 150f));

    return container;
  }

  static GameObject CreateSquare(Transform parentTransform) {
    GameObject square = new("Square", typeof(RectTransform));
    square.transform.SetParent(parentTransform, worldPositionStays: false);

    square.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    square.AddComponent<Image>()
        .SetSprite(CreateSquareSprite(length: 100))
        .SetPreserveAspect(true)
        .SetColor(Color.white);

    return square;
  }

  static Sprite CreateSquareSprite(int length) {
    Texture2D texture = new(length, length) {
      name = "SquareTexture",
      wrapMode = TextureWrapMode.Clamp,
    };

    Sprite sprite = Sprite.Create(texture, new(0f, 0f, length, length), new(0.5f, 0.5f));
    sprite.name = "SquareSprite";

    return sprite;
  }

  sealed class SquareInteraction : MonoBehaviour, IDragHandler, IPointerDownHandler {
    public event EventHandler<Vector2> OnClick;

    public void OnDrag(PointerEventData eventData) {
      OnClick?.Invoke(this, eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData) {
      OnClick?.Invoke(this, eventData.position);
    }
  }
}
