namespace Chatter;

using ComfyLib;

using GUIFramework;

using UnityEngine;
using UnityEngine.UI;

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
    foreach (Image image in chat.m_chatWindow.GetComponentsInChildren<Image>(includeInactive: true)) {
      image.gameObject.SetActive(toggleOn);
    }

    chat.m_chatWindow.GetComponent<RectMask2D>().enabled = toggleOn;
    chat.m_output.gameObject.SetActive(toggleOn);
  }

  public static void ToggleChatPanel(Chat chat, bool toggleOn) {
    if (!ChatPanel?.Panel) {
      ChatPanel = new(chat.m_chatWindow.parent);

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
