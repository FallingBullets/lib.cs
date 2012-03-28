using System;
using System.Collections.Generic;

namespace Algebra
{
	/// <summary>A set with a binary operation</summary>
	public interface IGroup<T> : ISet<T> where T : IEquatable<T>
	{
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
	public interface IRing<T> : ISet<T> where T : IEquatable<T>
	{
		Func<T, T, T> Addition { get; }
		Func<T, T, T> Multiplication { get; }
	}

	/// <summary></summary>
	public interface ILattice<T> : ISet<T> { }
	/// <summary></summary>
	public interface IModule<T> { }
	/// <summary></summary>
	public interface IAlgebra<T> { }
}
