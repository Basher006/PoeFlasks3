﻿using BotFW_CvSharp_01;
using PoeFlasks3.Settings;

namespace PoeFlasks3.SettingsThings
{
    public class Setup
    {
        public SelectedProfile SelectedProfile;
        public AppSettings AppConfig;
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
            ProfileManager.DeleteProfile(SelectedProfile.Name);
            Log.Write($"Profile: {SelectedProfile.Profile.Name} deleted!");

            if (ProfileManager.ProfileNames.Count < 1)
                ProfileManager.Load();

            SelectedProfile = ProfileManager.SelectProfile(0);
            Log.Write($"Selected profile change to: {SelectedProfile.Name}");
        }

        public void CreateAndSelectNewProfile()
        {
            var newSelectedProfile = ProfileManager.AddNewDefaultProfile();
            SelectedProfile = newSelectedProfile;
            UpdateAndSave_SelectedProfile_inConfig();
            Log.Write($"Create new profile: {SelectedProfile.Name}");
        }

        public void ChangeSelectedProfile(int index)
        {
            SelectedProfile = ProfileManager.SelectProfile(index);
            UpdateAndSave_SelectedProfile_inConfig();
            Log.Write($"Selected profile change to: {SelectedProfile.Name}");
        }

        private void UpdateAndSave_SelectedProfile_inConfig()
        {
            AppConfig.SelectedProfile = SelectedProfile.Name;
            AppConfig.Save();
        }
    }
}