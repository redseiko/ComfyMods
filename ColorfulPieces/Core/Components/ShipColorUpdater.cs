namespace ColorfulPieces;

using System.Collections;

using UnityEngine;

using static PluginConfig;
using static ShipColor;

public sealed class ShipColorUpdater : MonoBehaviour {
  void Awake() {
    StartCoroutine(UpdateShipColors());
  }

  IEnumerator UpdateShipColors() {
    WaitForSeconds waitInterval = new(UpdateColorsWaitInterval.Value);

    while (true) {
      int frameLimit = UpdateColorsFrameLimit.Value;
      int index = 0;

      while (index < ShipColorCache.Count) {
        int processed = 0;

        while (processed < frameLimit && ShipColorCache.Count > 0 && index < ShipColorCache.Count) {
          ShipColorCache[index].UpdateColors();

          index++;
          processed++;
        }

        yield return null;
      }

      yield return waitInterval;
    }
  }
}
