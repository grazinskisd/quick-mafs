using UnityEngine;
using Zenject;

namespace QuickMafs
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "QuickMafs/GameSettingsInstaller")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public FontSettings FontSettings;
        public BoardController.Settings BoardSettings;
        public TileController.Settings TileSettings;

        public override void InstallBindings()
        {
            Container.BindInstance(FontSettings);
            Container.BindInstance(BoardSettings);
            Container.BindInstance(TileSettings);
        }
    }
}