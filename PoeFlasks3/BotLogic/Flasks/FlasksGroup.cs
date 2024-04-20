using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic.Flasks
{
    public class FlasksGroup : IUsable
    {
        public readonly FlaskGroup Group;
        public readonly Flask[] Flasks;
        
        private List<FlaskSlot> GroupSlots { get => Flasks.Select(x => x.Slot).ToList(); }

        private int FlaskPointer;
        private readonly int GroupSize;

        private BaseAction firstFlaskBaseAction;

        public FlasksGroup(Profile profile, FlaskGroup group, bool debug = false)
        {
            Group = group;
            FlaskPointer = 0;


            var flasksSettings = profile.Setup.Flasks.Where((x) => x.Value.Group == Group).ToList();
            GroupSize = flasksSettings.Count;
            Flasks = new Flask[GroupSize];

            for (int i = 0; i < GroupSize; i++)
            {
                Flasks[i] = new(flasksSettings[i].Value, debug);
            }
            if (Flasks.Length > 0)
                firstFlaskBaseAction = new(Flasks[0]);
        }

        public void UseAsync()
        {
            firstFlaskBaseAction.UpdateKey(Flasks[FlaskPointer++].Setup.HotKey);
            firstFlaskBaseAction.UseAsync();
            //Flasks[FlaskPointer++].UseAsync();

            if (FlaskPointer >= GroupSize)
                FlaskPointer = 0;
        }

        public bool Chek(GrabedData? grabedData)
        {
            if (Flasks.Length < 1)
                return false;

            bool firstFlaskChek = Flasks[0].ChekGroup(grabedData, GroupSlots);

            return firstFlaskChek;
        }

        private int GetPreviosFlaskPointer(int pointer)
        {
            pointer--;
            if (pointer < 0)
                pointer = 0;
            return pointer;
        }
    }
}
