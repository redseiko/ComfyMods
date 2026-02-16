namespace Meishi;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class MeishiCard {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; }
  public Image Background { get; private set; }

  public GameObject Content { get; private set; }
  public TextMeshProUGUI TitleLabel { get; private set; }
  public TextMeshProUGUI NameLabel { get; private set; }
  public Transform BadgeGrid { get; private set; }

  public MeishiCard(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    Background = Container.GetComponent<Image>();

    Content = CreateContent(Container.transform);
    TitleLabel = CreateTitleText(Content.transform);
    NameLabel = CreateNameText(Content.transform);
    BadgeGrid = CreateBadgeGrid(Content.transform);
  }

  public void SetData(MeishiCardData data) {
    NameLabel.SetText(data.PlayerName);
    TitleLabel.SetText(data.Title);
    Background.SetSprite(UIResources.GetSprite(data.BackgroundId));

    foreach (Transform child in BadgeGrid) {
      Object.Destroy(child.gameObject);
    }

    foreach (string badgeId in data.BadgeIds) {
      MeishiBadge badge = new(BadgeGrid);
      badge.SetData(badgeId);
    }
  }

  static GameObject CreateContainer(Transform parent) {
    GameObject container = new("MeishiCard", typeof(RectTransform));
    container.transform.SetParent(parent, worldPositionStays: false);

    container.AddComponent<Image>()
        .SetColor(Color.white)
        .SetSprite(UIResources.GetSprite("woodpanel_info"))
        .SetType(Image.Type.Sliced);

    container.GetComponent<RectTransform>()
        .SetSizeDelta(new Vector2(400, 250));

    return container;
  }

  static GameObject CreateContent(Transform parentTransform) {
    GameObject content = new("Content", typeof(RectTransform));
    content.transform.SetParent(parentTransform, worldPositionStays: false);

    content.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetSizeDelta(new Vector2(-10f, -10f));

    return content;
  }

  static TextMeshProUGUI CreateNameText(Transform parent) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(parent);
    label.name = "PlayerName";

    label.rectTransform
        .SetAnchorMin(new Vector2(0f, 0.65f))
        .SetAnchorMax(new Vector2(1f, 0.9f))
        .SetSizeDelta(Vector2.zero);

    label
        .SetFontSize(32f)
        .SetAlignment(TextAlignmentOptions.Bottom)
        .SetColor(Color.white)
        .SetText("Player Name");

    return label;
  }

  static TextMeshProUGUI CreateTitleText(Transform parent) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(parent);
    label.name = "PlayerTitle";

    label.rectTransform
        .SetAnchorMin(new Vector2(0f, 0.5f))
        .SetAnchorMax(new Vector2(1f, 0.65f))
        .SetSizeDelta(Vector2.zero);

    label
        .SetFontSize(18f)
        .SetAlignment(TextAlignmentOptions.Top)
        .SetColor(new Color(0.8f, 0.8f, 0.8f, 1f))
        .SetText("Title");

    return label;
  }

  static Transform CreateBadgeGrid(Transform parent) {
    GameObject badgeGrid = new("BadgeGrid", typeof(RectTransform));
    badgeGrid.transform.SetParent(parent, worldPositionStays: false);

    badgeGrid.GetComponent<RectTransform>()
        .SetAnchorMin(new Vector2(0f, 0.1f))
        .SetAnchorMax(new Vector2(1f, 0.45f))
        .SetSizeDelta(new Vector2(-20f, 0f));

    badgeGrid.AddComponent<GridLayoutGroup>()
        .SetCellSize(new Vector2(40f, 40f))
        .SetSpacing(new Vector2(10f, 10f))
        .SetChildAlignment(TextAnchor.UpperCenter);

    return badgeGrid.transform;
  }
}
