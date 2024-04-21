using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Cysharp.Threading.Tasks;

namespace WATP.Data
{
    /// <summary>
    /// ��ü Atlas�� �����ϴ� Ŭ����
    /// �߰��� ��� parial�� ����
    /// </summary>
    public partial class AtlasContainer
    { 
        // ��ȯ�� ���� �ʿ��� atlas�� ĳ�÷� �����ؼ� �̸� load���� ó��(�޸� �̽� ����)
        private Dictionary<string, SpriteAtlas> spriteAtlasCacheDic;

        public AtlasContainer()
        {
            Bind();
            spriteAtlasCacheDic = new Dictionary<string, SpriteAtlas>();
            AddLoadProcess();
        }

        partial void AddLoadProcess();

        public void Destroy()
        {
            UnBind();
            spriteAtlasCacheDic.Clear();
        }

        private void Bind()
        {
            SpriteAtlasManager.atlasRequested += RequestAtlas;
        }

        private void UnBind()
        {
            SpriteAtlasManager.atlasRequested -= RequestAtlas;
        }


        #region Cache

        //atlas load �� ĳ�ÿ� ����
        public async UniTask<SpriteAtlas> LoadAtlasCache(string atlasName)
        {
            var atlas = await AssetLoader.LoadAsync<SpriteAtlas>(GetPath(atlasName));
            if (atlas == null) return null;

            if (spriteAtlasCacheDic.ContainsKey(atlasName) == false)
                spriteAtlasCacheDic.Add(atlasName, atlas);

            return atlas;
        }

        //caching�� �� atlas���� �̹����� ã�� ��ȯ
        //caching�� �ȵǾ� ������ �߰� �ʿ�(scene ����� ����)
        public Sprite GetImageSprite(string imgName)
        {
            foreach (var key in spriteAtlasCacheDic.Keys)
            {
                var sprite = spriteAtlasCacheDic[key].GetSprite(imgName);
                if (sprite != null) return sprite;
            }

            return null;
        }

        //atlas �� ������� �ʰ� �̹����� �ε��մϴ�.
        //������ unload �ʿ�
        public Sprite LoadSprite(string path)
        {
            return AssetLoader.Load<Sprite>(path);
        }

        #endregion

        //Late binding�� �ʿ�
        private void RequestAtlas(string name, Action<SpriteAtlas> action)
        {
            Action<SpriteAtlas> onLoaded = (SpriteAtlas atlas) =>
            {
               /* if (spriteAtlasCacheDic.ContainsKey(atlas.name) == false)
                    spriteAtlasCacheDic.Add(atlas.name, atlas);*/

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