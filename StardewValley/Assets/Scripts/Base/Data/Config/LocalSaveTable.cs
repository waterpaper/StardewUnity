
using System;

namespace WATP.Data
{
    [Serializable]
    class LocalSaveTable
    {
        public int gameFrame;

        public int bgm;
        public int sfx;

        public bool isBgm;
        public bool isSfx;
        public bool isAlarm;
        public bool isGrid;

        public LocalSaveTable()
        {
            gameFrame = 30;
            bgm = 100;
            sfx = 100;

            isBgm = true;
            isSfx = true;
            isAlarm = false;
            isGrid = true;
        }
    }

    class LocalSaveData
    {
        public SubjectData<int> gameFrame;

        public SubjectData<int> bgm;
        public SubjectData<int> sfx;

        public SubjectData<bool> isBgm;
        public SubjectData<bool> isSfx;
        public SubjectData<bool> isAlarm;
        public SubjectData<bool> isGrid;

        public LocalSaveData()
        {
            gameFrame.Value = 30;

            bgm.Value = 100;
            sfx.Value = 100;

            isBgm.Value = true;
            isSfx.Value = true;
            isAlarm.Value = false;
            isGrid.Value = true;
        }

        public LocalSaveData(LocalSaveTable saveTable)
        {
            gameFrame.Value = saveTable.gameFrame;

            isBgm.Value = saveTable.isBgm;
            isSfx.Value = saveTable.isSfx;
            isAlarm.Value = saveTable.isAlarm;
            isGrid.Value = saveTable.isGrid;
        }

        public LocalSaveTable SaveTable()
        {
            LocalSaveTable data = new LocalSaveTable();
            data.gameFrame = gameFrame.Value;
            data.bgm = bgm.Value;
            data.sfx = sfx.Value;
            data.isBgm = isBgm.Value;
            data.isSfx = isSfx.Value;
            data.isAlarm = isAlarm.Value;

            return data;
        }

    }
}