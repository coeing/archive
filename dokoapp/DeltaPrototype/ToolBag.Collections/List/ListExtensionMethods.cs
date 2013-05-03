using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolBag.Collections.List
{
    public static class ListExtensionMethods
    {
        /// <summary>
        ///   Randomly shuffles a list.
        /// </summary>
        /// <typeparam name="T">Type of objects in list.</typeparam>
        /// <param name="list">List to shuffle.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
