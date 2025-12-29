using System;
using System.Linq.Expressions;

namespace CssUI;

/*
 * Source:
 * https://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int
 * https://stackoverflow.com/a/23391746
 */

/// <summary>
/// Provides non-boxing type conversion for generic contexts.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why this exists:</b> In non-generic code, direct casts like <c>(int)MyEnum.Value</c>
/// compile to simple IL instructions with zero allocations. However, in generic methods
/// where the type isn't known at compile time, you cannot write <c>(int)genericValue</c>
/// directly—the compiler doesn't know the generic type is convertible to <c>int</c>.
/// </para>
/// <para>
/// The only built-in option is casting through <c>object</c>: <c>(int)(object)genericValue</c>,
/// which causes boxing for value types (allocating on the heap).
/// </para>
/// <para>
/// <b>How it works:</b> This class uses compiled expression trees to create strongly-typed
/// delegate functions that perform direct conversions. The delegates are cached per type pair,
/// so compilation cost is paid only once. After the first call, performance is nearly
/// identical to a direct cast.
/// </para>
/// <para>
/// <b>Example:</b>
/// <code>
/// // ❌ Won't compile in generic context
/// public int GetValue&lt;TEnum&gt;(TEnum e) => (int)e;
///
/// // ❌ Compiles but boxes the value
/// public int GetValue&lt;TEnum&gt;(TEnum e) => (int)(object)e;
///
/// // ✅ No boxing — uses cached compiled delegate
/// public int GetValue&lt;TEnum&gt;(TEnum e) => CastTo&lt;int&gt;.From(e);
/// </code>
/// </para>
/// </remarks>
/// <typeparam name="T">Target type to cast to.</typeparam>
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
