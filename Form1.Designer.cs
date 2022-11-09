namespace eSSP_example
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emptyPayoutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnHalt = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.logTickBox = new System.Windows.Forms.CheckBox();
            this.msLevel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAllToZeroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbPayout = new System.Windows.Forms.TextBox();
            this.btnPayout = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnResetPayout = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbChannelLevels = new System.Windows.Forms.TextBox();
            this.btnEmptyHopper = new System.Windows.Forms.Button();
            this.btnSmartEmptyHopper = new System.Windows.Forms.Button();
            this.tbCoinLevels = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnResetHopper = new System.Windows.Forms.Button();
            this.btnSMARTEmpty = new System.Windows.Forms.Button();
            this.btnEmptySMARTPayout = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.tbPayoutCurrency = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbMinPayout = new System.Windows.Forms.TextBox();
            this.tbFloatAmount = new System.Windows.Forms.TextBox();
            this.btnFloat = new System.Windows.Forms.Button();
            this.cbFloatSelect = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbFloatCurrency = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnPayoutByDenom = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.timerCoin = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.tutar = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.istenen = new System.Windows.Forms.Label();
            this.timeOutPay = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1.SuspendLayout();
            this.msLevel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(982, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.emptyPayoutToolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // emptyPayoutToolStripMenuItem1
            // 
            this.emptyPayoutToolStripMenuItem1.Name = "emptyPayoutToolStripMenuItem1";
            this.emptyPayoutToolStripMenuItem1.Size = new System.Drawing.Size(93, 22);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 400);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "&Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnHalt
            // 
            this.btnHalt.Location = new System.Drawing.Point(105, 400);
            this.btnHalt.Name = "btnHalt";
            this.btnHalt.Size = new System.Drawing.Size(75, 23);
            this.btnHalt.TabIndex = 2;
            this.btnHalt.Text = "&Halt";
            this.btnHalt.UseVisualStyleBackColor = true;
            this.btnHalt.Click += new System.EventHandler(this.btnHalt_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox1.Location = new System.Drawing.Point(12, 27);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(280, 367);
            this.textBox1.TabIndex = 4;
            // 
            // logTickBox
            // 
            this.logTickBox.AutoSize = true;
            this.logTickBox.Location = new System.Drawing.Point(212, 404);
            this.logTickBox.Name = "logTickBox";
            this.logTickBox.Size = new System.Drawing.Size(81, 17);
            this.logTickBox.TabIndex = 16;
            this.logTickBox.Text = "Comms Log";
            this.logTickBox.UseVisualStyleBackColor = true;
            this.logTickBox.CheckedChanged += new System.EventHandler(this.logTickBox_CheckedChanged);
            // 
            // msLevel
            // 
            this.msLevel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setLevelToolStripMenuItem,
            this.setAllToZeroToolStripMenuItem});
            this.msLevel.Name = "msLevel";
            this.msLevel.Size = new System.Drawing.Size(149, 48);
            // 
            // setLevelToolStripMenuItem
            // 
            this.setLevelToolStripMenuItem.Name = "setLevelToolStripMenuItem";
            this.setLevelToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.setLevelToolStripMenuItem.Text = "&Set Level";
            this.setLevelToolStripMenuItem.Click += new System.EventHandler(this.setLevelToolStripMenuItem_Click);
            // 
            // setAllToZeroToolStripMenuItem
            // 
            this.setAllToZeroToolStripMenuItem.Name = "setAllToZeroToolStripMenuItem";
            this.setAllToZeroToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.setAllToZeroToolStripMenuItem.Text = "Set All to Zero";
            this.setAllToZeroToolStripMenuItem.Click += new System.EventHandler(this.setAllToZeroToolStripMenuItem_Click);
            // 
            // tbPayout
            // 
            this.tbPayout.Location = new System.Drawing.Point(128, 16);
            this.tbPayout.Name = "tbPayout";
            this.tbPayout.Size = new System.Drawing.Size(38, 20);
            this.tbPayout.TabIndex = 17;
            // 
            // btnPayout
            // 
            this.btnPayout.Location = new System.Drawing.Point(26, 64);
            this.btnPayout.Name = "btnPayout";
            this.btnPayout.Size = new System.Drawing.Size(144, 23);
            this.btnPayout.TabIndex = 18;
            this.btnPayout.Text = "Payout";
            this.btnPayout.UseVisualStyleBackColor = true;
            this.btnPayout.Click += new System.EventHandler(this.btnPayout_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Amount to Payout:";
            // 
            // btnResetPayout
            // 
            this.btnResetPayout.Location = new System.Drawing.Point(627, 245);
            this.btnResetPayout.Name = "btnResetPayout";
            this.btnResetPayout.Size = new System.Drawing.Size(196, 23);
            this.btnResetPayout.TabIndex = 5;
            this.btnResetPayout.Text = "Reset SMART Payout";
            this.btnResetPayout.UseVisualStyleBackColor = true;
            this.btnResetPayout.Click += new System.EventHandler(this.btnResetPayout_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(625, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 39);
            this.label4.TabIndex = 1;
            this.label4.Text = "SMART Payout\r\nChannel\r\nLevels:";
            // 
            // tbChannelLevels
            // 
            this.tbChannelLevels.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbChannelLevels.Location = new System.Drawing.Point(628, 65);
            this.tbChannelLevels.Multiline = true;
            this.tbChannelLevels.Name = "tbChannelLevels";
            this.tbChannelLevels.ReadOnly = true;
            this.tbChannelLevels.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbChannelLevels.Size = new System.Drawing.Size(195, 116);
            this.tbChannelLevels.TabIndex = 0;
            // 
            // btnEmptyHopper
            // 
            this.btnEmptyHopper.Location = new System.Drawing.Point(297, 216);
            this.btnEmptyHopper.Name = "btnEmptyHopper";
            this.btnEmptyHopper.Size = new System.Drawing.Size(195, 23);
            this.btnEmptyHopper.TabIndex = 0;
            this.btnEmptyHopper.Text = "Empty Hopper";
            this.btnEmptyHopper.UseVisualStyleBackColor = true;
            this.btnEmptyHopper.Click += new System.EventHandler(this.btnEmptyHopper_Click);
            // 
            // btnSmartEmptyHopper
            // 
            this.btnSmartEmptyHopper.Location = new System.Drawing.Point(297, 187);
            this.btnSmartEmptyHopper.Name = "btnSmartEmptyHopper";
            this.btnSmartEmptyHopper.Size = new System.Drawing.Size(195, 23);
            this.btnSmartEmptyHopper.TabIndex = 1;
            this.btnSmartEmptyHopper.Text = "SMART Empty Hopper";
            this.btnSmartEmptyHopper.UseVisualStyleBackColor = true;
            this.btnSmartEmptyHopper.Click += new System.EventHandler(this.btnSmartEmptyHopper_Click);
            // 
            // tbCoinLevels
            // 
            this.tbCoinLevels.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbCoinLevels.ContextMenuStrip = this.msLevel;
            this.tbCoinLevels.Location = new System.Drawing.Point(297, 65);
            this.tbCoinLevels.Multiline = true;
            this.tbCoinLevels.Name = "tbCoinLevels";
            this.tbCoinLevels.ReadOnly = true;
            this.tbCoinLevels.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCoinLevels.Size = new System.Drawing.Size(195, 116);
            this.tbCoinLevels.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(294, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 39);
            this.label6.TabIndex = 3;
            this.label6.Text = "Hopper\r\nChannel\r\nLevels:";
            // 
            // btnResetHopper
            // 
            this.btnResetHopper.Location = new System.Drawing.Point(296, 245);
            this.btnResetHopper.Name = "btnResetHopper";
            this.btnResetHopper.Size = new System.Drawing.Size(196, 23);
            this.btnResetHopper.TabIndex = 4;
            this.btnResetHopper.Text = "Reset Hopper";
            this.btnResetHopper.UseVisualStyleBackColor = true;
            this.btnResetHopper.Click += new System.EventHandler(this.btnResetHopper_Click);
            // 
            // btnSMARTEmpty
            // 
            this.btnSMARTEmpty.Location = new System.Drawing.Point(628, 187);
            this.btnSMARTEmpty.Name = "btnSMARTEmpty";
            this.btnSMARTEmpty.Size = new System.Drawing.Size(195, 23);
            this.btnSMARTEmpty.TabIndex = 25;
            this.btnSMARTEmpty.Text = "SMART Empty Payout";
            this.btnSMARTEmpty.UseVisualStyleBackColor = true;
            this.btnSMARTEmpty.Click += new System.EventHandler(this.btnSMARTEmpty_Click);
            // 
            // btnEmptySMARTPayout
            // 
            this.btnEmptySMARTPayout.Location = new System.Drawing.Point(628, 216);
            this.btnEmptySMARTPayout.Name = "btnEmptySMARTPayout";
            this.btnEmptySMARTPayout.Size = new System.Drawing.Size(195, 23);
            this.btnEmptySMARTPayout.TabIndex = 24;
            this.btnEmptySMARTPayout.Text = "Empty Payout";
            this.btnEmptySMARTPayout.UseVisualStyleBackColor = true;
            this.btnEmptySMARTPayout.Click += new System.EventHandler(this.btnEmptySMARTPayout_Click);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // tbPayoutCurrency
            // 
            this.tbPayoutCurrency.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbPayoutCurrency.Location = new System.Drawing.Point(128, 40);
            this.tbPayoutCurrency.Margin = new System.Windows.Forms.Padding(2);
            this.tbPayoutCurrency.Name = "tbPayoutCurrency";
            this.tbPayoutCurrency.Size = new System.Drawing.Size(38, 20);
            this.tbPayoutCurrency.TabIndex = 27;
            this.tbPayoutCurrency.Text = "TRY";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 28;
            this.label5.Text = "Currency of Payout:";
            // 
            // tbMinPayout
            // 
            this.tbMinPayout.Location = new System.Drawing.Point(124, 22);
            this.tbMinPayout.Name = "tbMinPayout";
            this.tbMinPayout.Size = new System.Drawing.Size(48, 20);
            this.tbMinPayout.TabIndex = 29;
            // 
            // tbFloatAmount
            // 
            this.tbFloatAmount.Location = new System.Drawing.Point(124, 46);
            this.tbFloatAmount.Name = "tbFloatAmount";
            this.tbFloatAmount.Size = new System.Drawing.Size(48, 20);
            this.tbFloatAmount.TabIndex = 30;
            // 
            // btnFloat
            // 
            this.btnFloat.Location = new System.Drawing.Point(22, 118);
            this.btnFloat.Name = "btnFloat";
            this.btnFloat.Size = new System.Drawing.Size(149, 23);
            this.btnFloat.TabIndex = 31;
            this.btnFloat.Text = "Float";
            this.btnFloat.UseVisualStyleBackColor = true;
            this.btnFloat.Click += new System.EventHandler(this.btnFloat_Click);
            // 
            // cbFloatSelect
            // 
            this.cbFloatSelect.FormattingEnabled = true;
            this.cbFloatSelect.Items.AddRange(new object[] {
            "SMART Hopper",
            "SMART Payout"});
            this.cbFloatSelect.Location = new System.Drawing.Point(22, 93);
            this.cbFloatSelect.Margin = new System.Windows.Forms.Padding(2);
            this.cbFloatSelect.Name = "cbFloatSelect";
            this.cbFloatSelect.Size = new System.Drawing.Size(150, 21);
            this.cbFloatSelect.TabIndex = 32;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 24);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 33;
            this.label7.Text = "Minimum Payout:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(36, 48);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 13);
            this.label8.TabIndex = 34;
            this.label8.Text = "Amount to Float:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbFloatCurrency);
            this.groupBox1.Controls.Add(this.tbFloatAmount);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.btnFloat);
            this.groupBox1.Controls.Add(this.cbFloatSelect);
            this.groupBox1.Controls.Add(this.tbMinPayout);
            this.groupBox1.Location = new System.Drawing.Point(297, 274);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(195, 147);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Float Options";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 74);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Currency of Float:";
            // 
            // tbFloatCurrency
            // 
            this.tbFloatCurrency.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbFloatCurrency.Location = new System.Drawing.Point(124, 69);
            this.tbFloatCurrency.Name = "tbFloatCurrency";
            this.tbFloatCurrency.Size = new System.Drawing.Size(48, 20);
            this.tbFloatCurrency.TabIndex = 35;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnPayoutByDenom);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.tbPayout);
            this.groupBox2.Controls.Add(this.btnPayout);
            this.groupBox2.Controls.Add(this.tbPayoutCurrency);
            this.groupBox2.Location = new System.Drawing.Point(628, 285);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(195, 137);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Payout Options";
            // 
            // btnPayoutByDenom
            // 
            this.btnPayoutByDenom.Location = new System.Drawing.Point(26, 93);
            this.btnPayoutByDenom.Name = "btnPayoutByDenom";
            this.btnPayoutByDenom.Size = new System.Drawing.Size(144, 23);
            this.btnPayoutByDenom.TabIndex = 29;
            this.btnPayoutByDenom.Text = "Payout by Denomination";
            this.btnPayoutByDenom.UseVisualStyleBackColor = true;
            this.btnPayoutByDenom.Click += new System.EventHandler(this.btnPayoutByDenom_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(369, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 37;
            this.button1.Text = "Set";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timerCoin
            // 
            this.timerCoin.Tick += new System.EventHandler(this.timerCoin_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(212, 435);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 38;
            this.button2.Text = "logClear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tutar
            // 
            this.tutar.AutoSize = true;
            this.tutar.BackColor = System.Drawing.Color.Orange;
            this.tutar.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tutar.Location = new System.Drawing.Point(134, 462);
            this.tutar.Name = "tutar";
            this.tutar.Size = new System.Drawing.Size(35, 22);
            this.tutar.TabIndex = 39;
            this.tutar.Text = "0 ₺";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ForeColor = System.Drawing.Color.DarkRed;
            this.label3.Location = new System.Drawing.Point(9, 465);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 17);
            this.label3.TabIndex = 40;
            this.label3.Text = "Ödenen Tutar : ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label9.ForeColor = System.Drawing.Color.DarkRed;
            this.label9.Location = new System.Drawing.Point(11, 436);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(120, 17);
            this.label9.TabIndex = 41;
            this.label9.Text = "İstenen Tutar : ";
            // 
            // istenen
            // 
            this.istenen.AutoSize = true;
            this.istenen.BackColor = System.Drawing.Color.Lime;
            this.istenen.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.istenen.Location = new System.Drawing.Point(134, 434);
            this.istenen.Name = "istenen";
            this.istenen.Size = new System.Drawing.Size(35, 22);
            this.istenen.TabIndex = 42;
            this.istenen.Text = "0 ₺";
            // 
            // timeOutPay
            // 
            this.timeOutPay.Interval = 30000;
            this.timeOutPay.Tick += new System.EventHandler(this.timeOutPay_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 524);
            this.Controls.Add(this.istenen);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tutar);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnResetHopper);
            this.Controls.Add(this.btnSMARTEmpty);
            this.Controls.Add(this.btnEmptySMARTPayout);
            this.Controls.Add(this.tbCoinLevels);
            this.Controls.Add(this.btnResetPayout);
            this.Controls.Add(this.btnSmartEmptyHopper);
            this.Controls.Add(this.logTickBox);
            this.Controls.Add(this.btnEmptyHopper);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.tbChannelLevels);
            this.Controls.Add(this.btnHalt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.DoubleClick += new System.EventHandler(this.Form1_DoubleClick);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.msLevel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnHalt;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem emptyPayoutToolStripMenuItem1;
        private System.Windows.Forms.CheckBox logTickBox;
        private System.Windows.Forms.ContextMenuStrip msLevel;
        private System.Windows.Forms.ToolStripMenuItem setLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setAllToZeroToolStripMenuItem;
        private System.Windows.Forms.TextBox tbPayout;
        private System.Windows.Forms.Button btnPayout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbChannelLevels;
        private System.Windows.Forms.Button btnResetPayout;
        private System.Windows.Forms.Button btnResetHopper;
        private System.Windows.Forms.Button btnEmptyHopper;
        private System.Windows.Forms.Button btnSmartEmptyHopper;
        private System.Windows.Forms.TextBox tbCoinLevels;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSMARTEmpty;
        private System.Windows.Forms.Button btnEmptySMARTPayout;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.TextBox tbPayoutCurrency;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbMinPayout;
        private System.Windows.Forms.TextBox tbFloatAmount;
        private System.Windows.Forms.Button btnFloat;
        private System.Windows.Forms.ComboBox cbFloatSelect;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbFloatCurrency;
        private System.Windows.Forms.Button btnPayoutByDenom;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timerCoin;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label tutar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label istenen;
        private System.Windows.Forms.Timer timeOutPay;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}

