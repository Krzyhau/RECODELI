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

namespace Jitter.Dynamics.Constraints
{

    public class PointOnPoint : Constraint
    {
        private JVector localAnchor1, localAnchor2;
        private JVector r1, r2;

        private sfloat biasFactor = (sfloat)0.05f;
        private sfloat softness = (sfloat)0.01f;

        /// <summary>
        /// Initializes a new instance of the DistanceConstraint class.
        /// </summary>
        /// <param name="body1">The first body.</param>
        /// <param name="body2">The second body.</param>
        /// <param name="anchor1">The anchor point of the first body in world space. 
        /// The distance is given by the initial distance between both anchor points.</param>
        /// <param name="anchor2">The anchor point of the second body in world space.
        /// The distance is given by the initial distance between both anchor points.</param>
        public PointOnPoint(RigidBody body1, RigidBody body2, JVector anchor)
            : base(body1, body2)
        {
            JVector.Subtract(ref anchor, ref body1.position, out localAnchor1);
            JVector.Subtract(ref anchor, ref body2.position, out localAnchor2);

            JVector.Transform(ref localAnchor1, ref body1.invOrientation, out localAnchor1);
            JVector.Transform(ref localAnchor2, ref body2.invOrientation, out localAnchor2);
        }

        public sfloat AppliedImpulse { get { return accumulatedImpulse; } }

        /// <summary>
        /// Defines how big the applied impulses can get.
        /// </summary>
        public sfloat Softness { get { return softness; } set { softness = value; } }

        /// <summary>
        /// Defines how big the applied impulses can get which correct errors.
        /// </summary>
        public sfloat BiasFactor { get { return biasFactor; } set { biasFactor = value; } }

        sfloat effectiveMass = sfloat.Zero;
        sfloat accumulatedImpulse = sfloat.Zero;
        sfloat bias;
        sfloat softnessOverDt;

        JVector[] jacobian = new JVector[4];

        /// <summary>
        /// Called once before iteration starts.
        /// </summary>
        /// <param name="timestep">The 5simulation timestep</param>
        public override void PrepareForIteration(sfloat timestep)
        {
            JVector.Transform(ref localAnchor1, ref body1.orientation, out r1);
            JVector.Transform(ref localAnchor2, ref body2.orientation, out r2);

            JVector p1, p2, dp;
            JVector.Add(ref body1.position, ref r1, out p1);
            JVector.Add(ref body2.position, ref r2, out p2);

            JVector.Subtract(ref p2, ref p1, out dp);

            sfloat deltaLength = dp.Length();

            JVector n = p2 - p1;
            if (n.LengthSquared() != sfloat.Zero) n.Normalize();

            jacobian[0] = sfloat.MinusOne * n;
            jacobian[1] = sfloat.MinusOne * (r1 % n);
            jacobian[2] = sfloat.One * n;
            jacobian[3] = (r2 % n);

            effectiveMass = body1.inverseMass + body2.inverseMass
                + JVector.Transform(jacobian[1], body1.invInertiaWorld) * jacobian[1]
                + JVector.Transform(jacobian[3], body2.invInertiaWorld) * jacobian[3];

            softnessOverDt = softness / timestep;
            effectiveMass += softnessOverDt;

            effectiveMass = sfloat.One / effectiveMass;

            bias = deltaLength * biasFactor * (sfloat.One / timestep);

            if (!body1.isStatic)
            {
                body1.linearVelocity += body1.inverseMass * accumulatedImpulse * jacobian[0];
                body1.angularVelocity += JVector.Transform(accumulatedImpulse * jacobian[1], body1.invInertiaWorld);
            }

            if (!body2.isStatic)
            {
                body2.linearVelocity += body2.inverseMass * accumulatedImpulse * jacobian[2];
                body2.angularVelocity += JVector.Transform(accumulatedImpulse * jacobian[3], body2.invInertiaWorld);
            }


        }

        /// <summary>
        /// Iteratively solve this constraint.
        /// </summary>
        public override void Iterate()
        {
            sfloat jv =
                body1.linearVelocity * jacobian[0] +
                body1.angularVelocity * jacobian[1] +
                body2.linearVelocity * jacobian[2] +
                body2.angularVelocity * jacobian[3];

            sfloat softnessScalar = accumulatedImpulse * softnessOverDt;

            sfloat lambda = -effectiveMass * (jv + bias + softnessScalar);

            accumulatedImpulse += lambda;

            if (!body1.isStatic)
            {
                body1.linearVelocity += body1.inverseMass * lambda * jacobian[0];
                body1.angularVelocity += JVector.Transform(lambda * jacobian[1], body1.invInertiaWorld);
            }

            if (!body2.isStatic)
            {
                body2.linearVelocity += body2.inverseMass * lambda * jacobian[2];
                body2.angularVelocity += JVector.Transform(lambda * jacobian[3], body2.invInertiaWorld);
            }
        }

        public override void DebugDraw(IDebugDrawer drawer)
        {
            drawer.DrawLine(body1.position, body1.position + r1);
            drawer.DrawLine(body2.position, body2.position + r2);
        }

    }

}
