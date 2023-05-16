using SoftFloat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jitter.Dynamics
{

    // TODO: Check values, Documenation
    // Maybe some default materials, aka Material.Soft?
    public class Material
    {

        internal sfloat kineticFriction = (sfloat)0.3f;
        internal sfloat staticFriction = (sfloat)0.6f;
        internal sfloat restitution = sfloat.Zero;

        public Material() { }

        public sfloat Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }

        public sfloat StaticFriction
        {
            get { return staticFriction; }
            set { staticFriction = value; }
        }

        public sfloat KineticFriction
        {
            get { return kineticFriction; }
            set { kineticFriction = value; }
        }

    }
}
