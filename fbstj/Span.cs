using System;
using System.Collections.Generic;

namespace fbstj
{
    public struct Span<T> : IEquatable<Span<T>>
        where T : IComparable<T>
    {
        #region static
        /// <summary>Finds the intersection between two Spans</summary>
        public static Span<T> Intersect(Span<T> a, Span<T> b)
        {
            if (a.Equals(b)) return a;
            if (!a.Intersects(b)) return default(Span<T>);
            if (a.Contains(b)) return b;
            if (b.Contains(a)) return a;
            return new Span<T>(Max(a.Minimum, b.Minimum), Min(a.Maximum, b.Maximum));
        }

        /// <summary>Finds the union (if any) between two Spans</summary>
        public static Span<T> Union(Span<T> a, Span<T> b)
        {
            if (!a.Intersects(b))
                return default(Span<T>);
            return new Span<T>(Min(a.Minimum, b.Minimum), Max(a.Maximum, b.Maximum));
        }

        /// <summary>A helper method for finding the maximum value</summary>
        public static T Max(params T[] args)
        {
            T max = args[0];
            foreach (T t in args)
                if (max.CompareTo(t) < 0)
                    max = t;
            return max;
        }
        /// <summary>A helper method for finding the minimum value</summary>
        public static T Min(params T[] args)
        {
            T min = args[0];
            foreach (T t in args)
                if (min.CompareTo(t) > 0)
                    min = t;
            return min;
        }
        #endregion

        #region state
        /// <summary>The smallest value (inclusive) that exist in this span</summary>
        public readonly T Minimum;
        /// <summary>The largest value (inclusive) that exist in this span</summary>
        public readonly T Maximum;

        public Span(T a, T b)
            : this()
        {
            Maximum = Max(a, b);
            Minimum = Min(a, b);
        }
        #endregion

        #region tests
        /// <summary>The span contains no points between its maximum and minimum values</summary>
        public bool Empty { get { return Maximum.CompareTo(Minimum) == Minimum.CompareTo(Maximum); } }
        /// <summary>Finds if the passed value lies between the maximum and minimum values</summary>
        public bool Contains(T t) { return Maximum.CompareTo(t) >= 0 && Minimum.CompareTo(t) <= 0; }
        /// <summary>Finds if the passed span lies between the maximum and minimum values</summary>
        public bool Contains(Span<T> sp) { return Contains(sp.Minimum) && Contains(sp.Maximum); }
        /// <summary>Finds if there is overlap between this and the passed spans</summary>
        public bool Intersects(Span<T> s) { return s.Contains(this) || Contains(s.Maximum) || Contains(s.Minimum); }
        /// <summary>Finds if the passed span and this one have equal maximum and minimums</summary>
        bool IEquatable<Span<T>>.Equals(Span<T> other) { return Contains(other) && other.Contains(this); }
        #endregion

        #region overrides
        public override bool Equals(object obj) { return Equals((Span<T>)obj); }
        public override int GetHashCode() { return base.GetHashCode(); }
        /// <summary>Returns the mathematical representation of the span</summary>
        public override string ToString() { return "[" + Minimum + ".." + Maximum + "]"; }
        #endregion
    }
}
