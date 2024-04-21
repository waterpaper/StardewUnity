using System;
using System.Collections;
using System.Collections.Generic;

namespace WATP.Structure
{
    public enum SortType
    {
        ASCENDING,
        DESCENDING
    }

    /// <summary>
    /// 우선순위 queue
    /// </summary>
    public class PriorityQueue<T> where T : IComparable
    {
        protected readonly List<T> _nodes = new List<T>();

        protected SortType _sortType;

        public SortType SortType => _sortType;
        public int Count => _nodes.Count;

        public PriorityQueue(SortType sortType)
        {
            _sortType = sortType;
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public bool Contains(T node)
        {
            return Find(0, node);
        }

        public T Dequeue()
        {
            T node = default(T);

            if (_nodes.Count > 0)
            {
                node = _nodes[0];
                RemoveAt(0);
            }

            return node;
        }

        public T Peek()
        {
            return _nodes.Count > 0 ? _nodes[0] : default(T);
        }

        public void Enqueue(T node)
        {
            if (_nodes.Contains(node))
                return;

            _nodes.Add(node);
            int index = _nodes.Count - 1;

            while (ChangeIndex_Add(ref index)) { }
        }

        public void ChangeNode(T node)
        {
            int index = _nodes.FindIndex(_node => node.Equals(_node));

            if (index < 0)
                return;

            while (ChangeIndex(ref index)) { }
        }

        private bool RemoveAt(int index)
        {
            if (index < 0) return false;

            if (index == _nodes.Count - 1)
            {
                _nodes.RemoveAt(index);
                return true;
            }

            _nodes[index] = _nodes[_nodes.Count - 1];
            _nodes.RemoveAt(_nodes.Count - 1);

            while (ChangeIndex_Remove(ref index)) { }

            return true;
        }

        private bool ChangeIndex(ref int index)
        {
            int childIndex = CompareChildIndex(index);

            if (childIndex < 0)
                return false;

            if (CompareForSwap(childIndex, index))
            {
                index = childIndex;
                return true;
            }
            else
                return false;

        }

        private bool ChangeIndex_Add(ref int index)
        {
            if (index == 0) return false;

            int parentIndex = (index - 1) / 2;

            if (CompareForSwap(index, parentIndex))
            {
                index = parentIndex;
                return true;
            }
            else
                return false;
        }

        private bool ChangeIndex_Remove(ref int index)
        {
            int childIndex = CompareChildIndex(index);

            if (childIndex < 0) 
                return false;

            if (CompareForSwap(childIndex, index))
            {
                index = childIndex;
                return true;
            }
            else
                return false;

        }

        protected virtual int CompareChildIndex(int index)
        {
            int leftChildIndex = ((index + 1) * 2) - 1;
            int rightChildIndex = (index + 1) * 2;

            if ((leftChildIndex >= _nodes.Count) && (rightChildIndex >= _nodes.Count))
                return -1;
            else if ((leftChildIndex < _nodes.Count) && (rightChildIndex < _nodes.Count))
            {
                int result = _nodes[leftChildIndex].CompareTo(_nodes[rightChildIndex]);

                if (_sortType == SortType.ASCENDING)
                {
                    if (result < 0)
                        return leftChildIndex;
                    else
                        return rightChildIndex;
                }
                else
                {
                    if (result > 0)
                        return leftChildIndex;
                    else
                        return rightChildIndex;
                }
            }
            else if (leftChildIndex < _nodes.Count)
                return leftChildIndex;
            else if (rightChildIndex < _nodes.Count)
                return rightChildIndex;
            else
                return -1;
        }

        protected void Swap(int x, int y)
        {
            T temp = _nodes[y];
            _nodes[y] = _nodes[x];
            _nodes[x] = temp;
        }

        protected virtual bool CompareForSwap(int x, int y)
        {
            int compareResult = _nodes[x].CompareTo(_nodes[y]);

            if (_sortType == SortType.ASCENDING)
            {
                if (compareResult < 0)
                {
                    Swap(x, y);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if (compareResult > 0)
                {
                    Swap(x, y);
                    return true;
                }
                else
                    return false;
            }
        }

        protected virtual bool Find(int index, T node)
        {
            if (_nodes.Count <= 0 || index < 0 || index >= _nodes.Count)
                return false;
            else if (index < _nodes.Count && node.Equals(_nodes[index]))
                return true;

            int compareResult = _nodes[index].CompareTo(node);

            if (_sortType == SortType.ASCENDING)
            {
                if (compareResult < 0)
                {
                    return Find(((index + 1) * 2) - 1, node) || Find(((index + 1) * 2), node);
                }
                else
                    return false;
            }
            else
            {
                if (compareResult > 0)
                {
                    return Find(((index + 1) * 2) - 1, node) || Find(((index + 1) * 2), node);
                }
                else
                    return false;
            }
        }
    }
}
