namespace Chatter;

using System;
using System.Reflection;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Terminal))]
static class TerminalPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Terminal.AddString), typeof(string))]
  static void AddStringPostfix(Terminal __instance, string text) {
    if (IsModEnabled.Value && __instance is Chat && !ChatMessageUtils.IsChatMessageQueued) {
      ChatMessageUtils.AddChatMessage(
          new() {
            MessageType = ChatMessageType.Text,
            Timestamp = DateTime.Now,
            Text = text,
          });
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Terminal.InitTerminal))]
  static void InitTerminalPostfix() {
    if (TryGetDelegateMethod(Terminal.commands["say"], out MethodInfo sayMethod)) {
      Chatter.HarmonyInstance.Patch(
          sayMethod,
          postfix: new HarmonyMethod(AccessTools.Method(typeof(TerminalPatch), nameof(SayDelegatePostfix))));
    }

    if (TryGetDelegateMethod(Terminal.commands["s"], out MethodInfo shoutMethod)) {
      Chatter.HarmonyInstance.Patch(
          shoutMethod,
          postfix: new HarmonyMethod(AccessTools.Method(typeof(TerminalPatch), nameof(ShoutDelegatePostfix))));
    }

    if (TryGetDelegateMethod(Terminal.commands["w"], out MethodInfo whisperMethod)) {
      Chatter.HarmonyInstance.Patch(
          whisperMethod,
          postfix: new HarmonyMethod(AccessTools.Method(typeof(TerminalPatch), nameof(WhisperDelegatePostfix))));
    }
  }

  static bool TryGetDelegateMethod(Terminal.ConsoleCommand command, out MethodInfo method) {
    method = (command == default ? default : command.action?.Method ?? command.actionFailable?.Method);
    return method != default;
  }

  static void SayDelegatePostfix(ref object __result) {
    if (IsModEnabled.Value
        && (bool) __result == false
        && ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
      chatPanel.SetChatTextInputPrefix(Talker.Type.Normal);
      __result = true;
    }
  }

  static void ShoutDelegatePostfix(ref object __result) {
    if (IsModEnabled.Value
        && (bool) __result == false
        && ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
      chatPanel.SetChatTextInputPrefix(Talker.Type.Shout);
      __result = true;
    }
  }

  static void WhisperDelegatePostfix(ref object __result) {
    if (IsModEnabled.Value
        && (bool) __result == false
        && ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
      chatPanel.SetChatTextInputPrefix(Talker.Type.Whisper);
      __result = true;
    }
  }
}
