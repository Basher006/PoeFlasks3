using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic.Flasks
{
    public class FlasksGroup : IUsable
    {
        public readonly FlaskGroup Group;
        public readonly Flask[] Flasks;

        private int FlaskPointer;
        private readonly int GroupSize;

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
        }

        public void Use()
        {
            Flasks[FlaskPointer++].Use();

            if (FlaskPointer >= GroupSize)
                FlaskPointer = 0;
        }

        public bool Chek(GrabedData? grabedData, FlaskKeyUsedRecentlyCheker pauseCheker)
        {
            if (Flasks.Length < 1)
                return false;
            return Flasks[FlaskPointer].Chek(grabedData, pauseCheker);
        }
    }
}
