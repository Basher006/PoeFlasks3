using BotFW_CvSharp_01;
using BotFW_CvSharp_01.GlobalStructs;
using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic.Flasks
{
    public class BaseAction : IUsable
    {
        private const float MINIMUM_STATE_FOR_USE_FLASK = 0.05f;
        private const int OneUseTimerTreshold = 3000;

        public BaseActionSettings Setup { get; private set; }
        private readonly MyTimer ActionTimer;
        private readonly MyTimer OneUseTimer;

        private readonly bool DEBUG;

        private FlaskSlot? Slot { get; }


        public BaseAction(BaseAction baseAct)
        {
            Setup = baseAct.Setup;

            ActionTimer = baseAct.ActionTimer;
            OneUseTimer = baseAct.OneUseTimer;

            Slot = baseAct.Slot;
        }

        public BaseAction(BaseActionSettings setup, bool debug = false)
        {
            DEBUG = debug;

            Setup = setup;
            ActionTimer = new();
            OneUseTimer = new();

            Slot = null;
        }

        public BaseAction(BaseActionSettings setup, FlaskSlot slot, bool debug = false)
        {
            DEBUG = debug;

            Setup = setup;
            ActionTimer = new();
            OneUseTimer = new();

            Slot = slot;
        }


        public async void UseAsync()
        {
            ActionTimer.Update();
            Log.Write($"Send key: {Setup.HotKey}");
            await Task.Run(() => BotFW.SendKey(Setup.HotKey, debug: DEBUG));
        }

        public bool Chek(GrabedData? grabedData)
        {
            bool condition = false;
            bool flaskIsActive = false;
            if (Slot != null)
                flaskIsActive = FlaskIsActive(grabedData.Value, Slot.Value);

            if (!grabedData.HasValue)
                return false;

            // None
            if (Setup.ActType == ActivationType.None)
                return false;
            // Chek minimum cooldown
            if (!ChekMinCD())
                return false;
            // chek pause on not use other skill recently
            if (BotKeyHook.PauseIsEnable(Setup))
                return false;

            // HP
            if (Setup.ActType == ActivationType.HP)
                condition = ChekHP(grabedData.Value);
            // MP
            if (Setup.ActType == ActivationType.MP)
                condition = ChekMP(grabedData.Value);
            // ES
            if (Setup.ActType == ActivationType.ES)
                condition = ChekES(grabedData.Value);
            // Chek ingame flask CD state
            if (Setup.ActType == ActivationType.CD)
            {
                if (Slot == null)
                    condition = ChekMinCD();
                else
                    return !flaskIsActive;
            }
            // oneTime
            if (Slot != null)
            {
                if (Setup.ActType == ActivationType.OneTime)
                    condition =  ChekOnetime(grabedData.Value, Slot.Value);
            }

            return !flaskIsActive && condition;
        }

        public bool ChekGroup(GrabedData? grabedData, List<FlaskSlot> slots)
        {
            bool condition = false;
            bool anyFlaskIsActive = false;
            if (slots?.Any() == true)
            {
                foreach (var slot in slots)
                {
                    if (FlaskIsActive(grabedData.Value, slot))
                    {
                        anyFlaskIsActive = true;
                        break;
                    }
                }
            }

            if (!grabedData.HasValue)
                return false;

            // None
            if (Setup.ActType == ActivationType.None)
                return false;
            // Chek minimum cooldown
            if (!ChekMinCD())
                return false;
            // chek pause on not use other skill recently
            if (BotKeyHook.PauseIsEnable(Setup))
                return false;

            // HP
            if (Setup.ActType == ActivationType.HP)
                condition = ChekHP(grabedData.Value);
            // MP
            if (Setup.ActType == ActivationType.MP)
                condition = ChekMP(grabedData.Value);
            // ES
            if (Setup.ActType == ActivationType.ES)
                condition = ChekES(grabedData.Value);
            // Chek ingame flask CD state
            if (Setup.ActType == ActivationType.CD)
            {
                condition = ChekMinCD();
            }
            // oneTime
            if (Setup.ActType == ActivationType.OneTime)
            {
                foreach (var slot in slots)
                {
                    if (ChekOnetime(grabedData.Value, slot))
                        return true;
                }
            }

            return !anyFlaskIsActive && condition;
        }

        public void UpdateKey(Keys k)
        {
            var s = Setup;
            s.HotKey = k;
            Setup = s;
        }

        private bool ChekMinCD()
        {
            return ActionTimer.Chek((int)(Setup.MinCD * 1000f));
        }

        private bool ChekHP(GrabedData data)
        {
            if (!data.FindedFlags.HP_isFind)
                return false;

            if (Setup.UseActPercent)
                return data.HP.Precent < (float)(Setup.ActPercent / 100f);
            else
                return data.HP.Current < Setup.ActFlat;
        }

        private bool ChekMP(GrabedData data)
        {
            if (!data.FindedFlags.MP_isFind)
                return false;


            if (Setup.UseActPercent)
                return data.MP.Precent < (float)(Setup.ActPercent / 100f);
            else
                return data.MP.Current < Setup.ActFlat;
        }

        private bool ChekES(GrabedData data)
        {
            if (!data.FindedFlags.ES_isFind)
                return false;


            if (Setup.UseActPercent)
                return data.ES.Precent < (float)(Setup.ActPercent / 100f);
            else
                return data.ES.Current < Setup.ActFlat;
        }

        private bool FlaskIsActive(GrabedData data, FlaskSlot slot)
        {
            if (!data.FindedFlags.Any_isFind)
                return false;

            return !(data.FlasksState.States[slot] < MINIMUM_STATE_FOR_USE_FLASK);
        }

        private bool ChekOnetime(GrabedData data, FlaskSlot slot)
        {
            if (!FlaskIsActive(data, slot))
            {
                OneUseTimer.Update();
                return false;
            }

            if (OneUseTimer.Chek(OneUseTimerTreshold))
            {
                OneUseTimer.Update();
                return true;
            }

            return false;
        }
    }
}
