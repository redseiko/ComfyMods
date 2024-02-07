namespace ColorfulPieces;

using System.Collections;

using UnityEngine;

using static PieceColor;
using static PluginConfig;

public sealed class PieceColorUpdater : MonoBehaviour {
  void Awake() {
    StartCoroutine(UpdatePieceColors());
  }

  IEnumerator UpdatePieceColors() {
    WaitForSeconds waitInterval = new(UpdateColorsWaitInterval.Value);

    while (true) {
      int frameLimit = UpdateColorsFrameLimit.Value;
      int index = 0;

      while (index < PieceColorCache.Count) {
        int processed = 0;

        while (processed < frameLimit && PieceColorCache.Count > 0 && index < PieceColorCache.Count) {
          PieceColorCache[index].UpdateColors();

          index++;
          processed++;
        }

        yield return null;
      }

      yield return waitInterval;
    }
  }
}
