namespace Parrot;

using BetterZeeRouter;

using static PluginConfig;

public sealed class SayHandler : RpcMethodHandler {
  public static readonly int SayHashCode = "Say".GetStableHashCode();

  public static void Register(RoutedRpcManager rpcManager) {
    rpcManager.AddHandler(SayHashCode, _instance);
  }

  static readonly SayHandler _instance = new();

  SayHandler() {
    // ...
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    ZPackage parameters = routedRpcData.m_parameters;
    parameters.SetPos(0);

    int messageType = routedRpcData.m_parameters.ReadInt();
    string userInfoName = parameters.ReadString();
    string userInfoGamertag = parameters.ReadString();
    string userInfoNetworkUserId = parameters.ReadString();
    string messageText = parameters.ReadString();
    string senderNetworkUserId = parameters.ReadString();

    // ChatUtils.LogChatMessage();

    if (SuppressSayGamertag.Value) {
      userInfoGamertag = string.Empty;
      userInfoNetworkUserId = string.Empty;
      senderNetworkUserId = string.Empty;
    }

    parameters.Clear();
    parameters.Write(messageType);
    parameters.Write(userInfoName);
    parameters.Write(userInfoGamertag);
    parameters.Write(userInfoNetworkUserId);
    parameters.Write(messageText);
    parameters.Write(senderNetworkUserId);

    parameters.m_writer.Flush();
    parameters.m_stream.Flush();

    parameters.SetPos(0);

    return true;
  }
}
