using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public class ChatPanel {
    public GameObject Panel { get; private set; }
    public RectTransform PanelRectTransform { get; private set; }
    public Image PanelBackground { get; private set; }
    public CanvasGroup PanelCanvasGroup { get; private set; }
    public RectTransformDragger PanelDragger { get; private set; }

    public GameObject ContentViewport { get; private set; }
    public GameObject Content { get; private set; }
    public ScrollRect ContentScrollRect { get; private set; }

    public InputFieldCell TextInput { get; private set; }

    public ToggleRow MessageTypeToggleRow { get; private set; }

    public ChatPanel(Transform parentTransform) {
      Panel = CreateChildPanel(parentTransform);
      PanelRectTransform = Panel.GetComponent<RectTransform>();
      PanelBackground = Panel.GetComponent<Image>();
      PanelCanvasGroup = Panel.GetComponent<CanvasGroup>();
      PanelDragger = Panel.GetComponent<RectTransformDragger>();

      ContentViewport = CreateChildViewport(Panel.transform);
      Content = CreateChildContent(ContentViewport.transform);
      ContentScrollRect = CreateChildScrollRect(ContentViewport, Content);

      TextInput = new(Panel.transform);

      TextInput.Cell.AddComponent<LayoutElement>()
          .SetFlexible(width: 1f)
          .SetPreferred(height: 35f);

      MessageTypeToggleRow = new(Panel.transform);
      MessageTypeToggleRow.Row.AddComponent<LayoutElement>()
          .SetIgnoreLayout(true);
    }

    GameObject CreateChildPanel(Transform parentTransform) {
      GameObject panel = new("ChatPanel", typeof(RectTransform));
      panel.SetParent(parentTransform);

      panel.GetComponent<RectTransform>()
          .SetAnchorMin(new(0.5f, 0.5f))
          .SetAnchorMax(new(0.5f, 0.5f))
          .SetPivot(new(0.5f, 0.5f))
          .SetPosition(Vector2.zero)
          .SetSizeDelta(new(400f, 400f));

      panel.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetPadding(left: 8, right: 8, top: 8, bottom: 8)
          .SetSpacing(10f);

      panel.AddComponent<Image>()
          .SetType(Image.Type.Sliced)
          .SetSprite(UIBuilder.CreateSuperellipse(400, 400, 15))
          .SetColor(new(0f, 0f, 0f, 0.4f));

      panel.AddComponent<CanvasGroup>()
          .SetAlpha(1f)
          .SetBlocksRaycasts(true);

      panel.AddComponent<RectTransformDragger>()
          .SetTargetRectTransform(panel.GetComponent<RectTransform>());

      return panel;
    }

    GameObject CreateChildViewport(Transform parentTransform) {
      GameObject viewport = new("Viewport", typeof(RectTransform));
      viewport.SetParent(parentTransform);

      viewport.AddComponent<Image>()
          .SetType(Image.Type.Filled)
          .SetColor(Color.clear);

      viewport.AddComponent<RectMask2D>();

      viewport.AddComponent<LayoutElement>()
          .SetFlexible(width: 1f, height: 1f);

      return viewport;
    }

    GameObject CreateChildContent(Transform parentTransform) {
      GameObject content = new("Content", typeof(RectTransform));
      content.SetParent(parentTransform);

      content.GetComponent<RectTransform>()
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.right)
          .SetPivot(Vector2.zero)
          .SetPosition(Vector2.zero)
          .SetSizeDelta(Vector2.zero);

      content.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetPadding(left: 8, right: 8)
          .SetSpacing(10f);

      content.AddComponent<ContentSizeFitter>()
          .SetHorizontalFit(ContentSizeFitter.FitMode.Unconstrained)
          .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

      return content;
    }

    ScrollRect CreateChildScrollRect(GameObject viewport, GameObject content) {
      ScrollRect scrollRect = viewport.AddComponent<ScrollRect>();

      scrollRect.viewport = viewport.GetComponent<RectTransform>();
      scrollRect.content = content.GetComponent<RectTransform>();
      scrollRect.horizontal = false;
      scrollRect.vertical = true;
      scrollRect.movementType = ScrollRect.MovementType.Elastic;
      scrollRect.scrollSensitivity = 20f;

      return scrollRect;
    }

    public void OffsetContentVerticalScrollPosition(float offset) {
      float percent = (offset / (Content.transform as RectTransform).sizeDelta.y);
      ContentScrollRect.verticalNormalizedPosition += percent;
    }

    public void SetContentVerticalScrollPosition(float position) {
      ContentScrollRect.verticalNormalizedPosition = position;
    }

    public void ToggleGrabber(bool toggleOn) {
      MessageTypeToggleRow.Row.SetActive(toggleOn);
    }
  }
}
