// -----------------------------------------------------------------
// <copyright file="LoggerExtensions" company="Just Item">
//      Copyright (c) 2005-2019 Just. All rights reserved.
// </copyright>
// <editor>杨鹏</editor>
// <created-date>2019-09-17 15:28:49</created-date>
// -----------------------------------------------------------------

using System;

using Microsoft.Extensions.Logging;


namespace Galaxy.Logging
{
    /// <summary>
    /// <see cref="ILogger"/>扩展方法
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// 记录异常日志信息，并返回异常以便抛出
        /// </summary>
        public static Exception LogExceptionMessage(this ILogger logger, Exception exception, string message = null)
        {
            logger.LogError(exception, message ?? exception.Message);
            return exception;
        }
    }
}
