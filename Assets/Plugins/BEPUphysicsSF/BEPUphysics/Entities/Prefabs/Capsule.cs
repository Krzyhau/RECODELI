using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.EntityStateManagement;
 
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;
using SoftFloat;

namespace BEPUphysics.Entities.Prefabs
{
    /// <summary>
    /// Pill-shaped object that can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
    /// </summary>
    public class Capsule : Entity<ConvexCollidable<CapsuleShape>>
    {
        /// <summary>
        /// Gets or sets the length of the capsule.
        /// </summary>
        public sfloat Length
        {
            get
            {
                return CollisionInformation.Shape.Length;
            }
            set
            {
                CollisionInformation.Shape.Length = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the capsule.
        /// </summary>
        public sfloat Radius
        {
            get
            {
                return CollisionInformation.Shape.Radius;
            }
            set
            {
                CollisionInformation.Shape.Radius = value;
            }
        }

        private Capsule(sfloat len, sfloat rad)
            : base(new ConvexCollidable<CapsuleShape>(new CapsuleShape(len, rad)))
        {
        }

        private Capsule(sfloat len, sfloat rad, sfloat mass)
            : base(new ConvexCollidable<CapsuleShape>(new CapsuleShape(len, rad)), mass)
        {
        }



        ///<summary>
        /// Computes an orientation and length from a line segment.
        ///</summary>
        ///<param name="start">Starting point of the line segment.</param>
        ///<param name="end">Endpoint of the line segment.</param>
        ///<param name="orientation">Orientation of a line that fits the line segment.</param>
        ///<param name="length">Length of the line segment.</param>
        public static void GetCapsuleInformation(ref Vector3 start, ref Vector3 end, out Quaternion orientation, out sfloat length)
        {
            Vector3 segmentDirection;
            Vector3.Subtract(ref end, ref start, out segmentDirection);
            length = segmentDirection.Length();
            if (length > sfloat.Zero)
            {
                Vector3.Divide(ref segmentDirection, length, out segmentDirection);
                Quaternion.GetQuaternionBetweenNormalizedVectors(ref Toolbox.UpVector, ref segmentDirection, out orientation);
            }
            else
                orientation = Quaternion.Identity;
        }

        ///<summary>
        /// Constructs a new kinematic capsule.
        ///</summary>
        ///<param name="start">Line segment start point.</param>
        ///<param name="end">Line segment end point.</param>
        ///<param name="radius">Radius of the capsule to expand the line segment by.</param>
        public Capsule(Vector3 start, Vector3 end, sfloat radius)
            : this((end - start).Length(), radius)
        {
            sfloat length;
            Quaternion orientation;
            GetCapsuleInformation(ref start, ref end, out orientation, out length);
            this.Orientation = orientation;
            Vector3 position;
            Vector3.Add(ref start, ref end, out position);
            Vector3.Multiply(ref position, sfloat.Half, out position);
            this.Position = position;
        }


        ///<summary>
        /// Constructs a new dynamic capsule.
        ///</summary>
        ///<param name="start">Line segment start point.</param>
        ///<param name="end">Line segment end point.</param>
        ///<param name="radius">Radius of the capsule to expand the line segment by.</param>
        /// <param name="mass">Mass of the entity.</param>
        public Capsule(Vector3 start, Vector3 end, sfloat radius, sfloat mass)
            : this((end - start).Length(), radius, mass)
        {
            sfloat length;
            Quaternion orientation;
            GetCapsuleInformation(ref start, ref end, out orientation, out length);
            this.Orientation = orientation;
            Vector3 position;
            Vector3.Add(ref start, ref end, out position);
            Vector3.Multiply(ref position, sfloat.Half, out position);
            this.Position = position;
        }

        /// <summary>
        /// Constructs a physically simulated capsule.
        /// </summary>
        /// <param name="position">Position of the capsule.</param>
        /// <param name="length">Length of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        /// <param name="mass">Mass of the object.</param>
        public Capsule(Vector3 position, sfloat length, sfloat radius, sfloat mass)
            : this(length, radius, mass)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a nondynamic capsule.
        /// </summary>
        /// <param name="position">Position of the capsule.</param>
        /// <param name="length">Length of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        public Capsule(Vector3 position, sfloat length, sfloat radius)
            : this(length, radius)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a dynamic capsule.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="length">Length of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        /// <param name="mass">Mass of the object.</param>
        public Capsule(MotionState motionState, sfloat length, sfloat radius, sfloat mass)
            : this(length, radius, mass)
        {
            MotionState = motionState;
        }

        /// <summary>
        /// Constructs a nondynamic capsule.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="length">Length of the capsule.</param>
        /// <param name="radius">Radius of the capsule.</param>
        public Capsule(MotionState motionState, sfloat length, sfloat radius)
            : this(length, radius)
        {
            MotionState = motionState;
        }

    }
}