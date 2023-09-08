using System;

using ComfyLib;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chatter {
  public class PanelResizer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    Vector2 _lastMousePosition;

    public RectTransform TargetRectTransform { get; set; } = default!;
    public Outline TargetOutline { get; set; } = default!;
    public Action<Vector2> OnEndDragAction { get; set; } = default!;

    public void OnBeginDrag(PointerEventData eventData) {
      TargetOutline.SetEnabled(true);
      _lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
      Vector2 difference = _lastMousePosition - eventData.position;

      if (TargetRectTransform) {
        TargetRectTransform.anchoredPosition += new Vector2(0, -difference.y);
        TargetRectTransform.sizeDelta += new Vector2(difference.x, difference.y);
      }

      _lastMousePosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
      TargetOutline.SetEnabled(false);
      OnEndDragAction(TargetRectTransform.sizeDelta);
    }
  }
}
