namespace BetterZeeRouter;

using System.Collections.Generic;

public static class RoutedRpcManager {
  public static readonly Dictionary<int, string> HashCodeToMethodNameCache = [];

  static readonly Dictionary<int, List<RpcMethodHandler>> _rpcMethodHandlers = [];
  static readonly ZRoutedRpc.RoutedRPCData _routedRpcData = new();

  public static long ServerPeerId { get => _serverPeerId; }
  static long _serverPeerId = 0L;

  public static void SetupServerPeer() {
    _serverPeerId = ZDOMan.s_instance.m_sessionID;
  }

  public static void AddHandler(string methodName, RpcMethodHandler handler) {
    int methodHashCode = methodName.GetStableHashCode();
    HashCodeToMethodNameCache[methodHashCode] = methodName;

    BetterZeeRouter.LogInfo($"Adding handler for {methodName} ({methodHashCode}): {handler.GetType().FullName}");

    if (!_rpcMethodHandlers.TryGetValue(methodHashCode, out List<RpcMethodHandler> handlers)) {
      handlers = [];
      _rpcMethodHandlers[methodHashCode] = handlers;
    }

    handlers.Add(handler);
  }

  public static void ProcessRoutedRPC(ZRoutedRpc routedRpc, ZRpc rpc, ZPackage package) {
    _routedRpcData.DeserializeFrom(ref package);

    long targetPeerId = _routedRpcData.m_targetPeerID;
    long routedRpcId = routedRpc.m_id;

    if (targetPeerId == routedRpcId || targetPeerId == 0L) {
      routedRpc.HandleRoutedRPC(_routedRpcData);
    }

    if ((targetPeerId != routedRpcId || targetPeerId == _serverPeerId) && ProcessHandlers(_routedRpcData)) {
      routedRpc.RouteRPC(_routedRpcData);
    }
  }

  public static bool ProcessHandlers(ZRoutedRpc.RoutedRPCData routedRpcData) {
    if (!_rpcMethodHandlers.TryGetValue(routedRpcData.m_methodHash, out List<RpcMethodHandler> handlers)) {
      return true;
    }

    bool result = true;

    foreach (RpcMethodHandler handler in handlers) {
      result &= handler.Process(routedRpcData);
    }

    return result;
  }

  public static string MethodHashToString(int methodHash) {
    if (HashCodeToMethodNameCache.TryGetValue(methodHash, out string methodName)) {
      return methodName;
    }

    return $"RPC_{methodHash}";
  }
}
