using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Unity;
using System.Collections.Generic;
using UnityEngine;
using System;
using RecoDeli.Scripts.Gameplay;
using System.Linq;

namespace RecoDeli.Scripts.Audio
{
    public class PhysicsSoundEmitter : MonoBehaviour
    {
        [Serializable]
        public struct MaterialSounds
        {
            public PhysicsMaterialType Type;
            public AudioClip SmallHitSound;
            public AudioClip MediumHitSound;
            public AudioClip LargeHitSound;
            public AudioClip FrictionSound;
        }

        [SerializeField] private BepuSimulation physicsSimulation;
        [SerializeField] private AudioSource audioSourcePrefab;

        [SerializeField] private float smallHitThreshold;
        [SerializeField] private float mediumHitThreshold;
        [SerializeField] private float largeHitThreshold;

        [SerializeField] private List<MaterialSounds> materialSounds;

        private void OnEnable()
        {
            physicsSimulation.OnGlobalCollisionEnter += OnBepuCollisionEnter;
            physicsSimulation.OnGlobalCollisionStay += OnBepuCollisionStay;
            physicsSimulation.OnGlobalCollisionExit += OnBepuCollisionExit;
        }

        private void OnDisable() 
        {
            physicsSimulation.OnGlobalCollisionEnter -= OnBepuCollisionEnter;
            physicsSimulation.OnGlobalCollisionStay -= OnBepuCollisionStay;
            physicsSimulation.OnGlobalCollisionExit -= OnBepuCollisionExit;
        }

        private void OnBepuCollisionEnter(EntityCollidable collider, Collidable other, CollidablePairHandler pairHandler)
        {
            var material = PhysicsMaterialType.Wood;

            if(collider.Tag is BepuRigidbody rigidbody)
            {
                if(rigidbody.TryGetComponent<PhysicsElement>(out var physElement))
                {
                    material = physElement.Material;
                }
            }

            var sounds = materialSounds.Where(s => s.Type == material).First();

            float impulse = 0.0f;
            Vector3 position = Vector3.zero;

            foreach(var point in pairHandler.Contacts)
            {
                impulse += (float)point.NormalImpulse;
                position += point.Contact.Position.ToUnity();
            }

            impulse /= pairHandler.Contacts.Count;
            position /= pairHandler.Contacts.Count;

            var soundClip = impulse switch
            {
                float f when f > largeHitThreshold => sounds.LargeHitSound,
                float f when f > mediumHitThreshold => sounds.MediumHitSound,
                float f when f > smallHitThreshold => sounds.SmallHitSound,
                _ => null
            };

            // Debug.Log($"Playing sound {soundClip} at {position} (collision of {material} with force {impulse}ups)");

            PlayOneShotAt(soundClip, position);
        }

        private void OnBepuCollisionStay(EntityCollidable collider, Collidable other, CollidablePairHandler pairHandler)
        {

        }

        private void OnBepuCollisionExit(EntityCollidable collider, Collidable other, CollidablePairHandler pairHandler)
        {

        }

        private void PlayOneShotAt(AudioClip clip, Vector3 position)
        {
            if (clip == null) return;

            var audioSource = Instantiate(audioSourcePrefab, transform);
            audioSource.transform.position = position;
            audioSource.PlayOneShot(clip);

            Destroy(audioSource.gameObject, clip.length+2.0f);
        }
    }
}
