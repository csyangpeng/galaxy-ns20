﻿// -----------------------------------------------------------------
// <copyright file="JsonHelper" company="Just Item">
//      Copyright (c) 2005-2019 Just. All rights reserved.
// </copyright>
// <editor>杨鹏</editor>
// <created-date>2019-09-17 15:26:58</created-date>
// -----------------------------------------------------------------

using System;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Galaxy.Extensions;


namespace Galaxy.Json
{
    /// <summary>
    /// JSON辅助操作类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 处理Json的时间格式为正常格式
        /// </summary>
        public static string JsonDateTimeFormat(string json)
        {
            json.CheckNotNullOrEmpty("json");
            json = Regex.Replace(json,
                @"\\/Date\((\d+)\)\\/",
                match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                });
            return json;
        }

        /// <summary>
        /// 把对象序列化成Json字符串格式
        /// </summary>
        /// <param name="object">Json 对象</param>
        /// <param name="camelCase">是否小写名称</param>
        /// <param name="indented"></param>
        /// <returns>Json 字符串</returns>
        public static string ToJson(object @object, bool camelCase = false, bool indented = false)
        {
            @object.CheckNotNull("@object");

            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (camelCase)
            {
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }
            string json = JsonConvert.SerializeObject(@object, settings);
            return JsonDateTimeFormat(json);
        }

        /// <summary>
        /// 把Json字符串转换为强类型对象
        /// </summary>
        public static T FromJson<T>(string json)
        {
            json.CheckNotNullOrEmpty("json");

            json = JsonDateTimeFormat(json);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
