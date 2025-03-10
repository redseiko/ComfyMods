namespace Parrot;

using BetterZeeRouter;

public sealed class SayHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("Say", _instance);
  }

  static readonly SayHandler _instance = new();

  SayHandler() {
    // ...
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    return true;
  }
}
