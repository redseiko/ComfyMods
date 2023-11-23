namespace ComfyLib {
  public static class ZPackageExtensions {
    public static ZRoutedRpc.RoutedRPCData ReadRoutedRPCData(this ZPackage sourcePackage) {
      ZRoutedRpc.RoutedRPCData rpcData = new() {
        m_msgID = sourcePackage.ReadLong(),
        m_senderPeerID = sourcePackage.ReadLong(),
        m_targetPeerID = sourcePackage.ReadLong(),
        m_targetZDO = sourcePackage.ReadZDOID(),
        m_methodHash = sourcePackage.ReadInt()
      };

      rpcData.m_parameters.ReadPackageFrom(sourcePackage);

      return rpcData;
    }

    public static void ReadPackageFrom(this ZPackage package, ZPackage sourcePackage) {
      int count = sourcePackage.m_reader.ReadInt32();

      package.m_writer.Flush();
      package.m_stream.SetLength(count);
      package.m_stream.Position = 0L;

      sourcePackage.m_reader.Read(package.m_stream.GetBuffer(), 0, count);
    }
  }
}
