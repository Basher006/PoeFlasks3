using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.BotLogic.Flasks
{
    public class FlasksManager : IDisposable
    {
        private readonly Profile Profile;
        private readonly List<IUsable> UsableThings;
        private readonly FlaskKeyUsedRecentlyCheker FlaskUseRecentlyCheker;

        private readonly bool DEBUG;

        public FlasksManager(Profile profile, bool debug = false)
        {
            FlaskUseRecentlyCheker = new(profile.Setup);

            DEBUG = debug;

            Profile = profile;

            // Add Flasks groups;
            UsableThings = new()
            {
                new FlasksGroup(Profile, FlaskGroup.Group1, DEBUG),
                new FlasksGroup(Profile, FlaskGroup.Group2, DEBUG)
            };

            // Add Non groups Flasks
            var nonGroupFlasks = Profile.Setup.Flasks.Where((x) => x.Value.Group == FlaskGroup.None);
            foreach (var fl in nonGroupFlasks)
            {
                UsableThings.Add(new Flask(fl.Value, DEBUG));
            }

            // Add additional actions
            foreach (var addAction in Profile.Setup.AdditionalActions)
            {
                UsableThings.Add(new BaseAction(addAction, DEBUG));
            }
        }

        public void UseFlasks(GrabedData? data)
        {
            List<Action> useActions = new();

            foreach (var useblaThing in UsableThings)
            {
                if (useblaThing.Chek(data, FlaskUseRecentlyCheker))
                    useActions.Add(useblaThing.Use);
            }

            // Use async
            foreach (var useAct in useActions)
            {
                _useFlaskAsync(useAct);
            }
        }

        private async void _useFlaskAsync(Action useAction)
        {
            await new Task(useAction);
        }

        public void Dispose()
        {
            FlaskUseRecentlyCheker.Dispose();
        }
    }
}
