using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class QuickMafsInstaller : MonoInstaller<QuickMafsInstaller>
    {
        public Board GameBoard;

        public override void InstallBindings()
        {
            Container.BindInstance(GameBoard);
            Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();
        }
    }
}