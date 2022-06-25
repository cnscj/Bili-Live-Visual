using System;
using System.Collections.Generic;

namespace XLibrary.Collection
{
    public class PriorityQueue<T> where T : IComparable//, new()
    {
        private const int DEFAULT_CAP = 100;

        private T[] _datas = null;
        private int _capacity = DEFAULT_CAP;
        private int _size = 0;

        public PriorityQueue()
            : this(DEFAULT_CAP)
        {
        }

        public PriorityQueue(int capacity)
        {
            _capacity = capacity;
            _datas = new T[_capacity];
        }

        public PriorityQueue(T[] initData)
        {
            if (_capacity <= initData.Length * 2)
            {
                _capacity = initData.Length * 2;
            }

            _datas = new T[_capacity];
            Array.Copy(initData, 0, _datas, 1, initData.Length);
            for (int i = initData.Length / 2; i > 0; i--)
            {
                T item = _datas[i];
                if (_datas[i * 2].CompareTo(_datas[i * 2 + 1]) < 0)
                {
                    _datas[i] = _datas[i * 2];
                    _datas[i * 2] = item;
                }
                else
                {
                    _datas[i] = _datas[i * 2 + 1];
                    _datas[i * 2] = item;
                }
            }

            _size = initData.Length;
        }

        public bool IsEmpty()
        {
            return _size == 0;
        }

        public void Enqueue(T item)
        {
            if (IsFull())
            {
                ReBuild();
            }

            int i;
            for (i = ++_size; i > 1 && item.CompareTo(_datas[i / 2]) < 0; i /= 2)
            {
                _datas[i] = _datas[i / 2];
            }

            _datas[i] = item;
        }

        public T Dequeue()
        {
            if (IsEmpty())
            {
                throw new Exception("no item");
            }

            T first = _datas[1];
            T last = _datas[_size--];

            int i, child = 1;
            for (i = 1; i * 2 <= _size; i = child)
            {
                child = i * 2;
                if ((child != _size) && (_datas[child].CompareTo(_datas[child + 1]) > 0))
                {
                    child++;
                }

                if (last.CompareTo(_datas[child]) > 0)
                {
                    _datas[i] = _datas[child];
                }
                else
                {
                    break;
                }
            }
            _datas[i] = last;

            return first;
        }

        public void Clear()
        {
            _datas = new T[_capacity];
        }

        private bool IsFull()
        {
            return _size == (_datas.Length - 1);
        }

        private void ReBuild()
        {
            _capacity = _capacity * 2;
            T[] data = new T[_capacity];
            Array.Copy(_datas, data, _datas.Length);
            _datas = data;
        }

    }
}