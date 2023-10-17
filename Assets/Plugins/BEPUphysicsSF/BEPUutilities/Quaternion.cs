using System;
using BEPUutilities.FixedMath;
using UnityEngine;

namespace BEPUutilities
{
    /// <summary>
    /// Provides XNA-like quaternion support.
    /// </summary>
    public struct Quaternion : IEquatable<Quaternion>
    {
        /// <summary>
        /// X component of the quaternion.
        /// </summary>
        public fint X;

        /// <summary>
        /// Y component of the quaternion.
        /// </summary>
        public fint Y;

        /// <summary>
        /// Z component of the quaternion.
        /// </summary>
        public fint Z;

        /// <summary>
        /// W component of the quaternion.
        /// </summary>
        public fint W;

        /// <summary>
        /// Constructs a new Quaternion.
        /// </summary>
        /// <param name="x">X component of the quaternion.</param>
        /// <param name="y">Y component of the quaternion.</param>
        /// <param name="z">Z component of the quaternion.</param>
        /// <param name="w">W component of the quaternion.</param>
        public Quaternion(fint x, fint y, fint z, fint w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Adds two quaternions together.
        /// </summary>
        /// <param name="a">First quaternion to add.</param>
        /// <param name="b">Second quaternion to add.</param>
        /// <param name="result">Sum of the addition.</param>
        public static void Add(ref Quaternion a, ref Quaternion b, out Quaternion result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
            result.W = a.W + b.W;
        }

        /// <summary>
        /// Multiplies two quaternions.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Quaternion a, ref Quaternion b, out Quaternion result)
        {
            fint x = a.X;
            fint y = a.Y;
            fint z = a.Z;
            fint w = a.W;
            fint bX = b.X;
            fint bY = b.Y;
            fint bZ = b.Z;
            fint bW = b.W;
            result.X = x * bW + bX * w + y * bZ - z * bY;
            result.Y = y * bW + bY * w + z * bX - x * bZ;
            result.Z = z * bW + bZ * w + x * bY - y * bX;
            result.W = w * bW - x * bX - y * bY - z * bZ;
        }

        /// <summary>
        /// Scales a quaternion.
        /// </summary>
        /// <param name="q">Quaternion to multiply.</param>
        /// <param name="scale">Amount to multiply each component of the quaternion by.</param>
        /// <param name="result">Scaled quaternion.</param>
        public static void Multiply(ref Quaternion q, fint scale, out Quaternion result)
        {
            result.X = q.X * scale;
            result.Y = q.Y * scale;
            result.Z = q.Z * scale;
            result.W = q.W * scale;
        }

        /// <summary>
        /// Multiplies two quaternions together in opposite order.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Concatenate(ref Quaternion a, ref Quaternion b, out Quaternion result)
        {
            fint aX = a.X;
            fint aY = a.Y;
            fint aZ = a.Z;
            fint aW = a.W;
            fint bX = b.X;
            fint bY = b.Y;
            fint bZ = b.Z;
            fint bW = b.W;

            result.X = aW * bX + aX * bW + aZ * bY - aY * bZ;
            result.Y = aW * bY + aY * bW + aX * bZ - aZ * bX;
            result.Z = aW * bZ + aZ * bW + aY * bX - aX * bY;
            result.W = aW * bW - aX * bX - aY * bY - aZ * bZ;


        }

        /// <summary>
        /// Multiplies two quaternions together in opposite order.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Quaternion Concatenate(Quaternion a, Quaternion b)
        {
            Quaternion result;
            Concatenate(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Quaternion representing the identity transform.
        /// </summary>
        public static Quaternion Identity
        {
            get
            {
                return new Quaternion((fint)0, (fint)0, (fint)0, (fint)1);
            }
        }


        /// <summary>
        /// Converts a quaternion to euler angles.
        /// </summary>
        public Vector3 EulerAngles
        {
            get
            {
                ExtractYawPitchRollFromQuaternion(this, out var yaw, out var pitch, out var roll);
                return new Vector3(yaw, pitch, roll);
            }
            set
            {
                CreateFromYawPitchRoll(value.X, value.Y, value.Z, out this);
            }
        }


        /// <summary>
        /// Constructs a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix to create the quaternion from.</param>
        /// <param name="q">Quaternion based on the rotation matrix.</param>
        public static void CreateFromRotationMatrix(ref Matrix3x3 r, out Quaternion q)
        {
            fint trace = r.M11 + r.M22 + r.M33;
#if !WINDOWS
            q = new Quaternion();
#endif
            if (trace >= (fint)0)
            {
                var S = fint.Sqrt(trace + (fint)1) * (fint)2; // S=4*qw 
                var inverseS = (fint)1 / S;
                q.W = (fint)0.25f * S;
                q.X = (r.M23 - r.M32) * inverseS;
                q.Y = (r.M31 - r.M13) * inverseS;
                q.Z = (r.M12 - r.M21) * inverseS;
            }
            else if ((r.M11 > r.M22) & (r.M11 > r.M33))
            {
                var S = fint.Sqrt((fint)1 + r.M11 - r.M22 - r.M33) * (fint)2; // S=4*qx 
                var inverseS = (fint)1 / S;
                q.W = (r.M23 - r.M32) * inverseS;
                q.X = (fint)0.25f * S;
                q.Y = (r.M21 + r.M12) * inverseS;
                q.Z = (r.M31 + r.M13) * inverseS;
            }
            else if (r.M22 > r.M33)
            {
                var S = fint.Sqrt((fint)1 + r.M22 - r.M11 - r.M33) * (fint)2; // S=4*qy
                var inverseS = (fint)1 / S;
                q.W = (r.M31 - r.M13) * inverseS;
                q.X = (r.M21 + r.M12) * inverseS;
                q.Y = (fint)0.25f * S;
                q.Z = (r.M32 + r.M23) * inverseS;
            }
            else
            {
                var S = fint.Sqrt((fint)1 + r.M33 - r.M11 - r.M22) * (fint)2; // S=4*qz
                var inverseS = (fint)1 / S;
                q.W = (r.M12 - r.M21) * inverseS;
                q.X = (r.M31 + r.M13) * inverseS;
                q.Y = (r.M32 + r.M23) * inverseS;
                q.Z = (fint)0.25f * S;
            }
        }

        /// <summary>
        /// Creates a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix used to create a new quaternion.</param>
        /// <returns>Quaternion representing the same rotation as the matrix.</returns>
        public static Quaternion CreateFromRotationMatrix(Matrix3x3 r)
        {
            Quaternion toReturn;
            CreateFromRotationMatrix(ref r, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Constructs a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix to create the quaternion from.</param>
        /// <param name="q">Quaternion based on the rotation matrix.</param>
        public static void CreateFromRotationMatrix(ref Matrix r, out Quaternion q)
        {
            Matrix3x3 downsizedMatrix;
            Matrix3x3.CreateFromMatrix(ref r, out downsizedMatrix);
            CreateFromRotationMatrix(ref downsizedMatrix, out q);
        }

        /// <summary>
        /// Creates a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix used to create a new quaternion.</param>
        /// <returns>Quaternion representing the same rotation as the matrix.</returns>
        public static Quaternion CreateFromRotationMatrix(Matrix r)
        {
            Quaternion toReturn;
            CreateFromRotationMatrix(ref r, out toReturn);
            return toReturn;
        }


        /// <summary>
        /// Ensures the quaternion has unit length.
        /// </summary>
        /// <param name="quaternion">Quaternion to normalize.</param>
        /// <returns>Normalized quaternion.</returns>
        public static Quaternion Normalize(Quaternion quaternion)
        {
            Quaternion toReturn;
            Normalize(ref quaternion, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Ensures the quaternion has unit length.
        /// </summary>
        /// <param name="quaternion">Quaternion to normalize.</param>
        /// <param name="toReturn">Normalized quaternion.</param>
        public static void Normalize(ref Quaternion quaternion, out Quaternion toReturn)
        {
            fint inverse = (fint)((fint)1 / fint.Sqrt(quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W));
            toReturn.X = quaternion.X * inverse;
            toReturn.Y = quaternion.Y * inverse;
            toReturn.Z = quaternion.Z * inverse;
            toReturn.W = quaternion.W * inverse;
        }

        /// <summary>
        /// Scales the quaternion such that it has unit length.
        /// </summary>
        public void Normalize()
        {
            fint inverse = (fint)((fint)1 / fint.Sqrt(X * X + Y * Y + Z * Z + W * W));
            X *= inverse;
            Y *= inverse;
            Z *= inverse;
            W *= inverse;
        }

        /// <summary>
        /// Computes the squared length of the quaternion.
        /// </summary>
        /// <returns>Squared length of the quaternion.</returns>
        public fint LengthSquared()
        {
            return X * X + Y * Y + Z * Z + W * W;
        }

        /// <summary>
        /// Computes the length of the quaternion.
        /// </summary>
        /// <returns>Length of the quaternion.</returns>
        public fint Length()
        {
            return fint.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }


        /// <summary>
        /// Blends two quaternions together to get an intermediate state.
        /// </summary>
        /// <param name="start">Starting point of the interpolation.</param>
        /// <param name="end">Ending point of the interpolation.</param>
        /// <param name="interpolationAmount">Amount of the end point to use.</param>
        /// <param name="result">Interpolated intermediate quaternion.</param>
        public static void Slerp(ref Quaternion start, ref Quaternion end, fint interpolationAmount, out Quaternion result)
        {
            fint cosHalfTheta = start.W * end.W + start.X * end.X + start.Y * end.Y + start.Z * end.Z;
            if (cosHalfTheta < (fint)0)
            {
                //Negating a quaternion results in the same orientation, 
                //but we need cosHalfTheta to be positive to get the shortest path.
                end.X = -end.X;
                end.Y = -end.Y;
                end.Z = -end.Z;
                end.W = -end.W;
                cosHalfTheta = -cosHalfTheta;
            }
            // If the orientations are similar enough, then just pick one of the inputs.
            if (cosHalfTheta > (fint)(1.0 - 1e-12))
            {
                result.W = start.W;
                result.X = start.X;
                result.Y = start.Y;
                result.Z = start.Z;
                return;
            }
            // Calculate temporary values.
            fint halfTheta = fint.Acos(cosHalfTheta);
            fint sinHalfTheta = fint.Sqrt((fint)1 - cosHalfTheta * cosHalfTheta);

            fint aFraction = fint.Sin(((fint)1 - interpolationAmount) * halfTheta) / sinHalfTheta;
            fint bFraction = fint.Sin(interpolationAmount * halfTheta) / sinHalfTheta;

            //Blend the two quaternions to get the result!
            result.X = (fint)(start.X * aFraction + end.X * bFraction);
            result.Y = (fint)(start.Y * aFraction + end.Y * bFraction);
            result.Z = (fint)(start.Z * aFraction + end.Z * bFraction);
            result.W = (fint)(start.W * aFraction + end.W * bFraction);




        }

        /// <summary>
        /// Blends two quaternions together to get an intermediate state.
        /// </summary>
        /// <param name="start">Starting point of the interpolation.</param>
        /// <param name="end">Ending point of the interpolation.</param>
        /// <param name="interpolationAmount">Amount of the end point to use.</param>
        /// <returns>Interpolated intermediate quaternion.</returns>
        public static Quaternion Slerp(Quaternion start, Quaternion end, fint interpolationAmount)
        {
            Quaternion toReturn;
            Slerp(ref start, ref end, interpolationAmount, out toReturn);
            return toReturn;
        }


        /// <summary>
        /// Computes the conjugate of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to conjugate.</param>
        /// <param name="result">Conjugated quaternion.</param>
        public static void Conjugate(ref Quaternion quaternion, out Quaternion result)
        {
            result.X = -quaternion.X;
            result.Y = -quaternion.Y;
            result.Z = -quaternion.Z;
            result.W = quaternion.W;
        }

        /// <summary>
        /// Computes the conjugate of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to conjugate.</param>
        /// <returns>Conjugated quaternion.</returns>
        public static Quaternion Conjugate(Quaternion quaternion)
        {
            Quaternion toReturn;
            Conjugate(ref quaternion, out toReturn);
            return toReturn;
        }



        /// <summary>
        /// Computes the inverse of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to invert.</param>
        /// <param name="result">Result of the inversion.</param>
        public static void Inverse(ref Quaternion quaternion, out Quaternion result)
        {
            fint inverseSquaredNorm = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
            result.X = -quaternion.X * inverseSquaredNorm;
            result.Y = -quaternion.Y * inverseSquaredNorm;
            result.Z = -quaternion.Z * inverseSquaredNorm;
            result.W = quaternion.W * inverseSquaredNorm;
        }

        /// <summary>
        /// Computes the inverse of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to invert.</param>
        /// <returns>Result of the inversion.</returns>
        public static Quaternion Inverse(Quaternion quaternion)
        {
            Quaternion result;
            Inverse(ref quaternion, out result);
            return result;

        }

        /// <summary>
        /// Tests components for equality.
        /// </summary>
        /// <param name="a">First quaternion to test for equivalence.</param>
        /// <param name="b">Second quaternion to test for equivalence.</param>
        /// <returns>Whether or not the quaternions' components were equal.</returns>
        public static bool operator ==(Quaternion a, Quaternion b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        /// <summary>
        /// Tests components for inequality.
        /// </summary>
        /// <param name="a">First quaternion to test for equivalence.</param>
        /// <param name="b">Second quaternion to test for equivalence.</param>
        /// <returns>Whether the quaternions' components were not equal.</returns>
        public static bool operator !=(Quaternion a, Quaternion b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
        }

        /// <summary>
        /// Negates the components of a quaternion.
        /// </summary>
        /// <param name="a">Quaternion to negate.</param>
        /// <param name="b">Negated result.</param>
        public static void Negate(ref Quaternion a, out Quaternion b)
        {
            b.X = -a.X;
            b.Y = -a.Y;
            b.Z = -a.Z;
            b.W = -a.W;
        }      
        
        /// <summary>
        /// Negates the components of a quaternion.
        /// </summary>
        /// <param name="q">Quaternion to negate.</param>
        /// <returns>Negated result.</returns>
        public static Quaternion Negate(Quaternion q)
        {
            Negate(ref q, out var result);
            return result;
        }

        /// <summary>
        /// Negates the components of a quaternion.
        /// </summary>
        /// <param name="q">Quaternion to negate.</param>
        /// <returns>Negated result.</returns>
        public static Quaternion operator -(Quaternion q)
        {
            Negate(ref q, out var result);
            return result;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Quaternion other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (obj is Quaternion)
            {
                return Equals((Quaternion)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
        }

        /// <summary>
        /// Transforms the vector using a quaternion.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref Vector3 v, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            fint x2 = rotation.X + rotation.X;
            fint y2 = rotation.Y + rotation.Y;
            fint z2 = rotation.Z + rotation.Z;
            fint xx2 = rotation.X * x2;
            fint xy2 = rotation.X * y2;
            fint xz2 = rotation.X * z2;
            fint yy2 = rotation.Y * y2;
            fint yz2 = rotation.Y * z2;
            fint zz2 = rotation.Z * z2;
            fint wx2 = rotation.W * x2;
            fint wy2 = rotation.W * y2;
            fint wz2 = rotation.W * z2;
            //Defer the component setting since they're used in computation.
            fint transformedX = v.X * ((fint)1 - yy2 - zz2) + v.Y * (xy2 - wz2) + v.Z * (xz2 + wy2);
            fint transformedY = v.X * (xy2 + wz2) + v.Y * ((fint)1 - xx2 - zz2) + v.Z * (yz2 - wx2);
            fint transformedZ = v.X * (xz2 - wy2) + v.Y * (yz2 + wx2) + v.Z * ((fint)1 - xx2 - yy2);
            result.X = transformedX;
            result.Y = transformedY;
            result.Z = transformedZ;

        }

        /// <summary>
        /// Transforms the vector using a quaternion.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static Vector3 Transform(Vector3 v, Quaternion rotation)
        {
            Vector3 toReturn;
            Transform(ref v, ref rotation, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using a quaternion. Specialized for x,0,0 vectors.
        /// </summary>
        /// <param name="x">X component of the vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformX(fint x, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            fint y2 = rotation.Y + rotation.Y;
            fint z2 = rotation.Z + rotation.Z;
            fint xy2 = rotation.X * y2;
            fint xz2 = rotation.X * z2;
            fint yy2 = rotation.Y * y2;
            fint zz2 = rotation.Z * z2;
            fint wy2 = rotation.W * y2;
            fint wz2 = rotation.W * z2;
            //Defer the component setting since they're used in computation.
            fint transformedX = x * ((fint)1 - yy2 - zz2);
            fint transformedY = x * (xy2 + wz2);
            fint transformedZ = x * (xz2 - wy2);
            result.X = transformedX;
            result.Y = transformedY;
            result.Z = transformedZ;

        }

        /// <summary>
        /// Transforms a vector using a quaternion. Specialized for 0,y,0 vectors.
        /// </summary>
        /// <param name="y">Y component of the vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformY(fint y, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            fint x2 = rotation.X + rotation.X;
            fint y2 = rotation.Y + rotation.Y;
            fint z2 = rotation.Z + rotation.Z;
            fint xx2 = rotation.X * x2;
            fint xy2 = rotation.X * y2;
            fint yz2 = rotation.Y * z2;
            fint zz2 = rotation.Z * z2;
            fint wx2 = rotation.W * x2;
            fint wz2 = rotation.W * z2;
            //Defer the component setting since they're used in computation.
            fint transformedX = y * (xy2 - wz2);
            fint transformedY = y * ((fint)1 - xx2 - zz2);
            fint transformedZ = y * (yz2 + wx2);
            result.X = transformedX;
            result.Y = transformedY;
            result.Z = transformedZ;

        }

        /// <summary>
        /// Transforms a vector using a quaternion. Specialized for 0,0,z vectors.
        /// </summary>
        /// <param name="z">Z component of the vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformZ(fint z, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            fint x2 = rotation.X + rotation.X;
            fint y2 = rotation.Y + rotation.Y;
            fint z2 = rotation.Z + rotation.Z;
            fint xx2 = rotation.X * x2;
            fint xz2 = rotation.X * z2;
            fint yy2 = rotation.Y * y2;
            fint yz2 = rotation.Y * z2;
            fint wx2 = rotation.W * x2;
            fint wy2 = rotation.W * y2;
            //Defer the component setting since they're used in computation.
            fint transformedX = z * (xz2 + wy2);
            fint transformedY = z * (yz2 - wx2);
            fint transformedZ = z * ((fint)1 - xx2 - yy2);
            result.X = transformedX;
            result.Y = transformedY;
            result.Z = transformedZ;

        }


        /// <summary>
        /// Multiplies two quaternions.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            Quaternion toReturn;
            Multiply(ref a, ref b, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Creates a quaternion from an axis and angle.
        /// </summary>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <returns>Quaternion representing the axis and angle rotation.</returns>
        public static Quaternion CreateFromAxisAngle(Vector3 axis, fint angle)
        {
            fint halfAngle = angle * (fint)0.5f;
            fint s = fint.Sin(halfAngle);
            Quaternion q;
            q.X = (fint)(axis.X * s);
            q.Y = (fint)(axis.Y * s);
            q.Z = (fint)(axis.Z * s);
            q.W = fint.Cos(halfAngle);
            return q;
        }

        /// <summary>
        /// Creates a quaternion from an axis and angle.
        /// </summary>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <param name="q">Quaternion representing the axis and angle rotation.</param>
        public static void CreateFromAxisAngle(ref Vector3 axis, fint angle, out Quaternion q)
        {
            fint halfAngle = angle * (fint)0.5f;
            fint s = fint.Sin(halfAngle);
            q.X = (fint)(axis.X * s);
            q.Y = (fint)(axis.Y * s);
            q.Z = (fint)(axis.Z * s);
            q.W = fint.Cos(halfAngle);
        }

        /// <summary>
        /// Constructs a quaternion from yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Yaw of the rotation.</param>
        /// <param name="pitch">Pitch of the rotation.</param>
        /// <param name="roll">Roll of the rotation.</param>
        /// <returns>Quaternion representing the yaw, pitch, and roll.</returns>
        public static Quaternion CreateFromYawPitchRoll(fint yaw, fint pitch, fint roll)
        {
            Quaternion toReturn;
            CreateFromYawPitchRoll(yaw, pitch, roll, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Constructs a quaternion from yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Yaw of the rotation.</param>
        /// <param name="pitch">Pitch of the rotation.</param>
        /// <param name="roll">Roll of the rotation.</param>
        /// <param name="q">Quaternion representing the yaw, pitch, and roll.</param>
        public static void CreateFromYawPitchRoll(fint yaw, fint pitch, fint roll, out Quaternion q)
        {
            fint halfRoll = roll * (fint)0.5f;
            fint halfPitch = pitch * (fint)0.5f;
            fint halfYaw = yaw * (fint)0.5f;

            fint sinRoll = fint.Sin(halfRoll);
            fint sinPitch = fint.Sin(halfPitch);
            fint sinYaw = fint.Sin(halfYaw);

            fint cosRoll = fint.Cos(halfRoll);
            fint cosPitch = fint.Cos(halfPitch);
            fint cosYaw = fint.Cos(halfYaw);

            fint cosYawCosPitch = cosYaw * cosPitch;
            fint cosYawSinPitch = cosYaw * sinPitch;
            fint sinYawCosPitch = sinYaw * cosPitch;
            fint sinYawSinPitch = sinYaw * sinPitch;

            q.X = (fint)(cosYawSinPitch * cosRoll + sinYawCosPitch * sinRoll);
            q.Y = (fint)(sinYawCosPitch * cosRoll - cosYawSinPitch * sinRoll);
            q.Z = (fint)(cosYawCosPitch * sinRoll - sinYawSinPitch * cosRoll);
            q.W = (fint)(cosYawCosPitch * cosRoll + sinYawSinPitch * sinRoll);

        }

        /// <summary>
        /// Extracts yaw, pitch, and roll from Quaternion.
        /// </summary>
        /// <param name="q">Quaternion representing the yaw, pitch, and roll.</param>
        /// <param name="yaw">Yaw of the rotation.</param>
        /// <param name="pitch">Pitch of the rotation.</param>
        /// <param name="roll">Roll of the rotation.</param>
        public static void ExtractYawPitchRollFromQuaternion(Quaternion q, out fint yaw, out fint pitch, out fint roll)
        {
            pitch = fint.Atan2((fint)2 * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
            var sinp = -(fint)2 * (q.X * q.Z - q.W * q.Y);
            yaw = (fint.Abs(sinp) >= (fint)1) ? fint.Sign(sinp) * fint.PiOver2: fint.Asin(-(fint)2 * (q.X * q.Z - q.W * q.Y));
            roll = fint.Atan2((fint)2 * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);
        }

        /// <summary>
        /// Computes the angle change represented by a normalized quaternion.
        /// </summary>
        /// <param name="q">Quaternion to be converted.</param>
        /// <returns>Angle around the axis represented by the quaternion.</returns>
        public static fint GetAngleFromQuaternion(ref Quaternion q)
        {
            fint qw = fint.Abs(q.W);
            if (qw > (fint)1)
                return (fint)0;
            return (fint)2 * fint.Acos(qw);
        }

        /// <summary>
        /// Computes the axis angle representation of a normalized quaternion.
        /// </summary>
        /// <param name="q">Quaternion to be converted.</param>
        /// <param name="axis">Axis represented by the quaternion.</param>
        /// <param name="angle">Angle around the axis represented by the quaternion.</param>
        public static void GetAxisAngleFromQuaternion(ref Quaternion q, out Vector3 axis, out fint angle)
        {
#if !WINDOWS
            axis = new Vector3();
#endif
            fint qw = q.W;
            if (qw > (fint)0)
            {
                axis.X = q.X;
                axis.Y = q.Y;
                axis.Z = q.Z;
            }
            else
            {
                axis.X = -q.X;
                axis.Y = -q.Y;
                axis.Z = -q.Z;
                qw = -qw;
            }

            fint lengthSquared = axis.LengthSquared();
            if (lengthSquared > (fint)1e-14f)
            {
                Vector3.Divide(ref axis, fint.Sqrt(lengthSquared), out axis);
                angle = (fint)2 * fint.Acos(MathHelper.Clamp(qw, (fint)(-1), (fint)1));
            }
            else
            {
                axis = Toolbox.UpVector;
                angle = (fint)0;
            }
        }

        /// <summary>
        /// Computes the quaternion rotation between two normalized vectors.
        /// </summary>
        /// <param name="v1">First unit-length vector.</param>
        /// <param name="v2">Second unit-length vector.</param>
        /// <param name="q">Quaternion representing the rotation from v1 to v2.</param>
        public static void GetQuaternionBetweenNormalizedVectors(ref Vector3 v1, ref Vector3 v2, out Quaternion q)
        {
            fint dot;
            Vector3.Dot(ref v1, ref v2, out dot);
            //For non-normal vectors, the multiplying the axes length squared would be necessary:
            //sfloat w = dot + (sfloat)Math.Sqrt(v1.LengthSquared() * v2.LengthSquared());
            if (dot < -(fint)0.9999f) //parallel, opposing direction
            {
                //If this occurs, the rotation required is ~180 degrees.
                //The problem is that we could choose any perpendicular axis for the rotation. It's not uniquely defined.
                //The solution is to pick an arbitrary perpendicular axis.
                //Project onto the plane which has the lowest component magnitude.
                //On that 2d plane, perform a 90 degree rotation.
                fint absX = fint.Abs(v1.X);
                fint absY = fint.Abs(v1.Y);
                fint absZ = fint.Abs(v1.Z);
                if (absX < absY && absX < absZ)
                    q = new Quaternion((fint)0, -v1.Z, v1.Y, (fint)0);
                else if (absY < absZ)
                    q = new Quaternion(-v1.Z, (fint)0, v1.X, (fint)0);
                else
                    q = new Quaternion(-v1.Y, v1.X, (fint)0, (fint)0);
            }
            else
            {
                Vector3 axis;
                Vector3.Cross(ref v1, ref v2, out axis);
                q = new Quaternion(axis.X, axis.Y, axis.Z, dot + (fint)1);
            }
            q.Normalize();
        }

        //The following two functions are highly similar, but it's a bit of a brain teaser to phrase one in terms of the other.
        //Providing both simplifies things.

        /// <summary>
        /// Computes the rotation from the start orientation to the end orientation such that end = Quaternion.Concatenate(start, relative).
        /// </summary>
        /// <param name="start">Starting orientation.</param>
        /// <param name="end">Ending orientation.</param>
        /// <param name="relative">Relative rotation from the start to the end orientation.</param>
        public static void GetRelativeRotation(ref Quaternion start, ref Quaternion end, out Quaternion relative)
        {
            Quaternion startInverse;
            Conjugate(ref start, out startInverse);
            Concatenate(ref startInverse, ref end, out relative);
        }

        
        /// <summary>
        /// Transforms the rotation into the local space of the target basis such that rotation = Quaternion.Concatenate(localRotation, targetBasis)
        /// </summary>
        /// <param name="rotation">Rotation in the original frame of reference.</param>
        /// <param name="targetBasis">Basis in the original frame of reference to transform the rotation into.</param>
        /// <param name="localRotation">Rotation in the local space of the target basis.</param>
        public static void GetLocalRotation(ref Quaternion rotation, ref Quaternion targetBasis, out Quaternion localRotation)
        {
            Quaternion basisInverse;
            Conjugate(ref targetBasis, out basisInverse);
            Concatenate(ref rotation, ref basisInverse, out localRotation);
        }

        /// <summary>
        /// Gets a string representation of the quaternion.
        /// </summary>
        /// <returns>String representing the quaternion.</returns>
        public override string ToString()
        {
            return "{ X: " + X + ", Y: " + Y + ", Z: " + Z + ", W: " + W + "}";
        }
    }
}
