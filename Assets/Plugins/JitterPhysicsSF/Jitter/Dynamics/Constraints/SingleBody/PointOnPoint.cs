/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
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

namespace Jitter.Dynamics.Constraints.SingleBody
{

    public class PointOnPoint : Constraint
    {
        private JVector localAnchor1;
        private JVector anchor;

        private JVector r1;

        private sfloat biasFactor = (sfloat)0.1f;
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
        public PointOnPoint(RigidBody body, JVector localAnchor)
            : base(body, null)
        {
            localAnchor1 = localAnchor;

            this.anchor = body.position + JVector.Transform(localAnchor, body.orientation);
        }

        public sfloat AppliedImpulse { get { return accumulatedImpulse; } }

        /// <summary>
        /// Defines how big the applied impulses can get.
        /// </summary>
        public sfloat Softness { get { return softness; } set { softness = value; } }

        /// <summary>
        /// The anchor point in the world.
        /// </summary>
        public JVector Anchor { get { return anchor; } set { anchor = value; } }


        /// <summary>
        /// Defines how big the applied impulses can get which correct errors.
        /// </summary>
        public sfloat BiasFactor { get { return biasFactor; } set { biasFactor = value; } }

        sfloat effectiveMass = sfloat.Zero;
        sfloat accumulatedImpulse = sfloat.Zero;
        sfloat bias;
        sfloat softnessOverDt;

        JVector[] jacobian = new JVector[2];

        /// <summary>
        /// Called once before iteration starts.
        /// </summary>
        /// <param name="timestep">The 5simulation timestep</param>
        public override void PrepareForIteration(sfloat timestep)
        {
            JVector p1,dp;
            JVector.Transform(ref localAnchor1, ref body1.orientation, out r1);
            JVector.Add(ref body1.position, ref r1, out p1);

            JVector.Subtract(ref p1, ref anchor, out dp);
            sfloat deltaLength = dp.Length();

            JVector n = anchor - p1;
            if (n.LengthSquared() != sfloat.Zero) n.Normalize();

            jacobian[0] = sfloat.MinusOne * n;
            jacobian[1] = sfloat.MinusOne * (r1 % n);

            effectiveMass = body1.inverseMass + JVector.Transform(jacobian[1], body1.invInertiaWorld) * jacobian[1];

            softnessOverDt = softness / timestep;
            effectiveMass += softnessOverDt;

            effectiveMass = sfloat.One / effectiveMass;

            bias = deltaLength * biasFactor * (sfloat.One / timestep);

            if (!body1.isStatic)
            {
                body1.linearVelocity += body1.inverseMass * accumulatedImpulse * jacobian[0];
                body1.angularVelocity += JVector.Transform(accumulatedImpulse * jacobian[1], body1.invInertiaWorld);
            }
        }

        /// <summary>
        /// Iteratively solve this constraint.
        /// </summary>
        public override void Iterate()
        {
            sfloat jv =
                body1.linearVelocity * jacobian[0] +
                body1.angularVelocity * jacobian[1];

            sfloat softnessScalar = accumulatedImpulse * softnessOverDt;

            sfloat lambda = -effectiveMass * (jv + bias + softnessScalar);

            accumulatedImpulse += lambda;

            if (!body1.isStatic)
            {
                body1.linearVelocity += body1.inverseMass * lambda * jacobian[0];
                body1.angularVelocity += JVector.Transform(lambda * jacobian[1], body1.invInertiaWorld);
            }
        }

        public override void DebugDraw(IDebugDrawer drawer)
        {
            drawer.DrawPoint(anchor);
        }

    }

}
