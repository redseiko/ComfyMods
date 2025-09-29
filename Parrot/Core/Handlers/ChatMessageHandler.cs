namespace Parrot;

using System;

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
    if (!AddServerToPlayerList.Value) {
      return true;
    }

    if (routedRpcData.m_targetPeerID == 0) {
      return ChatManager.ProcessChatMessage(routedRpcData);
    }

    if (routedRpcData.m_targetPeerID == ZNetManager.ServerPeerId) {
      routedRpcData.m_targetPeerID = 0;
      return ChatManager.ProcessChatMessage(routedRpcData);
    }

    return false;
  }
}
