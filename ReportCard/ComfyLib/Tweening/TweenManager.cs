namespace ComfyLib;

using System.Collections.Generic;

using UnityEngine;

public class TweenManager : MonoBehaviour {
  static TweenManager _instance;

  public static TweenManager Instance {
    get {
      if (!_instance) {
        GameObject manager = new("TweenManager");
        _instance = manager.AddComponent<TweenManager>();
      }

      return _instance;
    }
  }

  readonly List<Tween> _tweens = [];
  readonly List<Tween> _tweensToAdd = [];

  void Update() {
    if (_tweensToAdd.Count > 0) {
      _tweens.AddRange(_tweensToAdd);
      _tweensToAdd.Clear();
    }

    for (int i = _tweens.Count - 1; i >= 0; i--) {
      Tween tween = _tweens[i];

      if (tween.IsComplete) {
        _tweens.RemoveAt(i);
        continue;
      }

      tween.Update(Time.deltaTime);
    }
  }

  public static void Add(Tween tween) {
    Instance._tweensToAdd.Add(tween);
  }
}
