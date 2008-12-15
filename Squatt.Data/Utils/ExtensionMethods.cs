using System.Collections.Generic;

namespace Piec.Info.Squatt.Data.Utils
{
    /// <summary>
    /// Some extension methods to .NET classes.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets first element of a list.
        /// </summary>
        /// <typeparam name="T">List type.</typeparam>
        /// <param name="list">List to get element from.</param>
        /// <returns>The first element from the list.</returns>
        public static T GetFirst<T>(this List<T> list)
        {
            if (list.Count > 0)
            {
                return list[0];
            }
         
            return default(T);
        }

        /// <summary>
        /// Gets last element of a list.
        /// </summary>
        /// <typeparam name="T">List type.</typeparam>
        /// <param name="list">List to get element from.</param>
        /// <returns>The last element from the list.</returns>
        public static T GetLast<T>(this List<T> list)
        {
            int count = list.Count;
            if (count > 0)
            {
                return list[count - 1];
            }

            return default(T);
        }
    }
}