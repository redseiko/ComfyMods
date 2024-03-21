namespace Chatter;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Menu))]
static class MenuPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Menu.Show))]
  static void ShowPostfix() {
    if (IsModEnabled.Value && ChatPanelController.ChatPanel?.Panel) {
      Chat.m_instance.m_hideTimer = 0f;
      ChatPanelController.
            ChatPanel.EnableOrDisableChatPanel(true);
      ChatPanelController.ChatPanel.ToggleGrabber(true);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Menu.Hide))]
  static void HidePostfix() {
    if (IsModEnabled.Value && ChatPanelController.ChatPanel?.Panel) {
      ChatPanelController.ChatPanel.SetContentVerticalScrollPosition(0f);
      ChatPanelController.ChatPanel.ToggleGrabber(false);
    }
  }
}
