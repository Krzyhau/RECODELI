using System;
using SoftFloat;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;
 
using BEPUphysics.Settings;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms
{
    ///<summary>
    /// Static class with methods to help with testing box shapes against sphere shapes.
    ///</summary>
    public static class BoxSphereTester
    {
        ///<summary>
        /// Tests if a box and sphere are colliding.
        ///</summary>
        ///<param name="box">Box to test.</param>
        ///<param name="sphere">Sphere to test.</param>
        ///<param name="boxTransform">Transform to apply to the box.</param>
        ///<param name="spherePosition">Transform to apply to the sphere.</param>
        ///<param name="contact">Contact point between the shapes, if any.</param>
        ///<returns>Whether or not the shapes were colliding.</returns>
        public static bool AreShapesColliding(BoxShape box, SphereShape sphere, ref RigidTransform boxTransform, ref Vector3 spherePosition, out ContactData contact)
        {
            contact = new ContactData();

            Vector3 localPosition;
            RigidTransform.TransformByInverse(ref spherePosition, ref boxTransform, out localPosition);
#if !WINDOWS
            Vector3 localClosestPoint = new Vector3();
#else
            Vector3 localClosestPoint;
#endif
            localClosestPoint.X = MathHelper.Clamp(localPosition.X, -box.halfWidth, box.halfWidth);
            localClosestPoint.Y = MathHelper.Clamp(localPosition.Y, -box.halfHeight, box.halfHeight);
            localClosestPoint.Z = MathHelper.Clamp(localPosition.Z, -box.halfLength, box.halfLength);

            RigidTransform.Transform(ref localClosestPoint, ref boxTransform, out contact.Position);

            Vector3 offset;
            Vector3.Subtract(ref spherePosition, ref contact.Position, out offset);
            sfloat offsetLength = offset.LengthSquared();

            if (offsetLength > (sphere.collisionMargin + CollisionDetectionSettings.maximumContactDistance) * (sphere.collisionMargin + CollisionDetectionSettings.maximumContactDistance))
            {
                return false;
            }

            //Colliding.
            if (offsetLength > Toolbox.Epsilon)
            {
                offsetLength = libm.sqrtf(offsetLength);
                //Outside of the box.
                Vector3.Divide(ref offset, offsetLength, out contact.Normal);
                contact.PenetrationDepth = sphere.collisionMargin - offsetLength;
            }
            else
            {
                //Inside of the box.
                Vector3 penetrationDepths;
                penetrationDepths.X = localClosestPoint.X < sfloat.Zero ? localClosestPoint.X + box.halfWidth : box.halfWidth - localClosestPoint.X;
                penetrationDepths.Y = localClosestPoint.Y < sfloat.Zero ? localClosestPoint.Y + box.halfHeight : box.halfHeight - localClosestPoint.Y;
                penetrationDepths.Z = localClosestPoint.Z < sfloat.Zero ? localClosestPoint.Z + box.halfLength : box.halfLength - localClosestPoint.Z;
                if (penetrationDepths.X < penetrationDepths.Y && penetrationDepths.X < penetrationDepths.Z)
                {
                    contact.Normal = localClosestPoint.X > sfloat.Zero ? Toolbox.RightVector : Toolbox.LeftVector; 
                    contact.PenetrationDepth = penetrationDepths.X;
                }
                else if (penetrationDepths.Y < penetrationDepths.Z)
                {
                    contact.Normal = localClosestPoint.Y > sfloat.Zero ? Toolbox.UpVector : Toolbox.DownVector; 
                    contact.PenetrationDepth = penetrationDepths.Y;
                }
                else
                {
                    contact.Normal = localClosestPoint.Z > sfloat.Zero ? Toolbox.BackVector : Toolbox.ForwardVector; 
                    contact.PenetrationDepth = penetrationDepths.Z;
                }
                contact.PenetrationDepth += sphere.collisionMargin;
                Quaternion.Transform(ref contact.Normal, ref boxTransform.Orientation, out contact.Normal);
            }


            return true;
        }
    }
}
