using HarmonyLib;

using System;

using static Enigma.Enigma;
using static Enigma.PluginConfig;

namespace Enigma.Patches {
  [HarmonyPatch(typeof(EnemyHud))]
  public class EnemyHudPatch {

    [HarmonyPostfix]
    [HarmonyPatch(nameof(EnemyHud.ShowHud))]
    public static void ShowHudPostfix(ref EnemyHud __instance, Character c, bool isMount) {
      if (IsModEnabled.Value 
          && IsBossAnnouncementEnabled.Value
          && c.TryGetComponent(out MonsterAI monsterAI) 
          && c.TryGetComponent(out ZNetView zNetView)
          && !zNetView.GetZDO().GetBool(HasSeenFieldName + Player.m_localPlayer.GetPlayerID().ToString(), false) 
          && !string.IsNullOrWhiteSpace(zNetView.GetZDO().GetString(CustomNameFieldName))
          && zNetView.GetZDO().GetBool(BossDesignationFieldName, false)) { 

        zNetView.GetZDO().Set(HasSeenFieldName + Player.m_localPlayer.GetPlayerID().ToString(), true);
        MessageHud.instance.ShowBiomeFoundMsg(zNetView.GetZDO().GetString(CustomNameFieldName), false);
      }
    }
  }
}