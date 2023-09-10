using UnityEngine;
using UnityEngine.EventSystems;

namespace ComfyLib {
  // Add to GameObjects that need to block dragging on a parent GameObject
  public class DummyIgnoreDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {
    public void OnBeginDrag(PointerEventData eventData) {}
    public void OnDrag(PointerEventData eventData) {}
    public void OnEndDrag(PointerEventData eventData) {}
  }
}
