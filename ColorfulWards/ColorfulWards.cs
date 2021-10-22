using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using HarmonyLib;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace ColorfulWards {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class ColorfulWards : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.colorfulwards";
    public const string PluginName = "ColorfulWards";
    public const string PluginVersion = "1.3.0";

    static readonly Dictionary<PrivateArea, PrivateAreaData> _privateAreaData = new();

    static ConfigEntry<bool> _isModEnabled;
    static ConfigEntry<Color> _targetWardColor;
    static ConfigEntry<string> _targetWardColorHex;

    static ConfigEntry<bool> _useRadiusForVerticalCheck;
    static ConfigEntry<bool> _showChangeColorHoverText;
    static ConfigEntry<int> _colorPromptFontSize;

    static ManualLogSource _logger;
    Harmony _harmony;

    public void Awake() {
      _logger = Logger;

      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _targetWardColor =
          Config.Bind("Color", "targetWardColor", Color.cyan, "Target color to set the ward glow effect to.");

      _targetWardColorHex =
          Config.Bind(
              "Color",
              "targetWardColorHex",
              $"#{ColorUtility.ToHtmlStringRGB(Color.cyan)}",
              "Target color to set the ward glow effect to, in HTML hex form.");

      _targetWardColor.SettingChanged += UpdateColorHexValue;
      _targetWardColorHex.SettingChanged += UpdateColorValue;

      _useRadiusForVerticalCheck =
          Config.Bind(
              "PrivateArea",
              "useRadiusForVerticalCheck",
              true,
              "Use the ward radius for access/permission checks vertically. Vanilla is infinite up/down.");

      _showChangeColorHoverText =
          Config.Bind(
              "Hud",
              "showChangeColorHoverText",
              true,
              "Show the 'change color' text when hovering over a lightsoure.");

      _colorPromptFontSize =
          Config.Bind("Hud", "colorPromptFontSize", 15, "Font size for the 'change/remove/copy' color text prompt.");

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    void UpdateColorHexValue(object sender, EventArgs eventArgs) {
      _targetWardColorHex.Value = $"#{GetColorHtmlString(_targetWardColor.Value)}";
    }

    void UpdateColorValue(object sender, EventArgs eventArgs) {
      if (ColorUtility.TryParseHtmlString(_targetWardColorHex.Value, out Color color)) {
        _targetWardColor.Value = color;
      }
    }

    static string GetColorHtmlString(Color color) {
      return color.a == 1.0f
          ? ColorUtility.ToHtmlStringRGB(color)
          : ColorUtility.ToHtmlStringRGBA(color);
    }

    [HarmonyPatch(typeof(PrivateArea))]
    class PrivateAreaPatch {
      static readonly int _privateAreaColorHashCode = "PrivateAreaColor".GetStableHashCode();
      static readonly int _privateAreaColorAlphaHashCode = "PrivateAreaColorAlpha".GetStableHashCode();
      static readonly int _wardLastColoredByHashCode = "WardLastColoredBy".GetStableHashCode();

      static readonly KeyboardShortcut _changeColorActionShortcut = new(KeyCode.E, KeyCode.LeftShift);

      [HarmonyPrefix]
      [HarmonyPatch(nameof(PrivateArea.IsInside))]
      static bool PrivateAreaIsInside(
          ref PrivateArea __instance, ref bool __result, Vector3 point, float radius) {
        if (!_isModEnabled.Value || !_useRadiusForVerticalCheck.Value) {
          return true;
        }

        __result = Vector3.Distance(__instance.transform.position, point) < __instance.m_radius + radius;
        return false;
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(PrivateArea.Awake))]
      static void PrivateAreaAwakePostfix(ref PrivateArea __instance) {
        if (!_isModEnabled.Value || !__instance) {
          return;
        }

        _privateAreaData.Add(__instance, new PrivateAreaData(__instance));
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(PrivateArea.OnDestroy))]
      static void PrivateAreaOnDestroyPrefix(ref PrivateArea __instance) {
        _privateAreaData.Remove(__instance);
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(PrivateArea.Interact))]
      static bool PrivateAreaInteractPrefix(
          ref PrivateArea __instance, ref bool __result, Humanoid human, bool hold) {
        if (!_isModEnabled.Value || hold || !_changeColorActionShortcut.IsDown() || !__instance.m_piece) {
          return true;
        }

        if (!__instance.m_nview && !__instance.m_nview.IsValid()) {
          _logger.LogWarning("PrivateArea does not have a valid ZNetView.");

          __result = true;
          return false;
        }

        if (!__instance.m_piece.IsCreator()) {
          _logger.LogWarning("You are not the owner of this Ward.");

          __result = true;
          return false;
        }

        if (!__instance.m_nview.IsOwner()) {
          __instance.m_nview.ClaimOwnership();
        }

        __instance.m_nview.m_zdo.Set(_privateAreaColorHashCode, Utils.ColorToVec3(_targetWardColor.Value));
        __instance.m_nview.m_zdo.Set(_privateAreaColorAlphaHashCode, _targetWardColor.Value.a);
        __instance.m_nview.m_zdo.Set(_wardLastColoredByHashCode, Player.m_localPlayer?.GetPlayerID() ?? 0L);

        __instance.m_flashEffect.Create(__instance.transform.position, __instance.transform.rotation);

        if (_privateAreaData.TryGetValue(__instance, out PrivateAreaData privateAreaData)) {
          privateAreaData.TargetColor = _targetWardColor.Value;
          SetPrivateAreaColors(__instance, privateAreaData);
        }

        __instance.StartCoroutine(UpdateEnabledEffect(__instance));

        __result = true;
        return false;
      }

      static IEnumerator UpdateEnabledEffect(PrivateArea privateArea) {
        if (privateArea.IsEnabled()) {
          privateArea.m_enabledEffect.SetActive(false);
          yield return new WaitForEndOfFrame();
          privateArea.m_enabledEffect.SetActive(true);
        }
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(PrivateArea.UpdateStatus))]
      static void PrivateAreaUpdateStatusPostfix(ref PrivateArea __instance) {
        if (!_isModEnabled.Value
            || !__instance
            || !__instance.m_nview
            || __instance.m_nview.m_zdo == null
            || __instance.m_nview.m_zdo.m_zdoMan == null
            || __instance.m_nview.m_zdo.m_vec3 == null
            || !__instance.m_nview.m_zdo.m_vec3.ContainsKey(_privateAreaColorHashCode)
            || !_privateAreaData.TryGetValue(__instance, out PrivateAreaData privateAreaData)) {
          return;
        }

        Color wardColor = Utils.Vec3ToColor(__instance.m_nview.m_zdo.m_vec3[_privateAreaColorHashCode]);
        wardColor.a = __instance.m_nview.m_zdo.GetFloat(_privateAreaColorAlphaHashCode, defaultValue: 1f);

        privateAreaData.TargetColor = wardColor;
        SetPrivateAreaColors(__instance, privateAreaData);
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(PrivateArea.GetHoverText))]
      static void PrivateAreaGetHoverText(ref PrivateArea __instance, ref string __result) {
        if (!_isModEnabled.Value
            || !_showChangeColorHoverText.Value
            || !__instance
            || !__instance.m_piece
            || !__instance.m_piece.IsCreator()) {
          return;
        }

        __result =
            string.Format(
                "{0}\n<size={4}>[<color={1}>{2}</color>] Change ward color to: <color=#{3}>#{3}</color></size>",
                __result,
                "#FFA726",
                _changeColorActionShortcut,
                GetColorHtmlString(_targetWardColor.Value),
                _colorPromptFontSize.Value);
      }
    }

    static void SetPrivateAreaColors(PrivateArea privateArea, PrivateAreaData privateAreaData) {
      foreach (Light light in privateAreaData.PointLight) {
        light.color = privateAreaData.TargetColor;
      }

      foreach (Material material in privateAreaData.GlowMaterial) {
        material.SetColor("_EmissionColor", privateAreaData.TargetColor);
      }

      foreach (ParticleSystem system in privateAreaData.SparcsSystem) {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = system.colorOverLifetime;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(privateAreaData.TargetColor);

        ParticleSystem.MainModule main = system.main;
        main.startColor = privateAreaData.TargetColor;
      }

      foreach (ParticleSystemRenderer renderer in privateAreaData.SparcsRenderer) {
        renderer.material.color = privateAreaData.TargetColor;
      }

      foreach (ParticleSystem system in privateAreaData.FlareSystem) {
        Color flareColor = privateAreaData.TargetColor;
        flareColor.a = 0.1f;

        ParticleSystem.MainModule main = system.main;
        main.startColor = flareColor;
      }
    }
  }
}
