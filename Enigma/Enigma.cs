using BepInEx;

using HarmonyLib;

using System.Reflection;

using static Enigma.PluginConfig;

namespace Enigma {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class Enigma : BaseUnityPlugin {
    public const string PluginGuid = "bruce.valheim.enigma";
    public const string PluginName = "Enigma";
    public const string PluginVersion = "1.1.0";

    Harmony _harmony;

    public static readonly string CustomNameFieldName = "customEnemyName";
    public static readonly string HasSeenFieldName = "hasSeenEnemy";
    public static readonly string BossDesignationFieldName = "isCustomBoss";

    public void Awake() {
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }
  }
}