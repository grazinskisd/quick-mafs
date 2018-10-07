using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class HighscoreController : IInitializable
    {
        [Inject] private ScoreController _scoreController;
        [Inject] private PlayerHighscoreService _highscoreService;

        public void Initialize()
        {
            _scoreController.ScoreUpdated += OnScoreUpdated;
        }

        private void OnScoreUpdated(int score)
        {
            if(score > _highscoreService.Highscore)
            {
                _highscoreService.Highscore = score;
            }
        }
    }
}
