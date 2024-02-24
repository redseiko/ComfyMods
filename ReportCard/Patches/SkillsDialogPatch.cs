namespace ReportCard;

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using TMPro;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(SkillsDialog))]
static class SkillsDialogPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(SkillsDialog.Awake))]
  static void AwakePostfix() {
    _playerStatElements.Clear();
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(SkillsDialog.Setup))]
  static void SetupPostfix(SkillsDialog __instance, Player player) {
    if (IsModEnabled.Value) {
      AddPlayerStats(__instance, player, Game.instance.m_playerProfile);
    }
  }

  static readonly List<TMP_Text> _playerStatElements = new();

  static void AddPlayerStats(SkillsDialog skillsDialog, Player player, PlayerProfile profile) {
    List<KeyValuePair<PlayerStatType, float>> stats = profile.m_playerStats.m_stats.ToList();

    float spacing = skillsDialog.m_spacing;
    int count = player.m_skills.m_skillData.Count;

    for (int i = _playerStatElements.Count; i < stats.Count; i++) {
      TMP_Text label = UIBuilder.CreateTMPLabel(skillsDialog.m_listRoot);
      _playerStatElements.Add(label);
    }

    for (int i = 0; i < stats.Count; i++) {
      count++;

      TMP_Text label = _playerStatElements[i];
      KeyValuePair<PlayerStatType, float> pair = stats[i];

      label.rectTransform
          .SetAnchorMin(Vector2.up)
          .SetAnchorMax(Vector2.one)
          .SetPivot(new(0.5f, 0f))
          .SetPosition(new(0f, count * spacing * -1f))
          .SetSizeDelta(new(-40f, spacing));

      label
          .SetFontSize(22f)
          .SetAlignment(TextAlignmentOptions.Left)
          .SetText(
              $"<align=left><color=#FFD600>{pair.Key}</color><line-height=0>\n"
                  + $"<align=right>{pair.Value}<line-height=1em>");
    }

    skillsDialog.m_listRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (count) * spacing);
  }
}
