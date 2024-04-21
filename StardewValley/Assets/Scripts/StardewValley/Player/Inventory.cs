using System;
using System.Collections.Generic;

namespace WATP.Player
{
    /// <summary>
    /// 플레이어 인벤트로 클래스
    /// </summary>
    public class Inventory
    {
        public SubjectData<int> selectIndex;
        public SubjectData<int> inventoryLevel;
        public Action<int> onChangeAction;
        public Action<int, int> onAddAction;

        List<ItemInfo> items = new();

        public ItemInfo SelectItem { get => items.Find(data => data.itemIndex == selectIndex.Value); }
        public bool IsFull { get => items.Count >= inventoryLevel.Value * Data.Config.INVENTORY_LEVEL_COUNT; }
        public int FullCount { get => inventoryLevel.Value * Data.Config.INVENTORY_LEVEL_COUNT; }
        public List<ItemInfo> Items { get => items; }

        public void Init()
        {
            selectIndex.Value = -1;
            inventoryLevel.Value = 3;
            items.Clear();
        }

        public void AddInventory(int id, int qty)
        {
            var item = GetItem_Id(id);
            if (item == null)
            {
                var firstIndex = FirstIndex();
                if (firstIndex == -1) return;
                var newItem = new ItemInfo() { itemId = id, itemQty = qty, itemIndex = firstIndex };
                items.Add(newItem);
                Sort();
                onChangeAction?.Invoke(firstIndex);
                onAddAction?.Invoke(id, qty);
            }
            else
            {
                item.itemQty += qty;
                onChangeAction?.Invoke(item.itemIndex);
                onAddAction?.Invoke(id, qty);
            }
        }

        public void AddInventory(int id, int qty, int index)
        {
            var item = GetItem_Index(index);

            if (item == null)
            {
                var newItem = new ItemInfo() { itemId = id, itemQty = qty, itemIndex = index };
                items.Add(newItem);
                Sort();
                onChangeAction?.Invoke(newItem.itemIndex);
                onAddAction?.Invoke(id, qty);
            }
            else
            {
                if(item.itemId == id)
                {
                    item.itemQty += qty;
                    onChangeAction?.Invoke(item.itemIndex);
                    onAddAction?.Invoke(id, qty);
                }
                else
                {
                    AddInventory(id, qty);
                }
            }
        }

        public void RemoveInventory(int index)
        {
            var item = GetItem_Index(index);

            if (item == null) return;
            items.Remove(item);

            onChangeAction?.Invoke(item.itemIndex);
        }

        public void RemoveInventory(int id, int qty)
        {
            var item = GetItem_Id(id);

            if (item == null) return;

            item.itemQty -= qty;

            if (item.itemQty <= 0)
                items.Remove(item);

            onChangeAction?.Invoke(item.itemIndex);
        }

        public void Swap(int indexA, int indexB)
        {
            var itemA = GetItem_Index(indexA);
            var itemB = GetItem_Index(indexB);

            if(itemA != null)
                itemA.itemIndex = indexB;

            if (itemB != null)
                itemB.itemIndex = indexA;

            Sort();
            onChangeAction?.Invoke(indexA);
            onChangeAction?.Invoke(indexB);
        }

        public ItemInfo GetItem_Index(int index)
        {
            if (index < 0) return null;

            return items.Find(data => data.itemIndex == index);
        }

        public ItemInfo GetItem_Id(int id)
        {
            return items.Find(data => data.itemId == id);
        }

        int FirstIndex()
        {
            if (items.Count == 0)
                return 0;

            for(int i = 0; i < FullCount; i++)
            {
                if (GetItem_Index(i) != null)
                    continue;

                return i;
            }

            return -1;
        }

        void Sort()
        {
            items.Sort((a, b) =>
            {
                if(a.itemIndex < b.itemIndex)
                    return 1;
                else
                    return -1;
            });
        }
    }
}