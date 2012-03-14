using System.Collections.Generic;

namespace fbstj
{
	/// <summary>A binary opperation on T</summary>
	public delegate T BinOp<T>(T a, T b);
	/// <summary>The value of x 'to the power of' e</summary>
	public delegate T PowOp<T>(T x, int e);

	public struct PolynomialRing<T>
		where T : struct
	{
		private List<T> _co;

		/// <summary>Constructs an abstract polynomial</summary>
		public PolynomialRing(params T[] coefficients)
		{
			_co = new List<T>(coefficients);
		}

		/// <summary>The degree of the polynomial</summary>
		public int Degree { get { return _co.Count; } }

		/// <summary>The coefficient of a term in the polynomial</summary>
		public T this[int i]
		{
			get { return _co[i]; }
			set
			{
				if (_co.Count > i)
					_co[i] = value;
				else
					_co.Add(value);
			}
		}

		/// <summary>Evaluate the polynomial at a specific point</summary>
		/// add and prod are the binary operators which form a Polynomial Ring over `T`
		/// pow is a function that takes a T to a positive power
		public T Evaluate(T at, BinOp<T> add, BinOp<T> prod, PowOp<T> pow)
		{
			T y = default(T);
			for (int i = 0; i < Degree; i++)
				y = add(y, prod(this[i], pow(at, i)));
			return y;
		}
	}
}

