using UnityEngine.UI;
using UnityEngine;

namespace CarbonCopy {
  public static class VerticalLayoutGroupExtensions {
    public static VerticalLayoutGroup SetChildControl(
        this VerticalLayoutGroup layoutGroup, bool width = false, bool height = false) {
      layoutGroup.childControlWidth = width;
      layoutGroup.childControlHeight = height;
      return layoutGroup;
    }

    public static VerticalLayoutGroup SetChildForceExpand(
        this VerticalLayoutGroup layoutGroup, bool width = false, bool height = false) {
      layoutGroup.childForceExpandWidth = width;
      layoutGroup.childForceExpandHeight = height;
      return layoutGroup;
    }

    public static VerticalLayoutGroup SetChildAlignment(
        this VerticalLayoutGroup layoutGroup, TextAnchor alignment) {
      layoutGroup.childAlignment = alignment;
      return layoutGroup;
    }

    public static VerticalLayoutGroup SetPadding(
        this VerticalLayoutGroup layoutGroup, int left = 0, int right = 0, int top = 0, int bottom = 0) {
      layoutGroup.padding.left = left;
      layoutGroup.padding.right = right;
      layoutGroup.padding.top = top;
      layoutGroup.padding.bottom = bottom;
      return layoutGroup;
    }

    public static VerticalLayoutGroup SetSpacing(this VerticalLayoutGroup layoutGroup, float spacing) {
      layoutGroup.spacing = spacing;
      return layoutGroup;
    }
  }
}
