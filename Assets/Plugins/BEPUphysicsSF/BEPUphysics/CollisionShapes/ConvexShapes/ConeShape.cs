using System;
using SoftFloat;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using BEPUutilities;

namespace BEPUphysics.CollisionShapes.ConvexShapes
{
    ///<summary>
    /// Symmetrical shape with a circular base and a point at the top.
    ///</summary>
    public class ConeShape : ConvexShape
    {

        sfloat height;
        ///<summary>
        /// Gets or sets the height of the cone.
        ///</summary>
        public sfloat Height
        {
            get { return height; }
            set
            {
                height = value;
                OnShapeChanged();
            }
        }

        sfloat radius;
        ///<summary>
        /// Gets or sets the radius of the cone base.
        ///</summary>
        public sfloat Radius
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
        public ConeShape(sfloat height, sfloat radius)
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
        public ConeShape(sfloat height, sfloat radius, ConvexShapeDescription description)
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
        public static ConvexShapeDescription ComputeDescription(sfloat height, sfloat radius, sfloat collisionMargin)
        {
            ConvexShapeDescription description;
            description.EntityShapeVolume.Volume = ((sfloat).333333 * sfloat.Pi * radius * radius * height);

            description.EntityShapeVolume.VolumeDistribution = new Matrix3x3();
            sfloat diagValue = ((sfloat).1f * height * height + (sfloat).15f * radius * radius);
            description.EntityShapeVolume.VolumeDistribution.M11 = diagValue;
            description.EntityShapeVolume.VolumeDistribution.M22 = (sfloat).3f * radius * radius;
            description.EntityShapeVolume.VolumeDistribution.M33 = diagValue;

            description.MaximumRadius = (sfloat)(collisionMargin + sfloat.Max((sfloat).75 * height, libm.sqrtf((sfloat).0625f * height * height + radius * radius)));

            sfloat denominator = radius / height;
            denominator = denominator / libm.sqrtf(denominator * denominator + sfloat.One);
            description.MinimumRadius = (sfloat)(collisionMargin + sfloat.Min((sfloat).25f * height, denominator * (sfloat).75 * height));

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
            sfloat sinThetaSquared = radius * radius / (radius * radius + height * height);
            //If d.Y * d.Y / d.LengthSquared >= sinthetaSquared
            if (direction.Y > sfloat.Zero && direction.Y * direction.Y >= direction.LengthSquared() * sinThetaSquared)
            {
                extremePoint = new Vector3(sfloat.Zero, (sfloat).75f * height, sfloat.Zero);
                return;
            }
            //Is it a bottom edge of the cone?
            sfloat horizontalLengthSquared = direction.X * direction.X + direction.Z * direction.Z;
            if (horizontalLengthSquared > Toolbox.Epsilon)
            {
                var radOverSigma = radius / libm.sqrtf(horizontalLengthSquared);
                extremePoint = new Vector3((sfloat)(radOverSigma * direction.X), -(sfloat).25f * height, (sfloat)(radOverSigma * direction.Z));
            }
            else // It's pointing almost straight down...
                extremePoint = new Vector3(sfloat.Zero, -(sfloat).25f * height, sfloat.Zero);


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
