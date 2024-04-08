using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WATP.UI
{
    public class Tab : Button
    {
        [SerializeField]
        public TabMenu tabMenu;
        [SerializeField]
        public GameObject targetObject;

        private bool isSelectTab;

        protected override void Awake()
        {
            if (tabMenu)
                tabMenu.AddTab(this);

            if (targetObject)
                targetObject.gameObject.SetActive(false);
        }

        protected override void OnDestroy()
        {
            if (tabMenu)
                tabMenu.CloseTab(this);
        }


        public override void OnPointerClick(PointerEventData eventData)
        {
            if (isSelectTab) return;

            base.OnPointerClick(eventData);
            tabMenu.SelectTab(this);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (isSelectTab) return;

            base.OnPointerUp(eventData);
        }

        public void SelectTab()
        {
            if (isSelectTab) return;
            isSelectTab = true;

            if(targetObject != null) targetObject.gameObject.SetActive(true);
            if(image != null) image.raycastTarget = false;
        }

        public void DeSelectTab()
        {
            if (!isSelectTab) return;
            isSelectTab = false;

            if(targetObject != null) targetObject.gameObject.SetActive(false);
            if (image != null) image.raycastTarget = true;
        }
    }
}
