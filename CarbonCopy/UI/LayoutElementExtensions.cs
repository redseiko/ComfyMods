using UnityEngine.UI;

namespace CarbonCopy {
  public static class LayoutElementExtensions {
    public static LayoutElement SetPreferred(this LayoutElement layoutElement, float width = -1f, float height = -1f) {
      layoutElement.preferredWidth = width;
      layoutElement.preferredHeight = height;

      return layoutElement;
    }
    public static LayoutElement SetFlexible(this LayoutElement layoutElement, float width = -1f, float height = -1f) {
      layoutElement.flexibleWidth = width;
      layoutElement.flexibleHeight = height;

      return layoutElement;
    }
  }
}
