using BotFW_CvSharp_01;
using BotFW_CvSharp_01.GlobalStructs;
using BotFW_CvSharp_01.KeyboardMouse;
using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic.Flasks
{
    // (!) maybe swith hook to single tone? (!)
    public class FlaskKeyUsedRecentlyCheker : IDisposable
    {
        private readonly List<Keys> KeysToHook;
        private Dictionary<PauseWhenSecondKeyNotUsedRecently, bool> FlasksKeyIsUsedRecently;
        private Dictionary<PauseWhenSecondKeyNotUsedRecently, MyTimer> FlasksKeysTimers;
        private readonly KeyboardHook Hook;

        public FlaskKeyUsedRecentlyCheker(FlasksSetup flasksSetup)
        {
            Hook = new();
            FlasksKeyIsUsedRecently = new();
            FlasksKeysTimers = new();
            KeysToHook = new();

            // Add global pause
            if (flasksSetup.GlobalPauseWhenSecondKeyNotUsedRecently.Enable)
            {
                AddPauseWhenSecondKeyNotUsedRecently(flasksSetup.GlobalPauseWhenSecondKeyNotUsedRecently);
            }
            // add each flask
            foreach (var flask in flasksSetup.Flasks)
            {
                if (flask.Value.BaseAction.PauseWhenSecondKeyNotUsedRecently.Enable)
                {
                    AddPauseWhenSecondKeyNotUsedRecently(flask.Value.BaseAction.PauseWhenSecondKeyNotUsedRecently);
                }
            }
            // Add additional actions
            foreach (var addAct in flasksSetup.AdditionalActions)
            {
                if (addAct.PauseWhenSecondKeyNotUsedRecently.Enable)
                {
                    AddPauseWhenSecondKeyNotUsedRecently(addAct.PauseWhenSecondKeyNotUsedRecently);
                }
            }

            foreach (var key in KeysToHook)
            {
                Hook.AddHook(key, UpdateKey);
            }
        }


        public bool PauseIsEnable(BaseActionSettings baseAction)
        {
            if (FlasksKeyIsUsedRecently.ContainsKey(baseAction.PauseWhenSecondKeyNotUsedRecently))
                return FlasksKeyIsUsedRecently[baseAction.PauseWhenSecondKeyNotUsedRecently];

            return false;
        }

        private void AddPauseWhenSecondKeyNotUsedRecently(PauseWhenSecondKeyNotUsedRecently flask)
        {
            FlasksKeyIsUsedRecently.Add(flask, false);
            FlasksKeysTimers.Add(flask, new());
            if (!KeysToHook.Contains(flask.Key))
                KeysToHook.Add(flask.Key);
        }

        private void UpdateKey(Keys k)
        {
            foreach (var item in FlasksKeyIsUsedRecently)
            {
                if (item.Key.Key == k)
                    FlasksKeyIsUsedRecently[item.Key] = FlasksKeysTimers[item.Key].Chek((int)(item.Key.PauseActivationDelay * 1000f));
            }
        }

        public void Dispose()
        {
            Hook.Dispose();
        }
    }
}
