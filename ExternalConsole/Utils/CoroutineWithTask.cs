using System.Collections;
using System.Threading.Tasks;

namespace ComfyLib {
  public sealed class CoroutineWithTask : IEnumerator {
    public Task WrappedTask { get; }

    public CoroutineWithTask(Task task) {
      WrappedTask = task;
    }

    public object Current => null;

    public bool MoveNext() {
      return !WrappedTask.IsCompleted;
    }

    public void Reset() { }
  }

  public sealed class CoroutineWithTask<T> : IEnumerator {
    public Task<T> WrappedTask { get; }
    public T Result { get => WrappedTask.Result; }

    public CoroutineWithTask(Task<T> task) {
      WrappedTask = task;
    }

    public object Current => null;

    public bool MoveNext() {
      return !WrappedTask.IsCompleted;
    }

    public void Reset() {}
  }

  public static class CoroutineWithTaskExtensions {
    public static CoroutineWithTask AsCoroutine(this Task task) {
      return new(task);
    }

    public static CoroutineWithTask<T> AsCoroutine<T>(this Task<T> task) {
      return new(task);
    }
  }
}
