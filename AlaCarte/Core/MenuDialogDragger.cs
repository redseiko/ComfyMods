namespace AlaCarte;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using static PluginConfig;

public sealed class MenuDialogDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
  public RectTransform TargetRectTransform;
  public readonly UnityEvent<Vector2> OnMenuDialogDragEnd = new();

  public void OnBeginDrag(PointerEventData eventData) {
    if (!MenuDialogCanDragPanel.Value) {
      eventData.pointerDrag = default;
    }
  }

  public void OnDrag(PointerEventData eventData) {
    TargetRectTransform.anchoredPosition += eventData.delta;
  }

  public void OnEndDrag(PointerEventData eventData) {
    OnMenuDialogDragEnd.Invoke(TargetRectTransform.anchoredPosition);
  }
}
