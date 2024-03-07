﻿using Drinker;
using PoeFlasks3.SettingsThings;

namespace DrinkerForm
{
    public partial class FlasksSettingsFrom : Form
    {
        private bool ProfileIsRenaimedRightNow = false;
        private string OldProfileName = "";
        private string DefaultRenameButtonText = "Rename";
        private string OnRenameRenameButtomText = "Save changes";
        private Color DefailtReanameButtonColor = Color.Gainsboro;
        private Color OnRenameRenameButtomColor = Color.LightGreen;

        private FlaskGUIElements[] FlasksGUIElements;

        public FlasksSettingsFrom()
        {
            InitializeComponent();

            InitGuiElements();
            InitFlask();
        }



        private void InitFlask()
        {
            InitProfilesDropBox();
            FlasksGUIManager.Init(PoeFlasks3.Program.Settings.SelectedProfile.Profile, ref FlasksGUIElements);
            FlasksGUIManager.onProfileChange += UpdProfile;
        }

        private void InitProfilesDropBox()
        {
            Profiles_dropBox.Items?.Clear();

            Profiles_dropBox.Items.AddRange(ProfileManager.ProfileNames.ToArray());
            Profiles_dropBox.SelectedIndex = PoeFlasks3.Program.Settings.SelectedProfile.Index;
        }

        private void UpdProfile(Profile profile)
        {
            FlasksGUIManager.Update(PoeFlasks3.Program.Settings.SelectedProfile.Profile, ref FlasksGUIElements);
        }

        private void Close_button_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FlasksSettingsFrom_FormClosed(object sender, FormClosedEventArgs e)
        {
            ProfileManager.SaveAllAsync();
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
            PoeFlasks3.Program.Settings.DeleteSelectedProfile();
            InitProfilesDropBox();
            FlasksGUIManager.onProfileChange?.Invoke(PoeFlasks3.Program.Settings.SelectedProfile.Profile);
        }

        private void ScreenUpdate_button_Click(object sender, EventArgs e)
        {
            // (!)
        }

        private void Profiles_dropBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProfileManager.UpdateProfile(PoeFlasks3.Program.Settings.SelectedProfile.Profile);
            PoeFlasks3.Program.Settings.ChangeSelectedProfile(Profiles_dropBox.SelectedIndex);
            FlasksGUIManager.onProfileChange?.Invoke(PoeFlasks3.Program.Settings.SelectedProfile.Profile);
            ProfileManager.SaveAllAsync();
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

        private void InitGuiElements()
        {
            FlasksGUIElements = new FlaskGUIElements[]
            {
                new()
                {
                    FlaskPanel = Flask1_panel,

                    ActType = Flask1_ActType_dropBox,

                    PercentRadioButton = Flask1_Percent_radioButton,
                    FlatRadioButton = Flask1_Flat_radioButton,
                    PercentValue = Flask1_Percent_numericUpDown,
                    FlatValue = Flask1_Flat_numericUpDown,

                    PauseEnable = Flask1_Pause_checkBox,
                    SecondKey = Flask1_SecondKey_dropBox,
                    PauseSecValue = Flask1_PauseSec_numericUpDown,

                    FlaskInGameHotkey = Flask1_InGameHotkey_dropBox,

                    FlaskMinimumKD = Flask1_MinCD_numericUpDown,

                    FlaskGroupBox = Flask1_group_dropBox,
                },
                new()
                {
                    FlaskPanel = Flask2_panel,

                    ActType = Flask2_ActType_dropBox,

                    PercentRadioButton = Flask2_Percent_radioButton,
                    FlatRadioButton = Flask2_Flat_radioButton,
                    PercentValue = Flask2_Percent_numericUpDown,
                    FlatValue = Flask2_Flat_numericUpDown,

                    PauseEnable = Flask2_Pause_checkBox,
                    SecondKey = Flask2_SecondKey_dropBox,
                    PauseSecValue = Flask2_PauseSec_numericUpDown,

                    FlaskInGameHotkey = Flask2_InGameHotkey_dropBox,

                    FlaskMinimumKD = Flask2_MinCD_numericUpDown,

                    FlaskGroupBox = Flask2_group_dropBox
                },
                new()
                {
                    FlaskPanel = Flask3_panel,

                    ActType = Flask3_ActType_dropBox,

                    PercentRadioButton = Flask3_Percent_radioButton,
                    FlatRadioButton = Flask3_Flat_radioButton,
                    PercentValue = Flask3_Percent_numericUpDown,
                    FlatValue = Flask3_Flat_numericUpDown,

                    PauseEnable = Flask3_Pause_checkBox,
                    SecondKey = Flask3_SecondKey_dropBox,
                    PauseSecValue = Flask3_PauseSec_numericUpDown,

                    FlaskInGameHotkey = Flask3_InGameHotkey_dropBox,

                    FlaskMinimumKD = Flask3_MinCD_numericUpDown,

                    FlaskGroupBox = Flask3_group_dropBox
                },
                new()
                {
                    FlaskPanel = Flask4_panel,

                    ActType = Flask4_ActType_dropBox,

                    PercentRadioButton = Flask4_Percent_radioButton,
                    FlatRadioButton = Flask4_Flat_radioButton,
                    PercentValue = Flask4_Percent_numericUpDown,
                    FlatValue = Flask4_Flat_numericUpDown,

                    PauseEnable = Flask4_Pause_checkBox,
                    SecondKey = Flask4_SecondKey_dropBox,
                    PauseSecValue = Flask4_PauseSec_numericUpDown,

                    FlaskInGameHotkey = Flask4_InGameHotkey_dropBox,

                    FlaskMinimumKD = Flask4_MinCD_numericUpDown,

                    FlaskGroupBox = Flask4_group_dropBox
                },
                new()
                {
                    FlaskPanel = Flask5_panel,

                    ActType = Flask5_ActType_dropBox,

                    PercentRadioButton = Flask5_Percent_radioButton,
                    FlatRadioButton = Flask5_Flat_radioButton,
                    PercentValue = Flask5_Percent_numericUpDown,
                    FlatValue = Flask5_Flat_numericUpDown,

                    PauseEnable = Flask5_Pause_checkBox,
                    SecondKey = Flask5_SecondKey_dropBox,
                    PauseSecValue = Flask5_PauseSec_numericUpDown,

                    FlaskInGameHotkey = Flask5_InGameHotkey_dropBox,

                    FlaskMinimumKD = Flask5_MinCD_numericUpDown,

                    FlaskGroupBox = Flask5_group_dropBox
                },
            };
        }
    }
}