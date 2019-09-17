// -----------------------------------------------------------------
// <copyright file="DistributedCacheExtensions" company="Just Item">
//      Copyright (c) 2005-2019 Just. All rights reserved.
// </copyright>
// <editor>杨鹏</editor>
// <created-date>2019-09-17 15:01:30</created-date>
// -----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using Galaxy.Collections;
using Galaxy.Dependency;
using Galaxy.Exceptions;
using Galaxy.Extensions;
using Galaxy.Filter;
using Galaxy.Json;
using Galaxy.Reflection;


namespace Galaxy.Caching
{
    /// <summary>
    /// <see cref="IDistributedCache"/>扩展方法
    /// </summary>
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// 将对象存入缓存中
        /// </summary>
        public static void Set(this IDistributedCache cache, string key, object value, DistributedCacheEntryOptions options = null)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));

            string json = value.ToJsonString();
            if (options == null)
            {
                cache.SetString(key, json);
            }
            else
            {
                cache.SetString(key, json, options);
            }
        }

        /// <summary>
        /// 异步将对象存入缓存中
        /// </summary>
        public static async Task SetAsync(this IDistributedCache cache, string key, object value, DistributedCacheEntryOptions options = null)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));

            string json = value.ToJsonString();
            if (options == null)
            {
                await cache.SetStringAsync(key, json);
            }
            else
            {
                await cache.SetStringAsync(key, json, options);
            }
        }

        /// <summary>
        /// 将对象存入缓存中，使用指定时长
        /// </summary>
        public static void Set(this IDistributedCache cache, string key, object value, int cacheSeconds)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            cache.Set(key, value, options);
        }

        /// <summary>
        /// 异步将对象存入缓存中，使用指定时长
        /// </summary>
        public static Task SetAsync(this IDistributedCache cache, string key, object value, int cacheSeconds)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            return cache.SetAsync(key, value, options);
        }

        /// <summary>
        /// 获取指定键的缓存项
        /// </summary>
        public static TResult Get<TResult>(this IDistributedCache cache, string key)
        {
            string json = cache.GetString(key);
            if (json == null)
            {
                return default(TResult);
            }
            return json.FromJsonString<TResult>();
        }

        /// <summary>
        /// 异步获取指定键的缓存项
        /// </summary>
        public static async Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key)
        {
            string json = await cache.GetStringAsync(key);
            if (json == null)
            {
                return default(TResult);
            }
            return json.FromJsonString<TResult>();
        }

        /// <summary>
        /// 获取指定键的缓存项，不存在则从指定委托获取，并回存到缓存中再返回
        /// </summary>
        public static TResult Get<TResult>(this IDistributedCache cache, string key, Func<TResult> getFunc, DistributedCacheEntryOptions options = null)
        {
            TResult result = cache.Get<TResult>(key);
            if (!Equals(result, default(TResult)))
            {
                return result;
            }
            result = getFunc();
            if (Equals(result, default(TResult)))
            {
                return default(TResult);
            }
            cache.Set(key, result, options);
            return result;
        }

        /// <summary>
        /// 异步获取指定键的缓存项，不存在则从指定委托获取，并回存到缓存中再返回
        /// </summary>
        public static async Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key, Func<Task<TResult>> getAsyncFunc, DistributedCacheEntryOptions options = null)
        {
            TResult result = await cache.GetAsync<TResult>(key);
            if (!Equals(result, default(TResult)))
            {
                return result;
            }
            result = await getAsyncFunc();
            if (Equals(result, default(TResult)))
            {
                return default(TResult);
            }
            await cache.SetAsync(key, result, options);
            return result;
        }

        /// <summary>
        /// 获取指定键的缓存项，不存在则从指定委托获取，并回存到缓存中再返回
        /// </summary>
        public static TResult Get<TResult>(this IDistributedCache cache, string key, Func<TResult> getFunc, int cacheSeconds)
        {
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            return cache.Get<TResult>(key, getFunc, options);
        }

        /// <summary>
        /// 异步获取指定键的缓存项，不存在则从指定委托获取，并回存到缓存中再返回
        /// </summary>
        public static Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key, Func<Task<TResult>> getAsyncFunc, int cacheSeconds)
        {
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            return cache.GetAsync<TResult>(key, getAsyncFunc, options);
        }

        /// <summary>
        /// 将结果转换为缓存的列表，如缓存存在，直接返回，否则从数据源查询，并存入缓存中再返回
        /// </summary>
        /// <typeparam name="TSource">数据源的项数据类型</typeparam>
        /// <typeparam name="TResult">结果集的项数据类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">数据查询表达式</param>
        /// <param name="selector">数据筛选表达式</param>
        /// <param name="cacheSeconds">缓存时间：秒</param>
        /// <param name="keyParams">缓存键参数</param>
        /// <returns></returns>
        public static List<TResult> ToCacheList<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            return source.Where(predicate).ToCacheList(selector, cacheSeconds, keyParams);
        }

        /// <summary>
        /// 将结果转换为缓存的数组，如缓存存在，直接返回，否则从数据源查询，并存入缓存中再返回
        /// </summary>
        /// <typeparam name="TSource">数据源的项数据类型</typeparam>
        /// <typeparam name="TResult">结果集的项数据类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">数据查询表达式</param>
        /// <param name="selector">数据筛选表达式</param>
        /// <param name="cacheSeconds">缓存时间：秒</param>
        /// <param name="keyParams">缓存键参数</param>
        /// <returns></returns>
        public static TResult[] ToCacheArray<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            return source.Where(predicate).ToCacheArray(selector, cacheSeconds, keyParams);
        }

        /// <summary>
        /// 将结果转换为缓存的列表，如缓存存在，直接返回，否则从数据源查询，并存入缓存中再返回
        /// </summary>
        /// <typeparam name="TSource">源数据类型</typeparam>
        /// <typeparam name="TResult">结果集的项数据类型</typeparam>
        /// <param name="source">查询数据源</param>
        /// <param name="selector">数据筛选表达式</param>
        /// <param name="cacheSeconds">缓存的秒数</param>
        /// <param name="keyParams">缓存键参数</param>
        /// <returns>查询结果</returns>
        public static List<TResult> ToCacheList<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source, selector, keyParams);
            return cache.Get(key, () => source.Select(selector).ToList(), cacheSeconds);
        }

        /// <summary>
        /// 将结果转换为缓存的数组，如缓存存在，直接返回，否则从数据源查询，并存入缓存中再返回
        /// </summary>
        /// <typeparam name="TSource">源数据类型</typeparam>
        /// <typeparam name="TResult">结果集的项数据类型</typeparam>
        /// <param name="source">查询数据源</param>
        /// <param name="selector">数据筛选表达式</param>
        /// <param name="cacheSeconds">缓存的秒数</param>
        /// <param name="keyParams">缓存键参数</param>
        /// <returns>查询结果</returns>
        public static TResult[] ToCacheArray<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source, selector, keyParams);
            return cache.Get(key, () => source.Select(selector).ToArray(), cacheSeconds);
        }

        /// <summary>
        /// 将结果转换为缓存的列表，如缓存存在，直接返回，否则从数据源查询，并存入缓存中再返回
        /// </summary>
        /// <typeparam name="TSource">源数据类型</typeparam>
        /// <param name="source">查询数据源</param>
        /// <param name="cacheSeconds">缓存的秒数</param>
        /// <param name="keyParams">缓存键参数</param>
        /// <returns>查询结果</returns>
        public static List<TSource> ToCacheList<TSource>(this IQueryable<TSource> source, int cacheSeconds = 60, params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source.Expression, keyParams);
            return cache.Get(key, source.ToList, cacheSeconds);
        }

        /// <summary>
        /// 将结果转换为缓存的数组，如缓存存在，直接返回，否则从数据源查询，并存入缓存中再返回
        /// </summary>
        /// <typeparam name="TSource">源数据类型</typeparam>
        /// <param name="source">查询数据源</param>
        /// <param name="cacheSeconds">缓存的秒数</param>
        /// <param name="keyParams">缓存键参数</param>
        /// <returns>查询结果</returns>
        public static TSource[] ToCacheArray<TSource>(this IQueryable<TSource> source, int cacheSeconds = 60, params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source.Expression, keyParams);
            return cache.Get(key, source.ToArray, cacheSeconds);
        }

        private static string GetKey<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            params object[] keyParams)
        {
            IQueryable<TResult> query = source.Select(selector);
            return GetKey(query.Expression, keyParams);
        }

        private static string GetKey(Expression expression, params object[] keyParams)
        {
            string key;
            try
            {
                key = new ExpressionCacheKeyGenerator(expression).GetKey(keyParams);
            }
            catch (TargetInvocationException)
            {
                key = new StringCacheKeyGenerator().GetKey(keyParams);
            }
            return key.ToMd5Hash();
        }
    }
}
