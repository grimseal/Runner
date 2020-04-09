using System;
using System.Collections;
using UnityEngine;

namespace Game.Helper
{
    public static class CoroutineHelper
    {
        public static Coroutine WaitForSeconds(Action callback, float duration, MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.StartCoroutine(Wait(callback, duration));
        }
    
        public static Coroutine Run(Action<float> action, float duration, 
            MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.StartCoroutine(BaseRoutine(action, Linear, duration));
        }
    
        public static Coroutine Run(Action<float> action, Action callback, float duration, 
            MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.StartCoroutine(BaseRoutine(action, Linear, callback, duration));
        }

        public static Coroutine Run(Action<float> action, Func<float, float> easing, float duration, 
            MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.StartCoroutine(BaseRoutine(action, easing, duration));
        }

        public static Coroutine Run(Action<float> action, Action callback, Func<float, float> easing, float duration, 
            MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.StartCoroutine(BaseRoutine(action, easing, callback, duration));
        }

        public static float Linear(float t)
        {
            return t;
        }

        public static float EaseInQuad(float t)
        {
            return t * t;
        }
    
        public static float EaseOutQuad(float t)
        {
            return -(t * (t - 2));
        }

        public static float EaseInOutQuad(float t)
        {
            if (t < 0.5f) return 2 * t * t;
            return -2 * t * t + 4 * t - 1;
        }

        private static IEnumerator Wait(Action callback, float t)
        {
            yield return new WaitForSeconds(t);
            callback();
        }
        
        private static IEnumerator BaseRoutine(Action<float> action, Func<float, float> easing, float duration)
        {
            var startTime = Time.time;
            var endTime = startTime + duration;
            var timeStep = 1f / duration;
            while (Time.time < endTime)
            {
                action(easing((Time.time - startTime) * timeStep));
                yield return null;
            }
            action(1f);
        }
    
        private static IEnumerator BaseRoutine(Action<float> action, Func<float, float> easing, Action callback, float duration)
        {
            var startTime = Time.time;
            var endTime = startTime + duration;
            var timeStep = 1f / duration;
            while (Time.time < endTime)
            {
                action(easing((Time.time - startTime) * timeStep));
                yield return null;
            }
            action(1f);
            callback();
        }
    }
}