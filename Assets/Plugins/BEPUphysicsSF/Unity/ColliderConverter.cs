
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUutilities.FixedMath;
using System.Linq;
using UnityEngine;
using BEPUphysics.CollisionShapes;

using UVector3 = UnityEngine.Vector3;
using UQuaternion = UnityEngine.Quaternion;
using BVector3 = BEPUutilities.Vector3;
using BQuaternion = BEPUutilities.Quaternion;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

namespace BEPUphysics.Unity
{
    public static class ColliderConverter
    {
        public static EntityCollidable ToBEPUCollideable(this Collider collider)
        {
            var scale = collider.transform.localScale.ToBEPU();
            switch (collider)
            {
                case BoxCollider boxCollider:
                    var boxSize = boxCollider.size.ToBEPU() * scale;
                    var boxCollideable = new ConvexCollidable<BoxShape>(new BoxShape(boxSize.X, boxSize.Y, boxSize.Z));
                    boxCollideable.WorldTransform = new RigidTransform(boxCollider.center.ToBEPU() * scale, BQuaternion.Identity);
                    return boxCollideable;
                case SphereCollider sphereCollider:
                    if (scale.X != scale.Y || scale.X != scale.Z)
                    {
                        Debug.LogWarning($"Non-uniform scale for SphereCollider {collider.gameObject}! {scale.X} {scale.Y} {scale.Z}");
                    }
                    var sphereRadius = (scale.X + scale.Y + scale.Z) / (fint)3.0f * (fint)sphereCollider.radius;
                    var sphereCollideable = new ConvexCollidable<SphereShape>(new SphereShape(sphereRadius));
                    sphereCollideable.WorldTransform = new RigidTransform(sphereCollider.center.ToBEPU() * scale, BQuaternion.Identity);
                    return sphereCollideable;
                case MeshCollider meshCollider:
                    var mesh = meshCollider.sharedMesh;
                    var vertices = mesh.vertices
                        .Select(vertex => vertex.ToBEPU())
                        .ToArray();
                    var indices = mesh.GetIndices(0);
                    var defaultTransform = new AffineTransform(BVector3.One, BQuaternion.Identity, BVector3.Zero);
                    var meshCollidable = new MobileMeshCollidable(new MobileMeshShape(vertices, indices, defaultTransform, MobileMeshSolidity.DoubleSided));
                    meshCollidable.WorldTransform = new RigidTransform(BVector3.Zero, BQuaternion.Identity);
                    return meshCollidable;
                case CapsuleCollider capsuleCollider:
                    if (scale.X != scale.Y || scale.X != scale.Z)
                    {
                        Debug.LogWarning($"Non-uniform scale for CapsuleCollider {collider.gameObject}! {scale.X} {scale.Y} {scale.Z}");
                    }
                    var size = (scale.X + scale.Y + scale.Z) / (fint)3.0f;
                    var capsuleRadius = (fint)capsuleCollider.radius;
                    var offsetAxis = capsuleCollider.direction switch
                    {
                        0 => BVector3.Right * scale.X,
                        1 => BVector3.Up * scale.Y,
                        2 => BVector3.Forward * scale.Z,
                        _ => BVector3.Zero
                    };
                    var halfHeight = (fint)capsuleCollider.height * (fint)0.5f;
                    var halfHeightMinusRadius = fint.Max((fint)0, halfHeight - capsuleRadius);
                    var center = capsuleCollider.center.ToBEPU() * scale;
                    var start = -offsetAxis * halfHeightMinusRadius;
                    var end = offsetAxis * halfHeightMinusRadius;
                    Capsule.GetCapsuleInformation(ref start, ref end, out var orientation, out var length);
                    var capsuleCollideable = new ConvexCollidable<CapsuleShape>(new CapsuleShape(length, capsuleRadius * size));
                    capsuleCollideable.WorldTransform = new RigidTransform(center, orientation);

                    return capsuleCollideable;
            }

            Debug.LogWarning($"Attempted to create BEPUphysics collider for {collider.gameObject} using unknown Unity collider type {collider.GetType().Name}");
            return null;
        }
    }
}