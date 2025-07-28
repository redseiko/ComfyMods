namespace PaperTrail;

using BetterZeeRouter;

public sealed class PickHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("RPC_Pick", _instance);
  }

  static readonly PickHandler _instance = new();

  PickHandler() {
    // ...
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    ZPackage parameters = routedRpcData.m_parameters;
    parameters.SetPos(0);
    int bonus = parameters.ReadInt();
    parameters.SetPos(0);

    PickableManager.LogRPCPick(
        routedRpcData.m_senderPeerID, routedRpcData.m_targetPeerID, routedRpcData.m_targetZDO, bonus);

    return true;
  }
}
