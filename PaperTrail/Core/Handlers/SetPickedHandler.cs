namespace PaperTrail;

using BetterZeeRouter;

public sealed class SetPickedHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("RPC_SetPicked", _instance);
  }

  static readonly SetPickedHandler _instance = new();

  SetPickedHandler() {
    // ...
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    ZPackage parameters = routedRpcData.m_parameters;
    parameters.SetPos(0);
    bool picked = parameters.ReadBool();
    parameters.SetPos(0);

    PickableManager.LogRPCSetPicked(
        routedRpcData.m_senderPeerID, routedRpcData.m_targetPeerID, routedRpcData.m_targetZDO, picked);

    return true;
  }
}
