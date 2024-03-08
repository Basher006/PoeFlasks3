using BotFW_CvSharp_01;
using DrinkerForm;
using PoeFlasks3.BotLogic;
using PoeFlasks3.Settings;
using PoeFlasks3.SettingsThings;

namespace Drinker
{
    public partial class Form1 : Form
    {
        // set it to true for debug mode
        private const bool DEBUG = true;

        private bool initIsComplite = false;

        FlasksSettingsFrom? SettingsForm;

        private Color GamePathColor_set = Color.Black;
        private Color GamePathColor_NotSet = Color.RoyalBlue;
        private string GamePathText_NotSet = "”казать путь к игре..";
        private int GamePath_textMaxLenght = 25;


        public Form1()
        {
            if (DEBUG)
                Log.Write("Run in debug mode");
            InitializeComponent();

            // TODO:

            // BOT LOGIC:
            // screen loop refactoring
            // screen loop chek refactoring
            // gui callbacks to bot logic

            // loop run/full pause/slowMode
            // full pause on settings form opening

            // MAIN GUI FORM:
            // fix crush on form closing (need stop bot loops)
            // start stop button colors and text

            // FLASKS SETUP GUI:
            // global pause when second key.. (!)
            // screen update button
            // additional actions


            // OTHER:
            // in game test
            // low resolution test
            // logging
            // BotKeyHook Send to bot logick all things on pause or not wor update button gui

            // sort profiles by creation time


            PoeFlasks3.Program.Init(DEBUG);
            InitProfilesDropBox();
            ApplyAppSettings();

            PoeFlasks3.BotLogic.Bot.updateGUI += UpdateUPS;

            Thread.Sleep(500);
            Log.Write("Init cpmplite!");
            SubscribeEvents();
            initIsComplite = true;
        }

        public void UpdateUPS(GrabedData? data, long ups)
        {
            // UPS
            UPS_lable.Text = $"UPS: {ups}";

            if (data != null)
            {
                // HP
                string hp = data.Value.FindedFlags.HP_isFind ? $"{data.Value.HP.Current} / {data.Value.HP.Max}" : "N/A";
                HP_lable.Text = $"HP: {hp}";

                // MP
                string mp = data.Value.FindedFlags.MP_isFind ? $"{data.Value.MP.Current} / {data.Value.MP.Max}" : "N/A";
                MP_lable.Text = $"MP: {mp}";

                //ES
                string es = data.Value.FindedFlags.ES_isFind ? $"{data.Value.ES.Current} / {data.Value.ES.Max}" : "N/A";
                ES_lable.Text = $"ES: {es}";

                // Flasks state
                string flasksState = $"" +
                    $"{(int)(data.Value.FlasksState.States[FlaskSlot.Slot1] * 100)}   " +
                    $"{(int)(data.Value.FlasksState.States[FlaskSlot.Slot2] * 100)}   " +
                    $"{(int)(data.Value.FlasksState.States[FlaskSlot.Slot3] * 100)}   " +
                    $"{(int)(data.Value.FlasksState.States[FlaskSlot.Slot4] * 100)}   " +
                    $"{(int)(data.Value.FlasksState.States[FlaskSlot.Slot5] * 100)}";
                FLasksState_lable.Text = $"Flasks state: {flasksState}";
            }
            else
            {
                HP_lable.Text = "HP: N/A";
                MP_lable.Text = "MP: N/A";
                ES_lable.Text = "ES: N/A";
                FLasksState_lable.Text = $"Flasks state: N/A";
            }

        }

        public void InitProfilesDropBox()
        {
            // add profiles to drop box
            Profile_dropBox.Items.Clear();
            Profile_dropBox.Items.AddRange(ProfileManager.ProfileNames.ToArray());
            Profile_dropBox.SelectedIndex = PoeFlasks3.Program.Settings.SelectedProfile.Index;
        }

        private void Profile_dropBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PoeFlasks3.Program.Settings.ChangeSelectedProfile(Profile_dropBox.SelectedIndex);
        }

        private void ApplyAppSettings()
        {
            Point newAppPos = new(PoeFlasks3.Program.Settings.AppConfig.AppPos_x, PoeFlasks3.Program.Settings.AppConfig.AppPos_y);
            Location = newAppPos;


            PauseEnable_chekbox.Checked = PoeFlasks3.Program.Settings.AppConfig.PauseInHo_checkboxState;
            PoeFlasks3.Program.Settings.AppConfig.Save();

            if (PauseEnable_chekbox.Checked)
            {
                TryGetPoeLogPath(out string poeLogPath);
                UpdatePoeLogPathInGUI(poeLogPath);
            }
            else
            {
                if (PoeFlasks3.Program.Settings.AppConfig.HaveValidePoeLogPath)
                {
                    GamePath_label.Text = PoeFlasks3.Program.Settings.AppConfig.PoeLogPath;
                    GamePath_label.ForeColor = GamePathColor_set;
                }
            }
        }

        private void PauseEnable_chekbox_CheckStateChanged(object sender, EventArgs e)
        {
            Log.Write($"Enable pause in ho chekbox state shanged to: {PauseEnable_chekbox.Checked}");
            Bot.OnPauseChange(PauseEnable_chekbox.Checked);

            PoeFlasks3.Program.Settings.AppConfig.PauseInHo_checkboxState = PauseEnable_chekbox.Checked;
            PoeFlasks3.Program.Settings.AppConfig.Save();

            if (PauseEnable_chekbox.Checked)
            {
                TryGetPoeLogPath(out string poeLogPath);
                UpdatePoeLogPathInGUI(poeLogPath);
                Log.Write($"PoE Log path changed to: {poeLogPath}");
            }
        }

        private bool TryGetPoeLogPath(out string poeLogPath)
        {
            Log.Write("Try get valide PoE Log path.");
            poeLogPath = "";
            if (PoeFlasks3.Program.Settings.AppConfig.HaveValidePoeLogPath)
            {
                poeLogPath = PoeFlasks3.Program.Settings.AppConfig.PoeLogPath;
                Log.Write("finded valide PoE Log path in App Settings!");
                return true;
            }
            else
            {
                if (PoeFlasks3.Program.poeClinet.TryGetPoeLogFolderFromRegistry(out poeLogPath))
                {
                    Log.Write("finded valide PoE Log path in Registry!");
                    if (AppSettings.IsValidePoePath(poeLogPath))
                    {
                        PoeFlasks3.Program.Settings.AppConfig.PoeLogPath = poeLogPath;
                        PoeFlasks3.Program.Settings.AppConfig.Save();
                        return true;
                    }
                }
            }

            Log.Write("Ask user where PoE Log file..");
            return AskUserWherePoeLog(out poeLogPath);
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            Log.Write($"App position changed to: x:{Location.X}, y:{Location.Y}");
            PoeFlasks3.Program.Settings.AppConfig.AppPos_x = Location.X;
            PoeFlasks3.Program.Settings.AppConfig.AppPos_y = Location.Y;

            PoeFlasks3.Program.Settings.AppConfig.Save();
        }

        private void FlasksSettings_button_Click(object sender, EventArgs e)
        {
            Log.Write("Open Settings button click!");
            SettingsForm = new();
            SettingsForm.ShowDialog();
        }

        private void GamePath_label_Click(object sender, EventArgs e)
        {
            Log.Write("Game path text click!");
            string newPath;
            if (PoeFlasks3.Program.Settings.AppConfig.HaveValidePoeLogPath)
            {
                string initDirectory = PoeFlasks3.Program.Settings.AppConfig.PoeLogPath.Replace(AppSettings.LocalSavePath, "");
                AskUserWherePoeLog(out newPath, initDirectory);
            }
            else
                AskUserWherePoeLog(out newPath);

            UpdatePoeLogPathInGUI(newPath);
        }

        private bool AskUserWherePoeLog(out string newLogPath, string initDirectory = "")
        {
            newLogPath = AskUserWherePoeLogDialog(initDirectory);
            bool isValide = AppSettings.IsValidePoePath(newLogPath);
            Log.Write($"User pick path: {newLogPath}. Is valide: {isValide}");
            return isValide;
        }

        private string AskUserWherePoeLogDialog(string initDirectory = "")
        {
            Log.Write("Open file dilaog..");
            string poeLogPath = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (!string.IsNullOrEmpty(initDirectory) && Directory.Exists(initDirectory))
                    openFileDialog.InitialDirectory = initDirectory;

                openFileDialog.Title = "”кажите путь к Client.txt в папке установки PoE";
                openFileDialog.Filter = "Client.txt (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    poeLogPath = openFileDialog.FileName;
                    Log.Write("User click \"OK\" in dialog..");
                }
                else
                    Log.Write("User Close dialog..");
            }

            return poeLogPath;
        }

        private void UpdatePoeLogPathInGUI(string poeLogPath)
        {
            if (AppSettings.IsValidePoePath(poeLogPath))
            {
                GamePath_label.Text = poeLogPath;
                GamePath_label.ForeColor = GamePathColor_set;
            }
            else
            {
                GamePath_label.Text = GamePathText_NotSet;
                GamePath_label.ForeColor = GamePathColor_NotSet;
                PauseEnable_chekbox.Checked = false;
            }

            PoeFlasks3.Program.Settings.AppConfig.PoeLogPath = poeLogPath;
            PoeFlasks3.Program.Settings.AppConfig.Save();
        }

        private void StartStop_button_Click(object sender, EventArgs e)
        {
            Log.Write("Start/Stop button click!");
            Bot.OnStartStopChange();
        }

        private void ChangeStartStopButton()
        {

        }

        private void GamePath_label_TextChanged(object sender, EventArgs e)
        {
            if (GamePath_label.Text.Length > GamePath_textMaxLenght + 2)
            {
                string newText = GamePath_label.Text.Substring(0, GamePath_textMaxLenght);
                newText += "..";
                GamePath_label.Text = newText;
            }
        }

        private void SubscribeEvents()
        {
            PauseEnable_chekbox.CheckStateChanged += PauseEnable_chekbox_CheckStateChanged;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Bot.StopLoop();
        }
    }
}
