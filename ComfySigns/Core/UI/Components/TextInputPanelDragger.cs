namespace ComfySigns;

using UnityEngine;
using UnityEngine.EventSystems;

public sealed class TextInputPanelDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  RectTransform _rectTransform;
  Vector2 _lastMousePosition;

  void Start() {
    _rectTransform = GetComponent<RectTransform>();
  }

  public void OnBeginDrag(PointerEventData eventData) {
    _lastMousePosition = eventData.position;
  }

  public void OnDrag(PointerEventData eventData) {
    Vector2 difference = eventData.position - _lastMousePosition;
    _rectTransform.position += new Vector3(difference.x, difference.y, 0f);
    _lastMousePosition = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData) {
    // ... Do nothing.
  }
}
