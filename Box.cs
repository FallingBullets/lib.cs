using System;
using System.Collections.Generic;

// reqires Span.cs

namespace fbstj
{
	public struct Box<Tx, Ty>
		where Tx : IComparable<Tx>
		where Ty : IComparable<Ty>
	{
		public static Box<Tx, Ty> Get(Tx x1, Ty y1, Tx x2, Ty y2)
		{
			Box<Tx,Ty> b = default(Box<Tx,Ty>);
			b.X = new Span<Tx>(x1, x2);
			b.Y = new Span<Ty>(y1, y2);
			return b;
		}

		public static Box<Tx,Ty> Bounds(params Box<Tx,Ty>[] boxes)
		{
			Box<Tx,Ty> b = boxes[0];
			foreach(var box in boxes)
			{
				b.X = new Span<Tx>(b.X.Maximum, box.X.Maximum, b.X.Minimum, box.X.Minimum);
				b.Y = new Span<Ty>(b.Y.Maximum, box.Y.Maximum, b.Y.Minimum, box.Y.Minimum);
			}
			return b;
		}

		private Span<Tx> X;
		private Span<Ty> Y;

		public bool Empty { get { return X.Empty && Y.Empty; } }

		public bool Contains(Tx x, Ty y) { return X.Contains(x) && Y.Contains(y); }

		public bool Intersects(Box<Tx,Ty> box) { return X.Intersects(box.X) && Y.Intersects(box.Y); }

		public bool Contains(Box<Tx,Ty> box) { return X.Contains(box.X) && Y.Contains(box.Y); }

		public override string ToString(){ return "{x:" + X + ",y:" + Y + "}"; }
	}
}
