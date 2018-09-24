using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class BoardController: IInitializable
    {
        [Inject] private Board _boardView;

        private Tile[,] _tiles;

        public void Initialize()
        {
            _boardView = GameObject.Instantiate(_boardView);
            _tiles = new Tile[_boardView.Width, _boardView.Height];
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < _boardView.Width; row++)
            {
                for (int col = 0; col < _boardView.Height; col++)
                {
                    var tile = GameObject.Instantiate(_boardView.TilePrefab);
                    tile.name = string.Format("Tile ({0}, {1})", row, col);
                    tile.transform.SetParent(_boardView.transform);
                    tile.transform.localPosition = new Vector2(row, col);

                    tile.Text.sprite = _boardView.Font.GetRandomLetterSprite();
                }
            }
        }
    }
}