using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace WATP
{
    using Debug = UnityEngine.Debug;

    public static class Debugger
    {
        /// 로그는 영어로 작성(인코딩환경이 다르면 깨지는 문제)
        private static readonly StringBuilder _builder = new(1024);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ping()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("[Debugger] UNITY_EDITOR is enabled.");
#else
            Debug.Log("[Debugger] UNITY_EDITOR is disabled.");
#endif
        }

#if !UNITY_EDITOR
        [Conditional("DEVELOPMENT_BUILD")]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string message)
        {
            Debug.Log(message);
        }

#if !UNITY_EDITOR
        [Conditional("DEVELOPMENT_BUILD")]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log<T>(T? obj)
            where T : struct
        {
            if (!obj.HasValue)
            {
                Debug.unityLogger.LogFormat(LogType.Log, "null");
                return;
            }

            Log(obj.Value);
        }
        
#if !UNITY_EDITOR
        [Conditional("DEVELOPMENT_BUILD")]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log<T>(T obj)
        {
            var str = obj == null ? "Null" : obj.ToString();
            Debug.unityLogger.LogFormat(LogType.Log, str);
        }

#if !UNITY_EDITOR
        [Conditional("DEVELOPMENT_BUILD")]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log<T>(IEnumerable<T> list)
        {
            _builder.Clear();
            _builder.AppendLine($"IEnumerable<{nameof(T)}> ↩");
            var index = 0;
            foreach (var i in list)
            {
                _builder.Append('[');
                _builder.Append(index.ToString());
                _builder.Append("]: ");
                _builder.AppendLine(i?.ToString() ?? "Null");
                
                index++;
            }

            Debug.unityLogger.LogFormat(LogType.Log, _builder.ToString());
        }

#if !UNITY_EDITOR
        [Conditional("DEVELOPMENT_BUILD")]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string message)
        {
            Debug.unityLogger.LogFormat(LogType.Warning, message);
        }

#if !UNITY_EDITOR
        [Conditional("DEVELOPMENT_BUILD")]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static void LogError(string message)
        {
            Debug.unityLogger.LogFormat(LogType.Error, message);
        }
    }
}