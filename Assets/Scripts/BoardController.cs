using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class BoardController: IInitializable
    {
        [Inject] private Board _boardView;
        [Inject] private TileController.Factory _tileFactory;

        public void Initialize()
        {
            _boardView = GameObject.Instantiate(_boardView);
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < _boardView.Width; row++)
            {
                for (int col = 0; col < _boardView.Height; col++)
                {
                    var parameters = new TileParams
                    {
                        Name = string.Format("Tile ({0}, {1})", row, col),
                        Position = new Vector2(row, col),
                        Parent = _boardView.transform
                    };
                    _tileFactory.Create(parameters);
                }
            }
        }
    }
}