using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static ColorfulPortals.PluginConfig;

namespace ColorfulPortals {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public sealed class ColorfulPortals : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.colorfulportals";
    public const string PluginName = "ColorfulPortals";
    public const string PluginVersion = "1.8.0";

    static ManualLogSource _logger;
    Harmony _harmony;

    void Awake() {
      _logger = Logger;
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static readonly Color NoColor = new(-1f, -1f, -1f);
    public static readonly Vector3 NoColorVector3 = new(-1f, -1f, -1f);
    public static readonly Vector3 BlackColorVector3 = new(0.00012345f, 0.00012345f, 0.00012345f);

    public static readonly int ColorShaderId = Shader.PropertyToID("_Color");

    public static readonly int TeleportWorldColorHashCode = "TeleportWorldColor".GetStableHashCode();
    public static readonly int TeleportWorldColorAlphaHashCode = "TeleportWorldColorAlpha".GetStableHashCode();
    public static readonly int PortalLastColoredByHashCode = "PortalLastColoredBy".GetStableHashCode();
    public static readonly int PortalLastColoredByHostHashCode = "PortalLastColoredByHost".GetStableHashCode();

    public static void ChangePortalColor(TeleportWorld targetPortal) {
      if (!targetPortal) {
        return;
      }

      if (!targetPortal.m_nview || !targetPortal.m_nview.IsValid()) {
        _logger.LogWarning("TeleportWorld does not have a valid ZNetView.");
        return;
      }

      if (!PrivateArea.CheckAccess(targetPortal.transform.position, flash: true)) {
        _logger.LogWarning("TeleportWorld is within a PrivateArea.");
        return;
      }

      targetPortal.m_nview.ClaimOwnership();

      SetPortalColorZdoValues(
          targetPortal.m_nview.m_zdo, ColorToVector3(TargetPortalColor.Value), TargetPortalColor.Value.a);

      if (targetPortal.TryGetComponent(out TeleportWorldColor teleportWorldColor)) {
        teleportWorldColor.UpdateColors(true);
      }
    }

    static void SetPortalColorZdoValues(ZDO zdo, Vector3 colorVector3, float colorAlpha) {
      zdo.Set(TeleportWorldColorHashCode, colorVector3);
      zdo.Set(TeleportWorldColorAlphaHashCode, colorAlpha);
      zdo.Set(PortalLastColoredByHashCode, Player.m_localPlayer.Ref()?.GetPlayerID() ?? 0L);
      zdo.Set(PortalLastColoredByHostHashCode, PrivilegeManager.GetNetworkUserId());
    }

    public static Vector3 ColorToVector3(Color color) {
      return color == Color.black ? BlackColorVector3 : new(color.r, color.g, color.b);
    }

    public static Color Vector3ToColor(Vector3 vector3) {
      return vector3 == BlackColorVector3 ? Color.black : new(vector3.x, vector3.y, vector3.z);
    }
  }
}