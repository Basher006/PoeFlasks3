using BotFW_CvSharp_01;
using BotFW_CvSharp_01.GlobalStructs;
using BotFW_CvSharp_01.KeyboardMouse;
using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic
{
    public static class BotKeyHook 
    {
        private static readonly Keys StartStopHotKey = Keys.F4;

        private static List<Keys> KeysToHook;
        private static Dictionary<PauseWhenSecondKeyNotUsedRecently, bool> FlasksKeyIsUsedRecently;
        private static Dictionary<PauseWhenSecondKeyNotUsedRecently, MyTimer> FlasksKeysTimers;

        private static KeyboardHook? Hook;

        private static bool Run = false;

        public static void Init()
        {
            Run = true;
            Hook = new();

            KeysToHook = new();
            FlasksKeyIsUsedRecently = new();
            FlasksKeysTimers = new();

            Hook.AddHook(StartStopHotKey, StartStopChange, suppress: true);

            //Hook.HookEnable();
            ActivatePauseAcyncLoop();
        }

        public static void UpdatePauseWhenSecondKeyNotUsedRecently(Profile profile)
        {
            UnHookOld();

            // Add global pause
            if (profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently.Enable)
            {
                AddPauseWhenSecondKeyNotUsedRecently(profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently);
            }
            // add each flask
            foreach (var flask in profile.Setup.Flasks)
            {
                if (flask.Value.BaseAction.PauseWhenSecondKeyNotUsedRecently.Enable)
                {
                    AddPauseWhenSecondKeyNotUsedRecently(flask.Value.BaseAction.PauseWhenSecondKeyNotUsedRecently);
                }
            }
            // Add additional actions
            foreach (var addAct in profile.Setup.AdditionalActions)
            {
                if (addAct.PauseWhenSecondKeyNotUsedRecently.Enable)
                {
                    AddPauseWhenSecondKeyNotUsedRecently(addAct.PauseWhenSecondKeyNotUsedRecently);
                }
            }

            foreach (var key in KeysToHook)
            {
                Hook?.AddHook(key, DeactivatePause);
            }
        }

        private static void UnHookOld()
        {
            foreach (var key in KeysToHook)
            {
                Hook?.UnHook(key);
            }

            KeysToHook.Clear();
            FlasksKeyIsUsedRecently.Clear();
            FlasksKeysTimers.Clear();
        }

        private static async void ActivatePauseAcyncLoop()
        {
            await Task.Run(ActivatePause);
        }

        private static void ActivatePause()
        {
            while (Run)
            {
                try
                {
                    bool someItemChange = false;
                    var expandedKeys = new List<Keys>();
                    var keys = FlasksKeyIsUsedRecently.Keys.ToList();
                    for (int i = 0; i < FlasksKeyIsUsedRecently.Count; i++)
                    {
                        int delay_tr_ms = (int)(keys[i].PauseActivationDelay * 1000f);
                        if (FlasksKeyIsUsedRecently[keys[i]])
                        {
                            bool timerIsExpand = FlasksKeysTimers[keys[i]].Chek(delay_tr_ms);
                            if (timerIsExpand)
                            {
                                FlasksKeyIsUsedRecently[keys[i]] = false;
                                someItemChange = true;
                                if (!expandedKeys.Contains(keys[i].Key))
                                    expandedKeys.Add(keys[i].Key);
                            }
                        }
                    }

                    if (someItemChange)
                        Log.Write($"Activate pause for keys: {string.Join(", ", expandedKeys)}");
                }
                catch (Exception)
                {
                    Log.Write("Exeption when try activate pause (if it happend once after any profile update, that ok, dont worry!)", Log.LogType.Warn);
                    Thread.Sleep(10);
                    continue;
                }

                Thread.Sleep(250);
            }
        }

        private static void DeactivatePause(Keys k)
        {
            var itemsToUpdate = FlasksKeyIsUsedRecently.Keys.ToList().Where((x) => x.Key == k).ToList();
            bool someItemChange = false;
            for (int i = 0; i < itemsToUpdate.Count(); i++)
            {
                FlasksKeysTimers[itemsToUpdate[i]].Update();
                if (!FlasksKeyIsUsedRecently[itemsToUpdate[i]])
                {
                    FlasksKeyIsUsedRecently[itemsToUpdate[i]] = true;
                    someItemChange = true;
                }
            }

            if (someItemChange)
                Log.Write($"Deactivate pause for key: {k}");
        }

        private static void AddPauseWhenSecondKeyNotUsedRecently(PauseWhenSecondKeyNotUsedRecently flask)
        {
            FlasksKeyIsUsedRecently.Add(flask, false);
            FlasksKeysTimers.Add(flask, new());
            if (!KeysToHook.Contains(flask.Key))
                KeysToHook.Add(flask.Key);
        }

        private static void StartStopChange()
        {
            Log.Write("Start/Stop Hotkey detected!");
            Bot.OnStartStopChange();
        }

        public static void DispouseHook()
        {
            Run = false;
            Hook?.Dispose();
        }
    }
}
