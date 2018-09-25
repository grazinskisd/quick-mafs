using Zenject;
using UnityEngine;

namespace QuickMafs
{
    public class TileController
    {
        [Inject] private FontSettings _font;
        [Inject] private TileView _view;

        private TileModel _model;

        [Inject]
        private void Initialize(Settings settings)
        {
            InitializeModel(settings);
            InitializeView(settings);
        }

        private void InitializeView(Settings settings)
        {
            _view = Object.Instantiate(_view, settings.Parent, false);
            _view.transform.localPosition = settings.Position;
            _view.Text.sprite = _font.GetSpriteForLetter(_model.Letter);
            _view.name = settings.Name;
        }

        private void InitializeModel(Settings settings)
        {
            _model = new TileModel
            {
                Letter = FontSettings.GetRandomLetter(),
                Position = settings.Position
            };
        }

        public class Factory: PlaceholderFactory<Settings, TileController> { }

        public class Settings
        {
            public string Name;
            public Vector3 Position;
            public Transform Parent;
        }
    }

    public class TileModel
    {
        public Letter Letter;
        public Vector3 Position;
    }
}
