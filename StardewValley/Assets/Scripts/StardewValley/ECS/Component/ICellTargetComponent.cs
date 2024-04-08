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
        /// <summary> ������ ���ɿ��� </summary>
        [SerializeField] public bool isEnable = true;
        /// <summary> ��ǥ /// </summary>
        [SerializeField] public bool finish = true;

        /// <summary> ��ǥ /// </summary>
        [SerializeField] public float refreshTime;
        /// <summary> ��ǥ /// </summary>
        [SerializeField] public float refreshTimer;

        /// <summary> ���� ������ /// </summary>
        [SerializeField] public List<Vector2> cellPath = new();
    }
}