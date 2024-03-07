using PoeFlasks3.BotLogic;
using PoeFlasks3.GameClinet;
using PoeFlasks3.SettingsThings;

namespace PoeFlasks3
{
    public static class Program
    {
        //public delegate void OnChange(Profile setup);
        //public static OnChange OnFlasksSetupChange;


        public static PoeClinet poeClinet;
        //public static GetScreenLoopThread screenLoop;
        public static bool ConfigLoaded { get; private set; }

        public static Setup Settings;

        public static void Init(bool debug)
        {
            Settings = new Setup();
            BotResourseLoader.Load();

            poeClinet = new PoeClinet(debug);
            Bot.Init(poeClinet, Settings.SelectedProfile, debug);
            Bot.Run();
        }
    }
}