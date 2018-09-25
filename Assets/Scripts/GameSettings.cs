using UnityEngine;

namespace QuickMafs
{
    [CreateAssetMenu(menuName = "QuickMafs/GameSettings")]
    public class GameSettings: ScriptableObject
    {
        public int BoardWidth;
        public int BoardHeight;
    }
}
