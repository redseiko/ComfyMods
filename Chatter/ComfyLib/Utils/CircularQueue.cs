namespace ComfyLib;

using System;
using System.Collections.Concurrent;

public sealed class CircularQueue<T> : ConcurrentQueue<T> {
  readonly int _capacity;
  readonly Action<T> _dequeueFunc;

  public CircularQueue(int capacity, Action<T> dequeueFunc) {
    _capacity = capacity;
    _dequeueFunc = dequeueFunc;
  }

  T _lastItem = default;

  public T LastItem {
    get => _lastItem;
  }

  public void EnqueueItem(T item) {
    while (Count + 1 > _capacity) {
      if (TryDequeue(out T itemToDequeue)) {
        _dequeueFunc(itemToDequeue);
      }
    }

    Enqueue(item);
    _lastItem = item;
  }

  public void ClearItems() {
    while (TryDequeue(out T itemToDequeue)) {
      _dequeueFunc(itemToDequeue);
    }

    _lastItem = default;
  }
}
