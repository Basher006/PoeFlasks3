using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic.Flasks
{
    public class Flask : BaseAction
    {
        private readonly FlaskSlotSettings FlaskSetup;
        public Flask(FlaskSlotSettings setup, bool debug = false) : base(setup.BaseAction, debug)
        {
            FlaskSetup = setup;
        }

        public new bool Chek(GrabedData? grabedData, FlaskKeyUsedRecentlyCheker pauseCheker)
        {
            return Chek(grabedData, pauseCheker, FlaskSetup.Slot);
        }
    }
}
