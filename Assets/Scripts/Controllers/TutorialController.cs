using System;
using UnityEngine.SceneManagement;
using Zenject;

namespace QuickMafs
{
    public class TutorialController : IInitializable
    {
        [Inject] private TutorialView _view;
        [Inject] private TutorialLayout _layout;

        private const string PAGES_FORMAT = "{0}/{1}";

        private int _totalPages;
        private int _currentPage;

        public void Initialize()
        {
            _totalPages = _layout.Pages.Length;
            _currentPage = 0;
            UpdatePage();
            _view.PreviousButton.onClick.AddListener(OnPreviousPressed);
            _view.NextButton.onClick.AddListener(OnNextPressed);
            _view.ExitButton.onClick.AddListener(OnExitPressed);
        }

        private void OnExitPressed()
        {
            SceneManager.LoadScene(Scene.MENU.SceneName);
        }

        private void OnNextPressed()
        {
            if(_currentPage+1 < _totalPages)
            {
                _currentPage++;
                UpdatePage();
            }
        }

        private void OnPreviousPressed()
        {
            if (_currentPage-1 >= 0)
            {
                _currentPage--;
                UpdatePage();
            }
        }

        private void UpdatePage()
        {
            DisplayPage(_layout.Pages[_currentPage]);
            UpdatePagesText();
        }

        private void UpdatePagesText()
        {
            _view.PagesText.text = string.Format(PAGES_FORMAT, _currentPage+1, _totalPages);
        }

        private void DisplayPage(TutorialPage page)
        {
            if (page.Image != null)
            {
                _view.TutorialImage.enabled = true;
                _view.TutorialImage.sprite = page.Image;
            }
            else
            {
                _view.TutorialImage.enabled = false;
            }
            _view.TutorialText.text = page.Text;
        }
    }
}
