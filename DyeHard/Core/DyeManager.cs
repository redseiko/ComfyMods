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
    if (!LocalPlayer || !LocalPlayer.m_visEquipment) {
      return;
    }

    string beardItem =
        IsModEnabled.Value && OverridePlayerBeardItem.Value ? PlayerBeardItem.Value : LocalPlayer.m_beardItem;

    if (LocalPlayer.m_nview) {
      LocalPlayer.m_visEquipment.SetBeardItem(beardItem);
    }


    LocalPlayer.m_visEquipment.m_beardItem = beardItem;
  }

  public static void SetPlayerHairItem() {
    if (!LocalPlayer || !LocalPlayer.m_visEquipment) {
      return;
    }

    string hairItem =
        IsModEnabled.Value && OverridePlayerHairItem.Value ? PlayerHairItem.Value : LocalPlayer.m_hairItem;

    if (LocalPlayer.m_nview) {
      LocalPlayer.m_visEquipment.SetHairItem(hairItem);
    }


    LocalPlayer.m_visEquipment.m_hairItem = hairItem;
  }

  public static void SetPlayerZDOHairColor() {
    if (!LocalPlayer || !LocalPlayer.m_visEquipment) {
      return;
    }

    Vector3 color =
        IsModEnabled.Value && OverridePlayerHairColor.Value
            ? GetPlayerHairColorVector()
            : LocalPlayer.m_hairColor;

    LocalPlayer.m_visEquipment.m_hairColor = color;

    if (!LocalPlayer.m_nview || !LocalPlayer.m_nview.IsValid()) {
      return;
    }

    if (!LocalPlayer.m_nview.m_zdo.TryGetVector3(ZDOVars.s_hairColor, out Vector3 cachedColor)
        || cachedColor != color) {
      LocalPlayer.m_nview.m_zdo.Set(ZDOVars.s_hairColor, color);
    }
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
