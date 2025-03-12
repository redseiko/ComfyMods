namespace CriticalDice;

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BetterZeeRouter;

using Steamworks;

using UnityEngine;

public sealed class SayHandler : RpcMethodHandler {
  public static void Register() {
    RoutedRpcManager.AddHandler("Say", _instance);
  }

  static readonly SayHandler _instance = new();

  SayHandler() {
    // ...
  }
  
  public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
    if (routedRpcData.m_targetPeerID != 0L && routedRpcData.m_targetPeerID != RoutedRpcManager.ServerPeerId) {
      return true;
    }

    ZPackage parameters = routedRpcData.m_parameters;
    parameters.SetPos(0);

    parameters.ReadInt();                         // Talker.Type
    string playerName = parameters.ReadString();  // UserInfo.Name
    parameters.ReadString();                      // UserInfo.UserId
    string messageText = parameters.ReadString();

    parameters.SetPos(0);

    if (messageText.Length >= 5 && messageText.StartsWith("!roll", StringComparison.Ordinal)) {
      ZNet.m_instance.StartCoroutine(ParseSayDataCoroutine(playerName, messageText, routedRpcData.m_targetZDO));
    }

    return true;
  }

  static readonly WaitForSeconds _waitInterval = new(seconds: 0.5f);
  static readonly Regex _htmlTagsRegex = new("<.*?>");
  static readonly System.Random _random = new();

  static IEnumerator ParseSayDataCoroutine(string playerName, string messageText, ZDOID targetZDOID) {
    yield return _waitInterval;

    long result = 0L;
    Task<bool> task = Task.Run(() => ParseDiceRoll(messageText, out result));

    while (!task.IsCompleted) {
      yield return null;
    }

    if (task.IsFaulted || !task.Result) {
      yield break;
    }

    SendDiceRollResponse(
        _htmlTagsRegex.Replace(playerName, string.Empty),
        targetZDOID,
        result,
        "<color=#aec6d3><b>Server</b></color>");
  }

  static void SendDiceRollResponse(
      string playerName, ZDOID targetZDOID, long result, string senderName) {
    ZRoutedRpc.s_instance.InvokeRoutedRPC(
        ZRoutedRpc.Everybody,
        targetZDOID,
        "Say",
        (int) Talker.Type.Normal,
        new UserInfo() {
          Name = senderName,
          UserId = new(ZNet.m_instance.m_steamPlatform, SteamGameServer.GetSteamID().ToString())
        },
        $"{playerName} rolled... {result}");
  }

  public static readonly Regex DiceRollRegex =
      new(@"^!roll\s+(?:(?<simple>\d+)(?:\s+.*)?$|(?<count>\d*)d(?<faces>\d+)\s*(?<modifier>[\+-]\d+)?(?:\s+.*)?$)");

  static bool ParseDiceRoll(string input, out long result) {
    result = 0;

    MatchCollection matches = DiceRollRegex.Matches(input);

    if (matches.Count <= 0) {
      return false;
    }

    Match match = matches[0];

    if (match.Groups["simple"].Length > 0) {
      if (!int.TryParse(match.Groups["simple"].Value, out int simple) || simple < 2) {
        return false;
      }

      result += _random.Next(simple) + 1;
      return true;
    }

    if (!int.TryParse(match.Groups["count"].Value, out int diceCount)) {
      diceCount = 1;
    }

    if (!int.TryParse(match.Groups["faces"].Value, out int diceFaces) || diceFaces < 2) {
      return false;
    }

    if (int.TryParse(match.Groups["modifier"].Value, out int modifier)) {
      result += modifier;
    }

    diceCount = Math.Min(diceCount, 20);
    diceFaces = Math.Min(diceFaces, 1000);

    for (int i = 0; i < diceCount; i++) {
      result += _random.Next(diceFaces) + 1;
    }

    return true;
  }
}
