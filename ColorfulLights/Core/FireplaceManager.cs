namespace ColorfulLights;

using ComfyLib;

using Splatform;

using UnityEngine;

using static PluginConfig;

public static class FireplaceManager {
  public static readonly int FirePlaceColorHashCode = "FireplaceColor".GetStableHashCode();
  public static readonly int FireplaceColorAlphaHashCode = "FireplaceColorAlpha".GetStableHashCode();
  public static readonly int LightLastColoredByHashCode = "LightLastColoredBy".GetStableHashCode();
  public static readonly int LightLastColoredByHostHashCode = "LightLastColoredByHost".GetStableHashCode();

  public static bool ChangeFireplaceColor(GameObject hoverTarget) {
    return
        hoverTarget
        && hoverTarget.TryGetComponentInParent(out Fireplace targetFireplace)
        && SetFireplaceColor(targetFireplace, TargetFireplaceColor.Value);
  }

  public static bool SetFireplaceColor(Fireplace targetFireplace, Color targetColor) {
    if (!targetFireplace) {
      return false;
    }

    ZNetView netView = targetFireplace.m_nview;

    if (!netView || !netView.IsValid() || !TryClaimOwnership(netView)) {
      return false;
    }

    Vector3 colorVec3 = Utils.ColorToVec3(targetColor);
    float colorAlpha = targetColor.a;

    SetFireplaceColorZDO(netView.m_zdo, colorVec3, colorAlpha);

    targetFireplace.m_fuelAddedEffects?.Create(
        targetFireplace.transform.position, targetFireplace.transform.rotation);

    if (targetFireplace.TryGetComponent(out FireplaceColor fireplaceColor)) {
      fireplaceColor.SetFireplaceColors(colorVec3, colorAlpha);
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
      ColorfulLights.LogError($"Container in target is currently in use!");
      return false;
    }

    netView.ClaimOwnership();

    return true;
  }

  public static void SetFireplaceColorZDO(ZDO zdo, Vector3 colorVec3, float colorAlpha) {
    zdo.Set(FirePlaceColorHashCode, colorVec3);
    zdo.Set(FireplaceColorAlphaHashCode, colorAlpha);
    zdo.Set(LightLastColoredByHashCode, Player.m_localPlayer.GetPlayerID());
    zdo.Set(LightLastColoredByHostHashCode, PlatformManager.DistributionPlatform.LocalUser.PlatformUserID.m_userID);
  }
}
