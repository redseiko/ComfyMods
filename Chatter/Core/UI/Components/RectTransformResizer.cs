namespace Chatter;

using System;

using UnityEngine;
using UnityEngine.EventSystems;

public sealed class RectTransformResizer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  [field: SerializeField]
  public RectTransform TargetRectTransform { get; private set; }

  public RectTransformResizer SetTargetRectTransform(RectTransform rectTransform) {
    TargetRectTransform = rectTransform;
    return this;
  }

  public event EventHandler<Vector2> OnEndDragEvent;

  Vector2 _lastMousePosition;

  public void OnBeginDrag(PointerEventData eventData) {
    _lastMousePosition = eventData.position;
  }

  public void OnDrag(PointerEventData eventData) {
    Vector2 difference = _lastMousePosition - eventData.position;

    TargetRectTransform.anchoredPosition += new Vector2(0, -difference.y);
    TargetRectTransform.sizeDelta += new Vector2(difference.x, difference.y);

    _lastMousePosition = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData) {
    OnEndDragEvent?.Invoke(this, TargetRectTransform.sizeDelta);
  }
}
