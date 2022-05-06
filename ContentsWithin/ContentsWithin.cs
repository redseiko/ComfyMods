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

    private static bool isRealGuiVisible;
    private static bool showContent = true;

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

    private void Update() {
      if (!_isModEnabled.Value) {
        return;
      }

      if (_toggleShowContentsShortcut.Value.IsDown()) {
        showContent = !showContent;

        if (MessageHud.instance) {
          MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"ShowContainerContents: {showContent}");
        }

        if (!showContent && !isRealGuiVisible && InventoryGui.instance) {
          InventoryGui.instance.Hide();
        }
      }
    }

    private static bool ShowRealGUI() {
      return !_isModEnabled.Value || !showContent || isRealGuiVisible;
    }

    [HarmonyPatch(typeof(Player))]
    public class PlayerPatch {
      [HarmonyPatch(nameof(Player.UpdateHover)), HarmonyPostfix]
      public static void UpdateHoverPostfix(Player __instance) {
        if (!_isModEnabled.Value || _lastHoverObject == __instance.m_hovering) {
          return;
        }

        _lastHoverObject = __instance.m_hovering;
        _lastHoverContainer = _lastHoverObject ? _lastHoverObject.GetComponentInParent<Container>() : null;
      }
    }

    [HarmonyPatch(typeof(GuiBar))]
    public class GuiBarPatch {
      [HarmonyPatch(nameof(GuiBar.SetValue)), HarmonyPrefix]
      public static void GuiBarSetValuePrefix(GuiBar __instance) {
        if (__instance.m_firstSet) {
          __instance.m_width = __instance.m_bar.sizeDelta.x;
        }
      }
    }

    [HarmonyPatch(typeof(InventoryGui))]
    public class InventoryGuiPatch {
      [HarmonyPatch(nameof(InventoryGui.Awake)), HarmonyPostfix]
      public static void AwakePostfix(ref InventoryGui __instance) {
        _inventoryPanel = __instance.m_player.Ref()?.gameObject;
        _containerPanel = __instance.m_container.Ref()?.gameObject;
        _infoPanel = __instance.m_infoPanel.Ref()?.gameObject;
        _craftingPanel = __instance.m_inventoryRoot.Find("Crafting").Ref()?.gameObject;
        _takeAllButton = __instance.m_takeAllButton.Ref()?.gameObject;
      }

      [HarmonyPatch(nameof(InventoryGui.Show)), HarmonyPostfix]
      public static void ShowPostfix() {
        isRealGuiVisible = true;
      }

      [HarmonyPatch(nameof(InventoryGui.Hide)), HarmonyPostfix]
      public static void HidePostfix() {
        isRealGuiVisible = false;
      }

      [HarmonyPatch(nameof(InventoryGui.Update)), HarmonyPrefix]
      public static void UpdatePrefix(InventoryGui __instance) {
        if (!ShowRealGUI()) {
          __instance.m_animator.SetBool("visible", false);
        }
      }

      [HarmonyPatch(nameof(InventoryGui.Update)), HarmonyPostfix]
      public static void UpdatePostfix(InventoryGui __instance) {
        _inventoryPanel.Ref()?.SetActive(ShowRealGUI());
        _craftingPanel.Ref()?.SetActive(ShowRealGUI());
        _infoPanel.Ref()?.SetActive(ShowRealGUI());
        _takeAllButton.Ref()?.SetActive(ShowRealGUI());

        if (ShowRealGUI()) {
          return;
        }

        if (HasContainerAccess(_lastHoverContainer)) {
          ShowPreviewContainer();
        } else {
          InventoryGui.instance.m_animator.SetBool("visible", false);
        }
      }

      private static bool HasContainerAccess(Container container) {
        if (!container) {
          return false;
        }

        bool areaAccess = PrivateArea.CheckAccess(container.transform.position, 0f, false, false);
        bool chestAccess = container.CheckAccess(Game.m_instance.m_playerProfile.m_playerID);

        return areaAccess && chestAccess;
      }

      [HarmonyPatch(nameof(InventoryGui.SetupDragItem)), HarmonyPrefix]
      public static bool SetupDragItemPrefix() {
        return ShowRealGUI();
      }

      private static void ShowPreviewContainer() {
        InventoryGui.instance.m_animator.SetBool("visible", true);
        InventoryGui.instance.m_hiddenFrames = 10;
        InventoryGui.instance.m_container.gameObject.SetActive(true);
        InventoryGui.instance.m_containerGrid.UpdateInventory(_lastHoverContainer.GetInventory(), null, null);
        InventoryGui.instance.m_containerGrid.ResetView();
        InventoryGui.instance.m_containerName.text = Localization.instance.Localize(_lastHoverContainer.GetInventory().GetName());
        int containerWeight = Mathf.CeilToInt(_lastHoverContainer.GetInventory().GetTotalWeight());
        InventoryGui.instance.m_containerWeight.text = containerWeight.ToString();
      }
    }
  }

  public static class ObjectExtensions {
    public static T Ref<T>(this T o) where T : UnityEngine.Object {
      return o ? o : null;
    }
  }
}
