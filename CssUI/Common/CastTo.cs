﻿using System;
using System.Linq.Expressions;

namespace CssUI
{
    /* 
     * Source: 
     * https://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int 
     * https://stackoverflow.com/a/23391746 
     */

    /// <summary>
    /// Class to cast to type <see cref="T"/>
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    public static class CastTo<T>
    {
        /// <summary>
        /// Casts <see cref="S"/> to <see cref="T"/>.
        /// This does not cause boxing for value types.
        /// Useful in generic methods.
        /// </summary>
        /// <typeparam name="S">Source type to cast from. Usually a generic type.</typeparam>
        public static T From<S>(S s)
        {
            return Cache<S>.caster(s);
        }

        public static T From(Type originalType, object value)
        {
            var p = Expression.Parameter(originalType);
            var c = Expression.ConvertChecked(p, typeof(T));
            return Expression.Lambda<Func<object, T>>(c, p).Compile().Invoke(value);
        }

        private static class Cache<S>
        {
            public static readonly Func<S, T> caster = Get();

            private static Func<S, T> Get()
            {
                var p = Expression.Parameter(typeof(S));
                var c = Expression.ConvertChecked(p, typeof(T));
                return Expression.Lambda<Func<S, T>>(c, p).Compile();
            }
        }
    }
}