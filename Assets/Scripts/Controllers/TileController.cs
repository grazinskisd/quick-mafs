using Zenject;
using UnityEngine;
using System;
using DG.Tweening;

namespace QuickMafs
{
    public delegate void TileControllerEventHandler(TileController sender);

    public class TileController
    {
        private const string NAME_FORMAT = "Tile ({0}, {1})";

        [Inject] private TileView _tileView;
        [Inject] private FontSettings _font;
        [Inject] private Settings _settings;

        private TileParams _params;

        public event TileControllerEventHandler Selected;
        public Letter Letter { get { return _params.Letter; } }
        public bool IsSelected { get; private set; }
        public int Row { get { return _params.Row; } }
        public int Col {
            get { return _params.Col; }
            set
            {
                var currentPos = _tileView.transform.localPosition;
                _tileView.transform.localPosition = new Vector2(currentPos.x, value);
                _params.Col = value;
            }
        }

        [Inject]
        private void Initialize(TileParams parameters)
        {
            _params = parameters;
            CreateTile();
        }

        private void CreateTile()
        {
            _tileView = GameObject.Instantiate(_tileView, _params.Parent, false);
            _tileView.transform.localPosition = new Vector2(_params.Row, _params.Col);
            _tileView.name = string.Format(NAME_FORMAT, _params.Row, _params.Col);
            _tileView.Selected += OnSelected;
            SetNewLetter(_params.Letter);
            SetDefaultColor();
            _tileView.transform.localScale = Vector3.one * 0.1f;
            _tileView.transform.DOScale(_settings.DefaultScale, _settings.ScaleDuration);
        }

        private void OnSelected()
        {
            if (_params.Board.IsSelectionValid(this))
            {
                SelectTile();
            }
        }

        public void DeselectTile()
        {
            IsSelected = false;
            SetDefaultColor();
            _tileView.transform.DOScale(_settings.DefaultScale, _settings.ScaleDuration);
        }

        public bool IsTileANumber()
        {
            return _params.Letter <= Letter.L_9;
        }

        public bool IsTileASymbol()
        {
            return _params.Letter > Letter.L_9;
        }

        public void SetNewLetter(Letter newLetter)
        {
            _params.Letter = newLetter;
            _tileView.Text.sprite = _font.GetSpriteForLetter(newLetter);
            _tileView.transform.localScale = Vector3.one * (_settings.DefaultScale + 0.4f);
            _tileView.transform.DOScale(_settings.DefaultScale, _settings.ScaleDuration);
        }

        public void Destroy()
        {
            GameObject.Destroy(_tileView.gameObject);
        }

        private void SelectTile()
        {
            IsSelected = true;
            SetSelectedColor();
            _tileView.transform.DOScale(_settings.SelectedScale, _settings.ScaleDuration);
            if(Selected != null)
            {
                Selected(this);
            }
        }

        private void SetSelectedColor()
        {
            _tileView.Foreground.color = _settings.SelectedTileColor;
        }

        private void SetDefaultColor()
        {
            _tileView.Foreground.color = _settings.DefaultTileColor;
        }

        public class Factory: PlaceholderFactory<TileParams, TileController> { }

        [System.Serializable]
        public class Settings
        {
            public Color DefaultTileColor;
            public Color SelectedTileColor;
            public float DefaultScale;
            public float SelectedScale;
            public float ScaleDuration;
        }
    }

    public class TileParams
    {
        public int Row, Col;
        public Transform Parent;
        public Letter Letter;
        public BoardController Board;
    }
}
