namespace Chatter;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using ComfyLib;

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

  static MethodInfo _sayMethodDelegate = default;
  static MethodInfo _shoutMethodDelegate = default;
  static MethodInfo _whisperMethodDelegate = default;

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Terminal.InitTerminal))]
  static IEnumerable<CodeInstruction> InitTerminalTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchCommandDelegate("say")
        .CopyOperand(out _sayMethodDelegate)
        .Start()
        .MatchCommandDelegate("s")
        .CopyOperand(out _shoutMethodDelegate)
        .Start()
        .MatchCommandDelegate("w")
        .CopyOperand(out _whisperMethodDelegate)
        .InstructionEnumeration();
  }

  static CodeMatcher MatchCommandDelegate(this CodeMatcher matcher, string command) {
    return matcher
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, command),
            new CodeMatch(OpCodes.Ldstr))
        .ThrowIfInvalid($"Could not patch Terminal.InitTerminal()! (ldstr-{command})")
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldsfld),
            new CodeMatch(OpCodes.Ldftn))
        .ThrowIfInvalid($"Could not patch Terminal.InitTerminal()! (ldftn-{command}")
        .Advance(offset: 1);
  }

  [HarmonyPatch]
  [HarmonyAfter(Chatter.PluginGuid)]
  static class SayDelegatePatch {
    [HarmonyTargetMethod]
    static MethodBase FindSayDelegateMethod() {
      return _sayMethodDelegate;
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
  [HarmonyAfter(Chatter.PluginGuid)]
  static class ShoutDelegatePatch {
    [HarmonyTargetMethod]
    static MethodBase FindShoutDelegateMethod() {
      return _shoutMethodDelegate;
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
  [HarmonyAfter(Chatter.PluginGuid)]
  static class WhisperDelegatePatch {
    [HarmonyTargetMethod]
    static MethodBase FindWhisperDelegateMethod() {
      return _whisperMethodDelegate;
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
