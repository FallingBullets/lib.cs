using System;
using System.Collections.Generic;
using Algebra.Extensions;

namespace Algebra
{
	/// <summary></summary>
	public struct Group<T> : IGroup<T> where T : IEquatable<T>
	{
		private Magma<T> _;
		public Func<T, T, T> Operation { get { return _.Operation; } }
		public T Identity { get { return _.Identity; } }

		public Group(ISet<T> els, Func<T, T, T> op) : this(new Magma<T>(els, op)) { }

		internal Group(Magma<T> m)
		{
			_ = m;
			if (!_.Closed)
				throw new ArgumentException("Operation not closed over set");
			if (!_.Associative)
				throw new ArgumentException("Operation not associative");
			var x = Identity; // throw if there is no identity element
			foreach (T t in _.Items)	// throw if there is an elemnt without an inverse
				x = _.Inverse(t);
		}

		public bool Abelian { get { return _.Commutative; } }

		public T Inverse(T a) { return _.Inverse(a); }

		#region ISet<T> interface exposes _.Items
		public bool Add(T item) { return _.Items.Add(item); }

		public void ExceptWith(IEnumerable<T> other) { _.Items.ExceptWith(other); }

		public void IntersectWith(IEnumerable<T> other) { _.Items.IntersectWith(other); }

		public bool IsProperSubsetOf(IEnumerable<T> other) { return _.Items.IsProperSubsetOf(other); }

		public bool IsProperSupersetOf(IEnumerable<T> other) { return _.Items.IsProperSupersetOf(other); }

		public bool IsSubsetOf(IEnumerable<T> other) { return _.Items.IsSubsetOf(other); }

		public bool IsSupersetOf(IEnumerable<T> other) { return _.Items.IsSupersetOf(other); }

		public bool Overlaps(IEnumerable<T> other) { return _.Items.Overlaps(other); }

		public bool SetEquals(IEnumerable<T> other) { return _.Items.SetEquals(other); }

		public void SymmetricExceptWith(IEnumerable<T> other) { _.Items.SymmetricExceptWith(other); }

		public void UnionWith(IEnumerable<T> other) { _.Items.UnionWith(other); }

		void ICollection<T>.Add(T item) { _.Items.Add(item); }

		public void Clear() { _.Items.Clear(); }

		public bool Contains(T item) { return _.Items.Contains(item); }

		public void CopyTo(T[] array, int arrayIndex) { _.Items.CopyTo(array, arrayIndex); }

		public int Count { get { return _.Items.Count; } }

		public bool IsReadOnly { get { return _.Items.IsReadOnly; } }

		public bool Remove(T item) { return _.Items.Remove(item); }

		public IEnumerator<T> GetEnumerator() { return _.Items.GetEnumerator(); }

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return _.Items.GetEnumerator(); }
		#endregion
	}
}
