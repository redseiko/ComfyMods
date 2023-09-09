using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

using HarmonyLib;

using TMPro;

using UnityEngine;

using static Chatter.Chatter;
using static Chatter.PluginConfig;

namespace Chatter {
  [HarmonyPatch(typeof(Chat))]
  static class ChatPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Chat.Awake))]
    static void AwakePostfix(Chat __instance) {
      //_chatInputField = __instance.m_input;
      //MessageRows.ClearItems();

      ToggleChatter(__instance, IsModEnabled.Value);
      SetupWorldText(__instance);
    }

    static void SetupWorldText(Chat chat) {
      chat.m_worldTextBase = WorldTextUtils.CreateWorldTextTemplate(chat.m_worldTextBase.transform.parent);
      chat.m_worldTextBase.SetActive(false);
    }

    //  [HarmonyTranspiler]
    //  [HarmonyPatch(nameof(Chat.InputText))]
    //  static IEnumerable<CodeInstruction> InputTextTranspiler(IEnumerable<CodeInstruction> instructions) {
    //    return new CodeMatcher(instructions)
    //        .MatchForward(useEnd: true, new CodeMatch(OpCodes.Ldstr, "say "))
    //        .Advance(offset: 1)
    //        .InsertAndAdvance(Transpilers.EmitDelegate<Func<string, string>>(PrefixSayDelegate))
    //        .InstructionEnumeration();
    //  }

    //  static string PrefixSayDelegate(string value) {
    //    if (IsModEnabled.Value) {
    //      return ChatInputTextDefaultPrefix;
    //    }

    //    return value;
    //  }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Chat.OnNewChatMessage))]
    static void OnNewChatMessagePrefix(long senderID, Vector3 pos, Talker.Type type, UserInfo user, string text) {
      if (!IsModEnabled.Value) {
        return;
      }

      ChatMessage message = new() {
        MessageType = ChatMessageUtils.GetChatMessageType(type),
        Timestamp = DateTime.Now,
        SenderId = senderID,
        Position = pos,
        TalkerType = type,
        Username = user.Name,
        Text = Regex.Replace(text, @"(<|>)", " "),
      };

      IsChatMessageQueued = true;
      AddChatMessage(message);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Chat.OnNewChatMessage))]
    static void OnNewChatMessagePostfix() {
      if (IsModEnabled.Value) {
        IsChatMessageQueued = false;
      }
    }

    //  [HarmonyTranspiler]
    //  [HarmonyPatch(nameof(Chat.Update))]
    //  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    //    return new CodeMatcher(instructions)
    //        .MatchForward(
    //            useEnd: true,
    //            new CodeMatch(OpCodes.Ldarg_0),
    //            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chat), nameof(Chat.m_hideTimer))),
    //            new CodeMatch(OpCodes.Ldarg_0),
    //            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chat), nameof(Chat.m_hideDelay))),
    //            new CodeMatch(OpCodes.Clt),
    //            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))))
    //        .InsertAndAdvance(
    //            new CodeInstruction(OpCodes.Ldarg_0),
    //            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Chat), nameof(Chat.m_hideTimer))),
    //            Transpilers.EmitDelegate<Action<float>>(HideChatPanelDelegate))
    //        .MatchForward(
    //            useEnd: false,
    //            new CodeMatch(OpCodes.Ldarg_0),
    //            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Terminal), nameof(Terminal.m_input))),
    //            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Component), "get_gameObject")),
    //            new CodeMatch(OpCodes.Ldc_I4_1),
    //            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))),
    //            new CodeMatch(OpCodes.Ldarg_0),
    //            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Terminal), nameof(Terminal.m_input))),
    //            new CodeMatch(
    //                OpCodes.Callvirt, AccessTools.Method(typeof(InputField), nameof(InputField.ActivateInputField))))
    //        .InsertAndAdvance(Transpilers.EmitDelegate<Action>(EnableChatPanelDelegate))
    //        .MatchForward(
    //            useEnd: false,
    //            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Terminal), nameof(Terminal.m_input))),
    //            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Component), "get_gameObject")),
    //            new CodeMatch(OpCodes.Ldc_I4_0),
    //            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))),
    //            new CodeMatch(OpCodes.Ldarg_0),
    //            new CodeMatch(OpCodes.Ldc_I4_0),
    //            new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(Terminal), nameof(Terminal.m_focused))))
    //        .Advance(offset: 3)
    //        .InsertAndAdvance(Transpilers.EmitDelegate<Func<bool, bool>>(DisableChatPanelDelegate))
    //        .InstructionEnumeration();
    //  }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Chat.Update))]
    static void UpdatePostfix(ref Chat __instance) {
      if (!IsModEnabled.Value || !ChatterChatPanel?.Panel) {
        return;
      }

      if (ScrollContentUpShortcut.Value.IsDown() && ChatterChatPanel.Panel.activeInHierarchy) {
        ChatterChatPanel.OffsetVerticalScrollPosition(ScrollContentOffsetInterval.Value);
        __instance.m_hideTimer = 0f;
      }

      if (ScrollContentDownShortcut.Value.IsDown()) {
        ChatterChatPanel.OffsetVerticalScrollPosition(-ScrollContentOffsetInterval.Value);
        __instance.m_hideTimer = 0f;
      }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chat.AddInworldText))]
    static IEnumerable<CodeInstruction> AddInworldTextTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(string), nameof(string.ToUpper))))
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<string, string>>(ToUpperDelegate))
          .InstructionEnumeration();
    }

    static string ToUpperDelegate(string text) {
      return IsModEnabled.Value ? text : text.ToUpper();
    }
  }
}
