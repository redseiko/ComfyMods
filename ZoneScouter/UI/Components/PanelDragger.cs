using UnityEngine;
using UnityEngine.EventSystems;

using System;

namespace ZoneScouter {
  public class PanelDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    Vector2 _lastMousePosition;
    RectTransform _rectTransform;

    public Action<Vector3> OnEndDragAction { get; set; } = _ => { };

    public void OnBeginDrag(PointerEventData eventData) {
      if (!_rectTransform) {
        _rectTransform = GetComponent<RectTransform>();
      }

      _lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
      Vector2 difference = eventData.position - _lastMousePosition;

      _rectTransform.position += new Vector3(difference.x, difference.y, transform.position.z);
      _lastMousePosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
      OnEndDragAction(_rectTransform.anchoredPosition);
    }
  }
}
