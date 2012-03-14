using System;
using System.Collections.Generic;

namespace fbstj
{
	public class Polynomial
	{
		public struct Term
		{
			public readonly double Coefficient;
			public readonly double Exponent;
			public Term(double n, double co) { Exponent = n; Coefficient = co; }

			public double Evaluate(double at) { return Coefficient * Math.Pow(at, Exponent); }
			public override string ToString()
			{
				string x;
				if (Exponent == 0)
					x = "";
				else if (Exponent == 1)
					x = "x";
				else
					x = "x^" + Exponent;
				return (Coefficient == 1) ? ((Exponent == 0) ? "1" : x) : (Coefficient + x);
			}
		}

		public static Polynomial operator +(Polynomial p, Polynomial q) { return Sum(p, q); }

		public static Polynomial Sum(params Polynomial[] args)
		{
			var v = new Polynomial();
			foreach (var arg in args)
				foreach (var term in arg)
					v[term.Exponent] += term.Coefficient;
			return v;
		}

		public static Polynomial operator *(double a, Polynomial p) { return Polynomial.From(a) * p; }

		public static Polynomial operator *(Polynomial p, Polynomial q)
		{
			var v = new Polynomial();
			foreach (var x in p)
				foreach (var y in q)
					v[x.Exponent + y.Exponent] += x.Coefficient * y.Coefficient;
			return v;
		}

		public static Polynomial From(params double[] coefficients)
		{
			var v = new Polynomial();
			for (int i = 0; i < coefficients.Length; i++)
				v[i] = coefficients[i];
			return v;
		}

		private readonly Dictionary<double, double> _co = new Dictionary<double, double>();

		public double this[double i]
		{
			get { return _co.ContainsKey(i) ? _co[i] : 0; }
			set { if (_co.ContainsKey(i)) _co[i] = value; else _co.Add(i, value); }
		}

		public IEnumerator<Term> GetEnumerator()
		{
			var terms = new List<Term>();
			foreach (var term in _co)
				if (term.Value != 0)
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
			foreach (var term in this)
				y += term.Evaluate(at);
			return y;
		}

		public override string ToString()
		{
			var terms = new List<string>();
			foreach (var term in this)
				terms.Add(term.ToString());
			return String.Join(" + ", terms.ToArray());
		}
	}
}

