using Zenject;

namespace QuickMafs
{
    public class GameController : IInitializable
    {
        [Inject] private GameSettings _settings;
        [Inject] private BoardController.Factory _boardFactory;

        public void Initialize()
        {
            var parameters = new BoardParams
            {
                Width = _settings.BoardWidth,
                Height = _settings.BoardHeight
            };
            _boardFactory.Create(parameters);
        }
    }
}
