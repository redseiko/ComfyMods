namespace PaperTrail;

using BetterZeeRouter;

public sealed class PickHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("RPC_Pick", _instance);
  }

  static readonly PickHandler _instance = new();

  PickHandler() {
    PickableManager.Initialize();
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    PickableManager.LogRPCPick(
        routedRpcData.m_senderPeerID, routedRpcData.m_targetPeerID, routedRpcData.m_targetZDO);

    return true;
  }
}
