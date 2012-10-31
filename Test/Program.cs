﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Algebra.Permutations;
using Algebra.Extensions;

namespace Algebra
{
	class Program
	{
		static void Main(string[] args)
		{
			Permutation p;

			Console.WriteLine("(1 10 3)(4 5 6)");
			p = Permutation.Parse("(1 10 3)(4 5 6)");
			Console.WriteLine(p);
			Console.WriteLine(string.Join("", p.Transpositions()));
			Console.WriteLine(p.Equals(p));
			Console.WriteLine(p.Inverse());

			Console.WriteLine("(1 10 3)(4 5 6)(7 1 3) = (3 7 10)(4 5 6)");
			p = Permutation.Parse("(1 10 3)(4 5 6)(7 1 3)");
			Console.WriteLine(p);
			Console.WriteLine(p.Inverse());

			Console.WriteLine("(1 10 3)(4 5 6)(1 4 6 2) = (1 5 6 2 10 3)");
			p = Permutation.Parse("(1 10 3)(4 5 6)(1 4 6 2)");
			Console.WriteLine(p);
			Console.WriteLine(p.Inverse());

			Console.WriteLine("(1 10 3)(4 5 6)");
			p = Permutation.Parse("(1 10 3)(4 5 6)");
			Console.WriteLine(p);
			Console.WriteLine(p.Equals(Permutation.Parse("(1 3)(1 10)(4 6)(4 5)")));
			Console.WriteLine(p.Equals(Permutation.Parse(string.Join("", p.Transpositions()))));
			Console.WriteLine(p.Equals(p.Transpositions()));

			p = new Permutation(Cycle.Parse("(1 2 3)"));
			Console.WriteLine(p);
			p = new Permutation(Permutation.Parse("(1 2 3)(4 2 5)"));
			Console.WriteLine(p);
			p = new Permutation(Cycle.Parse("(1 2 3)"), Cycle.Parse("(4 2 5)"), Permutation.Parse("(2 1)(1 2)"));
			Console.WriteLine(p);
			p = new Permutation(Cycle.Parse("(1 2 3)"), Cycle.Parse("(4 2 5)"), Permutation.Parse("(2 1)(1 2)"));
			Console.WriteLine(p);
			p = Permutation.Parse("(1 10 3)(4 5 6)(1 10)");
			Console.WriteLine(p);
			Console.WriteLine(p.Orbit.Count);
			p.Trim();
			Console.WriteLine(p);
			Console.WriteLine(p.Orbit.Count);

			Console.WriteLine("Powersets");
			foreach (var q in EnumerableExtensions.PermutationsOf(4))
				Console.WriteLine(new Permutation(q));

			var set = new HashSet<int> { 1, 2, 3 };

			while (true) ;
		}
	}
}
