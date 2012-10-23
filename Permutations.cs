using System;
using System.Collections.Generic;
using System.Linq;

namespace Algebra.Permutations
{
	public interface IPermutable<T>
	{
		T Permute(T e);
	}

	public struct Cycle<T> : IPermutable<T>, IEquatable<Cycle<T>>
	{
		public static implicit operator Cycle<T>(T element) { return new Cycle<T>(element); }

		private T[] _;

		public Cycle(params T[] elements)
		{
			if (new HashSet<T>(elements).Count != elements.Length)
				throw new ArgumentException("parameters not co-unique");
			this._ = elements;
		}

		public int Length { get { return _.Length;}}

		public T Permute(T e)
		{
			if (!_.Contains(e) || _.Count() == 1)
				return e;

			for (int i = 0; i < _.Length; i++)
				if (_.Equals(e))
					return _[(i + 1) % _.Length];

			return e;
		}

		public bool Intersects(Cycle<T> cy)
		{
			foreach (T e in cy._)
				if (_.Contains(e))
					return true;
			return false;
		}

		public bool Equals(Cycle<T> cy)
		{
			foreach (T e in cy._)
				if (!_.Contains(e))
					return false;
			return cy.Length == Length;
		}
	}

	public struct Permutation<T> : IPermutable<T>
	{
		private ISet<Cycle<T>> _;

		public Permutation(params T[] elements) : this(elements.Cast<Cycle<T>>().ToArray()) { }
		public Permutation(params Cycle<T>[] cycles)
		{
			_ = new HashSet<Cycle<T>>(cycles);
		}

		public T Permute(T e)
		{
			foreach (Cycle<T> cy in _)
				e = cy.Permute(e);
			return e;
		}
	}
}
