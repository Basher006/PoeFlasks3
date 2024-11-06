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
        private static Langueges AppLanguge;

        private static bool Run = true;
        private static bool IsStart = false;
        private static bool PauseEnable = false;
        private static bool PlayerInPauseZone = false;

        private static Bitmap? _screen;
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

            AppLanguge = Program.Settings.AppLanguege;
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
            string? OldWhyNotRun = null;

            while (Run)
            {
                // ===========================
                // bot state things
                // ===========================
                state = GetState(out string? whyNotRun);

                if (state != oldState || whyNotRun != OldWhyNotRun)
                {
                    updateStartStopButton?.Invoke(state, whyNotRun);

                    if (string.IsNullOrEmpty(whyNotRun))
                        Log.Write($"Bot state change to: {state}");
                    else
                        Log.Write($"Bot state change to: {state}, with reason: {whyNotRun}");
                }

                oldState = state;
                OldWhyNotRun = whyNotRun;

                if (!Client.Window.IsFinded)
                {
                    await Task.Run(Client.Window.TryFindWindow);
                    await Task.Delay(250);
                    UpdGUI(timer);
                    continue;
                }

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
                // loop management things
                // ===========================
                UpdGUI(timer);

                if (state != BotState.Run)
                    await Task.Delay(250);
                // ===========================
            }
        }

        private static void UpdGUI(Stopwatch timer)
        {
            if (timer == null || GrabedDataHolder == null || GrabedDataHolder.HasValue == false)
            {
                return;
            }
            else
            {
                timer.Stop();
                var loopTime = timer.ElapsedMilliseconds;
                if (loopTime > 0)
                    loopTime = 1000 / loopTime;
                updateGUI?.Invoke(GrabedDataHolder, loopTime);
                GC.Collect();
                timer.Restart();
            }    
        }

        private static void ScreenLoop()
        {
            ScreenIsReady = false;
            {
                if (_screen != null)
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
            if (_screen != null)
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
                    if (Client.Resolution != null && screenM.Width > 1 && screenM.Height > 1)
                    {
                        using var screens = ScreenSliser.Slise(screenM, Client.Resolution.Value);
                        GrabedData? grabedData = new();
                        try
                        {
                            grabedData = NumbersFinder.GrabData(screens, BotResourseLoader.Numbers, Client.Resolution.Value);
                        }
                        catch {
                            Log.Write("Error blyat!");
                        }

                        if (grabedData.HasValue == false)
                            grabedData = new();

                        if (grabedData.Value.FindedFlags.Any_isFind)
                            grabedData.Value.UpdateMaxNumbers();
                        GrabedDataHolder = grabedData.Value;
                        //GrabedDataHolder?.UpdateMaxNumbers();
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
            if (IsStart)
            {
                Client = new PoeClinet(DEBUG);
                if (Client.Window.IsFinded)
                {
                    Log.Write($"Poe clinet resolution: {Client.Window.Resolution}");
                    _screen = new Bitmap(Client.Window.WindowRect.w, Client.Window.WindowRect.h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    UpdateScreen();
                }
            }

            Log.Write($"Start/Stop change to: {IsStart}");
            var state = GetState(out string? whyPause);
            updateStartStopButton?.Invoke(state, whyPause);
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
                    whyNotRun = BotResourseLoader.LanguegesText[AppLanguge][9];
                    return BotState.Stop;
                }
                else if (!Client.ScreenResolutionIsAccept)
                {
                    IsStart = false;
                    Log.Write($"Start/Stop change to: {IsStart}");
                    Log.Write($"Game Resolution not accept({Client.Window.Resolution})! Accepdet resolutions: {string.Join(", ", PoeClinet.ACCEPT_SCREEN_RES)}", Log.LogType.Error);
                    whyNotRun = BotResourseLoader.LanguegesText[AppLanguge][10];
                    return BotState.Stop;
                }
                else if (!Client.Window.IsActive)
                {
                    whyNotRun = BotResourseLoader.LanguegesText[AppLanguge][11];
                    return BotState.Pause;
                }

                // Data
                if (GrabedData == null || !GrabedData.Value.FindedFlags.Any_isFind)
                {
                    whyNotRun = BotResourseLoader.LanguegesText[AppLanguge][12];
                    return BotState.Pause;
                }

                if (PauseEnable && PlayerInPauseZone)
                {
                    whyNotRun = BotResourseLoader.LanguegesText[AppLanguge][13];
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
