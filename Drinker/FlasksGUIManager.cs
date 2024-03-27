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
        private static readonly Dictionary<string, Keys> AddActInGameHotkey_dropBox_values = new Dictionary<string, Keys>()
        {
            { "Q", Keys.Q }, { "W", Keys.W }, { "E", Keys.E }, { "R", Keys.R }, { "T", Keys.T }, { "A", Keys.A }, { "S", Keys.S }, { "D", Keys.D }, { "F", Keys.F },
            { "Z", Keys.Z }, { "X", Keys.X }, { "C", Keys.C }, { "V", Keys.V }, { "Space", Keys.Space }, { "Mouse middle", Keys.MouseMiddle }, { "Mouse left", Keys.MouseLeft }, { "Mouse right", Keys.MouseRight },
            { "1", Keys.NUM_1 }, { "2", Keys.NUM_2 }, { "3", Keys.NUM_3 }, { "4", Keys.NUM_4 }, { "5", Keys.NUM_5 },
            { "6", Keys.NUM_6 }, { "7", Keys.NUM_7 }, { "8", Keys.NUM_8 }, { "9", Keys.NUM_9 }, { "0", Keys.NUM_0 },
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
        private static Dictionary<string, ActivationType> ActType_dropBox_values_flask;
        private static Dictionary<string, ActivationType> ActType_dropBox_values_addAct;
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

            SetGuiValues();
            SetPanelColor_flask();
            SetPanelColor_addAct();
            SetEnableFlags_flask();
            SetEnableFlags_addAct();

            SubscribeEvents();
        }

        public static void Update(Profile profile, ref FlaskGUIElements[] guielements, ref AdditionalActionGUIElemets[] addActElements)
        {
            FlasksSetup = profile;
            FlasksGuielements = guielements;
            AdditionalActionGUIElemets = addActElements;

            SetGuiValues();
            SetPanelColor_flask();
            SetPanelColor_addAct();
            SetEnableFlags_flask();
            SetEnableFlags_addAct();
        }


        private static void SetText()
        {
            ActType_dropBox_values_flask = new Dictionary<string, ActivationType>()
            {
                { BotResourseLoader.LanguegesText[AppLanguge][23], ActivationType.None }, { BotResourseLoader.LanguegesText[AppLanguge][24], ActivationType.HP },
                { BotResourseLoader.LanguegesText[AppLanguge][25], ActivationType.MP }, { BotResourseLoader.LanguegesText[AppLanguge][26], ActivationType.ES },
                { BotResourseLoader.LanguegesText[AppLanguge][27], ActivationType.CD }, { BotResourseLoader.LanguegesText[AppLanguge][28], ActivationType.OneTime }
            };

            ActType_dropBox_values_addAct = new Dictionary<string, ActivationType>()
            {
                { BotResourseLoader.LanguegesText[AppLanguge][23], ActivationType.None }, { BotResourseLoader.LanguegesText[AppLanguge][24], ActivationType.HP },
                { BotResourseLoader.LanguegesText[AppLanguge][25], ActivationType.MP }, { BotResourseLoader.LanguegesText[AppLanguge][26], ActivationType.ES },
                { BotResourseLoader.LanguegesText[AppLanguge][27], ActivationType.CD }
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

            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                AdditionalActionGUIElemets[i].PercentRadioButton.Text = BotResourseLoader.LanguegesText[AppLanguge][29];
                AdditionalActionGUIElemets[i].FlatRadioButton.Text = BotResourseLoader.LanguegesText[AppLanguge][30];
                AdditionalActionGUIElemets[i].PauseEnable.Text = BotResourseLoader.LanguegesText[AppLanguge][31];
                AdditionalActionGUIElemets[i].PauseEnableText.Text = BotResourseLoader.LanguegesText[AppLanguge][32];
                AdditionalActionGUIElemets[i].PauseSecText.Text = BotResourseLoader.LanguegesText[AppLanguge][33];
                AdditionalActionGUIElemets[i].AddActInGameHotkeyText.Text = BotResourseLoader.LanguegesText[AppLanguge][46];
                AdditionalActionGUIElemets[i].AddActMinimumCDText.Text = BotResourseLoader.LanguegesText[AppLanguge][35];
            }
        }

        private static void SetDropBoxesValuesRange(ref ComboBox globalSecondKey)
        {
            for (int i = 0; i < FlasksGuielements.Length; i++)
            {
                FlasksGuielements[i].ActType.Items.AddRange(ActType_dropBox_values_flask.Keys.ToArray());
                FlasksGuielements[i].FlaskInGameHotkey.Items.AddRange(FlaskInGameHotkey_dropBox_values.Keys.ToArray());
                FlasksGuielements[i].SecondKey.Items.AddRange(SecondKey_dropBox_values.Keys.ToArray());

                FlasksGuielements[i].FlaskGroupBox.Items.AddRange(FlaskGroups_dropBox_values.Keys.ToArray());
                FlasksGuielements[i].FlaskGroupBox.DrawItem += DrawDropBoxItem;
                FlasksGuielements[i].FlaskGroupBox.DrawMode = DrawMode.OwnerDrawFixed;
            }

            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                AdditionalActionGUIElemets[i].ActType.Items.AddRange(ActType_dropBox_values_addAct.Keys.ToArray());
                AdditionalActionGUIElemets[i].AddActInGameHotkey.Items.AddRange(AddActInGameHotkey_dropBox_values.Keys.ToArray());
                AdditionalActionGUIElemets[i].SecondKey.Items.AddRange(SecondKey_dropBox_values.Keys.ToArray());
            }

            globalSecondKey.Items.AddRange(SecondKey_dropBox_values.Keys.ToArray());
        }
        private static void SetGuiValues()
        {
            for (int i = 0; i < FlasksGuielements.Length; i++)
            {
                var basAction = FlasksSetup.Setup.FlasksList[i].BaseAction;
                // act type
                FlasksGuielements[i].ActType.SelectedIndex = (int)basAction.ActType;

                // percent/flat 
                FlasksGuielements[i].PercentRadioButton.Checked = basAction.UseActPercent;
                FlasksGuielements[i].FlatRadioButton.Checked = !basAction.UseActPercent;
                FlasksGuielements[i].PercentValue.Value = basAction.ActPercent;
                FlasksGuielements[i].FlatValue.Value = basAction.ActFlat;

                // pause when second key not send recently
                FlasksGuielements[i].PauseEnable.Checked = basAction.PauseWhenSecondKeyNotUsedRecently.Enable;
                FlasksGuielements[i].SecondKey.SelectedIndex = GetSecondKeyIndex(basAction.PauseWhenSecondKeyNotUsedRecently.Key);
                FlasksGuielements[i].PauseSecValue.Value = (decimal)basAction.PauseWhenSecondKeyNotUsedRecently.PauseActivationDelay;

                // in game hotkey
                int inGameHotkeyIndex = FlaskInGameHotkey_dropBox_values.Values.ToList().IndexOf(basAction.HotKey);
                FlasksGuielements[i].FlaskInGameHotkey.SelectedIndex = inGameHotkeyIndex;

                // min CD
                FlasksGuielements[i].FlaskMinimumCD.Value = (decimal)basAction.MinCD;

                // flask group
                int flaskGroupIndex = FlaskGroups_dropBox_values.Values.ToList().IndexOf(FlasksSetup.Setup.FlasksList[i].Group);
                FlasksGuielements[i].FlaskGroupBox.SelectedIndex = flaskGroupIndex;
            }

            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                var basAction = FlasksSetup.Setup.AdditionalActions[i];

                // act type
                AdditionalActionGUIElemets[i].ActType.SelectedIndex = (int)basAction.ActType;

                // percent/flat 
                AdditionalActionGUIElemets[i].PercentRadioButton.Checked = basAction.UseActPercent;
                AdditionalActionGUIElemets[i].FlatRadioButton.Checked = !basAction.UseActPercent;
                AdditionalActionGUIElemets[i].PercentValue.Value = basAction.ActPercent;
                AdditionalActionGUIElemets[i].FlatValue.Value = basAction.ActFlat;

                // pause when second key not send recently
                AdditionalActionGUIElemets[i].PauseEnable.Checked = basAction.PauseWhenSecondKeyNotUsedRecently.Enable;
                AdditionalActionGUIElemets[i].SecondKey.SelectedIndex = GetSecondKeyIndex(basAction.PauseWhenSecondKeyNotUsedRecently.Key);
                AdditionalActionGUIElemets[i].PauseSecValue.Value = (decimal)basAction.PauseWhenSecondKeyNotUsedRecently.PauseActivationDelay;

                // in game hotkey
                int inGameHotkeyIndex = AddActInGameHotkey_dropBox_values.Values.ToList().IndexOf(basAction.HotKey);
                AdditionalActionGUIElemets[i].AddActInGameHotkey.SelectedIndex = inGameHotkeyIndex;

                // min CD
                AdditionalActionGUIElemets[i].AddActMinimumCD.Value = (decimal)basAction.MinCD;
            }
        }
        private static void SetPanelColor_flask()
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
        private static void SetPanelColor_addAct()
        {
            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                if (AdditionalActionGUIElemets[i].ActType.SelectedIndex == 0)
                    AdditionalActionGUIElemets[i].AddActPanel.BackColor = PanelColorsDict[PanelColors.Disable];
                else
                    AdditionalActionGUIElemets[i].AddActPanel.BackColor = PanelColorsDict[PanelColors.Group1_active];
            }
        }
        private static void SetEnableFlags_flask()
        {
            // Flasks tab
            var flasksGroups = GetFlasksGroups();
            for (int i = 0; i < FlasksGuielements.Length; i++)
            {

                var flaskSlot = (FlaskSlot)i;
                var baseAction = FlasksSetup.Setup.Flasks[flaskSlot].BaseAction;
                var flaskGroup = FlasksSetup.Setup.Flasks[flaskSlot].Group;
                var ActType = baseAction.ActType;
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
                else if (ActType == ActivationType.None)
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

                    if (ActType == ActivationType.HP || ActType == ActivationType.MP || ActType == ActivationType.ES)
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
        private static void SetEnableFlags_addAct()
        {
            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                var baseAction = FlasksSetup.Setup.AdditionalActions[i];
                var ActType = baseAction.ActType;

                if (ActType == ActivationType.None)
                {
                    // disable all w/o act type, groups and ingame hotkey
                    AdditionalActionGUIElemets[i].ActType.Enabled = true;
                    AdditionalActionGUIElemets[i].PercentRadioButton.Enabled = false;

                    AdditionalActionGUIElemets[i].FlatRadioButton.Enabled = false;
                    AdditionalActionGUIElemets[i].PercentValue.Enabled = false;
                    AdditionalActionGUIElemets[i].FlatValue.Enabled = false;

                    AdditionalActionGUIElemets[i].PauseEnable.Enabled = false;
                    AdditionalActionGUIElemets[i].PauseEnableText.Enabled = false;
                    AdditionalActionGUIElemets[i].SecondKey.Enabled = false;
                    AdditionalActionGUIElemets[i].PauseSecValue.Enabled = false;

                    AdditionalActionGUIElemets[i].AddActMinimumCD.Enabled = false;
                }
                else
                {
                    // standart enable thing
                    AdditionalActionGUIElemets[i].ActType.Enabled = true;

                    if (ActType == ActivationType.HP || ActType == ActivationType.MP || ActType == ActivationType.ES)
                    {
                        AdditionalActionGUIElemets[i].PercentRadioButton.Enabled = true;
                        AdditionalActionGUIElemets[i].FlatRadioButton.Enabled = true;
                        AdditionalActionGUIElemets[i].PercentValue.Enabled = baseAction.UseActPercent;
                        AdditionalActionGUIElemets[i].FlatValue.Enabled = !baseAction.UseActPercent;
                    }
                    else
                    {
                        AdditionalActionGUIElemets[i].PercentRadioButton.Enabled = false;
                        AdditionalActionGUIElemets[i].FlatRadioButton.Enabled = false;
                        AdditionalActionGUIElemets[i].PercentValue.Enabled = false;
                        AdditionalActionGUIElemets[i].FlatValue.Enabled = false;
                    }

                    AdditionalActionGUIElemets[i].PauseEnable.Enabled = true;
                    AdditionalActionGUIElemets[i].PauseEnableText.Enabled = true;
                    AdditionalActionGUIElemets[i].SecondKey.Enabled = FlasksGuielements[i].PauseEnable.Checked;
                    AdditionalActionGUIElemets[i].PauseSecValue.Enabled = FlasksGuielements[i].PauseEnable.Checked;

                    AdditionalActionGUIElemets[i].AddActMinimumCD.Enabled = true;
                }
            }
        }
        private static void SubscribeEvents()
        {
            for (int i = 0; i < FlasksGuielements.Length; i++)
            {
                FlasksGuielements[i].ActType.SelectedValueChanged += OnActTypeChange_flask;
                FlasksGuielements[i].PercentRadioButton.CheckedChanged += OnActPercentOrFlatChange_flask;
                FlasksGuielements[i].PauseEnable.CheckedChanged += OnPauseEnableChanged_flask;

                FlasksGuielements[i].PercentValue.ValueChanged += OnPercentValueChange_flask;
                FlasksGuielements[i].FlatValue.ValueChanged += OnFlatValueChange_flask;

                FlasksGuielements[i].SecondKey.SelectedIndexChanged += OnSecondKeyChanged_flask;
                FlasksGuielements[i].PauseSecValue.ValueChanged += OnPauseDelayChanged_flask;

                FlasksGuielements[i].FlaskInGameHotkey.SelectedIndexChanged += OnInGameHotkeyChanged_flask;

                FlasksGuielements[i].FlaskMinimumCD.ValueChanged += OnMinCDChanged_flask;

                FlasksGuielements[i].FlaskGroupBox.SelectedIndexChanged += OnGroupChange;
            }

            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                AdditionalActionGUIElemets[i].ActType.SelectedValueChanged += OnActTypeChange_addAct;
                AdditionalActionGUIElemets[i].PercentRadioButton.CheckedChanged += OnActPercentOrFlatChange_addAct;
                AdditionalActionGUIElemets[i].PauseEnable.CheckedChanged += OnPauseEnableChanged_addAct;

                AdditionalActionGUIElemets[i].PercentValue.ValueChanged += OnPercentValueChange_addAct;
                AdditionalActionGUIElemets[i].FlatValue.ValueChanged += OnFlatValueChange_addAct;

                AdditionalActionGUIElemets[i].SecondKey.SelectedIndexChanged += OnSecondKeyChanged_addAct;
                AdditionalActionGUIElemets[i].PauseSecValue.ValueChanged += OnPauseDelayChanged_addAct;

                AdditionalActionGUIElemets[i].AddActInGameHotkey.SelectedIndexChanged += OnInGameHotkeyChanged_addAct;

                AdditionalActionGUIElemets[i].AddActMinimumCD.ValueChanged += OnMinCDChanged_addAct;
            }
        }

        private static void OnActTypeChange_flask(object? sender, EventArgs e)
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

                    SetPanelColor_flask();
                    SetEnableFlags_flask();
                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnActTypeChange_addAct(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (ComboBox)sender;


            var actType = (ActivationType)s.SelectedIndex;
            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                if (s.Name.Contains($"AddAct{i + 1}"))
                {
                    var basAction = FlasksSetup.Setup.AdditionalActions[i];
                    basAction.ActType = actType;
                    FlasksSetup.Setup.AdditionalActions[i] = basAction;

                    SetPanelColor_addAct();
                    SetEnableFlags_addAct();
                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnActPercentOrFlatChange_flask(object? sender, EventArgs e)
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

                    SetEnableFlags_flask();
                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnActPercentOrFlatChange_addAct(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (RadioButton)sender;

            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                if (s.Name.Contains($"AddAct{i + 1}"))
                {
                    bool useActPercent = s.Checked;
                    var basAction = FlasksSetup.Setup.AdditionalActions[i];
                    basAction.UseActPercent = useActPercent;
                    FlasksSetup.Setup.AdditionalActions[i] = basAction;

                    SetEnableFlags_flask();
                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnPauseEnableChanged_flask(object? sender, EventArgs e)
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

                    SetEnableFlags_flask();
                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnPauseEnableChanged_addAct(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (CheckBox)sender;


            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                if (s.Name.Contains($"AddAct{i + 1}"))
                {
                    bool usePausa = s.Checked;

                    var basAction = FlasksSetup.Setup.AdditionalActions[i];
                    var pause = basAction.PauseWhenSecondKeyNotUsedRecently;
                    pause.Enable = usePausa;
                    basAction.PauseWhenSecondKeyNotUsedRecently = pause;
                    FlasksSetup.Setup.AdditionalActions[i] = basAction;

                    SetEnableFlags_flask();
                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnPercentValueChange_flask(object? sender, EventArgs e)
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

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnPercentValueChange_addAct(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (NumericUpDown)sender;

            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                if (s.Name.Contains($"AddAct{i + 1}"))
                {
                    int value = (int)s.Value;

                    var basAction = FlasksSetup.Setup.AdditionalActions[i];
                    basAction.ActPercent = value;
                    FlasksSetup.Setup.AdditionalActions[i] = basAction;

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnFlatValueChange_flask(object? sender, EventArgs e)
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

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnFlatValueChange_addAct(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (NumericUpDown)sender;

            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                if (s.Name.Contains($"AddAct{i + 1}"))
                {
                    int value = (int)s.Value;

                    var basAction = FlasksSetup.Setup.AdditionalActions[i];
                    basAction.ActFlat = value;
                    FlasksSetup.Setup.AdditionalActions[i] = basAction;

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnSecondKeyChanged_flask(object? sender, EventArgs e)
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

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnSecondKeyChanged_addAct(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (ComboBox)sender;


            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                if (s.Name.Contains($"AddAct{i + 1}"))
                {
                    int index = s.SelectedIndex;
                    var newKey = GetSecondKey(index);

                    var basAction = FlasksSetup.Setup.AdditionalActions[i];
                    var pause = basAction.PauseWhenSecondKeyNotUsedRecently;
                    pause.Key = newKey;
                    basAction.PauseWhenSecondKeyNotUsedRecently = pause;
                    FlasksSetup.Setup.AdditionalActions[i] = basAction;

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnPauseDelayChanged_flask(object? sender, EventArgs e)
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

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnPauseDelayChanged_addAct(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (NumericUpDown)sender;

            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                if (s.Name.Contains($"AddAct{i + 1}"))
                {
                    float value = (float)s.Value;

                    var basAction = FlasksSetup.Setup.AdditionalActions[i];
                    var pause = basAction.PauseWhenSecondKeyNotUsedRecently;
                    pause.PauseActivationDelay = value;
                    basAction.PauseWhenSecondKeyNotUsedRecently = pause;
                    FlasksSetup.Setup.AdditionalActions[i] = basAction;

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnInGameHotkeyChanged_flask(object? sender, EventArgs e)
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
                    var hotkey = FlaskInGameHotkey_dropBox_values.Values.ToList()[index];

                    var flaskSlot = FlasksSetup.Setup.Flasks[slot];
                    var basAction = flaskSlot.BaseAction;
                    basAction.HotKey = hotkey;
                    flaskSlot.BaseAction = basAction;
                    FlasksSetup.Setup.Flasks[slot] = flaskSlot;

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnInGameHotkeyChanged_addAct(object? sender, EventArgs e)
        {
            if (sender == null)
                return;

            var s = (ComboBox)sender;

            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                if (s.Name.Contains($"AddAct{i + 1}"))
                {
                    int index = s.SelectedIndex;
                    var hotkey = AddActInGameHotkey_dropBox_values.Values.ToList()[index];

                    var basAction = FlasksSetup.Setup.AdditionalActions[i];
                    basAction.HotKey = hotkey;
                    FlasksSetup.Setup.AdditionalActions[i] = basAction;

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnMinCDChanged_flask(object? sender, EventArgs e)
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

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
        }
        private static void OnMinCDChanged_addAct(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            var s = (NumericUpDown)sender;

            for (int i = 0; i < AdditionalActionGUIElemets.Length; i++)
            {
                if (s.Name.Contains($"AddAct{i + 1}"))
                {
                    float value = (float)s.Value;

                    var basAction = FlasksSetup.Setup.AdditionalActions[i];
                    basAction.MinCD = value;
                    FlasksSetup.Setup.AdditionalActions[i] = basAction;

                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
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

                    SetPanelColor_flask();
                    SetEnableFlags_flask();
                    onProfileChange?.Invoke(FlasksSetup);
                    return;
                }
            }
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
