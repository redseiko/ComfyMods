using System;

using ComfyLib;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ColorfulPieces {
  public sealed class ColorPicker {
    public RectTransform Panel { get; private set; }
    public Image HueImage { get; private set; }
    public Image SaturationImage { get; private set; }
    public Image ColorImage { get; private set; }

    float _hue;
    float _saturation;
    float _value;

    readonly RectTransform _hueRectTransform;
    readonly Texture2D _hueTexture;

    readonly RectTransform _saturationRectTransform;
    readonly Texture2D _saturationTexture;

    public ColorPicker(Transform parentTransform) {
      Panel = CreatePanel(parentTransform);
      HueImage = CreateHueImage(Panel);
      SaturationImage = CreateSaturationImage(Panel);
      ColorImage = CreateColorImage(Panel);

      _hueRectTransform = HueImage.GetComponent<RectTransform>();
      _hueTexture = HueImage.sprite.texture;

      _saturationRectTransform = SaturationImage.GetComponent<RectTransform>();
      _saturationTexture = SaturationImage.sprite.texture;

      HueInteraction hueInteraction = HueImage.gameObject.AddComponent<HueInteraction>();
      hueInteraction.OnHueClicked += OnHueClicked;

      SaturationInteraction saturationInteraction = SaturationImage.gameObject.AddComponent<SaturationInteraction>();
      saturationInteraction.OnSaturationClicked += OnSaturationClicked;

      Color.RGBToHSV(_hueColors[0], out _hue, out _saturation, out _value);

      ResetSaturationTexture(_saturationTexture);
      ApplySaturationValue(_saturation, _value, ColorImage);
    }

    void OnHueClicked(object sender, Vector2 position) {
      RectTransformUtility.ScreenPointToLocalPointInRectangle(
          _hueRectTransform, position, null, out Vector2 localPoint);

      Rect r = _hueRectTransform.rect;

      _hue = (((localPoint.y - r.y) * _hueTexture.height) / r.height);
      int i0 = Mathf.Clamp((int) _hue, 0, 5);
      int i1 = (i0 + 1) % 6;
      Color color = Color.Lerp(_hueColors[i0], _hueColors[i1], _hue - i0);

      //ZLog.Log($"Clicked on {position} --> ({hue}, {i0}, {i1}) --> {color})");
      _saturationColors[3] = color;

      ResetSaturationTexture(_saturationTexture);
      ApplySaturationValue(_saturation, _value, ColorImage);
    }

    void OnSaturationClicked(object sender, Vector2 position) {
      RectTransformUtility.ScreenPointToLocalPointInRectangle(
          _saturationRectTransform, position, null, out Vector2 localPoint);

      Rect r = _saturationRectTransform.rect;

      _saturation = (((localPoint.x - r.x) * _saturationTexture.width) / r.width);
      _value = (((localPoint.y - r.y) * _saturationTexture.height) / r.height);

      ZLog.Log($"SaturationClicked: {position} --> {localPoint} --> S: {_saturation}, V: {_value}");

      ApplySaturationValue(_saturation, _value, ColorImage);
    }

    static void ApplySaturationValue(float saturation, float value, Image image) {
      Vector2 sv = new(saturation, value);
      Vector2 isv = new(1 - sv.x, 1 - sv.y);

      Color c0 = isv.x * isv.y * _saturationColors[0];
      Color c1 = sv.x * isv.y * _saturationColors[1];
      Color c2 = isv.x * sv.y * _saturationColors[2];
      Color c3 = sv.x * sv.y * _saturationColors[3];
      Color resultColor = c0 + c1 + c2 + c3;

      image.color = resultColor;
    }

    static RectTransform CreatePanel(Transform parentTransform) {
      GameObject panel = new("Panel", typeof(RectTransform));
      panel.transform.SetParent(parentTransform, worldPositionStays: false);

      panel.AddComponent<Canvas>();

      return panel.GetComponent<RectTransform>()
          .SetAnchorMin(new(0.5f, 0.5f))
          .SetAnchorMax(new(0.5f, 0.5f))
          .SetPivot(new(0.5f, 0.5f))
          .SetPosition(Vector2.zero)
          .SetSizeDelta(new(320f, 320f));
    }

    static Image CreateHueImage(Transform parentTransform) {
      GameObject hue = new("HueImage", typeof(RectTransform));
      hue.transform.SetParent(parentTransform, worldPositionStays: false);

      hue.GetComponent<RectTransform>()
          .SetAnchorMin(new(1f, 0f))
          .SetAnchorMax(new(1f, 1f))
          .SetPivot(new(1f, 0.5f))
          .SetPosition(new(-15f, 0f))
          .SetSizeDelta(new(40f, -30f));

      Image image =
          hue.AddComponent<Image>()
              .SetType(Image.Type.Simple)
              .SetSprite(CreateHueSprite())
              .SetRaycastTarget(true);

      return image;
    }

    static Sprite CreateHueSprite() {
      Sprite hueSprite = Sprite.Create(CreateHueTexture(), new(0f, 0.5f, 1, 6), new(0.5f, 0.5f));
      hueSprite.name = "HueSprite";

      return hueSprite;
    }

    static readonly Color[] _hueColors =
        new Color[] {
          Color.red,
          Color.yellow,
          Color.green,
          Color.cyan,
          Color.blue,
          Color.magenta,
        };

    static Texture2D CreateHueTexture() {
      Texture2D hueTexture = new(1, 7) {
        name = "HueTexture"
      };

      for (int i = 0; i < 7; i++) {
        hueTexture.SetPixel(0, i, _hueColors[i % 6]);
      }

      hueTexture.Apply();

      return hueTexture;
    }

    static Image CreateSaturationImage(Transform parentTransform) {
      GameObject saturation = new("SaturationImage", typeof(RectTransform));
      saturation.transform.SetParent(parentTransform, worldPositionStays: false);

      saturation.GetComponent<RectTransform>()
          .SetAnchorMin(new(0f, 0f))
          .SetAnchorMax(new(1f, 1f))
          .SetPivot(new(0f, 0f))
          .SetPosition(new(15f, 60f))
          .SetSizeDelta(new(-80f, -75f));

      Image image =
          saturation.AddComponent<Image>()
              .SetType(Image.Type.Simple)
              .SetSprite(CreateSaturationSprite())
              .SetRaycastTarget(true);

      return image;
    }

    static Sprite CreateSaturationSprite() {
      Texture2D saturationTexture = new(2, 2) {
        name = "SaturationTexture"
      };

      Sprite saturationSprite = Sprite.Create(saturationTexture, new(0.5f, 0.5f, 1f, 1f), new(0.5f, 0.5f));
      saturationSprite.name = "SaturationSprite";

      return saturationSprite;
    }

    static readonly Color[] _saturationColors =
        new Color[] {
          new(0f, 0f, 0f),
          new(0f, 0f, 0f),
          new(1f, 1f, 1f),
          _hueColors[0],
        };

    static void ResetSaturationTexture(Texture2D saturationTexture) {
      for (int j = 0; j < 2; j++) {
        for (int i = 0; i < 2; i++) {
          saturationTexture.SetPixel(i, j, _saturationColors[i + j * 2]);
        }
      }

      saturationTexture.Apply();
    }

    sealed class HueInteraction : MonoBehaviour, IDragHandler, IPointerDownHandler {
      public event EventHandler<Vector2> OnHueClicked;

      public void OnDrag(PointerEventData eventData) {
        OnHueClicked?.Invoke(this, eventData.position);
      }

      public void OnPointerDown(PointerEventData eventData) {
        OnHueClicked?.Invoke(this, eventData.position);
      }
    }

    sealed class SaturationInteraction : MonoBehaviour, IDragHandler, IPointerDownHandler {
      public event EventHandler<Vector2> OnSaturationClicked;

      public void OnDrag(PointerEventData eventData) {
        OnSaturationClicked?.Invoke(this, eventData.position);
      }

      public void OnPointerDown(PointerEventData eventData) {
        OnSaturationClicked?.Invoke(this, eventData.position);
      }
    }

    static Image CreateColorImage(Transform parentTransform) {
      GameObject color = new("ColorImage", typeof(RectTransform));
      color.transform.SetParent(parentTransform, worldPositionStays: false);

      color.GetComponent<RectTransform>()
          .SetAnchorMin(new(0f, 0f))
          .SetAnchorMax(new(0f, 0f))
          .SetPivot(new(0f, 0f))
          .SetPosition(new(15f, 15f))
          .SetSizeDelta(new(100f, 30f));

      Image image =
          color.AddComponent<Image>()
              .SetType(Image.Type.Simple)
              .SetColor(_hueColors[0]);

      return image;
    }
  }
}
