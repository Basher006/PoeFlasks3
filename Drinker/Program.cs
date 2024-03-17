using BotFW_CvSharp_01;
using PoeFlasks3.BotLogic;

namespace Drinker
{
    public static class Program
    {
        public static Form1 DrinkerForm = new Form1();
        [STAThread] static void Main()
        {
            Log.Write("\n============================\nApp start\n============================");

            try
            {
                ApplicationConfiguration.Initialize();
                Application.Run(DrinkerForm);
            }
            catch (Exception e)
            {
                Log.Write(e.ToString(), Log.LogType.Error);
                throw;
            }
            finally
            {
                BotKeyHook.DispouseHook();
                Log.Write("Application Close.");
            }
        }
    }
}