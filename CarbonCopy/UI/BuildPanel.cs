using System;
using UnityEngine;
using UnityEngine.UI;

namespace CarbonCopy {
  public class BuildPanel {
    DefaultControls.Resources _resources;

    public GameObject Panel { get; private set; }
    public GameObject Header { get; private set; }
    public GameObject Content { get; private set; }

    public InputField OriginInputField { get; private set; }
    public Button PinOriginButton { get; private set; }

    public Slider RadiusSlider { get; private set; }
    public InputField RadiusInputField { get; private set; }
    public Toggle ShowRadiusSphereToggle { get; private set; }

    public BuildPanel(Transform parentTransform) {
      _resources = UIResources.CreateResources();
      Panel = CreatePanel(parentTransform);
      Header = CreateHeader(Panel.transform, "CarbonCopy");
      Content = CreateContent(Panel.transform);

      CreateFilenameRow(Content.transform);
      CreateOriginRow(Content.transform);
      CreateRadiusRow(Content.transform);
      CreateRadiusOptionsRow(Content.transform);
    }

    GameObject CreatePanel(Transform parentTransform) {
      GameObject panel = DefaultControls.CreatePanel(_resources);
      panel.transform.SetParent(parentTransform, worldPositionStays: false);
      panel.name = "Panel";

      RectTransform panelTransform = panel.GetComponent<RectTransform>();
      panelTransform.anchorMin = new Vector2(0f, 0.5f);
      panelTransform.anchorMax = new Vector2(0f, 0.5f);
      panelTransform.pivot = new Vector2(0f, 0.5f);
      panelTransform.anchoredPosition = new Vector2(10f, 0f);

      panel.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetChildAlignment(TextAnchor.MiddleCenter)
          .SetPadding(left: 5, right: 5, top: 10, bottom: 10)
          .SetSpacing(8f);

      ContentSizeFitter panelFitter = panel.AddComponent<ContentSizeFitter>();
      panelFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
      panelFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

      panelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400f);

      return panel;
    }

    GameObject CreateHeader(Transform parentTransform, string headerLabel) {
      GameObject header = new("Header", typeof(RectTransform), typeof(HorizontalLayoutGroup));
      header.transform.SetParent(parentTransform, worldPositionStays: false);

      RectTransform headerTransform = header.GetComponent<RectTransform>();
      headerTransform.anchorMin = new Vector2(0.5f, 0.5f);
      headerTransform.anchorMax = new Vector2(0.5f, 0.5f);
      headerTransform.pivot = new Vector2(0.5f, 0.5f);

      header.GetComponent<HorizontalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetChildAlignment(TextAnchor.MiddleCenter)
          .SetPadding(left: 5, right: 5, top: 0, bottom: 0)
          .SetSpacing(8f);

      CreateIndicator(header.transform, new Color32(123, 36, 28, 255));

      CreateLabel(header.transform)
          .GetComponent<Text>()
          .SetAlignment(TextAnchor.MiddleCenter)
          .SetFontSize(16)
          .SetText(headerLabel);

      CreateSpacer(header.transform, flexibleWidth: 1f);

      return header;
    }

    GameObject CreateIndicator(Transform parentTransform, Color indicatorColor) {
      GameObject indicator = DefaultControls.CreateImage(_resources);
      indicator.transform.SetParent(parentTransform, worldPositionStays: false);
      indicator.name = "Indicator";

      Image indicatorImage = indicator.GetComponent<Image>();
      indicatorImage.color = indicatorColor;
      indicatorImage.raycastTarget = true;
      indicatorImage.maskable = true;

      indicator.AddComponent<LayoutElement>()
          .SetPreferred(width: 4f)
          .SetFlexible(height: 1f);

      return indicator;
    }

    GameObject CreateContent(Transform parentTransform) {
      GameObject content = new("Content", typeof(RectTransform), typeof(VerticalLayoutGroup));
      content.transform.SetParent(parentTransform, worldPositionStays: false);

      content.GetComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetChildAlignment(TextAnchor.MiddleCenter)
          .SetPadding(left: 5, right: 5, top: 0, bottom: 0)
          .SetSpacing(8f);

      CreateDivider(content.transform, new Color32(123, 36, 28, 255));

      return content;
    }

    GameObject CreateFilenameRow(Transform parentTransform) {
      GameObject row = new("FilenameRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
      row.transform.SetParent(parentTransform, worldPositionStays: false);

      row.GetComponent<HorizontalLayoutGroup>()
          .SetChildControl(width: true, height: false)
          .SetChildForceExpand(width: false, height: false)
          .SetPadding(left: 5, right: 5, top: 0, bottom: 0)
          .SetSpacing(8f);

      CreateLabel(row.transform)
          .GetComponent<Text>()
          .SetAlignment(TextAnchor.MiddleLeft)
          .SetText("Filename");

      GameObject inputFieldControl = CreateInputField(row.transform, "carbon.build");

      inputFieldControl.AddComponent<LayoutElement>()
          .SetFlexible(width: 1f);

      GameObject buttonControl = CreateButton(row.transform, "Save Build");

      InputField inputField = inputFieldControl.GetComponent<InputField>();
      Button button = buttonControl.GetComponent<Button>();

      inputField.onValueChanged.AddListener(value => button.interactable = value.Length > 0);
      button.interactable = false;

      return row;
    }

    GameObject CreateOriginRow(Transform parentTransform) {
      GameObject row = new("OriginRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
      row.transform.SetParent(parentTransform, worldPositionStays: false);

      row.GetComponent<HorizontalLayoutGroup>()
          .SetChildControl(width: true, height: false)
          .SetChildForceExpand(width: false, height: false)
          .SetChildAlignment(TextAnchor.MiddleCenter)
          .SetPadding(left: 5, right: 5, top: 0, bottom: 0)
          .SetSpacing(8f);

      CreateLabel(row.transform)
          .GetComponent<Text>()
          .SetAlignment(TextAnchor.MiddleLeft)
          .SetText("Origin");

      GameObject origin = CreateInputField(row.transform, "XYZ");

      origin.AddComponent<LayoutElement>()
          .SetFlexible(width: 1f);

      OriginInputField = origin.GetComponent<InputField>();
      OriginInputField.readOnly = true;

      PinOriginButton = CreateButton(row.transform, "Pin").GetComponent<Button>();

      return row;
    }

    GameObject CreateRadiusRow(Transform parentTransform) {
      GameObject row = new("RadiusRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
      row.transform.SetParent(parentTransform, worldPositionStays: false);

      row.GetComponent<HorizontalLayoutGroup>()
          .SetChildControl(width: true, height: false)
          .SetChildForceExpand(width: false, height: false)
          .SetChildAlignment(TextAnchor.MiddleCenter)
          .SetPadding(left: 5, right: 5, top: 0, bottom: 0)
          .SetSpacing(8f);

      CreateLabel(row.transform)
          .GetComponent<Text>()
          .SetAlignment(TextAnchor.MiddleLeft)
          .SetText("Radius");

      GameObject radiusSliderControl = CreateSlider(row.transform);
      radiusSliderControl
          .AddComponent<LayoutElement>()
          .SetFlexible(width: 0.75f);

      GameObject radiusInputFieldControl = CreateInputField(row.transform, "10");
      radiusInputFieldControl
          .AddComponent<LayoutElement>()
          .SetFlexible(width: 0.25f);

      RadiusSlider = radiusSliderControl.GetComponent<Slider>();
      RadiusSlider.value = 10f;
      RadiusSlider.minValue = 1f;
      RadiusSlider.maxValue = 128f;

      RadiusInputField = radiusInputFieldControl.GetComponent<InputField>();
      RadiusInputField.text = "10";

      RadiusSlider.onValueChanged.AddListener(value => RadiusInputField.text = value.ToString());
      RadiusInputField.onValueChanged.AddListener(
          value => {
            if (float.TryParse(value, out float radius)) {
              RadiusSlider.SetValueWithoutNotify(Mathf.Clamp(radius, RadiusSlider.minValue, RadiusSlider.maxValue));
            }
          });

      return row;
    }

    GameObject CreateRadiusOptionsRow(Transform parentTransform) {
      GameObject row = new("RadiusOptionsRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
      row.transform.SetParent(parentTransform, worldPositionStays: false);

      row.GetComponent<HorizontalLayoutGroup>()
          .SetChildControl(width: true, height: false)
          .SetChildForceExpand(width: false, height: false)
          .SetChildAlignment(TextAnchor.MiddleCenter)
          .SetPadding(left: 5, right: 5, top: 0, bottom: 0)
          .SetSpacing(8f);

      GameObject toggleControl = CreateToggle(row.transform, "Show radius sphere");
      toggleControl.AddComponent<LayoutElement>().SetFlexible(width: 1f);

      ShowRadiusSphereToggle = toggleControl.GetComponent<Toggle>();
      ShowRadiusSphereToggle.isOn = false;

      return row;
    }

    GameObject CreateSpacer(Transform parentTransform, float flexibleWidth = 0, float flexibleHeight = 0) {
      GameObject spacer = new("Spacer", typeof(RectTransform), typeof(LayoutElement));
      spacer.transform.SetParent(parentTransform, worldPositionStays: false);

      LayoutElement spacerLayout = spacer.GetComponent<LayoutElement>();
      spacerLayout.flexibleWidth = flexibleWidth;
      spacerLayout.flexibleHeight = flexibleHeight;

      return spacer;
    }

    GameObject CreateDivider(Transform parentTransform, Color dividerColor) {
      GameObject divider = DefaultControls.CreateImage(_resources);
      divider.transform.SetParent(parentTransform, worldPositionStays: false);
      divider.name = "Divider";

      Image indicatorImage = divider.GetComponent<Image>();
      indicatorImage.color = dividerColor;
      indicatorImage.raycastTarget = true;
      indicatorImage.maskable = true;

      LayoutElement indicatorLayout = divider.AddComponent<LayoutElement>();
      indicatorLayout.flexibleWidth = 2;
      indicatorLayout.preferredHeight = 2;

      return divider;
    }

    GameObject CreateInputField(Transform parentTransform, string placeholderText) {
      GameObject inputFieldControl = DefaultControls.CreateInputField(_resources);
      inputFieldControl.transform.SetParent(parentTransform, worldPositionStays: false);

      InputField inputField = inputFieldControl.GetComponent<InputField>();

      inputField.placeholder.GetComponent<Text>()
        .SetFont(inputField.textComponent.font)
        .SetText(placeholderText);

      return inputFieldControl;
    }

    GameObject CreateSlider(Transform parentTransform) {
      GameObject sliderControl = DefaultControls.CreateSlider(_resources);
      sliderControl.transform.SetParent(parentTransform, worldPositionStays: false);

      return sliderControl;
    }

    GameObject CreateToggle(Transform parentTransform, string toggleLabel) {
      GameObject toggleControl = DefaultControls.CreateToggle(_resources);
      toggleControl.transform.SetParent(parentTransform, worldPositionStays: false);
      toggleControl.GetComponentInChildren<Text>(includeInactive: false)?.SetText(toggleLabel);

      return toggleControl;
    }

    GameObject CreateButton(Transform parentTransform, string buttonLabel) {
      GameObject buttonControl = DefaultControls.CreateButton(_resources);
      buttonControl.transform.SetParent(parentTransform, worldPositionStays: false);

      buttonControl.AddComponent<HorizontalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetChildAlignment(TextAnchor.MiddleCenter)
          .SetPadding(left: 10, right: 10, top: 5, bottom: 5);

      buttonControl.GetComponentInChildren<Text>(includeInactive: false).SetText(buttonLabel);

      return buttonControl;
    }

    GameObject CreateLabel(Transform parentTransform) {
      GameObject textControl = DefaultControls.CreateText(_resources);
      textControl.transform.SetParent(parentTransform, worldPositionStays: false);

      return textControl;
    }
  }
}
