using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using Jotunn.Managers;
using Jotunn.Configs;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using UnityEngine;

using static PotteryBarn.PluginConfig;

namespace PotteryBarn {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class PotteryBarn : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.potterybarn";
    public const string PluginName = "PotteryBarn";
    public const string PluginVersion = "1.3.0";

    static ManualLogSource _logger;
    Harmony _harmony;

    static Piece.PieceCategory _prefabPieceCategory;
    static Piece.PieceCategory _cultivatorPrefabPieceCategory;
    static Sprite _standardPrefabIconSprite;
    static Quaternion _prefabIconRenderRotation;

    public static bool isDropTableDisabled = false;

    public void Awake() {
      _logger = Logger;

      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);

      _prefabPieceCategory = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", "CreatorShop");
      _cultivatorPrefabPieceCategory = PieceManager.Instance.AddPieceCategory("_CultivatorPieceTable", "CreatorShop");
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static IEnumerator AddPieces() {
      yield return AddHammerPieces(GetPieceTable("_HammerPieceTable"));
    }

    static IEnumerator AddHammerPieces(PieceTable pieceTable) {
      yield return null;

      if (!pieceTable) {
        _logger.LogError($"Could not find HammerPieceTable!");
        yield break;
      }

      _standardPrefabIconSprite = _standardPrefabIconSprite ??= CreateColorSprite(new Color32(34, 132, 73, 64));
      _prefabIconRenderRotation = Quaternion.Euler(0f, -45f, 0f);

      pieceTable.AddPiece(GetExistingPiece("turf_roof").SetName("turf_roof"));
      pieceTable.AddPiece(GetExistingPiece("turf_roof_top").SetName("turf_roof_top"));
      pieceTable.AddPiece(GetExistingPiece("turf_roof_wall").SetName("turf_roof_wall"));
      pieceTable.AddPiece(GetExistingPiece("ArmorStand_Female").SetName("ArmorStand_Female"));
      pieceTable.AddPiece(GetExistingPiece("ArmorStand_Male").SetName("ArmorStand_Male"));

      pieceTable.AddPiece(
          GetExistingPiece("stone_floor")
              .SetResource("Stone", r => r.SetAmount(12).SetRecover(true)));

      pieceTable.AddPiece(
          GetExistingPiece("wood_ledge")
              .SetResource("Wood", r => r.SetAmount(1).SetRecover(true)));

      foreach (KeyValuePair<string, Dictionary<string, int>> entry in Requirements.hammerCreatorShopItems.OrderBy(o => o.Key).ToList()) {
        GetOrAddPieceComponent(entry.Key, pieceTable)
          .SetResources(CreateRequirements(entry.Value))
          .SetCategory(_prefabPieceCategory)
          .SetCraftingStation(GetCraftingStation(entry.Key))
          .SetCanBeRemoved(true)
          .SetTargetNonPlayerBuilt(false);
      }

      foreach (KeyValuePair<string, Dictionary<string, int>> entry in Requirements.cultivatorCreatorShopItems.OrderBy(o => o.Key).ToList()) {
        GetOrAddPieceComponent(entry.Key, GetPieceTable("_CultivatorPieceTable"))
          .SetResources(CreateRequirements(entry.Value))
          .SetCanBeRemoved(false)
          .SetTargetNonPlayerBuilt(false)
          .SetGroundOnly(true);
      }
    }

    static Piece.Requirement[] CreateRequirements(Dictionary<string, int> data) {
      Piece.Requirement[] requirements = new Piece.Requirement[data.Count];
      for (int index = 0; index < data.Count; index++) {
        KeyValuePair<string, int> item = data.ElementAt(index);
        Piece.Requirement req = new Piece.Requirement();
        req.m_resItem = PrefabManager.Cache.GetPrefab<GameObject>(item.Key).GetComponent<ItemDrop>();
        req.m_amount = item.Value;
        requirements[index] = req;
      }
      return requirements;
    }

    static PieceTable GetPieceTable(string pieceTableName) {
      return ObjectDB.m_instance.Ref()?.m_items
          .Select(item => item.GetComponent<ItemDrop>().Ref()?.m_itemData?.m_shared?.m_buildPieces)
          .Where(table => table.Ref()?.name == pieceTableName)
          .FirstOrDefault();
    }

    static Piece GetExistingPiece(string prefabName) {
      return ZNetScene.m_instance.Ref()?.GetPrefab(prefabName).Ref()?.GetComponent<Piece>();
    }

    static Piece GetOrAddPieceComponent(string prefabName, PieceTable pieceTable) {
      GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);

      if (!prefab.TryGetComponent(out Piece piece)) {
        piece = prefab.AddComponent<Piece>();
        piece.m_name = FormatPrefabName(prefab.name);
        SetPlacementRestrictions(piece);
      }

      piece.m_icon ??= LoadOrRenderIcon(prefab, _prefabIconRenderRotation, _standardPrefabIconSprite);

      if (!pieceTable.m_pieces.Contains(prefab)) {
        pieceTable.m_pieces.Add(prefab);
        ZLog.Log($"Added Piece {piece.m_name} to PieceTable {pieceTable.name}");
      }

      piece.m_description = prefab.name;

      return piece;
    }

    static readonly Regex PrefabNameRegex = new(@"([a-z])([A-Z])");

    static string FormatPrefabName(string prefabName) {
      return PrefabNameRegex
          .Replace(prefabName, "$1 $2")
          .TrimStart(' ')
          .Replace('_', ' ')
          .Replace("  ", " ");
    }

    static Sprite LoadOrRenderIcon(GameObject prefab, Quaternion renderRotation, Sprite defaultSprite) {
      RenderManager.RenderRequest request = new(prefab) {
        Rotation = renderRotation,
      };

      return RenderManager.Instance.Render(request).Ref() ?? defaultSprite;
    }

    static Piece SetPlacementRestrictions(Piece piece) {
      piece.m_repairPiece = false;
      piece.m_groundOnly = false;
      piece.m_groundPiece = false;
      piece.m_cultivatedGroundOnly = false;
      piece.m_waterPiece = false;
      piece.m_noInWater = false;
      piece.m_notOnWood = false;
      piece.m_notOnTiltingSurface = false;
      piece.m_inCeilingOnly = false;
      piece.m_notOnFloor = false;
      piece.m_onlyInTeleportArea = false;
      piece.m_allowedInDungeons = false;
      piece.m_clipEverything = true;

      return piece;
    }

    static Sprite CreateColorSprite(Color color) {
      Texture2D texture = new(1, 1);
      texture.SetPixel(0, 0, color);
      texture.Apply();

      return Sprite.Create(texture, new(0, 0, 1, 1), Vector2.zero);
    }

    public static void log(string message) {
      _logger.LogMessage(message);
    }

    public static bool isCreatorShopPiece(Piece piece) {
      if (Requirements.hammerCreatorShopItems.Keys.Contains(piece.m_description) || Requirements.cultivatorCreatorShopItems.Keys.Contains(piece.m_description)) {
        return true;
      }
      return false;
    }

    public static bool isDestructibleCreatorShopPiece(string prefabName) {
      if (Requirements.hammerCreatorShopItems.Keys.Contains(prefabName)) {
        return true;
      }
      return false;
    }

    public static bool isDestructibleCreatorShopPiece(Piece piece) {
      if (Requirements.hammerCreatorShopItems.Keys.Contains(piece.m_description)) {
        return true;
      }
      return false;
    }

    public static bool isBuildHammerItem(string prefabName) {
      if (Requirements.hammerCreatorShopItems.Keys.Contains(prefabName)) {
        return true;
      }
      return false;
    }

    public static bool HasCraftingStationRequirement(string prefabName) {
      if(Requirements.craftingStationRequirements.Keys.Contains(prefabName)) {
        return true;
      }
      return false;
    }
    public static CraftingStation GetCraftingStation(string prefabName) {
      string stationName = Requirements.craftingStationRequirements[prefabName];
      if(!stationName.IsNullOrWhiteSpace()) {
        return PrefabManager.Instance.GetPrefab(stationName).GetComponent<CraftingStation>();
      }
      return null;
    }
  }
}