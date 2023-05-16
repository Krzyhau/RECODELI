/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution. 
*/

#region Using Statements
using System;
using System.Collections.Generic;

using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
using SoftFloat;
#endregion

namespace Jitter.LinearMath
{

    /// <summary>
    /// Contains some math operations used within Jitter.
    /// </summary>
    public sealed class JMath
    {

        /// <summary>
        /// PI.
        /// </summary>
        public static sfloat Pi = sfloat.Pi;

        public static sfloat PiOver2 = sfloat.PiOverTwo;

        /// <summary>
        /// A small value often used to decide if numeric 
        /// results are zero.
        /// </summary>
        public static sfloat Epsilon = sfloat.Epsilon;

        /// <summary>
        /// Gets the square root.
        /// </summary>
        /// <param name="number">The number to get the square root from.</param>
        /// <returns></returns>
        #region public static sfloat Sqrt(sfloat number)
        public static sfloat Sqrt(sfloat number)
        {
            return libm.sqrtf(number);
        }
        #endregion

        /// <summary>
        /// Gets the sine.
        /// </summary>
        /// <param name="number">The number to get the sine from.</param>
        /// <returns></returns>
        #region public static sfloat Sin(sfloat number)
        public static sfloat Sin(sfloat number)
        {
            return libm.sinf(number);
        }
        #endregion

        /// <summary>
        /// Gets the cosine.
        /// </summary>
        /// <param name="number">The number to get the cosine from.</param>
        /// <returns></returns>
        #region public static sfloat Cos(sfloat number)
        public static sfloat Cos(sfloat number)
        {
            return libm.cosf(number);
        }
        #endregion

        /// <summary>
        /// Gets the floor of given number.
        /// </summary>
        /// <param name="number">The number to get the floor value from.</param>
        /// <returns></returns>
        #region public static sfloat Floor(sfloat number)
        public static sfloat Floor(sfloat number)
        {
            return libm.floorf(number);
        }
        #endregion

        /// <summary>
        /// Gets the ceiling of given number.
        /// </summary>
        /// <param name="number">The number to get the ceiling value from.</param>
        /// <returns></returns>
        #region public static sfloat Ceiling(sfloat number)
        public static sfloat Ceiling(sfloat number)
        {
            return libm.ceilf(number);
        }
        #endregion

        /// <summary>
        /// Gets the arctan for given point.
        /// </summary>
        /// <param name="y">Y coordinate to get the arctan from.</param>
        /// <param name="x">X coordinate to get the arctan from.</param>
        /// <returns></returns>
        #region public static sfloat Atan2(sfloat y, sfloat x)
        public static sfloat Atan2(sfloat y, sfloat x)
        {
            return libm.atan2f(y, x);
        }
        #endregion

        /// <summary>
        /// Gets the power of given number.
        /// </summary>
        /// <param name="number">Base of the power.</param>
        /// <param name="exponent">Exponent of the power.</param>
        /// <returns></returns>
        #region public static sfloat Atan2(sfloat y, sfloat x)
        public static sfloat Pow(sfloat number, sfloat exponent)
        {
            return libm.powf(number, exponent);
        }
        #endregion

        /// <summary>
        /// Gets the maximum number of two values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <returns>Returns the largest value.</returns>
        #region public static sfloat Max(sfloat val1, sfloat val2)
        public static sfloat Max(sfloat val1, sfloat val2)
        {
            return (val1 > val2) ? val1 : val2;
        }
        #endregion

        /// <summary>
        /// Gets the minimum number of two values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <returns>Returns the smallest value.</returns>
        #region public static sfloat Min(sfloat val1, sfloat val2)
        public static sfloat Min(sfloat val1, sfloat val2)
        {
            return (val1 < val2) ? val1 : val2;
        }
        #endregion

        /// <summary>
        /// Gets the maximum number of three values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <param name="val3">The third value.</param>
        /// <returns>Returns the largest value.</returns>
        #region public static sfloat Max(sfloat val1, sfloat val2,sfloat val3)
        public static sfloat Max(sfloat val1, sfloat val2,sfloat val3)
        {
            sfloat max12 = (val1 > val2) ? val1 : val2;
            return (max12 > val3) ? max12 : val3;
        }
        #endregion

        /// <summary>
        /// Returns a number which is within [min,max]
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        #region public static sfloat Clamp(sfloat value, sfloat min, sfloat max)
        public static sfloat Clamp(sfloat value, sfloat min, sfloat max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }
        #endregion
        
        /// <summary>
        /// Changes every sign of the matrix entry to '+'
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="result">The absolute matrix.</param>
        #region public static void Absolute(ref JMatrix matrix,out JMatrix result)
        public static void Absolute(ref JMatrix matrix,out JMatrix result)
        {
            result.M11 = sfloat.Abs(matrix.M11);
            result.M12 = sfloat.Abs(matrix.M12);
            result.M13 = sfloat.Abs(matrix.M13);
            result.M21 = sfloat.Abs(matrix.M21);
            result.M22 = sfloat.Abs(matrix.M22);
            result.M23 = sfloat.Abs(matrix.M23);
            result.M31 = sfloat.Abs(matrix.M31);
            result.M32 = sfloat.Abs(matrix.M32);
            result.M33 = sfloat.Abs(matrix.M33);
        }
        #endregion
    }
}
