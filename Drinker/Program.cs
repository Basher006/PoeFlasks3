using BotFW_CvSharp_01;
using PoeFlasks3.BotLogic;

namespace Drinker
{
    public static class Program
    {
        public static Form1 DrinkerForm = new Form1();
        [STAThread]
        static void Main()
        {
            Log.Write("\n============================\nApp start\n============================");

            ApplicationConfiguration.Initialize();
            Application.ThreadException += new ThreadExceptionEventHandler(GlobalExeption);
            Application.ApplicationExit += OnAppExit;
            Application.Run(DrinkerForm);
        }

        private static void OnAppExit(object? sender, EventArgs e)
        {
            BotKeyHook.DispouseHook();
            Log.Write("Application Close.");
        }

        private static void GlobalExeption(object sender, ThreadExceptionEventArgs e)
        {
            Log.Write(e.Exception.ToString(), Log.LogType.Error);
            Application.Exit();
        }
    }
}