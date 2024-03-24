namespace Drinker
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            StartStop_button = new Button();
            FlasksSettings_button = new Button();
            PauseEnable_chekbox = new CheckBox();
            GamePath_label = new Label();
            UPS_lable = new Label();
            MP_lable = new Label();
            HP_lable = new Label();
            ES_lable = new Label();
            Profile_dropBox = new ComboBox();
            FLasksState_lable = new Label();
            SuspendLayout();
            // 
            // StartStop_button
            // 
            StartStop_button.BackColor = Color.IndianRed;
            StartStop_button.FlatStyle = FlatStyle.Flat;
            StartStop_button.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            StartStop_button.Location = new Point(12, 41);
            StartStop_button.Name = "StartStop_button";
            StartStop_button.Size = new Size(138, 75);
            StartStop_button.TabIndex = 0;
            StartStop_button.Text = "StartStop";
            StartStop_button.UseVisualStyleBackColor = false;
            StartStop_button.Click += StartStop_button_Click;
            // 
            // FlasksSettings_button
            // 
            FlasksSettings_button.BackColor = Color.DeepSkyBlue;
            FlasksSettings_button.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            FlasksSettings_button.Location = new Point(157, 41);
            FlasksSettings_button.Name = "FlasksSettings_button";
            FlasksSettings_button.Size = new Size(82, 75);
            FlasksSettings_button.TabIndex = 1;
            FlasksSettings_button.Text = "Flask settings";
            FlasksSettings_button.UseVisualStyleBackColor = false;
            FlasksSettings_button.Click += FlasksSettings_button_Click;
            // 
            // PauseEnable_chekbox
            // 
            PauseEnable_chekbox.AutoSize = true;
            PauseEnable_chekbox.Location = new Point(13, 144);
            PauseEnable_chekbox.Name = "PauseEnable_chekbox";
            PauseEnable_chekbox.RightToLeft = RightToLeft.Yes;
            PauseEnable_chekbox.Size = new Size(120, 19);
            PauseEnable_chekbox.TabIndex = 2;
            PauseEnable_chekbox.Text = "Auto pause in HO";
            PauseEnable_chekbox.UseVisualStyleBackColor = true;
            // 
            // GamePath_label
            // 
            GamePath_label.AutoSize = true;
            GamePath_label.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Underline);
            GamePath_label.ForeColor = Color.RoyalBlue;
            GamePath_label.Location = new Point(13, 130);
            GamePath_label.Name = "GamePath_label";
            GamePath_label.Size = new Size(105, 13);
            GamePath_label.TabIndex = 3;
            GamePath_label.Text = "Set path to PoE log..";
            GamePath_label.TextChanged += GamePath_label_TextChanged;
            GamePath_label.Click += GamePath_label_Click;
            // 
            // UPS_lable
            // 
            UPS_lable.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            UPS_lable.AutoSize = true;
            UPS_lable.BackColor = Color.Silver;
            UPS_lable.Font = new Font("Microsoft Sans Serif", 8.25F);
            UPS_lable.Location = new Point(185, 176);
            UPS_lable.MinimumSize = new Size(53, 13);
            UPS_lable.Name = "UPS_lable";
            UPS_lable.Size = new Size(53, 13);
            UPS_lable.TabIndex = 4;
            UPS_lable.Text = "UPS: 999";
            // 
            // MP_lable
            // 
            MP_lable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MP_lable.AutoSize = true;
            MP_lable.BackColor = Color.Silver;
            MP_lable.Font = new Font("Microsoft Sans Serif", 8.25F);
            MP_lable.Location = new Point(157, 137);
            MP_lable.MinimumSize = new Size(81, 13);
            MP_lable.Name = "MP_lable";
            MP_lable.Size = new Size(82, 13);
            MP_lable.TabIndex = 5;
            MP_lable.Text = "MP: 9999/9999";
            // 
            // HP_lable
            // 
            HP_lable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            HP_lable.AutoSize = true;
            HP_lable.BackColor = Color.Silver;
            HP_lable.Font = new Font("Microsoft Sans Serif", 8.25F);
            HP_lable.Location = new Point(157, 119);
            HP_lable.MinimumSize = new Size(81, 13);
            HP_lable.Name = "HP_lable";
            HP_lable.Size = new Size(81, 13);
            HP_lable.TabIndex = 6;
            HP_lable.Text = "HP: 9999/9999";
            // 
            // ES_lable
            // 
            ES_lable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ES_lable.AutoSize = true;
            ES_lable.BackColor = Color.Silver;
            ES_lable.Font = new Font("Microsoft Sans Serif", 8.25F);
            ES_lable.Location = new Point(157, 157);
            ES_lable.MinimumSize = new Size(81, 13);
            ES_lable.Name = "ES_lable";
            ES_lable.Size = new Size(81, 13);
            ES_lable.TabIndex = 7;
            ES_lable.Text = "ES: 9999/9999";
            // 
            // Profile_dropBox
            // 
            Profile_dropBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Profile_dropBox.DropDownStyle = ComboBoxStyle.DropDownList;
            Profile_dropBox.Location = new Point(12, 7);
            Profile_dropBox.Name = "Profile_dropBox";
            Profile_dropBox.Size = new Size(225, 23);
            Profile_dropBox.TabIndex = 9;
            Profile_dropBox.SelectedIndexChanged += Profile_dropBox_SelectedIndexChanged;
            // 
            // FLasksState_lable
            // 
            FLasksState_lable.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FLasksState_lable.AutoSize = true;
            FLasksState_lable.BackColor = Color.Silver;
            FLasksState_lable.Font = new Font("Microsoft Sans Serif", 8.25F);
            FLasksState_lable.Location = new Point(12, 176);
            FLasksState_lable.MinimumSize = new Size(170, 13);
            FLasksState_lable.Name = "FLasksState_lable";
            FLasksState_lable.Size = new Size(170, 13);
            FLasksState_lable.TabIndex = 10;
            FLasksState_lable.Text = "Flasks states: N/A";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(248, 198);
            Controls.Add(FLasksState_lable);
            Controls.Add(Profile_dropBox);
            Controls.Add(ES_lable);
            Controls.Add(HP_lable);
            Controls.Add(MP_lable);
            Controls.Add(UPS_lable);
            Controls.Add(GamePath_label);
            Controls.Add(PauseEnable_chekbox);
            Controls.Add(FlasksSettings_button);
            Controls.Add(StartStop_button);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(264, 237);
            MinimumSize = new Size(264, 237);
            Name = "Form1";
            StartPosition = FormStartPosition.Manual;
            Text = "Drinker";
            FormClosing += Form1_FormClosing;
            ResizeEnd += Form1_ResizeEnd;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button StartStop_button;
        private Button FlasksSettings_button;
        private CheckBox PauseEnable_chekbox;
        private Label GamePath_label;
        private Label UPS_lable;
        private Label MP_lable;
        private Label HP_lable;
        private Label ES_lable;
        private ComboBox Profile_dropBox;
        private Label FLasksState_lable;
    }
}
