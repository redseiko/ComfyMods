using System;

using HarmonyLib;

using static Chatter.PluginConfig;

namespace Chatter {
  [HarmonyPatch(typeof(MessageHud))]
  static class MessageHudPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageHud.ShowMessage))]
    static void ShowMessagePostfix(MessageHud.MessageType type, string text) {
      if (IsModEnabled.Value && type == MessageHud.MessageType.Center && ShowMessageHudCenterMessages.Value) {
        Chatter.AddChatMessage(
            new() {
              MessageType = ChatMessageType.HudCenter,
              Timestamp = DateTime.Now,
              Text = text
            });
      }
    }
  }
}
