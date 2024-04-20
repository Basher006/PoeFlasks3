using BotFW_CvSharp_01;
using PoeFlasks3.BotLogic;
using PoeFlasks3.Settings;
using System.Drawing;

namespace PoeFlasks3.SettingsThings
{
    public class Setup
    {
        private const string FLASKS_IMAGE_SAVE_DIRECTORY = "imgs\\Flasks screens\\";
        private const string FLASKS_IMAGE_FILE_EXTENTION = ".png";
        private const string FLASKS_IMAGE_DEFLAUT_IMAGE = "imgs\\Flasks screens\\DefaultFlaskSetupScreen.png";

        public SelectedProfile SelectedProfile;
        public AppSettings AppConfig;
        public Langueges AppLanguege;
        public bool HaveValidePoeLogPath { get; }


        public Setup()
        {
            // load flask setup profiles
            ProfileManager.Load();
            Log.Write($"Load total: {ProfileManager.ProfileNames.Count} Flasks profiles.");

            // load App Settings
            AppConfig = AppSettings.Load();
            Log.Write("Load app settings.");
            HaveValidePoeLogPath = IsValidePoePath(AppConfig);
            if (!HaveValidePoeLogPath)
                Log.Write("App settings dont have valide PoE log path!", Log.LogType.Warn);

            // load and set languge
            var languegeInConfig = AppConfig.Languege;
            if (BotResourseLoader.StrToLanguege.ContainsKey(languegeInConfig))
                AppLanguege = BotResourseLoader.StrToLanguege[languegeInConfig];
            else
            {
                AppLanguege = Langueges.EN;
                AppConfig.Languege = AppLanguege.ToString();
                Log.Write($"Cant load languge: {languegeInConfig}! Load default (EN)!", Log.LogType.Error);
            }

            // init hook
            BotKeyHook.Init();

            // select profile
            if (!ProfileManager.TrySelectProfile(AppConfig.SelectedProfile, out SelectedProfile))
            {
                // Not correct profile name in AppConfig
                // get first
                AppConfig.SelectedProfile = ProfileManager.ProfileNames.First();
                AppConfig.Save();
                if (!ProfileManager.TrySelectProfile(AppConfig.SelectedProfile, out SelectedProfile))
                    throw new Exception("Very strange things happend here"); // this is newer happend
            }

            if (AppConfig.HaveValidePoeLogPath)
                Bot.OnPauseChange(AppConfig.PauseInHo_checkboxState, AppConfig.PoeLogPath);
        }


        private bool IsValidePoePath(AppSettings appConfig)
        {
            if (string.IsNullOrEmpty(appConfig.PoeLogPath))
                return false;

            if (File.Exists(appConfig.PoeLogPath))
                return true;

            return false;
        }

        public bool TryRenameSelectedFrofile(string newName)
        {
            Log.Write($"Try to rename profile: {SelectedProfile.Name}");
            if (SelectedProfile.TryRename(newName))
            {
                UpdateAndSave_SelectedProfile_inConfig();
                Log.Write($"Profile renamed. New name: {SelectedProfile.Name}");
                return true;
            }

            return false;
        }

        public void DeleteSelectedProfile()
        {
            if (File.Exists(SelectedProfile.Profile.Setup.FlasksImagePreview) && SelectedProfile.Profile.Setup.FlasksImagePreview != FLASKS_IMAGE_DEFLAUT_IMAGE)
                File.Delete(SelectedProfile.Profile.Setup.FlasksImagePreview);
            ProfileManager.DeleteProfile(SelectedProfile.Name);

            Log.Write($"Profile: {SelectedProfile.Profile.Name} deleted!");


            if (ProfileManager.ProfileNames.Count < 1)
                ProfileManager.Load();

            SelectedProfile = ProfileManager.SelectProfile(0);
            Log.Write($"Selected profile change to: {SelectedProfile.Name}");

            BotKeyHook.UpdatePauseWhenSecondKeyNotUsedRecently(SelectedProfile.Profile);


        }

        public void CreateAndSelectNewProfile()
        {
            var newSelectedProfile = ProfileManager.AddNewDefaultProfile();
            SelectedProfile = newSelectedProfile;
            UpdateAndSave_SelectedProfile_inConfig();
            Log.Write($"Create new profile: {SelectedProfile.Name}");

            BotKeyHook.UpdatePauseWhenSecondKeyNotUsedRecently(SelectedProfile.Profile);
        }

        public void ChangeSelectedProfile(int index)
        {
            SelectedProfile = ProfileManager.SelectProfile(index);
            Bot.OnFlasksSetupChange(SelectedProfile.Profile);
            UpdateAndSave_SelectedProfile_inConfig();
            Log.Write($"Selected profile change to: {SelectedProfile.Name}");

            BotKeyHook.UpdatePauseWhenSecondKeyNotUsedRecently(SelectedProfile.Profile);
        }

        public void UpdateSelectedProfileFlasksImage(Bitmap bmp)
        {
            var savePath = FLASKS_IMAGE_SAVE_DIRECTORY + SelectedProfile.Name + FLASKS_IMAGE_FILE_EXTENTION;
            if (File.Exists(savePath))
                File.Delete(savePath);
            bmp.Save(savePath);
            Log.Write($"Save flasks image to: {savePath}");
            SelectedProfile.Profile.Setup.FlasksImagePreview = savePath;
            ProfileManager.UpdateProfile(SelectedProfile.Profile);
        }

        private void UpdateAndSave_SelectedProfile_inConfig()
        {
            AppConfig.SelectedProfile = SelectedProfile.Name;
            AppConfig.Save();
        }
    }
}
