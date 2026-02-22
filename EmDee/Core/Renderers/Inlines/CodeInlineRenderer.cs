namespace EmDee;

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

using UnityEngine;

public sealed class CodeInlineRenderer : MarkdownObjectRenderer<EmDeeRichTextRenderer, CodeInline> {
  protected override void Write(EmDeeRichTextRenderer renderer, CodeInline obj) {
    // Basic TMP implementation:
    // <font="MonospaceFontAsset"><mark=#BackgroundHex><color=#TextHex>code snippet</color></mark></font>
    
    // We get colors from the style
    string textHex = ColorUtility.ToHtmlStringRGBA(renderer.CurrentStyle.CodeTextColor);
    string bgHex = ColorUtility.ToHtmlStringRGBA(renderer.CurrentStyle.CodeBlockBackgroundColor);

    renderer.Write($"<mark=#{bgHex}><color=#{textHex}>");
    
    // Write the raw content
    renderer.Write(obj.Content);

    renderer.Write("</color></mark>");
  }
}
