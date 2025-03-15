namespace ColorfulPortals;

using ComfyLib;

using Splatform;

using UnityEngine;

using static PluginConfig;

public static class TeleportWorldManager {
  public static readonly int TeleportWorldColorHashCode = "TeleportWorldColor".GetStableHashCode();
  public static readonly int TeleportWorldColorAlphaHashCode = "TeleportWorldColorAlpha".GetStableHashCode();
  public static readonly int PortalLastColoredByHashCode = "PortalLastColoredBy".GetStableHashCode();
  public static readonly int PortalLastColoredByHostHashCode = "PortalLastColoredByHost".GetStableHashCode();

  public static readonly Color NoColor = new(-1f, -1f, -1f);
  public static readonly Vector3 NoColorVector3 = new(-1f, -1f, -1f);
  public static readonly Vector3 BlackColorVector3 = new(0.00012345f, 0.00012345f, 0.00012345f);

  public static Vector3 ColorToVector3(Color color) {
    return color == Color.black ? BlackColorVector3 : new(color.r, color.g, color.b);
  }

  public static Color Vector3ToColor(Vector3 vector3) {
    return vector3 == BlackColorVector3 ? Color.black : new(vector3.x, vector3.y, vector3.z);
  }

  public static bool ChangePortalColor(TeleportWorld targetTeleportWorld) {
    return SetTeleportWorldColor(targetTeleportWorld, TargetPortalColor.Value);
  }

  public static bool SetTeleportWorldColor(TeleportWorld targetTeleportWorld, Color targetColor) {
    if (!targetTeleportWorld) {
      return false;
    }

    ZNetView netView = targetTeleportWorld.m_nview;

    if (!TryClaimOwnership(netView)) {
      return false;
    }

    SetTeleportWorldColorZDO(netView.m_zdo, ColorToVector3(targetColor), targetColor.a);

    if (targetTeleportWorld.TryGetComponent(out TeleportWorldColor teleportWorldColor)) {
      teleportWorldColor.UpdateColors(true);
    }

    return true;
  }

  public static bool TryClaimOwnership(ZNetView netView) {
    if (!netView || !netView.IsValid()) {
      return false;
    }

    if (!PrivateArea.CheckAccess(netView.transform.position, flash: true)) {
      return false;
    }

    if (netView.gameObject.TryGetComponentInChildren(out Container container)
        && (container.m_inUse || netView.m_zdo.GetInt(ZDOVars.s_inUse, 0) == 1)) {
      ColorfulPortals.LogError($"Container in target is currently in use!");
      return false;
    }

    netView.ClaimOwnership();

    return true;
  }

  public static void SetTeleportWorldColorZDO(ZDO zdo, Vector3 colorVector3, float colorAlpha) {
    zdo.Set(TeleportWorldColorHashCode, colorVector3);
    zdo.Set(TeleportWorldColorAlphaHashCode, colorAlpha);
    zdo.Set(PortalLastColoredByHashCode, Player.m_localPlayer.GetPlayerID());
    zdo.Set(PortalLastColoredByHostHashCode, PlatformManager.DistributionPlatform.LocalUser.PlatformUserID.m_userID);
  }
}
