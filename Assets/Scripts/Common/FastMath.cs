using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class FastMath
{
    private const float Pi = 3.1415926f;
    private const float halfPi = 1.570796f;

    public static float Sin (float x)
    {
        float b = 4 / Pi;
        float c = -4 / (Pi * Pi);

        return -(b * x + c * x * ((x < 0) ? -x : x));
    }

    public static float Cos (float x)
    {
        return Sin(halfPi - x);
    }

}
