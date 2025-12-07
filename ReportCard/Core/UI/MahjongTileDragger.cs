namespace ReportCard;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public sealed class MahjongTileDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  MahjongTile _mahjongTile;
  CanvasGroup _canvasGroup;
  Transform _dragParent;

  public readonly UnityEvent<MahjongTile> OnTileDropped = new();

  void Awake() {
    _canvasGroup = GetComponent<CanvasGroup>();
    _dragParent = GetComponentInParent<Canvas>().transform;
  }

  public void Initialize(MahjongTile mahjongTile) {
    _mahjongTile = mahjongTile;
  }

  public void OnBeginDrag(PointerEventData eventData) {
    if (_mahjongTile == null) {
      return;
    }

    transform.SetParent(_dragParent, worldPositionStays: true);
    _canvasGroup.blocksRaycasts = false;
  }

  public void OnDrag(PointerEventData eventData) {
    if (_mahjongTile == null) {
      return;
    }

    (transform as RectTransform).position = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData) {
    if (_mahjongTile == null) {
      return;
    }

    OnTileDropped.Invoke(_mahjongTile);
    _canvasGroup.blocksRaycasts = true;
  }
}
