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
    public const string PluginVersion = "1.4.0";

    static ConfigEntry<bool> _isModEnabled;
    static ConfigEntry<Color> _targetPortalColor;
    static ConfigEntry<string> _targetPortalColorHex;

    static ConfigEntry<bool> _showChangeColorHoverText;
    static ConfigEntry<int> _colorPromptFontSize;

    static ManualLogSource _logger;
    Harmony _harmony;

    public void Awake() {
      _logger = Logger;

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

      _colorPromptFontSize =
          Config.Bind("Hud", "colorPromptFontSize", 15, "Font size for the 'change color' text prompt.");

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

      StartCoroutine(RemovedDestroyedTeleportWorldsCoroutine());
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    void UpdateColorHexValue(object sender, EventArgs eventArgs) {
      _targetPortalColorHex.Value = $"#{GetColorHtmlString(_targetPortalColor.Value)}";
    }

    void UpdateColorValue(object sender, EventArgs eventArgs) {
      if (ColorUtility.TryParseHtmlString(_targetPortalColorHex.Value, out Color color)) {
        _targetPortalColor.Value = color;
      }
    }

    static string GetColorHtmlString(Color color) {
      return color.a == 1.0f
          ? ColorUtility.ToHtmlStringRGB(color)
          : ColorUtility.ToHtmlStringRGBA(color);
    }

    class TeleportWorldData {
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

    static readonly Dictionary<TeleportWorld, TeleportWorldData> _teleportWorldDataCache = new();

    static IEnumerator RemovedDestroyedTeleportWorldsCoroutine() {
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

        if (portalCount > 0) {
          _logger.LogInfo($"Removed {portalCount - _teleportWorldDataCache.Count}/{portalCount} portal references.");
        }
      }
    }

    static bool TryGetTeleportWorld(TeleportWorld key, out TeleportWorldData value) {
      if (key) {
        return _teleportWorldDataCache.TryGetValue(key, out value);
      }

      value = default;
      return false;
    }

    [HarmonyPatch(typeof(TeleportWorld))]
    class TeleportWorldPatch {
      static readonly int _teleportWorldColorHashCode = "TeleportWorldColor".GetStableHashCode();
      static readonly int _teleportWorldColorAlphaHashCode = "TeleportWorldColorAlpha".GetStableHashCode();
      static readonly int _portalLastColoredByHashCode = "PortalLastColoredBy".GetStableHashCode();

      static readonly KeyboardShortcut _changeColorActionShortcut = new(KeyCode.E, KeyCode.LeftShift);

      [HarmonyPostfix]
      [HarmonyPatch(nameof(TeleportWorld.Awake))]
      static void TeleportWorldAwakePostfix(ref TeleportWorld __instance) {
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
      static void TeleportWorldGetHoverTextPostfix(ref TeleportWorld __instance, ref string __result) {
        if (!_isModEnabled.Value || !_showChangeColorHoverText.Value || !__instance) {
          return;
        }

        __result =
            string.Format(
                "{0}\n<size={4}>[<color={1}>{2}</color>] Change color to: <color={3}>{3}</color></size>",
                __result,
                "#FFA726",
                _changeColorActionShortcut,
                _targetPortalColorHex.Value,
                _colorPromptFontSize.Value);
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(TeleportWorld.Interact))]
      static bool TeleportWorldInteractPrefix(
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
        __instance.m_nview.m_zdo.Set(_portalLastColoredByHashCode, Player.m_localPlayer?.GetPlayerID() ?? 0L);

        if (_teleportWorldDataCache.TryGetValue(__instance, out TeleportWorldData teleportWorldData)) {
          teleportWorldData.TargetColor = _targetPortalColor.Value;
          SetTeleportWorldColors(teleportWorldData);
        }

        __result = true;
        return false;
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(TeleportWorld.UpdatePortal))]
      static void TeleportWorldUpdatePortalPostfix(ref TeleportWorld __instance) {
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

      static void SetTeleportWorldColors(TeleportWorldData teleportWorldData) {
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