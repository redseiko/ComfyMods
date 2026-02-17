namespace EmDee;

using ComfyLib;

using Markdig;
using Markdig.Syntax;

using UnityEngine;

public sealed class EmDeeManager {
  public static EmDeeManager Instance { get; } = new();

  EmDeeManager() { }

  public EmDeePanel MarkdownPanel { get; private set; }

  public bool IsMarkdownPanelValid() {
    return MarkdownPanel?.Panel;
  }

  public void CreateMarkdownPanel(FejdStartup fejdStartup) {
    DestroyMarkdownPanel();

    MarkdownPanel = new(fejdStartup.m_characterSelectScreen.transform);

    MarkdownPanel.RectTransform
        .SetAnchorMin(new(1f, 0.5f))
        .SetAnchorMax(new(1f, 0.5f))
        .SetPivot(new(1f, 0.5f))
        .SetPosition(new(-25f, 0f))
        .SetSizeDelta(new(400f, 600f));

    HideMarkdownPanel();
  }

  public void DestroyMarkdownPanel() {
    if (IsMarkdownPanelValid()) {
      UnityEngine.Object.Destroy(MarkdownPanel.Panel);
      MarkdownPanel = default;
    }
  }

  public void ShowMarkdownPanel() {
    if (IsMarkdownPanelValid()) {
      MarkdownPanel.Panel.SetActive(true);
    }
  }

  public void HideMarkdownPanel() {
    if (IsMarkdownPanelValid()) {
      MarkdownPanel.Panel.SetActive(false);
    }
  }

  public void RenderMarkdown(string inputText) {
    if (!IsMarkdownPanelValid()) {
      return;
    }

    MarkdownPanel.ClearContent();
    ShowMarkdownPanel();

    MarkdownDocument document = Markdown.Parse(inputText, new MarkdownPipelineBuilder().Build());
    EmDeeRenderer renderer = new(MarkdownPanel.RectTransform, new EmDeeStyle());
    renderer.Render(document);
  }
}
