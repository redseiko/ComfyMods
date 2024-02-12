namespace Intermission;

using System.Collections.Generic;

public sealed class RandomStringComparer : Comparer<string> {
  public static readonly RandomStringComparer Instance = new();

  public override int Compare(string x, string y) {
    return UnityEngine.Random.Range(-1, 2);
  }
}
