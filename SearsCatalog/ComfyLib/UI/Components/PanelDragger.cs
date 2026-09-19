namespace ComfyLib;

using UnityEngine;
using UnityEngine.EventSystems;

using System;
using UnityEngine.Events;

public sealed class PanelDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  public readonly UnityEvent<Vector2> OnPanelDragEnd = new();

  Vector2 _lastMousePosition;
  RectTransform _targetRectTransform;

  public void SetTargetRectTransform(RectTransform targetRectTransform) {
    _targetRectTransform = targetRectTransform;
  }

  void Start() {
    if (!_targetRectTransform) {
      _targetRectTransform = GetComponent<RectTransform>();
    }
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
    OnPanelDragEnd?.Invoke(_targetRectTransform.anchoredPosition);
  }
}
