namespace ColorfulWards;

using ComfyLib;

using Splatform;

using UnityEngine;

using static PluginConfig;

public static class PrivateAreaManager {
  public static readonly Color NoColor = new(-1f, -1f, -1f);
  public static readonly Vector3 NoColorVector3 = new(-1f, -1f, -1f);
  public static readonly Vector3 BlackColorVector3 = new(0.00012345f, 0.00012345f, 0.00012345f);

  public static Vector3 ColorToVector3(Color color) {
    return color == Color.black ? BlackColorVector3 : new(color.r, color.g, color.b);
  }

  public static Color Vector3ToColor(Vector3 vector3) {
    return vector3 == BlackColorVector3 ? Color.black : new(vector3.x, vector3.y, vector3.z);
  }

  public static readonly int PrivateAreaColorHashCode = "PrivateAreaColor".GetStableHashCode();
  public static readonly int PrivateAreaColorAlphaHashCode = "PrivateAreaColorAlpha".GetStableHashCode();
  public static readonly int WardLastColoredByHashCode = "WardLastColoredBy".GetStableHashCode();
  public static readonly int WardLastColoredByHostHashCode = "WardLastColoredByHost".GetStableHashCode();

  public static bool ChangePrivateAreaColor(PrivateArea targetPrivateArea) {
    return SetPrivateAreColor(targetPrivateArea, TargetWardColor.Value);
  }

  public static bool SetPrivateAreColor(PrivateArea targetPrivateArea, Color targetColor) {
    if (!targetPrivateArea) {
      return false;
    }

    ZNetView netView = targetPrivateArea.m_nview;

    if (!netView || !netView.IsValid() || !targetPrivateArea.m_piece.IsCreator()) {
      return false;
    }

    if (netView.gameObject.TryGetComponentInChildren(out Container container)
        && (container.m_inUse || netView.m_zdo.GetInt(ZDOVars.s_inUse, 0) == 1)) {
      ColorfulWards.LogError($"Container in target is currently in use!");
      return false;
    }

    netView.ClaimOwnership();
    SetPrivateAreaColorZDO(netView.m_zdo, targetColor);

    targetPrivateArea.m_flashEffect?.Create(targetPrivateArea.transform.position, Quaternion.identity);

    if (targetPrivateArea.TryGetComponent(out PrivateAreaColor privateAreaColor)) {
      privateAreaColor.UpdateColors(true);
    }

    return true;
  }

  public static void SetPrivateAreaColorZDO(ZDO zdo, Color targetWardColor) {
    zdo.Set(PrivateAreaColorHashCode, ColorToVector3(targetWardColor));
    zdo.Set(PrivateAreaColorAlphaHashCode, targetWardColor.a);
    zdo.Set(WardLastColoredByHashCode, Player.m_localPlayer.GetPlayerID());
    zdo.Set(WardLastColoredByHostHashCode, PlatformManager.DistributionPlatform.LocalUser.PlatformUserID.m_userID);
  }
}
