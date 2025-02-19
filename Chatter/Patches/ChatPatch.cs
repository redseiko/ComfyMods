namespace Chatter;

using System;
using System.Collections;
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
    __instance.StartCoroutine(DelayedAwakePostfix());
  }

  static IEnumerator DelayedAwakePostfix() {
    while (!Hud.m_instance) {
      yield return null;
    }

    Chat chat = Chat.m_instance;
    ContentRowManager.MessageRows.ClearItems();

    ChatPanelController.VanillaInputField = chat.m_input;
    ChatPanelController.ToggleChatter(chat, IsModEnabled.Value);

    SetupWorldText(chat);
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
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ChatPatch), nameof(PrefixSayDelegate))))
        .InstructionEnumeration();
  }

  static string PrefixSayDelegate(string value) {
    return IsModEnabled.Value ? ChatTextInputUtils.ChatTextInputPrefix : value;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Chat.OnNewChatMessage))]
  static void OnNewChatMessagePrefix(
      Chat __instance, long senderID, Vector3 pos, Talker.Type type, UserInfo sender, string text, ref float __state) {
    if (!IsModEnabled.Value) {
      return;
    }

    ChatMessage message = new() {
      MessageType = ChatMessageUtils.GetChatMessageType(type),
      Timestamp = DateTime.Now,
      SenderId = senderID,
      Position = pos,
      TalkerType = type,
      Username = sender.Name,
      Text = Regex.Replace(text, @"(<|>)", " "),
    };

    ChatMessageUtils.IsChatMessageQueued = true;
    ChatMessageUtils.IsChatMessageFiltered = !ChatMessageUtils.AddChatMessage(message);

    __state = __instance.m_hideTimer;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Chat.OnNewChatMessage))]
  static void OnNewChatMessagePostfix(Chat __instance, float __state) {
    if (!IsModEnabled.Value) {
      return;
    }

    ChatMessageUtils.IsChatMessageQueued = false;

    if (ChatMessageUtils.IsChatMessageFiltered) {
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
        .ThrowIfInvalid("Could not patch Chat.Update()! (hide-chat-panel)")
        .Advance(offset: 6)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Chat), nameof(Chat.m_hideTimer))),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ChatPatch), nameof(HideChatPanelDelegate))))
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
                OpCodes.Call,
                AccessTools.Method(typeof(Chat), nameof(Chat.TryShowTextCommunicationRestrictedSystemPopup))),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(
                OpCodes.Ldfld, AccessTools.Field(typeof(Chat), nameof(Chat.m_doubleOpenForVirtualKeyboard))))
        .ThrowIfInvalid("Could not patch Chat.Update()! (enable-chat-panel)")
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ChatPatch), nameof(EnableChatPanelDelegate))))
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
        .ThrowIfInvalid("Could not patch Chat.Update()! (disable-chat-panel)")
        .Advance(offset: 4)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ChatPatch), nameof(DisableChatPanelDelegate))))
        .InstructionEnumeration();
  }

  static void HideChatPanelDelegate(float hideTimer) {
    if (IsModEnabled.Value && ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
      bool isVisible = (hideTimer < HideChatPanelDelay.Value || Menu.IsVisible()) && !Hud.IsUserHidden();
      chatPanel.ShowOrHideChatPanel(isVisible);
    }
  }

  static void EnableChatPanelDelegate() {
    if (IsModEnabled.Value && ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
      chatPanel.EnableOrDisableChatPanel(true);
    }
  }

  static bool DisableChatPanelDelegate(bool active) {
    if (IsModEnabled.Value) {
      if (!Menu.IsVisible() && ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
        chatPanel.EnableOrDisableChatPanel(false);
      }

      return true;
    }

    return active;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Chat.Update))]
  static void UpdatePostfix(Chat __instance) {
    if (!IsModEnabled.Value || !ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
      return;
    }

    if (ScrollContentUpShortcut.Value.IsDown() && chatPanel.Panel.activeInHierarchy) {
      chatPanel.OffsetContentVerticalScrollPosition(ScrollContentOffsetInterval.Value);
      __instance.m_hideTimer = 0f;
    }

    if (ScrollContentDownShortcut.Value.IsDown()) {
      chatPanel.OffsetContentVerticalScrollPosition(-ScrollContentOffsetInterval.Value);
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
        .ThrowIfInvalid($"Could not patch Chat.SendInput()! (set-active)")
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ChatPatch), nameof(DisableChatPanelDelegate))))
        .InstructionEnumeration();
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Chat.SendInput))]
  static void SendInputPostfix() {
    if (IsModEnabled.Value && ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
      chatPanel.SetContentVerticalScrollPosition(0f);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Chat.HasFocus))]
  static void HasFocusPostfix(Chat __instance, ref bool __result) {
    if (IsModEnabled.Value && ChatPanelController.TryGetChatPanel(out ChatPanel chatPanel)) {
      __result = chatPanel.Panel.activeInHierarchy && __instance.m_input.isFocused;
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
        .ThrowIfInvalid($"Could not patch Chat.AddInworldText()! (to-upper)")
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ChatPatch), nameof(ToUpperDelegate))))
        .InstructionEnumeration();
  }

  static string ToUpperDelegate(string text) {
    return IsModEnabled.Value ? text : text.ToUpper();
  }
}
