// -----------------------------------------------------------------
// <copyright file="ICacheKeyGenerator" company="Just Item">
//      Copyright (c) 2005-2019 Just. All rights reserved.
// </copyright>
// <editor>杨鹏</editor>
// <created-date>2019-09-17 15:13:00</created-date>
// -----------------------------------------------------------------

namespace Galaxy.Caching
{
    /// <summary>
    /// 定义缓存键生成功能
    /// </summary>
    public interface ICacheKeyGenerator
    {
        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        string GetKey(params object[] args);
    }
}