namespace BetterZeeRouter;

public static class ZPackageExtensions {
  public static void ReadPackageTo(this ZPackage sourcePackage, ref ZPackage targetPackage) {
    int count = sourcePackage.m_reader.ReadInt32();

    targetPackage.m_writer.Flush();
    targetPackage.m_stream.SetLength(count);
    targetPackage.m_stream.Position = 0L;

    sourcePackage.m_reader.Read(targetPackage.m_stream.GetBuffer(), 0, count);
  }
}
