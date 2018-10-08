using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class TutorialInstaller : MonoInstaller<MenuInstaller>
    {
        public TutorialView TutorialView;
        public TutorialLayout TutorialLayout;

        public override void InstallBindings()
        {
            Container.BindInstance(TutorialView);
            Container.BindInstance(TutorialLayout);

            Container.BindInterfacesAndSelfTo<TutorialController>().AsSingle();
        }
    }
}