﻿namespace Chatter;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Chat))]
static class ChatPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Chat.Awake))]
  static void AwakePostfix(Chat __instance) {
    ContentRowManager.MessageRows.ClearItems();
    ChatPanelController.VanillaInputField = __instance.m_input;
    ChatPanelController.ToggleChatter(__instance, IsModEnabled.Value);
    SetupWorldText(__instance);
  }

  static void SetupWorldText(Chat chat) {
    chat.m_worldTextBase = WorldTextUtils.CreateWorldTextTemplate(chat.m_worldTextBase.transform.parent);
    chat.m_worldTextBase.SetActive(false);
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Chat.InputText))]
  static IEnumerable<CodeInstruction> InputTextTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(new CodeMatch(OpCodes.Ldstr, "say "))
        .ThrowIfInvalid($"Could not patch Chat.InputText()! (prefix-say)")
        .Advance(offset: 1)
        .InsertAndAdvance(Transpilers.EmitDelegate(PrefixSayDelegate))
        .InstructionEnumeration();
  }

  static string PrefixSayDelegate(string value) {
    return IsModEnabled.Value ? ChatTextInputUtils.ChatTextInputPrefix : value;
  }

  static bool _isChatMessageFiltered = false;

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Chat.OnNewChatMessage))]
  static void OnNewChatMessagePrefix(
      Chat __instance, long senderID, Vector3 pos, Talker.Type type, UserInfo user, string text, ref float __state) {
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

    ChatMessageUtils.IsChatMessageQueued = true;

    _isChatMessageFiltered = !ChatMessageUtils.AddChatMessage(message);
    __state = __instance.m_hideTimer;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Chat.OnNewChatMessage))]
  static void OnNewChatMessagePostfix(ref Chat __instance, float __state) {
    if (!IsModEnabled.Value) {
      return;
    }

    ChatMessageUtils.IsChatMessageQueued = false;

    if (_isChatMessageFiltered) {
      __instance.m_hideTimer = __state;
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Chat.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler1(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chat), nameof(Chat.m_hideTimer))),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chat), nameof(Chat.m_hideDelay))),
            new CodeMatch(OpCodes.Clt),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))))
        .ThrowIfInvalid("Could not patch Chat.Update()! (HideChatPanel)")
        .Advance(offset: 6)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Chat), nameof(Chat.m_hideTimer))),
            Transpilers.EmitDelegate(HideChatPanelDelegate))
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Chat.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler2(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Terminal), nameof(Terminal.m_input))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Component), "get_gameObject")),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(
                OpCodes.Ldfld, AccessTools.Field(typeof(Chat), nameof(Chat.m_doubleOpenForVirtualKeyboard))))
        .ThrowIfInvalid("Could not patch Chat.Update()! (EnableChatPanel)")
        .InsertAndAdvance(Transpilers.EmitDelegate(EnableChatPanelDelegate))
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Chat.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler3(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Terminal), nameof(Terminal.m_input))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Component), "get_gameObject")),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(Terminal), nameof(Terminal.m_focused))))
        .ThrowIfInvalid("Could not patch Chat.Update()! (DisableChatPanel)")
        .Advance(offset: 4)
        .InsertAndAdvance(Transpilers.EmitDelegate(DisableChatPanelDelegate))
        .InstructionEnumeration();
  }

  static void HideChatPanelDelegate(float hideTimer) {
    if (IsModEnabled.Value && ChatPanelController.ChatPanel?.Panel) {
      bool isVisible = (hideTimer < HideChatPanelDelay.Value || Menu.IsVisible()) && !Hud.IsUserHidden();
      ChatPanelController.ChatPanel.ShowOrHideChatPanel(isVisible);
    }
  }

  static void EnableChatPanelDelegate() {
    if (IsModEnabled.Value) {
      ChatPanelController.ChatPanel?.EnableOrDisableChatPanel(true);
    }
  }

  static bool DisableChatPanelDelegate(bool active) {
    if (IsModEnabled.Value) {
      if (!Menu.IsVisible()) {
        ChatPanelController.ChatPanel?.EnableOrDisableChatPanel(false);
      }

      return true;
    }

    return active;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Chat.Update))]
  static void UpdatePostfix(Chat __instance) {
    if (!IsModEnabled.Value || !ChatPanelController.ChatPanel?.Panel) {
      return;
    }

    if (ScrollContentUpShortcut.Value.IsDown() && ChatPanelController.ChatPanel.Panel.activeInHierarchy) {
      ChatPanelController.ChatPanel.OffsetContentVerticalScrollPosition(ScrollContentOffsetInterval.Value);
      __instance.m_hideTimer = 0f;
    }

    if (ScrollContentDownShortcut.Value.IsDown()) {
      ChatPanelController.ChatPanel.OffsetContentVerticalScrollPosition(-ScrollContentOffsetInterval.Value);
      __instance.m_hideTimer = 0f;
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Chat.SendInput))]
  static IEnumerable<CodeInstruction> SendInputTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), nameof(GameObject.SetActive))))
        .ThrowIfInvalid($"Could not patch Chat.SendInput()! (SetActive)")
        .InsertAndAdvance(Transpilers.EmitDelegate(DisableChatPanelDelegate))
        .InstructionEnumeration();
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Chat.SendInput))]
  static void SendInputPostfix() {
    if (IsModEnabled.Value) {
      ChatPanelController.ChatPanel?.SetContentVerticalScrollPosition(0f);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Chat.HasFocus))]
  static void HasFocusPostfix(ref Chat __instance, ref bool __result) {
    if (IsModEnabled.Value && ChatPanelController.ChatPanel?.Panel) {
      __result = ChatPanelController.ChatPanel.Panel.activeInHierarchy && __instance.m_input.isFocused;
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Chat.AddInworldText))]
  static bool AddInworldTextPrefix(string text) {
    if (IsModEnabled.Value && FilterInWorldShoutText.Value) {
      return ChatMessageUtils.ShouldShowText(ChatMessageType.Shout, text);
    }

    return true;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Chat.AddInworldText))]
  static IEnumerable<CodeInstruction> AddInworldTextTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(string), nameof(string.ToUpper))))
        .ThrowIfInvalid($"Could not patch Chat.AddInworldText()! (ToUpper)")
        .SetInstructionAndAdvance(Transpilers.EmitDelegate(ToUpperDelegate))
        .InstructionEnumeration();
  }

  static string ToUpperDelegate(string text) {
    return IsModEnabled.Value ? text : text.ToUpper();
  }
}
