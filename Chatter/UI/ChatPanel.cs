using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public class ChatPanel {
    public GameObject Panel { get; private set; }
    public CanvasGroup PanelCanvasGroup { get; private set; }

    public GameObject ContentViewport { get; private set; }
    public GameObject Content { get; private set; }
    public ScrollRect ContentScrollRect { get; private set; }

    public InputFieldCell TextInput { get; private set; }

    public CircularQueue<MessageCell> ContentMessages { get; private set; }

    //public GameObject Grabber { get; init; }

    public ChatPanel(Transform parentTransform) {
      Panel = CreateChildPanel(parentTransform);
      PanelCanvasGroup = Panel.GetComponent<CanvasGroup>();

      ContentViewport = CreateChildViewport(Panel.transform);
      Content = CreateChildContent(ContentViewport.transform);
      ContentScrollRect = CreateChildScrollRect(ContentViewport, Content);

      TextInput = new(Panel.transform);

      TextInput.Cell.AddComponent<LayoutElement>()
          .SetFlexible(width: 1f)
          .SetPreferred(height: 35f);

      ContentMessages = new(64, DestroyContentMessage);

    //  Grabber = CreateGrabber(Panel.transform);
    //  _grabberCanvasGroup = Grabber.GetComponent<CanvasGroup>();

    //  _contentWidthOffset = _contentLayoutGroup.padding.horizontal * -1f;
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
          .SetSpacing(8f);

      panel.AddComponent<Image>()
          .SetType(Image.Type.Sliced)
          .SetSprite(UIBuilder.CreateSuperellipse(400, 400, 15))
          .SetColor(new(0f, 0f, 0f, 0.4f));

      panel.AddComponent<CanvasGroup>()
          .SetAlpha(1f)
          .SetBlocksRaycasts(true);

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
      scrollRect.movementType = ScrollRect.MovementType.Clamped;
      scrollRect.scrollSensitivity = 30f;

      return scrollRect;
    }

    public MessageCell CreateContentMessage() {
      MessageCell cell = new(Content.transform);
      ContentMessages.EnqueueItem(cell);

      return cell;
    }

    void DestroyContentMessage(MessageCell contentMessage) {
      UnityEngine.Object.Destroy(contentMessage.Cell);
    }

    //public Toggle SayToggle { get; private set; } = default!;
    //public Toggle ShoutToggle { get; private set; } = default!;
    //public Toggle WhisperToggle { get; private set; } = default!;
    //public Toggle PingToggle { get; private set; } = default!;
    //public Toggle MessageHudToggle { get; private set; } = default!;
    //public Toggle TextToggle { get; private set; } = default!;

    //public void ToggleGrabber(bool toggle) {
    //  _grabberCanvasGroup.alpha = toggle ? 1f : 0f;
    //  _grabberCanvasGroup.blocksRaycasts = toggle;
    //}

    //public void SetVerticalScrollPosition(float position) {
    //  ScrollRect.verticalNormalizedPosition = position;
    //}

    //public void OffsetVerticalScrollPosition(float offset) {
    //  float percent = (offset / _contentRectTransform.sizeDelta.y);
    //  ScrollRect.verticalNormalizedPosition += percent;
    //}

    //GameObject CreateGrabber(Transform parentTransform) {
    //  GameObject grabber = new("ChatPanel.Grabber", typeof(RectTransform));
    //  grabber.SetParent(parentTransform);

    //  grabber.GetComponent<RectTransform>()
    //      .SetAnchorMin(Vector2.zero)
    //      .SetAnchorMax(Vector2.zero)
    //      .SetPivot(Vector2.zero)
    //      .SetPosition(Vector2.zero);

    //  grabber.AddComponent<HorizontalLayoutGroup>()
    //      .SetChildControl(width: true, height: true)
    //      .SetChildForceExpand(width: false, height: false);

    //  grabber.AddComponent<Image>()
    //      .SetSprite(UIBuilder.CreateRect(10, 10, Color.white))
    //      .SetType(Image.Type.Filled)
    //      .SetColor(new(1f, 1f, 1f, 0.125f));

    //  grabber.AddComponent<CanvasGroup>();

    //  CreateGrabberResizer(grabber.transform);
    //  CreateGrabberDragger(grabber.transform);
    //  CreateShowMessageTypeTogglesRow(grabber.transform);

    //  return grabber;
    //}

    //GameObject CreateGrabberResizer(Transform parentTransform) {
    //  TMP_Text resizerLabel = UIBuilder.CreateLabel(parentTransform);

    //  GameObject resizer = resizerLabel.gameObject;
    //  resizer.name = "Grabber.Resizer";

    //  resizer.AddComponent<LayoutElement>()
    //      .SetPreferred(width: 30f, height: 30f);

    //  resizer.AddComponent<PanelResizer>();

    //  resizerLabel.fontSize = 18f;
    //  resizerLabel.alignment = TextAlignmentOptions.Center;
    //  resizerLabel.color = Color.white;
    //  resizerLabel.text = "\u2922";

    //  return resizer;
    //}

    //GameObject CreateGrabberDragger(Transform parentTransform) {
    //  GameObject dragger = new("Grabber.Dragger", typeof(RectTransform));
    //  dragger.SetParent(parentTransform.transform);

    //  dragger.AddComponent<LayoutElement>()
    //      .SetFlexible(width: 1f)
    //      .SetPreferred(height: 30f);

    //  dragger.AddComponent<Image>()
    //      .SetSprite(UIBuilder.CreateRect(10, 10, Color.white))
    //      .SetType(Image.Type.Filled)
    //      .SetColor(new(0.75f, 0.75f, 0.75f, 0.1875f))
    //      .SetRaycastTarget(true);

    //  dragger.AddComponent<PanelDragger>();

    //  return dragger;
    //}

    //GameObject CreateShowMessageTypeTogglesRow(Transform parentTransform) {
    //  GameObject row = new("Show.Toggles.Row", typeof(RectTransform));
    //  row.SetParent(parentTransform.transform);

    //  row.AddComponent<HorizontalLayoutGroup>()
    //      .SetChildControl(width: true, height: true)
    //      .SetChildForceExpand(width: false, height: false)
    //      .SetPadding(left: 10, right: 10, top: 2, bottom: 2)
    //      .SetSpacing(10f);

    //  row.AddComponent<ContentSizeFitter>()
    //      .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
    //      .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    //  row.AddComponent<Image>()
    //      .SetSprite(UIBuilder.CreateRect(10, 10, Color.white))
    //      .SetType(Image.Type.Filled)
    //      .SetColor(new(0.75f, 0.75f, 0.75f, 0.125f));

    //  SayToggle = CreateMessageTypeToggle(row.transform, "Say");
    //  ShoutToggle = CreateMessageTypeToggle(row.transform, "Shout");
    //  WhisperToggle = CreateMessageTypeToggle(row.transform, "Whisper");
    //  PingToggle = CreateMessageTypeToggle(row.transform, "Ping");
    //  MessageHudToggle = CreateMessageTypeToggle(row.transform, "Hud");
    //  TextToggle = CreateMessageTypeToggle(row.transform, "Text");

    //  return row;
    //}

    //readonly List<TMP_Text> _toggleTextLabels = new();

    //public void SetupMessageTypeToggles() {
    //  foreach (TMP_Text label in _toggleTextLabels) {
    //    label.fontSize = PluginConfig.MessageToggleTextFontSize.Value;
    //  }
    //}

    //Toggle CreateMessageTypeToggle(Transform parentTransform, string label) {
    //  TMP_Text toggleText = UIBuilder.CreateLabel(parentTransform);
    //  _toggleTextLabels.Add(toggleText);

    //  GameObject togglePrefab = toggleText.gameObject;
    //  togglePrefab.name = $"Toggle.{label}";

    //  togglePrefab.AddComponent<LayoutElement>()
    //      .SetPreferred(height: 30f);

    //  toggleText.fontSize = PluginConfig.MessageToggleTextFontSize.Value;
    //  toggleText.color = PluginConfig.MessageToggleTextColorEnabled.Value;
    //  toggleText.alignment = TextAlignmentOptions.Center;
    //  toggleText.text = label;

    //  Toggle toggle = togglePrefab.AddComponent<Toggle>();
    //  toggle.targetGraphic = toggleText;

    //  toggle.onValueChanged.AddListener(
    //      isOn => {
    //        toggleText.color =
    //            isOn
    //                ? PluginConfig.MessageToggleTextColorEnabled.Value
    //                : PluginConfig.MessageToggleTextColorDisabled.Value;
    //      });

    //  toggle.isOn = false;

    //  return toggle;
    //}

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
