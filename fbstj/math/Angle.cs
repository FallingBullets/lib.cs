using System;
using System.Collections.Generic;

namespace fbstj.math
{
    public delegate double TrigFn(double angle);

    public struct Angle
    {
        #region static
        /// <summary>A set of angular units</summary>
        public enum Unit
        {
            /// <summary>360 degrees in a circle</summary>
            DEGREES,
            /// <summary>2*PI radians in a circle</summary>
            RADIANS,
            /// <summary>400 gradians in a circle</summary>
            GRADIANS,
            /// <summary>100% of the circle</summary>
            PERCENTILES
        }

        /// <summary>The absolute period of a unit</summary>
        /// <param name="unit">An angular unit</param>
        /// <returns>The number of units in a full circle</returns>
        public static double Period(Unit unit)
        {
            switch (unit)
            {
                case Unit.DEGREES:
                    return 360;
                case Unit.GRADIANS:
                    return 400;
                case Unit.PERCENTILES:
                    return 100;
                case Unit.RADIANS:
                    return Math.PI * 2;
                default:
                    return 1;
            }
        }

        /// <summary>The upper bound of a unit</summary>
        /// <param name="unit">An angular unit</param>
        /// <param name="zero">Center the period around 0</param>
        /// <returns>The maximum value that ther period take</returns>
        public static double Max(Unit unit, bool zero) { return Period(unit) / (zero ? 2 : 1); }

        /// <summary>A minimum period</summary>
        /// <param name="unit">The periods unit</param>
        /// <param name="zero">Center the period around 0</param>
        /// <returns>The minimum value that the period takes</returns>
        public static double Min(Unit unit, bool zero) { return zero ? -Period(unit) / 2 : 0; }

        /// <summary>The ratio between two units, used for converting between them.</summary>
        /// <param name="from">The old unit</param>
        /// <param name="to">The new unit</param>
        /// <returns>The ratio converting old units to new units</returns>
        public static double Ratio(Unit from, Unit to) { return (from == to) ? 1 : Period(to) / Period(from); }
        #endregion

        #region state
        private double _value;
        private Unit _unit;

        public Angle(double value, Unit unit) : this() { _value = value; _unit = unit; }

        public double this[Unit unit] { get { return _value * Ratio(_unit, unit); } }

        public double this[TrigFn fn] { get { return fn(this[Unit.RADIANS]); } }

        public double Normalised(Unit unit, bool aroundZero)
        {
            double _a = this[unit];
            double _p = Period(unit), _max = Max(unit, aroundZero), _min = Min(unit, aroundZero);
            while (_a >= _max) _a -= _p;
            while (_a < _min) _a += _p;
            return _a;
        }
        #endregion
    }
}
