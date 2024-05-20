using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Cysharp.Threading.Tasks;

namespace WATP.Data
{

    /// <summary>
    /// 전체 Atlas를 관리하는 클래스
    /// 추가할 경우 parial로 구현
    /// </summary>
    public partial class AtlasContainer
    { 
        // 변환이 자주 필요한 atlas는 캐시로 저장해서 미리 load히여 처리(메모리 이슈 존재)
        private Dictionary<string, SpriteAtlas> spriteAtlasCacheDic;

        public AtlasContainer()
        {
            Bind();
            spriteAtlasCacheDic = new Dictionary<string, SpriteAtlas>();
            AddLoadProcess();
        }

        partial void AddLoadProcess();

        private void Bind()
        {
            SpriteAtlasManager.atlasRequested += RequestAtlas;
        }

        private void UnBind()
        {
            SpriteAtlasManager.atlasRequested -= RequestAtlas;
        }

        public void Destroy()
        {
            UnBind();
            spriteAtlasCacheDic.Clear();
        }


        //atlas 를 사용하지 않고 이미지를 로드합니다.
        //해제시 unload 필요
        public Sprite LoadSprite(string path)
        {
            return AssetLoader.Load<Sprite>(path);
        }


        #region Cache

        //atlas load 후 캐시에 저장
        public async UniTask<SpriteAtlas> LoadAtlasCache(string atlasName)
        {
            var atlas = await AssetLoader.LoadAsync<SpriteAtlas>(GetPath(atlasName));
            if (atlas == null) return null;

            if (spriteAtlasCacheDic.ContainsKey(atlasName) == false)
                spriteAtlasCacheDic.Add(atlasName, atlas);

            return atlas;
        }

        //caching이 된 atlas에서 이미지를 찾아 반환
        //caching이 안되어 있으면 추가 필요(scene 변경시 삭제)
        public Sprite GetImageSprite(string imgName)
        {
            foreach (var key in spriteAtlasCacheDic.Keys)
            {
                var sprite = spriteAtlasCacheDic[key].GetSprite(imgName);
                if (sprite != null) return sprite;
            }

            return null;
        }

        #endregion

        private void RequestAtlas(string name, Action<SpriteAtlas> action)
        {
            Action<SpriteAtlas> onLoaded = (SpriteAtlas atlas) =>
            {
                if (spriteAtlasCacheDic.ContainsKey(atlas.name) == false)
                    spriteAtlasCacheDic.Add(atlas.name, atlas);

                action?.Invoke(atlas);
            };

            var spriteAtlas = AssetLoader.Load<SpriteAtlas>(GetPath(name));

            onLoaded?.Invoke(spriteAtlas);
        }

        protected string GetPath(string name)
        {
            return $"Address/Atlas/{name}.spriteatlasv2";
        }
    }
}