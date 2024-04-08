using UnityEngine;

namespace WATP.Data
{
    public class Config
    {
        #region Default

        //Custom
        public static readonly int CUSTOM_HAIR_MAX = 32;
        public static readonly int CUSTOM_CLOTHS_MAX = 112;

        public static readonly int MAP_LAYER_MAX = 2;

        #endregion


        #region item

        public static readonly int INVENTORY_LEVEL_MAX = 3;
        public static readonly int INVENTORY_LEVEL_COUNT = 12;

        #endregion

        #region npc

        public static readonly int NPC_MAX = 32;
        public static readonly int GIFT_LIKE = 10;
        public static readonly int GIFT_FAVOR_LIKE = 30;

        #endregion
    }
}
