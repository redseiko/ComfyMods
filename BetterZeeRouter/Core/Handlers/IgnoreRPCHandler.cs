namespace BetterZeeRouter;

using System.Collections.Generic;

public sealed class IgnoreRPCHandler : RpcMethodHandler {
  public static void Register(IEnumerable<string> methodNamesToIgnore) {
    foreach (string methodName in methodNamesToIgnore) {
      RoutedRpcManager.AddHandler(methodName, _instance);
    }
  }

  static readonly IgnoreRPCHandler _instance = new();

  IgnoreRPCHandler() {
    // ...
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    return false;
  }
}
