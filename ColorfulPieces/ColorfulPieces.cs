using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ColorfulPieces {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class ColorfulPieces : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.colorfulpieces";
    public const string PluginName = "ColorfulPieces";
    public const string PluginVersion = "1.1.0";

    private static readonly KeyboardShortcut _changeColorActionShortcut = new(KeyCode.R, KeyCode.LeftShift);
    private static readonly KeyboardShortcut _clearColorActionShortcut = new(KeyCode.R, KeyCode.LeftAlt);

    private static readonly int _pieceColorHashCode = "PieceColor".GetStableHashCode();
    private static readonly int _pieceEmissionColorFactorHashCode = "PieceEmissionColorFactor".GetStableHashCode();

    private class WearNTearData {
      public uint LastDataRevision { get; set; } = 0U;
      public List<Material> Materials { get; } = new List<Material>();
      public Color TargetColor { get; set; } = Color.clear;
      public float TargetEmissionColorFactor { get; set; } = 0f;

      public WearNTearData(WearNTear wearNTear) {
        Materials.AddRange(wearNTear.GetComponentsInChildren<MeshRenderer>(true).Select(r => r.material));
        Materials.AddRange(wearNTear.GetComponentsInChildren<SkinnedMeshRenderer>(true).Select(r => r.material));

        foreach (Material material in Materials) {
          SaveMaterialColors(material);
        }
      }

      private static void SaveMaterialColors(Material material) {
        if (material.HasProperty("_Color")) {
          material.SetColor("_SavedColor", material.GetColor("_Color"));
        }

        if (material.HasProperty("_EmissionColor")) {
          material.SetColor("_SavedEmissionColor", material.GetColor("_EmissionColor"));
        }
      }
    }

    private static readonly Dictionary<WearNTear, WearNTearData> _wearNTearDataCache = new();

    private static ConfigEntry<bool> _isModEnabled;
    private static ConfigEntry<Color> _targetPieceColor;
    private static ConfigEntry<string> _targetPieceColorHex;
    private static ConfigEntry<float> _targetPieceEmissionColorFactor;
    private static ConfigEntry<bool> _showChangeRemoveColorPrompt;

    private static ManualLogSource _logger;
    private Harmony _harmony;

    private void Awake() {
      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _targetPieceColor =
          Config.Bind("Color", "targetPieceColor", Color.cyan, "Target color to set the piece material to.");

      _targetPieceColorHex =
          Config.Bind(
              "Color",
              "targetPieceColorHex",
              $"#{ColorUtility.ToHtmlStringRGB(Color.cyan)}",
              "Target color to set the piece material to, in HTML hex form (alpha unsupported).");

      _targetPieceColor.SettingChanged += UpdateColorHexValue;
      _targetPieceColorHex.SettingChanged += UpdateColorValue;

      _targetPieceEmissionColorFactor =
          Config.Bind(
              "Color",
              "targetPieceEmissionColorFactor",
              0.4f,
              new ConfigDescription(
                  "Factor to multiply the target color by and set as emission color.",
                  new AcceptableValueRange<float>(0f, 0.6f)));

      _showChangeRemoveColorPrompt =
          Config.Bind("Hud", "showChangeRemoveColorPrompt", true, "Show the 'change/remove' color text prompt.");

      _logger = Logger;
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void OnDestroy() {
      if (_harmony != null) {
        _harmony.UnpatchSelf();
      }
    }

    private void UpdateColorHexValue(object sender, EventArgs eventArgs) {
      Color color = _targetPieceColor.Value;
      color.a = 1.0f; // Alpha transparency is unsupported.

      _targetPieceColorHex.Value = $"#{ColorUtility.ToHtmlStringRGB(color)}";
      _targetPieceColor.Value = color;
    }

    private void UpdateColorValue(object sender, EventArgs eventArgs) {
      if (ColorUtility.TryParseHtmlString(_targetPieceColorHex.Value, out Color color)) {
        color.a = 1.0f; // Alpha transparency is unsupported.
        _targetPieceColor.Value = color;
      }
    }

    private void Update() {
      if (!_isModEnabled.Value || !Player.m_localPlayer || !Player.m_localPlayer.m_hovering) {
        return;
      } else if (_clearColorActionShortcut.IsDown()) {
        ClearPieceColorAction(Player.m_localPlayer.m_hovering.GetComponentInParent<WearNTear>());
      } else if (_changeColorActionShortcut.IsDown()) {
        ChangePieceColorAction(Player.m_localPlayer.m_hovering.GetComponentInParent<WearNTear>());
      }
    }

    private bool ClaimOwnership(WearNTear wearNTear) {
      if (!wearNTear
          || !wearNTear.m_nview
          || !wearNTear.m_nview.IsValid()
          || !PrivateArea.CheckAccess(wearNTear.transform.position, flash: true)) {
        _logger.LogWarning("Piece does not have a valid ZNetView or is in a PrivateArea.");
        return false;
      }

      if (!wearNTear.m_nview.IsOwner()) {
        wearNTear.m_nview.ClaimOwnership();
      }

      return true;
    }

    private void ChangePieceColorAction(WearNTear wearNTear) {
      if (!ClaimOwnership(wearNTear)) {
        return;
      }

      wearNTear.m_nview.m_zdo.Set(_pieceColorHashCode, Utils.ColorToVec3(_targetPieceColor.Value));
      wearNTear.m_nview.m_zdo.Set(_pieceEmissionColorFactorHashCode, _targetPieceEmissionColorFactor.Value);

      if (_wearNTearDataCache.TryGetValue(wearNTear, out WearNTearData wearNTearData)) {
        wearNTearData.TargetColor = _targetPieceColor.Value;
        wearNTearData.TargetEmissionColorFactor = _targetPieceEmissionColorFactor.Value;

        SetWearNTearColors(wearNTearData);
      }

      if (wearNTear.m_piece) {
        wearNTear.m_piece.m_placeEffect.Create(wearNTear.transform.position, wearNTear.transform.rotation);
      }
    }

    private void ClearPieceColorAction(WearNTear wearNTear) {
      if (!ClaimOwnership(wearNTear)) {
        return;
      }

      if (wearNTear.m_nview.m_zdo.RemoveVec3(_pieceColorHashCode)
          || wearNTear.m_nview.m_zdo.RemoveFloat(_pieceEmissionColorFactorHashCode)) {
        wearNTear.m_nview.m_zdo.IncreseDataRevision();
      }

      if (_wearNTearDataCache.TryGetValue(wearNTear, out WearNTearData wearNTearData)) {
        wearNTearData.TargetColor = Color.clear;
        wearNTearData.TargetEmissionColorFactor = 0f;

        ClearWearNTearColors(wearNTearData);
      }

      if (wearNTear.m_piece) {
        wearNTear.m_piece.m_placeEffect.Create(wearNTear.transform.position, wearNTear.transform.rotation);
      }
    }

    [HarmonyPatch(typeof(WearNTear))]
    private class WearNTearPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(WearNTear.Awake))]
      private static void WearNTearAwakePostfix(ref WearNTear __instance) {
        if (!_isModEnabled.Value || !__instance) {
          return;
        }

        _wearNTearDataCache[__instance] = new WearNTearData(__instance);
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(WearNTear.OnDestroy))]
      private static void WearNTearOnDestroyPrefix(ref WearNTear __instance) {
        _wearNTearDataCache.Remove(__instance);
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(WearNTear.UpdateWear))]
      private static void WearNTearUpdateWearPostfix(ref WearNTear __instance) {
        if (!_isModEnabled.Value
            || !__instance
            || !__instance.m_nview
            || __instance.m_nview.m_zdo == null
            || __instance.m_nview.m_zdo.m_zdoMan == null
            || __instance.m_nview.m_zdo.m_vec3 == null
            || !_wearNTearDataCache.TryGetValue(__instance, out WearNTearData wearNTearData)
            || wearNTearData.LastDataRevision >= __instance.m_nview.m_zdo.m_dataRevision) {
          return;
        }

        if (__instance.m_nview.m_zdo.m_vec3.TryGetValue(_pieceColorHashCode, out Vector3 colorAsVector)) {
          wearNTearData.TargetColor = Utils.Vec3ToColor(colorAsVector);

          if (__instance.m_nview.m_zdo.m_floats != null
              && __instance.m_nview.m_zdo.m_floats.TryGetValue(_pieceEmissionColorFactorHashCode, out float factor)) {
            wearNTearData.TargetEmissionColorFactor = factor;
          }

          SetWearNTearColors(wearNTearData);
        } else if (wearNTearData.TargetColor != Color.clear) {
          wearNTearData.TargetColor = Color.clear;
          wearNTearData.TargetEmissionColorFactor = 0f;

          ClearWearNTearColors(wearNTearData);
        }

        wearNTearData.LastDataRevision = __instance.m_nview.m_zdo.m_dataRevision;
      }
    }

    [HarmonyPatch(typeof(Hud))]
    private class HudPatch {
      private static readonly string _hoverNameTextTemplate =
        "{0}{1}"
            + "[<color={2}>{3}</color>] Change piece color to: <color=#{4}>#{4}</color> (f: <color=#{4}>{5}</color>)\n"
            + "[<color={6}>{7}</color>] Clear existing piece color\n";

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Hud.UpdateCrosshair))]
      private static void HudUpdateCrosshairPostfix(ref Hud __instance, ref Player player) {
        if (!_isModEnabled.Value
            || !_showChangeRemoveColorPrompt.Value
            || !__instance
            || !player
            || player != Player.m_localPlayer
            || !player.m_hovering) {
          return;
        }

        WearNTear wearNTear = player.m_hovering.GetComponentInParent<WearNTear>();

        if (!wearNTear || !wearNTear.m_nview || !wearNTear.m_nview.IsValid()) {
          return;
        }

        __instance.m_hoverName.text =
            string.Format(
                _hoverNameTextTemplate,
                __instance.m_hoverName.text,
                __instance.m_hoverName.text.Length > 0 ? "\n" : string.Empty,
                "#FFA726",
                _changeColorActionShortcut,
                ColorUtility.ToHtmlStringRGB(_targetPieceColor.Value),
                _targetPieceEmissionColorFactor.Value.ToString("N2"),
                "#EF5350",
                _clearColorActionShortcut);
      }
    }

    private static void SetWearNTearColors(WearNTearData wearNTearData) {
      foreach (Material material in wearNTearData.Materials) {
        if (material.HasProperty("_EmissionColor")) {
          material.SetColor("_EmissionColor", wearNTearData.TargetColor * wearNTearData.TargetEmissionColorFactor);
        }

        material.color = wearNTearData.TargetColor;
      }
    }

    private static void ClearWearNTearColors(WearNTearData wearNTearData) {
      foreach (Material material in wearNTearData.Materials) {
        if (material.HasProperty("_SavedEmissionColor")) {
          material.SetColor("_EmissionColor", material.GetColor("_SavedEmissionColor"));
        }

        if (material.HasProperty("_SavedColor")) {
          material.SetColor("_Color", material.GetColor("_SavedColor"));
          material.color = material.GetColor("_SavedColor");
        }
      }
    }
  }

  internal static class ZDOExtensions {
    public static bool RemoveVec3(this ZDO zdo, int keyHashCode) {
      return zdo != null && zdo.m_vec3 != null && zdo.m_vec3.Remove(keyHashCode);
    }

    public static bool RemoveFloat(this ZDO zdo, int keyHashCode) {
      return zdo != null && zdo.m_floats != null && zdo.m_floats.Remove(keyHashCode);
    }
  }
}
