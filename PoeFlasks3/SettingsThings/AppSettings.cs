using BotFW_CvSharp_01.MyConfig;
using PoeFlasks3.SettingsThings;

namespace PoeFlasks3.Settings
{
    public struct AppSettings
    {
        private const string SaveDirectory = "Settings\\";
        public const string LocalSavePath = SaveDirectory + "config.cfg";

        public bool HaveValidePoeLogPath { get => IsValidePoePath(); }

        private ConfigLine _appPos_x;
        public int AppPos_x 
        { 
            get => (int)_appPos_x.Value;
            set => _appPos_x = new ConfigLine("AppPos_x", value);
        }


        private ConfigLine _appPos_y;
        public int AppPos_y 
        { 
            get => (int)_appPos_y.Value;
            set => _appPos_y = new ConfigLine("AppPos_y", value);
        }


        private ConfigLine _pauseInHo_checkboxState;
        public bool PauseInHo_checkboxState 
        { 
            get => (bool)_pauseInHo_checkboxState.Value;
            set => _pauseInHo_checkboxState = new ConfigLine("PauseInHo_checkboxState", value);
        }


        private ConfigLine _selectedProfile;
        public string SelectedProfile 
        {
            get => (string)_selectedProfile.Value;
            set => _selectedProfile = new ConfigLine("SelectedProfile", value);
        }

        private ConfigLine _poeLogPath;
        public string PoeLogPath
        {
            get => (string)_poeLogPath.Value;
            set => _poeLogPath = new ConfigLine("PoeLogPath", value);
        }

        public AppSettings(ConfigLine[] loadedArray)
        {
            this = new AppSettings();

            for (int i = 0; i < loadedArray.Length; i++)
            {
                string valueName = loadedArray[i].Name;
                switch (valueName)
                {
                    case "AppPos_x":
                        _appPos_x = loadedArray[i];
                        break;
                    case "AppPos_y":
                        _appPos_y = loadedArray[i];
                        break;
                    case "PauseInHo_checkboxState":
                        _pauseInHo_checkboxState = loadedArray[i];
                        break;
                    case "SelectedProfile":
                        _selectedProfile = loadedArray[i];
                        break;
                    case "PoeLogPath":
                        if (loadedArray[i].type == ConfigLineType.Empty)
                            _poeLogPath = new ConfigLine(loadedArray[i].Name, string.Empty);
                        else
                            _poeLogPath = loadedArray[i];
                        break;
                    default:
                        break;
                }
            }
        }
        public readonly ConfigLine[] ToArray()
        {
            return new ConfigLine[] { _appPos_x, _appPos_y, _pauseInHo_checkboxState, _selectedProfile, _poeLogPath };
        }

        public readonly void Save()
        {
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);

            var configLines = this.ToArray();
            AppConfig.Save(configLines, LocalSavePath);
        }

        public static AppSettings Load()
        {
            if (AppConfig.TryLoad(out var loadArray, LocalSavePath))
                return new AppSettings(loadArray);

            return GetDefault();
        }


        private static AppSettings GetDefault()
        {
            return new AppSettings()
            {
                _appPos_x = new ConfigLine("AppPos_x", 500),
                _appPos_y = new ConfigLine("AppPos_y", 300),
                _pauseInHo_checkboxState = new ConfigLine("PauseInHo_checkboxState", false),
                _selectedProfile = new ConfigLine("SelectedProfile", ProfileManager.DefaultProfileName),
                _poeLogPath = new ConfigLine("PoeLogPath", "")
            };
        }

        public static bool IsValidePoePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (File.Exists(path))
                return true;

            return false;
        }

        private bool IsValidePoePath()
        {
            return IsValidePoePath(PoeLogPath);
        }
    }
}
