namespace Pinnacle;

using System.Collections;

using UnityEngine;

using static PluginConfig;

public static class CenterMapHelper {
  static Coroutine _centerMapCoroutine;

  public static void CenterMapOnPosition(Vector3 targetPosition) {
    if (!Minimap.s_instance || !Player.m_localPlayer) {
      return;
    }

    if (_centerMapCoroutine != null) {
      Minimap.s_instance.StopCoroutine(_centerMapCoroutine);
    }

    _centerMapCoroutine =
        Minimap.s_instance.StartCoroutine(
            CenterMapCoroutine(
                  targetPosition - Player.m_localPlayer.transform.position, CenterMapLerpDuration.Value));
  }

  static IEnumerator CenterMapCoroutine(Vector3 targetPosition, float lerpDuration) {
    float timeElapsed = 0f;
    Vector3 startPosition = Minimap.s_instance.m_mapOffset;

    while (timeElapsed < lerpDuration) {
      float t = timeElapsed / lerpDuration;
      t = t * t * (3f - (2f * t));

      Minimap.s_instance.m_mapOffset = Vector3.Lerp(startPosition, targetPosition, t);
      timeElapsed += Time.deltaTime;

      yield return null;
    }

    Minimap.s_instance.m_mapOffset = targetPosition;
  }
}
