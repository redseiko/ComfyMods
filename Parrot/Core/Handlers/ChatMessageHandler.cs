namespace Parrot;

using BetterZeeRouter;

public sealed class ChatMessageHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("ChatMessage", _instance);
  }

  static readonly ChatMessageHandler _instance = new();

  ChatMessageHandler() {
    // ...
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    return true;
  }
}
