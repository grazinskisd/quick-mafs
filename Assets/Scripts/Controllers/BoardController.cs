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

        [Inject]
        public void Initialize()
        {
            SetupModel();
            SetupView();
            _camera = Camera.main;
            _tick.OnTick += Tick;
        }

        private void Tick()
        {
            if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
            {
                _state = State.MouseDown;
            }
            else if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON))
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
            var tileView = CastRayOnMousePosition();
            if (tileView && !_objectList.Contains(tileView) && IsNeighbouringWithLast(tileView))
            {
                SelectTile(tileView);
            }
        }

        private bool IsNeighbouringWithLast(TileView tileView)
        {
            var position = GetPositionFromView(tileView);
            var lastPosition = GetPositionFromView(_objectList[_objectList.Count - 1]);
            return ArePositionsNeighbouring(position, lastPosition);
        }

        private bool ArePositionsNeighbouring(TilePosition position, TilePosition lastPosition)
        {
            return Mathf.Abs(position.Row - lastPosition.Row) == 1 && position.Col == lastPosition.Col
                || Mathf.Abs(position.Col - lastPosition.Col) == 1 && position.Row == lastPosition.Row;
        }

        private void ProcessMouseDown()
        {
            var tileView = CastRayOnMousePosition();
            if (tileView)
            {
                SelectTile(tileView);
            }

            _state = State.Dragging;
        }

        private void SelectTile(TileView tileView)
        {
            var position = GetPositionFromView(tileView);
            _tileModels[position.Row, position.Col].isSelected = true;
            tileView.Foreground.color = _settings.SelectedTileColor;
            _objectList.Add(tileView);
        }

        private void DeselectTile(TileView tileView)
        {
            var position = GetPositionFromView(tileView);
            _tileModels[position.Row, position.Col].isSelected = false;
            tileView.Foreground.color = _settings.DefaultTileColor;
        }

        private TileView CastRayOnMousePosition()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            return hit ? hit.collider.gameObject.GetComponent<TileView>() : null;
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