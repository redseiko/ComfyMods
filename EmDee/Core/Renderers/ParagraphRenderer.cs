namespace EmDee;

using ComfyLib;

using Markdig.Renderers;
using Markdig.Syntax;

using TMPro;

using UnityEngine.UI;

public sealed class ParagraphRenderer : MarkdownObjectRenderer<EmDeeRenderer, ParagraphBlock> {
  protected override void Write(EmDeeRenderer renderer, ParagraphBlock block) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(renderer.CurrentContext.ParentTransform);

    label
        .SetAlignment(TextAlignmentOptions.TopLeft)
        .SetColor(renderer.CurrentStyle.TextColor)
        .SetFontSize(renderer.CurrentStyle.BodySize)
        .SetText(RenderInlines(block, renderer.CurrentStyle));

    // For paragraphs we want to wrap text based on content
    label.textWrappingMode = TextWrappingModes.Normal;

    // Simple auto-layout configuration
    label.gameObject.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(height: label.preferredHeight);
  }

  string RenderInlines(LeafBlock block, EmDeeStyle style) {
    if (block.Inline == null) return string.Empty;

    using var writer = new System.IO.StringWriter();
    var inlineRenderer = new EmDeeRichTextRenderer(writer, style);
    inlineRenderer.Render(block.Inline);
    
    return writer.ToString();
  }
}
