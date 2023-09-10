using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public class ChatPanel {
    public GameObject Panel { get; private set; }
    public CanvasGroup PanelCanvasGroup { get; private set; }
    public RectTransformDragger PanelDragger { get; private set; }

    public GameObject ContentViewport { get; private set; }
    public GameObject Content { get; private set; }
    public ScrollRect ContentScrollRect { get; private set; }

    public InputFieldCell TextInput { get; private set; }

    public ToggleRow MessageTypeToggleRow { get; private set; }

    public ChatPanel(Transform parentTransform) {
      Panel = CreateChildPanel(parentTransform);
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
      //.SetFlexible(width: 1f)
      //.SetPreferred(height: 35f);
      MessageTypeToggleRow.Row.GetComponent<RectTransform>()
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.right)
          .SetPivot(new(0.5f, 0f))
          .SetPosition(new(0f, -35f))
          .SetSizeDelta(new(0f, 35f));
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
          .SetSpacing(6f);

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
          .SetSpacing(8f);

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
      scrollRect.scrollSensitivity = 30f;

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

    //public GameObject CreateMessageDivider(Transform parentTransform) {
    //  GameObject divider = new("Message.Divider", typeof(RectTransform));
    //  divider.SetParent(parentTransform, worldPositionStays: false);

    //  divider.AddComponent<Image>()
    //      .SetSprite(UIBuilder.CreateRect(10, 10, Color.white))
    //      .SetType(Image.Type.Filled)
    //      .SetColor(new(1f, 1f, 1f, 0.0625f))
    //      .SetRaycastTarget(true)
    //      .SetMaskable(true);

    //  divider.AddComponent<LayoutElement>()
    //      .SetFlexible(width: 1f)
    //      .SetPreferred(height: 1f);

    //  return divider;
    //}

    //public GameObject CreateChatMessageRow(Transform parentTransform) {
    //  GameObject row = new(RowName, typeof(RectTransform));
    //  row.SetParent(parentTransform, worldPositionStays: false);

    //  row.AddComponent<VerticalLayoutGroup>()
    //      .SetChildControl(width: true, height: true)
    //      .SetChildForceExpand(width: false, height: false)
    //      .SetPadding(left: 0, right: 0, top: 0, bottom: 0)
    //      .SetSpacing(PluginConfig.ContentRowBodySpacing);

    //  row.AddComponent<ContentSizeFitter>()
    //      .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
    //      .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    //  return row;
    //}

    //public (GameObject header, TMP_Text leftCell, TMP_Text rightCell) CreateChatMessageRowHeader(
    //    Transform parentTransform, string leftText, string rightText) {
    //  GameObject header = new(RowHeaderName, typeof(RectTransform));
    //  header.SetParent(parentTransform, worldPositionStays: false);

    //  header.AddComponent<HorizontalLayoutGroup>()
    //      .SetChildControl(width: true, height: true)
    //      .SetChildForceExpand(width: false, height: false)
    //      .SetPadding(left: 0, right: 0, top: 0, bottom: 0);

    //  TMP_Text leftCell = UIBuilder.CreateLabel(header.transform);
    //  leftCell.name = HeaderLeftCellName;

    //  leftCell.text = leftText;
    //  leftCell.alignment = TextAlignmentOptions.Left;
    //  leftCell.fontSize -= 2f;

    //  leftCell.gameObject.AddComponent<LayoutElement>();

    //  GameObject spacer = new("Header.Spacer", typeof(RectTransform));
    //  spacer.SetParent(header.transform);
    //  spacer.AddComponent<LayoutElement>().SetFlexible(width: 1f);

    //  TMP_Text rightCell = UIBuilder.CreateLabel(header.transform);
    //  rightCell.name = HeaderRightCellName;

    //  rightCell.text = rightText;
    //  rightCell.alignment = TextAlignmentOptions.Right;
    //  rightCell.fontSize -= 2f;

    //  rightCell.gameObject.AddComponent<LayoutElement>();

    //  return (header, leftCell, rightCell);
    //}

    //public TMP_Text CreateChatMessageRowBody(Transform parentTransform, string text) {
    //  TMP_Text body = UIBuilder.CreateLabel(parentTransform);
    //  body.name = ContentRowBodyName;

    //  body.text = text;
    //  body.alignment = TextAlignmentOptions.Left;

    //  body.gameObject.AddComponent<LayoutElement>()
    //      .SetPreferred(width: _panelRectTransform.sizeDelta.x + _contentWidthOffset);

    //  return body;
    //}
  }
}
