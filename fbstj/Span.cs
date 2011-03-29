using System;
using System.Collections.Generic;

namespace fbstj
{
    public struct Span<T> : ISpan<T>, IContainer<T>, IEquatable<Span<T>>
        where T : IComparable<T>
    {
        #region static
        public static Span<T> Intersect(Span<T> a, Span<T> b)
        {
            Span<T> r = default(Span<T>);
            if (a.Equals(b)) return a;
            if (!a.Intersects(b)) return r;
            if (a.Contains(b)) return b;
            if (b.Contains(a)) return a;

            r.Minimum = a.Minimum.CompareTo(b.Minimum) > 0 ? a.Minimum : b.Minimum;
            r.Maximum = a.Maximum.CompareTo(b.Maximum) > 0 ? a.Maximum : b.Maximum;

            return r;
        }
        #endregion

        #region state
        public T Minimum { get; private set; }
        public T Maximum { get; private set; }

        public Span(T a, T b) : this()
        {
            if (a.CompareTo(b) == 0)
                Maximum = Minimum = a;
            else if (a.CompareTo(b) < 0) { Minimum = a; Maximum = b; }
            else { Maximum = a; Maximum = b; }
        }
        #endregion

        #region tests
        public bool Empty { get { return Maximum.CompareTo(Minimum) == Minimum.CompareTo(Maximum); } }
        public bool Contains(T t) { return Maximum.CompareTo(t) >= 0 && Minimum.CompareTo(t) <= 0; }
        public bool Contains(Span<T> sp) { return Contains(sp.Minimum) && Contains(sp.Maximum); }
        public bool Intersects(Span<T> s) { return s.Contains(this) || Contains(s.Maximum) || Contains(s.Minimum); }
        public bool Equals(Span<T> other) { return Contains(other) && other.Contains(this); }
        #endregion

        #region overrides
        public override bool Equals(object obj) { return Equals((Span<T>)obj); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return "[" + Minimum + ".." + Maximum + "]"; }
        #endregion
    }

    /// <summary>
    /// A interval between two 'values'
    /// </summary>
    /// <typeparam name="T">The orderable value type of the interval.</typeparam>
    interface ISpan<T>
        where T : IComparable<T>
    {
        /// <summary>The 'smaller' of the 'points'</summary>
        T Minimum { get; }
        /// <summary>The 'larger' of the 'points'</summary>
        T Maximum { get; }
    }
}
