namespace Chatter;

using ComfyLib;

using GUIFramework;

using UnityEngine;

using static PluginConfig;

public static class ChatPanelController {
  public static ChatPanel ChatPanel { get; private set; }
  public static GuiInputField VanillaInputField { get; set; }

  public static void ToggleChatter(bool toggleOn) {
    ToggleChatter(Chat.m_instance, toggleOn);
  }

  public static void ToggleChatter(Chat chat, bool toggleOn) {
    TerminalCommands.ToggleCommands(toggleOn);

    if (chat) {
      ToggleVanillaChat(chat, !toggleOn);
      ToggleChatPanel(chat, toggleOn);

      chat.m_input = toggleOn ? ChatPanel.TextInput.InputField : VanillaInputField;
    }
  }

  public static void ToggleVanillaChat(Chat chat, bool toggleOn) {
    chat.m_output.transform.parent.gameObject.SetActive(toggleOn);
    chat.m_output.gameObject.SetActive(toggleOn);
  }

  public static void ToggleChatPanel(Chat chat, bool toggleOn) {
    if (!ChatPanel?.Panel) {
      ChatPanel = new(Hud.m_instance.transform);

      ChatPanel.Panel.GetComponent<RectTransform>()
          .SetAnchorMin(Vector2.right)
          .SetAnchorMax(Vector2.right)
          .SetPivot(Vector2.right)
          .SetPosition(ChatPanelPosition.Value)
          .SetSizeDelta(ChatPanelSizeDelta.Value)
          .SetAsFirstSibling();

      ChatPanel.PanelDragger.OnEndDragEvent += OnChatterChatPanelEndDrag;
      ChatPanel.PanelResizer.OnEndDragEvent += OnChatterChatPanelEndResize;
      ChatPanel.TextInput.InputField.onSubmit.AddListener(OnChatterTextInputFieldSubmit);

      ChatPanel.SetChatTextInputPrefix(ChatPanelDefaultMessageTypeToUse.Value);
      ChatPanel.SetupContentRowToggles(ChatPanelContentRowTogglesToEnable.Value);
      ChatPanel.SetContentSpacing();

      ContentRowManager.RebuildContentRows();
    }

    ChatPanel.Panel.SetActive(toggleOn);
  }

  public static void SetScrollContentScrollSensitivity(float scrollSensitivity) {
    if (TryGetChatPanel(out ChatPanel chatPanel)) {
      chatPanel.ContentScrollRect.SetScrollSensitivity(scrollSensitivity);
    }
  }

  public static bool TryGetChatPanel(out ChatPanel chatPanel) {
    chatPanel = ChatPanel;
    return chatPanel?.Panel;
  }

  static void OnChatterChatPanelEndDrag(object sender, Vector3 position) {
    ChatPanelPosition.Value = position;
  }

  static void OnChatterChatPanelEndResize(object send, Vector2 sizeDelta) {
    ChatPanelSizeDelta.Value = sizeDelta;
  }

  static void OnChatterTextInputFieldSubmit(string input) {
    Chat.m_instance.SendInput();
  }
}
