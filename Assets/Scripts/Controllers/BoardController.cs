using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class BoardController
    {
        [Inject] private Settings _settings;
        [Inject] private BoardView _boardView;
        [Inject] private InputManager _input;
        [Inject] private BoardService _boardService;
        [Inject] private TileController.Factory _tileFactory;
        [Inject] private ScoreController _scoreController;

        private TileController[,] _tiles;
        private List<TileController> _selectedTiles = new List<TileController>();
        private int _currentResult;
        public int _multiplier = 1;
        private Letter _lastOperation;

        private bool _isLastANumber;

        [Inject]
        public void Initialize()
        {
            SetupView();
            _input.MouseUp += OnMouseUp;
        }

        public bool IsSelectionValid(TileController tile)
        {
            return IsValidFirstSelection(tile) || IsValidNextSelection(tile);
        }

        private bool IsValidNextSelection(TileController tile)
        {
            return _selectedTiles.Count > 0 &&
                            !_selectedTiles.Contains(tile) &&
                            _boardService.AreTilesNeighbouring(tile, _selectedTiles[_selectedTiles.Count - 1]) &&
                            _boardService.IsCorrectOrder(tile, _selectedTiles[_selectedTiles.Count - 1]) &&
                            IsNextResultInBounds(tile);
        }

        private bool IsValidFirstSelection(TileController tile)
        {
            return _selectedTiles.Count == 0 && tile.IsTileANumber();
        }

        private void SetupView()
        {
            _boardView = GameObject.Instantiate(_boardView);
            _tiles = new TileController[_settings.Width, _settings.Height];
            FillBoard();
        }

        private void FillBoard()
        {
            for (int row = 0; row < _settings.Width; row++)
            {
                for (int col = 0; col < _settings.Height; col++)
                {
                    if (_tiles[row, col] == null)
                    {
                        _tiles[row, col] = _tileFactory.Create(
                            NewTileParams(row, col,
                            _isLastANumber ? FontSettings.RandomSymbol() : FontSettings.RandomNumber())
                        );
                        _tiles[row, col].Selected += OnTileSelected;
                        _isLastANumber = !_isLastANumber;
                    }
                }
            }
        }

        private TileParams NewTileParams(int row, int col, Letter letter)
        {
            return new TileParams
            {
                Row = row,
                Col = col,
                Parent = _boardView.transform,
                Letter = letter,
                Board = this
            };
        }

        /// <summary>
        /// [min]: inclusive
        /// [max]: exclusive
        /// </summary>
        /// <param name="min">Inclusive</param>
        /// <param name="max">Exclusive</param>
        private void DestroyTilesInRange(int min, int max)
        {
            for (int i = min; i < max; i++)
            {
                DestroyTile(i);
            }
        }

        private void OnMouseUp()
        {
            if (AreNoTilesSelected()) return;
            UpdateSelectedTiles();
            DeselectSelectedTiles();
            _selectedTiles.Clear();
            _lastOperation = Letter.L_0;
            _multiplier = 1;
            _boardService.CleanupColumns(_tiles);
            FillBoard();
        }

        private void UpdateSelectedTiles()
        {
            var lastTile = _selectedTiles[_selectedTiles.Count - 1];
            if (lastTile.IsTileANumber() && _selectedTiles.Count > 1)
            {
                _scoreController.IncrementScore(CalculateScore());
                if (_currentResult == 0)
                {
                    DestroyTilesInRange(0, _selectedTiles.Count);
                }
                else
                {
                    DestroyTilesInRange(0, _selectedTiles.Count - 1);
                    lastTile.SetNewLetter((Letter)_currentResult);
                }
            }
        }

        private int CalculateScore()
        {
            int result = 0;
            for (int i = 0; i < _selectedTiles.Count; i++)
            {
                if (_selectedTiles[i].IsTileANumber())
                {
                    result += (int)_selectedTiles[i].Letter;
                }
            }
            return result * _multiplier;
        }

        private void DeselectSelectedTiles()
        {
            for (int i = 0; i < _selectedTiles.Count; i++)
            {
                if (_selectedTiles[i] != null)
                {
                    _selectedTiles[i].DeselectTile();
                }
            }
        }

        private void DestroyTile(int i)
        {
            var tile = _selectedTiles[i];
            tile.Destroy();
            tile.Selected -= OnTileSelected;
            _selectedTiles[i] = null;
            _tiles[tile.Row, tile.Col] = null;
        }

        private void OnTileSelected(TileController tile)
        {
            if (tile.IsTileASymbol())
            {
                if (_selectedTiles.Count > 0)
                {
                    var lastTile = _selectedTiles[_selectedTiles.Count - 1];
                    if (_lastOperation == Letter.L_minus && tile.Letter == Letter.L_minus && lastTile.IsTileASymbol())
                    {
                        _lastOperation = Letter.L_plus;
                    }
                    else if (!(_lastOperation == Letter.L_minus && tile.Letter == Letter.L_plus && lastTile.IsTileASymbol()))
                    {
                        _lastOperation = tile.Letter;
                    }
                }
                else
                {
                    _lastOperation = tile.Letter;
                }
            }
            else
            {
                _currentResult = AreNoTilesSelected() ? (int)tile.Letter : NextResult(tile);
                if(_currentResult == 0)
                {
                    _multiplier++;
                }
            }
            _selectedTiles.Add(tile);
        }

        private bool AreNoTilesSelected()
        {
            return _selectedTiles.Count == 0;
        }

        private bool IsNextResultInBounds(TileController tile)
        {
            if (tile.IsTileASymbol()) return true;
            return _boardService.IsInNumberBounds(NextResult(tile));
        }

        private int NextResult(TileController tile)
        {
            return _currentResult + _boardService.GetIncrement(_lastOperation, tile);
        }

        public class Factory: PlaceholderFactory<BoardController> { }

        [System.Serializable]
        public class Settings
        {
            public int Width;
            public int Height;
        }
    }
}