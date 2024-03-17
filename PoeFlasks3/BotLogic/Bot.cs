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

        public delegate void UpdateStartStopButton(BotState state, string? whyPause);
        public static UpdateStartStopButton updateStartStopButton;


        private static bool DEBUG;
        public static PoeClinet Client;

        private static bool Run = true;
        private static bool IsStart = false;
        private static bool PauseEnable = false;
        private static bool PlayerInPauseZone = false;

        private static Bitmap _screen;
        public static Bitmap Screen;
        private static bool ScreenIsBusy = false;
        private static bool ScreenIsFetched = false;
        private static bool ScreenIsReady = false;

        private static bool DataGrabIsDone = false;

        private static FlasksManager Manager;
        private static GrabedData? GrabedData;

        public static void Init(SelectedProfile selectedProfile, bool debug)
        {
            DEBUG = debug;
            Client = new PoeClinet(DEBUG);

            OnFlasksSetupChange(selectedProfile.Profile);
        }

        public static void RunLoop()
        {
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

            BotState state = BotState.None;
            BotState oldState = BotState.None;
            string? OldWhyPause = null;

            while (Run)
            {
                // ===========================
                // bot state things
                // ===========================
                state = GetState(out string? whyNotRun);
                if (!Client.Window.IsFinded)
                    await Task.Run(Client.Window.TryFindWindow);

                if (state != oldState)
                {
                    if (string.IsNullOrEmpty(whyNotRun))
                        Log.Write($"Bot state change to: {state}");
                    else
                        Log.Write($"Bot state change to: {state}, with reason: {whyNotRun}");
                }
                if (state != oldState || whyNotRun != OldWhyPause)
                    updateStartStopButton?.Invoke(state, whyNotRun);

                oldState = state;
                OldWhyPause = whyNotRun;

                // ===========================
                // data grab things
                // ===========================
                if (PauseEnable)
                    PoeLogReader.Chek();

                if (state == BotState.Run || state == BotState.Pause)
                {
                    await Task.Run(DataGrabLoop);
                    await Task.Run(ScreenLoop);

                    while (!ScreenIsReady && !DataGrabIsDone)
                    {
                        if (!Run)
                            break;
                        await Task.Delay(10);
                    }
                    if (!Run)
                        break;
                }

                // ===========================
                // do actions things
                // ===========================
                if (state == BotState.Run)
                    DoActions(); // its already async!

                // ===========================
                // loop managment thigs
                // ===========================
                timer.Stop();
                var loopTime = timer.ElapsedMilliseconds;
                if (loopTime > 0)
                    loopTime = 1000 / loopTime;
                updateGUI?.Invoke(GrabedData, loopTime);
                GC.Collect();
                timer.Restart();

                if (state != BotState.Run)
                    await Task.Delay(250);
                // ===========================
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
                    if (Client.Resolution != null)
                    {
                        using var screens = ScreenSliser.Slise(screenM, Client.Resolution.Value);
                        var grbedData = NumbersFinder.GrabData(screens, BotResourseLoader.Numbers, Client.Resolution.Value);
                        GrabedData = grbedData;
                    }
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
            var state = GetState(out string? whyPause);
            updateStartStopButton?.Invoke(state, whyPause);

            if (IsStart)
                Client = new PoeClinet(DEBUG);
        }

        public static void OnPauseChange(bool pause, string? poeLogPath )
        {
            PauseEnable = pause;
            PoeLogReader.OnZoneWasChanged = null;
            PoeLogReader.OnZoneWasChanged += OnZoneChange;
            PoeLogReader.Chek(poeLogPath);
        }
        private static void OnZoneChange()
        {
            PlayerInPauseZone = PoeLogReader.characterIsInPauseZone;
        }
        public static void OnFlasksSetupChange(Profile setup)
        {
            Manager = new(setup, DEBUG);
        }


        private static BotState GetState(out string? whyNotRun)
        {
            whyNotRun = null;

            if (IsStart)
            {
                // Window
                if (!Client.Window.IsFinded)
                {
                    IsStart = false;
                    Log.Write($"Start/Stop change to: {IsStart}");
                    Log.Write("Cannot Find Game window! Stoped bot.", Log.LogType.Error);
                    whyNotRun = "Cannot Find Game window! Stoped bot.";
                    return BotState.Stop;
                }
                else if (!Client.ScreenResolutionIsAccept)
                {
                    IsStart = false;
                    Log.Write($"Start/Stop change to: {IsStart}");
                    Log.Write($"Game Resolution not accept({Client.Window.Resolution})! Accepdet resolutions: {string.Join(", ", PoeClinet.ACCEPT_SCREEN_RES)}", Log.LogType.Error);
                    whyNotRun = "Game Resolution not accept!";
                    return BotState.Stop;
                }
                else if (!Client.Window.IsActive)
                {
                    whyNotRun = "Game window not active, Pause.. ";
                    return BotState.Pause;
                }

                // Data
                if (GrabedData == null || !GrabedData.Value.FindedFlags.Any_isFind)
                {
                    whyNotRun = "Pause.. Cannot find any data ";
                    return BotState.Pause;
                }

                if (PauseEnable && PlayerInPauseZone)
                {
                    whyNotRun = "Pause in HO ";
                    return BotState.Pause;
                }
                else
                    return BotState.Run;
            }

            else if (!IsStart)
                return BotState.Stop;

            Log.Write("Cannot correct define State", Log.LogType.Error);
            return BotState.None;
        }

        public enum BotState
        {
            None, // on app start
            Run,  // work
            Stop, // not work
            Pause, // pause/slow mode
        }
    }
}
