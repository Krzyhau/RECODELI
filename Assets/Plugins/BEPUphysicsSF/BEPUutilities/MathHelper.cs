using System;
using SoftFloat;

namespace BEPUutilities
{
    /// <summary>
    /// Contains helper math methods.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Approximate value of Pi.
        /// </summary>
        public static sfloat Pi = (sfloat)3.141592653589793239f;

        /// <summary>
        /// Approximate value of Pi multiplied by two.
        /// </summary>
        public static sfloat TwoPi = (sfloat)6.283185307179586477f;

        /// <summary>
        /// Approximate value of Pi divided by two.
        /// </summary>
        public static sfloat PiOver2 = (sfloat)1.570796326794896619f;

        /// <summary>
        /// Approximate value of Pi divided by four.
        /// </summary>
        public static sfloat PiOver4 = (sfloat)0.785398163397448310f;

        /// <summary>
        /// Reduces the angle into a range from -Pi to Pi.
        /// </summary>
        /// <param name="angle">Angle to wrap.</param>
        /// <returns>Wrapped angle.</returns>
        public static sfloat WrapAngle(sfloat angle)
        {
            angle = libm.remainderf(angle, TwoPi);
            if (angle < -Pi)
            {
                angle += TwoPi;
                return angle;
            }
            if (angle >= Pi)
            {
                angle -= TwoPi;
            }
            return angle;

        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">Value to clamp.</param>
        /// <param name="min">Minimum value.  If the value is less than this, the minimum is returned instead.</param>
        /// <param name="max">Maximum value.  If the value is more than this, the maximum is returned instead.</param>
        /// <returns>Clamped value.</returns>
        public static sfloat Clamp(sfloat value, sfloat min, sfloat max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            return value;
        }


        /// <summary>
        /// Returns the higher value of the two parameters.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns>Higher value of the two parameters.</returns>
        public static sfloat Max(sfloat a, sfloat b)
        {
            return a > b ? a : b;
        }

        /// <summary>
        /// Returns the lower value of the two parameters.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns>Lower value of the two parameters.</returns>
        public static sfloat Min(sfloat a, sfloat b)
        {
            return a < b ? a : b;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">Degrees to convert.</param>
        /// <returns>Radians equivalent to the input degrees.</returns>
        public static sfloat ToRadians(sfloat degrees)
        {
            return degrees * (Pi / (sfloat)180f);
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">Radians to convert.</param>
        /// <returns>Degrees equivalent to the input radians.</returns>
        public static sfloat ToDegrees(sfloat radians)
        {
            return radians * ((sfloat)180f / Pi);
        }

        /// <summary>
        /// Moves a value current towards target.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="target">The value to move towards.</param>
        /// <param name="maxDelta">The maximum change that should be applied to the value.</param>
        /// <returns>Degrees equivalent to the input radians.</returns>
        public static sfloat MoveTowards(sfloat current, sfloat target, sfloat maxDelta)
        {
            if (sfloat.Abs(target - current) <= maxDelta)
            {
                return target;
            }

            return current + (sfloat)(target - current).Sign() * maxDelta;
        }
    }
}
