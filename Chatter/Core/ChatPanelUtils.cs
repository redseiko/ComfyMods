using ComfyLib;

using static Chatter.PluginConfig;

namespace Chatter {
  public static class ChatPanelUtils {
    public static void ShowOrHideChatPanel(this ChatPanel chatPanel, bool isVisible) {
      if (isVisible == chatPanel.PanelCanvasGroup.blocksRaycasts) {
        return;
      }

      if (isVisible) {
        chatPanel.PanelCanvasGroup
            .SetAlpha(1f)
            .SetBlocksRaycasts(true);
      } else {
        chatPanel.PanelCanvasGroup
            .SetAlpha(Hud.IsUserHidden() ? 0f : HideChatPanelAlpha.Value)
            .SetBlocksRaycasts(false);

        chatPanel.SetContentVerticalScrollPosition(0f);
      }
    }

    public static void EnableOrDisableChatPanel(this ChatPanel chatPanel, bool isEnabled) {
      chatPanel.TextInput.InputField.Ref()?.SetEnabled(isEnabled);
    }

    public static void SetupContentSpacing(this ChatPanel chatPanel) {
      if (ChatMessageLayout.Value == MessageLayoutType.WithHeaderRow) {
        chatPanel.ContentLayoutGroup.SetSpacing(ChatPanelContentSpacing.Value);
      } else {
        chatPanel.ContentLayoutGroup.SetSpacing(ChatPanelContentSingleRowSpacing.Value);
      }
    }

    public static void SetupContentRowToggles(this ChatPanel chatPanel, ChatMessageType togglesToEnable) {
      ToggleRow toggleRow = chatPanel.MessageTypeToggleRow;

      toggleRow.SayToggle.Toggle.onValueChanged.AddListener(
          isOn => ContentRowManager.ToggleContentRows(isOn, ChatMessageType.Say));
      toggleRow.ShoutToggle.Toggle.onValueChanged.AddListener(
          isOn => ContentRowManager.ToggleContentRows(isOn, ChatMessageType.Shout));
      toggleRow.PingToggle.Toggle.onValueChanged.AddListener(
          isOn => ContentRowManager.ToggleContentRows(isOn, ChatMessageType.Ping));
      toggleRow.WhisperToggle.Toggle.onValueChanged.AddListener(
          isOn => ContentRowManager.ToggleContentRows(isOn, ChatMessageType.Whisper));
      toggleRow.MessageHudToggle.Toggle.onValueChanged.AddListener(
          isOn => ContentRowManager.ToggleContentRows(isOn, ChatMessageType.HudCenter));
      toggleRow.TextToggle.Toggle.onValueChanged.AddListener(
          isOn => ContentRowManager.ToggleContentRows(isOn, ChatMessageType.Text));

      toggleRow.SayToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Say));
      toggleRow.ShoutToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Shout));
      toggleRow.PingToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Ping));
      toggleRow.WhisperToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Whisper));
      toggleRow.MessageHudToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.HudCenter));
      toggleRow.TextToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Text));
    }

    public static bool IsMessageTypeToggleActive(this ChatPanel chatPanel, ChatMessageType messageType) {
      ToggleRow toggleRow = chatPanel.MessageTypeToggleRow;

      return messageType switch {
        ChatMessageType.Text => toggleRow.TextToggle.Toggle.isOn,
        ChatMessageType.HudCenter => toggleRow.MessageHudToggle.Toggle.isOn,
        ChatMessageType.Say => toggleRow.SayToggle.Toggle.isOn,
        ChatMessageType.Shout => toggleRow.ShoutToggle.Toggle.isOn,
        ChatMessageType.Whisper => toggleRow.WhisperToggle.Toggle.isOn,
        ChatMessageType.Ping => toggleRow.PingToggle.Toggle.isOn,
        _ => true,
      };
    }
  }
}
