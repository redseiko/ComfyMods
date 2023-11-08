using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BepInEx;

using BetterZeeRouter;

using HarmonyLib;

using UnityEngine;

namespace CriticalDice {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  [BepInDependency(BetterZeeRouter.BetterZeeRouter.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
  public class CriticalDice : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.criticaldice";
    public const string PluginName = "CriticalDice";
    public const string PluginVersion = "1.6.0";

    static readonly int _rpcSayHashCode = "Say".GetStableHashCode();
    static readonly SayHandler _sayHandler = new();

    static readonly Regex _htmlTagsRegex = new("<.*?>");
    static readonly System.Random _random = new();

    Harmony _harmony;

    public void Awake() {
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

      RoutedRpcManager.Instance.AddHandler(_rpcSayHashCode, _sayHandler);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    class SayHandler : RpcMethodHandler {
      const string _rollPrefix = "!roll";

      public override bool Process(ZRoutedRpc.RoutedRPCData routedRpcData) {
        routedRpcData.m_parameters.SetPos(0);

        // Talker.Type
        routedRpcData.m_parameters.ReadInt();

        string playerName = routedRpcData.m_parameters.ReadString();

        routedRpcData.m_parameters.ReadString(); // UserInfo.Gamertag
        routedRpcData.m_parameters.ReadString(); // UserInfo.NetworkUserId

        string messageText = routedRpcData.m_parameters.ReadString();

        routedRpcData.m_parameters.SetPos(0);

        if (messageText.StartsWith(_rollPrefix, StringComparison.Ordinal)) {
          ZNet.m_instance.StartCoroutine(ParseRpcSayDataCoroutine(playerName, messageText, routedRpcData.m_targetZDO));
        }

        return true;
      }
    }

    static readonly WaitForSeconds _waitInterval = new(seconds: 0.5f);

    static IEnumerator ParseRpcSayDataCoroutine(string playerName, string messageText, ZDOID targetZdo) {
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
          targetZdo,
          result,
          "<color=#AEC6D3><b>Server</b></color>",
          PrivilegeManager.GetNetworkUserId());
    }

    static void SendDiceRollResponse(
        string playerName, ZDOID targetZdoId, long result, string senderName, string networkUserId) {
      ZRoutedRpc.s_instance.InvokeRoutedRPC(
          ZRoutedRpc.Everybody,
          targetZdoId,
          "Say",
          (int) Talker.Type.Normal,
          new UserInfo() {
            Name = senderName,
            Gamertag = senderName,
            NetworkUserId = networkUserId
          },
          $"{playerName} rolled... {result}",
          networkUserId);
    }

    static readonly Regex _diceRollRegex =
        new(@"^!roll\s+(?:(?<simple>\d+)(?:\s+.*)?$|(?<count>\d*)d(?<faces>\d+)\s*(?<modifier>[\+-]\d+)?(?:\s+.*)?$)");

    static bool ParseDiceRoll(string input, out long result) {
      result = 0;

      MatchCollection matches = _diceRollRegex.Matches(input);

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
}