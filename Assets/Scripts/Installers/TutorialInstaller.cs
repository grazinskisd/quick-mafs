using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class TutorialInstaller : MonoInstaller<MenuInstaller>
    {
        public TutorialView TutorialView;

        public override void InstallBindings()
        {
            Container.BindInstance(TutorialView);

            Container.BindInterfacesAndSelfTo<TutorialController>().AsSingle();
        }
    }
}