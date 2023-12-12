using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class RobotEyesAnimator : MonoBehaviour
    {
        [SerializeField] private RobotController controller;
        [SerializeField] private MeshRenderer eyesRenderer;

        struct EyelidState
        {
            public Vector2 Normal;
            public float Offset;
            public float Size;
        }

        struct EyesState
        {
            public EyelidState LeftEyelids;
            public EyelidState RightEyelids;

            public Vector2 Direction;
        }

        private float blinkNext;
        private float blinkState;

        private float nextFreeLookTarget;
        private Vector2 targetFreeLook;
        private Vector2 currentFreeLook;

        private void OnEnable()
        {
            blinkNext = Time.time + Random.Range(0.1f, 3.0f);
            nextFreeLookTarget = Time.time + Random.Range(0.2f, 2.0f);
        }

        private void Update()
        {
            var state = new EyesState()
            {
                LeftEyelids = new() { Normal = Vector2.down, Offset = 0.15f, Size = 0.25f },
                RightEyelids = new() { Normal = Vector2.down, Offset = 0.15f, Size = 0.25f },
                Direction = Vector2.zero
            };

            ApplyFreeLook(ref state);
            ApplyBlinking(ref state);

            SetMaterialVariables(state);
        }

        private void ApplyFreeLook(ref EyesState state)
        {
            if(nextFreeLookTarget < Time.time)
            {
                nextFreeLookTarget = Time.time + Random.Range(0.2f, 2.0f);
                targetFreeLook = new(
                    Random.Range(-0.1f, 0.1f),
                    Random.Range(-0.1f, 0.1f)
                );
            }
            currentFreeLook = Vector2.MoveTowards(currentFreeLook, targetFreeLook, Time.deltaTime * 2.0f);
            state.Direction = currentFreeLook;
        }

        private void ApplyBlinking(ref EyesState state)
        {
            if (blinkNext < Time.time)
            {
                blinkNext = Time.time + Random.Range(0.1f, 3.0f);
                blinkState = 1.0f;
            }

            if (blinkState > 0)
            {
                float s = Mathf.Abs(Mathf.Cos(blinkState * Mathf.PI));

                state.LeftEyelids.Size *= s;
                state.RightEyelids.Size *= s;
                state.LeftEyelids.Offset *= s;
                state.RightEyelids.Offset *= s;

                blinkState -= Time.deltaTime * 4.0f;
            }
        }

        private void SetMaterialVariables(EyesState state)
        {
            var mat = eyesRenderer.material;

            mat.SetVector("LookingDirection", state.Direction);
            mat.SetVector("LeftEyelids", new Vector4(
                state.LeftEyelids.Normal.x, 
                state.LeftEyelids.Normal.y, 
                state.LeftEyelids.Offset, 
                state.LeftEyelids.Size
            ));
            mat.SetVector("RightEyelids", new Vector4(
                state.RightEyelids.Normal.x,
                state.RightEyelids.Normal.y,
                state.RightEyelids.Offset,
                state.RightEyelids.Size
            ));
        }
    }
}
