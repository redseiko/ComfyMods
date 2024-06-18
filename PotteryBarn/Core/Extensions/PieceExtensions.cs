namespace PotteryBarn;

using System;

public static class PieceExtensions {
  public static Piece SetCanBeRemoved(this Piece piece, bool canBeRemoved) {
    piece.m_canBeRemoved = canBeRemoved;
    return piece;
  }

  public static Piece SetCategory(this Piece piece, Piece.PieceCategory pieceCategory) {
    piece.m_category = pieceCategory;
    return piece;
  }

  public static Piece SetComfort(this Piece piece, int comfort) {
    piece.m_comfort = comfort;
    return piece;
  }

  public static Piece SetCraftingStation(this Piece piece, CraftingStation craftingStation) {
    piece.m_craftingStation = craftingStation;
    return piece;
  }

  public static Piece SetGroundOnly(this Piece piece, bool groundOnly) {
    piece.m_groundOnly = groundOnly;
    return piece;
  }

  public static Piece SetName(this Piece piece, string name) {
    piece.m_name = name;
    return piece;
  }

  public static Piece SetResources(this Piece piece, params Piece.Requirement[] requirements) {
    piece.m_resources = requirements;
    return piece;
  }

  public static Piece SetTargetNonPlayerBuilt(this Piece piece, bool canTarget) {
    piece.m_targetNonPlayerBuilt = canTarget;
    return piece;
  }

  public static Piece ModifyResource(
      this Piece piece,
      string resourceName,
      int? amount = default,
      int? amountPerLevel = default,
      bool? recover = default) {
    if (!amount.HasValue && !amountPerLevel.HasValue && !recover.HasValue) {
      throw new InvalidOperationException($"At least one of the optional parameters must be specified!");
    }

    Piece.Requirement resource = GetResource(piece, resourceName);

    if (amount.HasValue) {
      resource.m_amount = amount.Value;
    }

    if (amountPerLevel.HasValue) {
      resource.m_amountPerLevel = amountPerLevel.Value;
    }

    if (recover.HasValue) {
      resource.m_recover = recover.Value;
    }

    return piece;
  }

  public static Piece.Requirement GetResource(this Piece piece, string resourceName) {
    foreach (Piece.Requirement requirement in piece.m_resources) {
      if (requirement.m_resItem.name == resourceName) {
        return requirement;
      }
    }

    throw new InvalidOperationException($"No matching resource '{resourceName}' for Piece {piece.m_name}.");
  }
}
