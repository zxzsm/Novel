using System;
using System.ComponentModel;

namespace Novel.Common
{
    public enum BookState
    {
        /// <summary>
        /// 连载
        /// </summary>
        Serializing = 0,
        /// <summary>
        /// 完结
        /// </summary>
        End = 1
    }

    public enum SyncType
    {
        /// <summary>
        /// 笔趣云
        /// </summary>
        [Description("笔趣云")]
        BQY =2,
        /// <summary>
        /// 梦想中文
        /// </summary>
        [Description("梦想中文")]
        MXZW =3,
    }
}
