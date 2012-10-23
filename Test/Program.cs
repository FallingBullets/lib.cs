using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Algebra.Extensions;

namespace Algebra
{
	class Program
	{
		static void Main(string[] args)
		{
			var set = SetExtensions.Set<int>(1, 2, 3);
			var power = set.Powerset();
			foreach (var tuple in set.TuplesOf(5))
				Console.WriteLine("[" + String.Join(",", tuple) + "]");
		}
	}
}
