namespace Pinnacle;

using UnityEngine;

using static PluginConfig;

public static class PinnacleUtils {
  public static void CenterMapOnOrTeleportTo(Minimap.PinData targetPin) {
    if (IsModEnabled.Value
        && Console.m_instance.IsCheatsEnabled()
        && Player.m_localPlayer
        && ZInput.GetKey(KeyCode.LeftShift)
        && targetPin != null) {
      TeleportTo(targetPin.m_pos);
    } else {
      Pinnacle.TogglePinEditPanel(PinListPanelEditPinOnRowClick.Value ? targetPin : default);
      CenterMapHelper.CenterMapOnPosition(targetPin.m_pos);
    }
  }

  public static void TeleportTo(Vector3 targetPosition) {
    Player player = Player.m_localPlayer;

    if (!player) {
      Pinnacle.LogWarning($"No local Player found.");
      return;
    }

    if (Mathf.Approximately(targetPosition.y, 0f)) {
      targetPosition.y = GetHeight(targetPosition);
    }

    Pinnacle.LogInfo($"Teleporting player from {player.transform.position} to {targetPosition}.");
    player.TeleportTo(targetPosition, player.transform.rotation, distantTeleport: true);

    Minimap.m_instance.SetMapMode(Minimap.MapMode.Small);
  }

  public static float GetHeight(Vector3 targetPosition) {
    if (!Heightmap.GetHeight(targetPosition, out float height)) {
      height = GetHeightmapData(targetPosition).m_baseHeights[0];
    }

    return Mathf.Max(0f, height);
  }

  public static HeightmapBuilder.HMBuildData GetHeightmapData(Vector3 targetPosition) {
    HeightmapBuilder.HMBuildData heightmapData =
        new(targetPosition, width: 1, scale: 1f, distantLod: false, WorldGenerator.m_instance);

    HeightmapBuilder.m_instance.Build(heightmapData);

    return heightmapData;
  }
}
