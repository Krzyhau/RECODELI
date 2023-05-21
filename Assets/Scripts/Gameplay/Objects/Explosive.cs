using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BEPUphysics.Unity;
using SoftFloat;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace RecoDeli.Scripts.Gameplay
{
    public class Explosive : MonoBehaviour, IBepuEntityListener
    {
        [SerializeField] private ParticleSystem explosionParticle;
        [SerializeField] private MeshRenderer glowingIndicatorMesh;
        [SerializeField] private AudioSource indicatorBeepingSound;
        [SerializeField] private float indicationMaxSpeed;
        [SerializeField] private float indicationMinDistance;
        [SerializeField] private float explosionForce;
        [SerializeField] private float explosionRadius;

        private float indicationCycle;
        private bool currentIndicationState = false;

        public BepuRigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<BepuRigidbody>();
        }

        public void Update()
        {
            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
            var closestRigidbodyInRangeDistances = Rigidbody.Simulation.Rigidbodies
                .Where(r => r != null && r.GameObject != gameObject && r.Entity.IsDynamic)
                .Select(r => (float)(r.Entity.Position - Rigidbody.Entity.Position).Length())
                .OrderBy(r => r);

            if (closestRigidbodyInRangeDistances.Count() == 0)
            {
                indicationCycle = 0;
            }
            else
            {
                var minDistance = closestRigidbodyInRangeDistances.First() - indicationMinDistance;
                var indicationIncrement = Mathf.Lerp(indicationMaxSpeed, 0.0f, minDistance/explosionRadius);
                indicationCycle += Mathf.Min(indicationIncrement * Time.deltaTime, 0.5f);

                if (indicationCycle > 1.0f)
                {
                    indicationCycle -= 1.0f;
                }
            }

            var newIndicationState = indicationCycle > 0.5f;
            if(newIndicationState != currentIndicationState)
            {
                glowingIndicatorMesh.material.SetFloat("_Intensity", newIndicationState ? 30.0f : 5.0f);

                currentIndicationState = newIndicationState;
                if (newIndicationState)
                {
                    indicatorBeepingSound.Play();
                }
            }
        }

        public void OnBepuCollisionEnter(Collidable other, CollidablePairHandler info)
        {
            var rigidbodiesInRange = Rigidbody.Simulation.Rigidbodies
                .Where(r => r != null && r.GameObject != gameObject && r.Entity.IsDynamic)
                .ToList();

            foreach(var rigidbody in rigidbodiesInRange)
            {
                var delta = (rigidbody.Entity.Position - Rigidbody.Entity.Position);
                var distance = delta.Length();
                if (distance > (sfloat)explosionRadius) continue;

                var pushVector = BEPUutilities.Vector3.Lerp(
                    delta.Normalized() * (sfloat)explosionForce,
                    BEPUutilities.Vector3.Zero,
                    distance / (sfloat)explosionRadius
                );

                rigidbody.Entity.LinearVelocity += pushVector;
            }

            Instantiate(explosionParticle, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
