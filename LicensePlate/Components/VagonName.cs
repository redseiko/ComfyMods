namespace LicensePlate;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public sealed class VagonName : MonoBehaviour, TextReceiver {
  private ZNetView _netView;
  private Chat.NpcText _npcText;
  private Vagon _vagon;

  private string _vagonName = string.Empty;
  private string _vagonNameCache = string.Empty;

  private void Awake() {
    _vagon = GetComponent<Vagon>();
    _netView = GetComponent<ZNetView>();

    if (!_vagon || !_netView || !_netView.IsValid()) {
      return;
    }

    InvokeRepeating(nameof(UpdateVagonName), 0f, 1f);
  }

  private void UpdateVagonName() {
    if (!_netView || !_netView.IsValid() || !IsModEnabled.Value || !ShowCartNames.Value) {
      ClearNpcText();
      CancelInvoke(nameof(UpdateVagonName));
      return;
    }

    if (!Player.m_localPlayer) {
      ClearNpcText();
      return;
    }

    _vagonName = _netView.m_zdo.GetString(LicensePlate.VagonLicensePlateHashCode, string.Empty);

    float distance =
        Vector3.Distance(
            Player.m_localPlayer.transform.position + _vagon.m_attachOffset, _vagon.m_attachPoint.position);

    if (_npcText?.m_gui && _vagonName.Length > 0 && distance > CartNameMinimumDistance.Value) {
      UpdateNpcTextValue(_vagonName);
    } else {
      ClearNpcText();

      if (_vagonName.Length > 0
          && Player.m_localPlayer
          && distance > CartNameMinimumDistance.Value
          && distance < CartNameCutoffDistance.Value) {
        SetNpcText(_vagonName);
      }
    }
  }

  private void ClearNpcText() {
    if (_npcText != null) {
      Chat.m_instance.ClearNpcText(_npcText);
      _npcText = null;
    }

    _vagonNameCache = string.Empty;
  }

  private void SetNpcText(string vagonName) {
    Chat.m_instance.SetNpcText(
        gameObject,
        CartNameDisplayOffset.Value,
        CartNameCutoffDistance.Value,
        CartNameTimeToLive.Value,
        string.Empty,
        GetSanitizedVagonName(vagonName),
        false);

    _vagonNameCache = vagonName;
    _npcText = Chat.m_instance.FindNpcText(gameObject);

    if (_npcText?.m_gui) {
      CustomizeNpcText();
    }
  }

  private void UpdateNpcTextValue(string vagonName) {
    if (vagonName == _vagonNameCache) {
      return;
    }

    _vagonNameCache = vagonName;
    _npcText.m_textField.text = GetSanitizedVagonName(vagonName);
  }

  private string GetSanitizedVagonName(string vagonName) {
    if (vagonName.Length > 64) {
      vagonName = vagonName.Substring(0, 64);
    }

    if (CartNameStripHtmlTags.Value) {
      vagonName = LicensePlate.HtmlTagsRegex.Replace(vagonName, string.Empty);
    }

    return vagonName;
  }

  private void CustomizeNpcText() {
    _npcText.m_textField.enableAutoSizing = false;
    _npcText.m_textField.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
    _npcText.m_textField.overflowMode = TMPro.TextOverflowModes.Overflow;
    _npcText.m_textField.fontSize = CartNameFontSize.Value;
    _npcText.m_textField.fontSizeMax = 64f;

    CustomizeNpcTextBackground(_npcText.m_gui.transform.Find("Image").gameObject);
  }

  private void CustomizeNpcTextBackground(GameObject background) {
    RectTransform rectTransform = background.GetComponent<RectTransform>();
    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60f);

    Image image = background.GetComponent<Image>();
    Color color = image.color;
    color.a = 0.5f;

    image.color = color;
  }

  public string GetText() {
    return _netView && _netView.IsValid()
        ? _netView.m_zdo.GetString(LicensePlate.VagonLicensePlateHashCode, string.Empty)
        : string.Empty;
  }

  public void SetText(string text) {
    if (_netView && _netView.IsValid() && Player.m_localPlayer) {
      LicensePlate.LogInfo($"Setting Vagon ({_netView.m_zdo.m_uid}) name to: {text}");

      _netView.m_zdo.Set(LicensePlate.VagonLicensePlateHashCode, text);
      _netView.m_zdo.Set(LicensePlate.LicensePlateLastSetByHashCode, Player.m_localPlayer.GetPlayerID());

      UpdateVagonName();
    }
  }
}
