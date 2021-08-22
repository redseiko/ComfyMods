using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ColorfulPortals {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class ColorfulPortals : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.colorfulportals";
    public const string PluginName = "ColorfulPortals";
    public const string PluginVersion = "1.2.0";

    private static ConfigEntry<bool> _isModEnabled;
    private static ConfigEntry<Color> _targetPortalColor;
    private static ConfigEntry<string> _targetPortalColorHex;

    private static ConfigEntry<bool> _showChangeColorHoverText;

    private static ManualLogSource _logger;
    private Harmony _harmony;

    private void Awake() {
      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _targetPortalColor =
          Config.Bind("Color", "targetPortalColor", Color.cyan, "Target color to set the portal glow effect to.");

      _targetPortalColorHex =
          Config.Bind(
              "Color",
              "targetPortalColorHex",
              $"#{ColorUtility.ToHtmlStringRGB(Color.cyan)}",
              "Target color to set the portal glow effect to, in HTML hex form.");

      _targetPortalColor.SettingChanged += UpdateColorHexValue;
      _targetPortalColorHex.SettingChanged += UpdateColorValue;

      _showChangeColorHoverText =
          Config.Bind(
              "Hud", "showChangeColorHoverText", true, "Show the 'change color' text when hovering over a portal.");

      _logger = Logger;
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

      StartCoroutine(RemovedDestroyedTeleportWorldsCoroutine());
    }

    private void OnDestroy() {
      if (_harmony != null) {
        _harmony.UnpatchSelf();
      }
    }

    private void UpdateColorHexValue(object sender, EventArgs eventArgs) {
      _targetPortalColorHex.Value = $"#{GetColorHtmlString(_targetPortalColor.Value)}";
    }

    private void UpdateColorValue(object sender, EventArgs eventArgs) {
      if (ColorUtility.TryParseHtmlString(_targetPortalColorHex.Value, out Color color)) {
        _targetPortalColor.Value = color;
      }
    }

    private static string GetColorHtmlString(Color color) {
      return color.a == 1.0f
          ? ColorUtility.ToHtmlStringRGB(color)
          : ColorUtility.ToHtmlStringRGBA(color);
    }

    private class TeleportWorldData {
      public List<Light> Lights { get; } = new List<Light>();
      public List<ParticleSystem> Systems { get; } = new List<ParticleSystem>();
      public List<Material> Materials { get; } = new List<Material>();
      public Color TargetColor = Color.clear;

      public TeleportWorldData(TeleportWorld teleportWorld) {
        Lights.AddRange(teleportWorld.GetComponentsInNamedChild<Light>("Point light"));

        Systems.AddRange(teleportWorld.GetComponentsInNamedChild<ParticleSystem>("suck particles"));
        Systems.AddRange(teleportWorld.GetComponentsInNamedChild<ParticleSystem>("Particle System"));

        Materials.AddRange(
            teleportWorld.GetComponentsInNamedChild<ParticleSystemRenderer>("blue flames")
                .Where(psr => psr.material != null)
                .Select(psr => psr.material));
      }
    }

    private static readonly Dictionary<TeleportWorld, TeleportWorldData> _teleportWorldDataCache = new();

    private static IEnumerator RemovedDestroyedTeleportWorldsCoroutine() {
      WaitForSeconds waitThirtySeconds = new(seconds: 30f);
      List<KeyValuePair<TeleportWorld, TeleportWorldData>> existingPortals = new();
      int portalCount = 0;

      while (true) {
        yield return waitThirtySeconds;
        portalCount = _teleportWorldDataCache.Count;

        existingPortals.AddRange(_teleportWorldDataCache.Where(entry => entry.Key));
        _teleportWorldDataCache.Clear();

        foreach (KeyValuePair<TeleportWorld, TeleportWorldData> entry in existingPortals) {
          _teleportWorldDataCache[entry.Key] = entry.Value;
        }

        existingPortals.Clear();
        _logger.LogInfo($"Removed {portalCount - _teleportWorldDataCache.Count}/{portalCount} portal references.");
      }
    }

    private static bool TryGetTeleportWorld(TeleportWorld key, out TeleportWorldData value) {
      if (key) {
        return _teleportWorldDataCache.TryGetValue(key, out value);
      }

      value = default;
      return false;
    }

    [HarmonyPatch(typeof(TeleportWorld))]
    private class TeleportWorldPatch {
      private static readonly int _teleportWorldColorHashCode = "TeleportWorldColor".GetStableHashCode();
      private static readonly int _teleportWorldColorAlphaHashCode = "TeleportWorldColorAlpha".GetStableHashCode();

      private static readonly KeyboardShortcut _changeColorActionShortcut = new(KeyCode.E, KeyCode.LeftShift);

      [HarmonyPostfix]
      [HarmonyPatch(nameof(TeleportWorld.Awake))]
      private static void TeleportWorldAwakePostfix(ref TeleportWorld __instance) {
        if (!_isModEnabled.Value || !__instance) {
          return;
        }

        // Stone 'portal' prefab does not set this property.
        if (!__instance.m_proximityRoot) {
          __instance.m_proximityRoot = __instance.transform;
        }

        // Stone 'portal' prefab does not set this property.
        if (!__instance.m_target_found) {
          // The prefab does not have '_target_found_red' but instead '_target_found'.
          GameObject targetFoundObject = __instance.gameObject.transform.Find("_target_found").gameObject;

          // Disable the GameObject first, as adding component EffectFade calls its Awake() before being attached.
          targetFoundObject.SetActive(false);
          __instance.m_target_found = targetFoundObject.AddComponent<EffectFade>();
          targetFoundObject.SetActive(true);
        }

        _teleportWorldDataCache.Add(__instance, new TeleportWorldData(__instance));
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(TeleportWorld.GetHoverText))]
      private static void TeleportWorldGetHoverTextPostfix(ref TeleportWorld __instance, ref string __result) {
        if (!_isModEnabled.Value || !_showChangeColorHoverText.Value || !__instance) {
          return;
        }

        __result =
            string.Format(
                "{0}\n[<color={1}>{2}</color>] Change color to: <color={3}>{3}</color>",
                __result,
                "#FFA726",
                _changeColorActionShortcut,
                _targetPortalColorHex.Value);
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(TeleportWorld.Interact))]
      private static bool TeleportWorldInteractPrefix(
          ref TeleportWorld __instance, ref bool __result, Humanoid human, bool hold) {
        if (!_isModEnabled.Value || hold || !__instance.m_nview || !_changeColorActionShortcut.IsDown()) {
          return true;
        }

        if (!__instance.m_nview || !__instance.m_nview.IsValid()) {
          _logger.LogWarning("TeleportWorld does not have a valid ZNetView.");

          __result = true;
          return false;
        }

        if (!PrivateArea.CheckAccess(__instance.transform.position, flash: true)) {
          _logger.LogWarning("TeleportWorld is within a PrivateArea.");
          __result = true;
          return false;
        }

        if (!__instance.m_nview.IsOwner()) {
          __instance.m_nview.ClaimOwnership();
        }

        __instance.m_nview.m_zdo.Set(_teleportWorldColorHashCode, Utils.ColorToVec3(_targetPortalColor.Value));
        __instance.m_nview.m_zdo.Set(_teleportWorldColorAlphaHashCode, _targetPortalColor.Value.a);

        if (_teleportWorldDataCache.TryGetValue(__instance, out TeleportWorldData teleportWorldData)) {
          teleportWorldData.TargetColor = _targetPortalColor.Value;
          SetTeleportWorldColors(teleportWorldData);
        }

        __result = true;
        return false;
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(TeleportWorld.UpdatePortal))]
      private static void TeleportWorldUpdatePortalPostfix(ref TeleportWorld __instance) {
        if (!_isModEnabled.Value
            || !__instance
            || !__instance.m_nview
            || __instance.m_nview.m_zdo == null
            || __instance.m_nview.m_zdo.m_zdoMan == null
            || __instance.m_nview.m_zdo.m_vec3 == null
            || !__instance.m_nview.m_zdo.m_vec3.ContainsKey(_teleportWorldColorHashCode)
            || !_teleportWorldDataCache.TryGetValue(__instance, out TeleportWorldData teleportWorldData)) {
          return;
        }

        Color portalColor = Utils.Vec3ToColor(__instance.m_nview.m_zdo.m_vec3[_teleportWorldColorHashCode]);
        portalColor.a = __instance.m_nview.m_zdo.GetFloat(_teleportWorldColorAlphaHashCode, defaultValue: 1f);

        teleportWorldData.TargetColor = portalColor;
        SetTeleportWorldColors(teleportWorldData);
      }

      private static void SetTeleportWorldColors(TeleportWorldData teleportWorldData) {
        foreach (Light light in teleportWorldData.Lights) {
          light.color = teleportWorldData.TargetColor;
        }

        foreach (ParticleSystem system in teleportWorldData.Systems) {
          ParticleSystem.ColorOverLifetimeModule colorOverLifetime = system.colorOverLifetime;
          colorOverLifetime.color = new ParticleSystem.MinMaxGradient(teleportWorldData.TargetColor);

          ParticleSystem.MainModule main = system.main;
          main.startColor = teleportWorldData.TargetColor;
        }

        foreach (Material material in teleportWorldData.Materials) {
          material.color = teleportWorldData.TargetColor;
        }
      }
    }
  }

  internal static class TeleportWorldExtension {
    public static IEnumerable<T> GetComponentsInNamedChild<T>(this TeleportWorld teleportWorld, string childName) {
      return teleportWorld.GetComponentsInChildren<Transform>(includeInactive: true)
          .Where(transform => transform.name == childName)
          .Select(transform => transform.GetComponent<T>())
          .Where(component => component != null);
      }
  }
}