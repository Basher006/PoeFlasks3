using BotFW_CvSharp_01;
using BotFW_CvSharp_01.GlobalStructs;
using BotFW_CvSharp_01.KeyboardMouse;
using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic
{
    public static class BotKeyHook 
    {
        private static readonly Keys StartStopHotKey = Keys.F4;

        private static Dictionary<Keys, MyTimer> KeysUsedRecently = new ();

        private static KeyboardHook? Hook;

        public static void Init()
        {
            Hook = new();
            KeysUsedRecently = new();
            Hook.AddHook(StartStopHotKey, StartStopChange, suppress: true);

            //Hook.HookEnable();

#if !DEBUG
            Hook.HookEnable();
#endif
        }

        public static void UpdatePauseWhenSecondKeyNotUsedRecently(Profile profile)
        {
            UnHookOld();

            // Add global pause
            if (profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently.Enable)
                AddPauseWhenSecondKeyNotUsedRecently(profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently);

            // add each flask
            foreach (var flask in profile.Setup.Flasks)
            {
                if (flask.Value.BaseAction.PauseWhenSecondKeyNotUsedRecently.Enable)
                    AddPauseWhenSecondKeyNotUsedRecently(flask.Value.BaseAction.PauseWhenSecondKeyNotUsedRecently);
            }

            // Add additional actions
            foreach (var addAct in profile.Setup.AdditionalActions)
            {
                if (addAct.PauseWhenSecondKeyNotUsedRecently.Enable)
                    AddPauseWhenSecondKeyNotUsedRecently(addAct.PauseWhenSecondKeyNotUsedRecently);
            }
        }

        public static bool PauseIsEnable(BaseActionSettings baseAction)
        {
            var globapPause = Program.Settings.SelectedProfile.Profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently;
            if (globapPause.Enable)
            {
                var globalPauseIsEnable = _pauseIsEnable(globapPause);
                var baseActionPauseIsEnable = _pauseIsEnable(baseAction.PauseWhenSecondKeyNotUsedRecently);
                return globalPauseIsEnable || baseActionPauseIsEnable;
            }
            else
                return _pauseIsEnable(baseAction.PauseWhenSecondKeyNotUsedRecently);
        }

        private static bool _pauseIsEnable(PauseWhenSecondKeyNotUsedRecently pause)
        {
            if (pause.Enable && KeysUsedRecently.ContainsKey(pause.Key))
                return KeysUsedRecently[pause.Key].Chek((int)(pause.PauseActivationDelay * 1000f));

            return false;
        }

        private static void UnHookOld()
        {
            foreach (var key in KeysUsedRecently)
            {
                Hook?.UnHook(key.Key);
            }

            KeysUsedRecently.Clear();
        }

        private static void AddPauseWhenSecondKeyNotUsedRecently(PauseWhenSecondKeyNotUsedRecently flask)
        {
            if (!KeysUsedRecently.ContainsKey(flask.Key))
            {
                KeysUsedRecently.Add(flask.Key, new());

                //Hook?.AddHook(flask.Key, UpdateKeyTimer);
                Hook?.AddHook(flask.Key, UpdateKeyTimer, method: KeyboardHook.HookOnKey.Pressed); // Experemental!
            }
        }

        private static void UpdateKeyTimer(Keys k)
        {
            if (KeysUsedRecently.ContainsKey(k))
            {
                KeysUsedRecently[k].Update();
            }
        }

        private static void StartStopChange()
        {
            Log.Write("Start/Stop Hotkey detected!");
            Bot.OnStartStopChange();
        }

        public static void DispouseHook()
        {
            Hook?.Dispose();
        }
    }
}
