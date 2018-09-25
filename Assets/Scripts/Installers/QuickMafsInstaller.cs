using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class QuickMafsInstaller : MonoInstaller<QuickMafsInstaller>
    {
        public BoardView GameBoard;
        public TileView TileView;
        public FontSprites Font;
        public GameSettings GameSettings;

        public override void InstallBindings()
        {
            Container.BindInstance(GameBoard);
            Container.BindInstance(TileView);
            Container.BindInstance(Font);
            Container.BindInstance(GameSettings);

            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();
            Container.BindFactory<TileParams, TileController, TileController.Factory>().AsSingle();
            Container.BindFactory<BoardParams, BoardController, BoardController.Factory>().AsSingle();
        }
    }
}