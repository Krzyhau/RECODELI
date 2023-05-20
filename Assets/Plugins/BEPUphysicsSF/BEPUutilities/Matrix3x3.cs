﻿using System;
using SoftFloat;
 


namespace BEPUutilities
{
    /// <summary>
    /// 3 row, 3 column matrix.
    /// </summary>
    public struct Matrix3x3
    {
        /// <summary>
        /// Value at row 1, column 1 of the matrix.
        /// </summary>
        public sfloat M11;

        /// <summary>
        /// Value at row 1, column 2 of the matrix.
        /// </summary>
        public sfloat M12;

        /// <summary>
        /// Value at row 1, column 3 of the matrix.
        /// </summary>
        public sfloat M13;

        /// <summary>
        /// Value at row 2, column 1 of the matrix.
        /// </summary>
        public sfloat M21;

        /// <summary>
        /// Value at row 2, column 2 of the matrix.
        /// </summary>
        public sfloat M22;

        /// <summary>
        /// Value at row 2, column 3 of the matrix.
        /// </summary>
        public sfloat M23;

        /// <summary>
        /// Value at row 3, column 1 of the matrix.
        /// </summary>
        public sfloat M31;

        /// <summary>
        /// Value at row 3, column 2 of the matrix.
        /// </summary>
        public sfloat M32;

        /// <summary>
        /// Value at row 3, column 3 of the matrix.
        /// </summary>
        public sfloat M33;

        /// <summary>
        /// Constructs a new 3 row, 3 column matrix.
        /// </summary>
        /// <param name="m11">Value at row 1, column 1 of the matrix.</param>
        /// <param name="m12">Value at row 1, column 2 of the matrix.</param>
        /// <param name="m13">Value at row 1, column 3 of the matrix.</param>
        /// <param name="m21">Value at row 2, column 1 of the matrix.</param>
        /// <param name="m22">Value at row 2, column 2 of the matrix.</param>
        /// <param name="m23">Value at row 2, column 3 of the matrix.</param>
        /// <param name="m31">Value at row 3, column 1 of the matrix.</param>
        /// <param name="m32">Value at row 3, column 2 of the matrix.</param>
        /// <param name="m33">Value at row 3, column 3 of the matrix.</param>
        public Matrix3x3(sfloat m11, sfloat m12, sfloat m13, sfloat m21, sfloat m22, sfloat m23, sfloat m31, sfloat m32, sfloat m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        /// <summary>
        /// Gets the 3x3 identity matrix.
        /// </summary>
        public static Matrix3x3 Identity
        {
            get { return new Matrix3x3(
                sfloat.One, sfloat.Zero, sfloat.Zero,
                sfloat.Zero, sfloat.One, sfloat.Zero,
                sfloat.Zero, sfloat.Zero, sfloat.One
                ); }
        }


        /// <summary>
        /// Gets or sets the forward vector of the matrix.
        /// </summary>
        public Vector3 Forward
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = M31;
                vector.Y = M32;
                vector.Z = M33;
                return vector;
            }
            set
            {
                M31 = value.X;
                M32 = value.Y;
                M33 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the down vector of the matrix.
        /// </summary>
        public Vector3 Down
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = -M21;
                vector.Y = -M22;
                vector.Z = -M23;
                return vector;
            }
            set
            {
                M21 = -value.X;
                M22 = -value.Y;
                M23 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the backward vector of the matrix.
        /// </summary>
        public Vector3 Backward
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = -M31;
                vector.Y = -M32;
                vector.Z = -M33;
                return vector;
            }
            set
            {
                M31 = -value.X;
                M32 = -value.Y;
                M33 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the left vector of the matrix.
        /// </summary>
        public Vector3 Left
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = -M11;
                vector.Y = -M12;
                vector.Z = -M13;
                return vector;
            }
            set
            {
                M11 = -value.X;
                M12 = -value.Y;
                M13 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the right vector of the matrix.
        /// </summary>
        public Vector3 Right
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = M11;
                vector.Y = M12;
                vector.Z = M13;
                return vector;
            }
            set
            {
                M11 = value.X;
                M12 = value.Y;
                M13 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the up vector of the matrix.
        /// </summary>
        public Vector3 Up
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = M21;
                vector.Y = M22;
                vector.Z = M23;
                return vector;
            }
            set
            {
                M21 = value.X;
                M22 = value.Y;
                M23 = value.Z;
            }
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix3x3 a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            sfloat m11 = a.M11 + b.M11;
            sfloat m12 = a.M12 + b.M12;
            sfloat m13 = a.M13 + b.M13;

            sfloat m21 = a.M21 + b.M21;
            sfloat m22 = a.M22 + b.M22;
            sfloat m23 = a.M23 + b.M23;

            sfloat m31 = a.M31 + b.M31;
            sfloat m32 = a.M32 + b.M32;
            sfloat m33 = a.M33 + b.M33;

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            sfloat m11 = a.M11 + b.M11;
            sfloat m12 = a.M12 + b.M12;
            sfloat m13 = a.M13 + b.M13;

            sfloat m21 = a.M21 + b.M21;
            sfloat m22 = a.M22 + b.M22;
            sfloat m23 = a.M23 + b.M23;

            sfloat m31 = a.M31 + b.M31;
            sfloat m32 = a.M32 + b.M32;
            sfloat m33 = a.M33 + b.M33;

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix3x3 a, ref Matrix b, out Matrix3x3 result)
        {
            sfloat m11 = a.M11 + b.M11;
            sfloat m12 = a.M12 + b.M12;
            sfloat m13 = a.M13 + b.M13;

            sfloat m21 = a.M21 + b.M21;
            sfloat m22 = a.M22 + b.M22;
            sfloat m23 = a.M23 + b.M23;

            sfloat m31 = a.M31 + b.M31;
            sfloat m32 = a.M32 + b.M32;
            sfloat m33 = a.M33 + b.M33;

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix a, ref Matrix b, out Matrix3x3 result)
        {
            sfloat m11 = a.M11 + b.M11;
            sfloat m12 = a.M12 + b.M12;
            sfloat m13 = a.M13 + b.M13;

            sfloat m21 = a.M21 + b.M21;
            sfloat m22 = a.M22 + b.M22;
            sfloat m23 = a.M23 + b.M23;

            sfloat m31 = a.M31 + b.M31;
            sfloat m32 = a.M32 + b.M32;
            sfloat m33 = a.M33 + b.M33;

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Creates a skew symmetric matrix M from vector A such that M * B for some other vector B is equivalent to the cross product of A and B.
        /// </summary>
        /// <param name="v">Vector to base the matrix on.</param>
        /// <param name="result">Skew-symmetric matrix result.</param>
        public static void CreateCrossProduct(ref Vector3 v, out Matrix3x3 result)
        {
            result.M11 = sfloat.Zero;
            result.M12 = -v.Z;
            result.M13 = v.Y;
            result.M21 = v.Z;
            result.M22 = sfloat.Zero;
            result.M23 = -v.X;
            result.M31 = -v.Y;
            result.M32 = v.X;
            result.M33 = sfloat.Zero;
        }

        /// <summary>
        /// Creates a 3x3 matrix from an XNA 4x4 matrix.
        /// </summary>
        /// <param name="matrix4X4">Matrix to extract a 3x3 matrix from.</param>
        /// <param name="matrix3X3">Upper 3x3 matrix extracted from the XNA matrix.</param>
        public static void CreateFromMatrix(ref Matrix matrix4X4, out Matrix3x3 matrix3X3)
        {
            matrix3X3.M11 = matrix4X4.M11;
            matrix3X3.M12 = matrix4X4.M12;
            matrix3X3.M13 = matrix4X4.M13;

            matrix3X3.M21 = matrix4X4.M21;
            matrix3X3.M22 = matrix4X4.M22;
            matrix3X3.M23 = matrix4X4.M23;

            matrix3X3.M31 = matrix4X4.M31;
            matrix3X3.M32 = matrix4X4.M32;
            matrix3X3.M33 = matrix4X4.M33;
        }
        /// <summary>
        /// Creates a 3x3 matrix from an XNA 4x4 matrix.
        /// </summary>
        /// <param name="matrix4X4">Matrix to extract a 3x3 matrix from.</param>
        /// <returns>Upper 3x3 matrix extracted from the XNA matrix.</returns>
        public static Matrix3x3 CreateFromMatrix(Matrix matrix4X4)
        {
            Matrix3x3 matrix3X3;
            matrix3X3.M11 = matrix4X4.M11;
            matrix3X3.M12 = matrix4X4.M12;
            matrix3X3.M13 = matrix4X4.M13;

            matrix3X3.M21 = matrix4X4.M21;
            matrix3X3.M22 = matrix4X4.M22;
            matrix3X3.M23 = matrix4X4.M23;

            matrix3X3.M31 = matrix4X4.M31;
            matrix3X3.M32 = matrix4X4.M32;
            matrix3X3.M33 = matrix4X4.M33;
            return matrix3X3;
        }

        /// <summary>
        /// Constructs a uniform scaling matrix.
        /// </summary>
        /// <param name="scale">Value to use in the diagonal.</param>
        /// <param name="matrix">Scaling matrix.</param>
        public static void CreateScale(sfloat scale, out Matrix3x3 matrix)
        {
            matrix = new Matrix3x3 {M11 = scale, M22 = scale, M33 = scale};
        }

        /// <summary>
        /// Constructs a uniform scaling matrix.
        /// </summary>
        /// <param name="scale">Value to use in the diagonal.</param>
        /// <returns>Scaling matrix.</returns>
        public static Matrix3x3 CreateScale(sfloat scale)
        {
            var matrix = new Matrix3x3 {M11 = scale, M22 = scale, M33 = scale};
            return matrix;
        }

        /// <summary>
        /// Constructs a non-uniform scaling matrix.
        /// </summary>
        /// <param name="scale">Values defining the axis scales.</param>
        /// <param name="matrix">Scaling matrix.</param>
        public static void CreateScale(ref Vector3 scale, out Matrix3x3 matrix)
        {
            matrix = new Matrix3x3 {M11 = scale.X, M22 = scale.Y, M33 = scale.Z};
        }

        /// <summary>
        /// Constructs a non-uniform scaling matrix.
        /// </summary>
        /// <param name="scale">Values defining the axis scales.</param>
        /// <returns>Scaling matrix.</returns>
        public static Matrix3x3 CreateScale(ref Vector3 scale)
        {
            var matrix = new Matrix3x3 {M11 = scale.X, M22 = scale.Y, M33 = scale.Z};
            return matrix;
        }


        /// <summary>
        /// Constructs a non-uniform scaling matrix.
        /// </summary>
        /// <param name="x">Scaling along the x axis.</param>
        /// <param name="y">Scaling along the y axis.</param>
        /// <param name="z">Scaling along the z axis.</param>
        /// <param name="matrix">Scaling matrix.</param>
        public static void CreateScale(sfloat x, sfloat y, sfloat z, out Matrix3x3 matrix)
        {
            matrix = new Matrix3x3 {M11 = x, M22 = y, M33 = z};
        }

        /// <summary>
        /// Constructs a non-uniform scaling matrix.
        /// </summary>
        /// <param name="x">Scaling along the x axis.</param>
        /// <param name="y">Scaling along the y axis.</param>
        /// <param name="z">Scaling along the z axis.</param>
        /// <returns>Scaling matrix.</returns>
        public static Matrix3x3 CreateScale(sfloat x, sfloat y, sfloat z)
        {
            var matrix = new Matrix3x3 {M11 = x, M22 = y, M33 = z};
            return matrix;
        }

        /// <summary>
        /// Inverts the given matix.
        /// </summary>
        /// <param name="matrix">Matrix to be inverted.</param>
        /// <param name="result">Inverted matrix.</param>
        public static void Invert(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            sfloat determinantInverse = sfloat.One / matrix.Determinant();
            sfloat m11 = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * determinantInverse;
            sfloat m12 = (matrix.M13 * matrix.M32 - matrix.M33 * matrix.M12) * determinantInverse;
            sfloat m13 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * determinantInverse;

            sfloat m21 = (matrix.M23 * matrix.M31 - matrix.M21 * matrix.M33) * determinantInverse;
            sfloat m22 = (matrix.M11 * matrix.M33 - matrix.M13 * matrix.M31) * determinantInverse;
            sfloat m23 = (matrix.M13 * matrix.M21 - matrix.M11 * matrix.M23) * determinantInverse;

            sfloat m31 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * determinantInverse;
            sfloat m32 = (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32) * determinantInverse;
            sfloat m33 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * determinantInverse;

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Inverts the given matix.
        /// </summary>
        /// <param name="matrix">Matrix to be inverted.</param>
        /// <returns>Inverted matrix.</returns>
        public static Matrix3x3 Invert(Matrix3x3 matrix)
        {
            Matrix3x3 toReturn;
            Invert(ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Inverts the largest nonsingular submatrix in the matrix, excluding 2x2's that involve M13 or M31, and excluding 1x1's that include nondiagonal elements.
        /// </summary>
        /// <param name="matrix">Matrix to be inverted.</param>
        /// <param name="result">Inverted matrix.</param>
        public static void AdaptiveInvert(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            int submatrix;
            sfloat determinantInverse = sfloat.One / matrix.AdaptiveDeterminant(out submatrix);
            sfloat m11, m12, m13, m21, m22, m23, m31, m32, m33;
            switch (submatrix)
            {
                case 0: //Full matrix.
                    m11 = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * determinantInverse;
                    m12 = (matrix.M13 * matrix.M32 - matrix.M33 * matrix.M12) * determinantInverse;
                    m13 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * determinantInverse;

                    m21 = (matrix.M23 * matrix.M31 - matrix.M21 * matrix.M33) * determinantInverse;
                    m22 = (matrix.M11 * matrix.M33 - matrix.M13 * matrix.M31) * determinantInverse;
                    m23 = (matrix.M13 * matrix.M21 - matrix.M11 * matrix.M23) * determinantInverse;

                    m31 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * determinantInverse;
                    m32 = (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32) * determinantInverse;
                    m33 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * determinantInverse;
                    break;
                case 1: //Upper left matrix, m11, m12, m21, m22.
                    m11 = matrix.M22 * determinantInverse;
                    m12 = -matrix.M12 * determinantInverse;
                    m13 = sfloat.Zero;

                    m21 = -matrix.M21 * determinantInverse;
                    m22 = matrix.M11 * determinantInverse;
                    m23 = sfloat.Zero;

                    m31 = sfloat.Zero;
                    m32 = sfloat.Zero;
                    m33 = sfloat.Zero;
                    break;
                case 2: //Lower right matrix, m22, m23, m32, m33.
                    m11 = sfloat.Zero;
                    m12 = sfloat.Zero;
                    m13 = sfloat.Zero;

                    m21 = sfloat.Zero;
                    m22 = matrix.M33 * determinantInverse;
                    m23 = -matrix.M23 * determinantInverse;

                    m31 = sfloat.Zero;
                    m32 = -matrix.M32 * determinantInverse;
                    m33 = matrix.M22 * determinantInverse;
                    break;
                case 3: //Corners, m11, m31, m13, m33.
                    m11 = matrix.M33 * determinantInverse;
                    m12 = sfloat.Zero;
                    m13 = -matrix.M13 * determinantInverse;

                    m21 = sfloat.Zero;
                    m22 = sfloat.Zero;
                    m23 = sfloat.Zero;

                    m31 = -matrix.M31 * determinantInverse;
                    m32 = sfloat.Zero;
                    m33 = matrix.M11 * determinantInverse;
                    break;
                case 4: //M11
                    m11 = sfloat.One / matrix.M11;
                    m12 = sfloat.Zero;
                    m13 = sfloat.Zero;

                    m21 = sfloat.Zero;
                    m22 = sfloat.Zero;
                    m23 = sfloat.Zero;

                    m31 = sfloat.Zero;
                    m32 = sfloat.Zero;
                    m33 = sfloat.Zero;
                    break;
                case 5: //M22
                    m11 = sfloat.Zero;
                    m12 = sfloat.Zero;
                    m13 = sfloat.Zero;

                    m21 = sfloat.Zero;
                    m22 = sfloat.One / matrix.M22;
                    m23 = sfloat.Zero;

                    m31 = sfloat.Zero;
                    m32 = sfloat.Zero;
                    m33 = sfloat.Zero;
                    break;
                case 6: //M33
                    m11 = sfloat.Zero;
                    m12 = sfloat.Zero;
                    m13 = sfloat.Zero;

                    m21 = sfloat.Zero;
                    m22 = sfloat.Zero;
                    m23 = sfloat.Zero;

                    m31 = sfloat.Zero;
                    m32 = sfloat.Zero;
                    m33 = sfloat.One / matrix.M33;
                    break;
                default: //Completely singular.
                    m11 = sfloat.Zero; 
                    m12 = sfloat.Zero; 
                    m13 = sfloat.Zero; 
                    m21 = sfloat.Zero; 
                    m22 = sfloat.Zero;
                    m23 = sfloat.Zero; 
                    m31 = sfloat.Zero;
                    m32 = sfloat.Zero; 
                    m33 = sfloat.Zero;
                    break;
            }

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// <para>Computes the adjugate transpose of a matrix.</para>
        /// <para>The adjugate transpose A of matrix M is: det(M) * transpose(invert(M))</para>
        /// <para>This is necessary when transforming normals (bivectors) with general linear transformations.</para>
        /// </summary>
        /// <param name="matrix">Matrix to compute the adjugate transpose of.</param>
        /// <param name="result">Adjugate transpose of the input matrix.</param>
        public static void AdjugateTranspose(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            //Despite the relative obscurity of the operation, this is a fairly straightforward operation which is actually faster than a true invert (by virtue of cancellation).
            //Conceptually, this is implemented as transpose(det(M) * invert(M)), but that's perfectly acceptable:
            //1) transpose(invert(M)) == invert(transpose(M)), and
            //2) det(M) == det(transpose(M))
            //This organization makes it clearer that the invert's usual division by determinant drops out.

            sfloat m11 = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32);
            sfloat m12 = (matrix.M13 * matrix.M32 - matrix.M33 * matrix.M12);
            sfloat m13 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13);

            sfloat m21 = (matrix.M23 * matrix.M31 - matrix.M21 * matrix.M33);
            sfloat m22 = (matrix.M11 * matrix.M33 - matrix.M13 * matrix.M31);
            sfloat m23 = (matrix.M13 * matrix.M21 - matrix.M11 * matrix.M23);

            sfloat m31 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31);
            sfloat m32 = (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32);
            sfloat m33 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21);

            //Note transposition.
            result.M11 = m11;
            result.M12 = m21;
            result.M13 = m31;

            result.M21 = m12;
            result.M22 = m22;
            result.M23 = m32;

            result.M31 = m13;
            result.M32 = m23;
            result.M33 = m33;
        }

        /// <summary>
        /// <para>Computes the adjugate transpose of a matrix.</para>
        /// <para>The adjugate transpose A of matrix M is: det(M) * transpose(invert(M))</para>
        /// <para>This is necessary when transforming normals (bivectors) with general linear transformations.</para>
        /// </summary>
        /// <param name="matrix">Matrix to compute the adjugate transpose of.</param>
        /// <returns>Adjugate transpose of the input matrix.</returns>
        public static Matrix3x3 AdjugateTranspose(Matrix3x3 matrix)
        {
            Matrix3x3 toReturn;
            AdjugateTranspose(ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
        {
            Matrix3x3 result;
            Matrix3x3.Multiply(ref a, ref b, out result);
            return result;
        }        

        /// <summary>
        /// Scales all components of the matrix by the given value.
        /// </summary>
        /// <param name="m">First matrix to multiply.</param>
        /// <param name="f">Scaling value to apply to all components of the matrix.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Matrix3x3 operator *(Matrix3x3 m, sfloat f)
        {
            Matrix3x3 result;
            Multiply(ref m, f, out result);
            return result;
        }

        /// <summary>
        /// Scales all components of the matrix by the given value.
        /// </summary>
        /// <param name="m">First matrix to multiply.</param>
        /// <param name="f">Scaling value to apply to all components of the matrix.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Matrix3x3 operator *(sfloat f, Matrix3x3 m)
        {
            Matrix3x3 result;
            Multiply(ref m, f, out result);
            return result;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix3x3 a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            sfloat resultM11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
            sfloat resultM12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
            sfloat resultM13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;

            sfloat resultM21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
            sfloat resultM22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
            sfloat resultM23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;

            sfloat resultM31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
            sfloat resultM32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
            sfloat resultM33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix3x3 a, ref Matrix b, out Matrix3x3 result)
        {
            sfloat resultM11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
            sfloat resultM12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
            sfloat resultM13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;

            sfloat resultM21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
            sfloat resultM22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
            sfloat resultM23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;

            sfloat resultM31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
            sfloat resultM32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
            sfloat resultM33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            sfloat resultM11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
            sfloat resultM12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
            sfloat resultM13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;

            sfloat resultM21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
            sfloat resultM22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
            sfloat resultM23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;

            sfloat resultM31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
            sfloat resultM32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
            sfloat resultM33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
        }


        /// <summary>
        /// Multiplies a transposed matrix with another matrix.
        /// </summary>
        /// <param name="matrix">Matrix to be multiplied.</param>
        /// <param name="transpose">Matrix to be transposed and multiplied.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void MultiplyTransposed(ref Matrix3x3 transpose, ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            sfloat resultM11 = transpose.M11 * matrix.M11 + transpose.M21 * matrix.M21 + transpose.M31 * matrix.M31;
            sfloat resultM12 = transpose.M11 * matrix.M12 + transpose.M21 * matrix.M22 + transpose.M31 * matrix.M32;
            sfloat resultM13 = transpose.M11 * matrix.M13 + transpose.M21 * matrix.M23 + transpose.M31 * matrix.M33;

            sfloat resultM21 = transpose.M12 * matrix.M11 + transpose.M22 * matrix.M21 + transpose.M32 * matrix.M31;
            sfloat resultM22 = transpose.M12 * matrix.M12 + transpose.M22 * matrix.M22 + transpose.M32 * matrix.M32;
            sfloat resultM23 = transpose.M12 * matrix.M13 + transpose.M22 * matrix.M23 + transpose.M32 * matrix.M33;

            sfloat resultM31 = transpose.M13 * matrix.M11 + transpose.M23 * matrix.M21 + transpose.M33 * matrix.M31;
            sfloat resultM32 = transpose.M13 * matrix.M12 + transpose.M23 * matrix.M22 + transpose.M33 * matrix.M32;
            sfloat resultM33 = transpose.M13 * matrix.M13 + transpose.M23 * matrix.M23 + transpose.M33 * matrix.M33;

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
        }

        /// <summary>
        /// Multiplies a matrix with a transposed matrix.
        /// </summary>
        /// <param name="matrix">Matrix to be multiplied.</param>
        /// <param name="transpose">Matrix to be transposed and multiplied.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void MultiplyByTransposed(ref Matrix3x3 matrix, ref Matrix3x3 transpose, out Matrix3x3 result)
        {
            sfloat resultM11 = matrix.M11 * transpose.M11 + matrix.M12 * transpose.M12 + matrix.M13 * transpose.M13;
            sfloat resultM12 = matrix.M11 * transpose.M21 + matrix.M12 * transpose.M22 + matrix.M13 * transpose.M23;
            sfloat resultM13 = matrix.M11 * transpose.M31 + matrix.M12 * transpose.M32 + matrix.M13 * transpose.M33;

            sfloat resultM21 = matrix.M21 * transpose.M11 + matrix.M22 * transpose.M12 + matrix.M23 * transpose.M13;
            sfloat resultM22 = matrix.M21 * transpose.M21 + matrix.M22 * transpose.M22 + matrix.M23 * transpose.M23;
            sfloat resultM23 = matrix.M21 * transpose.M31 + matrix.M22 * transpose.M32 + matrix.M23 * transpose.M33;

            sfloat resultM31 = matrix.M31 * transpose.M11 + matrix.M32 * transpose.M12 + matrix.M33 * transpose.M13;
            sfloat resultM32 = matrix.M31 * transpose.M21 + matrix.M32 * transpose.M22 + matrix.M33 * transpose.M23;
            sfloat resultM33 = matrix.M31 * transpose.M31 + matrix.M32 * transpose.M32 + matrix.M33 * transpose.M33;

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
        }

        /// <summary>
        /// Scales all components of the matrix.
        /// </summary>
        /// <param name="matrix">Matrix to scale.</param>
        /// <param name="scale">Amount to scale.</param>
        /// <param name="result">Scaled matrix.</param>
        public static void Multiply(ref Matrix3x3 matrix, sfloat scale, out Matrix3x3 result)
        {
            result.M11 = matrix.M11 * scale;
            result.M12 = matrix.M12 * scale;
            result.M13 = matrix.M13 * scale;

            result.M21 = matrix.M21 * scale;
            result.M22 = matrix.M22 * scale;
            result.M23 = matrix.M23 * scale;

            result.M31 = matrix.M31 * scale;
            result.M32 = matrix.M32 * scale;
            result.M33 = matrix.M33 * scale;
        }

        /// <summary>
        /// Negates every element in the matrix.
        /// </summary>
        /// <param name="matrix">Matrix to negate.</param>
        /// <param name="result">Negated matrix.</param>
        public static void Negate(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            result.M11 = -matrix.M11;
            result.M12 = -matrix.M12;
            result.M13 = -matrix.M13;

            result.M21 = -matrix.M21;
            result.M22 = -matrix.M22;
            result.M23 = -matrix.M23;

            result.M31 = -matrix.M31;
            result.M32 = -matrix.M32;
            result.M33 = -matrix.M33;
        }

        /// <summary>
        /// Subtracts the two matrices from each other on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to subtract.</param>
        /// <param name="b">Second matrix to subtract.</param>
        /// <param name="result">Difference of the two matrices.</param>
        public static void Subtract(ref Matrix3x3 a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            sfloat m11 = a.M11 - b.M11;
            sfloat m12 = a.M12 - b.M12;
            sfloat m13 = a.M13 - b.M13;

            sfloat m21 = a.M21 - b.M21;
            sfloat m22 = a.M22 - b.M22;
            sfloat m23 = a.M23 - b.M23;

            sfloat m31 = a.M31 - b.M31;
            sfloat m32 = a.M32 - b.M32;
            sfloat m33 = a.M33 - b.M33;

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Creates a 4x4 matrix from a 3x3 matrix.
        /// </summary>
        /// <param name="a">3x3 matrix.</param>
        /// <param name="b">Created 4x4 matrix.</param>
        public static void ToMatrix4X4(ref Matrix3x3 a, out Matrix b)
        {
#if !WINDOWS
            b = new Matrix();
#endif
            b.M11 = a.M11;
            b.M12 = a.M12;
            b.M13 = a.M13;

            b.M21 = a.M21;
            b.M22 = a.M22;
            b.M23 = a.M23;

            b.M31 = a.M31;
            b.M32 = a.M32;
            b.M33 = a.M33;

            b.M44 = sfloat.One;
            b.M14 = sfloat.Zero;
            b.M24 = sfloat.Zero;
            b.M34 = sfloat.Zero;
            b.M41 = sfloat.Zero;
            b.M42 = sfloat.Zero;
            b.M43 = sfloat.Zero;
        }

        /// <summary>
        /// Creates a 4x4 matrix from a 3x3 matrix.
        /// </summary>
        /// <param name="a">3x3 matrix.</param>
        /// <returns>Created 4x4 matrix.</returns>
        public static Matrix ToMatrix4X4(Matrix3x3 a)
        {
#if !WINDOWS
            Matrix b = new Matrix();
#else
            Matrix b;
#endif
            b.M11 = a.M11;
            b.M12 = a.M12;
            b.M13 = a.M13;

            b.M21 = a.M21;
            b.M22 = a.M22;
            b.M23 = a.M23;

            b.M31 = a.M31;
            b.M32 = a.M32;
            b.M33 = a.M33;

            b.M44 = sfloat.One;
            b.M14 = sfloat.Zero;
            b.M24 = sfloat.Zero;
            b.M34 = sfloat.Zero;
            b.M41 = sfloat.Zero;
            b.M42 = sfloat.Zero;
            b.M43 = sfloat.Zero;
            return b;
        }
        
        /// <summary>
        /// Transforms the vector by the matrix.
        /// </summary>
        /// <param name="v">Vector3 to transform.</param>
        /// <param name="matrix">Matrix to use as the transformation.</param>
        /// <param name="result">Product of the transformation.</param>
        public static void Transform(ref Vector3 v, ref Matrix3x3 matrix, out Vector3 result)
        {
            sfloat vX = v.X;
            sfloat vY = v.Y;
            sfloat vZ = v.Z;
#if !WINDOWS
            result = new Vector3();
#endif
            result.X = vX * matrix.M11 + vY * matrix.M21 + vZ * matrix.M31;
            result.Y = vX * matrix.M12 + vY * matrix.M22 + vZ * matrix.M32;
            result.Z = vX * matrix.M13 + vY * matrix.M23 + vZ * matrix.M33;
        }


        /// <summary>
        /// Transforms the vector by the matrix.
        /// </summary>
        /// <param name="v">Vector3 to transform.</param>
        /// <param name="matrix">Matrix to use as the transformation.</param>
        /// <returns>Product of the transformation.</returns>
        public static Vector3 Transform(Vector3 v, Matrix3x3 matrix)
        {
            Vector3 result;
#if !WINDOWS
            result = new Vector3();
#endif
            sfloat vX = v.X;
            sfloat vY = v.Y;
            sfloat vZ = v.Z;

            result.X = vX * matrix.M11 + vY * matrix.M21 + vZ * matrix.M31;
            result.Y = vX * matrix.M12 + vY * matrix.M22 + vZ * matrix.M32;
            result.Z = vX * matrix.M13 + vY * matrix.M23 + vZ * matrix.M33;
            return result;
        }

        /// <summary>
        /// Transforms the vector by the matrix's transpose.
        /// </summary>
        /// <param name="v">Vector3 to transform.</param>
        /// <param name="matrix">Matrix to use as the transformation transpose.</param>
        /// <param name="result">Product of the transformation.</param>
        public static void TransformTranspose(ref Vector3 v, ref Matrix3x3 matrix, out Vector3 result)
        {
            sfloat vX = v.X;
            sfloat vY = v.Y;
            sfloat vZ = v.Z;
#if !WINDOWS
            result = new Vector3();
#endif
            result.X = vX * matrix.M11 + vY * matrix.M12 + vZ * matrix.M13;
            result.Y = vX * matrix.M21 + vY * matrix.M22 + vZ * matrix.M23;
            result.Z = vX * matrix.M31 + vY * matrix.M32 + vZ * matrix.M33;
        }

        /// <summary>
        /// Transforms the vector by the matrix's transpose.
        /// </summary>
        /// <param name="v">Vector3 to transform.</param>
        /// <param name="matrix">Matrix to use as the transformation transpose.</param>
        /// <returns>Product of the transformation.</returns>
        public static Vector3 TransformTranspose(Vector3 v, Matrix3x3 matrix)
        {
            sfloat vX = v.X;
            sfloat vY = v.Y;
            sfloat vZ = v.Z;
            Vector3 result;
#if !WINDOWS
            result = new Vector3();
#endif
            result.X = vX * matrix.M11 + vY * matrix.M12 + vZ * matrix.M13;
            result.Y = vX * matrix.M21 + vY * matrix.M22 + vZ * matrix.M23;
            result.Z = vX * matrix.M31 + vY * matrix.M32 + vZ * matrix.M33;
            return result;
        }

        /// <summary>
        /// Computes the transposed matrix of a matrix.
        /// </summary>
        /// <param name="matrix">Matrix to transpose.</param>
        /// <param name="result">Transposed matrix.</param>
        public static void Transpose(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            sfloat m21 = matrix.M12;
            sfloat m31 = matrix.M13;
            sfloat m12 = matrix.M21;
            sfloat m32 = matrix.M23;
            sfloat m13 = matrix.M31;
            sfloat m23 = matrix.M32;

            result.M11 = matrix.M11;
            result.M12 = m12;
            result.M13 = m13;
            result.M21 = m21;
            result.M22 = matrix.M22;
            result.M23 = m23;
            result.M31 = m31;
            result.M32 = m32;
            result.M33 = matrix.M33;
        }

        /// <summary>
        /// Computes the transposed matrix of a matrix.
        /// </summary>
        /// <param name="matrix">Matrix to transpose.</param>
        /// <param name="result">Transposed matrix.</param>
        public static void Transpose(ref Matrix matrix, out Matrix3x3 result)
        {
            sfloat m21 = matrix.M12;
            sfloat m31 = matrix.M13;
            sfloat m12 = matrix.M21;
            sfloat m32 = matrix.M23;
            sfloat m13 = matrix.M31;
            sfloat m23 = matrix.M32;

            result.M11 = matrix.M11;
            result.M12 = m12;
            result.M13 = m13;
            result.M21 = m21;
            result.M22 = matrix.M22;
            result.M23 = m23;
            result.M31 = m31;
            result.M32 = m32;
            result.M33 = matrix.M33;
        }
       
        /// <summary>
        /// Transposes the matrix in-place.
        /// </summary>
        public void Transpose()
        {
            sfloat intermediate = M12;
            M12 = M21;
            M21 = intermediate;

            intermediate = M13;
            M13 = M31;
            M31 = intermediate;

            intermediate = M23;
            M23 = M32;
            M32 = intermediate;
        }


        /// <summary>
        /// Creates a string representation of the matrix.
        /// </summary>
        /// <returns>A string representation of the matrix.</returns>
        public override string ToString()
        {
            return "{" + M11 + ", " + M12 + ", " + M13 + "} " +
                   "{" + M21 + ", " + M22 + ", " + M23 + "} " +
                   "{" + M31 + ", " + M32 + ", " + M33 + "}";
        }

        /// <summary>
        /// Calculates the determinant of the matrix.
        /// </summary>
        /// <returns>The matrix's determinant.</returns>
        public sfloat Determinant()
        {
            return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 -
                   M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
        }

        /// <summary>
        /// Calculates the determinant of largest nonsingular submatrix, excluding 2x2's that involve M13 or M31, and excluding all 1x1's that involve nondiagonal elements.
        /// </summary>
        /// <param name="subMatrixCode">Represents the submatrix that was used to compute the determinant.
        /// 0 is the full 3x3.  1 is the upper left 2x2.  2 is the lower right 2x2.  3 is the four corners.
        /// 4 is M11.  5 is M22.  6 is M33.</param>
        /// <returns>The matrix's determinant.</returns>
        internal sfloat AdaptiveDeterminant(out int subMatrixCode)
        {
            //Try the full matrix first.
            sfloat determinant = M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 -
                                M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
            if (determinant != sfloat.Zero) //This could be a little numerically flimsy.  Fortunately, the way this method is used, that doesn't matter!
            {
                subMatrixCode = 0;
                return determinant;
            }
            //Try m11, m12, m21, m22.
            determinant = M11 * M22 - M12 * M21;
            if (determinant != sfloat.Zero)
            {
                subMatrixCode = 1;
                return determinant;
            }
            //Try m22, m23, m32, m33.
            determinant = M22 * M33 - M23 * M32;
            if (determinant != sfloat.Zero)
            {
                subMatrixCode = 2;
                return determinant;
            }
            //Try m11, m13, m31, m33.
            determinant = M11 * M33 - M13 * M12;
            if (determinant != sfloat.Zero)
            {
                subMatrixCode = 3;
                return determinant;
            }
            //Try m11.
            if (M11 != sfloat.Zero)
            {
                subMatrixCode = 4;
                return M11;
            }
            //Try m22.
            if (M22 != sfloat.Zero)
            {
                subMatrixCode = 5;
                return M22;
            }
            //Try m33.
            if (M33 != sfloat.Zero)
            {
                subMatrixCode = 6;
                return M33;
            }
            //It's completely singular!
            subMatrixCode = -1;
            return sfloat.Zero;
        }
        
        /// <summary>
        /// Creates a 3x3 matrix representing the orientation stored in the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to use to create a matrix.</param>
        /// <param name="result">Matrix representing the quaternion's orientation.</param>
        public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix3x3 result)
        {
            sfloat qX2 = quaternion.X + quaternion.X;
            sfloat qY2 = quaternion.Y + quaternion.Y;
            sfloat qZ2 = quaternion.Z + quaternion.Z;
            sfloat XX = qX2 * quaternion.X;
            sfloat YY = qY2 * quaternion.Y;
            sfloat ZZ = qZ2 * quaternion.Z;
            sfloat XY = qX2 * quaternion.Y;
            sfloat XZ = qX2 * quaternion.Z;
            sfloat XW = qX2 * quaternion.W;
            sfloat YZ = qY2 * quaternion.Z;
            sfloat YW = qY2 * quaternion.W;
            sfloat ZW = qZ2 * quaternion.W;

            result.M11 = sfloat.One - YY - ZZ;
            result.M21 = XY - ZW;
            result.M31 = XZ + YW;

            result.M12 = XY + ZW;
            result.M22 = sfloat.One - XX - ZZ;
            result.M32 = YZ - XW;

            result.M13 = XZ - YW;
            result.M23 = YZ + XW;
            result.M33 = sfloat.One - XX - YY;
        }

        /// <summary>
        /// Creates a 3x3 matrix representing the orientation stored in the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to use to create a matrix.</param>
        /// <returns>Matrix representing the quaternion's orientation.</returns>
        public static Matrix3x3 CreateFromQuaternion(Quaternion quaternion)
        {
            Matrix3x3 result;
            CreateFromQuaternion(ref quaternion, out result);
            return result;
        }

        /// <summary>
        /// Computes the outer product of the given vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <param name="result">Outer product result.</param>
        public static void CreateOuterProduct(ref Vector3 a, ref Vector3 b, out Matrix3x3 result)
        {
            result.M11 = a.X * b.X;
            result.M12 = a.X * b.Y;
            result.M13 = a.X * b.Z;

            result.M21 = a.Y * b.X;
            result.M22 = a.Y * b.Y;
            result.M23 = a.Y * b.Z;

            result.M31 = a.Z * b.X;
            result.M32 = a.Z * b.Y;
            result.M33 = a.Z * b.Z;
        }

        /// <summary>
        /// Creates a matrix representing a rotation of a given angle around a given axis.
        /// </summary>
        /// <param name="axis">Axis around which to rotate.</param>
        /// <param name="angle">Amount to rotate.</param>
        /// <returns>Matrix representing the rotation.</returns>
        public static Matrix3x3 CreateFromAxisAngle(Vector3 axis, sfloat angle)
        {
            Matrix3x3 toReturn;
            CreateFromAxisAngle(ref axis, angle, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Creates a matrix representing a rotation of a given angle around a given axis.
        /// </summary>
        /// <param name="axis">Axis around which to rotate.</param>
        /// <param name="angle">Amount to rotate.</param>
        /// <param name="result">Matrix representing the rotation.</param>
        public static void CreateFromAxisAngle(ref Vector3 axis, sfloat angle, out Matrix3x3 result)
        {
            sfloat xx = axis.X * axis.X;
            sfloat yy = axis.Y * axis.Y;
            sfloat zz = axis.Z * axis.Z;
            sfloat xy = axis.X * axis.Y;
            sfloat xz = axis.X * axis.Z;
            sfloat yz = axis.Y * axis.Z;

            sfloat sinAngle = libm.sinf(angle);
            sfloat oneMinusCosAngle = sfloat.One - libm.cosf(angle);

            result.M11 = sfloat.One + oneMinusCosAngle * (xx - sfloat.One);
            result.M21 = -axis.Z * sinAngle + oneMinusCosAngle * xy;
            result.M31 = axis.Y * sinAngle + oneMinusCosAngle * xz;

            result.M12 = axis.Z * sinAngle + oneMinusCosAngle * xy;
            result.M22 = sfloat.One + oneMinusCosAngle * (yy - sfloat.One);
            result.M32 = -axis.X * sinAngle + oneMinusCosAngle * yz;

            result.M13 = -axis.Y * sinAngle + oneMinusCosAngle * xz;
            result.M23 = axis.X * sinAngle + oneMinusCosAngle * yz;
            result.M33 = sfloat.One + oneMinusCosAngle * (zz - sfloat.One);
        }






    }
}