namespace BetterZeeRouter;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public sealed class RoutedRpcManager {
  RoutedRpcManager() {}

  static readonly Lazy<RoutedRpcManager> _lazy = new(() => new(), isThreadSafe: true);
  public static RoutedRpcManager Instance { get { return _lazy.Value; } }

  readonly ConcurrentDictionary<int, Lazy<List<RpcMethodHandler>>> _rpcMethodHandlers = new();

  public void AddHandler(int methodHashCode, RpcMethodHandler handler) {
    BetterZeeRouter.LogInfo($"Adding handler: {handler.GetType().FullName}");

    List<RpcMethodHandler> handlers =
        _rpcMethodHandlers.GetOrAdd(methodHashCode, _ => new Lazy<List<RpcMethodHandler>>()).Value;

    handlers.Add(handler);
  }

  public bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    if (!_rpcMethodHandlers.TryGetValue(routedRpcData.m_methodHash, out Lazy<List<RpcMethodHandler>> handlers)) {
      return true;
    }

    bool result = true;

    foreach (RpcMethodHandler handler in handlers.Value) {
      result &= handler.Process(routedRpcData);
    }

    return result;
  }
}
