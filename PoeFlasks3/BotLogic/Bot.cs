using BotFW_CvSharp_01;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using PoeFlasks3.BotLogic.Flasks;
using PoeFlasks3.GameClinet;
using PoeFlasks3.SettingsThings;
using System.Diagnostics;
using System.Drawing;

namespace PoeFlasks3.BotLogic
{
    public static class Bot
    {
        public delegate void UpdateGUI(GrabedData? data, long ups);
        public static UpdateGUI updateGUI;


        private static bool DEBUG;
        private static State state = State.None;
        private static PoeClinet Client;

        private static bool IsStart = false;
        private static bool IsPause = false;

        private static Bitmap _screen;
        public static Bitmap Screen;
        private static bool ScreenIsBusy = false;
        private static bool ScreenIsFetched = false;
        private static bool ScreenIsReady = false;

        private static bool DataGrabIsDone = false;

        private static Profile FlasksProfile;
        private static FlasksManager Manager;
        private static GrabedData? GrabedData;

        public static void Init(PoeClinet client, SelectedProfile selectedProfile, bool debug)
        {
            Client = client;
            DEBUG = debug;

            OnFlasksSetupChange(selectedProfile.Profile);
        }

        public static void Run()
        {
            // ==init things==
            state = State.Start;
            Screen = new Bitmap(1920, 1080, System.Drawing.Imaging.PixelFormat.Format24bppRgb);



            //FlaskProfiles.Save();
            //AppConfig.Save();

            //Console.WriteLine(  );


            // ==chek things==
            // chek flask setup profile (?)


            // if start/stop == start => ok
            // else => sleep

            if (Client.ScreenResolutionIsAccept)
            {
                // ok
            }
            else
            {
                // rise msg box with screen res error
            }

            if (Client.Window.IsActive)
            {
                // ok
            }
            else
            {
                // slow mode && pause
            }

            TestLoop();
        }

        async private static void TestLoop()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while (true)
            {
                await Task.Run(TestDataGrab);
                await Task.Run(TestScreenLoop);


                while(!ScreenIsReady && !DataGrabIsDone)
                {
                    await Task.Delay(10);
                }

                DoActions(Manager);


                timer.Stop();
                var loopTime = timer.ElapsedMilliseconds;
                if (loopTime > 0)
                    loopTime = 1000 / loopTime;
                updateGUI?.Invoke(GrabedData, loopTime);
                GC.Collect();
                timer.Restart();
            }
        }

        private static void TestScreenLoop()
        {
            ScreenIsReady = false;
            {
                _screen = new Bitmap(Client.Window.WindowRect.w, Client.Window.WindowRect.h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                BotFW.GetScreen(_screen, Client.Window.WindowRect, Client.Window.WinState);

                // if old creen fetched, wait
                while (ScreenIsFetched)
                {
                    Thread.Sleep(1);
                }

                // update screen
                ScreenIsBusy = true;
                {
                    UpdateScreen();
                }
                ScreenIsBusy = false;
            }
            ScreenIsReady = true;
        }

        private static void UpdateScreen()
        {
            Screen = (Bitmap)_screen.Clone();
        }

        private static void TestDataGrab()
        {
            DataGrabIsDone = false;
            {
                // if screen updated right now, wait
                while (ScreenIsBusy)
                {
                    Thread.Sleep(1);
                }

                ScreenIsFetched = true;
                {
                    using Mat screenM = Screen.ToMat();
                    //using Mat screenM = new("imgs\\testScreens\\1080_fs.png");
                    using var screens = ScreenSliser.Slise(screenM, AcceptPoeResolutions.x_1050);
                    var grbedData = NumbersFinder.GrabData(screens, BotResourseLoader.Numbers, AcceptPoeResolutions.x_1050);
                    GrabedData = grbedData;
                }
                ScreenIsFetched = false;
            }
            DataGrabIsDone = true;
        }

        private static void DoActions(FlasksManager manager)
        {
            manager.UseFlasks(GrabedData);
        }
        public static void OnStartStopChange(bool startStop)
        {
            IsStart = startStop;
        }

        public static void OnPauseChange(bool pause)
        {
            IsPause = pause;
        }

        public static void OnFlasksSetupChange(Profile setup)
        {
            Manager = new(setup, DEBUG);
            FlasksProfile = setup;
        }


        private enum State
        {
            None,
            Start,
            Chek,
            DataGrab,

            ScreenResError,
        }
    }
}
