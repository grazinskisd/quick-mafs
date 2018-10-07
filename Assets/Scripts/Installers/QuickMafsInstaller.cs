using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class QuickMafsInstaller : MonoInstaller<QuickMafsInstaller>
    {
        public BoardView GameBoard;
        public TileView TileView;
        public ScoreView ScoreView;
        public BurstEffectView BurstEffectView;
        public ScoreLocator ScoreLocator;
        public GameMenuView GameMenuView;

        public override void InstallBindings()
        {
            Container.BindInstance(GameBoard);
            Container.BindInstance(TileView);
            Container.BindInstance(ScoreView);
            Container.BindInstance(ScoreLocator);
            Container.BindInstance(BurstEffectView);
            Container.BindInstance(GameMenuView);

            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.BindInterfacesAndSelfTo<TickManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardService>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreController>().AsSingle();
            Container.BindInterfacesAndSelfTo<SoundController>().AsSingle();
            Container.BindInterfacesAndSelfTo<LateTickController>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameMenuController>().AsSingle();
            Container.BindFactory<BoardController, BoardController.Factory>().AsSingle();
            Container.BindFactory<TileParams, TileController, TileController.Factory>().AsSingle();
            Container.BindFactory<ScoreEffectParameters, ScoreEffectController, ScoreEffectController.Factory>().AsSingle();
        }
    }
}