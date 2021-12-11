using UI.Screens.Game;

namespace Managers
{
    public class GameManager : Manager<GameManager>
    {
        private void Start()
        {
            //Initialization order
            ScreensManager.instance.Init();
            PlayerManager.instance.Init();
            TargetsManager.instance.Init();
            SettingsManager.instance.Init();


            ScreensManager.instance.Open<GameScreen>();
        }
    }
}