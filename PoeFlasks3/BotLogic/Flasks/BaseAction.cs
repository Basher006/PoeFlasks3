using BotFW_CvSharp_01;
using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic.Flasks
{
    public class BaseAction : IUsable
    {
        private const float MINIMUM_STATE_FOR_USE_FLASK = 0.05f;
        private const int OneUseTimerTreshold = 3000;

        private BaseActionSettings Setup { get; }
        private readonly MyTimer ActionTimer;
        private readonly MyTimer OneUseTimer;

        private readonly bool DEBUG;


        public BaseAction(BaseActionSettings setup, bool debug = false)
        {
            DEBUG = debug;

            Setup = setup;
            ActionTimer = new();
            OneUseTimer = new();
        }


        public void Use()
        {
            ActionTimer.Update();
            BotFW.SendKey(Setup.HotKey, debug: DEBUG);

        }

        public bool Chek(GrabedData? grabedData, FlaskKeyUsedRecentlyCheker pauseCheker)
        {
            if (!grabedData.HasValue)
                return false;

            // None
            if (Setup.ActType == ActivationType.None)
                return false;
            if (!ChekFlaskMinCD())
                return false;

            // chek pause on not use other skill recently
            if (pauseCheker.PauseIsEnable(Setup))
                return false;

            // HP
            if (Setup.ActType == ActivationType.HP)
                return ChekHP(grabedData.Value);
            // MP
            if (Setup.ActType == ActivationType.MP)
                return ChekMP(grabedData.Value);
            // ES
            if (Setup.ActType == ActivationType.ES)
                return ChekES(grabedData.Value);

            // CD
            if (Setup.ActType == ActivationType.CD)
                return ChekFlaskMinCD(); // this is newer happend (!)

            // not avalible in base action (!)

            //// oneTime
            //if (Setup.ActType == ActivationType.OneTime)
            //    return ChekOnetime(grabedData.Value);


            throw new Exception("Uncorrect Activation Type!");
        }

        protected bool Chek(GrabedData? grabedData, FlaskKeyUsedRecentlyCheker pauseCheker, FlaskSlot slot)
        {
            if (!grabedData.HasValue)
                return false;

            // None
            if (Setup.ActType == ActivationType.None)
                return false;
            if (!ChekFlaskMinCD())
                return false;

            // chek pause on not use other skill recently
            if (pauseCheker.PauseIsEnable(Setup))
                return false;

            // HP
            if (Setup.ActType == ActivationType.HP)
                return ChekHP(grabedData.Value);
            // MP
            if (Setup.ActType == ActivationType.MP)
                return ChekMP(grabedData.Value);
            // ES
            if (Setup.ActType == ActivationType.ES)
                return ChekES(grabedData.Value);
            // CD
            if (Setup.ActType == ActivationType.CD)
                return ChekCD(grabedData.Value, slot);
            // oneTime
            if (Setup.ActType == ActivationType.OneTime)
                return ChekOnetime(grabedData.Value, slot);


            throw new Exception("Uncorrect Activation Type!");
        }

        private bool ChekFlaskMinCD()
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

        private bool ChekCD(GrabedData data, FlaskSlot slot)
        {
            if (!data.FindedFlags.Any_isFind)
                return false;


            return data.FlasksState.States[slot] < MINIMUM_STATE_FOR_USE_FLASK;
        }

        private bool ChekOnetime(GrabedData data, FlaskSlot slot)
        {
            if (!ChekCD(data, slot))
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
