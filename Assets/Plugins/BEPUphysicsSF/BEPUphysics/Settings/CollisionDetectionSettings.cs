﻿using System;
using SoftFloat;
namespace BEPUphysics.Settings
{
    ///<summary>
    /// Settings class containing global information about collision detection.
    ///</summary>
    public static class CollisionDetectionSettings
    {


        internal static sfloat ContactInvalidationLengthSquared = (sfloat).01f;

        /// <summary>
        /// For persistent manifolds, contacts are represented by an offset in local space of two colliding bodies.
        /// The distance between these offsets transformed into world space and projected onto a plane defined by the contact normal squared is compared against this value.
        /// If this value is exceeded, the contact is removed from the contact manifold.
        /// 
        /// If the world is smaller or larger than 'normal' for the engine, adjusting this value proportionally can improve contact caching behavior.
        /// The default value of .1f works well for worlds that operate on the order of 1 unit.
        /// </summary>
        public static sfloat ContactInvalidationLength
        {
            get
            {
                return libm.sqrtf(ContactInvalidationLengthSquared);
            }
            set
            {
                ContactInvalidationLengthSquared = value * value;
            }
        }


        internal static sfloat ContactMinimumSeparationDistanceSquared = (sfloat).0009f;
        /// <summary>
        /// In persistent manifolds, if two contacts are too close together, then 
        /// the system will not use one of them.  This avoids redundant constraints.
        /// Defaults to .03f.
        /// </summary>
        public static sfloat ContactMinimumSeparationDistance
        {
            get
            {
                return libm.sqrtf(ContactMinimumSeparationDistanceSquared);
            }
            set
            {
                ContactMinimumSeparationDistanceSquared = value * value;
            }
        }

        internal static sfloat nonconvexNormalDotMinimum = (sfloat).99f;
        /// <summary>
        /// In regular convex manifolds, two contacts are considered redundant if their positions are too close together.  
        /// In nonconvex manifolds, the normal must also be tested, since a contact in the same location could have a different normal.
        /// This property is the minimum angle in radians between normals below which contacts are considered redundant.
        /// </summary>
        public static sfloat NonconvexNormalAngleDifferenceMinimum
        {
            get
            {
                return libm.acosf(nonconvexNormalDotMinimum);
            }
            set
            {
                nonconvexNormalDotMinimum = libm.cosf(value);
            }
        }

        /// <summary>
        /// The default amount of allowed penetration into the margin before position correcting impulses will be applied.
        /// Defaults to .01f.
        /// </summary>
        public static sfloat AllowedPenetration = (sfloat).01f;

        /// <summary>
        /// Default collision margin around objects.  Margins help prevent objects from interpenetrating and improve stability.
        /// Defaults to .04f.
        /// </summary>
        public static sfloat DefaultMargin = (sfloat).04f;

        internal static sfloat maximumContactDistance = (sfloat).1f;
        /// <summary>
        /// Maximum distance between the surfaces defining a contact point allowed before removing the contact.
        /// Defaults to .1f.
        /// </summary>
        public static sfloat MaximumContactDistance
        {
            get
            {
                return maximumContactDistance;
            }
            set
            {
                if (value >= sfloat.Zero)
                    maximumContactDistance = value;
                else
                    throw new ArgumentException("Distance must be nonnegative.");
            }
        }


        
    }
}
