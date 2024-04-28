namespace Parrot;

using BetterZeeRouter;

using UnityEngine;

using static PluginConfig;

public sealed class ChatMessageHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("ChatMessage", _instance);
  }

  static readonly ChatMessageHandler _instance = new();

  ChatMessageHandler() {
    // ...
  }

  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    ZPackage parameters = routedRpcData.m_parameters;
    parameters.SetPos(0);

    Vector3 targetPosition = parameters.ReadVector3();
    int messageType = routedRpcData.m_parameters.ReadInt();
    string userInfoName = parameters.ReadString();
    string userInfoGamertag = parameters.ReadString();
    string userInfoNetworkUserId = parameters.ReadString();
    string messageText = parameters.ReadString();
    string senderNetworkUserId = parameters.ReadString();

    // ChatUtils.LogChatMessage();

    if (SuppressChatMessageGamertag.Value) {
      userInfoGamertag = string.Empty;
      userInfoNetworkUserId = string.Empty;
      senderNetworkUserId = string.Empty;
    }

    parameters.Clear();
    parameters.Write(targetPosition);
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
