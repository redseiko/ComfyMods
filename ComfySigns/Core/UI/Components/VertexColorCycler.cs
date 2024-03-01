namespace ComfySigns;

using System.Collections;

using TMPro;

using UnityEngine;

// Modified from: https://gist.github.com/baba-s/e13eb2e813061e05a0535067c58d6126
public sealed class VertexColorCycler : MonoBehaviour {
  static readonly WaitForSeconds _longWait = new(seconds: 1f);
  static readonly WaitForSeconds _shortWait = new(seconds: 0.05f);

  TMP_Text _textComponent;

  void Awake() {
    _textComponent = GetComponent<TMP_Text>();
  }

  void Start() {
    StartCoroutine(AnimateVertexColors());
  }

  IEnumerator AnimateVertexColors() {
    TMP_TextInfo textInfo = _textComponent.textInfo;
    int currentCharacter = 0;

    Color32[] vertexColors;
    Color32 color;

    while (true) {
      int characterCount = textInfo.characterCount;

      if (characterCount == 0) {
        yield return _longWait;
        continue;
      }

      int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;
      vertexColors = textInfo.meshInfo[materialIndex].colors32;
      int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

      if (textInfo.characterInfo[currentCharacter].isVisible) {
        color =
            new Color32((byte) Random.Range(0, 255), (byte) Random.Range(0, 255), (byte) Random.Range(0, 255), 255);

        vertexColors[vertexIndex + 0] = color;
        vertexColors[vertexIndex + 1] = color;
        vertexColors[vertexIndex + 2] = color;
        vertexColors[vertexIndex + 3] = color;

        _textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
      }

      int lastCharacter = currentCharacter;
      currentCharacter = (currentCharacter + 1) % characterCount;

      yield return lastCharacter < currentCharacter ? _shortWait : _longWait;
    }
  }
}
