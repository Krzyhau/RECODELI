using BEPUutilities;
using BEPUutilities.FixedMath;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms.GJK
{


    ///<summary>
    /// GJK simplex supporting boolean intersection tests.
    ///</summary>
    public struct SimpleSimplex
    {
        ///<summary>
        /// First vertex of the simplex.
        ///</summary>
        public Vector3 A;
        ///<summary>
        /// Second vertex of the simplex.
        ///</summary>
        public Vector3 B;
        ///<summary>
        /// Third vertex of the simplex.
        ///</summary>
        public Vector3 C;
        ///<summary>
        /// Fourth vertex of the simplex.
        ///</summary>
        public Vector3 D;
        ///<summary>
        /// Current state of the simplex.
        ///</summary>
        public SimplexState State;

        ///<summary>
        /// Gets the point on the simplex closest to the origin.
        ///</summary>
        ///<param name="point">Closest point to the origin.</param>
        ///<returns>Whether or not the simplex encloses the origin.</returns>
        public bool GetPointClosestToOrigin(out Vector3 point)
        {
            //This method finds the closest point on the simplex to the origin.
            //Barycentric coordinates are assigned to the MinimumNormCoordinates as necessary to perform the inclusion calculation.
            //If the simplex is a tetrahedron and found to be overlapping the origin, the function returns true to tell the caller to terminate.
            //Elements of the simplex that are not used to determine the point of minimum norm are removed from the simplex.

            switch (State)
            {

                case SimplexState.Point:
                    point = A;
                    break;
                case SimplexState.Segment:
                    GetPointOnSegmentClosestToOrigin(out point);
                    break;
                case SimplexState.Triangle:
                    GetPointOnTriangleClosestToOrigin(out point);
                    break;
                case SimplexState.Tetrahedron:
                    return GetPointOnTetrahedronClosestToOrigin(out point);
                default:
                    point = Toolbox.ZeroVector;
                    break;


            }
            return false;
        }

        ///<summary>
        /// Gets the closest point on the segment to the origin.
        ///</summary>
        ///<param name="point">Closest point.</param>
        public void GetPointOnSegmentClosestToOrigin(out Vector3 point)
        {
            Vector3 segmentDisplacement;
            Vector3.Subtract(ref B, ref A, out segmentDisplacement);
            fint dotA;
            Vector3.Dot(ref segmentDisplacement, ref A, out dotA);

            //Inside segment.
            fint V = -dotA / segmentDisplacement.LengthSquared();

            Vector3.Multiply(ref segmentDisplacement, V, out point);
            Vector3.Add(ref point, ref A, out point);

            //if (dotB > 0)
            //{
            //}
            //else
            //{
            //    //It is not possible to be anywhere but within the segment in a 'boolean' GJK, where it early outs as soon as a separating axis is found.

            //    //Outside B.
            //    //Remove current A; we're becoming a point.
            //    A = B;
            //    State = SimplexState.Point;

            //    point = A;
            //}
            //It can never be outside A! 
            //That would mean that the origin is LESS extreme along the search direction than our extreme point--- our search direction would not have picked that direction.
        }

        ///<summary>
        /// Gets the closest point on the triangle to the origin.
        ///</summary>
        ///<param name="point">Closest point.</param>
        public void GetPointOnTriangleClosestToOrigin(out Vector3 point)
        {
            Vector3 ab, ac;
            Vector3.Subtract(ref B, ref A, out ab);
            Vector3.Subtract(ref C, ref A, out ac);
            //The point we are comparing against the triangle is 0,0,0, so instead of storing an "A->P" vector,
            //just use -A.
            //Same for B->, C->P...

            //CAN'T BE IN A'S REGION.

            //CAN'T BE IN B'S REGION.

            //CAN'T BE IN AB'S REGION.

            //Check to see if it's outside C.
            //TODO: Note that in a boolean-style GJK, it shouldn't be possible to be outside C.
            fint d5, d6;
            Vector3.Dot(ref ab, ref C, out d5);
            Vector3.Dot(ref ac, ref C, out d6);
            d5 = -d5;
            d6 = -d6;
            if (d6 >= (fint)0 && d5 <= d6)
            {
                //It is C!
                State = SimplexState.Point;
                A = C;
                point = A;
                return;
            }

            //Check if it's outside AC.            
            fint d1, d2;
            Vector3.Dot(ref ab, ref A, out d1);
            Vector3.Dot(ref ac, ref A, out d2);
            d1 = -d1;
            d2 = -d2;
            fint vb = d5 * d2 - d1 * d6;
            if (vb <= (fint)0 && d2 > (fint)0 && d6 < (fint)0) //Note > instead of >= and < instead of <=; prevents bad denominator
            {
                //Get rid of B.  Compress C into B.
                State = SimplexState.Segment;
                B = C;
                fint V = d2 / (d2 - d6);
                Vector3.Multiply(ref ac, V, out point);
                Vector3.Add(ref point, ref A, out point);
                return;
            }

            //Check if it's outside BC.
            fint d3, d4;
            Vector3.Dot(ref ab, ref B, out d3);
            Vector3.Dot(ref ac, ref B, out d4);
            d3 = -d3;
            d4 = -d4;
            fint va = d3 * d6 - d5 * d4;
            fint d3d4;
            fint d6d5;
            if (va <= (fint)0 && (d3d4 = d4 - d3) > (fint)0 && (d6d5 = d5 - d6) > (fint)0)//Note > instead of >= and < instead of <=; prevents bad denominator
            {
                //Throw away A.  C->A.
                //TODO: Does B->A, C->B work better?
                State = SimplexState.Segment;
                A = C;
                fint U = d3d4 / (d3d4 + d6d5);

                Vector3 bc;
                Vector3.Subtract(ref C, ref B, out bc);
                Vector3.Multiply(ref bc, U, out point);
                Vector3.Add(ref point, ref B, out point);
                return;
            }


            //On the face of the triangle.
            fint vc = d1 * d4 - d3 * d2;
            fint denom = (fint)1 / (va + vb + vc);
            fint v = vb * denom;
            fint w = vc * denom;

            Vector3.Multiply(ref ab, v, out point);
            Vector3 acw;
            Vector3.Multiply(ref ac, w, out acw);
            Vector3.Add(ref A, ref point, out point);
            Vector3.Add(ref point, ref acw, out point);




        }

        ///<summary>
        /// Gets the closest point on the tetrahedron to the origin.
        ///</summary>
        ///<param name="point">Closest point.</param>
        ///<returns>Whether or not the simplex encloses the origin.</returns>
        public bool GetPointOnTetrahedronClosestToOrigin(out Vector3 point)
        {
            //Thanks to the fact that D is new and that we know that the origin is within the extruded
            //triangular prism of ABC (and on the "D" side of ABC),
            //we can immediately ignore voronoi regions:
            //A, B, C, AC, AB, BC, ABC
            //and only consider:
            //D, DA, DB, DC, DAC, DCB, DBA

            //There is some overlap of calculations in this method, since DAC, DCB, and DBA are tested fully.

            SimpleSimplex minimumSimplex = new SimpleSimplex();
            point = new Vector3();
            fint minimumDistance = fint.MaxValue;


            SimpleSimplex candidate;
            fint candidateDistance;
            Vector3 candidatePoint;
            if (TryTetrahedronTriangle(ref A, ref C, ref D, ref B, out candidate, out candidatePoint))
            {
                point = candidatePoint;
                minimumSimplex = candidate;
                minimumDistance = candidatePoint.LengthSquared();
            }

            if (TryTetrahedronTriangle(ref C, ref B, ref D, ref A, out candidate, out candidatePoint) &&
                (candidateDistance = candidatePoint.LengthSquared()) < minimumDistance)
            {
                point = candidatePoint;
                minimumSimplex = candidate;
                minimumDistance = candidateDistance;
            }

            if (TryTetrahedronTriangle(ref B, ref A, ref D, ref C, out candidate, out candidatePoint) &&
                (candidateDistance = candidatePoint.LengthSquared()) < minimumDistance)
            {
                point = candidatePoint;
                minimumSimplex = candidate;
                minimumDistance = candidateDistance;
            }

            if (minimumDistance < fint.MaxValue)
            {
                this = minimumSimplex;
                return false;
            }
            return true;
        }

        private static bool TryTetrahedronTriangle(ref Vector3 A, ref Vector3 B, ref Vector3 C, 
                                                   ref Vector3 otherPoint, out SimpleSimplex simplex, out Vector3 point)
        {
            //Note that there may be some extra terms that can be removed from this process.
            //Some conditions could use less parameters, since it is known that the origin
            //is not 'behind' BC or AC.

            simplex = new SimpleSimplex();
            point = new Vector3();


            Vector3 ab, ac;
            Vector3.Subtract(ref B, ref A, out ab);
            Vector3.Subtract(ref C, ref A, out ac);
            Vector3 normal;
            Vector3.Cross(ref ab, ref ac, out normal);
            fint AdotN, ADdotN;
            Vector3 AD;
            Vector3.Subtract(ref otherPoint, ref A, out AD);
            Vector3.Dot(ref A, ref normal, out AdotN);
            Vector3.Dot(ref AD, ref normal, out ADdotN);

            //If (-A * N) * (AD * N) < 0, D and O are on opposite sides of the triangle.
            if (AdotN * ADdotN > (fint)0)
            {
                //The point we are comparing against the triangle is 0,0,0, so instead of storing an "A->P" vector,
                //just use -A.
                //Same for B->, C->P...

                //CAN'T BE IN A'S REGION.

                //CAN'T BE IN B'S REGION.

                //CAN'T BE IN AB'S REGION.

                //Check to see if it's outside C.
                //TODO: Note that in a boolean-style GJK, it shouldn't be possible to be outside C.
                fint CdotAB, CdotAC;
                Vector3.Dot(ref ab, ref C, out CdotAB);
                Vector3.Dot(ref ac, ref C, out CdotAC);
                CdotAB = -CdotAB;
                CdotAC = -CdotAC;
                if (CdotAC >= (fint)0 && CdotAB <= CdotAC)
                {
                    //It is C!
                    simplex.State = SimplexState.Point;
                    simplex.A = C;
                    point = C;
                    return true;
                }

                //Check if it's outside AC.            
                fint AdotAB, AdotAC;
                Vector3.Dot(ref ab, ref A, out AdotAB);
                Vector3.Dot(ref ac, ref A, out AdotAC);
                AdotAB = -AdotAB;
                AdotAC = -AdotAC;
                fint vb = CdotAB * AdotAC - AdotAB * CdotAC;
                if (vb <= (fint)0 && AdotAC > (fint)0 && CdotAC < (fint)0) //Note > instead of >= and < instead of <=; prevents bad denominator
                {
                    simplex.State = SimplexState.Segment;
                    simplex.A = A;
                    simplex.B = C;
                    fint V = AdotAC / (AdotAC - CdotAC);

                    Vector3.Multiply(ref ac, V, out point);
                    Vector3.Add(ref point, ref A, out point);
                    return true;
                }

                //Check if it's outside BC.
                fint BdotAB, BdotAC;
                Vector3.Dot(ref ab, ref B, out BdotAB);
                Vector3.Dot(ref ac, ref B, out BdotAC);
                BdotAB = -BdotAB;
                BdotAC = -BdotAC;
                fint va = BdotAB * CdotAC - CdotAB * BdotAC;
                fint d3d4;
                fint d6d5;
                if (va <= (fint)0 && (d3d4 = BdotAC - BdotAB) > (fint)0 && (d6d5 = CdotAB - CdotAC) > (fint)0)//Note > instead of >= and < instead of <=; prevents bad denominator
                {
                    simplex.State = SimplexState.Segment;
                    simplex.A = B;
                    simplex.B = C;
                    fint V = d3d4 / (d3d4 + d6d5);

                    Vector3 bc;
                    Vector3.Subtract(ref C, ref B, out bc);
                    Vector3.Multiply(ref bc, V, out point);
                    Vector3.Add(ref point, ref B, out point);
                    return true;
                }


                //On the face of the triangle.
                fint vc = AdotAB * BdotAC - BdotAB * AdotAC;
                simplex.A = A;
                simplex.B = B;
                simplex.C = C;
                simplex.State = SimplexState.Triangle;
                fint denom = (fint)1 / (va + vb + vc);
                fint w = vc * denom;
                fint v = vb * denom;

                Vector3.Multiply(ref ab, v, out point);
                Vector3 acw;
                Vector3.Multiply(ref ac, w, out acw);
                Vector3.Add(ref A, ref point, out point);
                Vector3.Add(ref point, ref acw, out point);
                return true;
            }
            return false;
        }





        ///<summary>
        /// Adds a new point to the simplex.
        ///</summary>
        ///<param name="point">Point to add.</param>
        public void AddNewSimplexPoint(ref Vector3 point)
        {
            switch (State)
            {
                case SimplexState.Empty:
                    State = SimplexState.Point;
                    A = point;
                    break;
                case SimplexState.Point:
                    State = SimplexState.Segment;
                    B = point;
                    break;
                case SimplexState.Segment:
                    State = SimplexState.Triangle;
                    C = point;
                    break;
                case SimplexState.Triangle:
                    State = SimplexState.Tetrahedron;
                    D = point;
                    break;
            }
        }

        ///<summary>
        /// Gets the error tolerance of the simplex.
        ///</summary>
        ///<returns>Error tolerance of the simplex.</returns>
        public fint GetErrorTolerance()
        {
            switch (State)
            {
                case SimplexState.Point:
                    return A.LengthSquared();
                case SimplexState.Segment:
                    return MathHelper.Max(A.LengthSquared(), B.LengthSquared());
                case SimplexState.Triangle:
                    return MathHelper.Max(A.LengthSquared(), MathHelper.Max(B.LengthSquared(), C.LengthSquared()));
                case SimplexState.Tetrahedron:
                    return MathHelper.Max(A.LengthSquared(), MathHelper.Max(B.LengthSquared(), MathHelper.Max(C.LengthSquared(), D.LengthSquared())));
            }
            return (fint)1;
        }


    }

}
