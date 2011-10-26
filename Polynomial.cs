using System;
using System.Collections.Generic;

namespace fbstj
{
	public struct Polynomial
	{
		public struct Term
		{
			public readonly double Coefficient;
			public readonly int Exponent;
			public Term(int n, double co) { Exponent = n; Coefficient = co; }

			public double Evaluate(double at) { return Coefficient * Math.Pow(at, Exponent); }
		}

		public static Polynomial operator +(Polynomial p, Polynomial q) { return Sum(p, q); }

		public static Polynomial Sum(params Polynomial[] args)
		{
			var v = new Polynomial();
			foreach(var arg in args)
				foreach(var term in arg)
					v[term.Exponent] += term.Coefficient;
			return v;
		}

		public static Polynomial operator *(double a, Polynomial p) { return new Polynomial(0, a) * p; }

		public static Polynomial operator *(Polynomial p, Polynomial q)
		{
			var v = new Polynomial();
			foreach(var x in p)
				foreach(var y in q)
					v[x.Exponent + y.Exponent] += x.Coefficient * y.Coefficient;
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
			get { _init(); return _co.ContainsKey(i) ? _co[i] : 0; }
			set
			{
				_init();
				if(_co.ContainsKey(i))
					_co[i] = value;
				else
					_co.Add(i, value);
			}
		}

		public IEnumerator<Term> GetEnumerator()
		{
			_init();
			var terms = new List<Term>();
			foreach(var term in _co)
				terms.Add(new Term(term.Key, term.Value));
			return terms.GetEnumerator();
		}

		/// <summary>
		/// Evaluate the equation at a particular value of 'x'
		/// </summary>
		/// <param name="at">The value of 'x' to evaluate at</param>
		public double Evaluate(double at)
		{
			double y = 0;
			foreach(var term in this)
				y += term.Evaluate(at);
			return y;
		}

		public override string ToString()
		{
			var terms = new List<string>();
			foreach(var term in this)
			{
				string x;
				if(term.Coefficient == 0)
					continue;
				switch(term.Exponent)
				{
					case 0:
						x = "";
						break;
					case 1:
						x =  "x";
						break;
					default:
						x = "x^" + term.Exponent;
						break;
				}
				terms.Add(term.Coefficient + x);
			}
			return String.Join(" + ", terms.ToArray());
		}
	}
}

