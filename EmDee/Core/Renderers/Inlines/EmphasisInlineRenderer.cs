namespace EmDee;

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

public sealed class EmphasisInlineRenderer : MarkdownObjectRenderer<EmDeeRichTextRenderer, EmphasisInline> {
  protected override void Write(EmDeeRichTextRenderer renderer, EmphasisInline obj) {
    // obj.DelimiterCount: 2 is typically **bold**, 1 is *italic*
    bool isBold = obj.DelimiterCount == 2;
    bool isItalic = obj.DelimiterCount == 1;

    if (isBold) {
      renderer.Write("<b>");
    } else if (isItalic) {
      renderer.Write("<i>");
    }

    renderer.WriteChildren(obj);

    if (isBold) {
      renderer.Write("</b>");
    } else if (isItalic) {
      renderer.Write("</i>");
    }
  }
}
