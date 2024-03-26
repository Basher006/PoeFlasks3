using PoeFlasks3.BotLogic;
using PoeFlasks3.SettingsThings;
using Keys = BotFW_CvSharp_01.GlobalStructs.Keys;

namespace DrinkerForm
{
    internal static class FlasksGUIManager
    {
        public delegate void OnProfileChange(Profile profile);
        public static OnProfileChange onProfileChange;


        private static Profile FlasksSetup;
        private static Langueges AppLanguge;


        private static readonly Dictionary<string, Keys> FlaskInGameHotkey_dropBox_values = new Dictionary<string, Keys>()
        {
            { "1", Keys.NUM_1 }, { "2", Keys.NUM_2 }, { "3", Keys.NUM_3 }, { "4", Keys.NUM_4 }, { "5", Keys.NUM_5 },
            { "6", Keys.NUM_6 }, { "7", Keys.NUM_7 }, { "8", Keys.NUM_8 }, { "9", Keys.NUM_9 }, { "0", Keys.NUM_0 },
            { "Numpad 1", Keys.NUMPAD_1 }, { "Numpad 2", Keys.NUMPAD_2 }, { "Numpad 3", Keys.NUMPAD_3 },
            { "Numpad 4", Keys.NUMPAD_4 }, { "Numpad 5", Keys.NUMPAD_5 }, { "Numpad 6", Keys.NUMPAD_6 },
            { "Numpad 7", Keys.NUMPAD_7 }, { "Numpad 8", Keys.NUMPAD_8 }, { "Numpad 9", Keys.NUMPAD_9 },
            { "Numpad 0", Keys.NUMPAD_0 },
        };
        private static readonly Dictionary<string, Keys> SecondKey_dropBox_values = new Dictionary<string, Keys>()
        {
            { "Mouse Left", Keys.MouseLeft }, { "Mouse Right", Keys.MouseRight }, { "Mouse Middle", Keys.MouseMiddle },
            { "Q", Keys.Q }, { "W", Keys.W }, { "E", Keys.E }, { "R", Keys.R }, { "T", Keys.T },
            { "A", Keys.A }, { "S", Keys.S }, { "D", Keys.D }, { "F", Keys.F }, { "G", Keys.G },
            { "Z", Keys.Z }, { "X", Keys.X }, { "C", Keys.C }, { "V", Keys.V }, { "B", Keys.B },
            { "1", Keys.NUM_1 }, { "2", Keys.NUM_2 }, { "3", Keys.NUM_3 }, { "4", Keys.NUM_4 }, { "5", Keys.NUM_5 }, { "6", Keys.NUM_6 }, { "7", Keys.NUM_7 }, { "8", Keys.NUM_8 }, { "9", Keys.NUM_9 }, { "0", Keys.NUM_0 },
            { "Shift", Keys.Shift }, { "Left Alt", Keys.Alt_left }, { "Ctrl", Keys.Ctrl }, { "Space", Keys.Space }, { "Tab", Keys.Tab },
        };
        private static Dictionary<string, ActivationType> ActType_dropBox_values;
        private static Dictionary<string, FlaskGroup> FlaskGroups_dropBox_values;
        private static readonly Dictionary<PanelColors, Color> PanelColorsDict = new Dictionary<PanelColors, Color>()
        { 
            { PanelColors.Disable, Color.LightGray }, 
            { PanelColors.GroupNone, Color.FromArgb(226, 247, 236) },
            { PanelColors.Group1_active, Color.FromArgb(155, 215, 239) },
            { PanelColors.Group1_passive, Color.FromArgb(175, 203, 214) },
            { PanelColors.Group2_active, Color.FromArgb(255, 210, 132) },
            { PanelColors.Group2_passive, Color.FromArgb(229, 197, 142) },
        };


        private static FlaskGUIElements[] FlasksGuielements;
        private static AdditionalActionGUIElemets[] AdditionalActionGUIElemets;


        public static void Init(Profile profile, ref FlaskGUIElements[] guielements, ref AdditionalActionGUIElemets[] addActElements, ref ComboBox globalSecondKey)
        {
            FlasksSetup = profile;
            FlasksGuielements = guielements;
            AdditionalActionGUIElemets = addActElements;
            AppLanguge = PoeFlasks3.Program.Settings.AppLanguege;
            SetText();

            SetDropBoxesValuesRange(ref globalSecondKey);

            SetFlasksGuiValues();
            SetPanelColor();
            SetEnableFlags();

            SubscribeEvents();
        }

        public static void Update(Profile profile, ref FlaskGUIElements[] guielements, ref AdditionalActionGUIElemets[] addActElements)
        {
            FlasksSetup = profile;
            FlasksGuielements = guielements;
            AdditionalActionGUIElemets = addActElements;

            SetFlasksGuiValues();
            SetPanelColor();
            SetEnableFlags();
        }


        private static void SetText()
        {
            ActType_dropBox_values = new Dictionary<string, ActivationType>()
            {
                { BotResourseLoader.LanguegesText[AppLanguge][23], ActivationType.None }, { BotResourseLoader.LanguegesText[AppLanguge][24], ActivationType.HP },
                { BotResourseLoader.LanguegesText[AppLanguge][25], ActivationType.MP }, { BotResourseLoader.LanguegesText[AppLanguge][26], ActivationType.ES },
                { BotResourseLoader.LanguegesText[AppLanguge][27], ActivationType.CD }, { BotResourseLoader.LanguegesText[AppLanguge][28], ActivationType.OneTime }
            };

            FlaskGroups_dropBox_values = new Dictionary<string, FlaskGroup>()
            {
                { BotResourseLoader.LanguegesText[AppLanguge][37], FlaskGroup.None }, 
                { BotResourseLoader.LanguegesText[AppLanguge][38], FlaskGroup.Group1 }, 
                { BotResourseLoader.LanguegesText[AppLanguge][39], FlaskGroup.Group2 }
            };


            for (int i = 0; i < FlasksGuielements.Length; i++)
            {
                FlasksGuielements[i].PercentRadioButton.Text = BotResourseLoader.LanguegesText[AppLanguge][29];
                FlasksGuielements[i].FlatRadioButton.Text = BotResourseLoader.LanguegesText[AppLanguge][30];
                FlasksGuielements[i].PauseEnable.Text = BotResourseLoader.LanguegesText[AppLanguge][31];
                FlasksGuielements[i].PauseEnableText.Text = BotResourseLoader.LanguegesText[AppLanguge][32];
                FlasksGuielements[i].PauseSecText.Text = BotResourseLoader.LanguegesText[AppLanguge][33];
                FlasksGuielements[i].FlaskInGameHotkeyText.Text = BotResourseLoader.LanguegesText[AppLanguge][34];
                FlasksGuielements[i].FlaskMinimumCDText.Text = BotResourseLoader.LanguegesText[AppLanguge][35];
                FlasksGuielements[i].FlaskGroupText.Text = BotResourseLoader.LanguegesText[AppLanguge][36];
            }
        }

        private static void SetDropBoxesValuesRange(ref ComboBox globalSecondKey)
        {
            for (int i = 0; i < FlasksGuielements.Length; i++)
            {
                FlasksGuielements[i].ActType.Items.AddRange(ActType_dropBox_values.Keys.ToArray());
                FlasksGuielements[i].FlaskInGameHotkey.Items.AddRange(FlaskInGameHotkey_dropBox_values.Keys.ToArray());
                FlasksGuielements[i].SecondKey.Items.AddRange(SecondKey_dropBox_values.Keys.ToArray());

                FlasksGuielements[i].FlaskGroupBox.Items.AddRange(FlaskGroups_dropBox_values.Keys.ToArray());
                FlasksGuielements[i].FlaskGroupBox.DrawItem += DrawDropBoxItem;
                FlasksGuielements[i].FlaskGroupBox.DrawMode = DrawMode.OwnerDrawFixed;
            }

            globalSecondKey.Items.AddRange(SecondKey_dropBox_values.Keys.ToArray());
        }
        private static void SetFlasksGuiValues()
        {
            for (int i = 0; i < FlasksGuielements.Length; i++)
            {
                var basAction = FlasksSetup.Setup.FlasksList[i].BaseAction;
                // dropboxes
                int actTypeIndex = (int)FlasksSetup.Setup.FlasksList[i].BaseAction.ActType;
                FlasksGuielements[i].ActType.SelectedIndex = actTypeIndex;

                int inGameHotkeyIndex = FlaskInGameHotkey_dropBox_values.Values.ToList().IndexOf(FlasksSetup.Setup.FlasksList[i].BaseAction.HotKey);
                FlasksGuielements[i].FlaskInGameHotkey.SelectedIndex = inGameHotkeyIndex;

                //int secondKeyIndex = SecondKey_dropBox_values.Values.ToList().IndexOf(FlasksSetup.Setup.FlasksList[i].BaseAction.PauseWhenSecondKeyNotUsedRecently.Key);
                FlasksGuielements[i].SecondKey.SelectedIndex = GetSecondKeyIndex(FlasksSetup.Setup.FlasksList[i].BaseAction.PauseWhenSecondKeyNotUsedRecently.Key);

                int flaskGroupIndex = FlaskGroups_dropBox_values.Values.ToList().IndexOf(FlasksSetup.Setup.FlasksList[i].Group);
                FlasksGuielements[i].FlaskGroupBox.SelectedIndex = flaskGroupIndex;

                // percent/flat values
                FlasksGuielements[i].PercentRadioButton.Checked = basAction.UseActPercent;
                FlasksGuielements[i].FlatRadioButton.Checked = !basAction.UseActPercent;

                FlasksGuielements[i].PercentValue.Value = basAction.ActPercent;
                FlasksGuielements[i].FlatValue.Value = basAction.ActFlat;

                // pause when second key not send recently
                FlasksGuielements[i].PauseEnable.Checked = basAction.PauseWhenSecondKeyNotUsedRecently.Enable;
                FlasksGuielements[i].PauseSecValue.Value = (decimal)basAction.PauseWhenSecondKeyNotUsedRecently.PauseActivationDelay;

                // min CD
                FlasksGuielements[i].FlaskMinimumCD.Value = (decimal)basAction.MinCD;
            }
        }
        private static void SetPanelColor()
        {
            var flasksGroups = GetFlasksGroups();
            for (int i = 0; i < FlasksGuielements.Length; i++)
            {
                var flaskSlot = (FlaskSlot)i;
                var flaskGroup = FlasksSetup.Setup.Flasks[flaskSlot].Group;
                var flaskActType = FlasksSetup.Setup.Flasks[flaskSlot].BaseAction.ActType;
                bool flaskInGroup = flaskGroup != FlaskGroup.None;
                bool flaskIsFirstInGroup = true;
                if (flaskInGroup && flasksGroups[flaskGroup].Count > 1)
                {
                    flaskIsFirstInGroup = flasksGroups[flaskGroup].IndexOf(flaskSlot) == 0;
                }



                if (flaskInGroup == false)
                {
                    FlasksGuielements[i].FlaskPanel.BackColor = flaskActType == ActivationType.None ? PanelColorsDict[PanelColors.Disable] : PanelColorsDict[PanelColors.GroupNone];
                }
                else
                {
                    if (flaskIsFirstInGroup)
                        FlasksGuielements[i].FlaskPanel.BackColor = flaskGroup == FlaskGroup.Group1 ? PanelColorsDict[PanelColors.Group1_active] : PanelColorsDict[PanelColors.Group2_active];
                    else
                        FlasksGuielements[i].FlaskPanel.BackColor = flaskGroup == FlaskGroup.Group1 ? PanelColorsDict[PanelColors.Group1_passive] : PanelColorsDict[PanelColors.Group2_passive];
                }
            }
        }
        private static void SetEnableFlags()
        {
            // Flasks tab
            var flasksGroups = GetFlasksGroups();
            for (int i = 0; i < FlasksGuielements.Length; i++)
            {

                var flaskSlot = (FlaskSlot)i;
                var baseAction = FlasksSetup.Setup.Flasks[flaskSlot].BaseAction;
                var flaskGroup = FlasksSetup.Setup.Flasks[flaskSlot].Group;
                var flaskActType = baseAction.ActType;
                bool flaskInGroup = flaskGroup != FlaskGroup.None;
                bool flaskIsFirstInGroup = true;
                if (flaskInGroup && flasksGroups[flaskGroup].Count > 1)
                {
                    flaskIsFirstInGroup = flasksGroups[flaskGroup].IndexOf(flaskSlot) == 0;
                }


                if (flaskInGroup && !flaskIsFirstInGroup)
                {
                    // disable all w/o groups and ingame hotkey
                    FlasksGuielements[i].ActType.Enabled = false;

                    FlasksGuielements[i].PercentRadioButton.Enabled = false;
                    FlasksGuielements[i].FlatRadioButton.Enabled = false;
                    FlasksGuielements[i].PercentValue.Enabled = false;
                    FlasksGuielements[i].FlatValue.Enabled = false;

                    FlasksGuielements[i].PauseEnable.Enabled = false;
                    FlasksGuielements[i].PauseEnableText.Enabled = false;
                    FlasksGuielements[i].SecondKey.Enabled = false;
                    FlasksGuielements[i].PauseSecValue.Enabled = false;

                    FlasksGuielements[i].FlaskMinimumCD.Enabled = false;
                }
                else if (flaskActType == ActivationType.None)
                {
                    // disable all w/o act type, groups and ingame hotkey
                    FlasksGuielements[i].ActType.Enabled = true;
                    FlasksGuielements[i].PercentRadioButton.Enabled = false;

                    FlasksGuielements[i].FlatRadioButton.Enabled = false;
                    FlasksGuielements[i].PercentValue.Enabled = false;
                    FlasksGuielements[i].FlatValue.Enabled = false;

                    FlasksGuielements[i].PauseEnable.Enabled = false;
                    FlasksGuielements[i].PauseEnableText.Enabled = false;
                    FlasksGuielements[i].SecondKey.Enabled = false;
                    FlasksGuielements[i].PauseSecValue.Enabled = false;

                    FlasksGuielements[i].FlaskMinimumCD.Enabled = false;
                }
                else
                {
                    // standart enable thing
                    FlasksGuielements[i].ActType.Enabled = true;

                    if (flaskActType == ActivationType.HP || flaskActType == ActivationType.MP || flaskActType == ActivationType.ES)
                    {
                        FlasksGuielements[i].PercentRadioButton.Enabled = true;
                        FlasksGuielements[i].FlatRadioButton.Enabled = true;
                        FlasksGuielements[i].PercentValue.Enabled = baseAction.UseActPercent;
                        FlasksGuielements[i].FlatValue.Enabled = !baseAction.UseActPercent;
                    }
                    else
                    {
                        FlasksGuielements[i].PercentRadioButton.Enabled = false;
                        FlasksGuielements[i].FlatRadioButton.Enabled = false;
                        FlasksGuielements[i].PercentValue.Enabled = false;
                        FlasksGuielements[i].FlatValue.Enabled = false;
                    }

                    FlasksGuielements[i].PauseEnable.Enabled = true;
                    FlasksGuielements[i].PauseEnableText.Enabled = true;
                    FlasksGuielements[i].SecondKey.Enabled = FlasksGuielements[i].PauseEnable.Checked;
                    FlasksGuielements[i].PauseSecValue.Enabled = FlasksGuielements[i].PauseEnable.Checked;

                    FlasksGuielements[i].FlaskMinimumCD.Enabled = true;

                }
            }
        }

        private static void SubscribeEvents()
        {
            for (int i = 0; i < FlasksGuielements.Length; i++)
            {
                FlasksGuielements[i].ActType.SelectedValueChanged += OnActTypeChange;
                FlasksGuielements[i].FlaskGroupBox.SelectedIndexChanged += OnGroupChange;
                FlasksGuielements[i].PercentRadioButton.CheckedChanged += OnActPercentOrFlatChange;
                FlasksGuielements[i].PauseEnable.CheckedChanged += OnPauseEnableChanged;

                FlasksGuielements[i].PercentValue.ValueChanged += OnPercentValueChange;
                FlasksGuielements[i].FlatValue.ValueChanged += OnFlatValueChange;

                FlasksGuielements[i].SecondKey.SelectedIndexChanged += OnSecondKeyChanged;
                FlasksGuielements[i].PauseSecValue.ValueChanged += OnPauseDelayChanged;

                FlasksGuielements[i].FlaskMinimumCD.ValueChanged += OnMinCDChanged;
            }
        }
        private static void OnActTypeChange(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (ComboBox)sender;


            var actType = (ActivationType)s.SelectedIndex;
            FlaskSlot slot;
            for (int i = 1; i < 6; i++)
            {
                if (s.Name.Contains($"Flask{i}"))
                {
                    slot = (FlaskSlot)i - 1;

                    var flaskSlot = FlasksSetup.Setup.Flasks[slot];
                    var basAction = flaskSlot.BaseAction;
                    basAction.ActType = actType;
                    flaskSlot.BaseAction = basAction;
                    FlasksSetup.Setup.Flasks[slot] = flaskSlot;

                    break;
                }
            }

            SetPanelColor();
            SetEnableFlags();
            onProfileChange?.Invoke(FlasksSetup);
        }
        private static void OnGroupChange(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (ComboBox)sender;


            var group = (FlaskGroup)s.SelectedIndex;
            FlaskSlot slot;
            for (int i = 1; i < 6; i++)
            {
                if (s.Name.Contains($"Flask{i}"))
                {
                    slot = (FlaskSlot)i - 1;

                    var flaskSlot = FlasksSetup.Setup.Flasks[slot];
                    flaskSlot.Group = group;
                    FlasksSetup.Setup.Flasks[slot] = flaskSlot;

                    break;
                }
            }

            SetPanelColor();
            SetEnableFlags();
            onProfileChange?.Invoke(FlasksSetup);
        }
        private static void OnActPercentOrFlatChange(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (RadioButton)sender;


            FlaskSlot slot;
            for (int i = 1; i < 6; i++)
            {
                if (s.Name.Contains($"Flask{i}"))
                {
                    slot = (FlaskSlot)i - 1;

                    bool useActPercent = s.Checked;

                    var flaskSlot = FlasksSetup.Setup.Flasks[slot];
                    var basAction = flaskSlot.BaseAction;
                    basAction.UseActPercent = useActPercent;
                    flaskSlot.BaseAction = basAction;
                    FlasksSetup.Setup.Flasks[slot] = flaskSlot;

                    break;
                }
            }

            SetEnableFlags();
            onProfileChange?.Invoke(FlasksSetup);
        }
        private static void OnPauseEnableChanged(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (CheckBox)sender;


            FlaskSlot slot;
            for (int i = 1; i < 6; i++)
            {
                if (s.Name.Contains($"Flask{i}"))
                {
                    slot = (FlaskSlot)i - 1;

                    bool usePausa = s.Checked;

                    var flaskSlot = FlasksSetup.Setup.Flasks[slot];
                    var basAction = flaskSlot.BaseAction;
                    var pause = basAction.PauseWhenSecondKeyNotUsedRecently;
                    pause.Enable = usePausa;
                    basAction.PauseWhenSecondKeyNotUsedRecently = pause;
                    flaskSlot.BaseAction = basAction;
                    FlasksSetup.Setup.Flasks[slot] = flaskSlot;

                    break;
                }
            }

            SetEnableFlags();
            onProfileChange?.Invoke(FlasksSetup);
        }
        private static void OnPercentValueChange(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (NumericUpDown)sender;


            FlaskSlot slot;
            for (int i = 1; i < 6; i++)
            {
                if (s.Name.Contains($"Flask{i}"))
                {
                    slot = (FlaskSlot)i - 1;

                    int value = (int)s.Value;

                    var flaskSlot = FlasksSetup.Setup.Flasks[slot];
                    var basAction = flaskSlot.BaseAction;
                    basAction.ActPercent = value;
                    flaskSlot.BaseAction = basAction;
                    FlasksSetup.Setup.Flasks[slot] = flaskSlot;

                    break;
                }
            }
            onProfileChange?.Invoke(FlasksSetup);
        }
        private static void OnFlatValueChange(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (NumericUpDown)sender;


            FlaskSlot slot;
            for (int i = 1; i < 6; i++)
            {
                if (s.Name.Contains($"Flask{i}"))
                {
                    slot = (FlaskSlot)i - 1;

                    int value = (int)s.Value;

                    var flaskSlot = FlasksSetup.Setup.Flasks[slot];
                    var basAction = flaskSlot.BaseAction;
                    basAction.ActFlat = value;
                    flaskSlot.BaseAction = basAction;
                    FlasksSetup.Setup.Flasks[slot] = flaskSlot;

                    break;
                }
            }
            onProfileChange?.Invoke(FlasksSetup);
        }
        private static void OnSecondKeyChanged(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (ComboBox)sender;


            FlaskSlot slot;
            for (int i = 1; i < 6; i++)
            {
                if (s.Name.Contains($"Flask{i}"))
                {
                    slot = (FlaskSlot)i - 1;

                    int index = s.SelectedIndex;
                    //var newKey = SecondKey_dropBox_values.Values.ToList()[index];
                    var newKey = GetSecondKey(index);

                    var flaskSlot = FlasksSetup.Setup.Flasks[slot];
                    var basAction = flaskSlot.BaseAction;
                    var pause = basAction.PauseWhenSecondKeyNotUsedRecently;
                    pause.Key = newKey;
                    basAction.PauseWhenSecondKeyNotUsedRecently = pause;
                    flaskSlot.BaseAction = basAction;
                    FlasksSetup.Setup.Flasks[slot] = flaskSlot;

                    break;
                }
            }
            onProfileChange?.Invoke(FlasksSetup);
        }
        private static void OnPauseDelayChanged(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (NumericUpDown)sender;


            FlaskSlot slot;
            for (int i = 1; i < 6; i++)
            {
                if (s.Name.Contains($"Flask{i}"))
                {
                    slot = (FlaskSlot)i - 1;

                    float value = (float)s.Value;

                    var flaskSlot = FlasksSetup.Setup.Flasks[slot];
                    var basAction = flaskSlot.BaseAction;
                    var pause = basAction.PauseWhenSecondKeyNotUsedRecently;
                    pause.PauseActivationDelay = value;
                    basAction.PauseWhenSecondKeyNotUsedRecently = pause;
                    flaskSlot.BaseAction = basAction;
                    FlasksSetup.Setup.Flasks[slot] = flaskSlot;

                    break;
                }
            }
            onProfileChange?.Invoke(FlasksSetup);
        }
        private static void OnMinCDChanged(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (NumericUpDown)sender;


            FlaskSlot slot;
            for (int i = 1; i < 6; i++)
            {
                if (s.Name.Contains($"Flask{i}"))
                {
                    slot = (FlaskSlot)i - 1;

                    float value = (float)s.Value;

                    var flaskSlot = FlasksSetup.Setup.Flasks[slot];
                    var basAction = flaskSlot.BaseAction;
                    basAction.MinCD = value;
                    flaskSlot.BaseAction = basAction;
                    FlasksSetup.Setup.Flasks[slot] = flaskSlot;

                    break;
                }
            }
            onProfileChange?.Invoke(FlasksSetup);
        }


        private static Dictionary<FlaskGroup, List<FlaskSlot>> GetFlasksGroups()
        {
            var result = new Dictionary<FlaskGroup, List<FlaskSlot>>() { { FlaskGroup.None, new() }, { FlaskGroup.Group1, new() }, { FlaskGroup.Group2, new() }};
            var flasks = FlasksSetup.Setup.FlasksList;
            for (int i = 0; i < flasks.Count; i++)
            {
                result[flasks[i].Group].Add(flasks[i].Slot);
            }

            return result;
        }

        private static void DrawDropBoxItem(object sender, DrawItemEventArgs e)
        {
            List<Color> clrs = new() { PanelColorsDict[PanelColors.GroupNone], PanelColorsDict[PanelColors.Group1_active], PanelColorsDict[PanelColors.Group2_active] };
            if (e.Index == -1)
                return;
            using (Brush br = new SolidBrush(clrs[e.Index]))
            {
                e.Graphics.FillRectangle(br, e.Bounds);
                e.Graphics.DrawString(FlaskGroups_dropBox_values.Keys.ToList()[e.Index], e.Font, Brushes.Black, e.Bounds);
            }
        }

        public static int GetSecondKeyIndex(Keys k)
        {
            return SecondKey_dropBox_values.Values.ToList().IndexOf(k);
        }

        public static Keys GetSecondKey(int index)
        {
            return SecondKey_dropBox_values.Values.ToList()[index];
        }

        private enum PanelColors
        {
            Disable,
            GroupNone,
            Group1_active,
            Group1_passive,
            Group2_active,
            Group2_passive
        }
    }
}
