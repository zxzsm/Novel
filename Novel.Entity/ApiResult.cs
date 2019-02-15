using Novel.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Novel.Entity
{
    public class ApiResult<T>
    {
        public int status { get; set; }
        public string msg { get; set; }
        public T data { get; set; }

        /// <summary>
        /// 返回JSON格式数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonUtil.SerializeObject(this);
        }

        /// <summary>
        /// 创建返回结果
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="pager">pager对象</param>
        /// <param name="status">状态，成功=0，失败=错误代码 > 0</param>
        /// <param name="error">错误内容</param>
        public static ApiResult<T> OK(T data, int status = 0, string msg = null)
        {
            var rd = new ApiResult<T>();
            rd.data = data;
            rd.status = status;
            rd.msg = msg;
            return rd;
        }

        /// <summary>
        /// 创建返回结果
        /// </summary>
        /// <param name="error">错误内容</param>
        /// <param name="data">数据</param>
        /// <param name="status">状态，成功=0，失败=错误代码 > 0</param>
        public static ApiResult<T> Fail(string error, T data = default(T), int status = 1)
        {
            var rd = new ApiResult<T>();
            rd.data = data;
            rd.status = status;
            rd.msg = error;
            return rd;
        }
    }
}
