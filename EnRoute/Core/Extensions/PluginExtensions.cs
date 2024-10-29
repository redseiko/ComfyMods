namespace EnRoute;

public static class PluginExtensions {
  public static bool IsSectorInRange(this Vector2i source, Vector2i target, int range) {
    return target.x + range >= source.x - range
        && target.x - range <= source.x + range
        && target.y + range >= source.y - range
        && target.y - range <= source.y + range;
  }
}

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
