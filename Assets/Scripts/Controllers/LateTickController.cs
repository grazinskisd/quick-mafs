using Zenject;

namespace QuickMafs
{
    public delegate void LateTickEventHandler();

    public class LateTickController : ILateTickable
    {
        public event LateTickEventHandler OnLateTick;

        public void LateTick()
        {
            if(OnLateTick != null)
            {
                OnLateTick();
            }
        }
    }
}
