namespace Silence;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(Chat))]
static class ChatPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Chat.Awake))]
  static void AwakePostfix(Chat __instance) {
    SilenceManager.ChatInstance = __instance;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Chat.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(Player), nameof(Player.m_localPlayer))),
            new CodeMatch(OpCodes.Ldnull),
            new CodeMatch(OpCodes.Call))
        .ThrowIfInvalid("Could not patch Chat.Update()! (local-player-not-null)")
        .Advance(offset: 3)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(ChatPatch), nameof(LocalPlayerNotNullDelegate))))
        .InstructionEnumeration();
  }

  static bool LocalPlayerNotNullDelegate(bool playerIsNotNull) {
    return playerIsNotNull && !SilenceManager.IsSilenced;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Chat.AddInworldText))]
  static bool AddInworldTextPrefix() {
    return !SilenceManager.IsSilenced;
  }

  [HarmonyPatch]
  static class OnNewChatMessageDelegatePatch {
    static Type _delegateType;
    static FieldInfo _chatField;

    [HarmonyTargetMethod]
    static MethodBase DelegateMethod() {
      _delegateType = AccessTools.Inner(typeof(Chat), "<>c__DisplayClass11_0");
      _chatField = AccessTools.Field(_delegateType, "<>4__this");

      return AccessTools.Method(_delegateType, "<OnNewChatMessage>b__2");
    }

    [HarmonyPostfix]
    static void DelegatePostfix(object __instance) {
      if (SilenceManager.IsSilenced) {
        Chat chat = (Chat) _chatField.GetValue(__instance);
        chat.m_hideTimer = chat.m_hideDelay;
      }
    }
  }
}
