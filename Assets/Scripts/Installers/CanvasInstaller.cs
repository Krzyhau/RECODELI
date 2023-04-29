using LudumDare.Scripts.Components;
using UnityEngine;
using Zenject;

namespace LudumDare.Scripts
{
    public class CanvasInstaller : MonoInstaller
    {
        [SerializeField] private InstructionButton instructionButtonPrefab;

        public override void InstallBindings()
        {
            InstallComponents();
        }

        private void InstallComponents()
        {
            Container.Bind<InstructionButton>().FromInstance(instructionButtonPrefab).AsCached();
        }
    }
}
