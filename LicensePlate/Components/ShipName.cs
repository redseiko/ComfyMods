namespace LicensePlate;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public sealed class ShipName : MonoBehaviour, TextReceiver {
  private ZNetView _netView;
  private Chat.NpcText _npcText;

  private ShipControlls _shipControlls;
  private Ship _ship;

  private string _shipName = string.Empty;
  private string _shipNameCache = string.Empty;

  private void Awake() {
    _shipControlls = GetComponent<ShipControlls>();
    _ship = _shipControlls.Ref()?.m_ship;
    _netView = _shipControlls.Ref()?.m_nview;

    if (!_shipControlls || !_ship || !_netView || !_netView.IsValid()) {
      return;
    }

    InvokeRepeating(nameof(UpdateShipName), 0f, 1f);
  }

  private void UpdateShipName() {
    if (!_netView || !_netView.IsValid() || !_ship || !IsModEnabled.Value || !ShowShipNames.Value) {
      ClearNpcText();
      CancelInvoke(nameof(UpdateShipName));
      return;
    }

    if (!Player.m_localPlayer) {
      ClearNpcText();
      return;
    }

    _shipName = _netView.m_zdo.GetString(LicensePlate.ShipLicensePlateHashCode, string.Empty);
    float distance = Vector3.Distance(Player.m_localPlayer.transform.position, gameObject.transform.position);

    if (_npcText?.m_gui && _shipName.Length > 0 && distance > ShipNameMinimumDistance.Value) {
      UpdateNpcTextValue(_shipName);
    } else {
      ClearNpcText();

      if (_shipName.Length > 0
          && Player.m_localPlayer
          && distance > ShipNameMinimumDistance.Value
          && distance < ShipNameCutoffDistance.Value) {
        SetNpcText(_shipName);
      }
    }
  }

  private void ClearNpcText() {
    if (_npcText != null) {
      Chat.m_instance.ClearNpcText(_npcText);
      _npcText = null;
    }
  }

  private void SetNpcText(string shipName) {
    Chat.m_instance.SetNpcText(
        _ship.gameObject,
        ShipNameDisplayOffset.Value,
        ShipNameCutoffDistance.Value,
        ShipNameTimeToLive.Value,
        string.Empty,
        GetSanitizedShipName(shipName),
        false);

    _shipNameCache = shipName;
    _npcText = Chat.m_instance.FindNpcText(_ship.gameObject);

    if (_npcText?.m_gui) {
      CustomizeNpcText();
    }
  }

  public void UpdateNpcTextValue(string shipName) {
    if (shipName == _shipNameCache) {
      return;
    }

    _shipNameCache = shipName;
    _npcText.m_textField.text = GetSanitizedShipName(shipName);
  }

  private string GetSanitizedShipName(string shipName) {
    if (shipName.Length > 64) {
      shipName = shipName.Substring(0, 64);
    }

    if (ShipNameStripHtmlTags.Value) {
      shipName = LicensePlate.HtmlTagsRegex.Replace(shipName, string.Empty);
    }

    return shipName;
  }

  private void CustomizeNpcText() {
    _npcText.m_textField.enableAutoSizing = false;
    _npcText.m_textField.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
    _npcText.m_textField.overflowMode = TMPro.TextOverflowModes.Overflow;
    _npcText.m_textField.fontSize = ShipNameFontSize.Value;
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
        ? _netView.m_zdo.GetString(LicensePlate.ShipLicensePlateHashCode, string.Empty)
        : string.Empty;
  }

  public void SetText(string text) {
    if (_netView && _netView.IsValid() && Player.m_localPlayer) {
      LicensePlate.LogInfo($"Setting Ship ({_netView.m_zdo.m_uid}) name to: {text}");

      _netView.m_zdo.Set(LicensePlate.ShipLicensePlateHashCode, text);
      _netView.m_zdo.Set(LicensePlate.LicensePlateLastSetByHashCode, Player.m_localPlayer.GetPlayerID());

      UpdateShipName();
    }
  }
}
