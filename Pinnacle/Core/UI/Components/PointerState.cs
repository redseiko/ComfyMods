namespace Pinnacle;

using UnityEngine;
using UnityEngine.EventSystems;

public sealed class PointerState : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  public bool IsPointerHovered { get; private set; } = false;

  public void OnPointerEnter(PointerEventData eventData) {
    IsPointerHovered = true;
  }

  public void OnPointerExit(PointerEventData eventData) {
    IsPointerHovered = false;
  }
}
