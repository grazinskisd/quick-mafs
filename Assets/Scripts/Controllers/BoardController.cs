using System;
using System.Collections.Generic;
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
        [Inject] private InputManager _input;

        private TileView[,] _tiles;

        List<TileView> _objectList = new List<TileView>();

        private int _currentResult;
        private Letter _lastOperation;

        [Inject]
        public void Initialize()
        {
            SetupView();
            _input.FirstTileSelected += OnFirstTileSelected;
            _input.TileSelected += OnTileSelected;
            _input.MouseUp += OnMouseUp;
        }

        private void SetupView()
        {
            _boardView = GameObject.Instantiate(_boardView);
            _tiles = new TileView[_settings.Width, _settings.Height];
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < _settings.Width; row++)
            {
                for (int col = 0; col < _settings.Height; col++)
                {
                    AddNewTile(row, col);
                }
            }
        }

        private void AddNewTile(int row, int col)
        {
            var tile = GameObject.Instantiate(_viewPrototype, _boardView.transform, false);
            tile.Letter = FontSettings.GetRandomLetter();
            tile.transform.localPosition = new Vector2(row, col);
            tile.Text.sprite = _font.GetSpriteForLetter(tile.Letter);
            tile.name = string.Format("Tile ({0}, {1})", row, col);
            tile.Row = row; tile.Col = col;
            tile.Foreground.color = _settings.DefaultTileColor;
            _tiles[row, col] = tile;
        }

        private void OnMouseUp()
        {
            if (_objectList.Count == 0) return;

            var lastTile = _objectList[_objectList.Count - 1];
            if (IsTileANumber(lastTile))
            {
                if (_currentResult == 0)
                {
                    for (int i = 0; i < _objectList.Count; i++)
                    {
                        var tile = _objectList[i];
                        _tiles[tile.Row, tile.Col] = null;
                        GameObject.Destroy(tile.gameObject);
                    }
                }
                else
                {
                    for (int i = 0; i < _objectList.Count - 1; i++)
                    {
                        var tile = _objectList[i];
                        _tiles[tile.Row, tile.Col] = null;
                        GameObject.Destroy(_objectList[i].gameObject);
                    }
                    DeselectTile(lastTile);
                    lastTile.Letter = (Letter)_currentResult;
                    lastTile.Text.sprite = _font.GetSpriteForLetter(lastTile.Letter);
                }
            }
            else
            {
                for (int i = 0; i < _objectList.Count; i++)
                {
                    DeselectTile(_objectList[i]);
                }
            }
            _objectList.Clear();
            CleanupColumns();
            RefilBoard();
        }

        private void OnTileSelected(TileView tile)
        {
            if (_objectList.Count > 0 &&
                !_objectList.Contains(_tiles[tile.Row, tile.Col]) &&
                IsNeighbouringWithLast(tile) &&
                IsCorrectOrder(tile))
            {
                if (IsTileASymbol(tile))
                {
                    _lastOperation = tile.Letter;
                    SelectTile(tile);
                }
                else
                {
                    int nextResult = 0;
                    switch (_lastOperation)
                    {
                        case Letter.L_plus:
                            nextResult = _currentResult + (int)tile.Letter;
                            break;
                        case Letter.L_minus:
                            nextResult = _currentResult - (int)tile.Letter;
                            break;
                        default:
                            break;
                    }

                    if (nextResult >= 0 && nextResult <= 9)
                    {
                        _currentResult = nextResult;
                        SelectTile(tile);
                    }
                }
            }
        }

        private void OnFirstTileSelected(TileView tileView)
        {
            if (IsTileANumber(tileView))
            {
                SelectTile(tileView);
                _currentResult = (int)tileView.Letter;
            }
        }

        private void RefilBoard()
        {
            for (int row = 0; row < _settings.Width; row++)
            {
                for (int col = 0; col < _settings.Height; col++)
                {
                    if (_tiles[row, col] == null)
                    {
                        AddNewTile(row, col);
                    }
                }
            }
        }

        private void CleanupColumns()
        {
            int nullCount = 0;
            for (int row = 0; row < _settings.Width; row++)
            {
                for (int col = 0; col < _settings.Height; col++)
                {
                    if(_tiles[row, col] == null)
                    {
                        nullCount += 1;
                    }
                    else if(nullCount > 0)
                    {
                        _tiles[row, col].transform.Translate(new Vector3(0, -nullCount));
                        _tiles[row, col].Col = col - nullCount;
                        _tiles[row, col - nullCount] = _tiles[row, col];
                        _tiles[row, col] = null;
                    }
                }
                nullCount = 0;
            }
        }

        private bool IsCorrectOrder(TileView tile)
        {
            var lastTile = _objectList[_objectList.Count - 1];
            return (IsTileANumber(lastTile) && IsTileASymbol(tile)) ||
                (IsTileANumber(tile) && IsTileASymbol(lastTile));
        }

        private bool IsNeighbouringWithLast(TileView tile)
        {
            return AreTilesNeighbouring(tile, _objectList[_objectList.Count - 1]);
        }

        private bool AreTilesNeighbouring(TileView tile, TileView otherTile)
        {
            return (Mathf.Abs(tile.Row - otherTile.Row) == 1 && tile.Col == otherTile.Col)
                || (Mathf.Abs(tile.Col - otherTile.Col) == 1 && tile.Row == otherTile.Row);
        }

        private bool IsTileANumber(TileView tileView)
        {
            return tileView.Letter <= Letter.L_9;
        }

        private bool IsTileASymbol(TileView tileView)
        {
            return tileView.Letter > Letter.L_9;
        }

        private void SelectTile(TileView tileView)
        {
            tileView.IsSelected = true;
            tileView.Foreground.color = _settings.SelectedTileColor;
            _objectList.Add(tileView);
        }

        private void DeselectTile(TileView tileView)
        {
            tileView.IsSelected = false;
            tileView.Foreground.color = _settings.DefaultTileColor;
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
}