using System;
using System.Collections.Generic;

namespace fbstj
{
	public class Polynomial
	{
		public struct Term
		{
			public readonly double Coefficient;
			public readonly int Exponent;
			public Term(int n, double co) { Exponent = n; Coefficient = co; }

			public double Evaluate(double at) { return Coefficient * Math.Pow(at, Exponent); }
			public override string ToString()
			{
				string x;
				switch(Exponent)
				{
					case 0:
						x = "";
						break;
					case 1:
						x =  "x";
						break;
					default:
						x = "x^" + Exponent;
						break;
				}
				return (Coefficient == 1) ? ((Exponent == 0) ? "1" : x) : (Coefficient + x);
			}
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

		public static Polynomial operator *(double a, Polynomial p) { return Polynomial.From(a) * p; }

		public static Polynomial operator *(Polynomial p, Polynomial q)
		{
			var v = new Polynomial();
			foreach(var x in p)
				foreach(var y in q)
					v[x.Exponent + y.Exponent] += x.Coefficient * y.Coefficient;
			return v;
		}

		public static Polynomial From(params double[] coefficients)
		{
			var v = new Polynomial();
			for(int i = 0; i < coefficients.Length; i++)
				v[i] = coefficients[i];
			return v;
		}

		private readonly Dictionary<int,double> _co = new Dictionary<int,double>();

		public double this[int i]
		{
			get { return _co.ContainsKey(i) ? _co[i] : 0; }
			set { if(_co.ContainsKey(i)) _co[i] = value; else _co.Add(i, value); }
		}

		public IEnumerator<Term> GetEnumerator()
		{
			var terms = new List<Term>();
			foreach(var term in _co)
				if(term.Value != 0)
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
				terms.Add(term.ToString());
			return String.Join(" + ", terms.ToArray());
		}
	}
}

