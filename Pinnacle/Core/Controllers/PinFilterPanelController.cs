namespace Pinnacle;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class PinFilterPanelController {
  static PinFilterPanel _currentPanel = default;

  public static void TogglePanel(bool toggleOn) {
    if (!IsValid()) {
      _currentPanel = new(Minimap.m_instance.m_largeRoot.transform);

      _currentPanel.RectTransform
          .SetAnchorMin(new(1f, 0.5f))
          .SetAnchorMax(new(1f, 0.5f))
          .SetPivot(new(1f, 0.5f))
          .SetPosition(PinFilterPanelPosition.Value);

      _currentPanel.PanelDragger.OnPanelEndDrag += SavePanelPosition;
    }

    _currentPanel.Panel.SetActive(toggleOn);
  }

  static void SavePanelPosition(object sender, Vector3 position) {
    PinFilterPanelPosition.Value = position;
  }

  public static bool IsValid() {
    return _currentPanel != default && _currentPanel.Panel;
  }

  public static void UpdateIconFilters() {
    if (IsValid()) {
      _currentPanel.UpdatePinIconFilters();
    }
  }

  public static void UpdateStyle() {
    if (IsValid()) {
      _currentPanel.SetPanelStyle();
    }
  }
}
