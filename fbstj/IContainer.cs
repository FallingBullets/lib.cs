using System;

namespace fbstj
{
    /// <summary>
    /// A container of things.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IContainer<T>
    {
        /// <summary>Contains no items.</summary>
        bool Empty { get; }

        /// <summary>
        /// Contains the passed item
        /// </summary>
        /// <param name="t">The item to test</param>
        /// <returns>true if the item is contained in this object.</returns>
        bool Contains(T t);
    }
}