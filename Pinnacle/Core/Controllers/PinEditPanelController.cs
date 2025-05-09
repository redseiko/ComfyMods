namespace Pinnacle;

using ComfyLib;

public static class PinEditPanelController {
  public static PinEditPanel PinEditPanel { get; private set; }

  public static void TogglePanel(Minimap.PinData pinToEdit = null) {
    if (!IsValid()) {
      PinEditPanel = new(Minimap.m_instance.m_largeRoot.transform);

      PinEditPanel.RectTransform
          .SetAnchorMin(new(0.5f, 0f))
          .SetAnchorMax(new(0.5f, 0f))
          .SetPivot(new(0.5f, 0f))
          .SetPosition(new(0f, 25f))
          .SetSizeDelta(new(200f, 200f));
    }

    if (pinToEdit == null) {
      PinEditPanel.SetTargetPin(null);
      PinEditPanel.SetActive(false);
    } else {
      CenterMapHelper.CenterMapOnPosition(pinToEdit.m_pos);

      PinEditPanel.SetTargetPin(pinToEdit);
      PinEditPanel.SetActive(true);
    }
  }

  public static bool IsValid() {
    return PinEditPanel != default && PinEditPanel.Panel;
  }

  public static bool HasFocus() {
    return PinEditPanel != default && PinEditPanel.Panel && PinEditPanel.Panel.activeSelf && PinEditPanel.HasFocus();
  }
}
