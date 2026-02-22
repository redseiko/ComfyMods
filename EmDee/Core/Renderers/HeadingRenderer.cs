namespace EmDee;

using ComfyLib;

using Markdig.Renderers;
using Markdig.Syntax;

using TMPro;

using UnityEngine.UI;

public sealed class HeadingRenderer : MarkdownObjectRenderer<EmDeeRenderer, HeadingBlock> {
  protected override void Write(EmDeeRenderer renderer, HeadingBlock block) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(renderer.CurrentContext.ParentTransform);

    float fontSize = GetFontSize(block.Level, renderer.CurrentStyle);
    
    label
        .SetAlignment(TextAlignmentOptions.Left)
        .SetColor(renderer.CurrentStyle.TextColor)
        .SetFontSize(fontSize)
        .SetText(RenderInlines(block, renderer.CurrentStyle));

    label.gameObject.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(height: fontSize * 1.2f);
  }

  float GetFontSize(int level, EmDeeStyle style) {
    return level switch {
      1 => style.H1Size,
      2 => style.H2Size,
      3 => style.H3Size,
      _ => style.BodySize
    };
  }

  string RenderInlines(LeafBlock block, EmDeeStyle style) {
    if (block.Inline == null) return string.Empty;

    using var writer = new System.IO.StringWriter();
    var inlineRenderer = new EmDeeRichTextRenderer(writer, style);
    inlineRenderer.Render(block.Inline);
    
    return writer.ToString();
  }
}
