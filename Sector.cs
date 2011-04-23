using System;
using System.Collections.Generic;

/* Sector - a circular sector
 * 
 * A representation of circular sectors
 * Consists of a polar coordinate `Radius<Center` and an angular sweep `Width`.
 */
namespace fbstj
{
    public struct Sector
    {
        #region static
        /// <summary>The period of a full circle (360 degs, 2PI rads, 400 grads, ...)</summary>
        public static double PERIOD = 360;
        #endregion

        #region state
        /// <summary>The central angle</summary>
        public double Center;

        /// <summary>The width</summary>
        public double Width;

        /// <summary>The radius</summary>
        public double Radius;

        /// <summary>The upper bound</summary>
        public double Maximum { get { return Center + Width / 2; } }

        /// <summary>The lower bound</summary>
        public double Minimum { get { return Center - Width / 2; } }

        private readonly double _circle;

        /// <summary>Initializes</summary>
        public Sector(double center, double width, double radius)
        {
            _circle = PERIOD;
            if (width <= 0) throw new ArgumentException("Width must be positive");
            if (radius <= 0) throw new ArgumentException("Radius must be positive");
            Center = center;
            Width = width > _circle ? _circle : width;
            Radius = radius;
        }
        /// <summary>Initializes with radius 1</summary>
        public Sector(double center, double width) : this(center, width, 1) { }
        /// <summary>Initializes with radius 1 and center 0</summary>
        public Sector(double width) : this(0, width) { }

        public bool Valid { get { return (0 < Width && Width <= _circle) && 0 < Radius; } }
        #endregion

        #region methods
        /// <summary>Tests for point containment</summary>
        public bool Contains(double point) { return Valid && (Minimum <= point && point <= Maximum); }

        public override string ToString() { return Radius + "<" + Center + "(+-" + (Width / 2) + ")" + (Valid?"":"{INVALID}"); }
        #endregion
    }
}
