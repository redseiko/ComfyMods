////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                             
//      _____ _            _____                   
//     |_   _|_|___ _ _   |_   _|_ _ _ ___ ___ ___ 
//       | | | |   | | |    | | | | | | -_| -_|   |
//       |_| |_|_|_|_  |    |_| |_____|___|___|_|_|
//                 |___|
// A Complete and Easy to use Tweens library in One File
//
// Basic use:
// using FronkonGames.TinyTween;
// GameObject gameObject = new();
// gameObject.TweenMove(new Vector3(10.0f, 0.0f, 0.0f), 1.0f, Ease.Bounce);
//
// Advanced use:
// using FronkonGames.TinyTween;
// GameObject clockHand = new();
// TweenQuaternion.Create()
//                .Origin(Quaternion.Euler(-30.0f, 0.0f, 0.0f))
//                .Destination(Quaternion.Euler(30.0f, 0.0f, 0.0f))
//                .Duration(1.0f)
//                .Loop(TweenLoop.YoYo)
//                .EasingIn(Ease.Back)
//                .EasingOut(Ease.Elastic)
//                .Owner(clockHand)
//                .Condition(tween => tween.ExecutionCount < 10)
//                .OnUpdate(tween => clockHand.transform.rotation = tween.Value)
//                .OnEnd(() => Debug.Log("It's show time!"))
//                .Start();
//
// Copyright (c) 2022 Martin Bustos @FronkonGames <fronkongames@gmail.com>
// 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of
// the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FronkonGames.TinyTween
{
  /// <summary> State of a Tween operation. </summary>
  public enum TweenState
  {
    Running, Paused, Finished
  }

  /// <summary> Execution modes of a Tween operation. </summary>
  public enum TweenLoop
  {
    // Just once. Start over from the beginning. Back and forth loop.  
    Once, Loop, YoYo,
  }

  /// <summary> Interface of a tween. </summary>
  public interface ITween
  {
    /// <summary> Tween status. </summary>
    TweenState State { get; }
    
    /// <summary> Update the Tween operation. </summary>
    void Update();
  }

  /// <summary> Generic interface of a tween. </summary>
  public interface ITween<T> : ITween where T : struct
  {
    /// <summary> Current value. </summary>
    T Value { get; }

    /// <summary> Tween operation progress (0, 1). </summary>
    float Progress { get; }

    /// <summary> Executions counter. </summary>
    int ExecutionCount { get; }
    
    /// <summary> Execute a tween operation. </summary>
    Tween<T> Start();

    /// <summary> Pause the tween. </summary>
    void Pause();

    /// <summary> Continue the tween. </summary>
    void Resume();

    /// <summary> Finish the Tween operation. </summary>
    /// <param name="moveToEnd">Move the value at the end or leave it as this.</param>
    void Stop(bool moveToEnd = true);

    /// <summary> Sets tween value at origin and time to 0. </summary>
    void Reset();
  }

  /// <summary> Tween operation. If it is created manually, Update() must be called. </summary>
  public abstract class Tween<T> : ITween<T> where T : struct
  {
    /// <inheritdoc/>
    public TweenState State { get; private set; } = TweenState.Paused;

    /// <inheritdoc/>
    public T Value { get; private set; }

    /// <inheritdoc/>
    public float Progress { get; private set; }

    /// <inheritdoc/>
    public int ExecutionCount { get; private set; }

    /// <summary> Time that the operation takes. </summary>
    public float Time { get; private set; }
    
    /// <summary> Does the Tween depend on another object? </summary>
    private bool IsOwned { get; set; }
    
    private object owner = null;
    
    private T origin, destination;
    private Ease easeIn = Ease.None, easeOut = Ease.None;
    private TweenLoop loop = TweenLoop.Once;

    private float duration = 1.0f;
    private float currentTime;
    private bool clamp;
    private int residueCount = -1;

    private Action<Tween<T>> updateFunction;
    private Action<Tween<T>> endFunction;
    private Func<Tween<T>, bool> condition;

    private readonly Func<Tween<T>, T, T, float, bool, T> interpolationFunction; // Tween, start, end, progress, clamp.
    
    /// <summary> Constructor. </summary>
    /// <param name="lerpFunc">Interpolation function.</param>
    protected Tween(Func<Tween<T>, T, T, float, bool, T> interpolationFunction) => this.interpolationFunction = interpolationFunction;

    /// <summary> Initial value. Use it only to create a new Tween. </summary>
    /// <returns>This.</returns>
    public Tween<T> Origin(T start) { origin = start; return this; }

    /// <summary> Final value. Use it only to create a new Tween. </summary>
    /// <returns>This.</returns>
    public Tween<T> Destination(T end) { destination = end; return this; }

    /// <summary> Time to execute the operation, must be greater than 0. Use it only to create a new Tween. </summary>
    /// <returns>This.</returns>
    public Tween<T> Duration(float duration) { this.duration = Mathf.Max(duration, 0.0f); return this; }

    /// <summary> Execution mode. Use it only to create a new Tween. </summary>
    /// <returns>This.</returns>
    public Tween<T> Loop(TweenLoop loop) { this.loop = loop; return this; }
    
    /// <summary> Easing function. Overwrite the In and Out functions. Use it only to create a new Tween. </summary>
    /// <param name="ease">Easing function.</param>
    /// <returns>This.</returns>
    public Tween<T> Easing(Ease ease) { easeIn = easeOut = ease; return this; }
    
    /// <summary> Easing In function. Use it only to create a new Tween. </summary>
    /// <param name="ease">Easing function.</param>
    /// <returns>This.</returns>
    public Tween<T> EasingIn(Ease ease) { easeIn = ease; return this; }
    
    /// <summary> Easing Out function. Use it only to create a new Tween. </summary>
    /// <param name="ease">Easing function.</param>
    /// <returns>This.</returns>
    public Tween<T> EasingOut(Ease ease) { easeOut = ease; return this; }
    
    /// <summary> Update callback. Use it to apply the Tween values. </summary>
    /// <param name="updateCallback">Callback.</param>
    /// <returns>This.</returns>
    public Tween<T> OnUpdate(Action<Tween<T>> updateCallback) { updateFunction = updateCallback; return this; }

    /// <summary> Executed at the end of the operation (optional). </summary>
    /// <param name="endCallback">Callback.</param>
    /// <returns>This.</returns>
    public Tween<T> OnEnd(Action<Tween<T>> endCallback) { endFunction = endCallback; return this; }

    /// <summary> Condition of progress, stops if the operation is not true (optional). </summary>
    /// <param name="condition">Condition function.</param>
    /// <returns>This.</returns>
    public Tween<T> Condition(Func<Tween<T>, bool> condition) { this.condition = condition; return this; }

    /// <summary> Limits the values of the interpolation to the range [0, 1]. </summary>
    /// <param name="clamp">Clamp.</param>
    /// <returns>This.</returns>
    public Tween<T> Clamp(bool clamp) { this.clamp = clamp; return this; }
    
    /// <summary>
    /// I set an object as the 'owner' of the Tween. If the object is destroyed, the Tween ends and is destroyed.
    /// </summary>
    /// <param name="owner">Owner</param>
    /// <returns>This.</returns>
    public Tween<T> Owner(object owner) { IsOwned = owner != null; this.owner = owner; return this; }

    /// <inheritdoc/>
    public Tween<T> Start()
    {
      Debug.Assert(duration > 0.0f, "[FronkonGames.TinyTween] The duration of the tween should be greater than zero.");
      Debug.Assert(easeIn != Ease.None && easeOut != Ease.None, "[FronkonGames.TinyTween] You must set some kind of Ease.");

      State = TweenState.Running;
      UpdateValue();

      return this;
    }

    /// <inheritdoc/>
    public void Pause() => State = TweenState.Paused;

    /// <inheritdoc/>
    public void Resume() => State = TweenState.Running;

    /// <inheritdoc/>
    public void Stop(bool moveToEnd = true)
    {
      if (State != TweenState.Finished)
      {
        State = TweenState.Finished;
        if (moveToEnd == true)
        {
          currentTime = duration;

          UpdateValue();
        }

        endFunction?.Invoke(this);
      }
    }

    /// <inheritdoc/>
    public void Reset()
    {
      currentTime = 0.0f;
      Value = origin;
    }
    
    /// <inheritdoc/>
    public void Update()
    {
      if (IsOwned == true && owner.Equals(null) || condition != null && condition(this) == false)
        Stop(false);
      else
      {
        currentTime += UnityEngine.Time.deltaTime;
        if (currentTime >= duration)
        {
          residueCount--;
          ExecutionCount++;

          switch (loop)
          {
            case TweenLoop.Once: Stop(); break;

            case TweenLoop.Loop:
              if (residueCount == 0)
                Stop();

              Value = origin;
              currentTime = Progress = 0.0f;
              break;

            case TweenLoop.YoYo:
              if (residueCount == 0)
                Stop();

              (destination, origin) = (origin, destination);
              currentTime = Progress = 0.0f;
              break;

            default: throw new ArgumentOutOfRangeException();
          }
        }
        else
          UpdateValue();
      }
    }

    private void UpdateValue()
    {
      float t = currentTime / duration;
      Progress = EasingFunctions.Calculate(t > 0.5f ? easeOut : easeIn, easeIn != Ease.None, easeOut != Ease.None, t);

      Value = interpolationFunction(this, origin, destination, Progress, clamp);

      updateFunction?.Invoke(this);
    }
  }

  /// <summary>
  /// Tweens manager, update tweens and delete those that have already ended.
  /// It is not necessary if you are in charge of maintaining your Tweens ;)
  /// </summary>
  public sealed class TinyTween : MonoBehaviour
  {
    public static TinyTween Instance => LazyInstance.Value;

    private static readonly Lazy<TinyTween> LazyInstance = new(CreateSingleton);
    
    private readonly List<ITween> tweens = new();

    /// <summary> Add an existing tween. </summary>
    /// <param name="tween">New tween</param>
    /// <returns>Tween.</returns>
    public ITween Add(ITween tween) { tweens.Add(tween); return tween; }

    private static TinyTween CreateSingleton()
    {
      GameObject ownerObject = new("TinyTween");
      TinyTween instance = ownerObject.AddComponent<TinyTween>();
      DontDestroyOnLoad(ownerObject);

      return instance;
    }

    private void Update()
    {
      int count = tweens.Count;
      for (int i = count - 1; i >= 0; --i)
      {
        ITween tween = tweens[i];

        if (tween.State == TweenState.Running)
          tween.Update();

        if (tween.State == TweenState.Finished && i < count)
          tweens.RemoveAt(i);
      }      
    }

    private void OnDisable() => tweens.Clear();
  }
  
  /// <summary> Tween float. </summary>
  public sealed class TweenFloat : Tween<float>
  {
    /// <summary> Create a Tween float and add it to the TinyTween manager. </summary>
    /// <returns>Tween.</returns>
    public static Tween<float> Create() => TinyTween.Instance.Add(new TweenFloat()) as Tween<float>;
    
    private static float Lerp(ITween<float> t, float start, float end, float progress, bool clamp) =>
        clamp == true ? Mathf.Lerp(start, end, progress) : Mathf.LerpUnclamped(start, end, progress);

    private TweenFloat() : base(Lerp) { }
  }

  /// <summary> Tween Vector2. </summary>
  public sealed class TweenVector2 : Tween<Vector2>
  {
    /// <summary> Create a Tween Vector2 and add it to the TinyTween manager. </summary>
    /// <returns>Tween.</returns>
    public static Tween<Vector2> Create() => TinyTween.Instance.Add(new TweenVector2()) as Tween<Vector2>;

    private static Vector2 Lerp(ITween<Vector2> t, Vector2 start, Vector2 end, float progress, bool clamp) =>
      clamp == true ? Vector2.Lerp(start, end, progress) : Vector2.LerpUnclamped(start, end, progress); 

    private TweenVector2() : base(Lerp) { }
  }

  /// <summary> Tween Vector3. </summary>
  public sealed class TweenVector3 : Tween<Vector3>
  {
    /// <summary> Create a Tween Vector3 and add it to the TinyTween manager. </summary>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> Create() => TinyTween.Instance.Add(new TweenVector3()) as Tween<Vector3>;
    
    private static Vector3 Lerp(ITween<Vector3> t, Vector3 start, Vector3 end, float progress, bool clamp) =>
      clamp == true ? Vector3.Lerp(start, end, progress) : Vector3.LerpUnclamped(start, end, progress);

    private TweenVector3() : base(Lerp) { }
  }

  /// <summary> Tween Vector4. </summary>
  public sealed class TweenVector4 : Tween<Vector4>
  {
    /// <summary> Create a Tween Vector4 and add it to the TinyTween manager. </summary>
    /// <returns>Tween.</returns>
    public static Tween<Vector4> Create() => TinyTween.Instance.Add(new TweenVector4()) as Tween<Vector4>;
    
    private static Vector4 Lerp(ITween<Vector4> t, Vector4 start, Vector4 end, float progress, bool clamp) =>
      clamp == true ? Vector4.Lerp(start, end, progress) : Vector4.LerpUnclamped(start, end, progress);

    private TweenVector4() : base(Lerp) { }
  }

  /// <summary> Tween Quaternion. </summary>
  public sealed class TweenQuaternion : Tween<Quaternion>
  {
    /// <summary> Create a Tween Quaternion and add it to the TinyTween manager. </summary>
    /// <returns>Tween.</returns>
    public static Tween<Quaternion> Create() => TinyTween.Instance.Add(new TweenQuaternion()) as Tween<Quaternion>;

    private static Quaternion Lerp(ITween<Quaternion> t, Quaternion start, Quaternion end, float progress, bool clamp) =>
      clamp == true ? Quaternion.Lerp(start, end, progress) : Quaternion.LerpUnclamped(start, end, progress);   

    private TweenQuaternion() : base(Lerp) { }
  }

  /// <summary> Tween Color. </summary>
  public sealed class TweenColor : Tween<Color>
  {
    /// <summary> Create a Tween Color and add it to the TinyTween manager. </summary>
    /// <returns>Tween.</returns>
    public static Tween<Color> Create() => TinyTween.Instance.Add(new TweenColor()) as Tween<Color>;

    private static Color Lerp(ITween<Color> t, Color start, Color end, float progress, bool clamp) =>
      clamp == true ? Color.Lerp(start, end, progress) : Color.LerpUnclamped(start, end, progress);

    private TweenColor() : base(Lerp) { }
  }

  /// <summary> Extensions to make it easy to use TinyTween. </summary>
  public static class TweenExtensions
  {
    /// <summary> Create and execute a tween. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start value.</param>
    /// <param name="destination">End value.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<float> Tween(this float self, float origin, float destination, float duration, Ease ease) =>
      TweenFloat.Create()
        .Origin(origin)
        .Destination(destination)
        .Duration(duration)
        .OnUpdate(tween => self = tween.Value)
        .Owner(self)
        .Easing(ease)
        .Start();

    /// <summary> Create and execute a tween, using as initial value the current value. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End value.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<float> Tween(this float self, float destination, float duration, Ease ease) => Tween(self, self, destination, duration, ease);

    /// <summary> Create and execute a tween. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start value.</param>
    /// <param name="destination">End value.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> Tween(this Vector3 self, Vector3 origin, Vector3 destination, float duration, Ease ease) =>
      TweenVector3.Create()
        .Origin(origin)
        .Destination(destination)
        .Duration(duration)
        .OnUpdate(tween => self = tween.Value)
        .Owner(self)
        .Easing(ease)
        .Start();
    
    /// <summary> Create and execute a tween, using as initial value the current value. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End value.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> Tween(this Vector3 self, Vector3 destination, float duration, Ease ease) => Tween(self, self, destination, duration, ease);

    /// <summary> Create and execute a tween. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start value.</param>
    /// <param name="destination">End value.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Quaternion> Tween(this Quaternion self, Quaternion origin, Quaternion destination, float duration, Ease ease) =>
      TweenQuaternion.Create()
        .Origin(origin)
        .Destination(destination)
        .Duration(duration)
        .OnUpdate(tween => self = tween.Value)
        .Owner(self)
        .Easing(ease)
        .Start();
    
    /// <summary> Create and execute a tween, using as initial value the current value. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End value.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Quaternion> Tween(this Quaternion self, Quaternion destination, float duration, Ease ease) => Tween(self, self, destination, duration, ease);
    
    /// <summary> Create and execute a tween. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start value.</param>
    /// <param name="destination">End value.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Color> Tween(this Color self, Color origin, Color destination, float duration, Ease ease) =>
      FronkonGames.TinyTween.TweenColor.Create()
        .Origin(origin)
        .Destination(destination)
        .Duration(duration)
        .OnUpdate(tween => self = tween.Value)
        .Owner(self)
        .Easing(ease)
        .Start();
    
    /// <summary> Create and execute a tween, using as initial value the current value. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End value.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Color> Tween(this Color self, Color destination, float duration, Ease ease) => Tween(self, self, destination, duration, ease);

    /// <summary> Moves a Transform. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start position.</param>
    /// <param name="destination">End position.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> TweenMove(this Transform self, Vector3 origin, Vector3 destination, float duration, Ease ease) =>
      TweenVector3.Create()
        .Origin(origin)
        .Destination(destination)
        .Duration(duration)
        .OnUpdate(tween => self.position = tween.Value)
        .Owner(self)
        .Easing(ease)
        .Start();

    /// <summary> Moves a Transform, using its current position as origin. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End position.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> TweenMove(this Transform self, Vector3 destination, float duration, Ease ease) => TweenMove(self, self.position, destination, duration, ease);

    /// <summary> Scale a Transform. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start scale.</param>
    /// <param name="destination">End scale.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> TweenScale(this Transform self, Vector3 origin, Vector3 destination, float duration, Ease ease) =>
      TweenVector3.Create()
        .Origin(origin)
        .Destination(destination)
        .Duration(duration)
        .OnUpdate(tween => self.localScale = tween.Value)
        .Owner(self)
        .Easing(ease)
        .Start();

    /// <summary> Scale a Transform, using its current scale as origin. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End scale.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> TweenScale(this Transform self, Vector3 destination, float duration, Ease ease) => TweenScale(self, self.localScale, destination, duration, ease);

    /// <summary> Rotate a Transform. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start rotation.</param>
    /// <param name="destination">End rotation.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Quaternion> TweenRotation(this Transform self, Quaternion origin, Quaternion destination, float duration, Ease ease) =>
      TweenQuaternion.Create()
        .Origin(origin)
        .Destination(destination)
        .Duration(duration)
        .OnUpdate(tween => self.rotation = tween.Value)
        .Owner(self)
        .Easing(ease)
        .Start();

    /// <summary> Rotate a Transform, using its current rotation as origin. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End rotation.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Quaternion> TweenRotation(this Transform self, Quaternion destination, float duration, Ease ease) => TweenRotation(self, self.rotation, destination, duration, ease);
    
    /// <summary> Moves a GameObject. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start position.</param>
    /// <param name="destination">End position.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> TweenMove(this GameObject self, Vector3 origin, Vector3 destination, float duration, Ease ease) => TweenMove(self.transform, origin, destination, duration, ease);
    
    /// <summary> Moves a GameObject, using its current position as origin. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End position.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> TweenMove(this GameObject self, Vector3 destination, float duration, Ease ease) => TweenMove(self.transform, self.transform.position, destination, duration, ease);
    
    /// <summary> Scale a GameObject. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start scale.</param>
    /// <param name="destination">End scale.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> TweenScale(this GameObject self, Vector3 origin, Vector3 destination, float duration, Ease ease) => TweenScale(self.transform, origin, destination, duration, ease);

    /// <summary> Scale a GameObject, using its current scale as origin. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End scale.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Vector3> TweenScale(this GameObject self, Vector3 destination, float duration, Ease ease) => TweenScale(self.transform, self.transform.localScale, destination, duration, ease);
    
    /// <summary> Rotate a GameObject. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start rotation.</param>
    /// <param name="destination">End rotation.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Quaternion> TweenRotation(this GameObject self, Quaternion origin, Quaternion destination, float duration, Ease ease) => TweenRotation(self.transform, origin, destination, duration, ease);

    /// <summary> Rotate a GameObject, using its current rotation as origin. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End rotation.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <returns>Tween.</returns>
    public static Tween<Quaternion> TweenRotation(this GameObject self, Quaternion destination, float duration, Ease ease) => TweenRotation(self.transform, self.transform.rotation, destination, duration, ease);
    
    /// <summary> Change the color of a Material. </summary>
    /// <param name="self">Self.</param>
    /// <param name="origin">Start color.</param>
    /// <param name="destination">End color.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <param name="name">The name of the color variable.</param>
    /// <returns>Tween.</returns>
    public static Tween<Color> TweenColor(this Material self, Color origin, Color destination, float duration, Ease ease, string name = "_Color") =>
      FronkonGames.TinyTween.TweenColor.Create()
        .Origin(origin)
        .Destination(destination)
        .Duration(duration)
        .OnUpdate(tween => self.SetColor(name, tween.Value))
        .Owner(self)
        .Easing(ease)
        .Start();
    
    /// <summary> Change the color of a Material, using its current color as origin. </summary>
    /// <param name="self">Self.</param>
    /// <param name="destination">End color.</param>
    /// <param name="duration">Time in seconds.</param>
    /// <param name="ease">Easing.</param>
    /// <param name="name">The name of the color variable.</param>
    /// <returns>Tween.</returns>
    public static Tween<Color> TweenColor(this Material self, Color destination, float duration, Ease ease, string name = "_Color") => TweenColor(self, self.GetColor(name), destination, duration, ease, name);
  }

  /// <summary> Types of Easing functions. See https://easings.net </summary>
  public enum Ease
  {
    None,
    Linear,
    Sine,
    Quad,
    Cubic,
    Quart,
    Quint,
    Expo,
    Circ,
    Back,
    Elastic,
    Bounce,
  }

  /// <summary> Easing functions. </summary>
  internal static class EasingFunctions
  {
    public static float Calculate(Ease ease, bool easingIn, bool easingOut, float t) => ease switch
    {
      Ease.Linear     => t,
      Ease.Sine       => easingIn && easingOut ? SineInOut(t)    : easingIn ? SineIn(t)    : SineOut(t),
      Ease.Quad       => easingIn && easingOut ? QuadInOut(t)    : easingIn ? QuadIn(t)    : QuadOut(t),
      Ease.Cubic      => easingIn && easingOut ? CubicInOut(t)   : easingIn ? CubicIn(t)   : CubicOut(t),
      Ease.Quart      => easingIn && easingOut ? QuartInOut(t)   : easingIn ? QuartIn(t)   : QuartOut(t),
      Ease.Quint      => easingIn && easingOut ? QuintInOut(t)   : easingIn ? QuintIn(t)   : QuintOut(t),
      Ease.Expo       => easingIn && easingOut ? ExpoInOut(t)    : easingIn ? ExpoIn(t)    : ExpoOut(t),
      Ease.Circ       => easingIn && easingOut ? CircInOut(t)    : easingIn ? CircIn(t)    : CircOut(t),
      Ease.Back       => easingIn && easingOut ? BackInOut(t)    : easingIn ? BackIn(t)    : BackOut(t),
      Ease.Elastic    => easingIn && easingOut ? ElasticInOut(t) : easingIn ? ElasticIn(t) : ElasticOut(t),
      Ease.Bounce     => easingIn && easingOut ? BounceInOut(t)  : easingIn ? BounceIn(t)  : BounceOut(t),
      _ => t
    };

    private static float SineIn(float t)    => 1.0f - Mathf.Cos(t * Mathf.PI / 2.0f);
    private static float SineOut(float t)   => Mathf.Sin(t * Mathf.PI / 2.0f);
    private static float SineInOut(float t) => 0.5f * (1.0f - Mathf.Cos(Mathf.PI * t));
    
    private static float QuadIn(float t)    => t * t;
    private static float QuadOut(float t)   => 1.0f - (1.0f - t) * (1.0f - t);
    private static float QuadInOut(float t) => t < 0.5f ? 2.0f * t * t : 1.0f - Mathf.Pow(-2.0f * t + 2.0f, 2.0f) / 2.0f;

    private static float CubicIn(float t) => t * t * t;
    private static float CubicOut(float t) => --t * t * t + 1.0f;
    private static float CubicInOut(float t) => t < 0.5f ? 4.0f * t * t * t : 1.0f - Mathf.Pow(-2.0f * t + 2.0f, 3.0f) / 2.0f;
    
    private static float QuartIn(float t) => t * t * t * t;
    private static float QuartOut(float t) => 1.0f - (--t * t * t * t);
    private static float QuartInOut(float t) => (t *= 2.0f) < 1.0f ? 0.5f * t * t * t * t : -0.5f * ((t -= 2.0f) * t * t * t - 2.0f);

    private static float QuintIn(float t) => t * t * t * t * t;
    private static float QuintOut(float t) => --t * t * t * t * t + 1.0f;
    private static float QuintInOut(float t) => t < 0.5f ? 16.0f * t * t * t * t * t : 1.0f - Mathf.Pow(-2.0f * t + 2.0f, 5.0f) / 2.0f;

    private static float ExpoIn(float t) => t == 0.0f ? 0.0f : Mathf.Pow(2.0f, 10.0f * t - 10.0f);
    private static float ExpoOut(float t) => t == 1.0f ? 1.0f : 1.0f - Mathf.Pow(2.0f, -10.0f * t);
    private static float ExpoInOut(float t) => t == 0.0f ? 0.0f
                                                         : t == 1.0f ? 1.0f
                                                                     : t < 0.5f ? Mathf.Pow(2.0f, 20.0f * t - 10.0f) / 2.0f
                                                                                : (2.0f - Mathf.Pow(2.0f, -20.0f * t + 10.0f)) / 2.0f;

    private static float CircIn(float t) => 1.0f - Mathf.Sqrt(1.0f - Mathf.Pow(t, 2.0f));
    private static float CircOut(float t) => Mathf.Sqrt(1.0f - Mathf.Pow(t - 1.0f, 2.0f));
    private static float CircInOut(float t) => t < 0.5f ? (1.0f - Mathf.Sqrt(1.0f - Mathf.Pow(2.0f * t, 2.0f))) / 2.0f
                                                        : (Mathf.Sqrt(1.0f - Mathf.Pow(-2.0f * t + 2.0f, 2.0f)) + 1.0f) / 2.0f;

    private static float BackIn(float t) => C3 * t * t * t - C1 * t * t;
    private static float BackOut(float t) => 1.0f + C3 * Mathf.Pow(t - 1.0f, 3.0f) + C1 * Mathf.Pow(t - 1.0f, 2.0f);
    private static float BackInOut(float t) => t < 0.5f ? Mathf.Pow(2.0f * t, 2.0f) * ((C2 + 1.0f) * 2.0f * t - C2) / 2.0f
                                                        : (Mathf.Pow(2.0f * t - 2.0f, 2.0f) * ((C2 + 1.0f) * (t * 2.0f - 2.0f) + C2) + 2.0f) / 2.0f;

    private static float ElasticIn(float t) => -Mathf.Pow(2.0f, 10.0f * t - 10.0f) * Mathf.Sin((t * 10.0f - 10.75f) * C4);
    private static float ElasticOut(float t) => Mathf.Pow(2.0f, -10.0f * t) * Mathf.Sin((t * 10.0f - 0.75f) * C4) + 1.0f;      
    private static float ElasticInOut(float t) => t < 0.5f ? -(Mathf.Pow(2.0f, 20.0f * t - 10.0f) * Mathf.Sin((20.0f * t - 11.125f) * C5)) / 2f
                                                           : Mathf.Pow(2.0f, -20.0f * t + 10.0f) * Mathf.Sin((20.0f * t - 11.125f) * C5) / 2.0f + 1.0f;

    private static float BounceIn(float t) => 1.0f - BounceOut(1.0f - t);
    private static float BounceOut(float t)
    {
      if (t < 1.0f / 2.75f)
        return 7.5625f * t * t;
      
      if (t < 2.0f / 2.75f)
        return 7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f;
      
      if (t < 2.5f / 2.75f)
        return 7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f;
      
      return 7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f;
    }
    private static float BounceInOut(float t) => t < 0.5f ? BounceIn(t * 2.0f) * 0.5f : BounceOut(t * 2.0f - 1.0f) * 0.5f + 0.5f;

    private const float C1 = 1.70158f;
    private const float C2 = C1 * 1.525f;
    private const float C3 = C1 + 1.0f;
    private const float C4 = 2.0f * Mathf.PI / 3.0f;
    private const float C5 = 2.0f * Mathf.PI / 4.5f;
  }
}