using BepInEx;
using BepInEx.Configuration;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace LetMePlay {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class LetMePlay : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.letmeplay";
    public const string PluginName = "LetMePlay";
    public const string PluginVersion = "1.1.0";

    static ConfigEntry<bool> _isModEnabled;
    static ConfigEntry<bool> _disableWardShieldFlash;
    static ConfigEntry<bool> _disableCameraSwayWhileSitting;
    static ConfigEntry<bool> _disableBuildPlacementMarker;

    private Harmony _harmony;

    public void Awake() {
      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _disableWardShieldFlash =
          Config.Bind("Effects", "disableWardShieldFlash", false, "Disable wards from flashing their blue shield.");

      _disableCameraSwayWhileSitting =
          Config.Bind("Camera", "disableCameraSwayWhileSitting", false, "Disables the camera sway while sitting.");

      _disableBuildPlacementMarker =
          Config.Bind(
              "Build",
              "disableBuildPlacementMarker",
              false,
              "Disables the yellow placement marker (and gizmo indicator) when building.");

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    static readonly Dictionary<string, Sprite> _spriteCache = new();

    static Sprite GetSprite(string spriteName) {
      if (!_spriteCache.TryGetValue(spriteName, out Sprite sprite)) {
        sprite = Resources.FindObjectsOfTypeAll<Sprite>().First(obj => obj.name == spriteName);
        _spriteCache[spriteName] = sprite;
      }


      return sprite;
    }

    [HarmonyPatch(typeof(PrivateArea))]
    class PrivateAreaPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(PrivateArea.RPC_FlashShield))]
      static bool PrivateAreaRpcFlashShield() {
        if (_isModEnabled.Value && _disableWardShieldFlash.Value) {
          return false;
        }

        return true;
      }
    }

    [HarmonyPatch(typeof(ItemDrop.ItemData))]
    class ItemDataPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(ItemDrop.ItemData.GetIcon))]
      static void ItemDataGetIcon(ref ItemDrop.ItemData __instance) {
        if (!_isModEnabled.Value) {
          return;
        }

        if (__instance.m_variant < 0 || __instance.m_variant >= __instance.m_shared.m_icons.Length) {
          Array.Resize(ref __instance.m_shared.m_icons, __instance.m_variant + 1);

          __instance.m_shared.m_icons[__instance.m_variant] = GetSprite("hammer_icon_small");
          __instance.m_shared.m_name = __instance.m_dropPrefab.name;
          __instance.m_shared.m_description = $"Non-player item: {__instance.m_dropPrefab.name}";
          __instance.m_shared.m_itemType = ItemDrop.ItemData.ItemType.Misc;
          __instance.m_crafterID = 12345678L;
          __instance.m_crafterName = "redseiko.valheim.letmeplay";
        }
      }
    }

    [HarmonyPatch(typeof(GameCamera))]
    class GameCameraPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(GameCamera.GetCameraBaseOffset))]
      static void GetCameraBaseOffsetPostfix(ref Vector3 __result, Player player) {
        if (_isModEnabled.Value && _disableCameraSwayWhileSitting.Value) {
          __result = player.m_eye.transform.position - player.transform.position;
        }
      }
    }

    [HarmonyPatch(typeof(Player))]
    class PlayerPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.UpdatePlacementGhost))]
      static void UpdatePlacementGhostPostfix(ref Player __instance) {
        if (__instance
            && __instance.m_placementMarkerInstance
            && __instance.m_placementMarkerInstance.activeSelf
            && _isModEnabled.Value
            && _disableBuildPlacementMarker.Value) {
          __instance.m_placementMarkerInstance.SetActive(false);
        }
      }
    }
  }
}
