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

        private static bool Run = true;
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
            //BotKeyHook.OnStartStopChange += OnStartStopChange;

        }

        public static void RunLoop()
        {
            state = State.Run;
            Screen = new Bitmap(1920, 1080, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            _runLoop();
        }

        public static void StopLoop()
        {
            Run = false;
        }

        async private static void _runLoop()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while (Run)
            {
                await Task.Run(DataGrabLoop);
                await Task.Run(ScreenLoop);

                // if slow mode
                // task delay

                while (!ScreenIsReady && !DataGrabIsDone)
                {
                    if (!Run)
                        break;
                    await Task.Delay(10);
                }
                if (!Run)
                    break;

                // if state == Start
                DoActions(); // its already async!


                timer.Stop();
                var loopTime = timer.ElapsedMilliseconds;
                if (loopTime > 0)
                    loopTime = 1000 / loopTime;
                updateGUI?.Invoke(GrabedData, loopTime);
                GC.Collect();
                timer.Restart();
            }
        }

        private static void ScreenLoop()
        {
            ScreenIsReady = false;
            {
                _screen = new Bitmap(Client.Window.WindowRect.w, Client.Window.WindowRect.h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                BotFW.GetScreen(_screen, Client.Window.WindowRect, Client.Window.WinState);

                // if old creen fetched, wait
                while (ScreenIsFetched)
                {
                    if (!Run)
                        break;
                    Thread.Sleep(1);
                }
                if (!Run)
                    return;

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

        private static void DataGrabLoop()
        {
            DataGrabIsDone = false;
            {
                // if screen updated right now, wait
                while (ScreenIsBusy)
                {
                    if (!Run)
                        break;
                    Thread.Sleep(1);
                }
                if (!Run)
                    return;

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

        private static void DoActions()
        {
            Manager.UseFlasks(GrabedData);
        }
        public static void OnStartStopChange()
        {
            IsStart = !IsStart;
            Log.Write($"Start/Stop change to: {IsStart}");
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

        private static State GetState()
        {
            // chek things
            return State.None;
        }

        private enum State
        {
            None, // on app start
            Run,  // work
            Stop, // not work
            Pause, // pause
        }
    }
}
