using HarmonyLib;

using static Chatter.Chatter;
using static Chatter.PluginConfig;

namespace Chatter {
  [HarmonyPatch(typeof(Menu))]
  static class MenuPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Menu.Show))]
    static void ShowPostfix() {
      if (IsModEnabled.Value && ChatterChatPanel?.Panel) {
        Chat.m_instance.m_hideTimer = 0f;

        ChatterChatPanel.EnableOrDisableChatPanel(true);
        ChatterChatPanel.ToggleGrabber(true);
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Menu.Hide))]
    static void HidePostfix() {
      if (IsModEnabled.Value && ChatterChatPanel?.Panel) {
        ChatterChatPanel.SetContentVerticalScrollPosition(0f);
        ChatterChatPanel.ToggleGrabber(false);
      }
    }
  }
}
