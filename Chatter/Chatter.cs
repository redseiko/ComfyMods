using System.Reflection;

using BepInEx;

using ComfyLib;

using Fishlabs;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

using static Chatter.PluginConfig;

namespace Chatter {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class Chatter : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.chatter";
    public const string PluginName = "Chatter";
    public const string PluginVersion = "2.4.0";

    Harmony _harmony;

    void Awake() {
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static CircularQueue<ChatMessage> MessageHistory { get; } = new(capacity: 50, _ => { });

    public static bool IsChatMessageQueued { get; set; }
    public static ChatPanel ChatterChatPanel { get; private set; }
    public static GuiInputField VanillaInputField { get; set; }

    public static void ToggleChatter(bool toggleOn) {
      ToggleChatter(Chat.m_instance, toggleOn);
    }

    public static void ToggleChatter(Chat chat, bool toggleOn) {
      TerminalCommands.ToggleCommands(toggleOn);

      if (chat) {
        ToggleVanillaChat(chat, !toggleOn);
        ToggleChatPanel(chat, toggleOn);

        chat.m_input = toggleOn ? ChatterChatPanel.TextInput.InputField : VanillaInputField;
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
      if (!ChatterChatPanel?.Panel) {
        ChatterChatPanel = new(chat.m_chatWindow.parent);

        ChatterChatPanel.Panel.GetComponent<RectTransform>()
            .SetAnchorMin(Vector2.right)
            .SetAnchorMax(Vector2.right)
            .SetPivot(Vector2.right)
            .SetPosition(ChatPanelPosition.Value)
            .SetSizeDelta(ChatPanelSizeDelta.Value)
            .SetAsFirstSibling();

        ChatterChatPanel.PanelDragger.OnEndDragEvent += (_, position) => ChatPanelPosition.Value = position;
        ChatterChatPanel.PanelResizer.OnEndDragEvent += (_, sizeDelta) => ChatPanelSizeDelta.Value = sizeDelta;
        ChatterChatPanel.TextInput.InputField.onSubmit.AddListener(_ => Chat.m_instance.SendInput());

        ChatterChatPanel.SetChatTextInputPrefix(ChatPanelDefaultMessageTypeToUse.Value);
        ChatterChatPanel.SetupContentRowToggles(ChatPanelContentRowTogglesToEnable.Value);
        ChatterChatPanel.SetContentSpacing();

        ContentRowManager.RebuildContentRows();
      }

      ChatterChatPanel.Panel.SetActive(toggleOn);
    }

    public static void AddChatMessage(ChatMessage message) {
      if (ChatMessageUtils.ShouldShowMessage(message)) {
        MessageHistory.EnqueueItem(message);
        ContentRowManager.CreateContentRow(message);
      }
    }
  }
}