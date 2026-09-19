namespace ComfyLib;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public sealed class PanelResizer :
    MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
  public UnityEvent<Vector2> OnPanelResizeEnd = new();

  CanvasGroup _canvasGroup;
  ComfyTween<float> _alphaTween;
  float _targetAlpha = 0f;

  Vector2 _lastMousePosition;
  RectTransform _targetRectTransform;

  public void SetTargetRectTransform(RectTransform rectTransform) {
    _targetRectTransform = rectTransform;
  }

  void Awake() {
    _canvasGroup = GetComponent<CanvasGroup>();
    _alphaTween = new ComfyTween<float>(
        this, 0.25f, () => _canvasGroup.alpha, alpha => _canvasGroup.SetAlpha(alpha), Mathf.Lerp);
  }

  void SetCanvasGroupAlpha(float alpha) {
    if (_canvasGroup.alpha == alpha) {
      return;
    }

    _alphaTween.To(alpha);
  }

  public void OnPointerEnter(PointerEventData eventData) {
    _targetAlpha = 1f;
    SetCanvasGroupAlpha(_targetAlpha);
  }

  public void OnPointerExit(PointerEventData eventData) {
    _targetAlpha = 0f;

    if (!eventData.dragging) {
      SetCanvasGroupAlpha(_targetAlpha);
    }
  }

  public void OnBeginDrag(PointerEventData eventData) {
    SetCanvasGroupAlpha(1f);
    _lastMousePosition = eventData.position;
  }

  public void OnDrag(PointerEventData eventData) {
    Vector2 difference = _lastMousePosition - eventData.position;
    _targetRectTransform.sizeDelta += new Vector2(-1f * difference.x, difference.y);

    _lastMousePosition = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData) {
    SetCanvasGroupAlpha(_targetAlpha);
    OnPanelResizeEnd.Invoke(_targetRectTransform.sizeDelta);
  }
}
