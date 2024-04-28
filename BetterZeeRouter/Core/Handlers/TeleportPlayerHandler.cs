namespace BetterZeeRouter;

using System;
using System.IO;

public sealed class TeleportPlayerHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("RPC_TeleportTo", _instance);
    RoutedRpcManager.AddHandler("RPC_TeleportPlayer", _instance);
  }

  static readonly TeleportPlayerHandler _instance = new();

  readonly SyncedList _teleportPlayerAccess;
  readonly StreamWriter _teleportPlayerLog;

  TeleportPlayerHandler() {
    _teleportPlayerAccess =
        new(
            Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), "TeleportPlayerAccess.txt"),
            "Allowed to send RPC_TeleportPlayer/RPC_TeleportTo RPCs.");

    _teleportPlayerLog =
        File.AppendText(Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), "TeleportPlayerLog.txt"));
    _teleportPlayerLog.AutoFlush = true;
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    long senderId = routedRpcData.m_senderPeerID;
    long targetId = routedRpcData.m_targetPeerID;
    string rpcMethod = MethodHashToString(routedRpcData.m_methodHash);
    bool isPermitted = IsPermitted(senderId);

    _teleportPlayerLog.WriteLine($"{timestamp},{senderId},{targetId},{rpcMethod},{isPermitted}");
    _teleportPlayerLog.Flush();

    BetterZeeRouter.LogInfo($"{rpcMethod} sent from {senderId} targeting {targetId}, permitted: {isPermitted}");

    return isPermitted;
  }

  bool IsPermitted(long peerId) {
    foreach (ZNetPeer peer in ZNet.m_instance.m_peers) {
      if (peer.m_uid == peerId) {
        string steamId = peer.m_socket.GetHostName();
        return !string.IsNullOrWhiteSpace(steamId) && _teleportPlayerAccess.Contains(steamId);
      }
    }

    return false;
  }

  static string MethodHashToString(int methodHash) {
    if (RoutedRpcManager.HashCodeToMethodNameCache.TryGetValue(methodHash, out string methodName)) {
      return methodName;
    }

    return $"RPC_{methodHash}";
  }
}
