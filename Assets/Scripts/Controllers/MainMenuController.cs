using Zenject;
using UnityEngine.SceneManagement;
using System;

namespace QuickMafs
{
    public class MainMenuController : IInitializable
    {
        [Inject] private MainMenuView _view;
        [Inject] private PlayerHighscoreService _highscoreService;

        public void Initialize()
        {
            _view.PlayButton.onClick.AddListener(OnPlayPressed);
            _view.tutorialButton.onClick.AddListener(OnTutorialPressed);
            _view.HighscoreText.text = _highscoreService.Highscore.ToString();
        }

        private void OnTutorialPressed()
        {
            SceneManager.LoadScene(Scene.TUTORIAL.SceneName, LoadSceneMode.Single);
        }

        private void OnPlayPressed()
        {
            SceneManager.LoadScene(Scene.GAME.SceneName, LoadSceneMode.Single);
        }
    }
}
