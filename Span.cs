using System;

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
		#region static
		/// <summary>Finds the intersection between two overlapping span</summary>
		public static Span<T> Intersect(Span<T> a, Span<T> b)
		{
			if (!a.Valid || !b.Valid || !a.Intersects(b))
				return default(Span<T>);

			if (a.Contains(b)) return b;
			if (b.Contains(a)) return a;

			return new Span<T>(Max(a.Minimum, b.Minimum), Min(a.Maximum, b.Maximum));
		}

		/// <summary>Finds the union of two overlapping spans</summary>
		public static Span<T> Union(Span<T> a, Span<T> b)
		{
			if (!a.Valid || !b.Valid || !a.Intersects(b))
				return default(Span<T>);

			return new Span<T>(Max(a.Maximum, b.Maximum), Min(a.Minimum, b.Minimum));
		}

		/// <summary>Finds the largest point in a selection</summary>
		public static T Max(params T[] points)
		{
			T max = points[0];
			foreach (T p in points)
				if (max.CompareTo(p) < 0)
					max = p;
			return max;
		}

		/// <summary>Finds the smallest point in a selection</summary>
		public static T Min(params T[] points)
		{
			T min = points[0];
			foreach (T p in points)
				if (min.CompareTo(p) > 0)
					min = p;
			return min;
		}
		#endregion

		#region state
		/// <summary>The lower bound</summary>
		public T Minimum;

		/// <summary>The upper bound</summary>
		public T Maximum;

		public Span(params T[] points) { Maximum = Max(points); Minimum = Min(points); }

		/// <summary>There are no points between this one and the previous</summary>
		public bool Empty { get { return Maximum.CompareTo(Minimum) == Minimum.CompareTo(Maximum); } }

		/// <summary>The Minimum is less than the Maximum</summary>
		public bool Valid { get { return Maximum.CompareTo(Minimum) > Minimum.CompareTo(Maximum); } }
		#endregion

		#region methods
		/// <summary>Tests for point containment</summary>
		public bool Contains(T item) { if (!Valid) return false; return Maximum.CompareTo(item) >= 0 && Minimum.CompareTo(item) <= 0; }

		#region span tests
		/// <summary>Tests for span containment</summary>
		public bool Contains(Span<T> sp) { if (!sp.Valid) return false; return Contains(sp.Minimum) && Contains(sp.Maximum); }

		/// <summary>Tests for span interesection</summary>
		public bool Intersects(Span<T> sp) { return sp.Contains(this) || Contains(sp.Minimum) || Contains(sp.Maximum); }

		/// <summary>Tests for span equality</summary>
		public bool Equals(Span<T> sp) { return Contains(sp) && sp.Contains(this); }
		#endregion

		/// <summary>The mathematical representation of an interval: [min..max]</summary>
		public override string ToString() { return "[" + Minimum + ".." + Maximum + "]"; }
		#endregion
	}
}
