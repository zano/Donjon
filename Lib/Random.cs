using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Lib {
    public static class Rand {
        private static readonly Random R = new Random();
        private static readonly double K = Sqrt(2 / E);

        public static bool Chance(int chanceForSuccess) => R.Next(100) < chanceForSuccess;

        public static int Next(int max) => R.Next(max);
        public static int Next(int min, int max) => R.Next(min, max);

        /// <summary>Returns a random result from <code>min</code> to <c>max</c> </summary>
        /// <param name="min">The lowest possible result</param>
        /// <param name="max">The highest possible result</param>
        /// <returns></returns>
        public static int Dice(int max, int min = 1) => R.Next(min, max + 1);

        public static int Roll(int rolls, int max, int min = 1) => new int[rolls].Sum(v => Dice(max, min));
        
        /// <summary> Generate a random number from a standard normal distribution </summary>  
        /// <remarks> Using Ratio-of-uniforms method
        ///  If a dataset follows a normal distribution, then about 68% of the 
        /// observations will fall within σ of the mean μ, which in this case is with the 
        /// interval (-1,1). About 95% of the observations will fall within 2 standard </remarks>
        /// <param name="mean">50% of the results will be greater, and 50% will be less than the <code>mean</code></param>
        /// <param name="standardDeviation">Approx 68% of the results will fall within ±1 <code>standardDeviation</code> from the <code>mean</code></param>
        /// <returns></returns>
        public static double Normal(double mean = 0, double standardDeviation = 1) {
            double a, b;
            do {
                a = R.NextDouble();
                b = (2 * R.NextDouble() - 1) * K;
            } while (-4.0 * a * a * Log(a) < b * b);
            var normal = b / a;
            return mean + normal * standardDeviation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <ref>http://www2.econ.osaka-u.ac.jp/~tanizaki/class/2013/econome3/13.pdf</ref>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static double Gamma(double alpha) {
            var b = Pow((alpha - 1) / E, 0.5 * alpha - 0.5);
            var d = Pow((alpha + 1) / E, 0.5 * alpha + 0.5);
            double gamma;
            double u;
            do {
                u = R.NextDouble() * b;
                gamma = R.NextDouble() * d / u;
            } while (2 * Log(u) > (alpha - 1) * Log(gamma) - gamma);
            return gamma;
        }        
    }
}