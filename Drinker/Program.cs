using BotFW_CvSharp_01;

namespace Drinker
{
    public static class Program
    {
        public static Form1 DrinkerForm = new Form1();
        [STAThread]
        static void Main()
        {
            Log.Write("\n============================\nApp start\n============================");
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            try
            {
                Application.Run(DrinkerForm);
            }
            catch (Exception e)
            {
                Log.Write(e.Message, Log.LogType.Error);
                throw;
            }

        }
    }
}