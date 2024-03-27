using BotFW_CvSharp_01;
using Drinker;
using PoeFlasks3.BotLogic;
using PoeFlasks3.SettingsThings;

namespace DrinkerForm
{
    public partial class FlasksSettingsFrom : Form
    {
        private Langueges AppLanguge;
        private bool ProfileIsRenaimedRightNow = false;
        private string OldProfileName = "";
        private string DefaultRenameButtonText;
        private string OnRenameRenameButtomText;
        private Color DefailtReanameButtonColor = Color.Gainsboro;
        private Color OnRenameRenameButtomColor = Color.LightGreen;

        private FlaskGUIElements[] FlasksGUIElements;
        private AdditionalActionGUIElemets[] AdditionalActionGUIElemets;

        public FlasksSettingsFrom()
        {
            InitializeComponent();

            AppLanguge = PoeFlasks3.Program.Settings.AppLanguege;
            SetLanguegeText();

            Focus();
            InitFlasksGuiElements();
            InitAdditionalActionGUIElemets();

            UpdateFlasksScreenPictureBox();
            //InitFlask(); // it moved to on_window_shown
        }


        private void InitFlask()
        {
            InitProfilesDropBox();
            FlasksGUIManager.Init(PoeFlasks3.Program.Settings.SelectedProfile.Profile, ref FlasksGUIElements, ref AdditionalActionGUIElemets, ref Global_SecondKey_dropBox);
            FlasksGUIManager.onProfileChange += UpdProfile;
        }

        private void InitProfilesDropBox()
        {
            Profiles_dropBox.Items?.Clear();

            Profiles_dropBox.Items.AddRange(ProfileManager.ProfileNames.ToArray());
            Profiles_dropBox.SelectedIndex = PoeFlasks3.Program.Settings.SelectedProfile.Index;
        }

        private void SetLanguegeText()
        {
            Profiles_label.Text = BotResourseLoader.LanguegesText[AppLanguge][14];
            DefaultRenameButtonText = BotResourseLoader.LanguegesText[AppLanguge][15];
            OnRenameRenameButtomText = BotResourseLoader.LanguegesText[AppLanguge][16];
            RenameProfile_button.Text = DefaultRenameButtonText;
            CreateNewProfile_button.Text = BotResourseLoader.LanguegesText[AppLanguge][17];
            DeleteProfile_button.Text = BotResourseLoader.LanguegesText[AppLanguge][18];
            RenameCancel_button.Text = BotResourseLoader.LanguegesText[AppLanguge][19];
            tabPage1.Text = BotResourseLoader.LanguegesText[AppLanguge][20];
            tabPage2.Text = BotResourseLoader.LanguegesText[AppLanguge][21];
            ScreenUpdate_button.Text = BotResourseLoader.LanguegesText[AppLanguge][22];

            GlobalSettings_groupBox.Text = BotResourseLoader.LanguegesText[AppLanguge][40];
            Global_EnablePause_chekbox.Text = BotResourseLoader.LanguegesText[AppLanguge][41];
            Flask1_GlobalSecondKey_label.Text = BotResourseLoader.LanguegesText[AppLanguge][42];
            Flask1_globalPauseAfter_label.Text = BotResourseLoader.LanguegesText[AppLanguge][33];
            Close_button.Text = BotResourseLoader.LanguegesText[AppLanguge][43];
        }

        private void UpdProfile(Profile profile) // (!?)
        {
            FlasksGUIManager.Update(PoeFlasks3.Program.Settings.SelectedProfile.Profile, ref FlasksGUIElements, ref AdditionalActionGUIElemets);
            UpdateFlasksScreenPictureBox();
            Bot.OnFlasksSetupChange(profile);
        }

        private void Close_button_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FlasksSettingsFrom_FormClosed(object sender, FormClosedEventArgs e)
        {
            ProfileManager.SaveAll();
            Program.DrinkerForm.InitProfilesDropBox();
        }

        private void CreateNewProfile_button_Click(object sender, EventArgs e)
        {
            ProfileManager.UpdateProfile(PoeFlasks3.Program.Settings.SelectedProfile.Profile);
            PoeFlasks3.Program.Settings.CreateAndSelectNewProfile();
            InitProfilesDropBox();
            FlasksGUIManager.onProfileChange?.Invoke(PoeFlasks3.Program.Settings.SelectedProfile.Profile);
        }

        private void DeleteProfile_button_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage?.Dispose();
            PoeFlasks3.Program.Settings.DeleteSelectedProfile();
            InitProfilesDropBox();
            FlasksGUIManager.onProfileChange?.Invoke(PoeFlasks3.Program.Settings.SelectedProfile.Profile);

        }

        private void ScreenUpdate_button_Click(object sender, EventArgs e)
        {
            Log.Write("Update flasks screen button click!");
            if (Bot.Client.TryGetFlasksScreen(out var bmp))
            {
                Log.Write("Sucsess get flasks screen!");
                pictureBox1.BackgroundImage?.Dispose();
                pictureBox1.BackgroundImage = bmp;
                PoeFlasks3.Program.Settings.UpdateSelectedProfileFlasksImage(bmp);
            }
        }

        private void UpdateFlasksScreenPictureBox()
        {
            var imagePath = PoeFlasks3.Program.Settings.SelectedProfile.Profile.Setup.FlasksImagePreview;
            if (File.Exists(imagePath))
            {
                var img = new Bitmap(imagePath);
                pictureBox1.BackgroundImage = img;
            }
        }

        private void Profiles_dropBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProfileManager.UpdateProfile(PoeFlasks3.Program.Settings.SelectedProfile.Profile);
            PoeFlasks3.Program.Settings.ChangeSelectedProfile(Profiles_dropBox.SelectedIndex);
            FlasksGUIManager.onProfileChange?.Invoke(PoeFlasks3.Program.Settings.SelectedProfile.Profile);
            ProfileManager.SaveAll();
        }

        private void RenameProfile_button_Click(object sender, EventArgs e)
        {
            if (ProfileIsRenaimedRightNow)
            {
                var newName = Profiles_dropBox.Text;
                ProfileIsRenaimedRightNow = false;

                Profiles_dropBox.DropDownStyle = ComboBoxStyle.DropDownList;

                RenameProfile_button.Text = DefaultRenameButtonText;
                RenameProfile_button.BackColor = DefailtReanameButtonColor;

                RenameCancel_button.Visible = false;

                if (!PoeFlasks3.Program.Settings.TryRenameSelectedFrofile(newName))
                    MessageBox.Show("Not correct profile name!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                InitProfilesDropBox();
            }
            else
            {
                ProfileIsRenaimedRightNow = true;
                OldProfileName = Profiles_dropBox.Text;

                Profiles_dropBox.DropDownStyle = ComboBoxStyle.Simple;
                Profiles_dropBox.Focus();
                Profiles_dropBox.SelectAll();
                RenameProfile_button.Text = OnRenameRenameButtomText;
                RenameProfile_button.BackColor = OnRenameRenameButtomColor;

                RenameCancel_button.Visible = true;
            }
        }

        private void RenameCancel_button_Click(object sender, EventArgs e)
        {
            ProfileIsRenaimedRightNow = false;

            Profiles_dropBox.Text = OldProfileName;
            OldProfileName = "";
            Profiles_dropBox.DropDownStyle = ComboBoxStyle.DropDownList;

            RenameProfile_button.Text = DefaultRenameButtonText;
            RenameProfile_button.BackColor = DefailtReanameButtonColor;

            RenameCancel_button.Visible = false;
        }

        private void Profiles_dropBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!ProfileIsRenaimedRightNow)
                return;

            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
                RenameCancel_button_Click(sender, e);
            else if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                RenameProfile_button_Click(sender, e);
        }

        private void Global_SecondKey_dropBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Global_SecondKey_dropBox.SelectedIndex == -1)
                return;

            var pauseWhen = PoeFlasks3.Program.Settings.SelectedProfile.Profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently;
            var newKey = FlasksGUIManager.GetSecondKey(Global_SecondKey_dropBox.SelectedIndex);
            pauseWhen.Key = newKey;

            PoeFlasks3.Program.Settings.SelectedProfile.Profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently = pauseWhen;
        }

        private void Global_EnablePause_chekbox_CheckedChanged(object sender, EventArgs e)
        {
            var pauseWhen = PoeFlasks3.Program.Settings.SelectedProfile.Profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently;
            pauseWhen.Enable = Global_EnablePause_chekbox.Checked;

            PoeFlasks3.Program.Settings.SelectedProfile.Profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently = pauseWhen;
        }

        private void Global_PauseSec_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            var pauseWhen = PoeFlasks3.Program.Settings.SelectedProfile.Profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently;
            pauseWhen.PauseActivationDelay = (float)Global_PauseSec_numericUpDown.Value;

            PoeFlasks3.Program.Settings.SelectedProfile.Profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently = pauseWhen;
        }

        private void FlasksSettingsFrom_Shown(object sender, EventArgs e)
        {
            InitFlask();
            SetValuesToGlobalPause(PoeFlasks3.Program.Settings.SelectedProfile.Profile);
            FlasksGUIManager.onProfileChange += SetValuesToGlobalPause;
        }

        private void SetValuesToGlobalPause(Profile profile)
        {
            Global_EnablePause_chekbox.Checked = profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently.Enable;

            var key = profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently.Key;
            var index = FlasksGUIManager.GetSecondKeyIndex(key);
            Global_SecondKey_dropBox.SelectedIndex = index;

            var pauseSec = profile.Setup.GlobalPauseWhenSecondKeyNotUsedRecently.PauseActivationDelay;
            Global_PauseSec_numericUpDown.Value = (decimal)pauseSec;
        }

        private void InitFlasksGuiElements()
        {
            FlasksGUIElements = new FlaskGUIElements[]
            {
                new()
                {
                    FlaskPanel =                Flask1_panel,

                    ActType =                   Flask1_ActType_dropBox,

                    PercentRadioButton =        Flask1_Percent_radioButton,
                    FlatRadioButton =           Flask1_Flat_radioButton,
                    PercentValue =              Flask1_Percent_numericUpDown,
                    FlatValue =                 Flask1_Flat_numericUpDown,

                    PauseEnable =               Flask1_Pause_checkBox,
                    PauseEnableText =           Flask1_Pause_label,
                    SecondKey =                 Flask1_SecondKey_dropBox,
                    PauseSecText =              Flask1_PauseAfter_label,
                    PauseSecValue =             Flask1_PauseSec_numericUpDown,

                    FlaskInGameHotkeyText =     Flask1_InGameHotkey_label,
                    FlaskInGameHotkey =         Flask1_InGameHotkey_dropBox,

                    FlaskMinimumCDText =        Flask1_minCD_label,
                    FlaskMinimumCD =            Flask1_MinCD_numericUpDown,

                    FlaskGroupText =            Flask1_Group_label,
                    FlaskGroupBox =             Flask1_group_dropBox,
                },
                new()
                {
                    FlaskPanel =                Flask2_panel,

                    ActType =                   Flask2_ActType_dropBox,

                    PercentRadioButton =        Flask2_Percent_radioButton,
                    FlatRadioButton =           Flask2_Flat_radioButton,
                    PercentValue =              Flask2_Percent_numericUpDown,
                    FlatValue =                 Flask2_Flat_numericUpDown,

                    PauseEnable =               Flask2_Pause_checkBox,
                    PauseEnableText =           Flask2_Pause_label,
                    SecondKey =                 Flask2_SecondKey_dropBox,
                    PauseSecText =              Flask2_PauseAfter_label,
                    PauseSecValue =             Flask2_PauseSec_numericUpDown,

                    FlaskInGameHotkeyText =     Flask2_InGameHotkey_label,
                    FlaskInGameHotkey =         Flask2_InGameHotkey_dropBox,

                    FlaskMinimumCDText =        Flask2_minCD_label,
                    FlaskMinimumCD =            Flask2_MinCD_numericUpDown,

                    FlaskGroupText =            Flask2_Group_label,
                    FlaskGroupBox =             Flask2_group_dropBox
                },
                new()
                {
                    FlaskPanel =                Flask3_panel,

                    ActType =                   Flask3_ActType_dropBox,

                    PercentRadioButton =        Flask3_Percent_radioButton,
                    FlatRadioButton =           Flask3_Flat_radioButton,
                    PercentValue =              Flask3_Percent_numericUpDown,
                    FlatValue =                 Flask3_Flat_numericUpDown,

                    PauseEnable =               Flask3_Pause_checkBox,
                    PauseEnableText =           Flask3_Pause_label,
                    SecondKey =                 Flask3_SecondKey_dropBox,
                    PauseSecText =              Flask3_PauseAfter_label,
                    PauseSecValue =             Flask3_PauseSec_numericUpDown,

                    FlaskInGameHotkeyText =     Flask3_InGameHotkey_label,
                    FlaskInGameHotkey =         Flask3_InGameHotkey_dropBox,

                    FlaskMinimumCDText =        Flask3_minCD_label,
                    FlaskMinimumCD =            Flask3_MinCD_numericUpDown,

                    FlaskGroupText =            Flask3_Group_label,
                    FlaskGroupBox =             Flask3_group_dropBox
                },
                new()
                {
                    FlaskPanel =                Flask4_panel,

                    ActType =                   Flask4_ActType_dropBox,

                    PercentRadioButton =        Flask4_Percent_radioButton,
                    FlatRadioButton =           Flask4_Flat_radioButton,
                    PercentValue =              Flask4_Percent_numericUpDown,
                    FlatValue =                 Flask4_Flat_numericUpDown,

                    PauseEnable =               Flask4_Pause_checkBox,
                    PauseEnableText =           Flask4_Pause_label,
                    SecondKey =                 Flask4_SecondKey_dropBox,
                    PauseSecText =              Flask4_PauseAfter_label,
                    PauseSecValue =             Flask4_PauseSec_numericUpDown,

                    FlaskInGameHotkeyText =     Flask4_InGameHotkey_label,
                    FlaskInGameHotkey =         Flask4_InGameHotkey_dropBox,

                    FlaskMinimumCDText =        Flask4_minCD_label,
                    FlaskMinimumCD =            Flask4_MinCD_numericUpDown,

                    FlaskGroupText =            Flask4_Group_label,
                    FlaskGroupBox =             Flask4_group_dropBox
                },
                new()
                {
                    FlaskPanel =                Flask5_panel,

                    ActType =                   Flask5_ActType_dropBox,

                    PercentRadioButton =        Flask5_Percent_radioButton,
                    FlatRadioButton =           Flask5_Flat_radioButton,
                    PercentValue =              Flask5_Percent_numericUpDown,
                    FlatValue =                 Flask5_Flat_numericUpDown,

                    PauseEnable =               Flask5_Pause_checkBox,
                    PauseEnableText =           Flask5_Pause_label,
                    SecondKey =                 Flask5_SecondKey_dropBox,
                    PauseSecText =              Flask5_PauseAfter_label,
                    PauseSecValue =             Flask5_PauseSec_numericUpDown,

                    FlaskInGameHotkeyText =     Flask5_InGameHotkey_label,
                    FlaskInGameHotkey =         Flask5_InGameHotkey_dropBox,

                    FlaskMinimumCDText =        Flask5_minCD_label,
                    FlaskMinimumCD =            Flask5_MinCD_numericUpDown,

                    FlaskGroupText =            Flask5_Group_label,
                    FlaskGroupBox =             Flask5_group_dropBox
                },
            };
        }

        private void InitAdditionalActionGUIElemets()
        {
            AdditionalActionGUIElemets = new AdditionalActionGUIElemets[]
            {
                new()
                {
                    AddActPanel =               AddAct1_panel,

                    ActType =                   AddAct1_ActType_dropBox,

                    PercentRadioButton =        AddAct1_Percent_radioButton,
                    FlatRadioButton =           AddAct1_Flat_radioButton,
                    PercentValue =              AddAct1_Percent_numericUpDown,
                    FlatValue =                 AddAct1_Flat_numericUpDown,

                    PauseEnable =               AddAct1_Pause_checkBox,
                    PauseEnableText =           AddAct1_Pause_label,
                    SecondKey =                 AddAct1_SecondKey_dropBox,
                    PauseSecText =              AddAct1_PauseAfter_label,
                    PauseSecValue =             AddAct1_PauseSec_numericUpDown,

                    AddActInGameHotkeyText =    AddAct1_InGameHotkey_label,
                    AddActInGameHotkey =        AddAct1_InGameHotkey_dropBox,

                    AddActMinimumCDText =       AddAct1_MinCD_label,
                    AddActMinimumCD =           AddAct1_MinCD_numericUpDown,
                },
                new()
                {
                    AddActPanel =               AddAct2_panel,

                    ActType =                   AddAct2_ActType_dropBox,

                    PercentRadioButton =        AddAct2_Percent_radioButton,
                    FlatRadioButton =           AddAct2_Flat_radioButton,
                    PercentValue =              AddAct2_Percent_numericUpDown,
                    FlatValue =                 AddAct2_Flat_numericUpDown,

                    PauseEnable =               AddAct2_Pause_checkBox,
                    PauseEnableText =           AddAct2_Pause_label,
                    SecondKey =                 AddAct2_SecondKey_dropBox,
                    PauseSecText =              AddAct2_PauseAfter_label,
                    PauseSecValue =             AddAct2_PauseSec_numericUpDown,

                    AddActInGameHotkeyText =    AddAct2_InGameHotkey_label,
                    AddActInGameHotkey =        AddAct2_InGameHotkey_dropBox,

                    AddActMinimumCDText =       AddAct2_MinCD_label,
                    AddActMinimumCD =           AddAct2_MinCD_numericUpDown,
                },
                new()
                {
                    AddActPanel =               AddAct3_panel,

                    ActType =                   AddAct3_ActType_dropBox,

                    PercentRadioButton =        AddAct3_Percent_radioButton,
                    FlatRadioButton =           AddAct3_Flat_radioButton,
                    PercentValue =              AddAct3_Percent_numericUpDown,
                    FlatValue =                 AddAct3_Flat_numericUpDown,

                    PauseEnable =               AddAct3_Pause_checkBox,
                    PauseEnableText =           AddAct3_Pause_label,
                    SecondKey =                 AddAct3_SecondKey_dropBox,
                    PauseSecText =              AddAct3_PauseAfter_label,
                    PauseSecValue =             AddAct3_PauseSec_numericUpDown,

                    AddActInGameHotkeyText =    AddAct3_InGameHotkey_label,
                    AddActInGameHotkey =        AddAct3_InGameHotkey_dropBox,

                    AddActMinimumCDText =       AddAct3_MinCD_label,
                    AddActMinimumCD =           AddAct3_MinCD_numericUpDown,
                },                
                new()
                {
                    AddActPanel =               AddAct4_panel,

                    ActType =                   AddAct4_ActType_dropBox,

                    PercentRadioButton =        AddAct4_Percent_radioButton,
                    FlatRadioButton =           AddAct4_Flat_radioButton,
                    PercentValue =              AddAct4_Percent_numericUpDown,
                    FlatValue =                 AddAct4_Flat_numericUpDown,

                    PauseEnable =               AddAct4_Pause_checkBox,
                    PauseEnableText =           AddAct4_Pause_label,
                    SecondKey =                 AddAct4_SecondKey_dropBox,
                    PauseSecText =              AddAct4_PauseAfter_label,
                    PauseSecValue =             AddAct4_PauseSec_numericUpDown,

                    AddActInGameHotkeyText =    AddAct4_InGameHotkey_label,
                    AddActInGameHotkey =        AddAct4_InGameHotkey_dropBox,

                    AddActMinimumCDText =       AddAct4_MinCD_label,
                    AddActMinimumCD =           AddAct4_MinCD_numericUpDown,
                },                
                new()
                {
                    AddActPanel =               AddAct5_panel,

                    ActType =                   AddAct5_ActType_dropBox,

                    PercentRadioButton =        AddAct5_Percent_radioButton,
                    FlatRadioButton =           AddAct5_Flat_radioButton,
                    PercentValue =              AddAct5_Percent_numericUpDown,
                    FlatValue =                 AddAct5_Flat_numericUpDown,

                    PauseEnable =               AddAct5_Pause_checkBox,
                    PauseEnableText =           AddAct5_Pause_label,
                    SecondKey =                 AddAct5_SecondKey_dropBox,
                    PauseSecText =              AddAct5_PauseAfter_label,
                    PauseSecValue =             AddAct5_PauseSec_numericUpDown,

                    AddActInGameHotkeyText =    AddAct5_InGameHotkey_label,
                    AddActInGameHotkey =        AddAct5_InGameHotkey_dropBox,

                    AddActMinimumCDText =       AddAct5_MinCD_label,
                    AddActMinimumCD =           AddAct5_MinCD_numericUpDown,
                }
            };
        }
    }
}
