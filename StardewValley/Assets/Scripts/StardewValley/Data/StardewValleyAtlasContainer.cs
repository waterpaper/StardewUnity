using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Cysharp.Threading.Tasks;

namespace WATP.Data
{

    /// <summary>
    /// 전체 Atlas를 관리하는 클래스
    /// index를 불러와야하는 atlas는 미리 캐싱해서 사용
    /// </summary>
    public partial class AtlasContainer
    {
        public readonly string FARMER_BODY_PATH = "Atlas_FarmerBody";
        public readonly string FARMER_ARM_PATH = "Atlas_FarmerArm";
        public readonly string FARMER_HAIR_PATH = "Atlas_FarmerHair";
        public readonly string FARMER_SHIRTS_PATH = "Atlas_FarmerShirts";
        public readonly string FARMER_PANTS_PATH = "Atlas_FarmerPants";

        public readonly string OBJECTS = "Atlas_Objects";
        public readonly string TOOLS = "Atlas_Tools";
        public readonly string WEAPONS = "Atlas_Weapons";
        public readonly string CROPS = "Atlas_Crops";
        public readonly string DEBRIS = "Atlas_Debris";
        public readonly string TREES = "Atlas_Trees";

        public readonly string HOEDIRT = "Atlas_Hoedirt";

        partial void AddLoadProcess()
        {
            Loading.AddLoadProcess(LoadingType.postProcess, AtlasCacheLoadAssets(), 1);
        }

        public void Clear()
        {
            spriteAtlasCacheDic.Clear();
        }

        public async UniTask AtlasCacheLoadAssets()
        {
            Action<SpriteAtlas> onLoaded = (SpriteAtlas at) =>
            {
                if (spriteAtlasCacheDic.ContainsKey(at.name) == false)
                    spriteAtlasCacheDic.Add(at.name, at);
            };

            var tasks = await UniTask.WhenAll(
                LoadAtlasCache(FARMER_BODY_PATH),
                LoadAtlasCache(FARMER_ARM_PATH),
                LoadAtlasCache(FARMER_HAIR_PATH),
                LoadAtlasCache(FARMER_SHIRTS_PATH),
                LoadAtlasCache(FARMER_PANTS_PATH),

                LoadAtlasCache(OBJECTS),
                LoadAtlasCache(TOOLS),
                LoadAtlasCache(WEAPONS),
                LoadAtlasCache(CROPS),
                LoadAtlasCache(DEBRIS),
                LoadAtlasCache(HOEDIRT),
                LoadAtlasCache(TREES)
                );
        }

        public Sprite GetSheetSprite(string sheetPath, string imgName)
        {
            if (spriteAtlasCacheDic.ContainsKey(sheetPath))
                return spriteAtlasCacheDic[sheetPath].GetSprite($"{imgName}");
            else
                return null;
        }
    }
}