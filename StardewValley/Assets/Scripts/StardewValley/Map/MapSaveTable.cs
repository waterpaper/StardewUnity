using System.Collections.Generic;
using System;
using UnityEngine;

namespace WATP.Map
{
    [Serializable]
    public class Portalinfo
    {
        public int portalPosX;
        public int portalPosY;
        public string portalName;
        public float portalNextPosX;
        public float portalNextPosY;
    }

    [Serializable]
    public class SaveTileInfo
    {
        public string imagePrefix;
        public string imageName;
        public int imageIndex;
        public int block;
        public char type;
    }

    [Serializable]
    public class SaveTileMapForm
    {
        public int tileX;
        public int tileY;
        [SerializeField]
        public List<SaveTileInfo> tiles;
        [SerializeField]
        public List<Portalinfo> portals;
    }

    [Serializable]
    public class SaveObjectInfo
    {
        public int objectId;
        public float posX;
        public float posY;
    }

    [Serializable]
    public class SaveForm
    {
        [SerializeField]
        public List<SaveTileMapForm> tilemaps;
        [SerializeField]
        public List<SaveObjectInfo> objectInfos;
    }
}