namespace EmDee;

using ComfyLib;

using Markdig.Renderers;
using Markdig.Syntax;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class DebugFallbackRenderer : MarkdownObjectRenderer<EmDeeRenderer, MarkdownObject> {
  protected override void Write(EmDeeRenderer renderer, MarkdownObject obj) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(renderer.CurrentContext.ParentTransform);

    label
        .SetAlignment(TextAlignmentOptions.TopLeft)
        .SetColor(Color.white)
        .SetText(obj.GetType().Name);

    label.gameObject.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(height: 30f);

    if (obj is ContainerBlock container) {
      GameObject childContainer = new("Container", typeof(RectTransform));
      childContainer.transform.SetParent(renderer.CurrentContext.ParentTransform);

      childContainer.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetPadding(left: 20);

      childContainer.AddComponent<ContentSizeFitter>()
          .SetHorizontalFit(ContentSizeFitter.FitMode.Unconstrained)
          .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

      renderer.PushContext(childContainer.transform);
      renderer.WriteChildren(container);
      renderer.PopContext();
    }
  }
}
