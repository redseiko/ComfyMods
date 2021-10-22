using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using HarmonyLib;

using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

namespace OdinSaves {
  [BepInPlugin(Package, ModName, Version)]
  public class OdinSaves : BaseUnityPlugin {
    public const string Package = "redseiko.valheim.odinsaves";
    public const string Version = "1.0.1";
    public const string ModName = "OdinSaves";

    private static ConfigEntry<bool> _isModEnabled;
    private static ConfigEntry<int> savePlayerProfileInterval;
    private static ConfigEntry<bool> setLogoutPointOnSave;
    private static ConfigEntry<bool> showMessageOnModSave;

    private static ManualLogSource _logger;
    private Harmony _harmony;

    private void Awake() {
      _isModEnabled = Config.Bind("Global", "isModEnabled", true, "Whether the mod should be enabled.");

      savePlayerProfileInterval = Config.Bind(
        "Global",
        "savePlayerProfileInterval",
        300,
        "Interval (in seconds) for how often to save the player profile. Game default (and maximum) is 1200s.");

      setLogoutPointOnSave = Config.Bind(
        "Global",
        "setLogoutPointOnSave",
        true,
        "Sets your logout point to your current position when the mod performs a save.");

      showMessageOnModSave = Config.Bind(
        "Global",
        "saveMessageOnModSave",
        true,
        "Show a message (in the middle of your screen) when the mod tries to save.");

      _logger = Logger;
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void OnDestroy() {
      if (_harmony != null) {
        _harmony.UnpatchAll(null);
      }
    }

    [HarmonyPatch(typeof(Game))]
    private class GamePatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Game.UpdateSaving))]
      private static void UpdateSavingPostfix(ref Game __instance) {
        if (!_isModEnabled.Value) {
          return;
        }

        if (__instance.m_saveTimer == 0f || __instance.m_saveTimer < savePlayerProfileInterval.Value) {
          return;
        }

        if (showMessageOnModSave.Value) {
          MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Saving player profile...");
        }

        __instance.m_saveTimer = 0f;

        __instance.StartCoroutine(GameSavePlayerProfileAsync(__instance, setLogoutPointOnSave.Value).AsIEnumerator());

        if (ZNet.instance) {
          ZNet.instance.Save(sync: false);
        }
      }
    }

    private static async Task GameSavePlayerProfileAsync(Game game, bool setLogoutPointOnSave) {
      _logger.LogInfo("Saving player profile asynchronously...");
      await Task.Run(() => game.SavePlayerProfile(setLogoutPointOnSave)).ConfigureAwait(false);
    }

    [HarmonyPatch(typeof(Player))]
    private class PlayerPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.OnDeath))]
      private static void PlayerOnDeathPostfix(ref Player __instance) {
        Game.instance.m_playerProfile.ClearLoguoutPoint();
      }
    }

    [HarmonyPatch(typeof(PlayerProfile))]
    private class PlayerProfilePatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(PlayerProfile.GetMapData))]
      private static void PlayerProfileGetMapDataPostfix(ref PlayerProfile __instance, ref byte[] __result) {
        __result = DecompressMapData(ref __result);
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(PlayerProfile.SetMapData))]
      private static void PlayerProfileSetMapDataPrefix(ref PlayerProfile __instance, ref byte[] data) {
        if (!_isModEnabled.Value || HasUncompressedData(__instance)) {
          return;
        }

        data = CompressMapData(ref data);
      }
    }

    private static byte[] CompressMapData(ref byte[] mapData) {
      if (mapData == null || IsGZipData(mapData)) {
        return mapData;
      }

      using MemoryStream outStream = new(capacity: mapData.Length);
      using (GZipStream deflateStream = new(outStream, CompressionMode.Compress)) {
        deflateStream.Write(mapData, 0, mapData.Length);
      }

      return outStream.ToArray();
    }

    private static byte[] DecompressMapData(ref byte[] mapData) {
      if (mapData == null || !IsGZipData(mapData)) {
        return mapData;
      }

      using var inStream = new MemoryStream(mapData);
      using var inflateStream = new GZipStream(inStream, CompressionMode.Decompress);
      using var outStream = new MemoryStream(capacity: 1024 * 1024 * 4);

      inflateStream.CopyTo(outStream);
      return outStream.ToArray();
    }

    private static bool IsGZipData(byte[] data) {
      return data != null && data.Length >= 3 && data[0] == 0x1f && data[1] == 0x8b && data[2] == 0x08;
    }

    private static bool HasUncompressedData(PlayerProfile profile) {
      return profile.m_worldData.Values.All(value => value.m_mapData == null || !IsGZipData(value.m_mapData));
    }

    private static GameObject _profileCompressionRoot;
    private static Button _compressDecompressButton;
    private static Text _profileCompressionText;

    [HarmonyPatch(typeof(FejdStartup))]
    private class FejdStartupPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(FejdStartup.Awake))]
      private static void FejdStartupAwakePostfix(ref FejdStartup __instance) {
        _profileCompressionRoot = CreateCompressionRoot(__instance.m_selectCharacterPanel.transform);

        _compressDecompressButton = CreateCompressDecompressButton(__instance, _profileCompressionRoot.transform);
        _profileCompressionText = CreateProfileCompressionText(__instance, _profileCompressionRoot.transform);
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(FejdStartup.UpdateCharacterList))]
      private static void FejdStartupUpdateCharacterListPostfix(ref FejdStartup __instance) {
        if (__instance.m_profileIndex >= 0 && __instance.m_profileIndex < __instance.m_profiles.Count) {
          PlayerProfile profile = __instance.m_profiles[__instance.m_profileIndex];

          UpdateCompressDecompressButton(__instance, profile);
          UpdateProfileCompressionText(profile);
        }
      }

      private static GameObject CreateCompressionRoot(Transform parent) {
        GameObject compressionRoot = new("CompressionRoot", typeof(RectTransform), typeof(VerticalLayoutGroup));
        compressionRoot.transform.SetParent(parent);

        RectTransform transform = compressionRoot.GetComponent<RectTransform>();
        transform.anchorMin = new Vector2(0.5f, 0f);
        transform.anchorMax = new Vector2(0.5f, 0f);
        transform.pivot = new Vector2(0.5f, 0.5f);
        transform.anchoredPosition = new Vector2(410f, 141f);

        VerticalLayoutGroup layoutGroup = compressionRoot.GetComponent<VerticalLayoutGroup>();
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = false;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;

        compressionRoot.SetActive(true);

        return compressionRoot;
      }

      private static Button CreateCompressDecompressButton(FejdStartup fejdStartup, Transform parent) {
        Button compressDecompressButton = Instantiate(fejdStartup.m_csNewButton, parent);
        compressDecompressButton.onClick.RemoveAllListeners();
        compressDecompressButton.onClick = new Button.ButtonClickedEvent();

        RectTransform transform = compressDecompressButton.GetComponent<RectTransform>();
        transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200f);

        Text text = compressDecompressButton.GetComponentInChildren<Text>();
        text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200f);
        text.text = "Compression";

        compressDecompressButton.gameObject.name = "CompressDecompressButton";
        compressDecompressButton.gameObject.SetActive(false);

        return compressDecompressButton;
      }

      private static Text CreateProfileCompressionText(FejdStartup fejdStartup, Transform parent) {
        Text profileCompressionText = Instantiate(fejdStartup.m_csName, parent);
        profileCompressionText.gameObject.name = "ProfileCompressiontext";
        profileCompressionText.text = "<size=20>Profile Compression Text</size>";

        return profileCompressionText;
      }

      private static void UpdateCompressDecompressButton(FejdStartup fejdStartup, PlayerProfile profile) {
        bool hasUncompressedData = HasUncompressedData(profile);

        _compressDecompressButton.GetComponentInChildren<Text>().text =
            hasUncompressedData ? "Compress Profile" : "Decompress Profile";

        _compressDecompressButton.onClick.RemoveAllListeners();
        _compressDecompressButton.onClick.AddListener(
            () =>
              fejdStartup.StartCoroutine(
                  CompressDecompressProfileCoroutine(fejdStartup, profile, hasUncompressedData)));

        _compressDecompressButton.gameObject.SetActive(true);
      }

      private static void UpdateProfileCompressionText(PlayerProfile profile) {
        float mapDataBytes =
            profile.m_worldData.Values.Select(value => value.m_mapData == null ? 0 : value.m_mapData.Length).Sum();

        _profileCompressionText.text =
            string.Format(
                "<size=20>Worlds: <color={0}>{1}</color>   MapData: <color={0}>{2}</color> KB</size>",
                "orange",
                profile.m_worldData.Count,
                (mapDataBytes / 1024).ToString("N0"));
      }

      private static IEnumerator CompressDecompressProfileCoroutine(
          FejdStartup fejdStartup, PlayerProfile profile, bool hasUncompressedData) {
        Selectable[] selectables = FindObjectsOfType<Selectable>();

        foreach (Selectable selectable in selectables) {
          selectable.interactable = false;
        }

        _compressDecompressButton.GetComponentInChildren<Text>().text =
            hasUncompressedData ? "Compressing..." : "Decompressing...";

        yield return CompressDecompressProfileAsync(profile, hasUncompressedData).AsIEnumerator();

        foreach (Selectable selectable in selectables) {
          selectable.interactable = true;
        }

        fejdStartup.UpdateCharacterList();
      }

      private static async Task CompressDecompressProfileAsync(PlayerProfile profile, bool hasUncompressedData) {
        await Task.Run(() => {
          foreach (PlayerProfile.WorldPlayerData worldPlayerData in profile.m_worldData.Values) {
            worldPlayerData.m_mapData =
                hasUncompressedData
                    ? CompressMapData(ref worldPlayerData.m_mapData)
                    : DecompressMapData(ref worldPlayerData.m_mapData);
          }

          profile.Save();
        }).ConfigureAwait(false);
      }
    }
  }
}
