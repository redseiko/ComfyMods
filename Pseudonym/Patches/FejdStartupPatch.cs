namespace Pseudonym;

using ComfyLib;

using HarmonyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  static Button _editButton;
  static PlayerProfile _editingPlayerProfile;
  static TextMeshProUGUI _editCharacterPanelTopicText;

  static TextMeshProUGUI _newCharacterPanelTopicText;
  static Button.ButtonClickedEvent _onNewCharacterDoneEvent;
  static PlayerCustomizaton _playerCustomization;

  static int _characterLimit;
  static TMP_InputField.ContentType _contentType;
  static TMP_InputValidator _inputValidator;

  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.Start))]
  static void StartPostfix(ref FejdStartup __instance) {
    CreateEditButton(__instance);

    _newCharacterPanelTopicText =
        __instance.m_newCharacterPanel.transform.Find("Topic").GetComponent<TextMeshProUGUI>();

    _editCharacterPanelTopicText =
        UnityEngine.Object.Instantiate(_newCharacterPanelTopicText, _newCharacterPanelTopicText.transform.parent);
    _editCharacterPanelTopicText.text = string.Empty;
    _editCharacterPanelTopicText.gameObject.SetActive(false);

    _onNewCharacterDoneEvent = __instance.m_csNewCharacterDone.onClick;

    _characterLimit = __instance.m_csNewCharacterName.characterLimit;
    _contentType = __instance.m_csNewCharacterName.contentType;
    _inputValidator = __instance.m_csNewCharacterName.inputValidator;

    _playerCustomization =
        __instance.m_newCharacterPanel.GetComponentInChildren<PlayerCustomizaton>(includeInactive: true);
  }

  static void CreateEditButton(FejdStartup fejdStartup) {
    UnityEngine.Object.Destroy(_editButton);

    _editButton =
        UnityEngine.Object.Instantiate(fejdStartup.m_csNewButton, fejdStartup.m_csNewButton.transform.parent);

    _editButton.onClick.RemoveAllListeners();
    _editButton.onClick.AddListener(() => OnEditCharacter(fejdStartup));

    _editButton.GetComponentInChildren<TextMeshProUGUI>().text = "Edit";
    _editButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(190f, 0f);
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.UpdateCharacterList))]
  static void UpdateCharacterList(FejdStartup __instance) {
    _editButton.Ref()?.gameObject.SetActive(__instance.m_profiles.Count > 0);
  }

  static void OnEditCharacter(FejdStartup fejdStartup) {
    if (fejdStartup.TryGetPlayerProfile(out PlayerProfile profile)) {
      Pseudonym.LogInfo($"Editing existing player: {profile.GetName()}");
      _editingPlayerProfile = profile;

      fejdStartup.m_newCharacterPanel.SetActive(true);
      fejdStartup.m_newCharacterError.SetActive(false);
      fejdStartup.m_selectCharacterPanel.SetActive(false);

      _newCharacterPanelTopicText.gameObject.SetActive(false);

      _editCharacterPanelTopicText.text = $"Edit Character: {profile.GetName()}";
      _editCharacterPanelTopicText.gameObject.SetActive(true);

      fejdStartup.m_csNewCharacterDone.onClick = new();
      fejdStartup.m_csNewCharacterDone.onClick.AddListener(() => OnEditCharacterDone(fejdStartup));

      fejdStartup.m_csNewCharacterName.characterLimit = 20;
      fejdStartup.m_csNewCharacterName.inputValidator = NameDigitInputValidator.Create();
      fejdStartup.m_csNewCharacterName.contentType = TMP_InputField.ContentType.Custom;
      fejdStartup.m_csNewCharacterName.characterValidation = TMP_InputField.CharacterValidation.CustomValidator;
      fejdStartup.m_csNewCharacterName.text = profile.GetName();

      fejdStartup.SetupCharacterPreview(profile);

      SetupPlayerCustomization(fejdStartup.m_playerInstance.Ref()?.GetComponent<Player>(), _playerCustomization);
    } else {
      _editingPlayerProfile = null;
    }
  }

  static void SetupPlayerCustomization(Player player, PlayerCustomizaton customization) {
    if (player && customization) {
      customization.m_maleToggle.SetIsOnWithoutNotify(player.m_modelIndex == 0);
      customization.m_femaleToggle.SetIsOnWithoutNotify(player.m_modelIndex == 1);

      float skinHue =
          InverseLerp(
              customization.m_skinColor0, customization.m_skinColor1, Utils.Vec3ToColor(player.m_skinColor));

      customization.m_skinHue.SetValueWithoutNotify(skinHue);

      float hairLevel = player.m_hairColor.x;
      Color hairColor = Utils.Vec3ToColor(player.m_hairColor / hairLevel);
      float hairTone = InverseLerp(customization.m_hairColor0, customization.m_hairColor1, hairColor);

      customization.m_hairTone.SetValueWithoutNotify(hairTone);
      customization.m_hairLevel.SetValueWithoutNotify(
          Mathf.InverseLerp(customization.m_hairMinLevel, customization.m_hairMaxLevel, hairLevel));
    } else {
      Pseudonym.LogError($"Could not setup player customization for editing.");
    }
  }

  static float InverseLerp(Vector4 a, Vector4 b, Vector4 value) {
    Vector4 ab = b - a;
    Vector4 av = value - a;

    return Vector4.Dot(ab, av) / Vector4.Dot(ab, ab);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(FejdStartup.OnNewCharacterCancel))]
  static void OnNewCharacterCancel(ref FejdStartup __instance) {
    _editingPlayerProfile = null;
    OnEditCharacterDone(__instance);
  }

  static void OnEditCharacterDone(FejdStartup fejdStartup) {
    if (_editingPlayerProfile != null) {
      string playerName = fejdStartup.m_csNewCharacterName.text;
      Pseudonym.LogInfo($"Saving existing player: {_editingPlayerProfile.GetName()} -> {playerName}");

      _editingPlayerProfile.SetName(playerName);
      _editingPlayerProfile.SavePlayerData(fejdStartup.m_playerInstance.GetComponent<Player>());
      _editingPlayerProfile.SavePlayerToDisk();
    }

    _editingPlayerProfile = null;

    _newCharacterPanelTopicText.gameObject.SetActive(true);
    _editCharacterPanelTopicText.gameObject.SetActive(false);

    fejdStartup.m_csNewCharacterDone.onClick = _onNewCharacterDoneEvent;
    fejdStartup.m_csNewCharacterName.text = string.Empty;
    fejdStartup.m_csNewCharacterName.characterLimit = _characterLimit;
    fejdStartup.m_csNewCharacterName.characterValidation = TMP_InputField.CharacterValidation.Name;
    fejdStartup.m_csNewCharacterName.contentType = TMP_InputField.ContentType.Name;
    fejdStartup.m_csNewCharacterName.inputValidator = _inputValidator;

    fejdStartup.m_selectCharacterPanel.SetActive(true);
    fejdStartup.m_newCharacterPanel.SetActive(false);
    fejdStartup.UpdateCharacterList();
  }
}
