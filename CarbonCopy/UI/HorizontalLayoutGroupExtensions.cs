using UnityEngine.UI;
using UnityEngine;

namespace CarbonCopy {
  public static class HorizontalLayoutGroupExtensions {
    public static HorizontalLayoutGroup SetChildControl(
        this HorizontalLayoutGroup layoutGroup, bool width = false, bool height = false) {
      layoutGroup.childControlWidth = width;
      layoutGroup.childControlHeight = height;
      return layoutGroup;
    }

    public static HorizontalLayoutGroup SetChildForceExpand(
        this HorizontalLayoutGroup layoutGroup, bool width = false, bool height = false) {
      layoutGroup.childForceExpandWidth = width;
      layoutGroup.childForceExpandHeight = height;
      return layoutGroup;
    }

    public static HorizontalLayoutGroup SetChildAlignment(
        this HorizontalLayoutGroup layoutGroup, TextAnchor alignment) {
      layoutGroup.childAlignment = alignment;
      return layoutGroup;
    }

    public static HorizontalLayoutGroup SetPadding(
        this HorizontalLayoutGroup layoutGroup, int left = 0, int right = 0, int top = 0, int bottom = 0) {
      layoutGroup.padding.left = left;
      layoutGroup.padding.right = right;
      layoutGroup.padding.top = top;
      layoutGroup.padding.bottom = bottom;
      return layoutGroup;
    }

    public static HorizontalLayoutGroup SetSpacing(this HorizontalLayoutGroup layoutGroup, float spacing) {
      layoutGroup.spacing = spacing;
      return layoutGroup;
    }
  }
}
