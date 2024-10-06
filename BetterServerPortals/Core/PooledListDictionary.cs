namespace BetterServerPortals;

using System.Collections;
using System.Collections.Generic;

using UnityEngine.Pool;

public sealed class PooledListDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>> {
  readonly Dictionary<TKey, List<TValue>> _keyToListDictionary = [];

  public List<TValue> this[TKey index] {
    get {
      if (!_keyToListDictionary.TryGetValue(index, out List<TValue> list)) {
        list = _listPool.Get();
        _keyToListDictionary[index] = list;
      }

      return list;
    }
  }

  public void Add(TKey key, TValue value) {
    this[key].Add(value);
  }

  public void Clear() {
    foreach (List<TValue> list in _keyToListDictionary.Values) {
      _listPool.Release(list);
    }

    _keyToListDictionary.Clear();
  }

  public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator() {
    return _keyToListDictionary.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }

  static readonly ObjectPool<List<TValue>> _listPool =
      new(
          createFunc: CreateList,
          actionOnGet: ClearList,
          actionOnRelease: ClearList,
          actionOnDestroy: ClearList,
          collectionCheck: false,
          defaultCapacity: 0);

  static List<TValue> CreateList() {
    return [];
  }

  static void ClearList(List<TValue> list) {
    list.Clear();
  }
}
