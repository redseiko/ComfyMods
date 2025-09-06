namespace Configula;

using UnityEngine;

using static PluginConfig;

public static class ConfigulaManager {
  static readonly Texture2D _tooltipBackground = GUIHelper.CreateColorTexture(1, 1, Color.black);

  static GUIStyle CreateTooltipStyle() {
    return new(GUI.skin.box) {
      wordWrap = true,
      alignment = TextAnchor.MiddleCenter,
      normal = new() {
        background = _tooltipBackground,
        textColor = Color.white,
      },
    };
  }

  public static void DrawTooltip(Rect area) {
    string tooltip = GUI.tooltip;

    if (string.IsNullOrEmpty(tooltip)) {
      return;
    }

    GUIContent tooltipContent = new(tooltip);
    GUIStyle tooltipStyle = CreateTooltipStyle();
    float tooltipWidth = Mathf.Min(AlternateTooltipWidth.Value, area.width);
    float tooltipHeight = tooltipStyle.CalcHeight(tooltipContent, tooltipWidth) + 10f;
    Vector2 mousePosition = Event.current.mousePosition;

    float x =
        mousePosition.x + tooltipWidth > area.width
            ? area.width - tooltipWidth
            : mousePosition.x;
    float y =
        mousePosition.y + 25f + tooltipHeight > area.height
            ? mousePosition.y - tooltipHeight
            : mousePosition.y + 25f;

    Rect boxRect = new(x, y, tooltipWidth, tooltipHeight);

    GUI.Box(boxRect, tooltipContent, tooltipStyle);
  }
}
