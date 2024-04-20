using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic.Flasks
{
    public class Flask : BaseAction
    {
        public FlaskSlot Slot;
        private readonly FlaskSlotSettings FlaskSetup;
        public Flask(FlaskSlotSettings setup, bool debug = false) : base(setup.BaseAction, setup.Slot, debug)
        {
            Slot = setup.Slot;
            FlaskSetup = setup;
        }
    }
}
