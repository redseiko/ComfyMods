using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace DyeHard {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class DyeHard : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.dyehard";
    public const string PluginName = "DyeHard";
    public const string PluginVersion = "1.2.0";

    static readonly int _hairColorHashCode = "HairColor".GetStableHashCode();

    static readonly string[] _beardItems = {
      string.Empty,
      "BeardNone",
      "Beard1",
      "Beard2",
      "Beard3",
      "Beard4",
      "Beard5",
      "Beard6",
      "Beard7",
      "Beard8",
      "Beard9",
      "Beard10"
    };

    static ConfigEntry<Color> _playerHairColor;
    static ConfigEntry<string> _playerHairColorHex;
    static ConfigEntry<float> _playerHairGlow;
    static ConfigEntry<string> _playerBeardItem;
    static ConfigEntry<bool> _isModEnabled;

    static Player _localPlayer;

    Harmony _harmony;

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

      _playerBeardItem =
          Config.Bind(
              "Beard",
              "playerBeardItem",
              string.Empty,
              new ConfigDescription(
                  "If non-empty, sets/overrides the player's beard (if any).",
                  new AcceptableValueList<string>(_beardItems)));

      _isModEnabled.SettingChanged += (sender, eventArgs) => SetPlayerZdoHairColor();
      _playerHairColor.SettingChanged += (sender, eventArgs) => UpdatePlayerHairColorHexValue();
      _playerHairColorHex.SettingChanged += (sender, eventArgs) => UpdatePlayerHairColorValue();
      _playerHairGlow.SettingChanged += (sender, eventArgs) => SetPlayerZdoHairColor();
      _playerBeardItem.SettingChanged += (sender, eventArgs) => SetPlayerBeardItem();

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    void UpdatePlayerHairColorHexValue() {
      Color color = _playerHairColor.Value;
      color.a = 1f; // Alpha transparency is unsupported.

      _playerHairColorHex.Value = $"#{ColorUtility.ToHtmlStringRGB(color)}";
      _playerHairColor.Value = color;

      SetPlayerZdoHairColor();
    }

    void UpdatePlayerHairColorValue() {
      if (ColorUtility.TryParseHtmlString(_playerHairColorHex.Value, out Color color)) {
        color.a = 1f; // Alpha transparency is unsupported.
        _playerHairColor.Value = color;

        SetPlayerZdoHairColor();
      }
    }

    static Vector3 GetPlayerHairColorVector() {
      Vector3 colorVector = Utils.ColorToVec3(_playerHairColor.Value);

      if (colorVector != Vector3.zero) {
        colorVector *= _playerHairGlow.Value / colorVector.magnitude;
      }

      return colorVector;
    }

    [HarmonyPatch(typeof(FejdStartup))]
    class FejdStartupPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(FejdStartup.SetupCharacterPreview))]
      static void FejdStartupSetupCharacterPreviewPostfix(ref FejdStartup __instance) {
        _localPlayer = __instance.m_playerInstance.GetComponent<Player>();
        SetPlayerZdoHairColor();
      }
    }

    [HarmonyPatch(typeof(VisEquipment))]
    class VisEquipmentPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(VisEquipment.SetHairColor))]
      static void VisEquipmentSetHairColorPrefix(ref VisEquipment __instance, ref Vector3 color) {
        if (_isModEnabled.Value && __instance.TryGetComponent(out Player player) && player == _localPlayer) {
          color = GetPlayerHairColorVector();
        }
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(VisEquipment.SetBeardItem))]
      static void SetBeardItemPrefix(ref VisEquipment __instance, ref string name) {
        if (_isModEnabled.Value && __instance.TryGetComponent(out Player player) && player == _localPlayer) {
          name = _playerBeardItem.Value;
        }
      }
    }

    [HarmonyPatch(typeof(Player))]
    class PlayerPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.SetLocalPlayer))]
      static void PlayerSetLocalPlayerPostfix(ref Player __instance) {
        _localPlayer = __instance;
        SetPlayerZdoHairColor();
        SetPlayerBeardItem();
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.OnSpawned))]
      static void PlayerOnSpawnedPostfix(ref Player __instance) {
        _localPlayer = __instance;
        SetPlayerZdoHairColor();
        SetPlayerBeardItem();
      }
    }

    static void SetPlayerZdoHairColor() {
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

    static void SetPlayerBeardItem() {
      if (!_localPlayer?.m_visEquipment) {
        return;
      }

      string beardItem = _isModEnabled.Value ? _playerBeardItem.Value : _localPlayer.m_beardItem;

      if (_localPlayer.m_nview) {
        _localPlayer.m_visEquipment.SetBeardItem(beardItem);
      }

      _localPlayer.m_visEquipment.m_beardItem = beardItem;
    }
  }
}