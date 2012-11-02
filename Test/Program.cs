using System;
using System.Linq;
using Algebra.Extensions;
using Algebra.Permutations;

namespace Algebra
{
	class Program
	{
		static void Main(string[] args)
		{
			TestPerms();
			TestBasePermPowersets();
			while (true) ;
		}
		static void TestBasePerms()
		{
			string s;
			Permutation p, q;

			s = "(1 10 3)(4 5 6)";
			Console.WriteLine(s);
			p = Permutation.Parse(s);
			Console.WriteLine(":= " + p);
			Console.WriteLine("-T: " + string.Join("", p.Transpositions()));
			Console.WriteLine("?= " + p.Equals(p));
			q = new Permutation { p.Inverse() };
			Console.WriteLine("-1: " + q);
			Console.WriteLine("-1? " + q.Equals(p.Inverse()));
			Console.WriteLine();

			s = "(1 10 3)(4 5 6)(7 1 3)";
			Console.WriteLine(s);
			p = Permutation.Parse(s);
			Console.WriteLine(":= " + p);
			Console.WriteLine("-1? " + p.Inverse());
			Console.WriteLine();

			s = "(1 10 3)(4 5 6)(1 4 6 2)";
			Console.WriteLine(s);
			p = Permutation.Parse(s);
			Console.WriteLine(":= " + p);
			Console.WriteLine("-1? " + p.Inverse());
			Console.WriteLine();

			s = "(1 3)(1 10)(4 6)(4 5)";
			Console.WriteLine(s);
			p = Permutation.Parse(s);
			Console.WriteLine(":= " + p);
			var s_trans = string.Join("", p.Transpositions());
			Console.WriteLine("-T: " + s_trans);
			Console.WriteLine(p.Equals(Permutation.Parse(s_trans)));
			Console.WriteLine(p.Equals(p.Transpositions()));
			Console.WriteLine(p.Equals(Permutation.Parse(string.Join("", p.Transpositions()))));
			Console.WriteLine();

			p = new Permutation { "(1 2 3)" };
			Console.WriteLine(p);
			p.Add("(4 2 5)");
			Console.WriteLine(p);
			p.Add(Permutation.Parse("(2 1)(1 2)"));
			Console.WriteLine(p);
			Console.WriteLine();

			p = new Permutation { "(1 10 3)", "(4 5 6)" };
			Console.WriteLine(":= " + p);
			Console.WriteLine("|p| " + p.Orbit.Count);
			p.Add("(10 3)");
			Console.WriteLine(":= " + p);
			Console.WriteLine("|p| " + p.Orbit.Count);
			Console.WriteLine();

			p = new Permutation { "(1 2 3)" };
			Console.WriteLine(":= " + p);
			Console.WriteLine("-1: " + p.Inverse());
			p.Add(p.Inverse());
			Console.WriteLine("0: " + p);
			Console.WriteLine("=? " + p.Equals(Permutation.Identity));
		}
		static void TestBasePermPowersets()
		{
			Console.WriteLine("Powersets");
			foreach (var p in EnumerableExtensions.PermutationsOf(4))
				Console.WriteLine(new Permutation { p });
		}

		static void TestPerm<T>(IPermutable<T> p)
		{
			// ToString
			Console.WriteLine(": " + p);
			// is identity?
			Console.WriteLine("identity? " + p.IsIdentity());
			// is transposition?
			Console.WriteLine("transposition? " + p.IsTransposition());
			// is cycle?
			Console.WriteLine("cycle? " + p.IsCycle());
			// number of cycles
			if (p.CountCycles() > 1)
			{
				Console.WriteLine("cycles: " + p.CountCycles());
				Console.WriteLine("\t" + string.Join(", ", p.Cycles().Select(cy => "(" + string.Join(" ", cy.Orbit) + ")")));
			}
			var q = p.Inverse();
			Console.WriteLine("inverse: " + q);
			Console.WriteLine("inverse-equals? " + q.Equals(p.Inverse()));
			Console.WriteLine("inverse? "); // TODO: addable?
			Console.WriteLine();
		}

		static void TestPerms()
		{
			TestPerm(new Permutation(1));
			TestPerm(new Cycle<uint>(1, 2));
			TestPerm(new Permutation { "(1 2 3)" });
			TestPerm(new Permutation { "(1 2 3)", "(4 5 6)" });
			TestPerm(Permutation.Parse("(1 2 3)(4 5 6)"));
			TestPerm(new Cycle<byte>(1, 2, 3, 4, 5));
			TestPerm(Cycle<bool>.Parse("(1 2 3)"));
		}
	}
}
