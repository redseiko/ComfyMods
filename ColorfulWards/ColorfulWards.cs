using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ColorfulWards {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class ColorfulWards : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.colorfulwards";
    public const string PluginName = "ColorfulWards";
    public const string PluginVersion = "1.1.0";

    private class PrivateAreaData {
      public List<Light> PointLight { get; } = new List<Light>();
      public List<Material> GlowMaterial { get; } = new List<Material>();
      public List<ParticleSystem> SparcsSystem { get; } = new List<ParticleSystem>();
      public List<ParticleSystemRenderer> SparcsRenderer { get; } = new List<ParticleSystemRenderer>();
      public List<ParticleSystem> FlareSystem { get; } = new List<ParticleSystem>();

      public Color TargetColor { get; set; } = Color.clear;

      public PrivateAreaData(PrivateArea privateArea) {
        PointLight.AddRange(FindChildren(privateArea, "Point light").Select(go => go.GetComponent<Light>()));
        GlowMaterial.AddRange(FindChildren(privateArea, "default").Select(go => go.GetComponent<Renderer>().material));

        foreach (GameObject sparcsObject in FindChildren(privateArea, "sparcs")) {
          SparcsSystem.Add(sparcsObject.GetComponent<ParticleSystem>());
          SparcsRenderer.Add(sparcsObject.GetComponent<ParticleSystemRenderer>());
        }

        FlareSystem.AddRange(FindChildren(privateArea, "flare").Select(go => go.GetComponent<ParticleSystem>()));
      }

      private IEnumerable<GameObject> FindChildren(PrivateArea privateArea, string name) {
        return privateArea
            .GetComponentsInChildren<Transform>()
            .Where(t => t.name == name && t.gameObject != null)
            .Select(t => t.gameObject);
      }
    }

    private static readonly ConditionalWeakTable<PrivateArea, PrivateAreaData> _privateAreaData =
        new ConditionalWeakTable<PrivateArea, PrivateAreaData>();

    private static ConfigEntry<bool> _isModEnabled;
    private static ConfigEntry<Color> _targetWardColor;
    private static ConfigEntry<string> _targetWardColorHex;

    private static ConfigEntry<bool> _useRadiusForVerticalCheck;
    private static ConfigEntry<bool> _showChangeColorHoverText;

    private static ManualLogSource _logger;
    private Harmony _harmony;

    private void Awake() {
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

      _logger = Logger;
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void OnDestroy() {
      if (_harmony != null) {
        _harmony.UnpatchSelf();
      }
    }

    private void UpdateColorHexValue(object sender, EventArgs eventArgs) {
      _targetWardColorHex.Value = $"{GetColorHtmlString(_targetWardColor.Value)}";
    }

    private void UpdateColorValue(object sender, EventArgs eventArgs) {
      if (ColorUtility.TryParseHtmlString(_targetWardColorHex.Value, out Color color)) {
        _targetWardColor.Value = color;
      }
    }

    private static string GetColorHtmlString(Color color) {
      return color.a == 1.0f
          ? ColorUtility.ToHtmlStringRGB(color)
          : ColorUtility.ToHtmlStringRGBA(color);
    }

    [HarmonyPatch(typeof(PrivateArea))]
    private class PrivateAreaPatch {
      private static readonly int _privateAreaColorHashCode = "PrivateAreaColor".GetStableHashCode();
      private static readonly int _privateAreaColorAlphaHashCode = "PrivateAreaColorAlpha".GetStableHashCode();

      private static readonly KeyboardShortcut _changeColorActionShortcut =
          new KeyboardShortcut(KeyCode.E, KeyCode.LeftShift);

      [HarmonyPrefix]
      [HarmonyPatch(nameof(PrivateArea.IsInside))]
      private static bool PrivateAreaIsInside(
          ref PrivateArea __instance, ref bool __result, Vector3 point, float radius) {
        if (!_isModEnabled.Value || !_useRadiusForVerticalCheck.Value) {
          return true;
        }

        __result = Vector3.Distance(__instance.transform.position, point) < __instance.m_radius + radius;
        return false;
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(PrivateArea.Awake))]
      private static void PrivateAreaAwakePostfix(ref PrivateArea __instance) {
        if (!_isModEnabled.Value || !__instance) {
          return;
        }

        _privateAreaData.Add(__instance, new PrivateAreaData(__instance));
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(PrivateArea.Interact))]
      private static bool PrivateAreaInteractPrefix(
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

        __instance.m_flashEffect.Create(__instance.transform.position, __instance.transform.rotation);

        if (_privateAreaData.TryGetValue(__instance, out PrivateAreaData privateAreaData)) {
          privateAreaData.TargetColor = _targetWardColor.Value;
          SetPrivateAreaColors(__instance, privateAreaData);
        }

        __instance.StartCoroutine(UpdateEnabledEffect(__instance));

        __result = true;
        return false;
      }

      private static IEnumerator UpdateEnabledEffect(PrivateArea privateArea) {
        if (privateArea.IsEnabled()) {
          privateArea.m_enabledEffect.SetActive(false);
          yield return new WaitForEndOfFrame();
          privateArea.m_enabledEffect.SetActive(true);
        }
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(PrivateArea.UpdateStatus))]
      private static void PrivateAreaUpdateStatusPostfix(ref PrivateArea __instance) {
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
      private static void PrivateAreaGetHoverText(ref PrivateArea __instance, ref string __result) {
        if (!_isModEnabled.Value
            || !_showChangeColorHoverText.Value
            || !__instance
            || !__instance.m_piece
            || !__instance.m_piece.IsCreator()) {
          return;
        }

        __result =
            string.Format(
                "{0}\n[<color={1}>{2}</color>] Change color to: <color=#{3}>#{3}</color>",
                __result,
                "#ffa726",
                _changeColorActionShortcut,
                GetColorHtmlString(_targetWardColor.Value));
      }
    }

    private static void SetPrivateAreaColors(PrivateArea privateArea, PrivateAreaData privateAreaData) {
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
