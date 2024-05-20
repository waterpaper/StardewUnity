using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using WATP.UI;

namespace WATP
{
    /// <summary>
    /// Maptool ¾À
    /// </summary>
    public class MaptoolScene : GameSceneBase
    {
        public override void Init()
        {
            Root.State.logicState.Value = LogicState.Parse;
        }

        public override void Complete()
        {
            Root.SoundManager.PlaySound(SoundTrack.BGM, "maptool", true);
        }

        public override IEnumerator Load()
        {
            yield return OpenMaptoolPage();
            Root.SceneLoader.TileMapManager.Clear();
        }

        public override IEnumerator Unload()
        {
            Root.SceneLoader.TileMapManager.Clear();
            yield return null;
        }

        private async UniTaskVoid OpenMaptoolPage()
        {
            var ingamePage = new IngamePage();
            ingamePage = await Root.UIManager.Widgets.CreateAsync<IngamePage>(ingamePage, IngamePage.DefaultPrefabPath);
        }
    }
}