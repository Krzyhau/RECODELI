﻿using System;
using BEPUutilities.FixedMath;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using BEPUutilities;

namespace BEPUphysics.CollisionShapes.ConvexShapes
{
    ///<summary>
    /// Convex shape with width, length, and height.
    ///</summary>
    public class BoxShape : ConvexShape
    {
        internal fint halfWidth;
        internal fint halfHeight;
        internal fint halfLength;


        /// <summary>
        /// Width of the box divided by two.
        /// </summary>
        public fint HalfWidth
        {
            get { return halfWidth; }
            set { halfWidth = value; OnShapeChanged(); }
        }

        /// <summary>
        /// Height of the box divided by two.
        /// </summary>
        public fint HalfHeight
        {
            get { return halfHeight; }
            set { halfHeight = value; OnShapeChanged(); }
        }

        /// <summary>
        /// Length of the box divided by two.
        /// </summary>
        public fint HalfLength
        {
            get { return halfLength; }
            set { halfLength = value; OnShapeChanged(); }
        }

        /// <summary>
        /// Width of the box.
        /// </summary>
        public fint Width
        {
            get { return halfWidth * (fint)2; }
            set { halfWidth = value * (fint)0.5f; OnShapeChanged(); }
        }

        /// <summary>
        /// Height of the box.
        /// </summary>
        public fint Height
        {
            get { return halfHeight * (fint)2; }
            set { halfHeight = value * (fint)0.5f; OnShapeChanged(); }
        }

        /// <summary>
        /// Length of the box.
        /// </summary>
        public fint Length
        {
            get { return halfLength * (fint)2; }
            set { halfLength = value * (fint)0.5f; OnShapeChanged(); }
        }


        ///<summary>
        /// Constructs a new box shape.
        ///</summary>
        ///<param name="width">Width of the box.</param>
        ///<param name="height">Height of the box.</param>
        ///<param name="length">Length of the box.</param>
        public BoxShape(fint width, fint height, fint length)
        {
            halfWidth = width * (fint)0.5f;
            halfHeight = height * (fint)0.5f;
            halfLength = length * (fint)0.5f;

            UpdateConvexShapeInfo(ComputeDescription(width, height, length, collisionMargin));
        }

        ///<summary>
        /// Constructs a new box shape from cached information.
        ///</summary>
        ///<param name="width">Width of the box.</param>
        ///<param name="height">Height of the box.</param>
        ///<param name="length">Length of the box.</param>
        /// <param name="description">Cached information about the shape. Assumed to be correct; no extra processing or validation is performed.</param>
        public BoxShape(fint width, fint height, fint length, ConvexShapeDescription description)
        {
            halfWidth = width * (fint)0.5f;
            halfHeight = height * (fint)0.5f;
            halfLength = length * (fint)0.5f;

            UpdateConvexShapeInfo(description);
        }

        protected override void OnShapeChanged()
        {
            UpdateConvexShapeInfo(ComputeDescription(halfWidth, halfHeight, halfLength, collisionMargin));
            base.OnShapeChanged();
        }

        /// <summary>
        /// Computes a convex shape description for a BoxShape.
        /// </summary>
        ///<param name="width">Width of the box.</param>
        ///<param name="height">Height of the box.</param>
        ///<param name="length">Length of the box.</param>
        /// <param name="collisionMargin">Collision margin of the shape.</param>
        /// <returns>Description required to define a convex shape.</returns>
        public static ConvexShapeDescription ComputeDescription(fint width, fint height, fint length, fint collisionMargin)
        {
            ConvexShapeDescription description;
            description.EntityShapeVolume.Volume = width * height * length;

            fint widthSquared = width * width;
            fint heightSquared = height * height;
            fint lengthSquared = length * length;
            fint inv12 = (fint)1 / (fint)12f;

            description.EntityShapeVolume.VolumeDistribution = new Matrix3x3();
            description.EntityShapeVolume.VolumeDistribution.M11 = (heightSquared + lengthSquared) * inv12;
            description.EntityShapeVolume.VolumeDistribution.M22 = (widthSquared + lengthSquared) * inv12;
            description.EntityShapeVolume.VolumeDistribution.M33 = (widthSquared + heightSquared) * inv12;

            description.MaximumRadius = (fint)0.5f * fint.Sqrt(width * width + height * height + length * length);
            description.MinimumRadius = (fint)0.5f * fint.Min(width, fint.Min(height, length));

            description.CollisionMargin = collisionMargin;
            return description;
        }





        /// <summary>
        /// Gets the bounding box of the shape given a transform.
        /// </summary>
        /// <param name="shapeTransform">Transform to use.</param>
        /// <param name="boundingBox">Bounding box of the transformed shape.</param>
        public override void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
        {
#if !WINDOWS
            boundingBox = new BoundingBox();
#endif

            Matrix3x3 o;
            Matrix3x3.CreateFromQuaternion(ref shapeTransform.Orientation, out o);
            //Sample the local directions from the orientation matrix, implicitly transposed.
            //Notice only three directions are used.  Due to box symmetry, 'left' is just -right.
            var right = new Vector3(fint.Sign(o.M11) * halfWidth, fint.Sign(o.M21) * halfHeight, fint.Sign(o.M31) * halfLength);

            var up = new Vector3(fint.Sign(o.M12) * halfWidth, fint.Sign(o.M22) * halfHeight, fint.Sign(o.M32) * halfLength);

            var backward = new Vector3(fint.Sign(o.M13) * halfWidth, fint.Sign(o.M23) * halfHeight, fint.Sign(o.M33) * halfLength);


            //Rather than transforming each axis independently (and doing three times as many operations as required), just get the 3 required values directly.
            Vector3 offset;
            TransformLocalExtremePoints(ref right, ref up, ref backward, ref o, out offset);

            //The positive and negative vectors represent the X, Y and Z coordinates of the extreme points in world space along the world space axes.
            Vector3.Add(ref shapeTransform.Position, ref offset, out boundingBox.Max);
            Vector3.Subtract(ref shapeTransform.Position, ref offset, out boundingBox.Min);

        }


        ///<summary>
        /// Gets the extreme point of the shape in local space in a given direction.
        ///</summary>
        ///<param name="direction">Direction to find the extreme point in.</param>
        ///<param name="extremePoint">Extreme point on the shape.</param>
        public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
        {
            extremePoint = new Vector3(fint.Sign(direction.X) * (halfWidth - collisionMargin), fint.Sign(direction.Y) * (halfHeight - collisionMargin), fint.Sign(direction.Z) * (halfLength - collisionMargin));
        }




        /// <summary>
        /// Gets the intersection between the box and the ray.
        /// </summary>
        /// <param name="ray">Ray to test against the box.</param>
        /// <param name="transform">Transform of the shape.</param>
        /// <param name="maximumLength">Maximum distance to travel in units of the direction vector's length.</param>
        /// <param name="hit">Hit data for the raycast, if any.</param>
        /// <returns>Whether or not the ray hit the target.</returns>
        public override bool RayTest(ref Ray ray, ref RigidTransform transform, fint maximumLength, out RayHit hit)
        {
            Vector3.Subtract(ref ray.Position, ref transform.Position, out var offset);
            Matrix3x3.CreateFromQuaternion(ref transform.Orientation, out var orientation);
            Matrix3x3.TransformTranspose(ref offset, ref orientation, out var localOffset);
            Matrix3x3.TransformTranspose(ref ray.Direction, ref orientation, out var localDirection);
            //Note that this division has two odd properties:
            //1) If the local direction has a near zero component, it is clamped to a nonzero but extremely small value. This is a hack, but it works reasonably well.
            //The idea is that any interval computed using such an inverse would be enormous. Those values will not be exactly accurate, but they will never appear as a result
            //because a parallel ray will never actually intersect the surface. The resulting intervals are practical approximations of the 'true' infinite intervals.
            //2) To compensate for the clamp and abs, we reintroduce the sign in the numerator. Note that it has the reverse sign since it will be applied to the offset to get the T value.
            var offsetToTScale = new Vector3(
                (localDirection.X < (fint)0 ? (fint)1 : (fint)(-1)) / fint.Max(fint.Epsilon, fint.Abs(localDirection.X)),
                (localDirection.Y < (fint)0 ? (fint)1 : (fint)(-1)) / fint.Max(fint.Epsilon, fint.Abs(localDirection.Y)),
                (localDirection.Z < (fint)0 ? (fint)1 : (fint)(-1)) / fint.Max(fint.Epsilon, fint.Abs(localDirection.Z)));

            //Compute impact times for each pair of planes in local space.
            var halfExtent = new Vector3(HalfWidth, HalfHeight, HalfLength);
            Vector3.Subtract(ref localOffset, ref halfExtent, out var negativeTNumerator);
            Vector3.Add(ref localOffset, ref halfExtent, out var positiveTNumerator);
            Vector3.Multiply(ref negativeTNumerator, ref offsetToTScale, out var negativeT);
            Vector3.Multiply(ref positiveTNumerator, ref offsetToTScale, out var positiveT);
            Vector3.Min(ref negativeT, ref positiveT, out var entryT);
            Vector3.Max(ref negativeT, ref positiveT, out var exitT);

            //In order for an impact to occur, the ray must enter all three slabs formed by the axis planes before exiting any of them.
            //In other words, the first exit must occur after the last entry.
            var earliestExit = exitT.X < exitT.Y ? exitT.X : exitT.Y;
            if (exitT.Z < earliestExit)
                earliestExit = exitT.Z;
            if (earliestExit > maximumLength)
                earliestExit = maximumLength;
            //The interval of ray-box intersection goes from latestEntry to earliestExit. If earliestExit is negative, then the ray is pointing away from the box.
            if (earliestExit < (fint)0)
            {
                hit = default;
                return false;
            }
            fint latestEntry;
            if (entryT.X > entryT.Y)
            {
                if (entryT.X > entryT.Z)
                {
                    latestEntry = entryT.X;
                    hit.Normal = new Vector3(orientation.M11, orientation.M12, orientation.M13);
                }
                else
                {
                    latestEntry = entryT.Z;
                    hit.Normal = new Vector3(orientation.M31, orientation.M32, orientation.M33);
                }
            }
            else
            {
                if (entryT.Y > entryT.Z)
                {
                    latestEntry = entryT.Y;
                    hit.Normal = new Vector3(orientation.M21, orientation.M22, orientation.M23);
                }
                else
                {
                    latestEntry = entryT.Z;
                    hit.Normal = new Vector3(orientation.M31, orientation.M32, orientation.M33);
                }
            }

            if (earliestExit < latestEntry)
            {
                //At no point is the ray in all three slabs at once.
                hit = default;
                return false;
            }
            hit.T = latestEntry < (fint)0 ? (fint)0 : latestEntry;
            //The normal should point away from the center of the box.
            if (Vector3.Dot(hit.Normal, offset) < (fint)0)
            {
                Vector3.Negate(ref hit.Normal, out hit.Normal);
            }
            Vector3.Multiply(ref ray.Direction, hit.T, out var offsetFromOrigin);
            Vector3.Add(ref ray.Position, ref offsetFromOrigin, out hit.Location);
            return true;
        }

        /// <summary>
        /// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
        /// </summary>
        /// <returns>EntityCollidable that uses this shape.</returns>
        public override EntityCollidable GetCollidableInstance()
        {
            return new ConvexCollidable<BoxShape>(this);
        }

    }
}
