using Zenject;
using UnityEngine.SceneManagement;

namespace QuickMafs
{
    public class MainMenuController : IInitializable
    {
        [Inject] private MainMenuView _view;
        [Inject] private PlayerHighscoreService _highscoreService;

        public void Initialize()
        {
            _view.PlayButton.onClick.AddListener(OnPlayPressed);
            _view.HighscoreText.text = _highscoreService.Highscore.ToString();
        }

        private void OnPlayPressed()
        {
            SceneManager.LoadScene(Scene.GAME.SceneName, LoadSceneMode.Single);
        }
    }
}
