namespace EmDee;

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

public sealed class LiteralInlineRenderer : MarkdownObjectRenderer<EmDeeRichTextRenderer, LiteralInline> {
  protected override void Write(EmDeeRichTextRenderer renderer, LiteralInline obj) {
    // Write the raw literal snippet. In a more complete implementation, 
    // we might need to escape '<' and '>' for TextMeshPro if it conflicts with actual TMP tags.
    renderer.Write(ref obj.Content);
  }
}
