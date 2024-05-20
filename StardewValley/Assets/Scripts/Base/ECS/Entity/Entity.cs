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

        /// <summary>
        /// 생성시 빌더 패턴을 통해 정해진 루틴대로 생성한다.
        /// </summary>
        public T Builder<T>() where T : Entity
        {
            this.uid = GenID.Get<Entity>();

            return this as T;
        }

        /// <summary>
        /// 초기화 로직
        /// </summary>
        public override void OnInitialize()
        {
            eventComponent.onEvent?.Invoke("Initialize");
        }

        /// <summary>
        /// 삭제 처리시 로직
        /// </summary>
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