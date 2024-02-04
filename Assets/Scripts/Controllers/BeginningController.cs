using System.Collections;
using UnityEngine;

namespace RecoDeli.Scripts.Controllers
{
    public class BeginningController : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private Canvas renderCanvas;

        private bool ended;
        public bool BeginningInProgress => !ended;
        private void Awake()
        {
            StartCoroutine(InitializeBeginning());
        }

        private IEnumerator InitializeBeginning()
        {
            ended = false;

            // Because of the rendering mess that I have created in order to
            // implement blurred interface, the initialisation of the interface
            // requires it to be drawn for the first frame. Then, to be able to set
            // intial state of interface reveal, it has to be set for at least one frame,
            // hence why I'm using these waits here. In order to actually hide this
            // mess from user, we disable render layers completely for this moment.

            renderCanvas.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            simulationManager.Interface.PrepareForInterfaceReveal();
            yield return new WaitForEndOfFrame();
            renderCanvas.gameObject.SetActive(true);
            simulationManager.Interface.RevealInterface(false);
            simulationManager.RobotController.ModelAnimator.SetTrigger("Landing");
            ended = true;
        }

    }
}
