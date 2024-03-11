using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic.Flasks
{
    public class Flask : BaseAction
    {
        private readonly FlaskSlotSettings FlaskSetup;
        public Flask(FlaskSlotSettings setup, bool debug = false) : base(setup.BaseAction, setup.Slot, debug)
        {
            FlaskSetup = setup;
        }
    }
}
