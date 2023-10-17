﻿



using BEPUutilities.FixedMath;

namespace BEPUutilities
{
    /// <summary>
    /// 2 row, 2 column matrix.
    /// </summary>
    public struct Matrix2x2
    {
        /// <summary>
        /// Value at row 1, column 1 of the matrix.
        /// </summary>
        public fint M11;

        /// <summary>
        /// Value at row 1, column 2 of the matrix.
        /// </summary>
        public fint M12;

        /// <summary>
        /// Value at row 2, column 1 of the matrix.
        /// </summary>
        public fint M21;

        /// <summary>
        /// Value at row 2, column 2 of the matrix.
        /// </summary>
        public fint M22;


        /// <summary>
        /// Constructs a new 2 row, 2 column matrix.
        /// </summary>
        /// <param name="m11">Value at row 1, column 1 of the matrix.</param>
        /// <param name="m12">Value at row 1, column 2 of the matrix.</param>
        /// <param name="m21">Value at row 2, column 1 of the matrix.</param>
        /// <param name="m22">Value at row 2, column 2 of the matrix.</param>
        public Matrix2x2(fint m11, fint m12, fint m21, fint m22)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
        }

        /// <summary>
        /// Gets the 2x2 identity matrix.
        /// </summary>
        public static Matrix2x2 Identity
        {
            get { return new Matrix2x2((fint)1, (fint)0, (fint)0, (fint)1); }
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix2x2 a, ref Matrix2x2 b, out Matrix2x2 result)
        {
            fint m11 = a.M11 + b.M11;
            fint m12 = a.M12 + b.M12;

            fint m21 = a.M21 + b.M21;
            fint m22 = a.M22 + b.M22;

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix a, ref Matrix2x2 b, out Matrix2x2 result)
        {
            fint m11 = a.M11 + b.M11;
            fint m12 = a.M12 + b.M12;

            fint m21 = a.M21 + b.M21;
            fint m22 = a.M22 + b.M22;

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix2x2 a, ref Matrix b, out Matrix2x2 result)
        {
            fint m11 = a.M11 + b.M11;
            fint m12 = a.M12 + b.M12;

            fint m21 = a.M21 + b.M21;
            fint m22 = a.M22 + b.M22;

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix a, ref Matrix b, out Matrix2x2 result)
        {
            fint m11 = a.M11 + b.M11;
            fint m12 = a.M12 + b.M12;

            fint m21 = a.M21 + b.M21;
            fint m22 = a.M22 + b.M22;

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;
        }

        /// <summary>
        /// Constructs a uniform scaling matrix.
        /// </summary>
        /// <param name="scale">Value to use in the diagonal.</param>
        /// <param name="matrix">Scaling matrix.</param>
        public static void CreateScale(fint scale, out Matrix2x2 matrix)
        {
            matrix.M11 = scale;
            matrix.M22 = scale;

            matrix.M12 = (fint)0;
            matrix.M21 = (fint)0;
        }


        /// <summary>
        /// Inverts the given matix.
        /// </summary>
        /// <param name="matrix">Matrix to be inverted.</param>
        /// <param name="result">Inverted matrix.</param>
        public static void Invert(ref Matrix2x2 matrix, out Matrix2x2 result)
        {
            fint determinantInverse = (fint)1 / (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21);
            fint m11 = matrix.M22 * determinantInverse;
            fint m12 = -matrix.M12 * determinantInverse;

            fint m21 = -matrix.M21 * determinantInverse;
            fint m22 = matrix.M11 * determinantInverse;

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix2x2 a, ref Matrix2x2 b, out Matrix2x2 result)
        {
            fint resultM11 = a.M11 * b.M11 + a.M12 * b.M21;
            fint resultM12 = a.M11 * b.M12 + a.M12 * b.M22;

            fint resultM21 = a.M21 * b.M11 + a.M22 * b.M21;
            fint resultM22 = a.M21 * b.M12 + a.M22 * b.M22;

            result.M11 = resultM11;
            result.M12 = resultM12;

            result.M21 = resultM21;
            result.M22 = resultM22;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix2x2 a, ref Matrix b, out Matrix2x2 result)
        {
            fint resultM11 = a.M11 * b.M11 + a.M12 * b.M21;
            fint resultM12 = a.M11 * b.M12 + a.M12 * b.M22;

            fint resultM21 = a.M21 * b.M11 + a.M22 * b.M21;
            fint resultM22 = a.M21 * b.M12 + a.M22 * b.M22;

            result.M11 = resultM11;
            result.M12 = resultM12;

            result.M21 = resultM21;
            result.M22 = resultM22;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix a, ref Matrix2x2 b, out Matrix2x2 result)
        {
            fint resultM11 = a.M11 * b.M11 + a.M12 * b.M21;
            fint resultM12 = a.M11 * b.M12 + a.M12 * b.M22;

            fint resultM21 = a.M21 * b.M11 + a.M22 * b.M21;
            fint resultM22 = a.M21 * b.M12 + a.M22 * b.M22;

            result.M11 = resultM11;
            result.M12 = resultM12;

            result.M21 = resultM21;
            result.M22 = resultM22;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix2x3 a, ref Matrix3x2 b, out Matrix2x2 result)
        {
            result.M11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
            result.M12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;

            result.M21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
            result.M22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
        }

        /// <summary>
        /// Negates every element in the matrix.
        /// </summary>
        /// <param name="matrix">Matrix to negate.</param>
        /// <param name="result">Negated matrix.</param>
        public static void Negate(ref Matrix2x2 matrix, out Matrix2x2 result)
        {
            fint m11 = -matrix.M11;
            fint m12 = -matrix.M12;

            fint m21 = -matrix.M21;
            fint m22 = -matrix.M22;


            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;
        }

        /// <summary>
        /// Subtracts the two matrices from each other on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to subtract.</param>
        /// <param name="b">Second matrix to subtract.</param>
        /// <param name="result">Difference of the two matrices.</param>
        public static void Subtract(ref Matrix2x2 a, ref Matrix2x2 b, out Matrix2x2 result)
        {
            fint m11 = a.M11 - b.M11;
            fint m12 = a.M12 - b.M12;

            fint m21 = a.M21 - b.M21;
            fint m22 = a.M22 - b.M22;

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;
        }

        /// <summary>
        /// Transforms the vector by the matrix.
        /// </summary>
        /// <param name="v">Vector2 to transform.</param>
        /// <param name="matrix">Matrix to use as the transformation.</param>
        /// <param name="result">Product of the transformation.</param>
        public static void Transform(ref Vector2 v, ref Matrix2x2 matrix, out Vector2 result)
        {
            fint vX = v.X;
            fint vY = v.Y;
#if !WINDOWS
            result = new Vector2();
#endif
            result.X = vX * matrix.M11 + vY * matrix.M21;
            result.Y = vX * matrix.M12 + vY * matrix.M22;
        }

        /// <summary>
        /// Computes the transposed matrix of a matrix.
        /// </summary>
        /// <param name="matrix">Matrix to transpose.</param>
        /// <param name="result">Transposed matrix.</param>
        public static void Transpose(ref Matrix2x2 matrix, out Matrix2x2 result)
        {
            fint m21 = matrix.M12;

            result.M11 = matrix.M11;
            result.M12 = matrix.M21;

            result.M21 = m21;
            result.M22 = matrix.M22;
        }
        
        /// <summary>
        /// Transposes the matrix in-place.
        /// </summary>
        public void Transpose()
        {
            fint m21 = M21;
            M21 = M12;
            M12 = m21;
        }      

        /// <summary>
        /// Creates a string representation of the matrix.
        /// </summary>
        /// <returns>A string representation of the matrix.</returns>
        public override string ToString()
        {
            return "{" + M11 + ", " + M12 + "} " +
                   "{" + M21 + ", " + M22 + "}";
        }

        /// <summary>
        /// Calculates the determinant of the matrix.
        /// </summary>
        /// <returns>The matrix's determinant.</returns>
        public fint Determinant()
        {
            return M11 * M22 - M12 * M21;
        }
    }
}