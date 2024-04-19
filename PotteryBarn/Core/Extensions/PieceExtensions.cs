namespace PotteryBarn;

using System;
using System.Linq;

using ComfyLib;

public static class PieceExtensions {
  public static Piece SetCanBeRemoved(this Piece piece, bool canBeRemoved) {
    piece.m_canBeRemoved = canBeRemoved;
    return piece;
  }

  public static Piece SetCategory(this Piece piece, Piece.PieceCategory pieceCategory) {
    piece.m_category = pieceCategory;
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

  public static Piece SetResource(
      this Piece piece, string resourceName, Action<Piece.Requirement> modifyResourceAction) {
    modifyResourceAction?.Invoke(piece.GetResource(resourceName));
    return piece;
  }

  public static Piece SetTargetNonPlayerBuilt(this Piece piece, bool canTarget) {
    piece.m_targetNonPlayerBuilt = canTarget;
    return piece;
  }

  public static Piece.Requirement GetResource(this Piece piece, string resourceName) {
    return piece.Ref()?.m_resources?.Where(req => req?.m_resItem.Ref()?.name == resourceName).FirstOrDefault();
  }
}

