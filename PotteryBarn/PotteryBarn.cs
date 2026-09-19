namespace PotteryBarn;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using ComfyLib;

using HarmonyLib;

using Jotunn.Managers;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
public sealed class PotteryBarn : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.potterybarn";
  public const string PluginName = "PotteryBarn";
  public const string PluginVersion = "1.23.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;
    BindConfig(Config);

    PieceManager.OnPiecesRegistered += PotteryManager.AddPieces;

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    Chat.m_instance.AddMessage(obj);
  }
}
