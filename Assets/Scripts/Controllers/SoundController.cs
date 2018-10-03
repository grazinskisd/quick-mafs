using Zenject;
using UnityEngine;

namespace QuickMafs
{
    public class SoundController : IInitializable
    {
        [Inject] private Settings _settings;

        private AudioSource _audioSource;
        private BoardController _board;

        public void Initialize()
        {
            _audioSource = GameObject.FindObjectOfType<AudioSource>();
        }

        public void AssignBoard(BoardController board)
        {
            _board = board;
            _board.TileSelected += OnTileSelected;
            _board.MatchMade += OnMatchMade;
        }

        private void OnMatchMade()
        {
            _audioSource.PlayOneShot(_settings.MatchMade);
        }

        private void OnTileSelected()
        {
            _audioSource.PlayOneShot(_settings.TileSelected, _settings.TileSelectedVolume);
        }

        [System.Serializable]
        public class Settings
        {
            public AudioClip TileSelected;
            public float TileSelectedVolume;
            public AudioClip MatchMade;
        }
    }
}
