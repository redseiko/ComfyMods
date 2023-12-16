using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using UnityEngine;

using static Silence.PluginConfig;

namespace Silence {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class Silence : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.silence";
    public const string PluginName = "Silence";
    public const string PluginVersion = "1.6.0";

    static ManualLogSource _logger;
    Harmony _harmony;

    void Awake() {
      _logger = Logger;
      BindConfig(Config);

      if (IsModEnabled.Value) {
        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
      }
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static Chat ChatInstance { get; set; }
    public static bool IsSilenced { get; set; } = false;
    public static readonly WaitForEndOfFrame EndOfFrame = new();

    public static IEnumerator ToggleSilenceCoroutine() {
      if (!ChatInstance) {
        yield break;
      }

      yield return EndOfFrame;

      IsSilenced = !IsSilenced;

      LogInfo($"IsSilenced: {IsSilenced}");
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
          Destroy(worldText.m_gui);
        }

        ChatInstance.m_worldTexts.Clear();
      }
    }

    public static void LogInfo(object o) {
      _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {o}");
    }
  }
}
