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

        private EyesState currentState;

        private void Start()
        {
            StartCoroutine(BlinkCoroutine());
            currentState.LeftEyelids = new() { Normal = Vector2.down, Offset = 0.15f, Size = 0.25f };
            currentState.RightEyelids = new() { Normal = Vector2.down, Offset = 0.15f, Size = 0.25f };
        }

        private void Update()
        {

            SetMaterialVariables(currentState);
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

        private IEnumerator BlinkCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 3.0f));

                float originalSize = currentState.LeftEyelids.Size;
                float originalOffset = currentState.LeftEyelids.Offset;


                for (float f = 0; f < 1.0; f += Time.deltaTime * 4.0f)
                {
                    float s = Mathf.Abs(Mathf.Cos(f * Mathf.PI));

                    currentState.LeftEyelids.Size = originalSize * s;
                    currentState.RightEyelids.Size = originalSize * s;
                    currentState.LeftEyelids.Offset = originalOffset * s;
                    currentState.RightEyelids.Offset = originalOffset * s;

                    yield return new WaitForEndOfFrame();
                }

                currentState.LeftEyelids.Size = originalSize;
                currentState.RightEyelids.Size = originalSize;
                currentState.LeftEyelids.Offset = originalOffset;
                currentState.RightEyelids.Offset = originalOffset;
            }
        }
    }
}
