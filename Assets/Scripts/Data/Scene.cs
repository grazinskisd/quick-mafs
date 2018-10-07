namespace QuickMafs
{
    public class Scene
    {
        public static Scene MENU = new Scene("MenuScene");
        public static Scene GAME = new Scene("GameScene");

        public string SceneName { get; private set; }
        public Scene(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}
