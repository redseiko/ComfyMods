namespace Parrot;

using UnityEngine;

public static class ChatManager {
  public static bool ProcessChatMessage(ZRoutedRpc.RoutedRPCData routedRpcData) {
    ChatMessageParams chatMessageParams = new(routedRpcData.m_parameters);

    if (chatMessageParams.TalkerType == Talker.Type.Shout) {
      Parrot.LogInfo(chatMessageParams.ToLogString());
    }

    return true;
  }
}

public sealed class ChatMessageParams {
  public Vector3 PlayerPosition { get; }
  public Talker.Type TalkerType { get; }
  public string PlayerName { get; }
  public string PlayerUserId { get; }
  public string MessageText { get; }

  public ChatMessageParams(ZPackage parameters) {
    parameters.SetPos(0);

    PlayerPosition = parameters.ReadVector3();
    TalkerType = (Talker.Type) parameters.ReadInt();
    PlayerName = parameters.ReadString();
    PlayerUserId = parameters.ReadString();
    MessageText = parameters.ReadString();

    parameters.SetPos(0);
  }

  public string ToLogString() {
    return $"{PlayerName} ({PlayerUserId}): {TalkerType:F}: {MessageText}";
  }
}
