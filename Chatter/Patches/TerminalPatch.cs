using System;

using HarmonyLib;

using static Chatter.PluginConfig;

namespace Chatter {
  [HarmonyPatch(typeof(Terminal))]
  static class TerminalPatch {
    //  [HarmonyTranspiler]
    //  [HarmonyPatch(nameof(Terminal.UpdateInput))]
    //  static IEnumerable<CodeInstruction> UpdateInputTranspiler(IEnumerable<CodeInstruction> instructions) {
    //    return new CodeMatcher(instructions)
    //        .MatchForward(
    //            useEnd: false,
    //            new CodeMatch(OpCodes.Ldarg_0),
    //            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Terminal), nameof(Terminal.m_input))),
    //            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Component), "get_gameObject")),
    //            new CodeMatch(OpCodes.Ldc_I4_0),
    //            new CodeMatch(OpCodes.Callvirt, typeof(GameObject).GetMethod(nameof(GameObject.SetActive))),
    //            new CodeMatch(OpCodes.Ret))
    //        .Advance(offset: 4)
    //        .InsertAndAdvance(Transpilers.EmitDelegate<Func<bool, bool>>(DisableChatPanelDelegate))
    //        .InstructionEnumeration();
    //  }

    //  [HarmonyPostfix]
    //  [HarmonyPatch(nameof(Terminal.SendInput))]
    //  static void SendInputPostfix(ref Terminal __instance) {
    //    if (IsModEnabled.Value && __instance == Chat.m_instance && Chatter.ChatPanel != null) {
    //      Chatter.ChatPanel.SetVerticalScrollPosition(0f);
    //    }
    //  }

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