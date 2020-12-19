namespace KeyNetClient
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.sendTextBox = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.sendButton = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.settingsBox = new System.Windows.Forms.GroupBox();
            this.frogButton = new System.Windows.Forms.RadioButton();
            this.dsaButton = new System.Windows.Forms.RadioButton();
            this.aesButton = new System.Windows.Forms.RadioButton();
            this.keyBox = new System.Windows.Forms.TextBox();
            this.keyButton = new System.Windows.Forms.Button();
            this.chatButton = new System.Windows.Forms.Button();
            this.keyLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.usersBox = new System.Windows.Forms.ComboBox();
            this.groupBox.SuspendLayout();
            this.settingsBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // sendTextBox
            // 
            this.sendTextBox.Location = new System.Drawing.Point(8, 385);
            this.sendTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.sendTextBox.Multiline = true;
            this.sendTextBox.Name = "sendTextBox";
            this.sendTextBox.Size = new System.Drawing.Size(551, 61);
            this.sendTextBox.TabIndex = 1;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(27, 36);
            this.label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(73, 17);
            this.label.TabIndex = 3;
            this.label.Text = "Ваше имя";
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(581, 385);
            this.sendButton.Margin = new System.Windows.Forms.Padding(4);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(145, 62);
            this.sendButton.TabIndex = 4;
            this.sendButton.Text = "Отправить сообщение";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.sendButton);
            this.groupBox.Controls.Add(this.sendTextBox);
            this.groupBox.Location = new System.Drawing.Point(313, 25);
            this.groupBox.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox.Size = new System.Drawing.Size(737, 473);
            this.groupBox.TabIndex = 5;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Сообщения";
            // 
            // settingsBox
            // 
            this.settingsBox.Controls.Add(this.frogButton);
            this.settingsBox.Controls.Add(this.dsaButton);
            this.settingsBox.Controls.Add(this.aesButton);
            this.settingsBox.Controls.Add(this.keyBox);
            this.settingsBox.Controls.Add(this.keyButton);
            this.settingsBox.Controls.Add(this.chatButton);
            this.settingsBox.Controls.Add(this.keyLabel);
            this.settingsBox.Controls.Add(this.label1);
            this.settingsBox.Controls.Add(this.label2);
            this.settingsBox.Controls.Add(this.nameLabel);
            this.settingsBox.Controls.Add(this.label);
            this.settingsBox.Controls.Add(this.usersBox);
            this.settingsBox.Location = new System.Drawing.Point(16, 25);
            this.settingsBox.Margin = new System.Windows.Forms.Padding(4);
            this.settingsBox.Name = "settingsBox";
            this.settingsBox.Padding = new System.Windows.Forms.Padding(4);
            this.settingsBox.Size = new System.Drawing.Size(289, 473);
            this.settingsBox.TabIndex = 6;
            this.settingsBox.TabStop = false;
            this.settingsBox.Text = "Настройки";
            // 
            // frogButton
            // 
            this.frogButton.AutoSize = true;
            this.frogButton.Location = new System.Drawing.Point(115, 431);
            this.frogButton.Name = "frogButton";
            this.frogButton.Size = new System.Drawing.Size(54, 21);
            this.frogButton.TabIndex = 5;
            this.frogButton.TabStop = true;
            this.frogButton.Text = "frog";
            this.frogButton.UseVisualStyleBackColor = true;
            this.frogButton.CheckedChanged += new System.EventHandler(this.frogButton_CheckedChanged);
            // 
            // dsaButton
            // 
            this.dsaButton.AutoSize = true;
            this.dsaButton.Location = new System.Drawing.Point(175, 431);
            this.dsaButton.Name = "dsaButton";
            this.dsaButton.Size = new System.Drawing.Size(52, 21);
            this.dsaButton.TabIndex = 13;
            this.dsaButton.TabStop = true;
            this.dsaButton.Text = "dsa";
            this.dsaButton.UseVisualStyleBackColor = true;
            this.dsaButton.CheckedChanged += new System.EventHandler(this.dsaButton_CheckedChanged);
            // 
            // aesButton
            // 
            this.aesButton.AutoSize = true;
            this.aesButton.Location = new System.Drawing.Point(57, 431);
            this.aesButton.Name = "aesButton";
            this.aesButton.Size = new System.Drawing.Size(52, 21);
            this.aesButton.TabIndex = 12;
            this.aesButton.TabStop = true;
            this.aesButton.Text = "aes";
            this.aesButton.UseVisualStyleBackColor = true;
            this.aesButton.CheckedChanged += new System.EventHandler(this.aesButton_CheckedChanged);
            // 
            // keyBox
            // 
            this.keyBox.Location = new System.Drawing.Point(31, 239);
            this.keyBox.Margin = new System.Windows.Forms.Padding(4);
            this.keyBox.Multiline = true;
            this.keyBox.Name = "keyBox";
            this.keyBox.ReadOnly = true;
            this.keyBox.Size = new System.Drawing.Size(231, 185);
            this.keyBox.TabIndex = 11;
            this.keyBox.TabStop = false;
            // 
            // keyButton
            // 
            this.keyButton.Location = new System.Drawing.Point(115, 193);
            this.keyButton.Margin = new System.Windows.Forms.Padding(4);
            this.keyButton.Name = "keyButton";
            this.keyButton.Size = new System.Drawing.Size(148, 28);
            this.keyButton.TabIndex = 10;
            this.keyButton.Text = "Создать ключ";
            this.keyButton.UseVisualStyleBackColor = true;
            this.keyButton.Click += new System.EventHandler(this.keyButton_Click);
            // 
            // chatButton
            // 
            this.chatButton.Location = new System.Drawing.Point(172, 132);
            this.chatButton.Margin = new System.Windows.Forms.Padding(4);
            this.chatButton.Name = "chatButton";
            this.chatButton.Size = new System.Drawing.Size(100, 28);
            this.chatButton.TabIndex = 9;
            this.chatButton.Text = "Чат";
            this.chatButton.UseVisualStyleBackColor = true;
            this.chatButton.Click += new System.EventHandler(this.chatButton_Click);
            // 
            // keyLabel
            // 
            this.keyLabel.AutoSize = true;
            this.keyLabel.Location = new System.Drawing.Point(31, 239);
            this.keyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.keyLabel.Name = "keyLabel";
            this.keyLabel.Size = new System.Drawing.Size(0, 17);
            this.keyLabel.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 199);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Ваш ключ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 112);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Кому пишем?";
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(27, 52);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(181, 17);
            this.nameLabel.TabIndex = 4;
            this.nameLabel.Text = "Ну тут типа имя написано";
            // 
            // usersBox
            // 
            this.usersBox.FormattingEnabled = true;
            this.usersBox.Location = new System.Drawing.Point(31, 132);
            this.usersBox.Margin = new System.Windows.Forms.Padding(4);
            this.usersBox.Name = "usersBox";
            this.usersBox.Size = new System.Drawing.Size(132, 24);
            this.usersBox.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AcceptButton = this.sendButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 512);
            this.Controls.Add(this.settingsBox);
            this.Controls.Add(this.groupBox);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.settingsBox.ResumeLayout(false);
            this.settingsBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox sendTextBox;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.GroupBox settingsBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label keyLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button chatButton;
        private System.Windows.Forms.ComboBox usersBox;
        private System.Windows.Forms.Button keyButton;
        private System.Windows.Forms.TextBox keyBox;
        private System.Windows.Forms.RadioButton frogButton;
        private System.Windows.Forms.RadioButton dsaButton;
        private System.Windows.Forms.RadioButton aesButton;
    }
}

