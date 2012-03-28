using System;
using System.Collections.Generic;
using System.Linq;

namespace Algebra.Extensions
{
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

		public static bool Surjection<From, To>(this Func<From, To> _, IEnumerable<From> from, IEnumerable<To> to)
		{
			var results = new List<To>(to);
			foreach (From f in from)
				results.Remove(_(f));
			return results.Count == 0;
		}

		public static bool Bijection<From, To>(this Func<From, To> _, IEnumerable<From> from, IEnumerable<To> to)
		{
			return _.Injection(from) && _.Surjection(from, to);
		}

		public static Func<To, From> Inverse<From, To>(this Func<From, To> _, IEnumerable<From> from, IEnumerable<To> to)
		{
			if (_.Bijection(from, to))
				throw new ArgumentException("Function not a bijection");
			return delegate(To t)
			{
				foreach (From f in from)
					if (t.Equals(_(f)))
						return f;
				throw new ArgumentException("Element not invertable");
			};
		}
	}

	/// <summary>Properties of relations</summary>
	public static class RelationExtensions
	{
		/// <summary>Every element of on is related to itsself. _(a,a)</summary>
		public static bool Reflexive<T>(this Func<T, T, bool> _, IEnumerable<T> on) { return on.All(t => _(t, t)); }

		/// <summary>_(a,b) iff _(b,a)</summary>
		public static bool Symetric<T>(this Func<T, T, bool> _, IEnumerable<T> on)
		{
			return on.All(a => on.All(b => _(a, b) ? _(b, a) : !_(b, a)));
		}

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

		/// <summary>_(a,b) & _(a,c) => _(b,c)</summary>
		public static bool Euclidean<T>(this Func<T, T, bool> _, IEnumerable<T> on)
		{
			return on.All(a => on.All(b => on.All(c => ((_(a, b) & _(a, c)) ? _(b, c) : true))));
		}
	}
}
