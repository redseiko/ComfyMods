namespace Parrot;

using System.Collections.Generic;

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

  public static readonly List<ZNetPeer> ParrotNetPeers = [];

  public static void RegisterParrotClient(ZNetPeer netPeer) {
    Parrot.LogInfo($"Registering new Parrot client: {netPeer.m_socket.GetHostName()}");

    ZNet.m_instance.m_peers.Add(netPeer);
    ParrotNetPeers.Add(netPeer);
    netPeer.m_rpc.Register<ZPackage>("ParrotLog", RPCParrotLog);
  }

  public static void RemoveParrotClient(ZNetPeer netPeer) {
    ParrotNetPeers.Remove(netPeer);
  }

  public static bool ConnectToParrotServer(string serverHost, int serverPort) {
    string serverHostPort = $"{serverHost}:{serverPort}";
    SteamNetworkingIPAddr serverAddress = new();

    if (!serverAddress.ParseString(serverHostPort)) {
      Parrot.LogError($"Could not parse Parrot server address: {serverHostPort}");
      return false;
    }

    Parrot.LogInfo($"Connecting to Parrot server at: {serverHostPort}");

    ZSteamSocket steamSocket = new(serverAddress);
    ZNetPeer netPeer = new(steamSocket, server: false);

    ZNet.m_instance.m_peers.Add(netPeer);
    ParrotNetPeers.Add(netPeer);
    netPeer.m_rpc.Register<ZPackage>("ParrotLog", RPCParrotLog);

    return true;
  }

  public static void SendParrotLog(string message) {
    if (ParrotNetPeers.Count <= 0) {
      return;
    }

    ZPackage package = new();
    package.Write(message);

    foreach (ZNetPeer netPeer in ParrotNetPeers) {
      netPeer.m_rpc.Invoke("ParrotLog", package);
    }
  }

  public static void RPCParrotLog(ZRpc rpc, ZPackage package) {
    string hostName = rpc.m_socket.GetHostName();
    string logMessage = package.ReadString();

    Parrot.LogInfo($"[RPC] [{hostName}] [ParrotLog] {logMessage}");
  }
}
