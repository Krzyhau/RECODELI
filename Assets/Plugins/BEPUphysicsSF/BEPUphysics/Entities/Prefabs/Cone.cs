using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.EntityStateManagement;
 
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;
using SoftFloat;

namespace BEPUphysics.Entities.Prefabs
{
    /// <summary>
    /// Cone-shaped object that can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
    /// </summary>
    public class Cone : Entity<ConvexCollidable<ConeShape>>
    {
        /// <summary>
        /// Gets or sets the length of the cone.
        /// </summary>
        public sfloat Height
        {
            get
            {
                return CollisionInformation.Shape.Height;
            }
            set
            {
                CollisionInformation.Shape.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the cone.
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


        private Cone(sfloat high, sfloat rad)
            :base(new ConvexCollidable<ConeShape>(new ConeShape(high, rad)))
        {
        }

        private Cone(sfloat high, sfloat rad, sfloat mass)
            :base(new ConvexCollidable<ConeShape>(new ConeShape(high, rad)), mass)
        {
        }



        /// <summary>
        /// Constructs a physically simulated cone.
        /// </summary>
        /// <param name="position">Position of the cone.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        /// <param name="mass">Mass of the object.</param>
        public Cone(Vector3 position, sfloat height, sfloat radius, sfloat mass)
            : this(height, radius, mass)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a nondynamic cone.
        /// </summary>
        /// <param name="position">Position of the cone.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        public Cone(Vector3 position, sfloat height, sfloat radius)
            : this(height, radius)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a physically simulated cone.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        /// <param name="mass">Mass of the object.</param>
        public Cone(MotionState motionState, sfloat height, sfloat radius, sfloat mass)
            : this(height, radius, mass)
        {
            MotionState = motionState;
        }

        /// <summary>
        /// Constructs a nondynamic cone.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        public Cone(MotionState motionState, sfloat height, sfloat radius)
            : this(height, radius)
        {
            MotionState = motionState;
        }

 


    }
}