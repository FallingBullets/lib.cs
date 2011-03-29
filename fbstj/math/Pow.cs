using System;
using System.Collections.Generic;

namespace fbstj.math
{
    public struct Pow
    {
        public static implicit operator double(Pow power) { return power.Root; }
        public static Pow operator ^(Pow mantisa, double exponent) { return mantisa[exponent]; }

        public static readonly Pow IDENTITY = new Pow(1);

        public readonly double Root;

        private List<Pow> _arr;

        private double _pow(double i) { return System.Math.Pow(Root, i); }

        public Pow(double num)
        {
            Root = num;
            _arr = new List<Pow>();
            _arr.Add(IDENTITY);
            _arr.Add(this);
        }

        public Pow this[double i]
        {
            get
            {
                if (i < 0 || i != (int)i)
                    return new Pow(_pow(i));

                int _i = (int)i;
                switch (_i)
                {
                    case 0:
                        return IDENTITY;
                    case 1:
                        return new Pow(this);
                    default:
                        if (_i >= _arr.Count)
                            for (int j = _arr.Count; j < _i; j++)
                                _arr.Add(new Pow(_arr[j - 1] * Root));

                        return _arr[_i];
                }
            }
        }
    }
}
