namespace Queryable;

public sealed class ItemSlot {
  public readonly int ItemHash;
  public readonly int DurabiltyHash;
  public readonly int StackHash;
  public readonly int QualityHash;
  public readonly int VariantHash;
  public readonly int CrafterIdHash;
  public readonly int CrafterNameHash;
  public readonly int DataCountHash;
  public readonly int WorldLevelHash;
  public readonly int PickedUpHash;

  public ItemSlot(int index) {
    string indexStr = index.ToString();

    ItemHash = $"{indexStr}_item".GetStableHashCode();
    DurabiltyHash = $"{indexStr}_durability".GetStableHashCode();
    StackHash = $"{indexStr}_stack".GetStableHashCode();
    QualityHash = $"{indexStr}_quality".GetStableHashCode();
    VariantHash = $"{indexStr}_variant".GetStableHashCode();
    CrafterIdHash = $"{indexStr}_crafterID".GetStableHashCode();
    CrafterNameHash = $"{indexStr}_crafterName".GetStableHashCode();
    DataCountHash = $"{indexStr}_dataCount".GetStableHashCode();
    WorldLevelHash = $"{indexStr}_worldLevel".GetStableHashCode();
    PickedUpHash = $"{indexStr}_pickedUp".GetStableHashCode();
  }
}
