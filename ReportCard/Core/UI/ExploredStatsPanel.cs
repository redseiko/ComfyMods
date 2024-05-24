namespace ReportCard;

using ComfyLib;

using TMPro;

using UnityEngine;

public sealed class ExploredStatsPanel {
  public GameObject Panel { get; }
  public RectTransform RectTransform { get; }
  public TextMeshProUGUI Title { get; }
  public ListView StatsListView { get; }
  public LabelButton CloseButton { get; }

  public ExploredStatsPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    Title = CreateTitle(RectTransform);
    StatsListView = CreateStatsListView(RectTransform);
    CloseButton = CreateCloseButton(RectTransform);
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

  static ListView CreateStatsListView(Transform parentTransform) {
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
