﻿namespace ReportCard;

using System;
using System.Collections.Generic;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerStatsPanel {
  public GameObject Panel { get; private set; }
  public RectTransform RectTransform { get; }
  public TMP_Text Title { get; private set; }
  public ListView StatsList { get; private set; }
  public LabelButton CloseButton { get; private set; }

  public PlayerStatsPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    Title = CreateTitle(Panel.transform);
    StatsList = CreateStatsList(Panel.transform);
    CloseButton = CreateCloseButton(Panel.transform);
  }

  public List<TMP_Text> StatLabels { get; } = [];

  public void UpdateStatsList(PlayerProfile profile) {
    List<KeyValuePair<PlayerStatType, float>> stats = [..profile.m_playerStats.m_stats];

    for (int i = StatLabels.Count; i < stats.Count; i++) {
      TMP_Text label = CreateStatLabel(StatsList.Content.transform);
      StatLabels.Add(label);
    }

    for (int i = 0; i < stats.Count; i++) {
      KeyValuePair<PlayerStatType, float> pair = stats[i];
      TMP_Text label = StatLabels[i];

      label.SetText(
          $"<align=left><color=#FFD600>{pair.Key}</color><line-height=0>\n"
              + $"<align=right>{GetFormattedValue(pair)}<line-height=1em>");
    }
  }

  static string GetFormattedValue(KeyValuePair<PlayerStatType, float> pair) {
    switch (pair.Key) {
      case PlayerStatType.TimeInBase:
      case PlayerStatType.TimeOutOfBase:
        return GetFormattedValue(TimeSpan.FromSeconds(pair.Value));

      case PlayerStatType.DistanceTraveled:
      case PlayerStatType.DistanceWalk:
      case PlayerStatType.DistanceRun:
      case PlayerStatType.DistanceSail:
      case PlayerStatType.DistanceAir:
        return $"{pair.Value:N2}";

      default:
        return $"{pair.Value}";
    }
  }

  static string GetFormattedValue(TimeSpan duration) {
    return duration.Days >= 1
        ? $"{duration.Days}d {duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}"
        : $"{duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
  }

  static TMP_Text CreateStatLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);
    label.name = "StatLabel";

    label.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.zero)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    label
        .SetFontSize(18f)
        .SetAlignment(TextAlignmentOptions.Left);

    label.gameObject.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(height: 30f);

    return label;
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "PlayerStatsPanel";

    panel.GetComponent<RectTransform>()
        .SetSizeDelta(new(400f, 600f));

    return panel;
  }

  static TMP_Text CreateTitle(Transform parentTransform) {
    TMP_Text title = UIBuilder.CreateTMPHeaderLabel(parentTransform);
    title.name = "Title";

    title
        .SetAlignment(TextAlignmentOptions.Top)
        .SetText("Stats");

    title.rectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 1f))
        .SetPosition(new(0f, -15f))
        .SetSizeDelta(new(0f, 40f));

    return title;
  }

  static ListView CreateStatsList(Transform parentTransform) {
    ListView listView = new(parentTransform);
    listView.Container.name = "StatsListView";

    listView.Container.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(new(20f, -60f))
        .SetSizeDelta(new(-40f, -135f));

    listView.ContentLayoutGroup.SetPadding(left: 10, right: 20);

    return listView;
  }

  static LabelButton CreateCloseButton(Transform parentTransform) {
    LabelButton closeButton = new(parentTransform);
    closeButton.Button.name = "CloseButton";

    closeButton.RectTransform
        .SetAnchorMin(Vector2.right)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.right)
        .SetPosition(new(-20f, 20f))
        .SetSizeDelta(new(100f, 42.5f));

    closeButton.Label
        .SetFontSize(18f)
        .SetText("Close");

    return closeButton;
  }
}
