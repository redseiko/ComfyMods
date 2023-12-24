using FronkonGames.TinyTween;

using UnityEngine;

using static Pinnacle.PluginConfig;

namespace Pinnacle {
  public static class CenterMapHelper {
    static ITween<Vector3> _centerMapTween;

    public static void CenterMapOnPosition(Vector3 targetPosition) {
      if (!Minimap.m_instance || !Player.m_localPlayer) {
        return;
      }

      _centerMapTween?.Stop(moveToEnd: false);

      _centerMapTween =
          TweenVector3.Create()
              .Origin(Minimap.m_instance.m_mapOffset)
              .Destination(targetPosition - Player.m_localPlayer.transform.position)
              .Duration(CenterMapLerpDuration.Value)
              .Easing(Ease.Cubic)
              .OnUpdate(tween => Minimap.m_instance.m_mapOffset = tween.Value)
              .Start();
    }
  }
}
