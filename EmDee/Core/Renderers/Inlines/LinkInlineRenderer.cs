namespace EmDee;

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

using UnityEngine;

public sealed class LinkInlineRenderer : MarkdownObjectRenderer<EmDeeRichTextRenderer, LinkInline> {
  protected override void Write(EmDeeRichTextRenderer renderer, LinkInline obj) {
    if (obj.IsImage) {
      // Images not supported yet in standard text flow easily via string only,
      // but TMP has <sprite> tags if a sprite asset is configured.
      // For now, we fallback to just writing the alt text or something.
      renderer.Write("<color=red>[Image unsupported]</color>");
      return;
    }

    // A standard link: <link="url"><u><color=#hex>text</color></u></link>
    // Note: Link color should ideally be in EmDeeStyle. We'll assume a default blue-ish for now if not present,
    // or just use <u> tags. Let's assume EmDeeStyle has LinkTextColor or we just use an underline.

    renderer.Write($"<link=\"{obj.Url}\"><u>");
    renderer.WriteChildren(obj);
    renderer.Write("</u></link>");
  }
}
