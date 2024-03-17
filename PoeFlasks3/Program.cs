using PoeFlasks3.BotLogic;
using PoeFlasks3.SettingsThings;

namespace PoeFlasks3
{
    public static class Program
    {
        public static bool ConfigLoaded { get; private set; }
        public static Setup Settings;

        public static void Init(bool debug)
        {
            Settings = new Setup();
            BotResourseLoader.Load();

            Bot.Init(Settings.SelectedProfile, debug);
            Bot.RunLoop();
        }
    }
}