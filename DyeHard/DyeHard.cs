using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace DyeHard {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class DyeHard : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.dyehard";
    public const string PluginName = "DyeHard";
    public const string PluginVersion = "1.0.0";

    private static readonly int _hairColorHashCode = "HairColor".GetStableHashCode();

    private static ConfigEntry<Color> _playerHairColor;
    private static ConfigEntry<string> _playerHairColorHex;
    private static ConfigEntry<float> _playerHairGlow;

    private static ConfigEntry<bool> _isModEnabled;

    private static ManualLogSource _logger;
    private Harmony _harmony;

    private static Player _localPlayer;

    public void Awake() {
      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _playerHairColor =
          Config.Bind("HairColor", "playerHairColor", Color.white, "Sets the color for your player's hair.");

      _playerHairColorHex =
          Config.Bind(
              "HairColor",
              "playerHairColorHex",
              $"#{ColorUtility.ToHtmlStringRGB(Color.white)}",
              "Sets the color of player hair, in HTML hex form (alpha unsupported).");

      _playerHairGlow =
          Config.Bind(
              "HairColor",
              "playerHairGlow",
              1f,
              new ConfigDescription(
                  "Hair glow multiplier for the hair color. Zero removes all color.",
                  new AcceptableValueRange<float>(0f, 3f)));

      _isModEnabled.SettingChanged += (sender, eventArgs) => SetPlayerZdoHairColor();
      _playerHairColor.SettingChanged += (sender, eventArgs) => UpdatePlayerHairColorHexValue();
      _playerHairColorHex.SettingChanged += (sender, eventArgs) => UpdatePlayerHairColorValue();
      _playerHairGlow.SettingChanged += (sender, eventArgs) => SetPlayerZdoHairColor();

      _logger = Logger;
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void OnDestroy() {
      if (_harmony != null) {
        _harmony.UnpatchSelf();
      }
    }

    private void UpdatePlayerHairColorHexValue() {
      Color color = _playerHairColor.Value;
      color.a = 1f; // Alpha transparency is unsupported.

      _playerHairColorHex.Value = $"#{ColorUtility.ToHtmlStringRGB(color)}";
      _playerHairColor.Value = color;

      SetPlayerZdoHairColor();
    }

    private void UpdatePlayerHairColorValue() {
      if (ColorUtility.TryParseHtmlString(_playerHairColorHex.Value, out Color color)) {
        color.a = 1f; // Alpha transparency is unsupported.
        _playerHairColor.Value = color;

        SetPlayerZdoHairColor();
      }
    }

    private static Vector3 GetPlayerHairColorVector() {
      Vector3 colorVector = Utils.ColorToVec3(_playerHairColor.Value);

      if (colorVector != Vector3.zero) {
        colorVector *= _playerHairGlow.Value / colorVector.magnitude;
      }

      return colorVector;
    }

    [HarmonyPatch(typeof(FejdStartup))]
    private class FejdStartupPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(FejdStartup.SetupCharacterPreview))]
      private static void FejdStartupSetupCharacterPreviewPostfix(ref FejdStartup __instance) {
        _localPlayer = __instance.m_playerInstance.GetComponent<Player>();
        SetPlayerZdoHairColor();
      }
    }

    [HarmonyPatch(typeof(VisEquipment))]
    private class VisEquipmentPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(VisEquipment.SetHairColor))]
      private static void VisEquipmentSetHairColorPrefix(ref VisEquipment __instance, ref Vector3 color) {
        if (_isModEnabled.Value && __instance.TryGetComponent(out Player player) && player == _localPlayer) {
          color = GetPlayerHairColorVector();
        }
      }
    }

    [HarmonyPatch(typeof(Player))]
    private class PlayerPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.SetLocalPlayer))]
      private static void PlayerSetLocalPlayerPostfix(ref Player __instance) {
        _localPlayer = __instance;
        SetPlayerZdoHairColor();
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.OnSpawned))]
      private static void PlayerOnSpawnedPostfix(ref Player __instance) {
        _localPlayer = __instance;
        SetPlayerZdoHairColor();
      }
    }

    private static void SetPlayerZdoHairColor() {
      if (!_localPlayer || !_localPlayer.m_visEquipment) {
        return;
      }

      Vector3 color = _isModEnabled.Value ? GetPlayerHairColorVector() : _localPlayer.m_hairColor;
      _localPlayer.m_visEquipment.m_hairColor = color;

      if (!_localPlayer.m_nview || !_localPlayer.m_nview.IsValid()) {
        return;
      }

      if (_localPlayer.m_nview.m_zdo.m_vec3 == null
          || !_localPlayer.m_nview.m_zdo.m_vec3.ContainsKey(_hairColorHashCode)
          || _localPlayer.m_nview.m_zdo.m_vec3[_hairColorHashCode] != color) {
        _localPlayer.m_nview.GetZDO().Set(_hairColorHashCode, color);
      }
    }
  }
}