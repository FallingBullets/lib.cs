using System;
using System.Collections.Generic;
using System.Linq;
using Algebra.Extensions;

namespace Algebra
{
	internal struct Ringoid<T> where T : IEquatable<T>
	{
		public readonly ISet<T> Elements;

		public readonly Magma<T> Addition;
		public readonly Magma<T> Multiplication;

		public Func<T, T, T> Add { get { return Addition.Operation; } }
		public Func<T, T, T> Multiply { get { return Multiplication.Operation; } }

		public Ringoid(ISet<T> el, Func<T, T, T> add, Func<T, T, T> multiply)
		{
			Elements = el;
			Addition = new Magma<T>(el, add);
			Multiplication = new Magma<T>(el, multiply);
		}
	}

	internal static class RingoidExtensions
	{
		public static bool Distributive<T>(this Ringoid<T> _) where T : IEquatable<T>
		{
			return _.Elements.All(a => _.Elements.All(b => _.Elements.All(c => _.Multiply.Distributes(_.Add, a, b, c))));
		}

		public static T Unit<T>(this Ringoid<T> _) where T : IEquatable<T>
		{
			return _.Multiplication.Identity();
		}

		public static T Zero<T>(this Ringoid<T> _) where T : IEquatable<T>
		{
			return _.Multiplication.Zero();
		}

		public static bool Shell<T>(this Ringoid<T> _) where T : IEquatable<T>
		{
			return _.Zero().Equals(_.Addition.Identity()) && _.Unit().Equals(_.Unit());
		}
	}
}
