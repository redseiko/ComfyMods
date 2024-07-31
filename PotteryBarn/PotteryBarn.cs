namespace PotteryBarn;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using Jotunn.Managers;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
public sealed class PotteryBarn : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.potterybarn";
  public const string PluginName = "PotteryBarn";
  public const string PluginVersion = "1.17.0";

  void Awake() {
    BindConfig(Config);
    PieceManager.OnPiecesRegistered += PotteryManager.AddPieces;

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
