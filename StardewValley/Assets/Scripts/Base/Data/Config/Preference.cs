using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace WATP.Data
{
    /// <summary>
    /// 환경설정을 관리하는 클래스
    /// 기본 옵션값을 각 기기에 저장해서 사용한다.
    /// 바이너리 파일로 save & load
    /// </summary>
    public class Preferences
    {
        private LocalSaveData saveData;

        private int simulationFrame = 30;
        private float simulationMultiply = 1;
        private float simulationDeltaTime = Mathf.Round(1.0f / 30 * 10000) / 10000;

        public int SimulationFrame { get => simulationFrame; }
        public float SimulationDeltaTime { get => simulationDeltaTime; }
        public float SimulationMultiply { get => simulationMultiply; }

        public int GameFrame { get => saveData.gameFrame.Value; }
        public Action<int> OnGameFrameChange { get => saveData.gameFrame.onChange; }

        public int Bgm { get => saveData.bgm.Value; set => saveData.bgm.Value = value; }
        public Action<int> OnBgmChange { get => saveData.bgm.onChange; set => saveData.bgm.onChange = value; }

        public int Sfx { get => saveData.sfx.Value; set => saveData.sfx.Value = value; }
        public Action<int> OnSfxChange { get => saveData.sfx.onChange; set => saveData.sfx.onChange = value; }

        public bool IsBgm { get => saveData.isBgm.Value; }
        public Action<bool> OnIsBgmChange { get => saveData.isBgm.onChange; }
        
        public bool IsSfx { get => saveData.isSfx.Value; }
        public Action<bool> OnIsSfxChange { get => saveData.isSfx.onChange; }
        
        public bool IsAlarm { get => saveData.isAlarm.Value; }
        public Action<bool> OnAlarmChange { get => saveData.isAlarm.onChange; }

        public bool IsGrid { get => saveData.isGrid.Value; }
        public Action<bool> OnIsGridChange { get => saveData.isGrid.onChange; set => saveData.isGrid.onChange = value; }


        private void Init()
        {
            saveData = new LocalSaveData();
            simulationDeltaTime = Mathf.Round(1.0f / SimulationFrame * 10000) / 10000;

/*#if !UNITY_EDITOR
            Application.targetFrameRate = GameFrame;
#endif*/
        }

        public void SetGameFrame(int frame)
        {
            saveData.gameFrame.Value = frame;
            Save();
            Application.targetFrameRate = GameFrame;
        }

        public void SetBgm(bool isBgm)
        {
            saveData.isBgm.Value = isBgm;
            Save();
        }

        public void SetSfx(bool isSfx)
        {
            saveData.isSfx.Value = isSfx;
            Save();
        }

        public void SetAlarm(bool isAlarm)
        {
            saveData.isAlarm.Value = isAlarm;
            Save();
        }

        public void SetIsGrid(bool isGrid)
        {
            saveData.isGrid.Value = isGrid;
            Save();
        }

        public void Save()
        {
            if (saveData == null) return;

             BinaryFormatter bf = new BinaryFormatter();
             FileStream file = File.Create(Application.persistentDataPath + "/Preferences.dat");
             bf.Serialize(file, saveData.SaveTable());
             file.Close();
             Debug.Log("Game data saved!");
        }

        public void Load()
        {
            if (File.Exists(Application.persistentDataPath + "/Preferences.dat") == false)
            {
                saveData = null;
                Init();
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Preferences.dat", FileMode.Open);

            try
            {
                LocalSaveTable localData = (LocalSaveTable)bf.Deserialize(file);

                saveData = new LocalSaveData(localData);
                file.Close();
            }
            catch(Exception e)
            {
                file.Close();

                File.Delete(Application.persistentDataPath + "/Preferences.dat");
                saveData = null;
                Init();
                return;
            }

            Debug.Log("Game data loaded!");
        }
    }
}
