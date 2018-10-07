using Zenject;
using UnityEngine;
using DG.Tweening;

namespace QuickMafs
{
    public delegate void ScoreEventHandler(int score);

    public class ScoreController : IInitializable
    {
        [Inject] private ScoreView _scoreView;

        public event ScoreEventHandler ScoreUpdated;

        private static string SCORE_FORMAT = "{0}";
        private int Score;

        private Tween _currentTween;

        public void Initialize()
        {
            var canvas = GameObject.FindObjectOfType<Canvas>();
            _scoreView = GameObject.Instantiate(_scoreView, canvas.transform, false);
            UpdateScoreText();
        }

        public void IncrementScore(int byValue)
        {
            Score += byValue;
            UpdateScoreText();
            if(ScoreUpdated != null)
            {
                ScoreUpdated(Score);
            }
        }

        private void UpdateScoreText()
        {
            _scoreView.TextComponent.text = string.Format(SCORE_FORMAT, Score);
        }
    }
}
