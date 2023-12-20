using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;

using HarmonyLib;

using static LicenseToSkill.PluginConfig;

namespace LicenseToSkill {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class LicenseToSkill : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.comfytools.licensetoskill";
    public const string PluginName = "LicenseToSkill";
    public const string PluginVersion = "1.2.1";

    Harmony _harmony;

    public const float DefaultHardDeathCooldown = 600f;

    public void Awake() {
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static void SetHardDeathCoolDown() {
      if (!Player.m_localPlayer) {
        return;
      }

      UpdateHardDeathCooldownTimer();

      if (IsModEnabled.Value) {
        Player.m_localPlayer.m_hardDeathCooldown = HardDeathCooldownOverride.Value;
        return;
      }

      Player.m_localPlayer.m_hardDeathCooldown = DefaultHardDeathCooldown;
    }

    static void UpdateHardDeathCooldownTimer() {
      if (!Player.m_localPlayer || Player.m_localPlayer.m_seman == null) {
        return;
      }

      StatusEffect hardDeathCooldown = Player.m_localPlayer.m_seman.m_statusEffects.Where(x => x.NameHash() == Player.s_statusEffectSoftDeath).FirstOrDefault();

      if (hardDeathCooldown == null) {
        return;
      }

      hardDeathCooldown.m_ttl = HardDeathCooldownOverride.Value * 60f - Player.m_localPlayer.m_timeSinceDeath;
    }
  }
}