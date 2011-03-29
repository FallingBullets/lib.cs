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
            for (int i = -540; i < 540; i += 10)
                Console.WriteLine(printAngle(i, Angle.Unit.RADIANS));
            while (true) ;
        }

        static string printAngle(double a, Angle.Unit u)
        {
            Angle _a = new Angle(a, Angle.Unit.DEGREES);
            return _a[Angle.Unit.DEGREES] + ":" + _a.Normalised(u, true);
        }

        static void test()
        {
            Console.WriteLine("testing");
        }
    }
}
