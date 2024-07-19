﻿namespace PotteryBarn;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Jotunn.Managers;

using UnityEngine;

public static class PotteryManager {
  public static readonly Regex PrefabNameRegex = new(@"([a-z])([A-Z])");

  public static readonly string HammerPieceTable = "_HammerPieceTable";
  public static PieceTable GetHammerPieceTable() => PieceManager.Instance.GetPieceTable(HammerPieceTable);

  public static Piece.PieceCategory HammerBuildingCategory;
  public static Piece.PieceCategory HammerMiscCategory;

  public static Piece.PieceCategory CreatorShopCategory;
  public static Piece.PieceCategory BuilderShopCategory;

  public static readonly string CultivatorPieceTable = "_CultivatorPieceTable";
  public static PieceTable GetCultivatorPieceTable() => PieceManager.Instance.GetPieceTable(CultivatorPieceTable);

  public static Piece.PieceCategory CultivatorMiscCategory;
  public static Piece.PieceCategory CultivatorCreatorShopCategory;

  public static readonly Quaternion PrefabIconRenderRotation = Quaternion.Euler(0f, -45f, 0f);

  public static bool IsDropTableDisabled { get; set; } = false;

  public static readonly Dictionary<string, Piece> ShopPieces = [];

  public static void AddPieces() {
    PieceManager.OnPiecesRegistered -= AddPieces;

    SetupHammerCategories(PieceManager.Instance);
    SetupCultivatorCategories(PieceManager.Instance);

    AddHammerPieces(PieceManager.Instance);
    AddCultivatorPieces(PieceManager.Instance.GetPieceTable("_CultivatorPieceTable"));
  }

  public static void SetupHammerCategories(PieceManager pieceManager) {
    HammerBuildingCategory = Piece.PieceCategory.BuildingWorkbench;
    HammerMiscCategory = Piece.PieceCategory.Misc;

    CreatorShopCategory = pieceManager.AddPieceCategory(HammerPieceTable, "CreatorShop");
    BuilderShopCategory = pieceManager.AddPieceCategory(HammerPieceTable, "BuilderShop");
  }

  public static void AddHammerPieces(PieceManager pieceManager) {  
    PieceTable pieceTable = pieceManager.GetPieceTable(HammerPieceTable);

    pieceTable.AddPiece(GetExistingPiece("turf_roof").SetName("turf_roof"));
    pieceTable.AddPiece(GetExistingPiece("turf_roof_top").SetName("turf_roof_top"));
    pieceTable.AddPiece(GetExistingPiece("turf_roof_wall").SetName("turf_roof_wall"));

    pieceTable.AddPiece(
        GetExistingPiece("ArmorStand_Female")
            .SetName("ArmorStand_Female")
            .SetComfort(0));

    pieceTable.AddPiece(
        GetExistingPiece("ArmorStand_Male")
            .SetName("ArmorStand_Male"));

    pieceTable.AddPiece(
        GetExistingPiece("stone_floor")
            .ModifyResource("Stone", amount: 12, recover: true));

    pieceTable.AddPiece(
        GetExistingPiece("wood_ledge")
            .ModifyResource("Wood", amount: 1, recover: true));

    foreach (
        KeyValuePair<string, Dictionary<string, int>> entry in
            Requirements.HammerCreatorShopItems.OrderBy(o => o.Key).ToList()) {
      pieceTable.AddPiece(
          GetOrAddPiece(entry.Key)
              .SetResources(CreateRequirements(entry.Value))
              .SetCategory(CreatorShopCategory)
              .SetCraftingStation(GetCraftingStation(Requirements.CraftingStationRequirements, entry.Key))
              .SetCanBeRemoved(true)
              .SetTargetNonPlayerBuilt(false));
    }

    foreach (
        KeyValuePair<string, Dictionary<string, int>> entry in
            Requirements.MiscPrefabs.OrderBy(o => o.Key).ToList()) {
      pieceTable.AddPiece(
          GetOrAddPiece(entry.Key)
              .SetResources(CreateRequirements(entry.Value))
              .SetCategory(HammerMiscCategory)
              .SetCraftingStation(GetCraftingStation(Requirements.CraftingStationRequirements, entry.Key))
              .SetCanBeRemoved(true)
              .SetTargetNonPlayerBuilt(true));
    }

    foreach (
        KeyValuePair<string, Dictionary<string, int>> entry in
            DvergrPieces.DvergrPrefabs.OrderBy(o => o.Key).ToList()) {
      pieceTable.AddPiece(
          GetOrAddPiece(entry.Key)
              .SetResources(CreateRequirements(entry.Value))
              .SetCategory(HammerBuildingCategory)
              .SetCraftingStation(GetCraftingStation(DvergrPieces.DvergrPrefabCraftingStationRequirements, entry.Key))
              .SetCanBeRemoved(true)
              .SetTargetNonPlayerBuilt(false));
    }

    AddPotteryPieces(GetHammerPieceTable(), CreatorShopCategory, CreatorShop.HammerPieces);
    AddPotteryPieces(GetHammerPieceTable(), BuilderShopCategory, BuilderShop.HammerPieces);
  }

  public static void SetupCultivatorCategories(PieceManager pieceManager) {
    CultivatorMiscCategory = Piece.PieceCategory.Misc;
    CultivatorCreatorShopCategory = pieceManager.AddPieceCategory(CultivatorPieceTable, "CreatorShop");

    pieceManager.GetPieceTable(CultivatorPieceTable)
        .m_useCategories = true;
  }

  public static void AddCultivatorPieces(PieceTable cultivatorPieceTable) {
    AddPotteryPieces(cultivatorPieceTable, CultivatorMiscCategory, VanillaShop.CultivatorPieces);
    AddPotteryPieces(cultivatorPieceTable, CreatorShopCategory, CreatorShop.CultivatorPieces);
  }

  public static void AddPotteryPieces(
      PieceTable pieceTable, Piece.PieceCategory pieceCategory, PotteryPieceList potteryPieces) {
    foreach (PotteryPiece potteryPiece in potteryPieces) {
      Piece piece = GetOrAddPiece(potteryPiece.PiecePrefab);

      piece
          .SetCategory(pieceCategory)
          .SetResources(CreatePieceRequirements(potteryPiece.PieceResources))
          .SetCraftingStation(GetCraftingStation(potteryPiece.CraftingStation))
          .SetCanBeRemoved(true);

      pieceTable.AddPiece(piece);
      ShopPieces.Add(potteryPiece.PiecePrefab, piece);

      if (!piece.TryGetComponent(out WearNTear _)) {
        Jotunn.Logger.LogInfo($"{piece.name},-WearNTear");
      }

      if (piece.TryGetComponent(out StaticPhysics _)) {
        Jotunn.Logger.LogInfo($"{piece.name},+StaticPhysics");
      }
    }
  }

  public static Piece GetExistingPiece(string prefabName) {
    return PrefabManager.Instance.GetPrefab(prefabName).GetComponent<Piece>();
  }

  public static Piece GetOrAddPiece(string prefabName) {
    GameObject prefab = PrefabManager.Instance.GetPrefab(prefabName);

    if (!prefab.TryGetComponent(out Piece piece)) {
      piece = prefab.AddComponent<Piece>();
      piece.m_name = piece.name;

      SetPlacementRestrictions(piece);
    }

    if (!piece.m_icon) {
      piece.m_icon = LoadOrRenderIcon(prefab, PrefabIconRenderRotation);
    }

    piece.m_description = prefab.name;

    return piece;
  }

  public static Piece.Requirement[] CreatePieceRequirements(ICollection<PieceResource> pieceResources) {
    return pieceResources.Select(CreatePieceRequirement).ToArray();
  }

  public static Piece.Requirement CreatePieceRequirement(PieceResource pieceResource) {
    return new() {
      m_resItem = PrefabManager.Instance.GetPrefab(pieceResource.Name).GetComponent<ItemDrop>(),
      m_amount = pieceResource.Amount,
    };
  }

  public static CraftingStation GetCraftingStation(string prefabName) {
    if (string.IsNullOrEmpty(prefabName)) {
      return default;
    }

    return PrefabManager.Instance.GetPrefab(prefabName).GetComponent<CraftingStation>();
  }

  public static Piece SetPlacementRestrictions(Piece piece) {
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

  public static string FormatPrefabName(string prefabName) {
    return PrefabNameRegex
        .Replace(prefabName, "$1 $2")
        .TrimStart(' ')
        .Replace('_', ' ')
        .Replace("  ", " ");
  }

  public static Piece.Requirement[] CreateRequirements(Dictionary<string, int> data) {
    Piece.Requirement[] requirements = new Piece.Requirement[data.Count];
    for (int index = 0; index < data.Count; index++) {
      KeyValuePair<string, int> item = data.ElementAt(index);

      Piece.Requirement req = new() {
        m_resItem = PrefabManager.Cache.GetPrefab<GameObject>(item.Key).GetComponent<ItemDrop>(),
        m_amount = item.Value
      };

      requirements[index] = req;
    }

    return requirements;
  }

  public static CraftingStation GetCraftingStation(Dictionary<string, string> requirements, string prefabName) {
    if (requirements.ContainsKey(prefabName)) {
      return PrefabManager.Instance
          .GetPrefab(requirements[prefabName])
          .GetComponent<CraftingStation>();
    }

    return null;
  }

  public static bool IsShopPiece(Piece piece) {
    return IsShopPiecePrefab(piece.m_description);
  }

  public static bool IsShopPiecePrefab(string prefabName) {
    return
        Requirements.HammerCreatorShopItems.ContainsKey(prefabName)
        || ShopPieces.ContainsKey(prefabName);
  }

  public static bool IsDvergrPiece(Piece piece) {
    return DvergrPieces.DvergrPrefabs.ContainsKey(piece.m_description);
  }

  public static bool CanBeRemoved(Piece piece) {
    if (IsShopPiece(piece) && !piece.IsCreator()) {
      return false;
    }

    if (IsDvergrPiece(piece) && !piece.IsPlacedByPlayer()) {
      return false;
    }

    return true;
  }

  public static Sprite LoadOrRenderIcon(GameObject prefab, Quaternion renderRotation) {
    RenderManager.RenderRequest request = new(prefab) {
      Rotation = renderRotation,
    };

    return RenderManager.Instance.Render(request);
  }
}
