using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    public static class CoroutineRunner
    {
        private static CoroutineBehaviour Runner;

        static CoroutineRunner() { Runner = new GameObject().AddComponent<CoroutineBehaviour>(); }

        public static Coroutine StartCoroutine(IEnumerator Routine) { return Runner.StartCoroutine(Routine); }

        public static void StopCoroutine(IEnumerator Routine) { Runner.StopCoroutine(Routine); }

        public static void StopCoroutine(Coroutine Coroutine) { Runner.StopCoroutine(Coroutine); }

        public class CoroutineBehaviour : MonoBehaviour { }
    }
}
