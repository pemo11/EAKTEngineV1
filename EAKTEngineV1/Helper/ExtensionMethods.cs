// ============================================================
// File: ExtensionMethods.cs
// ============================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Enthält Extension Methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Erweitert Collections um eine ToObservable-Methode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> enumerable)
        {
            var col = new ObservableCollection<T>();
            foreach (var cur in enumerable)
            {
                col.Add(cur);
            }
            return col;
        }
    }
}
