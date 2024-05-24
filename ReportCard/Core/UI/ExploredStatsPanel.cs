namespace ReportCard;

using System;
using System.Collections.Generic;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ExploredStatsPanel {
  public GameObject Panel { get; }
  public RectTransform RectTransform { get; }
  public TextMeshProUGUI Title { get; }
  public ListView StatsList { get; }
  public LabelButton CloseButton { get; }
  public List<TextMeshProUGUI> StatLabels { get; }

  public ExploredStatsPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    Title = CreateTitle(RectTransform);
    StatsList = CreateStatsList(RectTransform);
    CloseButton = CreateCloseButton(RectTransform);
    StatLabels = [];
  }

  public TextMeshProUGUI AddStatLabel() {
    TextMeshProUGUI label = CreateStatLabel(StatsList.Content.transform);
    StatLabels.Add(label);

    return label;
  }

  public void ResetStatsList() {
    foreach (TextMeshProUGUI label in StatLabels) {
      UnityEngine.Object.Destroy(label.gameObject);
    }

    StatLabels.Clear();
  }

  public void UpdateStatsList(ExploredStats exploredStats) {
    StatLabels.Add(CreateExploredStatLabel("Explored", exploredStats.ExploredCount(), exploredStats.TotalCount()));

    foreach (Heightmap.Biome biome in ExploredStats.GetHeightmapBiomes()) {
      int explored = exploredStats.ExploredCount(biome);
      int total = exploredStats.TotalCount(biome);

      if (total > 0) {
        StatLabels.Add(CreateExploredStatLabel(Enum.GetName(typeof(Heightmap.Biome), biome), explored, total));
      }
    }
  }

  TextMeshProUGUI CreateExploredStatLabel(string stat, int explored, int total) {
    float percent = ((explored * 1f) / (total * 1f)) * 100f;
    TextMeshProUGUI label = CreateStatLabel(StatsList.Content.transform);

    label.text =
        $"<align=left><color=#FFD600>{stat}</color> <size=-2>({explored}/{total})</size><line-height=0>\n"
            + $"<align=right>{percent:F2}%<line-height=1em>";

    return label;
  }

  static TextMeshProUGUI CreateStatLabel(Transform parentTransform) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(parentTransform);
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
    panel.name = "ExploredStatsPanel";

    return panel;
  }

  static TextMeshProUGUI CreateTitle(Transform parentTransform) {
    TextMeshProUGUI title = UIBuilder.CreateTMPHeaderLabel(parentTransform);
    title.name = "Title";

    title
        .SetAlignment(TextAlignmentOptions.Top)
        .SetText("Explored");

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

    closeButton.Container.GetComponent<RectTransform>()
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
