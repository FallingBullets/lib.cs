using System;
using System.Collections.Generic;
using System.Linq;
using Algebra.Extensions;

namespace Algebra
{
	public struct Magma<T> where T : IEquatable<T>
	{
		public readonly ISet<T> Items;
		public readonly Func<T, T, T> Operation;

		/// <summary>A groupoid around a set and an operation</summary>
		public Magma(ISet<T> items, Func<T, T, T> op) : this(items, op, false) { }
		public Magma(ISet<T> items, Func<T, T, T> op, bool close)
		{
			Items = items;
			Operation = op;
			if (close) this.Close();
		}

		#region interaction
		/// <summary></summary>
		public bool IsOvergroupOf(ISet<T> set)
		{
			var _ = new Magma<T>(set, Operation, false);
			return _.Items.IsSubsetOf(Items) && _.ClosedOverInverses();
		}

		/// <summary></summary>
		public IGroup<T> GenerateSubgroup(params T[] generators)
		{
			var _ = new Magma<T>(new SortedSet<T>(generators), Operation, true);
			if (!IsOvergroupOf(_.Items))
				throw new ArgumentException("Not a subset");
			return new Group<T>(_);
		}

		/// <summary>Left coset of a subgroup</summary>
		public ISet<T> LeftCoset(IGroup<T> subgroup, T item)
		{
			if (!IsOvergroupOf(subgroup))
				throw new ArgumentException("Not a subgroup");
			return new Magma<T>(subgroup, Operation).LeftMultiply(item);
		}
		/// <summary>Right coset of a subgroup</summary>
		public ISet<T> RightCoset(IGroup<T> subgroup, T item)
		{
			if (!IsOvergroupOf(subgroup))
				throw new ArgumentException("Not a subgroup");
			return new Magma<T>(subgroup, Operation).RightMultiply(item);
		}
		#endregion
	}

	/// <summary>A structure around groupoid properties</summary>
	/// <remarks>Used to get around the lack of `this` in closures</remarks>
	internal static class MagmaExtensions
	{
		#region properties
		public static bool Closed<T>(this Magma<T> _) where T : IEquatable<T>
		{
			return _.Items.All(a => _.Items.All(b => _.Items.Contains(_.Operation(a, b))));
		}

		public static bool ClosedOverInverses<T>(this Magma<T> _) where T : IEquatable<T>
		{
			return _.Items.All(a => _.Items.All(b => _.Items.Contains(_.Operation(a, _.Inverse(b, _.Identity())))));
		}

		public static bool Identity<T>(this Magma<T> _, T identity) where T : IEquatable<T>
		{
			return _.Items.All(item => _.Operation.Identity(identity, item));
		}

		public static T Identity<T>(this Magma<T> _) where T : IEquatable<T>
		{
			return _.Items.Single(item => Identity(_, item));
		}

		public static bool Zero<T>(this Magma<T> _, T zero) where T : IEquatable<T>
		{
			return _.Items.All(item => _.Operation.Zero(zero, item));
		}

		public static T Zero<T>(this Magma<T> _) where T : IEquatable<T>
		{
			return _.Items.Single(item => Zero(_, item));
		}

		public static T Inverse<T>(this Magma<T> _, T of, T identity) where T : IEquatable<T>
		{
			return _.Items.Single(item => _.Operation.Inverse(item, identity, of));
		}

		public static bool Idempotent<T>(this Magma<T> _) where T : IEquatable<T>
		{
			return _.Items.All(item => _.Operation.Idempotent(item));
		}

		public static bool Associative<T>(this Magma<T> _) where T : IEquatable<T>
		{
			return _.Items.All(a => _.Items.All(b => _.Items.All(c => _.Operation.Associates(a, b, c))));
		}

		public static bool Commutative<T>(this Magma<T> _) where T : IEquatable<T>
		{
			return _.Items.All(a => _.Items.All(b => _.Operation.Commutes(a, b)));
		}
		#endregion

		#region utility actions
		/// <summary></summary>
		public static void Close<T>(this Magma<T> _) where T : IEquatable<T>
		{
			while (!_.Closed())
			{
				var s = new SortedSet<T>(_.Items);
				Func<ISet<T>, bool> U = delegate(ISet<T> set) { s.UnionWith(set); return true; };
				s.All(item => U(_.LeftMultiply(item)) && U(_.RightMultiply(item)));
			}
		}

		public static ISet<T> LeftMultiply<T>(this Magma<T> _, T by) where T : IEquatable<T>
		{
			var s = new SortedSet<T>();
			_.Items.All(item => s.Add(_.Operation(by, item)));
			return s;
		}

		public static ISet<T> RightMultiply<T>(this Magma<T> _, T by) where T : IEquatable<T>
		{
			var s = new SortedSet<T>();
			_.Items.All(item => s.Add(_.Operation(item, by)));
			return s;
		}
		#endregion
	}
}