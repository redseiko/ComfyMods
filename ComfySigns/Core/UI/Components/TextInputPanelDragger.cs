namespace ComfySigns;

using UnityEngine;
using UnityEngine.EventSystems;

public sealed class TextInputPanelDragger : MonoBehaviour, IDragHandler {
  RectTransform _rectTransform;
  Canvas _canvas;

  void Start() {
    _rectTransform = GetComponent<RectTransform>();
    _canvas = GetComponentInParent<Canvas>();
  }

  public void OnDrag(PointerEventData eventData) {
    _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
  }
}
