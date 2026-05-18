namespace Atlas;

using System.Reflection;

using BepInEx;

using HarmonyLib;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class Atlas : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.atlas";
  public const string PluginName = "Atlas";
  public const string PluginVersion = "1.16.0";

  public const int TimeCreatedHash = -1420903867;       // timeCreated
  public const int EpochTimeCreatedHash = 1272608570;   // epochTimeCreated
  public const int OriginalUidUserIdHash = 1307602051;  // originalUid_u
  public const int OriginalUidIdHash = 1307602055;      // originalUid_i

  void Awake() {
    PluginLogger.BindLogger(Logger);
    PluginConfig.BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
  }
}
