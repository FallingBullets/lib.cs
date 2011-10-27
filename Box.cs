using System;
using System.Collections.Generic;

// reqires Span.cs

/* Span<T> - span/range/interval structure.
 *
 * Allows you to do some simple interval algebra.
 * Similar to DateTime/TimeSpan but for any IComparable
 */
namespace fbstj
{
	public struct Box<T>
		where T : IComparable<T>
	{
		public static Box<T> Get(T x1, T y1, T x2, T y2)
		{
			Box<T> b = default(Box<T>);
			b.X = new Span<T>(x1, x2);
			b.Y = new Span<T>(y1, y2);
			return b;
		}

		public static Box<T> Bounds(params Box<T>[] boxes)
		{
			Box<T> b = boxes[0];
			foreach(var box in boxes)
			{
				b.X = new Span<T>(b.X.Maximum, box.X.Maximum, b.X.Minimum, box.X.Minimum);
				b.Y = new Span<T>(b.Y.Maximum, box.Y.Maximum, b.Y.Minimum, box.Y.Minimum);
			}
			return b;
		}

		private Span<T> X, Y;

		public bool Empty { get { return X.Empty && Y.Empty; } }

		public bool Contains(T x, T y) { return X.Contains(x) && Y.Contains(y); }

		public bool Intersects(Box<T> box) { return X.Intersects(box.X) && Y.Intersects(box.Y); }

		public bool Contains(Box<T> box) { return X.Contains(box.X) && Y.Contains(box.Y); }

		public override string ToString(){ return "{x:" + X + ",y:" + Y + "}"; }
	}
}
