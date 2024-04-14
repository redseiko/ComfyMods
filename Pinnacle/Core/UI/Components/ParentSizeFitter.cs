namespace Pinnacle;

using UnityEngine;

public sealed class ParentSizeFitter : MonoBehaviour {
  RectTransform _parentRectTransform;
  RectTransform _rectTransform;

  void Awake() {
    _parentRectTransform = transform.parent.GetComponent<RectTransform>();
    _rectTransform = GetComponent<RectTransform>();
  }

  void OnRectTransformDimensionsChange() {
    if (!_parentRectTransform) {
      _parentRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    if (!_rectTransform) {
      _rectTransform = GetComponent<RectTransform>();
    }

    if (_rectTransform && _parentRectTransform) {
      _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _parentRectTransform.sizeDelta.x);
    }
  }
}
