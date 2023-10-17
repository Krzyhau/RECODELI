using System;
using BEPUutilities.FixedMath;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using BEPUutilities;

namespace BEPUphysics.CollisionShapes.ConvexShapes
{
    ///<summary>
    /// Symmetrical shape with a circular base and a point at the top.
    ///</summary>
    public class ConeShape : ConvexShape
    {

        fint height;
        ///<summary>
        /// Gets or sets the height of the cone.
        ///</summary>
        public fint Height
        {
            get { return height; }
            set
            {
                height = value;
                OnShapeChanged();
            }
        }

        fint radius;
        ///<summary>
        /// Gets or sets the radius of the cone base.
        ///</summary>
        public fint Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                OnShapeChanged();
            }
        }

        ///<summary>
        /// Constructs a new cone shape.
        ///</summary>
        ///<param name="height">Height of the cone.</param>
        ///<param name="radius">Radius of the cone base.</param>
        public ConeShape(fint height, fint radius)
        {
            this.height = height;
            this.radius = radius;

            UpdateConvexShapeInfo(ComputeDescription(height, radius, collisionMargin));
        }

        ///<summary>
        /// Constructs a new cone shape.
        ///</summary>
        ///<param name="height">Height of the cone.</param>
        ///<param name="radius">Radius of the cone base.</param>
        /// <param name="description">Cached information about the shape. Assumed to be correct; no extra processing or validation is performed.</param>
        public ConeShape(fint height, fint radius, ConvexShapeDescription description)
        {
            this.height = height;
            this.radius = radius;

            UpdateConvexShapeInfo(description);
        }


        protected override void OnShapeChanged()
        {
            UpdateConvexShapeInfo(ComputeDescription(height, radius, collisionMargin));
            base.OnShapeChanged();
        }


        /// <summary>
        /// Computes a convex shape description for a ConeShape.
        /// </summary>
        ///<param name="height">Height of the cone.</param>
        ///<param name="radius">Radius of the cone base.</param>
        ///<param name="collisionMargin">Collision margin of the shape.</param>
        /// <returns>Description required to define a convex shape.</returns>
        public static ConvexShapeDescription ComputeDescription(fint height, fint radius, fint collisionMargin)
        {
            ConvexShapeDescription description;
            description.EntityShapeVolume.Volume = ((fint).333333 * fint.Pi * radius * radius * height);

            description.EntityShapeVolume.VolumeDistribution = new Matrix3x3();
            fint diagValue = ((fint).1f * height * height + (fint).15f * radius * radius);
            description.EntityShapeVolume.VolumeDistribution.M11 = diagValue;
            description.EntityShapeVolume.VolumeDistribution.M22 = (fint).3f * radius * radius;
            description.EntityShapeVolume.VolumeDistribution.M33 = diagValue;

            description.MaximumRadius = (fint)(collisionMargin + fint.Max((fint).75 * height, fint.Sqrt((fint).0625f * height * height + radius * radius)));

            fint denominator = radius / height;
            denominator = denominator / fint.Sqrt(denominator * denominator + (fint)1);
            description.MinimumRadius = (fint)(collisionMargin + fint.Min((fint).25f * height, denominator * (fint).75 * height));

            description.CollisionMargin = collisionMargin;
            return description;
        }


        ///<summary>
        /// Gets the extreme point of the shape in local space in a given direction.
        ///</summary>
        ///<param name="direction">Direction to find the extreme point in.</param>
        ///<param name="extremePoint">Extreme point on the shape.</param>
        public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
        {
            //Is it the tip of the cone?
            fint sinThetaSquared = radius * radius / (radius * radius + height * height);
            //If d.Y * d.Y / d.LengthSquared >= sinthetaSquared
            if (direction.Y > (fint)0 && direction.Y * direction.Y >= direction.LengthSquared() * sinThetaSquared)
            {
                extremePoint = new Vector3((fint)0, (fint).75f * height, (fint)0);
                return;
            }
            //Is it a bottom edge of the cone?
            fint horizontalLengthSquared = direction.X * direction.X + direction.Z * direction.Z;
            if (horizontalLengthSquared > Toolbox.Epsilon)
            {
                var radOverSigma = radius / fint.Sqrt(horizontalLengthSquared);
                extremePoint = new Vector3((fint)(radOverSigma * direction.X), -(fint).25f * height, (fint)(radOverSigma * direction.Z));
            }
            else // It's pointing almost straight down...
                extremePoint = new Vector3((fint)0, -(fint).25f * height, (fint)0);


        }


        /// <summary>
        /// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
        /// </summary>
        /// <returns>EntityCollidable that uses this shape.</returns>
        public override EntityCollidable GetCollidableInstance()
        {
            return new ConvexCollidable<ConeShape>(this);
        }

    }
}
