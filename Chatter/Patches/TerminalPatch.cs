using System;
using System.Reflection;

using HarmonyLib;

using static Chatter.PluginConfig;

namespace Chatter {
  [HarmonyPatch(typeof(Terminal))]
  static class TerminalPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Terminal.AddString), typeof(string))]
    static void AddStringPostfix(Terminal __instance, string text) {
      if (IsModEnabled.Value && __instance is Chat && !Chatter.IsChatMessageQueued) {
        Chatter.AddChatMessage(
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
        return AccessTools.Method(AccessTools.Inner(typeof(Terminal), "<>c"), "<InitTerminal>b__7_119");
      }

      [HarmonyPostfix]
      static void SayDelegatePostfix(ref object __result) {
        if (IsModEnabled.Value && (bool) __result == false) {
          Chatter.ChatterChatPanel?.SetChatTextInputDefaultPrefix(Talker.Type.Normal);
          __result = true;
        }
      }
    }

    [HarmonyPatch]
    static class ShoutDelegatePatch {
      [HarmonyTargetMethod]
      static MethodBase FindShoutDelegateMethod() {
        return AccessTools.Method(AccessTools.Inner(typeof(Terminal), "<>c"), "<InitTerminal>b__7_120");
      }

      [HarmonyPostfix]
      static void ShoutDelegatePostfix(ref object __result) {
        if (IsModEnabled.Value && (bool) __result == false) {
          Chatter.ChatterChatPanel?.SetChatTextInputDefaultPrefix(Talker.Type.Shout);
          __result = true;
        }
      }
    }

    [HarmonyPatch]
    static class WhisperDelegatePatch {
      [HarmonyTargetMethod]
      static MethodBase FindWhisperDelegateMethod() {
        return AccessTools.Method(AccessTools.Inner(typeof(Terminal), "<>c"), "<InitTerminal>b__7_121");
      }

      [HarmonyPostfix]
      static void WhisperDelegatePostfix(ref object __result) {
        if (IsModEnabled.Value && (bool) __result == false) {
          Chatter.ChatterChatPanel?.SetChatTextInputDefaultPrefix(Talker.Type.Whisper);
          __result = true;
        }
      }
    }
  }
}