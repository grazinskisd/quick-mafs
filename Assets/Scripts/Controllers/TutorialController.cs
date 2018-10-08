using Zenject;

namespace QuickMafs
{
    public class TutorialController : IInitializable
    {
        [Inject] private TutorialView _view;

        public void Initialize()
        {

        }
    }
}
