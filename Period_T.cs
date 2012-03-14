using System;

/* Period<T> - generic period structure
 * 
 * Allows you to create periodic values, which have a method to normalize points into the boundary.
 * Emulates circles, where the points are angles, in whichever unit you wish.
 */
namespace fbstj
{
	/// <summary>A binary opperation which takes two values and returns a third relative to the two argumaents</summary>
	public delegate T Binary<T>(T a, T b) where T : IComparable<T>;

	public struct Period<T>
		where T : struct, IComparable<T>
	{
		#region state
		/// <summary>The additive identity of T</summary>
		private readonly T _0;

		/// <summary>The subtraction operation of T</summary>
		public readonly Binary<T> Subtract;

		/// <summary>The addition operation of T</summary>
		public readonly Binary<T> Add;

		/// <summary>The periodic value</summary>
		public readonly T Value;

		public Period(T max, Binary<T> sub)
		{
			_0 = default(T);
			if (max.CompareTo(_0) < 0)
				throw new ArgumentException("Maximum must be positive");
			if (sub(max, _0).CompareTo(max) != 0)
				throw new ArgumentException("`diff` must use `default(T)` as aditive identity");
			Value = max;
			Subtract = sub;
			Add = delegate(T a, T b) { return sub(a, sub(default(T), b)); };
		}
		#endregion

		#region methods
		/// <summary>Removes excess periods from a point</summary>
		public T Normalize(T point)
		{
			while (_0.CompareTo(point) > 0) //subtract -Maximum from point until point lies above Zero
				point = Subtract(point, Subtract(_0, Value));
			while (Value.CompareTo(point) <= 0) //subtract Maximum from point until point lies below Maximum
				point = Subtract(point, Value);
			return point;
		}

		/// <summary>The mathematical representation of an interval: [min..max]</summary>
		public override string ToString() { return "[" + _0 + ".." + Value + "]"; }
		#endregion
	}
}
