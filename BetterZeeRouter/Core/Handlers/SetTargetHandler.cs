namespace BetterZeeRouter;

using UnityEngine;

using static PluginConfig;

public sealed class SetTargetHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("RPC_SetTarget", _instance);
  }

  static readonly SetTargetHandler _instance = new();

  SetTargetHandler() {
    // ...
  }

  public static readonly int PlayerHashCode = "Player".GetStableHashCode();

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    ZPackage parameters = routedRpcData.m_parameters;

    parameters.SetPos(0);
    ZDOID targetZDOID = parameters.ReadZDOID();
    parameters.SetPos(0);

    if (targetZDOID == ZDOID.None
        || !ZDOMan.s_instance.m_objectsByID.TryGetValue(targetZDOID, out ZDO targetZDO)) {
      return true;
    }

    if (SetTargetHandlerShouldCheckDistance.Value
        && Utils.DistanceXZ(Vector3.zero, targetZDO.m_position) > SetTargetHandlerDistanceCheckRange.Value) {
      return true;
    }

    if (targetZDO.m_prefab == PlayerHashCode || targetZDO.GetBool(ZDOVars.s_tamed)) {
      parameters.Clear();
      parameters.Write(ZDOID.None);
      parameters.m_writer.Flush();
      parameters.m_stream.Flush();
      parameters.SetPos(0);

      // TODO(redseiko): temporarily needed for RouteRPC to force it to send back to the original sender.
      // Proper fix is to just provide a means of indicating this should be routed to all clients.
      routedRpcData.m_senderPeerID = ZRoutedRpc.s_instance.m_id;
    }

    return true;
  }
}
