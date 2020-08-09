using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public static class MathHelper
    {
        public static double Pow(this double x, int n)
        {
            var _x = x;

            if (n == 0)
                return 1.0;

            for (int i = 0; i < n - 1; i++)
                _x *= x;

            return _x;
        }
    }
}
