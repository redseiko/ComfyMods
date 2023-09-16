using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using BepInEx;

using HarmonyLib;

using Jotunn.Managers;

using UnityEngine;

using static PotteryBarn.PluginConfig;

namespace PotteryBarn {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  [BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
  public class PotteryBarn : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.potterybarn";
    public const string PluginName = "PotteryBarn";
    public const string PluginVersion = "1.9.0";

    Harmony _harmony;

    static Piece.PieceCategory _hammerCreatorShopCategory;
    static Piece.PieceCategory _hammerBuildingCategory;
    static Piece.PieceCategory _cultivatorCreatorShopCategory;
    static Sprite _standardPrefabIconSprite;
    static Quaternion _prefabIconRenderRotation;

    public static bool _debug = false;
    public static bool IsDropTableDisabled { get; set; } = false;

    public void Awake() {
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static void AddPieces() {
      AddHammerPieces(PieceManager.Instance.GetPieceTable("_HammerPieceTable"));
      AddCultivatorPieces(PieceManager.Instance.GetPieceTable("_CultivatorPieceTable"));
    }

    static void AddHammerPieces(PieceTable pieceTable) {
      _hammerCreatorShopCategory = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", "CreatorShop");
      _hammerBuildingCategory = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", "Building");

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

      foreach (
          KeyValuePair<string, Dictionary<string, int>> entry in
              Requirements.HammerCreatorShopItems.OrderBy(o => o.Key).ToList()) {

        GetOrAddPieceComponent(entry.Key, pieceTable)
            .SetResources(CreateRequirements(entry.Value))
            .SetCategory(_hammerCreatorShopCategory)
            .SetCraftingStation(GetCraftingStation(Requirements.craftingStationRequirements, entry.Key))
            .SetCanBeRemoved(true)
            .SetTargetNonPlayerBuilt(false);
      }

      foreach (
          KeyValuePair<string, Dictionary<string, int>> entry in
              DvergrPieces.DvergrPrefabs.OrderBy(o => o.Key).ToList()) {
        GetOrAddPieceComponent(entry.Key, pieceTable)
            .SetResources(CreateRequirements(entry.Value))
            .SetCategory(_hammerBuildingCategory)
            .SetCraftingStation(GetCraftingStation(DvergrPieces.DvergrPrefabCraftingStationRequirements, entry.Key))
            .SetCanBeRemoved(true)
            .SetTargetNonPlayerBuilt(false);
      }
    }

    static void AddCultivatorPieces(PieceTable pieceTable) {
      pieceTable.m_useCategories = true;

      PieceManager.Instance.AddPieceCategory("_CultivatorPieceTable", "Misc");

      _cultivatorCreatorShopCategory =
          PieceManager.Instance.AddPieceCategory("_CultivatorPieceTable", "CreatorShop");

      foreach (
          KeyValuePair<string, Dictionary<string, int>> entry in
              Requirements.CultivatorCreatorShopItems.OrderBy(o => o.Key).ToList()) {
        GetOrAddPieceComponent(entry.Key, pieceTable)
            .SetResources(CreateRequirements(entry.Value))
            .SetCategory(_cultivatorCreatorShopCategory)
            .SetCraftingStation(GetCraftingStation(Requirements.craftingStationRequirements, entry.Key))
            .SetCanBeRemoved(true)
            .SetTargetNonPlayerBuilt(false);
      }

      ResizePieceTableCategories(pieceTable, (int) _cultivatorCreatorShopCategory + 1);
    }

    static void ResizePieceTableCategories(PieceTable pieceTable, int pieceCategoryMax) {
      while (pieceTable.m_availablePieces.Count < pieceCategoryMax) {
        pieceTable.m_availablePieces.Add(new());
      }

      Array.Resize(ref pieceTable.m_selectedPiece, pieceCategoryMax);
    }

    static Piece.Requirement[] CreateRequirements(Dictionary<string, int> data) {
      Piece.Requirement[] requirements = new Piece.Requirement[data.Count];
      for (int index = 0; index < data.Count; index++) {
        KeyValuePair<string, int> item = data.ElementAt(index);
        Piece.Requirement req = new();
        req.m_resItem = PrefabManager.Cache.GetPrefab<GameObject>(item.Key).GetComponent<ItemDrop>();
        req.m_amount = item.Value;
        requirements[index] = req;
      }
      return requirements;
    }

    static Piece GetExistingPiece(string prefabName) {
      return ZNetScene.s_instance.Ref()?.GetPrefab(prefabName).Ref()?.GetComponent<Piece>();
    }

    static Piece GetOrAddPieceComponent(string prefabName, PieceTable pieceTable) {
      GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);

      if (!prefab.TryGetComponent(out Piece piece)) {
        piece = prefab.AddComponent<Piece>();
        piece.m_name = FormatPrefabName(prefab.name);

        SetPlacementRestrictions(piece);
      }

      if (!piece.m_icon) {
        piece.m_icon = LoadOrRenderIcon(prefab, _prefabIconRenderRotation, _standardPrefabIconSprite);
      }

      if (!pieceTable.m_pieces.Contains(prefab)) {
        pieceTable.m_pieces.Add(prefab);
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

    public static bool IsNewDvergrPiece(Piece piece) {
      if (DvergrPieces.DvergrPrefabs.Keys.Contains(piece.m_description)) {
        return true;
      }

      return false;
    }

    public static bool IsCreatorShopPiece(Piece piece) {
      if (Requirements.HammerCreatorShopItems.Keys.Contains(piece.m_description)) {
        return true;
      }

      return false;
    }

    public static bool IsDestructibleCreatorShopPiece(string prefabName) {
      if (Requirements.HammerCreatorShopItems.Keys.Contains(prefabName)) {
        return true;
      }

      return false;
    }

    public static bool IsDestructibleCreatorShopPiece(Piece piece) {
      if (Requirements.HammerCreatorShopItems.Keys.Contains(piece.m_description)) {
        return true;
      }

      return false;
    }

    public static bool IsBuildHammerItem(string prefabName) {
      if (Requirements.HammerCreatorShopItems.Keys.Contains(prefabName)) {
        return true;
      }

      return false;
    }

    public static bool HasCraftingStationRequirement(string prefabName) {
      if (Requirements.craftingStationRequirements.Keys.Contains(prefabName)) {
        return true;
      }

      return false;
    }

    public static CraftingStation GetCraftingStation(Dictionary<string, string> requirements, string prefabName) {
      if (requirements.ContainsKey(prefabName)) {
        return PrefabManager.Instance
            .GetPrefab(requirements[prefabName])
            .GetComponent<CraftingStation>();
      }

      return null;
    }
  }
}