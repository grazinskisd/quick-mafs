using Zenject;
using UnityEngine;

namespace QuickMafs
{
    public class PlayerHighscoreService : IInitializable
    {
        private const string HIGHSCORE = "PlayerHighscore";

        private int _highscore;

        public void Initialize()
        {
            if (!PlayerPrefs.HasKey(HIGHSCORE))
            {
                SetHighscore(0);
            }
            _highscore = PlayerPrefs.GetInt(HIGHSCORE);
        }

        private void SetHighscore(int score)
        {
            PlayerPrefs.SetInt(HIGHSCORE, score);
        }

        public int Highscore
        {
            get
            {
                return _highscore;
            }

            set
            {
                SetHighscore(value);
            }
        }
    }
}
