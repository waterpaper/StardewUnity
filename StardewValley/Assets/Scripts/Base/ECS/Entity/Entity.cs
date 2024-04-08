using System;
using UnityEngine;

namespace WATP.ECS
{
    /// <summary>
    /// 포지션과 정보를 갖고 시뮬레이션 기초가 되는 클래스입니다.
    /// </summary>
    [System.Serializable]
    public abstract class Entity : Model, IEntity
    {
        /// <summary> 자신의 위치 </summary>
        protected TransformComponent transformComponent = new();
        public TransformComponent TransformComponent { get => transformComponent; }


        protected EventComponent eventComponent = new();
        public EventComponent EventComponent { get => eventComponent;  }

        public bool DeleteReservation {
            get; set;
        }


        public T Builder<T>() where T : Entity
        {
            this.uid = GenID.Get<Entity>();

            return this as T;
        }

        public override void OnInitialize()
        {
            eventComponent.onEvent?.Invoke("Initialize");
        }

        public override void OnDestroy()
        {
            eventComponent.onEvent?.Invoke("Destroy");
            eventComponent.onEvent = null;
        }

        public override string ToString()
        {
            return $"{base.ToString()}::Pos({transformComponent.position})";
        }

        public abstract IEntity Clone();
    }
}