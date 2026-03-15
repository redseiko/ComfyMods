namespace ColorfulPieces;

using UnityEngine;

public interface IPieceColorable {
  void UpdateColors(bool forceUpdate = false);
  void OverrideColors(Color color, Color emissionColor);
}
