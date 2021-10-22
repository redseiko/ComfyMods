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

namespace ColorfulLights {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class ColorfulLights : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.colorfullights";
    public const string PluginName = "ColorfulLights";
    public const string PluginVersion = "1.5.0";

    static readonly Dictionary<Fireplace, FireplaceData> _fireplaceDataCache = new();

    static ConfigEntry<bool> _isModEnabled;
    static ConfigEntry<Color> _targetFireplaceColor;
    static ConfigEntry<string> _targetFireplaceColorHex;

    static ConfigEntry<bool> _showChangeColorHoverText;
    static ConfigEntry<int> _colorPromptFontSize;

    static ManualLogSource _logger;
    Harmony _harmony;

    public void Awake() {
      _logger = Logger;

      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _targetFireplaceColor =
          Config.Bind("Color", "targetFireplaceColor", Color.cyan, "Target color to set any torch/fire to.");

      _targetFireplaceColorHex =
          Config.Bind(
              "Color",
              "targetFireplaceColorHex",
              $"#{ColorUtility.ToHtmlStringRGB(Color.cyan)}",
              "Target color to set torch/fire to, in HTML hex-form.");

      _targetFireplaceColor.SettingChanged += UpdateColorHexValue;
      _targetFireplaceColorHex.SettingChanged += UpdateColorValue;

      _showChangeColorHoverText =
          Config.Bind(
              "Hud",
              "showChangeColorHoverText",
              true,
              "Show the 'change color' text when hovering over a lightsoure.");

      _colorPromptFontSize =
          Config.Bind("Hud", "colorPromptFontSize", 15, "Font size for the 'change color' text prompt.");

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

      StartCoroutine(RemoveDestroyedFireplacesCoroutine());
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    void UpdateColorHexValue(object sender, EventArgs eventArgs) {
      _targetFireplaceColorHex.Value = $"{GetColorHtmlString(_targetFireplaceColor.Value)}";
    }

    void UpdateColorValue(object sender, EventArgs eventArgs) {
      if (ColorUtility.TryParseHtmlString(_targetFireplaceColorHex.Value, out Color color)) {
        _targetFireplaceColor.Value = color;
      }
    }

    static string GetColorHtmlString(Color color) {
      return color.a == 1.0f
          ? ColorUtility.ToHtmlStringRGB(color)
          : ColorUtility.ToHtmlStringRGBA(color);
    }

    static IEnumerator RemoveDestroyedFireplacesCoroutine() {
      WaitForSeconds waitThirtySeconds = new(seconds: 30f);
      List<KeyValuePair<Fireplace, FireplaceData>> existingFireplaces = new();
      int fireplaceCount = 0;

      while (true) {
        yield return waitThirtySeconds;
        fireplaceCount = _fireplaceDataCache.Count;

        existingFireplaces.AddRange(_fireplaceDataCache.Where(entry => entry.Key));
        _fireplaceDataCache.Clear();

        foreach (KeyValuePair<Fireplace, FireplaceData> entry in existingFireplaces) {
          _fireplaceDataCache[entry.Key] = entry.Value;
        }

        existingFireplaces.Clear();

        if (fireplaceCount > 0) {
          _logger.LogInfo($"Removed {fireplaceCount - _fireplaceDataCache.Count}/{fireplaceCount} fireplace refs.");
        }
      }
    }

    static bool TryGetFireplace(Fireplace key, out FireplaceData value) {
      if (key) {
        return _fireplaceDataCache.TryGetValue(key, out value);
      }

      value = default;
      return false;
    }

    [HarmonyPatch(typeof(Fireplace))]
    class FireplacePatch {
      static readonly int _fuelHashCode = "fuel".GetStableHashCode();
      static readonly int _fireplaceColorHashCode = "FireplaceColor".GetStableHashCode();
      static readonly int _fireplaceColorAlphaHashCode = "FireplaceColorAlpha".GetStableHashCode();
      static readonly int _lightLastColoredByHashCode = "LightLastColoredBy".GetStableHashCode();

      static readonly KeyboardShortcut _changeColorActionShortcut = new(KeyCode.E, KeyCode.LeftShift);

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Fireplace.Awake))]
      static void FireplaceAwakePostfix(ref Fireplace __instance) {
        if (!_isModEnabled.Value || !__instance) {
          return;
        }

        _fireplaceDataCache.Add(__instance, new(__instance));
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Fireplace.GetHoverText))]
      static void FireplaceGetHoverTextPostfix(ref Fireplace __instance, ref string __result) {
        if (!_isModEnabled.Value || !_showChangeColorHoverText.Value || !__instance) {
          return;
        }

        __result = Localization.instance.Localize(
            string.Format(
                "{0}\n<size={4}>[<color={1}>{2}</color>] Change fire color to: <color=#{3}>#{3}</color></size>",
                __result,
                "#FFA726",
                _changeColorActionShortcut,
                GetColorHtmlString(_targetFireplaceColor.Value),
                _colorPromptFontSize.Value));
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(Fireplace.Interact))]
      static bool FireplaceInteractPrefix(ref Fireplace __instance, ref bool __result, bool hold) {
        if (!_isModEnabled.Value || hold || !_changeColorActionShortcut.IsDown()) {
          return true;
        }

        if (!__instance.m_nview && !__instance.m_nview.IsValid()) {
          _logger.LogWarning("Fireplace does not have a valid ZNetView.");

          __result = false;
          return false;
        }

        if (!PrivateArea.CheckAccess(__instance.transform.position, radius: 0f, flash: true, wardCheck: false)) {
          _logger.LogWarning("Fireplace is within a private area.");

          __result = false;
          return false;
        }

        if (!__instance.m_nview.IsOwner()) {
          __instance.m_nview.ClaimOwnership();
        }

        __instance.m_nview.m_zdo.Set(_fireplaceColorHashCode, Utils.ColorToVec3(_targetFireplaceColor.Value));
        __instance.m_nview.m_zdo.Set(_fireplaceColorAlphaHashCode, _targetFireplaceColor.Value.a);
        __instance.m_nview.m_zdo.Set(_lightLastColoredByHashCode, Player.m_localPlayer.GetPlayerID());

        __instance.m_fuelAddedEffects.Create(__instance.transform.position, __instance.transform.rotation);

        if (TryGetFireplace(__instance, out FireplaceData fireplaceData)) {
          SetParticleColors(
              fireplaceData.Lights, fireplaceData.Systems, fireplaceData.Renderers, _targetFireplaceColor.Value);

          fireplaceData.TargetColor = _targetFireplaceColor.Value;
        }

        __result = true;
        return false;
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(Fireplace.UseItem))]
      static bool FireplaceUseItemPrefix(
          ref Fireplace __instance, ref bool __result, Humanoid user, ItemDrop.ItemData item) {
        if (!_isModEnabled.Value
            || !__instance.m_fireworks
            || item.m_shared.m_name != __instance.m_fireworkItem.m_itemData.m_shared.m_name
            || !TryGetFireplace(__instance, out FireplaceData fireplaceData)) {
          return true;
        }

        if (!__instance.IsBurning()) {
          user.Message(MessageHud.MessageType.Center, "$msg_firenotburning");

          __result = true;
          return false;
        }

        if (user.GetInventory().CountItems(item.m_shared.m_name) < __instance.m_fireworkItems) {
          user.Message(MessageHud.MessageType.Center, $"$msg_toofew {item.m_shared.m_name}");

          __result = true;
          return false;
        }

        user.GetInventory().RemoveItem(item.m_shared.m_name, __instance.m_fireworkItems);
        user.Message(
            MessageHud.MessageType.Center, Localization.instance.Localize("$msg_throwinfire", item.m_shared.m_name));

        SetParticleColors(
            fireplaceData.Lights, fireplaceData.Systems, fireplaceData.Renderers, fireplaceData.TargetColor);

        Quaternion colorToRotation = __instance.transform.rotation;
        colorToRotation.x = fireplaceData.TargetColor.r;
        colorToRotation.y = fireplaceData.TargetColor.g;
        colorToRotation.z = fireplaceData.TargetColor.b;

        _logger.LogInfo($"Sending fireworks to spawn with color: {fireplaceData.TargetColor}");
        ZNetScene.instance.SpawnObject(__instance.transform.position, colorToRotation, __instance.m_fireworks);

        __instance.m_fuelAddedEffects.Create(__instance.transform.position, __instance.transform.rotation);

        __result = true;
        return false;
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Fireplace.UpdateFireplace))]
      static void FireplaceUpdateFireplacePostfix(ref Fireplace __instance) {
        if (!_isModEnabled.Value
            || !__instance.m_nview
            || __instance.m_nview.m_zdo == null
            || __instance.m_nview.m_zdo.m_zdoMan == null
            || __instance.m_nview.m_zdo.m_vec3 == null
            || !__instance.m_nview.m_zdo.m_vec3.ContainsKey(_fireplaceColorHashCode)
            || !TryGetFireplace(__instance, out FireplaceData fireplaceData)) {
          return;
        }

        Color fireplaceColor = Utils.Vec3ToColor(__instance.m_nview.m_zdo.m_vec3[_fireplaceColorHashCode]);
        fireplaceColor.a = __instance.m_nview.m_zdo.GetFloat(_fireplaceColorAlphaHashCode, 1f);

        SetParticleColors(fireplaceData.Lights, fireplaceData.Systems, fireplaceData.Renderers, fireplaceColor);
        fireplaceData.TargetColor = fireplaceColor;
      }
    }

    [HarmonyPatch(typeof(ZNetScene))]
    class ZNetScenePatch {
      static readonly int _vfxFireWorkTestHashCode = "vfx_FireWorkTest".GetStableHashCode();

      [HarmonyPrefix]
      [HarmonyPatch(nameof(ZNetScene.RPC_SpawnObject))]
      static bool ZNetSceneRPC_SpawnObjectPrefix(
          ref ZNetScene __instance, Vector3 pos, Quaternion rot, int prefabHash) {
        if (!_isModEnabled.Value || prefabHash != _vfxFireWorkTestHashCode || rot == Quaternion.identity) {
          return true;
        }

        Color fireworksColor = Utils.Vec3ToColor(new Vector3(rot.x, rot.y, rot.z));

        _logger.LogInfo($"Spawning fireworks with color: {fireworksColor}");
        GameObject fireworksClone = Instantiate(__instance.GetPrefab(prefabHash), pos, rot);

        SetParticleColors(
            Enumerable.Empty<Light>(),
            fireworksClone.GetComponentsInChildren<ParticleSystem>(includeInactive: true),
            fireworksClone.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true),
            fireworksColor);

        return false;
      }
    }

    static void SetParticleColors(
        IEnumerable<Light> lights,
        IEnumerable<ParticleSystem> systems,
        IEnumerable<ParticleSystemRenderer> renderers,
        Color targetColor) {
      var targetColorGradient = new ParticleSystem.MinMaxGradient(targetColor);

      foreach (ParticleSystem system in systems) {
        var colorOverLifetime = system.colorOverLifetime;

        if (colorOverLifetime.enabled) {
          colorOverLifetime.color = targetColorGradient;
        }

        var sizeOverLifetime = system.sizeOverLifetime;

        if (sizeOverLifetime.enabled) {
          var main = system.main;
          main.startColor = targetColor;
        }
      }

      foreach (ParticleSystemRenderer renderer in renderers) {
        renderer.material.color = targetColor;
      }

      foreach (Light light in lights) {
        light.color = targetColor;
      }
    }
  }
}
