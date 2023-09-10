using ComfyLib;

namespace Chatter {
  public static class ChatPanelUtils {
    public static void SetupContentRowToggles(this ChatPanel chatPanel, ChatMessageType togglesToEnable) {
      ToggleRow toggleRow = chatPanel.MessageTypeToggleRow;

      toggleRow.SayToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Say));
      toggleRow.ShoutToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Shout));
      toggleRow.PingToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Ping));
      toggleRow.WhisperToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Whisper));
      toggleRow.MessageHudToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.HudCenter));
      toggleRow.TextToggle.Toggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Text));

      // TODO: no point rebuilding, better to just use SetActive and the like.
      //toggleRow.SayToggle.Toggle.onValueChanged.AddListener(isOn => Chatter.RebuildMessageRows());
      //toggleRow.ShoutToggle.Toggle.onValueChanged.AddListener(isOn => Chatter.RebuildMessageRows());
      //toggleRow.PingToggle.Toggle.onValueChanged.AddListener(isOn => Chatter.RebuildMessageRows());
      //toggleRow.WhisperToggle.Toggle.onValueChanged.AddListener(isOn => Chatter.RebuildMessageRows());
      //toggleRow.MessageHudToggle.Toggle.onValueChanged.AddListener(isOn => Chatter.RebuildMessageRows());
      //toggleRow.TextToggle.Toggle.onValueChanged.AddListener(isOn => Chatter.RebuildMessageRows());
    }

    public static bool IsMessageTypeActive(this ChatPanel chatPanel, ChatMessageType messageType) {
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
