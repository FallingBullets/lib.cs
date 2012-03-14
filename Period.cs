using System;

/* Period - a periodic value
 * 
 * Allows you to create periodic values, which have a method to normalize points into the boundary.
 * Emulates circles, where the points are angles, in whichever unit you wish.
 * 
 * (contains a non-generic version of Span<double>)
 */
namespace fbstj
{
	public struct Period
	{
		#region state
		/// <summary>The upper bound of this period. Value + 1 ~ 1</summary>
		public double Value;

		public Period(double value)
		{
			if (value <= 0)
				throw new ArgumentException();
			Value = value;
		}
		#endregion

		#region methods
		/// <summary>Brings an angle to between the bounds of this period</summary>
		public double Normalize(double angle) { return Normalize(angle, Value / 2); }

		/// <summary>Brings an angle to between the bounds of this period, centered around an arbitrary point</summary>
		public double Normalize(double angle, double center)
		{
			Span _ = Center(center);
			//brings up to above min
			while (angle <= _.Minimum) angle += Value;
			//brings down to below max
			while (angle > _.Minimum) angle -= Value;
			return angle;
		}

		/// <summary>Retrieves a span which is centerd around the point passed</summary>
		public Span Center(double center) { return new Span(center - Value / 2, center + Value / 2); }

		public override string ToString() { return "[0.." + Value + "]"; }
		#endregion

		#region span
		public struct Span
		{
			#region static
			/// <summary>Finds the largest point in a selection</summary>
			public static double Max(params double[] points)
			{
				double max = points[0];
				foreach (double p in points)
					if (p > max) max = p;
				return max;
			}

			/// <summary>Finds the smallest point in a selection</summary>
			public static double Min(params double[] points)
			{
				double min = points[0];
				foreach (double p in points)
					if (p < min) min = p;
				return min;
			}
			#endregion

			#region state
			/// <summary>The upper bound</summary>
			public double Maximum;
			/// <summary>The lower bound</summary>
			public double Minimum;

			/// <summary>The point in the middle</summary>
			public double Center { get { return Minimum + Width / 2; } }
			/// <summary>The width</summary>
			public double Width { get { return Maximum - Minimum; } }

			public Span(params double[] points) { Maximum = Max(points); Minimum = Min(points); }

			/// <summary>There are no points between this one and the previous</summary>
			public bool Empty { get { return Maximum == Minimum; } }

			/// <summary>The Minimum is less than the Maximum</summary>
			public bool Valid { get { return Maximum >= Minimum; } }
			#endregion

			#region methods
			/// <summary>Tests for point containment</summary>
			public bool Contains(double point) { return false; }

			/// <summary>The mathematical representation of an interval: [min..max]</summary>
			public override string ToString() { return "[" + Minimum + ".." + Maximum + "]"; }
			#endregion
		}
		#endregion
	}
}
