using System.Collections;
using System.Threading.Tasks;

namespace BetterZeeDeeOhs {
  public static class TaskExtensions {
    public static IEnumerator ToIEnumerator(this Task task) {
      while (!task.IsCompleted) {
        yield return null;
      }

      if (task.IsFaulted) {
        ZLog.LogError($"Task failed with exception!\n{task.Exception}");
      }
    }
  }
}
