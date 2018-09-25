using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class QuickMafsInstaller : MonoInstaller<QuickMafsInstaller>
    {
        public Board GameBoard;
        public TileView TileView;
        public FontSprites Font;

        public override void InstallBindings()
        {
            Container.BindInstance(GameBoard);
            Container.BindInstance(TileView);
            Container.BindInstance(Font);

            Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();
            Container.BindFactory<TileParams, TileController, TileController.Factory>().AsSingle();
        }
    }
}