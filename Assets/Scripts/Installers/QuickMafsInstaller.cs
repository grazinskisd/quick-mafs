using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class QuickMafsInstaller : MonoInstaller<QuickMafsInstaller>
    {
        public BoardView GameBoard;
        public TileView TileView;

        public override void InstallBindings()
        {
            Container.BindInstance(GameBoard);
            Container.BindInstance(TileView);

            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.BindInterfacesAndSelfTo<TickManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardService>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardController>().AsSingle();
            Container.BindFactory<BoardController, BoardController.Factory>().AsSingle();
            Container.BindFactory<TileParams, TileController, TileController.Factory>().AsSingle();
        }
    }
}