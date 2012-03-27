using System;
using System.Collections.Generic;

namespace fbstj.Algebra
{
	/// <summary>Properties of binary operations</summary>
	public static class BinOp
	{
		/// <summary>a(bc) == (ab)c</summary>
		public static bool Associative<T>(this Func<T, T, T> _, T a, T b, T c)
			where T : IEquatable<T>
		{
			return _(a, _(b, c)).Equals(_(_(a, b), c));
		}

		/// <summary>ab == ba</summary>
		public static bool Commutative<T>(this Func<T, T, T> _, T a, T b)
			where T : IEquatable<T>
		{
			return _(a, b).Equals(_(b, a));
		}

		/// <summary>a * (b + c) = (a * c) + )b * c)</summary>
		public static bool LeftDistributive<T>(this Func<T, T, T> _, Func<T, T, T> over, T a, T b, T c)
			where T : IEquatable<T>
		{
			return _(a, over(b, c)).Equals(over(_(a, b), _(a, c)));
		}

		/// <summary>(a + b) * c = (a * c) + (b * c)</summary>
		public static bool RightDistributive<T>(this Func<T, T, T> _, Func<T, T, T> over, T a, T b, T c)
			where T : IEquatable<T>
		{
			return _(over(a, b), c).Equals(over(_(a, c), _(b, c)));
		}

		/// <summary>Left and Right distributive</summary>
		public static bool Distributive<T>(this Func<T, T, T> _, Func<T, T, T> over, T a, T b, T c)
			where T : IEquatable<T>
		{
			return _.LeftDistributive(over, a, b, c) & _.RightDistributive(over, a, b, c);
		}

		/// <summary>aa = a</summary>
		public static bool Idempotent<T>(this Func<T, T, T> _, T a)
			where T : IEquatable<T>
		{
			return _(a, a).Equals(a);
		}
	}

	/// <summary>Properties of functions</summary>
	public static class Function
	{
		public static bool Injection<From, To>(this Func<From, To> _, ICollection<From> from)
		{
			var results = new List<To>();
			foreach (From f in from)
				if (results.Contains(_(f)))
					return false;
				else
					results.Add(_(f));
			return true;
		}

		public static bool Surjection<From, To>(this Func<From, To> _, ICollection<From> from, ICollection<To> to)
		{
			var results = new List<To>(to);
			foreach (From f in from)
				results.Remove(_(f));
			return results.Count == 0;
		}

		public static bool Bijection<From, To>(this Func<From, To> _, ICollection<From> from, ICollection<To> to)
		{
			return _.Injection(from) && _.Surjection(from, to);
		}

		public static Func<To, From> Inverse<From, To>(this Func<From, To> _, ICollection<From> from, ICollection<To> to)
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
	public static class Relation
	{
		/// <summary>Every element of on is related to itsself. _(a,a)</summary>
		public static bool Reflexive<T>(this Func<T, T, bool> _, ICollection<T> on)
		{
			foreach (T t in on)
				if (!_(t, t))
					return false;
			return true;
		}

		/// <summary>_(a,b) iff _(b,a)</summary>
		public static bool Symetric<T>(this Func<T, T, bool> _, ICollection<T> on)
		{
			bool sym = true;
			foreach (T a in on)
				foreach (T b in on)
					sym &= _(a, b) ? _(b, a) : !_(b, a);
			return sym;
		}

		/// <summary>a~b and b~c -> a~c</summary>
		public static bool Transitive<T>(Func<T, T, bool> _, T a, T b, T c)
		{
			return (_(a, b) & _(b, c)) ? _(a, c) : true;
		}

		/// <summary>_(a,b) & _(b,c) => _(a,c)</summary>
		public static bool Transitive<T>(this Func<T, T, bool> _, ICollection<T> on)
		{
			foreach (T a in on)
				foreach (T b in on)
					foreach (T c in on)
						if (!Transitive(_, a, b, c))
							return false;
			return true;
		}

		/// <summary>_(a,b) & _(a,c) => _(b,c)</summary>
		public static bool Euclidean<T>(this Func<T, T, bool> _, ICollection<T> on)
		{
			bool e = true;
			foreach (T a in on)
				foreach (T b in on)
					foreach (T c in on)
						e &= (_(a, b) & _(a, c)) ? _(b, c) : true;
			return e;
		}
	}

	/// <summary>Distinguishes set's from other collections</summary>
	public interface ISet<T> : ICollection<T> where T : IEquatable<T>
	{
		bool Add(T element);
		bool Union(IEnumerable<T> other);
		bool Intersect(IEnumerable<T> other);
	}

	/// <summary>Concrete ISet for tetsing purposes</summary>
	public class Set<T> : List<T>, ISet<T> where T : IEquatable<T> { }

	/// <summary>A set with a binary operation</summary>
	public interface IGroup<T> where T : IEquatable<T>
	{
		/// <summary>The set of elements the group is based on</summary>
		ISet<T> Elements { get; }
		/// <summary>The binary operation the group works</summary>
		Func<T, T, T> Operation { get; }
		/// <summary>The group is abelian</summary>
		bool Abelian { get; }
		/// <summary>The identity element</summary>
		T Identity { get; }
		/// <summary>The inverse of the passed element</summary>
		T Inverse(T element);
	}

	/// <summary>A set with two binary operations</summary>
	public interface IRing<T> where T : IEquatable<T>
	{
		ISet<T> Elements { get; }
		Func<T, T, T> Addition { get; }
		Func<T, T, T> Multiplication { get; }
	}

	/// <summary></summary>
	public interface ILattice<T> { }
	/// <summary></summary>
	public interface IModule<T> { }
	/// <summary></summary>
	public interface IAlgebra<T> { }

	/// <summary>A structure around groupoid properties</summary>
	internal struct Magma<T> where T : IEquatable<T>
	{
		public readonly ISet<T> Elements;
		public readonly Func<T, T, T> Operation;

		/// <summary>A groupoid around a set and an operation</summary>
		public Magma(ISet<T> els, Func<T, T, T> op) { Elements = els; Operation = op; }

		/// <summary>For all x,y in S, x.y is in S</summary>
		public bool Closed
		{
			get
			{
				foreach (T a in Elements)
					foreach (T b in Elements)
						if (!Elements.Contains(Operation(a, b)))
							return false;
				return true;
			}
		}

		/// <summary>Brackets disappear</summary>
		public bool Associative
		{
			get
			{
				foreach (T a in Elements)
					foreach (T b in Elements)
						foreach (T c in Elements)
							if (!Operation.Associative(a, b, c))
								return false;
				return true;
			}
		}

		/// <summary>All elements commute</summary>
		public bool Commutative
		{
			get
			{

				foreach (T a in Elements)
					foreach (T b in Elements)
						if (!Operation.Commutative(a, b))
							return false;
				return true;
			}
		}

		public bool LeftIdentity(T e)
		{
			foreach (T a in Elements)
				if (!Operation(e, a).Equals(a))
					return false;
			return true;
		}

		public bool RightIdentity(T e)
		{
			foreach (T a in Elements)
				if (!Operation(a, e).Equals(a))
					return false;
			return true;
		}

		/// <summary>For all a in S, a.Identity == a == Identyty.a</summary>
		public T Identity
		{
			get
			{
				foreach (T e in Elements)
					if (LeftIdentity(e) & RightIdentity(e))
						return e;
				throw new Exception("No identity element");
			}
		}

		/// <summary>For all a in S, a * Inverse(a) == Identity</summary>
		public T Inverse(T a)
		{
			foreach (T b in Elements)
				if (Operation(a, b).Equals(Identity))
					return b;
			throw new Exception("No inverse for " + a.ToString());
		}

		/// <summary>For all a in S, a * Absorber == Absorber</summary>
		public T Absorber
		{
			get
			{
				foreach (T x in Elements)
				{
					bool y = true;
					foreach (T a in Elements)
						y &= Operation(x, a).Equals(x);
					if (y)
						return x;
				}
				throw new Exception("No absorbing element");
			}
		}
	}

	/// <summary></summary>
	public struct Group<T> : IGroup<T> where T : IEquatable<T>
	{
		private Magma<T> _;
		public ISet<T> Elements { get { return _.Elements; } }
		public Func<T, T, T> Operation { get { return _.Operation; } }
		public T Identity { get { return _.Identity; } }

		public Group(ISet<T> els, Func<T, T, T> op)
		{
			_ = new Magma<T>(els, op);
			if (!_.Closed)
				throw new ArgumentException("Operation not closed over set");
			if (!_.Associative)
				throw new ArgumentException("Operation not associative");
			var x = Identity; // throw if there is no identity element
			foreach (T t in els)	// throw if there is an elemnt without an inverse
				x = _.Inverse(t);
		}

		public bool Abelian { get { return _.Commutative; } }

		public T Inverse(T a) { return _.Inverse(a); }
	}

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

		/// <summary>Multiplication is distributive over addition</summary>
		public bool Distributive
		{
			get
			{
				foreach (T a in Elements)
					foreach (T b in Elements)
						foreach (T c in Elements)
							if (!Multiply.Distributive(Addition.Operation, a, b, c))
								return false;
				return true;
			}
		}

		public T Unit { get { return Multiplication.Identity; } }
		public T Zero { get { return Multiplication.Absorber; } }

		public bool Shell { get { return Zero.Equals(Addition.Identity) && Unit.Equals(Unit); } }
	}
}
