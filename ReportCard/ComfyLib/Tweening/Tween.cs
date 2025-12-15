namespace ComfyLib;

using System;

using UnityEngine;

public abstract class Tween {
  public UnityEngine.Object Target { get; protected set; }
  public float Duration { get; protected set; }
  public Ease EaseType { get; protected set; } = Ease.Linear;
  public bool IsComplete { get; protected set; }
  
  protected float _elapsedTime;
  protected Action _onCompleteCallback;

  public Tween SetEase(Ease ease) {
    EaseType = ease;
    return this;
  }

  public Tween OnComplete(Action callback) {
    _onCompleteCallback = callback;
    return this;
  }

  public void Update(float dt) {
    if (IsComplete) {
      return;
    }

    if (!Target) {
      IsComplete = true;
      return;
    }

    _elapsedTime += dt;

    float t = Mathf.Clamp01(_elapsedTime / Duration);
    float easedT = Easing.Evaluate(t, EaseType);

    OnUpdate(easedT);

    if (_elapsedTime >= Duration) {
      IsComplete = true;
      _onCompleteCallback?.Invoke();
    }
  }

  protected abstract void OnUpdate(float t);
}

public sealed class FloatTween : Tween {
  readonly Action<float> _setter;
  readonly float _start;
  readonly float _end;

  public FloatTween(UnityEngine.Object target, float start, float end, float duration, Action<float> setter) {
    Target = target;

    _start = start;
    _end = end;

    Duration = duration;

    _setter = setter;
  }

  protected override void OnUpdate(float t) {
    _setter(Mathf.LerpUnclamped(_start, _end, t));
  }
}

public sealed class Vector3Tween : Tween {
  readonly Action<Vector3> _setter;
  Vector3 _start;
  Vector3 _end;

  public Vector3Tween(UnityEngine.Object target, Vector3 start, Vector3 end, float duration, Action<Vector3> setter) {
    Target = target;

    _start = start;
    _end = end;

    Duration = duration;

    _setter = setter;
  }

  protected override void OnUpdate(float t) {
    _setter(Vector3.LerpUnclamped(_start, _end, t));
  }
}
