namespace PotteryBarn;

public static class PieceRequirementExtensions {
  public static Piece.Requirement SetAmount(this Piece.Requirement requirement, int amount) {
    requirement.m_amount = amount;
    return requirement;
  }

  public static Piece.Requirement SetRecover(this Piece.Requirement requirement, bool recover) {
    requirement.m_recover = recover;
    return requirement;
  }
}
