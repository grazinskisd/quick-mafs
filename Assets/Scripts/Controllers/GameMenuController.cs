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

        public void Initialize()
        {
            _view.RestartButton.onClick.AddListener(IssueRestartPressed);
            _view.ExitButton.onClick.AddListener(IssueExitPressed);
            _view.MuteButton.onClick.AddListener(IssueMutePressed);
        }

        private void IssueMutePressed()
        {
            IssueEvent(MutePressed);
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
