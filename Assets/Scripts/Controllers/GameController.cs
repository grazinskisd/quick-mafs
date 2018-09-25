using Zenject;

namespace QuickMafs
{
    public class GameController : IInitializable
    {
        [Inject] private BoardController.Factory _boardFactory;

        public void Initialize()
        {
            _boardFactory.Create();
        }
    }
}
