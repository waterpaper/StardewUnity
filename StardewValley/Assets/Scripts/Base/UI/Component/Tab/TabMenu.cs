using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WATP.UI
{
    public class TabMenu : MonoBehaviour
    {
        [SerializeField]
        private int selectIndex = 0;
        private Tab selectTab = null;
        private List<Tab> tabs = new List<Tab>();

        UnityEvent<int> onSelectTab = new UnityEvent<int>();

        public UnityEvent<int> OnSelectTab { get => onSelectTab; }
        public Tab SelectedTab { get => selectTab; }

        private void Start()
        {
            SelectTab(selectIndex);
        }

        private void OnDestroy()
        {
            tabs.Clear();
            onSelectTab.RemoveAllListeners();
        }

        public void AddTab(Tab addTab)
        {
            tabs.Add(addTab);
        }

        public void CloseTab(Tab closeTab)
        {
            tabs.Remove(closeTab);

            if (selectTab == closeTab)
            {
                if (tabs.Count <= 0)
                {
                    selectIndex = 0;
                    selectTab = null;
                }
                else
                    SelectTab(0);
            }
        }

        public void SelectTab(int index)
        {
            if (tabs.Count <= index)
            {
                if (tabs.Count <= 0) return;
                index = 0;
            }

            var selectTab = tabs[index];
            SelectTab(selectTab);
        }

        public void SelectTab(Tab selectTab)
        {
            for(var i =0; i< tabs.Count; i++)
            {
                if (tabs[i] == selectTab)
                {
                    selectIndex = i;
                    tabs[i].SelectTab();
                    onSelectTab.Invoke(i);
                    this.selectTab = tabs[i];
                }
                else
                    tabs[i].DeSelectTab();
            }
        }
    }
}
