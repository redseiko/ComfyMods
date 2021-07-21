using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using UnityEngine.UI;

namespace SeikoColorWheel
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class NewMod : BaseUnityPlugin
    {
        private const string ModName = "RedSeikoColorWheel";
        private const string ModVersion = "1.0";
        private const string ModGUID = "com.redseiko.comfy";
        private static readonly ConfigSync configSync = new(ModGUID) {DisplayName = ModName};
        private bool clorwards;
        private bool colorboats;
        private bool colorlights;
        private bool colorpieces;
        private bool colorportals;

        private ConfigEntry<Color> fireplacecolor;

        private ConfigEntry<KeyCode> overlayKey;
        private ConfigEntry<Color> piececolor;
        private ConfigEntry<Color> portalcolor;
        private ConfigEntry<Color> wardcolor;
        private ConfigEntry<Color> yachtcolor;
        private static GameObject ColorWheelMenu { get; set; }

        public KeyValuePair<string, PluginInfo> ColorfulLights { get; private set; }

        private static GameObject menu { get; set; }
        public KeyValuePair<string, PluginInfo> ColorfulWards { get; private set; }
        public KeyValuePair<string, PluginInfo> ColorfulPortals { get; private set; }
        public KeyValuePair<string, PluginInfo> ColorfulPieces { get; private set; }
        public KeyValuePair<string, PluginInfo> YachtClub { get; private set; }

        public void Awake()
        {
            Stream scriptstream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SeikoColorWheel.Libs.SeikoColorWheel.dll");
            byte[] buffer = new byte[scriptstream.Length];
            scriptstream.Read(buffer, 0, buffer.Length);
            Assembly scriptassembly = Assembly.Load(buffer);

            var assetstream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("SeikoColorWheel.Assets.colorwheel");
            var bundle = AssetBundle.LoadFromStream(assetstream);
            ColorWheelMenu = bundle.LoadAsset<GameObject>("SeikoColorWheel");

            overlayKey =
                Config.Bind(
                    "1 - General",
                    "Key to open overlay",
                    KeyCode.O,
                    new ConfigDescription("Key that opens the overlay"));

            Assembly assembly = Assembly.GetExecutingAssembly();
            Harmony harmony = new(ModGUID);
            harmony.PatchAll(assembly);
        }

        private void Start()
        {
            try
            {
                ColorfulLights = Chainloader.PluginInfos.First(p => p.Key == "redseiko.valheim.colorfullights");
                if (ColorfulLights.Value.Instance != null)
                {
                    Debug.Log($"loading{ColorfulLights}");
                    ColorfulLights.Value.Instance.Config.TryGetEntry("Color", "targetFireplaceColor",
                        out fireplacecolor);
                    colorlights = true;
                }
            }
            catch
            {
                Debug.Log("Colorful Lights not found ");
                colorlights = false;
            }

            try
            {
                ColorfulWards = Chainloader.PluginInfos.First(p => p.Key == "redseiko.valheim.colorfulwards");
                if (ColorfulWards.Value.Instance != null)
                {
                    Debug.Log($"Loading{ColorfulWards}");
                    ColorfulWards.Value.Instance.Config.TryGetEntry("Color", "targetWardColor", out wardcolor);
                    clorwards = true;
                }
            }
            catch
            {
                Debug.Log("Coloful Wards not found");
                clorwards = false;
            }

            try
            {
                ColorfulPortals = Chainloader.PluginInfos.First(p => p.Key == "redseiko.valheim.colorfulportals");
                if (ColorfulPortals.Value.Instance != null)
                {
                    Debug.Log($"Loading{ColorfulPortals}");
                    ColorfulPortals.Value.Instance.Config.TryGetEntry("Color", "targetPortalColor", out portalcolor);
                    colorportals = true;
                }
            }
            catch
            {
                Debug.Log("Colorful Portals not found");
                colorportals = false;
            }

            try
            {
                ColorfulPieces = Chainloader.PluginInfos.First(p => p.Key == "redseiko.valheim.colorfulpieces");
                if (ColorfulPieces.Value.Instance != null)
                {
                    Debug.Log($"Loading{ColorfulPieces}");
                    ColorfulPieces.Value.Instance.Config.TryGetEntry("Color", "targetPieceColor", out piececolor);
                    colorpieces = true;
                }
            }
            catch
            {
                Debug.Log("ColorfulfulPieces not found");
                colorpieces = false;
            }

            try
            {
                YachtClub = Chainloader.PluginInfos.First(p => p.Key == "redseiko.valheim.yachtclub");
                if (YachtClub.Value.Instance != null)
                {
                    Debug.Log($"Loading{YachtClub}");
                    YachtClub.Value.Instance.Config.TryGetEntry("Color", "targetFireplaceColor", out yachtcolor);
                    colorboats = true;
                }
            }
            catch
            {
                Debug.Log("Colorful yachts not found");
                colorboats = false;
            }
        }

        private void Update()
        {
            if (Player.m_localPlayer == null)
                return;
            PatchPlayerInputActive.skipOverlayActiveCheck = true;
            try
            {
                if (ButtonPressed(overlayKey.Value) && (!Chat.instance || !Chat.instance.HasFocus()) &&
                    !Console.IsVisible() && !TextInput.IsVisible() && !StoreGui.IsVisible() &&
                    !InventoryGui.IsVisible() && !Menu.IsVisible() &&
                    (!TextViewer.instance || !TextViewer.instance.IsVisible()) && !Minimap.IsOpen() &&
                    !GameCamera.InFreeFly())
                {
                    if (menu is null)
                    {
                        menu = Instantiate(ColorWheelMenu);
                        menu.name = "ColorMenu";
                        menu.transform.SetSiblingIndex(menu.transform.GetSiblingIndex() - 4);

                        //add if else statements determining the 
                        if (colorlights)
                        {
                            var torchbutton = menu.transform.Find("ColorfulTorches").GetComponent<Button>();
                            torchbutton.onClick.AddListener(() =>
                            {
                                ColorPicker.Create(fireplacecolor.Value,
                                    $"Choose the {ColorfulLights.Value.Instance.name}'s color!", SetColorFireplace,
                                    ColorFinished, true);
                            });
                        }
                        else if (colorlights == false)
                        {
                            var torchbutton = menu.transform.Find("ColorfulTorches").GetComponent<Button>();
                            torchbutton.gameObject.SetActive(false);
                        }


                        if (clorwards)
                        {
                            var wardbutton = menu.transform.Find("ColorfulWards").GetComponent<Button>();
                            wardbutton.onClick.AddListener(() =>
                            {
                                ColorPicker.Create(wardcolor.Value,
                                    $"Choose the {ColorfulWards.Value.Instance.name}'s color!", SetColorWard,
                                    ColorFinished, true);
                            });
                        }
                        else if (clorwards == false)
                        {
                            var wardbutton = menu.transform.Find("ColorfulWards").GetComponent<Button>();
                            wardbutton.gameObject.SetActive(false);
                        }


                        if (colorportals)
                        {
                            var portalbutton = menu.transform.Find("ColorfulPortals").GetComponent<Button>();
                            portalbutton.onClick.AddListener(() =>
                            {
                                ColorPicker.Create(portalcolor.Value,
                                    $"Choose the {ColorfulPortals.Value.Instance.name}'s color!", SetColorPortal,
                                    ColorFinished, true);
                            });
                        }
                        else if (colorportals == false)
                        {
                            var portalbutton = menu.transform.Find("ColorfulPortals").GetComponent<Button>();
                            portalbutton.gameObject.SetActive(false);
                        }

                        if (colorpieces)
                        {
                            var piecebutton = menu.transform.Find("ColorfulPieces").GetComponent<Button>();
                            piecebutton.onClick.AddListener(() =>
                            {
                                ColorPicker.Create(piececolor.Value,
                                    $"Choose the {ColorfulPieces.Value.Instance.name}'s color!", SetColorPiece,
                                    ColorFinished, true);
                            });
                        }
                        else if (colorpieces == false)
                        {
                            var piecebutton = menu.transform.Find("ColorfulPieces").GetComponent<Button>();
                            piecebutton.gameObject.SetActive(false);
                        }


                        if (colorboats)
                        {
                            var yachtbutton = menu.transform.Find("YachtClub").GetComponent<Button>();
                            yachtbutton.onClick.AddListener(() =>
                            {
                                ColorPicker.Create(yachtcolor.Value,
                                    $"Choose the {YachtClub.Value.Instance.name}'s color!", SetColorYacht,
                                    ColorFinished, true);
                            });
                        }
                        else if (colorboats == false)
                        {
                            var yachtbutton = menu.transform.Find("YachtClub").GetComponent<Button>();
                            yachtbutton.gameObject.SetActive(false);
                        }
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

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
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

        public static void ColorFinished(Color finishedColor)
        {
            Debug.Log("You chose the color " + ColorUtility.ToHtmlStringRGBA(finishedColor));
        }

        public void SetColorFireplace(Color currentColor)
        {
            fireplacecolor.Value = currentColor;
        }

        public void SetColorWard(Color currentColor)
        {
            wardcolor.Value = currentColor;
        }

        public void SetColorYacht(Color currentColor)
        {
            yachtcolor.Value = currentColor;
        }

        public void SetColorPiece(Color currentColor)
        {
            piececolor.Value = currentColor;
        }

        public void SetColorPortal(Color currentColor)
        {
            portalcolor.Value = currentColor;
        }

        [HarmonyPatch(typeof(Menu), "IsVisible")]
        private class PatchPlayerInputActive
        {
            public static bool skipOverlayActiveCheck;

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
    }
}