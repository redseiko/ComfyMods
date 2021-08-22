﻿using System.Collections;
using System.Threading.Tasks;

namespace OdinSaves {
  internal static class TaskExtensions {
    public static IEnumerator AsIEnumerator(this Task task) {
      while (!task.IsCompleted) {
        yield return null;
      }

      if (task.IsFaulted) {
        throw task.Exception;
      }
    }
  }
}
