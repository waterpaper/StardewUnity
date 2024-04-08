using System;
using System.Collections.Generic;

namespace WATP.Structure
{
    public class RandomWeight<T>
    {
        List<RandomElement<T>> list = new List<RandomElement<T>>();

        public List<RandomElement<T>> List { get => list; }

        public void Add(T element, float weight)
        {
            list.Add(new RandomElement<T>(element, weight));
        }

        public void Remove(T element)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].element.Equals(element) == true)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        public void Clear()
        {
            list.Clear();
        }

        public T Random()
        {
            double totalWeight = 0;
            for (int i = 0; i < list.Count; i++)
                totalWeight += list[i].weight;

            double before = 0;
            double pick = UnityEngine.Random.value * totalWeight;

            for (int i = 0; i < list.Count; i++)
            {
                if (pick < before + list[i].weight)
                    return list[i].element;
                else
                    before += list[i].weight;
            }

            return default;
        }
    }

    public class RandomElement<T>
    {
        public T element;
        public float weight;

        public RandomElement(T element, float weight)
        {
            this.element = element;
            this.weight = weight;
        }
    }
}
