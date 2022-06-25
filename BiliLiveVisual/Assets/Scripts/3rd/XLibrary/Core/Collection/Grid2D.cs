using System;
using System.Collections.Generic;

namespace XLibrary.Collection
{
    public class Grid2D<T>
    {
        private static readonly List<T> s_empty = new List<T>();
        private List<Dictionary<T,int>> m_gridsList;
        private Dictionary<T, Dictionary<T, int>> m_gridEnumerator;
        private Dictionary<int, bool> m_gridDirty;
        private Dictionary<int, List<T>> m_listCache;
        private int m_gridWidth;
        private int m_gridHeight;
        private int m_row;
        private int m_col;

        public Grid2D()
        {

        }

        public Grid2D(int row,int col,int width,int height)
        {
            Init(row, col, width, height);
        }

        public bool Init(int row, int col, int width, int height)
        {
            if (row > 0 && col > 0 && width > 0 && height > 0)
            {
                m_row = row;
                m_col = col;
                m_gridWidth = width / m_col;
                m_gridHeight = height / m_row;
                if (m_gridWidth > 0 && m_gridHeight > 0)
                {
                    m_gridsList = new List<Dictionary<T, int>>(m_row * m_col);
                    for (int i = 0; i < m_gridsList.Capacity; i++) m_gridsList.Add(null);

                    m_gridDirty = new Dictionary<int, bool>();
                    m_listCache = new Dictionary<int, List<T>>();
                    m_gridEnumerator = new Dictionary<T, Dictionary<T, int>>();
                    return true;
                }

            }
            return false;
        }

        public void Update(T obj, int x, int y)
        {
            if (m_gridsList == null) return;

            int index = ConvertGridIndex(x, y);
            if (index >= 0 && index < m_gridsList.Capacity)
            {
                var curMap = m_gridsList[index];
                if (curMap == null)
                {
                    curMap = new Dictionary<T, int>();
                    m_gridsList[index] = curMap;
                }

                //更新位置
                if (m_gridEnumerator.ContainsKey(obj))
                {
                    var lastMap = m_gridEnumerator[obj];
                    if (lastMap == curMap) return;

                    m_gridDirty[lastMap[obj]] = true;
                    lastMap.Remove(obj);
                    m_gridEnumerator.Remove(obj);
                }

                m_gridDirty[index] = true;
                curMap.Add(obj, index);
                m_gridEnumerator.Add(obj, curMap);
                
            }
            else
            {
                if (m_gridEnumerator.ContainsKey(obj))
                {
                    Remove(obj);
                }
            }
        }

        public void Remove(T obj)
        {
            if (m_gridsList == null) return;
            if (m_gridEnumerator.ContainsKey(obj))
            {
                var lastMap = m_gridEnumerator[obj];

                m_gridDirty.Remove(lastMap[obj]);
                lastMap.Remove(obj);
                m_gridEnumerator.Remove(obj);
            }
            
        }

        public List<T> Get(int index)
        {
            if (m_gridsList == null) return s_empty;
            List<T> retList = s_empty;
            if (index >= 0 && index < m_gridsList.Capacity)
            {
                bool isNeedUpdate = true;
                if (m_listCache.TryGetValue(index, out retList))    //直接从Cache里取
                {
                    bool isDirty;
                    m_gridDirty.TryGetValue(index, out isDirty);
                    isNeedUpdate = isDirty;
                }
               
                if (isNeedUpdate)
                {
                    var gridMap = m_gridsList[index];
                    if (gridMap != null)
                    {
                        if (gridMap.Count > 0)
                        {
                            retList = new List<T>(gridMap.Keys);                 //大量高速移动的话,还是会很频繁
                            m_listCache[index] = retList;
                        }
                    }
                }
            }

            m_gridDirty[index] = false;
            return retList;
        }

        public List<T> Local(T obj)
        {
            if (obj != null)
            {
                if (m_gridEnumerator.ContainsKey(obj))
                {
                    var lastMap = m_gridEnumerator[obj];
                    var index = lastMap[obj];
                    return Get(index);
                }
            }
            return s_empty;
        }

        public List<T> GetByGrid(int row, int col)
        { 
            int index = GetGridIndex(row, col);
            return Get(index);
        }

        public List<T> GetByPosition(int x, int y)
        {
            int index = ConvertGridIndex(x, y);
            return Get(index);
        }

        public void Clear()
        {
            if (m_gridsList == null) return;

            foreach(var list in m_gridsList)
            {
                if (list != null)
                {
                    list.Clear();
                }
            }
            m_gridDirty.Clear();
            m_listCache.Clear();
            m_gridEnumerator.Clear();
        }

        //
       
        public int GetGridIndex(int row, int col)
        {
            return Math.Max(0,row - 1) * m_col + col;
        }

        public int ConvertGridIndex(int x,int y)
        {
            int xIndex = x / m_gridWidth;
            int yIndex = y / m_gridHeight;

            return GetGridIndex(xIndex, yIndex);
        }


    }

}
