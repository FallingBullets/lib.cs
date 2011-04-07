using System;
using fbstj;
using fbstj.math;

namespace fbstj.test
{
    class Program
    {
        static void Main(string[] args)
        {
            test();
            while (true) ;
        }

        static void test()
        {
            Console.WriteLine("testing");

            Test.intersect(1,2,3, 4);
        }
    }

    static class Test
    {
        internal static void intersect(params int[] x)
        {
            Span<int> a = new Span<int>(x[0], x[1]), b = new Span<int>(x[2], x[3]);
            Console.WriteLine("" + a + "n" + b + "=" + Span<int>.Intersect(a, b));
        }

        internal static void union(params int[] x)
        {
            Span<int> a = new Span<int>(x[0], x[1]), b = new Span<int>(x[2], x[3]);
            Console.WriteLine("" + a + "u" + b + "=" + Span<int>.Union(a, b));
        }

        internal static string angle(double a, Angle.Unit u)
        {
            Angle _a = new Angle(a, Angle.Unit.DEGREES);
            return _a[Angle.Unit.DEGREES] + ":" + _a.Normalised(u, true);
        }
    }
}
