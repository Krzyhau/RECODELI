﻿using BEPUphysics.Materials;
using SoftFloat;

namespace BEPUphysics.Vehicle
{
    /// <summary>
    /// Function which takes the friction values from a wheel and a supporting material and computes the blended friction.
    /// </summary>
    /// <param name="wheelFriction">Friction coefficient associated with the wheel.</param>
    /// <param name="materialFriction">Friction coefficient associated with the support material.</param>
    /// <param name="usingKineticFriction">True if the friction coefficients passed into the blender are kinetic coefficients, false otherwise.</param>
    /// <param name="wheel">Wheel being blended.</param>
    /// <returns>Blended friction coefficient.</returns>
    public delegate sfloat WheelFrictionBlender(sfloat wheelFriction, sfloat materialFriction, bool usingKineticFriction, Wheel wheel);


}