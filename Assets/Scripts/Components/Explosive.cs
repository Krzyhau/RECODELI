using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LudumDare.Scripts.Components
{
    public class Explosive : MonoBehaviour
    {
        [SerializeField] private ParticleSystem explosionParticle;
        [SerializeField] private MeshRenderer glowingIndicatorMesh;
        [SerializeField] private float indicationMaxSpeed;
        [SerializeField] private float explosionForce;
        [SerializeField] private float explosionRadius;

        private float indicationCycle;
        private bool currentIndicationState = false;

        private void Update()
        {
            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
            var closestRigidbodyInRangeDistances = Physics.OverlapSphere(transform.position, explosionRadius)
                .Select(collider => collider.attachedRigidbody)
                .Where(r => r != null && r.gameObject != gameObject)
                .Select(r => (r.position - transform.position).magnitude)
                .OrderBy(r => r);

            if (closestRigidbodyInRangeDistances.Count() == 0)
            {
                indicationCycle = 0;
            }
            else
            {
                var minDistance = closestRigidbodyInRangeDistances.First();
                var indicationIncrement = Mathf.Lerp(indicationMaxSpeed * Time.deltaTime, 0.0f, minDistance/explosionRadius);
                indicationCycle += Mathf.Min(indicationIncrement, 0.5f);

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
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var rigidbodiesInRange = Physics.OverlapSphere(transform.position, explosionRadius)
                .Select(collider => collider.attachedRigidbody)
                .Where(r => r != null && r.gameObject != gameObject)
                .ToList();

            foreach(var rigidbody in rigidbodiesInRange)
            {
                var delta = (rigidbody.position - transform.position);
                var pushVector = Vector3.Lerp(delta.normalized * explosionForce, Vector2.zero, delta.magnitude / explosionRadius);

                rigidbody.AddForce(pushVector, ForceMode.Impulse);
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
