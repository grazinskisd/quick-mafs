using System;
using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public delegate void GameMenuEventHandler();

    public class GameMenuController : IInitializable
    {
        [Inject] private GameMenuView _view;

        public event GameMenuEventHandler RestartPressed;
        public event GameMenuEventHandler ExitPressed;
        public event GameMenuEventHandler MutePressed;

        private bool _isVolumeMuted = false;

        public void Initialize()
        {
            _view.RestartButton.onClick.AddListener(IssueRestartPressed);
            _view.ExitButton.onClick.AddListener(IssueExitPressed);
            _view.MuteButton.onClick.AddListener(IssueMutePressed);
        }

        public void SetRestartButtonInteractable(bool isInteractable)
        {
            _view.RestartButton.interactable = isInteractable;
        }

        private void IssueMutePressed()
        {
            _isVolumeMuted = !_isVolumeMuted;
            _view.MuteButtonImage.sprite = GetMuteSprite();
            IssueEvent(MutePressed);
        }

        private Sprite GetMuteSprite()
        {
            return _isVolumeMuted ? _view.NoVolumeSprite : _view.HighVolumeSprite;
        }

        private void IssueExitPressed()
        {
            IssueEvent(ExitPressed);
        }

        private void IssueRestartPressed()
        {
            IssueEvent(RestartPressed);
        }

        private void IssueEvent(GameMenuEventHandler eventToIssue)
        {
            if(eventToIssue != null)
            {
                eventToIssue();
            }
        }
    }
}
