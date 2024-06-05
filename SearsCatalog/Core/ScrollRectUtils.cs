namespace SearsCatalog;

using UnityEngine;
using UnityEngine.UI;

public static class ScrollRectUtils {
  // Modified from original snippet at: https://stackoverflow.com/a/61632269
  public static void EnsureVisibility(ScrollRect scrollRect, RectTransform child, float padding = 0f) {
    float viewportHeight = scrollRect.viewport.rect.height;
    Vector2 scrollPosition = scrollRect.content.anchoredPosition;

    float elementTop = child.anchoredPosition.y;
    float elementBottom = elementTop - child.rect.height;

    float visibleContentTop = -scrollPosition.y - padding;
    float visibleContentBottom = -scrollPosition.y - viewportHeight + padding;

    float scrollDelta =
        elementTop > visibleContentTop
            ? visibleContentTop - elementTop
            : elementBottom < visibleContentBottom
                ? visibleContentBottom - elementBottom
                : 0f;

    scrollPosition.y += scrollDelta;
    scrollRect.content.anchoredPosition = scrollPosition;
  }
}
