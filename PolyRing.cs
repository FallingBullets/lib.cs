using System;
using System.Collections.Generic;
using System.Linq;

namespace fbstj
{
	public static class RingExtensions
	{
		public static Tv EvaluatePolynomial<Ti, Tv>(this IDictionary<Ti, Tv> _, Tv at, Func<Tv, Tv, Tv> add, Func<Tv, Tv, Tv> prod, Func<Tv, Ti, Tv> pow)
		{
			Tv y = default(Tv);
			foreach (var k in _.Keys)
				y = add(y, prod(_[k], pow(at, k)));
			return y;
		}
	}
}

