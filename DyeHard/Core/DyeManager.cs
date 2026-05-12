namespace DyeHard;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class DyeManager {
  public static Player LocalPlayer { get; private set; }
  public static VisEquipment LocalVisEquipment { get; private set; }

  public static void SetLocalPlayer(Player player) {
    LocalPlayer = player;
    LocalVisEquipment = player ? player.m_visEquipment : default;
  }

  public static Vector3 GetPlayerHairColorVector() {
    Vector3 colorVector = Utils.ColorToVec3(PlayerHairColor.Value);

    if (colorVector != Vector3.zero) {
      colorVector *= PlayerHairGlow.Value / colorVector.magnitude;
    }

    return colorVector;
  }

  public static void SetPlayerBeardItem() {
    if (LocalPlayer && LocalPlayer.m_visEquipment) {
      SetPlayerBeardItem(LocalPlayer);
    }
  }

  public static void SetPlayerBeardItem(Player player) {
    string beardItem =
        IsModEnabled.Value && OverridePlayerBeardItem.Value
            ? PlayerBeardItem.Value
            : player.m_beardItem;

    player.m_visEquipment.SetBeardItem(beardItem.GetStableHashCode());
  }

  public static void SetPlayerHairItem() {
    if (LocalPlayer && LocalPlayer.m_visEquipment) {
      SetPlayerHairItem(LocalPlayer);
    }
  }

  public static void SetPlayerHairItem(Player player) {
    string hairItem =
        IsModEnabled.Value && OverridePlayerHairItem.Value
            ? PlayerHairItem.Value
            : player.m_hairItem;

    player.m_visEquipment.SetHairItem(hairItem.GetStableHashCode());
  }

  public static void SetPlayerZDOHairColor() {
    if (LocalPlayer && LocalPlayer.m_visEquipment) {
      SetPlayerZDOHairColor(LocalPlayer);
    }
  }

  public static void SetPlayerZDOHairColor(Player player) {
    Vector3 color =
        IsModEnabled.Value && OverridePlayerHairColor.Value
            ? GetPlayerHairColorVector()
            : player.m_hairColor;

    player.m_visEquipment.SetHairColor(color);
  }

  public static void SetCharacterPreviewPosition() {
    SetCharacterPreviewPosition(FejdStartup.m_instance);
  }

  static void SetCharacterPreviewPosition(FejdStartup fejdStartup) {
    if (fejdStartup && fejdStartup.m_playerInstance) {
      Vector3 targetPosition = fejdStartup.m_characterPreviewPoint.position;

      if (IsModEnabled.Value) {
        targetPosition += OffsetCharacterPreviewPosition.Value;
      }

      fejdStartup.m_playerInstance.transform.position = targetPosition;
    }
  }
}
