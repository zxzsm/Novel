using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Novel
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var controller = context.ActionDescriptor;
            #region 记录到内置日志
             
            #endregion
            if (_env.IsDevelopment())
            {
                //log.Error(context.Exception.ToString());
            }
            else
            {
                //log.Error(context.Exception.ToString());
                context.ExceptionHandled = true;
                context.Result = new RedirectResult("/home/Error");
            }
        }
        readonly ILoggerFactory _loggerFactory;//采用内置日志记录
        readonly IHostingEnvironment _env;//环境变量
        public GlobalExceptionFilter(ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            _loggerFactory = loggerFactory;
            _env = env;
        }
    }
}
