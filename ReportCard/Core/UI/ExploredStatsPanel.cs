namespace ReportCard;

using System;

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

  public ExploredStatsPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    Title = CreateTitle(RectTransform);
    StatsList = CreateStatsList(RectTransform);
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
    int explored = exploredStats.ExploredCount();
    int total = exploredStats.TotalCount();

    AddExploredStatsLabel("World", explored, total);
    AddExploredStatsSlider(Color.white, explored, total);

    CreateStatDivider(StatsList.Content.transform);

    foreach (Heightmap.Biome biome in ExploredStats.GetHeightmapBiomes()) {
      explored = exploredStats.ExploredCount(biome);
      total = exploredStats.TotalCount(biome);

      if (total <= 0) {
        continue;
      }

      Color color = BiomeToSliderColor(biome);
      AddExploredStatsLabel(Enum.GetName(typeof(Heightmap.Biome), biome), explored, total);
      AddExploredStatsSlider(color, explored, total);
    }
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

  void AddExploredStatsLabel(string stat, int explored, int total) {
    TextMeshProUGUI label = CreateStatLabel(StatsList.Content.transform);
    float percent = ((explored * 1f) / (total * 1f)) * 100f;

    label.text =
        $"<align=left><color=#FFD600>{stat}</color> <size=-2>({explored}/{total})</size><line-height=0>\n"
            + $"<align=right>{percent:F2}%<line-height=1em>";
  }

  void AddExploredStatsSlider(Color color, int explored, int total) {
    Slider slider = CreateStatSlider(StatsList.Content.transform);

    slider.fillRect.GetComponent<Image>()
        .SetColor(color);

    slider
        .SetInteractable(false)
        .SetTransition(Selectable.Transition.None)
        .SetWholeNumbers(true)
        .SetMinValue(0f)
        .SetMaxValue(total)
        .SetValueWithoutNotify(explored);
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
    Slider slider = UIBuilder.CreateSlider(parentTransform);
    slider.name = "StatSlider";

    slider.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.zero)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    slider.handleRect.gameObject.SetActive(false);

    slider.gameObject.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(height: 25f);

    return slider;
  }

  static GameObject CreateStatDivider(Transform parentTransform) {
    GameObject divider = new("Divider", typeof(RectTransform));
    divider.transform.SetParent(parentTransform, worldPositionStays: false);

    divider.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(height: 10f);

    return divider;
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
