using System;
using System.Collections.Generic;

namespace fbstj
{
	public struct Polynomial
	{
		private Dictionary<int,double> _co;

		public Polynomial(int offset, params double[] coefficients)
		{
			_co = new Dictionary<int,double>();
			foreach(double co in coefficients)
				this[offset++] = co;
		}

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

		public double Evaluate(double at)
		{
			double y = 0;
			foreach(var term in _co)
				y += Math.Pow(at, term.Key) * term.Value;
			return y;
		}
	}
}

