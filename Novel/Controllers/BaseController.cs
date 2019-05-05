using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Novel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Novel.Controllers
{
    public class BaseController : Controller
    {

        protected const int SAVECOOKIESTIME = 1051200;

        public int UserId
        {
            get
            {
                int userId = 0;
                if (User.Identity.IsAuthenticated && HttpContext.User.Claims.Any(m => m.Type == ClaimTypes.PrimarySid))
                {
                    userId = HttpContext.User.Claims.First(m => m.Type == ClaimTypes.PrimarySid).Value.AsInt();
                }
                return userId;
            }
        }


        /// <summary>
        /// 设置本地cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>  
        /// <param name="minutes">过期时长，单位：分钟</param>      
        protected void SetCookies(string key, string value, int minutes = 30)
        {
            HttpContext.Response.Cookies.Append(key, value, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(minutes)
            });
        }
        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        /// <param name="key">键</param>
        protected virtual void DeleteCookies(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        protected virtual string GetCookies(string key)
        {
            HttpContext.Request.Cookies.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            return value;
        }

        protected virtual T GetCookies<T>(string key, T t = default(T)) where T : class
        {
            string v = GetCookies(key);
            if (string.IsNullOrWhiteSpace(v))
            {
                return t;
            }
            t = JsonUtil.DeserializeJsonToObject<T>(v);
            return t;
        }

        protected string GetClientIp()
        {
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }

        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="chr">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把TXT代码转换成HTML格式
        protected virtual string TxtToHtml(string Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br />");
            sb.Replace("\n", "<br />");
            sb.Replace("\t", " ");
            //sb.Replace(" ", "&nbsp;");
            return sb.ToString();
        }


        /// <summary>
        /// 读取txt文件内容
        /// </summary>
        /// <param name="Path">文件地址</param>
        protected string ReadTxtContent(string Path)
        {
            using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sw = new StreamReader(fs, Encoding.UTF8))
                {
                    return sw.ReadToEnd();
                }

            }
        }
    }
}
