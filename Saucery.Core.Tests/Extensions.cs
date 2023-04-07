using System;
using System.Collections;
using System.Reflection;

namespace Saucery.Core.Tests;

internal static class Extensions
{
    public static bool IsStatic(this Type type) => type.GetTypeInfo().IsAbstract && type.GetTypeInfo().IsSealed;

    public static bool HasAttribute<T>(this ICustomAttributeProvider attributeProvider, bool inherit) => attributeProvider.IsDefined(typeof(T), inherit);

    public static bool HasAttribute<T>(this Type type, bool inherit) => ((ICustomAttributeProvider)type.GetTypeInfo()).HasAttribute<T>(inherit);

    public static T[] GetAttributes<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : class => (T[])attributeProvider.GetCustomAttributes(typeof(T), inherit);

    public static T[] GetAttributes<T>(this Assembly assembly) where T : class => assembly.GetAttributes<T>(inherit: false);

    public static T[] GetAttributes<T>(this Type type, bool inherit) where T : class => ((ICustomAttributeProvider)type.GetTypeInfo()).GetAttributes<T>(inherit);

    public static IEnumerable Skip(this IEnumerable enumerable, long skip)
    {
        var iterator = enumerable.GetEnumerator();
        using (iterator as IDisposable)
        {
            while (skip-- > 0)
            {
                if (!iterator.MoveNext())
                    yield break;
            }

            while (iterator.MoveNext())
            {
                yield return iterator.Current;
            }
        }
    }
}
