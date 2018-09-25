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

        private const int LEFT_MOUSE_BUTTON = 0;
        private Camera _camera;

        private State _state = State.Waiting;

        List<TileView> _objectList = new List<TileView>();

        private int _currentResult;
        private Letter _lastOperation;

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
                    var tile = GameObject.Instantiate(_viewPrototype, _boardView.transform, false);
                    tile.Letter = FontSettings.GetRandomLetter();
                    tile.transform.localPosition = new Vector2(row, col);
                    tile.Text.sprite = _font.GetSpriteForLetter(tile.Letter);
                    tile.name = string.Format("Tile ({0}, {1})", row, col);
                    tile.Row = row; tile.Col = col;
                    tile.Foreground.color = _settings.DefaultTileColor;

                    _tiles[row, col] = tile;
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
            var tile = CastRayOnMousePosition();
            if (tile != null &&
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

                    if(nextResult >= 0 && nextResult <= 9)
                    {
                        _currentResult = nextResult;
                        SelectTile(tile);
                    }

                    Debug.Log("CURRENT RESULT: " + _currentResult);
                }
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

        private void ProcessMouseDown()
        {
            var tileView = CastRayOnMousePosition();
            if (tileView && IsTileANumber(tileView))
            {
                SelectTile(tileView);
                _currentResult = (int)tileView.Letter;
                _state = State.Dragging;
            }
            else
            {
                _state = State.Waiting;
            }
        }

        private bool IsTileANumber(TileView tileView)
        {
            return (int)tileView.Letter <= 9;
        }

        private bool IsTileASymbol(TileView tileView)
        {
            return (int)tileView.Letter > 9;
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

        private TileView CastRayOnMousePosition()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            return hit ? hit.collider.gameObject.GetComponent<TileView>() : null;
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
    }

    public class BoardModel
    {
        public int Width;
        public int Height;
    }
}