namespace Silence;

using System.Collections;

using UnityEngine;

using static PluginConfig;

public static class SilenceManager {
  public static readonly WaitForEndOfFrame EndOfFrame = new();

  public static Chat ChatInstance { get; set; }
  public static bool IsSilenced { get; set; } = false;

  public static IEnumerator ToggleSilenceCoroutine() {
    if (!ChatInstance) {
      yield break;
    }

    yield return EndOfFrame;

    IsSilenced = !IsSilenced;

    Silence.LogInfo($"IsSilenced: {IsSilenced}");
    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"IsSilenced: {IsSilenced}");

    if (HideChatWindow.Value) {
      ToggleChatWindow(IsSilenced);
    }

    if (HideInWorldTexts.Value) {
      ToggleInWorldTexts(IsSilenced);
    }
  }

  static void ToggleChatWindow(bool isSilenced) {
    if (isSilenced) {
      ChatInstance.m_hideTimer = ChatInstance.m_hideDelay;
      ChatInstance.m_focused = false;
      ChatInstance.m_wasFocused = false;
      ChatInstance.m_input.DeactivateInputField();
      ChatInstance.m_input.gameObject.SetActive(false);
    }


    ChatInstance.m_chatWindow.gameObject.SetActive(isSilenced);
  }

  static void ToggleInWorldTexts(bool isSilenced) {
    if (isSilenced) {
      foreach (Chat.WorldTextInstance worldText in ChatInstance.m_worldTexts) {
        UnityEngine.Object.Destroy(worldText.m_gui);
      }


      ChatInstance.WorldTexts.Clear();
    }
  }
}
