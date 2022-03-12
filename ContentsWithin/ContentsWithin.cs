using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ContentsWithin {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class ContentsWithin : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.contentswithin";
    public const string PluginName = "ContentsWithin";
    public const string PluginVersion = "1.0.1";

    static ConfigEntry<bool> _isModEnabled;
    static ConfigEntry<KeyboardShortcut> _toggleShowContentsShortcut;

    static bool isVisible;

    static Container _lastHoverContainer = null;
    static GameObject _lastHoverObject = null;

    static GameObject _inventoryPanel;
    static GameObject _containerPanel;
    static GameObject _infoPanel;
    static GameObject _craftingPanel;
    static GameObject _takeAllButton;

    Harmony _harmony;

    public void Awake() {
      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _toggleShowContentsShortcut =
          Config.Bind(
              "Hotkeys",
              "toggleShowContentsShortcut",
              new KeyboardShortcut(KeyCode.P, KeyCode.RightShift),
              "Shortcut to toggle on/off the 'show container contents' feature.");

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    [HarmonyPatch(typeof(Player))]
    class PlayerPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.UpdateHover))]
      public static void UpdateHoverPostfix(Player __instance) {
        if (!_isModEnabled.Value || _lastHoverObject == __instance.m_hovering) {
          return;
        }

        _lastHoverObject = __instance.m_hovering;

        if (_lastHoverObject) {
          _lastHoverContainer = _lastHoverObject.GetComponentInParent<Container>();
        } else {
          _lastHoverContainer = null;
        }
      }
    }

    [HarmonyPatch(typeof(InventoryGui))]
    class InventoryGuiPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(InventoryGui.Awake))]
      static void AwakePostfix(ref InventoryGui __instance) {
        _inventoryPanel = __instance.m_player.Ref()?.gameObject;
        _containerPanel = __instance.m_container.Ref()?.gameObject;
        _infoPanel = __instance.m_infoPanel.Ref()?.gameObject;
        _craftingPanel = __instance.m_inventoryRoot.Find("Crafting").Ref()?.gameObject;
        _takeAllButton = __instance.m_takeAllButton.Ref()?.gameObject;
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(InventoryGui.Show))]
      public static void ShowPostfix(InventoryGui __instance, ref Container container) {
        isVisible = true;

        if (_isModEnabled.Value && !container && _lastHoverContainer) {
          container = _lastHoverContainer;
        }
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(InventoryGui.Hide))]
      public static void HidePrefix(InventoryGui __instance) {
        isVisible = false;
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(InventoryGui.Update))]
      public static void UpdatePrefix(InventoryGui __instance) {
        if (_isModEnabled.Value && !isVisible) {
          __instance.m_animator.SetBool("visible", false);
        }
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(InventoryGui.Update))]
      public static void UpdatePostfix(InventoryGui __instance) {
        _inventoryPanel.Ref()?.SetActive(!_isModEnabled.Value || isVisible);
        _craftingPanel.Ref()?.SetActive(!_isModEnabled.Value || isVisible);
        _infoPanel.Ref()?.SetActive(!_isModEnabled.Value || isVisible);
        _takeAllButton.Ref()?.SetActive(!_isModEnabled.Value || isVisible);

        if (!_isModEnabled.Value) {
          return;
        }

        if (!isVisible) {
          if (_lastHoverContainer) {
            InventoryGui.instance.m_animator.SetBool("visible", true);
            InventoryGui.instance.m_container.gameObject.SetActive(true);
            InventoryGui.instance.m_containerGrid.UpdateInventory(_lastHoverContainer.GetInventory(), null, null);
            InventoryGui.instance.m_hiddenFrames = 10;
          } else {
            InventoryGui.instance.m_container.gameObject.SetActive(false);
          }
        }
      }
    }
  }

  public static class ObjectExtensions {
    public static T Ref<T>(this T o) where T : UnityEngine.Object {
      return o ? o : null;
    }
  }
}
