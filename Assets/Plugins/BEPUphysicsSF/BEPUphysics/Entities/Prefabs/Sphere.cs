using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.EntityStateManagement;
 
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;
using BEPUutilities.FixedMath;

namespace BEPUphysics.Entities.Prefabs
{
    /// <summary>
    /// Ball-shaped object that can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
    /// </summary>
    public class Sphere : Entity<ConvexCollidable<SphereShape>>
    {
        /// <summary>
        /// Radius of the sphere.
        /// </summary>
        public fint Radius
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

        private Sphere(fint radius)
            :base(new ConvexCollidable<SphereShape>(new SphereShape(radius)))
        {
        }

        private Sphere(fint radius, fint mass)
            :base(new ConvexCollidable<SphereShape>(new SphereShape(radius)), mass)
        {
        }



        /// <summary>
        /// Constructs a physically simulated sphere.
        /// </summary>
        /// <param name="position">Position of the sphere.</param>
        /// <param name="radius">Radius of the sphere.</param>
        /// <param name="mass">Mass of the object.</param>
        public Sphere(Vector3 position, fint radius, fint mass)
            : this(radius, mass)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a nondynamic sphere.
        /// </summary>
        /// <param name="position">Position of the sphere.</param>
        /// <param name="radius">Radius of the sphere.</param>
        public Sphere(Vector3 position, fint radius)
            : this(radius)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a physically simulated sphere.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="radius">Radius of the sphere.</param>
        /// <param name="mass">Mass of the object.</param>
        public Sphere(MotionState motionState, fint radius, fint mass)
            : this(radius, mass)
        {
            MotionState = motionState;
        }

        /// <summary>
        /// Constructs a nondynamic sphere.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="radius">Radius of the sphere.</param>
        public Sphere(MotionState motionState, fint radius)
            : this(radius)
        {
            MotionState = motionState;
        }



    }
}