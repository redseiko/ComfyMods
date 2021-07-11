﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace ColorfulLights
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ColorfulLights : BaseUnityPlugin
    {
        public const string PluginGUID = "redseiko.valheim.colorfullights";
        public const string PluginName = "ColorfulLights";
        public const string PluginVersion = "1.0.0";

        public class FireplaceData
        {
            public List<ParticleSystem> Systems { get; }
            public List<ParticleSystemRenderer> Renderers { get; }
            public Color TargetColor { get; set; }

            public FireplaceData()
            {
                Systems = new List<ParticleSystem>();
                Renderers = new List<ParticleSystemRenderer>();
                TargetColor = Color.clear;
            }
        }

        private static readonly ConditionalWeakTable<Fireplace, FireplaceData> _fireplaceData =
            new ConditionalWeakTable<Fireplace, FireplaceData>();

        private static ConfigEntry<bool> _isModEnabled;
        private static ConfigEntry<Color> _targetFireplaceColor;
        private static ConfigEntry<string> _targetFireplaceColorHex;
        public static ConfigEntry<KeyCode> overlayKey;

        private static ManualLogSource _logger;
        private Harmony _harmony;

        public AssetBundle uitest { get; private set; }
        public static GameObject menuColor { get; private set; }
        private static GameObject menu;


        private void Awake()
        {
            _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

            _targetFireplaceColor =
                Config.Bind("Color", "targetFireplaceColor", Color.cyan, "Target color to set any torch/fire to.");

            _targetFireplaceColorHex =
                Config.Bind(
                    "Color",
                    "targetFireplaceColorHex",
                    $"#{ColorUtility.ToHtmlStringRGB(Color.cyan)}",
                    "Target color to set torch/fire to, in HTML hex-form.");

            overlayKey = 
                Config.Bind(
                    "1 - General", 
                    "Key to open overlay", 
                    KeyCode.O, 
                    new ConfigDescription("Key that opens the overlay"));

            void UpdateColorHexValue() =>
                _targetFireplaceColorHex.Value =
                    string.Format(
                        "#{0}",
                        _targetFireplaceColor.Value.a == 1.0f
                            ? ColorUtility.ToHtmlStringRGB(_targetFireplaceColor.Value)
                            : ColorUtility.ToHtmlStringRGBA(_targetFireplaceColor.Value));

            _targetFireplaceColor.SettingChanged += (sender, eventArgs) => UpdateColorHexValue();
            UpdateColorHexValue();

            _targetFireplaceColorHex.SettingChanged += (sender, eventArgs) =>
            {
                if (ColorUtility.TryParseHtmlString(_targetFireplaceColorHex.Value, out Color color))
                {
                    _targetFireplaceColor.Value = color;
                }
            };
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ColorfulLights.EmbeddedAssets.ColorWheel.dll");
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            Assembly assembly = Assembly.Load(buffer);
            LoadAssets();
            _logger = Logger;
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

     

        private void Update()
        {
            if (Player.m_localPlayer == null)
                return;
            PatchPlayerInputActive.skipOverlayActiveCheck = true;
            try
            {
                if (ButtonPressed(overlayKey.Value) && (!Chat.instance || !Chat.instance.HasFocus()) && !Console.IsVisible() && !TextInput.IsVisible() && !StoreGui.IsVisible() && !InventoryGui.IsVisible() && !Menu.IsVisible() && (!TextViewer.instance || !TextViewer.instance.IsVisible()) && !Minimap.IsOpen() && !GameCamera.InFreeFly())
                {
                    if (menu is null)
                    {
                        menu = Instantiate(menuColor);
                        menu.name = "ColorMenu";
                        menu.transform.SetSiblingIndex(menu.transform.GetSiblingIndex() - 4);
                        var button = menu.transform.Find("ChooseColor").GetComponent<Button>();
                        button.onClick.AddListener(() =>
                        {
                            ColorPicker.Create(_targetFireplaceColor.Value, $"Choose the { _targetFireplaceColor.Value}'s color!", SetColor, ColorFinished, true);
                        });
                    }
                    else
                    {
                        menu.SetActive(!menu.activeSelf);
                    }
                }
            }
            finally
            {
                PatchPlayerInputActive.skipOverlayActiveCheck = false;
            }
        }
        private void OnDestroy()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchSelf();
            }
        }
        public static void ColorFinished(Color finishedColor)
        {
            Debug.Log("You chose the color " + ColorUtility.ToHtmlStringRGBA(finishedColor));
        }
        public void SetColor(Color currentColor)
        {
            _targetFireplaceColor.Value = currentColor;
        }
        private void LoadAssets()
        {
            uitest = GetAssetBundleFromResources("seikowheel");
            menuColor = uitest.LoadAsset<GameObject>("SeikoColorWheel");
            var button2 = menuColor.transform.Find("ChooseGradient").gameObject;
            button2.SetActive(false);

            uitest?.Unload(false);

        }

        private static AssetBundle GetAssetBundleFromResources(string filename)
        {
            var execAssembly = Assembly.GetExecutingAssembly();
            var resourceName = execAssembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(filename));

            using (var stream = execAssembly.GetManifestResourceStream(resourceName))
            {
                return AssetBundle.LoadFromStream(stream);
            }
        }

        public static void TryRegisterFabs(ZNetScene zNetScene)
        {
            if (zNetScene == null || zNetScene.m_prefabs == null || zNetScene.m_prefabs.Count <= 0)
            {
                return;
            }
            zNetScene.m_prefabs.Add(menuColor);
        }

        private static bool ButtonPressed(KeyCode button)
        {
            try
            {
                return Input.GetKeyDown(button);
            }
            catch
            {
                return false;
            }

        }

        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        public static class ZNetScene_Awake_Patch
        {
            public static bool Prefix(ZNetScene __instance)
            {

                TryRegisterFabs(__instance);
#if DEBUG
                Debug.Log("Loading the Menu thing");
#endif
                return true;
            }
        }

        [HarmonyPatch(typeof(Fireplace))]
        private class FireplacePatch
        {
            private static readonly string _hoverTextTemplate =
                "{0} ( $piece_fire_fuel {1}/{2} )\n"
                    + "[<color={3}>$KEY_Use</color>] $piece_use\n"
                    + "[<color={4}>{5}</color>] Change color to: <color={6}>{6}</color>";

            private static readonly int _fuelHashCode = "fuel".GetStableHashCode();
            private static readonly int _fireplaceColorHashCode = "FireplaceColor".GetStableHashCode();

            private static readonly KeyboardShortcut _changeColorActionShortcut =
                new KeyboardShortcut(KeyCode.E, KeyCode.LeftShift);

            [HarmonyPostfix]
            [HarmonyPatch(nameof(Fireplace.Awake))]
            private static void FireplaceAwakePostfix(ref Fireplace __instance)
            {
                if (!_isModEnabled.Value || !__instance)
                {
                    return;
                }

                _fireplaceData.Add(__instance, ExtractFireplaceData(__instance));
            }

            [HarmonyPrefix]
            [HarmonyPatch(nameof(Fireplace.GetHoverText))]
            private static bool FireplaceGetHoverTextPrefix(ref Fireplace __instance, ref string __result)
            {
                if (!_isModEnabled.Value || !__instance)
                {
                    return true;
                }

                __result = Localization.instance.Localize(
                    string.Format(
                        _hoverTextTemplate,
                        __instance.m_name,
                        __instance.m_nview.m_zdo.GetFloat(_fuelHashCode, 0f).ToString("N02"),
                        __instance.m_maxFuel,
                        "#ffee58",
                        "#ffa726",
                        _changeColorActionShortcut,
                       _targetFireplaceColorHex.Value));

                return false;
            }

            [HarmonyPrefix]
            [HarmonyPatch(nameof(Fireplace.Interact))]
            private static bool FireplaceInteractPrefix(ref Fireplace __instance, ref bool __result, bool hold)
            {
                if (!_isModEnabled.Value || hold || !_changeColorActionShortcut.IsDown())
                {
                    return true;
                }

                if (!__instance.m_nview && !__instance.m_nview.IsValid())
                {
                    _logger.LogWarning("Fireplace does not have a valid ZNetView.");

                    __result = false;
                    return false;
                }

                if (!PrivateArea.CheckAccess(__instance.transform.position, radius: 0f, flash: true, wardCheck: false))
                {
                    _logger.LogWarning("Fireplace is within a private area.");

                    __result = false;
                    return false;
                }

                if (!__instance.m_nview.IsOwner())
                {
                    __instance.m_nview.ClaimOwnership();
                }

                __instance.m_nview.GetZDO().Set(_fireplaceColorHashCode, Utils.ColorToVec3(_targetFireplaceColor.Value));
                __instance.m_fuelAddedEffects.Create(__instance.transform.position, __instance.transform.rotation);

                if (_fireplaceData.TryGetValue(__instance, out FireplaceData fireplaceData))
                {
                    SetParticleColors(fireplaceData.Systems, fireplaceData.Renderers, _targetFireplaceColor.Value);
                    fireplaceData.TargetColor = _targetFireplaceColor.Value;
                }

                __result = true;
                return false;
            }

            [HarmonyPrefix]
            [HarmonyPatch(nameof(Fireplace.UseItem))]
            private static bool FireplaceUseItemPrefix(
                ref Fireplace __instance, ref bool __result, Humanoid user, ItemDrop.ItemData item)
            {
                if (!_isModEnabled.Value
                    || !__instance.m_fireworks
                    || item.m_shared.m_name != __instance.m_fireworkItem.m_itemData.m_shared.m_name
                    || !_fireplaceData.TryGetValue(__instance, out FireplaceData fireplaceData))
                {
                    return true;
                }

                if (!__instance.IsBurning())
                {
                    user.Message(MessageHud.MessageType.Center, "$msg_firenotburning");

                    __result = true;
                    return false;
                }

                if (user.GetInventory().CountItems(item.m_shared.m_name) < __instance.m_fireworkItems)
                {
                    user.Message(MessageHud.MessageType.Center, $"$msg_toofew {item.m_shared.m_name}");

                    __result = true;
                    return false;
                }

                user.GetInventory().RemoveItem(item.m_shared.m_name, __instance.m_fireworkItems);
                user.Message(
                    MessageHud.MessageType.Center, Localization.instance.Localize("$msg_throwinfire", item.m_shared.m_name));

                SetParticleColors(fireplaceData.Systems, fireplaceData.Renderers, fireplaceData.TargetColor);

                Quaternion colorToRotation = __instance.transform.rotation;
                colorToRotation.x = fireplaceData.TargetColor.r;
                colorToRotation.y = fireplaceData.TargetColor.g;
                colorToRotation.z = fireplaceData.TargetColor.b;

                _logger.LogInfo($"Sending fireworks to spawn with color: {fireplaceData.TargetColor}");
                ZNetScene.instance.SpawnObject(__instance.transform.position, colorToRotation, __instance.m_fireworks);

                __instance.m_fuelAddedEffects.Create(__instance.transform.position, __instance.transform.rotation);

                __result = true;
                return false;
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(Fireplace.UpdateFireplace))]
            private static void FireplaceUpdateFireplacePostfix(ref Fireplace __instance)
            {
                if (!_isModEnabled.Value
                    || !__instance.m_nview
                    || __instance.m_nview.m_zdo == null
                    || __instance.m_nview.m_zdo.m_zdoMan == null
                    || __instance.m_nview.m_zdo.m_vec3 == null
                    || !__instance.m_nview.m_zdo.m_vec3.ContainsKey(_fireplaceColorHashCode)
                    || !_fireplaceData.TryGetValue(__instance, out FireplaceData fireplaceData))
                {
                    return;
                }

                Color fireplaceColor = Utils.Vec3ToColor(__instance.m_nview.m_zdo.m_vec3[_fireplaceColorHashCode]);

                SetParticleColors(fireplaceData.Systems, fireplaceData.Renderers, fireplaceColor);
                fireplaceData.TargetColor = fireplaceColor;
            }
        }

        [HarmonyPatch(typeof(ZNetScene))]
        private class ZNetScenePatch
        {
            private static readonly int _vfxFireWorkTestHashCode = "vfx_FireWorkTest".GetStableHashCode();

            [HarmonyPrefix]
            [HarmonyPatch(nameof(ZNetScene.RPC_SpawnObject))]
            private static bool ZNetSceneRPC_SpawnObjectPrefix(
                ref ZNetScene __instance, Vector3 pos, Quaternion rot, int prefabHash)
            {
                if (!_isModEnabled.Value || prefabHash != _vfxFireWorkTestHashCode || rot == Quaternion.identity)
                {
                    return true;
                }

                Color fireworksColor = Utils.Vec3ToColor(new Vector3(rot.x, rot.y, rot.z));

                _logger.LogInfo($"Spawning fireworks with color: {fireworksColor}");
                GameObject fireworksClone = Instantiate(__instance.GetPrefab(prefabHash), pos, rot);

                SetParticleColors(
                    fireworksClone.GetComponentsInChildren<ParticleSystem>(),
                    fireworksClone.GetComponentsInChildren<ParticleSystemRenderer>(),
                    fireworksColor);

                return false;
            }
        }

        private static FireplaceData ExtractFireplaceData(Fireplace fireplace)
        {
            FireplaceData data = new FireplaceData();

            ExtractFireplaceData(data, fireplace.m_enabledObject);
            ExtractFireplaceData(data, fireplace.m_enabledObjectHigh);
            ExtractFireplaceData(data, fireplace.m_enabledObjectLow);
            ExtractFireplaceData(data, fireplace.m_fireworks);

            return data;
        }

        private static void ExtractFireplaceData(FireplaceData data, GameObject targetObject)
        {
            if (!targetObject)
            {
                return;
            }

            data.Systems.AddRange(targetObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true));
            data.Renderers.AddRange(targetObject.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true));
        }

        private static void SetParticleColors(
            IEnumerable<ParticleSystem> systems, IEnumerable<ParticleSystemRenderer> renderers, Color targetColor)
        {
            var targetColorGradient = new ParticleSystem.MinMaxGradient(targetColor);

            foreach (ParticleSystem system in systems)
            {
                var colorOverLifetime = system.colorOverLifetime;

                if (colorOverLifetime.enabled)
                {
                    colorOverLifetime.color = targetColorGradient;
                }

                var sizeOverLifetime = system.sizeOverLifetime;

                if (sizeOverLifetime.enabled)
                {
                    var main = system.main;
                    main.startColor = targetColor;
                }
            }

            foreach (ParticleSystemRenderer renderer in renderers)
            {
                renderer.material.color = targetColor;
            }
        }

        [HarmonyPatch(typeof(Menu), "IsVisible")]
        class PatchPlayerInputActive
        {
            public static bool skipOverlayActiveCheck = false;

            private static bool Prefix(ref bool __result)
            {
                if (menu != null && menu.gameObject.activeSelf && !skipOverlayActiveCheck)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        public static class CustomMonoBehavioursNames
        {
            public static string ColorPickerExampleScript = nameof(ColorPickerExampleScript);
            public static string ColorPicker = nameof(ColorPicker);
            public static string GradientPickerExample = nameof(GradientPickerExample);
        }

    }
}