using System.Collections.Generic;

namespace HoldMyMead {
  public static class PlayerDeathManager {
    public static readonly int OnDeathHashCode = "OnDeath".GetStableHashCode();

    public static void ProcessOnDeathRPC(ZRoutedRpc.RoutedRPCData rpcData) {
      string logMessage = $"Player.OnDeath() RPC from: {rpcData.m_senderPeerID}, ZDO: {rpcData.m_targetZDO}";

      if (TryGetPlayerInfo(rpcData.m_senderPeerID, out ZNet.PlayerInfo playerInfo)) {
        logMessage += $" ... {playerInfo.m_name} ({playerInfo.m_host}), position: {playerInfo.m_position}";
      }

      HoldMyMead.LogInfo(logMessage);
    }

    public static bool TryGetPlayerInfo(long peerId, out ZNet.PlayerInfo playerInfo) {
      List<ZNet.PlayerInfo> playerInfos = ZNet.m_instance.m_players;

      for (int i = 0, count = playerInfos.Count; i < count; i++) {
        playerInfo = playerInfos[i];

        if (playerInfo.m_characterID.UserID == peerId) {
          return true;
        }
      }

      playerInfo = default;
      return false;
    }
  }
}
