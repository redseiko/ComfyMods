using UnityEngine.UI;

namespace CarbonCopy {
  public static class ButtonExtensions {
    public static Button SetLabel(this Button button, string label) {
      Text text = button.GetComponentInChildren<Text>(includeInactive: false);

      if (text) {
        text.text = label;
      }

      return button;
    }
  }
}
