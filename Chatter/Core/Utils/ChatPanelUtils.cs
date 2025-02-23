namespace Chatter;

using ComfyLib;

using TMPro;

using static PluginConfig;

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

  public static void SetContentSpacing(this ChatPanel chatPanel) {
    if (ChatMessageLayout.Value == MessageLayoutType.WithHeaderRow) {
      chatPanel.ContentLayoutGroup.SetSpacing(ChatPanelContentSpacing.Value);
    } else {
      chatPanel.ContentLayoutGroup.SetSpacing(ChatPanelContentSingleRowSpacing.Value);
    }
  }

  public static void SetContentFontAsset(this ChatPanel chatPanel, TMP_FontAsset fontAsset) {
    if (!chatPanel?.Content || !fontAsset) {
      return;
    }

    foreach (TMP_Text tmpText in chatPanel.Content.GetComponentsInChildren<TMP_Text>(includeInactive: true)) {
      tmpText.font = fontAsset;
    }
  }

  public static void SetContentFontSize(this ChatPanel chatPanel, float fontSize) {
    if (!chatPanel?.Content) {
      return;
    }

    foreach (TMP_Text tmpText in chatPanel.Content.GetComponentsInChildren<TMP_Text>(includeInactive: true)) {
      tmpText.fontSize = fontSize;
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

    toggleRow.SayToggle.SetToggleIsOn(togglesToEnable.HasFlag(ChatMessageType.Say));
    toggleRow.ShoutToggle.SetToggleIsOn(togglesToEnable.HasFlag(ChatMessageType.Shout));
    toggleRow.PingToggle.SetToggleIsOn(togglesToEnable.HasFlag(ChatMessageType.Ping));
    toggleRow.WhisperToggle.SetToggleIsOn(togglesToEnable.HasFlag(ChatMessageType.Whisper));
    toggleRow.MessageHudToggle.SetToggleIsOn(togglesToEnable.HasFlag(ChatMessageType.HudCenter));
    toggleRow.TextToggle.SetToggleIsOn(togglesToEnable.HasFlag(ChatMessageType.Text));
  }

  public static bool IsMessageTypeToggleActive(this ChatPanel chatPanel, ChatMessageType messageType) {
    ToggleRow toggleRow = chatPanel.MessageTypeToggleRow;

    return messageType switch {
      ChatMessageType.Text => toggleRow.TextToggle.GetToggleIsOn(),
      ChatMessageType.HudCenter => toggleRow.MessageHudToggle.GetToggleIsOn(),
      ChatMessageType.Say => toggleRow.SayToggle.GetToggleIsOn(),
      ChatMessageType.Shout => toggleRow.ShoutToggle.GetToggleIsOn(),
      ChatMessageType.Whisper => toggleRow.WhisperToggle.GetToggleIsOn(),
      ChatMessageType.Ping => toggleRow.PingToggle.GetToggleIsOn(),
      _ => true,
    };
  }
}
