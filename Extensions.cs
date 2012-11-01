using System;
using System.Collections.Generic;
using System.Linq;
using Algebra.Permutations;

namespace Algebra.Extensions
{
	/// <summary>Properties of Sets and functions over sets</summary>
	public static class SetExtensions
	{
		#region Set logic
		/// <summary>Generates the intersection of the passed sets</summary>
		public static ISet<T> Intersection<T>(params ISet<T>[] sets)
		{
			var intersection = new SortedSet<T>(sets[0]);
			foreach (ISet<T> set in sets)
				intersection.IntersectWith(set);
			return intersection;
		}

		/// <summary>Generates the union of the passed sets</summary>
		public static ISet<T> Union<T>(params ISet<T>[] sets)
		{
			var union = new SortedSet<T>(sets[0]);
			foreach (ISet<T> set in sets)
				union.UnionWith(set);
			return union;
		}

		/// <summary>The passed sets partition this set</summary>
		public static bool Partitions<T>(this ISet<T> _, params ISet<T>[] sets)
		{
			return _.SetEquals(Union(sets))
				&& sets.All(a => sets.All(b => a.SetEquals(b) ? true : Intersection(a, b).Count == 0));
		}
		#endregion

		#region comparer
		public class Comparer<T> : IComparer<ISet<T>>
		{
			public int Compare(ISet<T> x, ISet<T> y)
			{
				return x.SetEquals(y) ? 0 : Union(x, y).Count;
			}
		}
		#endregion

		#region generators
		/// <summary>Lazy set generation</summary>
		public static ISet<T> Set<T>(params T[] items) { return new SortedSet<T>(items); }

		/// <summary>Generates the powerset</summary>
		public static ISet<ISet<T>> Powerset<T>(this ISet<T> _)
		{
			var s = new SortedSet<ISet<T>>(new Comparer<T>());
			((IEnumerable<T>)_).Powerset().All(item => s.Add(new SortedSet<T>(item)));
			return s;
		}
		#endregion
	}

	/// <summary>Properties of binary operations</summary>
	public static class OperationExtensions
	{
		#region Association
		/// <summary>a(bc) == (ab)c</summary>
		public static bool Associates<T>(this Func<T, T, T> _, T a, T b, T c) where T : IEquatable<T>
		{
			return _(a, _(b, c)).Equals(_(_(a, b), c));
		}
		#endregion

		#region Commutation
		/// <summary>ab == ba</summary>
		public static bool Commutes<T>(this Func<T, T, T> _, T a, T b) where T : IEquatable<T>
		{
			return _(a, b).Equals(_(b, a));
		}
		#endregion

		#region Distribution
		/// <summary>a * (b + c) = (a * c) + )b * c)</summary>
		public static bool LeftDistributes<T>(this Func<T, T, T> _, Func<T, T, T> over, T a, T b, T c) where T : IEquatable<T>
		{
			return _(a, over(b, c)).Equals(over(_(a, b), _(a, c)));
		}

		/// <summary>(a + b) * c = (a * c) + (b * c)</summary>
		public static bool RightDistributes<T>(this Func<T, T, T> _, Func<T, T, T> over, T a, T b, T c) where T : IEquatable<T>
		{
			return _(over(a, b), c).Equals(over(_(a, c), _(b, c)));
		}

		/// <summary>Left and Right distributive</summary>
		public static bool Distributes<T>(this Func<T, T, T> _, Func<T, T, T> over, T a, T b, T c) where T : IEquatable<T>
		{
			return _.LeftDistributes(over, a, b, c) & _.RightDistributes(over, a, b, c);
		}
		#endregion

		#region Idempotent
		/// <summary>aa = a</summary>
		public static bool Idempotent<T>(this Func<T, T, T> _, T a) where T : IEquatable<T>
		{
			return _(a, a).Equals(a);
		}
		#endregion

		#region Absorbers
		public static bool LeftZero<T>(this Func<T, T, T> _, T zero, T on) where T : IEquatable<T>
		{
			return _(zero, on).Equals(zero);
		}

		public static bool RightZero<T>(this Func<T, T, T> _, T zero, T on) where T : IEquatable<T>
		{
			return _(on, zero).Equals(zero);
		}

		public static bool Zero<T>(this Func<T, T, T> _, T zero, T on) where T : IEquatable<T>
		{
			return _.LeftZero(zero, on) && _.RightZero(zero, on);
		}
		#endregion

		#region Identity
		public static bool LeftIdentity<T>(this Func<T, T, T> _, T identity, T on) where T : IEquatable<T>
		{
			return _(identity, on).Equals(on);
		}

		public static bool RightIdentity<T>(this Func<T, T, T> _, T identity, T on) where T : IEquatable<T>
		{
			return _(on, identity).Equals(on);
		}

		public static bool Identity<T>(this Func<T, T, T> _, T identity, T on) where T : IEquatable<T>
		{
			return _.LeftIdentity(identity, on) && _.RightIdentity(identity, on);
		}
		#endregion

		#region Inverse
		public static bool LeftInverse<T>(this Func<T, T, T> _, T inverse, T identity, T of) where T : IEquatable<T>
		{
			return _(inverse, of).Equals(identity);
		}

		public static bool RightInverse<T>(this Func<T, T, T> _, T inverse, T identity, T of) where T : IEquatable<T>
		{
			return _(of, inverse).Equals(identity);
		}

		public static bool Inverse<T>(this Func<T, T, T> _, T inverse, T identity, T of) where T : IEquatable<T>
		{
			return _.LeftInverse(inverse, identity, of) && _.RightInverse(inverse, identity, of);
		}
		#endregion
	}

	/// <summary>Properties of functions</summary>
	public static class FunctionExtensions
	{
		#region *jections
		/// <summary>This function is injective over from</summary>
		public static bool Injection<From, To>(this Func<From, To> _, IEnumerable<From> from)
		{
			var results = new List<To>();
			foreach (From f in from)
				if (results.Contains(_(f)))
					return false;
				else
					results.Add(_(f));
			return true;
		}

		/// <summary>This function is surjective between from and to</summary>
		public static bool Surjection<From, To>(this Func<From, To> _, IEnumerable<From> from, IEnumerable<To> to)
		{
			var results = new List<To>(to);
			foreach (From f in from)
				results.Remove(_(f));
			return results.Count == 0;
		}

		/// <summary>This function is bijective between from and to</summary>
		public static bool Bijection<From, To>(this Func<From, To> _, IEnumerable<From> from, IEnumerable<To> to)
		{
			return _.Injection(from) && _.Surjection(from, to);
		}
		#endregion

		#region inversion
		/// <summary>Return a closure to generate a the inverse of _ over a given domain</summary>
		public static Func<IEnumerable<From>, Func<To, From>> Inverse<From, To>(this Func<From, To> _)
		{
			return from => t => from.Single(f => t.Equals(_(f)));
		}
		#endregion
	}
	/// <summary>Properties of relations</summary>
	public static class RelationExtensions
	{
		#region reflexive
		/// <summary>Every element of on is related to itsself. _(a,a)</summary>
		public static bool Reflexive<T>(this Func<T, T, bool> _, IEnumerable<T> on)
		{
			return on.All(t => _(t, t));
		}
		#endregion

		#region symetric
		/// <summary>_(a,b) iff _(b,a)</summary>
		public static bool Symetric<T>(this Func<T, T, bool> _, IEnumerable<T> on)
		{
			return on.All(a => on.All(b => _(a, b) ? _(b, a) : !_(b, a)));
		}
		#endregion

		#region transitive
		/// <summary>a~b and b~c -> a~c</summary>
		public static bool Transitive<T>(Func<T, T, bool> _, T a, T b, T c)
		{
			return (_(a, b) & _(b, c)) ? _(a, c) : true;
		}

		/// <summary>_(a,b) & _(b,c) => _(a,c)</summary>
		public static bool Transitive<T>(this Func<T, T, bool> _, IEnumerable<T> on)
		{
			return on.All(a => on.All(b => on.All(c => (_(a, b) & _(b, c)) ? _(a, c) : true)));
		}
		#endregion

		#region Euclidean
		/// <summary>_(a,b) & _(a,c) => _(b,c)</summary>
		public static bool Euclidean<T>(this Func<T, T, bool> _, IEnumerable<T> on)
		{
			return on.All(a => on.All(b => on.All(c => ((_(a, b) & _(a, c)) ? _(b, c) : true))));
		}
		#endregion
	}

	public static class EnumerableExtensions
	{
		/// <summary>Generates an ntuple</summary>
		public static IEnumerable<T[]> TuplesOf<T>(this IEnumerable<T> _, int length)
		{
			if (length < 1) throw new ArgumentException("Tuples have length greater than 1");
			foreach (var item in _)
			{
				if (length == 1)
					yield return new T[1] { item };
				else
				{
					foreach (var list in _.TuplesOf(length - 1))
					{
						var l = new List<T>(list);
						l.Add(item);
						yield return l.ToArray();
					}
				}
			}
		}

		/// <summary>Generates the powerset</summary>
		/// <remarks>From Rosetta Code</remarks>
		public static IEnumerable<IEnumerable<T>> Powerset<T>(this IEnumerable<T> _)
		{
			IEnumerable<IEnumerable<T>> seed = new List<IEnumerable<T>>() { Enumerable.Empty<T>() };
			return _.Aggregate(seed, (a, b) => a.Concat(a.Select(x => x.Concat(new List<T>() { b }))));
		}

		public static ISet<IPermutable<uint>> PermutationsOf(uint size)
		{
			var n = new uint[size];
			for (uint i = 0; i < size; i++)
				n[i] = i;
			var range = Powerset(n);
			var cys = new HashSet<IPermutable<uint>>();
			foreach (var cy in range)
			{
				if (cy.Count() > 1)
				{
					cys.Add(new Cycle(cy));
				}
			}
			var perms = new HashSet<IPermutable<uint>>();
			foreach (var cy in Powerset(cys))
			{
				var p = default(Permutation);
				foreach (var q in cy)
					p.Add(q);
				if (p.Equals(Permutation.Identity) || perms.Any(q => p.Equals(q)))
					continue;
				perms.Add(p);
			}
			return perms;
		}
	}
}
