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
        [Inject] private TickManager _tick;

        private BoardModel _model;
        private TileView[,] _tiles;
        private TileModel[,] _tileModels;

        private const int LEFT_MOUSE_BUTTON = 0;
        private Camera _camera;

        private State _state = State.Waiting;

        List<TileView> _objectList = new List<TileView>();

        private int _currentResult;

        [Inject]
        public void Initialize()
        {
            SetupModel();
            SetupView();
            _camera = Camera.main;
            _tick.OnTick += Tick;
        }

        #region Initialize
        private void SetupView()
        {
            _boardView = GameObject.Instantiate(_boardView);
            _tiles = new TileView[_settings.Width, _settings.Height];
            _tileModels = new TileModel[_settings.Width, _settings.Height];
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
                    var tileModel = new TileModel
                    {
                        Letter = FontSettings.GetRandomLetter()
                    };
                    var tile = GameObject.Instantiate(_viewPrototype, _boardView.transform, false);
                    tile.transform.localPosition = new Vector2(row, col);
                    tile.Text.sprite = _font.GetSpriteForLetter(tileModel.Letter);
                    tile.name = string.Format("Tile ({0}, {1})", row, col);
                    tile.Foreground.color = _settings.DefaultTileColor;

                    _tiles[row, col] = tile;
                    _tileModels[row, col] = tileModel;
                }
            }
        }
        #endregion

        #region Input

        private void Tick()
        {
            if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
            {
                _state = State.MouseDown;
            }
            else if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON) && _state == State.Dragging)
            {
                _state = State.MouseUp;
            }

            ProcessState();
        }

        private void ProcessState()
        {
            switch (_state)
            {
                case State.MouseDown:
                    ProcessMouseDown();
                    break;
                case State.Dragging:
                    ProcessDragging();
                    break;
                case State.MouseUp:
                    ProcessMouseUp();
                    break;
                default:
                    break;
            }
        }

        private void ProcessMouseUp()
        {
            for (int i = 0; i < _objectList.Count; i++)
            {
                DeselectTile(_objectList[i]);
            }
            _objectList.Clear();
            _state = State.Waiting;
        }

        private void ProcessDragging()
        {
            var position = CastRayOnMousePosition();
            if (position != null &&
                !_objectList.Contains(_tiles[position.Row, position.Col]) &&
                IsNeighbouringWithLast(position) &&
                IsCorrectOrder(position))
            {
                SelectTile(position);
            }
        }

        private bool IsCorrectOrder(TilePosition position)
        {
            var lastTile = GetPositionFromView(_objectList[_objectList.Count - 1]);
            return (IsTileANumber(lastTile) && IsTileASymbol(position)) ||
                (IsTileANumber(position) && IsTileASymbol(lastTile));
        }

        private bool IsNeighbouringWithLast(TilePosition position)
        {
            var lastPosition = GetPositionFromView(_objectList[_objectList.Count - 1]);
            return ArePositionsNeighbouring(position, lastPosition);
        }

        private bool ArePositionsNeighbouring(TilePosition position, TilePosition lastPosition)
        {
            return (Mathf.Abs(position.Row - lastPosition.Row) == 1 && position.Col == lastPosition.Col)
                || (Mathf.Abs(position.Col - lastPosition.Col) == 1 && position.Row == lastPosition.Row);
        }

        private void ProcessMouseDown()
        {
            var position = CastRayOnMousePosition();
            if (position != null && IsTileANumber(position))
            {
                SelectTile(position);
                _state = State.Dragging;
            }
            else
            {
                _state = State.Waiting;
            }
        }

        private bool IsTileANumber(TilePosition position)
        {
            return (int)_tileModels[position.Row, position.Col].Letter <= 9;
        }

        private bool IsTileASymbol(TilePosition position)
        {
            return (int)_tileModels[position.Row, position.Col].Letter > 9;
        }

        private void SelectTile(TilePosition position)
        {
            _tileModels[position.Row, position.Col].isSelected = true;
            _tiles[position.Row, position.Col].Foreground.color = _settings.SelectedTileColor;
            _objectList.Add(_tiles[position.Row, position.Col]);
        }

        private void DeselectTile(TileView tileView)
        {
            var position = GetPositionFromView(tileView);
            _tileModels[position.Row, position.Col].isSelected = false;
            tileView.Foreground.color = _settings.DefaultTileColor;
        }

        private TilePosition CastRayOnMousePosition()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit)
            {
                var component = hit.collider.gameObject.GetComponent<TileView>();
                return GetPositionFromView(component);
            }
            return null;
        }

        private TilePosition GetPositionFromView(TileView view)
        {
            for (int row = 0; row < _settings.Width; row++)
            {
                for (int col = 0; col < _settings.Height; col++)
                {
                    if (view == _tiles[row, col])
                    {
                        return new TilePosition { Row = row, Col = col };
                    }
                }
            }
            return null;
        }
        #endregion

        public class Factory: PlaceholderFactory<BoardController> { }

        [System.Serializable]
        public class Settings
        {
            public int Width;
            public int Height;
            public Color DefaultTileColor;
            public Color SelectedTileColor;
        }

        public enum State
        {
            MouseDown, Dragging, MouseUp, Waiting
        }

        public class TilePosition
        {
            public int Row;
            public int Col;
        }
    }

    public class BoardModel
    {
        public int Width;
        public int Height;
    }

    public class TileModel
    {
        public Letter Letter;
        public bool isSelected;
    }
}