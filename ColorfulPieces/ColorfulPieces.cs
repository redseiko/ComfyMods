using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using static ColorfulPieces.PluginConfig;

namespace ColorfulPieces {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class ColorfulPieces : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.colorfulpieces";
    public const string PluginName = "ColorfulPieces";
    public const string PluginVersion = "1.4.0";

    static readonly int _pieceColorHashCode = "PieceColor".GetStableHashCode();
    static readonly int _pieceEmissionColorFactorHashCode = "PieceEmissionColorFactor".GetStableHashCode();
    static readonly int _pieceLastColoredByHashCode = "PieceLastColoredBy".GetStableHashCode();

    static readonly Dictionary<ZDOID, WearNTearData> _wearNTearDataCache = new();

    static ManualLogSource _logger;
    Harmony _harmony;

    public void Awake() {
      _logger = Logger;

      CreateConfig(Config);

      _targetPieceColor.SettingChanged += UpdateColorHexValue;
      _targetPieceColorHex.SettingChanged += UpdateColorValue;

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    void UpdateColorHexValue(object sender, EventArgs eventArgs) {
      Color color = _targetPieceColor.Value;
      color.a = 1.0f; // Alpha transparency is unsupported.

      _targetPieceColorHex.Value = $"#{ColorUtility.ToHtmlStringRGB(color)}";
      _targetPieceColor.Value = color;
    }

    void UpdateColorValue(object sender, EventArgs eventArgs) {
      if (ColorUtility.TryParseHtmlString(_targetPieceColorHex.Value, out Color color)) {
        color.a = 1.0f; // Alpha transparency is unsupported.
        _targetPieceColor.Value = color;
      }
    }

    [HarmonyPatch(typeof(Player))]
    class PlayerPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.TakeInput))]
      static void TakeInputPostfix(ref bool __result) {
        if (_isModEnabled.Value
            && Player.m_localPlayer
            && Player.m_localPlayer.m_hovering 
            && ProcessColorAction(Player.m_localPlayer.m_hovering)) {
          __result = false;
        }
      }
    }

    static bool ProcessColorAction(GameObject hoveringObj) {
      if (_changePieceColorShortcut.Value.IsDown()) {
        ChangePieceColorAction(hoveringObj.GetComponentInParent<WearNTear>());
        return true;
      } else if (_clearPieceColorShortcut.Value.IsDown()) {
        ClearPieceColorAction(hoveringObj.GetComponentInParent<WearNTear>());
        return true;
      } else if (_copyPieceColorShortcut.Value.IsDown()) {
        CopyPieceColorAction(hoveringObj.GetComponentInParent<WearNTear>());
        return true;
      }

      return false;
    }

    static bool ClaimOwnership(WearNTear wearNTear) {
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

    static void ChangePieceColorAction(WearNTear wearNTear) {
      if (!ClaimOwnership(wearNTear)) {
        return;
      }

      wearNTear.m_nview.m_zdo.Set(_pieceColorHashCode, Utils.ColorToVec3(_targetPieceColor.Value));
      wearNTear.m_nview.m_zdo.Set(_pieceEmissionColorFactorHashCode, _targetPieceEmissionColorFactor.Value);
      wearNTear.m_nview.m_zdo.Set(_pieceLastColoredByHashCode, Player.m_localPlayer.GetPlayerID());

      if (_wearNTearDataCache.TryGetValue(wearNTear.m_nview.m_zdo.m_uid, out WearNTearData wearNTearData)) {
        wearNTearData.TargetColor = _targetPieceColor.Value;
        wearNTearData.TargetEmissionColorFactor = _targetPieceEmissionColorFactor.Value;

        SetWearNTearColors(wearNTearData);
      }

      if (wearNTear.m_piece) {
        wearNTear.m_piece.m_placeEffect.Create(wearNTear.transform.position, wearNTear.transform.rotation);
      }
    }

    static void ClearPieceColorAction(WearNTear wearNTear) {
      if (!ClaimOwnership(wearNTear)) {
        return;
      }

      if (wearNTear.m_nview.m_zdo.RemoveVec3(_pieceColorHashCode)
          || wearNTear.m_nview.m_zdo.RemoveFloat(_pieceEmissionColorFactorHashCode)) {
        wearNTear.m_nview.m_zdo.Set(_pieceLastColoredByHashCode, Player.m_localPlayer.GetPlayerID());
        wearNTear.m_nview.m_zdo.IncreseDataRevision();
      }

      if (_wearNTearDataCache.TryGetValue(wearNTear.m_nview.m_zdo.m_uid, out WearNTearData wearNTearData)) {
        wearNTearData.TargetColor = Color.clear;
        wearNTearData.TargetEmissionColorFactor = 0f;

        ClearWearNTearColors(wearNTearData);
      }

      if (wearNTear.m_piece) {
        wearNTear.m_piece.m_placeEffect.Create(wearNTear.transform.position, wearNTear.transform.rotation);
      }
    }

    static void CopyPieceColorAction(WearNTear wearNTear) {
      if (!wearNTear.m_nview
          || !wearNTear.m_nview.IsValid()
          || !wearNTear.m_nview.m_zdo.TryGetVec3(_pieceColorHashCode, out Vector3 colorAsVector)) {
        return;
      }

      _targetPieceColor.Value = Utils.Vec3ToColor(colorAsVector);
      _targetPieceColorHex.Value = $"#{ColorUtility.ToHtmlStringRGB(_targetPieceColor.Value)}";

      if (wearNTear.m_nview.m_zdo.TryGetFloat(_pieceEmissionColorFactorHashCode, out float factor)) {
        _targetPieceEmissionColorFactor.Value = factor;
      }

      MessageHud.instance?.ShowMessage(
          MessageHud.MessageType.TopLeft,
          $"Copied piece color: {_targetPieceColorHex.Value} (f: {_targetPieceEmissionColorFactor.Value})");
    }

    [HarmonyPatch(typeof(WearNTear))]
    class WearNTearPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(WearNTear.Awake))]
      static void WearNTearAwakePostfix(ref WearNTear __instance) {
        if (!_isModEnabled.Value || !__instance?.m_nview || !__instance.m_nview.IsValid()) {
          return;
        }

        WearNTearData wearNTearData = new(__instance);
        ClearWearNTearColors(wearNTearData);

        _wearNTearDataCache[__instance.m_nview.m_zdo.m_uid] = wearNTearData;
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(WearNTear.OnDestroy))]
      static void WearNTearOnDestroyPrefix(ref WearNTear __instance) {
        if (__instance?.m_nview
            && __instance.m_nview.IsValid()
            && _wearNTearDataCache.TryGetValue(__instance.m_nview.m_zdo.m_uid, out WearNTearData wearNTearData)) {
          wearNTearData.TargetColor = Color.clear;
          wearNTearData.TargetEmissionColorFactor = 0f;

          ClearWearNTearColors(wearNTearData);
        }
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(WearNTear.UpdateWear))]
      static void WearNTearUpdateWearPostfix(ref WearNTear __instance) {
        if (!_isModEnabled.Value
            || !__instance
            || !__instance.m_nview
            || __instance.m_nview.m_zdo == null
            || __instance.m_nview.m_zdo.m_zdoMan == null
            || __instance.m_nview.m_zdo.m_vec3 == null
            || !_wearNTearDataCache.TryGetValue(__instance.m_nview.m_zdo.m_uid, out WearNTearData wearNTearData)
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
    class HudPatch {
      static readonly string _hoverNameTextTemplate =
        "{0}{1}"
            + "<size={9}>"
            + "[<color={2}>{3}</color>] Set piece color: <color=#{4}>#{4}</color> (<color=#{4}>{5}</color>)\n"
            + "[<color={6}>{7}</color>] Clear piece color\n"
            + "[<color={6}>{8}</color>] Copy piece color\n"
            + "</size>";

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Hud.UpdateCrosshair))]
      static void HudUpdateCrosshairPostfix(ref Hud __instance, ref Player player) {
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
                _changePieceColorShortcut.Value,
                ColorUtility.ToHtmlStringRGB(_targetPieceColor.Value),
                _targetPieceEmissionColorFactor.Value.ToString("N2"),
                "#EF5350",
                _clearPieceColorShortcut.Value,
                _copyPieceColorShortcut.Value,
                _colorPromptFontSize.Value);
      }
    }

    static void SetWearNTearColors(WearNTearData wearNTearData) {
      foreach (Material material in wearNTearData.Materials) {
        if (material.HasProperty("_EmissionColor")) {
          material.SetColor("_EmissionColor", wearNTearData.TargetColor * wearNTearData.TargetEmissionColorFactor);
        }

        material.color = wearNTearData.TargetColor;
      }
    }

    static void ClearWearNTearColors(WearNTearData wearNTearData) {
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

  public static class ZDOExtensions {
    public static bool TryGetVec3(this ZDO zdo, int keyHashCode, out Vector3 value) {
      if (zdo == null || zdo.m_vec3 == null) {
        value = default;
        return false;
      }

      return zdo.m_vec3.TryGetValue(keyHashCode, out value);
    }

    public static bool TryGetFloat(this ZDO zdo, int keyHashCode, out float value) {
      if (zdo == null || zdo.m_floats == null) {
        value = default;
        return false;
      }

      return zdo.m_floats.TryGetValue(keyHashCode, out value);
    }

    public static bool RemoveVec3(this ZDO zdo, int keyHashCode) {
      return zdo != null && zdo.m_vec3 != null && zdo.m_vec3.Remove(keyHashCode);
    }

    public static bool RemoveFloat(this ZDO zdo, int keyHashCode) {
      return zdo != null && zdo.m_floats != null && zdo.m_floats.Remove(keyHashCode);
    }
  }
}
