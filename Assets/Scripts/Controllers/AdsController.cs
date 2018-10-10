using System;
using UnityEngine;
using UnityEngine.Advertisements;
using Zenject;

namespace QuickMafs
{
    public delegate void AdsEventHandler();

    public class AdsController : IInitializable, ITickable
    {
        [Inject] private GameMenuController _gameMenu;

        public event AdsEventHandler AdWatched;

        private bool _isAdAlreadyWatched = false;

        // TODO: change to real value
        private const string GAMEID = "2840080";
        private const string PLACEMENTID = "rewardedVideo";

        public void Initialize()
        {
            if (Advertisement.isSupported)
            {
                Advertisement.Initialize(GAMEID, true);
                _gameMenu.RestartPressed += OnRestartPressed;
            }
        }

        private void OnRestartPressed()
        {
            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleShowResult;

            Advertisement.Show(PLACEMENTID, options);
        }

        public void Tick()
        {
            if (!_isAdAlreadyWatched)
            {
                SetButtonInteractable(Advertisement.IsReady(PLACEMENTID));
            }
        }

        private void SetButtonInteractable(bool isInteractive)
        {
            _gameMenu.SetRestartButtonInteractable(isInteractive);
        }

        private void HandleShowResult(ShowResult result)
        {
            if (result == ShowResult.Finished)
            {
                Debug.Log("Video completed - Offer a reward to the player");
                IssueAdWatched();
                SetIsAdAlreadyWatched();
            }
            else if (result == ShowResult.Skipped)
            {
                Debug.LogWarning("Video was skipped - Do NOT reward the player");

            }
            else if (result == ShowResult.Failed)
            {
                Debug.LogError("Video failed to show");
            }
        }

        private void SetIsAdAlreadyWatched()
        {
            _isAdAlreadyWatched = true;
            SetButtonInteractable(false);
        }

        private void IssueAdWatched()
        {
            if (AdWatched != null)
            {
                AdWatched();
            }
        }
    }
}
