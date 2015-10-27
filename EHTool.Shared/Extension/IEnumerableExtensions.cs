﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHTool.Shared.Extension
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new EqualityComparer<T,V>(keySelector));
        }
    }


    public class EqualityComparer<T, V> : IEqualityComparer<T>
    {
        private Func<T, V> keySelector;

        public EqualityComparer(Func<T, V> keySelector)
        {
            this.keySelector = keySelector;
        }

        public bool Equals(T x, T y)
        {
            return EqualityComparer<V>.Default.Equals(keySelector(x), keySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return EqualityComparer<V>.Default.GetHashCode(keySelector(obj));
        }
    }
}
