using Zenject;
using UnityEngine;

namespace QuickMafs
{
    public class TileController
    {
        [Inject] private FontSprites _font;
        [Inject] private TileView _view;

        private TileModel _model;

        [Inject]
        private void Initialize(TileParams parameters)
        {
            InitializeModel(parameters);
            InitializeView(parameters);
        }

        private void InitializeView(TileParams parameters)
        {
            _view = Object.Instantiate(_view, parameters.Parent, false);
            _view.transform.localPosition = parameters.Position;
            _view.Text.sprite = _font.GetSpriteForLetter(_model.Letter);
            _view.name = parameters.Name;
        }

        private void InitializeModel(TileParams parameters)
        {
            _model = new TileModel
            {
                Letter = FontSprites.GetRandomLetter(),
                Position = parameters.Position
            };
        }

        public class Factory: PlaceholderFactory<TileParams, TileController> { }
    }

    public class TileParams
    {
        public string Name;
        public Vector3 Position;
        public Transform Parent;
    }

    public class TileModel
    {
        public Letter Letter;
        public Vector3 Position;
    }
}
