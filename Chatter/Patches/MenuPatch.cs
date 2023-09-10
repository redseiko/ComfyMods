using HarmonyLib;

using static Chatter.PluginConfig;

namespace Chatter {
  [HarmonyPatch(typeof(Menu))]
  static class MenuPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Menu.Show))]
    static void ShowPostfix() {
      if (IsModEnabled.Value) {
        Chat.m_instance.m_hideTimer = 0f;
        //ChatPanel?.ToggleGrabber(true);
        //ChatPanel?.SetPanelSize(ChatPanelSize.Value);
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Menu.Hide))]
    static void HidePostfix() {
      if (IsModEnabled.Value) {
        Chatter.ChatterChatPanel?.SetContentVerticalScrollPosition(0f);
        //ChatPanel?.ToggleGrabber(false);
        //ChatPanel?.SetPanelSize(ChatPanelSize.Value);
      }
    }
  }
}
