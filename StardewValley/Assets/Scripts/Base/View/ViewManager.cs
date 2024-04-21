using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using WATP.ECS;
using Unity.Entities;
using System;
using System.Threading;

namespace WATP.View
{
    /// <summary>
    /// entity의 해당되는 것중 view가 필요한 부분을 생성, 관리해주는 클래스
    /// event로 통신 하여 관리한다.
    /// </summary>
    public class ViewManager
    {
        private Transform root;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        protected Dictionary<int, IView> prsDic = new();

        #region Property
        public Transform Root { get => root; }

        #endregion

        public void Initialize()
        {
            root = new GameObject("View Root").transform;

            Bind();
        }

        public void Destroy()
        {
            cancellationToken.Cancel();
            cancellationToken.Dispose();

            Clear();
            UnBind();
        }

        public void Render()
        {
            foreach (var prs in prsDic.Values)
            {
                prs.Render();
            }
        }

        public T GetEntity<T>(int id) where T : class, IView
        {
            IView temp;
            prsDic.TryGetValue(id, out temp);
            return temp as T;
        }

        private void Bind()
        {
            ECSController.ServiceEvents.On<EventCreateEntity>(OnEventCreateEntity);
            ECSController.ServiceEvents.On<EventDeleteEntity>(OnEventDeleteEntity);
            ECSController.ServiceEvents.On<EventRefUpdate>(OnEventRefUpdate);
            WATP.Root.GameDataManager.Preferences.OnIsGridChange += OnViewGrid;
        }

        private void UnBind()
        {
            ECSController.ServiceEvents.Off<EventCreateEntity>(OnEventCreateEntity);
            ECSController.ServiceEvents.Off<EventDeleteEntity>(OnEventDeleteEntity);
            ECSController.ServiceEvents.Off<EventRefUpdate>(OnEventRefUpdate);
            WATP.Root.GameDataManager.Preferences.OnIsGridChange -= OnViewGrid;
        }

        /// <summary>
        /// ecs aspect를 view에 연결, 제작해주는 함수
        /// </summary>
        private async void CreateObj(IWATPObjectAspect entityData)
        {
            try
            {
                IView prs = InitObj(entityData);

                if (prs == null)
                    throw new Exception("Not view manager init class");
                
                prsDic.Add(entityData.Index, prs);
                ViewController.ServiceEvents.Emit(new ViewCreateEvent(prs));

                var prefab = prs as IPrefabHandler;
                await prefab.LoadAsync(prefab.PrefabPath, prefab.Parent, cancellationToken);
            }
            catch (Exception e)
            {
                throw new OperationCanceledException("Prefab(view) Load Error\n" + e);
            }
        }

        /// <summary>
        /// aspect에 맞는 view 생성 및 component 연결
        /// </summary>
        private IView InitObj(IWATPObjectAspect entityData)
        {
            EventActionComponent eventActionComponent = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentObject<EventActionComponent>(entityData.Entity);
            switch (entityData)
            {
                case FarmerAspect:
                    return new FarmerView((FarmerAspect)entityData, eventActionComponent, root);
                case NpcAspect:
                    return new NpcView((NpcAspect)entityData, eventActionComponent, root);
                case AnimalAspect:
                    return new AnimalView((AnimalAspect)entityData, eventActionComponent, root);
                case HoedirtAspect:
                    return new HoedirtView((HoedirtAspect)entityData, eventActionComponent, root);
                case CropsAspect:
                    return new CropsView((CropsAspect)entityData, eventActionComponent, root);
                case MapObjectAspect:
                    return new MapObjectView((MapObjectAspect)entityData, eventActionComponent, root);
            }

            return null;
        }

        private void RemoveObj(IWATPObjectAspect entityData)
        {
            if (prsDic.ContainsKey(entityData.Index))
            {
                ViewController.ServiceEvents.Emit(new ViewDeleteEvent(prsDic[entityData.Index]));

                if (prsDic[entityData.Index].IsAlreadyDisposed == false)
                    prsDic[entityData.Index].Dispose();

                prsDic.Remove(entityData.Index);
            }
        }

        private void Clear()
        {
            var prsList = prsDic.Values.ToList();
            foreach (var prs in prsList)
                prs.Dispose();

            prsDic.Clear();
        }


        #region time

        public void SetMultiply(float multiply)
        {
            foreach (var prs in prsDic.Values)
                prs.SetMultiply(multiply);
        }

        public void SetContinue(float multiply)
        {
            foreach (var prs in prsDic.Values)
                prs.SetContinue(multiply);
        }

        public void SetPause()
        {
            foreach (var prs in prsDic.Values)
                prs.SetPause();
        }
        #endregion


        #region event

        void OnEventCreateEntity(EventCreateEntity e)
        {
            CreateObj(e.Entity);
        }

        void OnEventDeleteEntity(EventDeleteEntity e)
        {
            RemoveObj(e.Entity);
        }

        void OnEventRefUpdate(EventRefUpdate e)
        {
            foreach (var prs in prsDic.Values)
            {
                if (prs.UID == e.Entity.Index)
                {
                    prs.ReRef(e.Entity);
                    break;
                }
            }
        }

        private void OnViewGrid(bool grid)
        {
            foreach (var prs in prsDic.Values)
            {
                if (prs is IGridView)
                    (prs as IGridView).SetGridView(grid);
            }
        }

        #endregion
    }
}
