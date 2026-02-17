namespace EmDee;

using UnityEngine;

public sealed class EmDeeContext {
  public Transform ParentTransform { get; private set; }

  public EmDeeContext(Transform parentTransform) {
    this.ParentTransform = parentTransform;
  }
}
