using System;
using System.Collections.Generic;

namespace WATP.ECS
{
    /// <summary>
    /// <see cref="IEntity"/>들을 관리하는 컨테이너 입니다.
    /// </summary>
    public class EntityContainer : SortedList<int, IEntity>
    {
        private const int CAPACITY = 128;

        public EntityContainer() : base(CAPACITY)
        {
        }
    }
}