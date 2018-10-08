using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class BoardController: IBoardController
    {
        [Inject] private Settings _settings;
        [Inject] private BoardView _boardView;
        [Inject] private InputManager _input;
        [Inject] private BoardService _boardService;
        [Inject] private TileController.Factory _tileFactory;
        [Inject] private ScoreController _scoreController;
        [Inject] private AdsController _adsController;

        [Inject] private ScoreEffectController.Factory _scoreEffectFacotory;

        private ScoreEffectController[,] _burstEffects;

        public event BoardEventHandler TileSelected;
        public event BoardEventHandler MatchMade;

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
            _adsController.AdWatched += ResetBoard;
        }

        private void ResetBoard()
        {
            _selectedTiles.Clear();
            _lastOperation = Letter.L_0;
            _multiplier = 1;
            for (int row = 0; row < _tiles.GetLength(0); row++)
            {
                for (int col = 0; col < _tiles.GetLength(1); col++)
                {
                    _tiles[row, col].Selected -= OnTileSelected;
                    _tiles[row, col].Dispose();
                    _tiles[row, col] = null;
                }
            }
            FillBoard();
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
            _burstEffects = new ScoreEffectController[_settings.Width, _settings.Height];
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
                            _isLastANumber ? LetterUtil.RandomSymbol() : LetterUtil.RandomNumber())
                        );
                        _tiles[row, col].Selected += OnTileSelected;
                        _isLastANumber = !_isLastANumber;
                    }

                    if (_burstEffects[row, col] == null)
                    {
                        _burstEffects[row, col] = _scoreEffectFacotory.Create(new ScoreEffectParameters
                        {
                            Row = row,
                            Col = col,
                            Parent = _boardView.transform
                        });
                        _burstEffects[row, col].ParticlesKilled += OnParticlesKilled;
                    }

                }
            }
            _isLastANumber = !_isLastANumber;
        }

        private void OnParticlesKilled(int particleCount)
        {
            _scoreController.IncrementScore(particleCount);
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
                PlayBurstEffect();
                if (_currentResult == 0)
                {
                    DestroyTilesInRange(0, _selectedTiles.Count);
                }
                else
                {
                    DestroyTilesInRange(0, _selectedTiles.Count - 1);
                    lastTile.SetNewLetter((Letter)_currentResult);
                }
                IssueEvent(MatchMade);
            }
        }

        private void PlayBurstEffect()
        {
            for (int i = 0; i < _selectedTiles.Count; i++)
            {
                var tile = _selectedTiles[i];
                if (tile.IsTileANumber())
                {
                    _burstEffects[tile.Row, tile.Col].Emit((int)tile.Letter * _multiplier);
                }
            }
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
            tile.Dispose();
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
            IssueEvent(TileSelected);
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

        public void IssueEvent(BoardEventHandler eventToIssue)
        {
            if(eventToIssue != null)
            {
                eventToIssue();
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
}