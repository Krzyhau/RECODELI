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
    /// A Quaternion representing an orientation. Member of the math 
    /// namespace, so every method has it's 'by reference' equivalent
    /// to speed up time critical math operations.
    /// </summary>
    public struct JQuaternion
    {

        /// <summary>The X component of the quaternion.</summary>
        public sfloat X;
        /// <summary>The Y component of the quaternion.</summary>
        public sfloat Y;
        /// <summary>The Z component of the quaternion.</summary>
        public sfloat Z;
        /// <summary>The W component of the quaternion.</summary>
        public sfloat W;

        static JQuaternion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the JQuaternion structure.
        /// </summary>
        /// <param name="x">The X component of the quaternion.</param>
        /// <param name="y">The Y component of the quaternion.</param>
        /// <param name="z">The Z component of the quaternion.</param>
        /// <param name="w">The W component of the quaternion.</param>
        public JQuaternion(sfloat x, sfloat y, sfloat z, sfloat w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Quaternions are added.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The sum of both quaternions.</returns>
        #region public static JQuaternion Add(JQuaternion quaternion1, JQuaternion quaternion2)
        public static JQuaternion Add(JQuaternion quaternion1, JQuaternion quaternion2)
        {
            JQuaternion result;
            JQuaternion.Add(ref quaternion1, ref quaternion2, out result);
            return result;
        }

        public static void CreateFromYawPitchRoll(sfloat yaw, sfloat pitch, sfloat roll, out JQuaternion result)
        {
            sfloat num9 = roll * sfloat.Half;
            sfloat num6 = JMath.Sin(num9);
            sfloat num5 = JMath.Cos(num9);
            sfloat num8 = pitch * sfloat.Half;
            sfloat num4 = JMath.Sin(num8);
            sfloat num3 = JMath.Cos(num8);
            sfloat num7 = yaw * sfloat.Half;
            sfloat num2 = JMath.Sin(num7);
            sfloat num = JMath.Cos(num7);
            result.X = ((num * num4) * num5) + ((num2 * num3) * num6);
            result.Y = ((num2 * num3) * num5) - ((num * num4) * num6);
            result.Z = ((num * num3) * num6) - ((num2 * num4) * num5);
            result.W = ((num * num3) * num5) + ((num2 * num4) * num6);
        }

 


        /// <summary>
        /// Quaternions are added.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The sum of both quaternions.</param>
        public static void Add(ref JQuaternion quaternion1, ref JQuaternion quaternion2, out JQuaternion result)
        {
            result.X = quaternion1.X + quaternion2.X;
            result.Y = quaternion1.Y + quaternion2.Y;
            result.Z = quaternion1.Z + quaternion2.Z;
            result.W = quaternion1.W + quaternion2.W;
        }
        #endregion

        public static JQuaternion Conjugate(JQuaternion value)
        {
            JQuaternion quaternion;
            quaternion.X = -value.X;
            quaternion.Y = -value.Y;
            quaternion.Z = -value.Z;
            quaternion.W = value.W;
            return quaternion;
        }

        /// <summary>
        /// Quaternions are subtracted.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The difference of both quaternions.</returns>
        #region public static JQuaternion Subtract(JQuaternion quaternion1, JQuaternion quaternion2)
        public static JQuaternion Subtract(JQuaternion quaternion1, JQuaternion quaternion2)
        {
            JQuaternion result;
            JQuaternion.Subtract(ref quaternion1, ref quaternion2, out result);
            return result;
        }

        /// <summary>
        /// Quaternions are subtracted.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The difference of both quaternions.</param>
        public static void Subtract(ref JQuaternion quaternion1, ref JQuaternion quaternion2, out JQuaternion result)
        {
            result.X = quaternion1.X - quaternion2.X;
            result.Y = quaternion1.Y - quaternion2.Y;
            result.Z = quaternion1.Z - quaternion2.Z;
            result.W = quaternion1.W - quaternion2.W;
        }
        #endregion

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The product of both quaternions.</returns>
        #region public static JQuaternion Multiply(JQuaternion quaternion1, JQuaternion quaternion2)
        public static JQuaternion Multiply(JQuaternion quaternion1, JQuaternion quaternion2)
        {
            JQuaternion result;
            JQuaternion.Multiply(ref quaternion1, ref quaternion2, out result);
            return result;
        }

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The product of both quaternions.</param>
        public static void Multiply(ref JQuaternion quaternion1, ref JQuaternion quaternion2, out JQuaternion result)
        {
            sfloat x = quaternion1.X;
            sfloat y = quaternion1.Y;
            sfloat z = quaternion1.Z;
            sfloat w = quaternion1.W;
            sfloat num4 = quaternion2.X;
            sfloat num3 = quaternion2.Y;
            sfloat num2 = quaternion2.Z;
            sfloat num = quaternion2.W;
            sfloat num12 = (y * num2) - (z * num3);
            sfloat num11 = (z * num4) - (x * num2);
            sfloat num10 = (x * num3) - (y * num4);
            sfloat num9 = ((x * num4) + (y * num3)) + (z * num2);
            result.X = ((x * num) + (num4 * w)) + num12;
            result.Y = ((y * num) + (num3 * w)) + num11;
            result.Z = ((z * num) + (num2 * w)) + num10;
            result.W = (w * num) - num9;
        }
        #endregion

        /// <summary>
        /// Scale a quaternion
        /// </summary>
        /// <param name="quaternion1">The quaternion to scale.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <returns>The scaled quaternion.</returns>
        #region public static JQuaternion Multiply(JQuaternion quaternion1, sfloat scaleFactor)
        public static JQuaternion Multiply(JQuaternion quaternion1, sfloat scaleFactor)
        {
            JQuaternion result;
            JQuaternion.Multiply(ref quaternion1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// Scale a quaternion
        /// </summary>
        /// <param name="quaternion1">The quaternion to scale.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <param name="result">The scaled quaternion.</param>
        public static void Multiply(ref JQuaternion quaternion1, sfloat scaleFactor, out JQuaternion result)
        {
            result.X = quaternion1.X * scaleFactor;
            result.Y = quaternion1.Y * scaleFactor;
            result.Z = quaternion1.Z * scaleFactor;
            result.W = quaternion1.W * scaleFactor;
        }
        #endregion

        /// <summary>
        /// Sets the length of the quaternion to one.
        /// </summary>
        #region public void Normalize()
        public void Normalize()
        {
            sfloat num2 = (((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z)) + (this.W * this.W);
            sfloat num = sfloat.One / (JMath.Sqrt(num2));
            this.X *= num;
            this.Y *= num;
            this.Z *= num;
            this.W *= num;
        }
        #endregion

        /// <summary>
        /// Creates a quaternion from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix representing an orientation.</param>
        /// <returns>JQuaternion representing an orientation.</returns>
        #region public static JQuaternion CreateFromMatrix(JMatrix matrix)
        public static JQuaternion CreateFromMatrix(JMatrix matrix)
        {
            JQuaternion result;
            JQuaternion.CreateFromMatrix(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix representing an orientation.</param>
        /// <param name="result">JQuaternion representing an orientation.</param>
        public static void CreateFromMatrix(ref JMatrix matrix, out JQuaternion result)
        {
            sfloat num8 = (matrix.M11 + matrix.M22) + matrix.M33;
            if (num8 > sfloat.Zero)
            {
                sfloat num = JMath.Sqrt(num8 + sfloat.One);
                result.W = num * sfloat.Half;
                num = sfloat.Half / num;
                result.X = (matrix.M23 - matrix.M32) * num;
                result.Y = (matrix.M31 - matrix.M13) * num;
                result.Z = (matrix.M12 - matrix.M21) * num;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                sfloat num7 = JMath.Sqrt(((sfloat.One + matrix.M11) - matrix.M22) - matrix.M33);
                sfloat num4 = sfloat.Half / num7;
                result.X = sfloat.Half * num7;
                result.Y = (matrix.M12 + matrix.M21) * num4;
                result.Z = (matrix.M13 + matrix.M31) * num4;
                result.W = (matrix.M23 - matrix.M32) * num4;
            }
            else if (matrix.M22 > matrix.M33)
            {
                sfloat num6 = JMath.Sqrt(((sfloat.One + matrix.M22) - matrix.M11) - matrix.M33);
                sfloat num3 = sfloat.Half / num6;
                result.X = (matrix.M21 + matrix.M12) * num3;
                result.Y = sfloat.Half * num6;
                result.Z = (matrix.M32 + matrix.M23) * num3;
                result.W = (matrix.M31 - matrix.M13) * num3;
            }
            else
            {
                sfloat num5 = JMath.Sqrt(((sfloat.One + matrix.M33) - matrix.M11) - matrix.M22);
                sfloat num2 = sfloat.Half / num5;
                result.X = (matrix.M31 + matrix.M13) * num2;
                result.Y = (matrix.M32 + matrix.M23) * num2;
                result.Z = sfloat.Half * num5;
                result.W = (matrix.M12 - matrix.M21) * num2;
            }
        }
        #endregion

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The product of both quaternions.</returns>
        #region public static sfloat operator *(JQuaternion value1, JQuaternion value2)
        public static JQuaternion operator *(JQuaternion value1, JQuaternion value2)
        {
            JQuaternion result;
            JQuaternion.Multiply(ref value1, ref value2,out result);
            return result;
        }
        #endregion

        /// <summary>
        /// Add two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The sum of both quaternions.</returns>
        #region public static sfloat operator +(JQuaternion value1, JQuaternion value2)
        public static JQuaternion operator +(JQuaternion value1, JQuaternion value2)
        {
            JQuaternion result;
            JQuaternion.Add(ref value1, ref value2, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// Subtract two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The difference of both quaternions.</returns>
        #region public static sfloat operator -(JQuaternion value1, JQuaternion value2)
        public static JQuaternion operator -(JQuaternion value1, JQuaternion value2)
        {
            JQuaternion result;
            JQuaternion.Subtract(ref value1, ref value2, out result);
            return result;
        }
        #endregion

    }
}
