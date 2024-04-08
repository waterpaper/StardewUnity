using System.Collections.Generic;
using UnityEngine;

namespace WATP.ECS
{
    public interface ICellTargetComponent : IComponent, IMoveComponent
    {
        public CellTargetComponent CellTargetComponent { get; }
    }

    [System.Serializable]
    public class CellTargetComponent
    {
        /// <summary> 움직임 가능여부 </summary>
        [SerializeField] public bool isEnable = true;
        /// <summary> 목표 /// </summary>
        [SerializeField] public bool finish = true;

        /// <summary> 목표 /// </summary>
        [SerializeField] public float refreshTime;
        /// <summary> 목표 /// </summary>
        [SerializeField] public float refreshTimer;

        /// <summary> 이전 포지션 /// </summary>
        [SerializeField] public List<Vector2> cellPath = new();
    }
}