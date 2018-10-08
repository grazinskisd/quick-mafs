using UnityEngine;
using Zenject;

namespace QuickMafs
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "QuickMafs/GameSettingsInstaller")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public BoardController.Settings BoardSettings;
        public TileController.Settings TileSettings;
        public SoundController.Settings SoundSettings;
        public ScoreEffectController.Settings ScoreEffectSettings;

        public override void InstallBindings()
        {
            Container.BindInstance(BoardSettings);
            Container.BindInstance(TileSettings);
            Container.BindInstance(SoundSettings);
            Container.BindInstance(ScoreEffectSettings);
        }
    }
}