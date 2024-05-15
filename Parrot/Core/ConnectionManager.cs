namespace Parrot;

using Steamworks;

public static class ConnectionManager {
  public static bool IsSteamGameServer(ZNetPeer netPeer) {
    ZSteamSocket steamSocket = (ZSteamSocket) netPeer.m_socket;

    if (steamSocket == null) {
      return false;
    }

    CSteamID steamId = steamSocket.m_peerID.GetSteamID();

    return steamId.GetEAccountType() switch {
      EAccountType.k_EAccountTypeAnonGameServer => true,
      EAccountType.k_EAccountTypeGameServer => true,
      _ => false,
    };
  }
}
