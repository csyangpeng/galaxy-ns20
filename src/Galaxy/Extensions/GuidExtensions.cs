// -----------------------------------------------------------------
// <copyright file="GuidExtensions" company="Just Item">
//      Copyright (c) 2005-2019 Just. All rights reserved.
// </copyright>
// <editor>杨鹏</editor>
// <created-date>2019-10-09 8:41:17</created-date>
// -----------------------------------------------------------------

using System;

namespace Galaxy.Extensions
{
    /// <summary>
    /// Guid 扩展辅助类
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// 将给定的Guid转换为一组路径, guid第1个字符/guid第2个字符/guid第3个字符/guid
        /// 示例: 如果 guid 为 "7c48a98e-0251-4fa3-af76-0374516d29bd",则转换后为 "7/c/4/7c48a98e-0251-4fa3-af76-0374516d29bd/"
        /// </summary>
        /// <param name="source"><see cref="Guid"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToFilePathFromGuid(this Guid source)
        {
            if (source == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(source), "guid 不能为空.");
            }

            string guidString = source.ToString();
            return $"{guidString.Substring(0, 1)}/{guidString.Substring(1, 1)}/{guidString.Substring(2, 1)}/{guidString}/";
        }
    }
}
