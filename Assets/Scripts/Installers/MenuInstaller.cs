using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class MenuInstaller : MonoInstaller<MenuInstaller>
    {
        public MainMenuView MenuView;

        public override void InstallBindings()
        {
            Container.BindInstance(MenuView);

            Container.BindInterfacesAndSelfTo<MainMenuController>().AsSingle();
        }
    }
}