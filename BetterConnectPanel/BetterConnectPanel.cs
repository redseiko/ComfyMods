using BepInEx;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

using static BetterConnectPanel.PluginConfig;

namespace BetterConnectPanel {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class BetterConnectPanel : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.betterconnectpanel";
    public const string PluginName = "BetterConnectPanel";
    public const string PluginVersion = "1.2.0";

    Harmony _harmony;

    void Awake() {
      CreateConfig(Config);

      _isModEnabled.SettingChanged += (sender, eventArgs) => TogglePanels();

      _networkPanelPosition.SettingChanged +=
          (sender, eventArgs) => _networkPanel?.SetPosition(_networkPanelPosition.Value);

      _networkPanelFontSize.SettingChanged +=
          (sender, eventArgs) => _networkPanel?.SetTextFontSize(_networkPanelFontSize.Value);

      _networkPanelBackgroundColor.SettingChanged +=
          (sender, eventArgs) => _networkPanel?.SetBackgroundColor(_networkPanelBackgroundColor.Value);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    [HarmonyPatch(typeof(ConnectPanel))]
    class ConnectPanelPatch {
      static readonly CodeMatch _inputGetKeyDownMatch =
          new(
              OpCodes.Call,
              AccessTools.Method(typeof(Input), nameof(Input.GetKeyDown), new Type[] { typeof(KeyCode) }));

      [HarmonyTranspiler]
      [HarmonyPatch(nameof(ConnectPanel.Update))]
      static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
        return new CodeMatcher(instructions)
            .MatchForward(useEnd: false, _inputGetKeyDownMatch)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(UpdateInputGetKeyDownDelegate).operand)
            .InstructionEnumeration();
      }

      static bool UpdateInputGetKeyDownDelegate(KeyCode keyCode) {
        if (_isModEnabled.Value && _networkPanelToggleShortcut.Value.IsDown()) {
          _isNetworkPanelEnabled = !_isNetworkPanelEnabled;
          TogglePanels();

          if (_networkPanelToggleShortcut.Value.MainKey == KeyCode.F2) {
            return false;
          }
        }

        return Input.GetKeyDown(KeyCode.F2);
      }
    }

    [HarmonyPatch(typeof(Hud))]
    class HudPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Hud.Awake))]
      static void AwakePostfix() {
        TogglePanels();
      }
    }

    static TwoColumnPanel _networkPanel;
    static TwoColumnPanel.PanelRow _serverFieldRow;
    static TwoColumnPanel.PanelRow _serverWorldNameRow;
    static TwoColumnPanel.PanelRow _connectionPingRow;
    static TwoColumnPanel.PanelRow _connectionQualityRow;
    static TwoColumnPanel.PanelRow _connectionOutRateRow;
    static TwoColumnPanel.PanelRow _connectionInRateRow;
    static TwoColumnPanel.PanelRow _conectionPendingReliableRow;
    static TwoColumnPanel.PanelRow _conectionSentUnackedReliableRow;
    static TwoColumnPanel.PanelRow _connectionEstimatedSendQueueTimeRow;
    static TwoColumnPanel.PanelRow _netTimeRow;

    static bool _isNetworkPanelEnabled = true;

    static void TogglePanels() {
      _networkPanel?.DestroyPanel();

      if (_isModEnabled.Value && Hud.instance && _isNetworkPanelEnabled) {
        CreateNetworkPanel(Hud.instance);
      }
    }

    static void CreateNetworkPanel(Hud hud) {
      _networkPanel =
          new TwoColumnPanel("NetworkPanel", hud.transform, hud.m_hoverName.font, _networkPanelFontSize.Value)
              .AddPanelRow("Server Host", "ServerAddress", out _serverFieldRow)
              .AddPanelRow("Server World", "WorldName", out _serverWorldNameRow)
              .AddPanelRow("Ping", "ConnectionPing", out _connectionPingRow)
              .AddPanelRow("Quality (L/R)", "ConnectionQuality", out _connectionQualityRow)
              .AddPanelRow("Send Rate", "OutRate", out _connectionOutRateRow)
              .AddPanelRow("Receive Rate", "InRate", out _connectionInRateRow)
              .AddPanelRow("Pending Send", "PendingReliable", out _conectionPendingReliableRow)
              .AddPanelRow("Unacked Send", "SentUnackedReliable", out _conectionSentUnackedReliableRow)
              .AddPanelRow("Est. Send Queue Time", "EstimatedSendQueueTIme", out _connectionEstimatedSendQueueTimeRow)
              .AddPanelRow("NetTime", "NetTime", out _netTimeRow)
              .SetPosition(_networkPanelPosition.Value)
              .SetBackgroundColor(_networkPanelBackgroundColor.Value)
              .SetActive(true);

      if (_networkPanel.Panel.TryGetComponent(out RectTransform transform)) {
        transform.anchorMin = new Vector2(0f, 0.5f);
        transform.anchorMax = new Vector2(0f, 0.5f);
        transform.pivot = new Vector2(0f, 0.5f);
      }

      hud.StartCoroutine(UpdateNetworkPanelCoroutine());
    }

    static IEnumerator UpdateNetworkPanelCoroutine() {
      WaitForSeconds waitInterval = new(seconds: 0.1f);
      ZSteamSocket serverSocket = null;

      while (serverSocket == null) {
        yield return waitInterval;
        serverSocket = (ZSteamSocket) ZNet.instance?.GetServerPeer()?.m_socket;
      }

      while (ZNet.m_world == null) {
        yield return waitInterval;
      }

      ZNet.m_serverIPAddr.ToString(out string serverAddress, true);
      _serverFieldRow.RightText.text = $"<color=#F7DC6F>{serverAddress}</color>";
      _serverWorldNameRow.RightText.text = $"<color=#F7DC6F>{ZNet.instance.GetWorldName()}</color>";

      while (true) {
        yield return waitInterval;

        _netTimeRow.RightText.text = $"<color=#F7DC6F>{ZNet.instance.m_netTime}</color>";

        UpdateConnectionStatusProperties(serverSocket);
      }
    }

    static void UpdateConnectionStatusProperties(ZSteamSocket serverSocket) {
      if (!SteamNetworkingSockets.GetQuickConnectionStatus(
          serverSocket.m_con, out SteamNetworkingQuickConnectionStatus status)) {
        return;
      }

      _connectionPingRow.RightText.text = $"<color=#F7DC6F>{status.m_nPing:N0}</color> ms";

      _connectionQualityRow.RightText.text = string.Format(
          "<color={0}>{1:0%}</color> / <color={0}>{2:0%}</color>",
          "#F7DC6F",
          status.m_flConnectionQualityLocal,
          status.m_flConnectionQualityRemote);

      _connectionOutRateRow.RightText.text =
          $"<color=#F7DC6F>{status.m_flOutBytesPerSec / 1024:N2}</color> KB/s";

      _connectionInRateRow.RightText.text =
          $"<color=#F7DC6F>{status.m_flInBytesPerSec / 1024:N2}</color> KB/s";

      _conectionPendingReliableRow.RightText.text = $"<color=#F7DC6F>{status.m_cbPendingReliable:N0}</color> B";

      _conectionSentUnackedReliableRow.RightText.text =
          $"<color=#F7DC6F>{status.m_cbSentUnackedReliable:N0}</color> B";

      _connectionEstimatedSendQueueTimeRow.RightText.text =
          $"<color=#F7DC6F>{status.m_usecQueueTime.m_SteamNetworkingMicroseconds / 1024:N0}</color> ms";
    }
  }
}