using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace SkyTree {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class SkyTree : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.skytree";
    public const string PluginName = "SkyTree";
    public const string PluginVersion = "1.1.0";

    private static ConfigEntry<bool> _isModEnabled;

    private static ManualLogSource _logger;
    private Harmony _harmony;

    private void Awake() {
      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _logger = Logger;
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    [HarmonyPatch(typeof(ZNetScene))]
    class ZNetScenePatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(ZNetScene.Awake))]
      static void AwakePostfix(ZNetScene __instance) {
        if (_isModEnabled.Value) {
          __instance.StartCoroutine(FixYggdrasilBranchCoroutine());
        }
      }
    }

    private static IEnumerator FixYggdrasilBranchCoroutine() {
      _logger.LogInfo("Starting FixYggdrasilBranch coroutine.");
      WaitForSeconds waitInterval = new(seconds: 3f);

      while (true) {
        yield return waitInterval;

        if (!ZNetScene.instance) {
          continue;
        }

        GameObject[] yggdrasils = Array.FindAll(
            FindObjectsOfType<GameObject>(), obj => obj.name.StartsWith("YggdrasilBranch"));

        if (yggdrasils.Length < 1) {
          continue;
        }

        foreach (GameObject yggdrasil in yggdrasils) {
          _logger.LogInfo("Setting YggdrasilBranch layer to 15.");
          yggdrasil.layer = 15;

          Transform branch = yggdrasil.transform.Find("branch");

          if (!branch) {
            continue;
          }

          _logger.LogInfo("Found YggdrasilBranch/branch, setting layer to 15.");
          branch.gameObject.layer = 15;

          MeshFilter filter = branch.GetComponentInChildren<MeshFilter>();

          if (!filter || !branch.TryGetComponent(out MeshCollider collider)) {
            continue;
          }

          _logger.LogInfo("Adding collider to YggdrasilBranch/branch.");
          collider = branch.gameObject.AddComponent<MeshCollider>();
          collider.sharedMesh = filter.sharedMesh;
        }

        _logger.LogInfo("Finished FixYggdrasilBranch coroutine.");
        yield break;
      }
    }
  }
}
