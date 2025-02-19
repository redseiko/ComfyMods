namespace Chatter;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Menu))]
static class MenuPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Menu.Show))]
  static void ShowPostfix() {
    if (IsModEnabled.Value && ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
      Chat.m_instance.m_hideTimer = 0f;
      chatPanel.EnableOrDisableChatPanel(true);
      chatPanel.ToggleGrabber(true);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Menu.Hide))]
  static void HidePostfix() {
    if (IsModEnabled.Value && ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
      chatPanel.SetContentVerticalScrollPosition(0f);
      chatPanel.ToggleGrabber(false);
    }
  }
}
