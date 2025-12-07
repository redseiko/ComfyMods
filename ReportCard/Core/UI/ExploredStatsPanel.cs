﻿namespace ReportCard;

using System;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public sealed class ExploredStatsPanel {
  public GameObject Panel { get; }
  public RectTransform RectTransform { get; }
  public TextMeshProUGUI Title { get; }
  public ListView StatsList { get; }
  public TextMeshProUGUI StatusLabel { get; }
  public LabelButton CloseButton { get; }

  public ExploredStatsPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    Title = CreateTitle(RectTransform);
    StatsList = CreateStatsList(RectTransform);
    StatusLabel = CreateStatusLabel(RectTransform);
    CloseButton = CreateCloseButton(RectTransform);
  }

  public TextMeshProUGUI AddStatLabel() {
    return CreateStatLabel(StatsList.Content.transform);
  }

  public void ResetStatsList() {
    foreach (Transform transform in StatsList.Content.transform) {
      UnityEngine.Object.Destroy(transform.gameObject);
    }
  }

  public void UpdateStatsList(ExploredStats exploredStats) {
    int worldExplored = exploredStats.ExploredCount();
    int worldTotal = exploredStats.TotalCount();

    AddExploredStatsLabel("World", worldExplored, worldTotal, worldTotal);
    AddExploredStatsSlider(Color.white, worldExplored, worldTotal);

    foreach (Heightmap.Biome biome in ExploredStats.GetHeightmapBiomes()) {
      int biomeExplored = exploredStats.ExploredCount(biome);
      int biomeTotal = exploredStats.TotalCount(biome);

      if (biomeTotal <= 0) {
        continue;
      }

      string name = Enum.GetName(typeof(Heightmap.Biome), biome);
      Color color = BiomeToSliderColor(biome);

      AddExploredStatsLabel(name, biomeExplored, biomeTotal, worldTotal);
      AddExploredStatsSlider(color, biomeExplored, biomeTotal);
    }
  }

  public void UpdateStatus(string status) {
    StatusLabel.text = status;
  }

  static Color BiomeToSliderColor(Heightmap.Biome biome) {
    return biome switch {
      Heightmap.Biome.Meadows => new(0f, 1f, 0.4f),
      Heightmap.Biome.BlackForest => new(0f, 0.46f, 0.33f),
      Heightmap.Biome.Swamp => new(1f, 0.6f, 0.46f),
      Heightmap.Biome.Mountain => new(0.26f, 1f, 1f),
      Heightmap.Biome.Plains => new(1f, 1f, 0f),
      Heightmap.Biome.Mistlands => new(0.6f, 0f, 0.66f),
      Heightmap.Biome.AshLands => new(1f, 0f, 0f),
      Heightmap.Biome.Ocean => new(0f, 0.46f, 1f),
      _ => Color.white,
    };
  }

  void AddExploredStatsLabel(string biomeName, int biomeExplored, int biomeTotal, int worldTotal) {
    TextMeshProUGUI label = CreateStatLabel(StatsList.Content.transform);

    if (ExploredStatsPanelShowRawValues.Value) {
      float biomePercent = ((biomeTotal * 1f) / (worldTotal * 1f)) * 100f;

      label.text +=
          $"<align=left><color=#FFD600>{biomeName}</color> "
              + $"(<color=#FFD600>{biomePercent:F2}%</color>)<line-height=0>\n"
              + $"<align=right>{biomeExplored:D}/<color=#FFD600>{biomeTotal:D}</color><line-height=1em>";
    } else {
      float exploredPercent = ((biomeExplored * 1f) / (biomeTotal * 1f)) * 100f;

      label.text =
          $"<align=left><color=#FFD600>{biomeName}</color><line-height=0>\n"
              + $"<align=right>{exploredPercent:F2}%<line-height=1em>";
    }
  }

  void AddExploredStatsSlider(Color color, int explored, int total) {
    Slider slider = CreateStatSlider(StatsList.Content.transform);

    slider.fillRect.GetComponent<Image>()
        .SetColor(color);

    slider
        .SetInteractable(false)
        .SetWholeNumbers(true)
        .SetMinValue(0f)
        .SetMaxValue(total)
        .SetValueWithoutNotify(explored);
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "ExploredStatsPanel";

    panel.AddComponent<MinimapFocus>();

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

    listView.ContentLayoutGroup
        .SetPadding(left: 10, right: 20)
        .SetSpacing(2.5f);

    return listView;
  }

  static TextMeshProUGUI CreateStatusLabel(Transform parentTransform) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(parentTransform);
    label.name = "StatusLabel";

    label.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.zero)
        .SetPosition(new(20f, 20f))
        .SetSizeDelta(new(-155f, 42.5f));

    label
        .SetAlignment(TextAlignmentOptions.MidlineLeft)
        .SetLineSpacing(10f)
        .SetText("Status");

    return label;
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

  static Slider CreateStatSlider(Transform parentTransform) {
    GameObject container = new("StatSlider", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.zero)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(0f, 20f));

    GameObject background = new("Background", typeof(RectTransform));
    background.transform.SetParent(container.transform, worldPositionStays: false);

    background.AddComponent<Image>()
        .SetColor(new(0.271f, 0.271f, 0.271f, 1f));

    background.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    GameObject fill = new("Fill", typeof(RectTransform));
    fill.transform.SetParent(container.transform, worldPositionStays: false);

    fill.AddComponent<Image>()
        .SetColor(Color.white);

    fill.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    Slider slider = container.AddComponent<Slider>();

    slider
        .SetDirection(Slider.Direction.LeftToRight)
        .SetFillRect(fill.GetComponent<RectTransform>())
        .SetTargetGraphic(fill.GetComponent<Image>())
        .SetTransition(Selectable.Transition.None);

    container.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(height: 10f);

    return slider;
  }
}
