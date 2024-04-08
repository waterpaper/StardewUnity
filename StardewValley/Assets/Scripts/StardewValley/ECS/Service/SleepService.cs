using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using WATP.UI;

namespace WATP.ECS
{
    /// <summary>
    /// 움직임 컴포넌트
    /// </summary>
    public class SleepService : IService
    {
        public List<ISleepComponent> sleepComponents = new();

        public void Add(IEntity entity)
        {
            if (entity is not ISleepComponent) return;

            sleepComponents.Add(entity as ISleepComponent);
        }

        public void Remove(IEntity entity)
        {
            if (entity is not ISleepComponent) return;

            sleepComponents.Remove(entity as ISleepComponent);
        }

        public void Clear()
        {
            sleepComponents.Clear();
        }

        public void Update(double frameTime)
        {
            foreach (var sleepComponent in sleepComponents)
            {
                if (IsSleepArea(sleepComponent.TransformComponent.position))
                {
                    if (sleepComponent.SleepComponent.isArea) return;
                    OpenSleepCheckPopup().Forget();
                    sleepComponent.SleepComponent.isArea = true;
                }
                else
                {
                    if (sleepComponent.SleepComponent.isArea)
                        sleepComponent.SleepComponent.isArea = false;
                }
            }
        }



        public bool IsSleepArea(Vector3 pos)
        {
            if (Root.SceneLoader.TileMapManager.MapName != "House") return false;


            return Vector3.Distance(pos, new Vector3(9, 4, 0)) < 1f;
        }

        private async UniTaskVoid OpenSleepCheckPopup()
        {
            var sleepCheckPopup = new SleepCheckPopup();
            sleepCheckPopup = await Root.UIManager.Widgets.CreateAsync<SleepCheckPopup>(sleepCheckPopup, SleepCheckPopup.DefaultPrefabPath);
        }

    }
}