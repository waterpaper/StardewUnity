using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace WATP.ECS
{
    public class ECSManager
    {
        private bool isChanged;
        private long frameCount;

        protected EntityContainer entitys;
        protected List<IService> processes;

        //reserv
        protected List<IEntityBuilder> addBuilderList;
        protected List<IEntity> addList;
        protected List<IEntity> deleteList;

        public bool IsChanged { get => isChanged; }
        public long FrameCount { get => frameCount; }
        public List<IEntity> Entitys { get => entitys.Values.ToList(); }


#if UNITY_EDITOR
        private long totalTime = 0;
        private System.Diagnostics.Stopwatch totalStopwatch = new System.Diagnostics.Stopwatch();
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
#endif

        public ECSManager()
        {
            isChanged = false;
            entitys = new();
            processes = new();

            addBuilderList = new();
            addList = new();
            deleteList = new();
        }

        public void Initialize()
        {
            Clear();


            processes.Add(new CellTargetService());
            processes.Add(new MoveService());

            processes.Add(new InputMoveService());
            processes.Add(new InputInteractionService());
            processes.Add(new InputUsingService());
            processes.Add(new PhysicsCollisionService());

            processes.Add(new StateService());

            processes.Add(new PhysicsService());
            processes.Add(new CellAddObjectService());
            processes.Add(new WarpService());
            processes.Add(new SleepService());
            processes.Add(new DelayDeleteService());

            //�߰��� ������� ������Ʈ -> ���� ����� ������ ����� �� ����
            /*processes.Add(new BuffProcess());
            processes.Add(new MapDebuffProcess());

            processes.Add(new StateProcess());

            // Ÿ���� ����
            processes.Add(new DetectionProcess());

            //������
            processes.Add(new MoveProcess());
            // �浹���� ���μ���
            processes.Add(new CollisionProcess());

            //����
            processes.Add(new AttackProcess()); // ��ų �ߵ� ���μ���

            processes.Add(new ActionProcess()); // �߰� �ൿ ���μ���(����, �߻�ü ��)

            //��ų ����
            processes.Add(new SkillActionProcess()); // skill action ������Ʈ ���μ���*/
            Bind();
        }

        public void Destroy()
        {
            Clear();
            UnBind();
        }

        private void Bind()
        {
            ECSController.ServiceEvents.On<EventAddEntity>(OnEventAddEntity);
            ECSController.ServiceEvents.On<EventCreateRoutine>(OnEventCreateRoutine);
            ECSController.ServiceEvents.On<EventDeleteRoutine>(OnEventDeleteRoutine);
        }

        private void UnBind()
        {
            ECSController.ServiceEvents.Off<EventAddEntity>(OnEventAddEntity);
            ECSController.ServiceEvents.Off<EventCreateRoutine>(OnEventCreateRoutine);
            ECSController.ServiceEvents.Off<EventDeleteRoutine>(OnEventDeleteRoutine);
        }

        public void Simulation(double frameTime)
        {
            frameCount++;

#if UNITY_EDITOR
            totalStopwatch.Start();
#endif

            //add ó��
            CreateEntitys();

            foreach (var process in processes)
            {
#if UNITY_EDITOR
                Profiler.BeginSample($"{process.GetType()} Process");
                stopwatch.Reset();
                stopwatch.Start();
                process.Update(frameTime);
                stopwatch.Stop();
                Profiler.EndSample();

                totalTime = totalStopwatch.ElapsedMilliseconds;
#else
                Profiler.BeginSample($"{process.GetType()} Process");
                process.Update(frameTime);
                Profiler.EndSample();
#endif
            };

            //dead ó��
            DestroyEntitys();

#if UNITY_EDITOR
            totalStopwatch.Stop();
#endif
        }

        public T AddSimulationEntity<T>() where T : Entity, new()
        {
            var result = new T().Builder<T>();
            addList.Add((T)result);
            isChanged = true;
            return (T)result;
        }

        public T AddSimulationEntityBuilder<T>(T entity) where T : IEntityBuilder
        {
            addBuilderList.Add(entity);
            return entity;
        }

        public void RemoveSimulationEntity(int entityID)
        {
            IEntity entity;
            if (entitys.TryGetValue(entityID, out entity) == false)
                return;

            entity.OnDestroy();
            entitys.Remove(entityID);
            isChanged = true;
        }

        public T GetEntity<T>(int id) where T : Entity
        {
            IEntity temp;
            bool hasUnit = entitys.TryGetValue(id, out temp);
            return hasUnit ? temp as T : null;
        }

        public void Clear()
        {
            frameCount = 0;
            isChanged = false;

            entitys.Clear();
            addBuilderList.Clear();
            addList.Clear();
            deleteList.Clear();
            isChanged = true;

            processes.ForEach(process =>
            {
                process.Clear();
            });
            processes.Clear();

#if UNITY_EDITOR
            stopwatch.Stop();
            totalStopwatch.Stop();
#endif
        }

        public void CreateEntitys()
        {
            for (int i = 0; i < addBuilderList.Count; i++)
            {
                addList.Add(addBuilderList[i].Build());
                isChanged = true;
            }

            addBuilderList.Clear();
            if (addList.Count <= 0) return;

            for (int i = 0; i < addList.Count; i++)
            {
                entitys.Add(addList[i].UID, addList[i]);
                addList[i].OnInitialize();
                foreach (var process in processes)
                    process.Add(addList[i]);

                ECSController.ServiceEvents.Emit(new EventCreateEntity(addList[i]));
            }

            addList.Clear();
            isChanged = true;
        }

        public void DestroyEntitys()
        {
            foreach (var entity in entitys.Values)
            {
                if (entity.DeleteReservation)
                {
                    deleteList.Add(entity);
                }
            }

            foreach (var entity in deleteList)
            {
                foreach (var process in processes)
                    process.Remove(entity);

                ECSController.ServiceEvents.Emit(new EventDeleteEntity(entity));
                entity.OnDestroy();
                entitys.Remove(entity.UID);
            }

            deleteList.Clear();
        }

        void OnEventAddEntity(EventAddEntity e)
        {
            AddSimulationEntityBuilder(e.EntityBuilder);
        }

        void OnEventCreateRoutine(EventCreateRoutine e)
        {
            CreateEntitys();
        }

        void OnEventDeleteRoutine(EventDeleteRoutine e)
        {
            if(e.isAll)
            {
                foreach (var entity in entitys.Values)
                    entity.DeleteReservation = true;
            }
            else if(e.isRemove)
            {
                foreach (var entity in entitys.Values)
                {
                    if (entity.TransformComponent.position.x == e.posX && entity.TransformComponent.position.y == e.posY)
                    {
                        entity.DeleteReservation = true;
                        break;
                    }
                }
            }

            DestroyEntitys();
        }
    }
}


