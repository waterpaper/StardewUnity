using System;
using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// <see cref="MEntity"/>�� �������̽� �Դϴ�.
    /// </summary>
    public interface IEntity : IModel, ITransformComponent, IEventComponent
    {
        bool DeleteReservation { get; set; }

        /// <summary>
        /// �ڽ��� �����մϴ�.
        /// </summary>
        /// <returns></returns>
        T Builder<T>() where T : Entity;

        /// <summary>
        /// �ڽ��� �����մϴ�.
        /// </summary>
        /// <returns></returns>
        IEntity Clone();
    }
}