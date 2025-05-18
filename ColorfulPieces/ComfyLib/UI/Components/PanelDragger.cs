namespace ComfyLib;

using UnityEngine;
using UnityEngine.EventSystems;

public sealed class PanelDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  RectTransform _targetRectTransform;
  Vector2 _lastMousePosition;

  void Start() {
    _targetRectTransform = GetComponent<RectTransform>();
  }

  public void OnBeginDrag(PointerEventData eventData) {
    _lastMousePosition = eventData.position;
  }

  public void OnDrag(PointerEventData eventData) {
    Vector2 difference = eventData.position - _lastMousePosition;
    _targetRectTransform.position += new Vector3(difference.x, difference.y, 0f);
    _lastMousePosition = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData) {
    // ...
  }
}
