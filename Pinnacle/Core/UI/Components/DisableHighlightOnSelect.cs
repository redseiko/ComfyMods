namespace Pinnacle;

using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class DisableHighlightOnSelect : MonoBehaviour, ISelectHandler {
  InputField _inputField;

  void Start() {
    _inputField = GetComponent<InputField>();
  }

  public void OnSelect(BaseEventData eventData) {
    StartCoroutine(DisableHighlight());
  }

  IEnumerator DisableHighlight() {
    Color original = _inputField.selectionColor;
    _inputField.selectionColor = Color.clear;

    yield return null;

    _inputField.MoveTextEnd(false);
    _inputField.selectionColor = original;
  }
}
