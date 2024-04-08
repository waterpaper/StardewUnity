using System.Collections.Generic;
using System;
using UnityEngine;

namespace WATP.Map
{

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