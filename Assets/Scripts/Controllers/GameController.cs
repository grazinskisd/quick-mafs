using UnityEngine.SceneManagement;
using Zenject;

namespace QuickMafs
{
    public class GameController : IInitializable
    {
        [Inject] private BoardController.Factory _boardFactory;
        [Inject] private SoundController _soundController;
        [Inject] private GameMenuController _gameMenu;

        BoardController _board;

        public void Initialize()
        {
            _board = _boardFactory.Create();
            _soundController.AssignBoard(_board);
            _gameMenu.ExitPressed += OnExitPressed;
        }

        private void OnExitPressed()
        {
            SceneManager.LoadScene(Scene.MENU.SceneName, LoadSceneMode.Single);
        }
    }
}
