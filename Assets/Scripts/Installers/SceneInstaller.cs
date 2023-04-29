using LudumDare.Scripts.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace LudumDare.Scripts
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private GameSettings gameSettings;
        public override void InstallBindings()
        {
            InstallSettings();
        }

        private void InstallSettings()
        {
            Container.Bind<GameSettings>().FromInstance(gameSettings).AsSingle().NonLazy();
        }
    }
}
