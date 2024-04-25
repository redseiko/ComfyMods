namespace ComfyLib; 

using System;

using UnityEngine;
using UnityEngine.EventSystems;

public sealed class PanelDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  public RectTransform TargetRectTransform;
  public event EventHandler<Vector3> OnPanelEndDrag;

  Vector2 _lastMousePosition;

  public void OnBeginDrag(PointerEventData eventData) {
    _lastMousePosition = eventData.position;
  }

  public void OnDrag(PointerEventData eventData) {
    Vector2 difference = eventData.position - _lastMousePosition;
    TargetRectTransform.position += new Vector3(difference.x, difference.y, 0);

    _lastMousePosition = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData) {
    OnPanelEndDrag?.Invoke(this, TargetRectTransform.anchoredPosition);
  }
}
