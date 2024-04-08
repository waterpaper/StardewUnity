using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine;
using UnityEngine.UI;
using WATP.Data;

namespace WATP.UI
{
    public class SaveLoadContainer : UIElement
    {
        private Text nameText;
        private Text farmNameText;

        private Button saveBtn;
        private Button loadBtn;

        private int type;
        private int index;
        private Action action;

        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            nameText = rectTransform.RecursiveFindChild("Txt_Name").GetComponent<Text>();
            farmNameText = rectTransform.RecursiveFindChild("Txt_FarmName").GetComponent<Text>();

            saveBtn = rectTransform.RecursiveFindChild("Bt_Save").GetComponent<Button>();
            loadBtn = rectTransform.RecursiveFindChild("Bt_Load").GetComponent<Button>();

            Bind();
        }

        public override void Dispose()
        {
            UnBind();
            base.Dispose();
        }

        public void Setting(int type, int index, Action action = null)
        {
            this.type = type;
            this.index = index;

            if (type == 1)
                loadBtn.gameObject.SetActive(false);
            else if (type == 2)
                saveBtn.gameObject.SetActive(false);

            InfoDataLoad();
            this.action = action;
        }

        public void InfoDataLoad()
        {
            Load();
        }

        public void Bind()
        {
            saveBtn.onClick.AddListener(OnSaveButton);
            loadBtn.onClick.AddListener(OnLoadButton);
        }

        public void UnBind()
        {
            saveBtn.onClick.RemoveAllListeners();
            loadBtn.onClick.RemoveAllListeners();
        }

        public void OnSaveButton()
        {
            Root.State.Save(index);
            action?.Invoke();
        }

        public void OnLoadButton()
        {
            var isNext = Root.State.Load(index);

            if (isNext == false) return;

            action?.Invoke();
            Root.SceneLoader.SceneLoad(SceneKind.Ingame);
        }

        public void Load()
        {
            if (File.Exists(Application.persistentDataPath + $"/SaveData_{index}.dat") == false)
            {
                nameText.text = "";
                farmNameText.text = "";
                loadBtn.interactable = false;
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + $"/SaveData_{index}.dat", FileMode.Open);

            try
            {
                SaveTableData saveTableData = (SaveTableData)bf.Deserialize(file);

                nameText.text = saveTableData.playerInfo.name;
                farmNameText.text = saveTableData.playerInfo.farmName;
                file.Close();
            }
            catch (Exception e)
            {
                file.Close();

                File.Delete(Application.persistentDataPath + $"/SaveData_{index}.dat");
                return;
            }
        }

    }
}
