using BotFW_CvSharp_01;
using DrinkerForm;
using PoeFlasks3.BotLogic;
using PoeFlasks3.Settings;
using PoeFlasks3.SettingsThings;
using static PoeFlasks3.BotLogic.Bot;

namespace Drinker
{
    public partial class Form1 : Form
    {
        // set it to true for debug mode
        // (in debug mode: not need game client window, not send keys )
        //private const bool DEBUG = true;
        private const bool DEBUG = false;

        private Langueges AppLanguege; 

        FlasksSettingsFrom? SettingsForm;

        private Color GamePathColor_set = Color.Black;
        private Color GamePathColor_NotSet = Color.RoyalBlue;
        private string GamePathText_NotSet;
        private int GamePath_textMaxLenght = 25;
        private string FlaskStateText;

        private readonly Dictionary<BotState, Color> StartStopButtonCollors = new() 
        { { BotState.Stop, Color.IndianRed }, { BotState.Run, Color.YellowGreen }, { BotState.Pause, Color.RosyBrown } };
        private Dictionary<BotState, string> StartStopButtonText;
        private string PauseWithText_text;

        public Form1()
        {
            if (DEBUG)
                Log.Write("Run in debug mode!");

            InitializeComponent();

            // TODO:

            // BOT LOGIC:
            // full pause on settings form opening
            // not correct poe resolution when start before game
            // remove base action, replace it with IUsable
            // when flask in group empty bot wait min kd instead of use next flask (bug? or..?)

            // MAIN GUI FORM:


            // FLASKS SETUP GUI:
            // profile update spaghetti code refactoring
            // fix lags


            // OTHER:
            // low resolution not work

            // add buffer to log, and async save log
            // sort profiles by creation time


            PoeFlasks3.Program.Init(DEBUG);
            InitProfilesDropBox();
            ApplyAppSettings();
            SetLanguege();

            Bot.updateGUI += UpdateUPS;
            Bot.updateStartStopButton += ChangeStartStopButton;
            ChangeStartStopButton(BotState.Stop, null);

            Thread.Sleep(500);
            SubscribeEvents();
            //initIsComplite = true;
            if (Bot.Client.Window.IsFinded)
                Log.Write($"Detected game resolution: { Bot.Client.Window.WindowRect }");
            else
                Log.Write($"Dont find PoE game window!", Log.LogType.Warn);

            Log.Write("Init cpmplite!");
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
                FLasksState_lable.Text = $"{FlaskStateText}{flasksState}";
            }
            else
            {
                HP_lable.Text = "HP: N/A";
                MP_lable.Text = "MP: N/A";
                ES_lable.Text = "ES: N/A";
                FLasksState_lable.Text = $"{FlaskStateText}N/A";
            }

        }

        public void ChangeStartStopButton(BotState state, string? whyNotRun)
        {
            if (state == BotState.None)
                state = BotState.Stop;

            string startStopButtonText;
            if (!string.IsNullOrEmpty(whyNotRun))
            {
                if (state == BotState.Pause)
                    startStopButtonText = whyNotRun + PauseWithText_text;
                else
                    startStopButtonText = whyNotRun;
            }
            else
                startStopButtonText = StartStopButtonText[state];
            try
            {
                StartStop_button.Text = startStopButtonText;
            }
            catch (Exception)
            {
                Log.Write("Update Start/Stop button text Exeption!", Log.LogType.Warn);
            }

            StartStop_button.BackColor = StartStopButtonCollors[state];
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

        private void SetLanguege()
        {
            AppLanguege = PoeFlasks3.Program.Settings.AppLanguege;

            // apply text
            PauseEnable_chekbox.Text = BotResourseLoader.LanguegesText[AppLanguege][0];
            FlasksSettings_button.Text = BotResourseLoader.LanguegesText[AppLanguege][1];
            GamePath_label.Text = BotResourseLoader.LanguegesText[AppLanguege][2];
            GamePathText_NotSet = BotResourseLoader.LanguegesText[AppLanguege][2];
            FlaskStateText = BotResourseLoader.LanguegesText[AppLanguege][3];
            FLasksState_lable.Text = $"{FlaskStateText} N/A";
            StartStopButtonText = new()
            {
                { BotState.Stop, BotResourseLoader.LanguegesText[AppLanguege][4] },
                { BotState.Run, BotResourseLoader.LanguegesText[AppLanguege][5] },
                { BotState.Pause, BotResourseLoader.LanguegesText[AppLanguege][6] }
            };
            PauseWithText_text = BotResourseLoader.LanguegesText[AppLanguege][7];
        }

        private void SubscribeEvents()
        {
            PauseEnable_chekbox.CheckStateChanged += PauseEnable_chekbox_CheckStateChanged;
        }

        private void PauseEnable_chekbox_CheckStateChanged(object sender, EventArgs e)
        {
            Log.Write($"Enable pause in ho chekbox state shanged to: {PauseEnable_chekbox.Checked}");


            PoeFlasks3.Program.Settings.AppConfig.PauseInHo_checkboxState = PauseEnable_chekbox.Checked;
            PoeFlasks3.Program.Settings.AppConfig.Save();

            if (PauseEnable_chekbox.Checked)
            {
                TryGetPoeLogPath(out string poeLogPath);
                UpdatePoeLogPathInGUI(poeLogPath);
                Bot.OnPauseChange(PauseEnable_chekbox.Checked, poeLogPath);
                Log.Write($"PoE Log path changed to: {poeLogPath}");
            }
            else
                Bot.OnPauseChange(PauseEnable_chekbox.Checked, null);
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
                if (Bot.Client.TryGetPoeLogFolderFromRegistry(out poeLogPath))
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
            newLogPath = AskUserWherePoeLog_Dialog_show(initDirectory);
            bool isValide = AppSettings.IsValidePoePath(newLogPath);
            Log.Write($"User pick path: {newLogPath}. Is valide: {isValide}");
            return isValide;
        }

        private string AskUserWherePoeLog_Dialog_show(string initDirectory = "")
        {
            Log.Write("Open file dilaog..");
            string poeLogPath = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (!string.IsNullOrEmpty(initDirectory) && Directory.Exists(initDirectory))
                    openFileDialog.InitialDirectory = initDirectory;

                openFileDialog.Title = BotResourseLoader.LanguegesText[AppLanguege][8];
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

        private void GamePath_label_TextChanged(object sender, EventArgs e)
        {
            if (GamePath_label.Text.Length > GamePath_textMaxLenght + 2)
            {
                string newText = GamePath_label.Text.Substring(0, GamePath_textMaxLenght);
                newText += "..";
                GamePath_label.Text = newText;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Bot.StopLoop();
        }
    }
}
