namespace BetterZeeRouter;

public sealed class HealthChangedHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("RPC_HealthChanged", _instance);
  }

  static readonly HealthChangedHandler _instance = new();

  HealthChangedHandler() {
    // ...
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    return false;
  }
}
