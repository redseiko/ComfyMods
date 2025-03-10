namespace Atlas;

using System.Collections.Generic;
using System.Reflection;

using BepInEx;

using HarmonyLib;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class Atlas : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.atlas";
  public const string PluginName = "Atlas";
  public const string PluginVersion = "1.15.0";

  public static readonly int TimeCreatedHashCode = "timeCreated".GetStableHashCode();
  public static readonly int EpochTimeCreatedHashCode = "epochTimeCreated".GetStableHashCode();
  public static readonly KeyValuePair<int, int> OriginalUidHashPair = ZDO.GetHashZDOID("originalUid");

  void Awake() {
    PluginLogger.BindLogger(Logger);
    PluginConfig.BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
  }
}
