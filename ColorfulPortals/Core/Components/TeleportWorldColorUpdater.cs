namespace ColorfulPortals;

using System.Collections;

using UnityEngine;

using static PluginConfig;
using static TeleportWorldColor;

public sealed class TeleportWorldColorUpdater : MonoBehaviour {
  void Awake() {
    StartCoroutine(UpdateTeleportWorldColors());
  }

  IEnumerator UpdateTeleportWorldColors() {
    WaitForSeconds waitInterval = new(seconds: UpdateColorsWaitInterval.Value);

    while (true) {
      int frameLimit = UpdateColorsFrameLimit.Value;
      int index = 0;

      while (index < TeleportWorldColorCache.Count) {
        int processed = 0;

        while (processed < frameLimit && TeleportWorldColorCache.Count > 0 && index < TeleportWorldColorCache.Count) {
          TeleportWorldColorCache[index].UpdateColors();

          index++;
          processed++;
        }

        yield return null;
      }

      yield return waitInterval;
    }
  }
}
