using Zenject;

namespace QuickMafs
{
    public delegate void TickEventHandler();
    public class TickManager : ITickable
    {
        public event TickEventHandler OnTick;

        public void Tick()
        {
            if(OnTick != null)
            {
                OnTick();
            }
        }
    }
}
