namespace OdinSaves;

using System;
using System.Collections;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public static class SaveManager {
  static readonly ZPackage _mapPackage = new();

  static byte[] CompressMapData(ref byte[] mapData) {
    if (mapData == null || IsCompressedMapData(mapData)) {
      return mapData;
    }

    _mapPackage.Clear();
    _mapPackage.Write(Minimap.MAPVERSION);
    _mapPackage.Write(Utils.Compress(mapData));

    return _mapPackage.GetArray();
  }

  static bool IsCompressedMapData(byte[] data) {
    return data != null && data.Length >= 4 && BitConverter.ToInt32(data, startIndex: 0) >= 7;
  }

  static bool HasUncompressedData(PlayerProfile profile) {
    return profile.m_worldData.Values.Any(value => value.m_mapData != null && !IsCompressedMapData(value.m_mapData));
  }

  static GameObject _profileCompressionRoot;
  static Button _compressMapDataButton;
  static TMP_Text _profileCompressionText;

  public static void CreateProfileCompressionUI(FejdStartup fejdStartup) {
    _profileCompressionRoot = CreateCompressionRoot(fejdStartup.m_selectCharacterPanel.transform);
    _compressMapDataButton = CreateCompressMapDataButton(fejdStartup, _profileCompressionRoot.transform);
    _profileCompressionText = CreateProfileCompressionText(fejdStartup, _profileCompressionRoot.transform);
  }

  static GameObject CreateCompressionRoot(Transform parent) {
    GameObject compressionRoot = new("CompressionRoot", typeof(RectTransform), typeof(VerticalLayoutGroup));
    compressionRoot.transform.SetParent(parent);

    RectTransform transform = compressionRoot.GetComponent<RectTransform>();
    transform.anchorMin = new(0.5f, 0f);
    transform.anchorMax = new(0.5f, 0f);
    transform.pivot = new(0.5f, 0f);
    transform.anchoredPosition = new(410f, 74f);

    VerticalLayoutGroup layoutGroup = compressionRoot.GetComponent<VerticalLayoutGroup>();
    layoutGroup.childControlHeight = false;
    layoutGroup.childControlWidth = false;
    layoutGroup.childForceExpandHeight = false;
    layoutGroup.childForceExpandWidth = false;
    layoutGroup.childAlignment = TextAnchor.UpperCenter;

    compressionRoot.SetActive(true);

    return compressionRoot;
  }

  static Button CreateCompressMapDataButton(FejdStartup fejdStartup, Transform parent) {
    Button compressMapDataButton = UnityEngine.Object.Instantiate(fejdStartup.m_csNewButton, parent);
    compressMapDataButton.onClick.RemoveAllListeners();
    compressMapDataButton.onClick = new Button.ButtonClickedEvent();

    RectTransform transform = compressMapDataButton.GetComponent<RectTransform>();
    transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200f);

    TMP_Text text = compressMapDataButton.GetComponentInChildren<TMP_Text>();
    text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200f);
    text.text = "Compression";

    compressMapDataButton.gameObject.name = "CompressMapData.Button";
    compressMapDataButton.gameObject.SetActive(false);

    return compressMapDataButton;
  }

  static TMP_Text CreateProfileCompressionText(FejdStartup fejdStartup, Transform parent) {
    TMP_Text profileCompressionText = UnityEngine.Object.Instantiate(fejdStartup.m_csName, parent);
    profileCompressionText.fontSize = 20;
    profileCompressionText.gameObject.name = "ProfileCompression.Text";
    profileCompressionText.text = "Profile Compression Text";

    RectTransform rectTransform = profileCompressionText.GetComponent<RectTransform>();
    rectTransform.anchorMin = Vector2.zero;
    rectTransform.anchorMax = Vector2.zero;
    rectTransform.pivot = Vector2.zero;
    rectTransform.anchoredPosition = new(-146f, -89f);
    rectTransform.ForceUpdateRectTransforms();

    return profileCompressionText;
  }

  public static void UpdateProfileCompressionUI(FejdStartup fejdStartup) {
    int profileIndex = fejdStartup.m_profileIndex;

    if (_profileCompressionRoot && profileIndex >= 0 && profileIndex < fejdStartup.m_profiles.Count) {
      PlayerProfile profile = fejdStartup.m_profiles[profileIndex];

      UpdateCompressMapDataButton(fejdStartup, profile);
      UpdateProfileCompressionText(profile);
    }
  }

  static void UpdateCompressMapDataButton(FejdStartup fejdStartup, PlayerProfile profile) {
    bool hasUncompressedData = HasUncompressedData(profile);

    _compressMapDataButton.GetComponentInChildren<TMP_Text>().text =
        hasUncompressedData ? "Compress MapData" : "Compressed!";

    _compressMapDataButton.onClick.RemoveAllListeners();
    _compressMapDataButton.onClick.AddListener(
        () => fejdStartup.StartCoroutine(CompressProfileMapDataCoroutine(fejdStartup, profile)));

    _compressMapDataButton.interactable = hasUncompressedData;
    _compressMapDataButton.gameObject.SetActive(true);
  }

  static void UpdateProfileCompressionText(PlayerProfile profile) {
    float mapDataBytes = profile.m_worldData.Values.Select(value => value.m_mapData?.Length ?? 0).Sum();

    int compressedCount =
        profile.m_worldData.Values
            .Select(value => (value.m_mapData == null || IsCompressedMapData(value.m_mapData)) ? 1 : 0)
            .Sum();

    _profileCompressionText.text =
        string.Format(
            "Worlds: <color={0}>{1}</color>/<color={0}>{2}</color> compressed   MapData: <color={0}>{3}</color> KB",
            "orange",
            profile.m_worldData.Count,
            compressedCount,
            (mapDataBytes / 1024).ToString("N0"));
  }

  static IEnumerator CompressProfileMapDataCoroutine(
    FejdStartup fejdStartup, PlayerProfile profile) {
    Selectable[] selectables = UnityEngine.Object.FindObjectsOfType<Selectable>();

    foreach (Selectable selectable in selectables) {
      selectable.interactable = false;
    }

    long count = 0;
    TMP_Text buttonText = _compressMapDataButton.GetComponentInChildren<TMP_Text>();

    foreach (PlayerProfile.WorldPlayerData worldPlayerData in profile.m_worldData.Values) {
      buttonText.text = $"Compressing... {++count}/{profile.m_worldData.Count}";
      worldPlayerData.m_mapData = CompressMapData(ref worldPlayerData.m_mapData);

      yield return null;
    }

    foreach (Selectable selectable in selectables) {
      selectable.interactable = true;
    }

    profile.SavePlayerToDisk();
    fejdStartup.UpdateCharacterList();

    _mapPackage.Clear();
  }
}
