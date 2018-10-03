using Zenject;

namespace QuickMafs
{
    public class GameController : IInitializable
    {
        [Inject] private BoardController.Factory _boardFactory;
        [Inject] private SoundController _soundController;

        BoardController _board;

        public void Initialize()
        {
            _board = _boardFactory.Create();
            _soundController.AssignBoard(_board);
        }
    }
}
