namespace BetterZeeRouter;

public sealed class DamageTextHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("DamageText", _instance);
  }

  static readonly DamageTextHandler _instance = new();

  DamageTextHandler() {
    // ...
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    return false;
  }
}
