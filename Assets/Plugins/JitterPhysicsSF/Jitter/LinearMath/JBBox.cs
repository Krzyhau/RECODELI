﻿/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
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
    /// Bounding Box defined through min and max vectors. Member
    /// of the math namespace, so every method has it's 'by reference'
    /// equivalent to speed up time critical math operations.
    /// </summary>
    public struct JBBox
    {
        /// <summary>
        /// Containment type used within the <see cref="JBBox"/> structure.
        /// </summary>
        public enum ContainmentType
        {
            /// <summary>
            /// The objects don't intersect.
            /// </summary>
            Disjoint,
            /// <summary>
            /// One object is within the other.
            /// </summary>
            Contains,
            /// <summary>
            /// The two objects intersect.
            /// </summary>
            Intersects
        }

        /// <summary>
        /// The maximum point of the box.
        /// </summary>
        public JVector Min;

        /// <summary>
        /// The minimum point of the box.
        /// </summary>
        public JVector Max;

        /// <summary>
        /// Returns the largest box possible.
        /// </summary>
        public static readonly JBBox LargeBox;

        /// <summary>
        /// Returns the smalltest box possible.
        /// </summary>
        public static readonly JBBox SmallBox;

        static JBBox()
        {
            LargeBox.Min = new JVector(sfloat.MinValue);
            LargeBox.Max = new JVector(sfloat.MaxValue);
            SmallBox.Min = new JVector(sfloat.MaxValue);
            SmallBox.Max = new JVector(sfloat.MinValue);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="min">The minimum point of the box.</param>
        /// <param name="max">The maximum point of the box.</param>
        public JBBox(JVector min, JVector max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Transforms the bounding box into the space given by orientation and position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <param name="result"></param>
        internal void InverseTransform(ref JVector position, ref JMatrix orientation)
        {
            JVector.Subtract(ref Max, ref position, out Max);
            JVector.Subtract(ref Min, ref position, out Min);

            JVector center;
            JVector.Add(ref Max, ref Min, out center);
            center.X *= sfloat.Half; center.Y *= sfloat.Half; center.Z *= sfloat.Half;

            JVector halfExtents;
            JVector.Subtract(ref Max, ref Min, out halfExtents);
            halfExtents.X *= sfloat.Half; halfExtents.Y *= sfloat.Half; halfExtents.Z *= sfloat.Half;

            JVector.TransposedTransform(ref center, ref orientation, out center);

            JMatrix abs; JMath.Absolute(ref orientation, out abs);
            JVector.TransposedTransform(ref halfExtents, ref abs, out halfExtents);

            JVector.Add(ref center, ref halfExtents, out Max);
            JVector.Subtract(ref center, ref halfExtents, out Min);
        }

        public void Transform(ref JMatrix orientation)
        {
            JVector halfExtents = sfloat.Half * (Max - Min);
            JVector center = sfloat.Half * (Max + Min);

            JVector.Transform(ref center, ref orientation, out center);

            JMatrix abs; JMath.Absolute(ref orientation, out abs);
            JVector.Transform(ref halfExtents, ref abs, out halfExtents);

            Max = center + halfExtents;
            Min = center - halfExtents;
        }

        /// <summary>
        /// Checks whether a point is inside, outside or intersecting
        /// a point.
        /// </summary>
        /// <returns>The ContainmentType of the point.</returns>
        #region public Ray/Segment Intersection

        private bool Intersect1D(sfloat start, sfloat dir, sfloat min, sfloat max,
            ref sfloat enter,ref sfloat exit)
        {
            if (dir * dir < JMath.Epsilon * JMath.Epsilon) return (start >= min && start <= max);

            sfloat t0 = (min - start) / dir;
            sfloat t1 = (max - start) / dir;

            if (t0 > t1) { sfloat tmp = t0; t0 = t1; t1 = tmp; }

            if (t0 > exit || t1 < enter) return false;

            if (t0 > enter) enter = t0;
            if (t1 < exit) exit = t1;
            return true;
        }


        public bool SegmentIntersect(ref JVector origin,ref JVector direction)
        {
            sfloat enter = sfloat.Zero, exit = sfloat.One;

            if (!Intersect1D(origin.X, direction.X, Min.X, Max.X,ref enter,ref exit))
                return false;

            if (!Intersect1D(origin.Y, direction.Y, Min.Y, Max.Y, ref enter, ref exit))
                return false;

            if (!Intersect1D(origin.Z, direction.Z, Min.Z, Max.Z,ref enter,ref exit))
                return false;

            return true;
        }

        public bool RayIntersect(ref JVector origin, ref JVector direction)
        {
            sfloat enter = sfloat.Zero, exit = sfloat.MaxValue;

            if (!Intersect1D(origin.X, direction.X, Min.X, Max.X, ref enter, ref exit))
                return false;

            if (!Intersect1D(origin.Y, direction.Y, Min.Y, Max.Y, ref enter, ref exit))
                return false;

            if (!Intersect1D(origin.Z, direction.Z, Min.Z, Max.Z, ref enter, ref exit))
                return false;

            return true;
        }

        public bool SegmentIntersect(JVector origin, JVector direction)
        {
            return SegmentIntersect(ref origin, ref direction);
        }

        public bool RayIntersect(JVector origin, JVector direction)
        {
            return RayIntersect(ref origin, ref direction);
        }

        /// <summary>
        /// Checks wether a point is within a box or not.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public ContainmentType Contains(JVector point)
        {
            return this.Contains(ref point);
        }

        /// <summary>
        /// Checks whether a point is inside, outside or intersecting
        /// a point.
        /// </summary>
        /// <param name="point">A point in space.</param>
        /// <returns>The ContainmentType of the point.</returns>
        public ContainmentType Contains(ref JVector point)
        {
            return ((((this.Min.X <= point.X) && (point.X <= this.Max.X)) &&
                ((this.Min.Y <= point.Y) && (point.Y <= this.Max.Y))) &&
                ((this.Min.Z <= point.Z) && (point.Z <= this.Max.Z))) ? ContainmentType.Contains : ContainmentType.Disjoint;
        }

        #endregion

        /// <summary>
        /// Retrieves the 8 corners of the box.
        /// </summary>
        /// <returns>An array of 8 JVector entries.</returns>
        #region public void GetCorners(JVector[] corners)

        public void GetCorners(JVector[] corners)
        {
            corners[0].Set(this.Min.X, this.Max.Y, this.Max.Z);
            corners[1].Set(this.Max.X, this.Max.Y, this.Max.Z);
            corners[2].Set(this.Max.X, this.Min.Y, this.Max.Z);
            corners[3].Set(this.Min.X, this.Min.Y, this.Max.Z);
            corners[4].Set(this.Min.X, this.Max.Y, this.Min.Z);
            corners[5].Set(this.Max.X, this.Max.Y, this.Min.Z);
            corners[6].Set(this.Max.X, this.Min.Y, this.Min.Z);
            corners[7].Set(this.Min.X, this.Min.Y, this.Min.Z);
        }

        #endregion


        public void AddPoint(JVector point)
        {
            AddPoint(ref point);
        }

        public void AddPoint(ref JVector point)
        {
            JVector.Max(ref this.Max, ref point, out this.Max);
            JVector.Min(ref this.Min, ref point, out this.Min);
        }

        /// <summary>
        /// Expands a bounding box with the volume 0 by all points
        /// given.
        /// </summary>
        /// <param name="points">A array of JVector.</param>
        /// <returns>The resulting bounding box containing all points.</returns>
        #region public static JBBox CreateFromPoints(JVector[] points)

        public static JBBox CreateFromPoints(JVector[] points)
        {
            JVector vector3 = new JVector(sfloat.MaxValue);
            JVector vector2 = new JVector(sfloat.MinValue);

            for (int i = 0; i < points.Length; i++)
            {
                JVector.Min(ref vector3, ref points[i], out vector3);
                JVector.Max(ref vector2, ref points[i], out vector2);
            }
            return new JBBox(vector3, vector2);
        }

        #endregion

        /// <summary>
        /// Checks whether another bounding box is inside, outside or intersecting
        /// this box. 
        /// </summary>
        /// <param name="box">The other bounding box to check.</param>
        /// <returns>The ContainmentType of the box.</returns>
        #region public ContainmentType Contains(JBBox box)

        public ContainmentType Contains(JBBox box)
        {
            return this.Contains(ref box);
        }

        /// <summary>
        /// Checks whether another bounding box is inside, outside or intersecting
        /// this box. 
        /// </summary>
        /// <param name="box">The other bounding box to check.</param>
        /// <returns>The ContainmentType of the box.</returns>
        public ContainmentType Contains(ref JBBox box)
        {
            ContainmentType result = ContainmentType.Disjoint;
            if ((((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X)) && ((this.Max.Y >= box.Min.Y) && (this.Min.Y <= box.Max.Y))) && ((this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z)))
            {
                result = ((((this.Min.X <= box.Min.X) && (box.Max.X <= this.Max.X)) && ((this.Min.Y <= box.Min.Y) && (box.Max.Y <= this.Max.Y))) && ((this.Min.Z <= box.Min.Z) && (box.Max.Z <= this.Max.Z))) ? ContainmentType.Contains : ContainmentType.Intersects;
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Creates a new box containing the two given ones.
        /// </summary>
        /// <param name="original">First box.</param>
        /// <param name="additional">Second box.</param>
        /// <returns>A JBBox containing the two given boxes.</returns>
        #region public static JBBox CreateMerged(JBBox original, JBBox additional)

        public static JBBox CreateMerged(JBBox original, JBBox additional)
        {
            JBBox result;
            JBBox.CreateMerged(ref original, ref additional, out result);
            return result;
        }

        /// <summary>
        /// Creates a new box containing the two given ones.
        /// </summary>
        /// <param name="original">First box.</param>
        /// <param name="additional">Second box.</param>
        /// <param name="result">A JBBox containing the two given boxes.</param>
        public static void CreateMerged(ref JBBox original, ref JBBox additional, out JBBox result)
        {
            JVector vector;
            JVector vector2;
            JVector.Min(ref original.Min, ref additional.Min, out vector2);
            JVector.Max(ref original.Max, ref additional.Max, out vector);
            result.Min = vector2;
            result.Max = vector;
        }

        #endregion

        public JVector Center { get { return (Min + Max)* sfloat.Half; } }

        internal sfloat Perimeter
        {
            get
            {
                return sfloat.Two * ((Max.X - Min.X) * (Max.Y - Min.Y) +
                    (Max.X - Min.X) * (Max.Z - Min.Z) +
                    (Max.Z - Min.Z) * (Max.Y - Min.Y));
            }
        }
    }
}
