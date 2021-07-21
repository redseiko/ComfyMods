using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using HarmonyLib;

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace OdinSaves {
  [BepInPlugin(OdinSaves.Package, OdinSaves.ModName, OdinSaves.Version)]
  public class OdinSaves : BaseUnityPlugin {
    public const string Package = "redseiko.valheim.odinsaves";
    public const string Version = "1.0.0";
    public const string ModName = "Odin Saves";

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
        __instance.SavePlayerProfile(setLogoutPoint: setLogoutPointOnSave.Value);

        if (ZNet.instance) {
          ZNet.instance.Save(sync: false);
        }
      }
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
      [HarmonyPatch(nameof(PlayerProfile.Load))]
      private static void PlayerProfileLoadPostfix(ref PlayerProfile __instance, ref bool __result) {
        if (!_isModEnabled.Value || !__result) {
          return;
        }

        foreach (PlayerProfile.WorldPlayerData worldPlayerData in __instance.m_worldData.Values) {
          CompressMapData(worldPlayerData);
        }
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(PlayerProfile.GetMapData))]
      private static void PlayerProfileGetMapDataPostfix(ref PlayerProfile __instance, ref byte[] __result) {
        if (__result == null || !IsGZipData(__result)) {
          return;
        }

        using (var inStream = new MemoryStream(__result))
        using (var inflateStream = new GZipStream(inStream, CompressionMode.Decompress))
        using (var outStream = new MemoryStream()) {
          inflateStream.CopyTo(outStream);
          __result = outStream.ToArray();
        }
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(PlayerProfile.SetMapData))]
      private static void PlayerProfileSetMapDataPrefix(ref PlayerProfile __instance, ref byte[] data) {
        if (!_isModEnabled.Value) {
          return;
        }

        using (MemoryStream memoryStream = new MemoryStream(capacity: data.Length)) {
          using (GZipStream deflateStream = new GZipStream(memoryStream, CompressionMode.Compress)) {
            deflateStream.Write(data, 0, data.Length);
            deflateStream.Close();
          }

          data = memoryStream.ToArray();
        }
      }
    }

    private static void CompressMapData(PlayerProfile.WorldPlayerData worldPlayerData) {
      if (worldPlayerData.m_mapData == null
          || worldPlayerData.m_mapData.Length < 3
          || IsGZipData(worldPlayerData.m_mapData)) {
        return;
      }

      using (MemoryStream memoryStream = new MemoryStream(capacity: worldPlayerData.m_mapData.Length)) {
        using (GZipStream deflateStream = new GZipStream(memoryStream, CompressionMode.Compress)) {
          deflateStream.Write(worldPlayerData.m_mapData, 0, worldPlayerData.m_mapData.Length);
        }

        worldPlayerData.m_mapData = memoryStream.ToArray();
      }
    }

    private static bool IsGZipData(byte[] data) {
      return data.Length >= 3 && data[0] == 0x1f && data[1] == 0x8b && data[2] == 0x08;
    }

    [HarmonyPatch(typeof(FejdStartup))]
    private class FejdStartupPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(FejdStartup.UpdateCharacterList))]
      [HarmonyAfter(new string[] { "MK_BetterUI" })]
      private static void FejdStartupUpdateCharacterListPostfix(ref FejdStartup __instance) {
        if (__instance.m_profileIndex >= 0 && __instance.m_profileIndex < __instance.m_profiles.Count) {
          PlayerProfile profile = __instance.m_profiles[__instance.m_profileIndex];
          float mapDataBytes = profile.m_worldData.Values.Select(value => value.m_mapData.Length).Sum();

          __instance.m_csName.text =
              string.Format(
                  "{0}<size=20>{1}Worlds: <color={2}>{3}</color>   MapData: <color={2}>{4}</color> KB</size>",
                  __instance.m_csName.text,
                  __instance.m_csName.text == profile.GetName() ? "\n" : "   ",
                  "orange",
                  profile.m_worldData.Count,
                  (mapDataBytes / 1024).ToString("N0"));
        }
      }
    }
  }
}
