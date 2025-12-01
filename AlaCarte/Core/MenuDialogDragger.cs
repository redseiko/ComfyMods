namespace AlaCarte; 

using System;

using UnityEngine;
using UnityEngine.EventSystems;

using static PluginConfig;

public sealed class MenuDialogDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  public RectTransform MenuRectTransform;
  public event EventHandler<Vector3> OnMenuDragEnd;

  Vector2 _lastMousePosition;

  public void OnBeginDrag(PointerEventData eventData) {
    _lastMousePosition = eventData.position;

    if (!MenuDialogCanDragPanel.Value) {
      eventData.pointerDrag = default;
    }
  }

  public void OnDrag(PointerEventData eventData) {
    Vector2 difference = eventData.position - _lastMousePosition;
    MenuRectTransform.position += new Vector3(difference.x, difference.y, 0f);

    _lastMousePosition = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData) {
    OnMenuDragEnd?.Invoke(this, MenuRectTransform.anchoredPosition);
  }
}
