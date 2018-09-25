using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class BoardController
    {
        [Inject] private Settings _settings;
        [Inject] private BoardView _boardView;
        [Inject] private TileController.Factory _tileFactory;

        private BoardModel _model;

        [Inject]
        public void Initialize()
        {
            SetupModel();
            SetupView();
        }

        private void SetupView()
        {
            _boardView = Object.Instantiate(_boardView);
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
                    var parameters = new TileController.Settings
                    {
                        Name = string.Format("Tile ({0}, {1})", row, col),
                        Position = new Vector2(row, col),
                        Parent = _boardView.transform
                    };
                    _tileFactory.Create(parameters);
                }
            }
        }

        public class Factory: PlaceholderFactory<BoardController> { }

        [System.Serializable]
        public class Settings
        {
            public int Width;
            public int Height;
        }
    }

    public class BoardModel
    {
        public int Width;
        public int Height;
    }
}