using System;
using System.Collections.Generic;
using System.Text;

namespace Novel.Utilities
{
    public class PageInfo
    {
        public static void CheckPageIndexAndSize(ref int index, ref int size)
        {
            if (index < 1)
            {
                index = 1;
            }

            if (size < 1)
            {
                size = 20;
            }
        }

        public static void CheckPageIndexAndSize(ref int index, int size, int count)
        {
            if (count >= index * size)
            {
                return;
            }

            index = count / size;
            if (count % size > 0)
            {
                index++;
            }

            if (index == 0)
            {
                index = 1;
            }
        }

    }

    public class PageInfo<T> : PageInfo
    {
        internal PageInfo()
        {
            DataList = new List<T>();
        }
        public PageInfo(int index, int pageSize, int count, List<T> dataList)
        {
            Index = index;
            PageSie = pageSize;
            Count = count;
            DataList = dataList;
        }

        public int Index { get; private set; }
        public int PageSie { get; private set; }
        public int Count { get; private set; }
        public List<T> DataList { get; private set; }

        public PageInfo<T> Empty
        {
            get { return new PageInfo<T>(); }
        }
    }
}
