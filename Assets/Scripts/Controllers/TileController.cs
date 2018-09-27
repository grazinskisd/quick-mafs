using Zenject;
using UnityEngine;
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
        private bool _isNewLetterSet = false;
        private Tween _scaleTween;

        public event TileControllerEventHandler Selected;
        public Letter Letter { get { return _params.Letter; } }
        public bool IsSelected { get; private set; }
        public int Row { get { return _params.Row; } }
        public int Col {
            get { return _params.Col; }
            set
            {
                var currentPos = _tileView.transform.localPosition;
                _tileView.transform.DOLocalMoveY(value, _settings.MoveTime);
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
            SetLocalScale(_settings.ScaleOnCreation);
            TweenScale(_settings.DefaultScale, _settings.ScaleDuration);
            _isNewLetterSet = false;
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
            if (!_isNewLetterSet)
            {
                TweenScale(_settings.DefaultScale, _settings.ScaleDuration);
            }
            _isNewLetterSet = false;
        }

        private void TweenScale(float targetScale, float duration)
        {
            _scaleTween.Kill();
            _scaleTween = _tileView.transform.DOScale(targetScale, duration);
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
            _isNewLetterSet = true;
            _tileView.Text.sprite = _font.GetSpriteForLetter(newLetter);
            SetLocalScale(_settings.ExcitedScale);
            TweenScale(_settings.DefaultScale, _settings.ScaleDuration);
        }

        private void SetLocalScale(float scale)
        {
            _tileView.transform.localScale = Vector3.one * scale;
        }

        public void Destroy()
        {
            GameObject.Destroy(_tileView.gameObject);
        }

        private void SelectTile()
        {
            IsSelected = true;
            SetSelectedColor();
            TweenScale(_settings.SelectedScale, _settings.ScaleDuration);
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

            [Header("Tweening properties")]
            [Tooltip("Has to be greater than 0. Otherwise causes the BoxCollider to not be visible after scaling up.")]
            [Range(0.1f, 1)]
            public float ScaleOnCreation;
            public float DefaultScale;
            public float SelectedScale;
            public float ExcitedScale;
            public float ScaleDuration;

            public float MoveTime;
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
