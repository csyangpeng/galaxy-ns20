// -----------------------------------------------------------------
// <copyright file="ExpressionCacheKeyGenerator" company="Just Item">
//      Copyright (c) 2005-2019 Just. All rights reserved.
// </copyright>
// <editor>杨鹏</editor>
// <created-date>2019-09-17 15:12:24</created-date>
// -----------------------------------------------------------------

using System.Linq;
using System.Linq.Expressions;

using Galaxy.Collections;


namespace Galaxy.Caching
{
    /// <summary>
    /// 表达式缓存键生成器
    /// </summary>
    public class ExpressionCacheKeyGenerator : ICacheKeyGenerator
    {
        private readonly Expression _expression;

        /// <summary>
        /// 初始化一个<see cref="ExpressionCacheKeyGenerator"/>类型的新实例
        /// </summary>
        public ExpressionCacheKeyGenerator(Expression expression)
        {
            _expression = expression;
        }

        #region Implementation of ICacheKeyGenerator

        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public string GetKey(params object[] args)
        {
            Expression expression = _expression;
            expression = Evaluator.PartialEval(expression, CanBeEvaluatedLocally);
            expression = LocalCollectionExpressionVisitor.Rewrite(expression);
            string key = expression.ToString();
            return key + args.ExpandAndToString();
        }

        #endregion

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Parameter)
            {
                return false;
            }
            if (typeof(IQueryable).IsAssignableFrom(expression.Type))
            {
                return false;
            }
            return true;
        }
    }
}

