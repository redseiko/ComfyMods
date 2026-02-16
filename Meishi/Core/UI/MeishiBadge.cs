namespace Meishi;

using UnityEngine;
using UnityEngine.UI;
using ComfyLib;

public sealed class MeishiBadge {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; }

  public Image Border { get; private set; }
  public Image Icon { get; private set; }

  public MeishiBadge(Transform parent) {
    Container = CreateContainer(parent);
    RectTransform = Container.GetComponent<RectTransform>();

    Border = Container.GetComponent<Image>();
    Icon = CreateIcon(Container.transform);
  }

  public void SetData(string badgeId) {
    // For now, just use a placeholder or basic color based on hash
    // In future, this will look up the badge sprite from UIResources
    if (Icon) {
      Icon.color = Color.Lerp(Color.white, Color.red, (badgeId.GetHashCode() % 100) / 100f);
    }
  }

  static GameObject CreateContainer(Transform parent) {
    GameObject container = new("Badge", typeof(RectTransform));
    container.transform.SetParent(parent, worldPositionStays: false);

    container.AddComponent<Image>()
        .SetColor(Color.white)
        .SetSprite(UIResources.GetSprite("woodpanel_trophys"))
        .SetType(Image.Type.Sliced);

    return container;
  }

  static Image CreateIcon(Transform parent) {
    GameObject iconObj = new("Icon", typeof(RectTransform));
    iconObj.transform.SetParent(parent, worldPositionStays: false);

    Image icon = iconObj.AddComponent<Image>()
        .SetPreserveAspect(true)
        .SetColor(Color.gray); // Default

    icon.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetSizeDelta(new Vector2(-4f, -4f)); // 2px padding

    return icon;
  }
}
