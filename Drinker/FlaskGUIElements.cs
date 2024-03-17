namespace DrinkerForm
{
    public partial class FlasksSettingsFrom : Form
    {
        public struct FlaskGUIElements
        {
            public Panel FlaskPanel;

            public ComboBox ActType;

            public RadioButton PercentRadioButton;
            public RadioButton FlatRadioButton;
            public NumericUpDown PercentValue;
            public NumericUpDown FlatValue;

            public CheckBox PauseEnable;
            public Label PauseEnableText;
            public ComboBox SecondKey;
            public NumericUpDown PauseSecValue;

            public ComboBox FlaskInGameHotkey;

            public NumericUpDown FlaskMinimumKD;

            public ComboBox FlaskGroupBox;
        }
    }
}
