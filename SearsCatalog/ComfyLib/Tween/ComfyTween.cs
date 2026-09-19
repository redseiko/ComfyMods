namespace ComfyLib;

using System;
using System.Collections;

using UnityEngine;

public sealed class ComfyTween<T> {
  readonly MonoBehaviour _runner;
  readonly float _duration;
  readonly Func<T> _getCurrentValue;
  readonly Action<T> _onUpdate;
  readonly Func<T, T, float, T> _lerpFunc;

  Coroutine _coroutine;

  public ComfyTween(
      MonoBehaviour runner,
      float duration,
      Func<T> getCurrentValue,
      Action<T> onUpdate,
      Func<T, T, float, T> lerpFunc) {
    _runner = runner;
    _duration = duration;
    _getCurrentValue = getCurrentValue;
    _onUpdate = onUpdate;
    _lerpFunc = lerpFunc;
  }

  public void To(T targetValue) {
    if (_coroutine != null) {
      _runner.StopCoroutine(_coroutine);
    }

    _coroutine = _runner.StartCoroutine(TweenCoroutine(_getCurrentValue(), targetValue));
  }

  IEnumerator TweenCoroutine(T start, T target) {
    float timeElapsed = 0f;

    while (timeElapsed < _duration) {
      float t = timeElapsed / _duration;
      t = t * t * (3f - (2f * t));

      _onUpdate(_lerpFunc(start, target, t));
      timeElapsed += Time.deltaTime;

      yield return null;
    }

    _onUpdate(target);
    _coroutine = null;
  }
}
