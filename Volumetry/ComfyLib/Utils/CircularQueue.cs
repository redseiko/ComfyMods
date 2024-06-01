namespace ComfyLib;

using System;
using System.Collections.Generic;

public sealed class CircularQueue<T> : Queue<T> {
  readonly int _capacity;
  readonly Action<T> _dequeueFunc;

  public CircularQueue(int capacity, Action<T> dequeueFunc = default) {
    _capacity = capacity;
    _dequeueFunc = dequeueFunc;
  }

  public void EnqueueItem(T item) {
    while (Count + 1 > _capacity) {
      T dequeuedItem = Dequeue();
      _dequeueFunc?.Invoke(dequeuedItem);
    }

    Enqueue(item);
  }

  public void ClearItems() {
    while (Count > 0) {
      T dequeuedItem = Dequeue();
      _dequeueFunc?.Invoke(dequeuedItem);
    }
  }
}
