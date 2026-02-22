namespace EmDee;

using System.IO;

using Markdig.Renderers;

public sealed class EmDeeRichTextRenderer : TextRendererBase<EmDeeRichTextRenderer> {
  public EmDeeStyle CurrentStyle { get; private set; }

  public EmDeeRichTextRenderer(TextWriter writer, EmDeeStyle style) : base(writer) {
    this.CurrentStyle = style;

    ObjectRenderers.Add(new LiteralInlineRenderer());
    ObjectRenderers.Add(new EmphasisInlineRenderer());
    ObjectRenderers.Add(new CodeInlineRenderer());
    ObjectRenderers.Add(new LinkInlineRenderer());
  }
}
