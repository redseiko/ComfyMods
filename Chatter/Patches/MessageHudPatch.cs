namespace Chatter;

using System;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(MessageHud))]
static class MessageHudPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(MessageHud.ShowMessage))]
  static void ShowMessagePostfix(MessageHud.MessageType type, string text) {
    if (IsModEnabled.Value && type == MessageHud.MessageType.Center && ShowMessageHudCenterMessages.Value) {
      ChatMessageUtils.AddChatMessage(
          new() {
            MessageType = ChatMessageType.HudCenter,
            Timestamp = DateTime.Now,
            Text = text
          });
    }
  }
}
