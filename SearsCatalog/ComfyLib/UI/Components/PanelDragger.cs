namespace ComfyLib;

using UnityEngine;
using UnityEngine.EventSystems;

using System;

public sealed class PanelDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  Vector2 _lastMousePosition;

  public RectTransform TargetRectTransform;
  public event EventHandler<Vector3> OnPanelEndDrag;

  public void OnBeginDrag(PointerEventData eventData) {
    _lastMousePosition = eventData.position;
  }

  public void OnDrag(PointerEventData eventData) {
    Vector2 difference = eventData.position - _lastMousePosition;

    if (TargetRectTransform) {
      TargetRectTransform.position += new Vector3(difference.x, difference.y, 0f);
    }

    _lastMousePosition = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData) {
    if (TargetRectTransform) {
      OnPanelEndDrag?.Invoke(this, TargetRectTransform.anchoredPosition);
    }
  }
}
