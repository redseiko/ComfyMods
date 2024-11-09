namespace Pinnacle;

using UnityEngine;

using static PluginConfig;

public static class PinnacleUtils {
  public static void CenterMapOnOrTeleportTo(Minimap.PinData targetPin) {
    if (IsModEnabled.Value
        && Console.m_instance.IsCheatsEnabled()
        && Player.m_localPlayer
        && ZInput.GetKey(KeyCode.LeftShift)) {
      TeleportTo(targetPin);
    } else {
      Pinnacle.TogglePinEditPanel(PinListPanelEditPinOnRowClick.Value ? targetPin : default);
      CenterMapHelper.CenterMapOnPosition(targetPin.m_pos);
    }
  }

  public static void TeleportTo(Minimap.PinData targetPin) {
    Vector3 targetPosition = targetPin.m_pos;

    if (targetPin.m_type == Minimap.PinType.Player && !targetPin.m_save && !targetPin.m_checked) {
      targetPosition = GetPlayerInfoPosition(targetPin);
    }

    TeleportTo(targetPosition);
  }

  public static Vector3 GetPlayerInfoPosition(Minimap.PinData targetPin) {
    string pinName = targetPin.m_name;

    foreach (ZNet.PlayerInfo playerInfo in ZNet.m_instance.m_players) {
      if (playerInfo.m_name == pinName && playerInfo.m_publicPosition) {
        Pinnacle.LogInfo(
            $"Found PlayerInfo for {playerInfo.m_name}: {targetPin.m_pos:F0} -> {playerInfo.m_position:F0}");

        return playerInfo.m_position;
      }
    }

    return targetPin.m_pos;
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

    Pinnacle.LogInfo($"Teleporting player from {player.transform.position:F0} to {targetPosition:F0}.");
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

  public static Minimap.PinData AddNewPin(Minimap minimap, Vector3 targetPosition) {
    if (Mathf.Approximately(targetPosition.y, 0f)) {
      targetPosition.y = GetHeight(targetPosition);
    }

    return minimap.AddPin(targetPosition, minimap.m_selectedType, string.Empty, true, false, 0L);
  }
}
