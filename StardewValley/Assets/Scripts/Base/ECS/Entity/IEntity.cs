using System;
using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// <see cref="MEntity"/>의 인터페이스 입니다.
    /// </summary>
    public interface IEntity : IModel, ITransformComponent, IEventComponent
    {
        bool DeleteReservation { get; set; }

        /// <summary>
        /// 자신을 생성합니다.
        /// </summary>
        /// <returns></returns>
        T Builder<T>() where T : Entity;

        /// <summary>
        /// 자신을 복사합니다.
        /// </summary>
        /// <returns></returns>
        IEntity Clone();
    }
}