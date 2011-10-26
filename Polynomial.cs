using System;
using System.Collections.Generic;

namespace fbstj
{
	public struct Polynomial
	{
		public static Polynomial operator +(Polynomial p, Polynomial q)
		{
			var v = new Polynomial();
			p._init(); q._init();
			foreach(var x in p._co)
				v[x.Key] = x.Value;
			foreach(var y in q._co)
				v[y.Key] += y.Value;
			return v;
		}

		private Dictionary<int,double> _co;

		private void _init() { if(_co == null) _co = new Dictionary<int,double>(); }

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
			get { _init(); return _co.ContainsKey(i)?_co[i]:0; }
			set
			{
				_init();
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
			_init();
			double y = 0;
			foreach(var term in _co)
				y += Math.Pow(at, term.Key) * term.Value;
			return y;
		}

		public override string ToString()
		{
			_init();
			var terms = new List<string>();
			foreach(var term in _co)
			{
				string x;
				if(term.Value == 0)
					continue;
				switch(term.Key)
				{
					case 0:
						x = "";
						break;
					case 1:
						x =  "x";
						break;
					default:
						x = "x^" + term.Key;
						break;
				}
				terms.Add(term.Value + x);
			}
			return String.Join(" + ", terms.ToArray());
		}
	}
}

