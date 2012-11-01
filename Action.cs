using System;
using System.Collections.Generic;
using System.Linq;

namespace Algebra
{
	public struct Action<G, X>
		where G : IEquatable<G>
		where X : IEquatable<X>
	{
		public Func<G, X, X> Operation;
		public IGroup<G> Group;

		public Action(Func<G, X, X> op, IGroup<G> grp) { Operation = op; Group = grp; }
	}
}
namespace Algebra.Extensions
{
	public static class ActionExtensions
	{
		#region closed
		public static bool LeftClose<G, X>(this Action<G, X> _, ISet<X> set)
			where G : IEquatable<G>
			where X : IEquatable<X>
		{
			return _.Group.All(g => set.All(x => set.Contains(_.Operation(g, x))));
		}

		public static bool LeftIdentity<G, X>(this Action<G, X> _, ISet<X> set)
			where G : IEquatable<G>
			where X : IEquatable<X>
		{
			return set.All(x => _.Operation(_.Group.Identity, x).Equals(x));
		}

		public static bool LeftAssociation<G, X>(this Action<G, X> _, ISet<X> set)
			where G : IEquatable<G>
			where X : IEquatable<X>
		{
			return _.Group.All(g => _.Group.All(h => set.All(x => _.Operation(_.Group.Operation(g, h), x).Equals(_.Operation(g, _.Operation(h, x))))));
		}
		#endregion

		#region Action
		public static bool LeftAction<T, X>(this Magma<T> _, ISet<X> set)
			where T : IEquatable<T>
			where X : IEquatable<X>
		{
			return false;
		}
		#endregion
	}

}
