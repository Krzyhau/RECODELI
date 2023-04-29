using LudumDare.Scripts.Components;
using Zenject;

namespace LudumDare.Scripts
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallScene();
        }

        private void InstallScene()
        {
            Container.BindInterfacesAndSelfTo<LevelLoop>().FromComponentInHierarchy().AsCached();
        }
    }
}
