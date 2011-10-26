using System;
using System.Collections.Generic;

namespace fbstj
{
	public struct Polynomial
	{
		private Dictionary<int,double> _co;

		/// <summary>
		/// Constructs a polynomial starting from a non-zero term
		/// </summary>
		/// <param name="start">The index of the first power of x in the polynomial</param>
		/// <param name="coefficients">The coefficients a_i</param>
		public Polynomial(int offset, params double[] coefficients)
		{
			_co = new Dictionary<int,double>();
			foreach(double co in coefficients)
				this[offset++] = co;
		}

		/// <summary>
		/// Constructs a polynomial a_0 + (a_1 * x) + (a_2 * x^2) + ... + (a_n * x^n)
		/// </summary>
		/// <param name="coefficients">The coefficients a_i</param>
		public Polynomial(params double[] coefficients) : this(0, coefficients) {  }

		public double this[int i]
		{
			get { return _co[i]; }
			set
			{
				if(_co.ContainsKey(i))
					_co[i] = value;
				else
					_co.Add(i, value);
			}
		}

		/// <summary>
		/// Evaluate the equation at a particular value of 'x'
		/// </summary>
		/// <param name="at">The value of 'x' to evaluate at</param>
		public double Evaluate(double at)
		{
			double y = 0;
			foreach(var term in _co)
				y += Math.Pow(at, term.Key) * term.Value;
			return y;
		}
	}
}

