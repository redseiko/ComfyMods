using System;

using HarmonyLib;

using static Chatter.PluginConfig;

namespace Chatter {
  [HarmonyPatch(typeof(Terminal))]
  static class TerminalPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Terminal.AddString), typeof(string))]
    static void AddStringFinalPostfix(Terminal __instance, string text) {
      if (IsModEnabled.Value && __instance is Chat && !Chatter.IsChatMessageQueued) {
        Chatter.AddChatMessage(
            new() {
              MessageType = ChatMessageType.Text,
              Timestamp = DateTime.Now,
              Text = text,
            });
      }
    }
  }
}