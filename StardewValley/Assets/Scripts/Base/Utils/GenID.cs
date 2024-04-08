using System;
using System.Collections.Generic;

namespace WATP
{
    /// <summary>
    /// unique id를 생성하는 클래스입니다.
    /// </summary>
    public static class GenID
    {
        private static readonly Dictionary<Type, int> idMap = new();

        /// <summary>
        /// 해당 타입중 겹치지 않게 Unique한 ID를 생성합니다.
        /// </summary>
        /// <typeparam name="T">타입</typeparam>
        /// <returns>생성된 Unique ID</returns>
        public static int Get<T>()
        {
            var type = typeof(T);
            if (!idMap.TryGetValue(type, out var id))
                idMap.Add(type, 0);

            idMap[type] = id + 1;
            return idMap[type];
        }
    }
}