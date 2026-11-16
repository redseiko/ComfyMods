namespace Queryable;

public sealed class ItemSlot {
  public readonly int ItemHash;
  public readonly int ItemDataHash;

  public ItemSlot(int index) {
    string indexStr = index.ToString();

    ItemHash = $"{indexStr}_item".GetStableHashCode();
    ItemDataHash = $"{indexStr}_itemData".GetStableHashCode();
  }
}
