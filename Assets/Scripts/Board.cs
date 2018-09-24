using UnityEngine;

namespace QuickMafs
{
    public class Board : MonoBehaviour
    {
        public int Width;
        public int Height;
        public Tile TilePrefab;
        public FontSprites Font;

        private GameObject[,] _board;

        private void Start()
        {
            _board = new GameObject[Width, Height];
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < Width; row++)
            {
                for (int col = 0; col < Height; col++)
                {
                    var tile = Instantiate(TilePrefab);
                    tile.name = string.Format("Tile ({0}, {1})", row, col);
                    tile.transform.SetParent(transform);
                    tile.transform.localPosition = new Vector2(row, col);

                    tile.Text.sprite = Font.GetRandomLetterSprite();
                }
            }
        }
    }
}