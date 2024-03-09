using PoeFlasks3.SettingsThings;
using static DrinkerForm.FlasksSettingsFrom;
using Keys = BotFW_CvSharp_01.GlobalStructs.Keys;

namespace DrinkerForm
{
    internal static class FlasksGUIManager
    {
        public delegate void OnProfileChange(Profile profile);
        public static OnProfileChange onProfileChange;


        private static Profile FlasksSetup;


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

        private static readonly Dictionary<string, ActivationType> ActType_dropBox_values = new Dictionary<string, ActivationType>()
        {
            { "None", ActivationType.None }, { "HP", ActivationType.HP },
            { "Mana", ActivationType.MP }, { "ES", ActivationType.ES },
            { "CD", ActivationType.CD }, { "One time", ActivationType.OneTime }
        };

        private static readonly Dictionary<string, FlaskGroup> FlaskGroups_dropBox_values = new Dictionary<string, FlaskGroup>()
        {
            { "None", FlaskGroup.None }, { "Group1", FlaskGroup.Group1 }, { "Group2", FlaskGroup.Group2 }
        };

        private static readonly Dictionary<PanelColors, Color> PanelColorsDict = new Dictionary<PanelColors, Color>()
        { 
            { PanelColors.Disable, Color.LightGray }, 
            { PanelColors.GroupNone, Color.FromArgb(226, 247, 236) },
            { PanelColors.Group1_active, Color.FromArgb(155, 215, 239) },
            { PanelColors.Group1_passive, Color.FromArgb(175, 203, 214) },
            { PanelColors.Group2_active, Color.FromArgb(255, 210, 132) },
            { PanelColors.Group2_passive, Color.FromArgb(229, 197, 142) },
        };


        private static FlaskGUIElements[] Guielements;


        public static void Init(Profile profile, ref FlaskGUIElements[] guielements, ref ComboBox globalSecondKey)
        {
            FlasksSetup = profile;

            SetDropBoxesValuesRange(ref guielements, ref globalSecondKey);

            SetFlasksGuiValues(ref guielements);
            SetPanelColor(ref guielements);
            SetEnableFlags(ref guielements);

            SubscribeEvents(ref guielements);

            Guielements = guielements;
        }

        public static void Update(Profile profile, ref FlaskGUIElements[] guielements)
        {
            FlasksSetup = profile;

            SetFlasksGuiValues(ref guielements);
            SetPanelColor(ref guielements);
            SetEnableFlags(ref guielements);
        }


        private static void SetDropBoxesValuesRange(ref FlaskGUIElements[] guielements, ref ComboBox globalSecondKey)
        {
            for (int i = 0; i < guielements.Length; i++)
            {
                guielements[i].ActType.Items.AddRange(ActType_dropBox_values.Keys.ToArray());
                guielements[i].FlaskInGameHotkey.Items.AddRange(FlaskInGameHotkey_dropBox_values.Keys.ToArray());
                guielements[i].SecondKey.Items.AddRange(SecondKey_dropBox_values.Keys.ToArray());

                guielements[i].FlaskGroupBox.Items.AddRange(FlaskGroups_dropBox_values.Keys.ToArray());
                guielements[i].FlaskGroupBox.DrawItem += DrawDropBoxItem;
                guielements[i].FlaskGroupBox.DrawMode = DrawMode.OwnerDrawFixed;
            }

            globalSecondKey.Items.AddRange(SecondKey_dropBox_values.Keys.ToArray());
        }
        private static void SetFlasksGuiValues(ref FlaskGUIElements[] guielements)
        {
            for (int i = 0; i < guielements.Length; i++)
            {
                var basAction = FlasksSetup.Setup.FlasksList[i].BaseAction;
                // dropboxes
                int actTypeIndex = (int)FlasksSetup.Setup.FlasksList[i].BaseAction.ActType;
                guielements[i].ActType.SelectedIndex = actTypeIndex;

                int inGameHotkeyIndex = FlaskInGameHotkey_dropBox_values.Values.ToList().IndexOf(FlasksSetup.Setup.FlasksList[i].BaseAction.HotKey);
                guielements[i].FlaskInGameHotkey.SelectedIndex = inGameHotkeyIndex;

                //int secondKeyIndex = SecondKey_dropBox_values.Values.ToList().IndexOf(FlasksSetup.Setup.FlasksList[i].BaseAction.PauseWhenSecondKeyNotUsedRecently.Key);
                guielements[i].SecondKey.SelectedIndex = GetSecondKeyIndex(FlasksSetup.Setup.FlasksList[i].BaseAction.PauseWhenSecondKeyNotUsedRecently.Key);

                int flaskGroupIndex = FlaskGroups_dropBox_values.Values.ToList().IndexOf(FlasksSetup.Setup.FlasksList[i].Group);
                guielements[i].FlaskGroupBox.SelectedIndex = flaskGroupIndex;

                // percent/flat values
                guielements[i].PercentRadioButton.Checked = basAction.UseActPercent;
                guielements[i].FlatRadioButton.Checked = !basAction.UseActPercent;

                guielements[i].PercentValue.Value = basAction.ActPercent;
                guielements[i].FlatValue.Value = basAction.ActFlat;

                // pause when second key not send recently
                guielements[i].PauseEnable.Checked = basAction.PauseWhenSecondKeyNotUsedRecently.Enable;
                guielements[i].PauseSecValue.Value = (decimal)basAction.PauseWhenSecondKeyNotUsedRecently.PauseActivationDelay;

                // min CD
                guielements[i].FlaskMinimumKD.Value = (decimal)basAction.MinCD;
            }
        }
        private static void SetPanelColor(ref FlaskGUIElements[] guielements)
        {
            var flasksGroups = GetFlasksGroups();
            for (int i = 0; i < guielements.Length; i++)
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
                    guielements[i].FlaskPanel.BackColor = flaskActType == ActivationType.None ? PanelColorsDict[PanelColors.Disable] : PanelColorsDict[PanelColors.GroupNone];
                }
                else
                {
                    if (flaskIsFirstInGroup)
                        guielements[i].FlaskPanel.BackColor = flaskGroup == FlaskGroup.Group1 ? PanelColorsDict[PanelColors.Group1_active] : PanelColorsDict[PanelColors.Group2_active];
                    else
                        guielements[i].FlaskPanel.BackColor = flaskGroup == FlaskGroup.Group1 ? PanelColorsDict[PanelColors.Group1_passive] : PanelColorsDict[PanelColors.Group2_passive];
                }
            }
        }
        private static void SetEnableFlags(ref FlaskGUIElements[] guielements)
        {
            var flasksGroups = GetFlasksGroups();
            for (int i = 0; i < guielements.Length; i++)
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
                    // disable all w/o groups
                    guielements[i].ActType.Enabled = false;

                    guielements[i].PercentRadioButton.Enabled = false;
                    guielements[i].FlatRadioButton.Enabled = false;
                    guielements[i].PercentValue.Enabled = false;
                    guielements[i].FlatValue.Enabled = false;

                    guielements[i].PauseEnable.Enabled = false;
                    guielements[i].SecondKey.Enabled = false;
                    guielements[i].PauseSecValue.Enabled = false;

                    guielements[i].FlaskMinimumKD.Enabled = false;
                }
                else if (flaskActType == ActivationType.None)
                {
                    // disable all w/o act type and groups
                    guielements[i].ActType.Enabled = true;
                    guielements[i].PercentRadioButton.Enabled = false;

                    guielements[i].FlatRadioButton.Enabled = false;
                    guielements[i].PercentValue.Enabled = false;
                    guielements[i].FlatValue.Enabled = false;

                    guielements[i].PauseEnable.Enabled = false;
                    guielements[i].SecondKey.Enabled = false;
                    guielements[i].PauseSecValue.Enabled = false;

                    guielements[i].FlaskMinimumKD.Enabled = false;
                }
                else
                {
                    // standart enable thing
                    guielements[i].ActType.Enabled = true;

                    if (flaskActType == ActivationType.HP || flaskActType == ActivationType.MP || flaskActType == ActivationType.ES)
                    {
                        guielements[i].PercentRadioButton.Enabled = true;
                        guielements[i].FlatRadioButton.Enabled = true;
                        guielements[i].PercentValue.Enabled = baseAction.UseActPercent;
                        guielements[i].FlatValue.Enabled = !baseAction.UseActPercent;
                    }
                    else
                    {
                        guielements[i].PercentRadioButton.Enabled = false;
                        guielements[i].FlatRadioButton.Enabled = false;
                        guielements[i].PercentValue.Enabled = false;
                        guielements[i].FlatValue.Enabled = false;
                    }

                    guielements[i].PauseEnable.Enabled = true;
                    guielements[i].SecondKey.Enabled = guielements[i].PauseEnable.Checked;
                    guielements[i].PauseSecValue.Enabled = guielements[i].PauseEnable.Checked;

                    guielements[i].FlaskMinimumKD.Enabled = true;

                }
            }
        }

        private static void SubscribeEvents(ref FlaskGUIElements[] guielements)
        {
            for (int i = 0; i < guielements.Length; i++)
            {
                guielements[i].ActType.SelectedValueChanged += OnActTypeChange;
                guielements[i].FlaskGroupBox.SelectedIndexChanged += OnGroupChange;
                guielements[i].PercentRadioButton.CheckedChanged += OnActPercentOrFlatChange;
                guielements[i].PauseEnable.CheckedChanged += OnPauseEnableChanged;

                guielements[i].PercentValue.ValueChanged += OnPercentValueChange;
                guielements[i].FlatValue.ValueChanged += OnFlatValueChange;

                guielements[i].SecondKey.SelectedIndexChanged += OnSecondKeyChanged;
                guielements[i].PauseSecValue.ValueChanged += OnPauseDelayChanged;

                guielements[i].FlaskMinimumKD.ValueChanged += OnMinCDChanged;
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

            SetPanelColor(ref Guielements);
            SetEnableFlags(ref Guielements);
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

            SetPanelColor(ref Guielements);
            SetEnableFlags(ref Guielements);
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

            SetEnableFlags(ref Guielements);
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

            SetEnableFlags(ref Guielements);
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
