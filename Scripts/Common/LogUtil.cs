using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class LogUtil
    {
        public static bool IsDebugBuild
        {
            get { return Debug.isDebugBuild; }
        }

        [System.Diagnostics.Conditional("OLYMPUS_DEBUG")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        [System.Diagnostics.Conditional("OLYMPUS_DEBUG")]
        public static void Log(object message, UnityEngine.Object context)
        {
            Debug.Log(message, context);
        }

        [System.Diagnostics.Conditional("OLYMPUS_DEBUG")]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        [System.Diagnostics.Conditional("OLYMPUS_DEBUG")]
        public static void LogError(object message, UnityEngine.Object context)
        {
            Debug.LogError(message, context);
        }

        [System.Diagnostics.Conditional("OLYMPUS_DEBUG")]
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        [System.Diagnostics.Conditional("OLYMPUS_DEBUG")]
        public static void LogWarning(object message, UnityEngine.Object context)
        {
            Debug.LogWarning(message, context);
        }

        [System.Diagnostics.Conditional("OLYMPUS_DEBUG")]
        public static void Assert(bool condition)
        {
            if (!condition) throw new Exception();
        }

        [System.Diagnostics.Conditional("OLYMPUS_DEBUG")]
        public static void Assert(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
        }
    }
}