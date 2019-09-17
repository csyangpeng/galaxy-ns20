// -----------------------------------------------------------------
// <copyright file="JsonExtensions" company="Just Item">
//      Copyright (c) 2005-2019 Just. All rights reserved.
// </copyright>
// <editor>杨鹏</editor>
// <created-date>2019-09-17 15:03:21</created-date>
// -----------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Galaxy.Json
{
    /// <summary>
    /// Json辅助扩展操作
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// 将对象转换为JSON字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <param name="camelCase">是否小写名称</param>
        /// <param name="indented"></param>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (camelCase)
            {
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}