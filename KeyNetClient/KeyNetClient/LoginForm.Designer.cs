namespace KeyNetClient
{
    partial class LoginForm
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
            this.loginButton = new System.Windows.Forms.Button();
            this.label = new System.Windows.Forms.Label();
            this.loginBox = new System.Windows.Forms.TextBox();
            this.serverBox = new System.Windows.Forms.ComboBox();
            this.serverLabel = new System.Windows.Forms.Label();
            this.errLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(114, 73);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(115, 23);
            this.loginButton.TabIndex = 0;
            this.loginButton.Text = "Подключиться";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(44, 31);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(101, 13);
            this.label.TabIndex = 1;
            this.label.Text = "Введите ваше имя";
            // 
            // loginBox
            // 
            this.loginBox.Location = new System.Drawing.Point(47, 47);
            this.loginBox.Name = "loginBox";
            this.loginBox.Size = new System.Drawing.Size(242, 20);
            this.loginBox.TabIndex = 2;
            // 
            // serverBox
            // 
            this.serverBox.FormattingEnabled = true;
            this.serverBox.Location = new System.Drawing.Point(47, 181);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(242, 21);
            this.serverBox.TabIndex = 3;
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(44, 165);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(135, 13);
            this.serverLabel.TabIndex = 4;
            this.serverLabel.Text = "Сервер для подключения";
            // 
            // errLabel
            // 
            this.errLabel.Location = new System.Drawing.Point(44, 99);
            this.errLabel.Name = "errLabel";
            this.errLabel.Size = new System.Drawing.Size(245, 66);
            this.errLabel.TabIndex = 5;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.loginButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 214);
            this.Controls.Add(this.errLabel);
            this.Controls.Add(this.serverLabel);
            this.Controls.Add(this.serverBox);
            this.Controls.Add(this.loginBox);
            this.Controls.Add(this.label);
            this.Controls.Add(this.loginButton);
            this.Name = "LoginForm";
            this.Text = "LogIn";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.TextBox loginBox;
        private System.Windows.Forms.ComboBox serverBox;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.Label errLabel;
    }
}