using Zenject;
using UnityEngine.SceneManagement;

namespace QuickMafs
{
    public class MainMenuController : IInitializable
    {
        [Inject] private MainMenuView _view;

        public void Initialize()
        {
            _view.PlayButton.onClick.AddListener(OnPlayPressed);
        }

        private void OnPlayPressed()
        {
            SceneManager.LoadScene(Scene.GAME.SceneName, LoadSceneMode.Single);
        }
    }
}
