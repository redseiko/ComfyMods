namespace BetterZeeRouter;

public static class RoutedRPCDataExtensions {
  public static void DeserializeFrom(this ZRoutedRpc.RoutedRPCData routedRpcData, ref ZPackage sourcePackage) {
    routedRpcData.m_msgID = sourcePackage.ReadLong();
    routedRpcData.m_senderPeerID = sourcePackage.ReadLong();
    routedRpcData.m_targetPeerID = sourcePackage.ReadLong();
    routedRpcData.m_targetZDO = sourcePackage.ReadZDOID();
    routedRpcData.m_methodHash = sourcePackage.ReadInt();
    sourcePackage.ReadPackageTo(ref routedRpcData.m_parameters);
  }

  public static void Clear(this ZRoutedRpc.RoutedRPCData routedRpcData) {
    routedRpcData.m_msgID = default;
    routedRpcData.m_senderPeerID = default;
    routedRpcData.m_targetPeerID = default;
    routedRpcData.m_targetZDO = default;
    routedRpcData.m_methodHash = default;
    routedRpcData.m_parameters?.Clear();
  }
}
