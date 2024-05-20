using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using WATP.ECS;

namespace WATP.View
{
    /// <summary>
    /// entity의 해당되는 것중 view가 필요한 부분을 생성, 관리해주는 클래스
    /// event로 통신 하여 관리한다.
    /// </summary>
    public class ViewManager
    {
        #region Property
        private Transform root;

        protected Dictionary<int, IView> prsDic = new();

        public Transform Root { get => root; }

        #endregion

        public void Initialize()
        {
            root = new GameObject("View Root").transform;

            Bind();
        }

        public void Destroy()
        {
            Clear();

            UnBind();
        }

        private void Bind()
        {
            ECSController.ServiceEvents.On<EventCreateEntity>(OnEventCreateEntity);
            ECSController.ServiceEvents.On<EventDeleteEntity>(OnEventDeleteEntity);
            WATP.Root.GameDataManager.Preferences.OnIsGridChange += OnViewGrid;
        }

        private void UnBind()
        {
            ECSController.ServiceEvents.Off<EventCreateEntity>(OnEventCreateEntity);
            ECSController.ServiceEvents.Off<EventDeleteEntity>(OnEventDeleteEntity);
            WATP.Root.GameDataManager.Preferences.OnIsGridChange -= OnViewGrid;
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


        public async void CreateObj(IEntity entityData)
        {
            IView prs = null;

            if (entityData is FarmerEntity)
            {
                var eFarmer = entityData as FarmerEntity;
                prs = new FarmerView(eFarmer, root);
            }

            else if (entityData is NpcEntity)
            {
                var eNpc = entityData as NpcEntity;
                prs = new NpcView(eNpc, root);
            }

            else if (entityData is AnimalEntity)
            {
                var eAnimal = entityData as AnimalEntity;
                prs = new AnimalView(eAnimal, root);
            }

            else if (entityData is HoedirtEntity)
            {
                var eHoe = entityData as HoedirtEntity;
                prs = new HoedirtView(eHoe, root);
            }

            else if (entityData is CropsEntity)
            {
                var eCrops = entityData as CropsEntity;
                prs = new CropsView(eCrops, root);
            }
            else if (entityData is MapObjectEntity)
            {
                var eMapObject = entityData as MapObjectEntity;
                prs = new MapObjectView(eMapObject, root);
            }

            if (prs == null) return;
            prsDic.Add(entityData.UID, prs);
            ViewController.ServiceEvents.Emit(new ViewCreateEvent(prs));

            var prefab = prs as IPrefabHandler;
            await prefab.LoadAsync(prefab.PrefabPath, prefab.Parent);
        }

        public void RemoveObj(IEntity entityData)
        {
            Remove(entityData.UID);
        }

        public void Remove(int id)
        {
            if (prsDic.ContainsKey(id))
            {
                ViewController.ServiceEvents.Emit(new ViewDeleteEvent(prsDic[id]));

                if (prsDic[id].IsAlreadyDisposed == false)
                    prsDic[id].Dispose();

                prsDic.Remove(id);
            }
        }

        public void ReadyGame(List<int> teamIndexs)
        {
            for (int i = 0; i < root.childCount; i++)
                GameObject.Destroy(root.GetChild(i).gameObject);

            for (int i = 0; i < teamIndexs.Count; i++)
            {
                GameObject temp = new GameObject($"Team {teamIndexs[i]}");
                temp.transform.SetParent(root);
            }
        }

        public void Clear()
        {
            var prsList = prsDic.Values.ToList();
            foreach (var prs in prsList)
                prs.Dispose();

            prsDic.Clear();
        }


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


        #region event

        void OnEventCreateEntity(EventCreateEntity e)
        {
            CreateObj(e.Entity);
        }

        void OnEventDeleteEntity(EventDeleteEntity e)
        {
            RemoveObj(e.Entity);
        }


        private void OnViewGrid(bool grid)
        {
            foreach (var prs in prsDic.Values)
            {
                if(prs is IGridView)
                    (prs as IGridView).SetGridView(grid);
            }
        }

        #endregion
    }
}
