using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class BoardController
    {
        [Inject] private Settings _settings;
        [Inject] private FontSettings _font;
        [Inject] private BoardView _boardView;
        [Inject] private TileView _viewPrototype;

        private BoardModel _model;
        private TileView[,] _tiles;

        [Inject]
        public void Initialize()
        {
            SetupModel();
            SetupView();
        }

        private void SetupView()
        {
            _boardView = Object.Instantiate(_boardView);
            _tiles = new TileView[_settings.Width, _settings.Height];
            InitializeBoard();
        }

        private void SetupModel()
        {
            _model = new BoardModel
            {
                Width = _settings.Width,
                Height = _settings.Height
            };
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < _model.Width; row++)
            {
                for (int col = 0; col < _model.Height; col++)
                {
                    var tile = Object.Instantiate(_viewPrototype, _boardView.transform, false);
                    tile.transform.localPosition = new Vector2(row, col);
                    tile.Text.sprite = _font.GetSpriteForLetter(FontSettings.GetRandomLetter());
                    tile.name = string.Format("Tile ({0}, {1})", row, col);
                    tile.Foreground.color = _settings.DefaultTileColor;

                    _tiles[row, col] = tile;
                }
            }
        }

        public class Factory: PlaceholderFactory<BoardController> { }

        [System.Serializable]
        public class Settings
        {
            public int Width;
            public int Height;
            public Color DefaultTileColor;
            public Color SelectedTileColor;
        }
    }

    public class BoardModel
    {
        public int Width;
        public int Height;
    }
}