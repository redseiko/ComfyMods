namespace Parrot;

using BetterZeeRouter;

using static PluginConfig;

public sealed class SayHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("Say", _instance);
  }

  static readonly SayHandler _instance = new();

  SayHandler() {
    // ...
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    if (!AddServerToPlayerList.Value || routedRpcData.m_targetPeerID == 0) {
      return true;
    }

    if (routedRpcData.m_targetPeerID == ZNetManager.ServerPeerId) {
      routedRpcData.m_targetPeerID = 0;
      return true;
    }

    return false;
  }
}
