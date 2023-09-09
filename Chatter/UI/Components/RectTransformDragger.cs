using UnityEngine;
using UnityEngine.EventSystems;

using System;

namespace Chatter {
  public class RectTransformDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    Vector2 _lastMousePosition;

    [field: SerializeField]
    public RectTransform TargetRectTransform { get; private set; }

    public RectTransformDragger SetTargetRectTransform(RectTransform rectTransform) {
      TargetRectTransform = rectTransform;
      return this;
    }

    public event EventHandler<Vector3> OnEndDragEvent;

    public void OnBeginDrag(PointerEventData eventData) {
      _lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
      Vector2 difference = eventData.position - _lastMousePosition;

      TargetRectTransform.position += new Vector3(difference.x, difference.y, 0f);
      _lastMousePosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
      OnEndDragEvent?.Invoke(this, TargetRectTransform.anchoredPosition);
    }
  }
}
