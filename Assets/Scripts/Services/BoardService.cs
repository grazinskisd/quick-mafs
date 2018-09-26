using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class BoardService
    {
        [Inject] private FontSettings _font;

        public void CleanupColumns(TileController[,] tiles)
        {
            int nullCount = 0;
            for (int row = 0; row < tiles.GetLength(0); row++)
            {
                for (int col = 0; col < tiles.GetLength(1); col++)
                {
                    if (tiles[row, col] == null)
                    {
                        nullCount += 1;
                    }
                    else if (nullCount > 0)
                    {
                        tiles[row, col].Col = col - nullCount;
                        tiles[row, col - nullCount] = tiles[row, col];
                        tiles[row, col] = null;
                    }
                }
                nullCount = 0;
            }
        }

        public bool IsCorrectOrder(TileController tile, TileController lastTile)
        {
            return (lastTile.IsTileANumber() && tile.IsTileASymbol()) ||
                (tile.IsTileANumber() && lastTile.IsTileASymbol());
        }

        public bool AreTilesNeighbouring(TileController tile, TileController otherTile)
        {
            return (Mathf.Abs(tile.Row - otherTile.Row) == 1 && tile.Col == otherTile.Col)
                || (Mathf.Abs(tile.Col - otherTile.Col) == 1 && tile.Row == otherTile.Row);
        }

        public int GetIncrement(Letter operation, TileController tile)
        {
            switch (operation)
            {
                case Letter.L_minus:
                    return -(int)tile.Letter;
                default:
                    return (int)tile.Letter;
            }
        }

        public bool IsInNumberBounds(int number)
        {
            return number >= 0 && number <= 9;
        }
    }
}