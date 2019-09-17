// -----------------------------------------------------------------
// <copyright file="StringCacheKeyGenerator" company="Just Item">
//      Copyright (c) 2005-2019 Just. All rights reserved.
// </copyright>
// <editor>杨鹏</editor>
// <created-date>2019-09-17 15:15:55</created-date>
// -----------------------------------------------------------------

using Galaxy.Collections;
using Galaxy.Extensions;


namespace Galaxy.Caching
{
    /// <summary>
    /// 字符串缓存键生成器
    /// </summary>
    public class StringCacheKeyGenerator : ICacheKeyGenerator
    {
        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public string GetKey(params object[] args)
        {
            args.CheckNotNullOrEmpty("args");
            return args.ExpandAndToString("-");
        }
    }
}
