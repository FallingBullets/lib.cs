using System;
using System.Collections.Generic;

/* Span<T> - span/range/interval structure.
 *
 * Allows you to do some simple interval algebra.
 * Similar to DateTime/TimeSpan but for any IComparable
 */
namespace fbstj
{
    public struct Span<T> : IEquatable<Span<T>>
        where T : IComparable<T>
    {
        /// <summary>The lower bound</summary>
        public T Minimum;
        /// <summary>The upper bound</summary>
        public T Maximum;

        public Span(T a, T b)
        {
            Maximum = (a.CompareTo(b) < 0) ? b : a;
            Minimum = (a.CompareTo(b) < 0) ? a : b;
        }

        public bool Empty { get { return Maximum.CompareTo(Minimum) == Minimum.CompareTo(Maximum); } }
        public bool Contains(T item) { return Maximum.CompareTo(item) >= 0 && Minimum.CompareTo(item) <= 0; }
        public bool Contains(Span<T> sp) { return Contains(sp.Minimum) && Contains(sp.Maximum); }
        public bool Intersects(Span<T> sp) { return sp.Contains(this) || Contains(sp.Minimum) || Contains(sp.Maximum); }
        public bool Equals(Span<T> sp) { return Contains(sp) && sp.Contains(this); }
        public override string ToString() { return "[" + Minimum + ".." + Maximum + "]"; }
    }
}
