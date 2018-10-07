using Zenject;
using UnityEngine;
using DG.Tweening;

namespace QuickMafs
{
    public delegate void TileControllerEventHandler(TileController sender);

    public class TileController: IPoolable<TileParams, IMemoryPool>, System.IDisposable
    {
        private const string NAME_FORMAT = "Tile ({0}, {1})";

        [Inject] private TileView _tileViewProto;
        [Inject] private FontSettings _font;
        [Inject] private Settings _settings;

        private TileParams _params;
        private bool _isNewLetterSet = false;
        private Tween _scaleTween;
        private TileView _view;

        private IMemoryPool _pool;

        public event TileControllerEventHandler Selected;
        public Letter Letter { get { return _params.Letter; } }
        public bool IsSelected { get; private set; }
        public int Row { get { return _params.Row; } }
        public int Col {
            get { return _params.Col; }
            set
            {
                var currentPos = _view.transform.localPosition;
                _view.transform.DOLocalMoveY(value, _settings.MoveTime);
                _params.Col = value;
            }
        }

        public void OnSpawned(TileParams parameters, IMemoryPool pool)
        {
            if (_view == null)
            {
                _view = GameObject.Instantiate(_tileViewProto);
                _view.Selected += OnSelected;
            }
            _pool = pool;
            _params = parameters;
            InitializeTile();
            _view.gameObject.SetActive(true);
        }

        public void Dispose()
        {
            _pool.Despawn(this);
        }

        public void OnDespawned()
        {
            _view.gameObject.SetActive(false);
            _pool = null;
        }

        private void InitializeTile()
        {
            _view.transform.SetParent(_params.Parent, false);
            _view.transform.localPosition = new Vector2(_params.Row, _params.Col);
            _view.name = string.Format(NAME_FORMAT, _params.Row, _params.Col);
            _view.Foreground.transform.Rotate(Vector3.forward, Random.Range(0, 360));
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
            _scaleTween = _view.transform.DOScale(targetScale, duration);
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
            _view.Text.sprite = _font.GetSpriteForLetter(newLetter);
            SetLocalScale(_settings.ExcitedScale);
            TweenScale(_settings.DefaultScale, _settings.ExcitedScaleDuration);
        }

        private void SetLocalScale(float scale)
        {
            _view.transform.localScale = Vector3.one * scale;
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
            _view.Foreground.color = _settings.SelectedTileColor;
        }

        private void SetDefaultColor()
        {
            if (IsTileANumber())
            {
                SetTileColor(_settings.DefaultTileColor);
            }
            else
            {
                SetTileColor(_params.Letter == Letter.L_minus ?
                    _settings.MinusTileColor : _settings.PlusTileColor);
            }
        }

        private void SetTileColor(Color color)
        {
            _view.Foreground.color = color;
        }

        public class Factory: PlaceholderFactory<TileParams, TileController> { }


        [System.Serializable]
        public class Settings
        {
            public Color DefaultTileColor;
            public Color SelectedTileColor;
            public Color PlusTileColor;
            public Color MinusTileColor;

            [Header("Tweening properties")]
            [Tooltip("Has to be greater than 0. Otherwise causes the BoxCollider to not be visible after scaling up.")]
            [Range(0.1f, 1)]
            public float ScaleOnCreation;
            public float DefaultScale;
            public float SelectedScale;
            public float ExcitedScale;
            public float ScaleDuration;
            public float ExcitedScaleDuration;

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
