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

  [HarmonyPatch]
  static class SayDelegatePatch {
    [HarmonyTargetMethod]
    static MethodBase FindSayDelegateMethod() {
      return AccessTools.Method(AccessTools.Inner(typeof(Terminal), "<>c"), "<InitTerminal>b__7_128");
    }

    [HarmonyPostfix]
    static void SayDelegatePostfix(ref object __result) {
      if (IsModEnabled.Value && (bool) __result == false) {
        ChatPanelController.ChatPanel?.SetChatTextInputPrefix(Talker.Type.Normal);
        __result = true;
      }
    }
  }

  [HarmonyPatch]
  static class ShoutDelegatePatch {
    [HarmonyTargetMethod]
    static MethodBase FindShoutDelegateMethod() {
      return AccessTools.Method(AccessTools.Inner(typeof(Terminal), "<>c"), "<InitTerminal>b__7_129");
    }

    [HarmonyPostfix]
    static void ShoutDelegatePostfix(ref object __result) {
      if (IsModEnabled.Value && (bool) __result == false) {
        ChatPanelController.ChatPanel?.SetChatTextInputPrefix(Talker.Type.Shout);
        __result = true;
      }
    }
  }

  [HarmonyPatch]
  static class WhisperDelegatePatch {
    [HarmonyTargetMethod]
    static MethodBase FindWhisperDelegateMethod() {
      return AccessTools.Method(AccessTools.Inner(typeof(Terminal), "<>c"), "<InitTerminal>b__7_130");
    }

    [HarmonyPostfix]
    static void WhisperDelegatePostfix(ref object __result) {
      if (IsModEnabled.Value && (bool) __result == false) {
        ChatPanelController.ChatPanel?.SetChatTextInputPrefix(Talker.Type.Whisper);
        __result = true;
      }
    }
  }
}
