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

  void OnEnable() {
    StartCoroutine(AnimateVertexColors());
  }

  void OnDisable() {
    StopAllCoroutines();

    if (_textComponent) {
      _textComponent.ForceMeshUpdate(ignoreActiveState: true);
    }
  }

  IEnumerator AnimateVertexColors() {
    int currentCharacter = 0;

    while (true) {
      TMP_TextInfo textInfo = _textComponent.textInfo;
      int characterCount = textInfo.characterCount;

      if (characterCount == 0) {
        yield return _longWait;
        continue;
      }

      if (currentCharacter >= characterCount) {
        currentCharacter = 0;
      }

      TMP_CharacterInfo charInfo = textInfo.characterInfo[currentCharacter];
      int materialIndex = charInfo.materialReferenceIndex;

      if (materialIndex < 0 || materialIndex >= textInfo.meshInfo.Length) {
        yield return _shortWait;
        continue;
      }

      Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
      int vertexIndex = charInfo.vertexIndex;

      if (vertexColors == null || vertexIndex + 3 >= vertexColors.Length) {
        yield return _shortWait;
        continue;
      }

      if (charInfo.isVisible) {
        Color32 color =
            new Color32((byte) Random.Range(0, 256), (byte) Random.Range(0, 256), (byte) Random.Range(0, 256), 255);

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
