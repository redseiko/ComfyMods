using BepInEx;
using BepInEx.Configuration;

using HarmonyLib;
using System.Collections;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace CarbonCopy {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class CarbonCopy : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.carboncopy";
    public const string PluginName = "CarbonCopy";
    public const string PluginVersion = "1.0.0";

    static ConfigEntry<bool> _isModEnabled;
    static ConfigEntry<KeyboardShortcut> _toggleBuildPanel;
    static ConfigEntry<Color> _radiusSphereColor;

    Harmony _harmony;

    public void Awake() {
      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _toggleBuildPanel =
          Config.Bind(
              "Build",
              "toggleBuildPanel",
              new KeyboardShortcut(KeyCode.F6),
              "Keyboard shortcut to toggle the Build Panel.");

      _radiusSphereColor = Config.Bind("Save", "radiusSphereColor", Color.cyan, "The color of the radius sphere.");

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    [HarmonyPatch(typeof(Terminal))]
    class TerminalPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(Terminal.InputText))]
      static bool InputTextPrefix(ref Terminal __instance) {
        if (_isModEnabled.Value && ParseText(__instance.m_input.text)) {
          return false;
        }

        return true;
      }
    }

    [HarmonyPatch(typeof(Chat))]
    class ChatPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(Chat.InputText))]
      static bool InputTextPrefix(ref Chat __instance) {
        if (_isModEnabled.Value && ParseText(__instance.m_input.text)) {
          return false;
        }

        return true;
      }
    }

    [HarmonyPatch(typeof(Hud))]
    class HudPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Hud.Update))]
      static void UpdatePostfix(ref Hud __instance) {
        if (_isModEnabled.Value && _toggleBuildPanel.Value.IsDown()) {
          ToggleBuildPanel(__instance);
        }
      }
    }

    [HarmonyPatch(typeof(Menu))]
    class MenuPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Menu.IsVisible))]
      static void IsVisiblePostfix(ref bool __result) {
        if (_isModEnabled.Value && _buildPanel != null && _buildPanel.Panel.activeSelf) {
          __result = true;
        }
      }
    }

    static bool ParseText(string text) {
      if (text == "/ccpanel") {
        ToggleBuildPanel(Hud.instance);
        return true;
      }

      return false;
    }

    static BuildPanel _buildPanel = null;

    static bool _usePinnedOrigin = false;
    static Vector3 _pinnedOrigin = Vector3.zero;

    static GameObject _radiusSphere;

    static void ToggleBuildPanel(Hud hud) {
      _buildPanel ??= CreateBuildPanel(hud);
      _buildPanel.Panel.SetActive(!_buildPanel.Panel.activeSelf);
    }

    static BuildPanel CreateBuildPanel(Hud hud) {
      BuildPanel buildPanel = new(hud.transform);
      buildPanel.Panel.SetActive(false);

      buildPanel.PinOriginButton.onClick.AddListener(() => {
        _usePinnedOrigin = !_usePinnedOrigin;
        _pinnedOrigin = Player.m_localPlayer?.transform.position ?? Vector3.zero;

        buildPanel.PinOriginButton.SetLabel(_usePinnedOrigin ? "Unpin" : "Pin");
        hud.StartCoroutine(ToggleRadiusSphere(buildPanel));
      });

      buildPanel.RadiusSlider.onValueChanged.AddListener(
          value => {
            if (_radiusSphere) {
              _radiusSphere.transform.localScale = new(value, value, value);
            }
          });

      buildPanel.ShowRadiusSphereToggle.onValueChanged.AddListener(
          value => hud.StartCoroutine(ToggleRadiusSphere(buildPanel)));

      hud.StartCoroutine(UpdateBuildPanelCoroutine(buildPanel));

      return buildPanel;
    }

    static IEnumerator UpdateBuildPanelCoroutine(BuildPanel buildPanel) {
      WaitForSeconds updateInterval = new(seconds: 0.25f);

      while (true) {
        yield return updateInterval;

        if (buildPanel.Panel.activeSelf) {
          buildPanel.OriginInputField.text =
              (_usePinnedOrigin ? _pinnedOrigin : Player.m_localPlayer?.transform.position ?? Vector3.zero).ToString();
        }
      }
    }

    static IEnumerator ToggleRadiusSphere(BuildPanel buildPanel) {
      Destroy(_radiusSphere);
      _radiusSphere = null;

      if (!buildPanel.ShowRadiusSphereToggle.isOn) {
        yield break;
      }

      if (!float.TryParse(buildPanel.RadiusInputField.text, out float radius)) {
        radius = buildPanel.RadiusSlider.value;
      }

      _radiusSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

      if (_usePinnedOrigin) {
        _radiusSphere.transform.position = _pinnedOrigin;
      } else {
        _radiusSphere.transform.parent = Player.m_localPlayer.transform;
        _radiusSphere.transform.localPosition = Vector3.zero;
      }

      _radiusSphere.transform.localScale = new Vector3(radius, radius, radius);

      MeshRenderer renderer = _radiusSphere.GetComponent<MeshRenderer>();

      renderer.material =
          Resources.FindObjectsOfTypeAll<Material>()
              .Where(material => material.name.StartsWith("ForceField"))
              .FirstOrDefault();

      renderer.material.SetColor("_Color", _radiusSphereColor.Value);
      renderer.material.shader = Shader.Find("Custom/Distortion");

      Destroy(_radiusSphere.GetComponentInChildren<Collider>(includeInactive: false));
    }
  }
}