namespace FabulousSteam;

using System.Collections.Generic;

public static class BackendManager {
  public static ZSteamSocket SteamHostSocket { get => ZSteamSocket.m_hostSocket; }
  public static ZPlayFabSocket PlayFabHostSocket { get => ZPlayFabSocket.s_listenSocket; }

  public static void OpenServers(ZNet net) {
    ZNet.m_openServer = true;

    bool hasPassword = !string.IsNullOrEmpty(ZNet.m_serverPassword);
    List<string> startingGlobalKeys = ZNet.m_world.m_startingGlobalKeys;
    string worldName = ZNet.m_world.m_seedName;

    StartSteamServer(net, hasPassword, startingGlobalKeys, worldName);
    StartPlayFabServer(hasPassword, startingGlobalKeys, worldName);

    net.m_hostSocket = ZSteamSocket.m_hostSocket;
  }

  static void StartSteamServer(ZNet net, bool hasPassword, List<string> startingGlobalKeys, string worldName) {
    ZSteamMatchmaking.instance.RegisterServer(
        ZNet.m_ServerName,
        hasPassword,
        Version.CurrentVersion,
        startingGlobalKeys,
        Version.m_networkVersion,
        ZNet.m_publicServer,
        worldName,
        net.OnSteamServerRegistered);

    ZSteamSocket steamSocket = new ZSteamSocket();
    steamSocket.StartHost();

    FabulousSteam.LogInfo($"Opened Steam server.");
  }

  static void StartPlayFabServer(bool hasPassword, List<string> startingGlobalKeys, string worldName) {
    ZPlayFabMatchmaking.instance.RegisterServer(
        ZNet.m_ServerName,
        hasPassword,
        ZNet.m_publicServer,
        Version.CurrentVersion,
        startingGlobalKeys,
        Version.m_networkVersion,
        worldName,
        needServerAccount: true);

    ZPlayFabSocket playFabSocket = new();
    playFabSocket.StartHost();

    FabulousSteam.LogInfo($"Opened PlayFab server.");
  }

  public static void StopServers(ZNet net) {
    ZSteamSocket.m_hostSocket?.Dispose();
    ZPlayFabSocket.s_listenSocket?.Dispose();

    net.m_hostSocket = default;
  }

  public static void ProcessIncomingServerConnections(ZNet net) {
    ProcessIncomingServerConnection(net, SteamHostSocket, OnlineBackendType.Steamworks);
    ProcessIncomingServerConnection(net, PlayFabHostSocket, OnlineBackendType.PlayFab);
  }

  static void ProcessIncomingServerConnection(ZNet net, ISocket hostSocket, OnlineBackendType backendType) {
    if (hostSocket == default) {
      return;
    }

    ISocket socket = hostSocket.Accept();

    if (socket == default) {
      return;
    }

    if (!socket.IsConnected()) {
      socket.Dispose();
      return;
    }

    FabulousSteam.LogInfo($"Incoming connection ({backendType:F}): {socket.GetHostName()}");

    ZNetPeer netPeer = new(socket, server: false);
    net.OnNewConnection(netPeer);
  }
}
