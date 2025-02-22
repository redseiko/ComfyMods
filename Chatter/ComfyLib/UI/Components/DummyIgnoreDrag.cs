namespace ComfyLib;

using UnityEngine;
using UnityEngine.EventSystems;

// Add to GameObjects that need to block dragging on a parent GameObject.
public sealed class DummyIgnoreDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {
  public void OnBeginDrag(PointerEventData eventData) {}
  public void OnDrag(PointerEventData eventData) {}
  public void OnEndDrag(PointerEventData eventData) {}
}
