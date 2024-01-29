using System.Collections.Generic;

namespace Transporter {
  public static class PlayerUtils {
    public static readonly Dictionary<long, ZDO> PlayerZDOsByPlayerId = new();

    public static void RefreshPlayerIdMapping() {
      Dictionary<ZDOID, ZDO> zdosById = ZDOMan.s_instance.m_objectsByID;
      PlayerZDOsByPlayerId.Clear();

      foreach (ZNetPeer netPeer in ZNet.m_instance.m_peers) {
        if (netPeer.m_characterID == ZDOID.None
            || !zdosById.TryGetValue(netPeer.m_characterID, out ZDO playerZDO)
            || !ZDOExtraData.GetLong(playerZDO.m_uid, ZDOVars.s_playerID, out long playerId)
            || playerId == 0L) {
          continue;
        }

        PlayerZDOsByPlayerId[playerId] = playerZDO;
      }
    }

    public static bool TryGetPlayerZDO(long playerId, out ZDO playerZDO) {
      return PlayerZDOsByPlayerId.TryGetValue(playerId, out playerZDO);
    }

    public static bool TryGetPlayerId(ZDOID playerZDOID, out long playerId) {
      if (ZDOMan.s_instance.m_objectsByID.TryGetValue(playerZDOID, out ZDO playerZDO)
          && ZDOExtraData.GetLong(playerZDO.m_uid, ZDOVars.s_playerID, out playerId)
          && playerId != 0L) {
        return true;
      }

      playerId = default;
      return false;
    }

    public static List<ZDO> GetPlayerZDOs(List<long> playerIds) {
      Dictionary<ZDOID, ZDO> zdosById = ZDOMan.s_instance.m_objectsByID;
      HashSet<long> ids = new(playerIds);
      List<ZDO> zdos = new();

      foreach (ZNetPeer netPeer in ZNet.m_instance.m_peers) {
        if (netPeer.m_characterID == ZDOID.None
            || !zdosById.TryGetValue(netPeer.m_characterID, out ZDO zdo)
            || !ZDOExtraData.GetLong(zdo.m_uid, ZDOVars.s_playerID, out long playerId)
            || !ids.Contains(playerId)) {
          continue;
        }

        zdos.Add(zdo);
        ids.Remove(playerId);

        if (ids.Count <= 0) {
          break;
        }
      }

      return zdos;
    }
  }
}
