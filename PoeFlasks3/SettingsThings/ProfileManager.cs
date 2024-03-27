using BotFW_CvSharp_01.GlobalStructs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PoeFlasks3.SettingsThings
{
    public struct Profile
    {
        public string Name;
        public FlasksSetup Setup;

        public Profile(string name, FlasksSetup data)
        {
            Name = name;
            Setup = data;
        }

        public readonly bool TrySave()
        {
            return ProfileManager.TrySave(this);
        }

        public bool TryRename(string newName)
        {
            if (newName == Name)
                return true;

            if (!ProfileManager.IsValideFileName(newName))
                return false;

            while (ProfileManager.ProfileNames.Contains(newName))
                newName = "New " + newName;

            string oldName = Name;
            Name = newName;
            if (TrySave())
            {
                ProfileManager.DeleteProfile(oldName);
                ProfileManager.AddProfile(this);
                return true;
            }
            else
            {
                Name = oldName;
            }

            return false;
        }
    }

    public struct SelectedProfile
    {
        public string Name;
        public int Index;
        public Profile Profile;

        public bool TryRename(string newName)
        {
            // if sucsess rename profile
            //      if sucsess getted index
            //          return true
            //      else
            //          cancel changes
            //
            // retrun false

            if (newName == Name)
                return true;

            if (Profile.TryRename(newName))
            {
                string oldName = Name;
                Name = newName;
                if (TryGetIndex(out int index))
                {
                    Index = index;
                    return true;
                }
                else
                {
                    Name = oldName;
                    if (TryGetIndex(out int index2))
                    {
                        Index = index2;
                        return false;
                    }
                    else
                        throw new Exception("Unexpected exception when try get profile index!");
                }
            }

            return false;
        }

        private readonly bool TryGetIndex(out int index)
        {
            if (ProfileManager.ProfileNames.Contains(Name))
            {
                index = ProfileManager.ProfileNames.IndexOf(Name);
                return true;
            }
            index = 0;

            return false;
        }
    }

    public struct FlasksSetup
    {
        public Dictionary<FlaskSlot, FlaskSlotSettings> Flasks { get; set; }
        [JsonIgnore] public readonly List<FlaskSlotSettings> FlasksList { get => Flasks.Values.ToList(); }
        public List<BaseActionSettings> AdditionalActions { get; set; }
        public PauseWhenSecondKeyNotUsedRecently GlobalPauseWhenSecondKeyNotUsedRecently { get; set; }
        public string FlasksImagePreview { get; set; }
    }

    public struct FlaskSlotSettings
    {
        public FlaskSlot Slot { get; set; }
        public FlaskGroup Group { get; set; }

        public BaseActionSettings BaseAction { get; set; }
    }

    public struct BaseActionSettings
    {
        public ActivationType ActType { get; set; }
        public bool UseActPercent { get; set; }
        public int ActPercent { get; set; }
        public int ActFlat { get; set; }
        public float MinCD { get; set; }
        public Keys HotKey { get; set; }
        public PauseWhenSecondKeyNotUsedRecently PauseWhenSecondKeyNotUsedRecently { get; set; }
    }

    public struct PauseWhenSecondKeyNotUsedRecently
    {
        private const float MAX_SECONDS_WAIT_FOR_USE_KEY_RECENTLY = 300f;

        // this id need for use this structs as uniq keys in dictionary
        private static int id;
        private readonly int _id;

        public bool Enable { get; set; }
        public Keys Key { get; set; }
        public float PauseActivationDelay { get => _pauseActivationDelay; set => _pauseActivationDelay = SetPauseActivationDelay(value); }
        private float _pauseActivationDelay;

        public PauseWhenSecondKeyNotUsedRecently()
        {
            Enable = false;
            Key = Keys.NUM_1;
            _pauseActivationDelay = 15f;
            _id = id++;
        }

        private readonly float SetPauseActivationDelay(float value)
        {
            return value > MAX_SECONDS_WAIT_FOR_USE_KEY_RECENTLY ? MAX_SECONDS_WAIT_FOR_USE_KEY_RECENTLY : value;
        }
    }

    public enum ActivationType
    {
        None,
        HP,
        MP,
        ES,
        CD,
        OneTime
    }

    public enum FlaskGroup
    {
        None,
        Group1,
        Group2
    }

    public enum FlaskSlot
    {
        Slot1,
        Slot2,
        Slot3,
        Slot4,
        Slot5,
    }


    public static class ProfileManager
    {
        public const string DefaultProfileName = "Default Profile";
        private const string SaveDirectory = "Profiles\\";
        private const string FileExtension = ".json";


        private static Dictionary<string, Profile> Profiles = new ();
        public static List<string> ProfileNames { get => Profiles.Keys.ToList(); }


        public static void SaveAll()
        {
            Profiles[Program.Settings.SelectedProfile.Name] = Program.Settings.SelectedProfile.Profile;

            foreach (var profile in Profiles)
            {
                profile.Value.TrySave();
            }
        }

        public static bool TrySave(Profile profile)
        {
            if (IsValideFileName(profile.Name))
            {
                SaveFlasksData(profile.Name, profile.Setup);
                return true;
            }

            
            return false;
        }

        public static bool IsValideFileName(string name)
        {
            return name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }

        private static void SaveFlasksData(string name, FlasksSetup data)
        {
            string savePath = SaveDirectory + name + FileExtension;
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(savePath, json);
        }

        public static void Load()
        {
            Profiles.Clear();
            _load();
            if (!Profiles.ContainsKey(DefaultProfileName)) // this happens when user delete default profile file.
            {
                var defaultProfile = GetDefaultProfile();
                AddProfile(defaultProfile);
                defaultProfile.TrySave();
            }
        }

        public static SelectedProfile SelectProfile(int index)
        {
            return SelectProfileByIndex(index);
        }

        public static bool TrySelectProfile(string name, out SelectedProfile selectedProfile)
        {
            return TrySelectProfileByName(name, out selectedProfile);
        }

        private static bool TrySelectProfileByName(string name, out SelectedProfile selectedProfile)
        {
            if (ProfileNames.Contains(name))
            {
                selectedProfile = new SelectedProfile() { Name = name, Index = ProfileNames.IndexOf(name), Profile = Profiles[name] };
                return true;
            }

            selectedProfile = new();
            return false;
        }

        private static SelectedProfile SelectProfileByIndex(int index)
        {
            string name = ProfileNames[index];
            return new SelectedProfile() { Name = name, Index = index, Profile = Profiles[name] };
        }


        public static void AddProfile(Profile profile)
        {
            Profiles.Add(profile.Name, profile);
        }

        public static SelectedProfile AddNewDefaultProfile()
        {
            var newProfile = GetDefaultProfile();
            while (ProfileNames.Contains(newProfile.Name))
                newProfile.Name = "New " + newProfile.Name;

            AddProfile(newProfile);
            newProfile.TrySave();

            TrySelectProfileByName(newProfile.Name, out SelectedProfile selectedProfile);

            return selectedProfile;
        }

        public static void UpdateProfile(Profile profile)
        {
            Profiles[profile.Name] = profile;
        }

        public static void DeleteProfile(string name)
        {
            Profiles.Remove(name);
            string path = $"{SaveDirectory}{name}{FileExtension}";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static void _load()
        {
            var profilesPathes = GetProfilesFileNames();
            if (profilesPathes.Length > 0) // if any profile file is found => load all finded files
            {
                
                for (int i = 0; i < profilesPathes.Length; i++)
                {
                    if (TryLoadFlaskSetupFromFile(out FlasksSetup data, profilesPathes[i]))
                    {
                        string profileName = profilesPathes[i].Replace(FileExtension, "").Replace(SaveDirectory, "");
                        var LoadedProfile = new Profile(profileName, data);
                        Profiles.Add(profileName, LoadedProfile);
                    }
                }
            }
            else 
            {
                // if any profile file not found:
                //      load default
                //      create directory
                //      create file
                LoadDefaultProfile(); 
                Directory.CreateDirectory(SaveDirectory);
                foreach (var profile in Profiles)
                {
                    TrySave(profile.Value);
                }
            }
        }

        private static bool TryLoadFlaskSetupFromFile(out FlasksSetup data, string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                data = JsonSerializer.Deserialize<FlasksSetup>(json);

                return true;
            }

            data = new();
            return false;
        }

        private static string[] GetProfilesFileNames()
        {
            if (Directory.Exists(SaveDirectory))
            {
                var files = Directory.EnumerateFiles(SaveDirectory);
                List<string> result = new();
                foreach (var f in files)
                {
                    if (f.Contains(FileExtension))
                        result.Add(f);
                }
                return result.ToArray();
            }
            else
                return Array.Empty<string>();

        }

        private static void LoadDefaultProfile()
        {
            Profiles.Clear();
            var dp = GetDefaultProfile();
            Profiles.Add(dp.Name, dp);
        }

        private static Profile GetDefaultProfile()
        {
            return new Profile() { Name = DefaultProfileName, Setup = GetDefaultSetup() };
        }

        private static FlasksSetup GetDefaultSetup()
        {
            FlasksSetup data = new()
            {
                Flasks = new()
                {
                    { FlaskSlot.Slot1, new FlaskSlotSettings() { Slot = FlaskSlot.Slot1, Group = FlaskGroup.None, BaseAction = new()
                    { ActType = ActivationType.None, UseActPercent = true, ActPercent = 50, ActFlat = 0, HotKey = Keys.NUM_1, MinCD = 3f,
                        PauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f }
                    } } },

                    { FlaskSlot.Slot2, new FlaskSlotSettings() { Slot = FlaskSlot.Slot2, Group = FlaskGroup.None, BaseAction = new()
                    { ActType = ActivationType.None, UseActPercent = true, ActPercent = 50, ActFlat = 0, HotKey = Keys.NUM_2, MinCD = 3f,
                        PauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f }
                    } } },

                    { FlaskSlot.Slot3, new FlaskSlotSettings() { Slot = FlaskSlot.Slot3, Group = FlaskGroup.None, BaseAction = new()
                    { ActType = ActivationType.None, UseActPercent = true, ActPercent = 50, ActFlat = 0, HotKey = Keys.NUM_3, MinCD = 3f,
                        PauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f }
                    } } },

                    { FlaskSlot.Slot4, new FlaskSlotSettings() { Slot = FlaskSlot.Slot4, Group = FlaskGroup.None, BaseAction = new()
                    { ActType = ActivationType.None, UseActPercent = true, ActPercent = 50, ActFlat = 0, HotKey = Keys.NUM_4, MinCD = 3f,
                        PauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f }
                    } } },

                    { FlaskSlot.Slot5, new FlaskSlotSettings() { Slot = FlaskSlot.Slot5, Group = FlaskGroup.None, BaseAction = new()
                    { ActType = ActivationType.None, UseActPercent = true, ActPercent = 50, ActFlat = 0, HotKey = Keys.NUM_5, MinCD = 3f,
                        PauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f }
                    } } },
                },

                AdditionalActions = new()
                {
                    new() { ActType = ActivationType.None, UseActPercent = true, ActPercent = 50, ActFlat = 0, HotKey = Keys.Q, MinCD = 3f,
                        PauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f }},
                    new() { ActType = ActivationType.None, UseActPercent = true, ActPercent = 50, ActFlat = 0, HotKey = Keys.W, MinCD = 3f,
                        PauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f }},
                    new() { ActType = ActivationType.None, UseActPercent = true, ActPercent = 50, ActFlat = 0, HotKey = Keys.E, MinCD = 3f,
                        PauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f }},
                    new() { ActType = ActivationType.None, UseActPercent = true, ActPercent = 50, ActFlat = 0, HotKey = Keys.R, MinCD = 3f,
                        PauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f }},
                    new() { ActType = ActivationType.None, UseActPercent = true, ActPercent = 50, ActFlat = 0, HotKey = Keys.T, MinCD = 3f,
                        PauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f }}
                },

                GlobalPauseWhenSecondKeyNotUsedRecently = new() { Enable = false, Key = Keys.MouseRight, PauseActivationDelay = 15f },

                FlasksImagePreview = "imgs\\Flasks screens\\DefaultFlaskSetupScreen.png"
            };

            return data;
        }
    }
}
