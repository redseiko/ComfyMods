namespace ComfyLib;

public static class RoutedRPCDataExtensions {
  public static void WriteToPackage(this ZRoutedRpc.RoutedRPCData rpcData, ZPackage package) {
    package.Write(rpcData.m_msgID);
    package.Write(rpcData.m_senderPeerID);
    package.Write(rpcData.m_targetPeerID);
    package.Write(rpcData.m_targetZDO);
    package.Write(rpcData.m_methodHash);

    int size = rpcData.m_parameters.Size();
    package.Write(size);
    package.m_stream.Write(rpcData.m_parameters.m_stream.GetBuffer(), 0, size);
  }
}
