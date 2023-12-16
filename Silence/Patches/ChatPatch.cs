using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

using static Silence.Silence;

namespace Silence {
  [HarmonyPatch(typeof(Chat))]
  static class ChatPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Chat.Awake))]
    static void AwakePostfix(Chat __instance) {
      ChatInstance = __instance;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chat.Update))]
    static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(
                  OpCodes.Call,
                  AccessTools.Method(
                      typeof(ZInput), nameof(ZInput.GetKeyDown), new Type[] { typeof(KeyCode), typeof(bool) })))
          .ThrowIfInvalid("Could not patch Chat.Update()! (GetKeyDown)")
          .Advance(offset: 1)
          .InsertAndAdvance(new CodeInstruction(Transpilers.EmitDelegate<Func<bool, bool>>(GetKeyDownDelegate)))
          .InstructionEnumeration();
    }

    static bool GetKeyDownDelegate(bool result) {
      return result && !IsSilenced;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Chat.AddInworldText))]
    static bool AddInworldTextPrefix() {
      return !IsSilenced;
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
        if (IsSilenced) {
          Chat chat = (Chat) _chatField.GetValue(__instance);
          chat.m_hideTimer = chat.m_hideDelay;
        }
      }
    }
  }
}
