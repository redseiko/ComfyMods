namespace PotteryBarn;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ComfyLib;

using Jotunn.Managers;

using UnityEngine;

public static class PotteryManager {
  public static readonly Regex PrefabNameRegex = new(@"([a-z])([A-Z])");

  public static readonly string HammerPieceTable = "_HammerPieceTable";
  public static PieceTable GetHammerPieceTable() => PieceManager.Instance.GetPieceTable(HammerPieceTable);

  public static readonly string CultivatorPieceTable = "_CultivatorPieceTable";
  public static PieceTable GetCultivatorPieceTable() => PieceManager.Instance.GetPieceTable(CultivatorPieceTable);

  public static Piece.PieceCategory BuildingCategory;
  public static Piece.PieceCategory MiscCategory;

  public static Piece.PieceCategory CreatorShopCategory;
  public static Piece.PieceCategory BuilderShopCategory;

  public static readonly Quaternion PrefabIconRenderRotation = Quaternion.Euler(0f, -37.5f, 0f);

  public static readonly Dictionary<string, Piece> VanillaPieces = [];
  public static readonly Dictionary<string, Piece> ShopPieces = [];

  public static readonly Dictionary<string, Piece.Requirement[]> VanillaPieceResources = [];

  public static void AddPieces() {
    PieceManager.OnPiecesRegistered -= AddPieces;

    SetupCategories(PieceManager.Instance);

    AddHammerPieces(GetHammerPieceTable());
    AddCultivatorPieces(GetCultivatorPieceTable());
  }

  public static void SetupCategories(PieceManager pieceManager) {
    BuildingCategory = Piece.PieceCategory.BuildingWorkbench;
    MiscCategory = Piece.PieceCategory.Misc;

    CreatorShopCategory = pieceManager.AddPieceCategory("CreatorShop");
    BuilderShopCategory = pieceManager.AddPieceCategory("BuilderShop");
  }

  public static void AddHammerPieces(PieceTable hammerPieceTable) {  
    hammerPieceTable.AddPiece(GetExistingPiece("turf_roof").SetName("turf_roof"));
    hammerPieceTable.AddPiece(GetExistingPiece("turf_roof_top").SetName("turf_roof_top"));
    hammerPieceTable.AddPiece(GetExistingPiece("turf_roof_wall").SetName("turf_roof_wall"));

    hammerPieceTable.AddPiece(
        GetExistingPiece("ArmorStand_Female")
            .SetName("ArmorStand_Female")
            .SetComfort(0));

    hammerPieceTable.AddPiece(
        GetExistingPiece("ArmorStand_Male")
            .SetName("ArmorStand_Male"));

    hammerPieceTable.AddPiece(
        GetExistingPiece("stone_floor")
            .ModifyResource("Stone", amount: 12, recover: true));

    hammerPieceTable.AddPiece(
        GetExistingPiece("wood_ledge")
            .ModifyResource("Wood", amount: 1, recover: true));

    AddPotteryPieces(hammerPieceTable, BuildingCategory, VanillaShop.BuildingPieces, VanillaPieces);
    AddPotteryPieces(hammerPieceTable, MiscCategory, VanillaShop.MiscPieces, VanillaPieces);

    AddPotteryPieces(hammerPieceTable, CreatorShopCategory, CreatorShop.HammerPieces, ShopPieces);
    AddPotteryPieces(hammerPieceTable, BuilderShopCategory, BuilderShop.HammerPieces, ShopPieces);
  }

  public static void AddCultivatorPieces(PieceTable cultivatorPieceTable) {
    AddPotteryPieces(cultivatorPieceTable, MiscCategory, VanillaShop.CultivatorPieces, VanillaPieces);
    AddPotteryPieces(cultivatorPieceTable, CreatorShopCategory, CreatorShop.CultivatorPieces, ShopPieces);
  }

  public static void AddPotteryPieces(
      PieceTable pieceTable,
      Piece.PieceCategory pieceCategory,
      PotteryPieceList potteryPieces,
      Dictionary<string, Piece> pieceByNameCache) {
    foreach (PotteryPiece potteryPiece in potteryPieces) {
      Piece piece = GetOrAddPiece(potteryPiece.PiecePrefab);

      piece
          .SetCategory(pieceCategory)
          .SetResources(CreatePieceRequirements(potteryPiece.PieceResources))
          .SetCraftingStation(GetCraftingStation(potteryPiece.CraftingStation))
          .SetCanBeRemoved(true);

      pieceTable.AddPiece(piece);
      pieceByNameCache.Add(potteryPiece.PiecePrefab, piece);
    }
  }

  public static Piece GetExistingPiece(string prefabName) {
    return PrefabManager.Instance.GetPrefab(prefabName).GetComponent<Piece>();
  }

  public static Piece GetOrAddPiece(string prefabName) {
    GameObject prefab = PrefabManager.Instance.GetPrefab(prefabName);

    if (prefab.TryGetComponent(out Piece piece)) {
      if (!VanillaPieceResources.ContainsKey(piece.name)) {
        VanillaPieceResources.Add(piece.name, piece.m_resources);
      }
    } else {
      piece = prefab.AddComponent<Piece>();
      piece.m_name = FormatPrefabName(prefab.name);

      SetPlacementRestrictions(piece);
    }

    piece.m_description = prefab.name;

    if (!piece.m_icon) {
      piece.m_icon = LoadOrRenderIcon(prefab, PrefabIconRenderRotation);
    }

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

  public static readonly Dictionary<string, CraftingStation> CraftingStationCache = [];

  public static CraftingStation GetCraftingStation(string prefabName) {
    if (string.IsNullOrEmpty(prefabName)) {
      return default;
    }

    if (!CraftingStationCache.TryGetValue(prefabName, out CraftingStation craftingStation)) {
      craftingStation = PrefabManager.Instance.GetPrefab(prefabName).GetComponent<CraftingStation>();
      CraftingStationCache.Add(prefabName, craftingStation);
    }

    return craftingStation;
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

  public static bool IsShopPiece(Piece piece) {
    return ShopPieces.ContainsKey(piece.m_description);
  }

  public static bool IsVanillaPiece(Piece piece) {
    return VanillaPieces.ContainsKey(piece.m_description);
  }

  public static Sprite LoadOrRenderIcon(GameObject prefab, Quaternion renderRotation) {
    RenderManager.RenderRequest request = new(prefab) {
      Rotation = renderRotation,
    };

    return RenderManager.Instance.Render(request);
  }

  public static readonly DropTable EmptyDropTable = new();
  public static bool IsPlacingPiece { get; set; } = false;

  public static void SetupPiece(Piece piece) {
    bool isShopPiece = IsShopPiece(piece);
    bool isVanillaPiece = IsVanillaPiece(piece);

    if (!isShopPiece && !isVanillaPiece) {
      return;
    }

    if (isShopPiece) {
      piece.m_canBeRemoved = piece.IsCreator();
    }

    if (isVanillaPiece) {
      piece.m_canBeRemoved = true;
    }

    if (IsPlacingPiece) {
      piece.SetIsPlacedByPotteryBarn(true);
    } else if (!piece.IsPlacedByPotteryBarn()) {
      if (VanillaPieceResources.TryGetValue(piece.name, out Piece.Requirement[] resources)) {
        piece.m_resources = resources;
      } else {
        piece.m_resources = [];
      }

      return;
    }

    if (piece.TryGetComponent(out DropOnDestroyed dropOnDestroyed)) {
      dropOnDestroyed.m_dropWhenDestroyed = EmptyDropTable;
    }

    if (piece.TryGetComponent(out Destructible destructible) && destructible.m_spawnWhenDestroyed) {
      CustomFieldUtils.SetDestructibleFields(destructible, "vfx_SawDust");
    }
  }

  public static void PlacePieceDoAttackPreDelegate(GameObject clonedObject) {
    if (clonedObject.TryGetComponent(out Piece piece)) {
      SetupPiece(piece);
    }

    if (clonedObject.TryGetComponent(out Container container)) {
      container.GetInventory().RemoveAll();
    }
  }

  public static int IsPlacedByPotteryBarnHashCode = "IsPlacedByPotteryBarn".GetStableHashCode();

  public static void SetIsPlacedByPotteryBarn(this Piece piece, bool isPlacedByPotteryBarn) {
    if (piece.m_nview && piece.m_nview.IsValid()) {
      piece.m_nview.m_zdo.Set(IsPlacedByPotteryBarnHashCode, true);
    }
  }

  public static bool IsPlacedByPotteryBarn(this Piece piece) {
    return
        piece.m_nview
        && piece.m_nview.IsValid()
        && piece.m_nview.m_zdo.GetBool(IsPlacedByPotteryBarnHashCode, false);
  }
}
