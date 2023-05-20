using System;
using SoftFloat;
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
        public sfloat X;

        /// <summary>
        /// Y component of the quaternion.
        /// </summary>
        public sfloat Y;

        /// <summary>
        /// Z component of the quaternion.
        /// </summary>
        public sfloat Z;

        /// <summary>
        /// W component of the quaternion.
        /// </summary>
        public sfloat W;

        /// <summary>
        /// Constructs a new Quaternion.
        /// </summary>
        /// <param name="x">X component of the quaternion.</param>
        /// <param name="y">Y component of the quaternion.</param>
        /// <param name="z">Z component of the quaternion.</param>
        /// <param name="w">W component of the quaternion.</param>
        public Quaternion(sfloat x, sfloat y, sfloat z, sfloat w)
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
            sfloat x = a.X;
            sfloat y = a.Y;
            sfloat z = a.Z;
            sfloat w = a.W;
            sfloat bX = b.X;
            sfloat bY = b.Y;
            sfloat bZ = b.Z;
            sfloat bW = b.W;
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
        public static void Multiply(ref Quaternion q, sfloat scale, out Quaternion result)
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
            sfloat aX = a.X;
            sfloat aY = a.Y;
            sfloat aZ = a.Z;
            sfloat aW = a.W;
            sfloat bX = b.X;
            sfloat bY = b.Y;
            sfloat bZ = b.Z;
            sfloat bW = b.W;

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
                return new Quaternion(sfloat.Zero, sfloat.Zero, sfloat.Zero, sfloat.One);
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
            sfloat trace = r.M11 + r.M22 + r.M33;
#if !WINDOWS
            q = new Quaternion();
#endif
            if (trace >= sfloat.Zero)
            {
                var S = libm.sqrtf(trace + sfloat.One) * sfloat.Two; // S=4*qw 
                var inverseS = sfloat.One / S;
                q.W = (sfloat)0.25f * S;
                q.X = (r.M23 - r.M32) * inverseS;
                q.Y = (r.M31 - r.M13) * inverseS;
                q.Z = (r.M12 - r.M21) * inverseS;
            }
            else if ((r.M11 > r.M22) & (r.M11 > r.M33))
            {
                var S = libm.sqrtf(sfloat.One + r.M11 - r.M22 - r.M33) * sfloat.Two; // S=4*qx 
                var inverseS = sfloat.One / S;
                q.W = (r.M23 - r.M32) * inverseS;
                q.X = (sfloat)0.25f * S;
                q.Y = (r.M21 + r.M12) * inverseS;
                q.Z = (r.M31 + r.M13) * inverseS;
            }
            else if (r.M22 > r.M33)
            {
                var S = libm.sqrtf(sfloat.One + r.M22 - r.M11 - r.M33) * sfloat.Two; // S=4*qy
                var inverseS = sfloat.One / S;
                q.W = (r.M31 - r.M13) * inverseS;
                q.X = (r.M21 + r.M12) * inverseS;
                q.Y = (sfloat)0.25f * S;
                q.Z = (r.M32 + r.M23) * inverseS;
            }
            else
            {
                var S = libm.sqrtf(sfloat.One + r.M33 - r.M11 - r.M22) * sfloat.Two; // S=4*qz
                var inverseS = sfloat.One / S;
                q.W = (r.M12 - r.M21) * inverseS;
                q.X = (r.M31 + r.M13) * inverseS;
                q.Y = (r.M32 + r.M23) * inverseS;
                q.Z = (sfloat)0.25f * S;
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
            sfloat inverse = (sfloat)(sfloat.One / libm.sqrtf(quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W));
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
            sfloat inverse = (sfloat)(sfloat.One / libm.sqrtf(X * X + Y * Y + Z * Z + W * W));
            X *= inverse;
            Y *= inverse;
            Z *= inverse;
            W *= inverse;
        }

        /// <summary>
        /// Computes the squared length of the quaternion.
        /// </summary>
        /// <returns>Squared length of the quaternion.</returns>
        public sfloat LengthSquared()
        {
            return X * X + Y * Y + Z * Z + W * W;
        }

        /// <summary>
        /// Computes the length of the quaternion.
        /// </summary>
        /// <returns>Length of the quaternion.</returns>
        public sfloat Length()
        {
            return libm.sqrtf(X * X + Y * Y + Z * Z + W * W);
        }


        /// <summary>
        /// Blends two quaternions together to get an intermediate state.
        /// </summary>
        /// <param name="start">Starting point of the interpolation.</param>
        /// <param name="end">Ending point of the interpolation.</param>
        /// <param name="interpolationAmount">Amount of the end point to use.</param>
        /// <param name="result">Interpolated intermediate quaternion.</param>
        public static void Slerp(ref Quaternion start, ref Quaternion end, sfloat interpolationAmount, out Quaternion result)
        {
            sfloat cosHalfTheta = start.W * end.W + start.X * end.X + start.Y * end.Y + start.Z * end.Z;
            if (cosHalfTheta < sfloat.Zero)
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
            if (cosHalfTheta > (sfloat)(1.0 - 1e-12))
            {
                result.W = start.W;
                result.X = start.X;
                result.Y = start.Y;
                result.Z = start.Z;
                return;
            }
            // Calculate temporary values.
            sfloat halfTheta = libm.acosf(cosHalfTheta);
            sfloat sinHalfTheta = libm.sqrtf(sfloat.One - cosHalfTheta * cosHalfTheta);

            sfloat aFraction = libm.sinf((sfloat.One - interpolationAmount) * halfTheta) / sinHalfTheta;
            sfloat bFraction = libm.sinf(interpolationAmount * halfTheta) / sinHalfTheta;

            //Blend the two quaternions to get the result!
            result.X = (sfloat)(start.X * aFraction + end.X * bFraction);
            result.Y = (sfloat)(start.Y * aFraction + end.Y * bFraction);
            result.Z = (sfloat)(start.Z * aFraction + end.Z * bFraction);
            result.W = (sfloat)(start.W * aFraction + end.W * bFraction);




        }

        /// <summary>
        /// Blends two quaternions together to get an intermediate state.
        /// </summary>
        /// <param name="start">Starting point of the interpolation.</param>
        /// <param name="end">Ending point of the interpolation.</param>
        /// <param name="interpolationAmount">Amount of the end point to use.</param>
        /// <returns>Interpolated intermediate quaternion.</returns>
        public static Quaternion Slerp(Quaternion start, Quaternion end, sfloat interpolationAmount)
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
            sfloat inverseSquaredNorm = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
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
            sfloat x2 = rotation.X + rotation.X;
            sfloat y2 = rotation.Y + rotation.Y;
            sfloat z2 = rotation.Z + rotation.Z;
            sfloat xx2 = rotation.X * x2;
            sfloat xy2 = rotation.X * y2;
            sfloat xz2 = rotation.X * z2;
            sfloat yy2 = rotation.Y * y2;
            sfloat yz2 = rotation.Y * z2;
            sfloat zz2 = rotation.Z * z2;
            sfloat wx2 = rotation.W * x2;
            sfloat wy2 = rotation.W * y2;
            sfloat wz2 = rotation.W * z2;
            //Defer the component setting since they're used in computation.
            sfloat transformedX = v.X * (sfloat.One - yy2 - zz2) + v.Y * (xy2 - wz2) + v.Z * (xz2 + wy2);
            sfloat transformedY = v.X * (xy2 + wz2) + v.Y * (sfloat.One - xx2 - zz2) + v.Z * (yz2 - wx2);
            sfloat transformedZ = v.X * (xz2 - wy2) + v.Y * (yz2 + wx2) + v.Z * (sfloat.One - xx2 - yy2);
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
        public static void TransformX(sfloat x, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            sfloat y2 = rotation.Y + rotation.Y;
            sfloat z2 = rotation.Z + rotation.Z;
            sfloat xy2 = rotation.X * y2;
            sfloat xz2 = rotation.X * z2;
            sfloat yy2 = rotation.Y * y2;
            sfloat zz2 = rotation.Z * z2;
            sfloat wy2 = rotation.W * y2;
            sfloat wz2 = rotation.W * z2;
            //Defer the component setting since they're used in computation.
            sfloat transformedX = x * (sfloat.One - yy2 - zz2);
            sfloat transformedY = x * (xy2 + wz2);
            sfloat transformedZ = x * (xz2 - wy2);
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
        public static void TransformY(sfloat y, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            sfloat x2 = rotation.X + rotation.X;
            sfloat y2 = rotation.Y + rotation.Y;
            sfloat z2 = rotation.Z + rotation.Z;
            sfloat xx2 = rotation.X * x2;
            sfloat xy2 = rotation.X * y2;
            sfloat yz2 = rotation.Y * z2;
            sfloat zz2 = rotation.Z * z2;
            sfloat wx2 = rotation.W * x2;
            sfloat wz2 = rotation.W * z2;
            //Defer the component setting since they're used in computation.
            sfloat transformedX = y * (xy2 - wz2);
            sfloat transformedY = y * (sfloat.One - xx2 - zz2);
            sfloat transformedZ = y * (yz2 + wx2);
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
        public static void TransformZ(sfloat z, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            sfloat x2 = rotation.X + rotation.X;
            sfloat y2 = rotation.Y + rotation.Y;
            sfloat z2 = rotation.Z + rotation.Z;
            sfloat xx2 = rotation.X * x2;
            sfloat xz2 = rotation.X * z2;
            sfloat yy2 = rotation.Y * y2;
            sfloat yz2 = rotation.Y * z2;
            sfloat wx2 = rotation.W * x2;
            sfloat wy2 = rotation.W * y2;
            //Defer the component setting since they're used in computation.
            sfloat transformedX = z * (xz2 + wy2);
            sfloat transformedY = z * (yz2 - wx2);
            sfloat transformedZ = z * (sfloat.One - xx2 - yy2);
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
        public static Quaternion CreateFromAxisAngle(Vector3 axis, sfloat angle)
        {
            sfloat halfAngle = angle * sfloat.Half;
            sfloat s = libm.sinf(halfAngle);
            Quaternion q;
            q.X = (sfloat)(axis.X * s);
            q.Y = (sfloat)(axis.Y * s);
            q.Z = (sfloat)(axis.Z * s);
            q.W = libm.cosf(halfAngle);
            return q;
        }

        /// <summary>
        /// Creates a quaternion from an axis and angle.
        /// </summary>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <param name="q">Quaternion representing the axis and angle rotation.</param>
        public static void CreateFromAxisAngle(ref Vector3 axis, sfloat angle, out Quaternion q)
        {
            sfloat halfAngle = angle * sfloat.Half;
            sfloat s = libm.sinf(halfAngle);
            q.X = (sfloat)(axis.X * s);
            q.Y = (sfloat)(axis.Y * s);
            q.Z = (sfloat)(axis.Z * s);
            q.W = libm.cosf(halfAngle);
        }

        /// <summary>
        /// Constructs a quaternion from yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Yaw of the rotation.</param>
        /// <param name="pitch">Pitch of the rotation.</param>
        /// <param name="roll">Roll of the rotation.</param>
        /// <returns>Quaternion representing the yaw, pitch, and roll.</returns>
        public static Quaternion CreateFromYawPitchRoll(sfloat yaw, sfloat pitch, sfloat roll)
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
        public static void CreateFromYawPitchRoll(sfloat yaw, sfloat pitch, sfloat roll, out Quaternion q)
        {
            sfloat halfRoll = roll * sfloat.Half;
            sfloat halfPitch = pitch * sfloat.Half;
            sfloat halfYaw = yaw * sfloat.Half;

            sfloat sinRoll = libm.sinf(halfRoll);
            sfloat sinPitch = libm.sinf(halfPitch);
            sfloat sinYaw = libm.sinf(halfYaw);

            sfloat cosRoll = libm.cosf(halfRoll);
            sfloat cosPitch = libm.cosf(halfPitch);
            sfloat cosYaw = libm.cosf(halfYaw);

            sfloat cosYawCosPitch = cosYaw * cosPitch;
            sfloat cosYawSinPitch = cosYaw * sinPitch;
            sfloat sinYawCosPitch = sinYaw * cosPitch;
            sfloat sinYawSinPitch = sinYaw * sinPitch;

            q.X = (sfloat)(cosYawSinPitch * cosRoll + sinYawCosPitch * sinRoll);
            q.Y = (sfloat)(sinYawCosPitch * cosRoll - cosYawSinPitch * sinRoll);
            q.Z = (sfloat)(cosYawCosPitch * sinRoll - sinYawSinPitch * cosRoll);
            q.W = (sfloat)(cosYawCosPitch * cosRoll + sinYawSinPitch * sinRoll);

        }

        /// <summary>
        /// Extracts yaw, pitch, and roll from Quaternion.
        /// </summary>
        /// <param name="q">Quaternion representing the yaw, pitch, and roll.</param>
        /// <param name="yaw">Yaw of the rotation.</param>
        /// <param name="pitch">Pitch of the rotation.</param>
        /// <param name="roll">Roll of the rotation.</param>
        public static void ExtractYawPitchRollFromQuaternion(Quaternion q, out sfloat yaw, out sfloat pitch, out sfloat roll)
        {
            pitch = libm.atan2f(sfloat.Two * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
            var sinp = -sfloat.Two * (q.X * q.Z - q.W * q.Y);
            yaw = (sfloat.Abs(sinp) >= sfloat.One) ? (sfloat)sinp.Sign() * sfloat.PiOverTwo: libm.asinf(-sfloat.Two * (q.X * q.Z - q.W * q.Y));
            roll = libm.atan2f(sfloat.Two * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);
        }

        /// <summary>
        /// Computes the angle change represented by a normalized quaternion.
        /// </summary>
        /// <param name="q">Quaternion to be converted.</param>
        /// <returns>Angle around the axis represented by the quaternion.</returns>
        public static sfloat GetAngleFromQuaternion(ref Quaternion q)
        {
            sfloat qw = sfloat.Abs(q.W);
            if (qw > sfloat.One)
                return sfloat.Zero;
            return sfloat.Two * libm.acosf(qw);
        }

        /// <summary>
        /// Computes the axis angle representation of a normalized quaternion.
        /// </summary>
        /// <param name="q">Quaternion to be converted.</param>
        /// <param name="axis">Axis represented by the quaternion.</param>
        /// <param name="angle">Angle around the axis represented by the quaternion.</param>
        public static void GetAxisAngleFromQuaternion(ref Quaternion q, out Vector3 axis, out sfloat angle)
        {
#if !WINDOWS
            axis = new Vector3();
#endif
            sfloat qw = q.W;
            if (qw > sfloat.Zero)
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

            sfloat lengthSquared = axis.LengthSquared();
            if (lengthSquared > (sfloat)1e-14f)
            {
                Vector3.Divide(ref axis, libm.sqrtf(lengthSquared), out axis);
                angle = sfloat.Two * libm.acosf(MathHelper.Clamp(qw, sfloat.MinusOne, sfloat.One));
            }
            else
            {
                axis = Toolbox.UpVector;
                angle = sfloat.Zero;
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
            sfloat dot;
            Vector3.Dot(ref v1, ref v2, out dot);
            //For non-normal vectors, the multiplying the axes length squared would be necessary:
            //sfloat w = dot + (sfloat)Math.Sqrt(v1.LengthSquared() * v2.LengthSquared());
            if (dot < -(sfloat)0.9999f) //parallel, opposing direction
            {
                //If this occurs, the rotation required is ~180 degrees.
                //The problem is that we could choose any perpendicular axis for the rotation. It's not uniquely defined.
                //The solution is to pick an arbitrary perpendicular axis.
                //Project onto the plane which has the lowest component magnitude.
                //On that 2d plane, perform a 90 degree rotation.
                sfloat absX = sfloat.Abs(v1.X);
                sfloat absY = sfloat.Abs(v1.Y);
                sfloat absZ = sfloat.Abs(v1.Z);
                if (absX < absY && absX < absZ)
                    q = new Quaternion(sfloat.Zero, -v1.Z, v1.Y, sfloat.Zero);
                else if (absY < absZ)
                    q = new Quaternion(-v1.Z, sfloat.Zero, v1.X, sfloat.Zero);
                else
                    q = new Quaternion(-v1.Y, v1.X, sfloat.Zero, sfloat.Zero);
            }
            else
            {
                Vector3 axis;
                Vector3.Cross(ref v1, ref v2, out axis);
                q = new Quaternion(axis.X, axis.Y, axis.Z, dot + sfloat.One);
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
