namespace EmDee;

using System.Collections.Generic;

using Markdig.Renderers;
using Markdig.Syntax;

using UnityEngine;

public sealed class EmDeeRenderer : RendererBase {
  readonly Stack<EmDeeContext> _contextStack = new();
  public EmDeeContext CurrentContext { get => _contextStack.Peek(); }
  public EmDeeStyle CurrentStyle { get; private set; }

  public EmDeeRenderer(Transform rootTransform, EmDeeStyle style) {
    AddObjectRenderers();

    this.CurrentStyle = style;
    PushContext(rootTransform);
  }

  void AddObjectRenderers() {
    ObjectRenderers.Add(new DebugFallbackRenderer());
  }

  public EmDeeRenderer PushContext(Transform parentTransform) {
    _contextStack.Push(new EmDeeContext(parentTransform));
    return this;
  }

  public EmDeeRenderer PopContext() {
    _contextStack.Pop();
    return this;
  }

  public override object Render(MarkdownObject markdownObject) {
    Write(markdownObject);
    return this;
  }
}
